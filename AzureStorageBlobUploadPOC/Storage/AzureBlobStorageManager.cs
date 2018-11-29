using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace AzureStorageBlobUploadPOC.Storage
{
    public class AzureBlobStorageManager : IStorageManager
    {
        readonly CloudBlobContainer _container;
        
        public AzureBlobStorageManager(string storageConnectionString, string containerName)
        {
            if (!CloudStorageAccount.TryParse(storageConnectionString, out var storageAccount))
            {
                throw new Exception("Cannot initialize CloudStorageAccount, error in storage connection string!");
            }
            
            var folderNameRegex = new Regex(@"^[a-z0-9\-]+$");
            if (!folderNameRegex.IsMatch(containerName))
            {
                throw new ArgumentException("Invalid container name format!");
            }
            
            var cloudBlobClient = storageAccount.CreateCloudBlobClient();
 
            _container = cloudBlobClient.GetContainerReference(containerName);
        }
        
        public async Task EnsureStorageExistsAsync()
        {
            if (!await _container.ExistsAsync())
            {
                await _container.CreateAsync();

                var permissions = new BlobContainerPermissions
                {
                    PublicAccess = BlobContainerPublicAccessType.Blob
                };
                await _container.SetPermissionsAsync(permissions);
            }
        }

        public async Task UploadAsync(string fileName, Stream source)
        {
            var blockBlob = _container.GetBlockBlobReference(fileName);

            await blockBlob.UploadFromStreamAsync(source);
        }

        public async Task DeleteAsync(string fileName)
        {
            var blockBlob = _container.GetBlockBlobReference(fileName);

            await blockBlob.DeleteAsync();
        }
    }
}