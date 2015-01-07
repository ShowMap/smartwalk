using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Client.iOS.Resources;
using SmartWalk.Client.iOS.Utils;

namespace SmartWalk.Client.iOS.Controls
{
    [Register("ButtonBarButton")]
    public class ButtonBarButton : UIButton
    {
        public static readonly SizeF DefaultVerticalSize = 
            new SizeF(
                UIConstants.ToolBarVerticalHeight,
                UIConstants.ToolBarVerticalHeight);
        public static readonly SizeF DefaultLandscapeSize = 
            new SizeF(
                UIConstants.ToolBarHorizontalHeight, 
                UIConstants.ToolBarHorizontalHeight);

        private UIImageView _iconImageView;
        private SemiTransparentType _semiTransparentType;

        private Circle _background;

        public ButtonBarButton(IntPtr handle) : base(handle)
        {
            Initialize(
                null, 
                null,
                DefaultVerticalSize,
                DefaultLandscapeSize);
            UpdateState();
        }

        public ButtonBarButton(
            UIImage verticalIcon,
            UIImage landscapeIcon,
            SemiTransparentType semiTransparentType = SemiTransparentType.None) 
            : this(
                verticalIcon,
                landscapeIcon,
                DefaultVerticalSize,
                DefaultLandscapeSize,
                semiTransparentType) 
        {
        }

        public ButtonBarButton(
            UIImage verticalIcon,
            UIImage landscapeIcon,
            SizeF? verticalSize,
            SizeF? landscapeSize,
            SemiTransparentType semiTransparentType = SemiTransparentType.None)
                : base(UIButtonType.Custom)
        {
            Initialize(
                verticalIcon, 
                landscapeIcon,
                verticalSize,
                landscapeSize,
                semiTransparentType);
            UpdateState();
        }

        public UIImage VerticalIcon { get; set; }
        public UIImage LandscapeIcon { get; set; }
        public SizeF VerticalSize { get; set; }
        public SizeF LandscapeSize { get; set; }

        public SemiTransparentType SemiTransparentType
        {
            get
            {
                return _semiTransparentType;
            }
            set
            {
                if (_semiTransparentType != value)
                {
                    _semiTransparentType = value;
                    UpdateBackgroundState();
                }
            }
        }

        private UIImageView IconImageView
        {
            get
            {
                if (_iconImageView == null)
                {
                    _iconImageView = new UIImageView();
                    AddSubview(_iconImageView);
                }

                return _iconImageView;
            }
        }

        public void UpdateState()
        {
            var frame = Frame;

            if (ScreenUtil.IsVerticalOrientation)
            {
                Frame = new RectangleF(frame.Location, VerticalSize);

                if (VerticalIcon != null || LandscapeIcon != null)
                {
                    IconImageView.Frame = new RectangleF(PointF.Empty, VerticalSize);
                    IconImageView.Image = VerticalIcon;
                }
            }
            else
            {
                Frame = new RectangleF(frame.Location, LandscapeSize);

                if (VerticalIcon != null || LandscapeIcon != null)
                {
                    IconImageView.Frame = new RectangleF(PointF.Empty, LandscapeSize);
                    IconImageView.Image = LandscapeIcon ?? VerticalIcon;
                }
            }

            _background.Frame = Bounds;

            switch (SemiTransparentType)
            {
                case SemiTransparentType.Light:
                    IconImageView.TintColor = ThemeColors.ContentLightText;
                    break;

                case SemiTransparentType.Dark:
                    IconImageView.TintColor = ThemeColors.ContentDarkText;
                    break;

                case SemiTransparentType.None:
                    IconImageView.TintColor = ThemeColors.ContentDarkText;
                    break;
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            ConsoleUtil.LogDisposed(this);
        }

        private void Initialize(
            UIImage verticalIcon,
            UIImage landscapeIcon,
            SizeF? verticalSize,
            SizeF? landscapeSize,
            SemiTransparentType semiTransparentType = SemiTransparentType.None)
        {
            _background = new Circle {
                UserInteractionEnabled = false
            };
            Add(_background);

            VerticalIcon = verticalIcon;
            LandscapeIcon = landscapeIcon;
            VerticalSize = verticalSize ?? DefaultVerticalSize;
            LandscapeSize = landscapeSize ?? DefaultLandscapeSize;
            SemiTransparentType = semiTransparentType;

            TouchDown += (sender, e) => 
                _background.FillColor = ThemeColors.ContentLightHighlight.ColorWithAlpha(0.58f);
            TouchUpInside += (sender, e) => UpdateBackgroundState();
            TouchUpOutside += (sender, e) => UpdateBackgroundState();
        }

        private void UpdateBackgroundState()
        {
            switch (SemiTransparentType)
            {
                case SemiTransparentType.Light:
                    _background.FillColor = ThemeColors.PanelBackgroundAlpha;
                    _background.LineColor = ThemeColors.BorderDark.ColorWithAlpha(0.63f);
                    _background.LineWidth = ScreenUtil.HairLine;
                    break;

                case SemiTransparentType.Dark:
                    _background.FillColor = ThemeColors.ContentDarkBackground.ColorWithAlpha(0.35f);
                    _background.LineColor = UIColor.Clear;
                    _background.LineWidth = 0;
                    break;

                default:
                    _background.FillColor = UIColor.Clear;
                    _background.LineColor = UIColor.Clear;
                    _background.LineWidth = 0;
                    break;
            }
        }
    }

    public enum SemiTransparentType
    {
        None,
        Dark,
        Light
    }
}