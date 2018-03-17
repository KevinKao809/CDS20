using Microsoft.Azure.Devices;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDSShareLib.Helper
{
    public class IoTHubHelper
    {
        string _connectionString;
        ServiceClient _serviceClient = null;
        RegistryManager _registryManager = null;

        public IoTHubHelper(string connectionString)
        {
            _connectionString = connectionString;
            _serviceClient = ServiceClient.CreateFromConnectionString(_connectionString);
            _registryManager = RegistryManager.CreateFromConnectionString(_connectionString);
        }

        public string[] GetPartitions()
        {
            EventHubClient eventHubClient = EventHubClient.CreateFromConnectionString(_connectionString, "messages/events");
            return eventHubClient.GetRuntimeInformation().PartitionIds;
        }

        public void SendC2DMessage(string deviceId, JObject message)
        {
            try
            {
                var msg = new Message(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(message)));
                _serviceClient.SendAsync(deviceId, msg);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<bool> RegisterDeviceByKey(string deviceId, string deviceKey)
        {
            Device device = new Device(deviceId)
            {
                Authentication = new AuthenticationMechanism()
                {
                    SymmetricKey = new SymmetricKey()
                    {
                        PrimaryKey = deviceKey,
                        SecondaryKey = deviceKey
                    }
                }
            };
            try
            {
                Device existDevice = await _registryManager.GetDeviceAsync(deviceId);
                if (existDevice == null)
                    await _registryManager.AddDeviceAsync(device);
                else
                    await _registryManager.UpdateDeviceAsync(device, true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            return true;
        }

        public async Task<bool> RegisterDeviceByCertThumbprint(string deviceId, string CertificateThumbprint)
        {
            Device device = new Device(deviceId)
            {
                Authentication = new AuthenticationMechanism()
                {
                    X509Thumbprint = new X509Thumbprint()
                    {
                        PrimaryThumbprint = CertificateThumbprint
                    }
                }
            };
            try
            {
                Device existDevice = await _registryManager.GetDeviceAsync(deviceId);
                if (existDevice == null)
                    await _registryManager.AddDeviceAsync(device);
                else
                    await _registryManager.UpdateDeviceAsync(device, true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            return true;
        }

        public async Task<bool> RemoveDevice(string deviceId)
        {
            try
            {
                Device existDevice = await _registryManager.GetDeviceAsync(deviceId);
                if (existDevice != null)
                    await _registryManager.RemoveDeviceAsync(deviceId);
                else
                    throw new Exception("No such IoT Device ID: " + deviceId);

            }
            catch (Exception)
            {
                throw;
            }
            return true;
        }

        public async Task<bool> UpdateTwinDesiredProperty(string deviceId, JObject _DeviceConfiguration)
        {
            try
            {
                var twin = await _registryManager.GetTwinAsync(deviceId);
                if (twin == null)
                {
                    throw new Exception("No such IoT Device ID: " + deviceId);
                }
                else
                {
                    //Clean old desired property
                    dynamic nullProperty = new ExpandoObject();
                    nullProperty.CDS_SystemConfig = null;
                    nullProperty.CDS_CustomizedConfig = null;
                    nullProperty.CDS_LastUpdatedTimestamp = 0;

                    var patch = new
                    {
                        properties = new
                        {
                            desired = nullProperty
                        }
                    };
                    twin = await _registryManager.UpdateTwinAsync(twin.DeviceId, JsonConvert.SerializeObject(patch), twin.ETag);

                    //Update IoTHub desired property
                    int nowUnixTimestamp = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                    dynamic _DeviceTwinsDesiredPropertyObj = new
                    {
                        CDS_SystemConfig = _DeviceConfiguration["CDS_SystemConfig"],
                        CDS_SystemConfig_CustomizedConfig = _DeviceConfiguration["CDS_CustomizedConfig"],
                        CDS_SystemConfig_LastUpdatedTimestamp = nowUnixTimestamp
                    };
                    patch = new
                    {
                        properties = new
                        {
                            desired = _DeviceTwinsDesiredPropertyObj
                        }
                    };

                    await _registryManager.UpdateTwinAsync(deviceId, JsonConvert.SerializeObject(patch), twin.ETag);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            return true;
        }
    }
}
