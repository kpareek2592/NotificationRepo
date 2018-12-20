using Newtonsoft.Json;

namespace SendGridEmailApplication.Models
{
    /// <summary>
    /// Model for SMS
    /// </summary>
    public class SmsContract
    {
        /// <summary>
        /// The number from which message is sent
        /// </summary>
        [JsonProperty("from", Order = 1)]
        public string From { get; set; }

        /// <summary>
        /// Body of SMS
        /// </summary>
        [JsonProperty("body", Order = 2)]
        public string Body { get; set; }

        /// <summary>
        /// Receipients
        /// </summary>
        [JsonProperty("to", Order = 3)]
        public string ToPhoneNumber { get; set; }        
    }
}