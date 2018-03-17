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
    public class SubscriptionPlanModel
    {
        public class Format_Detail
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public double DefaultRatePer1KMessageIngestion { get; set; }
            public double DefaultRatePer1KMessageHotStore { get; set; }
            public double DefaultRatePer1KMessageColdStore { get; set; }
            public int DefaultPlanDays { get; set; }
            public int DefaultMaxMessageQuotaPerDay { get; set; }
            public bool DefaultStoreHotMessage { get; set; }
            public bool DefaultStoreColdMessage { get; set; }
            public string DefaultCosmosDBConnectionString { get; set; }
            public int DefaultCollectionTTL { get; set; }
            public int DefaultCollectionReservedUnits { get; set; }
            public string DefaultIoTHubConnectionString { get; set; }
            public string DefaultStorageConnectionString { get; set; }
        }

        public class Format_Create
        {
            [Required]
            public string Name { get; set; }
            public string Description { get; set; }
            public double? DefaultRatePer1KMessageIngestion { get; set; }
            public double? DefaultRatePer1KMessageHotStore { get; set; }
            public double? DefaultRatePer1KMessageColdStore { get; set; }
            [Required]
            public int DefaultPlanDays { get; set; }
            [Required]
            public int DefaultMaxMessageQuotaPerDay { get; set; }
            public bool? DefaultStoreHotMessage { get; set; }
            public bool? DefaultStoreColdMessage { get; set; }
            public string DefaultCosmosDBConnectionString { get; set; }
            public int? DefaultCollectionTTL { get; set; }
            public int? DefaultCollectionReservedUnits { get; set; }
            public string DefaultIoTHubConnectionString { get; set; }
            public string DefaultStorageConnectionString { get; set; }
        }
        public class Format_Update
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public double? DefaultRatePer1KMessageIngestion { get; set; }
            public double? DefaultRatePer1KMessageHotStore { get; set; }
            public double? DefaultRatePer1KMessageColdStore { get; set; }
            public int? DefaultPlanDays { get; set; }
            public int? DefaultMaxMessageQuotaPerDay { get; set; }
            public bool? DefaultStoreHotMessage { get; set; }
            public bool? DefaultStoreColdMessage { get; set; }
            public string DefaultCosmosDBConnectionString { get; set; }
            public int? DefaultCollectionTTL { get; set; }
            public int? DefaultCollectionReservedUnits { get; set; }
            public string DefaultIoTHubConnectionString { get; set; }
            public string DefaultStorageConnectionString { get; set; }
        }
        public List<Format_Detail> GetAll()
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                var L2Enty = from c in dbEntity.SubscriptionPlan.AsNoTracking()
                             select c;

                return L2Enty.Select(s => new Format_Detail()
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description,
                    DefaultRatePer1KMessageIngestion = (double)s.DefaultRatePer1KMessageIngestion,
                    DefaultRatePer1KMessageHotStore = (double)s.DefaultRatePer1KMessageHotStore,
                    DefaultRatePer1KMessageColdStore = (double)s.DefaultRatePer1KMessageColdStore,
                    DefaultPlanDays = s.DefaultPlanDays,
                    DefaultMaxMessageQuotaPerDay = s.DefaultMaxMessageQuotaPerDay,
                    DefaultStoreHotMessage = (bool)s.DefaultStoreHotMessage,
                    DefaultStoreColdMessage = (bool)s.DefaultStoreColdMessage,
                    DefaultCosmosDBConnectionString = s.DefaultCosmosDBConnectionString,
                    DefaultCollectionTTL = (int)s.DefaultCollectionTTL,
                    DefaultCollectionReservedUnits = (int)s.DefaultCollectionReservedUnits,
                    DefaultIoTHubConnectionString = s.DefaultIoTHubConnectionString,
                    DefaultStorageConnectionString = s.DefaultStorageConnectionString
                }).ToList<Format_Detail>();
            }
        }

        public Format_Detail GetById(int id)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                SubscriptionPlan existingData = (from c in dbEntity.SubscriptionPlan.AsNoTracking()
                                                 where c.Id == id
                                                 select c).SingleOrDefault<SubscriptionPlan>();
                if (existingData == null)
                    throw new CDSException(10701);

                return new Format_Detail()
                {
                    Id = existingData.Id,
                    Name = existingData.Name,
                    Description = existingData.Description,
                    DefaultRatePer1KMessageIngestion = (double)existingData.DefaultRatePer1KMessageIngestion,
                    DefaultRatePer1KMessageHotStore = (double)existingData.DefaultRatePer1KMessageHotStore,
                    DefaultRatePer1KMessageColdStore = (double)existingData.DefaultRatePer1KMessageColdStore,
                    DefaultPlanDays = existingData.DefaultPlanDays,
                    DefaultMaxMessageQuotaPerDay = existingData.DefaultMaxMessageQuotaPerDay,
                    DefaultStoreHotMessage = (bool)existingData.DefaultStoreHotMessage,
                    DefaultStoreColdMessage = (bool)existingData.DefaultStoreColdMessage,
                    DefaultCosmosDBConnectionString = existingData.DefaultCosmosDBConnectionString,
                    DefaultCollectionTTL = (int)existingData.DefaultCollectionTTL,
                    DefaultCollectionReservedUnits = (int)existingData.DefaultCollectionReservedUnits,
                    DefaultIoTHubConnectionString = existingData.DefaultIoTHubConnectionString,
                    DefaultStorageConnectionString = existingData.DefaultStorageConnectionString
                };
            }
        }

        public int Create(Format_Create parseData)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                SubscriptionPlan newData = new SubscriptionPlan();
                newData.Name = parseData.Name;
                newData.Description = parseData.Description ?? "";
                newData.DefaultRatePer1KMessageIngestion = parseData.DefaultRatePer1KMessageIngestion ?? 0;
                newData.DefaultRatePer1KMessageHotStore = parseData.DefaultRatePer1KMessageHotStore ?? 0;
                newData.DefaultRatePer1KMessageColdStore = parseData.DefaultRatePer1KMessageColdStore ?? 0;
                newData.DefaultPlanDays = parseData.DefaultPlanDays;
                newData.DefaultMaxMessageQuotaPerDay = parseData.DefaultMaxMessageQuotaPerDay;
                newData.DefaultStoreHotMessage = parseData.DefaultStoreHotMessage ?? false;
                newData.DefaultStoreColdMessage = parseData.DefaultStoreColdMessage ?? false;
                newData.DefaultCosmosDBConnectionString = parseData.DefaultCosmosDBConnectionString ?? "";
                newData.DefaultCollectionTTL = parseData.DefaultCollectionTTL ?? 86400;
                newData.DefaultCollectionReservedUnits = parseData.DefaultCollectionReservedUnits ?? 400;
                newData.DefaultIoTHubConnectionString = parseData.DefaultIoTHubConnectionString ?? "";
                newData.DefaultStorageConnectionString = parseData.DefaultStorageConnectionString ?? "";

                dbEntity.SubscriptionPlan.Add(newData);
                dbEntity.SaveChanges();
                return newData.Id;
            }
        }

        public void Update(int id, Format_Update parseData)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                var existingData = dbEntity.SubscriptionPlan.Find(id);
                if (existingData == null)
                    throw new CDSException(10701);

                if (parseData.Name != null)
                    existingData.Name = parseData.Name;

                if (parseData.Description != null)
                    existingData.Description = parseData.Description;

                if (parseData.DefaultRatePer1KMessageIngestion.HasValue)
                    existingData.DefaultRatePer1KMessageIngestion = parseData.DefaultRatePer1KMessageIngestion;

                if (parseData.DefaultRatePer1KMessageHotStore.HasValue)
                    existingData.DefaultRatePer1KMessageHotStore = parseData.DefaultRatePer1KMessageHotStore;

                if (parseData.DefaultRatePer1KMessageHotStore.HasValue)
                    existingData.DefaultRatePer1KMessageColdStore = parseData.DefaultRatePer1KMessageColdStore;

                if (parseData.DefaultPlanDays.HasValue)
                    existingData.DefaultPlanDays = (int)parseData.DefaultPlanDays;

                if (parseData.DefaultMaxMessageQuotaPerDay.HasValue)
                    existingData.DefaultMaxMessageQuotaPerDay = (int)parseData.DefaultMaxMessageQuotaPerDay;

                if (parseData.DefaultStoreHotMessage.HasValue)
                    existingData.DefaultStoreHotMessage = parseData.DefaultStoreHotMessage;

                if (parseData.DefaultStoreColdMessage.HasValue)
                    existingData.DefaultStoreColdMessage = parseData.DefaultStoreColdMessage;

                if (parseData.DefaultCosmosDBConnectionString != null)
                    existingData.DefaultCosmosDBConnectionString = parseData.DefaultCosmosDBConnectionString;

                if (parseData.DefaultCollectionTTL.HasValue)
                    existingData.DefaultCollectionTTL = parseData.DefaultCollectionTTL;

                if (parseData.DefaultCollectionReservedUnits.HasValue)
                    existingData.DefaultCollectionReservedUnits = parseData.DefaultCollectionReservedUnits;

                if (parseData.DefaultIoTHubConnectionString != null)
                    existingData.DefaultIoTHubConnectionString = parseData.DefaultIoTHubConnectionString ?? "";

                if (parseData.DefaultStorageConnectionString != null)
                    existingData.DefaultStorageConnectionString = parseData.DefaultStorageConnectionString ?? "";
                
                dbEntity.SaveChanges();
            }
        }

        public void DeleteById(int id)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                SubscriptionPlan existingData = dbEntity.SubscriptionPlan.Find(id);
                if (existingData == null)
                    throw new CDSException(10701);

                dbEntity.SubscriptionPlan.Remove(existingData);
                dbEntity.SaveChanges();
            }
        }
    }
}