using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel.DataAnnotations;
using sfShareLib;
using Newtonsoft.Json.Linq;
using CDSShareLib.Helper;

namespace sfAPIService.Models
{
    public class EventRuleCatalogModel
    {
        public class Format_Detail
        {
            public int Id { get; set; }
            public int MessageCatalogId { get; set; }
            public string MessageCatalogName { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }            
            public int AggregateInSec { get; set; }
            public bool ActiveFlag { get; set; }
        }

        public class Format_Create
        {
            [Required]
            public int MessageCatalogId { get; set; }
            [Required]
            public string Name { get; set; }
            public string Description { get; set; }
            [Required]
            public int AggregateInSec { get; set; }
            [Required]
            public bool ActiveFlag { get; set; }

        }

        public class Format_Update
        {
            public int? MessageCatalogId { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public int? AggregateInSec { get; set; }
            public bool? ActiveFlag { get; set; }
        }
        public List<Format_Detail> GetAllByCompanyId(int companyId)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                var L2Enty = from c in dbEntity.EventRuleCatalog.AsNoTracking()
                             where c.CompanyId == companyId
                             select c;

                return L2Enty.Select(s => new Format_Detail()
                {
                    Id = s.Id,
                    MessageCatalogId = s.MessageCatalogId,
                    MessageCatalogName = s.MessageCatalog == null ? "" : s.MessageCatalog.Name,
                    Name = s.Name,
                    Description = s.Description ?? "",
                    AggregateInSec = s.AggregateInSec,
                    ActiveFlag = s.ActiveFlag
                }).ToList<Format_Detail>();
            }
        }

        public Format_Detail GetById(int id)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                EventRuleCatalog existingData = (from c in dbEntity.EventRuleCatalog.AsNoTracking()
                                                 where c.Id == id
                                                 select c).SingleOrDefault<EventRuleCatalog>();
                if (existingData == null)
                    throw new CDSException(10701);

                return new Format_Detail()
                {
                    Id = existingData.Id,
                    MessageCatalogId = existingData.MessageCatalogId,
                    Name = existingData.Name,
                    MessageCatalogName = existingData.MessageCatalog == null ? "" : existingData.MessageCatalog.Name,
                    Description = existingData.Description ?? "",
                    AggregateInSec = existingData.AggregateInSec,
                    ActiveFlag = existingData.ActiveFlag
                };
            }
        }

        public int Create(int companyId, Format_Create parseData)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                EventRuleCatalog newData = new EventRuleCatalog();
                newData.CompanyId = companyId;
                newData.MessageCatalogId = parseData.MessageCatalogId;
                newData.Name = parseData.Name;
                newData.Description = parseData.Description ?? "";
                newData.AggregateInSec = parseData.AggregateInSec;
                newData.ActiveFlag = parseData.ActiveFlag;

                dbEntity.EventRuleCatalog.Add(newData);
                dbEntity.SaveChanges();
                return newData.Id;
            }
        }

        public void Update(int id, Format_Update parseData)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                var existingData = dbEntity.EventRuleCatalog.Find(id);
                if (existingData == null)
                    throw new CDSException(10701);

                if (parseData.MessageCatalogId.HasValue)
                    existingData.MessageCatalogId = (int) parseData.MessageCatalogId;

                if (parseData.Name != null)
                    existingData.Name = parseData.Name;

                if (parseData.Description != null)
                    existingData.Description = parseData.Description;

                if (parseData.AggregateInSec.HasValue)
                    existingData.AggregateInSec = (int) parseData.AggregateInSec;

                if (parseData.ActiveFlag.HasValue)
                    existingData.ActiveFlag = (bool)parseData.ActiveFlag;

                dbEntity.SaveChanges();
            }
        }

        public void DeleteById(int id)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                EventRuleCatalog existingData = dbEntity.EventRuleCatalog.Find(id);
                if (existingData == null)
                    throw new CDSException(10701);

                dbEntity.EventRuleCatalog.Remove(existingData);
                dbEntity.SaveChanges();
            }
        }
    }
}