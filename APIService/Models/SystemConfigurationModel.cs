using CDSShareLib.Helper;
using sfShareLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;

namespace sfAPIService.Models
{
    public class SystemConfigurationModel
    {
        public class Format_Detail
        {
            public int Id { get; set; }
            public string Group { get; set; }
            public string Key { get; set; }
            public string Value { get; set; }
        }
        public class Format_Create
        {
            public string Group { get; set; }
            [Required]
            public string Key { get; set; }
            [Required]
            public string Value { get; set; }
        }
        public class Format_Update
        {
            public string Group { get; set; }
            public string Key { get; set; }
            public string Value { get; set; }
        }
        public List<Format_Detail> GetAll(int companyId)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                var L2Enty = from c in dbEntity.SystemConfiguration.AsNoTracking()
                             orderby c.Group
                             select c;

                return L2Enty.Select(s => new Format_Detail
                {
                    Id = s.Id,
                    Group = s.Group,
                    Key = s.Key,
                    Value = s.Value
                }).ToList<Format_Detail>();
            }
        }

        public Format_Detail GetById(int id)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                SystemConfiguration existingData = (from c in dbEntity.SystemConfiguration.AsNoTracking()
                                                    where c.Id == id
                                                    select c).SingleOrDefault<SystemConfiguration>();

                if (existingData == null)
                    throw new CDSException(11901);

                return new Format_Detail()
                {
                    Id = existingData.Id,
                    Group = existingData.Group,
                    Key = existingData.Key,
                    Value = existingData.Value
                };
            }
        }

        public int Create(int companyId, Format_Create parseData)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                SystemConfiguration newData = new SystemConfiguration()
                {
                    Group = parseData.Group ?? "",
                    Key = parseData.Key,
                    Value = parseData.Value
                };
                dbEntity.SystemConfiguration.Add(newData);
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

        public void Update(int id, Format_Update parseData)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                SystemConfiguration existingData = dbEntity.SystemConfiguration.Find(id);
                if (existingData == null)
                    throw new CDSException(11901);

                if (parseData.Group != null)
                    existingData.Group = parseData.Group;

                if (parseData.Key != null)
                    existingData.Key = parseData.Key;

                if (parseData.Value != null)
                    existingData.Value = parseData.Value;

                dbEntity.Entry(existingData).State = EntityState.Modified;
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
            }
        }

        public void DeleteById(int id)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                SystemConfiguration existingData = dbEntity.SystemConfiguration.Find(id);
                if (existingData == null)
                    throw new CDSException(11901);

                dbEntity.Entry(existingData).State = EntityState.Deleted;
                dbEntity.SaveChanges();
            }
        }
    }
}