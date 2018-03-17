using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using sfShareLib;
using System.ComponentModel.DataAnnotations;
using CDSShareLib.Helper;

namespace sfAPIService.Models
{
    public class EquipmentModels
    {
        public class Detail
        {
            public int Id { get; set; }
            public string EquipmentId { get; set; }
            public string Name { get; set; }
            public int EquipmentClassId { get; set; }
            public string EquipmentClassName { get; set; }
            public int CompanyId { get; set; }
            public int FactoryId { get; set; }
            public string FactoryName { get; set; }
            public int IoTDeviceId { get; set; }
            public string IoTDeviceTypeName { get; set; }
            public string Latitude { get; set; }
            public string Longitude { get; set; }
            public int MaxIdleInSec { get; set; }
            public string PhotoUrl { get; set; }
        }
        public class Detail_readonly
        {
            public string EquipmentId { get; set; }
            public string Name { get; set; }
            public int CompanyId { get; set; }
            public string EquipmentClassName { get; set; }
            public string FactoryName { get; set; }
            public int IoTDeviceId { get; set; }
            public string IoTDeviceTypeName { get; set; }
            public string Latitude { get; set; }
            public string Longitude { get; set; }
            public string PhotoUrl { get; set; }
        }
        public class Edit
        {
            [Required]
            public string EquipmentId { get; set; }
            [Required]
            public string Name { get; set; }
            [Required]
            public int CompanyId { get; set; }
            [Required]
            public int EquipmentClassId { get; set; }
            [Required]
            public int FactoryId { get; set; }
            [Required]
            public int IoTDeviceId { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public int MaxIdleInSec { get; set; }
        }

        public List<Detail> GetAllEquipment()
        {
            DBHelper._Equipment dbhelp_equipment = new DBHelper._Equipment();
            return dbhelp_equipment.GetAll().Select(s => new Detail()
            {
                Id = s.Id,
                EquipmentId = s.EquipmentId,
                Name = s.Name,
                CompanyId = s.CompanyID,
                EquipmentClassId = s.EquipmentClassId,
                EquipmentClassName = s.EquipmentClass.Name,
                FactoryId = s.FactoryId,
                FactoryName = s.Factory.Name,
                IoTDeviceId = s.IoTDeviceID,
                IoTDeviceTypeName = s.IoTDevice.DeviceType.Name,
                Latitude = (s.Latitude == null) ? "" : s.Latitude.ToString(),
                Longitude = (s.Longitude == null) ? "" : s.Longitude.ToString(),
                MaxIdleInSec = s.MaxIdleInSec,
                PhotoUrl = s.PhotoURL
            }).ToList<Detail>();
        }

        public List<Detail> GetAllEquipmentByCompanyId(int companyId)
        {
            DBHelper._Equipment dbhelp_equipment = new DBHelper._Equipment();
            List<Equipment> equipmentList = dbhelp_equipment.GetAllByCompanyId(companyId);
           
            return equipmentList.Select(s => new Detail()
            {
                Id = s.Id,
                EquipmentId = s.EquipmentId,
                Name = s.Name,
                CompanyId = s.CompanyID,
                EquipmentClassId = s.EquipmentClassId,
                EquipmentClassName = s.EquipmentClass.Name,
                FactoryId = s.FactoryId,
                FactoryName = s.Factory.Name,
                IoTDeviceId = s.IoTDeviceID,
                IoTDeviceTypeName = s.IoTDevice.DeviceType.Name,
                Latitude = (s.Latitude == null) ? "" : s.Latitude.ToString(),
                Longitude = (s.Longitude == null) ? "" : s.Longitude.ToString(),
                MaxIdleInSec = s.MaxIdleInSec,
                PhotoUrl = s.PhotoURL
            }).ToList<Detail>();
        }

        public List<Detail_readonly> GetAllEquipmentByCompanyIdReadonly(int companyId)
        {
            DBHelper._Equipment dbhelp_equipment = new DBHelper._Equipment();
            List<Equipment> equipmentList = dbhelp_equipment.GetAllByCompanyId(companyId);

            return equipmentList.Select(s => new Detail_readonly()
            {
                EquipmentId = s.EquipmentId,
                Name = s.Name,
                CompanyId = s.CompanyID,
                EquipmentClassName = s.EquipmentClass.Name,
                FactoryName = s.Factory.Name,
                IoTDeviceId = s.IoTDeviceID,
                IoTDeviceTypeName = s.IoTDevice.DeviceType.Name,
                Latitude = (s.Latitude == null) ? "" : s.Latitude.ToString(),
                Longitude = (s.Longitude == null) ? "" : s.Longitude.ToString(),
                PhotoUrl = s.PhotoURL
            }).ToList<Detail_readonly>();
        }

        public List<Detail> GetAllEquipmentByFactoryId(int factoryId)
        {
            DBHelper._Equipment dbhelp = new DBHelper._Equipment();

            return dbhelp.GetAllByFactoryId(factoryId).Select(s => new Detail()
            {
                Id = s.Id,
                EquipmentId = s.EquipmentId,
                Name = s.Name,
                CompanyId = s.CompanyID,
                EquipmentClassId = s.EquipmentClassId,
                EquipmentClassName = s.EquipmentClass.Name,
                FactoryId = s.FactoryId,
                FactoryName = s.Factory.Name,
                IoTDeviceId = s.IoTDeviceID,
                IoTDeviceTypeName = s.IoTDevice.DeviceType.Name,
                Latitude = (s.Latitude == null) ? "" : s.Latitude.ToString(),
                Longitude = (s.Longitude == null) ? "" : s.Longitude.ToString(),
                MaxIdleInSec = s.MaxIdleInSec,
                PhotoUrl = s.PhotoURL
            }).ToList<Detail>();
        }

        public List<Detail_readonly> GetAllEquipmentByFactoryIdReadonly(int factoryId)
        {     
            DBHelper._Equipment dbhelp_equipment = new DBHelper._Equipment();

            return dbhelp_equipment.GetAllByFactoryId(factoryId).Select(s => new Detail_readonly()
            {
                EquipmentId = s.EquipmentId,
                Name = s.Name,
                CompanyId = s.CompanyID,
                EquipmentClassName = s.EquipmentClass.Name,
                FactoryName = s.Factory.Name,
                IoTDeviceId = s.IoTDeviceID,
                IoTDeviceTypeName = s.IoTDevice.DeviceType.Name,
                Latitude = (s.Latitude == null) ? "" : s.Latitude.ToString(),
                Longitude = (s.Longitude == null) ? "" : s.Longitude.ToString(),
                PhotoUrl = s.PhotoURL
            }).ToList<Detail_readonly>();
        }

        public Detail getEquipmentById(int id)
        {
            DBHelper._Equipment dbhelp = new DBHelper._Equipment();
            Equipment equipment = dbhelp.GetByid(id);
            if (equipment == null)
                throw new CDSException(10501);

            return new Detail()
            {
                Id = equipment.Id,
                EquipmentId = equipment.EquipmentId,
                Name = equipment.Name,
                CompanyId = equipment.CompanyID,
                EquipmentClassId = equipment.EquipmentClassId,
                EquipmentClassName = equipment.EquipmentClass.Name,
                FactoryId = equipment.FactoryId,
                FactoryName = equipment.Factory.Name,
                IoTDeviceId = equipment.IoTDeviceID,
                IoTDeviceTypeName = equipment.IoTDevice.DeviceType.Name,
                Latitude = (equipment.Latitude == null) ? "" : equipment.Latitude.ToString(),
                Longitude = (equipment.Longitude == null) ? "" : equipment.Longitude.ToString(),
                MaxIdleInSec = equipment.MaxIdleInSec,
                PhotoUrl = equipment.PhotoURL
            };
        }

        public Detail_readonly getEquipmentByIdReadonly(int equipmentId)
        {
            DBHelper._Equipment dbhelp = new DBHelper._Equipment();
            Equipment equipment = dbhelp.GetByid(equipmentId);

            return new Detail_readonly()
            {
                EquipmentId = equipment.EquipmentId,
                Name = equipment.Name,
                CompanyId = equipment.CompanyID,
                EquipmentClassName = equipment.EquipmentClass.Name,
                FactoryName = equipment.Factory.Name,
                IoTDeviceId = equipment.IoTDeviceID,
                IoTDeviceTypeName = equipment.IoTDevice.DeviceType.Name,
                Latitude = (equipment.Latitude == null) ? "" : equipment.Latitude.ToString(),
                Longitude = (equipment.Longitude == null) ? "" : equipment.Longitude.ToString(),
                PhotoUrl = equipment.PhotoURL
            };
        }

        public int addEquipment(Edit equipment)
        {
            DBHelper._Equipment dbhelp = new DBHelper._Equipment();
            var newEquipment = new Equipment()
            {
                EquipmentId = equipment.EquipmentId,
                Name = equipment.Name,
                CompanyID = equipment.CompanyId,
                EquipmentClassId = equipment.EquipmentClassId,
                FactoryId = equipment.FactoryId,
                IoTDeviceID = equipment.IoTDeviceId,
                Latitude = equipment.Latitude,
                Longitude = equipment.Longitude,
                MaxIdleInSec = equipment.MaxIdleInSec
            };
            int newEquipmentId = dbhelp.Add(newEquipment);
            return newEquipmentId;
        }

        public void updateEquipment(int id, Edit equipment)
        {
            DBHelper._Equipment dbhelp = new DBHelper._Equipment();
            Equipment existingEquipment = dbhelp.GetByid(id);
            existingEquipment.EquipmentId = equipment.EquipmentId;
            existingEquipment.Name = equipment.Name;
            existingEquipment.CompanyID = equipment.CompanyId;
            existingEquipment.EquipmentClassId = equipment.EquipmentClassId;
            existingEquipment.FactoryId = equipment.FactoryId;
            existingEquipment.IoTDeviceID = equipment.IoTDeviceId;
            existingEquipment.Latitude = equipment.Latitude;
            existingEquipment.Longitude = equipment.Longitude;
            existingEquipment.MaxIdleInSec = equipment.MaxIdleInSec;

            dbhelp.Update(existingEquipment);
        }

        public void deleteEquipment(int id)
        {
            DBHelper._Equipment dbhelp = new DBHelper._Equipment();
            Equipment existingEquipment = dbhelp.GetByid(id);

            dbhelp.Delete(existingEquipment);
        }

        public int getCompanyId(int id)
        {
            DBHelper._Equipment dbhelp = new DBHelper._Equipment();
            Equipment existEquipment = dbhelp.GetByid(id);
            return existEquipment.CompanyID ;
        }

        public int getCompanyId(string equipmentId)
        {
            CDStudioEntities dbEntity = new CDStudioEntities();
            var companyId = from c in dbEntity.Equipment
                             where c.EquipmentId == equipmentId
                             select c.CompanyID;
            return (int)companyId.FirstOrDefault();
        }

        public void updateEquipmentLogoURL(int id, string url)
        {
            DBHelper._Equipment dbhelp = new DBHelper._Equipment();
            Equipment existEquipment = dbhelp.GetByid(id);
            existEquipment.PhotoURL = url;
            dbhelp.Update(existEquipment);
        }
    }
}