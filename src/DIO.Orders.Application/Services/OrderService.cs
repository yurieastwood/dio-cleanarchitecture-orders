using System.Collections.Generic;
using System.Linq;
using DIO.Orders.Application.Extensions;
using DIO.Orders.Application.Filters;
using DIO.Orders.Domain.Enums;
using DIO.Orders.Domain.Models;
using DIO.Orders.Domain.Repositories;
using DIO.Orders.Domain.Services;

namespace DIO.Orders.Application.Services
{
    /// <inheritdoc cref="IOrderService"/>
    /// <inheritdoc cref="ServiceStorableBase{T}"/>
    public class OrderService : ServiceStorableBase<Order>, IOrderService
    {
        /// <summary>
        /// An instance of <see cref="IProductRepository"/>.
        /// </summary>
        private readonly IProductRepository _productRepository;

        /// <summary>
        /// An instance of <see cref="ICustomerRepository"/>.
        /// </summary>
        private readonly ICustomerRepository _customerRepository;

        /// <summary>
        /// An instance of <see cref="IPromotionRepository"/>.
        /// </summary>
        private readonly IPromotionRepository _promotionRepository;

        /// <summary>
        /// Initialize an instance of <see cref="IOrderService"/>.
        /// </summary>
        /// <param name="orderRepository">An instance of <see cref="IOrderRepository"/>.</param>
        /// <param name="productRepository">An instance of <see cref="IProductRepository"/>.</param>
        /// <param name="customerRepository">An instance of <see cref="ICustomerRepository"/>.</param>
        /// <param name="promotionRepository">An instance of <see cref="IPromotionRepository"/>.</param>
        public OrderService(IOrderRepository orderRepository, IProductRepository productRepository,
            ICustomerRepository customerRepository, IPromotionRepository promotionRepository) : base(orderRepository) =>
            (_productRepository, _customerRepository, _promotionRepository) = (productRepository, customerRepository, promotionRepository);

        /// <inheritdoc />
        public override int Add(Order storableItem)
        {
            if (!IsAllProductsValid(storableItem.Products)) return (int) ResultCodeType.InvalidProduct;
            if (!IsCustomerValid(storableItem.Customer)) return (int) ResultCodeType.InvalidCustomer;
            if (!IsPromotionValid(storableItem.Promotion)) return (int) ResultCodeType.InvalidPromotion;

            return base.Add(storableItem);
        }

        /// <inheritdoc />
        public override bool Update(Order storableItem) => 
            IsInstanceValid(storableItem) && base.Update(storableItem);

        /// <inheritdoc />
        public int CreateOrder(int customerId, List<int> productIds)
        {
            if(!_productRepository.TryGetAll(storedItem => productIds.Contains(storedItem.Id ?? 0), out var products)) return -1;
            if (!_customerRepository.TryGetFirst(storedItem => storedItem.IsId(customerId), out var customer)) return -2;

            var order = new Order(products, customer);
            return base.Add(order);
        }

        /// <inheritdoc />
        public int RemoveProductsFrom(int orderId)
        {
            if (!Repository.TryGetFirst(storedItem => storedItem.IsId(orderId), out var order)) return 0;

            var productsRemoved = order.RemoveProducts();

            return Repository.AddOrUpdate(order) == orderId
                    ? productsRemoved
                    : 0;
        }
        public int RemoveProductFrom(int orderId, int productId)
        {
            if (!_productRepository.Contains(productId)) return 0;
            if (!Repository.TryGetFirst(storedItem => storedItem.IsId(orderId), out var order)) return 0;

            var productsRemoved = order.RemoveProduct(productId);

            return Repository.AddOrUpdate(order) == orderId
                    ? productsRemoved
                    : 0;
        }

        /// <inheritdoc />
        public bool AddProductsTo(int orderId, List<int> productIds)
        {
            if (!_productRepository.TryGetAll(storedItem => productIds.Contains(storedItem.Id ?? 0), out var products)) return false;
            if (!Repository.TryGetFirst(storedItem => storedItem.IsId(orderId), out var order)) return false;

            order.AddProducts(products);

            return Repository.AddOrUpdate(order) == orderId;
        }

        /// <inheritdoc />
        public double ApplyPromotionTo(int orderId, int promotionId)
        {
            if (!_promotionRepository.TryGetFirst(storedItem => storedItem.IsId(promotionId), out var promotion)) return 0;
            if (!Repository.TryGetFirst(storedItem => storedItem.IsId(orderId), out var order)) return 0;

            var discountAmount = order.ApplyPromotion(promotion);
            return Repository.AddOrUpdate(order) == orderId
                ? discountAmount
                : 0;
        }

        #region Validations

        /// <summary>
        /// Validate an instance of <see cref="Order"/> to check if we can update the item.
        /// </summary>
        /// <param name="order">An instance of <see cref="Order"/> to be validated if can be updated.</param>
        /// <returns>True when the instance is valid.</returns>
        private bool IsInstanceValid(Order order) =>
            IsAllProductsValid(order.Products) && IsCustomerValid(order.Customer) && IsPromotionValid(order.Promotion);

        /// <summary>
        /// Validate the <see cref="List{T}"/> of <see cref="Product"/> sent.
        /// </summary>
        /// <param name="products">A <see cref="List{T}"/> of <see cref="Product"/> to create an <see cref="Order"/> instance.</param>
        /// <returns>True when the list is valid.</returns>
        private bool IsAllProductsValid(IEnumerable<Product> products) =>
            _productRepository.ContainsAll(products.Select(product => product.Id ?? 0).ToList());

        /// <summary>
        /// Validate the <see cref="Customer"/> sent.
        /// </summary>
        /// <param name="customer">A <see cref="Customer"/> instance to create an <see cref="Order"/> instance.</param>
        /// <returns>True when the instance is valid.</returns>
        private bool IsCustomerValid(IStorable customer) => _customerRepository.Contains(customer.Id ?? 0);

        /// <summary>
        /// Validate the <see cref="Promotion"/> sent.
        /// </summary>
        /// <param name="promotion">A <see cref="Promotion"/> instance to create an <see cref="Order"/> instance.</param>
        /// <returns>True when the instance is valid.</returns>
        private bool IsPromotionValid(IStorable promotion) => promotion is null || _promotionRepository.Contains(promotion.Id ?? 0);

        #endregion
    }
}
