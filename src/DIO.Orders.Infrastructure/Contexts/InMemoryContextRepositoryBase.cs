using System;
using System.Collections.Generic;
using System.Linq;
using DIO.Orders.Domain.Models;
using DIO.Orders.Domain.Repositories;

namespace DIO.Orders.Infrastructure.Contexts
{
    /// <summary>
    /// Represents an abstract class that provides an implementation of <see cref="IRepository{T}"/> using memory context.
    /// </summary>
    /// <typeparam name="T">The type of repository.</typeparam>
    /// <inheritdoc cref="IRepository{T}"/>
    public abstract class InMemoryContextRepositoryBase<T> : IRepository<T> where T : IStorable
    {
        /// <summary>
        /// The internal counter to controls the identifier of the item.
        /// It will starts from one (1) and will never be decremented.
        ///
        /// The ReSharper warning was disabled mainly because we really want that the static field below creates one different instance for each generic type.
        /// </summary>
        // ReSharper disable once StaticMemberInGenericType
        private static int _counter;

        /// <summary>
        /// The <see cref="Product"/> memory database.
        /// </summary>
        private static readonly Dictionary<int, T> Repository = new();

        /// <inheritdoc cref="IRepository{T}"/>
        public virtual int AddOrUpdate(T value)
        {
            var key= value.Id ??= ++_counter;
            if (Repository.ContainsKey(key))
                Repository[key] = value;
            else
                Repository.Add(key, value);

            return key;
        }

        /// <inheritdoc cref="IRepository{T}"/>
        public virtual int Delete(Func<T, bool> filter)
        {
            var toDelete = Repository.Values.Where(filter).Select(stored => stored.Id ?? 0).ToList();
            if (toDelete.Any())
                toDelete.ForEach(Delete);

            return toDelete.Count;
        }

        /// <inheritdoc cref="IRepository{T}"/>
        public int Count() => Repository.Count;

        /// <inheritdoc cref="IRepository{T}"/>
        public virtual IEnumerable<T> Get() => Repository.Values;

        /// <inheritdoc cref="IRepository{T}"/>
        public virtual IEnumerable<T> Get(Func<T, bool> filter) => Repository.Values.Where(filter);

        /// <summary>
        /// Remove an instance of <see cref="T"/> based on the given key identifier.
        /// </summary>
        /// <param name="key">The identifier of the <see cref="T"/> instance that should be removed.</param>
        private void Delete(int key) => Repository.Remove(key);
    }
}
