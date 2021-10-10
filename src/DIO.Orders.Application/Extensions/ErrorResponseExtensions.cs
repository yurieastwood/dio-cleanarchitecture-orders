using System;
using DIO.Orders.Domain.Enums;
using DIO.Orders.Domain.Responses;

namespace DIO.Orders.Application.Extensions
{
    /// <summary>
    /// Extensions methods to create an instance of <see cref="ErrorResponse"/> objects.
    /// </summary>
    public static class ErrorResponseExtensions
    {
        /// <summary>
        /// Creates an instance of <see cref="ErrorResponse"/> based on the given <see cref="string"/> message.
        /// Usually it is a friendly message.
        /// </summary>
        /// <param name="message">The <see cref="string"/> message to create the <see cref="ErrorResponse"/> object.</param>
        /// <returns>An instance of <see cref="ErrorResponse"/>.</returns>
        public static ErrorResponse ToErrorResponse(this string message) => new (message);

        /// <summary>
        /// Creates an instance of <see cref="ErrorResponse"/> based on the given <see cref="Exception"/>.
        /// The <see cref="ErrorResponse"/> will be created using a generic message as a friendly one and the full stack trace will be returned.
        /// </summary>
        /// <param name="exception">The <see cref="Exception"/> object to create the <see cref="ErrorResponse"/> object.</param>
        /// <returns>An instance of <see cref="ErrorResponse"/>.</returns>
        public static ErrorResponse ToErrorResponse(this Exception exception) => new(exception);

        /// <summary>
        /// Creates an instance of <see cref="ErrorResponse"/> based on the given <see cref="ResultCodeType"/>.
        /// The friendly message will be the <see cref="ResultCodeType"/> description.
        /// </summary>
        /// <param name="resultCodeType">The <see cref="ResultCodeType"/> object to create the <see cref="ErrorResponse"/> object.</param>
        /// <returns>An instance of <see cref="ErrorResponse"/>.</returns>
        public static ErrorResponse ToErrorResponse(this ResultCodeType resultCodeType) => $"{resultCodeType}".ToErrorResponse();
    }
}
