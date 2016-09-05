using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;

namespace SimilarityMeasure
{
    class Spider
    {
        private Queue<string> _urls;
        private Dictionary<string, string> _data;
        public Dictionary<string, string> Data
        { get { return _data; } }

        public Spider()
        {
            _urls = new Queue<string>();
            _data = new Dictionary<string, string>();
        }

        public void AddUrl(string url)
        {
            _urls.Enqueue(url);
        }

        public void Fetch()
        {
            foreach (string url in _urls)
            {
                try
                {
                    string data = "";
                    HtmlWeb web = new HtmlWeb();
                    HtmlDocument htmlDoc = web.Load(url);

                    if (htmlDoc.DocumentNode != null)
                    {
                        HtmlNode bodyNode = htmlDoc.DocumentNode.SelectSingleNode("//body");

                        if (bodyNode != null)
                        {
                            var paragraphs = bodyNode.SelectNodes("//p");
                            foreach (var p in paragraphs)
                            {
                                data += " ";
                                data += p.InnerText;
                            }
                            _data.Add(url, data);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Empty document node at {0}", url);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Spider exception source: {0}", e.Message);
                }
            }
        }
    }
}
