using System;
using System.Collections.Generic;
using DIO.Orders.Application.Extensions;
using DIO.Orders.Domain.Enums;
using DIO.Orders.Domain.Models;
using DIO.Orders.Domain.Responses;
using DIO.Orders.Domain.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DIO.Orders.API.Controllers
{
    /// <summary>
    /// Encapsulate the APIs available for <see cref="Order"/>s.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly List<ResultCodeType> _knownResultCodes = new() { ResultCodeType.InvalidCustomer, ResultCodeType.InvalidProduct, ResultCodeType.InvalidPromotion };
        private readonly IOrderService _orderService;

        /// <summary>
        /// Initialize the Order controller.
        /// </summary>
        /// <param name="orderService">An instance of <see cref="IOrderService"/>.</param>
        public OrderController(IOrderService orderService) => _orderService = orderService;

        /// <summary>
        /// Retrieve the list of <see cref="Order"/>s from repository.
        /// </summary>
        /// <returns>All <see cref="Order"/>s from repository.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Order>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
        public IActionResult Get() => SafeExecutionForItemCollections(() => _orderService.Get());

        /// <summary>
        /// Retrieve a <see cref="Order"/> from repository based on the given identifier.
        /// </summary>
        /// <param name="id">An integer value representing the Order that should be selected.</param>
        /// <returns>The <see cref="Order"/> selected from repository.</returns>
        [HttpGet("{orderId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Order))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
        public IActionResult Get([FromQuery] int orderId) => SafeExecutionForUniqueItems(() => _orderService.Get(orderId));

        /// <summary>
        /// Retrieve a <see cref="Order"/> from repository based on the given identifier in a printable mode.
        /// </summary>
        /// <param name="id">An integer value representing the Order that should be selected.</param>
        /// <returns>An string representing a printable mode of the <see cref="Order"/> retrieved.</returns>
        [HttpGet("{orderId:int}/Print")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
        public IActionResult Print([FromQuery] int orderId) => SafeExecutionOkAndBadRequest(() => _orderService.Print(orderId));

        /// <summary>
        /// Creates an <see cref="Order"/> instance and include it into the repository.
        /// </summary>
        /// <param name="customerId">The <see cref="Customer"/> identifier that requested the <see cref="Order"/>.</param>
        /// <param name="productIds">A <see cref="List{T}"/> of <see cref="Product"/> identifiers to start the <see cref="Order"/>.</param>
        /// <returns>An integer representing the new <see cref="Order"/> identifier added.</returns>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(int))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
        public IActionResult Create(int customerId, List<int> productIds)
        {
            try
            {
                var idCreated = _orderService.CreateOrder(customerId, productIds);
                if (idCreated > 0)
                    return Created("Order//Create", idCreated);

                if (!Enum.TryParse($"{idCreated}", out ResultCodeType result) || !_knownResultCodes.Contains(result))
                    result = ResultCodeType.NotCreated;

                return BadRequest(result.ToErrorResponse());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToErrorResponse());
            }
        }

        /// <summary>
        /// Include new <see cref="Product"/>s into an existent <see cref="Order"/> based on the given order identifier.
        /// </summary>
        /// <param name="id">The <see cref="Order"/> identifier to include the <see cref="Product"/>s.</param>
        /// <param name="productIds">The <see cref="List{T}"/> <see cref="Product"/> identifiers to be included into the <see cref="Order"/>.</param>
        /// <returns>True when the item was updated successfully and the new <see cref="Product"/>s was included.</returns>
        [HttpPost("{orderId:int}/Products")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
        public IActionResult AddProducts(int orderId, List<int> productIds) => SafeExecutionOkAndBadRequest(() => _orderService.AddProductsTo(orderId, productIds));

        /// <summary>
        /// Remove the <see cref="Product"/>s from an existent <see cref="Order"/> based on the given order identifier.
        /// </summary>
        /// <param name="id">The <see cref="Order"/> identifier to remove the <see cref="Product"/>s.</param>
        /// <param name="productIds">The <see cref="List{T}"/> <see cref="Product"/> identifiers to be removed from the <see cref="Order"/>.</param>
        /// <returns>The quantity of <see cref="Product"/>s was removed.</returns>
        [HttpDelete("{orderId:int}/Products")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
        public IActionResult RemoveProducts([FromQuery] int orderId) => SafeExecutionOkAndBadRequest(() => _orderService.RemoveProductsFrom(orderId));

        /// <summary>
        /// Remove the <see cref="Product"/>s from an existent <see cref="Order"/> based on the given order identifier.
        /// </summary>
        /// <param name="id">The <see cref="Order"/> identifier to remove the <see cref="Product"/>s.</param>
        /// <param name="productId">The <see cref="List{T}"/> <see cref="Product"/> identifiers to be removed from the <see cref="Order"/>.</param>
        /// <returns>The quantity of <see cref="Product"/>s was removed.</returns>
        [HttpDelete("{orderId:int}/Products/{productId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
        public IActionResult RemoveProduct([FromQuery] int orderId, [FromQuery] int productId) => SafeExecutionOkAndBadRequest(() => _orderService.RemoveProductFrom(orderId, productId));

        /// <summary>
        /// Apply the given <see cref="Promotion"/>s into an existent <see cref="Order"/> based on the given order identifier.
        /// </summary>
        /// <param name="id">The <see cref="Order"/> identifier to remove the <see cref="Product"/>s.</param>
        /// <param name="promotionId">The <see cref="Promotion"/> identifier to be applied into the given <see cref="Order"/>.</param>
        /// <returns>The amount of discount applied into the given <see cref="Order"/>.</returns>
        [HttpPost("{orderId:int}/Promotion")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(double))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
        public IActionResult ApplyPromotion(int orderId, int promotionId) => SafeExecutionOkAndBadRequest(() => _orderService.ApplyPromotionTo(orderId, promotionId));

        /// <summary>
        /// Remove a <see cref="Order"/> instance from repository.
        /// </summary>
        /// <param name="id">The <see cref="Order"/> identifier to be removed.</param>
        /// <returns>True when the <see cref="Order"/> was removed successfully.</returns>
        [HttpDelete("{orderId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
        public IActionResult Delete(int orderId) => SafeExecutionOkAndBadRequest(() => _orderService.Delete(orderId));
    }
}
