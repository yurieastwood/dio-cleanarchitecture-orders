using DIO.Orders.Domain.Models;
using DIO.Orders.Domain.Repositories;
using DIO.Orders.Infrastructure.Contexts;
using DIO.Orders.Infrastructure.Repositories;
using FluentAssertions;
using NUnit.Framework;
using System;

namespace DIO.Orders.Infrastructure.Unit.Tests.Repositories
{
    [TestFixture]
    internal class ProductRepositoryUnitTests
    {
        private readonly ProductRepository _repository = new ProductRepository();

        [TestCase(typeof(IProductRepository))]
        [TestCase(typeof(InMemoryContextRepositoryBase<Product>))]
        public void ShouldImplementsOrInherit(Type type)
        {
            //Arrange
            //Act
            //Assert
            _repository.Should().BeAssignableTo(type);
        }
    }
}
