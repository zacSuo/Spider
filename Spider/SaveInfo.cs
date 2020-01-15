using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Spider
{
    class SaveInfo
    {
        public void SaveText(List<PackageInfo> list)
        {
            string strFileName = "C://zb/" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
            
            foreach (PackageInfo info in list)
            {
                string str = string.Format("项目：{0}\t时间：{1}\t采购方：{2}\t价格：{3}\t序号：{4}\t编号：{5}\t地址：{6}\t",
                    info.Name, info.Time, info.Seller, info.Price, info.Index, info.Number, info.Url);
                this.WriteTextFile(strFileName,str);
            }
        }

        public void WriteTextFile(string fileName, string content)
        {
            FileStream fs = File.OpenWrite(fileName);
            fs.Position = fs.Length;
            byte[] tmpBytes = Encoding.UTF8.GetBytes(content);
            fs.Write(tmpBytes, 0, tmpBytes.Length);
            fs.Flush();
            fs.Close();
        }

        public void WriteLog(string log)
        {
            string strFileName = "C://zb/logs.txt";
            this.WriteTextFile(strFileName, log);
        }
    }
}
