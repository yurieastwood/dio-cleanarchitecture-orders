using DIO.Orders.Application.Services;
using DIO.Orders.Domain.Repositories;
using DIO.Orders.Domain.Services;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace DIO.Orders.Application.Unit.Tests
{
    [TestFixture]
    internal class ApplicationModuleDependencyUnitTests
    {
        [Test]
        public void WhenAddDataBaseModule_ShouldAddAllRepositoriesInScopeOfService()
        {
            //Arrange
            var serviceCollection = new ServiceCollection();

            //Act
            serviceCollection.AddApplicationModule();
            serviceCollection.AddScoped(_ => default(IProductRepository));
            serviceCollection.AddScoped(_ => default(ICustomerRepository));
            serviceCollection.AddScoped(_ => default(IPromotionRepository));
            serviceCollection.AddScoped(_ => default(IOrderRepository));

            var provider = serviceCollection.BuildServiceProvider();

            //Assert
            Assert.IsInstanceOf<ProductService>(provider.GetService<IProductService>());
            Assert.IsInstanceOf<CustomerService>(provider.GetService<ICustomerService>());
            Assert.IsInstanceOf<PromotionService>(provider.GetService<IPromotionService>());
            Assert.IsInstanceOf<OrderService>(provider.GetService<IOrderService>());
        }
    }
}