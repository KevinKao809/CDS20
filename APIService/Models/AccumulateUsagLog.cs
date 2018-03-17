using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using sfShareLib;
using System.ComponentModel.DataAnnotations;
using System.Web.Helpers;
using CDSShareLib.Helper;

namespace sfAPIService.Models
{
    public class AccumulateUsagLog
    {
        public class Detail
        {
            public int CompanyId { get; set; }
            public string CompanyName { get; set; }
            public int? FactoryQty { get; set; }
            public int? EquipmentQty { get; set; }
            public long DeviceMessage { get; set; }
            
            public DateTime UpdatedAt { get; set; }
        }

        public List<Detail> getAll(int days, string order)
        {
            DBHelper._AccumulateUsageLog dbhelp = new DBHelper._AccumulateUsageLog();
            var usageLogList = dbhelp.GetAll(days, order);
            if (usageLogList == null)
                throw new CDSException(12101);

            return dbhelp.GetAll(days, order).Select(s => new Detail()
            {
                CompanyId = s.CompanyId,
                CompanyName = s.Company == null ? "" : s.Company.Name,
                FactoryQty = s.FactoryQty,
                EquipmentQty = s.EquipmentQty,
                UpdatedAt = s.UpdatedAt
            }).ToList<Detail>();
        }
        public List<Detail> getAllByCompanyId(int companyId, int days, string order)
        {
            DBHelper._AccumulateUsageLog dbhelp = new DBHelper._AccumulateUsageLog();
            var usageLogList = dbhelp.GetAllByCompanyId(companyId, days, order);
            if (usageLogList == null)
                throw new CDSException(12101);

            return usageLogList.Select(s => new Detail()
            {
                CompanyId = s.CompanyId,
                CompanyName = s.Company == null ? "" : s.Company.Name,
                FactoryQty = s.FactoryQty,
                EquipmentQty = s.EquipmentQty,
                UpdatedAt = s.UpdatedAt
            }).ToList<Detail>();
        }

        public Detail getLastByCompanyId(int companyId)
        {
            DBHelper._AccumulateUsageLog dbhelp = new DBHelper._AccumulateUsageLog();
            AccumulateUsageLog usageLog = dbhelp.GetLastByCommpanyId(companyId);
            if (usageLog == null)
                throw new CDSException(12102);

            return new Detail()
            {
                CompanyId = usageLog.CompanyId,
                CompanyName = usageLog.Company == null ? "" : usageLog.Company.Name,
                FactoryQty = usageLog.FactoryQty,
                EquipmentQty = usageLog.EquipmentQty,
                UpdatedAt = usageLog.UpdatedAt
            };
        }
    }
}