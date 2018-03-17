using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Dynamic;

using sfShareLib;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Infrastructure;
using CDSShareLib.Helper;

namespace sfAPIService.Models
{
    public class DeviceCommandCatalogModel
    {
        public class Format_Detail
        {
            public int Id { get; set; }
            public string Method { get; set; }
            public string Name { get; set; }
            public string Content { get; set; }
        }
        public class Format_Create
        {
            [Required]
            public string Name { get; set; }
            [Required]
            public string Method { get; set; }
            [Required]
            public string Content { get; set; }
        }
        public class Format_Update
        {
            public string Name { get; set; }
            public string Method { get; set; }
            public string Content { get; set; }
        }
        
        public List<Format_Detail> GetAllByCompanyId(int companyId)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                var L2Enty = from c in dbEntity.DeviceCommandCatalog.AsNoTracking()
                             where c.CompanyId == companyId
                             select c;

                return L2Enty.Select(s => new Format_Detail()
                {
                    Id = s.Id,
                    Name = s.Name,
                    Content = s.Content,
                    Method = s.Method
                }).ToList<Format_Detail>();
            }
        }

        public Format_Detail GetById(int id)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                DeviceCommandCatalog existingData = (from c in dbEntity.DeviceCommandCatalog.AsNoTracking()
                                               where c.Id == id
                                               select c).SingleOrDefault<DeviceCommandCatalog>();
                if (existingData == null)
                    throw new CDSException(10403);

                return new Format_Detail()
                {
                    Id = existingData.Id,
                    Name = existingData.Name,
                    Content = existingData.Content,
                    Method = existingData.Method
                };
            }
        }

        public int Create(int companyId, Format_Create parseData)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                DeviceCommandCatalog newData = new DeviceCommandCatalog();
                newData.CompanyId = companyId;
                newData.Name = parseData.Name;
                newData.Method = parseData.Method;
                newData.Content = parseData.Content ?? "";

                dbEntity.DeviceCommandCatalog.Add(newData);
                try
                {
                    dbEntity.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
                    if (ex.InnerException.InnerException.Message.Contains("Cannot insert duplicate key"))
                        throw new CDSException(10404);
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
                var existingData = dbEntity.DeviceCommandCatalog.Find(id);
                if (existingData == null)
                    throw new CDSException(10403);

                existingData.Name = parseData.Name;
                existingData.Method = parseData.Method;
                existingData.Content = parseData.Content;

                try
                {
                    dbEntity.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
                    if (ex.InnerException.InnerException.Message.Contains("Cannot insert duplicate key"))
                        throw new CDSException(10404);
                    else
                        throw ex;
                }
            }
        }

        public void DeleteById(int id)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                DeviceCommandCatalog existingData = dbEntity.DeviceCommandCatalog.Find(id);
                if (existingData == null)
                    throw new CDSException(10403);

                dbEntity.DeviceCommandCatalog.Remove(existingData);
                dbEntity.SaveChanges();
            }
        }
    }
}
