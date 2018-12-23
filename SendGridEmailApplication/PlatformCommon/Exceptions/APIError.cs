using System;
using System.Net;

namespace SendGridEmailApplication.Exceptions
{
    /// <summary>
    /// Base error class
    /// </summary>
    public class APIError
    {
        /// <summary>
        /// Set the Unique Error Id
        /// </summary>
        public APIError()
        {
            ErrorId = Guid.NewGuid();
        }
        /// <summary>
        /// Error Message
        /// </summary>
        public String Message { get; set; }
        /// <summary>
        /// Uniquely identify a particular error log.
        /// </summary>
        public Guid ErrorId { get; private set; }
        /// <summary>
        /// Status Code 400, 503 based on the exception
        /// </summary>
        public HttpStatusCode ErrorCode { get; set; }
        ///// <summary>
        ///// Link to the API Documentation
        ///// </summary>
        //public String Link { get; set; }
        /// <summary>
        /// Message Logging which might be specifically required when debugging the application
        /// </summary>
        public String DeveloperMessage { get; set; }
        /// <summary>
        /// Time at which this exception recorded
        /// </summary>
        public DateTime GeneratedTime { get; set; }
        /// <summary>
        /// URI when this exception recorded
        /// </summary>
        public String RequestedUri { get; set; }
    }
}
