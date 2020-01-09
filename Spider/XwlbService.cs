using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Timers;
using System.Text.RegularExpressions;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Net;

namespace Spider
{
    public partial class Xwlb : ServiceBase
    {
        private SaveInfo saver = new SaveInfo();
        public Xwlb()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            string strFileName = "C://news/logs.txt";
            this.saver.WriteTextFile(strFileName,DateTime.Now.ToString() + "START");

            // Set up a timer that triggers every minute. 设置定时器
            Timer timer = new Timer();
            timer.Interval = 7200000; //2小时执行一次
            timer.Elapsed += new ElapsedEventHandler(this.OnTimer);
            timer.Start();
        }

        private void OnTimer(object sender, ElapsedEventArgs args)
        {
            int currentHour = DateTime.Now.Hour;
            int currentMinute = DateTime.Now.Minute;
            if (currentHour < 21 || currentHour > 22)
                return;//晚上9点到11点之间处理

            string info = this.GetNewsContent();
            string tmpSemi = "；";
            if (info.IndexOf("；\r\n") > 0)
                tmpSemi = "；\r\n";

            info = info.Replace(tmpSemi, "；\n");
            info = info.Replace(".", ". ");

            Regex rLast = new Regex("（《.*");
            info = rLast.Replace(info, string.Empty);

            info = info.Replace("：（1）", "：\n（1）");

            Regex rList = new Regex("（([0-9]{1,2})）");
            info = rList.Replace(info, "    *    ");

            info = info.Replace("：1", "：\n1");

            string strFileName = "C://news/" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
            this.saver.WriteTextFile(strFileName, info);
        }

        protected override void OnStop()
        {
            string strFileName = "C://news/logs.txt";
            this.saver.WriteTextFile(strFileName,DateTime.Now.ToString() + "END");
        }


        private string GetNewsContent()
        {
            GetPage page = new GetPage();
            string strUrlLink = "https://search.cctv.com/ifsearch.php?page=1&qtext=%E6%96%B0%E9%97%BB%E8%81%94%E6%92%AD&sort=relevance&pageSize=20&type=video&vtime=-1&datepid=1&channel=&pageflag=0&qtext_str=%E6%96%B0%E9%97%BB%E8%81%94%E6%92%AD";
            string strContent = page.GetPageInfo(strUrlLink);
            string strRegex = DateTime.Today.ToString("yyyyMMdd") + ".*http[^(\")]*shtml.*" + DateTime.Today.ToString("yyyy-MM-dd");
            Regex rDateUrl = new Regex(strRegex);
            strContent = rDateUrl.Match(strContent).Value;

            if (strContent.Equals(string.Empty))
                return string.Empty;

            strRegex = "http[^(\")]*shtml";
            Regex rUrl = new Regex(strRegex);
            strContent = rUrl.Match(strContent).Value;

            strContent = page.GetPageInfo(strContent.Replace("\\/", "/"));

            //有时内容会加换行导致p的正则无法匹配
            //Regex rContent = new Regex("<p>.*</p>");
            //strContent = rContent.Matches(strContent)[2].Value;
            //int strStart = strContent.IndexOf("本期节目");
            //strContent = strContent.Substring(strStart);
            //strContent = strContent.Substring(0, strContent.Length - 4);

            int strStart = strContent.IndexOf("本期节目主要内容");
            int strEnd = strContent.IndexOf("（《新闻联播》");
            if (strStart > 0 && strEnd > strStart)
                strContent = strContent.Substring(strStart, strEnd - strStart);
            else
                strContent = "内容解析错误";

            return strContent;
        }
    }
}
