using System;
using Cirrious.MvvmCross.Binding.BindingContext;
using Cirrious.MvvmCross.Binding.Touch.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Core.Model;
using SmartWalk.iOS.Views.Common;

namespace SmartWalk.iOS.Views.HomeView
{
    public partial class OrgCell : TableCellBase
    {
        public static readonly UINib Nib = UINib.FromName("OrgCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("OrgCell");

        private MvxImageViewLoader _imageHelper;

        public OrgCell(IntPtr handle) : base(handle)
        {
            _imageHelper = new MvxImageViewLoader(() => OrgImageView);

            this.DelayBind(() => {
                var set = this.CreateBindingSet<OrgCell, EntityInfo>();
                set.Bind(OrgNameLabel).To(info => info.Name);
                set.Bind(_imageHelper).To(info => info.Logo);
                set.Apply();
            });
        }

        public static OrgCell Create()
        {
            return (OrgCell)Nib.Instantiate(null, null)[0];
        }

        protected override bool Initialize()
        {
            var result = InitializeImageView();
            result = result && InitializeBackground();

            return result;
        }

        private bool InitializeImageView()
        {
            if (OrgImageView != null)
            {
                OrgImageView.BackgroundColor = UIColor.White;
                OrgImageView.ClipsToBounds = true;
                OrgImageView.Layer.BorderColor = UIColor.LightGray.CGColor;
                OrgImageView.Layer.BorderWidth = 1;
                OrgImageView.Layer.CornerRadius = 5;

                return true;
            }

            return false;
        }

        private bool InitializeBackground()
        {
            if (BackgroudPanel != null)
            {
                BackgroudPanel.Layer.BorderColor = UIColor.Gray.CGColor;
                BackgroudPanel.Layer.CornerRadius = 8;

                return true;
            }

            return false;
        }
    }
}

