namespace SmartWalk.Shared.DataContracts
{
    using System;

    public interface IEventMetadata
    {
        int Id { get; set; }

        IReference[] Host { get; set; }

        string Title { get; set; }

        string Description { get; set; }

        DateTime StartTime { get; set; }

        DateTime EndTime { get; set; }

        bool IsCombined { get; set; }

        IReference[] Shows { get; set; } 
    }
}