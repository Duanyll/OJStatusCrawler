using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace OJStatusCrawler
{
    class POJCrawler : OJCrawler
    {
        const string POJUrl = "http://poj.org/status?user_id={0}&result=0";
        const string POJUrlPrefix = "http://poj.org/";
        const string ProblemIDMatcher = @"problem\?id=([0-9]{1,4})";
        const string NextPageMatcher = @"Previous Page</font></a>]&nbsp;&nbsp;\[<a href=(.+?)><font color=blue>Next Page</font>";
        const string ProblemPageURL = "http://poj.org/problem?id={0}";

        public override List<StatusLog> SearchForACStatus(string UserName)
        {
            List<StatusLog> ret = new List<StatusLog>();
            string Url = string.Format(POJUrl, UserName);
            while (Url != "")
            {
                WebCrawler crawler = new WebCrawler();
                crawler.OnCompleted += (s, e) =>
                {
                    Regex reg = new Regex(ProblemIDMatcher);//原来想用html解析器，太麻烦了。
                    foreach (var m in reg.Matches(e.PageSource))
                    {
                        StatusLog log = new StatusLog()
                        {
                            ProblemID = m.ToString().Substring(11),
                            URL = string.Format(ProblemPageURL, m.ToString().Substring(11)),
                            UserName = UserName,
                            Status = OJStatus.AC
                        };
                        ret.Add(log);
                    }
                    Regex findNextPage = new Regex(NextPageMatcher);
                    var match = findNextPage.Match(e.PageSource);
                    string newNext = POJUrlPrefix + match.Groups[1].Value;
                    Debug.WriteLine("下一页地址：" + newNext);
                    if (newNext == Url)
                    {
                        Url = "";
                    }
                    else
                    {
                        Url = newNext;
                    }
                };
                crawler.OnError += (s, e) =>
                {
                    Console.WriteLine(e.Message);
                    throw e;
                };
                crawler.Start(new Uri(Url)).Wait();//线程好麻烦
                Thread.Sleep(1000);
            }
            return ret;
        }
    }
}
