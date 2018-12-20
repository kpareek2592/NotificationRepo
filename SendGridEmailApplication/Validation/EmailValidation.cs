using SendGridEmailApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;

namespace SendGridEmailApplication.Validation
{
    public class EmailValidation : ApiController
    {
        public HttpResponseMessage Validate(EmailContract contract)
        {
            if (string.IsNullOrEmpty(contract.From))
            {
                var response = Request.CreateErrorResponse(HttpStatusCode.BadRequest, "From Email Address is required");
                return response;
            }
            if (string.IsNullOrEmpty(contract.ToEmailAddress))
            {
                var response = Request.CreateErrorResponse(HttpStatusCode.BadRequest, "To Email Address is required");
                return response;
            }

            if (!string.IsNullOrEmpty(contract.From))
            {
                bool isEmail = Regex.IsMatch(contract.From,
                    @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z",
                    RegexOptions.IgnoreCase);
                if (!isEmail)
                {
                    var response = Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid Email Address");
                    return response;
                }
            }
            var message = Request.CreateResponse(HttpStatusCode.OK);
            return message;
        }
    }
}