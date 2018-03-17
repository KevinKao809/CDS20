using CDSShareLib.Helper;
using CDSShareLib.ServiceBus;
using Newtonsoft.Json;
using sfShareLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace sfAPIService.Models
{
    public class BackendTaskModel
    {
        public class Format_Base
        {
            [Required]
            public string Requester { get; set; }
            [Required]
            public string requesterEmail { get; set; }
        }
        //Company's CosmosDBCollection
        public void CreateCompanyCosmosDBCollection(int companyId, Format_Base parseData)
        {
            CDSShareLib.ServiceBus.Model.CosmosDBCollectionCreateModel message = new CDSShareLib.ServiceBus.Model.CosmosDBCollectionCreateModel();
            message.content = new CDSShareLib.ServiceBus.Model.CosmosDBCollectionCreateModel.ContentFormat();

            //ServiceBus - Content
            message.content.companyId = companyId;
            message.content.partitionKey = "/messageContent/equipmentId"; //temporarily fixed            
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                //Find company's unexpired and latest subscription
                var subscription = (from c in dbEntity.CompanyInSubscriptionPlan.AsNoTracking()
                                    where c.CompanyID == companyId && c.ExpiredDate > DateTime.UtcNow
                                    orderby c.ExpiredDate descending
                                    select c).FirstOrDefault();
                if (subscription == null)
                    throw new CDSException(10201);

                message.content.collectionRU = subscription.CosmosDBCollectionReservedUnits.ToString();
                message.content.collectionTTL = subscription.CosmosDBCollectionTTL.ToString();

                //ServiceBus - Flexible parameter
                message.cosmosDBConnectionString = subscription.CosmosDBConnectionString;
            }         
                        
            //ServiceBus - Base parameter
            message.entityId = companyId;
            message.requester = parseData.Requester;
            message.requesterEmail = parseData.requesterEmail;

            //Operation task
            OperationTaskModel.Format_Create operationTaskData = new OperationTaskModel.Format_Create();
            operationTaskData.Entity = message.entity;
            operationTaskData.Name = message.task;
            operationTaskData.EntityId = message.entityId.ToString();
            operationTaskData.TaskContent = JsonConvert.SerializeObject(message);

            OperationTaskModel operationTaskModel = new OperationTaskModel();
            message.taskId = operationTaskModel.Create(companyId, operationTaskData);

            Global.ServiceBus.Helper.SendToQueue(Global.ServiceBus.Queue.Provision, JsonConvert.SerializeObject(message));
        }
        public void UpdateCompanyCosmosDBCollection(int companyId, Format_Base parseData)
        {
            CDSShareLib.ServiceBus.Model.CosmosDBCollectionUpdateModel message = new CDSShareLib.ServiceBus.Model.CosmosDBCollectionUpdateModel();
            message.content = new CDSShareLib.ServiceBus.Model.CosmosDBCollectionUpdateModel.ContentFormat();

            //ServiceBus - Content
            message.content.companyId = companyId;       
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                //Find company's unexpired and latest subscription
                var subscription = (from c in dbEntity.CompanyInSubscriptionPlan.AsNoTracking()
                                    where c.CompanyID == companyId && c.ExpiredDate > DateTime.UtcNow
                                    orderby c.ExpiredDate descending
                                    select c).FirstOrDefault();
                if (subscription == null)
                    throw new CDSException(10201);

                message.content.collectionRU = subscription.CosmosDBCollectionReservedUnits.ToString();
                message.content.collectionTTL = subscription.CosmosDBCollectionTTL.ToString();

                //ServiceBus - Flexible parameter
                message.cosmosDBConnectionString = subscription.CosmosDBConnectionString;
            }

            //ServiceBus - Base parameter
            message.entityId = companyId;
            message.requester = parseData.Requester;
            message.requesterEmail = parseData.requesterEmail;

            //Operation task
            OperationTaskModel.Format_Create operationTaskData = new OperationTaskModel.Format_Create();
            operationTaskData.Entity = message.entity;
            operationTaskData.Name = message.task;
            operationTaskData.EntityId = message.entityId.ToString();
            operationTaskData.TaskContent = JsonConvert.SerializeObject(message);

            OperationTaskModel operationTaskModel = new OperationTaskModel();
            message.taskId = operationTaskModel.Create(companyId, operationTaskData);

            Global.ServiceBus.Helper.SendToQueue(Global.ServiceBus.Queue.Provision, JsonConvert.SerializeObject(message));
        }
        public void DeleteCompanyCosmosDBCollection(int companyId, Format_Base parseData)
        {
            CDSShareLib.ServiceBus.Model.CosmosDBCollectionDeleteModel message = new CDSShareLib.ServiceBus.Model.CosmosDBCollectionDeleteModel();
            message.content = new CDSShareLib.ServiceBus.Model.CosmosDBCollectionDeleteModel.ContentFormat();

            //ServiceBus - Content
            message.content.companyId = companyId;
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                //Find company's latest subscription
                var subscription = (from c in dbEntity.CompanyInSubscriptionPlan.AsNoTracking()
                                    where c.CompanyID == companyId
                                    orderby c.ExpiredDate descending
                                    select c).FirstOrDefault();
                if (subscription == null)
                    throw new CDSException(10201);

                //ServiceBus - Flexible parameter
                message.cosmosDBConnectionString = subscription.CosmosDBConnectionString;
            }

            //ServiceBus - Base parameter
            message.entityId = companyId;
            message.requester = parseData.Requester;
            message.requesterEmail = parseData.requesterEmail;

            //Operation task
            OperationTaskModel.Format_Create operationTaskData = new OperationTaskModel.Format_Create();
            operationTaskData.Entity = message.entity;
            operationTaskData.Name = message.task;
            operationTaskData.EntityId = message.entityId.ToString();
            operationTaskData.TaskContent = JsonConvert.SerializeObject(message);

            OperationTaskModel operationTaskModel = new OperationTaskModel();
            message.taskId = operationTaskModel.Create(companyId, operationTaskData);

            Global.ServiceBus.Helper.SendToQueue(Global.ServiceBus.Queue.Provision, JsonConvert.SerializeObject(message));
        }

        //IoTHub        
        public class Format_LaunchIoTHubReceiver : Format_Base
        {
            [Required]
            public string Version { get; set; }    
        }        
        public void LaunchIoTHubReceiver(int companyId, int iotHubId, Format_LaunchIoTHubReceiver parseData)
        {
            CDSShareLib.ServiceBus.Model.IoTHubReceiverModel message = new CDSShareLib.ServiceBus.Model.IoTHubReceiverModel();
            message.job = TaskName.IoTHubReceiver_Launch;
            message.task = TaskName.IoTHubReceiver_Launch;
            message.content = new CDSShareLib.ServiceBus.Model.IoTHubReceiverModel.ContentFormat();

            //ServiceBus - Content
            message.content.companyId = companyId;
            message.content.iotHubId = iotHubId;
            message.content.version = parseData.Version;

            //ServiceBus - Base parameter
            message.entityId = iotHubId;
            message.requester = parseData.Requester;
            message.requesterEmail = parseData.requesterEmail;

            //Operation task
            OperationTaskModel.Format_Create operationTaskData = new OperationTaskModel.Format_Create();
            operationTaskData.Entity = message.entity;
            operationTaskData.Name = message.task;
            operationTaskData.EntityId = message.entityId.ToString();
            operationTaskData.TaskContent = JsonConvert.SerializeObject(message);

            OperationTaskModel operationTaskModel = new OperationTaskModel();
            message.taskId = operationTaskModel.Create(companyId, operationTaskData);

            Global.ServiceBus.Helper.SendToQueue(Global.ServiceBus.Queue.Provision, JsonConvert.SerializeObject(message));
        }

        public class Format_ShutdownIoTHubReceiver : Format_Base
        {
            [Required]
            public string Version { get; set; }
        }
        public void ShuudownIoTHubReceiver(int companyId, int iotHubId, Format_ShutdownIoTHubReceiver parseData)
        {
            CDSShareLib.ServiceBus.Model.IoTHubReceiverModel message = new CDSShareLib.ServiceBus.Model.IoTHubReceiverModel();
            message.job = TaskName.IoTHubReceiver_Shutdown;
            message.task = TaskName.IoTHubReceiver_Shutdown;
            message.content = new CDSShareLib.ServiceBus.Model.IoTHubReceiverModel.ContentFormat();

            //ServiceBus - Content
            message.content.companyId = companyId;
            message.content.iotHubId = iotHubId;
            message.content.version = parseData.Version;

            //ServiceBus - Base parameter
            message.entityId = iotHubId;
            message.requester = parseData.Requester;
            message.requesterEmail = parseData.requesterEmail;

            //Operation task
            OperationTaskModel.Format_Create operationTaskData = new OperationTaskModel.Format_Create();
            operationTaskData.Entity = message.entity;
            operationTaskData.Name = message.task;
            operationTaskData.EntityId = message.entityId.ToString();
            operationTaskData.TaskContent = JsonConvert.SerializeObject(message);

            OperationTaskModel operationTaskModel = new OperationTaskModel();
            message.taskId = operationTaskModel.Create(companyId, operationTaskData);

            Global.ServiceBus.Helper.SendToQueue(Global.ServiceBus.Queue.Provision, JsonConvert.SerializeObject(message));
        }
        
        //IoTDevice
        public void RegisterIoTDevice(int companyId, int iotDeviceId, Format_Base parseData)
        {
            CDSShareLib.ServiceBus.Model.IoTDeviceRegisterModel message = new CDSShareLib.ServiceBus.Model.IoTDeviceRegisterModel();
            message.content = new CDSShareLib.ServiceBus.Model.IoTDeviceRegisterModel.ContentFormat();

            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                IoTDevice iotDevice = dbEntity.IoTDevice.Find(iotDeviceId);
                if (iotDevice == null)
                    throw new CDSException(10902);

                //ServiceBus - Content
                message.content.iothubDeviceId = iotDevice.IoTHubDeviceID;
                message.content.authenticationType = iotDevice.AuthenticationType;
                message.content.iothubConnectionString = (iotDevice.IoTHub == null ? "" : iotDevice.IoTHub.IoTHubConnectionString);
                switch (message.content.authenticationType.ToLower())
                {
                    case "key":
                        message.content.iothubDeviceKey = iotDevice.IoTHubDeviceKey;
                        message.content.certificateThumbprint = null;
                        break;
                    case "certificate":
                        message.content.iothubDeviceKey = null;
                        message.content.certificateThumbprint = (iotDevice.DeviceCertificate == null ? "" : iotDevice.DeviceCertificate.Thumbprint);
                        break;
                }
            }
            
            //ServiceBus - Base parameter
            message.entityId = iotDeviceId;
            message.requester = parseData.Requester;
            message.requesterEmail = parseData.requesterEmail;

            //Operation task
            OperationTaskModel.Format_Create operationTaskData = new OperationTaskModel.Format_Create();
            operationTaskData.Entity = message.entity;
            operationTaskData.Name = message.task;
            operationTaskData.EntityId = message.entityId.ToString();
            operationTaskData.TaskContent = JsonConvert.SerializeObject(message);

            OperationTaskModel operationTaskModel = new OperationTaskModel();
            message.taskId = operationTaskModel.Create(companyId, operationTaskData);

            Global.ServiceBus.Helper.SendToQueue(Global.ServiceBus.Queue.Provision, JsonConvert.SerializeObject(message));
        }

        public class Format_UpdateIoTDevice : Format_Base
        {
            [Required]
            public string OldIothubConnectionString { get; set; }
        }
        public void UpdateIoTDevice(int companyId, int iotDeviceId, Format_UpdateIoTDevice parseData)
        {
            CDSShareLib.ServiceBus.Model.IoTDeviceUpdateModel message = new CDSShareLib.ServiceBus.Model.IoTDeviceUpdateModel();
            message.content = new CDSShareLib.ServiceBus.Model.IoTDeviceUpdateModel.ContentFormat();

            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                IoTDevice iotDevice = dbEntity.IoTDevice.Find(iotDeviceId);
                if (iotDevice == null)
                    throw new CDSException(10902);

                //ServiceBus - Content
                message.content.oldIothubConnectionString = parseData.OldIothubConnectionString;
                message.content.iothubDeviceId = iotDevice.IoTHubDeviceID;
                message.content.authenticationType = iotDevice.AuthenticationType;
                message.content.iothubConnectionString = (iotDevice.IoTHub == null ? "" : iotDevice.IoTHub.IoTHubConnectionString);
                switch (message.content.authenticationType.ToLower())
                {
                    case "key":
                        message.content.iothubDeviceKey = iotDevice.IoTHubDeviceKey;
                        message.content.certificateThumbprint = null;
                        break;
                    case "certificate":
                        message.content.iothubDeviceKey = null;
                        message.content.certificateThumbprint = (iotDevice.DeviceCertificate == null ? "" : iotDevice.DeviceCertificate.Thumbprint);
                        break;
                }
            }

            //ServiceBus - Base parameter
            message.entityId = iotDeviceId;
            message.requester = parseData.Requester;
            message.requesterEmail = parseData.requesterEmail;

            //Operation task
            OperationTaskModel.Format_Create operationTaskData = new OperationTaskModel.Format_Create();
            operationTaskData.Entity = message.entity;
            operationTaskData.Name = message.task;
            operationTaskData.EntityId = message.entityId.ToString();
            operationTaskData.TaskContent = JsonConvert.SerializeObject(message);

            OperationTaskModel operationTaskModel = new OperationTaskModel();
            message.taskId = operationTaskModel.Create(companyId, operationTaskData);

            Global.ServiceBus.Helper.SendToQueue(Global.ServiceBus.Queue.Provision, JsonConvert.SerializeObject(message));
        }

        public void DeleteIoTDevice(int companyId, int iotDeviceId, Format_Base parseData)
        {
            CDSShareLib.ServiceBus.Model.IoTDeviceDeleteModel message = new CDSShareLib.ServiceBus.Model.IoTDeviceDeleteModel();
            message.content = new CDSShareLib.ServiceBus.Model.IoTDeviceDeleteModel.ContentFormat();

            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                IoTDevice iotDevice = dbEntity.IoTDevice.Find(iotDeviceId);
                if (iotDevice == null)
                    throw new CDSException(10902);

                //ServiceBus - Content
                message.content.iothubDeviceId = iotDevice.IoTHubDeviceID;
                message.content.iothubConnectionString = (iotDevice.IoTHub == null ? "" : iotDevice.IoTHub.IoTHubConnectionString);
            }

            //ServiceBus - Base parameter
            message.entityId = iotDeviceId;
            message.requester = parseData.Requester;
            message.requesterEmail = parseData.requesterEmail;

            //Operation task
            OperationTaskModel.Format_Create operationTaskData = new OperationTaskModel.Format_Create();
            operationTaskData.Entity = message.entity;
            operationTaskData.Name = message.task;
            operationTaskData.EntityId = message.entityId.ToString();
            operationTaskData.TaskContent = JsonConvert.SerializeObject(message);

            OperationTaskModel operationTaskModel = new OperationTaskModel();
            message.taskId = operationTaskModel.Create(companyId, operationTaskData);

            Global.ServiceBus.Helper.SendToQueue(Global.ServiceBus.Queue.Provision, JsonConvert.SerializeObject(message));
        }

    }
}