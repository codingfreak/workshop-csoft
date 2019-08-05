using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;
using System.Threading.Tasks;

namespace commasoft.Workshop.StorageAccount
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // prepare config system
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            var configuration = builder.Build();
            // init storage account
            var storageAccountConnectionString = configuration["Azure:StorageConnectionString"];
            var account = CloudStorageAccount.Parse(storageAccountConnectionString);
            var blobClient = account.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(configuration["Azure:ContainerName"]);
            // ensure that the container exists
            await container.CreateIfNotExistsAsync();
            //await container.SetPermissionsAsync(new BlobContainerPermissions
            //{
            //    PublicAccess = BlobContainerPublicAccessType.Blob
            //});
            // upload files from local folder
            Console.WriteLine($"Using BLOB container '{container.Name}'");
            var directory = new DirectoryInfo(configuration["SourcePath"]);
            var files = directory.GetFiles();
            Console.WriteLine($"Uploading {files.Length} files...");
            foreach(var file in files)
            {
                var blob = container.GetBlockBlobReference(file.Name);
                Console.Write($"Uploading {file.Name}...");
                await blob.UploadFromFileAsync(file.FullName);
                Console.WriteLine("Done");
            }
            Console.WriteLine("Upload completed.");
            Console.ReadKey();
        }
    }
}
