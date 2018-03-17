using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using sfShareLib;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using CDSShareLib.Helper;

namespace sfAPIService.Models
{
    public class EquipmentModel
    {
        public class Format_Detail
        {
            public int Id { get; set; }
            public string EquipmentId { get; set; }
            public string Name { get; set; }
            public int EquipmentClassId { get; set; }
            public int FactoryId { get; set; }
            public int IoTDeviceId { get; set; }
            public string Latitude { get; set; }
            public string Longitude { get; set; }
            public int MaxIdleInSec { get; set; }
            public string PhotoURL { get; set; }
            public List<Format_MetaDataDetail> MetaData { get; set; }
        }
        public class Format_DetailForExternal
        {
            public int Id { get; set; }
            public string EquipmentId { get; set; }
            public string Name { get; set; }
            public string EquipmentClassName { get; set; }
            public string FactoryName { get; set; }
            public string IoTHubDeviceId { get; set; }
            public string IoTDeviceTypeName { get; set; }
            public string Latitude { get; set; }
            public string Longitude { get; set; }
            public int MaxIdleInSec { get; set; }
            public string PhotoURL { get; set; }
            public List<Format_MetaDataDetail> MetaData { get; set; }
        }
        public class Format_Create
        {
            [Required]
            public string EquipmentId { get; set; }
            [Required]
            public string Name { get; set; }
            [Required]
            public int EquipmentClassId { get; set; }
            [Required]
            public int FactoryId { get; set; }
            [Required]
            public int IoTDeviceId { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            [Required]
            public int MaxIdleInSec { get; set; }
            public List<Format_MetaData> MetaData { get; set; }
        }

        public class Format_Update
        {
            public string EquipmentId { get; set; }
            public string Name { get; set; }
            public int? EquipmentClassId { get; set; }
            public int? FactoryId { get; set; }
            public int? IoTDeviceId { get; set; }
            public double? Latitude { get; set; }
            public double? Longitude { get; set; }
            public int? MaxIdleInSec { get; set; }
            public List<Format_MetaData> MetaData { get; set; }
        }

        public class Format_MetaData
        {
            public int DefinationId { get; set; }
            public string ObjectValue { get; set; }
        }
        public class Format_MetaDataDetail
        {
            public int DefinationId { get; set; }
            public string DefinationName { get; set; }
            public string ObjectValue { get; set; }
        }

        //public List<Detail> GetAllEquipment()
        //{
        //    DBHelper._Equipment dbhelp_equipment = new DBHelper._Equipment();
        //    return dbhelp_equipment.GetAll().Select(s => new Detail()
        //    {
        //        Id = s.Id,
        //        EquipmentId = s.EquipmentId,
        //        Name = s.Name,
        //        CompanyId = s.CompanyID,
        //        EquipmentId = s.EquipmentId,
        //        EquipmentName = s.Equipment.Name,
        //        FactoryId = s.FactoryId,
        //        FactoryName = s.Factory.Name,
        //        IoTDeviceId = s.IoTDeviceID,
        //        IoTDeviceTypeName = s.IoTDevice.DeviceType.Name,
        //        Latitude = (s.Latitude == null) ? "" : s.Latitude.ToString(),
        //        Longitude = (s.Longitude == null) ? "" : s.Longitude.ToString(),
        //        MaxIdleInSec = s.MaxIdleInSec,
        //        PhotoUrl = s.PhotoURL
        //    }).ToList<Detail>();
        //}

        //public List<Detail> GetAllEquipmentByCompanyId(int companyId)
        //{
        //    DBHelper._Equipment dbhelp_equipment = new DBHelper._Equipment();
        //    List<Equipment> equipmentList = dbhelp_equipment.GetAllByCompanyId(companyId);

        //    return equipmentList.Select(s => new Detail()
        //    {
        //        Id = s.Id,
        //        EquipmentId = s.EquipmentId,
        //        Name = s.Name,
        //        CompanyId = s.CompanyID,
        //        EquipmentId = s.EquipmentId,
        //        EquipmentName = s.Equipment.Name,
        //        FactoryId = s.FactoryId,
        //        FactoryName = s.Factory.Name,
        //        IoTDeviceId = s.IoTDeviceID,
        //        IoTDeviceTypeName = s.IoTDevice.DeviceType.Name,
        //        Latitude = (s.Latitude == null) ? "" : s.Latitude.ToString(),
        //        Longitude = (s.Longitude == null) ? "" : s.Longitude.ToString(),
        //        MaxIdleInSec = s.MaxIdleInSec,
        //        PhotoUrl = s.PhotoURL
        //    }).ToList<Detail>();
        //}

        //public List<Detail_readonly> GetAllEquipmentByCompanyIdReadonly(int companyId)
        //{
        //    DBHelper._Equipment dbhelp_equipment = new DBHelper._Equipment();
        //    List<Equipment> equipmentList = dbhelp_equipment.GetAllByCompanyId(companyId);

        //    return equipmentList.Select(s => new Detail_readonly()
        //    {
        //        EquipmentId = s.EquipmentId,
        //        Name = s.Name,
        //        CompanyId = s.CompanyID,
        //        EquipmentName = s.Equipment.Name,
        //        FactoryName = s.Factory.Name,
        //        IoTDeviceId = s.IoTDeviceID,
        //        IoTDeviceTypeName = s.IoTDevice.DeviceType.Name,
        //        Latitude = (s.Latitude == null) ? "" : s.Latitude.ToString(),
        //        Longitude = (s.Longitude == null) ? "" : s.Longitude.ToString(),
        //        PhotoUrl = s.PhotoURL
        //    }).ToList<Detail_readonly>();
        //}

        //public List<Detail> GetAllEquipmentByFactoryId(int factoryId)
        //{
        //    DBHelper._Equipment dbhelp = new DBHelper._Equipment();

        //    return dbhelp.GetAllByFactoryId(factoryId).Select(s => new Detail()
        //    {
        //        Id = s.Id,
        //        EquipmentId = s.EquipmentId,
        //        Name = s.Name,
        //        CompanyId = s.CompanyID,
        //        EquipmentId = s.EquipmentId,
        //        EquipmentName = s.Equipment.Name,
        //        FactoryId = s.FactoryId,
        //        FactoryName = s.Factory.Name,
        //        IoTDeviceId = s.IoTDeviceID,
        //        IoTDeviceTypeName = s.IoTDevice.DeviceType.Name,
        //        Latitude = (s.Latitude == null) ? "" : s.Latitude.ToString(),
        //        Longitude = (s.Longitude == null) ? "" : s.Longitude.ToString(),
        //        MaxIdleInSec = s.MaxIdleInSec,
        //        PhotoUrl = s.PhotoURL
        //    }).ToList<Detail>();
        //}

        //public List<Detail_readonly> GetAllEquipmentByFactoryIdReadonly(int factoryId)
        //{     
        //    DBHelper._Equipment dbhelp_equipment = new DBHelper._Equipment();

        //    return dbhelp_equipment.GetAllByFactoryId(factoryId).Select(s => new Detail_readonly()
        //    {
        //        EquipmentId = s.EquipmentId,
        //        Name = s.Name,
        //        CompanyId = s.CompanyID,
        //        EquipmentName = s.Equipment.Name,
        //        FactoryName = s.Factory.Name,
        //        IoTDeviceId = s.IoTDeviceID,
        //        IoTDeviceTypeName = s.IoTDevice.DeviceType.Name,
        //        Latitude = (s.Latitude == null) ? "" : s.Latitude.ToString(),
        //        Longitude = (s.Longitude == null) ? "" : s.Longitude.ToString(),
        //        PhotoUrl = s.PhotoURL
        //    }).ToList<Detail_readonly>();
        //}

        //public Detail getEquipmentById(int id)
        //{
        //    DBHelper._Equipment dbhelp = new DBHelper._Equipment();
        //    Equipment equipment = dbhelp.GetByid(id);
        //    if (equipment == null)
        //        throw new CDSException(10501);

        //    return new Detail()
        //    {
        //        Id = equipment.Id,
        //        EquipmentId = equipment.EquipmentId,
        //        Name = equipment.Name,
        //        CompanyId = equipment.CompanyID,
        //        EquipmentId = equipment.EquipmentId,
        //        EquipmentName = equipment.Equipment.Name,
        //        FactoryId = equipment.FactoryId,
        //        FactoryName = equipment.Factory.Name,
        //        IoTDeviceId = equipment.IoTDeviceID,
        //        IoTDeviceTypeName = equipment.IoTDevice.DeviceType.Name,
        //        Latitude = (equipment.Latitude == null) ? "" : equipment.Latitude.ToString(),
        //        Longitude = (equipment.Longitude == null) ? "" : equipment.Longitude.ToString(),
        //        MaxIdleInSec = equipment.MaxIdleInSec,
        //        PhotoUrl = equipment.PhotoURL
        //    };
        //}

        //public Detail_readonly getEquipmentByIdReadonly(int equipmentId)
        //{
        //    DBHelper._Equipment dbhelp = new DBHelper._Equipment();
        //    Equipment equipment = dbhelp.GetByid(equipmentId);

        //    return new Detail_readonly()
        //    {
        //        EquipmentId = equipment.EquipmentId,
        //        Name = equipment.Name,
        //        CompanyId = equipment.CompanyID,
        //        EquipmentName = equipment.Equipment.Name,
        //        FactoryName = equipment.Factory.Name,
        //        IoTDeviceId = equipment.IoTDeviceID,
        //        IoTDeviceTypeName = equipment.IoTDevice.DeviceType.Name,
        //        Latitude = (equipment.Latitude == null) ? "" : equipment.Latitude.ToString(),
        //        Longitude = (equipment.Longitude == null) ? "" : equipment.Longitude.ToString(),
        //        PhotoUrl = equipment.PhotoURL
        //    };
        //}
        public List<Format_Detail> GetAllByCompanyId(int companyId)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                var L2Enty = from c in dbEntity.Equipment.AsNoTracking()
                             where c.CompanyID == companyId
                             select c;

                return L2Enty.Select(s => new Format_Detail
                {
                    Id = s.Id,
                    EquipmentId = s.EquipmentId,
                    Name = s.Name,
                    EquipmentClassId = s.EquipmentClassId,
                    FactoryId = s.FactoryId,
                    IoTDeviceId = s.IoTDeviceID,
                    Latitude = (s.Latitude == null) ? "" : s.Latitude.ToString(),
                    Longitude = (s.Longitude == null) ? "" : s.Longitude.ToString(),
                    MaxIdleInSec = s.MaxIdleInSec,
                    PhotoURL = s.PhotoURL
                }).ToList<Format_Detail>();
            }
        }
        
        public List<Format_Detail> GetAllByFactoryId(int factoryId)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                var L2Enty = from c in dbEntity.Equipment.AsNoTracking()
                             where c.FactoryId == factoryId
                             select c;

                return L2Enty.Select(s => new Format_Detail
                {
                    Id = s.Id,
                    EquipmentId = s.EquipmentId,
                    Name = s.Name,
                    EquipmentClassId = s.EquipmentClassId,
                    FactoryId = s.FactoryId,
                    IoTDeviceId = s.IoTDeviceID,
                    Latitude = (s.Latitude == null) ? "" : s.Latitude.ToString(),
                    Longitude = (s.Longitude == null) ? "" : s.Longitude.ToString(),
                    MaxIdleInSec = s.MaxIdleInSec,
                    PhotoURL = s.PhotoURL
                }).ToList<Format_Detail>();
            }
        }       

        public Format_Detail GetById(int id, bool metaData)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                Equipment existingData = (from c in dbEntity.Equipment.AsNoTracking()
                                          where c.Id == id
                                          select c).SingleOrDefault();

                if (existingData == null)
                    throw new CDSException(10501);

                Format_Detail returnData = new Format_Detail()
                {
                    Id = existingData.Id,
                    EquipmentId = existingData.EquipmentId,
                    Name = existingData.Name,
                    EquipmentClassId = existingData.EquipmentClassId,
                    FactoryId = existingData.FactoryId,
                    IoTDeviceId = existingData.IoTDeviceID,
                    Latitude = (existingData.Latitude == null) ? "" : existingData.Latitude.ToString(),
                    Longitude = (existingData.Longitude == null) ? "" : existingData.Longitude.ToString(),
                    MaxIdleInSec = existingData.MaxIdleInSec,
                    PhotoURL = existingData.PhotoURL
                };

                if (metaData)
                {
                    returnData.MetaData = (from c in dbEntity.MetaDataDefination
                                           where c.CompanyId == existingData.CompanyID && c.EntityType == Global.MetaDataEntityType.Equipment
                                           select c).Select(s => new Format_MetaDataDetail()
                                           {
                                               DefinationId = s.Id,
                                               DefinationName = s.ObjectName,
                                               ObjectValue = ""
                                           }).ToList();
                    if (returnData.MetaData.Count > 0)
                    {
                        List<int> definationIdList = returnData.MetaData.Select(s => s.DefinationId).ToList();
                        //<definationId, ObjectValue>
                        Dictionary<int, string> dictonary_metaDataValue = (from c in dbEntity.MetaDataValue.AsNoTracking()
                                                                           where c.ReferenceId == id && definationIdList.Contains(c.MetaDataDefinationId)
                                                                           select c).ToDictionary(s => s.MetaDataDefinationId, s => s.ObjectValue);
                        for (int i = 0; i < returnData.MetaData.Count; i++)
                        {
                            int definationId = returnData.MetaData[i].DefinationId;
                            returnData.MetaData[i].ObjectValue = dictonary_metaDataValue.ContainsKey(definationId) ? dictonary_metaDataValue[definationId] : "";
                        }
                    }
                }
                return returnData;
            }
        }
        
        public int Create(int companyId, Format_Create parseData)
        {
            try
            {
                using (CDStudioEntities dbEntity = new CDStudioEntities())
                {
                    using (var dbEntityTransaction = dbEntity.Database.BeginTransaction())
                    {
                        Equipment newData = new Equipment();
                        newData.EquipmentId = parseData.EquipmentId;
                        newData.Name = parseData.Name;
                        newData.EquipmentClassId = parseData.EquipmentClassId;
                        newData.CompanyID = companyId;
                        newData.FactoryId = parseData.FactoryId;
                        newData.IoTDeviceID = parseData.IoTDeviceId;
                        newData.Latitude = parseData.Latitude;
                        newData.Longitude = parseData.Latitude;
                        newData.MaxIdleInSec = parseData.MaxIdleInSec;
                        newData.PhotoURL = "";

                        dbEntity.Equipment.Add(newData);
                        dbEntity.SaveChanges();

                        /***** MetaData *****/
                        if (parseData.MetaData != null && parseData.MetaData.Count > 0)
                        {
                            List<MetaDataValue> metaDataValueList = new List<MetaDataValue>();
                            foreach (var data in parseData.MetaData)
                            {
                                metaDataValueList.Add(new MetaDataValue()
                                {
                                    MetaDataDefinationId = data.DefinationId,
                                    ObjectValue = data.ObjectValue ?? "",
                                    ReferenceId = newData.Id
                                });
                            }
                            dbEntity.MetaDataValue.AddRange(metaDataValueList);
                        }
                        dbEntity.SaveChanges();
                        dbEntityTransaction.Commit();

                        return newData.Id;
                    }
                }
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException.InnerException.Message.Contains("Cannot insert duplicate key"))
                    throw new CDSException(10503);
                else
                    throw ex;
            }
        }

        public void Update(int id, Format_Update parseData)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                using (var dbEntityTransaction = dbEntity.Database.BeginTransaction())
                {
                    var existingData = dbEntity.Equipment.Find(id);
                    if (existingData == null)
                        throw new CDSException(10501);

                    if (parseData.Name != null)
                        existingData.Name = parseData.Name;

                    if (parseData.EquipmentId != null)
                        existingData.EquipmentId = parseData.EquipmentId;

                    if (parseData.EquipmentClassId.HasValue)
                        existingData.EquipmentClassId = Convert.ToInt32(parseData.EquipmentClassId);

                    if (parseData.FactoryId.HasValue)
                        existingData.FactoryId = Convert.ToInt32(parseData.FactoryId);

                    if (parseData.IoTDeviceId.HasValue)
                        existingData.IoTDeviceID = Convert.ToInt32(parseData.IoTDeviceId);

                    if (parseData.Latitude.HasValue)
                        existingData.Latitude = Convert.ToDouble(parseData.Latitude);

                    if (parseData.Longitude.HasValue)
                        existingData.Longitude = Convert.ToDouble(parseData.Longitude);

                    if (parseData.MaxIdleInSec.HasValue)
                        existingData.MaxIdleInSec = Convert.ToInt32(parseData.MaxIdleInSec);
                    
                    dbEntity.SaveChanges();
                    
                    /***** MetaData *****/
                    if (parseData.MetaData != null && parseData.MetaData.Count > 0)
                    {
                        string query_deleteExistingMetaDataValue = String.Format(@"
                            Delete From {0}.[MetaDataValue] 
                            Where
                                [ReferenceId] = @ReferenceId And 
                                [MetaDataDefinationId] 
                                    In (
                                        Select Id From {0}.[MetaDataDefination] 
                                        Where 
                                            CompanyId = @CompanyId 
                                            And EntityType = @EntityType
                                    )", Global.DBSchemaName);
                        List<SqlParameter> deleteParameters = new List<SqlParameter>();
                        deleteParameters.Add(new SqlParameter("@ReferenceId", id));
                        deleteParameters.Add(new SqlParameter("@CompanyId", existingData.CompanyID));
                        deleteParameters.Add(new SqlParameter("@EntityType", Global.MetaDataEntityType.Equipment));
                        dbEntity.Database.ExecuteSqlCommand(query_deleteExistingMetaDataValue, deleteParameters.ToArray());

                        List<MetaDataValue> metaDataValueList = new List<MetaDataValue>();
                        foreach (var data in parseData.MetaData)
                        {
                            metaDataValueList.Add(new MetaDataValue()
                            {
                                MetaDataDefinationId = data.DefinationId,
                                ObjectValue = data.ObjectValue ?? "",
                                ReferenceId = id
                            });
                        }
                        dbEntity.MetaDataValue.AddRange(metaDataValueList);
                        dbEntity.SaveChanges();
                    }
                    dbEntityTransaction.Commit();
                }
            }
        }

        public void DeleteById(int id)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                Equipment existingData = dbEntity.Equipment.Find(id);
                if (existingData == null)
                    throw new CDSException(10501);

                dbEntity.Entry(existingData).State = EntityState.Deleted;
                dbEntity.SaveChanges();
            }
        }
        
        public void UpdatePhotoUrl(int id, string url)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                Equipment existingData = dbEntity.Equipment.Find(id);
                if (existingData == null)
                    throw new CDSException(10501);

                existingData.PhotoURL = url;
                dbEntity.SaveChanges();
            }
        }

        /*********************** For External API ***********************/
        public List<Format_DetailForExternal> External_GetAllByCompanyId(int companyId, bool metaData)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                var L2Enty = from c in dbEntity.Equipment.AsNoTracking()
                             where c.CompanyID == companyId
                             select c;

                List<Format_DetailForExternal> returnData = L2Enty.Select(s => new Format_DetailForExternal
                {
                    Id = s.Id,
                    EquipmentId = s.EquipmentId,
                    Name = s.Name,
                    EquipmentClassName = s.EquipmentClass == null ? "" : s.EquipmentClass.Name,
                    FactoryName = s.Factory == null ? "" : s.Factory.Name,
                    IoTHubDeviceId = s.IoTDevice == null ? "" : s.IoTDevice.IoTHubDeviceID,
                    Latitude = (s.Latitude == null) ? "" : s.Latitude.ToString(),
                    Longitude = (s.Longitude == null) ? "" : s.Longitude.ToString(),
                    MaxIdleInSec = s.MaxIdleInSec,
                    PhotoURL = s.PhotoURL,
                    IoTDeviceTypeName = s.IoTDevice == null || s.IoTDevice.DeviceType ==null ? "" : s.IoTDevice.DeviceType.Name,
                }).ToList<Format_DetailForExternal>();

                if (returnData != null && metaData)
                {
                    //id of Equipment, Format_MetaDataDetail
                    Dictionary<int, List<Format_MetaDataDetail>> dictionary_MetaDataDetail = new Dictionary<int, List<Format_MetaDataDetail>>();
                    List<Format_MetaDataDetail> standardMetaDataValueList = (from c in dbEntity.MetaDataDefination
                                                                             where c.CompanyId == companyId && c.EntityType == Global.MetaDataEntityType.Equipment
                                                                             select c).Select(s => new Format_MetaDataDetail()
                                                                             {
                                                                                 DefinationId = s.Id,
                                                                                 DefinationName = s.ObjectName,
                                                                                 ObjectValue = ""
                                                                             }).ToList();
                    if (standardMetaDataValueList != null && standardMetaDataValueList.Count > 0)
                    {
                        List<int> definationIdList = standardMetaDataValueList.Select(s => s.DefinationId).ToList();
                        //<if of equipment_definationId, ObjectValue> : <1_1, value>
                        Dictionary<string, string> dictonary_metaDataValue = (from c in dbEntity.MetaDataValue.AsNoTracking()
                                                                              where definationIdList.Contains(c.MetaDataDefinationId)
                                                                              select c).ToDictionary(s => s.ReferenceId + "_" + s.MetaDataDefinationId, s => s.ObjectValue);
                        for (int i = 0; i < returnData.Count; i++)
                        {
                            int ifOfEquipment = returnData[i].Id;
                            List<Format_MetaDataDetail> tempMetaDataValueList = new List<Format_MetaDataDetail>();
                            for (int j = 0; j < standardMetaDataValueList.Count; j++)
                            {
                                int definationId = standardMetaDataValueList[j].DefinationId;
                                string definationName = standardMetaDataValueList[j].DefinationName;
                                string dictionaryKey = ifOfEquipment + "_" + definationId;
                                string ObjectValue = dictonary_metaDataValue.ContainsKey(dictionaryKey) ? dictonary_metaDataValue[dictionaryKey] : "";

                                tempMetaDataValueList.Add(new Format_MetaDataDetail()
                                {
                                    DefinationId = definationId,
                                    DefinationName = definationName,
                                    ObjectValue = ObjectValue
                                });
                            }
                            returnData[i].MetaData = tempMetaDataValueList;
                        }
                    }
                }
                return returnData;
            }
        }
        public List<Format_DetailForExternal> External_GetAllByFactoryId(int factoryId, bool metaData, int companyId)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                var L2Enty = from c in dbEntity.Equipment.AsNoTracking()
                             where c.CompanyID == companyId && c.FactoryId == factoryId
                             select c;

                List<Format_DetailForExternal> returnData = L2Enty.Select(s => new Format_DetailForExternal
                {
                    Id = s.Id,
                    EquipmentId = s.EquipmentId,
                    Name = s.Name,
                    EquipmentClassName = s.EquipmentClass == null ? "" : s.EquipmentClass.Name,
                    FactoryName = s.Factory == null ? "" : s.Factory.Name,
                    IoTHubDeviceId = s.IoTDevice == null ? "" : s.IoTDevice.IoTHubDeviceID,
                    Latitude = (s.Latitude == null) ? "" : s.Latitude.ToString(),
                    Longitude = (s.Longitude == null) ? "" : s.Longitude.ToString(),
                    MaxIdleInSec = s.MaxIdleInSec,
                    PhotoURL = s.PhotoURL,
                    IoTDeviceTypeName = s.IoTDevice == null || s.IoTDevice.DeviceType == null ? "" : s.IoTDevice.DeviceType.Name,
                }).ToList<Format_DetailForExternal>();

                if (returnData != null && metaData)
                {
                    //id of Equipment, Format_MetaDataDetail
                    Dictionary<int, List<Format_MetaDataDetail>> dictionary_MetaDataDetail = new Dictionary<int, List<Format_MetaDataDetail>>();
                    List<Format_MetaDataDetail> standardMetaDataValueList = (from c in dbEntity.MetaDataDefination
                                                                             where c.CompanyId == companyId && c.EntityType == Global.MetaDataEntityType.Equipment
                                                                             select c).Select(s => new Format_MetaDataDetail()
                                                                             {
                                                                                 DefinationId = s.Id,
                                                                                 DefinationName = s.ObjectName,
                                                                                 ObjectValue = ""
                                                                             }).ToList();
                    if (standardMetaDataValueList != null && standardMetaDataValueList.Count > 0)
                    {
                        List<int> definationIdList = standardMetaDataValueList.Select(s => s.DefinationId).ToList();
                        //<if of equipment_definationId, ObjectValue> : <1_1, value>
                        Dictionary<string, string> dictonary_metaDataValue = (from c in dbEntity.MetaDataValue.AsNoTracking()
                                                                              where definationIdList.Contains(c.MetaDataDefinationId)
                                                                              select c).ToDictionary(s => s.ReferenceId + "_" + s.MetaDataDefinationId, s => s.ObjectValue);
                        for (int i = 0; i < returnData.Count; i++)
                        {
                            int ifOfEquipment = returnData[i].Id;
                            List<Format_MetaDataDetail> tempMetaDataValueList = new List<Format_MetaDataDetail>();
                            for (int j = 0; j < standardMetaDataValueList.Count; j++)
                            {
                                int definationId = standardMetaDataValueList[j].DefinationId;
                                string definationName = standardMetaDataValueList[j].DefinationName;
                                string dictionaryKey = ifOfEquipment + "_" + definationId;
                                string ObjectValue = dictonary_metaDataValue.ContainsKey(dictionaryKey) ? dictonary_metaDataValue[dictionaryKey] : "";

                                tempMetaDataValueList.Add(new Format_MetaDataDetail()
                                {
                                    DefinationId = definationId,
                                    DefinationName = definationName,
                                    ObjectValue = ObjectValue
                                });
                            }
                            returnData[i].MetaData = tempMetaDataValueList;
                        }
                    }
                }
                return returnData;
            }
        }

        public Format_DetailForExternal External_GetById(int id, bool metaData, int companyId)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                Equipment existingData = (from c in dbEntity.Equipment.AsNoTracking()
                                          where c.Id == id && c.CompanyID == companyId
                                          select c).SingleOrDefault();

                if (existingData == null)
                    throw new CDSException(10501);

                Format_DetailForExternal returnData = new Format_DetailForExternal()
                {
                    Id = existingData.Id,
                    EquipmentId = existingData.EquipmentId,
                    Name = existingData.Name,
                    EquipmentClassName = existingData.EquipmentClass == null ? "" : existingData.EquipmentClass.Name,
                    FactoryName = existingData.Factory == null ? "" : existingData.Factory.Name,
                    IoTHubDeviceId = existingData.IoTDevice == null ? "" : existingData.IoTDevice.IoTHubDeviceID,
                    Latitude = (existingData.Latitude == null) ? "" : existingData.Latitude.ToString(),
                    Longitude = (existingData.Longitude == null) ? "" : existingData.Longitude.ToString(),
                    MaxIdleInSec = existingData.MaxIdleInSec,
                    PhotoURL = existingData.PhotoURL,
                    IoTDeviceTypeName = existingData.IoTDevice == null || existingData.IoTDevice.DeviceType == null ? "" : existingData.IoTDevice.DeviceType.Name,
                };

                if (metaData)
                {
                    returnData.MetaData = (from c in dbEntity.MetaDataDefination
                                           where c.CompanyId == companyId && c.EntityType == Global.MetaDataEntityType.Equipment
                                           select c).Select(s => new Format_MetaDataDetail()
                                           {
                                               DefinationId = s.Id,
                                               DefinationName = s.ObjectName,
                                               ObjectValue = ""
                                           }).ToList();
                    if (returnData.MetaData.Count > 0)
                    {
                        List<int> definationIdList = returnData.MetaData.Select(s => s.DefinationId).ToList();
                        //<definationId, ObjectValue>
                        Dictionary<int, string> dictonary_metaDataValue = (from c in dbEntity.MetaDataValue.AsNoTracking()
                                                                           where c.ReferenceId == id && definationIdList.Contains(c.MetaDataDefinationId)
                                                                           select c).ToDictionary(s => s.MetaDataDefinationId, s => s.ObjectValue);
                        for (int i = 0; i < returnData.MetaData.Count; i++)
                        {
                            int definationId = returnData.MetaData[i].DefinationId;
                            returnData.MetaData[i].ObjectValue = dictonary_metaDataValue.ContainsKey(definationId) ? dictonary_metaDataValue[definationId] : "";
                        }
                    }
                }
                return returnData;
            }
        }
    }
}
 