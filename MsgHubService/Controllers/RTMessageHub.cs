using CDSShareLib;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Threading.Tasks;
using static CDSShareLib.Helper.AzureSQLHelper;

namespace MsgHubService.Controllers
{
    [HubName("RTMessageHub")]
    public class RTMessageHub : Hub
    {
        // Allow Client from AdminWeb, SuperAdminWeb and pre-defined Allow Domains
        public async Task Register(string CompanyId)
        {
            Startup._appLogger.Info("CompanyID: " + CompanyId);
            string clientOrigin = Context.Headers["Origin"];

            if (clientOrigin != null && clientOrigin.Contains("//"))
                clientOrigin = clientOrigin.Substring(clientOrigin.IndexOf("//") + 2);

            if (CompanyId != "0" && clientOrigin != null && (clientOrigin != Startup._adminWebURI || clientOrigin != Startup._superAdminWebURI))
            {
                Startup._appLogger.Warn(String.Format("Client Origin: {0}, Check Allow Domain ...", clientOrigin));
                try
                {
                    CompanyModel companyModel = new CompanyModel();
                    Company company = companyModel.GetById(int.Parse(CompanyId));
                    if (company.AllowDomain != null && company.AllowDomain != "*")
                    {
                        Startup._appLogger.Info("CompanyID: " + CompanyId + ", Allow Domain: " + company.AllowDomain);
                        if (!company.AllowDomain.Contains(clientOrigin))
                        {
                            Startup._appLogger.Warn("Unauthorized. Allow Domain (" + company.AllowDomain + "); Request Domain (" + clientOrigin + ")");
                            return;
                        }
                    }
                    Startup._appLogger.Warn("Authorized. Allow Domain (" + company.AllowDomain + "); Request Domain (" + clientOrigin + ")");
                }
                catch (Exception ex)
                {
                    Startup._appLogger.Error("Exception on Allow Domain Check: " + ex.Message + "," + ex.InnerException.Message);
                    return;
                }
            }
            await Groups.Add(Context.ConnectionId, CompanyId);
            PublishMessage(CompanyId, "{\"topic\":\"welcome\"}");
        }

        private void PublishMessage(string CompanyId, string message)
        {
            Clients.Group(CompanyId).onReceivedMessage(message);
        }
    }
}