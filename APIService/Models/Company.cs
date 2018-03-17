using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

using sfShareLib;
using System.ComponentModel.DataAnnotations;
using CDSShareLib.Helper;

namespace sfAPIService.Models
{
    public class CompanyModel_old
    {
        public class Detail
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

        public class Detail_readonly
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

        public class Add
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
        public class Update
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
            public bool? DeletedFlag { get; set; }
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

        public List<Detail> getAllCompanies()
        {
            DBHelper._Company dbhelp = new DBHelper._Company();

            var companyList = dbhelp.GetAll();
            if (companyList == null)
                throw new CDSException(10301);

            return companyList.Select(s => new Detail()
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
            }).ToList<Detail>();
        }

        public List<Detail> getAllCompaniesBySuperAdmin()
        {
            DBHelper._Company dbhelp = new DBHelper._Company();
            var companyList = dbhelp.GetAll();
            if (companyList == null)
                throw new CDSException(10301);

            return companyList.Select(s => new Detail()
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
                DeletedFlag = s.DeletedFlag,
            }).ToList<Detail>();
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

        public Detail getCompanyById(int id)
        {
            DBHelper._Company dbhelp = new DBHelper._Company();
            Company company = dbhelp.GetByid(id);
            if (company == null)
                throw new CDSException(10301);

            return new Detail()
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

        public Detail_readonly getCompanyByIdReadonly(int id)
        {
            DBHelper._Company dbhelp = new DBHelper._Company();
            Company company = dbhelp.GetByid(id);
            if (company == null)
                throw new CDSException(10301);

            return new Detail_readonly()
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

        public int addCompany(Add company)
        {
            DBHelper._Company dbhelp = new DBHelper._Company();
            var newCompany = new Company()
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
                CultureInfo = company.CultureInfoId,
                CreatedAt = DateTime.Parse(DateTime.Now.ToString()),
                DeletedFlag = false,
                AllowDomain = company.AllowDomain,
                ExtAppAuthenticationKey = company.ExtAppAuthenticationKey
            };
            return dbhelp.Add(newCompany);
        }

        public void updateCompany(int id, Update company)
        {
            DBHelper._Company dbhelp = new DBHelper._Company();
            Company existingCompany = dbhelp.GetByid(id);
            if (existingCompany == null)
                throw new CDSException(10301);

            existingCompany.Name = company.Name;
            existingCompany.ShortName = company.ShortName;
            existingCompany.Address = company.Address;
            existingCompany.CompanyWebSite = company.CompanyWebSite;
            existingCompany.ContactName = company.ContactName;
            existingCompany.ContactEmail = company.ContactEmail;
            existingCompany.ContactPhone = company.ContactPhone;
            existingCompany.Latitude = company.Latitude;
            existingCompany.Longitude = company.Longitude;
            existingCompany.CultureInfo = company.CultureInfoId;
            existingCompany.AllowDomain = company.AllowDomain;
            existingCompany.ExtAppAuthenticationKey = company.ExtAppAuthenticationKey;
            if (company.DeletedFlag.HasValue)
                existingCompany.DeletedFlag = (bool)company.DeletedFlag;

            dbhelp.Update(existingCompany);
        }

        public void deleteCompany(int id)
        {
            DBHelper._Company dbhelp = new DBHelper._Company();
            Company existingCompany = dbhelp.GetByid(id);
            if (existingCompany == null)
                throw new CDSException(10301);

            dbhelp.Delete(existingCompany);
        }

        public void updateCompanyLogoURL(int id, string url)
        {
            DBHelper._Company dbhelp = new DBHelper._Company();
            Company existingCompany = dbhelp.GetByid(id);
            if (existingCompany == null)
                throw new CDSException(10301);

            existingCompany.LogoURL = url;
            dbhelp.Update(existingCompany);
        }
    }
}