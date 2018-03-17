using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using sfShareLib;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace sfAPIService.Models
{
    public class EmployeeInRoleModel
    {
        public class Format_Detail
        {
            public int UserRoleId { get; set; }
            public string UserRoleName { get; set; }
        }

        public class Format_Create
        {
            [Required]
            public List<int> UserRoleIdList { get; set; }
        }

        public class Format_Update
        {
            public List<int> UserRoleIdList { get; set; }
        }

        public List<Format_Detail> GetAllByEmployeeId(int employeeId)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                var L2Enty = (from c in dbEntity.EmployeeInRole.AsNoTracking()
                              where c.EmployeeID == employeeId
                              select c);

                return L2Enty.Select(s => new Format_Detail()
                {
                    UserRoleId = s.UserRoleID,
                    UserRoleName = s.UserRole.Name
                }).ToList<Format_Detail>();
            }
        }
        

        public void CreateManyByEmployeeId(int employeeId, List<int> userRoleList)
        {            
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                List<EmployeeInRole> dataList = new List<EmployeeInRole>();
                foreach (int userRoleId in userRoleList)
                {
                    dataList.Add(new EmployeeInRole {
                        EmployeeID = employeeId,
                        UserRoleID = userRoleId
                    });
                }
                dbEntity.EmployeeInRole.AddRange(dataList);
                dbEntity.SaveChanges();
            }           
        }
        
        public void DeleteAllByEmployeeId(int employeeId)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                var existingDataList = (from c in dbEntity.EmployeeInRole
                                        where c.EmployeeID == employeeId
                                        select c).ToList();

                dbEntity.EmployeeInRole.RemoveRange(existingDataList);
                dbEntity.SaveChanges();
            }
        }
    }
}