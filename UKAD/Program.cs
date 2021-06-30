using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Xml;

namespace UKAD
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter link for evaluating a website performance");
            string link = Console.ReadLine();
            Console.WriteLine();
            Console.WriteLine();
            List<string> listFromWebsite = GetLinksFromWebsite(link);
            List<string> listFromSitemap = GetLinksFromXml(link + "/sitemap.xml");
           
            Console.WriteLine("Urls FOUNDED IN SITEMAP.XML but not founded after crawling a web site");
            Console.WriteLine();
            var mapNotWeb = listFromSitemap.Except(listFromWebsite).ToList();
            foreach (var item in mapNotWeb) {
                Console.WriteLine(item);
            }
           
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Urls FOUNDED BY CRAWLING THE WEBSITE but not in sitemap.xml");
            Console.WriteLine();
            var webNotMap = listFromWebsite.Except(listFromSitemap).ToList();
            foreach (var item in webNotMap)
            {
                Console.WriteLine(item);
            }
           
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("List with url and response time");
            Console.WriteLine();
            string [] allArr = listFromWebsite.Union(listFromSitemap).ToArray();
            PageForeSort [] sortArr = new PageForeSort[allArr.Length];

            for(int a = 0; a < allArr.Length; a++) {
                sortArr[a] = new PageForeSort();
                sortArr[a].link = allArr[a];
                sortArr[a].time = Response(allArr[a]);  

            }
            Array.Sort(sortArr, new PageComparer());
            foreach (var item in sortArr) {
                Console.WriteLine(item.link +" "+ item.time + "ms");
            }
           

            Console.WriteLine();
            Console.WriteLine("Urls(html documents) found after crawling a website: " + listFromWebsite.Count);
            Console.WriteLine("Urls found in sitemap: " + listFromSitemap.Count);
            
            Console.ReadLine();
        }
       

        public static List<string> GetLinksFromXml(string link)
        {

            WebClient client = new WebClient();
            string sitemap = client.DownloadString(link);
            XmlDocument document = new XmlDocument();
            document.LoadXml(sitemap);
            XmlNodeList xmlList = document.GetElementsByTagName("loc");
            List<string> resList = new List<string>();
            foreach (XmlNode item in xmlList)
            {
                resList.Add(item.InnerText);
            }
            return resList;
        }
        public static List<String> GetLinksFromWebsite(string link)
        {
            WebClient client = new WebClient();
            Queue<string> links = new Queue<string>();
            List<string> resList = new List<string>();
            links.Enqueue(link);
            try
            {
                while (links.Count > 0)
                {
                    string sitemap = client.DownloadString(links.Peek());
                    links.Dequeue();
                    HtmlDocument html = new HtmlDocument();
                    html.LoadHtml(sitemap);
                    resList.AddRange(html.DocumentNode
                    .SelectNodes("//a[starts-with(@href,'/')]")
                    .Select(node => link + node.Attributes["href"].Value)
                    .ToList());
                    foreach (var item in resList)
                    {
                        if (!links.Contains(item))
                        { links.Enqueue(item); }
                    }
                    links.Distinct();
                   resList = resList.Distinct().ToList();

                }
            }
            catch (Exception e) 
            { 
                
            }
            return resList;
        }
 
        public static long Response(string link) {
            
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(link);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                timer.Stop();
                response.Close();
            }
            catch (Exception e) 
            { 
                
            }
            return timer.ElapsedMilliseconds;
        }  
    }
}
