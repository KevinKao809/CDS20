using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using sfShareLib;
using System.ComponentModel.DataAnnotations;
using System.Web.Helpers;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using CDSShareLib.Helper;

namespace sfAPIService.Models
{
    public class IoTDeviceModel
    {
        public class Format_Detail
        {
            public int Id { get; set; }
            public string IoTHubDeviceId { get; set; }
            public int IoTHubId { get; set; }
            public string IoTHubName { get; set; }
            public string IoTHubProtocol { get; set; }
            public int FactoryId { get; set; }
            public string FactoryName { get; set; }
            public int? DeviceCertificateID { get; set; }
            public string AuthenticationType { get; set; }
            public int DeviceTypeId { get; set; }
            public string DeviceTypeName { get; set; }
            public string DeviceVendor { get; set; }
            public string DeviceModel { get; set; }
            public string MessageConvertScript { get; set; }
            public bool EnableMessageConvert { get; set; }
            public string OriginMessage { get; set; }
        }
        public class Format_DetailForExternal
        {
            public int Id { get; set; }
            public string IoTHubDeviceId { get; set; }
            public string IoTHubProtocol { get; set; }
            public int FactoryId { get; set; }
            public string FactoryName { get; set; }
            public int DeviceTypeId { get; set; }
            public string DeviceTypeName { get; set; }
            public string DeviceVendor { get; set; }
            public string DeviceModel { get; set; }
        }
        public class Format_Create
        {
            [Required]
            public string IoTHubDeviceId { get; set; }
            [Required]
            public string IoTHubDevicePW { get; set; }
            [Required]
            public int IoTHubID { get; set; }
            [Required]
            public string IoTHubProtocol { get; set; }
            [Required]
            public int FactoryId { get; set; }
            [Required]
            public string AuthenticationType { get; set; }
            public string IoTDeviceId { get; set; }
            [Required]
            public int DeviceTypeId { get; set; }
            public int? DeviceCertificateID { get; set; }
            public string DeviceVendor { get; set; }
            public string DeviceModel { get; set; }
            public string MessageConvertScript { get; set; }
            [Required]
            public bool EnableMessageConvert { get; set; }
            public string OriginMessage { get; set; }
        }
        public class Format_Update
        {
            public int? IoTHubID { get; set; }
            public string IoTHubProtocol { get; set; }
            public int? FactoryId { get; set; }
            public string AuthenticationType { get; set; }
            public int? DeviceTypeId { get; set; }
            public string DeviceVendor { get; set; }
            public string DeviceModel { get; set; }
            public int? DeviceCertificateID { get; set; }
            public string MessageConvertScript { get; set; }
            public bool? EnableMessageConvert { get; set; }
            public string OriginMessage { get; set; }
        }
        
        public class Format_CDSDeviceTwinsProperty
        {
            public JObject CDS_SystemConfig { get; set; }
            public JObject CDS_CustomizedConfig { get; set; }
        }

        public List<Format_Detail> GetAllByCompanyId(int companyId)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                var L2Enty = from c in dbEntity.IoTDevice.AsNoTracking()
                             where c.CompanyID == companyId
                             select c;

                return L2Enty.Select(s => new Format_Detail
                {
                    Id = s.Id,
                    IoTHubDeviceId = s.IoTHubDeviceID,
                    IoTHubId = s.IoTHubID,
                    IoTHubName = s.IoTHub == null ? "" : s.IoTHub.IoTHubName,
                    IoTHubProtocol = s.IoTHubProtocol,
                    FactoryId = s.FactoryID,
                    FactoryName = (s.Factory == null ? "" : s.Factory.Name),
                    AuthenticationType = s.AuthenticationType,
                    DeviceCertificateID = s.DeviceCertificateID,
                    DeviceTypeId = s.DeviceTypeId,
                    DeviceTypeName = (s.DeviceType == null ? "" : s.DeviceType.Name),
                    DeviceVendor = s.DeviceVendor,
                    DeviceModel = s.DeviceModel,
                    MessageConvertScript = s.MessageConvertScript,
                    EnableMessageConvert = s.EnableMessageConvert,
                    OriginMessage = s.OriginMessage
                }).ToList<Format_Detail>();
            }
        }

        public List<Format_Detail> GetAllByFactoryId(int factoryId)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                var L2Enty = from c in dbEntity.IoTDevice.AsNoTracking()
                             where c.FactoryID == factoryId
                             select c;

                return L2Enty.Select(s => new Format_Detail
                {
                    Id = s.Id,
                    IoTHubDeviceId = s.IoTHubDeviceID,
                    IoTHubId = s.IoTHubID,
                    IoTHubName = s.IoTHub == null ? "" : s.IoTHub.IoTHubName,
                    IoTHubProtocol = s.IoTHubProtocol,
                    FactoryId = s.FactoryID,
                    FactoryName = (s.Factory == null ? "" : s.Factory.Name),
                    AuthenticationType = s.AuthenticationType,
                    DeviceCertificateID = s.DeviceCertificateID,
                    DeviceTypeId = s.DeviceTypeId,
                    DeviceTypeName = (s.DeviceType == null ? "" : s.DeviceType.Name),
                    DeviceVendor = s.DeviceVendor,
                    DeviceModel = s.DeviceModel,
                    MessageConvertScript = s.MessageConvertScript,
                    EnableMessageConvert = s.EnableMessageConvert,
                    OriginMessage = s.OriginMessage
                }).ToList<Format_Detail>();
            }
        }

        public Format_Detail GetById(int id)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                IoTDevice existingData = (from c in dbEntity.IoTDevice.AsNoTracking()
                                                  where c.Id == id
                                                  select c).SingleOrDefault<IoTDevice>();

                if (existingData == null)
                    throw new CDSException(10902);

                return new Format_Detail()
                {
                    Id = existingData.Id,
                    IoTHubDeviceId = existingData.IoTHubDeviceID,
                    IoTHubId = existingData.IoTHubID,
                    IoTHubName = existingData.IoTHub == null ? "" : existingData.IoTHub.IoTHubName,
                    IoTHubProtocol = existingData.IoTHubProtocol,
                    FactoryId = existingData.FactoryID,
                    FactoryName = (existingData.Factory == null ? "" : existingData.Factory.Name),
                    AuthenticationType = existingData.AuthenticationType,
                    DeviceCertificateID = existingData.DeviceCertificateID,
                    DeviceTypeId = existingData.DeviceTypeId,
                    DeviceTypeName = (existingData.DeviceType == null ? "" : existingData.DeviceType.Name),
                    DeviceVendor = existingData.DeviceVendor,
                    DeviceModel = existingData.DeviceModel,
                    MessageConvertScript = existingData.MessageConvertScript,
                    EnableMessageConvert = existingData.EnableMessageConvert,
                    OriginMessage = existingData.OriginMessage
                };
            }
        }

        public int Create(int companyId, Format_Create parseData)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                //Format_CDSDeviceTwinsProperty twinsProperty = new Format_CDSDeviceTwinsProperty();
                //twinsProperty.CDS_CustomizedConfig = new JObject();
                //twinsProperty.CDS_SystemConfig = new JObject();

                IoTDevice newData = new IoTDevice()
                {
                    CompanyID = companyId,
                    IoTHubDeviceID = parseData.IoTHubDeviceId,
                    IoTHubDevicePW = Crypto.HashPassword(parseData.IoTHubDevicePW),
                    IoTHubDeviceKey = "",
                    IoTHubID = parseData.IoTHubID,
                    IoTHubProtocol = parseData.IoTHubProtocol ?? "",
                    FactoryID = parseData.FactoryId,
                    AuthenticationType = parseData.AuthenticationType ?? "",
                    DeviceCertificateID = parseData.DeviceCertificateID,
                    DeviceTypeId = parseData.DeviceTypeId,
                    DeviceVendor = parseData.DeviceVendor ?? "",
                    DeviceModel = parseData.DeviceModel ?? "",
                    MessageConvertScript = parseData.MessageConvertScript ?? "",
                    EnableMessageConvert = parseData.EnableMessageConvert,
                    OriginMessage = parseData.OriginMessage ?? ""
                };
                dbEntity.IoTDevice.Add(newData);

                try
                {
                    dbEntity.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
                    if (ex.InnerException.InnerException.Message.Contains("Cannot insert duplicate key"))
                        throw new CDSException(10904);
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
                IoTDevice existingData = dbEntity.IoTDevice.Find(id);
                if (existingData == null)
                    throw new CDSException(10902);
                
                if (parseData.IoTHubID.HasValue)
                    existingData.IoTHubID = (int)parseData.IoTHubID;

                if (parseData.IoTHubProtocol != null)
                    existingData.IoTHubProtocol = parseData.IoTHubProtocol;

                if (parseData.FactoryId.HasValue)
                    existingData.FactoryID = (int)parseData.FactoryId;

                if (parseData.DeviceTypeId.HasValue)
                    existingData.DeviceTypeId = (int)parseData.DeviceTypeId;

                if (parseData.DeviceVendor != null)
                    existingData.DeviceVendor = parseData.DeviceVendor;

                if (parseData.DeviceModel != null)
                    existingData.DeviceModel = parseData.DeviceModel;

                if (parseData.AuthenticationType != null)
                    existingData.AuthenticationType = parseData.AuthenticationType;

                if(parseData.MessageConvertScript != null)
                    existingData.MessageConvertScript = parseData.MessageConvertScript;

                if(parseData.EnableMessageConvert.HasValue)
                    existingData.EnableMessageConvert = (bool)parseData.EnableMessageConvert;

                if (parseData.DeviceCertificateID.HasValue)
                    existingData.DeviceCertificateID = (int)parseData.DeviceCertificateID;

                if (parseData.OriginMessage != null)
                    existingData.OriginMessage = parseData.OriginMessage;
                
                dbEntity.SaveChanges();
            }
        }
        
        public void DeleteById(int id)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                IoTDevice existingData = dbEntity.IoTDevice.Find(id);
                if (existingData == null)
                    throw new CDSException(10902);

                dbEntity.Entry(existingData).State = EntityState.Deleted;
                dbEntity.SaveChanges();
            }
        }

        public void ResetPassword(int id, PasswordModel.Format_Reset model)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                var existingIoTDevice = dbEntity.IoTDevice.Find(id);
                if (existingIoTDevice == null)
                    throw new CDSException(10902);

                existingIoTDevice.IoTHubDevicePW = Crypto.HashPassword(model.NewPassword);
                dbEntity.SaveChanges();
            }
        }

        /****************************** For External Use ******************************/
        public List<Format_DetailForExternal> External_GetAllByCompanyId(int companyId)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                var L2Enty = from c in dbEntity.IoTDevice.AsNoTracking()
                             where c.CompanyID == companyId
                             select c;

                return L2Enty.Select(s => new Format_DetailForExternal
                {
                    Id = s.Id,
                    IoTHubDeviceId = s.IoTHubDeviceID,
                    IoTHubProtocol = s.IoTHubProtocol,
                    FactoryId = s.FactoryID,
                    FactoryName = (s.Factory == null ? "" : s.Factory.Name),
                    DeviceTypeId = s.DeviceTypeId,
                    DeviceTypeName = (s.DeviceType == null ? "" : s.DeviceType.Name),
                    DeviceVendor = s.DeviceVendor,
                    DeviceModel = s.DeviceModel,
                }).ToList<Format_DetailForExternal>();
            }
        }

        public List<Format_DetailForExternal> External_GetAllByFactoryId(int factoryId, int companyId)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                var L2Enty = from c in dbEntity.IoTDevice.AsNoTracking()
                             where c.CompanyID == companyId && c.FactoryID == factoryId
                             select c;

                return L2Enty.Select(s => new Format_DetailForExternal
                {
                    Id = s.Id,
                    IoTHubDeviceId = s.IoTHubDeviceID,
                    IoTHubProtocol = s.IoTHubProtocol,
                    FactoryId = s.FactoryID,
                    FactoryName = (s.Factory == null ? "" : s.Factory.Name),
                    DeviceTypeId = s.DeviceTypeId,
                    DeviceTypeName = (s.DeviceType == null ? "" : s.DeviceType.Name),
                    DeviceVendor = s.DeviceVendor,
                    DeviceModel = s.DeviceModel
                }).ToList<Format_DetailForExternal>();
            }
        }

        public Format_DetailForExternal External_GetById(int id, int companyId)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                IoTDevice existingData = (from c in dbEntity.IoTDevice.AsNoTracking()
                                          where c.Id == id && c.CompanyID == companyId 
                                          select c).SingleOrDefault<IoTDevice>();

                if (existingData == null)
                    throw new CDSException(10902);

                return new Format_DetailForExternal()
                {
                    Id = existingData.Id,
                    IoTHubDeviceId = existingData.IoTHubDeviceID,
                    IoTHubProtocol = existingData.IoTHubProtocol,
                    FactoryId = existingData.FactoryID,
                    FactoryName = (existingData.Factory == null ? "" : existingData.Factory.Name),
                    DeviceTypeId = existingData.DeviceTypeId,
                    DeviceTypeName = (existingData.DeviceType == null ? "" : existingData.DeviceType.Name),
                    DeviceVendor = existingData.DeviceVendor,
                    DeviceModel = existingData.DeviceModel
                };
            }
        }
    }
}