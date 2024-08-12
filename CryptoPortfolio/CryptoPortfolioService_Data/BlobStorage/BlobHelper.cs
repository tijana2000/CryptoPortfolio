using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;

namespace CryptoPortfolioService_Data.BlobStorage
{
    public class BlobHelper
    {
        private CloudStorageAccount _storageAccount;
        private CloudBlobClient _blobClient;
        private CloudBlobContainer _container;
        public BlobHelper()
        {
            _storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            _blobClient = _storageAccount.CreateCloudBlobClient();
            _container = _blobClient.GetContainerReference("user");
            _container.CreateIfNotExists();
        }
        
        public string UploadProfileImage(string userId, byte[] imageByteArray)
        {
            string uniqueBlobName = string.Format($"image_{userId}");
            CloudBlockBlob blob = _container.GetBlockBlobReference(uniqueBlobName);

            blob.Properties.ContentType = "image/bmp";
            blob.UploadFromByteArray(imageByteArray, 0, imageByteArray.Length);

            return blob.Uri.ToString();
        }

        public byte[] DownloadImageToByteArray(string userId)
        {
            byte[] imageBytes;

            string uniqueBlobName = string.Format($"image_{userId}");
            CloudBlockBlob blob = _container.GetBlockBlobReference(uniqueBlobName);
            if (!blob.Exists())
                return null;
            
            using (MemoryStream ms = new MemoryStream())
            {
                blob.DownloadToStream(ms);
                imageBytes = ms.ToArray();
            }

            return imageBytes;
        }
    }
}
