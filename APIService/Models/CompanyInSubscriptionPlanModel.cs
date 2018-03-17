using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using sfShareLib;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Data.Entity;
using CDSShareLib.Helper;

namespace sfAPIService.Models
{
    public class CompanyInSubscriptionPlanModel
    {
        public class Format_Detail
        {
            public int Id { get; set; }
            public int SubscriptionPlanId { get; set; }
            public string SubscriptionName { get; set; }


            public double RatePer1KMessageIngestion { get; set; }
            public double RatePer1KMessageHotStore { get; set; }
            public double RatePer1KMessageColdStore { get; set; }
            
            public DateTime StartDate { get; set; }
            public DateTime ExpiredDate { get; set; }
            
            public int MaxMessageQuotaPerDay { get; set; }
            public bool StoreHotMessage { get; set; }
            public bool StoreColdMessage { get; set; }

            public string CosmosDBConnectionString { get; set; }
            public string CosmosDBName { get; set; }
            public string CosmosDBColletionId { get; set; }
            public int CosmosDBCollectionTTL { get; set; }
            public int CosmosDBCollectionReservedUnits { get; set; }
            
            public string IoTHubConnectionString { get; set; }
            public string IoTHubConsumerGroup { get; set; }
            public string StorageConnectionString { get; set; }
        }

        public class Format_Create
        {
            [Required]
            public int SubscriptionPlanId { get; set; }
            public string SubscriptionName { get; set; }


            public double RatePer1KMessageIngestion { get; set; }
            public double RatePer1KMessageHotStore { get; set; }
            public double RatePer1KMessageColdStore { get; set; }

            [Required]
            public DateTime StartDate { get; set; }
            [Required]
            public DateTime ExpiredDate { get; set; }

            [Required]
            public int MaxMessageQuotaPerDay { get; set; }
            public bool StoreHotMessage { get; set; }
            public bool StoreColdMessage { get; set; }

            [Required]
            public string CosmosDBConnectionString { get; set; }
            [Required]
            public string CosmosDBName { get; set; }
            [Required]
            public string CosmosDBColletionId { get; set; }
            public int CosmosDBCollectionTTL { get; set; }
            public int CosmosDBCollectionReservedUnits { get; set; }

            [Required]
            public string IoTHubConnectionString { get; set; }
            [Required]
            public string IoTHubConsumerGroup { get; set; }
            public string StorageConnectionString { get; set; }
        }
        public class Format_Update
        {
            public int? SubscriptionPlanId { get; set; }
            public string SubscriptionName { get; set; }
            public double? RatePer1KMessageIngestion { get; set; }
            public double? RatePer1KMessageHotStore { get; set; }
            public double? RatePer1KMessageColdStore { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? ExpiredDate { get; set; }
            public int? MaxMessageQuotaPerDay { get; set; }
            public bool? StoreHotMessage { get; set; }
            public bool? StoreColdMessage { get; set; }
            public string CosmosDBConnectionString { get; set; }
            public string CosmosDBName { get; set; }
            public string CosmosDBColletionId { get; set; }
            public int? CosmosDBCollectionTTL { get; set; }
            public int? CosmosDBCollectionReservedUnits { get; set; }
            public string IoTHubConnectionString { get; set; }
            public string IoTHubConsumerGroup { get; set; }
            public string StorageConnectionString { get; set; }
        }
        public List<Format_Detail> GetAllByCompanyId(int companyId)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                var L2Enty = from c in dbEntity.CompanyInSubscriptionPlan.AsNoTracking()
                             where c.CompanyID == companyId
                             select c;

                return L2Enty.Select(s => new Format_Detail()
                {
                    Id = s.Id,
                    SubscriptionPlanId = s.SubscriptionPlanID,
                    SubscriptionName = s.SubscriptionName,
                    RatePer1KMessageIngestion = (double)s.RatePer1KMessageIngestion,
                    RatePer1KMessageHotStore = (double)s.RatePer1KMessageHotStore,
                    RatePer1KMessageColdStore = (double)s.RatePer1KMessageColdStore,

                    StartDate = s.StartDate,
                    ExpiredDate = s.ExpiredDate,
                    MaxMessageQuotaPerDay = s.MaxMessageQuotaPerDay,
                    StoreHotMessage = s.StoreHotMessage,
                    StoreColdMessage = s.StoreColdMessage,

                    CosmosDBConnectionString = s.CosmosDBConnectionString,
                    CosmosDBName = s.CosmosDBName,
                    CosmosDBColletionId = s.CosmosDBCollectionID,
                    CosmosDBCollectionTTL = (int)s.CosmosDBCollectionTTL,
                    CosmosDBCollectionReservedUnits = (int)s.CosmosDBCollectionReservedUnits,

                    IoTHubConnectionString = s.IoTHubConnectionString,
                    IoTHubConsumerGroup = s.IoTHubConsumerGroup,
                    StorageConnectionString = s.StorageConnectionString
                }).ToList<Format_Detail>();
            }
        }

        public Format_Detail Get(int companyId, int subscriptionId)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                CompanyInSubscriptionPlan existingData = (from c in dbEntity.CompanyInSubscriptionPlan.AsNoTracking()
                                                          where c.Id == subscriptionId && c.CompanyID == companyId
                                                          select c).SingleOrDefault<CompanyInSubscriptionPlan>();
                if (existingData == null)
                    throw new CDSException(10302);

                return new Format_Detail()
                {
                    Id = existingData.Id,
                    SubscriptionPlanId = existingData.SubscriptionPlanID,
                    SubscriptionName = existingData.SubscriptionName,
                    RatePer1KMessageIngestion = (double)existingData.RatePer1KMessageIngestion,
                    RatePer1KMessageHotStore = (double)existingData.RatePer1KMessageHotStore,
                    RatePer1KMessageColdStore = (double)existingData.RatePer1KMessageColdStore,

                    StartDate = existingData.StartDate,
                    ExpiredDate = existingData.ExpiredDate,
                    MaxMessageQuotaPerDay = existingData.MaxMessageQuotaPerDay,
                    StoreHotMessage = existingData.StoreHotMessage,
                    StoreColdMessage = existingData.StoreColdMessage,

                    CosmosDBConnectionString = existingData.CosmosDBConnectionString,
                    CosmosDBName = existingData.CosmosDBName,
                    CosmosDBColletionId = existingData.CosmosDBCollectionID,
                    CosmosDBCollectionTTL = (int)existingData.CosmosDBCollectionTTL,
                    CosmosDBCollectionReservedUnits = (int)existingData.CosmosDBCollectionReservedUnits,

                    IoTHubConnectionString = existingData.IoTHubConnectionString,
                    IoTHubConsumerGroup = existingData.IoTHubConsumerGroup,
                    StorageConnectionString = existingData.StorageConnectionString
                };
            }
        }

        public int Create(int companyId, Format_Create parseData)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                CompanyInSubscriptionPlan newData = new CompanyInSubscriptionPlan();
                newData.CompanyID = companyId;
                newData.SubscriptionPlanID = parseData.SubscriptionPlanId;
                newData.SubscriptionName = parseData.SubscriptionName ?? "";
                newData.RatePer1KMessageIngestion = parseData.RatePer1KMessageIngestion;
                newData.RatePer1KMessageHotStore = parseData.RatePer1KMessageHotStore;
                newData.RatePer1KMessageColdStore = parseData.RatePer1KMessageColdStore;

                newData.StartDate = parseData.StartDate;
                newData.ExpiredDate = parseData.ExpiredDate;

                newData.MaxMessageQuotaPerDay = parseData.MaxMessageQuotaPerDay;

                newData.StoreHotMessage = parseData.StoreHotMessage;
                newData.StoreColdMessage = parseData.StoreColdMessage;

                newData.CosmosDBConnectionString = parseData.CosmosDBConnectionString ?? "";
                newData.CosmosDBName = parseData.CosmosDBName;
                newData.CosmosDBCollectionID = parseData.CosmosDBColletionId;
                newData.CosmosDBCollectionTTL = parseData.CosmosDBCollectionTTL;
                newData.CosmosDBCollectionReservedUnits = parseData.CosmosDBCollectionReservedUnits;

                newData.IoTHubConnectionString = parseData.IoTHubConnectionString ?? "";
                newData.IoTHubConsumerGroup = parseData.IoTHubConsumerGroup ?? "";
                newData.StorageConnectionString = parseData.StorageConnectionString ?? "";

                dbEntity.CompanyInSubscriptionPlan.Add(newData);
                dbEntity.SaveChanges();
                return newData.Id;
            }
        }

        public void Update(int companyId, int subscriptionId, Format_Update parseData)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                CompanyInSubscriptionPlan existingData = (from c in dbEntity.CompanyInSubscriptionPlan
                                                          where c.Id == subscriptionId && c.CompanyID == companyId
                                                          select c).SingleOrDefault<CompanyInSubscriptionPlan>();

                if (existingData == null)
                    throw new CDSException(10302);

                if (parseData.SubscriptionPlanId != null)
                    existingData.SubscriptionPlanID = (int)parseData.SubscriptionPlanId;

                if (parseData.SubscriptionName != null)
                    existingData.SubscriptionName = parseData.SubscriptionName;

                if (parseData.RatePer1KMessageIngestion.HasValue)
                    existingData.RatePer1KMessageIngestion = (double)parseData.RatePer1KMessageIngestion;

                if (parseData.RatePer1KMessageHotStore.HasValue)
                    existingData.RatePer1KMessageHotStore = (double)parseData.RatePer1KMessageHotStore;

                if (parseData.RatePer1KMessageHotStore.HasValue)
                    existingData.RatePer1KMessageColdStore = (double)parseData.RatePer1KMessageColdStore;

                if (parseData.StartDate.HasValue)
                    existingData.StartDate = (DateTime)parseData.StartDate;
                if (parseData.ExpiredDate.HasValue)
                    existingData.ExpiredDate = (DateTime)parseData.ExpiredDate;


                if (parseData.MaxMessageQuotaPerDay.HasValue)
                    existingData.MaxMessageQuotaPerDay = (int)parseData.MaxMessageQuotaPerDay;

                if (parseData.StoreHotMessage.HasValue)
                    parseData.StoreHotMessage = (bool)parseData.StoreHotMessage;

                if (parseData.StoreColdMessage.HasValue)
                    existingData.StoreColdMessage = (bool)parseData.StoreColdMessage;

                if (parseData.CosmosDBConnectionString != null)
                    existingData.CosmosDBConnectionString = parseData.CosmosDBConnectionString;

                if (parseData.CosmosDBName != null)
                    existingData.CosmosDBName = parseData.CosmosDBName;

                if (parseData.CosmosDBColletionId != null)
                    existingData.CosmosDBCollectionID = parseData.CosmosDBColletionId;

                if (parseData.CosmosDBCollectionTTL.HasValue)
                    existingData.CosmosDBCollectionTTL = (int)parseData.CosmosDBCollectionTTL;                

                if (parseData.CosmosDBCollectionReservedUnits.HasValue)
                    existingData.CosmosDBCollectionReservedUnits = (int)parseData.CosmosDBCollectionReservedUnits;

                if (parseData.IoTHubConnectionString != null)
                    existingData.IoTHubConnectionString = parseData.IoTHubConnectionString ?? "";

                if (parseData.IoTHubConsumerGroup != null)
                    existingData.IoTHubConsumerGroup = parseData.IoTHubConsumerGroup ?? "";

                if (parseData.StorageConnectionString != null)
                    existingData.StorageConnectionString = existingData.StorageConnectionString ?? "";
                
                dbEntity.SaveChanges();
            }
        }

        public void Delete(int companyId, int subscriptionId)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                CompanyInSubscriptionPlan existingData = (from c in dbEntity.CompanyInSubscriptionPlan
                                                          where c.Id == subscriptionId && c.CompanyID == companyId
                                                          select c).SingleOrDefault<CompanyInSubscriptionPlan>();
                if (existingData == null)
                    throw new CDSException(10302);

                dbEntity.CompanyInSubscriptionPlan.Remove(existingData);
                dbEntity.SaveChanges();
            }
        }
    }
}