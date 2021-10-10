using System.Collections.Generic;
using System.Linq;
using DIO.Orders.Application.Filters;
using DIO.Orders.Domain.Models;
using DIO.Orders.Domain.Repositories;
using DIO.Orders.Domain.Services;

namespace DIO.Orders.Application.Services
{
    /// <summary>
    /// Provides a base class for storable services which implements the storable functions.
    /// </summary>
    /// <typeparam name="T">A type of any object that implements <see cref="IStorable"/></typeparam>
    public abstract class ServiceStorableBase<T> : IStorableService<T> where T : IStorable
    {
        /// <summary>
        /// An instance of <see cref="IRepository{T}"/> that contains the methods exposed to manipulate the <see cref="IStorable"/> instance repository.
        /// </summary>
        protected readonly IRepository<T> Repository;

        /// <summary>
        /// Initialize an instance of <see cref="ServiceStorableBase{T}"/>.
        /// </summary>
        /// <param name="repository">An instance of <see cref="IRepository{T}"/>.</param>
        protected ServiceStorableBase(IRepository<T> repository) => Repository = repository;

        /// <inheritdoc />
        public virtual int Add(T storableItem) => Repository.AddOrUpdate(storableItem);

        /// <inheritdoc />
        public virtual bool Update(T storableItem) => Repository.AddOrUpdate(storableItem) == storableItem.Id;

        /// <inheritdoc />
        public virtual IEnumerable<T> Get() => Repository.Get();

        /// <inheritdoc />
        public virtual T Get(int id) => GetById(id);

        /// <inheritdoc />
        public virtual string Print(int id) => GetById(id)?.ToString() ?? $"{typeof(T).Name} does not exist in the repository!";

        /// <inheritdoc />
        public virtual bool Delete(int id) => Repository.Delete(customer => customer.IsId(id)) == 1;

        /// <summary>
        /// Filter the repository to select a <see cref="Customer"/> using the identifier.
        /// </summary>
        /// <param name="id">The <see cref="Customer"/> identifier to search.</param>
        /// <returns>The <see cref="Customer"/> that matched with the identifier requested.</returns>
        protected T GetById(int id) => Repository.Get(customer => customer.IsId(id)).FirstOrDefault();
    }
}
