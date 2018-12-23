using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;

namespace SendGridEmailApplication.Exceptions
{
    /// <summary>
    /// Bad Request could be due to wrong input, resource not found or unprocesseable entity
    /// </summary>
    [Serializable]
    public class BadRequestException : APIException
    {
        private BadRequestError _badErrorProp;
        /// <summary>
        /// Constructor for Sending Bad Request to Application User
        /// </summary>
        /// <param name="errorMessage">custom error message which can be understood by user easily</param>
        /// <param name="requestMessage"> Getting URI out of request object and to generate response message</param>
        /// <param name="parameter">Which parameter caused the issue</param>
        /// <param name="errorCode">What exactly is the issue</param>
        /// 
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "errorCode")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public BadRequestException(string errorMessage, HttpRequestMessage requestMessage, string parameter, HttpStatusCode errorCode = HttpStatusCode.BadRequest) : base(errorMessage, requestMessage)
        {
            _badErrorProp = new BadRequestError();
            _badErrorProp.Message = ErrorProperties.Message;
            _badErrorProp.RequestedUri = ErrorProperties.RequestedUri;
            _badErrorProp.GeneratedTime = ErrorProperties.GeneratedTime;
            _badErrorProp.ErrorCode = errorCode;
            _badErrorProp.InvalidParameters = new List<string>() { parameter };

            ErrorProperties = _badErrorProp;
        }

        public BadRequestException()
        {
            // Add any type-specific logic, and supply the default message.
        }
        /// <summary>
        /// Constructor for Sending Bad Request to Application User
        /// </summary>
        /// <param name="message">custom error message which can be understood by user easily</param>
        public BadRequestException(String message) : base(message)
        {
        }

        /// <summary>
        /// Constructor for Sending Bad Request to Application User (Mandatory Construtor for code analysis, not used)
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <param name="innerException"></param>
        public BadRequestException(String errorMessage, Exception innerException) : base(errorMessage, innerException)
        {
        }

        /// <summary>
        /// Constructor for Sending Bad Request to Application User (Mandatory Construtor for code analysis, not used)
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected BadRequestException(SerializationInfo info, StreamingContext context) : base(info, context)
        { }


        /// <summary>
        /// Get the Error Message Set in Constructor and adding parameter information to it
        /// </summary>
        /// <returns></returns>
        public override string GetErrorMessage()
        {
            return string.Format(Resources.BadRequestException, base.GetErrorMessage(), GetParameterString());
        }

        private String GetParameterString()
        {
            StringBuilder parameterString = new StringBuilder();

            foreach (var item in _badErrorProp.InvalidParameters)
            {
                parameterString.Append(" " + item + " ");
            }
            return parameterString.ToString();
        }
    }
}
