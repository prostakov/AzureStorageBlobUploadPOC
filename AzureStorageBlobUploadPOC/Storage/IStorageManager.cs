using System.IO;
using System.Threading.Tasks;

namespace AzureStorageBlobUploadPOC.Storage
{
    public interface IStorageManager
    {
        Task EnsureStorageExistsAsync();

        Task UploadAsync(string fileName, Stream source);
    }
}