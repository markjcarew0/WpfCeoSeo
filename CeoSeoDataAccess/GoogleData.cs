using HtmlAgilityPack;

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
            // build up the query string to search google with
            string url = "http://www.google.com/search?num=100&q=" + query.Replace(" ", "+"); // conveyancing+software";
            var result = new HtmlWeb().Load(url);

            // get the query filtered nodes 
            var nodes = result.DocumentNode.SelectNodes("//html//body//div[@class='g']");
            return nodes;
        }
    }
}
