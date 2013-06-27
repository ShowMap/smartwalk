using System;
using System.Collections.Generic;
using Cirrious.MvvmCross.ViewModels;

namespace SmartWalk.Core.ViewModels
{
    public interface IHomeViewModel : IMvxViewModel
	{
        string TestDateLabel { get; }

        void UpdateLabel();

		IEnumerable<Organization> Organizations { get; }
	}
}