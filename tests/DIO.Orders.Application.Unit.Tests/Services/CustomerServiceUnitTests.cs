using System;
using DIO.Orders.Application.Services;
using DIO.Orders.Domain.Enums;
using DIO.Orders.Domain.Models;
using DIO.Orders.Domain.Repositories;
using DIO.Orders.Domain.Services;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace DIO.Orders.Application.Unit.Tests.Services
{
    [TestFixture]
    internal class CustomerServiceUnitTests
    {
        private MockRepository _mockRepository;
        private Mock<ICustomerRepository> _mockCustomerRepository;

        private CustomerService _service;

        [SetUp]
        public void SetUp()
        {
            _mockRepository = new MockRepository(MockBehavior.Default);
            _mockCustomerRepository = _mockRepository.Create<ICustomerRepository>();

            _service = new CustomerService(_mockCustomerRepository.Object);
        }

        [TestCase(typeof(ICustomerService))]
        [TestCase(typeof(ServiceStorableBase<Customer>))]
        public void ShouldImplementsOrInherit(Type type)
        {
            //Arrange
            //Act
            //Assert
            _service.Should().BeAssignableTo(type);
        }

        [TestCase(0)]
        [TestCase(1)]
        public void WhenTryAddWithAnyId_ShouldNeverCallAddOrUpdate_FromRepository_AndReturnsAnInvalidCustomer(int identifier)
        {
            //Arrange            
            var customer = new Customer("Customer") { Id = identifier };

            //Act
            var id = (ResultCodeType)_service.Add(customer);

            //Assert
            id.Should().Be(ResultCodeType.InvalidCustomer);
            _mockCustomerRepository.Verify(repo => repo.AddOrUpdate(It.IsAny<Customer>()), Times.Never);
            _mockRepository.VerifyAll();
        }

        [Test]
        public void WhenTryAddWithValidId_ShouldCallAddOrUpdate_FromRepository_AndReturnsAnInvalidCustomer()
        {
            //Arrange            
            var customer = new Customer("Customer");

            _mockCustomerRepository.Setup(repo => repo.AddOrUpdate(It.Is<Customer>(c => c.Name == customer.Name))).Returns(1);

            //Act
            var id = _service.Add(customer);

            //Assert
            id.Should().Be(1);
            _mockCustomerRepository.Verify(repo => repo.AddOrUpdate(It.IsAny<Customer>()), Times.Once);
            _mockRepository.VerifyAll();
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void WhenTryUpdateWithIdLessOrEqualToZero_ShouldNeverCallAddOrUpdate_FromRepository_AndReturnsFalse(int identifier)
        {
            //Arrange            
            var customer = new Customer("Customer") { Id = identifier };

            //Act
            var id = _service.Update(customer);

            //Assert
            id.Should().BeFalse();
            _mockCustomerRepository.Verify(repo => repo.AddOrUpdate(It.IsAny<Customer>()), Times.Never);
            _mockRepository.VerifyAll();
        }

        [Test]
        public void WhenTryUpdateWithValidId_ShouldCallAddOrUpdate_FromRepository_AndReturnsTrue()
        {
            //Arrange            
            var customer = new Customer("Customer") { Id = 1 };

            _mockCustomerRepository.Setup(repo => repo.AddOrUpdate(It.Is<Customer>(c => c.Name == customer.Name))).Returns(1);

            //Act
            var id = _service.Update(customer);

            //Assert
            id.Should().BeTrue();
            _mockCustomerRepository.Verify(repo => repo.AddOrUpdate(It.IsAny<Customer>()), Times.Once);
            _mockRepository.VerifyAll();
        }
    }
}
