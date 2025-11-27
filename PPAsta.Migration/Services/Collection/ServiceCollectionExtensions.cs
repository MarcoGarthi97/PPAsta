using Microsoft.Extensions.DependencyInjection;
using PPAsta.Abstraction.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PPAsta.Migration.Services.Collection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSharedLibraryMigrations(this IServiceCollection services)
        {
            services.Scan(scan => scan
            .FromAssemblies(Assembly.GetExecutingAssembly())
            .AddClasses()
            .AsImplementedInterfaces(typeInfo =>
                typeInfo.GetInterfaces().Any(i =>
                    i == typeof(IForServiceCollectionExtension) ||
                    typeInfo == typeof(IForServiceCollectionExtension))
            )
            .WithScopedLifetime()
            );

            return services;
        }
    }
}
