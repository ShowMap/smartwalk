// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace SmartWalk.Client.iOS.Views.Common.EntityCell
{
	[Register ("MapCell")]
	partial class MapCell
	{
		[Outlet]
		MonoTouch.UIKit.UIView CoverView { get; set; }

		[Outlet]
		MonoTouch.MapKit.MKMapView MapView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (MapView != null) {
				MapView.Dispose ();
				MapView = null;
			}

			if (CoverView != null) {
				CoverView.Dispose ();
				CoverView = null;
			}
		}
	}
}
