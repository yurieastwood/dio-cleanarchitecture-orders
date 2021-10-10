using DIO.Orders.Application.Services;
using DIO.Orders.Domain.Repositories;

namespace DIO.Orders.Application.Unit.Tests.Stubs
{
    /// <summary>
    /// A fake instance of <see cref="IStorable"/> to be used in the <see cref="FakeService"/> to validate the <see cref="ServiceStorableBase{T}"/> methods and the repository extensions.
    /// </summary>
    public class FakeStorableItem : IStorable
    {
        /// <summary>
        /// The identifier.
        /// </summary>
        public int? Id { get; set; } = null;

        /// <summary>
        /// A simple description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// A friendly message to be used on ToString overriden.
        /// </summary>
        public string FriendlyMessage { get; set; }

        /// <summary>
        /// Update the description an return the instance of <see cref="FakeStorableItem"/>.
        /// </summary>
        /// <param name="description">The new description.</param>
        /// <returns>An instance of <see cref="FakeStorableItem"/> with the description updated.</returns>
        public FakeStorableItem WithDescription(string description)
        {
            Description = description;

            return this;
        }

        /// <summary>
        /// Update the id an return the instance of <see cref="FakeStorableItem"/>.
        /// </summary>
        /// <param name="id">The new description.</param>
        /// <returns>An instance of <see cref="FakeStorableItem"/> with the id updated.</returns>
        public FakeStorableItem WithId(int id)
        {
            Id = id;

            return this;
        }

        /// <summary>
        /// Update the friendlyMessage an return the instance of <see cref="FakeStorableItem"/>.
        /// </summary>
        /// <param name="friendlyMessage">The new description.</param>
        /// <returns>An instance of <see cref="FakeStorableItem"/> with the friendlyMessage updated.</returns>
        public FakeStorableItem WithFriendlyMessage(string friendlyMessage)
        {
            FriendlyMessage = friendlyMessage;

            return this;
        }

        /// <inheritdoc />
        public override string ToString() => FriendlyMessage;
    }
}
