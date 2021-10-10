using System;
using DIO.Orders.Domain.Enums;
using DIO.Orders.Domain.Models;
using FluentAssertions;
using NUnit.Framework;

namespace DIO.Orders.Domain.Unit.Tests.Models
{
    [TestFixture]
    internal class PromotionUnitTests
    {
        [Test]
        public void WhenCallToString_ShouldReturnTheFriendlyMessage()
        {
            //Arrange
            const string orderPromotionDescription = "Order Promotion";
            const string productPromotionDescription = "Product Promotion";
            const int discountPercentage = 10;
            const int targetId = 2;
            const int identifier = 1;

            var orderPromotion = new Promotion(PromotionType.Order, null, discountPercentage, orderPromotionDescription) { Id = identifier };
            var productPromotion = new Promotion(PromotionType.Product, targetId, discountPercentage, productPromotionDescription) { Id = identifier };
            var expectedOrderPromotionMessage = $"Promotion{{{identifier}}} [{PromotionType.Order.ToString().ToUpper()}] => {new { DiscountPercentage = discountPercentage }}";
            var expectedProductPromotionMessage = $"Promotion{{{identifier}}} [{PromotionType.Product.ToString().ToUpper()}] => {new { Product = targetId, DiscountPercentage = discountPercentage }}";

            //Act
            var friendlyOrderPromotionMessage = orderPromotion.ToString();
            var friendlyProductPromotionMessage = productPromotion.ToString();

            //Assert
            friendlyOrderPromotionMessage.Should().Be(expectedOrderPromotionMessage);
            friendlyProductPromotionMessage.Should().Be(expectedProductPromotionMessage);
        }

        [TestCase(null)]
        [TestCase(0)]
        [TestCase(-1)]
        public void OnPromotionCreation_ForTypeProduct_ShouldThrowException_WhenTargetIdWasNotSetOrItIsInvalid(int? targetId)
        {
            //Arrange
            //Act
            //Assert
            Assert.Throws<ArgumentNullException>(() => _ = new Promotion(PromotionType.Product, targetId, 10, string.Empty));
        }

        [TestCase(PromotionType.Order, null, -1)]
        [TestCase(PromotionType.Order, null, 101)]
        [TestCase(PromotionType.Product, 1, -1)]
        [TestCase(PromotionType.Product, 1, 101)]
        public void OnPromotionCreation_ShouldThrowException_WhenTheDiscountPercentageIsOutOfRange(PromotionType type, int? targetId, int discountPercentage)
        {
            //Arrange
            //Act
            //Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => _ = new Promotion(type, targetId, discountPercentage, string.Empty));
        }
    }
}
