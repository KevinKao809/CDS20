using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Dynamic;

using sfShareLib;
using System.ComponentModel.DataAnnotations;
using CDSShareLib.Helper;

namespace sfAPIService.Models
{
    public class MessageCatalogModel
    {
        public class Format_Detail
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public int MonitorFrequenceInMinSec { get; set; }
            public bool ChildMessageFlag { get; set; }
        }
        public class Format_Create
        {
            [Required]
            public string Name { get; set; }
            public string Description { get; set; }
            /// <summary>
            /// 使用者未填，預設請填入30
            /// </summary>
            [Required]
            public int MonitorFrequenceInMinSec { get; set; }
            [Required]
            public bool ChildMessageFlag { get; set; }
        }
        public class Format_Update
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public int? MonitorFrequenceInMinSec { get; set; }
            public bool? ChildMessageFlag { get; set; }
        }

        public class Schema_element
        {
            public string Name { get; set; }
            public string DataType { get; set; }
            public bool MandatoryFlag { get; set; }
        }

        public class Schema
        {
            public int MessageCatalogId { get; set; }
            public string Name { get; set; }
            public List<Schema_element> ElementList { get; set; }
        }

        public class Elements
        {
            public int Id { get; set; }
            public int? ParentId { get; set; }
            public string Name { get; set; }
            public string DataType { get; set; }
            public bool MandatoryFlag { get; set; }
        }

        public class Template
        {
            public string MessageName { get; set; }
            public int MessageId { get; set; }
            public dynamic MessagePayload { get; set; } 
        }

        public List<Format_Detail> GetAllByCompanyId(int companyId, int childFlag)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                var L2Enty = from c in dbEntity.MessageCatalog.AsNoTracking()
                             where c.CompanyID == companyId
                             select c;

                List<MessageCatalog> messageCatalogList = new List<MessageCatalog>();

                switch (childFlag)
                {
                    case -1:
                        messageCatalogList = (from c in dbEntity.MessageCatalog.AsNoTracking()
                                              where c.CompanyID == companyId
                                              select c).ToList<MessageCatalog>();
                        break;
                    case 0:
                        messageCatalogList = (from c in dbEntity.MessageCatalog.AsNoTracking()
                                              where c.CompanyID == companyId && c.ChildMessageFlag == false
                                              select c).ToList<MessageCatalog>();
                        break;
                    case 1:
                        messageCatalogList = (from c in dbEntity.MessageCatalog.AsNoTracking()
                                              where c.CompanyID == companyId && c.ChildMessageFlag == true
                                              select c).ToList<MessageCatalog>();
                        break;
                }

                return messageCatalogList.Select(s => new Format_Detail()
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description,
                    MonitorFrequenceInMinSec = s.MonitorFrequenceInMinSec,
                    ChildMessageFlag = s.ChildMessageFlag
                }).ToList<Format_Detail>();
            }
        }

        public Format_Detail GetById(int id)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                MessageCatalog existingData = (from c in dbEntity.MessageCatalog.AsNoTracking()
                                               where c.Id == id
                                               select c).SingleOrDefault<MessageCatalog>();
                if (existingData == null)
                    throw new CDSException(10701);

                return new Format_Detail()
                {
                    Id = existingData.Id,
                    Name = existingData.Name,
                    Description = existingData.Description,
                    MonitorFrequenceInMinSec = existingData.MonitorFrequenceInMinSec,
                    ChildMessageFlag = existingData.ChildMessageFlag
                };
            }
        }

        public int Create(int companyId, Format_Create parseData)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                MessageCatalog newData = new MessageCatalog();
                newData.CompanyID = companyId;
                newData.Name = parseData.Name;
                newData.Description = parseData.Description ?? "";
                newData.MonitorFrequenceInMinSec = parseData.MonitorFrequenceInMinSec;
                newData.ChildMessageFlag = parseData.ChildMessageFlag;

                dbEntity.MessageCatalog.Add(newData);
                dbEntity.SaveChanges();
                return newData.Id;
            }
        }

        public void Update(int id, Format_Update parseData)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                var existingData = dbEntity.MessageCatalog.Find(id);
                if (existingData == null)
                    throw new CDSException(10701);

                if (parseData.Name != null)
                    existingData.Name = parseData.Name;

                if (parseData.Description != null)
                    existingData.Description = parseData.Description;

                if (parseData.MonitorFrequenceInMinSec.HasValue)
                    existingData.MonitorFrequenceInMinSec = (int) parseData.MonitorFrequenceInMinSec;

                if (parseData.ChildMessageFlag.HasValue)
                    existingData.ChildMessageFlag = (bool)parseData.ChildMessageFlag;

                dbEntity.SaveChanges();
            }
        }

        public void DeleteById(int id)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                MessageCatalog existingData = dbEntity.MessageCatalog.Find(id);
                if (existingData == null)
                    throw new CDSException(10701);

                dbEntity.MessageCatalog.Remove(existingData);
                dbEntity.SaveChanges();
            }
        }



        //public Template GetMessageCatalogTemplate(int messageId, int iotHubDeviceId)
        //{
        //    DBHelper._MessageCatalog dbhelp = new DBHelper._MessageCatalog();
        //    MessageCatalog msgCatalog = dbhelp.GetByid(messageId);

        //    dynamic msgCatalogPayload = new ExpandoObject();
        //    GetMessageCatalogPayload(messageId, msgCatalog.CompanyID, iotHubDeviceId, ref msgCatalogPayload, "");

        //    Template template = new Template();
        //    template.MessageId = messageId;
        //    template.MessageName = msgCatalog.Name;
        //    template.MessagePayload = msgCatalogPayload;

        //    return template;
        //}
        //public Schema GetMessageCatalogElementSchema(int messageCatalogId)
        //{
        //    DBHelper._MessageCatalog dbhelp_msgCatalog = new DBHelper._MessageCatalog();
        //    DBHelper._MessageElement dbhelp_msgElement = new DBHelper._MessageElement();
        //    Schema returnSchem = new Schema();

        //    var msgCatalog = dbhelp_msgCatalog.GetByid(messageCatalogId);
        //    var msgElements = dbhelp_msgElement.GetAllByMessageCatalog(messageCatalogId);

        //    returnSchem.MessageCatalogId = msgCatalog.Id;
        //    returnSchem.Name = msgCatalog.Name;
        //    returnSchem.ElementList = new List<Schema_element>();

        //    foreach (var msgElement in msgElements)
        //    {
        //        if (msgElement.ElementDataType.ToLower().Equals("message"))
        //        {
        //            var msgChildElements = dbhelp_msgElement.GetAllByMessageCatalog((int)msgElement.ChildMessageCatalogID);
        //            foreach (var msgChildElement in msgChildElements)
        //            {
        //                returnSchem.ElementList.Add(new Schema_element()
        //                {
        //                    Name = msgElement.ElementName + "_" +  msgChildElement.ElementName,
        //                    DataType = msgChildElement.ElementDataType,
        //                    MandatoryFlag = msgChildElement.MandatoryFlag
        //                });
        //            }
        //            continue;
        //        }

        //        returnSchem.ElementList.Add(new Schema_element()
        //        {
        //            Name = msgElement.ElementName,
        //            DataType = msgElement.ElementDataType,
        //            MandatoryFlag = msgElement.MandatoryFlag
        //        });
        //    }

        //    return returnSchem;
        //}

        //public List<Elements> GetMessageCatalogElements(int messageCatalogId)
        //{
        //    DBHelper._MessageCatalog dbhelp_msgCatalog = new DBHelper._MessageCatalog();
        //    DBHelper._MessageElement dbhelp_msgElement = new DBHelper._MessageElement();

        //    var msgCatalog = dbhelp_msgCatalog.GetByid(messageCatalogId);
        //    var msgElements = dbhelp_msgElement.GetAllByMessageCatalog(messageCatalogId);

        //    List<Elements> elementList = new List<Elements>();

        //    foreach (var msgElement in msgElements)
        //    {
        //        if (msgElement.ElementDataType.ToLower().Equals("message"))
        //        {
        //            var msgChildElements = dbhelp_msgElement.GetAllByMessageCatalog((int)msgElement.ChildMessageCatalogID);
        //            foreach (var msgChildElement in msgChildElements)
        //            {
        //                elementList.Add(new Elements()
        //                {
        //                    Id = msgChildElement.Id,
        //                    ParentId = msgElement.Id,
        //                    Name = msgElement.ElementName + "_" + msgChildElement.ElementName,
        //                    DataType = msgChildElement.ElementDataType,
        //                    MandatoryFlag = msgChildElement.MandatoryFlag
        //                });
        //            }
        //            continue;
        //        }
        //        else
        //        {
        //            elementList.Add(new Elements()
        //            {
        //                Id = msgElement.Id,
        //                Name = msgElement.ElementName,
        //                DataType = msgElement.ElementDataType,
        //                MandatoryFlag = msgElement.MandatoryFlag
        //            });
        //        }
        //    }

        //    return elementList;
        //}

        //private void GetMessageCatalogPayload(int messageCatalogId, int companyId, int iotHubDeviceId, ref dynamic msgCatalogPayload, string prefixName)
        //{
        //    DBHelper._MessageElement dbhelp_messageElement = new DBHelper._MessageElement();
        //    var msgElements = dbhelp_messageElement.GetAllByMessageCatalog(messageCatalogId);

        //    foreach (var msgElement in msgElements)
        //    {
        //        switch (msgElement.ElementDataType.ToLower())
        //        {
        //            case "string":
        //                if (msgElement.ElementName.ToLower().Equals("equipmentid"))
        //                {
        //                    DBHelper._Equipment dbhelp_equipment = new DBHelper._Equipment();
        //                    int count = 0;
        //                    string availableEquipmentIdList = "";
        //                    foreach (Equipment equipment in dbhelp_equipment.GetAllByIoTHubDeviceId(iotHubDeviceId))
        //                    {
        //                        if (count > 0)
        //                            availableEquipmentIdList += (", " + equipment.EquipmentId);
        //                        else
        //                            availableEquipmentIdList += (" || " + equipment.EquipmentId);
        //                        count++;
        //                    }
        //                    AddProperty(msgCatalogPayload, prefixName + msgElement.ElementName, msgElement.ElementDataType + " || " + (msgElement.MandatoryFlag ? "mandatory" : "option") + availableEquipmentIdList);
        //                }
        //                else
        //                    AddProperty(msgCatalogPayload, prefixName + msgElement.ElementName, msgElement.ElementDataType + " || " + (msgElement.MandatoryFlag ? "mandatory" : "option"));
        //                break;
        //            case "numeric":
        //                switch (msgElement.ElementName.ToLower())
        //                {
        //                    case "companyid":
        //                        AddProperty(msgCatalogPayload, prefixName + msgElement.ElementName, msgElement.ElementDataType + " || " + (msgElement.MandatoryFlag ? "mandatory" : "option") + " || " + companyId.ToString());
        //                        break;
        //                    case "equipmentrunstatus":
        //                        AddProperty(msgCatalogPayload, prefixName + msgElement.ElementName, msgElement.ElementDataType + " || " + (msgElement.MandatoryFlag ? "mandatory" : "option") + " || 0, 1, -1");
        //                        break;
        //                    default:
        //                        AddProperty(msgCatalogPayload, prefixName + msgElement.ElementName, msgElement.ElementDataType + " || " + (msgElement.MandatoryFlag ? "mandatory" : "option"));
        //                        break;
        //                }
        //                break;
        //            case "bool":
        //                AddProperty(msgCatalogPayload, prefixName + msgElement.ElementName, msgElement.ElementDataType + " || " + (msgElement.MandatoryFlag ? "mandatory" : "option") + " || True, False");
        //                break;
        //            case "datetime":
        //                AddProperty(msgCatalogPayload, prefixName + msgElement.ElementName, msgElement.ElementDataType + " || " + (msgElement.MandatoryFlag ? "mandatory" : "option") + " || yyyy-MM-ddTHH:mm:ss");
        //                break;
        //            case "message":
        //                if (messageCatalogId != (int)msgElement.ChildMessageCatalogID)
        //                    GetMessageCatalogPayload((int)msgElement.ChildMessageCatalogID, companyId, iotHubDeviceId, ref msgCatalogPayload, prefixName + msgElement.ElementName + "_");   //Recurssive Call                        
        //                break;
        //        }
        //    }
        //}

        private void AddProperty(ExpandoObject expando, string propertyName, object propertyValue)
        {
            // ExpandoObject supports IDictionary so we can extend it like this
            var expandoDict = expando as IDictionary<string, object>;
            if (expandoDict.ContainsKey(propertyName))
                expandoDict[propertyName] = propertyValue;
            else
                expandoDict.Add(propertyName, propertyValue);
        }
    }
}
