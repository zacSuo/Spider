using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace Spider
{
    public struct PackageInfo
    {
        public int ID;

        /// <summary>
        /// 网址
        /// </summary>
        public string Url;
        /// <summary>
        /// 项目名称
        /// </summary>
        public string Name;
        /// <summary>
        /// 发布时间
        /// </summary>
        public string Time;
        /// <summary>
        /// 发包单位
        /// </summary>
        public string Seller;
        /// <summary>
        /// 项目编号
        /// </summary>
        public string Number;
        /// <summary>
        /// 招标价格
        /// </summary>
        public string Price;
        /// <summary>
        /// 索引编号
        /// </summary>
        public string Index;

        public override bool Equals(object obj)
        {
            if (obj.GetType() != this.GetType()) return false;

            PackageInfo item = (PackageInfo)obj;
            return item.Number.Equals(this.Number);
        }

        public override int GetHashCode()
        {
            return this.Number.GetHashCode();
        }
    }

     class GetPage
    {
         //有效关键字
         public string GetValidKey
         {
             get 
             {
                 return "(机器人|物联网|实验室|人工智能|AI|IOT|嵌入式|控制|自动化|采购)";
             }
         }

        /// <summary>
        /// Get网页信息（UTF8编码）
        /// </summary>
        /// <param name="strUrl">网页地址</param>
        /// <returns>网页内容</returns>
        public string GetPageInfo(string strUrl)
        {
            return GetPageInfo(strUrl, Encoding.UTF8);
        }
        /// <summary>
        /// Get网页信息
        /// </summary>
        /// <param name="strUrl">网页地址</param>
        /// <param name="codeType">文字编码</param>
        /// <returns>网页内容</returns>
        protected string GetPageInfo(string strUrl, Encoding codeType)
        {
            System.Net.HttpWebRequest request;
            // 创建一个HTTP请求
            request = (System.Net.HttpWebRequest)WebRequest.Create(strUrl);
            request.Method = "get";
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/78.0.3904.87 Safari/537.36";
            request.Accept = "application/json, text/javascript, */*; q=0.01";

            ServicePointManager.ServerCertificateValidationCallback = ValidateServiceCertificate;
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            StreamReader myreader = new System.IO.StreamReader(response.GetResponseStream(), codeType);
            string responseText = myreader.ReadToEnd();
            myreader.Close();

            return responseText;

        }

        /// <summary>
        /// 默认通过证书
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        /// <param name="sslPolicyError"></param>
        /// <returns></returns>
        protected bool ValidateServiceCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyError)
        {
            return true;
        }
    }
}
