using System;
using Cirrious.CrossCore;
using Cirrious.CrossCore.Core;
using Cirrious.CrossCore.WeakSubscription;
using Cirrious.MvvmCross.Binding;
using SmartWalk.Client.iOS.Resources;
using UIKit;

namespace SmartWalk.Client.iOS.Utils.MvvmCross
{
    public class MvxResizedImageViewLoader : IDisposable
    {
        private readonly Func<UIImageView> _imageViewAccess;
        private readonly IMvxResizedImageHelper<UIImage> _imageHelper;
        private readonly Action<UIImage> _imageSetAction;

        private IDisposable _subscription;

        private string _imageUrl;

        public MvxResizedImageViewLoader(
            Func<UIImageView> imageViewAccess, 
            Action afterImageChangeAction = null)
        {
            _imageViewAccess = imageViewAccess;
            _imageSetAction = image =>
                {
                    OnImage(imageViewAccess(), image);

                    if (afterImageChangeAction != null)
                    {
                        afterImageChangeAction();
                    }
                };

            if (!Mvx.TryResolve(out _imageHelper))
            {
                MvxBindingTrace.Error("Can't resolve IMvxResizedImageHelper");
                return;
            }

            var eventInfo = _imageHelper.GetType().GetEvent("ImageChanged");
            _subscription = eventInfo.WeakSubscribe<UIImage>(_imageHelper, ImageHelperOnImageChanged);

            DefaultImagePath = Theme.DefaultImagePath;
            ErrorImagePath = Theme.ErrorImagePath;
        }

        public bool UseRoundClip { get; set; }

        ~MvxResizedImageViewLoader()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Note - this is public because we use it in weak referenced situations
        public virtual void ImageHelperOnImageChanged(
            object sender, 
            MvxValueEventArgs<UIImage> mvxValueEventArgs)
        {
            _imageSetAction(mvxValueEventArgs.Value);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_subscription != null)
                {
                    _subscription.Dispose();
                    _subscription = null;
                }
            }
        }

        public string ImageUrl
        {
            get { return _imageUrl; }
            set 
            { 
                _imageUrl = value;
                _imageHelper.ImageUrl = 
                    GetImageUrlWithDimensions(_imageUrl, _imageViewAccess());
            }
        }

        public string DefaultImagePath
        {
            get { return _imageHelper.DefaultImagePath; }
            set { _imageHelper.DefaultImagePath = value; }
        }

        public string ErrorImagePath
        {
            get { return _imageHelper.ErrorImagePath; }
            set { _imageHelper.ErrorImagePath = value; }
        }

        private static void OnImage(UIImageView imageView, UIImage image)
        {
            if (imageView != null && image != null)
            {
                imageView.Image = image;
            }
        }

        private string GetImageUrlWithDimensions(string imageUrl, UIView view)
        {
            var dimensions = view != null && !view.Frame.IsEmpty
                ? string.Format(
                    "{0}{1}>{2}{3}",
                    MvxPlus.SizeParam,
                    (int)Math.Ceiling(view.Frame.Width * UIScreen.MainScreen.Scale), 
                    (int)Math.Ceiling(view.Frame.Height * UIScreen.MainScreen.Scale),
                    UseRoundClip ? string.Format(">{0}>{1}", MvxPlus.ClipParam, "true") : string.Empty)
                : string.Empty;
            return imageUrl + dimensions;
        }
    }
}