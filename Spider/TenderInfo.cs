using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Spider
{
    public interface ITender
    {
        List<PackageInfo> GetPackage();
        /// <summary>
        /// 两次查询需要等待时间
        /// </summary>
        int SleepMinutes
        {
            get;
        }

        /// <summary>
        /// 单次还需要等待时间
        /// </summary>
        int NeedWaitTime
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 广东省教育部门零散采购
    /// http://www.gdedulscg.cn/
    /// </summary>
    public class TenderGdEduLSCG : ITender
    {
        public int SleepMinutes
        {
            get
            {
                return 60;
            }
        }
        public int NeedWaitTime
        {
            get;
            set;
        }


        public List<PackageInfo> GetPackage()
        {
            const string strUrl = "http://www.gdedulscg.cn/";
            GetPage page = new GetPage();
            string strUrlLink = "http://www.gdedulscg.cn/home/bill/billlist";
            string strContent = page.GetPageInfo(strUrlLink);

            string[] strRegs = {"list_title_num_data.*</div>",//编号
                                    "list_title_unit_data.*</div>",//学校（采购单位）
                                    "<div.*list_title_theme_data.*</div>",//项目名称
                                    "list_title_high_data.*</div>",//学校报价
                                    "list_title_time_data.*</div>",//发布时间
                                    page.GetValidKey,
                                    "=\"[^\"]*\"",   //项目名称 - 具体文字
                                    ">.*</div>" ,    //编号，价格，学校，时间
                                    "see_info.*;",   //序号
                                };
            List<PackageInfo> rList = new List<PackageInfo>();

            Regex[] rUrl = new Regex[strRegs.Length];
            for (int i = 0; i < strRegs.Length; i++)
            {
                rUrl[i] = new Regex(strRegs[i]);
            }

            MatchCollection mList = rUrl[2].Matches(strContent);
            for (int i = 0; i < mList.Count; i++)
            {
                if (rUrl[5].Matches(mList[i].Value).Count == 0)
                {//没发现
                    continue;
                }

                try
                {
                    rList.Insert(0, new PackageInfo()
                    {
                        Name = rUrl[6].Matches(mList[i].Value)[0].Value.Substring(2),
                        Number = rUrl[7].Matches(rUrl[0].Matches(strContent)[i].Value)[0].Value.Substring(1),
                        Index = rUrl[8].Matches(mList[i].Value)[0].Value.Substring(8),
                        Price = rUrl[7].Matches(rUrl[3].Matches(strContent)[i].Value)[0].Value.Substring(1),
                        Seller = rUrl[7].Matches(rUrl[1].Matches(strContent)[i].Value)[0].Value.Substring(1),
                        Time = rUrl[7].Matches(rUrl[4].Matches(strContent)[i].Value)[0].Value.Substring(1),
                        Url = strUrl
                    });
                }
                catch (Exception e)
                {
                    new SaveInfo().WriteError(e);
                }
            }
            return rList;
        }
    }

    /// <summary>
    /// 广东省政府采购网
    /// http://www.gdgpo.gov.cn/
    /// </summary>
    public class TenderGdGpo : ITender
    {

        private void ReadInfo(GetPage page, string strLink, List<PackageInfo> rList)
        {
            const string strUrl = "http://www.gdgpo.gov.cn";
            string strContent = page.GetPageInfo(strLink);

            string[] strRegs = {"title=\"[^\"]*",//项目名称
                                    "<em>.*<",//时间
                                    page.GetValidKey,
                                    "/showNotice/id[^\"]*",   //详情url地址（序号）
                                    "项目编号[^<]*</span>" ,    //编号
                                    "预算金额（元）[^(span)]*span",   //价格
                                    "受.*的委托",//学校
                                };


            Regex[] rUrl = new Regex[strRegs.Length];
            for (int i = 0; i < strRegs.Length; i++)
            {
                rUrl[i] = new Regex(strRegs[i]);
            }
            MatchCollection mList = rUrl[0].Matches(strContent);
            for (int i = 0; i < mList.Count; i++)
            {
                if (rUrl[2].Matches(mList[i].Value).Count == 0)
                {//没发现
                    continue;
                }

                try
                {
                    PackageInfo p = new PackageInfo();

                    p.Name = mList[i].Value.Substring(7);
                    p.Time = rUrl[1].Matches(strContent)[i].Value.Substring(4);
                    p.Time = p.Time.Substring(0, p.Time.Length - 1);
                    p.Index = rUrl[3].Matches(strContent)[i].Value;
                    p.Url = strUrl;
                    
                    string strDetail = page.GetPageInfo(strUrl + p.Index);
                    
                    p.Number = rUrl[4].Match(strDetail).Value.Substring(5);
                    p.Number = p.Number.Substring(0, p.Number.IndexOf('<'));
                    p.Price = rUrl[5].Match(strDetail).Value.Substring(8);
                    p.Price = p.Price.Substring(0, p.Price.IndexOf('<'));
                    p.Seller = rUrl[6].Match(strDetail).Value;
                    rList.Add(p);
                }
                catch (Exception e)
                {
                    new SaveInfo().WriteError(e);
                }
            }
        }
        public List<PackageInfo> GetPackage()
        {
            GetPage page = new GetPage();
            string[] strUrlLink = {"http://www.gdgpo.gov.cn/queryMoreCityCountyInfoList2.do",
                                    "http://www.gdgpo.gov.cn/queryMoreCityCountyInfoList2/channelCode/00051.html"
                                  };

            List<PackageInfo> rList = new List<PackageInfo>();
            foreach (string str in strUrlLink)
            {
                this.ReadInfo(page, str, rList);
            }
            return rList;
        }
        public int SleepMinutes
        {
            get { return 30; }
        }
        public int NeedWaitTime
        {
            get;
            set;
        }
    }
}
