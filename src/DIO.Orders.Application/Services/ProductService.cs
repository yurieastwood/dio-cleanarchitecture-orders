using DIO.Orders.Domain.Enums;
using DIO.Orders.Domain.Models;
using DIO.Orders.Domain.Repositories;
using DIO.Orders.Domain.Services;

namespace DIO.Orders.Application.Services
{
    /// <inheritdoc cref="IProductService"/>
    /// <inheritdoc cref="ServiceStorableBase{T}"/>
    public class ProductService : ServiceStorableBase<Product>, IProductService
    {
        /// <summary>
        /// Initialize an instance of <see cref="IProductService"/>.
        /// </summary>
        /// <param name="productRepository">An instance of <see cref="IProductRepository"/>.</param>
        public ProductService(IProductRepository productRepository) : base(productRepository) { }

        /// <inheritdoc cref="IPromotionService"/>
        /// <inheritdoc cref="ServiceStorableBase{Promotion}"/>
        public override int Add(Product product) => product.Id != null ? (int)ResultCodeType.InvalidProduct : base.Add(product);

        /// <inheritdoc cref="IPromotionService"/>
        /// <inheritdoc cref="ServiceStorableBase{Promotion}"/>
        public override bool Update(Product product) => (product.Id ?? 0) > 0 && base.Update(product);
    }
}
