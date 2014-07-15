﻿EventViewModelExtended = function (settings, data) {
    var self = this;

    EventViewModelExtended.superClass_.constructor.call(self, data);

    self.settings = settings;

    // TODO: to simplify after bug fix of jqAuto (hide "undefined" and support "valueProp")
    self.hostData = ko.computed({
        read: function () {
            return self.host() ? self.host().toJSON().Name || null : null;
        },
        write: function (hostData) {
            self.host(hostData && $.isPlainObject(hostData)
                ? new EntityViewModel(hostData) : null);
        }
    });
    
    EventViewModelExtended.setupDialogs(self);
    
    self.venuesManager = new VmItemsManager(
        self.venues,
        function () {
            var venue = new EntityViewModel({ Type: EntityType.Venue });
            return venue;
        },
        {
            setEditingItem: function (editingItem) {
                if (self.venuesManager.items()) {
                    self.venuesManager.items().forEach(function (venue) {
                        venue.isEditing(editingItem == venue);

                        if (venue.showsManager.items()) {
                            venue.showsManager.items().forEach(function (show) {
                                show.isEditing(editingItem == show);
                            });
                        }
                    });
                }
            },
            initItem: function (venue) {
                EventViewModelExtended.initVenueViewModel(venue, self);
            },
            beforeSave: function (venue) {
                if (!venue.errors) {
                    EventViewModelExtended.setupVenueValidation(venue, self.settings);
                }
            },
            itemView: self.settings.eventVenueView,
            itemEditView: self.settings.eventVenueEditView
        });

    self.createVenue = function () {
        $(self.settings.venueFormName).dialog("open");
    };
    
    self.createHost = function () {
        $(self.settings.hostFormName).dialog("open");
    };
    
    self.getAutocompleteHosts = function (searchTerm, callback) {
        ajaxJsonRequest(
            { term: searchTerm },
            self.settings.hostAutocompleteUrl,
            callback
        );
    };

    self.getAutocompleteVenues = function (searchTerm, callback) {
        ajaxJsonRequest(
            {
                term: searchTerm,
                onlyMine: false,
                excludeIds: self.venuesManager.items() 
                    ? $.map(self.venuesManager.items(),
                        function (venue) { return venue.id(); })
                    : null
            },
            self.settings.venueAutocompleteUrl,
            callback
        );
    };
    
    self.saveEvent = function () {
        if (!self.errors) {
            EventViewModelExtended.setupValidation(self, settings);
        }
        
        if (self.isValidating()) {
            setTimeout(function () { self.saveEvent(); }, 50);
            return false;
        }

        if (self.errors().length == 0) {
            ajaxJsonRequest(self.toJSON(), self.settings.eventSaveUrl,
                function (eventData) {
                    self.settings.eventAfterSaveAction(eventData.Id);
                },
                function () {
                    // TODO: To show error message
                }
            );
        } else {
            self.errors.showAllMessages();
        }

        return true;
    };
};

inherits(EventViewModelExtended, EventViewModel);

// Static Methods
EventViewModelExtended.setupValidation = function (event, settings) {
    event.startDate
        .extend({ required: { message: settings.startTimeRequiredValidationMessage } })
        .extend({
            dateCompareValidation: {
                params: {
                    allowEmpty: true,
                    cmp: "LESS_THAN",
                    compareVal: event.endDate
                },
                message: settings.startTimeCompareValidationMessage
            }
        });

    event.endDate.extend({
        dateCompareValidation: {
            params: {
                allowEmpty: true,
                cmp: "GREATER_THAN",
                compareVal: event.startDate
            },
            message: settings.endTimeCompareValidationMessage
        },
    });

    event.host.extend({
        required: { message: settings.hostRequiredValidationMessage },
    });

    event.picture
        .extend({
            maxLength: {
                params: 255,
                message: settings.pictureLengthValidationMessage
            }
        })
        .extend({
            urlValidation: {
                params: { allowEmpty: true },
                message: settings.picturePatternValidationMessage
            }
        });

    event.isValidating = ko.computed(function () {
        return event.startDate.isValidating() ||
            event.host.isValidating() ||
            event.picture.isValidating();
    });

    event.errors = ko.validation.group({
        startDate: event.startDate,
        endDate: event.endDate,
        host: event.host,
        picture: event.picture,
    });
};

EventViewModelExtended.setupShowValidation = function (show, event, settings) {
    show.title
        .extend({ required: { params: true, message: settings.showMessages.titleRequiredValidationMessage } })
        .extend({ maxLength: { params: 255, message: settings.showMessages.titleLengthValidationMessage } });

    show.picture
        .extend({ maxLength: { params: 255, message: settings.showMessages.pictureLengthValidationMessage } })
        .extend({ urlValidation: { params: { allowEmpty: true }, message: settings.showMessages.pictureValidationMessage } });

    show.detailsUrl
        .extend({ maxLength: { params: 255, message: settings.showMessages.detailsLengthValidationMessage } })
        .extend({ urlValidation: { params: { allowEmpty: true }, message: settings.showMessages.detailsValidationMessage } });

    // TODO: To use wider regions for show time -1 day for start and +1 for end times
    show.startTime
        .extend({
            dateCompareValidation: {
                params: {
                    allowEmpty: true,
                    cmp: "LESS_THAN",
                    compareVal: show.endTime
                },
                message: settings.showMessages.startTimeValidationMessage
            }
        })
        .extend({
            dateCompareValidation: {
                params: {
                    allowEmpty: true,
                    cmp: "REGION",
                    compareVal: ko.computed(function () { return addDays(event.startDate(), -1); }),
                    compareValTo: ko.computed(function () { return addDays(event.endDate(), 1); }),
                },
                message: settings.showMessages.startDateValidationMessage
            }
        });

    show.endTime
        .extend({
            dateCompareValidation: {
                params: {
                    allowEmpty: true,
                    cmp: "GREATER_THAN",
                    compareVal: show.startTime
                },
                message: settings.showMessages.endTimeValidationMessage
            }
        })
        .extend({
            dateCompareValidation: {
                params: {
                    allowEmpty: true,
                    cmp: "REGION",
                    compareVal: ko.computed(function () { return addDays(event.startDate(), -1); }),
                    compareValTo: ko.computed(function () { return addDays(event.endDate(), 1); }),
                },
                message: settings.showMessages.endDateValidationMessage
            }
        });
    
    show.isValidating = ko.computed(function () {
        return show.title.isValidating() || show.picture.isValidating() ||
            show.detailsUrl.isValidating() || show.startTime.isValidating() ||
            show.endTime.isValidating();
    });

    show.errors = ko.validation.group(show);
};

EventViewModelExtended.setupVenueValidation = function(venue, settings) {
    venue.id
        .extend({
            required: {
                message: settings.venueRequiredValidationMessage
            }
        });

    venue.errors = ko.validation.group(venue);
};

EventViewModelExtended.initVenueViewModel = function (venue, event) {
    venue.showsManager = new VmItemsManager(
        venue.shows,
        function () {
            var show = new ShowViewModel({});
            return show;
        },
        {
            setEditingItem: function(item) {
                 return event.venuesManager.setEditingItem(item);
            },
            beforeSave: function (show) {
                if (!show.errors) {
                    EventViewModelExtended.setupShowValidation(show, event, event.settings);
                }
            },
            itemView: event.settings.showView,
            itemEditView: event.settings.showEditView
        });
};

EventViewModelExtended.setupDialogs = function (event) {
    var dialogOptions = {
        modal: true,
        autoOpen: false,
        resizable: false,
        width: 700,
        maxHeight: 600,
        close: function () {
            var entity = ko.dataFor(this);
            entity.loadData({});
            if (entity.errors) {
                entity.errors.showAllMessages(false);
            }
        },
    };

    var hostOptions = {
        title: event.settings.dialogCreateHostText,
        buttons: [
            {
                "class": "btn btn-default",
                text: event.settings.dialogCancelText,
                click: function () { $(this).dialog("close"); }
            },
            {
                "class": "btn btn-success",
                text: event.settings.dialogAddHostText,
                click: function () {
                    var dialog = this;
                    var host = ko.dataFor(dialog);
                    host.saveEntity(function (entityData) {
                        var newHost = new EntityViewModel(entityData);
                        event.host(newHost);
                        $(dialog).dialog("close");
                    });
                }
            }
        ]};
    $(event.settings.hostFormName).dialog($.extend(dialogOptions, hostOptions));
    
    var venueOptions = {
        title: event.settings.dialogCreateVenueText,
        buttons: [
            {
                "class": "btn btn-default",
                text: event.settings.dialogCancelText,
                click: function () { $(this).dialog("close"); }
            },
            {
                "class": "btn btn-success",
                text: event.settings.dialogAddVenueText,
                click: function () {
                    var dialog = this;
                    var venue = ko.dataFor(dialog);
                    venue.saveEntity(function (entityData) {
                        var editingVenue = $.grep(event.venuesManager.items(),
                            function (item) { return item.isEditing(); })[0];
                        if (editingVenue) {
                            editingVenue.loadData(entityData);
                            $(dialog).dialog("close");
                        }
                    });
                }
            }
        ]};
    $(event.settings.venueFormName).dialog($.extend(dialogOptions, venueOptions));
};