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
    public partial class Form5 : Form
    {
        public string AccountName;
        public string AccountKey;
        CloudStorageAccount storageAccount;
        CloudQueueClient queueClient;
        public static string DeletedAllQ_Name;
        public Form5(string accname, string acckey)
        {
            AccountName = accname;
            AccountKey = acckey;
            InitializeComponent();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Form5_Load(object sender, EventArgs e)
        {

            SetStorageConnection();
            backgroundWorker1.DoWork += new DoWorkEventHandler(newjob);
            backgroundWorker2.DoWork += new DoWorkEventHandler(QDelete);
            this.Invoke((Action)(() => dataGridView1.ReadOnly = true));
            this.Invoke((Action)(() => button6.Enabled = false));
            this.Invoke((Action)(() => pictureBox1.Visible = true));
            this.Invoke((Action)(() => textBox1.Enabled = false));
            this.Invoke((Action)(() => button9.Enabled = false));
            this.Invoke((Action)(() => panel2.Enabled = false));


            this.Invoke((Action)(() => button9.Enabled = false));
            this.Invoke((Action)(() => button4.Enabled = false));
            this.Invoke((Action)(() => button5.Enabled = false));
            this.Invoke((Action)(() => button6.Enabled = false));
            this.Invoke((Action)(() => button7.Enabled = false));
            LoadQvalues();
        }

        private void QDelete(object sender, DoWorkEventArgs e)
        {
            this.Invoke((Action)(() => pictureBox1.Visible = true));
            this.Invoke((Action)(() => panel3.Enabled = false));
            this.Invoke((Action)(() => panel2.Enabled = false));
            this.Invoke((Action)(() => panel2.Enabled = false));
            this.Invoke((Action)(() => panel4.Enabled = false));
            DeleteAllQueueMessages();
        }

        private void DeleteAllQueueMessages()
        {
             Int32 selectedCellCount = dataGridView1.GetCellCount(DataGridViewElementStates.Selected);
               CloudQueue queue = queueClient.GetQueueReference(DeletedAllQ_Name);
              queue.FetchAttributes();
             int? messagecount = queue.ApproximateMessageCount.Value;
             int ret_messagecount = messagecount != 0 ? messagecount.Value : 10;
             List<CloudQueueMessage> retrievedMessage = queue.GetMessages(ret_messagecount,TimeSpan.FromMilliseconds(50),null,null).ToList();
             for (int i = 0; i < selectedCellCount; i++)
             {
                 int selectedrowindex = dataGridView1.SelectedCells[i].RowIndex;
                 DataGridViewRow selectedRow = dataGridView1.Rows[selectedrowindex];
                 string qmsg = Convert.ToString(selectedRow.Cells["Id"].Value);
                 // Retrieve a reference to a queue.
                 foreach (var item in retrievedMessage)
                 {
                     if (item.Id == qmsg)
                     {
                         queue.DeleteMessage(item);
                     }
                 }
             }
             this.Invoke((Action)(() => dataGridView1.DataSource = null));
             this.Invoke((Action)(() => label3.Text = ":" + " " + "No Messages"));
             this.Invoke((Action)(() => pictureBox1.Visible = false));
             this.Invoke((Action)(() => panel3.Enabled = true));
             this.Invoke((Action)(() => panel2.Enabled = true));
             this.Invoke((Action)(() => panel2.Enabled = true));
             this.Invoke((Action)(() => panel4.Enabled = true));
             this.Invoke((Action)(() => textBox1.Enabled = false));
             this.Invoke((Action)(() => button9.Enabled = true));
                    
                                         
        }

        private void SetStorageConnection()
        {
            storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=http;AccountName=" + AccountName + ";AccountKey=" + AccountKey + "");
            queueClient = storageAccount.CreateCloudQueueClient();
        }

        private void LoadQvalues()
        {
            backgroundWorker1.RunWorkerAsync();
        }

        private void newjob(object sender, DoWorkEventArgs e)
        {

            LoadQs();
           
        }

        private void LoadfirstQValues()
        {
            if(listBox1.Items.Count > 0)
            {
                
                CloudQueue queue = queueClient.GetQueueReference(listBox1.Items[0].ToString());
                queue.FetchAttributes();
                int? messagecount = queue.ApproximateMessageCount.Value;
                int ret_messagecount = messagecount != 0 ? messagecount.Value : 10;
                List<CloudQueueMessage> peekedMessages = queue.PeekMessages(ret_messagecount, null, null).ToList();
                if (peekedMessages.Count > 0 )
                {
                    LoadQDataGrid(peekedMessages);
                }
                else
                {
                    this.Invoke((Action)(() => pictureBox1.Visible = false));
                    this.Invoke((Action)(() => label3.Text = ":" + " " + "No Messages"));
                }
                this.Invoke((Action)(() => button6.Enabled = true));
                this.Invoke((Action)(() => panel2.Enabled = true));



                this.Invoke((Action)(() => textBox1.Enabled = true));
                this.Invoke((Action)(() => button9.Enabled = true));
                this.Invoke((Action)(() => button4.Enabled = true));
                this.Invoke((Action)(() => button5.Enabled = true));
                this.Invoke((Action)(() => button6.Enabled = true));
                this.Invoke((Action)(() => button7.Enabled = true));
                 

            }
        }

        private void LoadQDataGrid(List<CloudQueueMessage> peekedMessage)
        {
            dataGridView1.DataSource = null;
            DataTable dt = new DataTable();
            dt.Columns.Add("Id");
            dt.Columns.Add("Queue Value");
            dt.Columns.Add("Insertion Time");
            dt.Columns.Add("Expiration Time");



            DataRow row = null;
            foreach(var qitem in peekedMessage)
            {
                row = dt.NewRow();
                row["Id"] = qitem.Id.ToString();
                row["Queue Value"] = qitem.AsString;
                row["Insertion Time"] = qitem.InsertionTime.ToString();
                row["Expiration Time"] = qitem.ExpirationTime.ToString();
                dt.Rows.Add(row);
            }
            this.Invoke((Action)(() => dataGridView1.DataSource = dt));
            this.Invoke((Action)(() => dataGridView1.Columns[0].Width = 200));
            this.Invoke((Action)(() => dataGridView1.Columns[1].Width = 200));
            this.Invoke((Action)(() => dataGridView1.Columns[2].Width = 125));
            this.Invoke((Action)(() => dataGridView1.Columns[3].Width = 125));
            this.Invoke((Action)(() => pictureBox1.Visible = false));
            this.Invoke((Action)(() => label3.Text = ":" + " " + peekedMessage.Count.ToString()));
            this.Invoke((Action)(() => textBox1.Enabled = true));
            this.Invoke((Action)(() => button9.Enabled = true));

     
            this.Invoke((Action)(() => button6.Enabled = true));

              
            this.Invoke((Action)(() => button4.Enabled = true));
            this.Invoke((Action)(() => button5.Enabled = true));
         
            this.Invoke((Action)(() => button7.Enabled = true));
            this.Invoke((Action)(() => panel2.Enabled = true));
          
        }

        private void LoadQs()
        {

            try
            {
                
                List<string> QueueLists = queueClient.ListQueues().Select(x => x.Name.ToString()).ToList();
                
                this.Invoke((Action)(() => listBox1.DataSource = QueueLists));
                this.Invoke((Action)(()=>label2.Text = ":" + "" + QueueLists.Count.ToString())); //label2.Text = 

            }
            catch (Exception ex)
            {

            }
            finally
            {
                LoadfirstQValues();
            }
        }

        private void listBox1_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (listBox1.SelectedIndex != -1)
                {
                    string selectedQ = listBox1.SelectedItem.ToString();
                    if (!string.IsNullOrEmpty(selectedQ))
                    {
                        CloudQueue queue = queueClient.GetQueueReference(selectedQ);
                        queue.FetchAttributes();
                        int? messagecount = queue.ApproximateMessageCount.Value;
                        int ret_messagecount = messagecount != 0 ? messagecount.Value : 10;
                        List<CloudQueueMessage> peekedMessages = queue.PeekMessages(ret_messagecount, null, null).ToList();
                        if (peekedMessages.Count > 0)
                        {
                            LoadQDataGrid(peekedMessages);
                        }
                        else
                        {
                            this.Invoke((Action)(() => dataGridView1.DataSource = null));
                            this.Invoke((Action)(() => label3.Text = ":" + " " + "No Messages"));
                            this.Invoke((Action)(() => textBox1.Enabled = false));
                            this.Invoke((Action)(() => button9.Enabled = false));
                        }
                    }
                }
            }
            catch(Exception ex)
            {

            }
       
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                 
                Form6 newq = new Form6(AccountName, AccountKey);
                DialogResult dr = newq.ShowDialog(this);
                if(Utility.NewQueue != null)
                {
                   List<string> QueueLists = queueClient.ListQueues().Select(x => x.Name.ToString()).ToList();
                   this.Invoke((Action)(() => listBox1.DataSource = QueueLists));
                   this.Invoke((Action)(() => label2.Text = ":" + "" + QueueLists.Count.ToString()));
                  //  listBox1.Items.Add(Utility.NewQueue);
                }
            }
            catch(Exception ex)
            {

            }
         
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (listBox1.SelectedIndex != -1)
                {
                    string selectedQ = listBox1.SelectedItem.ToString();
                    DialogResult DeleteQueue = MessageBox.Show("Queue Delete.. ? All the messages in the Queue:" + selectedQ + " " + "will be lost", "Delete Queue", MessageBoxButtons.OKCancel);
                    if (DeleteQueue == DialogResult.OK)
                    {
                        CloudQueue queue = queueClient.GetQueueReference(selectedQ);
                        queue.Delete();
                        List<string> QueueLists = queueClient.ListQueues().Select(x => x.Name.ToString()).ToList();
                        this.Invoke((Action)(() => listBox1.DataSource = QueueLists));
                        this.Invoke((Action)(() => label2.Text = ":" + "" + QueueLists.Count.ToString()));
                    }
                }
            }
            catch(Exception ex)
            {
               
            }
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                 
                List<string> QueueLists = queueClient.ListQueues().Select(x => x.Name.ToString()).ToList();
                this.Invoke((Action)(() => listBox1.DataSource = QueueLists));
                this.Invoke((Action)(() => label2.Text = ":" + "" + QueueLists.Count.ToString()));
            }
            catch(Exception ex)
            {

            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.Rows.Count > 0)
                {
                    if (dataGridView1.SelectedCells.Count > 0)
                    {
                        if (dataGridView1.SelectedCells.Count > 1)
                        {
                            MessageBox.Show("Please Select Only Queue Message to View");
                        }
                        else
                        {
                            Int32 selectedCellCount = dataGridView1.GetCellCount(DataGridViewElementStates.Selected);
                            if(selectedCellCount > 1 )
                            {
                                MessageBox.Show("Please select only one queue message to view");
                            }
                            for (int i = 0; i < selectedCellCount; i++)
                            {
                                int selectedrowindex = dataGridView1.SelectedCells[i].RowIndex;
                                DataGridViewRow selectedRow = dataGridView1.Rows[selectedrowindex];
                                Utility.QID = Convert.ToString(selectedRow.Cells["Id"].Value);
                                Utility.Qmsg = Convert.ToString(selectedRow.Cells["Queue Value"].Value);
                                Utility.QAdded = Convert.ToString(selectedRow.Cells["Insertion Time"].Value);
                                Utility.QExpry = Convert.ToString(selectedRow.Cells["Expiration Time"].Value);
                                Form7 newqueue = new Form7(AccountName, AccountKey, null, "view");
                                DialogResult dr = newqueue.ShowDialog(this);
                                Utility.QID = "";
                                Utility.Qmsg = "";
                                Utility.QAdded = "";
                                Utility.QExpry = "";
                               
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("No Queue Messages Exists");
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Something went wrong");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
            {
                string selectedQ = listBox1.SelectedItem.ToString();
                String typeofOps = "new";
                Form7 newqueue = new Form7(AccountName, AccountKey, selectedQ, typeofOps);
                DialogResult dr = newqueue.ShowDialog(this);
                CloudQueue queue = queueClient.GetQueueReference(selectedQ);
                queue.FetchAttributes();
                int? messagecount = queue.ApproximateMessageCount.Value;
                int ret_messagecount = messagecount != 0 ? messagecount.Value : 10;
                List<CloudQueueMessage> peekedMessages = queue.PeekMessages(ret_messagecount, null, null).ToList();
                if (peekedMessages.Count > 0)
                {
                    LoadQDataGrid(peekedMessages);
                }

            }
            else
            {
                MessageBox.Show("Please select a Queue to Insert a Message");
            }
           
             
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                 
                     if (dataGridView1.Rows.Count > 0)
                     {
                         if (dataGridView1.SelectedCells.Count > 0)
                         {
                             if (dataGridView1.SelectedCells.Count > 1)
                             {
                                 MessageBox.Show("Please Select Only Queue Message to Delete");
                             }
                             else
                             {
                                 DialogResult DeleteQueue = MessageBox.Show("Delete Selected Message.. ?", "Delete Message", MessageBoxButtons.OKCancel);
                                 if (DeleteQueue == DialogResult.OK)
                                 {
                                     Int32 selectedCellCount = dataGridView1.GetCellCount(DataGridViewElementStates.Selected);
                                     if (selectedCellCount > 1)
                                     {
                                         MessageBox.Show("Please select only one queue message to Delete");
                                     }
                                     for (int i = 0; i < selectedCellCount; i++)
                                     {
                                         //this.Invoke((Action)(() => pictureBox1.Visible = true));
                                         string selectedQ = listBox1.SelectedItem.ToString();
                                         int selectedrowindex = dataGridView1.SelectedCells[i].RowIndex;
                                         DataGridViewRow selectedRow = dataGridView1.Rows[selectedrowindex];
                                         string qmsg = Convert.ToString(selectedRow.Cells["Id"].Value);
                                         // Retrieve a reference to a queue.

                                         CloudQueue queue = queueClient.GetQueueReference(selectedQ);
                                         queue.FetchAttributes();
                                         int? messagecount = queue.ApproximateMessageCount.Value;
                                         int ret_messagecount = messagecount != 0 ? messagecount.Value : 10;
                                         List<CloudQueueMessage> retrievedMessage = queue.GetMessages(ret_messagecount,TimeSpan.FromMilliseconds(20),null,null).ToList();// queue.GetMessages(ret_messagecount, TimeSpan.FromSeconds(30)).Single(x => x.Id.Equals(qmsg)); //queue.GetMessages(ret_messagecount,30,ret_messagecount,null).Single(x => x.AsString.ToString().Contains(qmsg));            // null, null, null).Single(x => x.AsString.ToString().Contains(qmsg));// queue.PeekMessages(ret_messagecount, null, null).ToList().Single(x => x.AsString.Contains(qmsg));
                                         foreach(var item in retrievedMessage)
                                         {
                                             if(item.Id == qmsg)
                                             {
                                                queue.DeleteMessage(item);
                                              }
                                         }
                                         
                                        

                                       
                                         //load the grid again

                                         CloudQueue Afterqueue = queueClient.GetQueueReference(selectedQ);
                                         Afterqueue.FetchAttributes();
                                         int? AfterDeletemessagecount = Afterqueue.ApproximateMessageCount.Value;
                                         int AfterDeleteret_messagecount = AfterDeletemessagecount != 0 ? messagecount.Value : 10;
                                         List<CloudQueueMessage> peekedMessages = queue.PeekMessages(AfterDeleteret_messagecount, null, null).ToList();
                                         
                                         if (peekedMessages.Count > 0)
                                         {
                                             LoadQDataGrid(peekedMessages);
                                         }
                                         else
                                         {
                                             dataGridView1.DataSource = null;
                                             this.Invoke((Action)(() => textBox1.Enabled = false));
                                             this.Invoke((Action)(() => label3.Text = ":" + " " + "No Messages"));
                                             this.Invoke((Action)(() => button9.Enabled = true));
                                         //    this.Invoke((Action)(() => pictureBox1.Visible = true));
                                         }
                                         //ends here

                                     }
                                }
                             }
                         }
                     }
                     else
                     {
                         MessageBox.Show("No Queue Messages Exists");
                     }
                 
            }
            catch(Exception ex)
            {
                MessageBox.Show("Something Went Wrong. Please try Again");
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            try
            {
               

                if (dataGridView1.Rows.Count > 0)
                {
                    DialogResult DeleteAllQueue = MessageBox.Show("Delete All Messages.. ?", "Delete All Message", MessageBoxButtons.OKCancel);
                    if (DeleteAllQueue == DialogResult.OK)
                    {
                        DeletedAllQ_Name = listBox1.SelectedItem.ToString();
                        if (dataGridView1.Rows.Count > 0)
                        {
                            foreach (DataGridViewRow DGVR in dataGridView1.Rows)
                            {
                                DGVR.Cells["Id"].Selected = true;
                            }
                            backgroundWorker2.RunWorkerAsync();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("No Queue Messages Exists");
                }
            }
            catch(Exception ex)
            {

            }
        }

        private void button9_Click(object sender, EventArgs e)
        {

        }

        private void button9_Click_1(object sender, EventArgs e)
        {
            string stringmsg = textBox1.Text;
            if(!string.IsNullOrEmpty(stringmsg))
            {
                if (dataGridView1.Rows.Count > 0)
                {
                    foreach (System.Windows.Forms.DataGridViewRow r in dataGridView1.Rows)
                    {
                        if ((r.Cells[1].Value).ToString().ToUpper().Contains(stringmsg.ToUpper()))
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
                }
                else
                {
                    MessageBox.Show("No Queue Messages Exists");
                }
            }
            else
            {
                MessageBox.Show("Please enter something to search");
            }

        }

        private void button8_Click(object sender, EventArgs e)
        {
            try
            {
                this.Invoke((Action)(() => button6.Enabled = false));
             
                this.Invoke((Action)(() => textBox1.Enabled = false));
                this.Invoke((Action)(() => button9.Enabled = false));
                this.Invoke((Action)(() => button4.Enabled = false));
                this.Invoke((Action)(() => button5.Enabled = false));
                this.Invoke((Action)(() => button6.Enabled = false));
                this.Invoke((Action)(() => button7.Enabled = false));
                this.Invoke((Action)(() => panel2.Enabled = false));

                string selectedQ = listBox1.SelectedItem.ToString();
                if (!string.IsNullOrEmpty(selectedQ))
                {
                    CloudQueue queue = queueClient.GetQueueReference(selectedQ);
                    queue.FetchAttributes();
                    int? messagecount = queue.ApproximateMessageCount.Value;
                    int ret_messagecount = messagecount != 0 ? messagecount.Value : 10;
                    List<CloudQueueMessage> peekedMessages = queue.PeekMessages(ret_messagecount, null, null).ToList();
                    if (peekedMessages.Count > 0)
                    {
                        LoadQDataGrid(peekedMessages);
                    }
                    else
                    {
                        this.Invoke((Action)(() => dataGridView1.DataSource = null));
                        this.Invoke((Action)(() => label3.Text = ":" + " " + "No Messages"));
                        this.Invoke((Action)(() => textBox1.Enabled = false));
                        this.Invoke((Action)(() => button9.Enabled = false));
                        this.Invoke((Action)(() => button6.Enabled = true));
                      
                        this.Invoke((Action)(() => textBox1.Enabled = false));
                        this.Invoke((Action)(() => button9.Enabled = false));
                        this.Invoke((Action)(() => button4.Enabled = true));
                        this.Invoke((Action)(() => button5.Enabled = true));
                        this.Invoke((Action)(() => button6.Enabled = true));
                        this.Invoke((Action)(() => button7.Enabled = true));
                        this.Invoke((Action)(() => panel2.Enabled = true));
                    }
                }//LoadQvalues();
                //this.Invoke((Action)(() => button6.Enabled = true));
                //this.Invoke((Action)(() => pictureBox1.Visible = true));
                //this.Invoke((Action)(() => textBox1.Enabled = true));
                //this.Invoke((Action)(() => button9.Enabled = true));
                //this.Invoke((Action)(() => button4.Enabled = true));
                //this.Invoke((Action)(() => button5.Enabled = true));
                //this.Invoke((Action)(() => button6.Enabled = true));

                //this.Invoke((Action)(() => panel2.Enabled = true));
            }
            catch(Exception ex)
            {

            }
        
        }
 

      
    }
}
