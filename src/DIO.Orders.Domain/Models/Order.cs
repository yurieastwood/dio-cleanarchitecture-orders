using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using DIO.Orders.Domain.Enums;
using DIO.Orders.Domain.Repositories;

namespace DIO.Orders.Domain.Models
{
    /// <summary>
    /// Keep the infos related with the products sold.
    /// </summary>
    public class Order : IStorable
    {
        /// <summary>
        /// The <see cref="Order"/> identifier.
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// The <see cref="List{T}"/> of <see cref="Product"/> selected.
        /// </summary>
        public List<Product> Products { get; }

        /// <summary>
        /// The <see cref="Customer"/> that requested the order.
        /// </summary>
        [NotNull]
        public Customer Customer { get; }

        /// <summary>
        /// The total amount of the order that consists in the sum of all <see cref="Product"/>s value subtracted the discount value if any <see cref="Promotion"/> was applied.
        /// </summary>
        public double Total => _totalWithoutDiscount - _amountOfDiscount;

        /// <summary>
        /// Add the <see cref="Promotion"/> object to the order.
        /// </summary>
        public Promotion Promotion { get; set; }

        /// <summary>
        /// The amount of discount that can be over the product or in the entire order value.
        /// </summary>
        private double _amountOfDiscount = 0;

        /// <summary>
        /// The total amount without apply any discount
        /// </summary>
        private double _totalWithoutDiscount;

        /// <summary>
        /// Create an instance of <see cref="Order"/>.
        /// </summary>
        public Order([NotNull] List<Product> products, [NotNull] Customer customer, Promotion promotion = null)
        {
            if(!products.Any()) throw new ArgumentNullException(nameof(products), "Must contains at least one valid product!");

            Id = null;
            Customer = customer;
            Products = products;

            CalculateTotals();
            ApplyPromotion(promotion);
        }

        /// <summary>
        /// Include <see cref="Product"/>s into order.
        /// </summary>
        /// <param name="products">The <see cref="List{T}"/> of <see cref="Product"/> to be added.</param>
        public void AddProducts(List<Product> products)
        {
            var productsToAdd = products.Where(product => (product.Id ?? 0) > 0).ToList();
            if (!productsToAdd.Any()) return;

            Products.AddRange(productsToAdd);
            CalculateTotals();
        }

        /// <summary>
        /// Remove a <see cref="Product"/> from the products list based on the given product id.
        /// </summary>
        /// <param name="productIds">The <see cref="List{T}"/> of <see cref="Product"/> identifiers that should be removed.</param>
        /// <returns>The quantity of <see cref="Product"/>s removed.</returns>
        public int RemoveProducts(List<int> productIds)
        {
            var productsIdsToRemove = productIds.Where(id => id > 0).ToList();
            if (!productsIdsToRemove.Any()) return 0;

            var productsDeleted = 0;
            Products
                .Where(product => productsIdsToRemove.Contains(product.Id ?? -1))
                .ToList()
                .ForEach(product =>
                {
                    Products.Remove(product);
                    productsDeleted++;
                });

            CalculateTotals();
            return productsDeleted;
        }

        /// <summary>
        /// Apply the <see cref="Promotion"/> into the order.
        /// </summary>
        /// <param name="promotion">The <see cref="Promotion"/> to be applied.</param>
        /// <returns>The discount amount applied into the <see cref="Order"/>.</returns>
        public double ApplyPromotion(Promotion promotion)
        {
            if (promotion == null) return 0;

            Promotion = promotion;
            CalculateTotals();

            return _amountOfDiscount;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendLine($"Order{{{Id ?? 0}}}");
            builder.AppendLine($"\tClient: {Customer.Name.ToUpper()}");
            builder.AppendLine("\tProducts:");
            builder.AppendLine();
            Products.ForEach(product => builder.AppendLine($"\t\t{product.Id}\t{product.Name.ToUpper()}\t{product.Value:C}{(IsProductPromoted(product.Id ?? -1) ? "P" : string.Empty)}"));
            builder.AppendLine();
            builder.AppendLine($"\tDiscount: {_amountOfDiscount:C}");
            builder.AppendLine($"\tTotal: {Total:C}");

            return builder.ToString();
        }

        /// <summary>
        /// Updates the internal property with the full total without discounts.
        /// Calculates the amount of discount based on the <see cref="Promotion"/> object.
        /// </summary>
        private void CalculateTotals()
        {
            _amountOfDiscount = 0;
            _totalWithoutDiscount = Products.Sum(prd => prd.Value);

            if (Promotion == null) return;

            if (Promotion.Type == PromotionType.Order)
            {
                _amountOfDiscount = _totalWithoutDiscount * ((Promotion?.DiscountPercentage ?? 0) / 100D);
                return;
            }

            var productPromotionTargetId = Products.FirstOrDefault(product => (product.Id ?? 0) > 0 && product.Id == Promotion.TargetId);
            if (productPromotionTargetId == null) return;

            _amountOfDiscount = productPromotionTargetId.Value * ((Promotion?.DiscountPercentage ?? 0) / 100D);
        }

        /// <summary>
        /// Check if the promotion was applied for the given <see cref="Product"/>
        /// </summary>
        /// <returns>True if the type of <see cref="Promotion"/> is <see cref="PromotionType.Product"/> and the target id is equals to the given product identifier.</returns>
        private bool IsProductPromoted(int productId) => 
            Promotion is {Type: PromotionType.Product} && (Promotion?.TargetId ?? 0) > 0 && Promotion.TargetId == productId;
    }
}
