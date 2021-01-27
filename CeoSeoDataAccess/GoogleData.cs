using HtmlAgilityPack;
using System.Collections.Generic;
using System.Threading;

namespace CeoSeoDataAccess
{
    public static class GoogleData
    {
        /// <summary>
        /// run the google query against the query string passed to it
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public static HtmlNodeCollection GetGoogleSearchData(string query)
        {
            try
            {
                // throw new System.Exception();
                // build up the query string to search google with
                string url = "http://www.google.com/search?num=100&q=" + query.Replace(" ", "+"); // conveyancing+software";
                var result = new HtmlWeb().Load(url);

                // get the query filtered nodes 
                var nodes = result.DocumentNode.SelectNodes("//html//body//div[@class='g']");
                return nodes;
            }
            catch
            {
                HtmlNodeCollection list = new HtmlNodeCollection(null);
                return CreateEmptyHtmlNodeCollection(list);
            }
        }

        /// <summary>
        /// inform the user that the system returned no data 
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static HtmlNodeCollection CreateEmptyHtmlNodeCollection(HtmlNodeCollection list)
        {
            HtmlDocument doc = new HtmlDocument();
            HtmlNode textNode = doc.CreateElement("title");
            textNode.InnerHtml = HtmlDocument.HtmlEncode("No data was returned from the submitted query");
            list.Add(textNode);
            return list;
        }
    }
}
