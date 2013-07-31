using System;
using System.Collections;
using System.Drawing;
using Cirrious.MvvmCross.Binding.Touch.Views;
using MonoTouch.CoreAnimation;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Core.Model;
using SmartWalk.Core.ViewModels;
using SmartWalk.iOS.Utils;
using SmartWalk.Core.Utils;
using System.ComponentModel;

namespace SmartWalk.iOS.Views.Common.EntityCell
{
    public partial class EntityCell : TableCellBase
    {
        public const int DefaultLogoHeight = 240;

        private const int GotoButtonWidth = 60;
        private const int TextLineHeight = 19;
        private const int PagerHeight = 27;
        private const int Gap = 8;

        private const int MaxCollapsedCellHeight = 
            DefaultLogoHeight + 
            PagerHeight + 
            TextLineHeight * 3 + 
            Gap;

        public static readonly UINib Nib = UINib.FromName("EntityCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("EntityCell");

        private readonly MvxImageViewLoader _imageHelper;

        private UITapGestureRecognizer _descriptionTapGesture;
        private CAGradientLayer _bottomGradient;
        private int _proportionalImageHeight;

        public EntityCell(IntPtr handle) : base(handle)
        {
            _imageHelper = new MvxImageViewLoader(() => LogoImageView, () => UpdateScrollViewHeightState(true));
        }

        public Action<int, bool> ImageHeightUpdatedHandler { get; set; }
        public bool IsLogoSizeFixed { get; set; }
                
        public new EntityViewModel DataContext
        {
            get { return (EntityViewModel)base.DataContext; }
            set { base.DataContext = value; }
        }

        public override RectangleF Frame
        {
            set
            {
                var previousFrame = base.Frame;
                base.Frame = value;

                if (previousFrame.Size != value.Size)
                {
                    // if cell did not load yet
                    if (ScrollView == null)
                    {
                        return;
                    }

                    // resizing on frame change for smooth transition
                    UpdateScrollSubViewsConstraints();
                    UpdateScrollViewHeightState(false);
                }
            }
        }

        public static EntityCell Create()
        {
            return (EntityCell)Nib.Instantiate(null, null)[0];
        }

        public static float CalculateCellHeight(
            bool isExpanded, 
            Entity entity, 
            int logoHeight)
        {
            if (logoHeight == 0)
            {
                logoHeight = DefaultLogoHeight;
            }

            var noTextCellHeight = CalculateNoTextCellHeight(entity.Info, logoHeight);
            var textHeight = CalculateTextHeight(entity.Description);
            var linesCount = (int)Math.Round(
                1.0 * (MaxCollapsedCellHeight - noTextCellHeight) / TextLineHeight, 
                MidpointRounding.AwayFromZero);

            return isExpanded 
                ? noTextCellHeight + textHeight
                    : noTextCellHeight + Math.Min(textHeight, TextLineHeight * linesCount);
        }

        private static int CalculatePagesCount(EntityInfo info)
        {
            return info != null ? 
                (HasAddress(info) ? 1 : 0) +
                    (HasLogo(info) ? 1 : 0) +
                    (HasContact(info) ? 1 : 0) : 0;
        }

        private static int CalculatePagerHeight(EntityInfo info)
        {
            return CalculatePagesCount(info) < 2 ? Gap : PagerHeight;
        }

        private static float CalculateNoTextCellHeight(EntityInfo info, int logoHeight)
        {
            var isScrollVisible = IsScrollViewVisible(info);
            var pagerHeight = CalculatePagerHeight(info);
            var noTextCellHeight = (isScrollVisible ? logoHeight + pagerHeight : Gap) + Gap;
            return noTextCellHeight;
        }

        private static float CalculateTextHeight(string text)
        {
            if (text != null && text != string.Empty)
            {
                var frameSize = new SizeF(
                    ScreenUtil.CurrentScreenWidth - Gap * 2,
                    float.MaxValue); 
                var textSize = new NSString(text).StringSize(
                    UIFont.FromName("Helvetica", 15),
                    frameSize,
                    UILineBreakMode.TailTruncation);

                return textSize.Height;
            }

            return 0;
        }

        private static bool IsScrollViewVisible(EntityInfo info)
        {
            return info != null && (HasLogo(info) || HasContact(info) || HasAddress(info));
        }

        private static bool HasLogo(EntityInfo info)
        {
            return info != null && info.Logo != null;
        }

        private static bool HasContact(EntityInfo info)
        {
            return info != null && info.Contact != null && !info.Contact.IsEmpty;
        }

        private static bool HasAddress(EntityInfo info)
        {
            return info != null && info.Addresses != null && info.Addresses.Length > 0;
        }

        public override void DidRotate(UIInterfaceOrientation fromInterfaceOrientation)
        {
            base.DidRotate(fromInterfaceOrientation);

            //HACK: to fix the page position
            //TODO: to review EntityScrollViewDelegate.Scrolled() impl
            ScrollView.ContentOffset = new PointF(
                ScreenUtil.CurrentScreenWidth * PageControl.CurrentPage, 
                0);

            UpdateCollectionViewCellWidth();
            UpdateScrollViewHeightState(false);
            UpdateBottomGradientHiddenState();
        }

        public override void PrepareForReuse()
        {
            base.PrepareForReuse();

            DataContext = null;
            LogoImageView.Image = null;
            _proportionalImageHeight = 0;
            ImageHeightUpdatedHandler = null;
            MapView.RemoveAnnotations(MapView.Annotations);
        }

        protected override void OnInitialize()
        {
            InitializeGestures();
            InitializeScrollView();
            InitializeContactCollectionView();
            InitializeBottomGradientState();

            UpdateScrollSubViewsConstraints();
            UpdateCollectionViewCellWidth();
            UpdateScrollViewHeightState(false);
        }

        protected override void OnDataContextChanged(object previousContext, object newContext)
        {
            var context = previousContext as INotifyPropertyChanged;
            if (context != null)
            {
                context.PropertyChanged -= OnDataContextPropertyChanged;
            }

            context = newContext as INotifyPropertyChanged;
            if (context != null)
            {
                context.PropertyChanged += OnDataContextPropertyChanged;
            }

            UpdateBindedControls();
        }

        private void OnDataContextPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == DataContext.GetPropertyName(p => p.Entity) ||
                e.PropertyName == DataContext.GetPropertyName(p => p.IsDescriptionExpanded))
            {
                UpdateBindedControls();
            }
        }

        private void UpdateBindedControls()
        {
            _imageHelper.ImageUrl = DataContext != null 
                ? DataContext.Entity.Info.Logo : null;

            DescriptionLabel.Text = DataContext != null 
                ? DataContext.Entity.Description : null;

            var info = DataContext != null ? DataContext.Entity.Info : null;
            if (DataContext != null && HasAddress(info))
            {
                var annotation = new EntityAnnotation(
                    DataContext.Entity, 
                    DataContext.Entity.Info.Addresses[0]);
                MapView.SetRegion(
                    MapUtil.CoordinateRegionForCoordinates(annotation.Coordinate), false);
                MapView.AddAnnotation(annotation);
            }

            GoToMapButton.Hidden = DataContext != null && !HasAddress(info);
            GoToContactButton.Hidden = DataContext != null && !HasContact(info);

            PageControl.Pages = CalculatePagesCount(info);
            PageControl.Hidden = PageControl.Pages < 2;

            ((ContactCollectionSource)ContactCollectionView.WeakDataSource).ItemsSource =
                DataContext != null 
                    ? (IEnumerable)new ContactCollectionSourceConverter()
                    .Convert(DataContext.Entity.Info.Contact, typeof(IEnumerable), null, null) 
                    : null;

            UpdateScrollViewHeightState(false);
            UpdateScrollSubViewsConstraints();
            UpdateBottomGradientHiddenState();

            UpdateCurrentPageAndScroll(HasAddress(info) && HasLogo(info) ? 1 : 0, false);
        }

        private void InitializeGestures()
        {
            if (DescriptionLabel.GestureRecognizers == null ||
                DescriptionLabel.GestureRecognizers.Length == 0)
            {
                _descriptionTapGesture = new UITapGestureRecognizer(() => {
                    if (DataContext != null &&
                        DataContext.ExpandCollapseCommand.CanExecute(null))
                    {
                        DataContext.ExpandCollapseCommand.Execute(null);
                    }
                });

                _descriptionTapGesture.NumberOfTouchesRequired = (uint)1;
                _descriptionTapGesture.NumberOfTapsRequired = (uint)1;

                DescriptionLabel.AddGestureRecognizer(_descriptionTapGesture);
            }
        }

        private void InitializeScrollView()
        {
            if (ScrollView.Delegate == null)
            {
                ScrollView.Delegate = new EntityScrollViewDelegate(ScrollView, PageControl);
            }
        }

        private void InitializeContactCollectionView()
        {
            UpdateCollectionViewCellWidth();

            if (ContactCollectionView.Source == null)
            {
                var collectionSource = new ContactCollectionSource(ContactCollectionView);

                ContactCollectionView.Source = collectionSource;
                ContactCollectionView.Delegate = new ContactCollectionDelegate();

                ContactCollectionView.ReloadData();
            }
        }

        private void InitializeBottomGradientState()
        {
            _bottomGradient = new CAGradientLayer
                {
                    Frame = BottomGradientView.Bounds,
                    Colors = new [] { 
                        UIColor.White.ColorWithAlpha(0.2f).CGColor, 
                        UIColor.White.CGColor 
                    },
                };

            BottomGradientView.Layer.InsertSublayer(_bottomGradient, 0);
        }

        private void UpdateCollectionViewCellWidth()
        {
            var flowLayout = (UICollectionViewFlowLayout)ContactCollectionView.CollectionViewLayout;
            var itemsInRow = ScreenUtil.IsVerticalOrientation ? 1 : 2;

            var cellWith = (ScreenUtil.CurrentScreenWidth - 
                flowLayout.SectionInset.Left -
                flowLayout.SectionInset.Right - 
                flowLayout.MinimumInteritemSpacing * (itemsInRow - 1)) / itemsInRow;

            flowLayout.ItemSize = new SizeF(cellWith, 41);
        }

        private void UpdateScrollViewHeightState(bool updateTable)
        {
            _proportionalImageHeight = DefaultLogoHeight;
            if (!IsLogoSizeFixed && LogoImageView.Image != null)
            {
                var frame = LogoImageView.Layer.Frame;
                var imageSize = LogoImageView.SizeThatFits(frame.Size);
                _proportionalImageHeight = Math.Min(DefaultLogoHeight, 
                    (int)(1.0 * ScreenUtil.CurrentScreenWidth * imageSize.Height / imageSize.Width) + 1);
            }

            var info = DataContext != null ? DataContext.Entity.Info : null;
            var isScrollVisible = IsScrollViewVisible(info);

            ScrollViewHeightConstraint.Constant = isScrollVisible ? _proportionalImageHeight : 0;
            DescriptionTopSpaceConstraint.Constant = isScrollVisible ? CalculatePagerHeight(info) : Gap;

            UpdateBottomGradientHiddenState();

            if (!IsLogoSizeFixed && 
                ImageHeightUpdatedHandler != null)
            {
                ImageHeightUpdatedHandler(_proportionalImageHeight, updateTable);
            }
        }

        private void UpdateScrollSubViewsConstraints()
        {
            MapViewWithConstraint.Constant = 
                DataContext != null && HasAddress(DataContext.Entity.Info)
                    ? ScreenUtil.CurrentScreenWidth : 0;

            GoToMapButtonLeftConstraint.Constant = (int)MapViewWithConstraint.Constant != 0
                ? MapViewWithConstraint.Constant - GotoButtonWidth / 2 
                : 0;

            ImageViewWidthConstraint.Constant = 
                DataContext != null && HasLogo(DataContext.Entity.Info)
                    ? ScreenUtil.CurrentScreenWidth : 0;

            GoToContactButtonLeftConstraint.Constant = 
                (int)MapViewWithConstraint.Constant != 0 || (int)ImageViewWidthConstraint.Constant != 0
                    ? MapViewWithConstraint.Constant + ImageViewWidthConstraint.Constant - GotoButtonWidth / 2
                    : 0;

            ContactViewWidthConstraint.Constant = DataContext != null && HasContact(DataContext.Entity.Info)
                    ? ScreenUtil.CurrentScreenWidth : 0;
        }

        private void UpdateBottomGradientHiddenState()
        {
            if (_bottomGradient == null) return;

            _bottomGradient.Frame = BottomGradientView.Bounds;

            if (DataContext != null && !DataContext.IsDescriptionExpanded)
            {
                var textHeight = CalculateTextHeight(DataContext.Entity.Description);
                var labelHeight = CalculateCellHeight(false, DataContext.Entity, _proportionalImageHeight) - 
                    CalculateNoTextCellHeight(DataContext.Entity.Info, _proportionalImageHeight);

                _bottomGradient.Hidden = textHeight <= labelHeight;
            }
            else
            {
                _bottomGradient.Hidden = true;
            }
        }

        partial void OnGoToMapButtonTouchUpInside(UIButton sender, UIEvent @event)
        {
            UpdateCurrentPageAndScroll(PageControl.CurrentPage == 0 ? 1 : 0, true);
        }

        partial void OnGoToContactButtonTouchUpInside(UIButton sender, UIEvent @event)
        {
            UpdateCurrentPageAndScroll(PageControl.CurrentPage == 1 ? 2 : 1, true);
        }

        private void UpdateCurrentPageAndScroll(int page, bool animated)
        {
            PageControl.CurrentPage = page;
            ((EntityScrollViewDelegate)ScrollView.Delegate).ScrollToCurrentPage(animated);
        }
    }
}