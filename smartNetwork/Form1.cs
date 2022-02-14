using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.NetworkInformation;
using System.Threading;

namespace smartNetwork
{
    public partial class Form1 : Form
    {
        string[,] host1 = null;
        public Form1()
        {
            InitializeComponent();
            timer1.Tick += new EventHandler(timer1_Tick);
            timer1.Interval = 1000 * 60 * 5;
        }

        int timeElapsed = 0;
        System.Windows.Forms.Timer t = new System.Windows.Forms.Timer();

        private void Form1_Load(object sender, EventArgs e)
        {
            button2.Text = "Status Update Timer Start";
            button2.BackColor = Color.Green;
            button2.ForeColor = Color.White;            
            host1 = getConfigurationValue();
            Random r = new Random();
            if (host1 != null)
            {
                List<Label> labels = new List<Label>();
                List<Label> lables1 = new List<Label>();
                int x = 10;
                int y = 40;
                for (int i = 0; i < host1.GetLength(0); i++)
                {                    
                    labels.Add(new Label());
                    if (i == 0)
                    {
                        labels[i].Location = new Point(x, y);
                        labels[i].Text = host1[i, 1];
                    }
                    if (i == 1)
                    {
                        labels[i].Location = new Point(x + 100, y);
                        labels[i].Text = host1[i, 1];                     
                    }
                    if (i > 1 && i != 0)
                    {                    
                        if(i%2 == 0)
                        {
                            y += 15;
                            labels[i].Location = new Point(x, y);
                            labels[i].Text = host1[i, 1];
                        }
                        else
                        {                            
                            labels[i].Location = new Point(x+100, y);
                            labels[i].Text = host1[i, 1];
                        }
                    }
                    labels[i].Tag = i;
                    labels[i].Size = new Size(100, 15);
                    labels[i].Dock = DockStyle.None;
                    this.Controls.Add(labels[i]);   
                    if (y > 400)
                    {
                        x = 350;
                        y = 40;
                    }
                }
                x = 220;
                y = 40;
                for (int i = 0; i < host1.GetLength(0)/2; i++)
                {
                    
                    lables1.Add(new Label());                    
                    lables1[i].Location = new Point(x, y);
                    lables1[i].Name = "lbl" + i.ToString();
                    lables1[i].Text = "Not Connected";
                    lables1[i].TextAlign = ContentAlignment.MiddleCenter;
                    lables1[i].BackColor = Color.Red;
                    lables1[i].Tag = i + host1.GetLength(0);
                    lables1[i].Size = new Size(100, 15);
                    lables1[i].Dock = DockStyle.None;
                    this.Controls.Add(lables1[i]);
                    y += 15;
                    if (y > 400)
                    {
                        x = 550;
                        y = 40;
                    }
                }
            }
            progressBar1.Minimum = 0;
            progressBar1.Maximum = 100;
            progressBar1.Value = 0;
        }

        public static string[,] getConfigurationValue()
        {
            //string hId = ConfigurationManager.AppSettings["id"];
            //string hName = ConfigurationManager.AppSettings["name"];
            //string hIpAddress = ConfigurationManager.AppSettings["address"];
            //string[] returnData = { hId, hName, hIpAddress };
            //return returnData;
            string[,] returnData;
            var PostSetting = ConfigurationManager.GetSection("device/deviceDetails") as NameValueCollection;
            if (PostSetting.Count == 0)
            {
                returnData = new string[1,1];
                returnData[0, 0] = "Configuration not Found";
            }
            else
            {
                int i = 0;
                string check1, check2 = "";
                returnData = new string[PostSetting.Count, 2];
                foreach (string key in PostSetting.AllKeys)
                {
                    returnData[i, 0] = check1 = key;
                    returnData[i, 1] = check2 = PostSetting[key];
                    i++;
                }
            }
            return returnData;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            progressBar1.Value = 0;
            pingStatusUpdate(0);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            progressBar1.Value = 0;
            if (timer1.Enabled)
            {
                timer1.Stop();
                button2.Text = "Status Update Timer Start";
                button2.BackColor = Color.Green;
            }
            else
            {
                timer1.Start();
                button2.Text = "Status Update Timer Stop";
                button2.BackColor = Color.Red;
                //ThreadPool.QueueUserWorkItem((x) =>
                //{
                    pingStatusUpdate(1);
                //});
            }            
        }

        private void pingStatusUpdate(int calledby)
        {
            //MessageBox.Show("ping Status Update");
            //if (calledby == 1)
            //{
            //    Thread.Sleep(1000 * 60 * 20);
            //    timer1.Stop();
            //}            
            //List<Ping> newPing = new List<Ping>();
            //List<PingReply> PR = new List<PingReply>();
            int j = 0;
            float pleng = 100 / (host1.GetLength(0));
            //MessageBox.Show(pleng.ToString());
            for (int i = 0; i < host1.GetLength(0); i++)
            {
                if (i % 2 != 0)
                {
                    //MessageBox.Show(host1[i, 1]);                                        
                    Ping P = new Ping();
                    PingReply r;
                    r = P.Send(host1[i, 1]);
                    if (r.Status == IPStatus.Success)
                    {
                        Label lbl = null;
                        string lblname = "lbl" + j.ToString();
                        if (Controls.ContainsKey(lblname))
                        {
                            lbl = Controls[lblname] as Label;
                        }
                        if (null != lbl)
                        {
                            lbl.BackColor = Color.Green;
                            lbl.Text = "Connected";
                        }
                        //MessageBox.Show("Alive" + " " + host1[i, 1]);
                    }
                    else
                    {
                        Label lbl = null;
                        string lblname = "lbl" + j.ToString();
                        if (Controls.ContainsKey(lblname))
                        {
                            lbl = Controls[lblname] as Label;
                        }
                        if (null != lbl)
                        {
                            lbl.BackColor = Color.Red;
                            lbl.Text = "Not Connected";
                        }
                        //MessageBox.Show("Not Alive" + " " + host1[i, 1]);
                    }
                    j++;
                }
                progressBar1.Value += ((int)pleng);
            }
            progressBar1.Value = 100;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timeElapsed += timer1.Interval;
            timeLbl1.Text = timeElapsed.ToString();
            //MessageBox.Show("timer1_Tick");
        }
    }
}
