using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Web.Http.Results;

namespace SendGridEmailApplication.Exceptions
{
    /// <summary>
    /// Base API exception for WebAPI
    /// </summary>
    public class APIException : Exception
    {

        public virtual APIError ErrorProperties { get; set; }

        public APIException(String errorMessage) : base(errorMessage)
        {
            Initialize(errorMessage);
        }

        public APIException(String errorMessage,HttpStatusCode statusCode) : base(errorMessage)
        {
            Initialize(errorMessage);
            ErrorProperties.ErrorCode = statusCode;
        }

        public APIException(String errorMessage, Exception innerException) : base(errorMessage, innerException)
        {
            Initialize(errorMessage);
        }


        protected APIException(SerializationInfo info, StreamingContext context) : base(info, context)
        { }
        public APIException()
        {
            Initialize("");
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "1#")]
        public APIException(string errorMessage, string uri) : base(errorMessage)
        {
            Initialize(errorMessage);
            ErrorProperties.RequestedUri = uri;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1")]
        public APIException(string errorMessage, HttpRequestMessage requestMessage) : base(errorMessage)
        {
            Initialize(errorMessage);
            if (requestMessage != null)
            {
                if (requestMessage.RequestUri != null)
                {
                    ErrorProperties.RequestedUri = requestMessage.RequestUri.ToString();
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1")]
        public APIException(string errorMessage, HttpRequestMessage requestMessage, Exception exception, HttpStatusCode statusCode = HttpStatusCode.InternalServerError) : base(errorMessage, exception)
        {
            Initialize(errorMessage);
            if (requestMessage != null)
            {
                if (requestMessage.RequestUri != null)
                {
                    ErrorProperties.RequestedUri = requestMessage.RequestUri.ToString();
                }
            }
            ErrorProperties.ErrorCode = statusCode;
        }

        private void Initialize(String message)
        {
            ErrorProperties = new APIError();
            ErrorProperties.Message = message;
            ErrorProperties.GeneratedTime = DateTime.Now;
            ErrorProperties.ErrorCode = HttpStatusCode.InternalServerError; //Internal Server Error
        }

        /// <summary>
        /// Get Error Message in specific Format
        /// </summary>
        /// <returns></returns>
        public virtual string GetErrorMessage()
        {
            if (String.IsNullOrEmpty(ErrorProperties.DeveloperMessage) && this.InnerException != null)
                ErrorProperties.DeveloperMessage = this.InnerException.Message;
            if (String.IsNullOrEmpty(ErrorProperties.DeveloperMessage))
            {
                return String.Format(Resources.APIExceptionWithoutDeveloperMessage, ErrorProperties.ErrorId.ToString(), ErrorProperties.ErrorCode, this.Message, ErrorProperties.GeneratedTime.ToString());
            }
            else
            {
                return String.Format(Resources.APIExceptionWithDeveloperMessage, ErrorProperties.ErrorId.ToString(), ErrorProperties.ErrorCode, this.Message, ErrorProperties.DeveloperMessage, ErrorProperties.GeneratedTime.ToString());
            }
        }

        /// <summary>
        /// Error Message with Stack Trace
        /// </summary>
        /// <returns></returns>
        public virtual string GetLogErrorMessage()
        {
            if (this.InnerException != null && !string.IsNullOrEmpty(this.InnerException.ToString()))
                return String.Format("{0}, StackTrace: {1}", GetErrorMessage(), this.InnerException.ToString());
            else
                return GetErrorMessage();
        }

        /// <summary>
        /// Exception response for client
        /// </summary>
        /// <returns></returns>
        public virtual ResponseMessageResult GetExceptionResponse(HttpRequestMessage request)
        {
            if (request == null) return null;
            String _tempDevMessage = ErrorProperties.DeveloperMessage;
            ErrorProperties.DeveloperMessage = null;
            var response = request.CreateResponse();

            response.Content = new StringContent(JsonConvert.SerializeObject(ErrorProperties, Newtonsoft.Json.Formatting.None,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            }));
            response.StatusCode = ErrorProperties.ErrorCode;
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            response.Headers.Add("isLogged", "true");
            return new ResponseMessageResult(response);
        }
    }
}
