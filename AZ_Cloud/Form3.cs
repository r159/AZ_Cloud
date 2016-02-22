using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AZ_Cloud
{
    public partial class COPYCONTAINER : Form
    {
        private static string sourcecontainer;
        private static string destcontainer;
        public string AccountName;
        public string AccountKey;
        public static string NewDestinationContainer;
        public bool mCompleted = false;
        public bool mClosePending = false;
        public COPYCONTAINER(string accname, string acckey)
        {
            AccountName = accname;
            AccountKey = acckey;
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            this.panel1.Visible = true;
            backgroundWorker1.WorkerReportsProgress = true;
         
            backgroundWorker1.ProgressChanged += bg1_ProgressChanged;
            backgroundWorker1.DoWork += new DoWorkEventHandler(backgroundWorker1_DoWork);
            this.label3.Visible = false;
            this.progressBar1.Visible = false;
            //LoadContainterSourceCombox();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=http;AccountName=" + AccountName + ";AccountKey=" + AccountKey + "");
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer sourceContainer = blobClient.GetContainerReference(sourcecontainer);
            CloudBlobContainer targetContainer = blobClient.GetContainerReference(NewDestinationContainer);
            CloudBlockBlob sourceBlob;
            CloudBlockBlob targetBlob;
             

            int AllBlobs = sourceContainer.ListBlobs().Count();//sourcecontainer.Count;;
            if(AllBlobs > 1)
            {
                int count = 1;
                foreach (IListBlobItem item in sourceContainer.ListBlobs(null, false))
                {
                    if (item.GetType() == typeof(CloudBlockBlob))
                    {
                        CloudBlockBlob blob = (CloudBlockBlob)item;
                        sourceBlob = sourceContainer.GetBlockBlobReference(blob.Name.ToString());
                        targetBlob = targetContainer.GetBlockBlobReference(blob.Name.ToString());
                        targetBlob.StartCopy(sourceBlob, null, null, null, null);
                    }
                    else if (item.GetType() == typeof(CloudPageBlob))
                    {
                        CloudPageBlob pageBlob = (CloudPageBlob)item;
                        sourceBlob = sourceContainer.GetBlockBlobReference(pageBlob.Name.ToString());
                        targetBlob = targetContainer.GetBlockBlobReference(pageBlob.Name.ToString());
                        targetBlob.StartCopy(sourceBlob, null, null, null, null);
                    }
                    Thread.Sleep(500);
                    backgroundWorker1.ReportProgress((count * 100) / AllBlobs);

                    count = count + 1;


                }
                MessageBox.Show("Blobs Copied.");
                this.Invoke((Action)(() => this.Hide()));
            }
            else
            {
                MessageBox.Show("Source Container Doesnt Contain Any Blobs");
             
            }
         

        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            e.Cancel = true;
            //if (!mCompleted)
            //{
            //    backgroundWorker1.CancelAsync();
            //    this.Enabled = false;
            //    e.Cancel = true;
            //    mClosePending = true;
            //    return;
            //}
           // base.OnFormClosing(e);
            Hide();
           
        }
        private void bg1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
            this.label3.Text = "Copying.. " + " " + e.ProgressPercentage.ToString() + "%";
        }
        public void SetContainerSource(string containername)
        {
            textBox1.Text = containername.ToString();
            sourcecontainer = containername.ToString();
        }
        public void LoadContainterDestinationCombox(List<string> destinationcontainers)
        {
            foreach (var item in destinationcontainers)
             {
                 comboBox1.Items.Add(item);
             }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(sourcecontainer))
            {
                var destinationcontainer=  comboBox1.SelectedItem;
                string[] tempdestinationcontainer = destinationcontainer.ToString().Split(new string[] { "(" }, StringSplitOptions.None);
                NewDestinationContainer = tempdestinationcontainer[0].ToString();
                if(destinationcontainer != null)
                {
                    if (NewDestinationContainer == sourcecontainer)
                    {
                        MessageBox.Show("Source Container Cannot be Same as Destination Container");
                    }
                    else
                    {
                        this.progressBar1.Visible = true;
                        this.label3.Visible = true;
                        backgroundWorker1.RunWorkerAsync();
                       
                    }


                }
                else
                {
                    MessageBox.Show("Please Select Destination Container");
                }
               
            }
            else
            {
                MessageBox.Show("OOPS, Something Went Wrong Please Close the Dialog and Try Again");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.panel1.Visible = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Hide();
        }
    }
}
