using Microsoft.Data.Sqlite;
using PPAsta.Abstraction.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPAsta.Repository.Services.FactorySQL
{
    public interface IDatabaseConnectionFactory : IForServiceCollectionExtension
    {
        Task<SqliteConnection> CreateConnectionAsync();
    }

    public class MdlSqliteConnectionFactory : IDatabaseConnectionFactory, IDisposable
    {
        private readonly string _connectionString;
        private SqliteConnection _connection;

        public MdlSqliteConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<SqliteConnection> CreateConnectionAsync()
        {
            if (_connection == null)
            {
                _connection = new SqliteConnection(_connectionString);
                await _connection.OpenAsync();
            }
            else if (_connection.State != System.Data.ConnectionState.Open)
            {
                await _connection.OpenAsync();
            }

            return _connection;
        }

        public void Dispose()
        {
            _connection.Dispose();
        }
    }
}
