using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using sfShareLib;
using System.ComponentModel.DataAnnotations;
using CDSShareLib.Helper;

namespace sfAPIService.Models
{
    public class IoTHubModels
    {

        public class Detail
        {
            public int Id { get; set; }
            public string IoTHubName { get; set; }
            public string Description { get; set; }
            public string CompanyId { get; set; }
            public string CompanyName { get; set; }
            public string IoTHubEndPoint { get; set; }
            public string IoTHubConnectionString { get; set; }
            public string EventConsumerGroup { get; set; }
            public string EventHubStorageConnectionString { get; set; }
            public string UploadContainer { get; set; }            
        }
        public class Edit
        {
            [Required]
            public string IoTHubName { get; set; }
            [Required]
            public int CompanyId { get; set; }
            public string Description { get; set; }
            public string IoTHubEndPoint { get; set; }
            [Required]
            public string IoTHubConnectionString { get; set; }
            public string EventConsumerGroup { get; set; }
            public string EventHubStorageConnectionString { get; set; }
            public string UploadContainer { get; set; }           
        }

        public List<Detail> GetAllIoTHub()
        {
            DBHelper._IoTHub dbhelp = new DBHelper._IoTHub();
            var iotHubList = dbhelp.GetAll();
            if (iotHubList == null)
                throw new CDSException(10901);

            return dbhelp.GetAll().Select(s => new Detail()
            {
                Id = s.Id,
                IoTHubName = s.IoTHubName,
                Description = s.Description,
                CompanyId = s.Company == null ? "" : s.Company.Id.ToString(),
                CompanyName = s.Company == null ? "" : s.Company.Name,
                IoTHubEndPoint = s.IoTHubEndPoint,
                IoTHubConnectionString = s.IoTHubConnectionString,
                EventConsumerGroup = s.EventConsumerGroup,
                EventHubStorageConnectionString = s.EventHubStorageConnectionString,
                UploadContainer = s.UploadContainer
                
            }).ToList<Detail>();
        }

        public List<Detail> GetAllIoTHubByCompanyId(int companyId)
        {
            DBHelper._IoTHub dbhelp = new DBHelper._IoTHub();
            var iotHubList = dbhelp.GetAllByCompanyId(companyId);
            if (iotHubList == null)
                throw new CDSException(10901);

            return iotHubList.Select(s => new Detail()
            {
                Id = s.Id,
                IoTHubName = s.IoTHubName,
                Description = s.Description,
                CompanyId = s.Company == null ? "" : s.Company.Id.ToString(),
                CompanyName = s.Company == null ? "" : s.Company.Name,
                IoTHubEndPoint = s.IoTHubEndPoint,
                IoTHubConnectionString = s.IoTHubConnectionString,
                EventConsumerGroup = s.EventConsumerGroup,
                EventHubStorageConnectionString = s.EventHubStorageConnectionString,
                UploadContainer = s.UploadContainer          
            }).ToList<Detail>();

        }

        public Detail getIoTHubById(int Id)
        {
            DBHelper._IoTHub dbhelp = new DBHelper._IoTHub();
            IoTHub iotHub = dbhelp.GetByid(Id);            
            if (iotHub == null)
                throw new CDSException(10901);

            return new Detail()
            {
                Id = iotHub.Id,
                IoTHubName = iotHub.IoTHubName,
                Description = iotHub.Description,
                CompanyId = iotHub.Company == null ? "" : iotHub.Company.Id.ToString(),
                CompanyName = iotHub.Company == null ? "" : iotHub.Company.Name,
                IoTHubEndPoint = iotHub.IoTHubEndPoint,
                IoTHubConnectionString = iotHub.IoTHubConnectionString,
                EventConsumerGroup = iotHub.EventConsumerGroup,
                EventHubStorageConnectionString = iotHub.EventHubStorageConnectionString,
                UploadContainer = iotHub.UploadContainer             
            };
        }

        public void addIoTHub(Edit iotHub)
        {
            DBHelper._IoTHub dbhelp = new DBHelper._IoTHub();
            var newIoTHub = new IoTHub()
            {
                IoTHubName = iotHub.IoTHubName,
                Description = iotHub.Description,
                CompanyID = iotHub.CompanyId,
                IoTHubEndPoint = iotHub.IoTHubEndPoint,
                IoTHubConnectionString = iotHub.IoTHubConnectionString,
                EventConsumerGroup = iotHub.EventConsumerGroup,
                EventHubStorageConnectionString = iotHub.EventHubStorageConnectionString,
                UploadContainer = iotHub.UploadContainer                
            };
            dbhelp.Add(newIoTHub);
        }

        public void updateIoTHub(int Id, Edit iotHub)
        {
            DBHelper._IoTHub dbhelp = new DBHelper._IoTHub();
            IoTHub existingIoTHub = dbhelp.GetByid(Id);
            if (existingIoTHub == null)
                throw new CDSException(10901);

            existingIoTHub.IoTHubName = iotHub.IoTHubName;
            existingIoTHub.Description = iotHub.Description;
            existingIoTHub.CompanyID = iotHub.CompanyId;
            existingIoTHub.IoTHubEndPoint = iotHub.IoTHubEndPoint;
            existingIoTHub.IoTHubConnectionString = iotHub.IoTHubConnectionString;
            existingIoTHub.EventConsumerGroup = iotHub.EventConsumerGroup;
            existingIoTHub.EventHubStorageConnectionString = iotHub.EventHubStorageConnectionString;
            existingIoTHub.UploadContainer = iotHub.UploadContainer;          

            dbhelp.Update(existingIoTHub);
        }

        public void deleteIoTHub(int Id)
        {
            DBHelper._IoTHub dbhelp = new DBHelper._IoTHub();
            IoTHub existingIoTHub = dbhelp.GetByid(Id);
            if (existingIoTHub == null)
                throw new CDSException(10901);

            dbhelp.Delete(existingIoTHub);
        }
    }
}