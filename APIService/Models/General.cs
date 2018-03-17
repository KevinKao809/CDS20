using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using sfShareLib;

namespace sfAPIService.Models
{
    public class General
    {
        public static bool IsFactoryUnderCompany(int factoryId, int companyId)
        {
            DBHelper._Factory dbhelp_factory = new DBHelper._Factory();
            var factory = dbhelp_factory.GetByid(factoryId, companyId);
            if (factory == null)
                return false;
            else
                return true;
        }

        public static bool IsEquipmentUnderCompany(int equipmentId, int companyId)
        {
            DBHelper._Equipment dbhelp = new DBHelper._Equipment();
            if (companyId == dbhelp.GetCompanyId(equipmentId))
                return true;
            else
                return false;
        }
    }
}