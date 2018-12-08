using System;

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
            Console.ReadKey();
        }
    }
}
