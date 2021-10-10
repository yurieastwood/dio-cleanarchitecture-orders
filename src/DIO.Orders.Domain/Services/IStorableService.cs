using System.Collections.Generic;
using DIO.Orders.Domain.Repositories;

namespace DIO.Orders.Domain.Services
{
    /// <summary>
    /// Encapsulate the methods to manipulate <see cref="IStorable"/> instances.
    /// </summary>
    public interface IStorableService<T> where T : IStorable
    {
        /// <summary>
        /// Insert the given <see cref="T"/> into <see cref="T"/> repository.
        /// </summary>
        /// <param name="T">The <see cref="T"/> instance to be added or updated.</param>
        /// <returns>The new identifier created.</returns>
        int Add(T T);

        /// <summary>
        /// Update the given <see cref="T"/> in the <see cref="T"/> repository.
        /// </summary>
        /// <param name="T">The <see cref="T"/> instance to be added or updated.</param>
        /// <returns>True when the item was updated successfully.</returns>
        bool Update(T T);

        /// <summary>
        /// Select all <see cref="T"/> stored.
        /// </summary>
        /// <returns>All <see cref="T"/>s from repository.</returns>
        IEnumerable<T> Get();

        /// <summary>
        /// Retrieve the <see cref="T"/> from repository based on the given id.
        /// </summary>
        /// <param name="id">The <see cref="T"/> identifier to be retrieved.</param>
        /// <returns>The <see cref="T"/> item retrieved from repository.</returns>
        T Get(int id);

        /// <summary>
        /// Retrieve the <see cref="T"/> from repository based on the given id and transform into a printable mode.
        /// </summary>
        /// <param name="id">The <see cref="T"/> identifier to be retrieved in the printable mode.</param>
        /// <returns>A text representing the <see cref="T"/> item retrieved from repository.</returns>
        string Print(int id);

        /// <summary>
        /// Remove a <see cref="T"/> instance from repository.
        /// </summary>
        /// <param name="id">The <see cref="T"/> identifier to be removed.</param>
        /// <returns>True when the <see cref="T"/> was removed successfully.</returns>
        bool Delete(int id);
    }
}
