﻿using SmartWalk.Shared.DataContracts;

namespace SmartWalk.Server.DataContracts
{
    public class Reference : IReference
    {
        public int Id { get; set; }
        public string Storage { get; set; }
        public int? Type { get; set; }
    }
}