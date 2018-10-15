using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace LiveState
{
    class LiveRoom
    {
        //直播间数组
        private static JArray LiveRoomList = new JArray();

        //直播平台名字的最长长度，用于格式化字符串
        private static int LiveRoomInfoLiveNameMaxLen = 0;

        //直播间ID的最长长度，用于格式化字符串
        private static int LiveRoomInfoLiveIdMaxLen = 0;

         

        //直播间存储文件路径
        private static string LiveStateRoomInfoFilePath = Directory.GetCurrentDirectory() + "\\LiveStateRoom.json";

        /// <summary>
        /// 添加直播间信息
        /// </summary>
        public static void AddRoomInfo(string live,string id)
        {
            foreach (JObject v in LiveRoomList)
            {
                if (v["room"].ToString() == id && v["live"].ToString() == live)
                {
                    return;
                }
            }
           
                if (live.Length > LiveRoomInfoLiveNameMaxLen)
                {
                    LiveRoomInfoLiveNameMaxLen = live.Length;
                }
           
                if (id.Length > LiveRoomInfoLiveIdMaxLen)
                {
                    LiveRoomInfoLiveIdMaxLen = id.Length;
                }
            
   
            JObject j = new JObject();
            j["live"] = live;
            j["room"] = id;
            j["state"] = "未直播";
            j["roomname"] = "";
            j["roomhost"] = "";
            LiveRoomList.Add(j);
        }

        /// <summary>
        /// 删除特定的直播间信息
        /// </summary>
        /// <param name="liveinfo"></param>
        public static void RemoveRoomInfo(int index)
        {
            LiveRoomList.RemoveAt(index);
            SaveRoomInfo();
        }

        /// <summary>
        /// 保存直播间信息至存储文件中
        /// </summary>
        public static void SaveRoomInfo()
        {
            StreamWriter fs = new StreamWriter(LiveStateRoomInfoFilePath, false);
            fs.Write(LiveRoomList.ToString());
            fs.Close();
        }

        /// <summary>
        /// 从存储文件中读取直播间信息
        /// </summary>
        public static void LoadRoomInfo()
        {
            if (CheckRoomInfoFileExist() == false) return;
            StreamReader fs = new StreamReader(LiveStateRoomInfoFilePath);
            string cstr = fs.ReadToEnd();
            fs.Close();
            LiveRoomList.Clear();
            try
            {

                JArray j = JArray.Parse(cstr);
                foreach(JObject v in j)
                {
                    if(v.Property("live")!=null&&v.Property("room")!=null)
                    {
                        AddRoomInfo(v["live"].ToString(), v["room"].ToString());

                    }
                    else
                    {
                        MessageBox.Show("该房间信息已损坏，请重新添加,出错内容:\n" + v.ToString());
                    }
                }
            }
            catch
            {
                MessageBox.Show("配置文件遭破坏！请不要修改文件("+ LiveStateRoomInfoFilePath + ")的内容！该配置文件已删除，请重新添加");
            }
            SaveRoomInfo();
        }

        /// <summary>
        /// 检查直播间信息文件是否存在
        /// </summary>
        /// <returns></returns>
        public static Boolean CheckRoomInfoFileExist()
        {
            return File.Exists(LiveStateRoomInfoFilePath);
        }


        /// <summary>
        /// 以字符串的形式返回直播间的状态
        /// </summary>
        /// <param name="j"></param>
        /// <returns></returns>
        public static string GetRoomInfo(JObject v)
        {
           
            string tmp = "";       
            tmp = PadRightEx(v["live"].ToString(),LiveRoomInfoLiveNameMaxLen);
            tmp = tmp + "|" + v["room"].ToString().PadRight(LiveRoomInfoLiveIdMaxLen);
            tmp = tmp + "|" + v["state"].ToString();
            tmp = tmp + "|" + v["roomhost"].ToString();
            tmp = tmp + (v["roomhost"].ToString()==""?" ":"|") + v["roomname"].ToString();
            return tmp;
        }

        /// <summary>
        /// 以字符串数组的形式返回每一个直播间的状态
        /// </summary>
        /// <returns></returns>
        public static List<string> GetRoomInfoList()
        {
            List<string> nlist = new List<string>(); 
            foreach(JObject v in LiveRoomList)
            {
                nlist.Add(GetRoomInfo(v));
            }
            return nlist;
        }

        /// <summary>
        /// 以JARRAY形式返回房间列表信息
        /// </summary>
        /// <returns></returns>
        public static JArray GetRoomInfoJarray()
        {
            return LiveRoomList;
        }

        /// <summary>
        /// 修正中文对齐
        /// </summary>
        /// <param name="str"></param>
        /// <param name="totalByteCount"></param>
        /// <returns></returns>
        private static string PadRightEx(string str, int totalByteCount)
        {
            Encoding coding = Encoding.GetEncoding("gb2312");
            int dcount = 0;
            foreach (char ch in str.ToCharArray())
            {
                if (coding.GetByteCount(ch.ToString()) == 2)
                    dcount++;
            }
            string w = str.PadRight(totalByteCount - dcount);
            return w;
        }
    }
}
