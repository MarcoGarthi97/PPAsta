using Microsoft.Extensions.DependencyInjection;
using PPAsta.Abstraction.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PPAsta.Services.Collection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSharedLibrary(this IServiceCollection services)
        {
            services.Scan(scan => scan
            .FromAssemblies(Assembly.GetExecutingAssembly())
            .AddClasses(classes => classes.AssignableTo<IForServiceCollectionExtension>())
            .AsSelf()
            .AsImplementedInterfaces()
            .WithTransientLifetime());

            return services;
        }
    }
}
