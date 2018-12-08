using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace OJStatusCrawler
{
    class HDUCrawler : OJCrawler
    {
        const string HDUUrl = "http://acm.hdu.edu.cn/status.php?first=&pid=&user={0}&lang=0&status=5";
        const string HDUUrlPrefix = "http://acm.hdu.edu.cn";
        const string ProblemIDMatcher = @"showproblem.php\?pid=([0-9]{1,4})";
        const string NextPageMatcher = "< Prev Page</a><a style=\"margin-right:20px\" href=\"(.+?)\">Next Page ></a><a href=\"/status.php?";
        const string ProblemPageURL = "http://acm.hdu.edu.cn/showproblem.php?pid={0}";

        public override List<StatusLog> SearchForACStatus(string UserName)
        {
            List<StatusLog> ret = new List<StatusLog>();
            string Url = string.Format(HDUUrl, UserName);
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
                            ProblemID = m.ToString().Substring(20),
                            URL = string.Format(ProblemPageURL, m.ToString().Substring(20)),
                            UserName = UserName,
                            Status = OJStatus.AC
                        };
                        ret.Add(log);
                    }
                    Regex findNextPage = new Regex(NextPageMatcher);
                    var match = findNextPage.Match(e.PageSource);
                    if (match.Success)
                    {
                        string newNext = HDUUrlPrefix + match.Groups[1].Value;
                        Debug.WriteLine("下一页地址：" + newNext);
                        Url = newNext;
                    }
                    else
                    {
                        Url = "";
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
