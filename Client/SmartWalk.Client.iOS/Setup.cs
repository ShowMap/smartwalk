using System;
using Cirrious.CrossCore;
using Cirrious.CrossCore.IoC;
using Cirrious.CrossCore.Plugins;
using Cirrious.MvvmCross.Plugins.DownloadCache;
using Cirrious.MvvmCross.Touch.Platform;
using Cirrious.MvvmCross.Touch.Views.Presenters;
using Cirrious.MvvmCross.ViewModels;
using MonoTouch.UIKit;
using SmartWalk.Client.Core;
using SmartWalk.Client.Core.Services;
using SmartWalk.Client.iOS.Utils;
using SmartWalk.Client.iOS.Utils.MvvmCross;

namespace SmartWalk.Client.iOS
{
    public class Setup : MvxTouchSetup
    {
        private readonly AppSettings _settings;

        private CacheConfiguration _picsCacheConfig;
        private CacheConfiguration _resizedPicsCacheConfig;
        private CacheConfiguration _dataCacheConfig;

        public Setup(
            MvxApplicationDelegate appDelegate, 
            IMvxTouchViewPresenter presenter,
            AppSettings settings)
            : base(appDelegate, presenter)
        {
            _settings = settings;
        }

        protected override IMvxApplication CreateApp()
        {
            InitializeCacheSettings();

#if DEBUG
            var host = _settings.DebugServerHost;
#else
            var host = _settings.ServerHost;
#endif

            Mvx.RegisterSingleton<IConfiguration>(
                new Configuration(host, _settings.PostponeTime, _dataCacheConfig));

            CreatableTypes()
                .EndingWith("Service")
                .AsInterfaces()
                .RegisterAsLazySingleton();

            return new SmartWalkApplication();
        }

        protected override IMvxPluginConfiguration GetPluginConfiguration(Type plugin)
        {
            return plugin == typeof(PluginLoader) ? _picsCacheConfig : null;
        }

        protected override void InitializeLastChance()
        {
            Mvx.RegisterSingleton<IMvxImageCache<UIImage>>(
                () => MvxPlus.CreateImageCache(_picsCacheConfig));

            Mvx.RegisterSingleton<IMvxResizedImageCache<UIImage>>(
                () => MvxPlus.CreateResizedImageCache(_resizedPicsCacheConfig));

            Mvx.RegisterType<IMvxResizedImageHelper<UIImage>, MvxResizedDynamicImageHelper<UIImage>>();
            Mvx.RegisterSingleton<IMvxHttpFileDownloader>(MvxPlus.CreateHttpFileDownloader);
            Mvx.LazyConstructAndRegisterSingleton<IMvxExtendedFileStore, MvxExtendedFileStore>();

            base.InitializeLastChance();
        }

        private void InitializeCacheSettings()
        {
            foreach (var cache in _settings.Caches)
            {
                if (cache.CacheName == "Pictures.MvvmCross")
                {
                    _picsCacheConfig = cache;
                }

                if (cache.CacheName == "ResizedPictures.MvvmCross")
                {
                    _resizedPicsCacheConfig = cache;
                }

                if (cache.CacheName == "Data.SmartWalk")
                {
                    _dataCacheConfig = cache;
                }
            }
        }
    }
}