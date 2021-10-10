using System;
using System.Collections.Generic;
using System.Linq;
using DIO.Orders.Application.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace DIO.Orders.API.Controllers
{
    /// <summary>
    /// Provide some common methods for <see cref="Controller"/> instances
    /// </summary>
    public abstract class ControllerBase : Controller
    {
        /// <summary>
        /// Provide a safe execution for a function that returns a collection of objects and the response should be one instance of <see cref="IActionResult"/> with the types <see cref="OkResult"/>, <see cref="NoContentResult"/> or <see cref="BadRequestResult"/>.
        /// </summary>
        /// <typeparam name="TResult">The type of the function returned.</typeparam>
        /// <param name="function">The <see cref="Func{TResult}"/> to be safety executed.</param>
        /// <returns>A safe result encapsulated in one <see cref="IActionResult"/> object.</returns>
        /// <returns>An instance of <see cref="OkResult"/> when executed the function a collection of objects was returned.</returns>
        /// <returns>An instance of <see cref="NoContentResult"/> when executed the function and no value or an empty collection was returned.</returns>
        /// <returns>An instance of <see cref="BadRequestResult"/> when failed.</returns>
        protected virtual IActionResult SafeExecutionForItemCollections<TResult>(Func<IEnumerable<TResult>> function)
        {
            try
            {
                var result = function();
                return result.Any()
                    ? Ok(result)
                    : NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToErrorResponse());
            }
        }

        /// <summary>
        /// Provide a safe execution for a function that returns a unique object and the response should be one instance of <see cref="IActionResult"/> with the types <see cref="OkResult"/>, <see cref="NotFoundResult"/> or <see cref="BadRequestResult"/>.
        /// </summary>
        /// <typeparam name="TResult">The type of the function returned.</typeparam>
        /// <param name="function">The <see cref="Func{TResult}"/> to be safety executed.</param>
        /// <returns>A safe result encapsulated in one <see cref="IActionResult"/> object.</returns>
        /// <returns>An instance of <see cref="OkResult"/> when executed the function and collect the object.</returns>
        /// <returns>An instance of <see cref="NotFoundResult"/> when executed the function and no value was returned.</returns>
        /// <returns>An instance of <see cref="BadRequestResult"/> when failed.</returns>
        protected virtual IActionResult SafeExecutionForUniqueItems<TResult>(Func<TResult> function)
        {
            try
            {
                var result = function();
                return result != null
                    ? Ok(result)
                    : NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToErrorResponse());
            }
        }

        /// <summary>
        /// Provide a safe execution for a function returning an object of <see cref="IActionResult"/> when the result will be <see cref="OkResult"/> or <see cref="BadRequestResult"/>.
        /// </summary>
        /// <typeparam name="TResult">The type of the function returned.</typeparam>
        /// <param name="function">The <see cref="Func{TResult}"/> to be safety executed.</param>
        /// <returns>A safe result encapsulated in one <see cref="IActionResult"/> object.</returns>
        /// <returns>An instance of <see cref="OkResult"/> when executed the function and collect the object.</returns>
        /// <returns>An instance of <see cref="BadRequestResult"/> when failed.</returns>
        protected virtual IActionResult SafeExecutionOkAndBadRequest<TResult>(Func<TResult> function)
        {
            try
            {
                var result = function();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToErrorResponse());
            }
        }
    }
}
