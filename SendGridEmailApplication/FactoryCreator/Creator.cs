using SendGridEmailApplication.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SendGridEmailApplication.FactoryCreator
{
    public abstract class Creator
    {
        public abstract IEmail SendEmailType(string type); 
    }
}