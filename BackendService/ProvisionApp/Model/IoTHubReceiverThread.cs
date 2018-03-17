using CDSShareLib;
using CDSShareLib.Helper;
using CDSShareLib.ServiceBus;
using CDSShareLib.ServiceBus.Model;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using static CDSShareLib.Helper.AzureSQLHelper;

namespace ProvisionApp.Model
{
    public class IoTHubReceiverThread
    {
        public static string _SrvFabricBaseURI = SystemConfigurationModel.GetCDSConfigValueByKey("ServiceFabricURI");
        public static string _SrvFabricIoTHubReceiverTypeName = SystemConfigurationModel.GetCDSConfigValueByKey("SrvFabricIoTHubReceiverTypeName");
        public static string _SrvFabricCertificate = SystemConfigurationModel.GetCDSConfigValueByKey("SrvFabricCertificateLocation");
        public static string _SrvFabricCertificatePW = SystemConfigurationModel.GetCDSConfigValueByKey("SrvFabricCertificatePassword");
        public static string _CertStorageName = SystemConfigurationModel.GetCDSConfigValueByKey("SystemStorageName");
        public static string _CertStorageKey = SystemConfigurationModel.GetCDSConfigValueByKey("SystemStorageKey");

        public string _SrvFabricIoTHubReceiverTypeVersion = SystemConfigurationModel.GetCDSConfigValueByKey("SrvFabricIoTHubReceiverTypeVersion");
        public byte[] _certbytes;
        public string _CompanyId;
        public string _Version;
        IoTHubReceiverModel _IoTHubReceiverMsg;

        public IoTHubReceiverThread(string messagePayload)
        {
            _IoTHubReceiverMsg = JsonConvert.DeserializeObject<IoTHubReceiverModel>(messagePayload);

            if (_IoTHubReceiverMsg.content.iotHubId == 0)
                throw new Exception("iotHubId can't be null");     
        }

        public void ThreadProc()
        {
            OperationTaskModel operationTask = new OperationTaskModel();
            try
            {
                AzureSQLHelper.IoTHubModel iotHubModel = new AzureSQLHelper.IoTHubModel();
                IoTHub iotHub = iotHubModel.GetById(_IoTHubReceiverMsg.content.iotHubId);
                string iotHubConnectionString = iotHub.IoTHubConnectionString;

                //IoTHub iotHub = new IoTHub();
                //iotHub.Id = 1001;
                //iotHub.IoTHubName = "OPC UA Default";
                //string iotHubConnectionString = "HostName=opcuademobox.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=2eHR7uKDmrajO6201NBfUlzGGLrIDi55A2URg0EZuCo=";

                IoTHubHelper iotHubHelper = new IoTHubHelper(iotHubConnectionString);
                string[] partitions = iotHubHelper.GetPartitions();

                _Version = _IoTHubReceiverMsg.content.version;
                if (string.IsNullOrEmpty(_Version))
                    _Version = _SrvFabricIoTHubReceiverTypeVersion;

                _CompanyId = _IoTHubReceiverMsg.content.companyId.ToString();
                if (_CompanyId == "0")
                    _CompanyId = iotHub.CompanyID.ToString();

                ProvisionApp._appLogger.Info(string.Format("Company ID: {0}, IoTHub ID: {1}, Version: {2}", _CompanyId, _IoTHubReceiverMsg.content.iotHubId, _Version));

                switch (_IoTHubReceiverMsg.task)
                {
                    case TaskName.IoTHubReceiver_Launch:
                        if (iotHub.EnableMultipleReceiver)
                        {
                            for (int i = 0; i < partitions.Length; i++)
                            {
                                string label = i + "-" + partitions.Length;
                                lanuchIoTHubReceiver(iotHub.CompanyID.ToString(), iotHub, partitions[i], label);
                                ProvisionApp._appLogger.Info("[IoTHubReceiver] " + _IoTHubReceiverMsg.task + " success: CompanyId-" + _CompanyId + ", IoTHubReceiverName-" + iotHub.IoTHubName + ", Label-" + label);
                            }
                        }
                        else
                        {
                            lanuchIoTHubReceiver(_CompanyId, iotHub, "ALL", "ALL");
                            ProvisionApp._appLogger.Info("[IoTHubReceiver] " + _IoTHubReceiverMsg.task + " success: CompanyId-" + _CompanyId + ", IoTHubReceiverName-" + iotHub.IoTHubName + ", Label-" + "ALL");
                        }                        
                        break;
                    case TaskName.IoTHubReceiver_Shutdown:
                        // Shutdown All Partitions Processes (it maybe not running)
                        for (int i = 0; i < partitions.Length; i++)
                        {
                            string label = i + "-" + partitions.Length;
                            shutdownIoTHubReceiver(_CompanyId, iotHub, partitions[i], label);
                            ProvisionApp._appLogger.Info("[IoTHubReceiver] " + _IoTHubReceiverMsg.task + " success: CompanyId-" + _CompanyId + ", IoTHubReceiverName-" + iotHub.IoTHubName + ", Label-" + label);
                        }
                        // Shutdown Non-Partition Process (it maybe not running)
                        shutdownIoTHubReceiver(_CompanyId, iotHub, "ALL", "ALL");
                        ProvisionApp._appLogger.Info("[IoTHubReceiver] " + _IoTHubReceiverMsg.task + " success: CompanyId-" + _CompanyId + ", IoTHubReceiverName-" + iotHub.IoTHubName + ", Label-" + "ALL");
                        break;
                }
                operationTask.UpdateTaskBySuccess(_IoTHubReceiverMsg.taskId);
            }
            catch (Exception ex)
            {
                StringBuilder logMessage = new StringBuilder();
                logMessage.AppendLine("[IoTHubReceiver] " + _IoTHubReceiverMsg.task + " Failed: CompanyId-" + _CompanyId + ", IoTHubId-" + _IoTHubReceiverMsg.content.iotHubId);
                logMessage.AppendLine("\tMessage:" + JsonConvert.SerializeObject(this));
                logMessage.AppendLine("\tException:" + ex.Message);
                ProvisionApp._appLogger.Error(logMessage);
                operationTask.UpdateTaskByFail(_IoTHubReceiverMsg.taskId, ex.Message);
            }
        }

        private void lanuchIoTHubReceiver(string companyId, IoTHub iotHub, string partition, string label)
        {
            string iotHubReceiverLongName = "C" + companyId + "_I" + iotHub.Id + "_P" + partition + "_" + label + "_" + iotHub.IoTHubName;
            if (iotHubReceiverLongName.Length > 35)
                iotHubReceiverLongName = iotHubReceiverLongName.Substring(0, 35) + "..";

            string endPoint = "Applications/$/Create?api-version=1.0";
            string postData = "{\"Name\":\"fabric:/" + iotHubReceiverLongName + "\",\"TypeName\":\"" + _SrvFabricIoTHubReceiverTypeName + "\",\"TypeVersion\":\"" + _Version + "\",\"ParameterList\":{\"input_CompanyId\":\"" + companyId + "\",\"input_IoTHubId\":\"" + iotHub.Id + "\",\"input_Partition\":\"" + partition + "\",\"input_Label\":\"" + label + "\",\"IoTHubEventProcessor_InstanceCount\":\"1\" }}";
            CallServiceFabricAPI(endPoint, postData);
        }

        private void shutdownIoTHubReceiver(string companyId, IoTHub iotHub, string partition, string label)
        {
            string iotHubReceiverLongName = "C" + companyId + "_I" + iotHub.Id + "_P" + partition + "_" + label + "_" + iotHub.IoTHubName;
            if (iotHubReceiverLongName.Length > 35)
                iotHubReceiverLongName = iotHubReceiverLongName.Substring(0, 35) + "..";

            string endPoint = "Applications/" + iotHubReceiverLongName + "/$/Delete?api-version=1.0";
            CallServiceFabricAPI(endPoint, null);
        }

        private void CallServiceFabricAPI(string endPoint, string rawData)
        {
            ProvisionApp._appLogger.Debug("Call Service Fabric (endPoint): " + endPoint);
            ProvisionApp._appLogger.Debug("Call Service Fabric (rawData): " + rawData);
            HttpWebRequest req = null;
            string URI = _SrvFabricBaseURI + endPoint;

            try
            {
                ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback((sender, certificate, chain, policyErrors) => { return true; });

                req = (HttpWebRequest)WebRequest.Create(URI);
                req.Method = "POST";
                req.Headers.Add("api-version", "1.0");

                if (URI.ToLower().StartsWith("https://"))
                {
                    if (_certbytes == null)
                        _certbytes = loadServiceFabricCertificate();
                    X509Certificate2 cert = new X509Certificate2(_certbytes, _SrvFabricCertificatePW);
                    req.ClientCertificates.Add(cert);
                }

                if (!string.IsNullOrEmpty(rawData))
                {
                    using (Stream stm = req.GetRequestStream())
                    {
                        using (StreamWriter stmw = new StreamWriter(stm))
                        {
                            stmw.Write(rawData);
                            stmw.Close();
                        }
                    }
                    WebResponse response = req.GetResponse();
                    Stream responseStream = response.GetResponseStream();
                    response = req.GetResponse();
                    StreamReader sr = new StreamReader(response.GetResponseStream());
                    string result = sr.ReadToEnd();
                    sr.Close();
                    if (req != null) req.GetRequestStream().Close();
                }
                else
                {
                    req.ContentLength = 0;
                    WebResponse response = req.GetResponse();
                }
            }
            catch (Exception ex)
            {
                ProvisionApp._appLogger.Error("Call Service Fabric Exception: " + ex.Message);
                throw;
            }
        }

        private byte[] loadServiceFabricCertificate()
        {
            try
            {
                StorageCredentials creds = new StorageCredentials(_CertStorageName, _CertStorageKey);
                CloudStorageAccount strAcc = new CloudStorageAccount(creds, true);
                CloudBlobClient blobClient = strAcc.CreateCloudBlobClient();

                string certStore = _SrvFabricCertificate;
                int div = certStore.IndexOf("/");
                string containerName = certStore.Substring(0, div);
                string certFile = certStore.Substring(div + 1, certStore.Length - (div + 1));

                CloudBlobContainer container = blobClient.GetContainerReference(containerName);
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(certFile);
                blockBlob.FetchAttributes();
                byte[] certbytes = new byte[blockBlob.Properties.Length];
                blockBlob.DownloadToByteArray(certbytes, 0);
                return certbytes;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
