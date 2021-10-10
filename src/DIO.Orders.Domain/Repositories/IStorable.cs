using System.ComponentModel;

namespace DIO.Orders.Domain.Repositories
{
    /// <summary>
    /// Encapsulate the mandatory information for all object that can be stored.
    /// </summary>
    public interface IStorable
    {
        /// <summary>
        /// The instance identifier.
        /// </summary>
        [DefaultValue(null)]
        int? Id { get; set; }
    }
}
