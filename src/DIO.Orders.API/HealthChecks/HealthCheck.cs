using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DIO.Orders.Domain.Services;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace DIO.Orders.API.HealthChecks
{
    /// <summary>
    /// Check the infrastructure
    /// </summary>
    public class HealthCheck : IHealthCheck
    {
        private readonly IProductService _productService;
        private readonly ICustomerService _customerService;
        private readonly IPromotionService _promotionService;
        private readonly IOrderService _orderService;

        /// <summary>
        /// Initialize an instance of <see cref="HealthCheck"/>.
        /// </summary>
        /// <param name="productService">An instance of <see cref="IProductService"/>.</param>
        /// <param name="customerService">An instance of <see cref="ICustomerService"/>.</param>
        /// <param name="promotionService">An instance of <see cref="IPromotionService"/>.</param>
        /// <param name="orderService">An instance of <see cref="IOrderService"/>.</param>
        public HealthCheck(IProductService productService, ICustomerService customerService, IPromotionService promotionService, IOrderService orderService) =>
            (_productService, _customerService, _promotionService, _orderService) = (productService, customerService, promotionService, orderService);

        /// <inheritdoc />
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            try
            {
                var servicesHealthy = true;
                servicesHealthy &= _productService != null;
                servicesHealthy &= _customerService != null;
                servicesHealthy &= _promotionService != null;
                servicesHealthy &= _orderService != null;

                if (!servicesHealthy)
                    return Task.FromResult(
                        new HealthCheckResult(HealthStatus.Unhealthy,
                            "An unhealthy result. One or more services were not loaded properly."));

                var infrastructureHealthy = true;
                infrastructureHealthy &= _productService.Get()?.Any() ?? false;
                infrastructureHealthy &= _customerService.Get()?.Any() ?? false;
                infrastructureHealthy &= _promotionService.Get()?.Any() ?? false;

                if (!infrastructureHealthy)
                {
                    return Task.FromResult(
                        new HealthCheckResult(HealthStatus.Degraded,
                            "A degrade result. The database was not initialized."));
                }

                return Task.FromResult(
                    HealthCheckResult.Healthy("A healthy result."));
            }
            catch(Exception ex)
            {
                return Task.FromResult(
                    new HealthCheckResult(context.Registration.FailureStatus,
                        "An unhealthy result. An exception was thrown when try to execute the health check", ex));
            }
        }
    }
}
