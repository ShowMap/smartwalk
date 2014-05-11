﻿using System;
using System.Linq;
using System.Threading.Tasks;
using MonoTouch.CoreLocation;
using SmartWalk.Client.Core.Model;
using SmartWalk.Client.Core.Services;
using SmartWalk.Client.Core.Utils;

namespace SmartWalk.Client.iOS.Services
{
    public class LocationService : ILocationService
    {
        private const string UnknownLocation = "Unknown Location";

        private readonly IExceptionPolicy _exceptionPolicy;
        private readonly CLLocationManager _locationManager;
        private readonly CLGeocoder _geocoder;

        public LocationService(IExceptionPolicy exceptionPolicy)
        {
            _exceptionPolicy = exceptionPolicy;

            if (CLLocationManager.LocationServicesEnabled)
            {
                _geocoder = new CLGeocoder();
                _locationManager = new CLLocationManager();
                _locationManager.DesiredAccuracy = CLLocation.AccuracyThreeKilometers;

                _locationManager.LocationsUpdated += (s, e) => 
                {
                    UpdateLocationString().ContinueWithThrow();

                    if (LocationChanged != null)
                    {
                        LocationChanged(this, EventArgs.Empty);
                    }
                };
            }

            UpdateLocationString().ContinueWithThrow();
        }

        public event EventHandler LocationChanged;
        public event EventHandler LocationStringChanged;

        public Location CurrentLocation 
        { 
            get
            { 
                if (_locationManager != null &&
                    _locationManager.Location != null)
                {
                    var result = new Location(
                        Math.Round(_locationManager.Location.Coordinate.Latitude, 2),
                        Math.Round(_locationManager.Location.Coordinate.Longitude, 2));
                    return result;
                }

                return Location.Empty;
            }
        }

        public string CurrentLocationString { get; private set; }

        private async Task UpdateLocationString()
        {
            if (_locationManager != null &&
                _locationManager.Location != null)
            {
                var placemarks = default(CLPlacemark[]);

                try
                {
                    placemarks = await _geocoder
                        .ReverseGeocodeLocationAsync(_locationManager.Location);
                }
                catch (Exception ex)
                {
                    _exceptionPolicy.Trace(ex, false);
                }

                if (placemarks != null)
                {
                    var placemark = placemarks.FirstOrDefault();
                    if (placemark != null)
                    {
                        CurrentLocationString = string.Format(
                            "{0}, {1}", 
                            placemark.Locality, 
                            placemark.Country);
                    }
                }
            }
            else
            {
                CurrentLocationString = UnknownLocation;
            }

            if (LocationStringChanged != null)
            {
                LocationStringChanged(this, EventArgs.Empty);
            }
        }
    }
}