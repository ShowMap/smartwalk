﻿using System;
using System.Net;
using Newtonsoft.Json;
using SmartWalk.Shared.DataContracts.Api;

namespace SmartWalk.Labs.Api
{
    public class MainClass
    {
        private const string SmartWalkUrl = @"http://smartwalk.com:8091/api";

        public static void Main(string[] args)
        {
            Console.WriteLine("SmartWalk Protocol JSON objects demo");

            Console.WriteLine("\nHome View");

            var homeViewRequest = RequestFactory.CreateHomeViewRequest("United States", "California", "San Francisco");
            var json = JsonConvert.SerializeObject(
                homeViewRequest,
                new JsonSerializerSettings
                    {
                        Formatting = Formatting.Indented,
                        NullValueHandling = NullValueHandling.Ignore
                    });
            Console.WriteLine(json);

            using (var client = new WebClient())
            {
                client.Headers[HttpRequestHeader.ContentType] = "application/json";

                try
                {
                    var result = client.UploadString(SmartWalkUrl, json);
                    Console.WriteLine(result);

                    var response = JsonConvert.DeserializeObject<Response>(result);
                    Console.WriteLine("Response hash code: " + response.GetHashCode());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            Console.WriteLine("\nEvent View");
            var eventViewRequest = RequestFactory.CreateEventViewRequest(5);
            json = JsonConvert.SerializeObject(
                eventViewRequest,
                new JsonSerializerSettings
                    {
                        Formatting = Formatting.Indented,
                        NullValueHandling = NullValueHandling.Ignore
                    });
            Console.WriteLine(json);

            using (var client = new WebClient())
            {
                client.Headers[HttpRequestHeader.ContentType] = "application/json";

                try
                {
                    var result = client.UploadString(SmartWalkUrl, json);
                    Console.WriteLine(result);

                    var response = JsonConvert.DeserializeObject<Response>(result);
                    Console.WriteLine("Response hash code: " + response.GetHashCode());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            Console.WriteLine("\nVenue View");
            var venueViewRequest = RequestFactory.CreateVenueViewRequest(6, new[] { 220, 221, 222, 223, 224 });
            json = JsonConvert.SerializeObject(
                venueViewRequest,
                new JsonSerializerSettings
                    {
                        Formatting = Formatting.Indented,
                        NullValueHandling = NullValueHandling.Ignore
                    });
            Console.WriteLine(json);

            using (var client = new WebClient())
            {
                client.Headers[HttpRequestHeader.ContentType] = "application/json";

                try
                {
                    var result = client.UploadString(SmartWalkUrl, json);
                    Console.WriteLine(result);

                    var response = JsonConvert.DeserializeObject<Response>(result);
                    Console.WriteLine("Response hash code: " + response.GetHashCode());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            Console.ReadLine();
        }
    }
}