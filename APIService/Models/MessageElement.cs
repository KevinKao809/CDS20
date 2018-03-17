using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Dynamic;

using sfShareLib;
using System.ComponentModel.DataAnnotations;

namespace sfAPIService.Models
{
    public class MessageElementModels
    {
        public class Detail
        {
            public int Id { get; set; }
            public int MessageCatalogID { get; set; }
            public string ElementName { get; set; }
            public string ElementDataType { get; set; }
            public int? ChildMessageCatalogID { get; set; }
            public bool MandatoryFlag { get; set; }
            public bool CDSMandatoryFlag { get; set; }
            public bool ShowOnEquipmentList { get; set; }
            public bool ShowOnFactoryBoard { get; set; }
        }
        public class Add
        {
            public int MessageCatalogID { get; set; }
            public string ElementName { get; set; }
            public string ElementDataType { get; set; }
            public int? ChildMessageCatalogID { get; set; }
            [Required]
            public bool MandatoryFlag { get; set; }
            [Required]
            public bool CDSMandatoryFlag { get; set; }
            public bool ShowOnEquipmentList { get; set; }
            public bool ShowOnFactoryBoard { get; set; }
        }

        public class Update
        {
            public string ElementName { get; set; }
            public string ElementDataType { get; set; }
            public int? ChildMessageCatalogID { get; set; }
            [Required]
            public bool MandatoryFlag { get; set; }
            [Required]
            public bool CDSMandatoryFlag { get; set; }
            public bool ShowOnEquipmentList { get; set; }
            public bool ShowOnFactoryBoard { get; set; }
        }

        public List<Detail> GetAllMessageElementByMessageCatalogId(int messageCatalogId)
        {
            DBHelper._MessageElement dbhelp = new DBHelper._MessageElement();
            
            List<Detail> result = new List<Detail>();

            foreach (var element in dbhelp.GetAllByMessageCatalog(messageCatalogId))
            {
                if (element.ChildMessageCatalogID != null)
                {
                    DBHelper._MessageCatalog dbhelp_messageCatalog = new DBHelper._MessageCatalog();
                    if (dbhelp_messageCatalog.GetByid((int)element.ChildMessageCatalogID) == null)
                        continue;
                }

                result.Add(new Detail()
                {
                    Id = element.Id,
                    MessageCatalogID = element.MessageCatalogID,
                    ElementName = element.ElementName,
                    ElementDataType = element.ElementDataType,
                    ChildMessageCatalogID = element.ChildMessageCatalogID,
                    MandatoryFlag = element.MandatoryFlag,
                    CDSMandatoryFlag = element.CDSMandatoryFlag
                });
            }

            return result;
        }
        
        public void addMessageElement(Add messageElement)
        {
            DBHelper._MessageElement dbhelp = new DBHelper._MessageElement();

            var newMessageElement = new MessageElement()
            {
                MessageCatalogID = messageElement.MessageCatalogID,
                ElementName = messageElement.ElementName,
                ElementDataType = messageElement.ElementDataType,
                ChildMessageCatalogID = messageElement.ElementDataType.ToLower().Equals("message") ? messageElement.ChildMessageCatalogID : null,
                MandatoryFlag = messageElement.MandatoryFlag,
                CDSMandatoryFlag = messageElement.CDSMandatoryFlag
            };
            dbhelp.Add(newMessageElement);
        }

        public void updateMessageElement(int id, Update messageElement)
        {
            DBHelper._MessageElement dbhelp = new DBHelper._MessageElement();
            MessageElement existingMessageElement = dbhelp.GetByid(id);
            existingMessageElement.ElementName = messageElement.ElementName;
            existingMessageElement.ElementDataType = messageElement.ElementDataType;
            existingMessageElement.ChildMessageCatalogID = messageElement.ElementDataType.ToLower().Equals("message") ? messageElement.ChildMessageCatalogID : null;
            existingMessageElement.MandatoryFlag = messageElement.MandatoryFlag;
            existingMessageElement.CDSMandatoryFlag = messageElement.CDSMandatoryFlag;

            dbhelp.Update(existingMessageElement);
        }

        public void deleteMessageElement(int id)
        {
            DBHelper._MessageElement dbhelp = new DBHelper._MessageElement();
            MessageElement existingMessageElement = dbhelp.GetByid(id);

            dbhelp.Delete(existingMessageElement);
        }
        
    }
}