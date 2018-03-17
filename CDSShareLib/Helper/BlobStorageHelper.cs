using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDSShareLib.Helper
{
    public class BlobStorageHelper
    {
        private CloudBlobContainer _container;
        
        public BlobStorageHelper(string storageConnectionString, string storageContainer)
        {
            initialContainer(storageConnectionString, storageContainer);
        }
        private void initialContainer(string storageConnectionString, string storageContainer)
        {
            try
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

                _container = blobClient.GetContainerReference(storageContainer);
                _container.CreateIfNotExistsAsync();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public string SaveFiletoStorage(string localFileName, string uploadFilePath)
        {
            CloudBlockBlob blockBlob = _container.GetBlockBlobReference(uploadFilePath);
            // Create or overwrite the "blockBlob" blob with contents from a local file.
            using (var fileStream = File.OpenRead(localFileName))
            {
                blockBlob.UploadFromStream(fileStream);
                return blockBlob.SnapshotQualifiedUri.ToString();
            }
        }

        public async Task Save(string blobName, string data)
        {
            try
            {
                CloudBlockBlob blockBlob = _container.GetBlockBlobReference(blobName);
                await blockBlob.UploadTextAsync(data);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
