using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using sfShareLib;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using CDSShareLib.Helper;

namespace sfAPIService.Models
{
    public class PermissionCatalogModel
    {
        public class Format_Detail
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public int Code { get; set; }
        }
        public class Format_Create
        {
            [Required]
            public string Name { get; set; }
            public string Description { get; set; }
            [Required]
            public int Code { get; set; }
        }

        public class Format_Update
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public int? Code { get; set; }
        }
        
        public List<Format_Detail> GetAll()
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                var L2Enty = from c in dbEntity.PermissionCatalog.AsNoTracking()
                             select c;

                return L2Enty.Select(s => new Format_Detail()
                {
                    Name = s.Name,
                    Description = s.Description,
                    Code = s.Code
                }).ToList<Format_Detail>();
            }
        }

        public Format_Detail GetByCode(int code)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                PermissionCatalog existingData = dbEntity.PermissionCatalog.Find(code);
                if (existingData == null)
                    throw new CDSException(11701);

                return new Format_Detail()
                {
                    Name = existingData.Name,
                    Description = existingData.Description,
                    Code = existingData.Code
                };
            }
        }

        public List<Format_Detail> GetAllPermissionByEmployeeId(int employeeId)
        {
            CDStudioEntities dbEntity = new CDStudioEntities();
            var L2Enty = from er in dbEntity.EmployeeInRole.AsNoTracking()
                         join urp in dbEntity.UserRolePermission.AsNoTracking() on er.UserRoleID equals urp.UserRoleID
                         where er.EmployeeID == employeeId
                         orderby urp.PermissionCatalog.Code ascending
                         select urp.PermissionCatalog;
            return L2Enty.Select(s => new Format_Detail
            {
                Code = s.Code,
                Description = s.Description,
                Name = s.Name
            }).ToList<Format_Detail>();            
        }

        public void Create(Format_Create parseModel)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                PermissionCatalog newData = new PermissionCatalog();
                newData.Name = parseModel.Name;
                newData.Description = parseModel.Description ?? "";
                newData.Code = parseModel.Code;

                dbEntity.PermissionCatalog.Add(newData);
                dbEntity.SaveChanges();
            }
        }

        public void Update(int code, Format_Update parseModel)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                var existingData = dbEntity.PermissionCatalog.Find(code);
                if (existingData == null)
                    throw new CDSException(11701);

                if (parseModel.Name != null)
                    existingData.Name = parseModel.Name;

                if (parseModel.Description != null)
                    existingData.Description = parseModel.Description;

                if (parseModel.Code.HasValue)
                    existingData.Code = (int)parseModel.Code;

                dbEntity.SaveChanges();
            }
        }

        public void DeleteByCode(int code)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                var L2Enty = from c in dbEntity.PermissionCatalog
                             where c.Code == code
                             select c;
                PermissionCatalog existingData = L2Enty.First();
                if (existingData == null)
                    throw new CDSException(11701);

                dbEntity.PermissionCatalog.Attach(existingData);
                dbEntity.PermissionCatalog.Remove(existingData);
                dbEntity.SaveChanges();
            }
        }
    }
}