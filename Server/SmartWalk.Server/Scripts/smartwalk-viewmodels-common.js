﻿function ContactViewModel(data) {
    var self = this;

    self.Id = ko.observable();
    self.EntityId = ko.observable();
    self.Type = ko.observable();
    self.State = ko.observable();
    self.IsChecked = ko.observable();

    self.Title = ko.observable();
    self.Contact = ko.observable();

    self.DisplayContact = ko.computed(function () {
        return (self.Title() ? self.Title() : "") + (self.Contact() ? ' [' + self.Contact() + ']' : "");
    }, self);    

    self.loadData = function (data) {
        self.Id(data.Id);
        self.EntityId(data.EntityId);
        self.Type(data.Type);
        self.State(data.State);
        self.IsChecked(false);

        self.Title(data.Title);
        self.Contact(data.Contact);
    };

    self.loadData(data);
    
    if (data.validationUrl) {
        self.Contact.extend({ asyncValidation: { validationUrl: data.validationUrl, propName: 'Contact', model: $.parseJSON(ko.toJSON(self)) } });
        self.Title.extend({ asyncValidation: { validationUrl: data.validationUrl, propName: 'Title', model: $.parseJSON(ko.toJSON(self)) } });

        self.isValidating = ko.computed(function () {
            return self.Contact.isValidating() || self.Title.isValidating();
        }, self);
    };

    if (data.messages) {
        self.Type.extend({ dependencies: [self.Contact] });

        self.Contact
            .extend({ required: { params: true, message: data.messages.contactRequiredValidationMessage } })
            .extend({ maxLength: { params: 255, message: data.messages.contactLengthValidationMessage } })
            .extend({ contactValidation: { allowEmpty: true, contactType: self.Type, messages: data.messages } });
        
        self.Title
            .extend({ maxLength: { params: 255, message: data.messages.contactTitleValidationMessage } });
    };

    self.errors = ko.validation.group(self);
}

function AddressViewModel(data) {
    var self = this;

    self.Id = ko.observable();
    self.EntityId = ko.observable();
    self.Address = ko.observable();
    self.Tip = ko.observable();
    self.State = ko.observable();
    self.IsChecked = ko.observable();

    self.Latitude = ko.observable();
    self.Longitude = ko.observable();

    self.GetMapLink = function () {
        if (!self.Address())
            return "";
        var res = self.Address().replace(/&/g, "").replace(/,\s+/g, ",").replace(/\s+/g, "+");
        return "https://www.google.com/maps/embed/v1/place?q=" + res + "&key=AIzaSyAOwfPuE85Mkr-xoNghkIB7enlmL0llMgo";
    };    

    self.loadData = function (data) {
        self.Id(data.Id);
        self.EntityId(data.EntityId);
        self.Address(data.Address);
        self.Tip(data.Tip);
        self.State(data.State);
        self.IsChecked(false);

        self.Latitude(data.Latitude);
        self.Longitude(data.Longitude);
    };

    self.loadData(data);
    
    if (data.validationUrl) {
        self.Address.extend({ asyncValidation: { validationUrl: data.validationUrl, propName: 'Address', model: $.parseJSON(ko.toJSON(self)) } });
        self.Tip.extend({ asyncValidation: { validationUrl: data.validationUrl, propName: 'Tip', model: $.parseJSON(ko.toJSON(self)) } });

        self.isValidating = ko.computed(function () {
            return self.Address.isValidating() || self.Tip.isValidating();
        }, self);
    };
    
    if (data.messages) {
        self.Address
            .extend({ required: { params: true, message: data.messages.addressRequiredValidationMessage } })
            .extend({ maxLength: { params: 255, message: data.messages.addressLengthValidationMessage } });

        self.Tip
            .extend({ maxLength: { params: 255, message: data.messages.addressTipValidationMessage } });
    };

    self.errors = ko.validation.group(self);
}

function ShowViewModel(data) {
    var self = this;

    self.Id = ko.observable();
    self.EventMetadataId = ko.observable();
    self.VenueId = ko.observable();
    self.IsReference = ko.observable();
    self.Title = ko.observable();
    self.Description = ko.observable();
    self.StartDate = ko.observable();
    self.StartTime = ko.observable();
    self.EndDate = ko.observable();
    self.EndTime = ko.observable();
    self.Picture = ko.observable();
    self.DetailsUrl = ko.observable();
    self.State = ko.observable();

    self.IsChecked = ko.observable();


    self.TimeText = ko.computed(function () {
        if (self.EndTime()) {
            return self.StartTime() + '&nbsp-&nbsp' + self.EndTime();
        }

        return self.StartTime();
    }, this);

    self.loadData = function (data) {
        self.Id(data.Id);
        self.EventMetadataId(data.EventMetadataId);
        self.VenueId(data.VenueId);
        self.IsReference(data.IsReference);
        self.Title(data.Title);
        self.Description(data.Description);
        self.StartDate(data.StartDate ? data.StartDate : '');
        self.StartTime(data.StartTime ? data.StartTime : '');
        self.EndDate(data.EndDate ? data.EndDate : '');
        self.EndTime(data.EndTime ? data.EndTime : '');
        self.Picture(data.Picture);
        self.DetailsUrl(data.DetailsUrl);
        self.State(data.State);
        self.IsChecked(false);
    };

    self.loadData(data);
    
    if (data.messages) {
        self.Title
            .extend({ required: { params: true, message: data.messages.titleRequiredValidationMessage } })
            .extend({ maxLength: { params: 255, message: data.messages.titleLengthValidationMessage } });

        self.Picture
            .extend({ maxLength: { params: 255, message: data.messages.pictureLengthValidationMessage } })
            .extend({ urlValidation: { params: { allowEmpty: true }, message: data.messages.pictureValidationMessage } });
        
        self.DetailsUrl
            .extend({ maxLength: { params: 255, message: data.messages.detailsLengthValidationMessage } })
            .extend({ urlValidation: { params: { allowEmpty: true }, message: data.messages.detailsValidationMessage } });

        self.StartDate
            .extend({ dateCompareValidation: { params: { allowEmpty: true, cmp: 'LESS_THAN', compareVal: self.EndDate }, message: data.messages.startDateValidationMessage } })
            .extend({ dateCompareValidation: { params: { allowEmpty: true, cmp: 'REGION', compareVal: data.eventDtFrom, compareValTo: data.eventDtTo }, message: data.messages.startTimeValidationMessage } });

        self.EndDate
            .extend({ dateCompareValidation: { params: { allowEmpty: true, cmp: 'GREATER_THAN', compareVal: self.EndDate }, message: data.messages.endDateValidationMessage } })
            .extend({ dateCompareValidation: { params: { allowEmpty: true, cmp: 'REGION', compareVal: data.eventDtFrom, compareValTo: data.eventDtTo }, message: data.messages.endTimeValidationMessage } });
    };

    self.errors = ko.validation.group(self);
}