using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Client.Core.Model;
using SmartWalk.Client.iOS.Views.Common.Base;
using SmartWalk.Client.iOS.Utils.MvvmCross;

namespace SmartWalk.Client.iOS.Views.HomeView
{
    public partial class OrgCell : CollectionCellBase
    {
        public static readonly UINib Nib = UINib.FromName("OrgCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("OrgCell");

        private readonly MvxResizedImageViewLoader _imageHelper;

        public const float DefaultHeight = 74;

        public OrgCell(IntPtr handle) : base(handle)
        {
            _imageHelper = new MvxResizedImageViewLoader(() => OrgImageView);
        }

        public new OrgEvent DataContext
        {
            get { return (OrgEvent)base.DataContext; }
            set { base.DataContext = value; }
        }

        public static OrgCell Create()
        {
            return (OrgCell)Nib.Instantiate(null, null)[0];
        }

        protected override void OnInitialize()
        {
            Separator.IsLineOnTop = true;
        }

        protected override void OnDataContextChanged()
        {
            OrgImageView.Image = null;

            _imageHelper.ImageUrl = DataContext != null ? DataContext.Picture : null;
            OrgNameLabel.Text = DataContext != null ? DataContext.Title : null;
        }
    }
}