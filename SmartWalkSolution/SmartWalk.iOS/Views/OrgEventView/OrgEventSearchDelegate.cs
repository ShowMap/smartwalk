using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.UIKit;
using SmartWalk.Core.Model;
using SmartWalk.Core.ViewModels;

namespace SmartWalk.iOS.Views.OrgEventView
{
    public class OrgEventSearchDelegate : UISearchDisplayDelegate
    {
        private readonly OrgEventViewModel _viewModel;

        private Dictionary<SearchKey, string> _searchableTexts;
        private Venue[] _itemsSource;
        private Venue[] _searchResults;

        public OrgEventSearchDelegate(OrgEventViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public Venue[] ItemsSource
        {
            get
            {
                return _itemsSource;
            }
            set
            {
                _itemsSource = value;
                _searchableTexts = null;
            }
        }

        private Dictionary<SearchKey, string> SearchableTexts
        {
            get
            {
                if (_searchableTexts == null && ItemsSource != null)
                {
                    _searchableTexts = new Dictionary<SearchKey, string>();

                    foreach (var venue in ItemsSource)
                    {
                        var text = venue.SearchableText;
                        _searchableTexts[new SearchKey(venue)] = 
                            text != null ? text.ToLower() : null;

                        if (venue.Shows != null)
                        {
                            foreach (var show in venue.Shows)
                            {
                                text = show.SearchableText;
                                _searchableTexts[new SearchKey(venue, show)] = 
                                    text != null ? text.ToLower() : null;
                            }
                        }
                    }
                }

                return _searchableTexts;
            }
        }

        public override bool ShouldReloadForSearchString(
            UISearchDisplayController controller,
            string forSearchString)
        {
            if (SearchableTexts == null) return false;

            var matches = SearchableTexts
                .Where(kvp => kvp.Value != null && 
                       kvp.Value.Contains(forSearchString.ToLower()))
                    .ToArray();

            var searchResults = new List<Venue>();
            foreach (var match in matches)
            {
                var venue = searchResults.FirstOrDefault(v => Equals(v.Info, match.Key.Item1.Info));
                if (venue == null)
                {
                    venue = match.Key.Item1.Clone();
                    venue.Shows = match.Key.Item2 != null 
                        ? new [] { match.Key.Item2 } 
                        : new VenueShow[0];
                    searchResults.Add(venue);
                }
                else
                {
                    venue.Shows = venue.Shows.Union(new [] {match.Key.Item2}).ToArray();
                }
            }

            if (searchResults.Count > 0)
            {
                _searchResults = searchResults.ToArray();
            }
            else
            {
                _searchResults = null;
            }

            if (controller.SearchResultsTableView.Source != null)
            {
                var tableSource = (OrgEventTableSource)controller.SearchResultsTableView.Source;

                tableSource.IsSearchSource = _searchResults != null;
                tableSource.ItemsSource = _searchResults;
            }

            return false;
        }

        public override void WillShowSearchResults(UISearchDisplayController controller, UITableView tableView)
        {
            if (tableView.Source == null)
            {
                var tableSource = new OrgEventTableSource(tableView, _viewModel);
                tableView.Source = tableSource;
                tableSource.IsSearchSource = _searchResults != null;
                tableSource.ItemsSource = _searchResults;
            }
        }

        private class SearchKey : Tuple<Venue, VenueShow>
        {
            public SearchKey(Venue venue) : base(venue, null) {}
            public SearchKey(Venue venue, VenueShow show) : base(venue, show) {}
        }
    }
}
