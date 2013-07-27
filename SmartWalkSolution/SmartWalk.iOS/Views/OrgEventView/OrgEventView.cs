using System.ComponentModel;
using System.Linq;
using Cirrious.MvvmCross.Binding.BindingContext;
using MonoTouch.CoreLocation;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Core.Model;
using SmartWalk.Core.Utils;
using SmartWalk.Core.ViewModels;
using SmartWalk.iOS.Utils;
using SmartWalk.iOS.Views.Common;
using SmartWalk.iOS.Controls;
using MonoTouch.CoreAnimation;

namespace SmartWalk.iOS.Views.OrgEventView
{
    public partial class OrgEventView : ListViewBase
    {
        private UIBarButtonItem _modeButton;
        private OrgEvent _currentMapViewOrgEvent;

        public new OrgEventViewModel ViewModel
        {
            get { return (OrgEventViewModel)base.ViewModel; }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            if (ViewModel.SwitchModeCommand.CanExecute(OrgEventViewMode.List))
            {
                ViewModel.SwitchModeCommand.Execute(OrgEventViewMode.List);
            }

            VenuesMapView.Delegate = new OrgEventMapDelegate(ViewModel);

            InitializeToolBar();
            InitializeGestures();

            UpdateViewState(false);
        }

        public override void DidRotate(UIInterfaceOrientation fromInterfaceOrientation)
        {
            base.DidRotate(fromInterfaceOrientation);

            // to fix the bug: http://stackoverflow.com/questions/14307037/bug-in-uitableview-layout-after-orientation-change
            VenuesAndShowsTableView.BeginUpdates();
            VenuesAndShowsTableView.EndUpdates();
        }

        protected override ListViewDecorator GetListView()
        { 
            return new ListViewDecorator(VenuesAndShowsTableView);  
        }

        protected override void UpdateViewTitle()
        {
            if (ViewModel.OrgEvent != null && ViewModel.OrgEvent.Info != null)
            {
                NavigationItem.Title = ViewModel.OrgEvent.Info.Date.ToShortDateString();
            }
        }

        protected override object CreateListViewSource()
        {
            var tableSource = new OrgEventTableSource(VenuesAndShowsTableView, ViewModel);

            this.CreateBinding(tableSource).To((OrgEventViewModel vm) => vm.OrgEvent.Venues).Apply();

            return tableSource;
        }

        protected override void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == ViewModel.GetPropertyName(vm => vm.OrgEvent))
            {
                if (ViewModel.Mode == OrgEventViewMode.Map)
                {
                    InitializeMapView();
                }
            }
            else if (e.PropertyName == ViewModel.GetPropertyName(vm => vm.Mode))
            {
                UpdateViewState();
            }
            else if (e.PropertyName == ViewModel.GetPropertyName(vm => vm.SelectedVenueOnMap))
            {
                SelectVenueMapAnnotation(ViewModel.SelectedVenueOnMap);
            }
            else if (e.PropertyName == ViewModel.GetPropertyName(vm => vm.ExpandedShow))
            {
                VenueShowCell.SetVenueCellsTableIsResizing(VenuesAndShowsTableView, true);
                VenueShowCell.CollapseVenueShowCell(VenuesAndShowsTableView);

                CATransaction.Begin();
                CATransaction.CompletionBlock = new NSAction(
                    () => VenueShowCell.SetVenueCellsTableIsResizing(VenuesAndShowsTableView, false));

                VenuesAndShowsTableView.BeginUpdates();
                VenuesAndShowsTableView.EndUpdates();

                CATransaction.Commit();

                VenueShowCell.ExpandVenueShowCell(VenuesAndShowsTableView, ViewModel.ExpandedShow);
            }
        }

        private void InitializeToolBar()
        {
            _modeButton = new UIBarButtonItem();

            _modeButton.Clicked += (sender, e) => 
                {
                    if (ViewModel.SwitchModeCommand.CanExecute(null))
                    {
                        ViewModel.SwitchModeCommand.Execute(null);
                    }
                };

            NavigationItem.SetRightBarButtonItem(_modeButton, true);
        }

        private void InitializeGestures()
        {
            var swipeLeft = new UISwipeGestureRecognizer(rec => 
                {
                    if (ViewModel.SwitchModeCommand.CanExecute(OrgEventViewMode.Map))
                    {
                        ViewModel.SwitchModeCommand.Execute(OrgEventViewMode.Map);
                    }
                });

            swipeLeft.Direction = UISwipeGestureRecognizerDirection.Left;
            TablePanel.AddGestureRecognizer(swipeLeft);
        }

        private void InitializeMapView()
        {
            if (ViewModel.OrgEvent != null &&
                ViewModel.OrgEvent.Venues != null)
            {
                if (_currentMapViewOrgEvent != ViewModel.OrgEvent)
                {
                    VenuesMapView.RemoveAnnotations(VenuesMapView.Annotations);

                    var annotations = ViewModel.OrgEvent.Venues
                        .SelectMany(v => v.Info.Addresses
                            .Select(a => new VenueAnnotation(v, a))).ToArray();
                    var coordinates = annotations
                        .Select(va => va.Coordinate)
                        .Where(c => c.Latitude != 0 && c.Longitude != 0).ToArray();

                    VenuesMapView.SetRegion(MapUtil.CoordinateRegionForCoordinates(coordinates), false);
                    VenuesMapView.AddAnnotations(annotations);

                    if (ViewModel.SelectedVenueOnMap != null)
                    {
                        SelectVenueMapAnnotation(ViewModel.SelectedVenueOnMap);
                    }

                    _currentMapViewOrgEvent = ViewModel.OrgEvent;
                }
            }
            else
            {
                VenuesMapView.RemoveAnnotations(VenuesMapView.Annotations);
                _currentMapViewOrgEvent = null;
            }
        }

        private void SelectVenueMapAnnotation(Venue venue)
        {
            if (venue != null)
            {
                var annotation = VenuesMapView.Annotations
                .OfType<VenueAnnotation>()
                    .FirstOrDefault(an => an.Venue == venue);

                if (annotation != null)
                {
                    VenuesMapView.SelectAnnotation(annotation, true);
                }
            }
            else if (VenuesMapView.SelectedAnnotations.Any())
            {
                foreach (var annotation in VenuesMapView.SelectedAnnotations)
                {
                    VenuesMapView.DeselectAnnotation(annotation, false);
                }
            }
        }
       
        private void UpdateViewState(bool isAnimated = true)
        {
            if (ViewModel.Mode == OrgEventViewMode.Map)
            {
                _modeButton.Title = "List";

                var completeHandler = new NSAction(() => 
                    {
                        TablePanel.Hidden = true;
                        MapPanel.Hidden = false;

                        InitializeMapView();
                    });

                if (isAnimated)
                {
                    UIView.Transition(
                        TablePanel, 
                        MapPanel, 
                        0.8, 
                        UIViewAnimationOptions.TransitionFlipFromRight | 
                        UIViewAnimationOptions.ShowHideTransitionViews, 
                        completeHandler);
                }
                else
                {
                    completeHandler();
                }
            }
            else
            {
                _modeButton.Title = "Map";

                var completeHandler = new NSAction(() => 
                    {
                        TablePanel.Hidden = false;
                        MapPanel.Hidden = true;
                    });

                if (isAnimated)
                {
                    UIView.Transition(
                        MapPanel, 
                        TablePanel, 
                        0.8,
                        UIViewAnimationOptions.TransitionFlipFromLeft | 
                        UIViewAnimationOptions.ShowHideTransitionViews,
                        completeHandler);
                }
                else
                {
                    completeHandler();
                }
            }
        }
    }
}