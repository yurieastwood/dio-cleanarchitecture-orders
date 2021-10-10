using System;
using System.Collections.Generic;
using DIO.Orders.Application.Extensions;
using DIO.Orders.Domain.Enums;
using DIO.Orders.Domain.Models;
using DIO.Orders.Domain.Responses;
using Microsoft.AspNetCore.Mvc;
using DIO.Orders.Domain.Services;
using Microsoft.AspNetCore.Http;

namespace DIO.Orders.API.Controllers
{
    /// <summary>
    /// Encapsulate the APIs available for <see cref="Product"/>s.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        /// <summary>
        /// Initialize the product controller.
        /// </summary>
        /// <param name="productService">An instance of <see cref="IProductService"/>.</param>
        public ProductController(IProductService productService) => _productService = productService;

        /// <summary>
        /// Retrieve the list of <see cref="Product"/>s from repository.
        /// </summary>
        /// <returns>All <see cref="Product"/>s from repository.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Product>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
        public IActionResult Get() => SafeExecutionForItemCollections(() => _productService.Get());

        /// <summary>
        /// Retrieve a <see cref="Product"/> from repository based on the given identifier.
        /// </summary>
        /// <param name="id">An integer value representing the product that should be selected.</param>
        /// <returns>The <see cref="Product"/> selected from repository.</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Product))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
        public IActionResult Get(int id) => SafeExecutionForUniqueItems(() => _productService.Get(id));

        /// <summary>
        /// Retrieve a <see cref="Product"/> from repository based on the given identifier in a printable mode.
        /// </summary>
        /// <param name="id">An integer value representing the product that should be selected.</param>
        /// <returns>An string representing a printable mode of the <see cref="Product"/> retrieved.</returns>
        [HttpGet("{id:int}/Print")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
        public IActionResult Print(int id) => SafeExecutionOkAndBadRequest(() => _productService.Print(id));

        /// <summary>
        /// Insert a <see cref="Product"/> into repository.
        /// </summary>
        /// <param name="product">The <see cref="Product"/> instance to be inserted.</param>
        /// <returns>An integer representing the new identifier added.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(int))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
        public IActionResult Add(Product product)
        {
            try
            {
                var idCreated = _productService.Add(product);
                if (idCreated > 0)
                    return Created("Product//Add", idCreated);

                return Enum.TryParse($"{idCreated}", out ResultCodeType result) && result == ResultCodeType.InvalidProduct
                    ? BadRequest(result.ToErrorResponse())
                    : BadRequest(ResultCodeType.NotCreated.ToErrorResponse());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToErrorResponse());
            }
        }

        /// <summary>
        /// Update an existent <see cref="Product"/>.
        /// </summary>
        /// <param name="product">The <see cref="Product"/> instance to be updated.</param>
        /// <returns>True when the item was updated successfully.</returns>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
        public IActionResult Update(Product product) => SafeExecutionOkAndBadRequest(() => _productService.Update(product));

        /// <summary>
        /// Remove a <see cref="Product"/> instance from repository.
        /// </summary>
        /// <param name="id">The <see cref="Product"/> identifier to be removed.</param>
        /// <returns>True when the <see cref="Product"/> was removed successfully.</returns>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
        public IActionResult Delete(int id) => SafeExecutionOkAndBadRequest(() => _productService.Delete(id));
    }
}
