using System;
using SmartWalk.Core.Services;
using System.Collections.Generic;
using Cirrious.MvvmCross.ViewModels;

namespace SmartWalk.Core.ViewModels
{
    public class HomeViewModel : MvxViewModel, IHomeViewModel
	{
		private readonly IOrganizationService _organizationService;

		private IEnumerable<Organization> _organizations;

		public HomeViewModel(IOrganizationService organizationService)
		{
			_organizationService = organizationService;
		}

        string _testDateLabel;

        public string TestDateLabel
        {
            get
            {
                return _testDateLabel;
            }
            set
            {
                _testDateLabel = value;
                RaisePropertyChanged(() => TestDateLabel);
            }
        }

        public void UpdateLabel()
        {
            TestDateLabel = DateTime.Now.ToLongTimeString();
        }

		public IEnumerable<Organization> Organizations 
		{
			get
			{
				return _organizations;
			}
			set
			{
				_organizations = value;
				RaisePropertyChanged(() => Organizations);
			}
		}

		public override void Start()
		{
			UpdateOrganizations();
			base.Start();
		}

		private void UpdateOrganizations()
		{
			_organizationService.GetOrganizations((orgs, ex) => 
          		{
					if (ex == null) 
					{
						Organizations = orgs;
					}
					else 
					{
						// TODO: handling
					}
				});
		}
	}
}