using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AZ_Cloud
{
    public partial class Form4 : Form
    {
        public string AccountName;
        public string AccountKey;
        public string ContainerSelectedMetaData;
        public string BlobName;
        public Form4(string accname, string acckey, string container, string blob)
        {
            AccountName = accname;
            AccountKey = acckey;
            ContainerSelectedMetaData = container;
            BlobName = blob;
            InitializeComponent();
        }

        private void Form4_Load(object sender, EventArgs e)
        {

            this.textBox1.BackColor = Color.Beige;// "#f9f9d2";//Color.Azure;
            this.textBox2.BackColor = Color.Beige;
            this.textBox3.BackColor = Color.Beige;
            this.textBox4.BackColor = Color.Beige;
            this.textBox5.BackColor = Color.Beige;
            this.textBox6.BackColor = Color.Beige;
            this.textBox7.BackColor = Color.Beige;
            this.textBox8.BackColor = Color.Beige;
            this.textBox9.BackColor = Color.Beige;
            this.textBox10.BackColor = Color.Beige;
            GetBlobMetaProperties();
           // this.textBox1.Enabled = false;
        }

        private void GetBlobMetaProperties()
        {
            List<string> BlobDetail = new List<string>();
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=http;AccountName=" + AccountName + ";AccountKey=" + AccountKey + "");

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer container = blobClient.GetContainerReference(ContainerSelectedMetaData);

            foreach (IListBlobItem item in container.ListBlobs(null, false))
            {
                
                if (item.GetType() == typeof(CloudBlockBlob))
                {
                    CloudBlockBlob blob = (CloudBlockBlob)item;
                    if(blob.Name == BlobName)
                    {
                        BlobDetail.Add(blob.Properties.BlobType.ToString());
                        BlobDetail.Add(blob.Name);
                        BlobDetail.Add(blob.Properties.Length.ToString());
                        BlobDetail.Add(blob.Properties.LeaseState.ToString() != null ? blob.Properties.LeaseState.ToString() : "");
                        BlobDetail.Add(blob.Properties.LeaseDuration.ToString() != null ? blob.Properties.LeaseDuration.ToString() : "");
                        BlobDetail.Add((blob.Properties.LastModified.ToString()));
                        BlobDetail.Add(blob.Uri.ToString());
                        BlobDetail.Add(blob.IsSnapshot.ToString() != null ? blob.IsSnapshot.ToString() : "");

                    }
                    
                }
                else if (item.GetType() == typeof(CloudPageBlob))
                {
                    CloudPageBlob pageBlob = (CloudPageBlob)item;
                    if(pageBlob.Name ==  BlobName)
                    {
                        BlobDetail.Add(pageBlob.Properties.BlobType.ToString());
                        BlobDetail.Add(pageBlob.Name);
                        BlobDetail.Add(pageBlob.Properties.Length.ToString());
                        BlobDetail.Add(pageBlob.Properties.LeaseState.ToString() != null ? pageBlob.Properties.LeaseState.ToString() : "");
                        BlobDetail.Add(pageBlob.Properties.LeaseDuration.ToString() != null ? pageBlob.Properties.LeaseDuration.ToString() : "");
                        BlobDetail.Add((pageBlob.Properties.LastModified.ToString()));
                        BlobDetail.Add(pageBlob.Uri.ToString());
                        BlobDetail.Add(pageBlob.IsSnapshot.ToString() != null ? pageBlob.IsSnapshot.ToString() : "");
                    }
                    


                }


            }
          
            textBox1.Text = ContainerSelectedMetaData;
            textBox2.Text =  BlobDetail[0].ToString();
            textBox3.Text = BlobName;
            textBox4.Text = BlobDetail[1].ToString();

            textBox5.Text = BlobDetail[2].ToString();
            textBox7.Text = BlobDetail[3].ToString();
            textBox6.Text = BlobDetail[4].ToString();
            textBox8.Text = BlobDetail[5].ToString();
            textBox9.Text = BlobDetail[6].ToString();
           // textBox10.Text = blob.CopyState.ToString();// != null ? blob.CopyState.ToString() : "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Hide();
        }
    }
}
