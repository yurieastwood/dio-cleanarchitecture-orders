using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using DIO.Orders.Domain.Enums;
using DIO.Orders.Domain.Repositories;

namespace DIO.Orders.Domain.Models
{
    /// <summary>
    /// Encapsulate the information regarding the offers that can be apply into the orders.
    /// </summary>
    public class Promotion : IStorable
    {
        /// <summary>
        /// The <see cref="Promotion"/> identifier.
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// A <see cref="PromotionType"/> object that indicates where the promotion discount should be applied.
        /// </summary>
        public PromotionType Type { get; }

        /// <summary>
        /// Keep the identifier of the product when the type is <see cref="PromotionType.Product"/> otherwise will be null because the discount will be in the total amount of the <see cref="Order"/>.
        /// </summary>
        public int? TargetId { get; }

        /// <summary>
        /// The percent value that should be discounted in the order total value or in the product value.
        /// </summary>
        [Range(1, 100, ErrorMessage = "Only allowed values in the interval 1 ~ 100.")]
        public int DiscountPercentage { get; }

        /// <summary>
        /// A short description for the <see cref="Promotion"/>
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Creates an instance of <see cref="Promotion"/>
        /// </summary>
        public Promotion(PromotionType type, int? targetId, [NotNull] int discountPercentage, string description)
        {
            if (type == PromotionType.Product && (!targetId.HasValue || (targetId ?? 0) <= 0)) throw new ArgumentNullException(nameof(targetId), $"Must be set for promotions of type {PromotionType.Product}!");
            if (discountPercentage is <= 0 or > 100) throw new ArgumentOutOfRangeException(nameof(discountPercentage), "Invalid amount of discount. Must be between 1 and 100!");

            Id = null;
            Type = type;
            TargetId = targetId ?? 0;
            DiscountPercentage = discountPercentage;
            Description = description;
        }

        /// <inheritdoc />
        public override string ToString() => 
            (TargetId ?? 0) > 0
                ? $"Promotion{{{Id ?? 0}}} [{Type.ToString().ToUpper()}] => {new { Product = TargetId.Value, DiscountPercentage }}"
                : $"Promotion{{{Id ?? 0}}} [{Type.ToString().ToUpper()}] => {new { DiscountPercentage }}";
    }
}
