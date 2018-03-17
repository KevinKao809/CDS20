using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CDSShareLib.Helper
{
    public class DocDBHelper
    {
        public string _connectionString, _databaseName, _collectionId;
        private DocumentClient _client;

        public DocDBHelper(string connectionString)
        {
            try
            {
                _connectionString = connectionString;

                initDocumentClient();
            }
            catch (Exception)
            {
                throw new Exception("[DocumentDB] DocDBHelper initial error : ConnectionString's format is wrong");
            }
        }

        public DocDBHelper(string connectionString, string databaseName, string collectionId)
        {
            try
            {
                _connectionString = connectionString;
                _databaseName = databaseName;
                _collectionId = collectionId;

                initDocumentClient();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void initDocumentClient()
        {
            try
            {
                //init DocumentClient
                _connectionString = _connectionString.Replace("AccountEndpoint=", "");
                _connectionString = _connectionString.Replace(";", "");
                _connectionString = _connectionString.Replace("AccountKey=", ";");
                string endpointUri = _connectionString.Split(';')[0];
                string primaryKey = _connectionString.Split(';')[1];
                _client = new DocumentClient(new Uri(endpointUri), primaryKey);
            }
            catch (Exception)
            {
                throw new Exception("[DocumentDB] DocDBHelper initial error : ConnectionString's format is wrong");
            }
        }

        public async Task UpdateCollection(string databaseName, string collectionId, int defaultTimeToLive, int reservedRU)
        {
            try
            {
                DocumentCollection collectionInfo = await _client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(databaseName, collectionId));

                collectionInfo.DefaultTimeToLive = defaultTimeToLive;
                await _client.ReplaceDocumentCollectionAsync(collectionInfo);

                Offer offer = _client.CreateOfferQuery().Where(r => r.ResourceLink == collectionInfo.SelfLink).AsEnumerable().SingleOrDefault();
                offer = new OfferV2(offer, reservedRU);
                await _client.ReplaceOfferAsync(offer);

            }
            catch (DocumentClientException de)
            {
                if (de.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new Exception("[DocumentDB] Database-" + databaseName + " not exist!");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("[DocumentDB] Update Database-" + databaseName + ", collection-" + collectionId + " fail. Exception: " + ex.Message);
            }
        }

        public async Task CreateDatabaseAndCollection(string databaseName, string collectionId, string partitionKey, int defaultTimeToLive, int reservedRU)
        {
            try
            {
                await _client.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(databaseName));
            }
            catch (DocumentClientException de)
            {

                if (de.StatusCode == HttpStatusCode.NotFound)
                {
                    await _client.CreateDatabaseAsync(new Database { Id = databaseName });
                }
                else
                {
                    throw new Exception("[DocumentDB] Database-" + databaseName + " exist!");
                }
            }

            //Create collection
            try
            {
                await _client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(databaseName, collectionId));
            }
            catch (DocumentClientException de)
            {
                // If the document collection does not exist, create a new collection
                if (de.StatusCode == HttpStatusCode.NotFound)
                {
                    DocumentCollection collectionInfo = new DocumentCollection();
                    collectionInfo.Id = collectionId;
                    collectionInfo.PartitionKey.Paths.Add(partitionKey);

                    // Configure collections for maximum query flexibility including string range queries.
                    collectionInfo.IndexingPolicy = new IndexingPolicy(new RangeIndex(DataType.String) { Precision = -1 }, new RangeIndex(DataType.Number) { Precision = -1 });
                    collectionInfo.IndexingPolicy.IndexingMode = IndexingMode.Lazy;
                    collectionInfo.DefaultTimeToLive = defaultTimeToLive; //seconds

                    // Here we create a collection with 400 RU/s.
                    DocumentCollection ttlEnabledCollection = await _client.CreateDocumentCollectionAsync(
                        UriFactory.CreateDatabaseUri(databaseName),
                        collectionInfo,
                        new RequestOptions { OfferThroughput = reservedRU });
                }
                else
                {
                    throw new Exception("[DocumentDB] Databae-" + databaseName + ", CollectionId-" + collectionId + "exists!");
                }
            }
        }

        public async Task PurgeDatabase(string databaseName)
        {
            try
            {
                await _client.DeleteDatabaseAsync(UriFactory.CreateDatabaseUri(databaseName));
            }
            catch (DocumentClientException de)
            {
                if (de.StatusCode != HttpStatusCode.NotFound)
                {
                    throw;
                }
            }
        }

        public async Task PurgeDatabase()
        {
            try
            {
                await _client.DeleteDatabaseAsync(UriFactory.CreateDatabaseUri(_databaseName));
            }
            catch (DocumentClientException de)
            {
                if (de.StatusCode != HttpStatusCode.NotFound)
                {
                    throw;
                }
            }
        }

        public async Task checkDatabaseIfNotExists(string documentDbDatabaseName)
        {
            // Check to verify a database with the DocumentDbDatabaseName does not exist
            await _client.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(documentDbDatabaseName));
        }

        public async Task checkDatabaseIfNotExists()
        {
            // Check to verify a database with the DocumentDbDatabaseName does not exist
            await _client.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(_databaseName));
        }

        public async Task checkCollectionIfNotExists(string documentDbDatabaseName, string documentDbCollectionId)
        {
            await _client.ReadDocumentCollectionAsync(
                    UriFactory.CreateDocumentCollectionUri(
                        documentDbDatabaseName,
                        documentDbCollectionId));
        }

        public async Task checkCollectionIfNotExists()
        {
            await _client.ReadDocumentCollectionAsync(
                    UriFactory.CreateDocumentCollectionUri(
                        _databaseName,
                        _collectionId));
        }

        public async Task<ResourceResponse<Document>> putDocumentAsync(string documentDbDatabaseName, string documentDbCollectionId, object document)
        {
            return await _client.CreateDocumentAsync(
                UriFactory.CreateDocumentCollectionUri(documentDbDatabaseName, documentDbCollectionId),
                document);
        }

        public async Task<ResourceResponse<Document>> putDocumentAsync(object document)
        {
            return await _client.CreateDocumentAsync(
                UriFactory.CreateDocumentCollectionUri(_databaseName, _collectionId),
                document);
        }
    }
}
