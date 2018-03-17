using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Azure; // Namespace for CloudConfigurationManager
using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Blob; // Namespace for Blob storage types
using System.IO;
using Microsoft.WindowsAzure.Storage.Auth;

namespace sfShareLib
{
    /*
    public class FileHelper
    {
        public string SaveFiletoStorage(string storageName, string storageKey, string localFileName, string uploadFilePath, string containerName)
        {
            // Parse the connection string and return a reference to the storage account.
            StorageCredentials creds = new StorageCredentials(storageName, storageKey);
            CloudStorageAccount strAcc = new CloudStorageAccount(creds, true);            

            CloudBlobClient blobClient = strAcc.CreateCloudBlobClient();

            // Retrieve a reference to a container.
            CloudBlobContainer container = blobClient.GetContainerReference(containerName);
            container.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });

            // Create the container if it doesn't already exist.
            container.CreateIfNotExists();

            CloudBlockBlob blockBlob = container.GetBlockBlobReference(uploadFilePath);
            // Create or overwrite the "blockBlob" blob with contents from a local file.
            using (var fileStream = File.OpenRead(localFileName))
            {
                blockBlob.UploadFromStream(fileStream);
                return blockBlob.SnapshotQualifiedUri.ToString();
            }
        }

        public bool CheckImageExtensionName(string columnName, string extensionName)
        {
            extensionName = LowerAndFilterString(extensionName);
            columnName = LowerAndFilterString(columnName);
            if (columnName.Equals("image") && (extensionName.Equals("png") || extensionName.Equals("jpg")))
                return true;

            return false;
        }

        public string LowerAndFilterString(string str)
        {
            char[] trimChar = { '\"' };
            return str.ToLower().Trim(trimChar); ;
        }
    }
    */
}
