using DIO.Orders.Domain.Models;
using FluentAssertions;
using NUnit.Framework;

namespace DIO.Orders.Domain.Unit.Tests.Models
{
    [TestFixture]
    internal class ProductUnitTests
    {
        [Test]
        public void WhenCallToString_ShouldReturnTheFriendlyMessage()
        {
            //Arrange
            const string productName = "Product-1";
            const double value = 10;
            const int identifier = 1;

            var product = new Product(productName, value) {Id = identifier};
            var expectedMessage = $"Product{{{identifier}}} => {new {Name = productName, Value = value}}";

            //Act
            var friendlyMessage = product.ToString();

            //Assert
            friendlyMessage.Should().Be(expectedMessage);
        }
    }
}
