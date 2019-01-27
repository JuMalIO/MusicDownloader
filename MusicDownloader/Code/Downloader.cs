using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MusicDownloader.Code
{
    public class Downloader
    {
        private static readonly string URL = "https://www.youtube.com/results?search_query=";

        public string[] GetSongs(string file)
        {
            return File.ReadAllLines(file);
        }

        public void Download(string searchText, string binaryFolder, string outputFolder)
        {
            var song = GetSongUrlAndName(searchText);

            var url = song.Item1;
            var outputFile = outputFolder + song.Item2;

            if (!File.Exists(outputFile + ".mp3"))
            {
                var process = new Process();
                var startInfo = new ProcessStartInfo();
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.FileName = binaryFolder + @"youtube-dl.exe";
                startInfo.Arguments = $"-f bestaudio[ext=m4a] --extract-audio --audio-format mp3 --output \"{outputFile}.%(ext)s\" {url}";
                process.StartInfo = startInfo;
                process.Start();
                process.WaitForExit();
            }
        }

        private Tuple<string, string> GetSongUrlAndName(string searchText)
        {
            searchText = HttpUtility.UrlEncode(searchText);

            var html = DownloadHtml(URL + searchText);

            var list = GetUrlsAndTitles(html);

            if (list.Count > 0)
            {
                return list[0];
            }

            return null;
        }

        private static string DownloadHtml(string url)
        {
            using (var client = new WebClient())
            {
                var htmlCode = client.DownloadString(url);
                return htmlCode;
            }
        }

        private static List<Tuple<string, string>> GetUrlsAndTitles(string html)
        {
            int index;
            var list = new List<Tuple<string, string>>();
            const string searchFor = "<a href=\"/watch?v=";
            while ((index = html.IndexOf(searchFor)) > 0)
            {
                html = html.Substring(index + searchFor.Length, html.Length - index - searchFor.Length);

                var i = html.IndexOf("\"");
                string code = html.Substring(0, i);
                i = html.IndexOf(">");
                string subHtml = html.Substring(0, i);
                string searchFor2 = "title=\"";
                i = subHtml.IndexOf(searchFor2);
                string title = subHtml.Substring(i + searchFor2.Length, subHtml.Length - i - searchFor2.Length);
                i = title.IndexOf("\"");
                title = HttpUtility.HtmlDecode(title.Substring(0, i));

                foreach (char c in Path.GetInvalidFileNameChars())
                {
                    title = title.Replace(c, '_');
                }

                var key = "https://www.youtube.com/watch?v=" + code;
                list.Add(new Tuple<string, string>(key, title));
            }
            return list;
        }
    }
}
