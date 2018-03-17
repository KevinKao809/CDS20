using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using System.Security.Claims;

using System.Web.Helpers;
using sfAPIService.Models;
using Microsoft.Owin.Security;
using sfShareLib;

namespace sfAPIService.Providers
{
    public class UserClaims
    {
        public bool IsAuthenticated { get; set; }
        public int CompanyId { get; set; }
    }

    public class OAuthProviders : OAuthAuthorizationServerProvider
    {
        public override async System.Threading.Tasks.Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            string clientId = string.Empty;
            string clientSecret = string.Empty;

            if (!context.TryGetBasicCredentials(out clientId, out clientSecret))
            {
                context.TryGetFormCredentials(out clientId, out clientSecret);
            }

            if (context.ClientId != null)
            {
                switch (context.ClientId)
                {
                    case APIServiceClient.SuperAdmin:
                    case APIServiceClient.Admin:
                    case APIServiceClient.Device:
                    case APIServiceClient.External:
                        context.Validated();
                        break;
                    default:
                        context.SetError("invalid_clientId", "Client secret is invalid.");
                        break;
                }
            }
            else
                context.SetError("invalid_clientId", "ClientId should be sent.");
        }
                
        public override async System.Threading.Tasks.Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            string userName = context.UserName;
            string password = context.Password;
            string clientId = context.ClientId;            

            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(clientId))
            {
                context.SetError("Authentication Fail", "Incomplete parameters");
            }

            UserClaims userClaims = VerifyAccountPassword(userName, password, clientId);
            if (userClaims.IsAuthenticated)
            {
                var identity = new ClaimsIdentity(context.Options.AuthenticationType);
                identity.AddClaim(new Claim("Roles", clientId, ClaimValueTypes.String));
                identity.AddClaim(new Claim("CompanyId", userClaims.CompanyId.ToString(), ClaimValueTypes.Integer32));

                //Set current principal
                var claimPrincipal = new ClaimsPrincipal(identity);
                Thread.CurrentPrincipal = claimPrincipal;

                var ticket = new AuthenticationTicket(identity, CustomizeAuthenticationProperties(userName, clientId));
                context.Validated(ticket);
                
                //context.Validated(identity);  //original
            }
            else
            {
                //帳密驗證失敗
                //context.Response.StatusCode = 404;
                context.SetError("Authentication Fail", "Authentication Fail.");
                
            }
        }

        public override Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
        {
            string originalClient = context.Ticket.Properties.Dictionary["Client_Id"];
            string currentClient = context.ClientId;

            if (originalClient != currentClient)
            {
                context.SetError("invalid_clientId", "Refresh token is issued to a different clientId.");
                return Task.FromResult<object>(null);
            }
            
            context.Validated();
            return Task.FromResult<object>(null);
        }

        //change token return format
        public override System.Threading.Tasks.Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }
            return System.Threading.Tasks.Task.FromResult<object>(null);
        }

        //帳密驗證
        private  UserClaims VerifyAccountPassword(string userName, string password, string serviceRole)
        {
            UserClaims userClaims = new UserClaims();
            userClaims.IsAuthenticated = false;
            userClaims.CompanyId = 0;

            switch (serviceRole)
            {
                case APIServiceClient.SuperAdmin:
                    {
                        SuperAdminModel model = new SuperAdminModel();
                        userClaims.IsAuthenticated = model.VerifyPassword(userName, password);
                    }
                    break;
                case APIServiceClient.Admin:
                    {
                        EmployeeModel model = new EmployeeModel();
                        int companyId = model.VerifyPassword(userName, password);

                        if (companyId != -1)
                        {
                            userClaims.IsAuthenticated = true;
                            userClaims.CompanyId = companyId;
                        }
                    }
                    break;
                case APIServiceClient.Device:
                    AccountModels accountModels = new AccountModels();
                    userClaims.IsAuthenticated = accountModels.CheckIoTDevicePassword(userName, password);
                    break;
                case APIServiceClient.External:
                    {
                        CompanyModel model = new CompanyModel();
                        int companyId = model.GetIdByExtAppAuthenticationKey(password);

                        if (companyId != -1)
                        {
                            userClaims.IsAuthenticated = true;
                            userClaims.CompanyId = companyId;
                        }
                    }
                    break;
            }
            return userClaims;
        }
        
        private AuthenticationProperties CustomizeAuthenticationProperties(string username, string clientId)
        {
            switch (clientId)
            {
                case APIServiceClient.Admin:
                    {
                        EmployeeModel model = new EmployeeModel();
                        var employee = model.GetByEmail(username);

                        if (employee != null)
                        {
                            var employeeTokenInfo = new AuthenticationProperties(new Dictionary<string, string>
                            {
                                { "Id", employee.Id.ToString()},
                                { "EmployeeNumber", (employee.EmployeeNumber!=null) ? employee.EmployeeNumber : ""},
                                { "FirstName", (employee.FirstName!=null) ? employee.FirstName.ToString() : ""},
                                { "LastName", (employee.LastName!=null) ? employee.LastName.ToString() : ""},
                                { "Email", employee.Email},
                                { "PhotoURL", (employee.PhotoURL!=null) ? employee.PhotoURL.ToString() : ""},
                                { "Lang", (employee.Lang!=null) ? employee.Lang.ToString() : ""},
                                { "AdminFlag", employee.AdminFlag.ToString()},
                                { "Client_Id", APIServiceClient.Admin}
                            });

                            return employeeTokenInfo;                            
                        }
                    }
                    break;
                case APIServiceClient.SuperAdmin:
                    {
                        SuperAdminModel model = new SuperAdminModel();
                        var superAdmin = model.GetByEmail(username);

                        if (superAdmin != null)
                        {
                            var superAdminTokenInfo = new AuthenticationProperties(new Dictionary<string, string>
                            {
                                { "Id", superAdmin.Id.ToString() },
                                { "FirstName", (superAdmin.FirstName!=null) ? superAdmin.FirstName.ToString() : "" },
                                { "LastName", (superAdmin.LastName!=null) ? superAdmin.LastName.ToString() : "" },
                                { "Email", superAdmin.Email},
                                { "Client_Id", APIServiceClient.SuperAdmin}
                            });
                            return superAdminTokenInfo;
                        }
                    }
                    break;
            }
            return null;
        }
    }
}