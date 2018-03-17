using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using sfShareLib;
using System.ComponentModel.DataAnnotations;

namespace sfAPIService.Models
{
    public class EquipmentClassMessageCatalogModel
    {
        public class Format_Detail
        {
            public int Id { get; set; }
            public int EquipmentClassId { get; set; }
            public int MessageCatalogId { get; set; }
            public string MessageCatalogName { get; set; }
        }
        public class Format_Update
        {
            public List<int> MessageCatalogId { get; set; }
        }
        public List<Format_Detail> GetAllByEquipmentClassId(int equipmentClassId)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                var L2Enty = from c in dbEntity.EquipmentClassMessageCatalog.AsNoTracking()
                             where c.EquipmentClassID == equipmentClassId
                             select c;

                return L2Enty.Select(s => new Format_Detail()
                {
                    Id = s.Id,
                    EquipmentClassId = s.EquipmentClassID,
                    MessageCatalogId = s.MessageCatalogID,
                    MessageCatalogName = s.MessageCatalog == null ? "" : s.MessageCatalog.Name
                }).ToList<Format_Detail>();
            }
        }
        
        public void UpdateByEquipmentClassId(int equipmentClassId, Format_Update parseData)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                var existingData = from c in dbEntity.EquipmentClassMessageCatalog
                                   where c.EquipmentClassID == equipmentClassId
                                   select c;
                dbEntity.EquipmentClassMessageCatalog.RemoveRange(existingData);

                if (parseData.MessageCatalogId != null)
                {
                    List<EquipmentClassMessageCatalog> newDataList = new List<EquipmentClassMessageCatalog>();
                    foreach(int messageCatalogId in parseData.MessageCatalogId)
                    {
                        newDataList.Add(new EquipmentClassMessageCatalog()
                        {
                            EquipmentClassID = equipmentClassId,
                            MessageCatalogID = messageCatalogId
                        });
                    }
                    dbEntity.EquipmentClassMessageCatalog.AddRange(newDataList);
                }
                     
                dbEntity.SaveChanges();
            }
        }
        
    }
}