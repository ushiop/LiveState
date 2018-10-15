using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LiveState
{
    class Config
    {

        /// <summary>
        /// LiveState配置文件路径
        /// </summary>
        private static string LiveStateConfigFilePath = Directory.GetCurrentDirectory() + "\\LiveStateConfig.json";

        /// <summary>
        /// 平台配置数组
        /// </summary>
        private static JArray Cfg = new JArray();



        /// <summary>
        /// 创建配置文件
        /// </summary>
        public static void CreateConfigFile()
        {
            JArray json = new JArray();
            JObject descript = new JObject();
            descript.Add("tip", "平台配置请按以下标准进行设置");
            descript.Add("1", "本程序仅能接受以JSON返回结果的直播平台WEBAPI");
            descript.Add("2", "每一个直播平台单独以一个JSON对象进行配置(如[{斗鱼},{熊猫}]");
            descript.Add("3", "JSON对象内，'live'属性为该平台的名字");
            descript.Add("4", "JSON对象内,'api'属性为该平台获取直播状态的API-URL（保留至参数，如http://a.com/api/roomid/10300,保留至最后一个/)");
            descript.Add("5", "JSON对象内,'title'属性为该平台API返回的JSON内标明房间名的属性，若为嵌套属性则用'属性#属性#属性'的格式填写（如返回为roominfo/title,则填写为roominfo#title)");
            descript.Add("6","JSON对象内，'state'属性为该平台API返回的JSON内标明直播状态的属性，若为嵌套属性则和房间名处理方式一样(该属性为空则仅判断房间名,房间名不为空则认为开播了[比如B站]");
            descript.Add("7", "JSON对象内,'state_tag'属性为该平台API返回的直播状态属性的值，若返回值和该属性值相等则为直播中,若'state'为空则该属性不起作用");
            descript.Add("8", "JSON对象内，'rage'属性为该平台API返回的字符串中需要进行的正则处理（比如B站删除括号跟分号），空格分割多个正则表达式");
            descript.Add("9", "JSON对象内，'hostname'属性为该平台API返回的主播名属性，若为嵌套属性则和房间名处理方式一样");
            descript.Add("10", "JSON对象内，可任意添加其他属性作为注释，本程序仅读取上述属性进行处理");
            descript.Add("11", "修改后请保存配置文件并重启程序进行读取或点击刷新读取");
            descript.Add("12", "本配置文件也是使用的JSON格式，程序报错时请检查格式正确与否");
            json.Add(descript);
            StreamWriter fs = new StreamWriter(LiveStateConfigFilePath);
            fs.Write(json.ToString());
            fs.Close();
        }

        /// <summary>
        /// 检查LiveState的配置文件是否存在
        /// </summary>
        /// <returns>true-存在,false-不存在</returns>
        public static Boolean CheckConfigFile()
        {
            return File.Exists(LiveStateConfigFilePath);
        }

        /// <summary>
        /// 返回平台配置中的平台名称数组列表
        /// </summary>
        /// <returns></returns>
        public static List<string> GetConfigLiveNameList()
        {
            List<string> nlist = new List<string>();
            foreach(JObject v in Cfg)
            {
                nlist.Add(v.GetValue("live").ToString());
            }
            return nlist;
        }

        /// <summary>
        /// 获取指定直播平台的配置
        /// </summary>
        /// <param name="livename"></param>
        /// <returns>null为没有该平台的配置</returns>
        public static JObject GetConfigFromLiveName(string livename)
        {
            foreach(JObject v in Cfg)
            {
                if(v["live"].ToString()==livename)
                {
                    return v;
                }
            }return null;
        }

        /// <summary>
        /// 从配置文件读取配置内容
        /// </summary>
        /// <returns></returns>
        public static JArray ReLoadConfig()
        { 
            StreamReader fs = new StreamReader(LiveStateConfigFilePath);
            string cstr = fs.ReadToEnd();
            fs.Close();
            Cfg.Clear();
            try
            {
                JArray j = JArray.Parse(cstr);
                JObject zero = (JObject)j[0];
                if(zero.Property("tip")!=null) j.RemoveAt(0);
                foreach(JObject v in j)
                {
                    if (v.Property("api") != null && v.Property("title") != null && v.Property("state") != null && v.Property("live") != null&&v.Property("state_tag")!=null&&v.Property("rage")!=null&&v.Property("hostname")!=null)
                    {
                        Cfg.Add(v);
                    }
                    else
                    {
                        MessageBox.Show("平台配置出错了，请检查JSON！ 出错内容:\n" + v.ToString());
                    }
                }
            }
            catch
            {
                MessageBox.Show("读取配置时出现了一个错误，请检查JSON格式是否正确");
            }
            return Cfg;
        }
         
        /// <summary>
        /// 返回配置文件的完整路径
        /// </summary>
        /// <returns></returns>
        public static string GetConfigPath()
        {
            return LiveStateConfigFilePath;
        }
    }
}
