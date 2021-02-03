// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindowViewModel.cs" company="MarkJC">
//   Author Mark Carew
//   Date Sunday 24-01-2020
// </copyright>
// <summary>
// View Model for the Ceo Seo UI MainWindow.xaml
// version 3.0 attempt to remove the use of .Result from all task code
// 03 Feb 2021
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
        /// show only data that contains the string smokeball
        /// </summary>
        private bool smokeBallOnly;

        /// <summary>
        ///     syncro context
        /// </summary>
        private readonly TaskScheduler synchronisationContext;

        /// <summary>
        /// private backing field for HideQueryEntryData
        /// </summary>
        private bool hideQueryEntryDataingData; 

        /// <summary>
        /// constructor for MainWindowViewModel
        /// </summary>
        public MainWindowViewModel(ILogger _logger, IGoogleDataService _googleDataService, bool unitest = false)
        {
            logger = _logger;
            logger.Information("MainWindowViewModel open");

            googleDataService = _googleDataService;

            // read only initialisation
            this.SourceData = new List<IGoogleSearchData>();

            InitialiseVariables();

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
                // kick off the search
                this.QuerySearchString = "conveyancing software";
            }
        }

        /// <summary>
        /// start off settings for all properties
        /// </summary>
        private void InitialiseVariables()
        {
            // initially 
            // we want to all data returned from the query
            this.SmokeBallOnly = false;

            // clear all collections
            this.ListData = new List<IGoogleSearchData>();
            this.RankList = new List<int>();
            this.HideQueryEntryData = false;
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
        ///     Gets the RAW source that was returned from the google query
        /// </summary>
        public List<IGoogleSearchData> SourceData
        {
            get;
        }

        /// <summary>
        /// this is set to true when data is being loaded via the call to google 
        /// </summary>
        public bool HideQueryEntryData
        {
            get => hideQueryEntryDataingData;

            set
            {
                hideQueryEntryDataingData = value;
                this.OnPropertyChanged(nameof(this.HideQueryEntryData));
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
        /// pull apart the HtmlNodeCollection to get the data lines returned
        /// </summary>
        /// <param name="rawNodes"></param>
        public async Task ProcessRawNodes(HtmlNodeCollection rawNodes)
        {
            this.SourceData.Clear();

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

            // build raw data range
            this.SourceData.AddRange(returnNodesData);

            // build up the rank list for smokeball appearances
            await MakeRankListAsync();
        }

        /// <summary>
        ///  get the raw data from the google query
        /// </summary>
        /// <param name="query"></param>
        private async Task GetQueryResultDataAsync(string query)
        {
            try
            {
                this.HideQueryEntryData = true;

                // turn on the spinner wait control showing that query is being resolved
                var uiFactory = new TaskFactory(this.synchronisationContext);
                var task = uiFactory.StartNew(() => Messenger.SendMessageSingleton("ShowSpinTheDotsInAdorner", null, "MainWindow"));

                // run the search in a task so that we can set the ui 
                await task.ContinueWith(_ =>
                {
                    // get the data from the google query
                    return GeneralDataBroker.GetSearchResponse(query);
                })
                    .ContinueWith(
                        async x =>
                        {
                            if (x != null)
                            {
                                // this.SourceData.Clear();

                                await CreateSourceDataAsync(x);

                                await CreateFilteredListDataAsync(this.SmokeBallOnly);

                                // turn off the spinner wait control showing that query has been being resolved
                                Messenger.SendMessageSingleton("CloseSpinTheDotsAdorner", "Queries", "MainWindow");

                                // tell the that DataDatagrid it is time to set  focus to the first row
                                // now that the data from the query has been loaded
                                Messenger.SendMessageSingleton("SetFocus", "RefreshDataDatagrid", "MainWindow");

                                this.HideQueryEntryData = false;
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
        private async Task CreateSourceDataAsync(Task<HtmlNodeCollection> x)
        {
            HtmlNodeCollection nodes = null;
            var awaitTask = Task.Run(async () =>
            {
                nodes = await x;
                await ProcessRawNodes(nodes);
            });

            await awaitTask;
        }

        /// <summary>
        /// create the data list to show
        /// which is data from SourceData property
        /// filtered or not as required
        /// </summary>
        /// <param name="filterOrNotFilter"></param>
        private async Task CreateFilteredListDataAsync(bool filterOrNotFilter)
        {
            List<IGoogleSearchData> theList = FilterTheData(filterOrNotFilter);

            // process the derived list into the list to display
            await CopyListDataAsync(theList);
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
            // waiting 500 milliseconds for the user to stop typing
            this.WhenAnyValue(vm => vm.QuerySearchString)
            .Where(x => x != null)
            .Throttle(TimeSpan.FromMilliseconds(500), scheduler)
            .ObserveOn(scheduler)
            .Subscribe(FetchGoogleDataAsyncActionBuild());

            // subscribe to 
            // creating the data that is bound to the ui view
            // when the user changes the value bound to the check box
            // refresh the data that is in the datagrid accordingly
            this.WhenAnyValue(vm => vm.SmokeBallOnly)
                    .ObserveOn(scheduler)
                    .Subscribe(FetchFilteredListDataActionBuild());
        }

        /// <summary>
        /// return thr data filtered or not
        /// </summary>
        /// <param name="filterOrNotFilter"></param>
        /// <returns></returns>
        private List<IGoogleSearchData> FilterTheData(bool filterOrNotFilter)
        {
            List<IGoogleSearchData> theList = new List<IGoogleSearchData>();
            if (filterOrNotFilter)
            {
                // only filter when required
                // showing smokeball data only
                theList =
                        this.SourceData
                        .Where(x => x.IsSmokeBall == filterOrNotFilter)
                        .ToList();
            }
            else
            {
                // show all data unfiltered
                theList =
                       this.SourceData
                       .ToList();
            }

            return theList;
        }

        /// <summary>
        /// pass the query string to the google query processor
        /// waiting for it to complete
        /// </summary>
        /// <returns></returns>
        private Action<string> FetchGoogleDataAsyncActionBuild()
        {
            return async (x) => await this.GetQueryResultDataAsync(x);
        }

        /// <summary>
        /// filter the data
        /// waiting for it to complete
        /// </summary>
        /// <returns></returns>
        private Action<bool> FetchFilteredListDataActionBuild()
        {
            return async (x) => await this.CreateFilteredListDataAsync(x);
        }

        /// <summary>
        ///  build up the rank list for smokeball appearances
        /// </summary>
        private Task MakeRankListAsync()
        {
            this.RankList.Clear();
            this.RankList =
                this.SourceData
                    .Where(x => x.IsSmokeBall)
                    .Select(x => x.QueryPosition)
                    .ToList<int>();

            return Task.CompletedTask;
        }

        /// <summary>
        /// build up the List Data for dispaly by the UI
        /// </summary>
        /// <param name="theList"></param>
        /// <returns></returns>
        private Task CopyListDataAsync(List<IGoogleSearchData> theList)
        {
            this.ListData = theList;
            return Task.CompletedTask;
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
