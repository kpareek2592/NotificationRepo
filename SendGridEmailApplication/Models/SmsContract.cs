using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace SendGridEmailApplication.Models
{
    /// <summary>
    /// Model for SMS
    /// </summary>
    public class SmsContract
    {
        /// <summary>
        /// Body of SMS
        /// </summary>
        [JsonProperty("body", Order = 1)]
        public string Body { get; set; }

        /// <summary>
        /// Receipients
        /// </summary>
        [JsonProperty("to", Order = 2)]
        public string ToPhoneNumber { get; set; }        
    }
}