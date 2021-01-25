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
using HTMLXaml;
using ReactiveUI;
using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Threading;

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

        private string html;

        private bool searchSpinnerOn;

        private string flowDocumentXamlString;

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
               .Subscribe(this.BuildXamlFlowDocmentFromHtml);
        }

        private void BuildXamlFlowDocmentFromHtml(string rawHtml)
        {
            FlowDocumentXamlString = HtmlToXamlConverter.ConvertHtmlToXaml(rawHtml, true);
        }
        public string Html
        {
            get { return html; }
            set
            {
                html = value;
                this.OnPropertyChanged(nameof(Html));
            }
        }

        public string FlowDocumentXamlString
        {
            get
            {
                return this.flowDocumentXamlString;
            }

            set
            {
                this.flowDocumentXamlString = value;
                this.OnPropertyChanged("FlowDocumentXamlString");
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
