using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDSShareLib.Helper
{
    public class AzureSQLHelper
    {
        public class ErrorMessageModel
        {
            public List<ErrorMessage> GetAllErrorMessage()
            {
                CDS20AzureSQL dbEntity = new CDS20AzureSQL();
                var L2Enty = from c in dbEntity.ErrorMessage.AsNoTracking()
                             select c;
                return L2Enty.ToList<ErrorMessage>();
            }            
        }

        public class CompanyModel
        {
            public Company GetById(int Id)
            {
                CDS20AzureSQL dbEntity = new CDS20AzureSQL();
                var L2Enty = from c in dbEntity.Company.AsNoTracking()
                             where c.Id == Id
                             select c;
                return L2Enty.FirstOrDefault();
            }

            public CompanyInSubscriptionPlan GetActiveSubscriptionPlanByCompanyId(int Id)
            {
                CDS20AzureSQL dbEntity = new CDS20AzureSQL();
                DateTime DTNow = DateTime.UtcNow;
                var L2Enty = from c in dbEntity.CompanyInSubscriptionPlan
                             where c.CompanyID == Id && DTNow >= c.StartDate && c.ExpiredDate >= DTNow
                             select c;
                return L2Enty.FirstOrDefault();
            }
        }

        public class EventRuleCatalogModel
        {
            public class DetailForRuleEngineModel
            {
                public int Id { get; set; }
                public int EventRuleCatalogId { get; set; }
                public int Ordering { get; set; }
                public int MessageElementId { get; set; }
                public string MessageElementFullName { get; set; }
                public string MessageElementDataType { get; set; }
                public string EqualOperation { get; set; }
                public string Value { get; set; }
                public string BitWiseOperation { get; set; }
            }

            public EventRuleCatalog GetById(int Id)
            {
                CDS20AzureSQL dbEntity = new CDS20AzureSQL();
                var L2Enty = from c in dbEntity.EventRuleCatalog.AsNoTracking()
                             where c.Id == Id
                             select c;
                return L2Enty.FirstOrDefault();
            }

            public List<Application> GetActionApplicationById(int Id)
            {
                CDS20AzureSQL dbEntity = new CDS20AzureSQL();
                var appList = from eventAction in dbEntity.EventInAction
                              join app in dbEntity.Application on eventAction.ApplicationId equals app.Id
                              where eventAction.EventRuleCatalogId == Id
                              select app;
                return appList.ToList<Application>();
            }
        }

        public class IoTDeviceModel
        {
            public IoTDevice GetById(int Id)
            {
                CDS20AzureSQL dbEntity = new CDS20AzureSQL();
                var L2Enty = from c in dbEntity.IoTDevice.AsNoTracking()
                             where c.Id == Id
                             select c;
                return L2Enty.FirstOrDefault();
            }
        }

        public class IoTHubModel
        {
            public IoTHub GetById(int Id)
            {
                CDS20AzureSQL dbEntity = new CDS20AzureSQL();
                var L2Enty = from c in dbEntity.IoTHub.AsNoTracking()
                             where c.Id == Id
                             select c;
                return L2Enty.FirstOrDefault();
            }
        }

        public class MessageCatalogModel
        {
            public List<MessageCatalog> GetAllMasterByCompanyId(int companyId)
            {
                CDS20AzureSQL dbEntity = new CDS20AzureSQL();
                var L2Enty = from c in dbEntity.MessageCatalog.AsNoTracking()
                             where c.CompanyID == companyId && c.ChildMessageFlag == false
                             select c;
                return L2Enty.ToList<MessageCatalog>();
            }

            public MessageCatalog GetById(int Id)
            {
                CDS20AzureSQL dbEntity = new CDS20AzureSQL();
                var L2Enty = from c in dbEntity.MessageCatalog.AsNoTracking()
                             where c.Id == Id
                             select c;
                return L2Enty.FirstOrDefault();
            }

            public List<MessageElement> GetElementsById(int Id)
            {
                CDS20AzureSQL dbEntity = new CDS20AzureSQL();
                var L2Enty = from element in dbEntity.MessageElement.AsNoTracking()
                             join catalog in dbEntity.MessageCatalog.AsNoTracking() on element.MessageCatalogID equals catalog.Id
                             where catalog.Id == Id && element.MessageCatalogID == Id
                             select element;
                return L2Enty.ToList<MessageElement>();
            }
        }

        public class OperationTaskModel
        {
            public OperationTask GetByid(int Id)
            {
                CDS20AzureSQL dbEntity = new CDS20AzureSQL();
                var L2Enty = from c in dbEntity.OperationTask.AsNoTracking()
                             where c.Id == Id && c.DeletedFlag == false
                             select c;
                return L2Enty.FirstOrDefault();
            }

            public void Update(OperationTask operationTask)
            {
                CDS20AzureSQL dbEntity = new CDS20AzureSQL();
                operationTask.UpdatedAt = DateTime.Parse(DateTime.UtcNow.ToString());
                dbEntity.OperationTask.Attach(operationTask);
                dbEntity.Entry(operationTask).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }

            public void UpdateTaskBySuccess(int Id)
            {
                try
                {
                    OperationTask operationTask = GetByid(Id);
                    if (operationTask == null)
                        return;
                    operationTask.CompletedAt = DateTime.UtcNow;
                    operationTask.TaskLog = operationTask.TaskLog + Environment.NewLine + DateTime.UtcNow + ": Done.";
                    operationTask.TaskStatus = "Completed";
                    Update(operationTask);
                }
                catch (Exception)
                {
                    throw;
                }
            }

            public void UpdateTaskByFail(int Id, string failLog)
            {
                try
                {
                    OperationTask operationTask = GetByid(Id);
                    if (operationTask == null)
                        return;
                    operationTask.TaskLog = operationTask.TaskLog + Environment.NewLine + DateTime.UtcNow + ": " + failLog;
                    operationTask.TaskStatus = "Fail";
                    Update(operationTask);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        public class SystemConfigurationModel
        {
            private static Dictionary<string, string> cdsConfig = new Dictionary<string, string>();
            private static void loadSystemConfig()
            {
                if (cdsConfig.Count == 0)
                {
                    CDS20AzureSQL dbEntity = new CDS20AzureSQL();
                    var L2Enty = from c in dbEntity.SystemConfiguration
                                 select c;
                    var cdsConfigList = L2Enty.ToList<SystemConfiguration>();
                    if (cdsConfigList != null)
                        foreach (var c in cdsConfigList)
                            cdsConfig.Add(c.Key, c.Value);
                }
            }
            public static string GetCDSConfigValueByKey(string key)
            {
                if (cdsConfig.ContainsKey(key) == true)
                {
                    return cdsConfig[key];
                }
                else
                {
                    loadSystemConfig();
                    if (cdsConfig.ContainsKey(key) == true)
                    {
                        return cdsConfig[key];
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }
    }
}
