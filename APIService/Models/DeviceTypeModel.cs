using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using sfShareLib;
using System.ComponentModel.DataAnnotations;
using CDSShareLib.Helper;

namespace sfAPIService.Models
{
    public class DeviceTypeModel
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
        public List<Format_Detail> GetAll()
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                var L2Enty = from c in dbEntity.DeviceType.AsNoTracking()
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
                DeviceType DeviceType = (from c in dbEntity.DeviceType.AsNoTracking()
                                         where c.Id == id
                                         select c).SingleOrDefault<DeviceType>();
                if (DeviceType == null)
                    throw new CDSException(10701);

                return new Format_Detail()
                {
                    Id = DeviceType.Id,
                    Name = DeviceType.Name,
                    Description = DeviceType.Description
                };
            }
        }

        public int Create(Format_Create parseData)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                DeviceType newDeviceType = new DeviceType();
                newDeviceType.Name = parseData.Name;
                newDeviceType.Description = parseData.Description ?? "";

                dbEntity.DeviceType.Add(newDeviceType);
                dbEntity.SaveChanges();
                return newDeviceType.Id;
            }
        }

        public void Update(int id, Format_Update dataModel)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                var existingDeviceType = dbEntity.DeviceType.Find(id);
                if (existingDeviceType == null)
                    throw new CDSException(10701);

                if (dataModel.Name != null)
                    existingDeviceType.Name = dataModel.Name;

                if (dataModel.Description != null)
                    existingDeviceType.Description = dataModel.Description;
                dbEntity.SaveChanges();
            }
        }

        public void DeleteById(int id)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                DeviceType existingData = dbEntity.DeviceType.Find(id);
                if (existingData == null)
                    throw new CDSException(10701);

                dbEntity.DeviceType.Remove(existingData);
                dbEntity.SaveChanges();
            }
        }

    }
}