using DIO.Orders.Domain.Models;
using DIO.Orders.Domain.Repositories;

namespace DIO.Orders.Domain.Enums
{
    /// <summary>
    /// Keep the error types that can happen in the validation process when try to manipulate <see cref="Order"/>s object.
    /// </summary>
    public enum ResultCodeType
    {
        /// <summary>
        /// When it was not possible to determine the problem happened.
        /// </summary>
        Undefined = -999,

        /// <summary>
        /// When one of the <see cref="Product"/> sent is invalid.
        /// </summary>
        InvalidProduct = -1,

        /// <summary>
        /// When one of the <see cref="Customer"/> sent is invalid.
        /// </summary>
        InvalidCustomer = -2,

        /// <summary>
        /// When the <see cref="Promotion"/> that we are trying to apply is invalid.
        /// </summary>
        InvalidPromotion = -3,

        /// <summary>
        /// When the <see cref="IStorable"/> item was not created for any reason.
        /// </summary>
        NotCreated = -4
    }
}
