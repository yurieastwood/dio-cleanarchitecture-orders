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
    /// Encapsulate the APIs available for <see cref="Customer"/>s.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        /// <summary>
        /// Initialize the Customer controller.
        /// </summary>
        /// <param name="customerService">An instance of <see cref="ICustomerService"/>.</param>
        public CustomerController(ICustomerService customerService) => _customerService = customerService;

        /// <summary>
        /// Retrieve the list of <see cref="Customer"/>s from repository.
        /// </summary>
        /// <returns>All <see cref="Customer"/>s from repository.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Customer>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
        public IActionResult Get() => SafeExecutionForItemCollections(() => _customerService.Get());

        /// <summary>
        /// Retrieve a <see cref="Customer"/> from repository based on the given identifier.
        /// </summary>
        /// <param name="id">An integer value representing the Customer that should be selected.</param>
        /// <returns>The <see cref="Customer"/> selected from repository.</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Customer))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
        public IActionResult Get(int id) => SafeExecutionForUniqueItems(() => _customerService.Get(id));

        /// <summary>
        /// Retrieve a <see cref="Customer"/> from repository based on the given identifier in a printable mode.
        /// </summary>
        /// <param name="id">An integer value representing the Customer that should be selected.</param>
        /// <returns>An string representing a printable mode of the <see cref="Customer"/> retrieved.</returns>
        [HttpGet("{id:int}/Print")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
        public IActionResult Print(int id) => SafeExecutionOkAndBadRequest(() => _customerService.Print(id));

        /// <summary>
        /// Insert a <see cref="Customer"/> into repository.
        /// </summary>
        /// <param name="customer">The <see cref="Customer"/> instance to be inserted.</param>
        /// <returns>An integer representing the new identifier added.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(int))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
        public IActionResult Add(Customer customer)
        {
            try
            {
                var idCreated = _customerService.Add(customer);
                if (idCreated > 0)
                    return Created("Customer//Add", idCreated);

                return Enum.TryParse($"{idCreated}", out ResultCodeType result) && result == ResultCodeType.InvalidCustomer
                    ? BadRequest(result.ToErrorResponse())
                    : BadRequest(ResultCodeType.NotCreated.ToErrorResponse());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToErrorResponse());
            }
        }

        /// <summary>
        /// Update an existent <see cref="Customer"/>.
        /// </summary>
        /// <param name="customer">The <see cref="Customer"/> instance to be updated.</param>
        /// <returns>True when the item was updated successfully.</returns>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
        public IActionResult Update(Customer customer) => SafeExecutionOkAndBadRequest(() => _customerService.Update(customer));

        /// <summary>
        /// Remove a <see cref="Customer"/> instance from repository.
        /// </summary>
        /// <param name="id">The <see cref="Customer"/> identifier to be removed.</param>
        /// <returns>True when the <see cref="Customer"/> was removed successfully.</returns>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
        public IActionResult Delete(int id) => SafeExecutionOkAndBadRequest(() => _customerService.Delete(id));
    }
}