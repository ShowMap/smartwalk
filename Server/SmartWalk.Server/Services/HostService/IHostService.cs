﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard;
using SmartWalk.Server.Records;
using SmartWalk.Server.ViewModels;

namespace SmartWalk.Server.Services.HostService
{
    public interface IHostService : IDependency {
        IList<EntityVm> GetUserHosts(SmartWalkUserRecord user);
        EntityVm GetHostVmById(int hostId);
        EntityRecord SaveOrAddHost(SmartWalkUserRecord user, EntityVm hostVm);
        void DeleteHost(int hostId);

        ContactRecord SaveOrAddContact(EntityRecord host, ContactVm contactVm);
        void DeleteContact(int contactId);
    }
}