using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace ExternalApplicationAgent.Models
{
    public class EmailModel
    {
        public string senderName { get; set; }
        [Required]
        public string senderEmail { get; set; }
        public string receverName { get; set; }
        [Required]
        public string receverEmail { get; set; }
        [Required]
        public string subject { get; set; }
        [Required]
        public string plainTextContent { get; set; }
        public string htmlContent { get; set; }
        
        public async Task Send()
        {
            var apiKey = "xxxxxxx";
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(senderEmail, senderName);
            var to = new EmailAddress(receverEmail, receverName);            
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }
    }
}