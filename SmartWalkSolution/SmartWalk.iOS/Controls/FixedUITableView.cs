using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Drawing;

namespace SmartWalk.iOS.Controls
{
    // HACK: taken from http://stackoverflow.com/questions/14307037/bug-in-uitableview-layout-after-orientation-change
    [Register("FixedUITableView")]
    public class FixedUITableView : UITableView
    {
        public FixedUITableView(IntPtr handle) : base(handle)
        {
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            if (ContentSize != SizeF.Empty)
            {
                var contentSize = ContentSize;
                contentSize.Width = Bounds.Size.Width;
                ContentSize = contentSize;
            }
        }
    }
}