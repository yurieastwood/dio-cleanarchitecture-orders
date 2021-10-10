using System;
using System.Collections.Generic;
using System.Linq;
using DIO.Orders.Application.Filters;
using DIO.Orders.Domain.Repositories;

namespace DIO.Orders.Application.Extensions
{
    /// <summary>
    /// Provide extensions methods for <see cref="IRepository{T}"/> objects
    /// </summary>
    public static class RepositoryExtensions
    {
        /// <summary>
        /// Check if an item is valid searching for the identifier in the repository.
        /// </summary>
        /// <typeparam name="T">Any type that implements <see cref="IStorable"/>.</typeparam>
        /// <param name="repository">An <see cref="IRepository{T}"/> instance to search.</param>
        /// <param name="id">The item identifier to be searched.</param>
        /// <returns>True if the item exists in the repository.</returns>
        public static bool Contains<T>(this IRepository<T> repository, int id) where T : IStorable =>
            id > 0 && repository.Get(storedItem => storedItem.IsId(id)).Any();

        /// <summary>
        /// Check if all identifiers in a list exists in the repository.
        /// </summary>
        /// <typeparam name="T">Any type that implements <see cref="IStorable"/>.</typeparam>
        /// <param name="repository">An <see cref="IRepository{T}"/> instance to search.</param>
        /// <param name="ids">A <see cref="List{T}"/> of identifiers to be searched.</param>
        /// <returns>True if all the items exists in the repository.</returns>
        public static bool ContainsAll<T>(this IRepository<T> repository, IReadOnlyCollection<int> ids) where T : IStorable =>
            ids.All(id => id > 0) && repository.Get(storedItem => ids.Contains(storedItem.Id ?? 0))?.Count() == ids.Count;

        /// <summary>
        /// Try to get an storable item based on the given identifier.
        /// </summary>
        /// <typeparam name="T">Any type that implements <see cref="IStorable"/>.</typeparam>
        /// <param name="repository">An <see cref="IRepository{T}"/> instance to search.</param>
        /// <param name="filter">The filter function to search the <see cref="IStorable"/> item.</param>
        /// <param name="result">The result of the search operation.</param>
        /// <returns>True when found any item that matched with the given filter.</returns>
        public static bool TryGetAll<T>(this IRepository<T> repository, Func<T, bool> filter, out List<T> result) where T : IStorable
        {
            result = default;
            try
            {
                result = repository.Get(filter).ToList();
                return result.Any();
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Try to get the first <see cref="IStorable"/> based on the results of the repository filter.
        /// </summary>
        /// <typeparam name="T">Any type that implements <see cref="IStorable"/>.</typeparam>
        /// <param name="repository">An <see cref="IRepository{T}"/> instance to search.</param>
        /// <param name="filter">The filter function to search the <see cref="IStorable"/> item.</param>
        /// <param name="result">An instance of <see cref="IStorable"/> item or null, that means the result of the search operation.</param>
        /// <returns>True when found at least one item that matched with the given filter.</returns>
        public static bool TryGetFirst<T>(this IRepository<T> repository, Func<T, bool> filter, out T result) where T : IStorable
        {
            result = default;
            try
            {
                result = repository.Get(filter).FirstOrDefault();
                return result is not null;
            }
            catch
            {
                return false;
            }
        }
    }
}
