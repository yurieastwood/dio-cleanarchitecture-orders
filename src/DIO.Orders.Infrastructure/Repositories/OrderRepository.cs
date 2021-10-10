using DIO.Orders.Domain.Models;
using DIO.Orders.Domain.Repositories;
using DIO.Orders.Infrastructure.Contexts;

namespace DIO.Orders.Infrastructure.Repositories
{
    /// <inheritdoc cref="IOrderRepository"/>
    /// <inheritdoc cref="InMemoryContextRepositoryBase{T}"/>
    public class OrderRepository : InMemoryContextRepositoryBase<Order>, IOrderRepository { }
}
