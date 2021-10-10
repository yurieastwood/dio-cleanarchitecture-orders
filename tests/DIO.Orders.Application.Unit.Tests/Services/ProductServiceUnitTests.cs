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
    internal class ProductServiceUnitTests
    {
        private MockRepository _mockRepository;
        private Mock<IProductRepository> _mockProductRepository;

        private ProductService _service;

        [SetUp]
        public void SetUp()
        {
            _mockRepository = new MockRepository(MockBehavior.Default);
            _mockProductRepository = _mockRepository.Create<IProductRepository>();

            _service = new ProductService(_mockProductRepository.Object);
        }

        [TestCase(typeof(IProductService))]
        [TestCase(typeof(ServiceStorableBase<Product>))]
        public void ShouldImplementsOrInherit(Type type)
        {
            //Arrange
            //Act
            //Assert
            _service.Should().BeAssignableTo(type);
        }

        [TestCase(0)]
        [TestCase(1)]
        public void WhenTryAddWithAnyId_ShouldNeverCallAddOrUpdate_FromRepository_AndReturnsAnInvalidProduct(int identifier)
        {
            //Arrange            
            var product = new Product("Product", 10) { Id = identifier };

            //Act
            var id = (ResultCodeType)_service.Add(product);

            //Assert
            id.Should().Be(ResultCodeType.InvalidProduct);
            _mockProductRepository.Verify(repo => repo.AddOrUpdate(It.IsAny<Product>()), Times.Never);
            _mockRepository.VerifyAll();
        }

        [Test]
        public void WhenTryAddWithValidId_ShouldCallAddOrUpdate_FromRepository_AndReturnsAnInvalidProduct()
        {
            //Arrange            
            var product = new Product("Product", 10);

            _mockProductRepository.Setup(repo => repo.AddOrUpdate(It.Is<Product>(c => c.Name == product.Name))).Returns(1);

            //Act
            var id = _service.Add(product);

            //Assert
            id.Should().Be(1);
            _mockProductRepository.Verify(repo => repo.AddOrUpdate(It.IsAny<Product>()), Times.Once);
            _mockRepository.VerifyAll();
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void WhenTryUpdateWithIdLessOrEqualToZero_ShouldNeverCallAddOrUpdate_FromRepository_AndReturnsFalse(int identifier)
        {
            //Arrange            
            var product = new Product("Product", 10) { Id = identifier };

            //Act
            var id = _service.Update(product);

            //Assert
            id.Should().BeFalse();
            _mockProductRepository.Verify(repo => repo.AddOrUpdate(It.IsAny<Product>()), Times.Never);
            _mockRepository.VerifyAll();
        }

        [Test]
        public void WhenTryUpdateWithValidId_ShouldCallAddOrUpdate_FromRepository_AndReturnsTrue()
        {
            //Arrange            
            var product = new Product("Product", 10) { Id = 1 };

            _mockProductRepository.Setup(repo => repo.AddOrUpdate(It.Is<Product>(c => c.Name == product.Name))).Returns(1);

            //Act
            var id = _service.Update(product);

            //Assert
            id.Should().BeTrue();
            _mockProductRepository.Verify(repo => repo.AddOrUpdate(It.IsAny<Product>()), Times.Once);
            _mockRepository.VerifyAll();
        }
    }
}
