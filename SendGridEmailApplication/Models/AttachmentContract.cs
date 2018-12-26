using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SendGridEmailApplication.Models
{
    public class AttachmentContract
    {
        public byte[] fileBytes { get; set; }
        public string  FileName { get; set; }
        public string ContentType { get; set; }
    }
}