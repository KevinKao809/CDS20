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
    public class DashboardModel
    {
        public class Format_Detail
        {
            public int Id { get; set; }
            public string DashboardType { get; set; }
            public int? FactoryId { get; set; }
            public string FactoryName { get; set; }
            public int? DashboardId { get; set; }
            public string DashboardName { get; set; }
            public int? EquipmentClassId { get; set; }
            public string EquipmentClassName { get; set; }
            public int? EquipmentId { get; set; }
            public string EquipmentName { get; set; }
        }

        public class Format_DetailForFactory
        {
            public int Id { get; set; }
            public string DashboardType { get; set; }
            public string FactorydId { get; set; }
            public string FactoryName { get; set; }
            public string FactoryDescription { get; set; }
        }

        public class Format_DetailForEquipmentClassBoard
        {
            public int Id { get; set; }
            public string DashboardType { get; set; }
            public string EquipmentClassyId { get; set; }
            public string EquipmentClassName { get; set; }
            public string EquipmentClassDescription { get; set; }
        }

        public class Format_Create
        {
            public int? FactoryId { get; set; }
            public int? EquipmentClassId { get; set; }
            public int? EquipmentId { get; set; }
            [Required]
            public string DashboardType { get; set; }
        }

        //public List<Detail> getAllDashboardByCompnayId(int companyId, string type = null)
        //{
        //    DBHelper._Dashboard dbhelp = new DBHelper._Dashboard();

        //    if (string.IsNullOrEmpty(type))
        //    {
        //        return dbhelp.GetAllByCompanyId(companyId).Select(s => new Detail()
        //        {
        //            Id = s.Id,
        //            CompanyId = s.CompanyID,
        //            DashboardId = s.DashboardID,
        //            DashboardName = (s.Dashboard == null ? "" : s.Dashboard.Name),
        //            EquipmentId = s.EquipmentID,
        //            EquipmentName = (s.Equipment == null ? "" : s.Equipment.Name),
        //            EquipmentClassId = s.EquipmentClassID,
        //            EquipmentClassName = (s.EquipmentClass == null ? "" : s.EquipmentClass.Name),
        //            DashboardType = s.DashboardType
        //        }).ToList<Detail>();
        //    }
        //    else
        //    {
        //        return dbhelp.GetAllByCompanyId(companyId, type).Select(s => new Detail()
        //        {
        //            Id = s.Id,
        //            CompanyId = s.CompanyID,
        //            DashboardId = s.DashboardID,
        //            DashboardName = (s.Dashboard == null ? "" : s.Dashboard.Name),
        //            EquipmentId = s.EquipmentID,
        //            EquipmentName = (s.Equipment == null ? "" : s.Equipment.Name),
        //            EquipmentClassId = s.EquipmentClassID,
        //            EquipmentClassName = (s.EquipmentClass == null ? "" : s.EquipmentClass.Name),
        //            DashboardType = s.DashboardType
        //        }).ToList<Detail>();
        //    }            
        //}

        //public List<Detail> getAllDashboardByDashboardId(int DashboardId)
        //{
        //    DBHelper._Dashboard dbhelp = new DBHelper._Dashboard();

        //    return dbhelp.GetAllByDashboardId(DashboardId).Select(s => new Detail()
        //    {
        //        Id = s.Id,
        //        CompanyId = s.CompanyID,
        //        DashboardId = s.DashboardID,
        //        DashboardName = (s.Dashboard == null ? "" : s.Dashboard.Name),
        //        EquipmentId = s.EquipmentID,
        //        EquipmentName = (s.Equipment == null ? "" : s.Equipment.Name),
        //        EquipmentClassId = s.EquipmentClassID,
        //        EquipmentClassName = (s.EquipmentClass == null ? "" : s.EquipmentClass.Name),
        //        DashboardType = s.DashboardType
        //    }).ToList<Detail>();
        //}

        //public List<Detail_EquipmentClass> getAllEquipmentClassDashboardByCompnayId(int companyId)
        //{
        //    List<Detail_EquipmentClass> resultList = new List<Detail_EquipmentClass>();
        //    CDStudioEntities dbEntity = new CDStudioEntities();
        //    var allDashboardIds = from c in dbEntity.Dashboard.AsNoTracking()
        //                       where c.CompanyId == companyId
        //                       select c.Id;

        //    var allEquipmentClassesGroup = from c in dbEntity.Equipment.AsNoTracking()
        //                              where allDashboardIds.Contains(c.DashboardId)
        //                              join ec in dbEntity.EquipmentClass on c.EquipmentClassId equals ec.Id
        //                              select ec;

        //    var allEquipmentClasses = allEquipmentClassesGroup.GroupBy(equipmentClass => equipmentClass.Id)
        //                                .Select(s => s.FirstOrDefault())
        //                                .Select(s => new Detail_EquipmentClass() {
        //                                    EquipmentClassId = s.Id,
        //                                    EquipmentClassName = s.Name,
        //                                    Description = s.Description,
        //                                    IsReady = false
        //                                });

        //    var allExistEquipmentClassDashboards = from c in dbEntity.Dashboard.AsNoTracking()
        //                                               where c.CompanyID == companyId && c.DashboardType == "EquipmentClass"
        //                                               select c;
        //    var existingEquipmentClassId = from c in allExistEquipmentClassDashboards
        //                                   select c.EquipmentClassID;

        //    foreach (var equipmentClass in allEquipmentClasses)
        //    {
        //        if (existingEquipmentClassId.Contains(equipmentClass.EquipmentClassId))
        //        {
        //            equipmentClass.IsReady = true;
        //            var tmp = from c in allExistEquipmentClassDashboards
        //                      where c.EquipmentClassID == equipmentClass.EquipmentClassId
        //                      select c.Id;
        //            equipmentClass.DashboardId = tmp.FirstOrDefault();
        //        }
        //        resultList.Add(equipmentClass);
        //    }

        //    return resultList;
        //}

        //public List<Detail> getAllElementClassDashboardByCompnayId(int companyId)
        //{
        //    DBHelper._Dashboard dbhelp = new DBHelper._Dashboard();

        //    return dbhelp.GetAllByCompanyId(companyId).Select(s => new Detail()
        //    {
        //        Id = s.Id,
        //        CompanyId = s.CompanyID,
        //        DashboardId = s.DashboardID,
        //        DashboardName = (s.Dashboard == null ? "" : s.Dashboard.Name),
        //        EquipmentId = s.EquipmentID,
        //        EquipmentName = (s.Equipment == null ? "" : s.Equipment.Name),
        //        EquipmentClassId = s.EquipmentClassID,
        //        EquipmentClassName = (s.EquipmentClass == null ? "" : s.EquipmentClass.Name),
        //        DashboardType = s.DashboardType
        //    }).ToList<Detail>();
        //}

        public List<Format_DetailForFactory> GetAllFactoryByCompanyId(int companyId)
        {
            List<Format_DetailForFactory> returnDataList = new List<Format_DetailForFactory>();
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                var L2Enty = from c in dbEntity.Dashboard.AsNoTracking()
                             where c.CompanyID == companyId && c.DashboardType == "factory"
                             select c;

                List<Dashboard> dashboardList = L2Enty.ToList();
                foreach (var dashboard in dashboardList)
                {

                    returnDataList.Add(new Format_DetailForFactory
                    {
                        Id = dashboard.Id,
                        DashboardType = dashboard.DashboardType,
                        FactorydId = dashboard.FactoryID == null ? "" : dashboard.FactoryID.ToString(),
                        FactoryName = dashboard.Factory == null ? "" : dashboard.Factory.Name,
                        FactoryDescription = dashboard.Factory == null ? "" : dashboard.Factory.Description
                    });
                }

                return returnDataList;
            }
        }

        public List<Format_DetailForEquipmentClassBoard> GetAllEquipmentClassBoardByCompanyId(int companyId)
        {
            List<Format_DetailForEquipmentClassBoard> returnDataList = new List<Format_DetailForEquipmentClassBoard>();
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                var L2Enty = from c in dbEntity.Dashboard.AsNoTracking()
                             where c.CompanyID == companyId && c.DashboardType == "equipmentclass"
                             select c;

                List<Dashboard> dashboardList = L2Enty.ToList();
                foreach (var dashboard in dashboardList)
                {
                    returnDataList.Add(new Format_DetailForEquipmentClassBoard
                    {
                        Id = dashboard.Id,
                        DashboardType = dashboard.DashboardType,
                        EquipmentClassyId = dashboard.EquipmentClassID == null ? "" : dashboard.EquipmentClassID.ToString(),
                        EquipmentClassName = dashboard.EquipmentClass == null ? "" : dashboard.EquipmentClass.Name,
                        EquipmentClassDescription = dashboard.EquipmentClass == null ? "" : dashboard.EquipmentClass.Description
                    });
                }

                return returnDataList;
            }
        }

        public Format_Detail GetById(int id)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                Dashboard existingData = (from c in dbEntity.Dashboard.AsNoTracking()
                                          where c.Id == id
                                          select c).SingleOrDefault<Dashboard>();

                if (existingData == null)
                    throw new CDSException(10402);

                return new Format_Detail()
                {
                    Id = existingData.Id,
                    DashboardType = existingData.DashboardType,
                    FactoryId = existingData.FactoryID,
                    FactoryName = existingData.Factory == null ? "" : existingData.Factory.Name,
                    EquipmentClassId = existingData.EquipmentClassID,
                    EquipmentClassName = existingData.EquipmentClass == null ? "" : existingData.EquipmentClass.Name,
                    EquipmentId = existingData.EquipmentClassID,
                    EquipmentName = existingData.Equipment == null ? "" : existingData.Equipment.Name
                };
            }
        }

        public int Create(int companyId, Format_Create parseData)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                Dashboard newData = new Dashboard()
                {
                    CompanyID = companyId,
                    DashboardType = parseData.DashboardType,
                    FactoryID = parseData.FactoryId,
                    EquipmentClassID = parseData.EquipmentClassId,
                    EquipmentID = parseData.EquipmentId
                };
                dbEntity.Dashboard.Add(newData);
                dbEntity.SaveChanges();
                return newData.Id;
            }
        }
        
        public void DeleteById(int id)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                Dashboard existingData = dbEntity.Dashboard.Find(id);
                if (existingData == null)
                    throw new CDSException(10402);

                dbEntity.Entry(existingData).State = EntityState.Deleted;
                dbEntity.SaveChanges();
            }
        }
    }
}