using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using DIO.Orders.Domain.Repositories;

namespace DIO.Orders.Domain.Models
{
    /// <summary>
    /// Encapsulate the information related to the products.
    /// </summary>
    public class Product : IStorable
    {
        /// <summary>
        /// The <see cref="Product"/> identifier.
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// The name of the product.
        /// </summary>
        [NotNull]
        public string Name { get; set; }

        /// <summary>
        /// The value of the product.
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "Invalid value for product")]
        public double Value { get; set; }

        /// <summary>
        /// Creates an instance of <see cref="Product"/>.
        /// </summary>
        /// <param name="name">The name of the product.</param>
        /// <param name="value">The value that the product can be sold.</param>
        public Product([NotNull] string name, double value)
        {
            Id = null;
            Name = name;
            Value = value;
        }

        /// <inheritdoc />
        public override string ToString() => $"Product{{{Id ?? 0}}} => {new {Name, Value}}";
    }
}
