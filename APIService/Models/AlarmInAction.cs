using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using sfShareLib;
using System.ComponentModel.DataAnnotations;

namespace sfAPIService.Models
{
    public class EventInActionModels
    {
        public class Detail
        {
            public int? ApplicationId { get; set; }
            public string ApplicationName { get; set; }
        }
        public class Edit
        {
            public int[] ApplicationIdList { get; set; }
        }

        public List<Detail> GetAllApplicationByEventRuleCatalogId(int EventRuleCatalogId)
        {
            DBHelper._EventInAction dbhelp = new DBHelper._EventInAction();

            return dbhelp.GetAllByEventRuleCatalogId(EventRuleCatalogId).Select(s => new Detail()
            {
                ApplicationId = s.ApplicationId,
                ApplicationName = (s.Application == null) ? "" : s.Application.Name
            }).ToList<Detail>();

        }

        public void AttachApplication(int EventRuleCatalogId, Edit EventInAction)
        {
            DBHelper._EventInAction dbhelp = new DBHelper._EventInAction();
            List<EventInAction> newExternalApplicationList = new List<EventInAction>();
            List<EventInAction> existExternalApplicationList = dbhelp.GetAllByEventRuleCatalogId(EventRuleCatalogId);

            dbhelp.Delete(existExternalApplicationList);
            if (EventInAction != null)
            {
                foreach (int applicationId in EventInAction.ApplicationIdList)
                {
                    if (applicationId > 0)
                    {
                        newExternalApplicationList.Add(new EventInAction()
                        {
                            EventRuleCatalogId = EventRuleCatalogId,
                            ApplicationId = applicationId
                        });
                    }                    
                }
            }

            dbhelp.Add(newExternalApplicationList);
        }
    }
}