using DIO.Orders.Domain.Models;
using DIO.Orders.Domain.Repositories;
using DIO.Orders.Infrastructure.Contexts;

namespace DIO.Orders.Infrastructure.Repositories
{
    /// <inheritdoc cref="IProductRepository"/>
    /// <inheritdoc cref="InMemoryContextRepositoryBase{T}"/>
    public class ProductRepository : InMemoryContextRepositoryBase<Product>, IProductRepository { }
}
