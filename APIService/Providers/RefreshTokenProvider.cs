using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler.Serializer;
using Microsoft.Owin.Security.Infrastructure;
using sfAPIService.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;

namespace sfAPIService.Providers
{
    public class RefreshTokenProvider : IAuthenticationTokenProvider
    {
        public async Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            string userId, clientId, refreshTokenId;

            if (context.Ticket.Properties.Dictionary.ContainsKey("Id") && context.Ticket.Properties.Dictionary.ContainsKey("Client_Id"))
            {
                userId = context.Ticket.Properties.Dictionary["Id"];
                clientId = context.Ticket.Properties.Dictionary["Client_Id"];
                refreshTokenId = Guid.NewGuid().ToString("n");
            }
            else
                return;

            // copy properties and set the desired lifetime of refresh token
            // record refresh token into DB
            DateTime refreshTokenIssuedUtc = DateTime.UtcNow;
            DateTime refreshTokenExpiresUtc = DateTime.UtcNow.AddHours(Global.TokenRefreshLifeTimeByHour).AddMinutes(20);
            context.Ticket.Properties.IssuedUtc = refreshTokenIssuedUtc;
            context.Ticket.Properties.ExpiresUtc = refreshTokenExpiresUtc;

            APIServiceRefreshTokenModel.Format_Create refreshToken = new APIServiceRefreshTokenModel.Format_Create();
            refreshToken.ClientId = clientId;
            refreshToken.UserId = Int32.Parse(userId);
            refreshToken.RefreshToken = Crypto.SHA256(refreshTokenId);
            refreshToken.ProtectedTicket = context.SerializeTicket();
            refreshToken.IssusedAt = refreshTokenIssuedUtc;
            refreshToken.ExpiredAt = refreshTokenExpiresUtc;
            
            APIServiceRefreshTokenModel model = new APIServiceRefreshTokenModel();             
            try
            {
                int id = model.Create(refreshToken);
                if (id > 0)
                {
                    context.SetToken(refreshTokenId);
                }
            }
            catch(Exception ex)
            {
            }
            
        }

        public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            string token = Crypto.SHA256(context.Token);

            try
            {
                APIServiceRefreshTokenModel model = new APIServiceRefreshTokenModel();
                var tokenInfo = model.GetByRefreshToken(token);
                if (tokenInfo != null)
                {
                    context.DeserializeTicket(tokenInfo.ProtectedTicket);
                    model.DeleteById(tokenInfo.Id);
                }
            }
            catch (Exception ex)
            {

            }            
        }
        
        public void Create(AuthenticationTokenCreateContext context)
        {
            throw new NotImplementedException();
        }

        public void Receive(AuthenticationTokenReceiveContext context)
        {
            throw new NotImplementedException();
        }
    }
}