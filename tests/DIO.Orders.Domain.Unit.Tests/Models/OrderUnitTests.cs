using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DIO.Orders.Domain.Enums;
using DIO.Orders.Domain.Models;
using FluentAssertions;
using NUnit.Framework;

namespace DIO.Orders.Domain.Unit.Tests.Models
{
    [TestFixture]
    internal class OrderUnitTests
    {
        [Test]
        public void OnOrderCreation_ShouldThrowException_WhenProductListIsEmpty()
        {
            //Arrange
            //Act
            //Assert
            Assert.Throws<ArgumentNullException>(() => _ = new Order(new List<Product>(), new Customer(string.Empty)));
        }

        [Test]
        public void WhenTryToRemoveAnEmptyProductList_ShouldReturnZero()
        {
            //Arrange
            var order = new Order(new List<Product> { new(string.Empty, 10) }, new Customer(string.Empty));

            //Act
            var result = order.RemoveProducts(new List<int>());

            //Assert
            result.Should().Be(0);
        }

        [Test]
        public void WhenTryToAddAnEmptyProductList_ShouldDoNothing()
        {
            //Arrange
            var order = new Order(new List<Product> { new(string.Empty, 10) }, new Customer(string.Empty));

            //Act
            order.AddProducts(new List<Product>());

            //Assert
            order.Products.Count.Should().Be(1);
        }

        [Test]
        public void WhenTryToApplyProductPromotion_AndTheOrderDoesNotContainsTheProduct_ShouldDoNothing()
        {
            //Arrange
            var order = new Order(new List<Product> { new(string.Empty, 10) { Id = 1 } }, new Customer(string.Empty));
            var promotion = new Promotion(PromotionType.Product, 2, 10, string.Empty);

            //Act
            var result = order.ApplyPromotion(promotion);

            //Assert
            result.Should().Be(0);
        }

        [Test]
        public void WhenCallToString_WithProductPromoted_ShouldReturnTheFriendlyTextAsExpected()
        {
            //Arrange
            const int identifier = 1;
            var products = new[] { new Product("Product-1", 50) { Id = identifier }, new Product("Product-2", 30) { Id = identifier } }.ToList();
            var customer = new Customer("Customer-1");
            var promotion= new Promotion(PromotionType.Product, 1, 10, "Product-Promotion-1");
            var order = new Order(products, customer, promotion) { Id = identifier };
            
            var expectedMessage = new StringBuilder();
            expectedMessage.AppendLine($"Order{{{identifier}}}");
            expectedMessage.AppendLine($"\tClient: {customer.Name.ToUpper()}");
            expectedMessage.AppendLine("\tProducts:");
            expectedMessage.AppendLine();
            products.ForEach(product => expectedMessage.AppendLine($"\t\t{product.Id}\t{product.Name.ToUpper()}\t{product.Value:C}P"));
            expectedMessage.AppendLine();
            // ReSharper disable once PossiblyMistakenUseOfInterpolatedStringInsert
            expectedMessage.AppendLine($"\tDiscount: {5:C}");
            expectedMessage.AppendLine($"\tTotal: {75:C}");

            //Act
            var friendlyText = order.ToString();

            //Assert
            friendlyText.Should().Be(expectedMessage.ToString());
        }

        [Test]
        public void WhenCallToString_WithOrderPromotion_ShouldReturnTheFriendlyTextAsExpected()
        {
            //Arrange
            const int identifier = 1;
            var products = new[] { new Product("Product-1", 50) { Id = identifier }, new Product("Product-2", 30) { Id = identifier } }.ToList();
            var customer = new Customer("Customer-1");
            var promotion = new Promotion(PromotionType.Order, null, 10, "Product-Promotion-1");
            var order = new Order(products, customer, promotion) { Id = identifier };

            var expectedMessage = new StringBuilder();
            expectedMessage.AppendLine($"Order{{{identifier}}}");
            expectedMessage.AppendLine($"\tClient: {customer.Name.ToUpper()}");
            expectedMessage.AppendLine("\tProducts:");
            expectedMessage.AppendLine();
            products.ForEach(product => expectedMessage.AppendLine($"\t\t{product.Id}\t{product.Name.ToUpper()}\t{product.Value:C}"));
            expectedMessage.AppendLine();
            // ReSharper disable once PossiblyMistakenUseOfInterpolatedStringInsert
            expectedMessage.AppendLine($"\tDiscount: {8:C}");
            expectedMessage.AppendLine($"\tTotal: {72:C}");

            //Act
            var friendlyText = order.ToString();

            //Assert
            friendlyText.Should().Be(expectedMessage.ToString());
        }
    }
}
