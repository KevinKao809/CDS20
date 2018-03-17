using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using sfShareLib;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using CDSShareLib.Helper;

namespace sfAPIService.Models
{
    public class EquipmentClassModel
    {
        public class Format_Detail
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
        }
        public class Format_Create
        {
            [Required]
            public string Name { get; set; }
            public string Description { get; set; }
        }
        public class Format_Update
        {
            public string Name { get; set; }
            public string Description { get; set; }
        }

        public List<Format_Detail> GetAllByCompanyId(int companyId)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                var L2Enty = from c in dbEntity.EquipmentClass.AsNoTracking()
                             where c.CompanyId == companyId
                             select c;

                return L2Enty.Select(s => new Format_Detail()
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description
                }).ToList<Format_Detail>();
            }
        }

        public Format_Detail GetById(int id)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                var existingData = (from c in dbEntity.EquipmentClass.AsNoTracking()
                                                 where c.Id == id
                                                 select c).SingleOrDefault<EquipmentClass>();

                if (existingData == null)
                    throw new CDSException(10701);

                return new Format_Detail()
                {
                    Id = existingData.Id,
                    Name = existingData.Name,
                    Description = existingData.Description
                };
            }
        }

        public int Create(int companyId, Format_Create parseData)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                EquipmentClass newData = new EquipmentClass();
                newData.CompanyId = companyId;
                newData.Name = parseData.Name;
                newData.Description = parseData.Description ?? "";

                dbEntity.EquipmentClass.Add(newData);
                dbEntity.SaveChanges();
                return newData.Id;
            }
        }

        public void Update(int id, Format_Update parseData)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                var existingData = dbEntity.EquipmentClass.Find(id);
                if (existingData == null)
                    throw new CDSException(10701);

                if (parseData.Name != null)
                    existingData.Name = parseData.Name;

                if (parseData.Description != null)
                    existingData.Description = parseData.Description;                

                dbEntity.SaveChanges();
            }
        }

        public void DeleteById(int id)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                EquipmentClass existingData = dbEntity.EquipmentClass.Find(id);
                if (existingData == null)
                    throw new CDSException(10701);

                dbEntity.Entry(existingData).State = EntityState.Deleted;
                dbEntity.SaveChanges();
            }
        }
    }
}