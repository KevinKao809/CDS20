using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using sfShareLib;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace sfAPIService.Models
{
    public class IoTDeviceCommandModel
    {
        public class Format_Detail
        {
            public int IoTDeviceId { get; set; }
            public int DeviceCommandId { get; set; }
            public string DeviceCommandName { get; set; }
        }
        
        public class Format_UpdateByIoTDevice
        {
            public List<int> DeviceCommandIdList { get; set; }
        }

        public List<Format_Detail> GetAllByIoTDeviceId(int iotDeviceId)
        {
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                var L2Enty = (from c in dbEntity.IoTDeviceCommand
                              where c.IoTDeviceId == iotDeviceId
                              select c);

                return L2Enty.Select(s => new Format_Detail()
                {
                    IoTDeviceId = s.IoTDeviceId,
                    DeviceCommandId = s.DeviceCommandId,
                    DeviceCommandName = (s.DeviceCommandCatalog == null ? "" : s.DeviceCommandCatalog.Name)
                }).ToList<Format_Detail>();
            }
        }
        

        public void UpdateByIoTDevieId(int iotDeviceId, Format_UpdateByIoTDevice parseData)
        {            
            using (CDStudioEntities dbEntity = new CDStudioEntities())
            {
                using (var dbEntityTransaction = dbEntity.Database.BeginTransaction())
                {
                    var existingDataList = (from c in dbEntity.IoTDeviceCommand
                                            where c.IoTDeviceId == iotDeviceId
                                            select c).ToList();
                    dbEntity.IoTDeviceCommand.RemoveRange(existingDataList);
                    dbEntity.SaveChanges();

                    if (parseData != null && parseData.DeviceCommandIdList != null)
                    {
                        List<IoTDeviceCommand> dataList = new List<IoTDeviceCommand>();
                        foreach (int commandId in parseData.DeviceCommandIdList)
                        {
                            dataList.Add(new IoTDeviceCommand
                            {
                                IoTDeviceId = iotDeviceId,
                                DeviceCommandId = commandId
                            });
                        }
                        dbEntity.IoTDeviceCommand.AddRange(dataList);
                        dbEntity.SaveChanges();
                    }
                    
                    dbEntityTransaction.Commit();
                }
            }           
        }
    }
}