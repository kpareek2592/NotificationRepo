﻿using SendGridEmailApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SendGridEmailApplication.Interface
{
    public interface ISmsSender
    {
        void SendSms(SmsContract contract); 
    }
}