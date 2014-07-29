﻿using System.Linq;
using SmartWalk.Server.Records;
using SmartWalk.Server.Services.EventService;
using SmartWalk.Server.ViewModels;
using SmartWalk.Shared.Utils;

namespace SmartWalk.Server.Services.EntityService
{
    public static class ViewModelContractFactory
    {
        public static EntityVm CreateViewModelContract(EntityRecord record, LoadMode mode)
        {
            if (record == null) return null;

            switch (mode)
            {
                case LoadMode.Compact:
                    return new EntityVm
                        {
                            Id = record.Id,
                            Name = record.Name,
                            Abbreviation = record.Name.GetAbbreviation(2),
                            Picture = record.Picture,
                            Addresses = record.AddressRecords.Select(CreateViewModelContract).ToList(),
                        };

                case LoadMode.Full:
                default:
                    return new EntityVm
                        {
                            Id = record.Id,
                            Type = record.Type,
                            Name = record.Name,
                            Abbreviation = record.Name.GetAbbreviation(2),
                            Picture = record.Picture,
                            Description = record.Description,
                            Contacts = record.ContactRecords.Select(CreateViewModelContract).ToList(),
                            Addresses = record.AddressRecords.Select(CreateViewModelContract).ToList(),
                        };
            }
        }

        public static EntityVm CreateViewModelContract(
            EntityRecord record, 
            EventMetadataRecord metadata)
        {
            if (record == null)
                return null;

            var res = CreateViewModelContract(record, LoadMode.Full);

            res.Shows =
                metadata.ShowRecords
                        .Where(s => s.EntityRecord.Id == record.Id && 
                            !s.IsDeleted && !s.IsReference)
                        .Select(CreateViewModelContract)
                        .ToArray();

            return res;
        }

        public static ShowVm CreateViewModelContract(ShowRecord record)
        {
            if (record == null)
                return null;

            return new ShowVm
                {
                    Id = record.Id,
                    Title = record.Title,
                    Description = record.Description,
                    StartTime = record.StartTime,
                    EndTime = record.EndTime,
                    Picture = record.Picture,
                    DetailsUrl = record.DetailsUrl
                };
        }

        private static AddressVm CreateViewModelContract(AddressRecord record)
        {
            if (record == null) return null;

            return new AddressVm
                {
                    Id = record.Id,
                    Address = record.Address,
                    Tip = record.Tip,
                    Latitude = record.Latitude,
                    Longitude = record.Longitude,
                };
        }

        private static ContactVm CreateViewModelContract(ContactRecord record)
        {
            if (record == null) return null;

            return new ContactVm
                {
                    Id = record.Id,
                    Type = record.Type,
                    Title = record.Title,
                    Contact = record.Contact
                };
        }
    }
}