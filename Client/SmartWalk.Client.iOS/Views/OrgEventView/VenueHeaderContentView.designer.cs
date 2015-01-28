// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace SmartWalk.Client.iOS.Views.OrgEventView
{
	[Register ("VenueHeaderContentView")]
	partial class VenueHeaderContentView
	{
		[Outlet]
		SmartWalk.Client.iOS.Controls.CopyLabel AddressLabel { get; set; }

		[Outlet]
		SmartWalk.Client.iOS.Controls.Line BottomSeparator { get; set; }

		[Outlet]
		UIKit.UIImageView GoRightImageView { get; set; }

		[Outlet]
		UIKit.UILabel ImageLabel { get; set; }

		[Outlet]
		UIKit.UIView ImageLabelView { get; set; }

		[Outlet]
		UIKit.UIImageView LogoImageView { get; set; }

		[Outlet]
		SmartWalk.Client.iOS.Controls.CopyLabel NameLabel { get; set; }

		[Outlet]
		UIKit.UIButton NavigateOnMapButton { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint PinTopGapConstraint { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint TitleLeftGapConstraint { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint TitleTopGapConstraint { get; set; }

		[Outlet]
		SmartWalk.Client.iOS.Controls.Line TopSeparator { get; set; }

		[Action ("OnNavigateOnMapClick:")]
		partial void OnNavigateOnMapClick (UIKit.UIButton sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (AddressLabel != null) {
				AddressLabel.Dispose ();
				AddressLabel = null;
			}

			if (BottomSeparator != null) {
				BottomSeparator.Dispose ();
				BottomSeparator = null;
			}

			if (GoRightImageView != null) {
				GoRightImageView.Dispose ();
				GoRightImageView = null;
			}

			if (ImageLabel != null) {
				ImageLabel.Dispose ();
				ImageLabel = null;
			}

			if (ImageLabelView != null) {
				ImageLabelView.Dispose ();
				ImageLabelView = null;
			}

			if (LogoImageView != null) {
				LogoImageView.Dispose ();
				LogoImageView = null;
			}

			if (NameLabel != null) {
				NameLabel.Dispose ();
				NameLabel = null;
			}

			if (NavigateOnMapButton != null) {
				NavigateOnMapButton.Dispose ();
				NavigateOnMapButton = null;
			}

			if (PinTopGapConstraint != null) {
				PinTopGapConstraint.Dispose ();
				PinTopGapConstraint = null;
			}

			if (TitleLeftGapConstraint != null) {
				TitleLeftGapConstraint.Dispose ();
				TitleLeftGapConstraint = null;
			}

			if (TitleTopGapConstraint != null) {
				TitleTopGapConstraint.Dispose ();
				TitleTopGapConstraint = null;
			}

			if (TopSeparator != null) {
				TopSeparator.Dispose ();
				TopSeparator = null;
			}
		}
	}
}
