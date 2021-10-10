using System;
using System.Collections.Generic;
using DIO.Orders.API.HealthChecks;
using DIO.Orders.Domain.Enums;
using DIO.Orders.Domain.Models;
using DIO.Orders.Domain.Services;
using FluentAssertions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Moq;
using NUnit.Framework;

namespace DIO.Orders.API.Unit.Tests.HealthChecks
{
    [TestFixture]
    internal class HealthCheckUnitTests
    {
        private MockRepository _mockRepository;
        private Mock<IProductService> _mockProductService;
        private Mock<ICustomerService> _mockCustomerService;
        private Mock<IPromotionService> _mockPromotionService;
        private Mock<IOrderService> _mockOrderService;

        private HealthCheck _healthCheck;
        private HealthCheckContext _context;

        [SetUp]
        public void SetUp()
        {
            _mockRepository = new MockRepository(MockBehavior.Default);
            _mockProductService = _mockRepository.Create<IProductService>();
            _mockCustomerService = _mockRepository.Create<ICustomerService>();
            _mockPromotionService = _mockRepository.Create<IPromotionService>();
            _mockOrderService = _mockRepository.Create<IOrderService>();
        }

        [Test]
        public void WhenTheDataBaseWasInitialize_AndAllServicesLoaded_ShouldReturnHealthy()
        {
            //Arrange
            var products = new List<Product> {new ("Product", 10)};
            var customers = new List<Customer> {new ("Customer")};
            var promotions = new List<Promotion> {new (PromotionType.Order, null, 10, "Promotion")};

            _mockProductService.Setup(service => service.Get()).Returns(products);
            _mockCustomerService.Setup(service => service.Get()).Returns(customers);
            _mockPromotionService.Setup(service => service.Get()).Returns(promotions);

            _healthCheck = new HealthCheck(_mockProductService.Object, _mockCustomerService.Object, _mockPromotionService.Object, _mockOrderService.Object);
            _context = new HealthCheckContext
            {
                Registration = new HealthCheckRegistration("Health Check Unit Test", _healthCheck, HealthStatus.Unhealthy, null)
            };

            //Act
            var result = _healthCheck.CheckHealthAsync(_context).Result;

            //Assert
            result.Status.Should().Be(HealthStatus.Healthy);
        }

        [Test]
        public void WhenTheDataBaseWasNotInitialize_AndServicesLoaded_ShouldReturnDegrade()
        {
            //Arrange
            _mockProductService.Setup(service => service.Get()).Returns(new List<Product>());
            _mockCustomerService.Setup(service => service.Get()).Returns(new List<Customer>());
            _mockPromotionService.Setup(service => service.Get()).Returns(new List<Promotion>());

            _healthCheck = new HealthCheck(_mockProductService.Object, _mockCustomerService.Object, _mockPromotionService.Object, _mockOrderService.Object);
            _context = new HealthCheckContext
            {
                Registration = new HealthCheckRegistration("Health Check Unit Test", _healthCheck, HealthStatus.Unhealthy, null)
            };

            //Act
            var result = _healthCheck.CheckHealthAsync(_context).Result;

            //Assert
            result.Status.Should().Be(HealthStatus.Degraded);
            result.Exception.Should().BeNull();
        }

        [Test]
        public void WhenTheServicesNotLoaded_ShouldReturnUnhealthy()
        {
            //Arrange
            _healthCheck = new HealthCheck(null, null, null, null);
            _context = new HealthCheckContext
            {
                Registration = new HealthCheckRegistration("Health Check Unit Test", _healthCheck, HealthStatus.Unhealthy, null)
            };

            //Act
            var result = _healthCheck.CheckHealthAsync(_context).Result;

            //Assert
            result.Status.Should().Be(HealthStatus.Unhealthy);
            result.Exception.Should().BeNull();
        }

        [Test]
        public void WhenFailedToCheckHealthStatus_ShouldReturnUnhealthy()
        {
            //Arrange
            var exception = new Exception("Generic Failure");
            _mockProductService.Setup(service => service.Get()).Throws(exception);

            _healthCheck = new HealthCheck(_mockProductService.Object, _mockCustomerService.Object, _mockPromotionService.Object, _mockOrderService.Object);
            _context = new HealthCheckContext
            {
                Registration = new HealthCheckRegistration("Health Check Unit Test", _healthCheck, HealthStatus.Unhealthy, null)
            };

            //Act
            var result = _healthCheck.CheckHealthAsync(_context).Result;

            //Assert
            result.Status.Should().Be(HealthStatus.Unhealthy);
            result.Exception?.Should().BeAssignableTo(exception.GetType());
            result.Exception?.Message.Should().Be(exception.Message);
        }
    }
}
