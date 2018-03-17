using CDSShareLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTHubReceiver.Helper
{
    class TestSampleHelper
    {
        public IoTHub generateIoTHubForTest(string iotHubId)
        {
            IoTHub iotHub = new IoTHub();

            iotHub.Id = 1234;
            iotHub.IoTHubName = "cdsdemo";
            iotHub.Description = "Walker's IoT Hub";
            iotHub.CompanyID = 69;
            iotHub.IoTHubEndPoint = "messages/events";
            iotHub.IoTHubConnectionString = "HostName=cds-dev.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=XuIGgnCmx7DrOGSj6T3nhTYm60+jhP4gWAsHkSjXDyc=";
            iotHub.EventConsumerGroup = "cdsdemo";
            iotHub.EventHubStorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=sfephost;AccountKey=0Exx0b+nflOF8w+F84I3x7UW949fSoUMlxJa7hmRLEA/X7WTgfswnaE2dipOlLMuu+Y++wrVCNMtz04mFA2KHQ==;EndpointSuffix=core.windows.net";
            iotHub.UploadContainer = "cdsdemo-attachment";
            Factory factory1 = new Factory();
            factory1.Id = 40;
            iotHub.Company = new Company();
            iotHub.Company.Factory.Add(factory1);
            iotHub.IoTDevice = generateIoTDevicesForTest(iotHub.CompanyID, iotHub.Company.Factory.ElementAt(0).Id, iotHub.Id);

            return iotHub;
        }

        private List<IoTDevice> generateIoTDevicesForTest(int companyId, int factoryId, int iotHubId)
        {
            List<IoTDevice> iotDevices = new List<IoTDevice>();

            IoTDevice device1 = new IoTDevice();
            device1.Id = 10001;
            device1.IoTHubDeviceID = "iotdevicedemo303";
            device1.IoTHubID = iotHubId;
            device1.CompanyID = companyId;
            device1.FactoryID = factoryId;
            device1.MessageConvertScript = null;
            device1.EnableMessageConvert = false;
            device1.Equipment = generateEquipmentsForTest(device1.Id);
            iotDevices.Add(device1);

            IoTDevice device2 = new IoTDevice();
            device2.Id = 10002;
            device2.IoTHubDeviceID = "iotdevicedemo404";
            device2.IoTHubID = iotHubId;
            device2.CompanyID = companyId;
            device2.FactoryID = factoryId;
            device2.MessageConvertScript = null;
            device2.EnableMessageConvert = false;
            device2.Equipment = generateEquipmentsForTest(device2.Id);
            iotDevices.Add(device2);

            IoTDevice device3 = new IoTDevice();
            device3.Id = 10003;
            device3.IoTHubDeviceID = "iotdevicedemo505";
            device3.IoTHubID = iotHubId;
            device3.CompanyID = companyId;
            device3.FactoryID = factoryId;
            device3.MessageConvertScript = null;
            device3.EnableMessageConvert = false;
            device3.Equipment = generateEquipmentsForTest(device3.Id);
            iotDevices.Add(device3);

            IoTDevice device4 = new IoTDevice();
            device4.Id = 10004;
            device4.IoTHubDeviceID = "iotdevicedemo606";
            device4.IoTHubID = iotHubId;
            device4.CompanyID = companyId;
            device4.FactoryID = factoryId;
            device4.MessageConvertScript = null;
            device4.EnableMessageConvert = false;
            device4.Equipment = generateEquipmentsForTest(device4.Id);
            iotDevices.Add(device4);

            IoTDevice device5 = new IoTDevice();
            device5.Id = 10005;
            device5.IoTHubDeviceID = "opcuadev001";
            device5.IoTHubID = iotHubId;
            device5.CompanyID = companyId;
            device5.FactoryID = factoryId;
            device5.MessageConvertScript = "{    \"messageCatalogId\": 84,    \"companyId\": 69,    \"msgTimestamp\": \"#ifcondition(#existsandnotempty($.Value.SourceTimestamp),true,#valueof($.Value.SourceTimestamp),CDS_MESSAGE_TRANSFORM_PROPERTY_NOT_FOUND)\",    \"equipmentId\": \"#ifcondition(#ifcondition(#exists($.ApplicationUri),true,#valueof($.ApplicationUri),CDS_MESSAGE_TRANSFORM_PROPERTY_NOT_FOUND),urn:OPC-A:ICPDAS:ICPDAS_OPC_UA_ServerA01,A001,#ifcondition(#ifcondition(#exists($.ApplicationUri),true,#valueof($.ApplicationUri),CDS_MESSAGE_TRANSFORM_PROPERTY_NOT_FOUND),urn:OPC-A:ICPDAS:ICPDAS_OPC_UA_ServerA02,A002,#substring(#ifcondition(#exists($.ApplicationUri),true,#valueof($.ApplicationUri),CDS_MESSAGE_TRANSFORM_PROPERTY_NOT_FOUND),#add(#lastindexof(#ifcondition(#exists($.ApplicationUri),true,#valueof($.ApplicationUri),CDS_MESSAGE_TRANSFORM_PROPERTY_NOT_FOUND),:),1),#subtract(#lastindexof(#concat(#ifcondition(#exists($.ApplicationUri),true,#valueof($.ApplicationUri),CDS_MESSAGE_TRANSFORM_PROPERTY_NOT_FOUND),:),:),#add(#lastindexof(#ifcondition(#exists($.ApplicationUri),true,#valueof($.ApplicationUri),CDS_MESSAGE_TRANSFORM_PROPERTY_NOT_FOUND),:),1)))))\",    \"equipmentRunStatus\": 1,    \"Temperature\": \"#ifcondition(#ifcondition(#existsandnotempty($.NodeId),true,#valueof($.NodeId),CDS_MESSAGE_TRANSFORM_PROPERTY_NOT_FOUND),CDS_MESSAGE_TRANSFORM_PROPERTY_NOT_FOUND,CDS_MESSAGE_TRANSFORM_PROPERTY_NOT_FOUND,#ifcondition(#ifcondition(#existsandnotempty($.NodeId),true,#valueof($.NodeId),CDS_MESSAGE_TRANSFORM_PROPERTY_NOT_FOUND),ns=2;s=MTCP_CurrentValueTask.Temperature_C,#ifcondition(#existsandnotempty($.Value.Value),true,#valueof($.Value.Value),CDS_MESSAGE_TRANSFORM_PROPERTY_NOT_FOUND),CDS_MESSAGE_TRANSFORM_PROPERTY_NOT_FOUND))\",    \"TemperatureF\": \"#ifcondition(#ifcondition(#existsandnotempty($.NodeId),true,#valueof($.NodeId),CDS_MESSAGE_TRANSFORM_PROPERTY_NOT_FOUND),CDS_MESSAGE_TRANSFORM_PROPERTY_NOT_FOUND,CDS_MESSAGE_TRANSFORM_PROPERTY_NOT_FOUND,#ifcondition(#ifcondition(#existsandnotempty($.NodeId),true,#valueof($.NodeId),CDS_MESSAGE_TRANSFORM_PROPERTY_NOT_FOUND),ns=2;s=MTCP_CurrentValueTask.Temperature_F,#ifcondition(#existsandnotempty($.Value.Value),true,#valueof($.Value.Value),CDS_MESSAGE_TRANSFORM_PROPERTY_NOT_FOUND),CDS_MESSAGE_TRANSFORM_PROPERTY_NOT_FOUND))\"  }";
            device5.EnableMessageConvert = true;
            device5.Equipment = generateEquipmentsForTest(device5.Id);
            iotDevices.Add(device5);

            IoTDevice device6 = new IoTDevice();
            device6.Id = 10006;
            device6.IoTHubDeviceID = "opcuaDevice002";
            device6.IoTHubID = iotHubId;
            device6.CompanyID = companyId;
            device6.FactoryID = factoryId;
            device6.MessageConvertScript = "{    \"messageCatalogId\": 84,    \"companyId\": 69,    \"msgTimestamp\": \"#ifcondition(#existsandnotempty($.Value.SourceTimestamp),true,#valueof($.Value.SourceTimestamp),CDS_MESSAGE_TRANSFORM_PROPERTY_NOT_FOUND)\",    \"equipmentId\": \"#ifcondition(#ifcondition(#exists($.ApplicationUri),true,#valueof($.ApplicationUri),CDS_MESSAGE_TRANSFORM_PROPERTY_NOT_FOUND),urn:OPC-A:ICPDAS:ICPDAS_OPC_UA_ServerA01,A001,#ifcondition(#ifcondition(#exists($.ApplicationUri),true,#valueof($.ApplicationUri),CDS_MESSAGE_TRANSFORM_PROPERTY_NOT_FOUND),urn:OPC-A:ICPDAS:ICPDAS_OPC_UA_ServerA02,A002,#substring(#ifcondition(#exists($.ApplicationUri),true,#valueof($.ApplicationUri),CDS_MESSAGE_TRANSFORM_PROPERTY_NOT_FOUND),#add(#lastindexof(#ifcondition(#exists($.ApplicationUri),true,#valueof($.ApplicationUri),CDS_MESSAGE_TRANSFORM_PROPERTY_NOT_FOUND),:),1),#subtract(#lastindexof(#concat(#ifcondition(#exists($.ApplicationUri),true,#valueof($.ApplicationUri),CDS_MESSAGE_TRANSFORM_PROPERTY_NOT_FOUND),:),:),#add(#lastindexof(#ifcondition(#exists($.ApplicationUri),true,#valueof($.ApplicationUri),CDS_MESSAGE_TRANSFORM_PROPERTY_NOT_FOUND),:),1)))))\",    \"equipmentRunStatus\": 1,    \"Temperature\": \"#ifcondition(#ifcondition(#existsandnotempty($.NodeId),true,#valueof($.NodeId),CDS_MESSAGE_TRANSFORM_PROPERTY_NOT_FOUND),CDS_MESSAGE_TRANSFORM_PROPERTY_NOT_FOUND,CDS_MESSAGE_TRANSFORM_PROPERTY_NOT_FOUND,#ifcondition(#ifcondition(#existsandnotempty($.NodeId),true,#valueof($.NodeId),CDS_MESSAGE_TRANSFORM_PROPERTY_NOT_FOUND),ns=2;s=MTCP_CurrentValueTask.Temperature_C,#ifcondition(#existsandnotempty($.Value.Value),true,#valueof($.Value.Value),CDS_MESSAGE_TRANSFORM_PROPERTY_NOT_FOUND),CDS_MESSAGE_TRANSFORM_PROPERTY_NOT_FOUND))\",    \"TemperatureF\": \"#ifcondition(#ifcondition(#existsandnotempty($.NodeId),true,#valueof($.NodeId),CDS_MESSAGE_TRANSFORM_PROPERTY_NOT_FOUND),CDS_MESSAGE_TRANSFORM_PROPERTY_NOT_FOUND,CDS_MESSAGE_TRANSFORM_PROPERTY_NOT_FOUND,#ifcondition(#ifcondition(#existsandnotempty($.NodeId),true,#valueof($.NodeId),CDS_MESSAGE_TRANSFORM_PROPERTY_NOT_FOUND),ns=2;s=MTCP_CurrentValueTask.Temperature_F,#ifcondition(#existsandnotempty($.Value.Value),true,#valueof($.Value.Value),CDS_MESSAGE_TRANSFORM_PROPERTY_NOT_FOUND),CDS_MESSAGE_TRANSFORM_PROPERTY_NOT_FOUND))\"  }";
            device6.EnableMessageConvert = true;
            device6.Equipment = generateEquipmentsForTest(device6.Id);
            iotDevices.Add(device6);

            IoTDevice device7 = new IoTDevice();
            device7.Id = 10007;
            device7.IoTHubDeviceID = "windowspublisherv212";
            device7.IoTHubID = iotHubId;
            device7.CompanyID = companyId;
            device7.FactoryID = factoryId;
            device7.MessageConvertScript = "{    \"messageCatalogId\": 85,    \"companyId\": 69,    \"msgTimestamp\": \"#ifcondition(#existsandnotempty($.Value.SourceTimestamp),true,#valueof($.Value.SourceTimestamp),CDS_MESSAGE_TRANSFORM_PROPERTY_NOT_FOUND)\",    \"equipmentId\": \"#ifcondition(#ifcondition(#exists($.ApplicationUri),true,#valueof($.ApplicationUri),CDS_MESSAGE_TRANSFORM_PROPERTY_NOT_FOUND),urn:OPC-A:ICPDAS:ICPDAS_OPC_UA_Server,Windwos-OPC-UA-Publisher-v212,CDS_MESSAGE_TRANSFORM_PROPERTY_NOT_FOUND)\",    \"equipmentRunStatus\": 1,    \"TemperatureC\": \"#ifcondition(#ifcondition(#existsandnotempty($.NodeId),true,#valueof($.NodeId),CDS_MESSAGE_TRANSFORM_PROPERTY_NOT_FOUND),CDS_MESSAGE_TRANSFORM_PROPERTY_NOT_FOUND,CDS_MESSAGE_TRANSFORM_PROPERTY_NOT_FOUND,#ifcondition(#ifcondition(#existsandnotempty($.NodeId),true,#valueof($.NodeId),CDS_MESSAGE_TRANSFORM_PROPERTY_NOT_FOUND),ns=2;s=MTCP_CurrentValueTask.Temperature_C,#ifcondition(#existsandnotempty($.Value.Value),true,#valueof($.Value.Value),CDS_MESSAGE_TRANSFORM_PROPERTY_NOT_FOUND),CDS_MESSAGE_TRANSFORM_PROPERTY_NOT_FOUND))\",    \"CO2\": \"#ifcondition(#ifcondition(#existsandnotempty($.NodeId),true,#valueof($.NodeId),CDS_MESSAGE_TRANSFORM_PROPERTY_NOT_FOUND),CDS_MESSAGE_TRANSFORM_PROPERTY_NOT_FOUND,CDS_MESSAGE_TRANSFORM_PROPERTY_NOT_FOUND,#ifcondition(#ifcondition(#existsandnotempty($.NodeId),true,#valueof($.NodeId),CDS_MESSAGE_TRANSFORM_PROPERTY_NOT_FOUND),ns=2;s=MTCP_CurrentValueTask.CO2,#ifcondition(#existsandnotempty($.Value.Value),true,#valueof($.Value.Value),CDS_MESSAGE_TRANSFORM_PROPERTY_NOT_FOUND),CDS_MESSAGE_TRANSFORM_PROPERTY_NOT_FOUND))\",    \"Humidity\": \"#ifcondition(#ifcondition(#existsandnotempty($.NodeId),true,#valueof($.NodeId),CDS_MESSAGE_TRANSFORM_PROPERTY_NOT_FOUND),CDS_MESSAGE_TRANSFORM_PROPERTY_NOT_FOUND,CDS_MESSAGE_TRANSFORM_PROPERTY_NOT_FOUND,#ifcondition(#ifcondition(#existsandnotempty($.NodeId),true,#valueof($.NodeId),CDS_MESSAGE_TRANSFORM_PROPERTY_NOT_FOUND),ns=2;s=MTCP_CurrentValueTask.Humidity,#ifcondition(#existsandnotempty($.Value.Value),true,#valueof($.Value.Value),CDS_MESSAGE_TRANSFORM_PROPERTY_NOT_FOUND),CDS_MESSAGE_TRANSFORM_PROPERTY_NOT_FOUND))\",    \"TemperatureF\": \"#ifcondition(#ifcondition(#existsandnotempty($.NodeId),true,#valueof($.NodeId),CDS_MESSAGE_TRANSFORM_PROPERTY_NOT_FOUND),CDS_MESSAGE_TRANSFORM_PROPERTY_NOT_FOUND,CDS_MESSAGE_TRANSFORM_PROPERTY_NOT_FOUND,#ifcondition(#ifcondition(#existsandnotempty($.NodeId),true,#valueof($.NodeId),CDS_MESSAGE_TRANSFORM_PROPERTY_NOT_FOUND),ns=2;s=MTCP_CurrentValueTask.Temperature_F,#ifcondition(#existsandnotempty($.Value.Value),true,#valueof($.Value.Value),CDS_MESSAGE_TRANSFORM_PROPERTY_NOT_FOUND),CDS_MESSAGE_TRANSFORM_PROPERTY_NOT_FOUND))\"  }";
            device7.EnableMessageConvert = true;
            device7.Equipment = generateEquipmentsForTest(device7.Id);
            iotDevices.Add(device7);

            return iotDevices;
        }

        private List<Equipment> generateEquipmentsForTest(int devieId)
        {
            List<Equipment> equipments = new List<Equipment>();
            Equipment equipment = new Equipment();// mine is 1 device on 1 equipment
            EquipmentClass ec = new EquipmentClass();
            ec.EquipmentClassMessageCatalog = new List<EquipmentClassMessageCatalog>();
            EquipmentClassMessageCatalog ecmc = new EquipmentClassMessageCatalog();// // mine is 1 equipment on 1 Message Catalog

            switch (devieId)
            {
                case 10001:
                    equipment.EquipmentId = "Equipment303";
                    ecmc.MessageCatalogID = 79;
                    break;
                case 10002:
                    equipment.EquipmentId = "Equipment404";
                    ecmc.MessageCatalogID = 79;
                    break;
                case 10003:
                    equipment.EquipmentId = "Equipment505";
                    ecmc.MessageCatalogID = 79;
                    break;
                case 10004:
                    equipment.EquipmentId = "Equipment606";
                    ecmc.MessageCatalogID = 79;
                    break;
                case 10005:
                    equipment.EquipmentId = "OPC-Dev01";
                    ecmc.MessageCatalogID = 84;
                    break;
                case 10006:
                    equipment.EquipmentId = "A001";
                    ecmc.MessageCatalogID = 84;
                    break;
                case 10007:
                    equipment.EquipmentId = "Windwos-OPC-UA-Publisher-v212";
                    ecmc.MessageCatalogID = 85;
                    break;
                default:
                    throw new ArgumentException();
            }

            ecmc.MessageCatalog = generateMessageCatalogForTest(ecmc.MessageCatalogID);
            ec.EquipmentClassMessageCatalog.Add(ecmc);

            equipment.EquipmentClass = ec;

            equipments.Add(equipment);
            return equipments;
        }

        private MessageCatalog generateMessageCatalogForTest(int messageCatalogId)
        {
            MessageCatalog mc = new MessageCatalog();
            mc.Id = messageCatalogId;

            switch (messageCatalogId)
            {
                case 79:
                    mc.Name = "Message102";
                    mc.MonitorFrequenceInMinSec = 10000;
                    break;
                case 84:
                    mc.Name = "OPCUA-Msg-Temperature_C";
                    mc.MonitorFrequenceInMinSec = 10000;
                    break;
                case 85:
                    mc.Name = "ICP_DAS-UA5231";
                    mc.MonitorFrequenceInMinSec = 15000;
                    break;
                default:
                    throw new ArgumentException();
            }

            mc.MessageElement = generateMessageElementList(messageCatalogId);
            mc.EventRuleCatalog = generateEventRuleCatalogList(messageCatalogId);
            return mc;
        }

        private List<MessageElement> generateMessageElementList(int messageCatalogId)
        {
            List<MessageElement> msgElementList = new List<MessageElement>();

            switch (messageCatalogId)
            {
                case 79:
                    msgElementList.Add(createMessageElement(messageCatalogId, 444));
                    msgElementList.Add(createMessageElement(messageCatalogId, 445));
                    msgElementList.Add(createMessageElement(messageCatalogId, 446));
                    msgElementList.Add(createMessageElement(messageCatalogId, 447));
                    msgElementList.Add(createMessageElement(messageCatalogId, 448));
                    msgElementList.Add(createMessageElement(messageCatalogId, 449));
                    break;
                case 84:
                    msgElementList.Add(createMessageElement(messageCatalogId, 470));
                    msgElementList.Add(createMessageElement(messageCatalogId, 471));
                    msgElementList.Add(createMessageElement(messageCatalogId, 472));
                    msgElementList.Add(createMessageElement(messageCatalogId, 473));
                    msgElementList.Add(createMessageElement(messageCatalogId, 474));
                    msgElementList.Add(createMessageElement(messageCatalogId, 475));
                    break;
                case 85:
                    msgElementList.Add(createMessageElement(messageCatalogId, 478));
                    msgElementList.Add(createMessageElement(messageCatalogId, 479));
                    msgElementList.Add(createMessageElement(messageCatalogId, 480));
                    msgElementList.Add(createMessageElement(messageCatalogId, 481));

                    msgElementList.Add(createMessageElement(messageCatalogId, 482));
                    msgElementList.Add(createMessageElement(messageCatalogId, 483));
                    msgElementList.Add(createMessageElement(messageCatalogId, 484));
                    msgElementList.Add(createMessageElement(messageCatalogId, 485));
                    break;
                default:
                    throw new ArgumentException();
            }

            return msgElementList;
        }

        private MessageElement createMessageElement(int messageCatalogId, int messageElementId)
        {
            MessageElement me = new MessageElement();
            me.Id = messageElementId;
            me.MessageCatalogID = messageCatalogId;
            switch (messageElementId)
            {
                case 442: // messageCatalogId:78
                    me.ElementName = "Color";
                    me.ElementDataType = "string";
                    me.ChildMessageCatalogID = null;
                    me.MandatoryFlag = false;
                    break;
                case 443: // messageCatalogId:78
                    me.ElementName = "StartTime";
                    me.ElementDataType = "datetime";
                    me.ChildMessageCatalogID = null;
                    me.MandatoryFlag = false;
                    break;
                case 444:
                    me.ElementName = "companyId";
                    me.ElementDataType = "numeric";
                    me.ChildMessageCatalogID = null;
                    me.MandatoryFlag = true;
                    break;
                case 445:
                    me.ElementName = "msgTimestamp";
                    me.ElementDataType = "datetime";
                    me.ChildMessageCatalogID = null;
                    me.MandatoryFlag = true;
                    break;
                case 446:
                    me.ElementName = "equipmentId";
                    me.ElementDataType = "string";
                    me.ChildMessageCatalogID = null;
                    me.MandatoryFlag = true;
                    break;
                case 447:
                    me.ElementName = "equipmentRunStatus";
                    me.ElementDataType = "numeric";
                    me.ChildMessageCatalogID = null;
                    me.MandatoryFlag = true;
                    break;
                case 448:
                    me.ElementName = "MachineA";
                    me.ElementDataType = "message";
                    me.ChildMessageCatalogID = 78;
                    me.MandatoryFlag = false;
                    break;
                case 449:
                    me.ElementName = "MachineB";
                    me.ElementDataType = "message";
                    me.ChildMessageCatalogID = 78;
                    me.MandatoryFlag = false;
                    break;
                case 470:
                    me.ElementName = "companyId";
                    me.ElementDataType = "numeric";
                    me.ChildMessageCatalogID = null;
                    me.MandatoryFlag = true;
                    break;
                case 471:
                    me.ElementName = "msgTimestamp";
                    me.ElementDataType = "datetime";
                    me.ChildMessageCatalogID = null;
                    me.MandatoryFlag = true;
                    break;
                case 472:
                    me.ElementName = "equipmentId";
                    me.ElementDataType = "string";
                    me.ChildMessageCatalogID = null;
                    me.MandatoryFlag = true;
                    break;
                case 473:
                    me.ElementName = "equipmentRunStatus";
                    me.ElementDataType = "numeric";
                    me.ChildMessageCatalogID = null;
                    me.MandatoryFlag = true;
                    break;
                case 474:
                    me.ElementName = "Temperature";
                    me.ElementDataType = "numeric";
                    me.ChildMessageCatalogID = null;
                    me.MandatoryFlag = false;
                    break;
                case 475:
                    me.ElementName = "TemperatureF";
                    me.ElementDataType = "numeric";
                    me.ChildMessageCatalogID = null;
                    me.MandatoryFlag = false;
                    break;
                case 478:
                    me.ElementName = "companyId";
                    me.ElementDataType = "numeric";
                    me.ChildMessageCatalogID = null;
                    me.MandatoryFlag = true;
                    break;
                case 479:
                    me.ElementName = "msgTimestamp";
                    me.ElementDataType = "datetime";
                    me.ChildMessageCatalogID = null;
                    me.MandatoryFlag = true;
                    break;
                case 480:
                    me.ElementName = "equipmentId";
                    me.ElementDataType = "string";
                    me.ChildMessageCatalogID = null;
                    me.MandatoryFlag = true;
                    break;
                case 481:
                    me.ElementName = "equipmentRunStatus";
                    me.ElementDataType = "numeric";
                    me.ChildMessageCatalogID = null;
                    me.MandatoryFlag = true;
                    break;
                case 482:
                    me.ElementName = "TemperatureC";
                    me.ElementDataType = "numeric";
                    me.ChildMessageCatalogID = null;
                    me.MandatoryFlag = false;
                    break;
                case 483:
                    me.ElementName = "CO2";
                    me.ElementDataType = "numeric";
                    me.ChildMessageCatalogID = null;
                    me.MandatoryFlag = false;
                    break;
                case 484:
                    me.ElementName = "Humidity";
                    me.ElementDataType = "numeric";
                    me.ChildMessageCatalogID = null;
                    me.MandatoryFlag = false;
                    break;
                case 485:
                    me.ElementName = "TemperatureF";
                    me.ElementDataType = "numeric";
                    me.ChildMessageCatalogID = null;
                    me.MandatoryFlag = false;
                    break;
                default:
                    throw new ArgumentException();
            }

            return me;
        }

        private List<EventRuleCatalog> generateEventRuleCatalogList(int messageCatalogId)
        {
            List<EventRuleCatalog> eventRuleCatalogList = new List<EventRuleCatalog>();
            EventRuleCatalog erc1, erc2, erc3, erc4;

            switch (messageCatalogId)
            {
                case 79:
                    erc1 = new EventRuleCatalog();
                    erc1.Id = 54;
                    erc1.MessageCatalogId = messageCatalogId;
                    erc1.Name = "MachineA_Color_Red_Warning";
                    erc1.Description = "MachineA_Color_Red_Warning";
                    erc1.AggregateInSec = 0;
                    erc1.ActiveFlag = true;
                    erc1.EventRuleItem = generateEventRuleItemList(erc1.Id);
                    erc1.EventInAction = generateEventInActionList(erc1.Id);
                    eventRuleCatalogList.Add(erc1);

                    erc2 = new EventRuleCatalog();
                    erc2.Id = 55;
                    erc2.MessageCatalogId = messageCatalogId;
                    erc2.Name = "MachineB_Color_Green_Warning";
                    erc2.Description = "MachineB_Color_Green_Warning";
                    erc2.AggregateInSec = 30;
                    erc2.ActiveFlag = true;
                    erc2.EventRuleItem = generateEventRuleItemList(erc2.Id);
                    erc2.EventInAction = generateEventInActionList(erc2.Id);
                    eventRuleCatalogList.Add(erc2);

                    erc3 = new EventRuleCatalog();
                    erc3.Id = 56;
                    erc3.MessageCatalogId = messageCatalogId;
                    erc3.Name = "MaA_NotWhite_n_NotBlack_OR_MaB_Yellow";
                    erc3.Description = "MaA_NotWhite_n_NotBlack_OR_MaB_Yellow";
                    erc3.AggregateInSec = 60;
                    erc3.ActiveFlag = true;
                    erc3.EventRuleItem = generateEventRuleItemList(erc3.Id);
                    erc3.EventInAction = generateEventInActionList(erc3.Id);
                    eventRuleCatalogList.Add(erc3);

                    erc4 = new EventRuleCatalog();
                    erc4.Id = 59;
                    erc4.MessageCatalogId = messageCatalogId;
                    erc4.Name = "MachineA_Color_Black_Warning";
                    erc4.Description = "MachineA_Color_Black_Warning";
                    erc4.AggregateInSec = 60;
                    erc4.ActiveFlag = false;
                    erc4.EventRuleItem = generateEventRuleItemList(erc4.Id);
                    erc4.EventInAction = generateEventInActionList(erc4.Id);
                    eventRuleCatalogList.Add(erc4);
                    break;
                case 84:
                    break;
                case 85:
                    erc1 = new EventRuleCatalog();
                    erc1.Id = 60;
                    erc1.MessageCatalogId = messageCatalogId;
                    erc1.Name = "TemperatureAlert";
                    erc1.Description = "TemperatureAlert";
                    erc1.AggregateInSec = 60;
                    erc1.ActiveFlag = true;
                    erc1.EventRuleItem = generateEventRuleItemList(erc1.Id);
                    erc1.EventInAction = generateEventInActionList(erc1.Id);
                    eventRuleCatalogList.Add(erc1);
                    break;
                default:
                    throw new ArgumentException();
            }

            return eventRuleCatalogList;
        }

        private List<EventInAction> generateEventInActionList(int eventRuleItemId)
        {
            List<EventInAction> eventInActionList = new List<EventInAction>();

            return eventInActionList;
        }

        private List<EventRuleItem> generateEventRuleItemList(int eventRuleItemId)
        {
            List<EventRuleItem> eriList = new List<EventRuleItem>();

            EventRuleItem eri1, eri2, eri3;
            switch (eventRuleItemId)
            {
                case 54:
                    eri1 = new EventRuleItem();
                    eri1.Id = 204;
                    eri1.EventRuleCatalogId = eventRuleItemId;
                    eri1.Ordering = 1;
                    eri1.MessageElementParentId = 448;
                    eri1.MessageElementId = 442;
                    eri1.EqualOperation = "=";
                    eri1.Value = "RED";
                    eri1.BitWiseOperation = "end";
                    eri1.MessageElement = createMessageElement(79, 448);
                    eri1.MessageElement1 = createMessageElement(78, 442);
                    eriList.Add(eri1);
                    break;
                case 55:
                    eri1 = new EventRuleItem();
                    eri1.Id = 205;
                    eri1.EventRuleCatalogId = eventRuleItemId;
                    eri1.Ordering = 1;
                    eri1.MessageElementParentId = 449;
                    eri1.MessageElementId = 442;
                    eri1.EqualOperation = "=";
                    eri1.Value = "GREEN";
                    eri1.BitWiseOperation = "end";
                    eri1.MessageElement = createMessageElement(79, 449);
                    eri1.MessageElement1 = createMessageElement(78, 442);
                    eriList.Add(eri1);
                    break;
                case 56:
                    eri1 = new EventRuleItem();
                    eri1.Id = 206;
                    eri1.EventRuleCatalogId = eventRuleItemId;
                    eri1.Ordering = 1;
                    eri1.MessageElementParentId = 448;
                    eri1.MessageElementId = 442;
                    eri1.EqualOperation = "!=";
                    eri1.Value = "WHITE";
                    eri1.BitWiseOperation = "AND";
                    eri1.MessageElement = createMessageElement(79, 448);
                    eri1.MessageElement1 = createMessageElement(78, 442);
                    eriList.Add(eri1);

                    eri2 = new EventRuleItem();
                    eri2.Id = 207;
                    eri2.EventRuleCatalogId = eventRuleItemId;
                    eri2.Ordering = 2;
                    eri2.MessageElementParentId = 448;
                    eri2.MessageElementId = 442;
                    eri2.EqualOperation = "!=";
                    eri2.Value = "BLACK";
                    eri2.BitWiseOperation = "OR";
                    eri2.MessageElement = createMessageElement(79, 448);
                    eri2.MessageElement1 = createMessageElement(78, 442);
                    eriList.Add(eri2);

                    eri3 = new EventRuleItem();
                    eri3.Id = 208;
                    eri3.EventRuleCatalogId = eventRuleItemId;
                    eri3.Ordering = 3;
                    eri3.MessageElementParentId = 449;
                    eri3.MessageElementId = 442;
                    eri3.EqualOperation = "=";
                    eri3.Value = "YELLOW";
                    eri3.BitWiseOperation = "end";
                    eri3.MessageElement = createMessageElement(79, 449);
                    eri3.MessageElement1 = createMessageElement(78, 442);
                    eriList.Add(eri3);
                    break;
                case 59:
                    eri1 = new EventRuleItem();
                    eri1.Id = 213;
                    eri1.EventRuleCatalogId = eventRuleItemId;
                    eri1.Ordering = 1;
                    eri1.MessageElementParentId = 448;
                    eri1.MessageElementId = 442;
                    eri1.EqualOperation = "=";
                    eri1.Value = "BLACK";
                    eri1.BitWiseOperation = "end";
                    eri1.MessageElement = createMessageElement(79, 448);
                    eri1.MessageElement1 = createMessageElement(78, 442);
                    eriList.Add(eri1);
                    break;
                case 60:
                    eri1 = new EventRuleItem();
                    eri1.Id = 216;
                    eri1.EventRuleCatalogId = eventRuleItemId;
                    eri1.Ordering = 1;
                    eri1.MessageElementParentId = null;
                    eri1.MessageElementId = 482;
                    eri1.EqualOperation = ">=";
                    eri1.Value = "50";
                    eri1.BitWiseOperation = "AND";
                    eri1.MessageElement = null;
                    eri1.MessageElement1 = createMessageElement(85, 482);
                    eriList.Add(eri1);

                    eri2 = new EventRuleItem();
                    eri2.Id = 217;
                    eri2.EventRuleCatalogId = eventRuleItemId;
                    eri2.Ordering = 2;
                    eri2.MessageElementParentId = null;
                    eri2.MessageElementId = 483;
                    eri2.EqualOperation = ">=";
                    eri2.Value = "60";
                    eri2.BitWiseOperation = "end";
                    eri2.MessageElement = null;
                    eri2.MessageElement1 = createMessageElement(85, 483);
                    eriList.Add(eri2);
                    break;
            }

            return eriList;
        }
    }
}
