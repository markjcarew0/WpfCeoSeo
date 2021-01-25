using System.Collections.Specialized;
using System.Net;

namespace CeoSeoDataAccess
{
    public static class GoogleData
    {
        public static string GetGoogleSearchData(string searchData)
        {
            string uriString = "http://www.google.com/search";
            string keywordString = searchData;

            WebClient webClient = new WebClient();

            NameValueCollection nameValueCollection = new NameValueCollection();
            nameValueCollection.Add("q", keywordString);

            webClient.QueryString.Add(nameValueCollection);
            var data = webClient.DownloadString(uriString);

            return data;
        }
    }
}
