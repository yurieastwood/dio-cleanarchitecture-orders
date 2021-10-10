namespace DIO.Orders.Domain.Enums
{
    /// <summary>
    /// The available types for <see cref="Models.Promotion"/> object.
    /// </summary>
    public enum PromotionType
    {
        /// <summary>
        /// When the promotion is applicable only for <see cref="Models.Product"/> objects.
        /// </summary>
        Product,

        /// <summary>
        /// When the promotion is applicable in the total amount of the <see cref="Models.Order"/> object.
        /// </summary>
        Order
    }
}
