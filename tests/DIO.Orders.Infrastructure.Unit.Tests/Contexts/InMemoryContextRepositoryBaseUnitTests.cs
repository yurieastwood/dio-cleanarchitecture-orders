using DIO.Orders.Domain.Repositories;
using DIO.Orders.Infrastructure.Contexts;
using FluentAssertions;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace DIO.Orders.Infrastructure.Unit.Tests.Contexts
{
    [TestFixture]
    internal class InMemoryContextRepositoryBaseUnitTests
    {
        private readonly FakeRepository _repository = new ();

        [Test]
        public void BaseShouldImplementsIRepository()
        {
            //Arrance
            //Act
            //Assert
            _repository.GetType().BaseType.Should().BeAssignableTo<IRepository<FakeStorableItem>>();
        }

        [Test]
        public void RepositoryId_ShouldStartFromOne()
        {
            //Arrange
            var newRepository = new FakeRepository();

            //Act
            //Assert
            newRepository.AddOrUpdate(new FakeStorableItem()).Should().Be(1);
        }

        [Test]
        public void WhenAddOrUpdate_ShouldIncludeNewItems_UpdateExistentOnes_AndReturnTheId()
        {
            //Arrange
            const string newDescription = "New-Description";
            int idInserted, idUpdated, quantityAfterInsertBeforeUpdate, quantityBeforeInsert, expectedQuantityAfterInsert, quantityAfterUpdate, expectedQuantityAfterUpdate;
            var item = new FakeStorableItem();
            FakeStorableItem itemUpdated;

            //Act
            quantityBeforeInsert = _repository.Count();
            idInserted = _repository.AddOrUpdate(item);
            quantityAfterInsertBeforeUpdate = _repository.Count();

            idUpdated = _repository.AddOrUpdate(item.WithId(idInserted).WithDescription(newDescription));
            quantityAfterUpdate = _repository.Count();

            itemUpdated = _repository.Get(storable => storable.Id == idUpdated).FirstOrDefault(); 

            expectedQuantityAfterInsert = quantityBeforeInsert + 1;
            expectedQuantityAfterUpdate = quantityAfterInsertBeforeUpdate;

            //Assert
            idInserted.Should().BeGreaterThan(0);
            idUpdated.Should().Be(idInserted);
            itemUpdated.Should().NotBeNull();
            itemUpdated.Should().BeSameAs(item);
            quantityAfterInsertBeforeUpdate.Should().Be(expectedQuantityAfterInsert);
            quantityAfterUpdate.Should().Be(expectedQuantityAfterUpdate);
        }

        [Test]
        public void WhenDelete_ShouldRemoeTheItemFromRepository_AndReturnTheQuantityOfItemsDeleted()
        {
            //Arrange
            int quantityBeforeDelete, quantityAfterDelete, expectedQuantityAfterDelete;
            FakeStorableItem item;
            var itemsAdded = new List<int>();

            //Act
            Enumerable
                .Range(1, 3)
                .ToList()
                .ForEach(_ => itemsAdded.Add(_repository.AddOrUpdate(new FakeStorableItem())));

            quantityBeforeDelete = _repository.Count();
            var quantityDeleted = _repository.Delete(storable => itemsAdded.Contains(storable.Id ?? 0));
            quantityAfterDelete = _repository.Count();

            item = _repository.Get(storable => storable.Id == itemsAdded.First()).FirstOrDefault();

            expectedQuantityAfterDelete = quantityBeforeDelete - itemsAdded.Count;

            //Assert
            quantityDeleted.Should().Be(itemsAdded.Count);
            quantityAfterDelete.Should().Be(expectedQuantityAfterDelete);
            item.Should().BeNull();
        }

        [Test]
        public void WhenGet_WithoutFilter_ShouldReturnAllItemsInRepository()
        {
            //Arrange
            int quantityOfItems;
            IEnumerable<FakeStorableItem> items;

            //Act
            quantityOfItems = _repository.Count();
            items = _repository.Get();

            //Assert
            items.Count().Should().Be(quantityOfItems);
        }

        [Test]
        public void WhenGet_WithFilter_ShouldFilterTheItemsAndReturnOnlyIfMatch()
        {
            //Arrange
            IEnumerable<FakeStorableItem> itemsSelected;
            var itemsAdded = new List<int>();

            //Act
            Enumerable
                .Range(1, 3)
                .ToList()
                .ForEach(_ => itemsAdded.Add(_repository.AddOrUpdate(new FakeStorableItem())));

            itemsSelected = _repository.Get(storable => itemsAdded.Contains(storable.Id ?? 0));

            //Assert
            itemsSelected.Select(item => item.Id).Should().BeEquivalentTo(itemsAdded);
        }

        private class FakeStorableItem : IStorable
        {
            public int? Id { get; set; } = null;

            public string Description { get; set; }

            public FakeStorableItem WithDescription(string description) 
            {
                Description = description;

                return this;
            }

            public FakeStorableItem WithId(int id)
            {
                Id = id;

                return this;
            }
        }

        private class FakeRepository : InMemoryContextRepositoryBase<FakeStorableItem> { }
    }
}
