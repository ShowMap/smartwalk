using System;
using Cirrious.MvvmCross.ViewModels;
using Cirrious.CrossCore;
using SmartWalk.Core.ViewModels;
using SmartWalk.Core.Services;

namespace SmartWalk.Core
{
	public class SmartWalkApplication : MvxApplication
	{
		public SmartWalkApplication()
		{
			Mvx.RegisterType<IHomeViewModel, HomeViewModel>();
            Mvx.RegisterSingleton<IMvxAppStart>(new MvxAppStart<HomeViewModel>());
		}
	}
}

