using System;

namespace OJStatusCrawler
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("请输入用户名:");
            string UserName = Console.ReadLine();
            BZOJCrawler crawler = new BZOJCrawler();
            var list = crawler.SearchForACStatus(UserName);
            foreach(var i in list)
            {
                Console.WriteLine(i.ProblemID);
            }
            Console.ReadKey();
        }
    }
}
