using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using sfShareLib;
using System.ComponentModel.DataAnnotations;
using CDSShareLib.Helper;

namespace sfAPIService.Models
{
    public class IoTDeviceCustomizedConfigurationModel
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
            [Required]
            [MaxLength(10)]
            public string DataType { get; set; }
            public string Description { get; set; }
            [MaxLength(50)]
            public string DefaultValue { get; set; }
        }
        public List<Format_Detail> GetAllByCompanyId(int companyId)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                var L2Enty = from c in dbEntity.IoTDeviceCustomizedConfiguration.AsNoTracking()
                             where c.CompanyId == companyId
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
                IoTDeviceCustomizedConfiguration existingData = (from c in dbEntity.IoTDeviceCustomizedConfiguration.AsNoTracking()
                                                                 where c.Id == id
                                                                 select c).SingleOrDefault<IoTDeviceCustomizedConfiguration>();
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

        public int Create(int companyId, Format_Create parseData)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                IoTDeviceCustomizedConfiguration newData = new IoTDeviceCustomizedConfiguration();
                newData.CompanyId = companyId;
                newData.Name = parseData.Name;
                newData.DataType = parseData.DataType;
                newData.Description = parseData.Description ?? "";
                newData.DefaultValue = parseData.DefaultValue ?? "";

                dbEntity.IoTDeviceCustomizedConfiguration.Add(newData);
                dbEntity.SaveChanges();
                return newData.Id;
            }
        }

        public void Update(int id, Format_Update parseData)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                var existingIoTDeviceCustomizedConfiguration = dbEntity.IoTDeviceCustomizedConfiguration.Find(id);
                if (existingIoTDeviceCustomizedConfiguration == null)
                    throw new CDSException(10701);

                if (parseData.Name != null)
                    existingIoTDeviceCustomizedConfiguration.Name = parseData.Name;

                if (parseData.DataType != null)
                    existingIoTDeviceCustomizedConfiguration.DataType = parseData.DataType;

                if (parseData.Description != null)
                    existingIoTDeviceCustomizedConfiguration.Description = parseData.Description;

                if (parseData.DefaultValue != null)
                    existingIoTDeviceCustomizedConfiguration.DefaultValue = parseData.DefaultValue;

                dbEntity.SaveChanges();
            }
        }

        public void DeleteById(int id)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                IoTDeviceCustomizedConfiguration existingData = dbEntity.IoTDeviceCustomizedConfiguration.Find(id);
                if (existingData == null)
                    throw new CDSException(10701);

                dbEntity.IoTDeviceCustomizedConfiguration.Remove(existingData);
                dbEntity.SaveChanges();
            }
        }
    }
}