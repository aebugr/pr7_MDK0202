using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HttpNewsPAT
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                using (StreamWriter file = new StreamWriter("debug.log", true))
                {
                    Trace.Listeners.Add(new TextWriterTraceListener(file));
                    Trace.AutoFlush = true;
                    SingIn("student", "Asdfg123");
                    Cookie tokenCookie = new Cookie("token", "your_token_here") { Domain = "news.permaviat.ru" };
                    GetContent(tokenCookie);
                    string htmlCode = GetHtml("http://news.permaviat.ru/main");
                    ParsingHtml(htmlCode);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла ошибка: {ex.Message}");
                Trace.WriteLine($"Произошла ошибка: {ex.Message}");
            }
            Console.Read();
        }
        public static void SingIn (string Login, string Password)
        {
            string url = "http://10.111.20.16/ajax/login.php";
            Debug.WriteLine($"Выполняем запрос: {url}");
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.CookieContainer = new CookieContainer();
            string postData = "";
            byte[] Data = Encoding.ASCII.GetBytes(postData);
            request.ContentLength = Data.Length;
            using (var stream = request.GetRequestStream())
            {
                stream.Write(Data, 0, Data.Length);
            }
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Debug.WriteLine($"Статус выполнения: {response.StatusCode}");
            string responseFromServer = new StreamReader(response.GetResponseStream()).ReadToEnd();
            Console.WriteLine(responseFromServer);
        }
        public static void GetContent(Cookie Token)
        {
            string url = "http://10.111.20.16/main";
            Debug.WriteLine($"Выполняем запрос: {url}");
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.CookieContainer = new CookieContainer();
            request.CookieContainer.Add(Token);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Debug.WriteLine($"Статус выполнения: {response.StatusCode}");
            string responseFromServer = new StreamReader(response.GetResponseStream()).ReadToEnd();
            Console.WriteLine(responseFromServer);
        }
        public static void ParsingHtml(string htmlCode)
        {
            var html = new HtmlDocument();
            html.LoadHtml(htmlCode);
            var Document = html.DocumentNode;
            IEnumerable<HtmlNode> Content = Document.Descendants(0).Where(n => n.HasClass(""));
            foreach (HtmlNode content in Content)
            {
                var src = content.ChildNodes[1].GetAttributeValue("src", "none");
                var name = content.ChildNodes[3].InnerText;
                var description = content.ChildNodes[5].InnerText;
                Console.WriteLine(name + "\n" + "Изображение: " + src + "\n" + "Описание: " + description);
            }
        }
        public static string GetHtml(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string htmlCode = new StreamReader(response.GetResponseStream()).ReadToEnd();
            return htmlCode;
        }
    }
}
