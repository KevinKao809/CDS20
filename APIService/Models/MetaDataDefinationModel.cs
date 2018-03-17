using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

using sfShareLib;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using CDSShareLib.Helper;

namespace sfAPIService.Models
{
    public class MetaDataDefinationModel
    {
        public class Format_Detail
        {
            public int Id { get; set; }
            public string EntityType { get; set; }
            public string ObjectName { get; set; }
        }
        public class Format_Create
        {
            [Required]
            public string EntityType { get; set; }
            [Required]
            public string ObjectName { get; set; }
        }
        public class Format_Update
        {
            public string EntityType { get; set; }
            public string ObjectName { get; set; }
        }
        public List<Format_Detail> GetAllByCompanyId(int companyId)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                var L2Enty = from c in dbEntity.MetaDataDefination.AsNoTracking()
                             where c.CompanyId == companyId
                             select c;

                return L2Enty.Select(s => new Format_Detail
                {
                    Id = s.Id,
                    EntityType = s.EntityType,
                    ObjectName = s.ObjectName
                }).ToList<Format_Detail>();
            }
        }

        public Format_Detail GetById(int id, int companyId)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                MetaDataDefination existingData = (from c in dbEntity.MetaDataDefination.AsNoTracking()
                                                  where c.Id == id && c.CompanyId == companyId
                                                  select c).SingleOrDefault<MetaDataDefination>();

                if (existingData == null)
                    throw new CDSException(11301);

                return new Format_Detail()
                {
                    Id = existingData.Id,
                    EntityType = existingData.EntityType,
                    ObjectName = existingData.ObjectName
                };
            }
        }

        public int Create(int companyId, Format_Create parseData)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                MetaDataDefination newData = new MetaDataDefination()
                {
                    CompanyId = companyId,
                    EntityType = parseData.EntityType,
                    ObjectName = parseData.ObjectName
                };
                dbEntity.MetaDataDefination.Add(newData);
                try
                {
                    dbEntity.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
                    if (ex.InnerException.InnerException.Message.Contains("Cannot insert duplicate key"))
                        throw new CDSException(11902);
                    else
                        throw ex;
                }
                return newData.Id;
            }
        }

        public void Update(int id, Format_Update parseData, int companyId)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                //MetaDataDefination existingData = dbEntity.MetaDataDefination.Find(id);
                MetaDataDefination existingData = (from c in dbEntity.MetaDataDefination
                                                  where c.Id == id && c.CompanyId == companyId
                                                  select c).SingleOrDefault<MetaDataDefination>();
                if (existingData == null)
                    throw new CDSException(11301);

                if (parseData.EntityType != null)
                    existingData.EntityType = parseData.EntityType;
                
                if (parseData.ObjectName != null)
                    existingData.ObjectName = parseData.ObjectName;

                dbEntity.Entry(existingData).State = EntityState.Modified;
                try
                {
                    dbEntity.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
                    if (ex.InnerException.InnerException.Message.Contains("Cannot insert duplicate key"))
                        throw new CDSException(11302);
                    else
                        throw ex;
                }
            }
        }

        public void DeleteById(int id, int companyId)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                MetaDataDefination existingData = (from c in dbEntity.MetaDataDefination
                                                  where c.Id == id && c.CompanyId == companyId
                                                  select c).SingleOrDefault<MetaDataDefination>();
                if (existingData == null)
                    throw new CDSException(11301);

                dbEntity.Entry(existingData).State = EntityState.Deleted;
                dbEntity.SaveChanges();
            }
        }
    }
}