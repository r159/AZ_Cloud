using BusinessLayer.Features;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AZ_Cloud
{
    public partial class Form7 : Form
    {
        public string AccountName;
        public string AccountKey;
        public string QueueName;
        public static string OperationType;
        CloudStorageAccount storageAccount;
        CloudQueueClient queueClient;
        public Form7(string accname, string acckey, string qname, string typeofops)
        {
            AccountName = accname;
            AccountKey = acckey;
            QueueName = qname;
            OperationType = typeofops;
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form7_Load(object sender, EventArgs e)
        {
            if(OperationType.ToLower() == "new")
            {
                   HideForNewQueue();
            }
            else if(OperationType.ToLower() == "view")
            {
                HideForViewQueue();
            }
            else
            {
                MessageBox.Show("Something Went Wrong, Please Try Again");
                Hide();
            }
         
            SetStorageConnection();
        }
        private void HideForNewQueue()
        {
            this.Invoke((Action)(() => textBox2.Visible = false));
            this.Invoke((Action)(() => textBox3.Visible = false));
            this.Invoke((Action)(() => textBox4.Visible = false));
            this.Invoke((Action)(() => label1.Visible = false));
            this.Invoke((Action)(() => label2.Visible = false));
            this.Invoke((Action)(() => label3.Visible = false));
            this.Invoke((Action)(() => label4.Visible = false));
            this.Invoke((Action)(() => button3.Visible = false));
            this.Invoke((Action)(() => button1.Visible = true));
            this.Invoke((Action)(() => button2.Visible = true));
            this.Invoke((Action)(() => this.Height = 181));
            this.Invoke((Action)(() => this.Width = 523));
            this.Invoke((Action)(() => label1.Text = "Message Text: "));
            this.Invoke((Action)(() => this.Text = "New Queue Message"));


        }
        private void HideForViewQueue()
        {
            this.Invoke((Action)(() => button3.Visible = true));
            this.Invoke((Action)(() => button1.Visible = false));
            this.Invoke((Action)(() => button2.Visible = false));
            this.Invoke((Action)(() => label1.Text = "ID: "));
            this.Invoke((Action)(() => label2.Text = "Queue Message: "));
            this.Invoke((Action)(() => label3.Text = "Queue Added on: "));
            this.Invoke((Action)(() => label4.Text = "Queue Expires on: "));
            this.Invoke((Action)(() => this.Height = 370));
            this.Invoke((Action)(() => this.Width = 523));
            this.Invoke((Action)(() => this.Text = "View Queue Message"));
            this.Invoke((Action)(() => textBox1.Text = Utility.QID));
            this.Invoke((Action)(() => textBox2.Text = Utility.Qmsg));
            this.Invoke((Action)(() => textBox3.Text = Utility.QAdded));
            this.Invoke((Action)(() => textBox4.Text = Utility.QExpry));
        }
        private void SetStorageConnection()
        {
            storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=http;AccountName=" + AccountName + ";AccountKey=" + AccountKey + "");
            queueClient = storageAccount.CreateCloudQueueClient();
        }

        private void button1_Click(object sender, EventArgs e)
        {
           try
           {
               string queuemsg = textBox1.Text;
               int stringbytecount =System.Text.ASCIIEncoding.Unicode.GetByteCount(queuemsg);
               if(!string.IsNullOrEmpty(queuemsg))
               {
                   if(stringbytecount >= 64000)
                   {
                       MessageBox.Show("Size Limit Exceeded");
                       textBox1.Text = "";
                   }
                   else
                   {
                       // Retrieve a reference to a queue.
                       CloudQueue queue = queueClient.GetQueueReference(QueueName);
                       // Create the queue if it doesn't already exist.
                       queue.CreateIfNotExists();
                       // Create a message and add it to the queue.
                       CloudQueueMessage message = new CloudQueueMessage(queuemsg);
                       queue.AddMessage(message);
                       Hide();
                   }
                
               }
               else
               {
                   MessageBox.Show("Please enter a Message");
               }
              
           }
           catch(Exception ex )
           {
               MessageBox.Show("Something Went Wrong, Please Try Again");
               Hide();

           }
           
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Hide();
        }

    }
}
