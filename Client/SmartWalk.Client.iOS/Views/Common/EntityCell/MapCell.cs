using System;
using System.Linq;
using System.Windows.Input;
using MonoTouch.CoreLocation;
using MonoTouch.Foundation;
using MonoTouch.MapKit;
using MonoTouch.UIKit;
using SmartWalk.Client.Core.Model.DataContracts;
using SmartWalk.Client.Core.Utils;
using SmartWalk.Client.iOS.Resources;
using SmartWalk.Client.iOS.Utils;
using SmartWalk.Client.iOS.Utils.Map;
using SmartWalk.Client.iOS.Views.Common.Base;

namespace SmartWalk.Client.iOS.Views.Common.EntityCell
{
    public partial class MapCell : CollectionCellBase
    {
        public static readonly UINib Nib = UINib.FromName("MapCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("MapCell");

        private UITapGestureRecognizer _mapTapGesture;

        public MapCell(IntPtr handle) : base (handle)
        {
        }

        public new Entity DataContext
        {
            get { return (Entity)base.DataContext; }
            set { base.DataContext = value; }
        }

        public ICommand NavigateAddressesCommand { get; set; }

        public static MapCell Create()
        {
            return (MapCell)Nib.Instantiate(null, null)[0];
        }

        public override void WillMoveToSuperview(UIView newsuper)
        {
            base.WillMoveToSuperview(newsuper);

            if (newsuper == null)
            {
                NavigateAddressesCommand = null;

                DisposeGestures();
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            ConsoleUtil.LogDisposed(this);
        }

        protected override void OnInitialize()
        {
            InitializeGestures();

            if (UIDevice.CurrentDevice.CheckSystemVersion(7, 0))
            {
                MapView.TintColor = Theme.MapTint;
            }

            MapView.Delegate = new MapDelegate { CanShowDetails = false };
        }

        protected override void OnDataContextChanged()
        {
            var previousAnnotations = MapView.Annotations.Where(a => !(a is MKUserLocation)).ToArray();
            var isFirstLoading = previousAnnotations.Length == 0;
            MapView.RemoveAnnotations(previousAnnotations);

            if (DataContext != null && 
                DataContext.HasAddresses())
            {
                var annotations = DataContext.Addresses
                    .Select( a => new MapViewAnnotation(DataContext.Name, a))
                    .ToArray();
                var coordinates = MapUtil.GetAnnotationsCoordinates(annotations);

                MapView.SetRegion(
                    MapUtil.CoordinateRegionForCoordinates(
                        coordinates, 
                        new MKMapSize(2000, 2000)),
                    !isFirstLoading);

                MapView.SetCenterCoordinate(
                    new CLLocationCoordinate2D(
                        annotations[0].Coordinate.Latitude + 0.00065, // moving a bit down to fit callout
                        annotations[0].Coordinate.Longitude),
                    !isFirstLoading);

                MapView.AddAnnotations(annotations);
                MapView.SelectAnnotation(annotations[0], !isFirstLoading);
            }
        }

        private void InitializeGestures()
        {
            _mapTapGesture = new UITapGestureRecognizer(() => {
                if (NavigateAddressesCommand != null &&
                    NavigateAddressesCommand.CanExecute(DataContext))
                {
                    NavigateAddressesCommand.Execute(DataContext);
                }
            }) {
                NumberOfTouchesRequired = (uint)1,
                NumberOfTapsRequired = (uint)1
            };

            CoverView.AddGestureRecognizer(_mapTapGesture);
        }

        private void DisposeGestures()
        {
            if (_mapTapGesture != null)
            {
                CoverView.RemoveGestureRecognizer(_mapTapGesture);
                _mapTapGesture.Dispose();
                _mapTapGesture = null;
            }
        }
    }
}