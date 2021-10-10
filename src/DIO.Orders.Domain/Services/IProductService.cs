using DIO.Orders.Domain.Models;

namespace DIO.Orders.Domain.Services
{
    /// <summary>
    /// Encapsulate the methods to manipulate <see cref="Product"/> instances.
    /// </summary>
    public interface IProductService : IStorableService<Product> { }
}
