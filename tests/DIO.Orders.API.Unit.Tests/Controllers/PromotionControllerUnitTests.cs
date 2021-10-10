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
    internal class PromotionControllerUnitTests
    {
        private const int TargetId = 5;
        private const int DiscountPercentage = 10;
        private const int FirstId = 1;
        private readonly Exception _exception = new("Generic Exception");
        private readonly Promotion _promotion = new(PromotionType.Product,  TargetId, DiscountPercentage, "Product Promotion") { Id = FirstId };

        private PromotionController _controller;

        private MockRepository _mockRepository;
        private Mock<IPromotionService> _mockService;


        [SetUp]
        public void SetUp()
        {
            _mockRepository = new MockRepository(MockBehavior.Default);
            _mockService = _mockRepository.Create<IPromotionService>();

            _controller = new PromotionController(_mockService.Object);
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
        public void WhenGet_ShouldReturnAsExpected(int quantityOfPromotions, bool throwException, Type expectedTypeResult)
        {
            //Arrange
            var promotions = new List<Promotion>();

            Enumerable
                .Range(0, quantityOfPromotions)
                .ToList()
                .ForEach(index => promotions.Add(new Promotion(PromotionType.Order, null, index + 1, $"Order Promotion-{index}")));

            if (throwException)
                _mockService.Setup(_ => _.Get()).Throws(_exception);
            else
                _mockService.Setup(_ => _.Get()).Returns(promotions);

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
            else if (quantityOfPromotions > 0)
            {
                value.Should().NotBeNull();
                value?.Should().BeOfType<List<Promotion>>();
                (value as List<Promotion>)?.Should().BeEquivalentTo(promotions);
            }
        }

        [TestCase(false, false, typeof(NotFoundResult))]
        [TestCase(true, false, typeof(OkObjectResult))]
        [TestCase(true, true, typeof(BadRequestObjectResult))]
        public void WhenGetById_ShouldReturnAsExpected(bool promotionExists, bool throwException, Type expectedTypeResult)
        {
            //Arrange
            var promotion = promotionExists ? _promotion : null;

            if (throwException)
                _mockService.Setup(_ => _.Get(FirstId)).Throws(_exception);
            else
                _mockService.Setup(_ => _.Get(FirstId)).Returns(promotion);

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
            else if (promotionExists)
            {
                value.Should().NotBeNull();
                value?.Should().BeOfType<Promotion>();
                (value as Promotion)?.Id.Should().Be(promotion.Id);
                (value as Promotion)?.Description.Should().Be(promotion.Description);
                (value as Promotion)?.TargetId.Should().Be(promotion.TargetId);
                (value as Promotion)?.DiscountPercentage.Should().Be(promotion.DiscountPercentage);
            }
        }

        [TestCase(false, typeof(OkObjectResult))]
        [TestCase(true, typeof(BadRequestObjectResult))]
        public void WhenCallPrint_ShouldReturnAsExpected(bool throwException, Type expectedTypeResult)
        {
            //Arrange
            var expectedMessage = _promotion.ToString();

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
                _mockService.Setup(_ => _.Update(It.IsAny<Promotion>())).Throws(_exception);
            else
                _mockService.Setup(_ => _.Update(It.Is<Promotion>(p => 
                    p.Type == _promotion.Type && 
                    p.TargetId == _promotion.TargetId && 
                    p.Description == _promotion.Description && 
                    p.DiscountPercentage == _promotion.DiscountPercentage && 
                    p.Id == _promotion.Id))).Returns(updateResult);

            //Act
            var result = _controller.Update(_promotion);
            var value = (result as ObjectResult)?.Value;

            //Assert
            _mockService.Verify(_ => _.Update(It.IsAny<Promotion>()), Times.Once);
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
        [TestCase(-3, ResultCodeType.InvalidPromotion, false, typeof(BadRequestObjectResult))]
        [TestCase(-100, ResultCodeType.NotCreated, false, typeof(BadRequestObjectResult))]
        [TestCase(1, null, true, typeof(BadRequestObjectResult))]
        public void WhenCallAdd_ShouldReturnAsExpected(int idReturned, ResultCodeType? resultType, bool throwException, Type expectedTypeResult)
        {
            //Arrange
            var promotion = new Promotion(_promotion.Type, _promotion.TargetId, _promotion.DiscountPercentage, _promotion.Description);
            if (throwException)
                _mockService.Setup(_ => _.Add(It.IsAny<Promotion>())).Throws(_exception);
            else
                _mockService.Setup(_ => _.Add(It.Is<Promotion>(p =>
                    p.Type == _promotion.Type &&
                    p.TargetId == _promotion.TargetId &&
                    p.Description == _promotion.Description &&
                    p.DiscountPercentage == _promotion.DiscountPercentage &&
                    p.Id == null))).Returns(idReturned);

            //Act
            var result = _controller.Add(promotion);
            var value = (result as ObjectResult)?.Value;

            //Assert
            _mockService.Verify(_ => _.Add(It.IsAny<Promotion>()), Times.Once);
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
