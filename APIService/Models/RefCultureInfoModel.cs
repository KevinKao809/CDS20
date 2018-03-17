using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using sfShareLib;
using System.Data.Entity;

namespace sfAPIService.Models
{
    public class RefCultureInfoModel
    {
        public class Format_Detail
        {
            public string CultureCode { get; set; }
            public string Name { get; set; }
        }

        public List<Format_Detail> GetAll()
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                var L2Enty = from c in dbEntity.RefCultureInfo.AsNoTracking()
                             select c;
                return L2Enty.Select(s => new Format_Detail()
                {
                    CultureCode = s.CultureCode,
                    Name = s.Name
                }).ToList<Format_Detail>();
            }
        }
    }
}