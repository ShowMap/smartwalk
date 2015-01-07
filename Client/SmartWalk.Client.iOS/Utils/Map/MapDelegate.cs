using System.Collections.Generic;
using System.Drawing;
using System.Windows.Input;
using MonoTouch.Foundation;
using MonoTouch.MapKit;
using MonoTouch.UIKit;
using SmartWalk.Client.iOS.Resources;
using SmartWalk.Client.iOS.Utils.Map;
using SmartWalk.Client.iOS.Controls;

namespace SmartWalk.Client.iOS.Utils.Map
{
    public class MapDelegate : MKMapViewDelegate
    {
        private readonly List<MKAnnotationView> _viewLinksList = 
            new List<MKAnnotationView>(); // to prevent GC

        private readonly string _annotationIdentifier = "BasicAnnotation";

        public MapDelegate()
        {
            CanShowCallout = true;
            CanShowDetails = true;
        }

        public bool IsMapBeingTouched { get; set; }

        public bool CanShowCallout { get; set; }
        public bool CanShowDetails { get; set; }

        public ICommand SelectAnnotationCommand { get; set; }
        public ICommand ShowDetailsCommand { get; set; }

        public override MKAnnotationView GetViewForAnnotation(
            MKMapView mapView,
            NSObject annotation)
        {
            if (annotation is MKUserLocation)
            {
                return null;
            }

            var annotationView = mapView.DequeueReusableAnnotation(_annotationIdentifier);
            if (annotationView == null)
            {
                annotationView = 
                    new MKAnnotationView(annotation, _annotationIdentifier);
            }
            else
            {
                annotationView.Annotation = annotation;
            }

            if (!_viewLinksList.Contains(annotationView))
            {
                _viewLinksList.Add(annotationView);
            }

            var mapAnnotation = annotation as IMapAnnotation;
            if (mapAnnotation != null)
            {
                annotationView.CanShowCallout = CanShowCallout;
                annotationView.Image = ThemeIcons.MapPin;
                annotationView.CenterOffset = Theme.MapPinOffset;
                annotationView.CalloutOffset = new PointF(0, 0);
                annotationView.RemoveSubviews();

                var numberLabel = new UILabel { 
                    TextColor = ThemeColors.ContentDarkText,
                    Font = Theme.MapPinFont,
                    TextAlignment = UITextAlignment.Center,
                    BackgroundColor = UIColor.Clear,
                    Text = mapAnnotation.Pin,
                    Frame = new RectangleF(Theme.MapPinTextOffset, new SizeF(25, 25))
                };
                annotationView.AddSubview(numberLabel);

                if (CanShowCallout && CanShowDetails)
                {
                    var detailButton = UIButton.FromType(UIButtonType.Custom);
                    detailButton.Frame = new RectangleF(0, 0, 32, 32);
                    detailButton.ImageView.ContentMode = UIViewContentMode.Center;
                    detailButton.ClipsToBounds = false;
                    detailButton.SetImage(ThemeIcons.Forward, UIControlState.Normal);
                    detailButton.TintColor = ThemeColors.Action;

                    detailButton.TouchUpInside += (s, e) => 
                        {
                            if (ShowDetailsCommand != null &&
                                ShowDetailsCommand.CanExecute(mapAnnotation.DataContext))
                            {
                                ShowDetailsCommand.Execute(mapAnnotation.DataContext);
                            }
                        };

                    annotationView.RightCalloutAccessoryView = detailButton;
                }
            }

            return annotationView;
        }

        public override void DidSelectAnnotationView(MKMapView mapView, MKAnnotationView view)
        {
            var mapAnnotation = view.Annotation as IMapAnnotation;
            var customMapView = mapView as CustomMKMapView;

            if (mapAnnotation != null &&
                customMapView != null &&
                customMapView.IsBeingTouched &&
                SelectAnnotationCommand != null &&
                SelectAnnotationCommand.CanExecute(mapAnnotation.DataContext))
            {
                SelectAnnotationCommand.Execute(mapAnnotation.DataContext);
            }
        }

        public override void DidDeselectAnnotationView(MKMapView mapView, MKAnnotationView view)
        {
            var customMapView = mapView as CustomMKMapView;

            if (customMapView != null &&
                customMapView.IsBeingTouched &&
                SelectAnnotationCommand != null &&
                SelectAnnotationCommand.CanExecute(null))
            {
                SelectAnnotationCommand.Execute(null);
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            ConsoleUtil.LogDisposed(this);
        }
    }
}