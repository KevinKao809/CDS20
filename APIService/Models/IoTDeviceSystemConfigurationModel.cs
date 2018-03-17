using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using sfShareLib;
using System.ComponentModel.DataAnnotations;
using CDSShareLib.Helper;

namespace sfAPIService.Models
{
    public class IoTDeviceSystemConfigurationModel
    {
        public class Format_Detail
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string DataType { get; set; }
            public string Description { get; set; }
            public string DefaultValue { get; set; }
        }
        public class Format_Create
        {
            [Required]
            public string Name { get; set; }
            [Required]
            [MaxLength(10)]
            public string DataType { get; set; }
            public string Description { get; set; }
            [MaxLength(50)]
            public string DefaultValue { get; set; }
        }
        public class Format_Update
        {
            public string Name { get; set; }
            [MaxLength(10)]
            public string DataType { get; set; }
            public string Description { get; set; }
            [MaxLength(50)]
            public string DefaultValue { get; set; }
        }

        public List<Format_Detail> GetAll()
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                var L2Enty = from c in dbEntity.IoTDeviceSystemConfiguration.AsNoTracking()
                             select c;

                return L2Enty.Select(s => new Format_Detail()
                {
                    Id = s.Id,
                    Name = s.Name,
                    DataType = s.DataType,
                    Description = s.Description,
                    DefaultValue = s.DefaultValue
                }).ToList<Format_Detail>();
            }
        }

        public Format_Detail GetById(int id)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                IoTDeviceSystemConfiguration existingData = (from c in dbEntity.IoTDeviceSystemConfiguration.AsNoTracking()
                                                             where c.Id == id
                                                             select c).SingleOrDefault<IoTDeviceSystemConfiguration>();
                if (existingData == null)
                    throw new CDSException(10701);

                return new Format_Detail()
                {
                    Id = existingData.Id,
                    Name = existingData.Name,
                    DataType = existingData.DataType,
                    Description = existingData.Description,
                    DefaultValue = existingData.DefaultValue
                };
            }
        }

        public int Create(Format_Create parseData)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                IoTDeviceSystemConfiguration newData = new IoTDeviceSystemConfiguration();
                newData.Name = parseData.Name;
                newData.DataType = parseData.DataType;
                newData.Description = parseData.Description ?? "";
                newData.DefaultValue = parseData.DefaultValue ?? "";

                dbEntity.IoTDeviceSystemConfiguration.Add(newData);
                dbEntity.SaveChanges();
                return newData.Id;
            }
        }

        public void Update(int id, Format_Update parseData)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                var existingIoTDeviceSystemConfiguration = dbEntity.IoTDeviceSystemConfiguration.Find(id);
                if (existingIoTDeviceSystemConfiguration == null)
                    throw new CDSException(10701);

                if (parseData.Name != null)
                    existingIoTDeviceSystemConfiguration.Name = parseData.Name;

                if (parseData.DataType != null)
                    existingIoTDeviceSystemConfiguration.DataType = parseData.DataType;

                if (parseData.Description != null)
                    existingIoTDeviceSystemConfiguration.Description = parseData.Description;

                if (parseData.DefaultValue != null)
                    existingIoTDeviceSystemConfiguration.DefaultValue = parseData.DefaultValue;

                dbEntity.SaveChanges();
            }
        }

        public void DeleteById(int id)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                IoTDeviceSystemConfiguration existingData = dbEntity.IoTDeviceSystemConfiguration.Find(id);
                if (existingData == null)
                    throw new CDSException(10701);

                dbEntity.IoTDeviceSystemConfiguration.Remove(existingData);
                dbEntity.SaveChanges();
            }
        }
    }
}