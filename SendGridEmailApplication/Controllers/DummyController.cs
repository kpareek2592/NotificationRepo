using SendGridEmailApplication.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Http;

namespace SendGridEmailApplication.Controllers
{
    /// <summary>
    /// This is a dummy controller to put validations for time being
    /// </summary>
    public class DummyController : ApiController
    {
        /// <summary>
        /// Validation for null value
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="parameterName"></param>
        public void ValidateParameterForNull(object parameter, string parameterName = "")
        {
            if (string.IsNullOrEmpty(parameter.ToString()))
                throw new BadRequestException(Resources.InvalidRequest, Request, parameterName);
        }

        /// <summary>
        /// Method to validate the uploaded file for size and extension
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns>boolen</returns>
        public static bool ValidateAttachment(string fileName)
        {
            try
            {
                var ext = fileName.Substring(fileName.LastIndexOf('.') + 1).ToLower();
                if (ext == "exe")
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Validates input is a valid email address. 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="parameterName"></param>
        public void ValidateEmail(string email, string parameterName = "")
        {
            if (!string.IsNullOrEmpty(email))
            {
                bool isEmail = Regex.IsMatch(email,
                    @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z",
                    RegexOptions.IgnoreCase);
                if (!isEmail)
                    throw new BadRequestException(Resources.InvalidEmail, Request, parameterName);
            }
        }

        /// <summary>
        /// Validates input is a valid phone number. 
        /// </summary>
        /// <param name="number">The phone number to validate</param>
        /// <param name="parameterName"></param>
        public void ValidatePhoneNumber(string number, string parameterName = "")
        {

            if (!string.IsNullOrEmpty(number))
            {
                bool isValid = Regex.IsMatch(number,
                     @"\(?\d{3}\)?-? *\d{3}-? *-?\d{4}",
                    RegexOptions.IgnoreCase);
                if (!isValid)
                    throw new BadRequestException(Resources.InvalidPhoneNumber, Request, parameterName);
            }
        }

        /// <summary>
        /// Validates string for empty or null and return the trimed value.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="parameterName">name of the parameter to add in error detail</param>
        /// <exception cref="BadRequestException">
        /// </exception>
        public string ParseString(string parameter, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(parameter))
                throw new BadRequestException(Resources.InvalidRequest, Request, parameterName);

            return parameter.Trim();
        }
    }
}
