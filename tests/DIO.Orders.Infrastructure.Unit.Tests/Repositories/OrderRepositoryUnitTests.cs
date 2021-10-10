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
    internal class OrderRepositoryUnitTests
    {
        private readonly OrderRepository _repository = new OrderRepository();

        [TestCase(typeof(IOrderRepository))]
        [TestCase(typeof(InMemoryContextRepositoryBase<Order>))]
        public void ShouldImplementsOrInherit(Type type)
        {
            //Arrange
            //Act
            //Assert
            _repository.Should().BeAssignableTo(type);
        }
    }
}
