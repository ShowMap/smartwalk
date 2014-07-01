﻿namespace SmartWalk.Server.ViewModels
{
    public class AddressVm
    {
        public int Id { get; set; }
        public int EntityId { get; set; }
        public VmItemState State { get; set; }
        public string Address { get; set; }
        public string Tip { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}