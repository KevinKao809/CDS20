using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sfShareLib
{
    public class DBHelper
    {
        /*
        public class Common
        {
            public List<ErrorMessage> GetAllErrorMessage()
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.ErrorMessage.AsNoTracking()
                             select c;
                return L2Enty.ToList<ErrorMessage>();
            }

            private static Dictionary<string, string> cdsConfig = new Dictionary<string, string>();
            private static void loadSystemConfig()
            {
                if (cdsConfig.Count == 0)
                {
                    CDStudioEntities dbEntity = new CDStudioEntities();
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
        */

        public class APIService
        {
            public List<SubscriptionPlan> GetAllSubscriptionPlan()
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.SubscriptionPlan.AsNoTracking()
                             select c;
                return L2Enty.ToList<SubscriptionPlan>();
            }

            public List<CompanyInSubscriptionPlan> GetSubscriptionPlanByCompanyId(int companyId)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.CompanyInSubscriptionPlan.AsNoTracking()
                             where c.CompanyID == companyId
                             select c;
                return L2Enty.ToList<CompanyInSubscriptionPlan>();
            }

            public List<CompanyInSubscriptionPlan> GetValidSubscriptionPlanByCompanyId(int companyId)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.CompanyInSubscriptionPlan.AsNoTracking()
                             where c.CompanyID == companyId && c.ExpiredDate >= DateTime.UtcNow
                             select c;
                return L2Enty.ToList<CompanyInSubscriptionPlan>();
            }

            public SubscriptionPlan GetSubscriptionPlanById(int id)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.SubscriptionPlan.AsNoTracking()
                             where c.Id == id
                             select c;
                return L2Enty.FirstOrDefault();
            }
            public int AddSubscriptionPlan(SubscriptionPlan subscriptionPlan)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                dbEntity.SubscriptionPlan.Add(subscriptionPlan);
                dbEntity.SaveChanges();
                return subscriptionPlan.Id;
            }

            public void UpdateSubscriptionPlan(SubscriptionPlan subscriptionPlan)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                dbEntity.SubscriptionPlan.Attach(subscriptionPlan);
                dbEntity.Entry(subscriptionPlan).State = System.Data.Entity.EntityState.Modified;
                dbEntity.SaveChanges();
            }

            public void DeleteSubscriptionPlan(SubscriptionPlan subscriptionPlan)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                dbEntity.SubscriptionPlan.Attach(subscriptionPlan);
                dbEntity.Entry(subscriptionPlan).State = System.Data.Entity.EntityState.Modified;
                dbEntity.SaveChanges();
            }

            public List<SystemConfiguration> GetAllSystemConfiguration()
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                try
                {
                    var L2Enty = from c in dbEntity.SystemConfiguration
                                 select c;
                    return L2Enty.ToList<SystemConfiguration>();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                return null;
            }

            public SystemConfiguration GetSystemConfigurationById(int id)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.SystemConfiguration.AsNoTracking()
                             where c.Id == id
                             select c;
                return L2Enty.FirstOrDefault();
            }

            public int AddSystemConfiguration(SystemConfiguration systemConfiguration)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                dbEntity.SystemConfiguration.Add(systemConfiguration);
                dbEntity.SaveChanges();
                return systemConfiguration.Id;
            }

            public void UpdateSystemConfiguration(SystemConfiguration systemConfiguration)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                dbEntity.SystemConfiguration.Attach(systemConfiguration);
                dbEntity.Entry(systemConfiguration).State = System.Data.Entity.EntityState.Modified;
                dbEntity.SaveChanges();
            }

            public void DeleteSystemConfiguration(SystemConfiguration systemConfiguration)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                dbEntity.SystemConfiguration.Attach(systemConfiguration);
                dbEntity.Entry(systemConfiguration).State = System.Data.Entity.EntityState.Deleted;
                dbEntity.SaveChanges();
            }
        }
        
        public class _Company
        {
            public List<Company> GetAll()
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.Company.AsNoTracking()
                             where c.DeletedFlag == false
                             select c;
                return L2Enty.ToList<Company>();
            }

            public List<Company> GetAllBySuperAdmin()
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.Company.AsNoTracking()
                             select c;
                return L2Enty.ToList<Company>();
            }

            public Company GetByid(int id)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.Company.AsNoTracking()
                             where c.Id == id
                             select c;
                return L2Enty.FirstOrDefault();
            }

            public int Add(Company company)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                company.DeletedFlag = false;
                company.CreatedAt = DateTime.Parse(DateTime.UtcNow.ToString());
                dbEntity.Company.Add(company);
                dbEntity.SaveChanges();
                return company.Id;
            }

            public void Update(Company company)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                company.UpdatedAt = DateTime.Parse(DateTime.UtcNow.ToString());
                dbEntity.Company.Attach(company);
                dbEntity.Entry(company).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }

            public void Delete(Company company)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                company.UpdatedAt = DateTime.Parse(DateTime.UtcNow.ToString());
                company.DeletedFlag = true;
                dbEntity.Company.Attach(company);
                dbEntity.Entry(company).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }

        }

        public class _Employee
        {
            public List<Employee> GetAll()
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.Employee.AsNoTracking()
                             select c;
                return L2Enty.ToList<Employee>();
            }

            public List<Employee> GetAllBySuperAdmin()
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.Employee.AsNoTracking()
                             select c;
                return L2Enty.ToList<Employee>();
            }

            public List<Employee> GetAllByCompanyId(int companyId)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.Employee.AsNoTracking()
                             where c.CompanyId == companyId
                             select c;
                return L2Enty.ToList<Employee>();
            }

            public Employee GetByid(int id)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.Employee.AsNoTracking()
                             where c.Id == id
                             select c;
                return L2Enty.FirstOrDefault();
            }

            public int Add(Employee employee)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();                
                dbEntity.Employee.Add(employee);
                dbEntity.SaveChanges();
                return employee.Id;
            }

            public void Update(Employee employee)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                dbEntity.Employee.Attach(employee);
                dbEntity.Entry(employee).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }

            public void Delete(Employee employee)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                dbEntity.Employee.Attach(employee);
                dbEntity.Entry(employee).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }

        }

        public class _Factory
        {
            public List<Factory> GetAllByCompanyId(int companyId)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.Factory.AsNoTracking()
                             where c.CompanyId == companyId 
                             select c;
                return L2Enty.ToList<Factory>();
            }

            public Factory GetByid(int id)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.Factory.AsNoTracking()
                             where c.Id == id
                             select c;
                return L2Enty.FirstOrDefault();
            }

            public Factory GetByid(int id, int companyId)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.Factory.AsNoTracking()
                             where c.Id == id && c.CompanyId == companyId
                             select c;
                return L2Enty.FirstOrDefault();
            }

            public int Add(Factory factory)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                dbEntity.Factory.Add(factory);
                dbEntity.SaveChanges();
                return factory.Id;
            }

            public void Update(Factory factory)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                dbEntity.Factory.Attach(factory);
                dbEntity.Entry(factory).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }

            public void Delete(Factory factory)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                dbEntity.Factory.Attach(factory);
                dbEntity.Entry(factory).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }

        }

        public class _EmployeeInRole
        {
            public List<EmployeeInRole> GetAllByUserRoleId(int userRoleId)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.EmployeeInRole.AsNoTracking()
                             where c.UserRoleID == userRoleId
                             select c;
                return L2Enty.ToList<EmployeeInRole>();
            }

            public List<EmployeeInRole> GetAllByEmployeeId(int employeeId)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.EmployeeInRole.AsNoTracking()
                             where c.EmployeeID == employeeId
                             select c;
                return L2Enty.ToList<EmployeeInRole>();
            }

            public EmployeeInRole GetByid(int id)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.EmployeeInRole.AsNoTracking()
                             where c.Id == id 
                             select c;
                return L2Enty.FirstOrDefault();
            }

            public int Add(EmployeeInRole employeeInRole)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                dbEntity.EmployeeInRole.Add(employeeInRole);
                dbEntity.SaveChanges();
                return employeeInRole.Id;
            }

            public void AddManyRows(List<EmployeeInRole> employeeInRoleList)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                foreach (EmployeeInRole employeeInRole in employeeInRoleList)
                {
                    dbEntity.EmployeeInRole.Add(employeeInRole);
                }
                dbEntity.SaveChanges();
            }

            public void Update(EmployeeInRole employeeInRole)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                dbEntity.EmployeeInRole.Attach(employeeInRole);
                dbEntity.Entry(employeeInRole).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }
            public void UpdateManyRows(List<EmployeeInRole> employeeInRoleList)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                foreach (EmployeeInRole employeeInRole in employeeInRoleList)
                {
                    dbEntity.EmployeeInRole.Attach(employeeInRole);
                    dbEntity.Entry(employeeInRole).State = System.Data.Entity.EntityState.Modified;
                }
                dbEntity.SaveChanges();
            }

            public void Delete(EmployeeInRole employeeInRole)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                dbEntity.EmployeeInRole.Attach(employeeInRole);
                dbEntity.Entry(employeeInRole).State = System.Data.Entity.EntityState.Deleted;

                dbEntity.SaveChanges();
            }

            public void Delete(List<EmployeeInRole> employeeInRoleList)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                foreach (EmployeeInRole employeeInRole in employeeInRoleList)
                {
                    dbEntity.EmployeeInRole.Attach(employeeInRole);
                    dbEntity.Entry(employeeInRole).State = System.Data.Entity.EntityState.Deleted;
                }

                dbEntity.SaveChanges();
            }

        }

        public class _IoTDevice
        {
            public List<IoTDevice> GetAllByIoTHubID(int iotHubId)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.IoTDevice.AsNoTracking()
                             where c.IoTHubID == iotHubId
                             select c;
                return L2Enty.ToList<IoTDevice>();
            }

            public List<IoTDevice> GetAllByFactory(int factoryId)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.IoTDevice.AsNoTracking()
                             where c.FactoryID == factoryId
                             select c;
                return L2Enty.ToList<IoTDevice>();
            }

            public List<IoTDevice> GetAllByCompany(int companyId)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.IoTDevice.AsNoTracking()
                             where c.CompanyID == companyId
                             select c;
                return L2Enty.ToList<IoTDevice>();
            }

            public IoTDevice GetByid(string iotHubDeviceId)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.IoTDevice.AsNoTracking()
                             where c.IoTHubDeviceID == iotHubDeviceId
                             select c;
                return L2Enty.FirstOrDefault();
            }

            public IoTDevice GetByid(int Id)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.IoTDevice.AsNoTracking()
                             where c.Id == Id
                             select c;
                return L2Enty.FirstOrDefault();
            }

            public int GetCompanyId(string iotHubDeviceId)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var companyId = from c in dbEntity.IoTDevice.AsNoTracking()
                                where c.IoTHubDeviceID == iotHubDeviceId
                                select c.CompanyID;

                return (int)companyId.FirstOrDefault();
            }
            public int Add(IoTDevice iotDevice)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                dbEntity.IoTDevice.Add(iotDevice);
                dbEntity.SaveChanges();
                return iotDevice.Id;
            }

            public void Update(IoTDevice iotDevice)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                dbEntity.IoTDevice.Attach(iotDevice);
                dbEntity.Entry(iotDevice).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }

            public void UpdateDeviceConfigurationStatusAndProperty(int id, int status, string desiredProperty = null, string reportedProeperty = null)
            {
                //CDStudioEntities dbEntity = new CDStudioEntities();
                //IoTDevice iotDevice = dbEntity.IoTDevice.Find(id);
                //iotDevice.DeviceConfigurationStatus = status;

                //if (!string.IsNullOrEmpty(desiredProperty))
                //    iotDevice.DeviceTwinsDesired = desiredProperty;

                //if (!string.IsNullOrEmpty(reportedProeperty))
                //    iotDevice.DeviceTwinsReported = reportedProeperty;

                //dbEntity.Entry(iotDevice).State = System.Data.Entity.EntityState.Modified;

                //dbEntity.SaveChanges();
            }

            public void Delete(IoTDevice iotDevice)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                dbEntity.IoTDevice.Attach(iotDevice);
                dbEntity.Entry(iotDevice).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }

        }

        public class _IoTDeviceConfiguration
        {
            public List<IoTDeviceSystemConfiguration> GetAll()
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.IoTDeviceSystemConfiguration.AsNoTracking()
                             select c;
                return L2Enty.ToList<IoTDeviceSystemConfiguration>();
            }

            public IoTDeviceSystemConfiguration GetByid(int id)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.IoTDeviceSystemConfiguration.AsNoTracking()
                             where c.Id == id
                             select c;
                return L2Enty.FirstOrDefault();
            }

            public int Add(IoTDeviceSystemConfiguration ioTDeviceSystemConfiguration)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                dbEntity.IoTDeviceSystemConfiguration.Add(ioTDeviceSystemConfiguration);
                dbEntity.SaveChanges();
                return ioTDeviceSystemConfiguration.Id;
            }

            public void Update(IoTDeviceSystemConfiguration ioTDeviceSystemConfiguration)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                dbEntity.IoTDeviceSystemConfiguration.Attach(ioTDeviceSystemConfiguration);
                dbEntity.Entry(ioTDeviceSystemConfiguration).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }

            public void Delete(IoTDeviceSystemConfiguration ioTDeviceSystemConfiguration)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                dbEntity.IoTDeviceSystemConfiguration.Attach(ioTDeviceSystemConfiguration);
                dbEntity.Entry(ioTDeviceSystemConfiguration).State = System.Data.Entity.EntityState.Deleted;

                dbEntity.SaveChanges();
            }

        }

        public class _IoTDeviceCustomizedConfiguration
        {
            public List<IoTDeviceCustomizedConfiguration> GetAllByCompanyId(int companyId)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.IoTDeviceCustomizedConfiguration.AsNoTracking()
                             where c.CompanyId == companyId
                             select c;
                return L2Enty.ToList<IoTDeviceCustomizedConfiguration>();
            }

            public IoTDeviceCustomizedConfiguration GetByid(int id)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.IoTDeviceCustomizedConfiguration.AsNoTracking()
                             where c.Id == id
                             select c;
                return L2Enty.FirstOrDefault();
            }

            public int Add(IoTDeviceCustomizedConfiguration customizedConfig)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                dbEntity.IoTDeviceCustomizedConfiguration.Add(customizedConfig);
                dbEntity.SaveChanges();
                return customizedConfig.Id;
            }

            public void Update(IoTDeviceCustomizedConfiguration customizedConfig)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                dbEntity.IoTDeviceCustomizedConfiguration.Attach(customizedConfig);
                dbEntity.Entry(customizedConfig).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }

            public void Delete(IoTDeviceCustomizedConfiguration customizedConfig)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                dbEntity.IoTDeviceCustomizedConfiguration.Attach(customizedConfig);
                dbEntity.Entry(customizedConfig).State = System.Data.Entity.EntityState.Deleted;

                dbEntity.SaveChanges();
            }
        }

        public class _EquipmentClassMessageCatalog
        {
            public List<EquipmentClassMessageCatalog> GetAllByEquipmentClassId(int equipmentClassId)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.EquipmentClassMessageCatalog.AsNoTracking()
                             where c.EquipmentClassID == equipmentClassId
                             select c;
                return L2Enty.ToList<EquipmentClassMessageCatalog>();
            }

            public EquipmentClassMessageCatalog GetById(int id)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.EquipmentClassMessageCatalog.AsNoTracking()
                             where c.Id == id
                             select c;
                return L2Enty.FirstOrDefault<EquipmentClassMessageCatalog>();
            }

            public void Add(List<EquipmentClassMessageCatalog> equipmentClassMessageCatalogList)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                foreach (EquipmentClassMessageCatalog equipmentClassMessageCatalog in equipmentClassMessageCatalogList)
                {
                    dbEntity.EquipmentClassMessageCatalog.Add(equipmentClassMessageCatalog);
                }
                dbEntity.SaveChanges();
            }

            public void Delete(List<EquipmentClassMessageCatalog> equipmentClassMessageCatalogList)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                foreach (EquipmentClassMessageCatalog equipmentClassMessageCatalog in equipmentClassMessageCatalogList)
                {
                    dbEntity.EquipmentClassMessageCatalog.Attach(equipmentClassMessageCatalog);
                    dbEntity.Entry(equipmentClassMessageCatalog).State = System.Data.Entity.EntityState.Deleted;
                }

                dbEntity.SaveChanges();
            }

        }

        public class _DeviceCertificate
        {
            public List<DeviceCertificate> GetAllByCompanyId(int companyId)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.DeviceCertificate.AsNoTracking()
                             where c.CompanyID == companyId
                             select c;
                return L2Enty.ToList<DeviceCertificate>();
            }

            public DeviceCertificate GetByid(int id)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.DeviceCertificate.AsNoTracking()
                             where c.Id == id
                             select c;
                return L2Enty.FirstOrDefault();
            }

            public int Add(DeviceCertificate deviceCertificate)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                dbEntity.DeviceCertificate.Add(deviceCertificate);
                dbEntity.SaveChanges();
                return deviceCertificate.Id;
            }

            public void Update(DeviceCertificate deviceCertificate)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                dbEntity.DeviceCertificate.Attach(deviceCertificate);
                dbEntity.Entry(deviceCertificate).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }

            public void Delete(DeviceCertificate deviceCertificate)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                dbEntity.DeviceCertificate.Attach(deviceCertificate);
                dbEntity.Entry(deviceCertificate).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }

        }

        public class _DeviceType
        {
            public List<DeviceType> GetAll()
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.DeviceType.AsNoTracking()
                             select c;
                return L2Enty.ToList<DeviceType>();
            }

            public List<DeviceType> GetAllBySuperAdmin()
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.DeviceType.AsNoTracking()
                             select c;
                return L2Enty.ToList<DeviceType>();
            }

            public DeviceType GetByid(int id)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.DeviceType.AsNoTracking()
                             where c.Id == id
                             select c;
                return L2Enty.FirstOrDefault();
            }

            public int Add(DeviceType deviceType)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                dbEntity.DeviceType.Add(deviceType);
                dbEntity.SaveChanges();
                return deviceType.Id;
            }

            public void Update(DeviceType deviceType)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                dbEntity.DeviceType.Attach(deviceType);
                dbEntity.Entry(deviceType).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }

            public void Delete(DeviceType deviceType)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                dbEntity.DeviceType.Attach(deviceType);
                dbEntity.Entry(deviceType).State = System.Data.Entity.EntityState.Deleted;

                dbEntity.SaveChanges();
            }

        }

        public class _IoTHub
        {
            public List<IoTHub> GetAll()
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.IoTHub.AsNoTracking()
                             select c;
                return L2Enty.ToList<IoTHub>();
            }
            public List<IoTHub> GetAllByCompanyId(int companyId)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.IoTHub.AsNoTracking()
                             where c.CompanyID == companyId 
                             select c;
                return L2Enty.ToList<IoTHub>();
            }

            public IoTHub GetByid(int id)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.IoTHub.AsNoTracking()
                             where c.Id == id
                             select c;
                return L2Enty.FirstOrDefault();
            }

            public int Add(IoTHub iotHub)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                dbEntity.IoTHub.Add(iotHub);
                dbEntity.SaveChanges();
                return iotHub.Id;
            }

            public void Update(IoTHub iotHub)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                dbEntity.IoTHub.Attach(iotHub);
                dbEntity.Entry(iotHub).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }

            public void Delete(IoTHub iotHub)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                dbEntity.IoTHub.Attach(iotHub);
                dbEntity.Entry(iotHub).State = System.Data.Entity.EntityState.Deleted;

                dbEntity.SaveChanges();
            }

        }

        public class _Equipment
        {
            public List<Equipment> GetAll()
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.Equipment.AsNoTracking()
                             select c;
                return L2Enty.ToList<Equipment>();
            }

            public List<Equipment> GetAllByCompanyId(int companyId)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.Equipment.AsNoTracking()
                             where c.CompanyID == companyId
                             select c;
                return L2Enty.ToList<Equipment>();
            }

            public List<Equipment> GetAllByFactoryId(int factoryId)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.Equipment.AsNoTracking()
                             where c.FactoryId == factoryId
                             select c;
                return L2Enty.ToList<Equipment>();
            }

            public List<Equipment> GetAllByIoTHubDeviceId(int iotHubDeviceId)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.Equipment.AsNoTracking()
                             select c;
                return L2Enty.ToList<Equipment>();
            }

            public Equipment GetByid(int id)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.Equipment.AsNoTracking()
                             where c.Id == id
                             select c;
                return L2Enty.FirstOrDefault();
            }

            public Equipment GetByid(string equipmentId)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.Equipment.AsNoTracking()
                             where c.EquipmentId == equipmentId
                             select c;
                return L2Enty.FirstOrDefault();
            }

            public int Add(Equipment equipment)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                dbEntity.Equipment.Add(equipment);
                dbEntity.SaveChanges();
                return equipment.Id;
            }

            public void Update(Equipment equipment)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                dbEntity.Equipment.Attach(equipment);
                dbEntity.Entry(equipment).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }

            public void Delete(Equipment equipment)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                dbEntity.Equipment.Attach(equipment);
                dbEntity.Entry(equipment).State = System.Data.Entity.EntityState.Deleted;

                dbEntity.SaveChanges();
            }

            public int GetCompanyId(int id)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var companyId = from c in dbEntity.Equipment
                                where c.Id == id
                                select c.CompanyID;

                return (int)companyId.FirstOrDefault();
            }
        }

        public class _EquipmentClass
        {
            public List<EquipmentClass> GetAll()
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.EquipmentClass.AsNoTracking()
                             select c;
                return L2Enty.ToList<EquipmentClass>();
            }

            public List<EquipmentClass> GetAllBySuperAdmin()
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.EquipmentClass.AsNoTracking()
                             select c;
                return L2Enty.ToList<EquipmentClass>();
            }

            public EquipmentClass GetByid(int id)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.EquipmentClass.AsNoTracking()
                             where c.Id == id
                             select c;
                return L2Enty.FirstOrDefault();
            }

            public int Add(EquipmentClass equipmentClass)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                dbEntity.EquipmentClass.Add(equipmentClass);
                dbEntity.SaveChanges();
                return equipmentClass.Id;
            }

            public void Update(EquipmentClass equipmentClass)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                dbEntity.EquipmentClass.Attach(equipmentClass);
                dbEntity.Entry(equipmentClass).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }

            public void Delete(EquipmentClass equipmentClass)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                dbEntity.EquipmentClass.Attach(equipmentClass);
                dbEntity.Entry(equipmentClass).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }

        }

        public class _UserRole
        {
            public List<UserRole> GetAllByCompanyId(int companyId)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.UserRole.AsNoTracking()
                             where c.CompanyId == companyId
                             select c;
                return L2Enty.ToList<UserRole>();
            }

            public UserRole GetByid(int id)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.UserRole.AsNoTracking()
                             where c.Id == id
                             select c;
                return L2Enty.FirstOrDefault();
            }

            public int Add(UserRole userRole)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                dbEntity.UserRole.Add(userRole);
                dbEntity.SaveChanges();
                return userRole.Id;
            }

            public void Update(UserRole userRole)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                dbEntity.UserRole.Attach(userRole);
                dbEntity.Entry(userRole).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }

            public void Delete(UserRole userRole)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                dbEntity.UserRole.Attach(userRole);
                dbEntity.Entry(userRole).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }

        }

        public class _UserRolePermission
        {
            public List<UserRolePermission> GetAllByPermissionCatalogCode(int permissionCatalogCode)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.UserRolePermission.AsNoTracking()
                             where c.PermissionCatalogCode == permissionCatalogCode
                             select c;
                return L2Enty.ToList<UserRolePermission>();
            }

            public List<UserRolePermission> GetAllByUserRoleId(int userRoleId)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.UserRolePermission.AsNoTracking()
                             where c.UserRoleID == userRoleId
                             select c;
                return L2Enty.ToList<UserRolePermission>();
            }

            public List<UserRolePermission> GetAllByUserRoleIdIncludeDelete(int userRoleId)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.UserRolePermission.AsNoTracking()
                             where c.UserRoleID == userRoleId
                             select c;
                return L2Enty.ToList<UserRolePermission>();
            }

            public UserRolePermission GetByid(int id)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.UserRolePermission.AsNoTracking()
                             where c.Id == id
                             select c;
                return L2Enty.FirstOrDefault();
            }

            public int Add(UserRolePermission userRolePermission)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                dbEntity.UserRolePermission.Add(userRolePermission);
                dbEntity.SaveChanges();
                return userRolePermission.Id;
            }

            public void AddManyRows(List<UserRolePermission> userRolePermissionList)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                foreach (UserRolePermission userRolePermission in userRolePermissionList)
                {
                    dbEntity.UserRolePermission.Add(userRolePermission);
                }
                dbEntity.SaveChanges();
            }

            public void Update(UserRolePermission userRolePermission)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                dbEntity.UserRolePermission.Attach(userRolePermission);
                dbEntity.Entry(userRolePermission).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }
            public void UpdateManyRows(List<UserRolePermission> userRolePermissionList)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                foreach (UserRolePermission userRolePermission in userRolePermissionList)
                {
                    dbEntity.UserRolePermission.Attach(userRolePermission);
                    dbEntity.Entry(userRolePermission).State = System.Data.Entity.EntityState.Modified;
                }
                dbEntity.SaveChanges();
            }

            public void Delete(UserRolePermission userRolePermission)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                dbEntity.UserRolePermission.Attach(userRolePermission);
                dbEntity.Entry(userRolePermission).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }

        }

        public class _PermissionCatalog
        {
            public List<PermissionCatalog> GetAll()
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.PermissionCatalog.AsNoTracking()
                             select c;
                return L2Enty.ToList<PermissionCatalog>();
            }

            public List<PermissionCatalog> GetAllBySuperAdmin()
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.PermissionCatalog.AsNoTracking()
                             select c;
                return L2Enty.ToList<PermissionCatalog>();
            }

            public PermissionCatalog GetByid(int id)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.PermissionCatalog.AsNoTracking()
                             where c.Id == id
                             select c;
                return L2Enty.FirstOrDefault();
            }

            public int Add(PermissionCatalog permissionCatalog)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                dbEntity.PermissionCatalog.Add(permissionCatalog);
                dbEntity.SaveChanges();
                return permissionCatalog.Id;
            }

            public void Update(PermissionCatalog permissionCatalog)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                dbEntity.PermissionCatalog.Attach(permissionCatalog);
                dbEntity.Entry(permissionCatalog).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }

            public void Delete(PermissionCatalog permissionCatalog)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                dbEntity.PermissionCatalog.Attach(permissionCatalog);
                dbEntity.Entry(permissionCatalog).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }

        }

        public class _MessageCatalog
        {
            public List<MessageCatalog> GetAllByCompanyId(int companyId)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.MessageCatalog.AsNoTracking()
                             where c.CompanyID == companyId
                             select c;
                return L2Enty.ToList<MessageCatalog>();
            }

            public List<MessageCatalog> GetAllNonChildByCompanyId(int companyId)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.MessageCatalog.AsNoTracking()
                             where c.CompanyID == companyId && c.ChildMessageFlag == false
                             select c;
                return L2Enty.ToList<MessageCatalog>();
            }

            public MessageCatalog GetByid(int id)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.MessageCatalog.AsNoTracking()
                             where c.Id == id
                             select c;
                return L2Enty.FirstOrDefault();
            }

            public int Add(MessageCatalog messageCatalog)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                dbEntity.MessageCatalog.Add(messageCatalog);
                dbEntity.SaveChanges();
                return messageCatalog.Id;
            }

            public void Update(MessageCatalog messageCatalog)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                dbEntity.MessageCatalog.Attach(messageCatalog);
                dbEntity.Entry(messageCatalog).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }

            public void Delete(MessageCatalog messageCatalog)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                dbEntity.MessageCatalog.Attach(messageCatalog);
                dbEntity.Entry(messageCatalog).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }

        }

        public class _MessageElement
        {
            public List<MessageElement> GetAllByMessageCatalog(int messageCatalogId)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from element in dbEntity.MessageElement.AsNoTracking()
                             join catalog in dbEntity.MessageCatalog.AsNoTracking() on element.MessageCatalogID equals catalog.Id
                             where catalog.Id == messageCatalogId && element.MessageCatalogID == messageCatalogId
                             select element;
                return L2Enty.ToList<MessageElement>();
            }

            public MessageElement GetByid(int id)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.MessageElement.AsNoTracking()
                             where c.Id == id
                             select c;
                return L2Enty.FirstOrDefault();
            }

            public MessageElement GetByid(int[] idList)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.MessageElement.AsNoTracking()
                             where idList.Contains(c.Id)
                             select c;
                return L2Enty.FirstOrDefault();
            }

            public int Add(MessageElement messageElement)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                dbEntity.MessageElement.Add(messageElement);
                dbEntity.SaveChanges();
                return messageElement.Id;
            }

            public void Update(MessageElement messageElement)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                dbEntity.MessageElement.Attach(messageElement);
                dbEntity.Entry(messageElement).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }

            public void Delete(MessageElement messageElement)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                dbEntity.MessageElement.Attach(messageElement);
                dbEntity.Entry(messageElement).State = System.Data.Entity.EntityState.Deleted;

                dbEntity.SaveChanges();
            }

        }

        public class _MessageMandatoryElementDef
        {
            public List<MessageMandatoryElementDef> GetAll()
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.MessageMandatoryElementDef.AsNoTracking()
                             select c;
                return L2Enty.ToList<MessageMandatoryElementDef>();
            }

            public List<MessageMandatoryElementDef> GetAllBySuperAdmin()
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.MessageMandatoryElementDef.AsNoTracking()
                             select c;
                return L2Enty.ToList<MessageMandatoryElementDef>();
            }

            public MessageMandatoryElementDef GetByid(int id)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.MessageMandatoryElementDef.AsNoTracking()
                             where c.Id == id
                             select c;
                return L2Enty.FirstOrDefault();
            }

            public int Add(MessageMandatoryElementDef messageMandatoryElementDef)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                dbEntity.MessageMandatoryElementDef.Add(messageMandatoryElementDef);
                dbEntity.SaveChanges();
                return messageMandatoryElementDef.Id;
            }

            public void Update(MessageMandatoryElementDef messageMandatoryElementDef)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                dbEntity.MessageMandatoryElementDef.Attach(messageMandatoryElementDef);
                dbEntity.Entry(messageMandatoryElementDef).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }

            public void Delete(MessageMandatoryElementDef messageMandatoryElementDef)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                dbEntity.MessageMandatoryElementDef.Attach(messageMandatoryElementDef);
                dbEntity.Entry(messageMandatoryElementDef).State = System.Data.Entity.EntityState.Deleted;

                dbEntity.SaveChanges();
            }
        }

        public class _OperationTask
        {
            public List<OperationTask> Search(string taskStatus, int hours)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                DateTime searchDatetime = DateTime.UtcNow.AddHours(hours);

                if (taskStatus != null)
                {
                    var L2Enty = from c in dbEntity.OperationTask.AsNoTracking()
                                 where c.CreatedAt > searchDatetime && c.TaskStatus.ToString() == taskStatus.ToString() && c.DeletedFlag == false
                                 orderby c.CreatedAt descending
                                 select c;
                    return L2Enty.ToList<OperationTask>();
                }
                else
                {
                    var L2Enty = from c in dbEntity.OperationTask.AsNoTracking()
                                 where c.CreatedAt > searchDatetime && c.DeletedFlag == false
                                 orderby c.CreatedAt descending
                                 select c;
                    return L2Enty.ToList<OperationTask>();
                }

            }
            public List<OperationTask> Search(string taskStatus, int hours, int companyId)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                DateTime searchDatetime = DateTime.UtcNow.AddHours(hours);

                if (taskStatus != null)
                {
                    var L2Enty = from c in dbEntity.OperationTask.AsNoTracking()
                                 where c.CompanyId == companyId && c.CreatedAt > searchDatetime && c.TaskStatus.ToString() == taskStatus.ToString() && c.DeletedFlag == false
                                 orderby c.CreatedAt descending
                                 select c;
                    return L2Enty.ToList<OperationTask>();
                }
                else
                {
                    var L2Enty = from c in dbEntity.OperationTask.AsNoTracking()
                                 where c.CompanyId == companyId && c.CreatedAt > searchDatetime && c.DeletedFlag == false
                                 orderby c.CreatedAt descending
                                 select c;
                    return L2Enty.ToList<OperationTask>();
                }

            }
            public List<OperationTask> GetAllByCompanyId(int companyId)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.OperationTask.AsNoTracking()
                             where c.CompanyId == companyId && c.DeletedFlag == false
                             select c;
                return L2Enty.ToList<OperationTask>();
            }

            public OperationTask GetByid(int id)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.OperationTask.AsNoTracking()
                             where c.Id == id && c.DeletedFlag == false
                             select c;
                return L2Enty.FirstOrDefault();
            }

            public int Add(OperationTask operationTask)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                operationTask.CreatedAt = DateTime.Parse(DateTime.UtcNow.ToString());
                dbEntity.OperationTask.Add(operationTask);
                dbEntity.SaveChanges();
                return operationTask.Id;
            }

            public void Update(OperationTask operationTask)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                operationTask.UpdatedAt = DateTime.Parse(DateTime.UtcNow.ToString());
                dbEntity.OperationTask.Attach(operationTask);
                dbEntity.Entry(operationTask).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }

            public void Delete(OperationTask operationTask)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                operationTask.UpdatedAt = DateTime.Parse(DateTime.UtcNow.ToString());
                operationTask.DeletedFlag = true;
                dbEntity.OperationTask.Attach(operationTask);
                dbEntity.Entry(operationTask).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }

        }

        public class _EventRuleCatalog
        {
            public List<EventRuleCatalog> GetAllByCompanyId(int companyId)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.EventRuleCatalog.AsNoTracking()
                             where c.CompanyId == companyId
                             select c;
                return L2Enty.ToList<EventRuleCatalog>();
            }

            public EventRuleCatalog GetByid(int id)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.EventRuleCatalog.AsNoTracking()
                             where c.Id == id
                             select c;
                return L2Enty.FirstOrDefault();
            }

            public int Add(EventRuleCatalog EventRuleCatalog)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                dbEntity.EventRuleCatalog.Add(EventRuleCatalog);
                dbEntity.SaveChanges();
                return EventRuleCatalog.Id;
            }

            public void Update(EventRuleCatalog EventRuleCatalog)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                dbEntity.EventRuleCatalog.Attach(EventRuleCatalog);
                dbEntity.Entry(EventRuleCatalog).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }

            public void Delete(EventRuleCatalog EventRuleCatalog)
            {
                int id = EventRuleCatalog.Id;
                CDStudioEntities dbEntity = new CDStudioEntities();
                dbEntity.EventRuleCatalog.Attach(EventRuleCatalog);
                dbEntity.Entry(EventRuleCatalog).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }

        }

        public class _Application
        {
            public List<Application> GetAllByCompanyId(int companyId)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.Application.AsNoTracking()
                             where c.CompanyId == companyId
                             select c;
                return L2Enty.ToList<Application>();
            }

            public Application GetByid(int id)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.Application.AsNoTracking()
                             where c.Id == id
                             select c;
                return L2Enty.FirstOrDefault();
            }

            public int Add(Application externalApplication)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                dbEntity.Application.Add(externalApplication);
                dbEntity.SaveChanges();
                return externalApplication.Id;
            }

            public void Update(Application externalApplication)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                dbEntity.Application.Attach(externalApplication);
                dbEntity.Entry(externalApplication).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }

            public void Delete(Application externalApplication)
            {
                int id = externalApplication.Id;
                CDStudioEntities dbEntity = new CDStudioEntities();
                dbEntity.Application.Attach(externalApplication);
                dbEntity.Entry(externalApplication).State = System.Data.Entity.EntityState.Modified;

                // Delete AlaraInAction
                dbEntity.EventInAction.RemoveRange(dbEntity.EventInAction.Where(s => s.ApplicationId == id));

                dbEntity.SaveChanges();
            }

        }
        
        public class _EventInAction
        {
            public List<EventInAction> GetAllByEventRuleCatalogId(int EventRuleCatalogId)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.EventInAction.AsNoTracking()
                             where c.EventRuleCatalogId == EventRuleCatalogId
                             select c;
                return L2Enty.ToList<EventInAction>();
            }

            public EventInAction GetById(int id)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.EventInAction.AsNoTracking()
                             where c.Id == id
                             select c;
                return L2Enty.FirstOrDefault<EventInAction>();
            }

            public void Add(List<EventInAction> EventInActionList)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                foreach (EventInAction EventInAction in EventInActionList)
                {
                    dbEntity.EventInAction.Add(EventInAction);
                }
                dbEntity.SaveChanges();
            }

            public void Update(List<EventInAction> EventInActionList)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                foreach (EventInAction EventInAction in EventInActionList)
                {
                    dbEntity.EventInAction.Attach(EventInAction);
                    dbEntity.Entry(EventInAction).State = System.Data.Entity.EntityState.Modified;
                }

                dbEntity.SaveChanges();
            }

            public void Delete(List<EventInAction> EventInActionList)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                foreach (EventInAction EventInAction in EventInActionList)
                {
                    dbEntity.EventInAction.Attach(EventInAction);
                    dbEntity.Entry(EventInAction).State = System.Data.Entity.EntityState.Deleted;
                }

                dbEntity.SaveChanges();
            }

        }

        public class _EventRuleItem
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

            public List<DetailForRuleEngineModel> GetAllByEventRuleCatalogIdForRuleEngine(int EventRuleCatalogId)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                List<EventRuleItem> itemList = (from c in dbEntity.EventRuleItem.AsNoTracking()
                                                where c.EventRuleCatalogId == EventRuleCatalogId
                                                orderby c.Ordering ascending
                                                select c).ToList<EventRuleItem>();

                List<DetailForRuleEngineModel> returnDataList = new List<DetailForRuleEngineModel>();

                foreach (var item in itemList)
                {
                    DetailForRuleEngineModel returnData = new DetailForRuleEngineModel();
                    returnData.Id = item.Id;
                    returnData.EventRuleCatalogId = EventRuleCatalogId;
                    returnData.Ordering = item.Ordering;
                    returnData.MessageElementId = item.MessageElementId;
                    returnData.EqualOperation = item.EqualOperation;
                    returnData.Value = item.Value;
                    returnData.BitWiseOperation = item.BitWiseOperation;
                    returnData.MessageElementDataType = item.MessageElement1.ElementDataType;

                    if (item.MessageElement != null)
                        returnData.MessageElementFullName = item.MessageElement.ElementName + "_" + item.MessageElement1.ElementName;
                    else
                        returnData.MessageElementFullName = item.MessageElement1.ElementName;

                    returnDataList.Add(returnData);
                }
                return returnDataList;
            }

            public List<EventRuleItem> GetAllByEventRuleCatalogId(int EventRuleCatalogId)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.EventRuleItem.AsNoTracking()
                             where c.EventRuleCatalogId == EventRuleCatalogId
                             orderby c.Ordering ascending
                             select c;
                return L2Enty.ToList<EventRuleItem>();
            }

            public EventRuleItem GetByid(int id)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.EventRuleItem.AsNoTracking()
                             where c.Id == id
                             select c;
                return L2Enty.FirstOrDefault();
            }

            public int Add(EventRuleItem EventRuleItem)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                dbEntity.EventRuleItem.Add(EventRuleItem);
                dbEntity.SaveChanges();
                return EventRuleItem.Id;
            }

            public void Add(List<EventRuleItem> EventRuleItemList)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                foreach (EventRuleItem EventRuleItem in EventRuleItemList)
                {
                    dbEntity.EventRuleItem.Add(EventRuleItem);
                }

                dbEntity.SaveChanges();
            }

            public void Update(EventRuleItem EventRuleItem)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                dbEntity.EventRuleItem.Attach(EventRuleItem);
                dbEntity.Entry(EventRuleItem).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }

            public void Delete(EventRuleItem EventRuleItem)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                dbEntity.EventRuleItem.Attach(EventRuleItem);
                dbEntity.Entry(EventRuleItem).State = System.Data.Entity.EntityState.Deleted;

                dbEntity.SaveChanges();
            }

            public void Delete(List<EventRuleItem> EventRuleItemList)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                foreach (EventRuleItem EventRuleItem in EventRuleItemList)
                {
                    dbEntity.EventRuleItem.Attach(EventRuleItem);
                    dbEntity.Entry(EventRuleItem).State = System.Data.Entity.EntityState.Deleted;
                }

                dbEntity.SaveChanges();
            }
        }

        public class _WidgetClass
        {
            public List<WidgetClass> GetAll()
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.WidgetClass.AsNoTracking()
                             select c;
                return L2Enty.ToList<WidgetClass>();
            }

            public List<WidgetClass> GetAllByLevel(string level)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.WidgetClass.AsNoTracking()
                             where c.Level == level.ToLower() 
                             select c;
                return L2Enty.ToList<WidgetClass>();
            }

            public WidgetClass GetByid(int id)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.WidgetClass.AsNoTracking()
                             where c.Id == id
                             select c;
                return L2Enty.FirstOrDefault();
            }

            public int Add(WidgetClass widgetClass)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                dbEntity.WidgetClass.Add(widgetClass);
                dbEntity.SaveChanges();
                return widgetClass.Id;
            }

            public void Update(WidgetClass widgetClass)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                dbEntity.WidgetClass.Attach(widgetClass);
                dbEntity.Entry(widgetClass).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }

            public void Delete(WidgetClass widgetClass)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                dbEntity.WidgetClass.Attach(widgetClass);
                dbEntity.Entry(widgetClass).State = System.Data.Entity.EntityState.Deleted;

                dbEntity.SaveChanges();
            }

        }

        public class _WidgetCatalog
        {
            public List<WidgetCatalog> GetAllByCompanyId(int companyId)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.WidgetCatalog.AsNoTracking()
                             where c.CompanyID == companyId
                             select c;
                return L2Enty.ToList<WidgetCatalog>();
            }

            public List<WidgetCatalog> GetAllByCompanyId(int companyId, string level)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.WidgetCatalog.AsNoTracking()
                             where c.CompanyID == companyId && c.Level == level.ToLower() 
                             select c;
                return L2Enty.ToList<WidgetCatalog>();
            }

            public List<WidgetCatalog> GetAllByLevel(string level)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.WidgetCatalog.AsNoTracking()
                             where c.Level == level.ToLower()
                             select c;
                return L2Enty.ToList<WidgetCatalog>();
            }

            public WidgetCatalog GetByid(int id)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.WidgetCatalog.AsNoTracking()
                             where c.Id == id 
                             select c;
                return L2Enty.FirstOrDefault();
            }

            public int Add(WidgetCatalog widgetCatalog)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                dbEntity.WidgetCatalog.Add(widgetCatalog);
                dbEntity.SaveChanges();
                return widgetCatalog.Id;
            }

            public void Update(WidgetCatalog widgetCatalog)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                dbEntity.WidgetCatalog.Attach(widgetCatalog);
                dbEntity.Entry(widgetCatalog).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }

            public void Delete(WidgetCatalog widgetCatalog)
            {
                int id = widgetCatalog.Id;
                CDStudioEntities dbEntity = new CDStudioEntities();
                dbEntity.WidgetCatalog.Attach(widgetCatalog);
                dbEntity.Entry(widgetCatalog).State = System.Data.Entity.EntityState.Modified;

                dbEntity.DashboardWidgets.RemoveRange(dbEntity.DashboardWidgets.Where(s => s.WidgetCatalogID == id));

                dbEntity.SaveChanges();
            }
        }

        public class _Dashboard
        {
            public List<Dashboard> GetAllByCompanyId(int companyId)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.Dashboard.AsNoTracking()
                             where c.CompanyID == companyId
                             select c;
                return L2Enty.ToList<Dashboard>();
            }

            public List<Dashboard> GetAllByCompanyId(int companyId, string type)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.Dashboard.AsNoTracking()
                             where c.CompanyID == companyId && c.DashboardType == type.ToLower()
                             select c;
                return L2Enty.ToList<Dashboard>();
            }

            public List<Dashboard> GetAllEquipmentClassByCompanyId(int companyId)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.Dashboard.AsNoTracking()
                             where c.CompanyID == companyId && c.DashboardType == "EquipmentClass"
                             select c;
                return L2Enty.ToList<Dashboard>();
            }
            public List<Dashboard> GetAllEquipmentByCompanyId(int companyId)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.Dashboard.AsNoTracking()
                             where c.CompanyID == companyId && c.DashboardType == "Equipment"
                             select c;
                return L2Enty.ToList<Dashboard>();
            }
            public List<Dashboard> GetAllByFactoryId(int factoryId)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.Dashboard.AsNoTracking()
                             where c.FactoryID == factoryId
                             select c;
                return L2Enty.ToList<Dashboard>();
            }

            public Dashboard GetByid(int id)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.Dashboard.AsNoTracking()
                             where c.Id == id
                             select c;
                return L2Enty.FirstOrDefault();
            }

            public int Add(Dashboard dashboard)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                dbEntity.Dashboard.Add(dashboard);
                dbEntity.SaveChanges();
                return dashboard.Id;
            }

            public void Update(Dashboard dashboard)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                dbEntity.Dashboard.Attach(dashboard);
                dbEntity.Entry(dashboard).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }

            public void Delete(Dashboard dashboard)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                dbEntity.Dashboard.Attach(dashboard);
                dbEntity.Entry(dashboard).State = System.Data.Entity.EntityState.Deleted;

                dbEntity.SaveChanges();
            }

        }

        public class _DashboardWidget
        {
            public List<DashboardWidgets> GetAllByDashboardId(int dashboardId)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.DashboardWidgets.AsNoTracking()
                             where c.DashboardID == dashboardId
                             orderby c.RowNo ascending, c.ColumnSeq ascending
                             select c;
                return L2Enty.ToList<DashboardWidgets>();
            }

            public DashboardWidgets GetByid(int id)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.DashboardWidgets.AsNoTracking()
                             where c.Id == id
                             select c;
                return L2Enty.FirstOrDefault();
            }

            public int Add(DashboardWidgets dashboardWidget)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                dbEntity.DashboardWidgets.Add(dashboardWidget);
                dbEntity.SaveChanges();
                return dashboardWidget.Id;
            }

            public void Update(DashboardWidgets dashboardWidget)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                dbEntity.DashboardWidgets.Attach(dashboardWidget);
                dbEntity.Entry(dashboardWidget).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }
            public void Update(List<DashboardWidgets> dashboardWidgetList)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                foreach (DashboardWidgets widget in dashboardWidgetList)
                {
                    dbEntity.DashboardWidgets.Attach(widget);
                    dbEntity.Entry(widget).State = System.Data.Entity.EntityState.Modified;
                }

                dbEntity.SaveChanges();
            }

            public void Delete(DashboardWidgets dashboardWidget)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                dbEntity.DashboardWidgets.Attach(dashboardWidget);
                dbEntity.Entry(dashboardWidget).State = System.Data.Entity.EntityState.Deleted;

                dbEntity.SaveChanges();
            }
        }

        public class _AccumulateUsageLog
        {
            public List<AccumulateUsageLog> GetAllByCompanyId(int companyId, int days, string order)
            {
                DateTime datetime_0000 = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd"));
                days--;
                if (days > 0)
                    datetime_0000 = datetime_0000.AddDays(-days);

                CDStudioEntities dbEntity = new CDStudioEntities();
                if (order.ToLower().Equals("desc"))
                {
                    var L2Enty = from c in dbEntity.AccumulateUsageLog.AsNoTracking()
                                 where c.CompanyId == companyId && c.UpdatedAt >= datetime_0000
                                 orderby c.UpdatedAt descending
                                 select c;
                    return L2Enty.ToList<AccumulateUsageLog>();
                }
                else
                {
                    var L2Enty = from c in dbEntity.AccumulateUsageLog.AsNoTracking()
                                 where c.CompanyId == companyId && c.UpdatedAt >= datetime_0000
                                 orderby c.UpdatedAt ascending
                                 select c;
                    return L2Enty.ToList<AccumulateUsageLog>();
                }
            }

            public AccumulateUsageLog GetLastByCommpanyId(int companyId)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();

                var L2Enty = from c in dbEntity.AccumulateUsageLog.AsNoTracking()
                             where c.CompanyId == companyId
                             orderby c.UpdatedAt descending
                             select c;
                return L2Enty.FirstOrDefault<AccumulateUsageLog>();
            }

            public List<AccumulateUsageLog> GetAll(int days, string order)
            {
                DateTime datetime_0000 = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd"));
                days--;
                if (days > 0)
                    datetime_0000 = datetime_0000.AddDays(-days);

                CDStudioEntities dbEntity = new CDStudioEntities();
                if (order.ToLower().Equals("desc"))
                {
                    var L2Enty = from c in dbEntity.AccumulateUsageLog.AsNoTracking()
                                 where c.UpdatedAt >= datetime_0000
                                 orderby c.UpdatedAt descending
                                 select c;
                    return L2Enty.ToList<AccumulateUsageLog>();
                }
                else
                {
                    var L2Enty = from c in dbEntity.AccumulateUsageLog.AsNoTracking()
                                 where c.UpdatedAt >= datetime_0000
                                 select c;
                    return L2Enty.ToList<AccumulateUsageLog>();
                }
            }

            public AccumulateUsageLog GetByid(int id)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                var L2Enty = from c in dbEntity.AccumulateUsageLog.AsNoTracking()
                             where c.Id == id
                             select c;
                return L2Enty.FirstOrDefault();
            }

            public int Add(AccumulateUsageLog accUsageLog)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                dbEntity.AccumulateUsageLog.Add(accUsageLog);
                dbEntity.SaveChanges();
                return accUsageLog.Id;
            }

            public void Add(List<AccumulateUsageLog> accUsageLogList)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                foreach (AccumulateUsageLog accUsageLog in accUsageLogList)
                {
                    dbEntity.AccumulateUsageLog.Add(accUsageLog);
                }
                dbEntity.SaveChanges();
            }

            public void Update(AccumulateUsageLog accUsageLog)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                dbEntity.AccumulateUsageLog.Attach(accUsageLog);
                dbEntity.Entry(accUsageLog).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }

            public void Delete(AccumulateUsageLog accUsageLog)
            {
                CDStudioEntities dbEntity = new CDStudioEntities();
                dbEntity.AccumulateUsageLog.Attach(accUsageLog);
                dbEntity.Entry(accUsageLog).State = System.Data.Entity.EntityState.Deleted;

                dbEntity.SaveChanges();
            }
        }

        //public class _UsageLogSumByDay
        //{
        //    public List<UsageLogSumByDay> GetAll(int days, string order)
        //    {
        //        DateTime datetime_0000 = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd"));
        //        days--;
        //        if (days > 0)
        //            datetime_0000 = datetime_0000.AddDays(-days);

        //        CDStudioEntities dbEntity = new CDStudioEntities();
        //        if (order.ToLower().Equals("desc"))
        //        {
        //            var L2Enty = from c in dbEntity.UsageLogSumByDay.AsNoTracking()
        //                         where c.UpdatedDateTime >= datetime_0000
        //                         orderby c.UpdatedDateTime descending
        //                         select c;
        //            return L2Enty.ToList<UsageLogSumByDay>();
        //        }
        //        else
        //        {
        //            var L2Enty = from c in dbEntity.UsageLogSumByDay.AsNoTracking()
        //                         where c.UpdatedDateTime >= datetime_0000
        //                         select c;
        //            return L2Enty.ToList<UsageLogSumByDay>();
        //        }
        //    }
        //    public UsageLogSumByDay GetLast()
        //    {
        //        CDStudioEntities dbEntity = new CDStudioEntities();
        //        var L2Enty = from c in dbEntity.UsageLogSumByDay.AsNoTracking()
        //                     orderby c.UpdatedDateTime descending
        //                     select c;
        //        return L2Enty.FirstOrDefault<UsageLogSumByDay>();
        //    }
        //}
    }
}
