using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PPAsta.Repository.Services.FactorySQL
{
    public static class SqliteBulkExtensions
    {
        /// <summary>
        /// Bulk insert ottimizzato per SQLite usando transazioni e prepared statements
        /// </summary>
        public static async Task BulkInsertAsync<T>(this SqliteConnection connection, IEnumerable<T> entities, int batchSize = 1000)
        {
            if (entities == null || !entities.Any())
                return;

            var entityList = entities.ToList();
            var tableName = GetTableName<T>();
            var properties = GetWritablePropertiesForInsert<T>();

            // Costruisci la query INSERT
            var columnNames = string.Join(", ", properties.Select(p => $"[{GetColumnName(p)}]"));
            var paramNames = string.Join(", ", properties.Select(p => $"@{p.Name}"));
            var insertQuery = $"INSERT INTO [{tableName}] ({columnNames}) VALUES ({paramNames})";

            var connectionWasOpen = connection.State == ConnectionState.Open;
            if (!connectionWasOpen)
                await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();
            try
            {
                using var command = connection.CreateCommand();
                command.Transaction = transaction;
                command.CommandText = insertQuery;

                // Aggiungi i parametri una volta sola (prepared statement)
                foreach (var prop in properties)
                {
                    command.Parameters.Add(new SqliteParameter($"@{prop.Name}", GetSqliteType(prop.PropertyType)));
                }

                // Inserisci in batch per migliori performance
                var batches = entityList.Chunk(batchSize);
                foreach (var batch in batches)
                {
                    foreach (var entity in batch)
                    {
                        // Assegna i valori ai parametri
                        for (int i = 0; i < properties.Length; i++)
                        {
                            var value = properties[i].GetValue(entity) ?? DBNull.Value;
                            command.Parameters[i].Value = value;
                        }
                        await command.ExecuteNonQueryAsync();
                    }
                }

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
            finally
            {
                if (!connectionWasOpen)
                    connection.Close();
            }
        }

        /// <summary>
        /// Bulk update ottimizzato per SQLite
        /// </summary>
        public static async Task BulkUpdateAsync<T>(this SqliteConnection connection, IEnumerable<T> entities, int batchSize = 1000)
        {
            if (entities == null || !entities.Any())
                return;

            var entityList = entities.ToList();
            var tableName = GetTableName<T>();
            var properties = GetWritableProperties<T>();
            var keyProperty = GetKeyProperty<T>();

            if (keyProperty == null)
                throw new InvalidOperationException($"No key property found for type {typeof(T).Name}");

            // Costruisci la query UPDATE
            var setClause = string.Join(", ", properties.Where(p => p.Name != keyProperty.Name)
                .Select(p => $"[{GetColumnName(p)}] = @{p.Name}"));
            var updateQuery = $"UPDATE [{tableName}] SET {setClause} WHERE [{GetColumnName(keyProperty)}] = @{keyProperty.Name}";

            var connectionWasOpen = connection.State == ConnectionState.Open;
            if (!connectionWasOpen)
                await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();
            try
            {
                using var command = connection.CreateCommand();
                command.Transaction = transaction;
                command.CommandText = updateQuery;

                foreach (var prop in properties)
                {
                    command.Parameters.Add(new SqliteParameter($"@{prop.Name}", GetSqliteType(prop.PropertyType)));
                }

                var batches = entityList.Chunk(batchSize);
                foreach (var batch in batches)
                {
                    foreach (var entity in batch)
                    {
                        for (int i = 0; i < properties.Length; i++)
                        {
                            var value = properties[i].GetValue(entity) ?? DBNull.Value;
                            command.Parameters[i].Value = value;
                        }
                        await command.ExecuteNonQueryAsync();
                    }
                }

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
            finally
            {
                if (!connectionWasOpen)
                    connection.Close();
            }
        }

        /// <summary>
        /// Bulk delete ottimizzato per SQLite
        /// </summary>
        public static async Task BulkDeleteAsync<T>(this SqliteConnection connection, IEnumerable<T> entities, int batchSize = 1000)
        {
            if (entities == null || !entities.Any())
                return;

            var entityList = entities.ToList();
            var tableName = GetTableName<T>();
            var keyProperty = GetKeyProperty<T>();

            if (keyProperty == null)
                throw new InvalidOperationException($"No key property found for type {typeof(T).Name}");

            var deleteQuery = $"DELETE FROM [{tableName}] WHERE [{GetColumnName(keyProperty)}] = @Key";

            var connectionWasOpen = connection.State == ConnectionState.Open;
            if (!connectionWasOpen)
                await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();
            try
            {
                using var command = connection.CreateCommand();
                command.Transaction = transaction;
                command.CommandText = deleteQuery;
                command.Parameters.Add(new SqliteParameter("@Key", GetSqliteType(keyProperty.PropertyType)));

                var batches = entityList.Chunk(batchSize);
                foreach (var batch in batches)
                {
                    foreach (var entity in batch)
                    {
                        var keyValue = keyProperty.GetValue(entity) ?? DBNull.Value;
                        command.Parameters[0].Value = keyValue;
                        await command.ExecuteNonQueryAsync();
                    }
                }

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
            finally
            {
                if (!connectionWasOpen)
                    connection.Close();
            }
        }

        // Helper methods
        private static string GetTableName<T>()
        {
            var type = typeof(T);
            var tableAttr = type.GetCustomAttribute<System.ComponentModel.DataAnnotations.Schema.TableAttribute>();
            return tableAttr?.Name ?? type.Name;
        }

        /// <summary>
        /// Ottiene le proprietà scrivibili ESCLUSA la chiave auto-incrementante (per INSERT)
        /// </summary>
        private static PropertyInfo[] GetWritablePropertiesForInsert<T>()
        {
            return typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.CanWrite)
                .Where(p => !p.GetCustomAttributes<System.ComponentModel.DataAnnotations.Schema.NotMappedAttribute>().Any())
                .Where(p => !IsAutoGeneratedKey(p)) // Escludi chiavi auto-generate
                .ToArray();
        }

        /// <summary>
        /// Ottiene le proprietà scrivibili INCLUSA la chiave (per UPDATE/DELETE/SELECT)
        /// </summary>
        private static PropertyInfo[] GetWritableProperties<T>()
        {
            return typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.CanWrite)
                .Where(p => !p.GetCustomAttributes<System.ComponentModel.DataAnnotations.Schema.NotMappedAttribute>().Any())
                .ToArray();
        }

        /// <summary>
        /// Verifica se una proprietà è una chiave auto-generata
        /// </summary>
        private static bool IsAutoGeneratedKey(PropertyInfo property)
        {
            var hasKey = property.GetCustomAttribute<System.ComponentModel.DataAnnotations.KeyAttribute>() != null;

            if (!hasKey)
                return false;

            var dbGenAttr = property.GetCustomAttribute<System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedAttribute>();

            // Se ha [Key] ma NON ha [DatabaseGenerated], assumiamo sia auto-generata
            if (dbGenAttr == null)
                return true;

            // Se ha [DatabaseGenerated(None)], NON è auto-generata
            return dbGenAttr.DatabaseGeneratedOption != System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None;
        }

        private static PropertyInfo GetKeyProperty<T>()
        {
            var properties = typeof(T).GetProperties();

            // Cerca [Key] attribute
            var keyProp = properties.FirstOrDefault(p =>
                p.GetCustomAttribute<System.ComponentModel.DataAnnotations.KeyAttribute>() != null);

            // Se non trovato, cerca "Id" o "NomeClasseId"
            if (keyProp == null)
            {
                var typeName = typeof(T).Name;
                keyProp = properties.FirstOrDefault(p =>
                    p.Name.Equals("Id", StringComparison.OrdinalIgnoreCase) ||
                    p.Name.Equals($"{typeName}Id", StringComparison.OrdinalIgnoreCase));
            }

            return keyProp;
        }

        private static string GetColumnName(PropertyInfo property)
        {
            var columnAttr = property.GetCustomAttribute<System.ComponentModel.DataAnnotations.Schema.ColumnAttribute>();
            return columnAttr?.Name ?? property.Name;
        }

        private static SqliteType GetSqliteType(Type type)
        {
            type = Nullable.GetUnderlyingType(type) ?? type;

            if (type == typeof(int) || type == typeof(long) || type == typeof(short) ||
                type == typeof(byte) || type == typeof(bool))
                return SqliteType.Integer;

            if (type == typeof(double) || type == typeof(float) || type == typeof(decimal))
                return SqliteType.Real;

            if (type == typeof(string))
                return SqliteType.Text;

            if (type == typeof(byte[]))
                return SqliteType.Blob;

            return SqliteType.Text;
        }
    }

    // Extension for IEnumerable<T>.Chunk if not available in your .NET version
    public static class EnumerableExtensions
    {
        public static IEnumerable<T[]> Chunk<T>(this IEnumerable<T> source, int size)
        {
            var array = source.ToArray();
            for (int i = 0; i < array.Length; i += size)
            {
                yield return array.Skip(i).Take(size).ToArray();
            }
        }
    }
}
