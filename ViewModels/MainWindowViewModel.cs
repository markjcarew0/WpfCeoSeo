// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindowViewModel.cs" company="MarkJC">
//   Author Mark Carew
//   Date Sunday 24-01-2020
// </copyright>
// <summary>
// View Model for the Ceo Seo UI MainWindow.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CeoSeoViewModels
{
    using CeoSeoCommon;
    using DataTransferObjects;
    using HtmlAgilityPack;
    using Messaging;
    using ReactiveUI;
    using Serilog;
    using Serilog.Events;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Threading.Tasks;

    public class MainWindowViewModel : ViewModelBase, IDisposable, IMainWindowViewModel
    {
        /// <summary>
        /// google data service to create instances of google data
        /// </summary>
        IGoogleDataService googleDataService;

        /// <summary>
        /// logger 
        /// </summary>
        private ILogger logger;

        /// <summary>
        /// the data that is shown in the UI
        /// filtered by
        /// </summary>
        private List<IGoogleSearchData> listData;

        /// <summary>
        /// private backing field 
        /// for public Property QuerySearchString 
        /// </summary>
        private string querySearchString;

        /// <summary>
        /// private backing field 
        /// for public Property RankList 
        /// </summary>
        private List<int> rankList;

        /// <summary>
        /// private backing field
        /// for public Property SearchSpinnerOn 
        /// </summary>
        private bool searchSpinnerOn;

        /// <summary>
        /// show only data that contains the string smokeball
        /// </summary>
        private bool smokeBallOnly;

        /// <summary>
        ///     syncro context
        /// </summary>
        private readonly TaskScheduler synchronisationContext;

        /// <summary>
        /// constructor for MainWindowViewModel
        /// </summary>
        public MainWindowViewModel(ILogger _logger, IGoogleDataService _googleDataService, bool unitest = false)
        {
            logger = _logger;
            logger.Information("MainWindowViewModel open");

            googleDataService = _googleDataService;

            this.SearchSpinnerOn = false;
            this.SmokeBallOnly = false;

            // clear all collections
            this.SourceData = new List<IGoogleSearchData>();
            this.ListData = new List<IGoogleSearchData>();
            this.RankList = new List<int>();

            // listen to UI changes in bound properties
            SetupReactiveUIObservers();

            // ony initialise the query string if we are not running in a unit test
            if (unitest)
            {
                this.synchronisationContext = TaskScheduler.Current;
            }
            else
            {
                // If there is no SyncContext for this thread (e.g. we are in a unit test
                // or console scenario instead of running in an app), then just use the
                // default scheduler because there is no UI thread to sync with.

                this.synchronisationContext = TaskScheduler.FromCurrentSynchronizationContext();

                // initialise the query string to the requirement to search for "conveyancing software"
                this.QuerySearchString = "conveyancing software";
            }
        }

        /// <summary>
        /// setup reacting to UI changes
        /// appropriately
        /// </summary>
        private void SetupReactiveUIObservers()
        {
            var scheduler = RxApp.MainThreadScheduler;

            // subscribe to 
            // building original data set
            // when the query string is changed after
            // waiting 350 milliseconds for the user to stop typing
            this.WhenAnyValue(vm => vm.QuerySearchString)
            .Where(x => x != null)
            .Throttle(TimeSpan.FromMilliseconds(350), scheduler)
            .ObserveOn(scheduler)
            .Subscribe(this.GetQueryResultData);

            // subscribe to 
            // creating the data that is bound to the ui view
            // when the user changes the value bound to the check box
            // refresh the data that is in the datagrid accordingly
            this.WhenAnyValue(vm => vm.SmokeBallOnly)
                 .ObserveOn(scheduler)
                 .Subscribe(CreateFilteredListData);
        }

        /// <summary>
        ///     Gets the RAW source that was returned from the google query
        /// </summary>
        public List<IGoogleSearchData> SourceData
        {
            get;
        }

        /// <summary>
        ///     Gets the listdata that is the filtered RAW source
        ///     that is displayed in the UI.
        /// </summary>
        public List<IGoogleSearchData> ListData
        {
            get => listData;
            set
            {
                listData = value;
                this.OnPropertyChanged(nameof(this.ListData));
            }
        }

        /// <summary>
        /// contains the query string UI entered for the google search 
        /// </summary>
        public string QuerySearchString
        {
            get
            {
                return this.querySearchString;
            }

            set
            {
                this.querySearchString = value;
                this.OnPropertyChanged(nameof(this.QuerySearchString));
            }
        }

        /// <summary>
        /// property that toggles on and off the visibility of 
        /// the spinner control 
        /// that indicates that processing activity is going on
        /// in retrieving data from google 
        /// </summary>
        public bool SearchSpinnerOn
        {
            get
            {
                return searchSpinnerOn;
            }

            set
            {
                this.searchSpinnerOn = value;
                this.OnPropertyChanged(nameof(this.SearchSpinnerOn));
            }
        }

        /// <summary>
        /// filter to show only smokeball data
        /// </summary>
        public bool SmokeBallOnly
        {
            get => smokeBallOnly;

            set
            {
                this.smokeBallOnly = value;
                this.OnPropertyChanged(nameof(this.SmokeBallOnly));
            }
        }

        /// <summary>
        /// a list showing the rank positions in seo order that the
        /// query returned data results that containing the string smokeball
        /// </summary>
        public List<int> RankList
        {
            get => rankList;
            set
            {
                rankList = value;
                this.OnPropertyChanged(nameof(this.RankList));
            }
        }

        /// <summary>
        /// minimum implementation of IDisposable
        /// </summary>
        public void Dispose()
        {
            // cleanUp.Dispose();
        }

        /// <summary>
        /// create the data list to show
        /// which is data from SourceData property
        /// filtered or not as required
        /// </summary>
        /// <param name="flterOrNotFilter"></param>
        private void CreateFilteredListData(bool flterOrNotFilter)
        {
            if (flterOrNotFilter)
            {
                // only filter when required
                // showing smokeball data only
                this.ListData =
                        this.SourceData
                        .Where(x => x.IsSmokeBall == flterOrNotFilter)
                        .ToList();
            }
            else
            {
                // show all data unfiltered
                this.ListData =
                       this.SourceData
                       .ToList();
            }
        }

        /// <summary>
        ///  get the raw data from the google query
        /// </summary>
        /// <param name="query"></param>
        public void GetQueryResultData(string query)
        {
            // turn on the spinner wait control showing that query is being resolved
            this.SearchSpinnerOn = true;

            try
            {
                // run the search in a task so that we can set the ui 
                var CancelTask = Task.Run(
                        () =>
                        {
                            return GeneralDataBroker.GetSearchResponse(query);
                        })
                    .ContinueWith(
                        x =>
                        {
                            if (x.Result != null)
                            {
                                this.SourceData.Clear();

                                CreateSourceData(x);

                                CreateFilteredListData(this.SmokeBallOnly);

                                // turn off the spinner wait control showing that query has been being resolved
                                this.SearchSpinnerOn = false;

                                // tell the that DataDatagrid it is time to set  focus to the first row
                                // now that the data from the query has been loaded
                                Messenger.SendMessageSingleton("SetFocus", "RefreshDataDatagrid", "MainWindow");
                            }
                        },
                        this.synchronisationContext)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logger.Write(LogEventLevel.Error, ex, "Reading google data");
            }
        }

        /// <summary>
        /// create the source data from the google query returned results
        /// </summary>
        /// <param name="x"></param>
        public void CreateSourceData(Task<HtmlNodeCollection> x)
        {
            var rawNodes = x.Result;

            ProcessRawNodes(rawNodes);
        }

        public void ProcessRawNodes(HtmlNodeCollection rawNodes)
        {
            var returnNodesData = new List<IGoogleSearchData>();

            var positionInList = 0;

            foreach (HtmlNode oneNode in rawNodes)
            {
                var nodeContent = oneNode.InnerText;
                var newLine = GetGoogleDataInstance();
                newLine.FoundData = nodeContent;
                newLine.IsSmokeBall = nodeContent.Contains("smokeball", StringComparison.OrdinalIgnoreCase);
                newLine.QueryPosition = ++positionInList;

                returnNodesData.Add(newLine);
            }
            // turn off the spinner wait control showing that search action is completed
            this.SourceData.AddRange(returnNodesData);

            // build up the rank list for smokeball appearances
            MakeRankList();
        }

        /// <summary>
        ///  build up the rank list for smokeball appearances
        /// </summary>
        private void MakeRankList()
        {
            this.RankList.Clear();
            this.RankList =
                this.SourceData
                    .Where(x => x.IsSmokeBall)
                    .Select(x => x.QueryPosition)
                    .ToList<int>();
        }

        /// <summary>
        /// retrieve the results from the google query
        /// </summary>
        /// <returns></returns>
        private IGoogleSearchData GetGoogleDataInstance()
        {
            var instance = googleDataService.GetGoogleSearchData();
            return instance;
        }
    }
}
