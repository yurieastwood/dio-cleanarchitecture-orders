using DIO.Orders.Domain.Models;
using DIO.Orders.Domain.Repositories;
using DIO.Orders.Infrastructure.Contexts;

namespace DIO.Orders.Infrastructure.Repositories
{
    /// <inheritdoc cref="IPromotionRepository"/>
    /// <inheritdoc cref="InMemoryContextRepositoryBase{T}"/>
    public class PromotionRepository : InMemoryContextRepositoryBase<Promotion>, IPromotionRepository { }
}
