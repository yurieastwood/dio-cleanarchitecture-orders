using DIO.Orders.Domain.Models;
using FluentAssertions;
using NUnit.Framework;

namespace DIO.Orders.Domain.Unit.Tests.Models
{
    [TestFixture]
    internal class CustomerUnitTests
    {
        [Test]
        public void WhenCallToString_ShouldReturnTheFriendlyMessage()
        {
            //Arrange
            const string customerName = "Customer-1";
            const int identifier = 1;

            var customer = new Customer(customerName) { Id = identifier };
            var expectedMessage = $"Customer{{{identifier}}} => {new { Name = customerName }}";

            //Act
            var friendlyMessage = customer.ToString();

            //Assert
            friendlyMessage.Should().Be(expectedMessage);
        }
    }
}
