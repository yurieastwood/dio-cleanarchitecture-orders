using System;
using System.Collections.Generic;

namespace DIO.Orders.Domain.Repositories
{
    /// <summary>
    /// Encapsulate the common methods to manipulate the repository of the <see cref="T"/> items.
    /// </summary>
    public interface IRepository<T> where T : IStorable
    {
        /// <summary>
        /// Insert a new item <see cref="T"/> in the repository when it is not exists, otherwise update it.
        /// </summary>
        /// <param name="value">The <see cref="T"/> instance to be added or updated.</param>
        /// <returns>The identifier of the <see cref="T"/> item added or updated.</returns>
        int AddOrUpdate(T value);

        /// <summary>
        /// Select all <see cref="T"/> instances stored in repository.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> object with all <see cref="T"/> instances from repository.</returns>
        IEnumerable<T> Get();

        /// <summary>
        /// Search in the repository all the items that matches with the given filter function.
        /// </summary>
        /// <param name="filter">The filter function to search items in the repository.</param>
        /// <returns>The <see cref="IEnumerable{T}"/> of <see cref="T"/> that matched with the given filter.</returns>
        IEnumerable<T> Get(Func<T, bool> filter);

        /// <summary>
        /// Search in the repository all the items that matches with the given filter function and remove them.
        /// </summary>
        /// <param name="filter">The filter function to search items in the repository to delete.</param>
        /// <returns>The quantity of <see cref="T"/> removed based on the given filter.</returns>
        int Delete(Func<T, bool> filter);

        /// <summary>
        /// Retrieve the quantity of items in the repository.
        /// </summary>
        /// <returns>The quantity of items in the repository.</returns>
        int Count();
    }
}
