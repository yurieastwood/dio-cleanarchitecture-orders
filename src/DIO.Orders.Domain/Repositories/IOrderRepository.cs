using DIO.Orders.Domain.Models;

namespace DIO.Orders.Domain.Repositories
{
    /// <summary>
    /// The <see cref="Order"/> repository.
    /// </summary>
    /// <inheritdoc cref="IRepository{T}"/>
    public interface IOrderRepository : IRepository<Order> { }
}
