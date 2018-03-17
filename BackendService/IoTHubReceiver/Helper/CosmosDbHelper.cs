using CDSShareLib.Helper;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTHubReceiver.Helper
{
    //class CosmosDbHelper
    //{
    //    private DocDBHelper docDBHelper { get; set; }
    //    private string docDbDatabaseName { get; set; }
    //    private string docDbCollectionId { get; set; }

    //    public CosmosDbHelper(string connectionString, string dbName, string collectionId)
    //    {
    //        this.docDBHelper = new DocDBHelper(connectionString);
    //        this.docDbDatabaseName = dbName;
    //        this.docDbCollectionId = collectionId;
    //    }
        
    //    public async Task checkDatabaseIfNotExists()
    //    {
    //        await docDBHelper.checkDatabaseIfNotExists(docDbDatabaseName);
    //    }

    //    public async Task checkCollectionIfNotExists()
    //    {
    //        await docDBHelper.checkCollectionIfNotExists(docDbDatabaseName, docDbCollectionId);
    //    }

    //    public async Task<ResourceResponse<Document>> putDocumentAsync(object document)
    //    {
    //        return await docDBHelper.putDocumentAsync(docDbDatabaseName, docDbCollectionId, document);
    //    }
    //}
}
