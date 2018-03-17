using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel.DataAnnotations;
using sfShareLib;

namespace sfAPIService.Models
{
    public class EventRuleItemModel
    {
        public class Format_Detail
        {
            public int Id { get; set; }
            public int EventRuleCatalogId { get; set; }
            public int Ordering { get; set; }
            public int? MessageElementParentId { get; set; }
            public string MessageElementParentName { get; set; }
            public int MessageElementId { get; set; }
            public string MessageElementName { get; set; }
            public string EqualOperation { get; set; }
            public string Value { get; set; }
            public string BitWiseOperation { get; set; }
        }
        public class Format_Update
        {
            public List<RuleItem> Rule { get; set; }
        }
        public class RuleItem
        {
            [Required]
            public int Ordering { get; set; }
            public int? MessageElementParentId { get; set; }
            [Required]
            public int MessageElementId { get; set; }
            /// <summary>
            /// Should be : =, != ... 
            /// </summary>            
            [Required]
            [MaxLength(20)]
            public string EqualOperation { get; set; }
            [Required]
            [MaxLength(50)]
            public string Value { get; set; }
            /// <summary>
            /// Should be : AND, OR, XOR, END
            /// </summary>
            [Required]
            [MaxLength(10)]
            public string BitWiseOperation { get; set; }
        }
               
        public List<Format_Detail> GetAllByEventRuleCatalogId(int EventRuleCatalogId)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                var L2Enty = from c in dbEntity.EventRuleItem.AsNoTracking()
                             where c.EventRuleCatalogId == EventRuleCatalogId
                             select c;

                //MessageElementParentName = s.MessageElement
                //MessageElementName = s.MessageElement1
                return L2Enty.Select(s => new Format_Detail()
                {
                    Id = s.Id,
                    EventRuleCatalogId = s.EventRuleCatalogId,
                    Ordering = s.Ordering,
                    MessageElementParentId = s.MessageElementParentId,
                    MessageElementParentName = s.MessageElement == null ? "" : s.MessageElement.ElementName,
                    MessageElementId = s.MessageElementId,
                    MessageElementName = s.MessageElement1 == null ? "" : s.MessageElement1.ElementName,
                    EqualOperation = s.EqualOperation,
                    Value = s.Value,
                    BitWiseOperation = s.BitWiseOperation
                }).ToList<Format_Detail>();
            }
        }

        public void UpdateAllByEventRuleCatalogId(int EventRuleCatalogId, Format_Update parseData)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                //Delete all existing ruleItem
                List<EventRuleItem> existingDataList = (from c in dbEntity.EventRuleItem
                                                        where c.EventRuleCatalogId == EventRuleCatalogId
                                                        select c).ToList<EventRuleItem>();
                dbEntity.EventRuleItem.RemoveRange(existingDataList);
                dbEntity.SaveChanges();

                if (parseData.Rule.Count > 0)
                {
                    foreach (var rule in parseData.Rule)
                    {
                        dbEntity.EventRuleItem.Add(new EventRuleItem()
                        {
                            EventRuleCatalogId = EventRuleCatalogId,
                            Ordering = rule.Ordering,
                            MessageElementParentId = rule.MessageElementParentId,
                            MessageElementId = rule.MessageElementId,
                            EqualOperation = rule.EqualOperation,
                            Value = rule.Value,
                            BitWiseOperation = rule.BitWiseOperation
                        });
                    }
                    dbEntity.SaveChanges();
                }
            }
        }
    }
}