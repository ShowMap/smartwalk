﻿using System;
using System.Collections.Generic;
using Orchard;
using SmartWalk.Server.Records;
using SmartWalk.Server.Services.EventService;
using SmartWalk.Server.ViewModels;

namespace SmartWalk.Server.Services.EntityService
{
    public interface IEntityService : IDependency {
        IList<EntityVm> GetEntities(SmartWalkUserRecord user, EntityType type, int pageNumber, int pageSize, Func<EntityRecord, IComparable> orderBy, bool isDesc, string searchString);
        IList<EntityVm> GetEventEntities(EventMetadataRecord eventRecord);
        IList<EntityVm> GetAccesibleUserVenues(SmartWalkUserRecord user, int eventId, int pageNumber, int pageSize, string searchString);
        bool IsNameExists(EntityVm item, EntityType type);
        AccessType GetEntityAccess(SmartWalkUserRecord user, int entityId);
        EntityVm GetEntityVmById(int entityId, EntityType type);
        EntityVm GetEntityVm(EntityRecord entity, LoadMode mode = LoadMode.Full);
        EntityVm SaveOrAddEntity(SmartWalkUserRecord user, EntityVm entityVm);
        void DeleteEntity(int hostId);

        ShowVm SaveOrAddShow(ShowVm item, int eventMetadataId, int venueId);
    }
}