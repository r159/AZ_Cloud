using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Features
{
    public class ContainerBlobCreateOps
    {
        //check the 

       public string CreateContainer(string containername, bool containerpermission)
        
       {
           try
           {
               CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=http;AccountName=" + BlobOperations.AcountName + ";AccountKey=" + BlobOperations.AccountKey + "");
               // Create the blob client.
               CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
               List<CloudBlobContainer> ContainerExists = blobClient.ListContainers().Where(u => u.Name.Equals(containername)).ToList();
               if (ContainerExists.Count < 1 )
               {
                   if (containerpermission)
                   {
                       CloudBlobContainer container = blobClient.GetContainerReference(containername);
                       //Create the container if it doesn't already exist.
                       container.CreateIfNotExists();
                       return "Success";
                   }
                   else
                   {
                       CloudBlobContainer container = blobClient.GetContainerReference(containername);
                       //Create the container if it doesn't already exist.
                       container.CreateIfNotExists();
                       container.SetPermissions(
                        new BlobContainerPermissions
                        {
                            PublicAccess = BlobContainerPublicAccessType.Blob
                        });
                       return "Success";
                   }
                 
               }
               else
               {
                   return "Container Already Exists, Delete And Create Container Again";
               }
               
           }
           catch
           {
               return "Failed";
           }
        }
     
    }
}
