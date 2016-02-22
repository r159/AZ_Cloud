using BusinessLayer.Features;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AZ_Cloud
{
    public partial class Form2 : Form
    {
        
       
        private static string selectedContainer;
        public string AccountName;
        public string AccountKey;
        List<string> FilePaths = new List<string>();
        List<string> DownloadBlobNames = new List<string>();
        public static string DownloadedBlobPath;
        public static int DownloadFilesCount;
        public static string DownloadblobsContainer;
        private static string _containterselectedupload;
        public static int tempdownloadcount = 1;
        public static string ContainerSelectedUpload
        {
            get
            {
                return _containterselectedupload;
            }
            set
            {
                _containterselectedupload = value;
            }
        }
        private static int _storagetotalcontainer;
        public static int StorageTotalContainer
        {
            get
            {
                return _storagetotalcontainer;
            }
            set{
                _storagetotalcontainer = value;
            }
        }
     //   public string folderPath = @"C:\Ffmeg\newoutput\";
        OrderedDictionary FileNamePath = new OrderedDictionary();
        public List<Task> TaskList = new List<Task>();  
        long temptotallength = 0;
        int tempFilebytenumber = 0;
        private static int _totallength;
        public static int totallength
        {
            get
            {
                return _totallength;
            }
            set
            {
                _totallength = value;
            }
           
        }
        public static int TotalLengthDownload;
        public static int TotalTempDownload=0;
        public Form2(string accname, string acckey)
        {

            AccountName = accname;
            AccountKey = acckey;
            InitializeComponent();
        }
        private void Calculate(int i)
        {
            double pow = Math.Pow(i, i);
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            BlobOperations ObjblobOp = new BlobOperations(AccountName, AccountKey);
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker3.WorkerReportsProgress = true;
            backgroundWorker4.WorkerReportsProgress = true;
            backgroundWorker5.WorkerReportsProgress = true;
            backgroundWorker1.ProgressChanged += bg1_ProgressChanged;
            backgroundWorker1.DoWork += new DoWorkEventHandler(backgroundWorker1_DoWork);
            backgroundWorker2.DoWork += new DoWorkEventHandler(bgworkerloadblobs_DoWork);
            backgroundWorker3.DoWork += new DoWorkEventHandler(bgworkerSearchContainers_Dowork);
            backgroundWorker4.DoWork += new DoWorkEventHandler(bgworkerUploadBlobs_Dowork);
            backgroundWorker5.DoWork += new DoWorkEventHandler(bgworkerdownloadBlobs_Dowork);
            backgroundWorker3.ProgressChanged += bg3_ProgressChanged;
            backgroundWorker4.ProgressChanged += bg4_ProgressChanged;
            backgroundWorker5.ProgressChanged += bg5_ProgressChanged;
           
            LoadContainersCombobox();
          //  this.panel4.Visible = false;
            this.button2.Enabled = false;
            this.Invoke((Action)(() => cHANGEACCOUNTToolStripMenuItem.Enabled = false));
            //hide  the progressbar for search
            this.Invoke((Action)(() => progressBar2.Visible = false));//this.progressBar1.Visible = false;
            this.Invoke((Action)(() => label8.Visible = false));
            //ends here 
            this.Invoke((Action)(() => button12.Enabled = false));
            this.Invoke((Action)(()=> panel6.Visible = false));
            this.Invoke((Action)(() => textBox1.Enabled = false));
            this.Invoke((Action)(() => comboBox2.Enabled = false));

            this.Invoke((Action)(() => button6.Enabled = false));
            this.Invoke((Action)(() => button8.Enabled = false));
            this.Invoke((Action)(() => button12.Enabled = false));
            this.Invoke((Action)(() => dataGridView1.AllowUserToAddRows = false));
            this.Invoke((Action)(() => dataGridView1.ReadOnly = true));
            this.Invoke((Action)(() => button17.Enabled = false));
            this.Invoke((Action)(() => textBox3.Enabled = false));
            this.Invoke((Action)(() => panel1.Enabled = false));

            //added on 5-1-2016
            this.Invoke((Action)(() => progressBar3.Visible = false));
            this.Invoke((Action)(() => label15.Visible = false));
            //ends here
            //added on 6-1-2015
            this.Invoke((Action)(() => progressBar4.Visible = false));
            this.Invoke((Action)(() => label16.Visible = false));
            //ends here 
            
        }

        private void bg5_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar4.Value = e.ProgressPercentage;
            // Set the text.
            this.label16.Text = "Downloading.... " + " " + e.ProgressPercentage.ToString() + "%";
        }

        private void bgworkerdownloadBlobs_Dowork(object sender, DoWorkEventArgs e)
        {
            this.Invoke((Action)(() => progressBar4.Visible = true));
            this.Invoke((Action)(() => label16.Visible = true));
            this.Invoke((Action)(() => textBox3.Text = ""));
            DownloadingBlob();
            this.Invoke((Action)(() => button4.Enabled = true));
         
            this.Invoke((Action)(() => button14.Enabled = true));
            this.Invoke((Action)(() => progressBar4.Value = 0));
            this.Invoke((Action)(() => label16.Text = "Downloading..."));
            this.Invoke((Action)(() => progressBar4.Visible = false));
            this.Invoke((Action)(() => label16.Visible = false));
        }
        private void DownloadingBlob()
        {
            Task[] tasksdownload = new Task[10];
            int downloadcount = DownloadFilesCount;
            if (downloadcount <= 10)
            {
                for (int taskNumber = 1; taskNumber <= downloadcount; )
                {
                    string filename = DownloadBlobNames[taskNumber - 1].ToString();
                    tasksdownload[taskNumber - 1] = Task.Factory.StartNew(() => DownloadFileToContainer(filename, DownloadedBlobPath));//(filename, filepath));
                    TaskList.Add(tasksdownload[taskNumber - 1]);
                    taskNumber = taskNumber + 1;

                }
                DownloadBlobNames.Clear();
            }
            else
            {
                int taskNumber = 0;
                int tasklimit = 10;
                int startTask = 1;
                int curretiteration = 0;
                int addnumber = 0;
                int adnumber1 = 1;
                int filestemp = 0;
                float noofiteration = (DownloadFilesCount / 10F);
                double noofiterations = Math.Ceiling(noofiteration);
                while (curretiteration < noofiterations)
                {

                    if (noofiterations > 1 && curretiteration != 0)
                    {
                        int tempcurretiteration = curretiteration + 1;
                        //int tempcurr2 = curretiteration + 2;
                        tasklimit = int.Parse(tempcurretiteration.ToString() + addnumber.ToString());  //curretiteration + Int16.Parse("0");
                        tasklimit = tasklimit > DownloadFilesCount ? DownloadFilesCount : tasklimit;
                        tasklimit = tasklimit - 10;
                        tasksdownload = new Task[tasklimit];
                        startTask = int.Parse(curretiteration.ToString() + adnumber1.ToString());
                        startTask = startTask > tasklimit ? (startTask - 10) : startTask;

                    }

                    for (taskNumber = startTask; taskNumber <= tasklimit; )
                    {
                        string filename = DownloadBlobNames[filestemp].ToString();

                        tasksdownload[taskNumber - 1] = Task.Factory.StartNew(() => DownloadFileToContainer(filename, DownloadedBlobPath));
                        TaskList.Add(tasksdownload[taskNumber - 1]);
                        taskNumber = taskNumber + 1;
                        filestemp = filestemp + 1;
                    }

                    curretiteration = curretiteration + 1;



                }

            }
            Task.WaitAll(TaskList.ToArray());
        }

        private void DownloadFileToContainer(string filename, string DownloadedBlobPath)
        {
            try
            {
               // MessageBox.Show("Start: "+TotalTempDownload.ToString());
                
                //CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=http;AccountName=" + AccountName + ";AccountKey=" + AccountKey + "");

                //// Create the blob client.
                //CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

                //CloudBlobContainer container = blobClient.GetContainerReference(DownloadblobsContainer);
                //CloudBlockBlob blob = container.GetBlockBlobReference(filename);

                //int blockSize = 1024 * 1024; //256 kb
                //string downloadfileloc = DownloadedBlobPath + "\\" + filename;
                //using (FileStream fileStream = new FileStream(downloadfileloc, FileMode.OpenOrCreate))
                //{
                //    long fileSize = fileStream.Length;

                //    //block count is the number of blocks + 1 for the last one
                //    int blockCount = (int)((float)fileSize / (float)blockSize) + 1;


                //    //starting block number - 1
                //    int blockNumber = 0;

                //    try
                //    {
                //        int bytesRead = 0; //number of bytes read so far
                //        long bytesLeft = fileSize; //number of bytes left to read and upload

                //        //do until all of the bytes are uploaded
                //        while (bytesLeft > 0)
                //        {
                //            blockNumber++;
                //            tempFilebytenumber = tempFilebytenumber + 1;
                //            int bytesToRead;
                //            if (bytesLeft >= blockSize)
                //            {
                //                //more than one block left, so put up another whole block
                //                bytesToRead = blockSize;
                //            }
                //            else
                //            {
                //                //less than one block left, read the rest of it
                //                bytesToRead = (int)bytesLeft;
                //            }
                //            byte[] bytes = new byte[bytesToRead];
                //            using (MemoryStream ms = new MemoryStream())
                //            {
                //                blob.DownloadRangeToStream(ms, bytesRead, blockSize);
                //                ms.Position = 0;
                //                ms.Read(bytes, 0, bytes.Length);
                //                using (FileStream fs = new FileStream("", FileMode.OpenOrCreate))
                //                {
                //                    fs.Position = bytesRead;
                //                    fs.Write(bytes, 0, bytes.Length);
                //                }
                //            }
                //            //create a blockID from the block number, add it to the block ID list
                //            //the block ID is a base64 string

                //            //set up new buffer with the right size, and read that many bytes into it 


                //            //increment/decrement counters
                //            bytesRead += bytesToRead;
                //            bytesLeft -= bytesToRead;
                //            int tempProgressbarvalue = (tempFilebytenumber * 100) / totallength;// (tempFilebytenumber1 * 100) / totallength;
                //            //  MessageBox.Show(tempProgressbarvalue.ToString());
                //            //  MessageBox.Show(totallength.ToString());
                //            if (tempProgressbarvalue <= 100)
                //                backgroundWorker4.ReportProgress((tempFilebytenumber * 100) / totallength);
                //        }



                //    }
                //}













                var cloudStorageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=http;AccountName=" + AccountName + ";AccountKey=" + AccountKey + "");
                var containerName = DownloadblobsContainer;
                var blobName = filename;
                int segmentSize = 1 * 1024 * 1024;//1 MB chunk
                var blobContainer = cloudStorageAccount.CreateCloudBlobClient().GetContainerReference(containerName);
                var blob = blobContainer.GetBlockBlobReference(blobName);
                blob.FetchAttributes();
                var blobLengthRemaining = blob.Properties.Length;
                string downloadfileloc = DownloadedBlobPath + "\\" + filename;
                long startPosition = 0;
                int sizeofblob = 0;
                string saveFileName = downloadfileloc;

                while (blobLengthRemaining > 0) 
                {
                    long blockSize = Math.Min(segmentSize, blobLengthRemaining);
                    // int tempsize = unchecked((int)blockSize);
                    // sizeofblob = sizeofblob + tempsize;
                    //       int currentblockcount = sizeofblob / segmentSize;
                    TotalTempDownload = TotalTempDownload + 1;
                    byte[] blobContents = new byte[blockSize];
                    using (MemoryStream ms = new MemoryStream())
                    {
                        blob.DownloadRangeToStream(ms, startPosition, blockSize);
                        ms.Position = 0;
                        ms.Read(blobContents, 0, blobContents.Length);
                        using (FileStream fs = new FileStream(saveFileName, FileMode.OpenOrCreate))
                        {
                            fs.Position = startPosition;
                            fs.Write(blobContents, 0, blobContents.Length);
                        }
                    }
                    startPosition += blockSize;
                    blobLengthRemaining -= blockSize;
                    int tempProgressbarvalue = (TotalTempDownload * 100) / TotalLengthDownload;
                   // MessageBox.Show("TotalTempDownload" + TotalTempDownload);
                 //   MessageBox.Show("TotalLengthDownload" + TotalLengthDownload);
                   // MessageBox.Show(tempProgressbarvalue.ToString());
                    // if (tempProgressbarvalue <= 100)
               //     MessageBox.Show(tempProgressbarvalue.ToString());
                        backgroundWorker5.ReportProgress((TotalTempDownload * 100) / TotalLengthDownload);
                }
                








                //CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=http;AccountName=" + AccountName + ";AccountKey=" + AccountKey + "");
                //// Create the blob client.
                //CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

                //// Retrieve reference to a previously created container.
                //CloudBlobContainer container = blobClient.GetContainerReference(DownloadblobsContainer);

                //// Retrieve reference to a blob named "photo1.jpg".
                //CloudBlockBlob blockBlob = container.GetBlockBlobReference(filename);

                //// Save blob contents to a file.
                //string downloadfileloc = DownloadedBlobPath + "\\" + filename;
                ////MessageBox.Show(downloadfileloc);
                //using (var fileStream = System.IO.File.OpenWrite(downloadfileloc))
                //{
                //    blockBlob.DownloadToStream(fileStream);
                //}
                //if (progressBar4.Value <= 100)
                //    backgroundWorker5.ReportProgress((tempdownloadcount * 100) / DownloadFilesCount);
                //tempdownloadcount = tempdownloadcount + 1;
            }
            catch(Exception ex)
            {
             //   backgroundWorker5.CancelAsync();
                MessageBox.Show("Ooops...Something Went Wrong While Downloading..");
            }
            
        
        }
        private void UploadingBlob()
        {
            Task[] tasks = new Task[10];
            int filescount = FileNamePath.Count;
            object[] keys = new object[FileNamePath.Keys.Count];
            FileNamePath.Keys.CopyTo(keys, 0);
            if (filescount <= 10)
            {
                for (int taskNumber = 1; taskNumber <= filescount; )
                {
                    string filename = keys[taskNumber - 1].ToString();
                    string filepath = FileNamePath[taskNumber - 1].ToString();
                    tasks[taskNumber - 1] = Task.Factory.StartNew(() => UploadFileToContainer(filename, filepath));//(filename, filepath));
                    TaskList.Add(tasks[taskNumber - 1]);
                    taskNumber = taskNumber + 1;

                }
            
                
                FileNamePath.Clear();

            }
            else
            {
                //Task[] tasks = new Task[10];
                //int filescount = FileNamePath.Count;
                //object[] keys = new object[FileNamePath.Keys.Count];
                //FileNamePath.Keys.CopyTo(keys, 0);
                //Starts here
                int taskNumber = 0;
                int tasklimit = 10;
                int startTask = 1;
                int curretiteration = 0;
                int addnumber = 0;
                int adnumber1 = 1;
                int filestemp = 0;
                float noofiteration = (filescount / 10F);
                double noofiterations = Math.Ceiling(noofiteration);
                while (curretiteration < noofiterations)
                {

                    if (noofiterations > 1 && curretiteration != 0)
                    {
                        int tempcurretiteration = curretiteration + 1;
                        //int tempcurr2 = curretiteration + 2;
                        tasklimit = int.Parse(tempcurretiteration.ToString() + addnumber.ToString());  //curretiteration + Int16.Parse("0");
                        tasklimit = tasklimit > filescount ? filescount : tasklimit;
                        tasklimit = tasklimit - 10;
                        tasks = new Task[tasklimit];
                        startTask = int.Parse(curretiteration.ToString() + adnumber1.ToString());
                        startTask = startTask > tasklimit ? (startTask - 10) : startTask;

                    }

                    for (taskNumber = startTask; taskNumber <= tasklimit; )
                    {
                        string filename = keys[filestemp].ToString();
                        string filepath = FileNamePath[filestemp].ToString();
                        tasks[taskNumber - 1] = Task.Factory.StartNew(() => UploadFileToContainer(filename, filepath));
                        TaskList.Add(tasks[taskNumber - 1]);
                        taskNumber = taskNumber + 1;
                        filestemp = filestemp + 1;
                    }
                
                    curretiteration = curretiteration + 1;
                   
                    
                   
                }
                FileNamePath.Clear();
              
            }
            Task.WaitAll(TaskList.ToArray());
        }
        private void bgworkerUploadBlobs_Dowork(object sender, DoWorkEventArgs e)
        {
            //added on 5-1-2016 to show the progressbar and value
            this.Invoke((Action)(() => progressBar3.Visible = true));
            this.Invoke((Action)(() => label15.Visible = true));
            //ends here
            UploadingBlob();
            ////load the new blobs if the gridview is loaded
            //if(dataGridView1.Rows.Count > 0 )
            //{

            //}
            ////ends here
            this.Invoke((Action)(() => button3.Enabled = true));
            this.Invoke((Action)(() => label3.Text = "Uploading.. "));
            this.Invoke((Action)(() => progressBar3.Value = 0));
            this.Invoke((Action)(() => panel9.Enabled = true));
         //   this.button3.Enabled = true;
        }

        private void UploadFileToContainer(string filename, string filepath)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=http;AccountName=" + AccountName + ";AccountKey=" + AccountKey + "");

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer container = blobClient.GetContainerReference(ContainerSelectedUpload);
            CloudBlockBlob blob = container.GetBlockBlobReference(Path.GetFileName(filepath));

            int blockSize = 1024 * 1024; //256 kb

            using (FileStream fileStream =
              new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                long fileSize = fileStream.Length;

                //block count is the number of blocks + 1 for the last one
                int blockCount = (int)((float)fileSize / (float)blockSize) + 1;

                //List of block ids; the blocks will be committed in the order of this list 
                List<string> blockIDs = new List<string>();

                //starting block number - 1
                int blockNumber = 0;

                try
                {
                    int bytesRead = 0; //number of bytes read so far
                    long bytesLeft = fileSize; //number of bytes left to read and upload

                    //do until all of the bytes are uploaded
                    while (bytesLeft > 0)
                    {
                        blockNumber++;
                        tempFilebytenumber = tempFilebytenumber + 1;
                        int bytesToRead;
                        if (bytesLeft >= blockSize)
                        {
                            //more than one block left, so put up another whole block
                            bytesToRead = blockSize;
                        }
                        else
                        {
                            //less than one block left, read the rest of it
                            bytesToRead = (int)bytesLeft;
                        }

                        //create a blockID from the block number, add it to the block ID list
                        //the block ID is a base64 string
                        string blockId =
                          Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(string.Format("BlockId{0}",
                            blockNumber.ToString("0000000"))));
                        blockIDs.Add(blockId);
                        //set up new buffer with the right size, and read that many bytes into it 
                        byte[] bytes = new byte[bytesToRead];
                        fileStream.Read(bytes, 0, bytesToRead);

                        //calculate the MD5 hash of the byte array
                        string blockHash = GetMD5HashFromStream(bytes);

                        //upload the block, provide the hash so Azure can verify it
                        blob.PutBlock(blockId, new MemoryStream(bytes), blockHash);

                        //increment/decrement counters
                        bytesRead += bytesToRead;
                        bytesLeft -= bytesToRead;
                        int tempProgressbarvalue = (tempFilebytenumber * 100) / totallength;// (tempFilebytenumber1 * 100) / totallength;
                        //  MessageBox.Show(tempProgressbarvalue.ToString());
                      //  MessageBox.Show(totallength.ToString());
                        if (tempProgressbarvalue <= 100)
                            backgroundWorker4.ReportProgress((tempFilebytenumber * 100) / totallength);
                    }

                    //commit the blocks
                    blob.PutBlockList(blockIDs);
                   
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Something went wrong While Uploading... Please try again");
                }
            }
               
            
          
           // return null;
        }
        private string GetMD5HashFromStream(byte[] data)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] blockHash = md5.ComputeHash(data);
            return Convert.ToBase64String(blockHash, 0, 16);
        }
   


       private void HideAndDisableControls()
        {
            this.Invoke((Action)(() => panel6.Visible = false));
            this.Invoke((Action)(() => textBox2.Text = ""));  //text2.Visible = false));
            this.Invoke((Action)(() => radioButton1.Checked = false));//   textBox2.Text = "")); 
            this.Invoke((Action)(() => radioButton2.Checked = false));
            this.Invoke((Action)(() => textBox3.Text = ""));
        }

        

        private void LoadContainersCombobox()
        {
            dataGridView1.Visible = false;
            dataGridView1.DataSource = null;
          
         
            backgroundWorker1.RunWorkerAsync();

        }
       private int DropDownWidth(ComboBox myCombo)
        {
            int maxWidth = 0, temp = 0;
            foreach (var obj in myCombo.Items)
            {
                temp = TextRenderer.MeasureText(obj.ToString(), myCombo.Font).Width;
                if (temp > maxWidth)
                {
                    maxWidth = temp +10;
                }
            }
            return maxWidth;
        }
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region newCode
                this.Invoke((Action)(() => button2.Enabled = false));
                this.Invoke((Action)(() => comboBox1.Items.Clear()));
                this.Invoke((Action)(() => label3.Text = "Loading.. "));
                this.Invoke((Action)(() => progressBar1.Value = 0));
                this.Invoke((Action)(() => label11.Text = ":"));
                int count = 1;
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=http;AccountName=" + AccountName + ";AccountKey=" + AccountKey + "");

                // Create the blob client.
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                List<CloudBlobContainer> ContainerLists = blobClient.ListContainers().ToList();
            
                foreach (var item in ContainerLists)
                {
                    string tempcname = item.Name.ToString();
                    string tempcperm = item.GetPermissions().PublicAccess.ToString() == "Off" ? "Private" : "Public";
                    string tempcontainer = tempcname + "(" + tempcperm + ")";
                    this.Invoke((Action)(() => comboBox1.Items.Add(tempcontainer)));
                    backgroundWorker1.ReportProgress((count * 100) / ContainerLists.Count);

                    count = count + 1;
                 //   Thread.Sleep(45);
                    //ContainerListString.Add(item.Name.ToString(), item.GetPermissions().PublicAccess.ToString() == "Off" ? "Private" : "Public");
                }
                //Dictionary<string, string> Containers = BlobOperations.GetContainerList(AccountName, AccountKey);
                //int count = 1;
                //if (Containers.Count > 0)
                //{
                //    this.Invoke((Action)(() => label11.Text = ":" + " " + Containers.Count.ToString()));
                //    foreach (var item in Containers)
                //    {

                //        this.Invoke((Action)(() => comboBox1.Items.Add(item.Key + "(" + item.Value + ")")));
                //        backgroundWorker1.ReportProgress((count * 100) / Containers.Count);

                //        count = count + 1;
                //        Thread.Sleep(45);

                //    }
                this.Invoke((Action)(() => panel1.Enabled = true));
                this.Invoke((Action)(() => progressBar1.Visible = false));//this.progressBar1.Visible = false;
                this.Invoke((Action)(() => label3.Visible = false));
                this.Invoke((Action)(() => button2.Enabled = true));
                this.Invoke((Action)(() => cHANGEACCOUNTToolStripMenuItem.Enabled = true));
                this.Invoke((Action)(() => button2.Enabled = true));
                this.Invoke((Action)(() => label11.Text = ":" + " " + ContainerLists.Count.ToString()));
                 this.Invoke((Action)(() => StorageTotalContainer =ContainerLists.Count));
                //this.Invoke  
                #endregion

                #region OldCode
                //this.Invoke((Action)(() => button2.Enabled = false));
                //this.Invoke((Action)(() => comboBox1.Items.Clear()));
                //this.Invoke((Action)(() => label3.Text = "Loading.. "));
                //this.Invoke((Action)(() => progressBar1.Value = 0));
                //this.Invoke((Action)(() => label11.Text = ":"));
                //Dictionary<string, string> Containers = BlobOperations.GetContainerList(AccountName, AccountKey);
                //int count = 1;
                //if (Containers.Count > 0)
                //{
                //    this.Invoke((Action)(() => label11.Text = ":" + " " + Containers.Count.ToString()));
                //    foreach (var item in Containers)
                //    {

                //        this.Invoke((Action)(() => comboBox1.Items.Add(item.Key + "(" + item.Value + ")")));
                //        backgroundWorker1.ReportProgress((count * 100) / Containers.Count);

                //        count = count + 1;
                //        Thread.Sleep(45);

                //    }
                //    this.Invoke((Action)(() => panel1.Enabled = true));
                //    this.Invoke((Action)(() => progressBar1.Visible = false));//this.progressBar1.Visible = false;
                //    this.Invoke((Action)(() => label3.Visible = false));
                //    this.Invoke((Action)(() => button2.Enabled = true));
                //    this.Invoke((Action)(() => cHANGEACCOUNTToolStripMenuItem.Enabled = true));
                //    this.Invoke((Action)(() => button2.Enabled = true));
             
               
                //    //       this.Invoke((Action)(() => comboBox1.Width =  DropDownWidth(comboBox1)));// comboBox1.AutoScrollOffset = true));  // = DropDownWidth(comboBox1)));//
                //    //     this.Invoke((Action)(() => panel8.Width = DropDownWidth(comboBox1)));
                //}
                #endregion
            }
            catch(Exception ex)
            {
                MessageBox.Show("Something Went Wrong");
            }
          
            
          
           
        }

        private void bg1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // Change the value of the ProgressBar to the BackgroundWorker progress.
            progressBar1.Value = e.ProgressPercentage;
            // Set the text.
            this.label3.Text = "Loading.. " + " " + e.ProgressPercentage.ToString() + "%";
        }
        private void bg3_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar2.Value = e.ProgressPercentage;
            // Set the text.
            this.label8.Text = "Searching.. " + " " + e.ProgressPercentage.ToString() + "%";
        }
        private void bg4_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar3.Value = e.ProgressPercentage;
            // Set the text.
            this.label15.Text = "Uploading.. " + " " + e.ProgressPercentage.ToString() + "%";
        }
        static readonly string[] SizeSuffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
        static string SizeSuffix(Int64 value)
        {
            if (value < 0) { return "-" + SizeSuffix(-value); }
            if (value == 0) { return "0.0 bytes"; }

            int mag = (int)Math.Log(value, 1024);
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            return string.Format("{0:n1} {1}", adjustedSize, SizeSuffixes[mag]);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            HideAndDisableControls();
            this.Invoke((Action)(() => label2.Text = ":"));
            this.progressBar1.Visible = true;
            this.label3.Visible = true;
            LoadContainersCombobox();
        }
        private void bgworkerloadblobs_DoWork(object sender, DoWorkEventArgs e)
        {
            GenerateDataGrid(selectedContainer);
        }
        private void GenerateDataGrid(string containername)
        {
            Dictionary<string, List<string>> getsads = BlobOperations.GetBlobDetails(containername);
            if (getsads.Count > 0)
            {
                //dataGridView1.Rows.Clear();
              // this.Invoke((Action)(()=> dataGridView1.DataSource = null));
                this.Invoke((Action)(() => dataGridView1.Visible = true));
                this.Invoke((Action)(() => label2.Text = ":" + getsads.Count.ToString()));

                //comboBox1.Items.Add(item.Key)));
                //label2.Text = ":" + getsads.Count.ToString();
                DataTable dt = new DataTable();
     
                dt.Columns.Add("Blob Name");
                dt.Columns.Add("Blob Length");
                dt.Columns.Add("Blob Type");
                dt.Columns.Add("Blob ContentType");
                dt.Columns.Add("Blob LastModified");
                dt.Columns.Add("Blob AbsoluteUri");


                DataRow row = null;
                
                //for (int i = 0; i < getsads.Count; i++)
                //{
                    foreach (var item1 in getsads)
                    {
                        row = dt.NewRow();
                        row["Blob Name"] = item1.Key;
                        row["Blob Length"] = SizeSuffix(long.Parse(item1.Value[0].ToString())); //item1.Value[0].ToString();
                        row["Blob Type"] = item1.Value[1].ToString();
                        row["Blob ContentType"] = item1.Value[2].ToString();
                        row["Blob LastModified"] = item1.Value[3].ToString();
                        row["Blob AbsoluteUri"] = item1.Value[4].ToString();
                        dt.Rows.Add(row);
                    }

               // }
                this.Invoke((Action)(() => dataGridView1.DataSource = dt));

                this.Invoke((Action)(() => dataGridView1.Columns[0].Width = 120));
                this.Invoke((Action)(() => dataGridView1.Columns[1].Width = 100));
                this.Invoke((Action)(() => dataGridView1.Columns[2].Width = 85));
                this.Invoke((Action)(() => dataGridView1.Columns[3].Width = 115));
                this.Invoke((Action)(() => dataGridView1.Columns[4].Width = 115));
                this.Invoke((Action)(() => dataGridView1.Columns[5].Width = dataGridView1.Width - dataGridView1.Columns[0].Width - dataGridView1.Columns[2].Width - dataGridView1.Columns[3].Width - dataGridView1.Columns[4].Width - 5));
                this.Invoke((Action)(() => button17.Enabled = true));
                this.Invoke((Action)(() => textBox3.Enabled = true));
            }
            else
            {

                this.Invoke((Action)(() => label2.Text = ": No Blobs Exists"));
                this.Invoke((Action)(() => dataGridView1.DataSource = null));

            }


        }

        private void button1_Click(object sender, EventArgs e)
        {
            HideAndDisableControls();
            var item = comboBox1.SelectedItem;
            if (item != null)
            {
                //  GenerateDataGrid(item.ToString());
               // dataGridView1.AllowUserToAddRows = false;
                dataGridView1.RowHeadersVisible = false;
                string[] tempcontainer = item.ToString().Split(new string[] { "(" }, StringSplitOptions.None);
                selectedContainer = tempcontainer[0].ToString();//item.ToString();
                //     bg3.ProgressChanged += bg3_ProgressChanged;
                backgroundWorker2.RunWorkerAsync();

            }
            else
            {
                MessageBox.Show("Please Select a Container");
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            HideAndDisableControls();
         //  string containerExists =  
            string searchitem = textBox1.Text;
            if(searchitem != null && searchitem != "")
            {
                backgroundWorker3.RunWorkerAsync();
            }
            else
            {
                MessageBox.Show("Please enter the container name to search");
            }
            
          // this.comboBox2.Items.Add(containerExists);
        }
        private void bgworkerSearchContainers_Dowork(object sender, DoWorkEventArgs e)
        {
            this.Invoke((Action)(() => progressBar2.Visible = true));//this.progressBar1.Visible = false;
            this.Invoke((Action)(() => label8.Visible = true));
            this.Invoke((Action)(() => comboBox2.Items.Clear()));
            int count = 1;
            List<string> searchContainername = BlobOperations.GetSingleContainer(textBox1.Text);
            if(searchContainername.Count > 0)
            {
                foreach (var SEContainer in searchContainername)
                {
                    this.Invoke((Action)(() => comboBox2.Items.Add(SEContainer)));
                    backgroundWorker3.ReportProgress((count * 100) / searchContainername.Count);
                    count = count + 1;
                   // Thread.Sleep(250);
                }
                this.Invoke((Action)(() => progressBar2.Visible = false));//this.progressBar1.Visible = false;
                this.Invoke((Action)(() => label8.Visible = false));
              //  this.Invoke((Action)(() => button1.Enabled = true));
                this.Invoke((Action)(() => button12.Enabled = true));
            }
            else
            {
                this.Invoke((Action)(() => progressBar2.Visible = false));
                this.Invoke((Action)(() => label8.Visible = false));
             
                MessageBox.Show("No Container Found with the Specified Name");
            }
         
        }
        private void button7_Click(object sender, EventArgs e)
        {
            HideAndDisableControls();
            this.comboBox1.Enabled = false;
            this.button1.Enabled = false;
            this.button2.Enabled = false;
            this.Invoke((Action)(() => textBox1.Enabled = true));
            this.Invoke((Action)(() => comboBox2.Enabled = true));
            //this.Invoke((Action)(()=> panel1.Enabled =))
            this.Invoke((Action)(() => button6.Enabled = true));
            this.Invoke((Action)(() => button8.Enabled = true));
            this.Invoke((Action)(() => button12.Enabled = true));
           // this.button7.Visible = false;
         //   this.panel4.Visible = true;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            HideAndDisableControls();
            this.comboBox2.Items.Clear();
            this.Invoke((Action)(() => dataGridView1.DataSource = null));
            this.Invoke((Action)(() => label2.Text = ""));
            //this.comboBox2.SelectedIndex = -1;
            this.comboBox2.Text = "";
            this.textBox1.Text = "";
         //   this.panel4.Visible = false;
            this.comboBox1.Enabled = true;
            this.button1.Enabled = true;
            this.button2.Enabled = true;
            this.button7.Visible = true;
            this.button12.Enabled = false;

            this.Invoke((Action)(() => textBox1.Enabled = false));
            this.Invoke((Action)(() => comboBox2.Enabled = false));

            this.Invoke((Action)(() => button6.Enabled = false));
            this.Invoke((Action)(() => button8.Enabled = false));
            this.Invoke((Action)(() => button12.Enabled = false));
        }

        private void cHANGEACCOUNTToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Form2 frmcurrent = new Form2(null, null);
            ////frmcurrent.Hide();
            //frmcurrent.Close();
            this.Close();
          //  Form2.
            Form1 frmprevious = new Form1();
            frmprevious.Show();
          

        }

        private void button12_Click(object sender, EventArgs e)
        {
            HideAndDisableControls();
            var item = comboBox2.SelectedItem;
            if (item != null)
            {
                //  GenerateDataGrid(item.ToString());

                selectedContainer = item.ToString();
                //     bg3.ProgressChanged += bg3_ProgressChanged;
                backgroundWorker2.RunWorkerAsync();

            }
            else
            {
                MessageBox.Show("Please Select a Container");
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            this.Invoke((Action)(() => panel6.Visible = true));
            this.Invoke((Action)(() => textBox3.Text = ""));
        }

        private void button16_Click(object sender, EventArgs e)
        {
            this.Invoke((Action)(() => panel6.Visible = false));
            this.Invoke((Action)(() => textBox2.Text = ""));  //text2.Visible = false));
            this.Invoke((Action)(() => radioButton1.Checked = false));//   textBox2.Text = "")); 
            this.Invoke((Action)(() => radioButton2.Checked = false));
        }

        private void button11_Click_1(object sender, EventArgs e)
        {
            this.Invoke((Action)(() => textBox3.Text = ""));
            string tempnewcontainer = textBox2.Text;
            if((!string.IsNullOrEmpty(tempnewcontainer)) && (tempnewcontainer.Length > 2)  &&(radioButton1.Checked == true || radioButton2.Checked == true) && (tempnewcontainer.Any(c=>char.IsUpper(c)) == false) && ((char.IsLetter(tempnewcontainer[0]) == false) || (char.IsDigit(tempnewcontainer[0]) == false))&& (tempnewcontainer.Length <64))
            {
                string pattern = @"^[a-zA-Z0-9]+(-[a-zA-Z0-9]+)*$";
                Match match = Regex.Match(tempnewcontainer, pattern);
                if(match.Success)
                {
                    bool tempcontainerperm;
                    if(radioButton1.Checked)
                    {
                        tempcontainerperm = true;
                    }
                    else
                    {
                        tempcontainerperm = false;
                    }
                    ContainerBlobCreateOps ObjConCreate = new ContainerBlobCreateOps();
                    string  ContainerStatus = ObjConCreate.CreateContainer(tempnewcontainer, tempcontainerperm);
                    if (ContainerStatus == "Success")
                    {
                        //Add the container in the combobox 
                        string temp_permission =  tempcontainerperm == true ? "Private" : "Public";
                        this.Invoke((Action)(() => comboBox1.Items.Add(tempnewcontainer + "(" + temp_permission + ")")));
                        StorageTotalContainer = StorageTotalContainer + 1;
                        this.Invoke((Action)(() => label11.Text = ":" + " " + StorageTotalContainer.ToString()));
                        //ends here 
                        MessageBox.Show(ContainerStatus +",Container Created");
                    }
                    else
                    {
                        MessageBox.Show(ContainerStatus);
                    }
                }
                else
                {
                    MessageBox.Show("Container name Invalid, Violates 2th Rule! Click on help for info");
                }
               // bool hasuppercase = 
            }
            else
            {
                MessageBox.Show("Please Enter Valid Container name and Select permission for Container");
            }
        }

        private void label14_Click(object sender, EventArgs e)
        {
            var containermessage = new StringBuilder();
            containermessage.AppendLine("1.Container names must start with a letter or number, and can contain only letters, numbers, and the dash (-) character.");
            containermessage.AppendLine("2.Every dash (-) character must be immediately preceded and followed by a letter or number; consecutive dashes are not permitted in container names");
            containermessage.AppendLine("3.All letters in a container name must be lowercase.");
            containermessage.AppendLine("4.Container names must be from 3 through 63 characters long.");
            MessageBox.Show(containermessage.ToString());
        }
        //bool mCompleted;
        //bool mClosePending;
        //protected override void OnFormClosing(FormClosingEventArgs e)
        //{
            
            
        //    if (!mCompleted)
        //    {
        //        backgroundWorker1.CancelAsync();
        //        this.Enabled = false;
        //        e.Cancel = true;
        //        mClosePending = true;
        //        return;
        //    }
        //    base.OnFormClosing(e);
        //}
        //void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        //{
        //    mCompleted = true;
        //    if (mClosePending) this.Close();
        //}
        private void button17_Click(object sender, EventArgs e)
        {
            if(dataGridView1.Rows.Count > 0 )
            {
                string searchValue = textBox3.Text;
                foreach (System.Windows.Forms.DataGridViewRow r in dataGridView1.Rows)
                {
                    if ((r.Cells[0].Value).ToString().ToUpper().Contains(searchValue.ToUpper()))
                    {
                        dataGridView1.Rows[r.Index].Visible = true;
                        dataGridView1.Rows[0].Selected = true;
                    }
                    else
                    {
                        dataGridView1.CurrentCell = null;
                        dataGridView1.Rows[r.Index].Visible = false;
                    }
                }
              //  (dataGridView1.DataSource as DataTable).DefaultView.RowFilter = string.Format("Field = '{Blob Name}'", textBox3.Text);
               //dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
               //BindingSource source1 = new BindingSource();
                //source1.Filter = string.Format("{0} = '{1}'", "Blob Name", searchValue);
                //try
                //{
                //    //BindingSource source1 = new BindingSource();
                //    //source1.DataSource = dataGridView1;
                //    foreach (DataGridViewRow row in dataGridView1.Rows)
                //    {
                //        if (row.Cells[0].Value.ToString().Equals(searchValue))
                //        {
                //            row.Selected = true;
                //            row.Visible = true;
                //            break;
                //        }
                //    }
                //}
                //catch (Exception exc)
                //{
                //    MessageBox.Show(exc.Message);
                //}
            }
            else
            {
                MessageBox.Show("No Blobs Exists for the Container");
            }
        }
        private void HideWhileUploadingBlobs()
        {
            //button11.Enabled = false;
            //button10.Enabled = false;
            //button9.Enabled = false;
            //button15.Enabled = false;
            panel5.Enabled = false;
            this.Invoke((Action)(() => textBox3.Text = ""));
        }
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBox1.SelectedIndex > -1)
                {
                    this.Invoke((Action)(() => panel9.Enabled = false));
                    HideWhileUploadingBlobs();
                    string UploadtoContainer = comboBox1.SelectedItem.ToString();
                    if (!string.IsNullOrEmpty(UploadtoContainer))
                    {
                        string[] tempcontainer = UploadtoContainer.ToString().Split(new string[] { "(" }, StringSplitOptions.None);
                        // selectedContainer = tempcontainer[0].ToString();
                        ContainerSelectedUpload = tempcontainer[0].ToString();
                        this.button3.Enabled = false;
                        OpenFileDialog fileDialog = new OpenFileDialog();

                        fileDialog.InitialDirectory = @"C:\";
                        fileDialog.Multiselect = true;
                        fileDialog.Title = "Upload Files";
                        int blockSizes = 1024 * 1024; //256 kb
                        if (fileDialog.ShowDialog() == DialogResult.OK)
                        {
                            int count = fileDialog.FileNames.Count();
                            label3.Text = label3.Text + count.ToString();
                            foreach (string item in fileDialog.FileNames)
                            {
                                //FileInfo f = new FileInfo(item);
                                //long length = f.Length;
                                //long lengthinMB = (length / (1024 * 1024));
                                FilePaths.Add(item);
                                FileNamePath.Add(Path.GetFileName(item), item);
                                using (FileStream fileStream = new FileStream(item, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                                {
                                    long fileSize = fileStream.Length;
                                    temptotallength = temptotallength + fileSize;
                                }

                               //commented on 8-1-2015// totallength = (int)((float)temptotallength / (float)blockSizes) + 1;

                            }
                            totallength = (int)((float)temptotallength / (float)blockSizes) + 1;
                          //  MessageBox.Show(totallength.ToString());

                        }
                        backgroundWorker4.RunWorkerAsync();
                    }
                }
                else if(comboBox2.SelectedIndex > -1)
                {
                    this.Invoke((Action)(() => panel9.Enabled = false));
                    HideWhileUploadingBlobs();
                    string UploadtoContainer = comboBox1.SelectedItem.ToString();
                    if (!string.IsNullOrEmpty(UploadtoContainer))
                    {
                        string[] tempcontainer = UploadtoContainer.ToString().Split(new string[] { "(" }, StringSplitOptions.None);
                        // selectedContainer = tempcontainer[0].ToString();
                        ContainerSelectedUpload = tempcontainer[0].ToString();
                        this.button3.Enabled = false;
                        OpenFileDialog fileDialog = new OpenFileDialog();

                        fileDialog.InitialDirectory = @"C:\";
                        fileDialog.Multiselect = true;
                        fileDialog.Title = "Upload Files";
                        int blockSizes = 1024 * 1024; //256 kb
                        if (fileDialog.ShowDialog() == DialogResult.OK)
                        {
                            int count = fileDialog.FileNames.Count();
                            label3.Text = label3.Text + count.ToString();
                            foreach (string item in fileDialog.FileNames)
                            {
                                //FileInfo f = new FileInfo(item);
                                //long length = f.Length;
                                //long lengthinMB = (length / (1024 * 1024));
                                FilePaths.Add(item);
                                FileNamePath.Add(Path.GetFileName(item), item);
                                using (FileStream fileStream = new FileStream(item, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                                {
                                    long fileSize = fileStream.Length;
                                    temptotallength = temptotallength + fileSize;
                                }

                                totallength = (int)((float)temptotallength / (float)blockSizes) + 1;

                            }
                            //  MessageBox.Show(totallength.ToString());

                        }
                        backgroundWorker4.RunWorkerAsync();
                    }
                }

                else
                {
                    MessageBox.Show("Please Select a Container");
                }

            }
            catch(Exception ex)
            {
                MessageBox.Show("Something Went Wrong");

            }
         

        }

        private void panel9_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button15_Click(object sender, EventArgs e)
        {
            this.Invoke((Action)(() => textBox3.Text = ""));
            var selecteditem = comboBox1.SelectedItem;//comboBox1.SelectedItem.ToString();
            if (selecteditem != null)//  comboBox1.Items.Count > 0)
            {
                DialogResult ConfirmDeleteContainer = MessageBox.Show("Are You Sure To Delete the container, All the blobs inside the container will be deleted", "Delete Container - AzCloud", MessageBoxButtons.OKCancel);
                if (ConfirmDeleteContainer == DialogResult.OK)
                {
                    try
                    {
                        string ContainerDelete = comboBox1.SelectedItem.ToString();
                        string tempContainerDelete = ContainerDelete;
                        string[] tempcontainer = ContainerDelete.ToString().Split(new string[] { "(" }, StringSplitOptions.None);
                        ContainerDelete = tempcontainer[0].ToString();
                        CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=http;AccountName=" + AccountName + ";AccountKey=" + AccountKey + "");
                        CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                        CloudBlobContainer sourceContainer = blobClient.GetContainerReference(ContainerDelete);
                        sourceContainer.Delete();
                        comboBox1.Items.Remove(tempContainerDelete);
                        StorageTotalContainer = StorageTotalContainer -1;
                        this.Invoke((Action)(() => label11.Text = ":" + " " + StorageTotalContainer.ToString()));

                        MessageBox.Show("Container Deleted");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Something Went Wrong, Please try Again");
                    }

                }
            }
            else
            {
                MessageBox.Show("Please Select a Container to Delete");
            }
           
        }
        private void HideNewContainerDetails()
        {
            this.Invoke((Action)(() => panel6.Visible = false));
            this.Invoke((Action)(() => textBox2.Text = ""));
            this.Invoke((Action)(() => radioButton1.Checked = false)); 
            this.Invoke((Action)(() => radioButton2.Checked = false));
        }
        private void button10_Click(object sender, EventArgs e)
        {
            this.Invoke((Action)(() => textBox3.Text = ""));
            HideNewContainerDetails();
           var selecteditem = comboBox1.SelectedItem;//comboBox1.SelectedItem.ToString();
           if (selecteditem != null)//  comboBox1.Items.Count > 0)
           {
                  
               COPYCONTAINER frm3;
               frm3 = new COPYCONTAINER(AccountName,AccountKey);
               List<string> ContainerSource = new List<string>();
               foreach (var item in comboBox1.Items)
               {
                   ContainerSource.Add(item.ToString());
               }
               frm3.LoadContainterDestinationCombox(ContainerSource);
               //source container
               string[] tempcontainer = selecteditem.ToString().Split(new string[] { "(" }, StringSplitOptions.None);
               selectedContainer = tempcontainer[0].ToString();
               //ends here
               frm3.SetContainerSource(selectedContainer);
               DialogResult dr = frm3.ShowDialog(this);
               //if (dr == DialogResult.Cancel)
               //{
               //    frm3.Close();
               //}
               //else if (dr == DialogResult.OK)
               //{
               //    //List<string> ContainerSource = new List<string>();
               //    //foreach(var item in comboBox1.Items)
               //    //{
               //    //    ContainerSource.Add(item.ToString());
               //    //}
               //    //frm3.LoadContainterSourceCombox(ContainerSource);
               //  //  comboBox1.Item
               //  //  textBox1.Text = frm3.getText();
               //    frm3.Close();
               //}
           }
           else
           {
               MessageBox.Show("Please select a Source Container from the Lists");
           }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            this.Invoke((Action)(() => textBox3.Text = ""));
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                TotalLengthDownload = 0;
                TotalTempDownload = 0;
                if (dataGridView1.Rows.Count > 0)
                {

                  //  MessageBox.Show(TotalLengthDownload.ToString());
                    if (dataGridView1.SelectedCells.Count > 0)
                    {
                        DialogResult result = folderBrowserDialog1.ShowDialog();
                        if (result == DialogResult.OK)
                        {
                            DownloadedBlobPath = folderBrowserDialog1.SelectedPath;


                            Int32 selectedCellCount = dataGridView1.GetCellCount(DataGridViewElementStates.Selected);
                            DownloadFilesCount = selectedCellCount;
                            if (comboBox1.SelectedIndex > -1)
                            {
                                string tempdownloadblobscontainer = comboBox1.SelectedItem.ToString();
                                string[] tempcontainer = tempdownloadblobscontainer.ToString().Split(new string[] { "(" }, StringSplitOptions.None);
                                selectedContainer = tempcontainer[0].ToString();
                                DownloadblobsContainer = selectedContainer;
                            }
                            else if (comboBox2.SelectedIndex > -1)
                            {
                                string tempdownloadblobscontainer = comboBox1.SelectedItem.ToString();
                                string[] tempcontainer = tempdownloadblobscontainer.ToString().Split(new string[] { "(" }, StringSplitOptions.None);
                                selectedContainer = tempcontainer[0].ToString();
                                DownloadblobsContainer = selectedContainer;
                            }
                            var cloudStorageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=http;AccountName=" + AccountName + ";AccountKey=" + AccountKey + "");
                            var blobContainer = cloudStorageAccount.CreateCloudBlobClient().GetContainerReference(DownloadblobsContainer);
                            int segmentSize = 1 * 1024 * 1024;//1 MB chunk
                            for (int i = 0; i < selectedCellCount; i++)
                            {
                                int selectedrowindex = dataGridView1.SelectedCells[i].RowIndex;
                                DataGridViewRow selectedRow = dataGridView1.Rows[selectedrowindex];
                                string BlobNameSelected = Convert.ToString(selectedRow.Cells["Blob Name"].Value);
                                // MessageBox.Show(BlobNameSelected);
                                DownloadBlobNames.Add(BlobNameSelected);
                                var blob = blobContainer.GetBlockBlobReference(BlobNameSelected);
                                blob.FetchAttributes();
                                var blobLengthRemaining = blob.Properties.Length;
                                int tempdownloadlen = int.Parse(blobLengthRemaining.ToString());
                                tempdownloadlen = (int)((float)(tempdownloadlen) / (float)segmentSize) +1;
                                //temptotallength / blockSizes) + 1;
                                TotalLengthDownload = TotalLengthDownload + tempdownloadlen;
                            }
                            this.Invoke((Action)(() => button4.Enabled = false));
                        
                            this.Invoke((Action)(() => button14.Enabled = false));

                            backgroundWorker5.RunWorkerAsync();
                        }

                    }
                    else
                    {
                        MessageBox.Show("Please Select a Blob to Download");
                    }
                }
                else
                {
                    MessageBox.Show("Load the Blobs");
                }
        
            }
            catch(Exception ex)
            {
                MessageBox.Show("Oops..Something Went Wrong");
            }
         
        }

        private void combobox1_selectedindexchange(object sender, EventArgs e)
        {
            dataGridView1.DataSource = null;
            this.Invoke((Action)(() => label2.Text = ":"));
        }

        private void button5_Click(object sender, EventArgs e)
        {

        }

        private void button13_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.Rows.Count > 0)
                {
                    if (dataGridView1.SelectedCells.Count > 0)
                    {
                        if(dataGridView1.SelectedCells.Count > 1)
                        {
                            MessageBox.Show("Please Select Only One Blob");
                        }
                        else{
                                Int32 selectedCellCount = dataGridView1.GetCellCount(DataGridViewElementStates.Selected);
                                string containermetadata = string.Empty;
                                string BlobNameSelected = string.Empty;
                                if(comboBox1.Enabled == true)
                                {
                                    if (comboBox1.SelectedIndex > -1)
                                    {
                                        string tempdownloadblobscontainer = comboBox1.SelectedItem.ToString();
                                        string[] tempcontainer = tempdownloadblobscontainer.ToString().Split(new string[] { "(" }, StringSplitOptions.None);
                                        containermetadata = tempcontainer[0].ToString(); ;
                                    }
                                }
                                else if(comboBox2.Enabled == true)
                                {
                                    if (comboBox2.SelectedIndex > -1)
                                    {
                                        string tempdownloadblobscontainer = comboBox2.SelectedItem.ToString();
                                        string[] tempcontainer = tempdownloadblobscontainer.ToString().Split(new string[] { "(" }, StringSplitOptions.None);
                                        containermetadata = tempcontainer[0].ToString(); ;
                                    }
                                }
                              
                                for (int i = 0; i < selectedCellCount; i++)
                                {
                                    int selectedrowindex = dataGridView1.SelectedCells[i].RowIndex;
                                    DataGridViewRow selectedRow = dataGridView1.Rows[selectedrowindex];
                                    BlobNameSelected = Convert.ToString(selectedRow.Cells["Blob Name"].Value);
                                }
                                Form4 BlobMetaData = new Form4(AccountName, AccountKey, containermetadata, BlobNameSelected);
                                DialogResult dr = BlobMetaData.ShowDialog(this);
                                //Form4 ViewMetaData = new Form4(AccountName, Account);
                              //  newF.Show();
                               // Hide();
                        }
                        
                    }
                }
                else
                {
                    MessageBox.Show("Please Load Blobs to View");
                }
            }
            catch(Exception  ex)
            {
                MessageBox.Show("OOPS, Something went wrong, Please Close the app and restart");
            }
            
        }

        private void button9_Click_1(object sender, EventArgs e)
        {
            try{
                if(dataGridView1.Rows.Count > 0)
                {
                    foreach(DataGridViewRow DGVR in dataGridView1.Rows)
                    {
                        DGVR.Cells["Blob Name"].Selected = true;
                    }
                    //foreach(DataRow row in dataGridView1.Rows)
                    //{
                    //    row[0] = true;
                    //}
                  //  dataGridView1.SelectAll();
                }
                else
                {
                    MessageBox.Show("Please Load Blobs");
                }

            }
            catch(Exception ex)
            {
                MessageBox.Show("Opps..Something Went Wrong");
            }
            
        }

        private void button18_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.Rows.Count > 0)
                {
                    dataGridView1.ClearSelection();
                }
                else
                {
                    MessageBox.Show("Please Load Blobs");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Opps..Something Went Wrong");
            }
        }

        private void button14_Click(object sender, EventArgs e)
        {
            try
            {


            }
            catch(Exception ex)
            {

            }
        }

        private void qUEUESToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
             
            Form5 frmqueue = new Form5(AccountName, AccountKey);
            DialogResult dr = frmqueue.ShowDialog(this);
            
        }
       
       
    }
}
