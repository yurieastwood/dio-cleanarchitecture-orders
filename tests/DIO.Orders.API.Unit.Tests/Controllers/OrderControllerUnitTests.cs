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
    internal class OrderControllerUnitTests
    {
        private const int FirstId = 1;
        private const int TargetId = 5;
        private const int DiscountPercentage = 10;
        private readonly Exception _exception = new("Generic Exception");
        private readonly Product _product = new("Product", 10) { Id = FirstId };
        private readonly List<Product> _products = new();
        private readonly Customer _customer = new("Customer") { Id = FirstId };
        private readonly Promotion _promotion = new(PromotionType.Product, TargetId, DiscountPercentage, "Product Promotion") { Id = FirstId };
        private Order _order;

        private OrderController _controller;

        private MockRepository _mockRepository;
        private Mock<IOrderService> _mockService;


        [SetUp]
        public void SetUp()
        {
            _mockRepository = new MockRepository(MockBehavior.Default);
            _mockService = _mockRepository.Create<IOrderService>();

            _products.Add(_product);
            _order = new Order(_products, _customer, _promotion) {Id = FirstId};

            _controller = new OrderController(_mockService.Object);
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
        public void WhenGet_ShouldReturnAsExpected(int quantityOfOrders, bool throwException, Type expectedTypeResult)
        {
            //Arrange
            var orders = new List<Order>();

            Enumerable
                .Range(0, quantityOfOrders)
                .ToList()
                .ForEach(_ => orders.Add(new Order(_products, _customer)));

            if (throwException)
                _mockService.Setup(_ => _.Get()).Throws(_exception);
            else
                _mockService.Setup(_ => _.Get()).Returns(orders);

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
            else if (quantityOfOrders > 0)
            {
                value.Should().NotBeNull();
                value?.Should().BeOfType<List<Order>>();
                (value as List<Order>)?.Should().BeEquivalentTo(orders);
            }
        }

        [TestCase(false, false, typeof(NotFoundResult))]
        [TestCase(true, false, typeof(OkObjectResult))]
        [TestCase(true, true, typeof(BadRequestObjectResult))]
        public void WhenGetById_ShouldReturnAsExpected(bool orderExists, bool throwException, Type expectedTypeResult)
        {
            //Arrange
            var order = orderExists ? _order : null;

            if (throwException)
                _mockService.Setup(_ => _.Get(FirstId)).Throws(_exception);
            else
                _mockService.Setup(_ => _.Get(FirstId)).Returns(order);

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
            else if (orderExists)
            {
                value.Should().NotBeNull();
                value?.Should().BeOfType<Order>();
                (value as Order)?.Id.Should().Be(order.Id);
                (value as Order)?.Customer.Id.Should().Be(_customer.Id);
                (value as Order)?.Customer.Name.Should().Be(_customer.Name);
                (value as Order)?.Promotion.Id.Should().Be(_promotion.Id);
                (value as Order)?.Promotion.TargetId.Should().Be(_promotion.TargetId);
                (value as Order)?.Promotion.Description.Should().Be(_promotion.Description);
                (value as Order)?.Promotion.Type.Should().Be(_promotion.Type);
                (value as Order)?.Promotion.DiscountPercentage.Should().Be(_promotion.DiscountPercentage);
                (value as Order)?.Products.Should().BeEquivalentTo(_products);
            }
        }

        [TestCase(false, typeof(OkObjectResult))]
        [TestCase(true, typeof(BadRequestObjectResult))]
        public void WhenCallPrint_ShouldReturnAsExpected(bool throwException, Type expectedTypeResult)
        {
            //Arrange
            var expectedMessage = _order.ToString();

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
        [TestCase(-2, ResultCodeType.InvalidCustomer, false, typeof(BadRequestObjectResult))]
        [TestCase(-3, ResultCodeType.InvalidPromotion, false, typeof(BadRequestObjectResult))]
        [TestCase(-100, ResultCodeType.NotCreated, false, typeof(BadRequestObjectResult))]
        [TestCase(1, null, true, typeof(BadRequestObjectResult))]
        public void WhenCallCreate_ShouldReturnAsExpected(int idReturned, ResultCodeType? resultType, bool throwException, Type expectedTypeResult)
        {
            //Arrange
            var productIds = _products.Select(_ => _.Id ?? 0).ToList();

            if (throwException)
                _mockService.Setup(_ => _.CreateOrder(It.IsAny<int>(), It.IsAny<List<int>>())).Throws(_exception);
            else
                _mockService.Setup(_ => _.CreateOrder(_customer.Id ?? 0, It.Is<List<int>>(products =>
                    products.TrueForAll(productIds.Contains)))).Returns(idReturned);

            //Act
            var result = _controller.Create(_customer.Id ?? 0, productIds);
            var value = (result as ObjectResult)?.Value;

            //Assert
            _mockService.Verify(_ => _.CreateOrder(It.IsAny<int>(), It.IsAny<List<int>>()), Times.Once);
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

        [TestCase(true, false, typeof(OkObjectResult))]
        [TestCase(false, false, typeof(OkObjectResult))]
        [TestCase(true, true, typeof(BadRequestObjectResult))]
        public void WhenCallAddProducts_ShouldReturnAsExpected(bool addProductsResult, bool throwException, Type expectedTypeResult)
        {
            //Arrange
            var productIds = _products.Select(_ => _.Id ?? 0).ToList();
            if (throwException)
                _mockService.Setup(_ => _.AddProductsTo(It.IsAny<int>(), It.IsAny<List<int>>())).Throws(_exception);
            else
                _mockService.Setup(_ => _.AddProductsTo(FirstId, It.Is<List<int>>(products =>
                    products.TrueForAll(productIds.Contains)))).Returns(addProductsResult);

            //Act
            var result = _controller.AddProducts(FirstId, productIds);
            var value = (result as ObjectResult)?.Value;

            //Assert
            _mockService.Verify(_ => _.AddProductsTo(It.IsAny<int>(), It.IsAny<List<int>>()), Times.Once);
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
                (value as bool?)?.Should().Be(addProductsResult);
            }
        }

        [TestCase(1, false, typeof(OkObjectResult))]
        [TestCase(0, false, typeof(OkObjectResult))]
        [TestCase(0, true, typeof(BadRequestObjectResult))]
        public void WhenCallRemoveProducts_ShouldReturnAsExpected(int quantityRemoved, bool throwException, Type expectedTypeResult)
        {
            //Arrange
            var productIds = _products.Select(_ => _.Id ?? 0).ToList();
            if (throwException)
                _mockService.Setup(_ => _.RemoveProductsFrom(It.IsAny<int>(), It.IsAny<List<int>>())).Throws(_exception);
            else
                _mockService.Setup(_ => _.RemoveProductsFrom(FirstId, It.Is<List<int>>(products =>
                    products.TrueForAll(productIds.Contains)))).Returns(quantityRemoved);

            //Act
            var result = _controller.RemoveProducts(FirstId, productIds);
            var value = (result as ObjectResult)?.Value;

            //Assert
            _mockService.Verify(_ => _.RemoveProductsFrom(It.IsAny<int>(), It.IsAny<List<int>>()), Times.Once);
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
                value?.Should().BeOfType<int>();
                (value as int?)?.Should().Be(quantityRemoved);
            }
        }

        [TestCase(30.8, false, typeof(OkObjectResult))]
        [TestCase(0, false, typeof(OkObjectResult))]
        [TestCase(0, true, typeof(BadRequestObjectResult))]
        public void WhenCallApplyPromotion_ShouldReturnAsExpected(double discountApplied, bool throwException, Type expectedTypeResult)
        {
            //Arrange
            if (throwException)
                _mockService.Setup(_ => _.ApplyPromotionTo(It.IsAny<int>(), It.IsAny<int>())).Throws(_exception);
            else
                _mockService.Setup(_ => _.ApplyPromotionTo(FirstId, _promotion.Id ?? 0)).Returns(discountApplied);

            //Act
            var result = _controller.ApplyPromotion(FirstId, _promotion.Id ?? 0);
            var value = (result as ObjectResult)?.Value;

            //Assert
            _mockService.Verify(_ => _.ApplyPromotionTo(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
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
                value?.Should().BeOfType<double>();
                (value as double?)?.Should().Be(discountApplied);
            }
        }
    }
}
