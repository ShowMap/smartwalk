using System;
using Cirrious.MvvmCross.Binding.Touch.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Client.Core.Model;
using SmartWalk.Client.Core.ViewModels;
using SmartWalk.Client.iOS.Controls;
using SmartWalk.Client.iOS.Utils;
using SmartWalk.Client.iOS.Views.Common.EntityCell;
using SmartWalk.Client.iOS.Views.Common.GroupHeader;

namespace SmartWalk.Client.iOS.Views.OrgView
{
    public class OrgTableSource : MvxTableViewSource, IListViewSource
    {
        private readonly OrgViewModel _viewModel;

        public OrgTableSource(UITableView tableView, OrgViewModel viewModel)
            : base(tableView)
        {
            _viewModel = viewModel;

            tableView.RegisterClassForHeaderFooterViewReuse(typeof(GroupHeaderView), GroupHeaderView.Key);
            tableView.RegisterNibForCellReuse(EntityCell.Nib, EntityCell.Key);
            tableView.RegisterNibForCellReuse(OrgEventCell.Nib, OrgEventCell.Key);
        }

        public GroupContainer[] GroupItemsSource
        {
            get { return (GroupContainer[])ItemsSource; }
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            var eventInfo = GetItemAt(indexPath) as OrgEvent;

            if (eventInfo != null &&
                _viewModel.NavigateOrgEventViewCommand.CanExecute(eventInfo))
            {
                _viewModel.NavigateOrgEventViewCommand.Execute(eventInfo);
            }

            TableView.DeselectRow(indexPath, false);
        }

        public override float GetHeightForHeader(UITableView tableView, int section)
        {
            return 
                GroupItemsSource != null &&
                    GroupItemsSource[section].Key != null 
                        ? GroupHeaderView.DefaultHeight 
                        : 0f;
        }

        public override float GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            var item = GetItemAt(indexPath);

            var entityCellContext = item as IEntityCellContext;
            if (entityCellContext != null)
            {
                var height = EntityCell.CalculateCellHeight(
                    tableView.Frame.Width,
                    entityCellContext.IsDescriptionExpanded,
                    entityCellContext.Entity);

                return (float)Math.Ceiling(height);
            }

            if (item is OrgEvent)
            {
                return OrgEventCell.DefaultHeight;
            }

            throw new Exception("There is an unsupported type in the list.");
        }

        public override int NumberOfSections(UITableView tableView)
        {
            return GroupItemsSource != null ? GroupItemsSource.Length : 0;
        }

        public override int RowsInSection(UITableView tableview, int section)
        {
            return GroupItemsSource != null ? GroupItemsSource[section].Count : 0;
        }

        public override string TitleForHeader(UITableView tableView, int section)
        {
            return null;
        }

        public override UIView GetViewForHeader(UITableView tableView, int section)
        {
            var dataContext = GroupItemsSource != null ? GroupItemsSource[section].Key : null;
            if (dataContext != null)
            {
                var headerView = (GroupHeaderView)tableView
                    .DequeueReusableHeaderFooterView(GroupHeaderView.Key);

                headerView.DataContext = dataContext;

                return headerView;
            }

            return null;
        }

        protected override UITableViewCell GetOrCreateCellFor(
            UITableView tableView, 
            NSIndexPath indexPath, 
            object item)
        {
            var cell = default(UITableViewCell);

            var entityCellContext = item as IEntityCellContext;
            if (entityCellContext != null)
            {
                cell = tableView.DequeueReusableCell(EntityCell.Key, indexPath);
                ((EntityCell)cell).ExpandCollapseCommand = _viewModel.ExpandCollapseCommand;
                ((EntityCell)cell).ShowImageFullscreenCommand = _viewModel.ShowHideFullscreenImageCommand;
                ((EntityCell)cell).NavigateWebSiteCommand = _viewModel.NavigateWebLinkCommand;
                ((EntityCell)cell).NavigateAddressesCommand = _viewModel.NavigateAddressesCommand;
                ((EntityCell)cell).DataContext = entityCellContext;
            }

            var orgEvent = item as OrgEvent;
            if (orgEvent != null)
            {
                cell = tableView.DequeueReusableCell(OrgEventCell.Key, indexPath);
                ((OrgEventCell)cell).DataContext = orgEvent;
                ((OrgEventCell)cell).IsSeparatorVisible = 
                    indexPath.Row < GroupItemsSource[indexPath.Section].Count - 1 ||
                    indexPath.Section == GroupItemsSource.Length - 1;
            }

            return cell;
        }

        protected override object GetItemAt(NSIndexPath indexPath)
        {
            return GroupItemsSource[indexPath.Section][indexPath.Row];
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            ConsoleUtil.LogDisposed(this);
        }
    }
}