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
    public partial class Form6 : Form
    {
        public string AccountName;
        public string AccountKey;
        CloudStorageAccount storageAccount;
        CloudQueueClient queueClient;
        public Form6(string accname, string acckey)
        {
            AccountName = accname;
            AccountKey = acckey;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Utility.NewQueue = null;
                string newqueue = textBox1.Text;
                if ((!string.IsNullOrEmpty(newqueue)) && (newqueue.Length > 2) && (newqueue.Any(c => char.IsUpper(c)) == false) && ((char.IsLetter(newqueue[0]) == false) || (char.IsDigit(newqueue[0]) == false)) && (newqueue.Length < 64))
                 {
                    Utility.NewQueue = newqueue;
                    CloudQueue queue = queueClient.GetQueueReference(newqueue);
                    queue.CreateIfNotExists();
                    Hide();
                }
                else
                {
                    MessageBox.Show("Please Enter Queue Name");
                }
              
            }
            catch(Exception ex)
            {

            }
        }

        private void Form6_Load(object sender, EventArgs e)
        {
            storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=http;AccountName=" + AccountName + ";AccountKey=" + AccountKey + "");
            queueClient = storageAccount.CreateCloudQueueClient();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Hide();
        }
    }
}
