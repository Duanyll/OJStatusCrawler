using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace OJStatusCrawler
{
    class BZOJCrawler : OJCrawler
    {
        const string BZOJUrl = "https://www.lydsy.com/JudgeOnline/status.php?problem_id=&user_id={0}&language=-1&jresult=4";
        const string BZOJUrlPrefix = "https://www.lydsy.com/JudgeOnline/";

        public override List<StatusLog> Go(string UserName)
        {
            List<StatusLog> ret = new List<StatusLog>();
            string Url = string.Format(BZOJUrl, UserName);
            while (Url != "")
            {
                WebCrawler crawler = new WebCrawler();
                crawler.OnCompleted += (s, e) =>
                {
                    Regex reg = new Regex(@"problem.php\?id=([0-9]{1,4})");//原来想用html解析器，太麻烦了。
                    foreach(var m in reg.Matches(e.PageSource)){
                        StatusLog log = new StatusLog()
                        {
                            ProblemID = m.ToString(),
                            UserName = UserName,
                            Status = OJStatus.AC
                        };
                        ret.Add(log);
                    }
                    Regex findNextPage = new Regex(@"Previous Page</a>\]&nbsp;&nbsp;\[<a href=(.+?)>Next");
                    string match = findNextPage.Match(e.PageSource).ToString();
                    string newNext = BZOJUrlPrefix + match;
                    Debug.WriteLine("下一页地址：" + newNext);
                        //TODO: 修复翻页功能
                    if (newNext == Url)
                    {
                        Url = "";
                    }
                    else
                    {
                        Url = newNext;
                    }
                };
                crawler.Start(new Uri(Url)).Wait();//线程好麻烦
                Thread.Sleep(1000);
            }
            return ret;
        }
    }
}
