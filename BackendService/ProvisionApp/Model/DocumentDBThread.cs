using CDSShareLib.Helper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Text;

namespace ProvisionApp.Model
{
    public class DocumentDBThread
    {
        public string _ConnectionString;
        public string _DatabaseName;
        public string _CollectionId;
        public string _Action;
        public int _TaskId;
        public JObject _JsonMessage;

        public DocumentDBThread(string connectionString, JObject jsonMessage, string action, int taskId)
        {
            _ConnectionString = connectionString;
            _DatabaseName = jsonMessage["Content"]["companyId"].ToString();
            _CollectionId = jsonMessage["Content"]["companyId"].ToString();
            if (string.IsNullOrEmpty(_DatabaseName) || string.IsNullOrEmpty(_CollectionId))
                throw new Exception("DatabaseName and CollectionId can't be null");

            _DatabaseName = "db" + _DatabaseName;
            _Action = action;
            _TaskId = taskId;
            _JsonMessage = jsonMessage;
        }

        public async void ThreadProc()
        {
            string partitionKey = "";
            int collectionTTL = 0, collectionRU = 0;
            AzureSQLHelper.OperationTaskModel operationTask = new AzureSQLHelper.OperationTaskModel();
            try
            {
                DocDBHelper docDBHelper = new DocDBHelper(_ConnectionString);
                switch (_Action)
                {
                    case "create cosmosdb collection":
                        partitionKey = _JsonMessage["Content"]["partitionKey"].ToString();
                        collectionTTL = int.Parse(_JsonMessage["Content"]["collectionTTL"].ToString());
                        collectionRU = int.Parse(_JsonMessage["Content"]["collectionRU"].ToString());
                        await docDBHelper.CreateDatabaseAndCollection(_DatabaseName, _CollectionId, partitionKey, collectionTTL, collectionRU);
                        break;
                    case "purge cosmosdb collection":
                        await docDBHelper.PurgeDatabase(_DatabaseName);
                        break;
                    case "update cosmosdb collection":
                        collectionTTL = int.Parse(_JsonMessage["Content"]["collectionTTL"].ToString());
                        collectionRU = int.Parse(_JsonMessage["Content"]["collectionRU"].ToString());
                        await docDBHelper.UpdateCollection(_DatabaseName, _CollectionId, collectionTTL, collectionRU);
                        break;
                }
                ProvisionApp._appLogger.Info("[Cosmosdb] " + _Action + " success: Databae-" + _DatabaseName + ", CollectionId-" + _CollectionId);
                operationTask.UpdateTaskBySuccess(_TaskId);
            }
            catch (Exception ex)
            {
                StringBuilder logMessage = new StringBuilder();
                logMessage.AppendLine("[Cosmosdb] " + _Action + " Failed: Databae-" + _DatabaseName + ", CollectionId-" + _CollectionId);
                logMessage.AppendLine("\tMessage:" + JsonConvert.SerializeObject(this));
                logMessage.AppendLine("\tException:" + ex.Message);
                ProvisionApp._appLogger.Error(logMessage);
                operationTask.UpdateTaskByFail(_TaskId, ex.Message);
            }
        }
    }
}
