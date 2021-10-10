using DIO.Orders.Domain.Repositories;

namespace DIO.Orders.Application.Filters
{
    /// <summary>
    /// Keep the common filter for storable objects.
    /// </summary>
    public static class Filters
    {
        /// <summary>
        /// Filter a <see cref="IStorable"/> item by id.
        /// </summary>
        /// <typeparam name="T">Any type that implements <see cref="IStorable"/>.</typeparam>
        /// <param name="storedItem">The item that will be checked.</param>
        /// <param name="id">The id to filter.</param>
        /// <returns>True when match with the identifier requested.</returns>
        public static bool IsId<T>(this T storedItem, int id) where T : IStorable => storedItem.Id == id;
    }
}
