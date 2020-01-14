using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Spider
{
    partial class TenderService : ServiceBase
    {
        bool saveFlag = false;
        List<PackageInfo> itemList = new List<PackageInfo>();
        ITender[] tenders = {new TenderGdEduLSCG(),
                                new  TenderGdGpo()
                            };
        public TenderService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            string strFileName = "C://zb/logs.txt";
            new SaveInfo().WriteTextFile(strFileName, DateTime.Now.ToString() + "START");

            // TODO: 在此处添加代码以启动服务。
            Timer timer = new Timer();
            timer.Interval = 60000; //1分钟执行一次
            timer.Elapsed += new ElapsedEventHandler(this.OnTimer);
            timer.Start();
        }

        private void OnTimer(object sender, ElapsedEventArgs args)
        {
            if(DateTime.Now.Hour == 0 && DateTime.Now.Minute == 0)
            {
                this.saveFlag = true;
            }
            foreach (ITender item in tenders)
            {
                if (item.NeedWaitTime > 0)
                {
                    item.NeedWaitTime--;
                    continue;
                }
                item.NeedWaitTime = item.SleepMinutes;
                List<PackageInfo> tempList = item.GetPackage();
                foreach (PackageInfo page in tempList)
                {
                    if (itemList.Contains(page))
                    {
                        itemList.Add(page);
                    }
                }
            }

            if (saveFlag)
            {
                new SaveInfo().SaveText(itemList);
                itemList.RemoveAll(null);
                this.saveFlag = false;
            }
        }

        protected override void OnStop()
        {
            // TODO: 在此处添加代码以执行停止服务所需的关闭操作。

            string strFileName = "C://zb/logs.txt";
            new SaveInfo().WriteTextFile(strFileName, DateTime.Now.ToString() + "END");
        }
    }
}
