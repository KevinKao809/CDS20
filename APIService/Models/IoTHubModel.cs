using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using sfShareLib;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using CDSShareLib.Helper;

namespace sfAPIService.Models
{
    public class IoTHubModel
    {

        public class Format_Detail
        {
            public int Id { get; set; }
            public string IoTHubName { get; set; }
            public string Description { get; set; }
            public string CompanyName { get; set; }
            public string IoTHubEndPoint { get; set; }
            public string IoTHubConnectionString { get; set; }
            public string EventConsumerGroup { get; set; }
            public string EventHubStorageConnectionString { get; set; }
            public string UploadContainer { get; set; }            
            public bool EnableMultipleReceiver { get; set; }
        }
        public class Format_Create
        {
            [Required]
            public string IoTHubName { get; set; }
            public string Description { get; set; }
            public string IoTHubEndPoint { get; set; }
            [Required]
            public string IoTHubConnectionString { get; set; }
            public string EventConsumerGroup { get; set; }
            public string EventHubStorageConnectionString { get; set; }
            public string UploadContainer { get; set; }         
            [Required]
            public bool EnableMultipleReceiver { get; set; }
        }
        public class Format_Update
        {
            public string IoTHubName { get; set; }
            public string Description { get; set; }
            public string IoTHubEndPoint { get; set; }
            /// <summary>
            /// 是否要禁止修改?
            /// </summary>
            public string IoTHubConnectionString { get; set; }
            public string EventConsumerGroup { get; set; }
            public string EventHubStorageConnectionString { get; set; }
            public string UploadContainer { get; set; }
            public bool? EnableMultipleReceiver { get; set; }
        }
        
        public List<Format_Detail> GetAllByCompanyId(int companyId)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                var L2Enty = from c in dbEntity.IoTHub.AsNoTracking()
                             where c.CompanyID == companyId
                             select c;

                return L2Enty.Select(s => new Format_Detail
                {
                    Id = s.Id,
                    IoTHubName = s.IoTHubName,
                    Description = s.Description,
                    CompanyName = s.Company == null ? "" : s.Company.Name,
                    IoTHubEndPoint = s.IoTHubEndPoint,
                    IoTHubConnectionString = s.IoTHubConnectionString,
                    EventConsumerGroup = s.EventConsumerGroup,
                    EventHubStorageConnectionString = s.EventHubStorageConnectionString,
                    UploadContainer = s.UploadContainer,
                    EnableMultipleReceiver = s.EnableMultipleReceiver
                }).ToList<Format_Detail>();
            }
        }

        public Format_Detail GetById(int id)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                IoTHub existingData = (from c in dbEntity.IoTHub.AsNoTracking()
                                 where c.Id == id
                                 select c).SingleOrDefault<IoTHub>();

                if (existingData == null)
                    throw new CDSException(10901);

                return new Format_Detail()
                {
                    Id = existingData.Id,
                    IoTHubName = existingData.IoTHubName,
                    Description = existingData.Description,
                    CompanyName = existingData.Company == null ? "" : existingData.Company.Name,
                    IoTHubEndPoint = existingData.IoTHubEndPoint,
                    IoTHubConnectionString = existingData.IoTHubConnectionString,
                    EventConsumerGroup = existingData.EventConsumerGroup,
                    EventHubStorageConnectionString = existingData.EventHubStorageConnectionString,
                    UploadContainer = existingData.UploadContainer,
                    EnableMultipleReceiver = existingData.EnableMultipleReceiver
                };
            }
        }

        public int Create(int companyId, Format_Create parseData)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                IoTHub newData = new IoTHub()
                {
                    IoTHubName = parseData.IoTHubName,
                    Description = parseData.Description ?? "",
                    CompanyID = companyId,
                    IoTHubEndPoint = parseData.IoTHubEndPoint ?? "",
                    IoTHubConnectionString = parseData.IoTHubConnectionString ?? "",
                    EventConsumerGroup = parseData.EventConsumerGroup ?? "",
                    EventHubStorageConnectionString = parseData.EventHubStorageConnectionString ?? "",
                    UploadContainer = parseData.UploadContainer ?? "",
                    EnableMultipleReceiver = parseData.EnableMultipleReceiver
                };
                dbEntity.IoTHub.Add(newData);
                try
                {
                    dbEntity.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
                    if (ex.InnerException.InnerException.Message.Contains("Cannot insert duplicate key"))
                        throw new CDSException(10905);
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
                IoTHub existingData = dbEntity.IoTHub.Find(id);
                if (existingData == null)
                    throw new CDSException(10901);

                if (parseData.IoTHubName != null)
                    existingData.IoTHubName = parseData.IoTHubName;

                if (parseData.Description != null)
                    existingData.Description = parseData.Description;

                if (parseData.IoTHubEndPoint != null)
                    existingData.IoTHubEndPoint = parseData.IoTHubEndPoint;

                if (parseData.IoTHubConnectionString != null)
                    existingData.IoTHubConnectionString = parseData.IoTHubConnectionString;

                if (parseData.EventConsumerGroup != null)
                    existingData.EventConsumerGroup = parseData.EventConsumerGroup;

                if (parseData.EventHubStorageConnectionString != null)
                    existingData.EventHubStorageConnectionString = parseData.EventHubStorageConnectionString;

                if (parseData.UploadContainer != null)
                    existingData.UploadContainer = parseData.UploadContainer;

                if (parseData.EnableMultipleReceiver.HasValue)
                    existingData.EnableMultipleReceiver = (bool)parseData.EnableMultipleReceiver;

                try
                {
                    dbEntity.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
                    if (ex.InnerException.InnerException.Message.Contains("Cannot insert duplicate key"))
                        throw new CDSException(10905);
                    else
                        throw ex;
                }
            }
        }

        public void DeleteById(int id)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                IoTHub existingData = dbEntity.IoTHub.Find(id);
                if (existingData == null)
                    throw new CDSException(10901);

                dbEntity.Entry(existingData).State = EntityState.Deleted;
                dbEntity.SaveChanges();
            }
        }
    }
}