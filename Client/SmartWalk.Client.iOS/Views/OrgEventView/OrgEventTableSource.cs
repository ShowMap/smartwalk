using System;
using System.Collections;
using System.Linq;
using Cirrious.MvvmCross.Binding.Touch.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Client.Core.Model;
using SmartWalk.Client.Core.Model.DataContracts;
using SmartWalk.Client.Core.ViewModels;
using SmartWalk.Client.iOS.Controls;
using SmartWalk.Client.iOS.Utils;

namespace SmartWalk.Client.iOS.Views.OrgEventView
{
    public class OrgEventTableSource : HiddenHeaderTableSource
    {
        private static readonly NSString EmptyCellKey = new NSString("empty");

        private readonly OrgEventViewModel _viewModel;

        private Venue[] _flattenItemsSource;

        public OrgEventTableSource(UITableView tableView, OrgEventViewModel viewModel)
            : base(tableView)
        {
            _viewModel = viewModel;

            UseAnimations = true;

            tableView.RegisterClassForHeaderFooterViewReuse(typeof(VenueHeaderView), VenueHeaderView.Key);
            tableView.RegisterClassForCellReuse(typeof(UITableViewCell), EmptyCellKey);
            tableView.RegisterNibForCellReuse(VenueShowCell.Nib, VenueShowCell.Key);
        }

        public bool IsSearchSource { get; set; }

        public override IEnumerable ItemsSource
        {
            set
            {
                _flattenItemsSource = null;
                base.ItemsSource = value;
            }
        }

        private Venue[] VenueItemsSource
        {
            get { return (Venue[])ItemsSource; }
        }

        private Venue[] FlattenItemsSource
        {
            get
            {
                if (_flattenItemsSource == null &&
                    VenueItemsSource != null)
                {
                    _flattenItemsSource = 
                        VenueItemsSource
                            .SelectMany(
                                v => 
                                    v.Shows != null
                                        ? v.Shows.Select(
                                            s =>
                                            { 
                                                var venue = new Venue(v.Info) 
                                                    { 
                                                        Shows = new [] { s } 
                                                    }; 
                                                return venue;
                                            })
                                        : Enumerable.Empty<Venue>())
                            .ToArray();
                }

                return _flattenItemsSource;
            }
        }

        private Venue[] CurrentItemsSource
        {
            get 
            { 
                return _viewModel.IsGroupedByLocation 
                    ? VenueItemsSource 
                    : FlattenItemsSource
                        .OrderBy(v => v.Shows[0], new ShowComparer(_viewModel.SortBy))
                        .ToArray(); 
            }
        }

        public NSIndexPath GetItemIndex(Show show)
        {
            for (var i = 0; i < CurrentItemsSource.Length; i++)
            {
                if (CurrentItemsSource[i].Shows.Contains(show))
                {
                    return NSIndexPath.FromItemSection(
                        Array.IndexOf(CurrentItemsSource[i].Shows, show), 
                        i);
                }
            }

            return NSIndexPath.FromItemSection(0, 0);
        }

        public NSIndexPath GetItemIndex(Venue venue)
        {
            var venueNumber = Array.IndexOf(CurrentItemsSource, venue);
            return venueNumber >= 0 
                ? NSIndexPath.FromRowSection(int.MaxValue, venueNumber)
                : null;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            TableView.DeselectRow(indexPath, false);
        }

        public override float GetHeightForHeader(UITableView tableView, int section)
        {
            return _viewModel.IsGroupedByLocation || 
                (_viewModel.ExpandedShow != null && 
                CurrentItemsSource[section].Shows.Contains(_viewModel.ExpandedShow)) 
                    ? VenueHeaderView.DefaultHeight : 0;
        }

        public override float GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            var item = GetItemAt(indexPath);
            var venueShow = item as Show;
            if (venueShow != null)
            {
                var height = VenueShowCell.CalculateCellHeight(
                    tableView.Frame.Width,
                    Equals(_viewModel.ExpandedShow, venueShow),
                    venueShow);
                return height;
            }

            return VenueShowCell.DefaultHeight;
        }

        public override int NumberOfSections(UITableView tableView)
        {
            return CurrentItemsSource != null ? CurrentItemsSource.Length : 0;
        }

        public override int RowsInSection(UITableView tableview, int section)
        {
            var emptyRow = IsSearchSource &&
                section == NumberOfSections(tableview) - 1 ? 1 : 0; // empty row for search

            return (CurrentItemsSource != null &&
            CurrentItemsSource[section].Shows != null 
                    ? CurrentItemsSource[section].Shows.Length 
                    : 0) + emptyRow;
        }

        public override string TitleForHeader(UITableView tableView, int section)
        {
            return null;
        }

        public override UIView GetViewForHeader(UITableView tableView, int section)
        {
            if (_viewModel.IsGroupedByLocation || 
                (_viewModel.ExpandedShow != null && 
                    CurrentItemsSource[section].Shows.Contains(_viewModel.ExpandedShow)))
            {
                var headerView = (VenueHeaderView)tableView.DequeueReusableHeaderFooterView(VenueHeaderView.Key);

                headerView.DataContext = CurrentItemsSource[section];
                headerView.NavigateVenueCommand = _viewModel.NavigateVenueCommand;
                headerView.NavigateVenueOnMapCommand = _viewModel.NavigateVenueOnMapCommand;

                return headerView;
            }

            return null;
        }

        protected override UITableViewCell GetOrCreateCellFor(
            UITableView tableView,
            NSIndexPath indexPath,
            object item)
        {
            if (IsSearchSource && item == null)
            {
                var emptyCell = tableView.DequeueReusableCell(EmptyCellKey, indexPath);
                emptyCell.SelectionStyle = UITableViewCellSelectionStyle.None;
                return emptyCell;
            }

            var cell = default(UITableViewCell);

            var show = item as Show;
            if (show != null)
            {
                cell = tableView.DequeueReusableCell(VenueShowCell.Key, indexPath);
                ((VenueShowCell)cell).ShowImageFullscreenCommand = _viewModel.ShowHideFullscreenImageCommand;
                ((VenueShowCell)cell).ExpandCollapseShowCommand = _viewModel.ExpandCollapseShowCommand;
                ((VenueShowCell)cell).NavigateDetailsLinkCommand = _viewModel.NavigateWebLinkCommand;
                ((VenueShowCell)cell).DataContext = show;
                ((VenueShowCell)cell).IsExpanded = Equals(_viewModel.ExpandedShow, item);
                ((VenueShowCell)cell).IsHighlighted = ((VenueShowCell)cell).IsExpanded && 
                    !_viewModel.IsGroupedByLocation;
                ((VenueShowCell)cell).IsSeparatorVisible = 
                    !_viewModel.IsGroupedByLocation ||
                    indexPath.Row < CurrentItemsSource[indexPath.Section].Shows.Length - 1 ||
                    indexPath.Section == CurrentItemsSource.Length - 1;
            }

            return cell;
        }

        protected override object GetItemAt(NSIndexPath indexPath)
        {
            if (CurrentItemsSource != null &&
                CurrentItemsSource[indexPath.Section].Shows != null)
            {
                // Asumming that there may be an empty row for search
                return indexPath.Row < CurrentItemsSource[indexPath.Section].Shows.Length
                    ? CurrentItemsSource[indexPath.Section].Shows[indexPath.Row] 
                    : null;
            }
           
            return null;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            ConsoleUtil.LogDisposed(this);
        }
    }

    /// <summary>
    /// This is a helper base class that incapsulates the HACK for initial hiding of table's header view.
    /// </summary>
    public class HiddenHeaderTableSource : MvxTableViewSource, IListViewSource
    {
        private bool _isTouched;

        protected HiddenHeaderTableSource(UITableView tableView) : base(tableView)
        {
        }

        public bool IsHeaderViewHidden
        {
            get
            {
                return TableView.TableHeaderView == null ||
                    TableView.ContentOffset.Y >= HeaderHeight;
            }
        }

        protected virtual float HeaderHeight
        {
            get
            {
                return TableView != null && 
                        TableView.TableHeaderView != null
                    ? TableView.TableHeaderView.Frame.Height 
                    : 0; 
            }
        }

        public override void ReloadTableData()
        {
            base.ReloadTableData();

            if (!IsHeaderViewHidden)
            {
                ScrollUtil.ScrollOutHeaderAfterReload(
                    TableView, 
                    HeaderHeight, 
                    this, 
                    _isTouched);
            }
        }

        public void ScrollOutHeader()
        {
            ScrollUtil.ScrollOutHeader(TableView, HeaderHeight, _isTouched);
        }

        public override void DraggingStarted(UIScrollView scrollView)
        {
            _isTouched = true;
        }

        public override void DraggingEnded(UIScrollView scrollView, bool willDecelerate)
        {
            if (TableView.TableHeaderView == null) return;

            ScrollUtil.AdjustHeaderPosition(scrollView, HeaderHeight);
        }

        protected override UITableViewCell GetOrCreateCellFor(
            UITableView tableView,
            NSIndexPath indexPath,
            object item)
        {
            return null;
        }
    }
}