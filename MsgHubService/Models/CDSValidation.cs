using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsgHubService.Models
{
    public class CDSValidation
    {
        public static bool AllowIP(String CallerIP)
        {
            if (CallerIP != "::1")
            {
                String[] IP = CallerIP.Split('.');
                Int64 CallerIPNum = Int64.Parse(IP[0]) * Int64.Parse(IP[1]) * Int64.Parse(IP[2]) * Int64.Parse(IP[3]);
                if (CallerIPNum < Startup._allowStartIP || CallerIPNum > Startup._allowEndIP)
                    return false;
            }
            return true;
        }
    }
}