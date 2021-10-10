using System.Diagnostics.CodeAnalysis;
using System.Linq;
using DIO.Orders.Domain.Enums;
using DIO.Orders.Domain.Models;
using DIO.Orders.Domain.Services;

namespace DIO.Orders.Application.Initializers
{
    /// <summary>
    /// Initializer the database with some pre-defined <see cref="Product"/>, <see cref="Customer"/> and <see cref="Promotion"/>.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class DataBaseInitializer
    {
        private readonly IProductService _productService;
        private readonly ICustomerService _customerService;
        private readonly IPromotionService _promotionService;

        /// <summary>
        /// Initialize an instance of <see cref="DataBaseInitializer"/>.
        /// </summary>
        /// <param name="productService">An instance of <see cref="IProductService"/>.</param>
        /// <param name="customerService">An instance of <see cref="ICustomerService"/>.</param>
        /// <param name="promotionService">An instance of <see cref="IPromotionService"/>.</param>
        public DataBaseInitializer(IProductService productService, ICustomerService customerService, IPromotionService promotionService) =>
            (_productService, _customerService, _promotionService) = (productService, customerService, promotionService);

        /// <summary>
        /// Fill the repository with pre-defined objects
        /// </summary>
        public void Seed()
        {
            CustomersLoader();
            ProductsLoader();
            PromotionsLoader();
        }

        /// <summary>
        /// Load initial <see cref="Customer"/>.
        /// </summary>
        /// <param name="quantity">Quantity of <see cref="Customer"/> to include in the repository.</param>
        private void CustomersLoader(int quantity = 1) =>
            Enumerable.Range(1, quantity).ToList().ForEach(index => _customerService.Add(new Customer($"Customer-{index}")));

        /// <summary>
        /// Load initial <see cref="Product"/>.
        /// </summary>
        /// <param name="quantity">Quantity of <see cref="Product"/> to include in the repository.</param>
        private void ProductsLoader(int quantity = 3) =>
            Enumerable.Range(1, quantity).ToList().ForEach(index => _productService.Add(new Product($"Product-{index}", index * 10)));

        /// <summary>
        /// Load initial <see cref="Promotion"/>.
        /// </summary>
        /// <param name="quantity">Quantity of <see cref="Promotion"/> to include in the repository.</param>
        private void PromotionsLoader(int quantity = 2) =>
            Enumerable.Range(1, quantity).ToList().ForEach(index =>
            {
                _promotionService.Add(index % 2 == 0
                    ? new Promotion(PromotionType.Product, index, index * 2, $"{index * 2}% of discount on product code {index}")
                    : new Promotion(PromotionType.Order, null, index * 5, $"{index * 5}% of discount above total")
                );
            });
    }
}
