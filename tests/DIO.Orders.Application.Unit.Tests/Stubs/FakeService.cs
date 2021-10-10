using DIO.Orders.Application.Services;
using DIO.Orders.Domain.Repositories;

namespace DIO.Orders.Application.Unit.Tests.Stubs
{
    /// <summary>
    /// A fake service to create the unit test for the <see cref="ServiceStorableBase{T}"/> methods.
    /// </summary>
    public class FakeService : ServiceStorableBase<FakeStorableItem>
    {
        /// <summary>
        /// Initialize an instance of <see cref="FakeService"/>.
        /// </summary>
        /// <param name="repository">An instance of <see cref="IRepository{T}"/> or <see cref="FakeStorableItem"/>.</param>
        public FakeService(IRepository<FakeStorableItem> repository) : base(repository) { }
    }
}
