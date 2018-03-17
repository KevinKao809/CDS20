using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Http;

using Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using System.Web.Script.Serialization;
using sfAPIService.Providers;
using sfShareLib;
using sfAPIService.Models;
using System.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using System.Threading;
using CDSShareLib.Helper;

[assembly: OwinStartup(typeof(sfAPIService.Startup))]
namespace sfAPIService
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Initial all global variable
            InitGlobalVariable();

            HttpConfiguration config = new HttpConfiguration();
            //Setting of Cors should be here
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);

            ConfigureOAuth(app);            

            WebApiConfig.Register(config);            
            app.UseWebApi(config);
        }

        public void ConfigureOAuth(IAppBuilder app)
        {
            OAuthAuthorizationServerOptions OAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromHours(Global.TokenRefreshLifeTimeByHour),
                Provider = new OAuthProviders(),
                RefreshTokenProvider = new RefreshTokenProvider()
            };
            app.UseOAuthAuthorizationServer(OAuthServerOptions);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());  
        }
        
        private void InitGlobalVariable()
        {
            LogLevel logLevel = (LogLevel)Enum.Parse(typeof(LogLevel), AzureSQLHelper.SystemConfigurationModel.GetCDSConfigValueByKey("APIServiceLogLevel"));
            Global._systemStorageName = AzureSQLHelper.SystemConfigurationModel.GetCDSConfigValueByKey("SystemStorageName");
            Global._systemStorageKey = AzureSQLHelper.SystemConfigurationModel.GetCDSConfigValueByKey("SystemStorageKey");
            Global._imageStorageContainer = AzureSQLHelper.SystemConfigurationModel.GetCDSConfigValueByKey("ImageStorageContainer");

            Global._appLogger = new LogHelper(Global._systemStorageName, Global._systemStorageKey, AzureSQLHelper.SystemConfigurationModel.GetCDSConfigValueByKey("APIServiceLogStorageContainerApp"), logLevel);
            Global._enableRedisCache = bool.Parse(AzureSQLHelper.SystemConfigurationModel.GetCDSConfigValueByKey("APIServiceRedisCacheEnable"));
            Global._jsSerializer = new JavaScriptSerializer();
            Global.TokenRefreshLifeTimeByHour = Int32.Parse(AzureSQLHelper.SystemConfigurationModel.GetCDSConfigValueByKey("TokenRefreshLifeTimeByHour"));
            Global._companyLogoWidthHeight = AzureSQLHelper.SystemConfigurationModel.GetCDSConfigValueByKey("CompanyLogoWidthHeight");
            Global._companyIconWidthHeight = AzureSQLHelper.SystemConfigurationModel.GetCDSConfigValueByKey("CompanyIconWidthHeight");
            Global._factoryPhotoWidthHeight = AzureSQLHelper.SystemConfigurationModel.GetCDSConfigValueByKey("FactoryPhotoWidthHeight");
            Global._equipmentPhotoWidthHeight = AzureSQLHelper.SystemConfigurationModel.GetCDSConfigValueByKey("EquipmentPhotoWidthHeight");
            Global._employeePhotoWidthHeight = AzureSQLHelper.SystemConfigurationModel.GetCDSConfigValueByKey("EmployeePhotoWidthHeight");
            Global._employeeIconWidthHeight = AzureSQLHelper.SystemConfigurationModel.GetCDSConfigValueByKey("EmployeeIconWidthHeight");
            Global._imageBgColor = AzureSQLHelper.SystemConfigurationModel.GetCDSConfigValueByKey("ImageBgColor");
            Global._imageFormat = AzureSQLHelper.SystemConfigurationModel.GetCDSConfigValueByKey("ImageConvertFormat");

            //Service Bus
            Global.ServiceBus.Helper = new CDSShareLib.ServiceBus.Helper(AzureSQLHelper.SystemConfigurationModel.GetCDSConfigValueByKey("ServiceBusConnectionString"));
            Global.ServiceBus.Queue.Provision = AzureSQLHelper.SystemConfigurationModel.GetCDSConfigValueByKey("ProvisionQueue");
            Global.ServiceBus.Queue.EventAction = AzureSQLHelper.SystemConfigurationModel.GetCDSConfigValueByKey("EventActionQueue");
            Global.ServiceBus.Topic.IoTHubReceiver = AzureSQLHelper.SystemConfigurationModel.GetCDSConfigValueByKey("IoTHubReceiverTopic");
        }
    }

    public static class Global
    {
        public static LogHelper _appLogger;
        public static bool _enableRedisCache;
        public static string _companyLogoWidthHeight, _companyIconWidthHeight, _factoryPhotoWidthHeight, _equipmentPhotoWidthHeight, _employeePhotoWidthHeight, _employeeIconWidthHeight;
        public static string _imageBgColor, _imageFormat;
        public static string _systemStorageName, _systemStorageKey;
        public static string _imageStorageContainer;
        public static JavaScriptSerializer _jsSerializer;
        public static double TokenRefreshLifeTimeByHour;

        public const string DBSchemaName = "[CDS20]";

        public static class MetaDataEntityType
        {
            public const string Equipment = "equipment";
        }
        public static class ServiceBus
        {
            public static CDSShareLib.ServiceBus.Helper Helper;
            public static class Queue
            {
                public static string Provision = "";
                public static string EventAction = "";
            }
            public static class Topic
            {
                public static string IoTHubReceiver = "";
            }

        }


        public static int GetCompanyIdFromToken()
        {
            var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
            var companyId = identity.Claims.Where(c => c.Type == "CompanyId")
                   .Select(c => c.Value).SingleOrDefault();
            return Convert.ToInt32(companyId);
        }
    }

    public static class APIServiceClient
    {
        public const string SuperAdmin = "superadmin";
        public const string Admin = "admin";
        public const string Device = "device";
        public const string External = "external";
    }
    
}