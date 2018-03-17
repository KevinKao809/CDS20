using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;

using System.ComponentModel.DataAnnotations;
using sfShareLib;
using CDSShareLib.Helper;

namespace sfAPIService.Models
{
    public class OperationTaskModel
    {
        public class Format_Detail
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string CompanyName { get; set; }
            public string TaskStatus { get; set; }
            public string CompletedAt { get; set; }
            public int RetryCounter { get; set; }
            public string Entity { get; set; }
            public string EntityId { get; set; }
            public JObject TaskContent { get; set; }
            public string TaskLog { get; set; }
            public DateTime CreatedAt { get; set; }
        }

        public class Format_Create
        {
            [Required]
            [MaxLength(50)]
            public string Name { get; set; }
            [MaxLength(50)]
            [Required]
            public string Entity { get; set; }
            [Required]
            public string EntityId { get; set; }
            public string TaskContent { get; set; }
        }

        public class Format_Update
        {
            [MaxLength(20)]
            public string TaskStatus { get; set; }
            public DateTime? CompletedAt { get; set; }
            public int? RetryCounter { get; set; }
            public string TaskLog { get; set; }
        }
        
        public List<Format_Detail> GetAll(string taskStatus, int hours)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                hours = -(Math.Abs(hours));
                DateTime searchDatetime = DateTime.UtcNow.AddHours(hours);
                List<OperationTask> operationTaskList = new List<OperationTask>();                

                switch (taskStatus)
                {
                    case "All":
                        {
                            operationTaskList = (from c in dbEntity.OperationTask.AsNoTracking()
                                                 where c.CreatedAt > searchDatetime && c.DeletedFlag == false
                                                 orderby c.CreatedAt descending
                                                 select c).ToList<OperationTask>();
                        }
                        break;
                    default:
                        {
                            operationTaskList = (from c in dbEntity.OperationTask.AsNoTracking()
                                                 where c.CreatedAt > searchDatetime && c.DeletedFlag == false && c.TaskStatus == taskStatus
                                                 orderby c.CreatedAt descending
                                                 select c).ToList<OperationTask>();
                        }
                        break;
                }

                List<Format_Detail> returnDataList = new List<Format_Detail>();
                foreach (var operationTask in operationTaskList)
                {
                    returnDataList.Add(new Format_Detail()
                    {
                        Id = operationTask.Id,
                        Name = operationTask.Name,
                        CompanyName = operationTask.Company.Name,
                        TaskStatus = operationTask.TaskStatus,
                        CompletedAt = operationTask.CompletedAt.ToString() ?? "",
                        RetryCounter = operationTask.RetryCounter,
                        Entity = operationTask.Entity,
                        EntityId = operationTask.EntityId,
                        TaskContent = JObject.Parse(operationTask.TaskContent),
                        TaskLog = operationTask.TaskLog,
                        CreatedAt = operationTask.CreatedAt
                    });
                }

                return returnDataList;
            }
        }

        public List<Format_Detail> GetAllByCompanyId(int companyId, string taskStatus, int hours)
        {     
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                hours = -(Math.Abs(hours));
                DateTime searchDatetime = DateTime.UtcNow.AddHours(hours);
                List<OperationTask> operationTaskList = new List<OperationTask>();

                switch (taskStatus)
                {
                    case "All":
                        {
                            operationTaskList = (from c in dbEntity.OperationTask.AsNoTracking()
                                                 where c.CreatedAt > searchDatetime && c.DeletedFlag == false && c.CompanyId == companyId
                                                 orderby c.CreatedAt descending
                                                 select c).ToList<OperationTask>();
                        }
                        break;
                    default:
                        {
                            operationTaskList = (from c in dbEntity.OperationTask.AsNoTracking()
                                                 where c.CreatedAt > searchDatetime && c.DeletedFlag == false && c.CompanyId == companyId && c.TaskStatus == taskStatus
                                                 orderby c.CreatedAt descending
                                                 select c).ToList<OperationTask>();
                        }
                        break;
                }

                List<Format_Detail> returnDataList = new List<Format_Detail>();
                foreach (var operationTask in operationTaskList)
                {
                    returnDataList.Add(new Format_Detail()
                    {
                        Id = operationTask.Id,
                        Name = operationTask.Name,
                        CompanyName = operationTask.Company.Name,
                        TaskStatus = operationTask.TaskStatus,
                        CompletedAt = operationTask.CompletedAt.ToString() ?? "",
                        RetryCounter = operationTask.RetryCounter,
                        Entity = operationTask.Entity,
                        EntityId = operationTask.EntityId,
                        TaskContent = JObject.Parse(operationTask.TaskContent),
                        TaskLog = operationTask.TaskLog,
                        CreatedAt = operationTask.CreatedAt
                    });
                }

                return returnDataList;
            }
        }

        public Format_Detail GetById(int id)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                OperationTask existingData = (from c in dbEntity.OperationTask.AsNoTracking()
                                              where c.Id == id && c.DeletedFlag == false
                                              select c).SingleOrDefault<OperationTask>();
                if (existingData == null)
                    throw new CDSException(10701);

                return new Format_Detail()
                {
                    Id = existingData.Id,
                    Name = existingData.Name,
                    TaskStatus = existingData.TaskStatus,
                    CompletedAt = existingData.CompletedAt.ToString() ?? "",
                    RetryCounter = existingData.RetryCounter,
                    Entity = existingData.Entity,
                    EntityId = existingData.EntityId,
                    TaskContent = JObject.Parse(existingData.TaskContent),
                    TaskLog = existingData.TaskLog,
                    CreatedAt = existingData.CreatedAt
                };
            }
        }

        public int Create(int companyId, Format_Create parseData)
        {
            try
            {
                JObject.Parse(parseData.TaskContent);
            }
            catch
            {
                throw new CDSException(11501);
            }

            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                OperationTask newData = new OperationTask();
                newData.CompanyId = companyId;
                newData.Name = parseData.Name;
                newData.TaskStatus = "Submit";
                newData.Entity = parseData.Entity;
                newData.EntityId = parseData.EntityId;
                newData.TaskContent = parseData.TaskContent ?? "{}";

                newData.RetryCounter = 0;
                newData.CreatedAt = DateTime.UtcNow;
                newData.DeletedFlag = false;
                dbEntity.OperationTask.Add(newData);
                dbEntity.SaveChanges();
                return newData.Id;
            }
        }

        public void Update(int id, Format_Update parseData)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                var existingData = dbEntity.OperationTask.Find(id);
                if (existingData == null)
                    throw new CDSException(10701);

                if (parseData.CompletedAt.HasValue)
                    existingData.CompletedAt = (DateTime)parseData.CompletedAt;

                if (parseData.RetryCounter.HasValue)
                    existingData.RetryCounter = (int)parseData.RetryCounter;

                if (parseData.TaskStatus != null)
                    existingData.TaskStatus = parseData.TaskStatus;

                if (parseData.TaskLog != null)
                    existingData.TaskLog = parseData.TaskLog;

                existingData.UpdatedAt = DateTime.UtcNow;
                dbEntity.SaveChanges();
            }
        }

        public void DeleteById(int id)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                OperationTask existingData = dbEntity.OperationTask.Find(id);
                if (existingData == null)
                    throw new CDSException(10701);

                existingData.DeletedFlag = true;
                existingData.UpdatedAt = DateTime.UtcNow;
                dbEntity.SaveChanges();
            }
        }
    }
}