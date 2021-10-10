using System;
using System.Collections.Generic;
using System.Linq;
using DIO.Orders.API.Controllers;
using DIO.Orders.Domain.Enums;
using DIO.Orders.Domain.Models;
using DIO.Orders.Domain.Responses;
using DIO.Orders.Domain.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using ControllerBase = DIO.Orders.API.Controllers.ControllerBase;

namespace DIO.Orders.API.Unit.Tests.Controllers
{
    [TestFixture]
    internal class ProductControllerUnitTests
    {
        private const int FirstId = 1;
        private readonly Exception _exception = new("Generic Exception");
        private readonly Product _product = new("Product", 10) { Id = FirstId };

        private ProductController _controller;

        private MockRepository _mockRepository;
        private Mock<IProductService> _mockService;


        [SetUp]
        public void SetUp()
        {
            _mockRepository = new MockRepository(MockBehavior.Default);
            _mockService = _mockRepository.Create<IProductService>();

            _controller = new ProductController(_mockService.Object);
        }

        [Test]
        public void ShouldInheritFromControllerBase()
        {
            //Arrange
            //Act
            //Assert
            _controller.Should().BeAssignableTo<ControllerBase>();
        }

        [TestCase(0, false, typeof(NoContentResult))]
        [TestCase(1, false, typeof(OkObjectResult))]
        [TestCase(1, true, typeof(BadRequestObjectResult))]
        public void WhenGet_ShouldReturnAsExpected(int quantityOfProducts, bool throwException, Type expectedTypeResult)
        {
            //Arrange
            var products = new List<Product>();

            Enumerable
                .Range(0, quantityOfProducts)
                .ToList()
                .ForEach(index => products.Add(new Product($"Product-{index}", index)));

            if (throwException)
                _mockService.Setup(_ => _.Get()).Throws(_exception);
            else
                _mockService.Setup(_ => _.Get()).Returns(products);

            //Act
            var result = _controller.Get();
            var value = (result as ObjectResult)?.Value;

            //Assert
            _mockService.Verify(_ => _.Get(), Times.Once);
            result.Should().BeAssignableTo(expectedTypeResult);

            if (throwException)
            {
                value.Should().NotBeNull();
                value.Should().BeOfType<ErrorResponse>();
                (value as ErrorResponse)?.Error.Should().BeAssignableTo(_exception.GetType());
            }
            else if (quantityOfProducts > 0)
            {
                value.Should().NotBeNull();
                value?.Should().BeOfType<List<Product>>();
                (value as List<Product>)?.Should().BeEquivalentTo(products);
            }
        }

        [TestCase(false, false, typeof(NotFoundResult))]
        [TestCase(true, false, typeof(OkObjectResult))]
        [TestCase(true, true, typeof(BadRequestObjectResult))]
        public void WhenGetById_ShouldReturnAsExpected(bool productExists, bool throwException, Type expectedTypeResult)
        {
            //Arrange
            var product = productExists ? _product : null;

            if (throwException)
                _mockService.Setup(_ => _.Get(FirstId)).Throws(_exception);
            else
                _mockService.Setup(_ => _.Get(FirstId)).Returns(product);

            //Act
            var result = _controller.Get(FirstId);
            var value = (result as ObjectResult)?.Value;

            //Assert
            _mockService.Verify(_ => _.Get(FirstId), Times.Once);
            result.Should().BeAssignableTo(expectedTypeResult);

            if (throwException)
            {
                value.Should().NotBeNull();
                value.Should().BeOfType<ErrorResponse>();
                (value as ErrorResponse)?.Error.Should().BeAssignableTo(_exception.GetType());
            }
            else if (productExists)
            {
                value.Should().NotBeNull();
                value?.Should().BeOfType<Product>();
                (value as Product)?.Id.Should().Be(product.Id);
                (value as Product)?.Name.Should().Be(product.Name);
            }
        }

        [TestCase(false, typeof(OkObjectResult))]
        [TestCase(true, typeof(BadRequestObjectResult))]
        public void WhenCallPrint_ShouldReturnAsExpected(bool throwException, Type expectedTypeResult)
        {
            //Arrange
            var expectedMessage = _product.ToString();

            if (throwException)
                _mockService.Setup(_ => _.Print(FirstId)).Throws(_exception);
            else
                _mockService.Setup(_ => _.Print(FirstId)).Returns(expectedMessage);

            //Act
            var result = _controller.Print(FirstId);
            var value = (result as ObjectResult)?.Value;

            //Assert
            _mockService.Verify(_ => _.Print(FirstId), Times.Once);
            result.Should().BeAssignableTo(expectedTypeResult);

            if (throwException)
            {
                value.Should().NotBeNull();
                value.Should().BeOfType<ErrorResponse>();
                (value as ErrorResponse)?.Error.Should().BeAssignableTo(_exception.GetType());
            }
            else
            {
                value.Should().NotBeNull();
                value?.Should().BeOfType<string>();
                value?.ToString().Should().Be(expectedMessage);
            }
        }

        [TestCase(true, false, typeof(OkObjectResult))]
        [TestCase(false, false, typeof(OkObjectResult))]
        [TestCase(true, true, typeof(BadRequestObjectResult))]
        public void WhenCallUpdate_ShouldReturnAsExpected(bool updateResult, bool throwException, Type expectedTypeResult)
        {
            //Arrange
            if (throwException)
                _mockService.Setup(_ => _.Update(It.IsAny<Product>())).Throws(_exception);
            else
                _mockService.Setup(_ => _.Update(It.Is<Product>(p => p.Name == _product.Name && p.Id == _product.Id))).Returns(updateResult);

            //Act
            var result = _controller.Update(_product);
            var value = (result as ObjectResult)?.Value;

            //Assert
            _mockService.Verify(_ => _.Update(It.IsAny<Product>()), Times.Once);
            result.Should().BeAssignableTo(expectedTypeResult);

            if (throwException)
            {
                value.Should().NotBeNull();
                value.Should().BeOfType<ErrorResponse>();
                (value as ErrorResponse)?.Error.Should().BeAssignableTo(_exception.GetType());
            }
            else
            {
                value.Should().NotBeNull();
                value?.Should().BeOfType<bool>();
                (value as bool?)?.Should().Be(updateResult);
            }
        }

        [TestCase(true, false, typeof(OkObjectResult))]
        [TestCase(false, false, typeof(OkObjectResult))]
        [TestCase(true, true, typeof(BadRequestObjectResult))]
        public void WhenCallDelete_ShouldReturnAsExpected(bool deleteResult, bool throwException, Type expectedTypeResult)
        {
            //Arrange
            if (throwException)
                _mockService.Setup(_ => _.Delete(It.IsAny<int>())).Throws(_exception);
            else
                _mockService.Setup(_ => _.Delete(FirstId)).Returns(deleteResult);

            //Act
            var result = _controller.Delete(FirstId);
            var value = (result as ObjectResult)?.Value;

            //Assert
            _mockService.Verify(_ => _.Delete(It.IsAny<int>()), Times.Once);
            result.Should().BeAssignableTo(expectedTypeResult);

            if (throwException)
            {
                value.Should().NotBeNull();
                value.Should().BeOfType<ErrorResponse>();
                (value as ErrorResponse)?.Error.Should().BeAssignableTo(_exception.GetType());
            }
            else
            {
                value.Should().NotBeNull();
                value?.Should().BeOfType<bool>();
                (value as bool?)?.Should().Be(deleteResult);
            }
        }

        [TestCase(1, null, false, typeof(CreatedResult))]
        [TestCase(-1, ResultCodeType.InvalidProduct, false, typeof(BadRequestObjectResult))]
        [TestCase(-100, ResultCodeType.NotCreated, false, typeof(BadRequestObjectResult))]
        [TestCase(1, null, true, typeof(BadRequestObjectResult))]
        public void WhenCallAdd_ShouldReturnAsExpected(int idReturned, ResultCodeType? resultType, bool throwException, Type expectedTypeResult)
        {
            //Arrange
            var product = new Product(_product.Name, _product.Value);
            if (throwException)
                _mockService.Setup(_ => _.Add(It.IsAny<Product>())).Throws(_exception);
            else
                _mockService.Setup(_ => _.Add(It.Is<Product>(p => p.Id == null && p.Name == product.Name))).Returns(idReturned);

            //Act
            var result = _controller.Add(product);
            var value = (result as ObjectResult)?.Value;

            //Assert
            _mockService.Verify(_ => _.Add(It.IsAny<Product>()), Times.Once);
            result.Should().BeAssignableTo(expectedTypeResult);

            if (throwException)
            {
                value.Should().NotBeNull();
                value.Should().BeOfType<ErrorResponse>();
                (value as ErrorResponse)?.Error.Should().BeAssignableTo(_exception.GetType());
            }
            else if (idReturned > 0)
            {
                value.Should().NotBeNull();
                value?.Should().BeOfType<int>();
                (value as int?)?.Should().Be(idReturned);
            }
            else
            {
                value.Should().NotBeNull();
                value.Should().BeOfType<ErrorResponse>();
                (value as ErrorResponse)?.Error.Should().BeNull();
                (value as ErrorResponse)?.Message.Should().Be($"{resultType}");
            }
        }
    }
}
