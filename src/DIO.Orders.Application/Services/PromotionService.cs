using DIO.Orders.Domain.Enums;
using DIO.Orders.Domain.Models;
using DIO.Orders.Domain.Repositories;
using DIO.Orders.Domain.Services;

namespace DIO.Orders.Application.Services
{
    /// <inheritdoc cref="IPromotionService"/>
    /// <inheritdoc cref="ServiceStorableBase{T}"/>
    public class PromotionService : ServiceStorableBase<Promotion>, IPromotionService
    {
        /// <summary>
        /// Initialize an instance of <see cref="IPromotionService"/>.
        /// </summary>
        /// <param name="promotionRepository">An instance of <see cref="IPromotionRepository"/>.</param>
        public PromotionService(IPromotionRepository promotionRepository) : base(promotionRepository) { }

        /// <inheritdoc cref="IPromotionService"/>
        /// <inheritdoc cref="ServiceStorableBase{Promotion}"/>
        public override int Add(Promotion promotion) => promotion.Id != null ? (int)ResultCodeType.InvalidPromotion : base.Add(promotion);

        /// <inheritdoc cref="IPromotionService"/>
        /// <inheritdoc cref="ServiceStorableBase{Promotion}"/>
        public override bool Update(Promotion promotion) => (promotion.Id ?? 0) > 0 && base.Update(promotion);
    }
}