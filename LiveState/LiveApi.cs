using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Web;
using System.Text.RegularExpressions;

namespace LiveState
{
    class LiveApi
    {

        /// <summary>
        /// 获取直播间状态,返回false表示获取失败,true表示成功并已更新至房间信息，等待刷新列表
        /// </summary>
        /// <param name="Room"></param>
        public static Boolean GetRoomState(JObject Room)
        {
            JObject Live = Config.GetConfigFromLiveName(Room["live"].ToString());
            if (Live == null)
            {
                MessageBox.Show("平台[" + Room["live"].ToString() + "]的配置文件不存在，请检查平台配置");
                return false;
            }
            string url = Live["api"].ToString() + Room["room"].ToString();
            WebClient w = new WebClient();
            w.Encoding = Encoding.UTF8;
            string ws = "";
            try
            {
                 ws= w.DownloadString(url);
            }
            catch
            {
                return false;
            }
            string rage = Live["rage"].ToString(); //获得平台结果正则匹配串
            foreach (string exp in rage.Split(new string[] { " "},StringSplitOptions.RemoveEmptyEntries))
            {
                Match r = Regex.Match(ws, exp);
                ws = r.Groups[0].ToString();
            }
            JObject j=JObject.Parse(ws);
            string title = FindJsonValueExistFromKey(j, Live["title"].ToString()) == null ? "" : FindJsonValueExistFromKey(j, Live["title"].ToString());
            string state= FindJsonValueExistFromKey(j, Live["state"].ToString()) == null ? "" : FindJsonValueExistFromKey(j, Live["state"].ToString());
            string hostname = FindJsonValueExistFromKey(j, Live["hostname"].ToString()) == null ? "" : FindJsonValueExistFromKey(j, Live["hostname"].ToString());
            Room["roomname"] = title;
            Room["roomhost"] = hostname;
            if((state==Live["state_tag"].ToString()&&Live["state"].ToString()!="")||(Live["state"].ToString()==""&&title!=""))
            {
                Room["state"] = "直播中";
            }
            else
            {
                Room["state"] = "未直播";
            }
            return true;
        }
 
        /// <summary>
        /// 在一个JSON对象中搜索findkey是否存在，存在则返回值，否则返回空
        /// </summary>
        /// <param name="json"></param>
        /// <param name="findkey"></param>
        /// <returns></returns>
        private static string FindJsonValueExistFromKey(JObject json,string findkey)
        {
            string result = null;
            string[] key = findkey.Split(new string[] { "#" }, StringSplitOptions.RemoveEmptyEntries);
            JObject Target = json;
            foreach(string k in key)
            {
                if (Target.Property(k) != null)
                {
                    if(Target.GetValue(k).GetType().ToString()=="Newtonsoft.Json.Linq.JObject")
                    {
                        Target = (JObject)Target.GetValue(k);
                    }
                    else
                    {
                        result = Target.GetValue(k).ToString();
                    }
                }
            }
            return result;
        }
    }
}
