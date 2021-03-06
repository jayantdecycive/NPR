﻿

/*//////////////////////////////////////////////////////////////////////////////////////
////// Autogenerated by JaySvcUtil.exe http://JayData.org for more info        /////////
//////                             oData V3                                    /////////
//////////////////////////////////////////////////////////////////////////////////////*/
(function(global, $data, undefined) {

  $data.Entity.extend('cfacore.shared.domain.store.Store', {
    'LocationNumber': { 'key':true,'type':'Edm.String','nullable':false,'required':true },
    'BusinessConsultantId': { 'type':'Edm.String' },
    'ConceptCode': { 'type':'Edm.String','nullable':false,'required':true },
    'ConceptCodeId': { 'type':'Edm.Int32','nullable':false,'required':true },
    'CorporateOwned': { 'type':'Edm.Boolean','nullable':false,'required':true },
    'Phone': { 'type':'cfacore.shared.domain.user.Phone' },
    'PhoneString': { 'type':'Edm.String' },
    'Fax': { 'type':'cfacore.shared.domain.user.Phone' },
    'FaxString': { 'type':'Edm.String' },
    'VoiceMail': { 'type':'cfacore.shared.domain.user.Phone' },
    'VoiceMailString': { 'type':'Edm.String' },
    'Email': { 'type':'Edm.String','nullable':false,'required':true },
    'Features': { 'type':'cfacore.shared.domain.store.StoreFeatures' },
    'Playground': { 'type':'Edm.String','nullable':false,'required':true },
    'FinancialConsultantId': { 'type':'Edm.String' },
    'GMTOffset': { 'type':'Edm.String' },
    'Coordinates': { 'type':'cfacore.shared.domain.store.GeographicCoordinate' },
    'LocationCode': { 'type':'Edm.String','nullable':false,'required':true },
    'LocationCodeId': { 'type':'Edm.Int32','nullable':false,'required':true },
    'MarketableName': { 'type':'Edm.String','nullable':false,'required':true },
    'MarketableURL': { 'type':'System.Uri' },
    'MarketableUrlString': { 'type':'Edm.String' },
    'Message': { 'type':'Edm.String' },
    'LocationDescription': { 'type':'Edm.String' },
    'Name': { 'type':'Edm.String','nullable':false,'required':true },
    'OpenDate': { 'type':'Edm.DateTime','nullable':false,'required':true },
    'OperatorTeamName': { 'type':'Edm.String' },
    'PriceGroupNumber': { 'type':'Edm.String' },
    'ProjectedOpenDate': { 'type':'Edm.DateTime','nullable':false,'required':true },
    'RegionName': { 'type':'Edm.String' },
    'ServiceTeamName': { 'type':'Edm.String' },
    'Status': { 'type':'Edm.String','nullable':false,'required':true },
    'StatusId': { 'type':'Edm.Int32','nullable':false,'required':true },
    'SiteStatus': { 'type':'Edm.String' },
    'BusinessConsultant': { 'type':'cfacore.domain.user.User','inverseProperty':'$$unbound' },
    'BillingAddress': { 'type':'cfacore.shared.domain.user.Address','inverseProperty':'$$unbound' },
    'ShippingAddress': { 'type':'cfacore.shared.domain.user.Address','inverseProperty':'$$unbound' },
    'StreetAddress': { 'type':'cfacore.shared.domain.user.Address','inverseProperty':'$$unbound' },
    'Distributor': { 'type':'cfacore.shared.domain.store.Distributor','inverseProperty':'$$unbound' },
    'FinancialConsultant': { 'type':'cfacore.domain.user.User','inverseProperty':'$$unbound' },
    'LocationContact': { 'type':'cfacore.domain.user.User','inverseProperty':'$$unbound' },
    'MarketingConsultant': { 'type':'cfacore.domain.user.User','inverseProperty':'$$unbound' },
    'Operator': { 'type':'cfacore.domain.user.User','inverseProperty':'$$unbound' },
    'UnitMarketingDirector': { 'type':'cfacore.domain.user.User','inverseProperty':'$$unbound' }
  });
  
  $data.Entity.extend('cfacore.shared.domain.store.Distributor', {
    'DistributionCenter': { 'type':'Edm.String','nullable':false,'required':true },
    'Name': { 'type':'Edm.String','nullable':false,'required':true },
    'ShortName': { 'type':'Edm.String','nullable':false,'required':true },
    'DistributorId': { 'key':true,'type':'Edm.String','nullable':false,'required':true }
  });
  
  $data.Entity.extend('cfacore.shared.domain.store.GeographicCoordinate', {
    'Latitude': { 'type':'Edm.Double','nullable':false,'required':true },
    'Longitude': { 'type':'Edm.Double','nullable':false,'required':true }
  });
  
  $data.Entity.extend('cfacore.shared.domain.store.StoreFeatures', {
    'AcceptsCfaCard': { 'type':'Edm.Boolean','nullable':false,'required':true },
    'HasDiningRoom': { 'type':'Edm.Boolean','nullable':false,'required':true },
    'HasDriveThru': { 'type':'Edm.Boolean','nullable':false,'required':true },
    'OffersOnlineOrdering': { 'type':'Edm.Boolean','nullable':false,'required':true },
    'OffersWireless': { 'type':'Edm.Boolean','nullable':false,'required':true },
    'Playground': { 'type':'Edm.String','nullable':false,'required':true },
    'ServesBreakfast': { 'type':'Edm.Boolean','nullable':false,'required':true }
  });
  
  $data.Entity.extend('cfacore.domain.user.User', {
    'Creation': { 'type':'Edm.DateTime' },
    'LastActivity': { 'type':'Edm.DateTime' },
    'Email': { 'type':'Edm.String','nullable':false,'required':true },
    'Username': { 'type':'Edm.String' },
    'UID': { 'type':'Edm.String' },
    'DN': { 'type':'Edm.String' },
    'NameString': { 'type':'Edm.String','nullable':false,'required':true },
    'HomePhoneString': { 'type':'Edm.String' },
    'MobilePhoneString': { 'type':'Edm.String' },
    'Authority': { 'type':'Edm.String' },
    'AuthorityUID': { 'type':'Edm.String' },
    'UserId': { 'key':true,'type':'Edm.Int32','nullable':false,'required':true }
  });
  
  $data.Entity.extend('cfacore.shared.domain.user.Address', {
    'Line2': { 'type':'Edm.String' },
    'Line3': { 'type':'Edm.String' },
    'Zip': { 'type':'cfacore.shared.domain.user.Zip' },
    'ZipString': { 'type':'Edm.String' },
    'County': { 'type':'Edm.String' },
    'State': { 'type':'Edm.String' },
    'AddressId': { 'key':true,'type':'Edm.Int32','nullable':false,'required':true },
    'Coordinates': { 'type':'cfacore.shared.domain.store.GeographicCoordinate' },
    'Line1': { 'type':'Edm.String' },
    'City': { 'type':'Edm.String' },
    'Label': { 'type':'Edm.String' },
    'Name': { 'type':'cfacore.shared.domain.user.Name' },
    'NameString': { 'type':'Edm.String' }
  });
  
  $data.Entity.extend('cfacore.shared.domain.user.Name', {
    'First': { 'type':'Edm.String','nullable':false,'required':true },
    'NickName': { 'type':'Edm.String' },
    'Last': { 'type':'Edm.String','nullable':false,'required':true },
    'Middle': { 'type':'Edm.String' },
    'Surname': { 'type':'Edm.String' }
  });
  
  $data.Entity.extend('cfacore.shared.domain.user.Zip', {
    'PlusFour': { 'type':'Edm.Int32' },
    'Code': { 'type':'Edm.Int32','nullable':false,'required':true }
  });
  
  $data.Entity.extend('cfacore.shared.domain.user.Phone', {
    'Number': { 'type':'Edm.Int32','nullable':false,'required':true },
    'AreaCode': { 'type':'Edm.Int32','nullable':false,'required':true },
    'Extension': { 'type':'Edm.String' },
    'Carrier': { 'type':'Edm.String' }
  });
  
  $data.Entity.extend('cfares.domain.store.ResStore', {
    
  });
  
  $data.Entity.extend('cfares.domain.user.ResUser', {
    'OperationRole': { 'type':'Edm.Int32' },
    'Tickets': { 'type':'Array','elementType':'cfares.domain._event.Ticket','inverseProperty':'$$unbound' }
  });
  
  $data.Entity.extend('cfares.domain.user.ResAdmin', {
    
  });
  
  $data.Entity.extend('cfares.domain.user.OperatorResUser', {
    'storeNumber': { 'type':'Edm.Int32','nullable':false,'required':true },
    'employeeID': { 'type':'Edm.Int32','nullable':false,'required':true }
  });
  
  $data.Entity.extend('cfares.domain._event.Schedule', {
    
  });
  
  $data.Entity.extend('cfares.domain._event.Slot', {
    'TicketsAvailable': { 'type':'Edm.Int32' },
    'TotalTickets': { 'type':'Edm.Int32' },
    'SlotId': { 'key':true,'type':'Edm.Int32','nullable':false,'required':true },
    'IsScheduled': { 'type':'Edm.Boolean' },
    'Status': { 'type':'Edm.String' },
    'Capacity': { 'type':'Edm.Int32' },
    'Start': { 'type':'Edm.DateTime' },
    'Cutoff': { 'type':'Edm.DateTime' },
    'End': { 'type':'Edm.DateTime' },
    'OccurrenceId': { 'type':'Edm.String' },
    'ScheduleId': { 'type':'Edm.Int32' }
  });
  
  $data.Entity.extend('cfares.domain._event.Ticket', {
    'Note': { 'type':'Edm.String' },
    'SlotId': { 'type':'Edm.Int32','nullable':false,'required':true },
    'OwnerId': { 'type':'Edm.String' },
    'TicketId': { 'key':true,'type':'Edm.Int32','nullable':false,'required':true },
    'CreationSrc': { 'type':'Edm.String' },
    'CardNumber': { 'type':'Edm.Int32','nullable':false,'required':true },
    'Slot': { 'type':'cfares.domain._event.Slot','inverseProperty':'$$unbound' },
    'Owner': { 'type':'cfares.domain.user.ResUser','inverseProperty':'$$unbound' }
  });
  
  $data.Entity.extend('cfares.domain._event.Occurrence', {
    'Start': { 'type':'Edm.DateTime','nullable':false,'required':true },
    'End': { 'type':'Edm.DateTime','nullable':false,'required':true },
    'SlotRangeStart': { 'type':'Edm.DateTime','nullable':false,'required':true },
    'SlotRangeEnd': { 'type':'Edm.DateTime','nullable':false,'required':true },
    'BoundToPrototype': { 'type':'Edm.Boolean','nullable':false,'required':true },
    'GMTOffset': { 'type':'System.TimeZoneInfo' },
    'OccurrenceId': { 'key':true,'type':'Edm.Int32','nullable':false,'required':true },
    'Status': { 'type':'Edm.String','nullable':false,'required':true },
    'StatusId': { 'type':'Edm.Int32','nullable':false,'required':true },
    'StoreId': { 'type':'Edm.String' },
    'RegistrationAvailability': { 'type':'cfacore.shared.domain._base.DateRange' },
    'SlotRange': { 'type':'cfacore.shared.domain._base.DateRange' },
    'ResEvent': { 'type':'cfares.domain._event.ResEvent','inverseProperty':'$$unbound' },
    'Slots': { 'type':'Array','elementType':'cfares.domain._event.Slot','inverseProperty':'$$unbound' },
    'SlotsList': { 'type':'Array','elementType':'cfares.domain._event.Slot','inverseProperty':'$$unbound' },
    'Tickets': { 'type':'Array','elementType':'cfares.domain._event.Ticket','inverseProperty':'$$unbound' },
    'Store': { 'type':'cfares.domain.store.ResStore','inverseProperty':'$$unbound' }
  });
  
  $data.Entity.extend('cfares.domain._event.ResEvent', {
    'RegistrationStart': { 'type':'Edm.DateTime','nullable':false,'required':true },
    'RegistrationEnd': { 'type':'Edm.DateTime','nullable':false,'required':true },
    'SiteStart': { 'type':'Edm.DateTime','nullable':false,'required':true },
    'SiteEnd': { 'type':'Edm.DateTime','nullable':false,'required':true },
    'UrlName': { 'type':'Edm.String','nullable':false,'required':true },
    'Url': { 'type':'Edm.String' },
    'Description': { 'type':'Edm.String' },
    'TemplateName': { 'type':'Edm.String' },
    'Status': { 'type':'Edm.String','nullable':false,'required':true },
    'StatusId': { 'type':'Edm.Int32','nullable':false,'required':true },
    'ResEventId': { 'key':true,'type':'Edm.Int32','nullable':false,'required':true },
    'Name': { 'type':'Edm.String' },
    'OccurancesList': { 'type':'Array','elementType':'cfares.domain._event.Occurrence','inverseProperty':'$$unbound' },
    'Occurances': { 'type':'Array','elementType':'cfares.domain._event.Occurrence','inverseProperty':'$$unbound' },
    'Slots': { 'type':'Array','elementType':'cfares.domain._event.Slot','inverseProperty':'$$unbound' },
    'ReservationType': { 'type':'cfares.domain._event.ReservationType','required':true,'inverseProperty':'$$unbound' },
    'Template': { 'type':'cfares.domain._event.ResTemplate','inverseProperty':'$$unbound' },
    'ProtoOccurrence': { 'type':'cfares.domain._event.Occurrence','inverseProperty':'$$unbound' }
  });
  
  $data.Entity.extend('cfares.domain._event.ResTemplate', {
    'ResTemplateId': { 'key':true,'type':'Edm.String','nullable':false,'required':true },
    'Description': { 'type':'Edm.String','nullable':false,'required':true },
    'Name': { 'type':'Edm.String','nullable':false,'required':true },
    'BrowserMedia': { 'type':'Edm.String','nullable':false,'required':true },
    'DefaultReservationTypeId': { 'type':'Edm.String' },
    'Preview': { 'type':'cfacore.shared.domain.media.Media','inverseProperty':'$$unbound' }
  });
  
  $data.Entity.extend('cfares.domain._event.ReservationType', {
    'Description': { 'type':'Edm.String','nullable':false,'required':true },
    'StrType': { 'type':'Edm.String' },
    'Name': { 'type':'Edm.String','nullable':false,'required':true },
    'UrlName': { 'type':'Edm.String','nullable':false,'required':true },
    'ReservationTypeId': { 'key':true,'type':'Edm.String','nullable':false,'required':true }
  });
  
  $data.Entity.extend('cfares.domain._event.ExecutiveTourEvent', {
    
  });
  
  $data.Entity.extend('cfares.domain._event.slot.tours.TourSlot', {
    'TourSlotId': { 'type':'Edm.Int32','nullable':false,'required':true },
    'KidFriendly': { 'type':'Edm.Boolean','nullable':false,'required':true },
    'SpecialNeeds': { 'type':'Edm.String' }
  });
  
  $data.Entity.extend('cfares.domain._event.slot.tours.StoryTourSlot', {
    
  });
  
  $data.Entity.extend('cfares.domain._event.slot.tours.LargeStoryTourSlot', {
    
  });
  
  $data.Entity.extend('cfares.domain._event.slot.tours.TeamTourSlot', {
    
  });
  
  $data.Entity.extend('cfares.domain._event._ticket.tours.TourTicket', {
    'GuestCount': { 'type':'Edm.Int32','nullable':false,'required':true },
    'TourTicketId': { 'type':'Edm.String' },
    'TotalCostOfLunches': { 'type':'Edm.Decimal','nullable':false,'required':true },
    'OptInForLunch': { 'type':'Edm.Boolean','nullable':false,'required':true },
    'NumberOfAdultLunches': { 'type':'Edm.Int32','nullable':false,'required':true },
    'NumberOfKidLunches': { 'type':'Edm.Int32','nullable':false,'required':true },
    'NumberOfSpecialNeedLunches': { 'type':'Edm.Int32','nullable':false,'required':true },
    'SpecialDietNeedsDescription': { 'type':'Edm.String' },
    'VisitMarketing': { 'type':'Edm.Boolean','nullable':false,'required':true },
    'VisitTech': { 'type':'Edm.Boolean','nullable':false,'required':true },
    'VisitInnovation': { 'type':'Edm.Boolean','nullable':false,'required':true },
    'VisitTraining': { 'type':'Edm.Boolean','nullable':false,'required':true },
    'VisitWellness': { 'type':'Edm.Boolean','nullable':false,'required':true },
    'VisitWareHouse': { 'type':'Edm.Boolean','nullable':false,'required':true },
    'VisitIT': { 'type':'Edm.Boolean','nullable':false,'required':true },
    'VisitOther': { 'type':'Edm.Boolean','nullable':false,'required':true },
    'VisitOtherDescription': { 'type':'Edm.String' },
    'HasSpecialNeeds': { 'type':'Edm.Boolean','nullable':false,'required':true },
    'IsVisuallyImpaired': { 'type':'Edm.Boolean','nullable':false,'required':true },
    'OtherNeeds': { 'type':'Edm.Boolean','nullable':false,'required':true },
    'OtherNeedsDescription': { 'type':'Edm.String' },
    'IsHearingImpaired': { 'type':'Edm.Boolean','nullable':false,'required':true },
    'NeedsWheelChair': { 'type':'Edm.Boolean','nullable':false,'required':true },
    'IsFamilyWithKids': { 'type':'Edm.Boolean','nullable':false,'required':true },
    'IsSchoolGroup': { 'type':'Edm.Boolean','nullable':false,'required':true },
    'IsFamilyWithoutKids': { 'type':'Edm.Boolean','nullable':false,'required':true },
    'IsAdultGroup': { 'type':'Edm.Boolean','nullable':false,'required':true },
    'IsReligiousGroup': { 'type':'Edm.Boolean','nullable':false,'required':true },
    'IsSeniorGroup': { 'type':'Edm.Boolean','nullable':false,'required':true },
    'IsBusinessGroup': { 'type':'Edm.Boolean','nullable':false,'required':true },
    'IsRavingFans': { 'type':'Edm.Boolean','nullable':false,'required':true },
    'IsTeamMemberGroup': { 'type':'Edm.Boolean','nullable':false,'required':true },
    'IsOtherTypeOfGroup': { 'type':'Edm.Boolean','nullable':false,'required':true },
    'OtherTypeDescription': { 'type':'Edm.String' },
    'GroupName': { 'type':'Edm.String','nullable':false,'required':true },
    'GuestList': { 'type':'Edm.String' },
    'GuestNames': { 'type':'Array','elementType':'cfacore.shared.domain.user.Name','nullable':false,'required':true },
    'AllGuestNames': { 'type':'Array','elementType':'cfacore.shared.domain.user.Name','nullable':false,'required':true },
    'Guests': { 'type':'Array','elementType':'cfares.domain.user.ResUser','inverseProperty':'$$unbound' }
  });
  
  $data.Entity.extend('cfares.domain._event._ticket.tours.TeamTourTicket', {
    
  });
  
  $data.Entity.extend('cfacore.shared.domain.media.Media', {
    'MediaUriStr': { 'type':'Edm.String' },
    'IsSystem': { 'type':'Edm.Boolean' },
    'Width': { 'type':'Edm.Int32' },
    'Height': { 'type':'Edm.Int32' },
    'Length': { 'type':'Edm.Int32' },
    'Size': { 'type':'Edm.Int32' },
    'CreationDate': { 'type':'Edm.DateTime' },
    'FileSize': { 'type':'Edm.Int64' },
    'Name': { 'type':'Edm.String' },
    'Description': { 'type':'Edm.String' },
    'MediaId': { 'key':true,'type':'Edm.Int32','nullable':false,'required':true }
  });
  
  $data.Entity.extend('cfacore.shared.domain._base.DateRange', {
    'Start': { 'type':'Edm.DateTime','nullable':false,'required':true },
    'End': { 'type':'Edm.DateTime','nullable':false,'required':true },
    'Span': { 'type':'Edm.Time','nullable':false,'required':true }
  });
  
  $data.Entity.extend('System.Uri', {
    'Segments': { 'type':'Array','elementType':'Edm.String' }
  });
  
  $data.Entity.extend('System.TimeZoneInfo', {
    
  });
  
  $data.Entity.extend('cfares.domain._event.resevent.store.StoreEvent', {
    
  });
  
  $data.Entity.extend('cfares.domain._event.resevent.store.GiveawayEvent', {
    'AllowedProducts': { 'type':'Array','elementType':'cfares.domain._event.menu.MenuItem','inverseProperty':'$$unbound' }
  });
  
  $data.Entity.extend('cfares.domain._event.menu.MenuItem', {
    'MenuItemId': { 'key':true,'type':'Edm.Int32','nullable':false,'required':true },
    'DomId': { 'type':'Edm.String' },
    'Name': { 'type':'Edm.String' },
    'ImageUrl': { 'type':'Edm.String' },
    'ShortName': { 'type':'Edm.String' },
    'URLName': { 'type':'Edm.String' },
    'AppliedEvents': { 'type':'Array','elementType':'cfares.domain._event.resevent.store.GiveawayEvent','inverseProperty':'$$unbound' }
  });
  
  $data.Entity.extend('cfares.domain._event.resevent.tours.TourEvent', {
    
  });
  
  $data.Entity.extend('cfares.domain._event.resevent.tours.StoryTourEvent', {
    
  });
  
  $data.Entity.extend('cfares.domain._event.resevent.tours.LargeStoryTourEvent', {
    
  });
  
  $data.Entity.extend('cfares.domain._event.resevent.tours.TeamTourEvent', {
    
  });
  
  $data.EntityContext.extend('Default.Container', {
    'Store': { type: $data.EntitySet, elementType: cfacore.shared.domain.store.Store },
    'ResStore': { type: $data.EntitySet, elementType: cfares.domain.store.ResStore },
    'Address': { type: $data.EntitySet, elementType: cfacore.shared.domain.user.Address },
    'User': { type: $data.EntitySet, elementType: cfacore.domain.user.User },
    'ResUser': { type: $data.EntitySet, elementType: cfares.domain.user.ResUser },
    'Distributor': { type: $data.EntitySet, elementType: cfacore.shared.domain.store.Distributor },
    'Schedule': { type: $data.EntitySet, elementType: cfares.domain._event.Schedule },
    'Slot': { type: $data.EntitySet, elementType: cfares.domain._event.Slot },
    'Ticket': { type: $data.EntitySet, elementType: cfares.domain._event.Ticket },
    'TourSlot': { type: $data.EntitySet, elementType: cfares.domain._event.slot.tours.TourSlot },
    'TourTicket': { type: $data.EntitySet, elementType: cfares.domain._event._ticket.tours.TourTicket },
    'Occurrence': { type: $data.EntitySet, elementType: cfares.domain._event.Occurrence },
    'Event': { type: $data.EntitySet, elementType: cfares.domain._event.ResEvent },
    'Media': { type: $data.EntitySet, elementType: cfacore.shared.domain.media.Media },
    'Template': { type: $data.EntitySet, elementType: cfares.domain._event.ResTemplate },
    'ReservationType': { type: $data.EntitySet, elementType: cfares.domain._event.ReservationType }
  });

  $data.generatedContexts = $data.generatedContexts || [];
  $data.generatedContexts.push(Default.Container);
  
  /*Context Instance*/
  Default.context = new Default.Container( { name:'oData', oDataServiceHost: 'http://res.local.chick-fil-a.com/api' });

      
})(window, $data);
      
    