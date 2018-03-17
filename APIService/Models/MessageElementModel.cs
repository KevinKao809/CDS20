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
    public class MessageElementModel
    {
        public class Format_Detail
        {
            public int Id { get; set; }
            public int MessageCatalogId { get; set; }
            public string ElementName { get; set; }
            public string ElementDataType { get; set; }
            public int? ChildMessageCatalogID { get; set; }
            public bool MandatoryFlag { get; set; }
        }
        public class Format_Create
        {
            [Required]
            public int MessageCatalogId { get; set; }
            [Required]
            public string ElementName { get; set; }
            /// <summary>
            /// Should be : bool, string, numeric, datetime, message
            /// </summary>
            [Required]
            public string ElementDataType { get; set; }
            public int? ChildMessageCatalogID { get; set; }
            [Required]
            public bool MandatoryFlag { get; set; }
        }

        public class Format_Update
        {
            public string ElementName { get; set; }
            public string ElementDataType { get; set; }
            /// <summary>
            /// If value is integer, must be sent!
            /// </summary>
            public int? ChildMessageCatalogID { get; set; }
            public bool? MandatoryFlag { get; set; }
        }
        public List<Format_Detail> GetAllByMessageCatalogId(int messageCatalogId)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                var L2Enty = from c in dbEntity.MessageElement.AsNoTracking()
                             where c.MessageCatalogID == messageCatalogId
                             select c;

                return L2Enty.Select(s => new Format_Detail()
                {
                    Id = s.Id,
                    ElementName = s.ElementName,
                    ElementDataType = s.ElementDataType,
                    ChildMessageCatalogID = s.ChildMessageCatalogID,
                    MandatoryFlag = s.MandatoryFlag
                }).ToList<Format_Detail>();
            }
        }

        public Format_Detail GetById(int id)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                MessageElement existingData = (from c in dbEntity.MessageElement.AsNoTracking()
                                               where c.Id == id
                                               select c).SingleOrDefault<MessageElement>();
                if (existingData == null)
                    throw new CDSException(10701);

                return new Format_Detail()
                {
                    Id = existingData.Id,
                    ElementName = existingData.ElementName,
                    ElementDataType = existingData.ElementDataType,
                    ChildMessageCatalogID = existingData.ChildMessageCatalogID,
                    MandatoryFlag = existingData.MandatoryFlag
                };
            }
        }

        public int Create(Format_Create parseData)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                MessageElement newData = new MessageElement();
                newData.MessageCatalogID = parseData.MessageCatalogId;
                newData.ElementName = parseData.ElementName;
                newData.ElementDataType = parseData.ElementDataType;
                newData.ChildMessageCatalogID = parseData.ChildMessageCatalogID;
                newData.MandatoryFlag = parseData.MandatoryFlag;
                newData.CDSMandatoryFlag = false;

                dbEntity.MessageElement.Add(newData);
                dbEntity.SaveChanges();
                return newData.Id;
            }
        }

        public void Update(int id, Format_Update parseData)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                var existingData = dbEntity.MessageElement.Find(id);
                if (existingData == null)
                    throw new CDSException(10701);

                if (parseData.ElementName != null)
                    existingData.ElementName = parseData.ElementName;

                if (parseData.ElementDataType != null)
                    existingData.ElementDataType = parseData.ElementDataType;                    

                if (parseData.MandatoryFlag.HasValue)
                    existingData.MandatoryFlag = (bool) parseData.MandatoryFlag;
                
                //ChildMessageCatalogID may be null or integer
                existingData.ChildMessageCatalogID = parseData.ChildMessageCatalogID;

                dbEntity.SaveChanges();
            }
        }

        public void DeleteById(int id)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                MessageElement existingData = dbEntity.MessageElement.Find(id);
                if (existingData == null)
                    throw new CDSException(10701);

                dbEntity.MessageElement.Remove(existingData);
                dbEntity.SaveChanges();
            }
        }
    }
}