﻿EntityViewModelBase = function (data) {
    EntityViewModelBase.superClass_.constructor.call(this);

    this.Id = ko.observable();
    this.State = ko.observable();
    this.Type = ko.observable();
    this.Name = ko.observable();
    this.Picture = ko.observable();
    this.EventMetadataId = ko.observable();
    this.Description = ko.observable();

    this.AllContacts = ko.observableArray();
    this.AllAddresses = ko.observableArray();
    this.AllShows = ko.observableArray();

    //Contacts computed
    this.Contacts = ko.computed(function () {
        return this.Items_(this.AllContacts());
    }, this);

    this.DeletedContacts = ko.computed(function () {
        return this.DeletedItems_(this.AllContacts());
    }, this);

    this.CheckedContacts = ko.computed(function () {
        return this.CheckedItems_(this.Contacts());
    }, this);

    this.IsAnyContactSelected = ko.computed(function () {
        return this.IsAnyItemSelected_(this.Contacts());
    }, this);

    this.AllContactsChecked = ko.computed({
        read: function () {
            return this.GetAllItemsChecked_(this.Contacts());
        },
        write: function (value) {
            this.SetAllItemsChecked_(this.Contacts(), value);
        },
    }, this);

    //Addresses computed
    this.Addresses = ko.computed(function () {
        return this.Items_(this.AllAddresses());
    }, this);

    this.DeletedAddresses = ko.computed(function () {
        return this.DeletedItems_(this.AllAddresses());
    }, this);

    this.CheckedAddresses = ko.computed(function () {
        return this.CheckedItems_.call(this, this.Addresses());
    }, this);
    
    this.IsAnyAddressSelected = ko.computed(function () {
        return this.IsAnyItemSelected_(this.Addresses());
    }, this);

    this.AllAddressesChecked = ko.computed({
        read: function () {
            return this.GetAllItemsChecked_(this.Addresses());
        },
        write: function (value) {
            this.SetAllItemsChecked_(this.Addresses(), value);
        }
    }, this);

    this.IsMapVisible = ko.computed(function () {
        if (this.Addresses().length > 1)
            return true;
        if (this.Addresses().length == 1 && this.Addresses()[0] != this.selectedItem())
            return true;
        return false;
    }, this);

    //Shows computed
    this.Shows = ko.computed(function () {
        return this.Items_(this.AllShows());
    }, this);

    this.CheckedShows = ko.computed(function () {
        return this.CheckedItems_.call(this, this.Shows());
    }, this);

    this.AllShowsChecked = ko.computed({
        read: function () {
            return this.GetAllItemsChecked_(this.AllShows());
        },
        write: function (value) {
            this.SetAllItemsChecked_(this.AllShows(), value);
        }
    }, this);

    this.IsChecked = this.AllShowsChecked;
  
    this.toJSON = ko.computed(function () {
        return {
            Id: this.Id(),
            State: this.State(),
            Type: this.Type(),
            Name: this.Name(),
            Picture: this.Picture(),
            EventMetadataId: this.EventMetadataId(),
            Description: this.Description(),

            AllContacts: this.AllContacts(),
            AllAddresses: this.AllAddresses(),
            AllShows: this.AllShows(),
        };
    }, this);

    this.loadData(data);

    if (this.setupValidations)
        this.setupValidations(this);    
};

inherits(EntityViewModelBase, ViewModelBase);

EntityViewModelBase.prototype.DisplayAddress = ko.computed(function() {
    if (!this.AllAddresses || this.AllAddresses().length <= 0)
        return '';
    return this.AllAddresses()[0];
});

EntityViewModelBase.prototype.loadCollections_ = function (data) {
    var self = this;
    
    if (data.AllContacts)
        this.AllContacts($.map(data.AllContacts, function(item) { return new ContactViewModel(item); }));
    if (data.AllAddresses)
        this.AllAddresses($.map(data.AllAddresses, function(item) { return new AddressViewModel(item); }));
    if (data.AllShows)
        this.AllShows($.map(data.AllShows, function(item) { return new ShowViewModel(item); }));
};

EntityViewModelBase.prototype.loadData = function (data) {
    this.Id(data.Id);
    this.State(data.State);
    this.Type(data.Type);
    this.Name(data.Name);
    this.Picture(data.Picture);
    this.EventMetadataId(data.EventMetadataId);
    this.Description(data.Description);

    this.loadCollections_(data);
};

EntityViewModel = function (data) {
    EntityViewModel.superClass_.constructor.call(this, data);        
};

inherits(EntityViewModel, EntityViewModelBase);