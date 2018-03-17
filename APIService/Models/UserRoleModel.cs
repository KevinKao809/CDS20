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
    public class UserRoleModel
    {
        public class Format_Detail
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public List<UserRolePermissionModel.Format_Detail> PermissionCatalogList { get; set; }
        }

        public class Format_Create
        {
            [Required]
            [MaxLength(50)]
            public string Name { get; set; }
            public List<int> PermissionCatalogCode { get; set; }
        }

        public class Format_Update
        {
            [Required]
            [MaxLength(50)]
            public string Name { get; set; }
            public List<int> PermissionCatalogCode { get; set; }
        }

        public List<Format_Detail> GetAllByCompanyId(int companyId)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                List<Format_Detail> returnDataList = new List<Format_Detail>();
                var L2Enty = from c in dbEntity.UserRole.AsNoTracking()
                             where c.CompanyId == companyId
                             select c;

                returnDataList = L2Enty.Select(s => new Format_Detail()
                {
                    Id = s.Id,
                    Name = s.Name
                }).ToList();

                if (returnDataList.Count == 0)
                    return returnDataList;
                
                Dictionary<int, Format_Detail> returnDataDic = returnDataList.Select(s => new Format_Detail()
                {
                    Id = s.Id,
                    Name = s.Name,
                    PermissionCatalogList = new List<UserRolePermissionModel.Format_Detail>()
                }).ToDictionary(o => o.Id, o => o);

                //In order to keep performance
                List<int> userRoleIdList = returnDataList.Select(s => s.Id).ToList();

                UserRolePermissionModel model = new UserRolePermissionModel();
                var permissionCatalogList = model.GetAllByUserRoleId(userRoleIdList);

                // add permissionCatalog into each userRole
                foreach (var permissionCatalog in permissionCatalogList)
                {
                    returnDataDic[permissionCatalog.UserRoleId].PermissionCatalogList.Add(permissionCatalog);
                }

                //Clean old userRole data, insert userRole included permissionCatalog
                returnDataList.Clear();
                foreach (var returnData in returnDataDic)
                {
                    returnDataList.Add(returnData.Value);
                }
                return returnDataList;
            }
        }
                       
        public Format_Detail GetById(int id)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                var L2Enty = from c in dbEntity.UserRole.AsNoTracking()
                             where c.Id == id
                             select c;

                Format_Detail returnData = L2Enty.Select(s => new Format_Detail()
                {
                    Id = s.Id,
                    Name = s.Name
                }).SingleOrDefault<Format_Detail>();

                if (returnData == null)
                    throw new CDSException(12103);

                UserRolePermissionModel model = new UserRolePermissionModel();
                returnData.PermissionCatalogList = model.GetAllByUserRoleId(id);

                return returnData;
            }
        }

        public int Create(int companyId, Format_Create parseData)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                UserRole newData = new UserRole();
                newData.CompanyId = companyId;
                newData.Name = parseData.Name;

                try
                {
                    dbEntity.UserRole.Add(newData);
                    dbEntity.SaveChanges();
                }
                catch
                {
                    throw new CDSException(12104);
                }
                
                int userRoleId = newData.Id;

                if (parseData.PermissionCatalogCode != null)
                {
                    UserRolePermissionModel model = new UserRolePermissionModel();
                    model.CreateManyByUserRoleId(userRoleId, parseData.PermissionCatalogCode);
                }               

                return newData.Id;
            }
        }

        public void Update(int userRoleId, Format_Update parseData)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                UserRole existingData = dbEntity.UserRole.Find(userRoleId);
                if (existingData == null)
                    throw new CDSException(12103);

                if (existingData.Name != parseData.Name)
                {
                    try
                    {
                        existingData.Name = parseData.Name;
                        dbEntity.SaveChanges();
                    }
                    catch
                    {
                        throw new CDSException(12104);
                    }
                }               

                //adjust user role's permission
                UserRolePermissionModel model = new UserRolePermissionModel();
                model.DeleteAllByUserRoleId(userRoleId);

                if (parseData.PermissionCatalogCode != null)
                {                    
                    model.CreateManyByUserRoleId(userRoleId, parseData.PermissionCatalogCode);
                }                
            }
        }

        public void DeleteById(int id)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                UserRole existingData = dbEntity.UserRole.Find(id);
                if (existingData == null)
                    throw new CDSException(12103);

                dbEntity.Entry(existingData).State = EntityState.Deleted;
                dbEntity.SaveChanges();
            }
        }        
    }
}