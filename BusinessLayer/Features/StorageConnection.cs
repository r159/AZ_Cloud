using BusinessLayer.Interface;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BusinessLayer.Features
{
    public class StorageConnection : IStorageConnection
    {
        CloudStorageAccount cloudStorageAccount;
        CloudBlobClient blobClient;
        CloudBlobContainer blobContainer;
        BlobContainerPermissions containerPermissions;
        CloudBlob blob;
        private const string container = "test";
        private const string filename = "test.txt";
        
        public bool CheckvalidStorageAcc(string acckey, string accname)
        {
            try
            {


               // storagecredentials objcredentials = new storagecredentials();
                cloudStorageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=http;AccountName=" + accname + ";AccountKey=" + acckey + "");
                blobClient = cloudStorageAccount.CreateCloudBlobClient();
                string containername = container + Regex.Replace(DateTime.Now.ToString(), "[^0-9a-zA-Z]+", "").ToLower();
                blobContainer = blobClient.GetContainerReference(containername);
                blobContainer.CreateIfNotExists();
                containerPermissions = new BlobContainerPermissions();
                containerPermissions.PublicAccess = BlobContainerPublicAccessType.Blob;
                blobContainer.SetPermissions(containerPermissions);
                //CloudBlockBlob blob = blobContainer.GetBlockBlobReference(filename);
                //blob.UploadText("Hi blob");
                blobContainer.Delete();
               // return objcredentials;
                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }
    }
    public class storagecredentials
    {
        private string _accname;

        public string AccountName
        {
            get { return _accname; }
            set { _accname = value; }
        }
        private string _acckey;

        public string  AccountKey
        {
            get { return _acckey; }
            set { _acckey = value; }
        }
    }
}
