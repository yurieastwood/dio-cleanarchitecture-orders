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
    internal class PromotionServiceUnitTests
    {
        private MockRepository _mockRepository;
        private Mock<IPromotionRepository> _mockPromotionRepository;

        private PromotionService _service;

        [SetUp]
        public void SetUp()
        {
            _mockRepository = new MockRepository(MockBehavior.Default);
            _mockPromotionRepository = _mockRepository.Create<IPromotionRepository>();

            _service = new PromotionService(_mockPromotionRepository.Object);
        }

        [TestCase(typeof(IPromotionService))]
        [TestCase(typeof(ServiceStorableBase<Promotion>))]
        public void ShouldImplementsOrInherit(Type type)
        {
            //Arrange
            //Act
            //Assert
            _service.Should().BeAssignableTo(type);
        }

        [TestCase(0)]
        [TestCase(1)]
        public void WhenTryAddWithAnyId_ShouldNeverCallAddOrUpdate_FromRepository_AndReturnsAnInvalidPromotion(int identifier)
        {
            //Arrange
            var promotion = new Promotion(PromotionType.Order, null, 10, "Order Promotion") { Id = identifier };

            //Act
            var id = (ResultCodeType)_service.Add(promotion);

            //Assert
            id.Should().Be(ResultCodeType.InvalidPromotion);
            _mockPromotionRepository.Verify(repo => repo.AddOrUpdate(It.IsAny<Promotion>()), Times.Never);
            _mockRepository.VerifyAll();
        }

        [Test]
        public void WhenTryAddWithValidId_ShouldCallAddOrUpdate_FromRepository_AndReturnsAnInvalidPromotion()
        {
            //Arrange            
            var promotion = new Promotion(PromotionType.Product, 5, 10, "Order Promotion");

            _mockPromotionRepository.Setup(repo => repo.AddOrUpdate(It.Is<Promotion>(p => 
                p.Type == promotion.Type && 
                p.TargetId == promotion.TargetId &&
                p.Description == promotion.Description &&
                p.DiscountPercentage == promotion.DiscountPercentage))).Returns(1);

            //Act
            var id = _service.Add(promotion);

            //Assert
            id.Should().Be(1);
            _mockPromotionRepository.Verify(repo => repo.AddOrUpdate(It.IsAny<Promotion>()), Times.Once);
            _mockRepository.VerifyAll();
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void WhenTryUpdateWithIdLessOrEqualToZero_ShouldNeverCallAddOrUpdate_FromRepository_AndReturnsFalse(int identifier)
        {
            //Arrange            
            var promotion = new Promotion(PromotionType.Order, null, 10, "Order Promotion") { Id = identifier };

            //Act
            var id = _service.Update(promotion);

            //Assert
            id.Should().BeFalse();
            _mockPromotionRepository.Verify(repo => repo.AddOrUpdate(It.IsAny<Promotion>()), Times.Never);
            _mockRepository.VerifyAll();
        }

        [Test]
        public void WhenTryUpdateWithValidId_ShouldCallAddOrUpdate_FromRepository_AndReturnsTrue()
        {
            //Arrange            
            var promotion = new Promotion(PromotionType.Product, 5, 10, "Order Promotion") { Id = 1 };

            _mockPromotionRepository.Setup(repo => repo.AddOrUpdate(It.Is<Promotion>(p =>
                p.Type == promotion.Type &&
                p.TargetId == promotion.TargetId &&
                p.Description == promotion.Description &&
                p.DiscountPercentage == promotion.DiscountPercentage))).Returns(1);

            //Act
            var id = _service.Update(promotion);

            //Assert
            id.Should().BeTrue();
            _mockPromotionRepository.Verify(repo => repo.AddOrUpdate(It.IsAny<Promotion>()), Times.Once);
            _mockRepository.VerifyAll();
        }
    }
}
