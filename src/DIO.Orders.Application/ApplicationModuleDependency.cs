using DIO.Orders.Application.Initializers;
using DIO.Orders.Application.Services;
using DIO.Orders.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DIO.Orders.Application
{
    /// <summary>
    /// Injects the application dependencies in the API scope
    /// </summary>
    public static class ApplicationModuleDependency
    {
        /// <summary>
        /// Export the application modules available
        /// </summary>
        /// <param name="services">An instance of <see cref="IServiceCollection"/></param>
        public static void AddApplicationModule(this IServiceCollection services)
        {
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IPromotionService, PromotionService>();
            services.AddScoped<IOrderService, OrderService>();

            services.AddTransient<DataBaseInitializer>();
        }
    }
}
