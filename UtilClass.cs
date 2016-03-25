using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Windows.Forms;

namespace DownLoadYoutube
{
    public class UtilClass
    {
        public delegate void SetTextCallback(object sender, EventArgs eventArgs);
        public static WebClient WebClient; 
        public static readonly Stopwatch Sw = new Stopwatch();

        public UtilClass(string id)
        {
            var info = GetVideoInfo(id);
            Url = GetUrl(info);
            VideoName = GetTitle(info);
            FullPathVideoFileName = FileVideoDir + "\\" + VideoName + ".mp4";
            FullPathMp3FileName = FileAudioDir + "\\" + VideoName + ".mp3";
        }

        public UtilClass()
        {
            FileAudioDir = Directory.GetCurrentDirectory() + "\\" + "Mp3";
            FileVideoDir = Directory.GetCurrentDirectory() + "\\" + "Video";
            GreateDir(FileAudioDir);
            GreateDir(FileVideoDir);
        }

        private static string FileVideoDir { get; set; }
        private static string FileAudioDir { get; set; }


        public string VideoName { get; }

        public string Url { get; private set; }

        public string FullPathVideoFileName { get; private set; }

        public string FullPathMp3FileName { get; }

        private static void GreateDir(string dir)
        {
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }

        private static string GetVideoInfo(string id)
        {
            var url = @"http://www.youtube.com/get_video_info?video_id=" + id;
            var proxyRequest = (HttpWebRequest) WebRequest.Create(url);
            proxyRequest.Method = "GET";
            proxyRequest.ContentType = "application/x-www-form-urlencoded";
            proxyRequest.UserAgent =
                "Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US) AppleWebKit/532.5 (KHTML, like Gecko) Chrome/4.0.249.89 Safari/532.5";
            proxyRequest.KeepAlive = true;
            var resp = proxyRequest.GetResponse() as HttpWebResponse;
            string html;
            // ReSharper disable once PossibleNullReferenceException
            // ReSharper disable once AssignNullToNotNullAttribute
            using (var sr = new StreamReader(resp.GetResponseStream(), Encoding.GetEncoding(1251)))
                html = sr.ReadToEnd();
            html = html.Trim();

            if (html == string.Empty) MessageBox.Show(@"Не получилось получить информацию о файле");
            return html;
        }

        private static string GetUrl(string html)
        {
            var pattern = "url_encoded_fmt_stream_map=";
            var index = html.LastIndexOf(pattern, StringComparison.Ordinal) + pattern.Length;
            html = html.Substring(index);
            // var z = HttpUtility.HtmlDecode(HttpUtility.HtmlDecode(html));
            html = HttpUtility.UrlDecode(html);
            //Console.WriteLine(z);// Выводим в листбокс listbox, но можешь записывать и в файл

            if (html == null) return null;
            pattern = "url=";
            index = html.IndexOf(pattern, StringComparison.Ordinal) + pattern.Length;
            html = html.Substring(index);
            index = html.IndexOf("&", StringComparison.Ordinal);
            html = html.Remove(index);
            html = HttpUtility.UrlDecode(html);
            return html;
        }

        private static string GetTitle(string html)
        {
            var index = html.LastIndexOf("title=", StringComparison.Ordinal) + "title=".Length;
            html = html.Substring(index);
            index = html.IndexOf("&", StringComparison.Ordinal);
            if (index > 0) html = html.Remove(index);
            html = HttpUtility.UrlDecode(html);

            return html?.Trim(Path.GetInvalidFileNameChars());
        }
    }
}