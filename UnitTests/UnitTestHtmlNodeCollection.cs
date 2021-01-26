using CeoSeoDataAccess;
using HtmlAgilityPack;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class UnitTestHtmlNodeCollection
    {
        [TestMethod]
        public void EnsureThatAndNonEmptyHtmlNodeCollectionIsReturnedFromCreateEmptyHtmlNodeCollection()
        {
            // arrange
            HtmlNodeCollection list = new HtmlNodeCollection(null);

            // act
            var nodes = GoogleData.CreateEmptyHtmlNodeCollection(list);

            // assert
            var countNodes = 0;
            foreach (HtmlNode oneNode in nodes)
            {
                countNodes++;
            }

           Assert.IsTrue(countNodes == 1);
        }
    }
}
