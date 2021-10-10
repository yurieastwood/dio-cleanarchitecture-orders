using DIO.Orders.Domain.Models;

namespace DIO.Orders.Domain.Services
{
    /// <summary>
    /// Encapsulate the methods to manipulate <see cref="Customer"/> instances.
    /// </summary>
    public interface ICustomerService : IStorableService<Customer> { }
}