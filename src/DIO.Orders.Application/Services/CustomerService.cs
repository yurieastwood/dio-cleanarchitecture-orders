using DIO.Orders.Domain.Enums;
using DIO.Orders.Domain.Models;
using DIO.Orders.Domain.Repositories;
using DIO.Orders.Domain.Services;

namespace DIO.Orders.Application.Services
{
    /// <inheritdoc cref="ICustomerService"/>
    /// <inheritdoc cref="ServiceStorableBase{T}"/>
    public class CustomerService : ServiceStorableBase<Customer>, ICustomerService
    {
        /// <summary>
        /// Initialize an instance of <see cref="ICustomerService"/>.
        /// </summary>
        /// <param name="customerRepository">An instance of <see cref="ICustomerRepository"/>.</param>
        public CustomerService(ICustomerRepository customerRepository) : base(customerRepository) { }

        /// <inheritdoc cref="IPromotionService"/>
        /// <inheritdoc cref="ServiceStorableBase{Promotion}"/>
        public override int Add(Customer customer) => customer.Id != null ? (int)ResultCodeType.InvalidCustomer : base.Add(customer);

        /// <inheritdoc cref="IPromotionService"/>
        /// <inheritdoc cref="ServiceStorableBase{Promotion}"/>
        public override bool Update(Customer customer) => (customer.Id ?? 0) > 0 && base.Update(customer);
    }
}