using BusinessLayer.Features;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace AZ_Cloud
{
    public partial class Form1 : Form
    {
        StorageConnection ObjCS = new StorageConnection();
        storagecredentials ObjCRD = new storagecredentials();
        // public static string folderpathexe =  System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
      //  string FolderPath = folderpathexe + "\\" + ConfigurationManager.AppSettings["SaveXml"];
        string FolderPath = ConfigurationManager.AppSettings["SaveXml"];
        public Form1()
        {
            InitializeComponent();
      
        }

        private void CheckExitingStorageConn()
        {
          
             
            // FolderPath = folderpathexe + "\\" + ConfigurationManager.AppSettings["SaveXml"];
             
             string savepath = FolderPath + "storageconnect.xml";
             bool exists = File.Exists(savepath); //System.IO.Directory.Exists(System.IO.Path.GetFullPath(savepath));
            //if folderpath exists Load the Accounts in dropdownlists
             if (exists)
             {
                 //FolderPath = folderpathexe + "\\" + ConfigurationManager.AppSettings["SaveXml"];
                 //string savepath = FolderPath + "storageconnect.xml";
                 XDocument doc = XDocument.Load(savepath);
                 if(doc != null)
                 {
                     var listss = (from x in doc.Descendants("Storageinfo")
                                   select new
                                   {
                                       Name = x.Descendants("AccountName").First().Value,
                                   }).ToList();

                     foreach (var item in listss)
                     {
                         comboBox1.Items.Add(item.Name);


                     }
                 }
                 
                 
                 #region Deleted Code
                 //listView1.Columns.Add("Test");
                 // FolderPath = folderpathexe + "\\" + ConfigurationManager.AppSettings["SaveXml"];
                 // string savepath =  FolderPath + "storageconnect.xml";
                 // XDocument doc = XDocument.Load(savepath);
                 // var listss = (from x in doc.Descendants("Storageinfo")
                 //             select new
                 //             {
                 //                 Name = x.Descendants("AccountName").First().Value,
                 //             }).ToList();
                 // listView1.BeginUpdate();


                 //foreach(var row in listss)
                 //{
                 //    ListViewItem lvi = listView1.Items.Add(row.Name);
                 //    lvi.SubItems.Add(lvi.Text);
                 //   // var item = new ListViewItem(listView1.Items.Count.ToString());
                 //   //item.SubItems.AddRange(row);
                 //   // listView1.Items.Add(item.Text);
                 //}
                 //listView1.EndUpdate();
                 //listView1.Items[0].Selected = false;
                 //listView1.Items[0].Selected = true;
                 #endregion
              
             }
             
        
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if(textBox1.Text != "" && textBox2.Text != "")
                {
                    string acckey = textBox2.Text;
                    string accname = textBox1.Text;

                    //check whether the account already Exists
                   // FolderPath = folderpathexe + "\\" + ConfigurationManager.AppSettings["SaveXml"];
                    string savepath = FolderPath + "storageconnect.xml";
                    bool MainFileexists = File.Exists(savepath); //System.IO.Directory.CreateDirectory(System.IO.Path.GetFullPath(savepath));
                    if (MainFileexists)
                    {
                        XDocument doc = XDocument.Load(savepath);
                        var query = doc.Element("AZ_Cloud").Elements().Where(exx => exx.Element("AccountName").Value.Equals(accname)).ToList();
                        if (query.Count > 0)
                        {
                            MessageBox.Show("Account Already Exists");
                        }
                        else
                        {
                            //check network connectivity
                            //bool netconnt = Utility.IsNetConnected();
                            bool netconnt = Utility.IsNetConnected();
                        
                            if (netconnt)
                            {
                                bool success = ObjCS.CheckvalidStorageAcc(acckey, accname);
                                if (success)
                                {
                                    bool Fileexists = System.IO.File.Exists(System.IO.Path.GetFullPath(FolderPath) + "storageconnect.xml"); //System.IO.Directory.Exists(System.IO.Path.GetFullPath(FolderPath));
                                    if (Fileexists)
                                    {
                                        doc.Element("AZ_Cloud").Add(
                                        new XElement("Storageinfo", new XAttribute("ID", accname + "-" + "Date-" + DateTime.Now.ToShortDateString().Replace("/", "")),
                                         new XElement("AccountName", accname),

                                        new XElement("AccountKey", acckey)));
                                        doc.Save(savepath);
                                    }
                                  
                                    DialogResult successresult = MessageBox.Show("Account Added, Continue to the Storage", "Az_Cloud", MessageBoxButtons.OKCancel);
                                    if (successresult == DialogResult.OK)
                                    {
                                        Form2 newF = new Form2(accname, acckey);
                                        newF.Show();
                                        Hide();
                                       
                                    }
                                    else
                                    {
                                        //clear the combobox and load
                                        comboBox1.Items.Clear();
                                        
                                        XDocument doccombo = XDocument.Load(savepath);
                                        if (doccombo != null)
                                        {
                                            var listss = (from x in doccombo.Descendants("Storageinfo")
                                                          select new
                                                          {
                                                              Name = x.Descendants("AccountName").First().Value,
                                                          }).ToList();

                                            foreach (var item in listss)
                                            {
                                                comboBox1.Items.Add(item.Name);


                                            }
                                        }
                                    }

                                }
                                else
                                {
                                    MessageBox.Show("Not Valid Account.... Check the Name and Key, & Try Again");
                                    textBox1.Text = "";
                                    textBox2.Text = "";
                                }

                            }
                            else
                            {
                                MessageBox.Show("Please Check the Network Connectivity");
                                textBox1.Text = "";
                                textBox2.Text = "";
                            }

                        }
                    }
                    else
                    {
                        //new file created
                        System.IO.Directory.CreateDirectory(System.IO.Path.GetFullPath(FolderPath));
                        string[] row = { accname, DateTime.Now.ToString("dd/mm/yyyy") };
                        XDocument docnew = new XDocument(
                            new XDeclaration("1.0", "utf-8", "yes"),
                            new XComment("AZ_Cloud Storage Credentials"),
                            new XElement("AZ_Cloud",
                                new XElement("Storageinfo", new XAttribute("ID", accname + "-" + "Date" + "-" + DateTime.Now.ToShortDateString().Replace("/", "")),
                                    new XElement("AccountName", accname),
                                    new XElement("AccountKey", acckey)
                            )));
                       
                        savepath = FolderPath + "storageconnect.xml";
                        docnew.Save(savepath);
                        DialogResult successresult = MessageBox.Show("Account Added, Continue to the Storage", "Az_Cloud", MessageBoxButtons.OKCancel);
                        if (successresult == DialogResult.OK)
                        {
                            Form2 newF = new Form2(accname, acckey);
                            newF.Show();
                            Hide();
                            
                        }
                    }
                     
                   
                }
                else
                {
                    MessageBox.Show("Please enter Account Name & Key");
                    textBox1.Text = "";
                    textBox2.Text = "";
                }
              
                //ends here 

                 
            }
            catch(Exception ex)
            {
                MessageBox.Show("Something Went Wrong, Please Try Again");
                MessageBox.Show(ex.InnerException.ToString()); 
                textBox1.Text = "";
                textBox2.Text = "";
            }
         
            

        }

        private void Form1_Load(object sender, EventArgs e)
        {
           // comboBox2.Items.Add("iiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiihjjjkkkkkkkkkkkkkkkkkkkkkkkkk");
          //  DropDownWidth(comboBox2);
            CheckExitingStorageConn();
            //button2.Visible = false;
        }
        //private int DropDownWidth(ComboBox myCombo)
        //{
        //    int maxWidth = 0, temp = 0;
        //    foreach (var obj in myCombo.Items)
        //    {
        //        temp = TextRenderer.MeasureText(obj.ToString(), myCombo.Font).Width;
        //        if (temp > maxWidth)
        //        {
        //            maxWidth = temp + 10;
        //        }
        //    }
        //    return maxWidth;
        //}
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
           // MessageBox.Show("hi");
        //    button2.Visible = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //var item = comboBox1.SelectedItem;
            //using (Form2 form2 = new Form2())
            //{
            //    item = form2.AccountKey;
            //}
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var item = comboBox1.SelectedItem;
            if(item != null)
            {
                //check the net connectivity
                bool isnetconnt = Utility.IsNetConnected();
                if (isnetconnt)
                {

                   // FolderPath = folderpathexe + "\\" + ConfigurationManager.AppSettings["SaveXml"];
                    string savepath = FolderPath + "storageconnect.xml";
                    XDocument doc = XDocument.Load(savepath);

                    var query = doc.Element("AZ_Cloud").Elements().Where(exx => exx.Element("AccountName").Value.Equals(item)).ToList();
                    string tempaccount = string.Empty;
                    foreach (var item1 in query)
                    {
                        tempaccount = item1.Value;
                    }
                    string[] splitacc = tempaccount.Split(new string[] { item.ToString() }, StringSplitOptions.None);
                    string accname = item.ToString();
                    string acckey = splitacc[1].ToString();
                    //query[0].AccountName
                    Form2 newF = new Form2(accname, acckey);
                    newF.Show();
                    Hide();
                }
                else
                {
                    MessageBox.Show("Please Check the Network Connectivity");
                }
                //ends here 
            }
            else
            {
                MessageBox.Show("Please Select An Account");
            }
           
          
            
        }

        private void bLOBSToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
