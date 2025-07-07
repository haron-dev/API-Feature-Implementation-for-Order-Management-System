using Microsoft.Extensions.DependencyInjection;
using System;

namespace OrderManagementSystem.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection Decorate<TService, TDecorator>(this IServiceCollection services)
            where TService : class
            where TDecorator : class, TService
        {
            // Get the existing service descriptor
            var descriptor = services.FindServiceDescriptor<TService>();
            if (descriptor == null)
            {
                throw new InvalidOperationException($"Service of type {typeof(TService).Name} not registered.");
            }

            // Remove the existing registration
            services.Remove(descriptor);

            // Register the decorator with the original service injected
            if (descriptor.ImplementationType != null)
            {
                services.Add(ServiceDescriptor.Describe(
                    typeof(TService),
                    sp =>
                    {
                        var original = ActivatorUtilities.CreateInstance(sp, descriptor.ImplementationType);
                        return ActivatorUtilities.CreateInstance<TDecorator>(sp, original);
                    },
                    descriptor.Lifetime));
            }
            else if (descriptor.ImplementationFactory != null)
            {
                services.Add(ServiceDescriptor.Describe(
                    typeof(TService),
                    sp =>
                    {
                        var original = descriptor.ImplementationFactory(sp);
                        return ActivatorUtilities.CreateInstance<TDecorator>(sp, original);
                    },
                    descriptor.Lifetime));
            }
            else
            {
                services.Add(ServiceDescriptor.Describe(
                    typeof(TService),
                    sp => ActivatorUtilities.CreateInstance<TDecorator>(sp, descriptor.ImplementationInstance),
                    descriptor.Lifetime));
            }

            return services;
        }

        private static ServiceDescriptor FindServiceDescriptor<T>(this IServiceCollection services)
        {
            return services.FirstOrDefault(d => d.ServiceType == typeof(T));
        }
    }
}
