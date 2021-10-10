using System.Diagnostics.CodeAnalysis;
using DIO.Orders.Domain.Repositories;

namespace DIO.Orders.Domain.Models
{
    /// <summary>
    /// Encapsulate the information regarding the customers.
    /// </summary>
    public class Customer : IStorable
    {
        /// <summary>
        /// The <see cref="Customer"/> identifier.
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// The name of the customer.
        /// </summary>
        [NotNull]
        public string Name { get; }

        /// <summary>
        /// Creates an instance of <see cref="Customer"/>.
        /// </summary>
        /// <param name="name">The name of the customer.</param>
        public Customer([NotNull] string name)
        {
            Id = null;
            Name = name;
        }

        /// <inheritdoc />
        public override string ToString() => $"Customer{{{Id ?? 0}}} => {new { Name }}";
    }
}
