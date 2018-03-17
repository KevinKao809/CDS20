using System;
using System.Threading.Tasks;
using CDSShareLib.Helper;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using MsgHubService.Controllers;
using Owin;

[assembly: OwinStartup(typeof(MsgHubService.Startup))]

namespace MsgHubService
{
    public class Startup
    {
        public static LogHelper _appLogger = null;
        public static Int64 _allowStartIP = 0, _allowEndIP = (Int64) 255*255*255*255;
        public static IHubContext _hubContext = GlobalHost.ConnectionManager.GetHubContext<RTMessageHub>();
        public static String _adminWebURI, _superAdminWebURI;
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=316888
            // Initial SignalR
            app.MapSignalR();

            // Initial CDS Configuration
            String logStorageConnectionString = AzureSQLHelper.SystemConfigurationModel.GetCDSConfigValueByKey("SystemStorageConnectionString");
            String logStorageContainer = AzureSQLHelper.SystemConfigurationModel.GetCDSConfigValueByKey("MsgHubServiceLogStorageContainerApp");
            LogLevel logLevel = (LogLevel)Enum.Parse(typeof(LogLevel), AzureSQLHelper.SystemConfigurationModel.GetCDSConfigValueByKey("MsgHubServiceLogLevel"));
            _appLogger = new LogHelper(logStorageConnectionString, logStorageContainer, logLevel);
            _appLogger.Info("Initial Program Start...");

            _adminWebURI = AzureSQLHelper.SystemConfigurationModel.GetCDSConfigValueByKey("AdminWebURI");
            if (_adminWebURI != null && _adminWebURI.Contains("//"))
                _adminWebURI = _adminWebURI.Substring(_adminWebURI.IndexOf("//") + 2);

            _superAdminWebURI = AzureSQLHelper.SystemConfigurationModel.GetCDSConfigValueByKey("SuperAdminWebURI");
            if (_superAdminWebURI != null && _superAdminWebURI.Contains("//"))
                _superAdminWebURI = _superAdminWebURI.Substring(_superAdminWebURI.IndexOf("//") + 2);
            try
            {
                String msgHubAllowIPStart = AzureSQLHelper.SystemConfigurationModel.GetCDSConfigValueByKey("MessageHubAllowIPStart");
                String[] allowStartIP = msgHubAllowIPStart.Split('.');
                _allowStartIP = Int64.Parse(allowStartIP[0]) * Int64.Parse(allowStartIP[1]) * Int64.Parse(allowStartIP[2]) * Int64.Parse(allowStartIP[3]);

                String msgHubAllowIPend = AzureSQLHelper.SystemConfigurationModel.GetCDSConfigValueByKey("MessageHubAllowIpEnd");
                String[] allowEndIP = msgHubAllowIPend.Split('.');
                _allowEndIP = Int64.Parse(allowEndIP[0]) * Int64.Parse(allowEndIP[1]) * Int64.Parse(allowEndIP[2]) * Int64.Parse(allowEndIP[3]);
                _appLogger.Info(string.Format("Allow IP: {0} ~ {1}", msgHubAllowIPStart, msgHubAllowIPend));
            }
            catch (Exception)
            {
            }
        }
    }
}
