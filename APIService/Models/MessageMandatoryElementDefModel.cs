using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using sfShareLib;
using System.ComponentModel.DataAnnotations;
using CDSShareLib.Helper;

namespace sfAPIService.Models
{
    public class MessageMandatoryElementDefModel
    {
        public class Format_Detail
        {
            public int Id { get; set; }
            public string ElementName { get; set; }
            public string ElementDataType { get; set; }
            public bool MandatoryFlag { get; set; }
            public string Description { get; set; }
        }
        public class Format_Create
        {
            [Required]
            public string ElementName { get; set; }
            [Required]
            public string ElementDataType { get; set; }
            [Required]
            public bool MandatoryFlag { get; set; }
            public string Description { get; set; }
        }
        public class Format_Update
        {
            public string ElementName { get; set; }
            public string ElementDataType { get; set; }
            public bool? MandatoryFlag { get; set; }
            public string Description { get; set; }
        }

        public List<Format_Detail> GetAll()
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                var L2Enty = from c in dbEntity.MessageMandatoryElementDef.AsNoTracking()
                             select c;

                return L2Enty.Select(s => new Format_Detail()
                {
                    Id = s.Id,
                    ElementName = s.ElementName,
                    ElementDataType = s.ElementDataType,
                    Description = s.Description
                }).ToList<Format_Detail>();
            }
        }

        public Format_Detail GetById(int id)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                var existingData = (from c in dbEntity.MessageMandatoryElementDef.AsNoTracking()
                                    where c.Id == id
                                    select c).SingleOrDefault<MessageMandatoryElementDef>();

                if (existingData == null)
                    throw new CDSException(10701);

                return new Format_Detail()
                {
                    Id = existingData.Id,
                    ElementName = existingData.ElementName,
                    ElementDataType = existingData.ElementDataType,
                    Description = existingData.Description
                };
            }
        }

        public int Create(Format_Create parseData)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                MessageMandatoryElementDef newData = new MessageMandatoryElementDef();
                newData.ElementName = parseData.ElementName;
                newData.ElementDataType = parseData.ElementDataType;
                newData.Description = parseData.Description ?? "";
                newData.MandatoryFlag = parseData.MandatoryFlag;

                dbEntity.MessageMandatoryElementDef.Add(newData);
                dbEntity.SaveChanges();
                return newData.Id;
            }
        }

        public void Update(int id, Format_Update parseData)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                var existingData = dbEntity.MessageMandatoryElementDef.Find(id);
                if (existingData == null)
                    throw new CDSException(10701);

                if (parseData.ElementName != null)
                    existingData.ElementName = parseData.ElementName;

                if (parseData.ElementDataType != null)
                    existingData.ElementDataType = parseData.ElementDataType;

                if (parseData.Description != null)
                    existingData.Description = parseData.Description;

                if (parseData.MandatoryFlag.HasValue)
                    existingData.MandatoryFlag = (bool) parseData.MandatoryFlag;

                dbEntity.SaveChanges();
            }
        }

        public void DeleteById(int id)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                MessageMandatoryElementDef existingData = dbEntity.MessageMandatoryElementDef.Find(id);
                if (existingData == null)
                    throw new CDSException(10701);

                dbEntity.MessageMandatoryElementDef.Remove(existingData);
                dbEntity.SaveChanges();
            }
        }
    }
}