﻿using SmartWalk.Shared;

namespace SmartWalk.Server.Records
{
    // ReSharper disable ClassWithVirtualMembersNeverInherited.Global
    public class AddressRecord
    // ReSharper restore ClassWithVirtualMembersNeverInherited.Global
    {
        [UsedImplicitly]
        public virtual int Id { get; set; }
        public virtual EntityRecord EntityRecord { get; set; }
        public virtual double Latitude { get; set; }
        public virtual double Longitude { get; set; }
        public virtual string Address { get; set; }
        public virtual string Tip { get; set; }
        public virtual bool IsDeleted { get; set; }
    }
}