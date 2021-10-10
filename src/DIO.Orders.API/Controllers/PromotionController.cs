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
    /// Encapsulate the APIs available for <see cref="Promotion"/>s.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class PromotionController : ControllerBase
    {
        private readonly IPromotionService _promotionService;

        /// <summary>
        /// Initialize the Promotion controller.
        /// </summary>
        /// <param name="promotionService">An instance of <see cref="IPromotionService"/>.</param>
        public PromotionController(IPromotionService promotionService) => _promotionService = promotionService;

        /// <summary>
        /// Retrieve the list of <see cref="Promotion"/>s from repository.
        /// </summary>
        /// <returns>All <see cref="Promotion"/>s from repository.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Promotion>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
        public IActionResult Get() => SafeExecutionForItemCollections(() => _promotionService.Get());

        /// <summary>
        /// Retrieve a <see cref="Promotion"/> from repository based on the given identifier.
        /// </summary>
        /// <param name="id">An integer value representing the Promotion that should be selected.</param>
        /// <returns>The <see cref="Promotion"/> selected from repository.</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Promotion))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
        public IActionResult Get(int id) => SafeExecutionForUniqueItems(() => _promotionService.Get(id));

        /// <summary>
        /// Retrieve a <see cref="Promotion"/> from repository based on the given identifier in a printable mode.
        /// </summary>
        /// <param name="id">An integer value representing the Promotion that should be selected.</param>
        /// <returns>An string representing a printable mode of the <see cref="Promotion"/> retrieved.</returns>
        [HttpGet("{id:int}/Print")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
        public IActionResult Print(int id) => SafeExecutionOkAndBadRequest(() => _promotionService.Print(id));

        /// <summary>
        /// Insert a <see cref="Promotion"/> into repository.
        /// </summary>
        /// <param name="promotion">The <see cref="Promotion"/> instance to be inserted.</param>
        /// <returns>An integer representing the new identifier added.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(int))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
        public IActionResult Add(Promotion promotion)
        {
            try
            {
                var idCreated = _promotionService.Add(promotion); ;
                if (idCreated > 0)
                    return Created("Promotion//Add", idCreated);

                return Enum.TryParse($"{idCreated}", out ResultCodeType result) && result == ResultCodeType.InvalidPromotion
                    ? BadRequest(result.ToErrorResponse())
                    : BadRequest(ResultCodeType.NotCreated.ToErrorResponse());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToErrorResponse());
            }
        }

        /// <summary>
        /// Update an existent <see cref="Promotion"/>.
        /// </summary>
        /// <param name="promotion">The <see cref="Promotion"/> instance to be updated.</param>
        /// <returns>True when the item was updated successfully.</returns>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
        public IActionResult Update(Promotion promotion) => SafeExecutionOkAndBadRequest(() => _promotionService.Update(promotion));

        /// <summary>
        /// Remove a <see cref="Promotion"/> instance from repository.
        /// </summary>
        /// <param name="id">The <see cref="Promotion"/> identifier to be removed.</param>
        /// <returns>True when the <see cref="Promotion"/> was removed successfully.</returns>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
        public IActionResult Delete(int id) => SafeExecutionOkAndBadRequest(() => _promotionService.Delete(id));
    }
}