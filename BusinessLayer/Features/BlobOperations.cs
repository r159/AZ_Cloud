using BusinessLayer.Interface;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Features
{
    public class BlobOperations 
    {
        //public static List<>
        private static string _accname;
        private static string _acckey;
       
        public static string AcountName
        {
            get
            {
                return _accname;
            }
            set
            {
                _accname = value;
            }
        }
        public static string AccountKey
        {
            get
            {
                return _acckey;
            }
            set
            {
                _acckey = value;
            }
        }
      
        public BlobOperations(string accname, string acckey)
        {
            AcountName = accname;
            AccountKey = acckey;
        }
        public static Dictionary<string, string> GetContainerList(string accname, string acckey)
        {
            try
            {
                AcountName = accname;
                AccountKey = acckey;
                Dictionary<string, string> ContainerListString = new Dictionary<string, string>();
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=http;AccountName=" + accname + ";AccountKey=" + acckey + "");

                // Create the blob client.
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                List<CloudBlobContainer> ContainerLists = blobClient.ListContainers().ToList();
               // CloudBlobContainer ContainerLists1 = blobClient.ListContainers().ToList().Single(s => s.Name == "sas");
                foreach (var item in ContainerLists)
                {
                    ContainerListString.Add(item.Name.ToString(), item.GetPermissions().PublicAccess.ToString() == "Off" ? "Private" : "Public");
                }
                return ContainerListString;

            }
            catch(Exception e)
            {
                return null;
            }
        
           

        }
        public static Dictionary<string, List<string>> GetBlobDetails(string Container)
        {
            try
            {
                Dictionary<string, List<string>> Get = new Dictionary<string, List<string>>();
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=http;AccountName=" + AcountName + ";AccountKey=" + AccountKey + "");

                // Create the blob client.
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobClient.GetContainerReference(Container);
                //IListBlobItem ss =  container.ListBlobs
                //foreach(IListBlobItem item in container.ListBlobs())
                // Loop over items within the container and output the length and URI.
                foreach (IListBlobItem item in container.ListBlobs(null, false))
                {
                    if (item.GetType() == typeof(CloudBlockBlob))
                    {
                        CloudBlockBlob blob = (CloudBlockBlob)item;
                        Get.Add(blob.Name, new List<string> { blob.Properties.Length.ToString(), blob.Properties.BlobType.ToString(), blob.Properties.ContentType.ToString(), blob.Properties.LastModified.ToString(), blob.Uri.AbsoluteUri.ToString() });
                    }
                    else if (item.GetType() == typeof(CloudPageBlob))
                    {
                        CloudPageBlob pageBlob = (CloudPageBlob)item;
                        Get.Add(pageBlob.Name, new List<string> { pageBlob.Properties.Length.ToString(), pageBlob.Properties.BlobType.ToString(), pageBlob.Properties.ContentType.ToString(), pageBlob.Properties.LastModified.ToString(), pageBlob.Uri.AbsoluteUri.ToString() });


                    }


                }
                return Get;
            }
            catch(Exception e)
            {
                return null;
            }
            
        }
        public static List<string> GetSingleContainer(String Containername)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=http;AccountName=" + AcountName + ";AccountKey=" + AccountKey + "");
            List<string> gtname = new List<string>();
            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
           // CloudBlobContainer getcontainer = blobClient.ListContainers().ToList().Single(s => s.Name == Containername);
            List<CloudBlobContainer> ggsas = blobClient.ListContainers().Where(u => u.Name.Contains(Containername)).ToList();  //.Any(u => Containername.ToUpper().Contains(u));
            foreach(var item in ggsas)
            {
                gtname.Add(item.Name);
            }
            return gtname;
        }
       //async public static Task ListBlobsSegmentedInFlatListing(string containera)
       // {
       //     CloudBlobContainer container = containera;
       //     //List blobs to the console window, with paging.
       //     Console.WriteLine("List blobs in pages:");

       //     int i = 0;
       //     BlobContinuationToken continuationToken = null;
       //     BlobResultSegment resultSegment = null;

       //     //Call ListBlobsSegmentedAsync and enumerate the result segment returned, while the continuation token is non-null.
       //     //When the continuation token is null, the last page has been returned and execution can exit the loop.
       //     do
       //     {
       //         //This overload allows control of the page size. You can return all remaining results by passing null for the maxResults parameter,
       //         //or by calling a different overload.
       //         resultSegment = await container.ListBlobsSegmentedAsync("", true, BlobListingDetails.All, 10, continuationToken, null, null);
       //         if (resultSegment.Results.Count<IListBlobItem>() > 0) { Console.WriteLine("Page {0}:", ++i); }
       //         foreach (var blobItem in resultSegment.Results)
       //         {
       //             Console.WriteLine("\t{0}", blobItem.StorageUri.PrimaryUri);
       //         }
       //         Console.WriteLine();

       //         //Get the continuation token.
       //         continuationToken = resultSegment.ContinuationToken;
       //     }
       //     while (continuationToken != null);
       // }
       
    }
}
