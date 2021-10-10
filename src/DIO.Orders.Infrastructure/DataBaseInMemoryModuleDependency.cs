using DIO.Orders.Domain.Repositories;
using DIO.Orders.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace DIO.Orders.Infrastructure
{
    /// <summary>
    /// Injects the database dependencies in the API scope
    /// </summary>
    public static class DataBaseInMemoryModuleDependency
    {
        /// <summary>
        /// Export the database modules available
        /// </summary>
        /// <param name="services">An instance of <see cref="IServiceCollection"/></param>
        public static void AddDataBaseInMemoryModule(this IServiceCollection services)
        {
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IPromotionRepository, PromotionRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
        }
    }
}
