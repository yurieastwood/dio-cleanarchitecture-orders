using DIO.Orders.Domain.Repositories;
using DIO.Orders.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace DIO.Orders.Infrastructure.Unit.Tests
{
    [TestFixture]
    internal class DataBaseInMemoryModuleDependencyUnitTests
    {
        [Test]
        public void WhenAddDataBaseModule_ShouldAddAllRepositoriesInScopeOfService()
        {
            //Arrange
            var serviceCollection = new ServiceCollection();

            //Act
            serviceCollection.AddDataBaseInMemoryModule();
            var provider = serviceCollection.BuildServiceProvider();

            //Assert
            Assert.IsInstanceOf<ProductRepository>(provider.GetService<IProductRepository>());
            Assert.IsInstanceOf<CustomerRepository>(provider.GetService<ICustomerRepository>());
            Assert.IsInstanceOf<PromotionRepository>(provider.GetService<IPromotionRepository>());
            Assert.IsInstanceOf<OrderRepository>(provider.GetService<IOrderRepository>());
        }
    }
}