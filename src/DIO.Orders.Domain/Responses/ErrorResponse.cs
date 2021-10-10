using System;

namespace DIO.Orders.Domain.Responses
{
    /// <summary>
    /// Encapsulate the errors in a unique object to be used as response for the API controllers.
    /// </summary>
    public class ErrorResponse
    {
        /// <summary>
        /// A generic message used as a friendly one when the instance is created from an <see cref="Exception"/>.
        /// </summary>
        private const string GenericFailureMessage = "Generic error happened!";

        /// <summary>
        /// The friendly message to be returned.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// The <see cref="Exception"/> instance that will be returned for generic failures.
        /// </summary>
        public Exception Error { get; set; }

        /// <summary>
        /// Constructors methods to initialize instance of <see cref="ErrorResponse"/> based on a given <see cref="string"/> message or an <see cref="Exception"/> object.
        /// </summary>
        public ErrorResponse() { Message = GenericFailureMessage; }
        public ErrorResponse(string message) { Message = message; }
        public ErrorResponse(Exception exception) : this() { Error = exception; }
    }
}
