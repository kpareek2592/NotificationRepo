﻿using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using SendGridEmailApplication.Models;

namespace SendGridEmailApplication.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }

        
        // Capture the SendGrid Event Webhook POST's at the /api/SendGrid endpoint
            [HttpPost]
        [Route("api/SendGrid")]
        [ValidateInput(false)]
            public ActionResult SendGrid()
            {
                System.IO.StreamReader reader = new System.IO.StreamReader(HttpContext.Request.InputStream);
                string rawSendGridJSON = reader.ReadToEnd();
                List<SendGridEvents> sendGridEvents = JsonConvert.DeserializeObject<List<SendGridEvents>>(rawSendGridJSON);
                string count = sendGridEvents.Count.ToString();
                System.Diagnostics.Trace.TraceError(rawSendGridJSON); // For debugging to the Azure Streaming logs
                foreach (SendGridEvents sendGridEvent in sendGridEvents)
                {
                    // Here is where you capture the event data
                    System.Diagnostics.Trace.TraceError(sendGridEvent.email); // For debugging to the Azure Streaming logs
                }
                return new HttpStatusCodeResult(200);
            }
        }
    }
