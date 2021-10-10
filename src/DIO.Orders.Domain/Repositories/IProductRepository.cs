using DIO.Orders.Domain.Models;

namespace DIO.Orders.Domain.Repositories
{
    /// <summary>
    /// The <see cref="Product"/> repository.
    /// </summary>
    /// <inheritdoc cref="IRepository{T}"/>
    public interface IProductRepository : IRepository<Product> { }
}
