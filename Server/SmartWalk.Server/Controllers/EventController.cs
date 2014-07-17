﻿using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Themes;
using SmartWalk.Server.Controllers.Base;
using SmartWalk.Server.Extensions;
using SmartWalk.Server.Models;
using SmartWalk.Server.Records;
using SmartWalk.Server.Services.EntityService;
using SmartWalk.Server.Services.EventService;
using SmartWalk.Server.ViewModels;
using SmartWalk.Server.Views;

namespace SmartWalk.Server.Controllers
{
    [HandleError, Themed]
    public class EventController : BaseController {
        private readonly IOrchardServices _orchardServices;

        private readonly IEventService _eventService;

        public EventController(
            IEventService eventService,
            IOrchardServices orchardServices)
        {
            _orchardServices = orchardServices;
            _eventService = eventService;
        }

        public ActionResult List(
            DisplayType display = DisplayType.All, 
            SortType sort = SortType.Date)
        {
            var user = _orchardServices.WorkContext.CurrentUser.As<SmartWalkUserPart>();

            var result = _eventService.GetEvents(
                user == null || display == DisplayType.All
                    ? null
                    : user.Record,
                0,
                ViewSettings.ItemsLoad,
                GetSortFunc(sort),
                sort == SortType.Date);

            return View(new ListViewVm
                {
                    Parameters = new ListViewParametersVm { Display = display, Sort = sort },
                    Data = result
                });
        }

        public ActionResult View(int eventId) {
            var item = _eventService.GetEventVmById(eventId);

            if (item.Id != eventId)
                return new HttpNotFoundResult();

            return View(new ViewParametersVm { IsReadOnly = _orchardServices.WorkContext.CurrentUser == null, Data = item });
        }

        // TODO: Make sure the rest of controllers' method have CompressFilter
        [CompressFilter]
        public ActionResult Edit(int eventId)
        {
            if (_orchardServices.WorkContext.CurrentUser == null)
                return new HttpUnauthorizedResult();

            var user = _orchardServices.WorkContext.CurrentUser.As<SmartWalkUserPart>();

            var item = _eventService.GetEventVmById(eventId);

            if (item.Id != eventId)
                return new HttpNotFoundResult();

            var access = _eventService.GetEventAccess(user.Record, eventId);

            if (access == AccessType.AllowEdit)
                return View(item);

            return new HttpUnauthorizedResult();
        }

        public ActionResult Create()
        {
            return Edit(0);
        }

        #region Addresses

        // TODO: To validate address on event Save
        private IDictionary<string, string> ValidateAddress(AddressVm model)
        {
            var res = new Dictionary<string, string>();

            #region Address
            if (string.IsNullOrEmpty(model.Address))
                res.Add("Address", T("Address can not be empty!").Text);
            else if (model.Address.Length > 255)
                res.Add("Address", T("Address can not be larger than 255 characters!").Text);
            #endregion

            #region Tip
            if (!string.IsNullOrEmpty(model.Tip))
            {
                if (model.Tip.Length > 255)
                    res.Add("Tip", T("Address tip can not be larger than 255 characters!").Text);
            }
            #endregion

            return res;
        }

        #endregion

        #region Contacts

        // TODO: To validate contact on event Save
        private IDictionary<string, string> ValidateContact(ContactVm model)
        {
            var res = new Dictionary<string, string>();

            #region Contact
            if (string.IsNullOrEmpty(model.Contact))
                res.Add("Contact", T("Contact can not be empty!").Text);
            else if (model.Contact.Length > 255)
                res.Add("Contact", T("Contact can not be larger than 255 characters!").Text);
            #endregion

            #region Title
            if (!string.IsNullOrEmpty(model.Title))
            {
                if (model.Title.Length > 255)
                    res.Add("Title", T("Contact title can not be larger than 255 characters!").Text);
            }
            #endregion

            return res;
        }

        #endregion

        #region Shows

        // TODO: To validate show on event Save
        private IDictionary<string, string> ValidateShow(ShowVm model)
        {
            var res = new Dictionary<string, string>();

            #region Title
            if (string.IsNullOrEmpty(model.Title))
                res.Add("Title", T("Title can not be empty!").Text);
            else if (model.Title.Length > 255)
                res.Add("Title", T("Title can not be larger than 255 characters!").Text);
            #endregion

            #region Picture
            if (!string.IsNullOrEmpty(model.Picture))
            {
                if (model.Picture.Length > 255)
                    res.Add("Picture", T("Picture url can not be larger than 255 characters!").Text);
                else if (!model.Picture.IsUrlValid())
                    res.Add("Picture", T("Picture url is in bad format!").Text);
            }
            #endregion

            #region DetailsUrl
            if (!string.IsNullOrEmpty(model.DetailsUrl))
            {
                if (model.DetailsUrl.Length > 255)
                    res.Add("DetailsUrl", T("Details url can not be larger than 255 characters!").Text);
                else if (!model.DetailsUrl.IsUrlValid())
                    res.Add("DetailsUrl", T("Details url is in bad format!").Text);
            }
            #endregion

            return res;
        }

        #endregion

        #region Events

        // TODO: To validate if host is owned by current user
        // TODO: To validate if there are duplicated venues
        private IDictionary<string, string> ValidateEvent(EventMetadataVm model)
        {
            var res = new Dictionary<string, string>();

            #region StartDate
            if (!model.StartDate.HasValue)
                res.Add("StartDate", T("StartDate can not be empty!").Text);            
            #endregion

            #region Picture
            if (!string.IsNullOrEmpty(model.Picture))
            {
                if (model.Picture.Length > 255)
                    res.Add("Picture", T("Picture url can not be larger than 255 characters!").Text);
                else if (!model.Picture.IsUrlValid())
                    res.Add("Picture", T("Picture url is in bad format!").Text);
            }
            #endregion

            #region EndDate
            if (model.Host == null)
                res.Add("Host", T("Event organizer can not be empty!").Text);
            #endregion

            return res;
        }

        [HttpPost]
        [CompressFilter]
        public ActionResult GetEvents(int pageNumber, string query, ListViewParametersVm parameters)
        {
            var user = _orchardServices.WorkContext.CurrentUser.As<SmartWalkUserPart>();

            var result = _eventService.GetEvents(
                user == null || parameters.Display == DisplayType.All
                    ? null
                    : user.Record,
                pageNumber,
                ViewSettings.ItemsLoad,
                GetSortFunc(parameters.Sort),
                parameters.Sort == SortType.Date,
                query);
            return Json(result);
        }

        [HttpPost]
        [CompressFilter]
        public ActionResult SaveEvent(EventMetadataVm item)
        {
            if (_orchardServices.WorkContext.CurrentUser == null)
            {
                return new HttpUnauthorizedResult();
            }

            var user = _orchardServices.WorkContext.CurrentUser.As<SmartWalkUserPart>();

            var errors = ValidateEvent(item);
            return Json(errors.Count > 0 ? null : _eventService.SaveOrAddEvent(user.Record, item));
        } 

        public ActionResult DeleteEvent(int eventId)
        {
            if (_orchardServices.WorkContext.CurrentUser == null)
            {
                return new HttpUnauthorizedResult();
            }

            _eventService.DeleteEvent(eventId);

            return RedirectToAction("List");
        }
        #endregion

        private Func<EventMetadataRecord, IComparable> GetSortFunc(SortType sortType)
        {
            var result =
                sortType == SortType.Date
                    ? new Func<EventMetadataRecord, IComparable>(emr => emr.StartTime)
                    : (emr => emr.Title ?? emr.EntityRecord.Name);
            return result;
        }
    }
}