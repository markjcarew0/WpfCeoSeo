// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitTestHtmlNodeCollection.cs" company="MarkJC">
//   Author Mark Carew
// </copyright>
// <summary>
//   Unit tests
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace UnitTests
{
    using CeoSeoCommon;
    using CeoSeoViewModels;
    using DataTransferObjects;
    using FakeItEasy;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Serilog;
    using System.Threading.Tasks;

    [TestClass]
    public class UnitTestMainWindowModel
    {
        [TestInitialize]
        public void TestSetUp()
        {
            // SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
        }

        [TestMethod]
        public void CanISeeTheViewModelAndInteractWithIt()
        {
            // arrange
            var logger = A.Fake<ILogger>();
            var googleDataService = new GoogleDataService();

            // we dont need the logger
            // we do need the google data service
            // we are running from within a unittest
            var viewModel = new MainWindowViewModel(logger, googleDataService, true);

            // act
            // viewModel.QuerySearchString = "Brown Cow";

            var rawNodes = GeneralDataBroker.GetSearchResponse("Brown Cow");

            var awaitTask = Task.Run(async () =>
            {
                await viewModel.ProcessRawNodes(rawNodes);
            });

            // unittest - break the rules 
            awaitTask.GetAwaiter().GetResult();

            // there should be some brow cow entries
            Assert.IsTrue(viewModel.SourceData.Count > 0);

            // non of them have anything to do with smokeball
            Assert.IsTrue(viewModel.RankList.Count == 0);

            // no filtered list has been created
            Assert.IsTrue(viewModel.ListData.Count == 0);
        }
    }
}
