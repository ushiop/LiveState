using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace LiveState
{
    public partial class Form1 : Form
    {
        private static int GetRoomStateInfoIndex =0;

        private static int UpDataTime =0;//5秒刷新一次

        public Form1()
        { 
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //检查配置文件
            if(Config.CheckConfigFile()==false)
            {//配置文件不存在则创建默认配置文件
                Config.CreateConfigFile();
            }
            ReLoadLiveStateConfig();
            LiveRoom.LoadRoomInfo();
            ReLoadLiveStateRoom();
            timer1.Start();
            notifyIcon1.Text = "LiveState";
            notifyIcon1.Visible = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("notepad.exe",Config.GetConfigPath());
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ReLoadLiveStateConfig();
            timer1.Start();
        }

        private void ReLoadLiveStateConfig()
        {
            //从配置文件读取内容
            Config.ReLoadConfig();
            //清空平台选择框的内容
            comboBox1.Items.Clear();
            //添加平台名称至平台选择框
            foreach (string v in Config.GetConfigLiveNameList())
            {
                comboBox1.Items.Add(v);
            }
            if(comboBox1.Items.Count!=0) comboBox1.SelectedIndex = 0;
        }

        private void ReLoadLiveStateRoom()
        {
            //清空房间信息框内容
            listBox1.Items.Clear();
            //重置房间索引指向
            GetRoomStateInfoIndex = 0;
            //添加房间信息至房间信息框
            foreach (string v in LiveRoom.GetRoomInfoList())
            {
                listBox1.Items.Add(v);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "可用空格分隔多个直播间ID") return;
            string[] ids = textBox1.Text.Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string v in ids)
            {
                LiveRoom.AddRoomInfo(comboBox1.SelectedItem.ToString(), v);
            }
            LiveRoom.SaveRoomInfo(); 
            ReLoadLiveStateRoom();
            timer1.Start();
        }
         
        private void textBox1_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)32 && e.KeyChar != (char)8)
{
                e.Handled = true;
            }
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            int id = listBox1.SelectedIndex;
            LiveRoom.RemoveRoomInfo(id);
            ReLoadLiveStateRoom();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            progressBar1.Value = UpDataTime;
            if (UpDataTime!=5000)
            {
                UpDataTime = UpDataTime + 1000;
                return;
            }
            UpDataTime = 0; 
            if (listBox1.Items.Count==0)
            {

                timer1.Stop();
                return;
            }
            //每隔5秒更新一个直播间的信息，以免过于频繁的调用导致API封锁 
            JObject j =(JObject) LiveRoom.GetRoomInfoJarray()[GetRoomStateInfoIndex];
            if (LiveApi.GetRoomState(j) == true)
            {
                //checkBox1 开播通知
                //checkBox2 关播通知
                string tmp = listBox1.Items[GetRoomStateInfoIndex].ToString();
                string oldstate = tmp.IndexOf("未直播") == -1 ? "直播中" : "未直播";
                if(j["state"].ToString()!=oldstate)
                {

                    JObject Live = Config.GetConfigFromLiveName(j["live"].ToString());
                    if (j["state"].ToString()=="直播中"&&checkBox1.Checked==true)
                    { 
                        notifyIcon1.BalloonTipTitle = Live["live"].ToString() + "|" + j["room"].ToString() + (j["roomhost"].ToString() == "" ? " " :"|"+j["roomhost"].ToString()+ "") + "|直播中";
                        notifyIcon1.BalloonTipText = j["roomname"].ToString()+" 正在直播中 ";
                        notifyIcon1.ShowBalloonTip(0);
                    }
                    if (j["state"].ToString() == "未直播" && checkBox2.Checked == true)
                    {
                        notifyIcon1.BalloonTipTitle = Live["live"].ToString() + "|" + j["room"].ToString() + (j["roomhost"].ToString() == "" ? " " : "|" + j["roomhost"].ToString()+ "") + "|未直播";
                        notifyIcon1.BalloonTipText = j["roomname"].ToString() + " 停止直播了 ";
                        notifyIcon1.ShowBalloonTip(0);
                    }
                    
                } 
                listBox1.Items[GetRoomStateInfoIndex] = LiveRoom.GetRoomInfo(j);
            }
           
            GetRoomStateInfoIndex = GetRoomStateInfoIndex + 1;
            if (GetRoomStateInfoIndex == listBox1.Items.Count)
            {
                GetRoomStateInfoIndex = 0;
            }
       
        }
 

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                
            }
        }

        private void notifyIcon1_Click(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

 
    }
}
