﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Spider
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main()
        {
            //ServiceBase[] ServicesToRun;
            //ServicesToRun = new ServiceBase[] 
            //{ 
            //    //new Xwlb() 
            //    new TenderService()
            //};
            //ServiceBase.Run(ServicesToRun);

            ITender tenders = new TenderGdGpo();
            tenders.GetPackage();
        }
    }
}
