using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using sfShareLib;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace sfAPIService.Models
{
    public class UserRolePermissionModel
    {
        public class Format_Detail
        {
            public int UserRoleId { get; set; }
            public int Code { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
        }
        
        public List<Format_Detail> GetAllByUserRoleId(int userRoleId)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                var L2Enty = from c in dbEntity.UserRolePermission.AsNoTracking()
                             where c.UserRoleID == userRoleId
                             select c;

                return L2Enty.Select(s => new Format_Detail()
                {
                    UserRoleId = s.UserRoleID,
                    Code = s.PermissionCatalogCode,
                    Name = s.PermissionCatalog.Name,
                    Description = s.PermissionCatalog.Description
                }).ToList<Format_Detail>();
            }
        }

        public List<Format_Detail> GetAllByUserRoleId(List<int> userRoleIdList)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                var L2Enty = from c in dbEntity.UserRolePermission.AsNoTracking()
                             where userRoleIdList.Contains(c.UserRoleID)
                             orderby c.UserRoleID, c.PermissionCatalogCode
                             select c;

                return L2Enty.Select(s => new Format_Detail()
                {
                    UserRoleId = s.UserRoleID,
                    Code = s.PermissionCatalogCode,
                    Name = s.PermissionCatalog.Name,
                    Description = s.PermissionCatalog.Description
                }).ToList<Format_Detail>();
            }
        }

        public void CreateManyByUserRoleId(int userRoleId, List<int> permissionCatalogList)
        {            
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                List<UserRolePermission> dataModelList = new List<UserRolePermission>();
                foreach (int permissionCatalogCode in permissionCatalogList)
                {
                    dataModelList.Add(new UserRolePermission {
                        UserRoleID = userRoleId,
                        PermissionCatalogCode = permissionCatalogCode
                    });
                }
                dbEntity.UserRolePermission.AddRange(dataModelList);
                dbEntity.SaveChanges();
            }           
        }
        
        public void DeleteAllByUserRoleId(int userRoleId)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                var dataModelList = (from c in dbEntity.UserRolePermission
                                     where c.UserRoleID == userRoleId
                                     select c).ToList();

                dbEntity.UserRolePermission.RemoveRange(dataModelList);
                dbEntity.SaveChanges();
            }
        }
    }
}