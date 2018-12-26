using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SendGridEmailApplication.Models
{
    public class EmailServiceException : Exception
    {
        public string Body { get;private set; }

        public EmailServiceException(string message, string body) : base(message)
        {
            Body = body;
        }

        public EmailServiceException(string message, string body, Exception innerException) : base(message, innerException)
        {
            Body = body;
        }
    }


    public class EmailResponse
    {
        public DateTime DateSent { get; internal set; }
        public string UniqueMessageId { get; internal set; }
        public string email { get; set; }
        public int timestamp { get; set; }
        public int uid { get; set; }
        public int id { get; set; }
        public string sendgrid_event_id { get; set; }
        [JsonProperty("smtp-id")] // switched to underscore for consistancy
        public string smtp_id { get; set; }
        public string sg_message_id { get; set; }
        [JsonProperty("event")] // event is protected keyword
        public string sendgrid_event { get; set; }
        public string type { get; set; }
        public IList<string> category { get; set; }
        public string reason { get; set; }
        public string status { get; set; }
        public string url { get; set; }
        public string useragent { get; set; }
        public string ip { get; set; }

        // Add your custom fields here
        public string purchase { get; set; } // this is a custom field sent by our tester
    }
}