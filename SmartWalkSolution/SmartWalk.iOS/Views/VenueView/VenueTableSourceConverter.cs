using System;
using System.Collections.Generic;
using System.Globalization;
using Cirrious.CrossCore.Converters;
using SmartWalk.Core.Model;
using SmartWalk.Core.ViewModels;
using SmartWalk.iOS.Views.Common;

namespace SmartWalk.iOS.Views.VenueView
{
    public class VenueTableSourceConverter : IMvxValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var venue = value as Venue;
            var venueViewModel = parameter as VenueViewModel;
            if (venue != null && venueViewModel != null)
            {
                var result = new List<GroupContainer>();

                result.Add(new GroupContainer(new [] { venueViewModel }));

                if (venue.Shows != null &&
                    venue.Shows.Length > 0)
                {
                    result.Add(
                        new GroupContainer(venue.Shows) 
                        {
                            Key = "Shows"
                        });
                }
                else
                {
                    result.Add(
                        new GroupContainer(new object[] {}) 
                        {
                            Key = "No Shows"
                        });
                }

                return result.ToArray();
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}