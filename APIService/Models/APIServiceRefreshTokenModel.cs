using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using sfShareLib;
using System.Data.Entity;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace sfAPIService.Models
{
    public class APIServiceRefreshTokenModel
    {
        public class Format_Detail
        {
            public int Id { get; set; }
            public int UserId { get; set; }
            public string ClientId { get; set; }
            public string RefreshToken { get; set; }
            public string ProtectedTicket { get; set; }
            public DateTime IssusedAt { get; set; }
            public DateTime ExpiredAt { get; set; }
        }

        public class Format_Create
        {
            [Required]
            public int UserId { get; set; }
            [Required]
            public string ClientId { get; set; }
            [Required]
            public string RefreshToken { get; set; }
            [Required]
            public string ProtectedTicket { get; set; }
            [Required]
            public DateTime IssusedAt { get; set; }
            [Required]
            public DateTime ExpiredAt { get; set; }
        }

        public Format_Detail GetByRefreshToken(string refreshToken)
        {
            CDStudioEntities dbEntity = new CDStudioEntities();
            var L2Enty = from c in dbEntity.APIServiceRefreshToken
                         where c.RefreshToken == refreshToken
                         select c;
            return L2Enty.Select(s => new Format_Detail()
            {
                Id = s.Id,
                UserId = s.UserId,
                ClientId = s.ClientId,
                RefreshToken = s.RefreshToken,
                ProtectedTicket = s.ProtectedTicket,
                IssusedAt = s.IssuedAt,
                ExpiredAt = s.ExpiredAt
            }).Single<Format_Detail>();
        }

        public int Create(Format_Create dataModel)
        {     
            APIServiceRefreshToken newAPIServiceRefreshToken = new APIServiceRefreshToken();
            newAPIServiceRefreshToken.UserId = dataModel.UserId;
            newAPIServiceRefreshToken.ClientId = dataModel.ClientId;
            newAPIServiceRefreshToken.RefreshToken = dataModel.RefreshToken;
            newAPIServiceRefreshToken.IssuedAt = dataModel.IssusedAt;
            newAPIServiceRefreshToken.ExpiredAt = dataModel.ExpiredAt;
            newAPIServiceRefreshToken.ProtectedTicket = dataModel.ProtectedTicket;

            newAPIServiceRefreshToken.CreatedAt = DateTime.UtcNow;

            CDStudioEntities dbEntity = new CDStudioEntities();
            dbEntity.APIServiceRefreshToken.Add(newAPIServiceRefreshToken);
            dbEntity.SaveChanges();
            return newAPIServiceRefreshToken.Id;
        }

        public void DeleteById(int id)
        {
            CDStudioEntities dbEntity = new CDStudioEntities();
            var existingDataModel = dbEntity.APIServiceRefreshToken.Find(id);

            dbEntity.Entry(existingDataModel).State = EntityState.Deleted;
            dbEntity.SaveChanges();
        }
    }
}