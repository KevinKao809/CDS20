using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMSApp.Model
{
    class SMSModel
    {
        public string senderPhoneNumber = "";
        public string receiverPhoneNumber { get; set; }
        public string smsContent { get; set; }
    }
}
