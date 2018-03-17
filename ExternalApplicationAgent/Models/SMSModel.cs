using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace ExternalApplicationAgent.Models
{
    public class SMSModel
    {        
        public string senderPhoneNumber = "+12067354674";
        [Required]
        public string receiverPhoneNumber { get; set; }
        [Required]
        public string smsContent { get; set; }

        public void Send()
        {
            // Find your Account Sid and Auth Token at twilio.com/user/account
            const string accountSid = "AC0a222b8ee3da33ff9f90f89b629a1ef6";
            const string authToken = "fa46a700c072980cd35f8922f0ed1af2";

            if (!receiverPhoneNumber.StartsWith("+"))
                receiverPhoneNumber = "+" + receiverPhoneNumber;

            // Initialize the Twilio client
            TwilioClient.Init(accountSid, authToken);           
            MessageResource.Create(
                    from: new PhoneNumber(senderPhoneNumber), 
                    to: new PhoneNumber(receiverPhoneNumber),                                                    // Message content
                    body: smsContent);            
        }
    }
}