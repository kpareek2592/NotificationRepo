using SendGridEmailApplication.FactoryCreator;
using SendGridEmailApplication.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SendGridEmailApplication.Common
{
    public class ConcreteCreator : Creator
    {
        public override IEmail SendEmailType(string type)
        {
            switch (type)
            {
                case "SendGrid": return new SendGridEmailService();

                default: throw new ArgumentException("Invalid type", "type");
            }
        }
    }
}