using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailApp.Model
{
    public class EmailModel
    {
        public string senderName { get; set; }
        public string senderEmail { get; set; }
        public string receverName { get; set; }
        public string receverEmail { get; set; }
        public string subject { get; set; }
        public string plainTextContent { get; set; }
        public string htmlContent { get; set; }
    }
}
