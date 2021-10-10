using System;
using System.Collections.Generic;
using DIO.Orders.Application.Extensions;
using DIO.Orders.Application.Unit.Tests.Stubs;
using DIO.Orders.Domain.Repositories;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace DIO.Orders.Application.Unit.Tests.Extensions
{
    [TestFixture]
    internal class RepositoryExtensionsUnitTests
    {
        private MockRepository _mockRepository;
        private Mock<IRepository<FakeStorableItem>> _mockItemRepository;

        [SetUp]
        public void SetUp()
        {
            _mockRepository = new MockRepository(MockBehavior.Default);
            _mockItemRepository = _mockRepository.Create<IRepository<FakeStorableItem>>();
        }

        [Test]
        public void OnContains_ShouldVerifyIfAnSpecificItemWasReturnedFromRepository_AndReturnAsExpected()
        {
            //Arrange
            const int firstId = 1;
            const int secondId = 2;

            var validItem = new FakeStorableItem().WithId(firstId);
            var invalidItem = new FakeStorableItem().WithId(secondId);
            var resultCollection = new List<FakeStorableItem> { validItem };

            Func<FakeStorableItem, bool> filter = null;
            _mockItemRepository.Setup(repo => repo.Get(It.IsAny<Func<FakeStorableItem, bool>>())).Returns(resultCollection).Callback<Func<FakeStorableItem, bool>>(_ => filter = _);

            //Act
            var result = _mockItemRepository.Object.Contains(firstId);

            //Assert
            result.Should().BeTrue();
            filter(validItem).Should().BeTrue();
            filter(invalidItem).Should().BeFalse();
        }

        [Test]
        public void OnTryGetAll_IfFailed_ShouldReturnFalse()
        {
            //Arrange
            _mockItemRepository.Setup(repo => repo.Get(It.IsAny<Func<FakeStorableItem, bool>>())).Throws(new Exception());

            //Act
            var result = _mockItemRepository.Object.TryGetAll(It.IsAny<Func<FakeStorableItem, bool>>(), out _);

            //Assert
            result.Should().BeFalse();
        }

        [Test]
        public void OnTryGet_IfFailed_ShouldReturnFalse()
        {
            //Arrange
            _mockItemRepository.Setup(repo => repo.Get(It.IsAny<Func<FakeStorableItem, bool>>())).Throws(new Exception());

            //Act
            var result = _mockItemRepository.Object.TryGetFirst(_ => true, out _);

            //Assert
            result.Should().BeFalse();
        }
    }
}
