using System.Collections.Generic;

namespace SendGridEmailApplication.Exceptions
{
    /// <summary>
    /// Invalid parameter values error
    /// </summary>
    public class BadRequestError : APIError
    {
        /// <summary>
        /// parameter name which are having errors / wrong input
        /// </summary>
        public List<string> InvalidParameters { get; set; }
    }
}
