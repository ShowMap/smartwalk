﻿using System;
using System.Xml.Serialization;
using SmartWalk.Client.iOS.Utils.MvvmCross;

namespace SmartWalk.Client.iOS.Utils
{
    [Serializable]
    [XmlRoot(ElementName = "settings")]
    public class AppSettings
    {
        [XmlElement("trackingId")]
        public string TrackingId { get;set; }

        [XmlElement("testFlightToken")]
        public string TestFlightToken { get;set; }

        [XmlElement("serverHost")]
        public string ServerHost { get;set; }

        [XmlElement("cachesPath")]
        public string CachesPath { get;set; }

        [XmlArray("caches")]
        [XmlArrayItem("cache")]
        public CacheConfiguration[] Caches { get;set; }
    }
}