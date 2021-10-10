using DIO.Orders.Domain.Models;

namespace DIO.Orders.Domain.Services
{
    /// <summary>
    /// Encapsulate the methods to manipulate <see cref="Promotion"/> instances.
    /// </summary>
    public interface IPromotionService : IStorableService<Promotion> { }
}