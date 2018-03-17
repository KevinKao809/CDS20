using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Web.Http;

namespace sfAPIService.Controllers
{
    public class CDSApiController : ApiController
    {
        protected class CDSTokenProperties
        {
            public int CompanyId { get; }
            public CDSTokenProperties()
            {
                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
                var companyId = identity.Claims
                                .Where(c => c.Type == "CompanyId")
                                .Select(c => c.Value).SingleOrDefault();
                CompanyId = Convert.ToInt32(companyId);
            }
        }
        protected CDSTokenProperties UserToken { get; }

        public CDSApiController()
        {
            UserToken = new CDSTokenProperties();
        }
    }
}
