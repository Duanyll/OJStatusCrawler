using System;
using System.IO;

namespace OJStatusCrawler
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("请输入用户名:");
            string UserName = Console.ReadLine();
            var crawler = new HDUCrawler();
            var list = crawler.SearchForACStatus(UserName);
            Console.WriteLine("抓取完毕");
            foreach(var i in list)
            {
                Console.WriteLine(i.ProblemID);
            }
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"result.md", true))
            {
                file.WriteLine();
                file.WriteLine($"# {UserName}的AC提交记录");
                file.WriteLine($"抓取时间:{DateTime.Now}");
                file.WriteLine("|题目编号|链接|");
                file.WriteLine("|-|-|");
                foreach (var item in list)
                {
                    file.WriteLine($"|{item.ProblemID}|[点击查看]({item.URL})|");
                }
                file.WriteLine("----------------------------------");
            }
            Console.WriteLine("按任意键退出");
            Console.ReadKey();
        }
    }
}
