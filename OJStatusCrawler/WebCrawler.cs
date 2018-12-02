using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OJStatusCrawler
{
    public class OnStartEventArgs
    {
        public OnStartEventArgs(Uri uri)
        {
            Uri = uri;
        }

        public Uri Uri { get; set; }
    }

    public class OnCompletedEventArgs
    {
        public Uri Uri { get; set; }
        public int ThreadID { get; set; }
        public long millseconds { get; set; }
        public string PageSource { get; set; }

        public OnCompletedEventArgs(Uri uri,int threadid,long time,string source)
        {
            Uri = uri;
            ThreadID = threadid;
            millseconds = time;
            PageSource = source;
        }
    }

    class WebCrawler
    {
        public event EventHandler<OnStartEventArgs> OnStart;
        /// <summary>
        /// 爬虫完成事件
        /// </summary>
        public event EventHandler<OnCompletedEventArgs> OnCompleted;

        /// <summary>
        /// 爬虫出错事件
        /// </summary>
        public event EventHandler<Exception> OnError;
        /// <summary>
        /// 定义cookie容器
        /// </summary>
        public CookieContainer CookieContainer { get; set; }

        /// <summary>
        /// 异步创建爬虫
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="proxy"></param>
        /// <returns></returns>
        public async Task<string> Start(Uri uri, WebProxy proxy = null)
        {
            return await Task.Run(() =>
            {
                var pageSource = string.Empty;
                try
                {
                    if (this.OnStart != null)
                        this.OnStart(this, new OnStartEventArgs(uri));
                    Stopwatch watch = new Stopwatch();
                    watch.Start();
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                    request.Accept = "*/*";
                    //定义文档类型及编码
                    request.ContentType = "application/x-www-form-urlencoded";
                    request.AllowAutoRedirect = false;//禁止自动跳转
                    request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.113 Safari/537.36";
                    //定义请求超时事件为5s
                    request.Timeout = 5000;
                    //长连接
                    request.KeepAlive = true;
                    request.Method = "GET";
                    //设置代理服务器IP，伪装请求地址
                    if (proxy != null)
                        request.Proxy = proxy;
                    //附加Cookie容器
                    request.CookieContainer = this.CookieContainer;
                    //定义最大链接数
                    request.ServicePoint.ConnectionLimit = int.MaxValue;
                    //获取请求响应
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    //将Cookie加入容器，保持登录状态
                    foreach (Cookie cookie in response.Cookies)
                        this.CookieContainer.Add(cookie);
                    //获取响应流
                    Stream stream = response.GetResponseStream();
                    //以UTF8的方式读取流
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    //获取网站资源
                    pageSource = reader.ReadToEnd();
                    watch.Stop();
                    //获取当前任务线程ID
                    var threadID = Thread.CurrentThread.ManagedThreadId;
                    //获取请求执行时间
                    var milliseconds = watch.ElapsedMilliseconds;
                    reader.Close();
                    stream.Close();
                    request.Abort();
                    response.Close();
                    OnCompleted?.Invoke(this, new OnCompletedEventArgs(uri, threadID, milliseconds, pageSource));
                }
                catch (Exception ex)
                {
                    OnError?.Invoke(this, ex);

                }
                return pageSource;
            });
        }
    }
}
