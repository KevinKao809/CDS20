using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

using sfShareLib;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Data.Entity;
using CDSShareLib.Helper;

namespace sfAPIService.Models
{
    public class CompanyModel
    {
        public class Format_Detail
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string ShortName { get; set; }
            public string Address { get; set; }
            public string CompanyWebSite { get; set; }
            public string ContactName { get; set; }
            public string ContactPhone { get; set; }
            public string ContactEmail { get; set; }
            public string LogoURL { get; set; }
            public float Latitude { get; set; }
            public float Longitude { get; set; }
            public string CultureInfoId { get; set; }
            public string CultureInfoName { get; set; }
            public string AllowDomain { get; set; }
            public string ExtAppAuthenticationKey { get; set; }
            public bool DeletedFlag { get; set; }
        }
        public class Format_DetailForExternal
        {
            public string Name { get; set; }
            public string ShortName { get; set; }
            public string Address { get; set; }
            public string CompanyWebSite { get; set; }
            public string ContactName { get; set; }
            public string ContactPhone { get; set; }
            public string ContactEmail { get; set; }
            public string LogoURL { get; set; }
            public float Latitude { get; set; }
            public float Longitude { get; set; }
            public string CultureInfoName { get; set; }
        }
        public class Format_Create
        {
            [Required]
            public string Name { get; set; }
            [MaxLength(10)]
            public string ShortName { get; set; }
            public string Address { get; set; }
            public string CompanyWebSite { get; set; }
            public string ContactName { get; set; }
            public string ContactPhone { get; set; }
            public string ContactEmail { get; set; }
            public float Latitude { get; set; }
            public float Longitude { get; set; }
            public string CultureInfoId { get; set; }
            public string AllowDomain { get; set; }
            public string ExtAppAuthenticationKey { get; set; }
        }
        public class Format_Update
        {
            public string Name { get; set; }
            [MaxLength(10)]
            public string ShortName { get; set; }
            public string Address { get; set; }
            public string CompanyWebSite { get; set; }
            public string ContactName { get; set; }
            public string ContactPhone { get; set; }
            public string ContactEmail { get; set; }
            public float Latitude { get; set; }
            public float Longitude { get; set; }
            public string CultureInfoId { get; set; }
            public string AllowDomain { get; set; }
            public string ExtAppAuthenticationKey { get; set; }
            public string LogoURL { get; set; }
            public bool? DeletedFlag { get; set; }
        }

        public List<Format_Detail> GetAll()
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                var L2Enty = from c in dbEntity.Company.AsNoTracking()
                             where c.DeletedFlag == false
                             select c;
                return L2Enty.Select(s => new Format_Detail()
                {
                    Id = s.Id,
                    Name = s.Name,
                    ShortName = s.ShortName,
                    Address = s.Address,
                    CompanyWebSite = s.CompanyWebSite,
                    ContactName = s.ContactName,
                    ContactPhone = s.ContactPhone,
                    ContactEmail = s.ContactEmail,
                    Latitude = (float)s.Latitude,
                    Longitude = (float)s.Longitude,
                    LogoURL = s.LogoURL,
                    CultureInfoId = s.CultureInfo,
                    CultureInfoName = (s.RefCultureInfo == null ? "" : s.RefCultureInfo.Name),
                    AllowDomain = s.AllowDomain,
                    ExtAppAuthenticationKey = s.ExtAppAuthenticationKey,
                    DeletedFlag = s.DeletedFlag
                }).ToList<Format_Detail>();
            }
        }        

        public Format_Detail GetById(int id)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                Company company = (from c in dbEntity.Company.AsNoTracking()
                                   where c.Id == id && c.DeletedFlag == false
                                   select c).SingleOrDefault<Company>();
                if (company == null)
                    throw new CDSException(10701);

                return new Format_Detail()
                {
                    Id = company.Id,
                    Name = company.Name,
                    ShortName = company.ShortName,
                    Address = company.Address,
                    CompanyWebSite = company.CompanyWebSite,
                    ContactName = company.ContactName,
                    ContactPhone = company.ContactPhone,
                    ContactEmail = company.ContactEmail,
                    Latitude = (float)company.Latitude,
                    Longitude = (float)company.Longitude,
                    LogoURL = company.LogoURL,
                    CultureInfoId = company.CultureInfo,
                    CultureInfoName = (company.RefCultureInfo == null ? "" : company.RefCultureInfo.Name),
                    AllowDomain = company.AllowDomain,
                    ExtAppAuthenticationKey = company.ExtAppAuthenticationKey,
                    DeletedFlag = company.DeletedFlag
                };
            }
        }

        public Format_DetailForExternal GetCompanyByIdForExternal(int id)
        {
            DBHelper._Company dbhelp = new DBHelper._Company();
            Company company = dbhelp.GetByid(id);
            if (company == null)
                throw new CDSException(10701);

            return new Format_DetailForExternal()
            {
                Name = company.Name,
                ShortName = company.ShortName,
                Address = company.Address,
                CompanyWebSite = company.CompanyWebSite,
                ContactName = company.ContactName,
                ContactPhone = company.ContactPhone,
                ContactEmail = company.ContactEmail,
                Latitude = (float)company.Latitude,
                Longitude = (float)company.Longitude,
                LogoURL = company.LogoURL,
                CultureInfoName = (company.RefCultureInfo == null ? "" : company.RefCultureInfo.Name)
            };
        }

        public int Create(Format_Create dataModel)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                Company newCompany = new Company();
                newCompany.Name = dataModel.Name;
                newCompany.ShortName = dataModel.ShortName ?? "";
                newCompany.Address = dataModel.Address ?? "";
                newCompany.CompanyWebSite = dataModel.CompanyWebSite ?? "";
                newCompany.ContactName = dataModel.ContactName ?? "";
                newCompany.ContactPhone = dataModel.ContactPhone ?? "";
                newCompany.ContactEmail = dataModel.ContactEmail ?? "";
                newCompany.Latitude = (float)dataModel.Latitude;
                newCompany.Longitude = (float)dataModel.Longitude;
                newCompany.CultureInfo = dataModel.CultureInfoId;
                newCompany.AllowDomain = dataModel.AllowDomain ?? "";
                newCompany.ExtAppAuthenticationKey = dataModel.ExtAppAuthenticationKey ?? "";

                newCompany.LogoURL = "";
                newCompany.CreatedAt = DateTime.UtcNow;
                newCompany.DeletedFlag = false;

                dbEntity.Company.Add(newCompany);
                dbEntity.SaveChanges();
                return newCompany.Id;
            }
        }

        public void UpdateById(int id, Format_Update dataModel)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                var existingCompany = dbEntity.Company.Find(id);
                if (existingCompany == null || existingCompany.DeletedFlag == true)
                    throw new CDSException(10701);

                if (dataModel.Name != null)
                    existingCompany.Name = dataModel.Name;

                if (dataModel.ShortName != null)
                    existingCompany.ShortName = dataModel.ShortName;

                if (dataModel.Address != null)
                    existingCompany.Address = dataModel.Address;

                if (dataModel.CompanyWebSite != null)
                    existingCompany.CompanyWebSite = dataModel.CompanyWebSite;

                if (dataModel.ContactName != null)
                    existingCompany.ContactName = dataModel.ContactName;

                if (dataModel.ContactEmail != null)
                    existingCompany.ContactEmail = dataModel.ContactEmail;

                if (dataModel.ContactPhone != null)
                    existingCompany.ContactPhone = dataModel.ContactPhone;

                if (dataModel.Latitude != 0)
                    existingCompany.Latitude = dataModel.Latitude;

                if (dataModel.Longitude != 0)
                    existingCompany.Longitude = dataModel.Longitude;

                if (dataModel.CultureInfoId != null)
                    existingCompany.CultureInfo = dataModel.CultureInfoId;

                if (dataModel.AllowDomain != null)
                    existingCompany.AllowDomain = dataModel.AllowDomain;

                if (dataModel.ExtAppAuthenticationKey != null)
                    existingCompany.ExtAppAuthenticationKey = dataModel.ExtAppAuthenticationKey;

                if (dataModel.DeletedFlag.HasValue)
                    existingCompany.DeletedFlag = (bool)dataModel.DeletedFlag;

                existingCompany.UpdatedAt = DateTime.UtcNow;
                dbEntity.SaveChanges();
            }
        }

        public void DeleteById(int id)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                var existingCompany = dbEntity.Company.Find(id);

                if (existingCompany == null)
                    throw new CDSException(10701);

                existingCompany.DeletedFlag = true;
                dbEntity.Entry(existingCompany).State = EntityState.Modified;
                dbEntity.SaveChanges();
            }
        }

        public void UpdateLogoURL(int id, string logoURL, string iconURL)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                var existingCompany = dbEntity.Company.Find(id);
                if (existingCompany == null || existingCompany.DeletedFlag == true)
                    throw new CDSException(10701);

                existingCompany.LogoURL = logoURL;
                existingCompany.IconURL = iconURL;
                dbEntity.Entry(existingCompany).State = EntityState.Modified;
                dbEntity.SaveChanges();
            }
        }

        public int GetIdByExtAppAuthenticationKey(string key)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                Company company = (from c in dbEntity.Company.AsNoTracking()
                                     where c.ExtAppAuthenticationKey == key
                                     select c).SingleOrDefault<Company>();
                if (company == null)
                    return -1;
                else
                    return company.Id;
            }
        }





        public class SubscriptionPlan
        {
            public int Id { get; set; }
            public int SubscriptionPlanID { get; set; }
            public string SubscriptionPlanName { get; set; }
            public string SubscriptionName { get; set; }
            public float RatePer1KMessageIngestion { get; set; }
            public float RatePer1KMessageHotStore { get; set; }
            public float RatePer1KMessageColdStore { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime ExpiredDate { get; set; }
            public int MaxMessageQuotaPerDay { get; set; }
            public string CosmosDBConnectionString { get; set; }
            public int CosmosDBCollectionTTL { get; set; }
            public int CosmosDBCollectionReservedUnits { get; set; }
            public string IoTHubConnectionString { get; set; }
            public string StorageConnectionString { get; set; }
        }

        public List<SubscriptionPlan> GetSubscriptionPlanByCompanyId(int companyId)
        {
            DBHelper.APIService dbhelp = new DBHelper.APIService();
            var subscriptionPlanList = dbhelp.GetSubscriptionPlanByCompanyId(companyId);
            if (subscriptionPlanList == null)
                throw new CDSException(10302);

            return subscriptionPlanList.Select(s => new SubscriptionPlan()
            {
                Id = s.Id,
                SubscriptionPlanID = s.SubscriptionPlanID,
                SubscriptionPlanName = s.SubscriptionPlan.Name,
                SubscriptionName = s.SubscriptionName,
                RatePer1KMessageIngestion = (float)s.RatePer1KMessageIngestion,
                RatePer1KMessageHotStore = (float)s.RatePer1KMessageHotStore,
                RatePer1KMessageColdStore = (float)s.RatePer1KMessageColdStore,
                StartDate = (DateTime)s.StartDate,
                ExpiredDate = (DateTime)s.ExpiredDate,
                MaxMessageQuotaPerDay = (int)s.MaxMessageQuotaPerDay,
                CosmosDBConnectionString = s.CosmosDBConnectionString,
                CosmosDBCollectionTTL = (int)s.CosmosDBCollectionTTL,
                CosmosDBCollectionReservedUnits = (int)s.CosmosDBCollectionReservedUnits,
                IoTHubConnectionString = s.IoTHubConnectionString,
                StorageConnectionString = s.StorageConnectionString
            }).ToList<SubscriptionPlan>();
        }
        public SubscriptionPlan GetValidSubscriptionPlanByCompanyId(int companyId)
        {
            DBHelper.APIService dbhelp = new DBHelper.APIService();
            var subscriptionPlanList = dbhelp.GetValidSubscriptionPlanByCompanyId(companyId);
            if (subscriptionPlanList == null || subscriptionPlanList.Count == 0)
                throw new CDSException(10303);

            return subscriptionPlanList.Select(s => new SubscriptionPlan()
            {
                Id = s.Id,
                SubscriptionPlanID = s.SubscriptionPlanID,
                SubscriptionPlanName = s.SubscriptionPlan.Name,
                SubscriptionName = s.SubscriptionName,
                RatePer1KMessageIngestion = (float)s.RatePer1KMessageIngestion,
                RatePer1KMessageHotStore = (float)s.RatePer1KMessageHotStore,
                RatePer1KMessageColdStore = (float)s.RatePer1KMessageColdStore,
                StartDate = (DateTime)s.StartDate,
                ExpiredDate = (DateTime)s.ExpiredDate,
                MaxMessageQuotaPerDay = (int)s.MaxMessageQuotaPerDay,
                CosmosDBConnectionString = s.CosmosDBConnectionString,
                CosmosDBCollectionTTL = (int)s.CosmosDBCollectionTTL,
                CosmosDBCollectionReservedUnits = (int)s.CosmosDBCollectionReservedUnits,
                IoTHubConnectionString = s.IoTHubConnectionString,
                StorageConnectionString = s.StorageConnectionString
            }).FirstOrDefault<SubscriptionPlan>();
        }

    }
}