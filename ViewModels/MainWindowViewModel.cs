// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindowViewModel.cs" company="MarkJC">
//   Author Mark Carew
//   Date Sunday 24-01-2020
// </copyright>
// <summary>
// View Model for the Ceo Seo UI 
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using CeoSeoCommon;
using ReactiveUI;
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace CeoSeoViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        /// <summary>
        /// private backing field 
        /// for public Property QuerySearchString 
        /// </summary>
        private string querySearchString;

        /// <summary>
        ///     sync context tied to mainline
        /// </summary>
        private readonly TaskScheduler synchronisationContext;

        /// <summary>
        /// constructor for MainWindowViewModel
        /// </summary>
        public MainWindowViewModel()
        {
            this.synchronisationContext = TaskScheduler.FromCurrentSynchronizationContext();

            this.QuerySearchString = "conveyancing software";

            var scheduler = RxApp.MainThreadScheduler;
            this.SearchSpinnerOn = false;
            this.WhenAnyValue(vm => vm.QuerySearchString)
                .Where(x => x != null)
                .Throttle(TimeSpan.FromMilliseconds(350), scheduler)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(this.DoGoogleSearch);

            this.WhenAnyValue(vm => vm.Html)
               .Where(x => x != null)
               .Throttle(TimeSpan.FromMilliseconds(350), scheduler)
               .ObserveOn(RxApp.MainThreadScheduler)
               .Subscribe(this.BuildUriHtml);

        }

        private void BuildUriHtml(string obj)
        {
            // UriHtml = Path.Combine(Environment.CurrentDirectory, /)
            UriHtml = new Uri("https://youtube.com");
        }

        private string html;

        public string Html
        {
            get { return html; }
            set
            { 
                html = value;
                this.OnPropertyChanged(nameof(Html));
            }
        }

        private Uri uriHtml;

        public Uri UriHtml
        {
            get 
            {
                return uriHtml;
            }

            set 
            {
                uriHtml = value;
                this.OnPropertyChanged(nameof(UriHtml));
            }
        }



        private void DoGoogleSearch(string obj)
        {
            // set searching spinner on
            // and do tas.run search
            this.SearchSpinnerOn = false;

            Task.Run(
                    () =>
                    {
                        var result = GoogleDataBroker.GetSearchResponse(this.QuerySearchString);
                        return result;
                    })
                .ContinueWith(
                    x =>
                    {
                        this.SearchSpinnerOn = true;
                        this.Html = x.Result;
                    },
                    this.synchronisationContext)
                .ConfigureAwait(false);
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
                this.OnPropertyChanged(nameof(QuerySearchString));
            }
        }

        private bool searchSpinnerOn;

        public bool SearchSpinnerOn
        {
            get 
            {
                return searchSpinnerOn; 
            }

            set
            {
                searchSpinnerOn = value;
                this.OnPropertyChanged(nameof(SearchSpinnerOn));
            }
        }

    }
}
