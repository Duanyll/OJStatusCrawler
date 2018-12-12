using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Collections;

namespace OJStatusCrawler
{
    class Program
    {
        const string HelpText = @"
命令行使用方法:
user:   指定用户名,若不指定,则从stdin读入
oj:     指定OJ,若不指定,则从stdin读入
            目前支持bzoj,hdu,poj
output: 指定输出文件(默认result.md),附加到文件末尾
mode:   firstac(默认),ac,all(尚未支持)
reverse:是否倒序输出(默认true)

示例:
    dotnet run user=llf0703 oj=bzoj
        抓取llf0703的bzoj上的AC提交记录.
";
        static void Main(string[] args)
        {
            if (args.Length >= 1 && args[0] == "help")
            {
                Console.Write(HelpText);
                return;
            }
            var Config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>(){
                    {"user",""},
                    {"oj",""},
                    {"output","result.md"},
                    {"mode","firstac"},
                    {"reverse","true"}
                })
                .AddCommandLine(args)
                .Build();

            string UserName;
            if (Config["user"] == "")
            {
                Console.WriteLine("请输入用户名:");
                UserName = Console.ReadLine();
            }
            else
            {
                UserName = Config["user"];
            }

            string OJName;
            if (Config["oj"] == "")
            {
                Console.WriteLine("请输入OJ代码:");
                OJName = Console.ReadLine();
            }
            else
            {
                OJName = Config["oj"];
            }

            OJCrawler crawler;
            switch (OJName)
            {
                case "bzoj":
                    crawler = new BZOJCrawler();
                    break;
                case "hdu":
                    crawler = new HDUCrawler();
                    break;
                case "poj":
                    crawler = new POJCrawler();
                    break;
                default:
                    Console.WriteLine("OJ代码未知或尚未支持");
                    return;
            }

            List<StatusLog> list;
            switch(Config["mode"]){
                case "ac":
                    list = crawler.SearchForACStatus(UserName);
                    break;
                case "firstac":
                    var tmp = crawler.SearchForACStatus(UserName);
                    tmp.Reverse();
                    var solved = new HashSet<string>();
                    list = new List<StatusLog>();
                    foreach(var i in tmp){
                        if(!solved.Contains(i.ProblemID)){
                            solved.Add(i.ProblemID);
                            list.Add(i);
                        }
                    }
                    list.Reverse();
                    break;
                case "all":
                    Console.WriteLine("尚未支持该抓取模式");
                    return;
                default:
                    Console.WriteLine("未知抓取模式");
                    return;
            }

            switch(Config["reverse"]){
                case "false":
                    break;
                case "true":
                    list.Reverse();
                    break;
                default:
                    Console.WriteLine("reverse参数应为true或false");
                    return;
            }

            Console.WriteLine("抓取完毕");
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(Config["output"], true))
            {
                file.WriteLine();
                file.WriteLine($"# {UserName}在{OJName}上的AC提交记录");
                file.WriteLine($"抓取时间:{DateTime.Now}");
                file.WriteLine("|题目编号|链接|");
                file.WriteLine("|-|-|");
                foreach (var item in list)
                {
                    file.WriteLine($"|{item.ProblemID}|[点击查看]({item.URL})|");
                }
                file.WriteLine("----------------------------------");
            }
            Console.WriteLine($"已写入结果到{Config["output"]}");
            Console.WriteLine("按任意键退出");
            Console.ReadKey();
        }
    }
}
