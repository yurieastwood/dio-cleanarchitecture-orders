using System.Collections.Generic;
using DIO.Orders.Domain.Models;

namespace DIO.Orders.Domain.Services
{
    /// <summary>
    /// Encapsulate the methods to manipulate <see cref="Order"/> instances.
    /// </summary>
    public interface IOrderService : IStorableService<Order>
    {
        /// <summary>
        /// Create an instance of <see cref="Order"/>.
        /// </summary>
        /// <param name="customerId">The <see cref="Customer"/> identifier.</param>
        /// <param name="productIds">The <see cref="List{T}"/> of <see cref="Product"/> to start the <see cref="Order"/></param>
        /// <returns>The <see cref="Order"/> identifier created.</returns>
        int CreateOrder(int customerId, List<int> productIds);

        /// <summary>
        /// Remove the products from a given <see cref="Order"/>.
        /// </summary>
        /// <param name="orderId">The <see cref="Order"/> identifier to remove the <see cref="Product"/>s.</param>
        /// <param name="productIds">The <see cref="Product"/> identifiers to be removed.</param>
        /// <returns>The number of <see cref="Product"/>s removed.</returns>
        int RemoveProductsFrom(int orderId, List<int> productIds);

        /// <summary>
        /// Include new products into a given <see cref="Order"/>.
        /// </summary>
        /// <param name="orderId">The <see cref="Order"/> identifier to include the <see cref="Product"/>s.</param>
        /// <param name="productIds">Tje <see cref="List{T}"/> of <see cref="Product"/> identifiers to be included.</param>
        bool AddProductsTo(int orderId, List<int> productIds);

        /// <summary>
        /// Apply one <see cref="Promotion"/> into a given <see cref="Order"/>
        /// </summary>
        /// <param name="orderId">The <see cref="Order"/> identifier to apply the promotion.</param>
        /// <param name="promotionId">The <see cref="Promotion"/> identifier to be applied.</param>
        /// <returns>The discount amount applied by the <see cref="Promotion"/>.</returns>
        double ApplyPromotionTo(int orderId, int promotionId);
    }
}
