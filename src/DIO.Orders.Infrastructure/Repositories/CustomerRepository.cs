using DIO.Orders.Domain.Models;
using DIO.Orders.Domain.Repositories;
using DIO.Orders.Infrastructure.Contexts;

namespace DIO.Orders.Infrastructure.Repositories
{
    /// <inheritdoc cref="ICustomerRepository"/>
    /// <inheritdoc cref="InMemoryContextRepositoryBase{T}"/>
    public class CustomerRepository : InMemoryContextRepositoryBase<Customer>, ICustomerRepository { }
}
