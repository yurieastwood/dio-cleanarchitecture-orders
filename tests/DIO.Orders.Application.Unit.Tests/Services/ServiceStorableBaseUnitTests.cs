using System;
using System.Collections.Generic;
using DIO.Orders.Application.Unit.Tests.Stubs;
using DIO.Orders.Domain.Repositories;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace DIO.Orders.Application.Unit.Tests.Services
{
    [TestFixture]
    internal class ServiceStorableBaseUnitTests
    {
        private const string Description = "Description";
        private const string FriendlyMessage = "Friendly Printing";
        
        private readonly string _messageNotFoundForPrint = $"{nameof(FakeStorableItem)} does not exist in the repository!";
        private readonly Random _random = new (1);

        private FakeService _storableService;

        private MockRepository _mockRepository;
        private Mock<IRepository<FakeStorableItem>> _mockItemRepository;

        [SetUp]
        public void SetUp()
        {
            _mockRepository = new MockRepository(MockBehavior.Default);
            _mockItemRepository = _mockRepository.Create<IRepository<FakeStorableItem>>();

            _storableService = new FakeService(_mockItemRepository.Object);
        }

        [Test]
        public void WhenAdd_ShouldCallAddOrUpdate_FromRepository_AndReturnsAnInteger()
        {
            //Arrange            
            var idInserted = _random.Next(10);
            var item = new FakeStorableItem().WithDescription(Description);

            _mockItemRepository.Setup(repo =>
                repo.AddOrUpdate(It.Is<FakeStorableItem>(storable =>
                    storable.Id == null && storable.Description.Equals(Description)))).Returns(idInserted);

            //Act
            var id = _storableService.Add(item);

            //Assert
            id.Should().Be(idInserted);
            _mockItemRepository.Verify(repo => repo.AddOrUpdate(It.IsAny<FakeStorableItem>()), Times.Once);
            _mockRepository.VerifyAll();
        }

        [TestCase(1, 2, false)]
        [TestCase(1, 1, true)]
        public void WhenUpdate_ShouldCallAddOrUpdate_FromRepository_AndReturnsResultAsExpected(int itemId, int idReturned, bool expectedResult)
        {
            //Arrange            
            var item = new FakeStorableItem().WithId(itemId).WithDescription(Description);

            _mockItemRepository.Setup(repo =>
                repo.AddOrUpdate(It.Is<FakeStorableItem>(storable =>
                    storable.Id == itemId && storable.Description.Equals(Description)))).Returns(idReturned);

            //Act
            var result = _storableService.Update(item);

            //Assert
            result.Should().Be(expectedResult);
            _mockItemRepository.Verify(repo => repo.AddOrUpdate(It.IsAny<FakeStorableItem>()), Times.Once);
            _mockRepository.VerifyAll();
        }

        [Test]
        public void OnGet_ShouldCallGet_FromRepository_AndReturnAllItems()
        {
            //Arrange            
            _mockItemRepository.Setup(repo => repo.Get()).Returns(new List<FakeStorableItem>());

            //Act
            _ = _storableService.Get();

            //Assert
            _mockItemRepository.Verify(repo => repo.Get(), Times.Once);
            _mockRepository.VerifyAll();
        }

        [Test]
        public void WhenGetById_ShouldCallGet_WithFilter_FromRepository_AndReturnTheItem()
        {
            //Arrange
            var idSelected = _random.Next(10);
            var distinctId = idSelected + 1;
            var expectedItem = new FakeStorableItem().WithId(idSelected);
            var invalidItem = new FakeStorableItem().WithId(distinctId);
            Func<FakeStorableItem, bool> filter = null;


            _mockItemRepository
                .Setup(repo => repo.Get(It.IsAny<Func<FakeStorableItem, bool>>()))
                .Returns(new List<FakeStorableItem> { new FakeStorableItem().WithId(idSelected) })
                .Callback<Func<FakeStorableItem, bool>>(_ => filter = _);
            
            //Act
            var item = _storableService.Get(idSelected);
            var filteredWithCorrectId = filter(expectedItem);
            var filteredWithDistinctId = filter(invalidItem);

            //Assert
            item.Id.Should().Be(idSelected);
            filteredWithCorrectId.Should().BeTrue();
            filteredWithDistinctId.Should().BeFalse();
            _mockItemRepository.Verify(repo => repo.Get(It.IsAny<Func<FakeStorableItem, bool>>()), Times.Once);
            _mockRepository.VerifyAll();
        }

        [TestCase(1, true)]
        [TestCase(2, false)]
        public void WhenDelete_WithId_ShouldCallDelete_WithFilter_FromRepository_AndReturnsTrueWhenOnlyOneItemWasDeleted(int quantityDeleted, bool expectedResult)
        {
            //Arrange
            var idDeleted = _random.Next(10);
            var distinctId = idDeleted + 1;
            var deletedItem = new FakeStorableItem().WithId(idDeleted);
            var invalidItem = new FakeStorableItem().WithId(distinctId);
            Func<FakeStorableItem, bool> filter = null;

            _mockItemRepository
                .Setup(repo => repo.Delete(It.IsAny<Func<FakeStorableItem, bool>>()))
                .Returns(quantityDeleted)
                .Callback<Func<FakeStorableItem, bool>>(_ => filter = _);

            //Act
            var result = _storableService.Delete(idDeleted);
            var filteredWithCorrectId = filter(deletedItem);
            var filteredWithDistinctId = filter(invalidItem);

            //Assert
            result.Should().Be(expectedResult);
            filteredWithCorrectId.Should().BeTrue();
            filteredWithDistinctId.Should().BeFalse();
            _mockItemRepository.Verify(repo => repo.Delete(It.IsAny<Func<FakeStorableItem, bool>>()), Times.Once);
            _mockRepository.VerifyAll();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void WhenPrinting_ShouldGetFromRepository_AndReturnFriendlyStringCasting(bool itemExists)
        {
            //Arrange
            var expectedMessage = itemExists ? FriendlyMessage : _messageNotFoundForPrint;
            var idSelected = _random.Next(10);
            var distinctId = idSelected + 1;
            var expectedItem = new FakeStorableItem().WithId(idSelected).WithFriendlyMessage(FriendlyMessage);
            var invalidItem = new FakeStorableItem().WithId(distinctId);
            var getResult = new List<FakeStorableItem>();
            if(itemExists) getResult.Add(expectedItem);

            Func<FakeStorableItem, bool> filter = null;

            _mockItemRepository
                .Setup(repo => repo.Get(It.IsAny<Func<FakeStorableItem, bool>>()))
                .Returns(getResult)
                .Callback<Func<FakeStorableItem, bool>>(_ => filter = _);

            //Act
            var message = _storableService.Print(idSelected);
            var filteredWithCorrectId = filter(expectedItem);
            var filteredWithDistinctId = filter(invalidItem);

            //Assert
            message.Should().Be(expectedMessage);
            filteredWithCorrectId.Should().BeTrue();
            filteredWithDistinctId.Should().BeFalse();
            _mockItemRepository.Verify(repo => repo.Get(It.IsAny<Func<FakeStorableItem, bool>>()), Times.Once);
            _mockRepository.VerifyAll();
        }
    }
}
