// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace SmartWalk.Client.iOS.Views.HomeView
{
    [Register ("HomeView")]
    partial class HomeView
    {
        [Outlet]
        SmartWalk.Client.iOS.Views.HomeView.HomeCollectionView OrgCollectionView { get; set; }

        [Outlet]
        MonoTouch.UIKit.UIView ProgressViewContainer { get; set; }
        
        void ReleaseDesignerOutlets ()
        {
            if (OrgCollectionView != null) {
                OrgCollectionView.Dispose ();
                OrgCollectionView = null;
            }

            if (ProgressViewContainer != null) {
                ProgressViewContainer.Dispose ();
                ProgressViewContainer = null;
            }
        }
    }
}