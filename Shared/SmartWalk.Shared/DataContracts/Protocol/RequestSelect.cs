﻿namespace SmartWalk.Shared.DataContracts.Protocol
{
    public class RequestSelect
    {
        public string[] Fields { get; set; }

        public string As { get; set; }

        public string From { get; set; }

        public RequestSelectWhere Where { get; set; }
    }
}