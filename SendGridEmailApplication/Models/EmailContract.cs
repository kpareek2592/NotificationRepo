using Newtonsoft.Json;

namespace SendGridEmailApplication.Models
{
    public class EmailContract
    {
        /// <summary>
        /// Sender of the email
        /// </summary>
        [JsonProperty("from", Order = 1)]
        public string From { get; set; }

        /// <summary>
        /// Email Subject
        /// </summary>
        [JsonProperty("subject", Order = 2)]
        public string Subject { get; set; }

        /// <summary>
        /// Email Body
        /// </summary>
        [JsonProperty("body", Order = 3)]
        public string Body { get; set; }

        /// <summary>
        /// Receivers of the email
        /// </summary>
        [JsonProperty("tos", Order = 4)]
        public string ToEmailAddress { get; set; }

        /// <summary>
        /// CC Receivers of the email
        /// </summary>
        [JsonProperty("ccs", Order = 5)]
        public string CcEmailAddress { get; set; }

        /// <summary>
        /// BCC Receivers of the email
        /// </summary>
        [JsonProperty("bccs", Order = 6)]
        public string BccEmailAddress { get; set; }
    }
}