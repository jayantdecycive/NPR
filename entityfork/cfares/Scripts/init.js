Date.UserTimeZoneOffset= function () { 
        return (new Date().getTimezoneOffset());
};

/*==================BEGIN API ADDITIONS=================*/

    /*
    * Location - Store Methods and Searches
    */


$cfa.net.toursLeftSide = function (self) {
    if (!self)
        self = this;
    var port = "8080";
    var base = "DataService";
    var domain="tours.chick-fil-a.com"
    return self.protocal + "://" + domain + ":" + port + "/" + base;
}

    $cfa.TourData = $cfa.fn.TourData = function () {

        var self = this.root ? this : $cfa;

        var cmd = arguments[0];


        var args
        var service = false;
        if (cmd == "service") {
            service = true;
            cmd = arguments[1];
            args = [].slice.call(arguments, 2);
        } else
            args = [].slice.call(arguments, 1);

        args = $cfa.TourData[cmd].apply(self, args);


        var url = $cfa.net.toursLeftSide($cfa.net) + "/ResEvent.svc/" + args.cmd + "?format=json";
        if (service)
            return url;

        return $cfa.Call.call(self, url, args);



    };

    $cfa.fn.TourData.SlotMonthRange = $cfa.TourData.SlotMonthRange = function (eventType, success, error) {


        var args = {
            success: success,
            error: error,
            odata: true,
            format: function (d) {
                for (var i = 0; i < d.length; i++) {
                    d[i].Year = Number(d[i].SlotYearMonth.toString().substring(0, 4));
                    d[i].Month = Number(d[i].SlotYearMonth.toString().substring(4, 6));
                    
                }
                return d;
            },
            data: {
                $filter: "ResEventType eq '{0}'".format(eventType)
            },
            cmd: "SlotMonth_EventType"
        };
        return (args);
    };

    $cfa.fn.TourData.SlotMonthRangeCheckGroupSize = $cfa.TourData.SlotMonthRangeCheckGroupSize = function (eventType, size, success, error) {


        var args = {
            success: success,
            error: error,
            odata: true,
            format: function (d) {
                for (var i = 0; i < d.length; i++) {
                    d[i].Year = Number(d[i].SlotYearMonth.toString().substring(0, 4));
                    d[i].Month = Number(d[i].SlotYearMonth.toString().substring(4, 6));

                }
                return d;
            },
            data: {
                $filter: "ResEventType eq '{0}' and CapacityTotal sub TicketTotal gt {1}".format(eventType,size)
            },
            cmd: "SlotMonth_EventType_TourCapacity"
        };
        return (args);
    };

    $cfa.fn.TourData.SlotDateRange = $cfa.TourData.SlotDateRange = function (eventType, year, month, success, error) {
        function pad(num, size) {
            var s = num + "";
            while (s.length < size) s = "0" + s;
            return s;
        }
        month = pad(month, 2);

        var args = {
            success: success,
            error: error,
            odata: true,
            format: function (d) {
                for (var i = 0; i < d.length; i++) {
                    d[i].Year = Number(d[i].SlotYearMonth.toString().substring(0, 4));
                    d[i].Month = Number(d[i].SlotYearMonth.toString().substring(4, 6));
                    d[i].Date = d[i].SlotDate;
                }
                return d;
            },
            data: {
                $filter: "ResEventType eq '{0}' and SlotYearMonth eq {1}{2}".format(eventType, year, month)
            },
            cmd: "SlotDate_EventType"
        };
        return (args);
    };

    $cfa.fn.TourData.SlotDateRangeCheckGroupSize = $cfa.TourData.SlotDateRangeCheckGroupSize = function (eventType, year, month, size, success, error) {
        function pad(num, size) {
            var s = num + "";
            while (s.length < size) s = "0" + s;
            return s;
        }
        month = pad(month, 2);

        var args = {
            success: success,
            error: error,
            odata: true,
            format: function (d) {
                for (var i = 0; i < d.length; i++) {
                    d[i].Year = Number(d[i].SlotYearMonth.toString().substring(0, 4));
                    d[i].Month = Number(d[i].SlotYearMonth.toString().substring(4, 6));
                    d[i].Date = d[i].SlotDate;
                }
                return d;
            },
            data: {
                $filter: "ResEventType eq '{0}' and SlotYearMonth eq {1}{2} and CapacityTotal sub TicketTotal gt {3}".format(eventType, year, month,size)
            },
            cmd: "SlotDate_EventType_TourCapacity"
        };
        return (args);
    };

/*==================END API ADDITIONS===================*/


/*==================BEGIN DOMAIN MODEL ADDITIONS=================*/

DomainModel.TourSlot.prototype.LoadCameos = function (type, options) {

    if (!options) {
        options = type;
        type = null;
    }

    var id = this.id;
    if (!id) {
        if (options.error)
            options.error({ message: "No ID, object is not initialized" });
        return;
    }

    var CameoCollection = new DomainModel.ResUserCollection();



    if (type != null)
        CameoCollection.url = "/DataService/Slot.svc/Slot_Tour_Cameo?$expand=User&$filter=SlotId%20eq%20{0}L%20and%20Type%20eq%20{1}".format(id, type);
    else
        CameoCollection.url = "/DataService/Slot.svc/Slot_Tour_Cameo?$expand=User&$filter=SlotId%20eq%20{0}L".format(id);

    CameoCollection.parse = function (response) {
        var d = response.d;
        var data = [];
        for (var i = 0; i < d.length; i++) {
            var mdl = ModelTools.FixDate(d[i].User);
            mdl.CameoType = mdl.Type = d[i].Type;
            data.push(mdl);
        }
        return data;
    }

    var varName;
    if (type == null)
        varName = "Cameos";
    else
        varName = Enum.CameoType[type] + "Cameos";

    this.set(varName, CameoCollection);
    this.attributes[varName].fetch(options);
}

DomainModel.TourSlot.prototype.DeleteCameo = function (UserId, options) {
    var self = this;
    var id = this.id;
    if (UserId.attributes)
        UserId = UserId.get("UserId");


    var url = "/DataService/Slot.svc/Slot_Tour_Cameo(SlotId={0}L,UserId={1}L)".format(id,UserId);    

    $.ajax({
        dataType: ("json"),
        url: url,
        type: "DELETE",        
        contentType: "application/json",
        success: function (result) {
            self.LoadCameos(options);
        },
        error: function (result) {
            options.error('' + result.statusText);
        }
    });
}

DomainModel.TourSlot.prototype.AddCameo = function (UserId, type, options) {
    var self = this;
    var id = this.id;
    if (UserId.attributes)
        UserId = UserId.get("UserId");
    if (!options) {
        options = type;
        type = null;
    }
    var args = { SlotId: "{0}".format(id), UserId: "{0}".format(UserId),CreationDate:(new Date()) };
    if (type != null)
        args.Type = type;

    $.ajax({
        dataType: ("json"),
        url: "/DataService/Slot.svc/Slot_Tour_Cameo",
        type: "POST",
        data: JSON.stringify(args),
        contentType: "application/json",
        success: function (result) {
            self.LoadCameos(type, options);
        },
        error: function (result) {
            options.error('' + result.statusText);
        }
    });
}

DomainModel.TourSlot.prototype.SaveCameos = function (args, options) {
    if (!options) {
        options = args;
        args = null;
    }
    var self = this;

    for (var q = 0; q < Enum.CameoType.length; q++) {
        var cameoEnum = Enum.CameoType[q];
        if (self.attributes[cameoEnum + "Cameos"])
        self.attributes[cameoEnum+"Cameos"].each(function (model, i) {
            if (model.isNew) {
                self.AddCameo(model.get("UserId"), cameoEnum, { error: function () { console.log(arguments); } });
            }
        });
    }
}


DomainModel.SlotCollection.prototype.GetByEventTypeWithDateRange = function (args, options) {

    var type = args.EventType;
    var start = args.Start;
    var end = args.End;




    this.url = "/Service/Slot.svc/GetTourSlotByEventTypeWithDateRange";


    this.parse = function (response) {
        var d = response.GetTourSlotByEventTypeWithDateRangeResult;
        var data = [];

        for (var i = 0; i < d.length; i++) {
            var dd = ModelTools.FixDate(d[i]);            
            dd.TicketsAvailable = Math.max(Number(dd.TicketsAvailable), 0);
            data.push(dd);

        }

        return data;
    }

    var data = {};
    data.EventType = type;
    data.Start = start;
    data.End = end;
    options.data = $.param(data);
    this.fetch(options);
}

DomainModel.ResUser.EmailResetPassword = function (args, options) {
    $.ajax({
        dataType: ("json"),
        url: "/Service/User.svc/EmailResetPassword",
        type: "POST",
        data: JSON.stringify(args),
        contentType: "application/json",
        success: function (result) {
            options.success.apply(this,arguments);
        },
        error: function (result) {
            options.error.apply(this,arguments);
        }
    });
}

/*==================END DOMAIN MODEL ADDITIONS===================*/





var Page = {};
if (!window.console)
    window.console = { log: function () { } };

function isNumber(n) {
    return !isNaN(parseFloat(n)) && isFinite(n);
}

var CompleteLabel = {
    location: function (val) {
        
        return "{0} ({1})".format(val.Name, val.Line1);
    }
};

var DataFormat = {
    date: function (sourceObject, val, column) {
        var re = /-?\d+/;
        var m = re.exec(val);
        var d = new Date(parseInt(m[0]));

        return d.toString("M/d/yyyy ").toLowerCase() + "<strong class='time'>" + d.toString("h:mmtt").toLowerCase() + "</strong>";
    },
    day: function (sourceObject, val, column) {
        var re = /-?\d+/;
        var m = re.exec(val);
        var d = new Date(parseInt(m[0]));

        return d.toString("d").toLowerCase();
    },
    defaultToZero: function (sourceObject, val, column) {
        if (!val)
            return "0";
    },
    weekday: function (sourceObject, val, column) {
        var re = /-?\d+/;
        var m = re.exec(val);
        var d = new Date(parseInt(m[0]));

        return d.toString("dddd");
    },
    weekdayByInt: function (sourceObject, val, column) {

        return Date.getDayNameFromNumber(val);
    },
    year: function (sourceObject, val, column) {
        var re = /-?\d+/;
        var m = re.exec(val);
        var d = new Date(parseInt(m[0]));

        return d.toString("yyyy");
    },
    month: function (sourceObject, val, column) {
        var re = /-?\d+/;
        var m = re.exec(val);
        var d = new Date(parseInt(m[0]));

        return d.toString("MMMM");
    },
    time: function (sourceObject, val, column) {
        var re = /-?\d+/;
        var m = re.exec(val);
        var d = new Date(parseInt(m[0]));

        return d.toString("h:mmtt");
    },
    Radio: function (sourceObject, val, column) {
        if (!val)
            return "";
        var pk = $(this).closest(".select-wrapper").attr("data-pk");
        var checkString = pk == val.toString() ? "checked='checked'" : "";

        return "<input name='tableselect' type='radio' {1} value='{0}' />".format(val, checkString);
    },
    Checkbox: function (sourceObject, val, column) {
        if (!val)
            return "";
        var pk = $(this).closest(".select-wrapper").attr("data-pk");
        var checkString = pk == val.toString() ? "checked='checked'" : "";

        return "<input name='tableselect' type='checkbox' {1} value='{0}' />".format(val, checkString);
    },
    ModifyUrl: function (sourceObject, val, column) {
        if (val == null)
            return "No Ticket";

        return "<a class='modify-link' href='http://" + sourceObject.ReservationType + ".tours.chick-fil-a.com/tours/Reservation/ModifySlot/{0}'>Modify</a>".format(val);
    },
    CancelUrl: function (sourceObject, val, column) {
        if (val == null)
            return "No Ticket";

        return "<a class='cancel-link' href='/tours/Reservation/NewRegistrationCancellation/{0}'>Cancel</a>".format(val);
    },
    TicketUrl: function (sourceObject, val, column) {
        if (val == null)
            return "No Ticket";
        return "<a href='/Admin/Ticket/Details/{0}'>View</a>".format(val);
    },
    TourTicketUrl: function (sourceObject, val, column) {
        if (val == null)
            return "No Ticket";
        return "<a href='/Admin/Ticket/TourTicket-Details/{0}'>View</a>".format(val);
    },
    TourTicketDeleteUrl: function (sourceObject, val, column) {
        if (val == null)
            return "No Ticket";
        return "<a href='/Admin/Ticket/TourTicket-Delete/{0}'>Delete</a>".format(val);
    },
    SlotUrl: function (sourceObject, val, column) {
        if (val == null)
            return "No Slot";
        return "<a href='/Admin/Slot/Details/{0}'>View</a>".format(val);
    },
    ScheduleUrl: function (sourceObject, val, column) {
        if (val == null)
            return "No Schedule";
        return "<a href='/Admin/Schedule/Details/{0}'>View</a>".format(val);
    },
    ResEventUrl: function (sourceObject, val, column) {
        if (val == null)
            return "No Event";
        return "<a href='/Admin/Event/Details/{0}'>View</a>".format(val);
    },
    ResEventUrlSummary: function (sourceObject, val, column) {
        if (val == null)
            return "No Event";
        return "<a href='/Admin/Event/Summary/{0}'>View</a>".format(val);
    },
    ResEventSlotDash: function (sourceObject, val, column) {
        if (val == null)
            return "No Dash";
        return "<a href='/Admin/Event/Edit-Dash/{0}'>Slots Dash</a>".format(val);
    },
    OccurrenceUrl: function (sourceObject, val, column) {
        if (val == null)
            return "No Occurrence";
        return "<a href='/Admin/Occurrence/Details/{0}'>View</a>".format(val);
    },
    StoreUrl: function (sourceObject, val, column) {
        if (val == null)
            return "No Store";
        return "<a href='/Admin/Store/Details/{0}'>View</a>".format(val);
    },
    SlotTourUrl: function (sourceObject, val, column) {
        if (val == null)
            return "No Tour";
        return "<a href='/Admin/Slot/TourSlot-Details/{0}'>View</a>".format(val);
    },
    SlotTourDeleteUrl: function (sourceObject, val, column) {
        if (val == null)
            return "No Tour";
        return "<a href='/Admin/Slot/TourSlot-Delete/{0}'>Delete</a>".format(val);
    },
    UserUrl: function (sourceObject, val, column) {
        if (val == null)
            return "No User";
        return "<a href='/Admin/User/Details/{0}'>View</a>".format(val);
    },
    GuideNameUrl: function (sourceObject, val, column) {

        if (val == null)
            return "No Guide";
        return "<a href='/Admin/User/Details/{0}'>{1}</a>".format(val, sourceObject.GuideName);
    },
    UserNameUrl: function (sourceObject, val, column) {
        if (val == null)
            return "No User";
        return "<a href='/Admin/User/Details/{0}'>{1}</a>".format(val, sourceObject.Name);
    },
    EmailLink: function (sourceObject, val, column) {
        if (val == null)
            return "";
        return "<a href='mailto:{0}'>{0}</a>".format(val);
    }
};



var Filter = {
    

};


$(function () {
    //$.getJSON(requestUri, function (d) {
    //  console.log(d);
    //});
    $('.data-table').each(function () {
        DataTableRes.parseAndRender(this);
    });


});

$(function () {
    Page.init();
});
Page.init = function (context) {
    if (!context)
        context = document;

    $(".select-wrapper input.select-all", context).click(function () {

        var isThisChecked = $(this).is(":checked");
        if (isThisChecked) {
            $(this).closest(".select-wrapper").find("input:checkbox").attr("checked", "checked");
        } else {
            $(this).closest(".select-wrapper").find("input:checkbox").removeAttr("checked");
        }
    });


    $(".guide-collection .jq-auto-complete", context).bind("autocomplete.pk", function (e, selected) {

        var pk = selected.pk;

        var vals = $(this).closest(".guide-collection").find(".to-accept").val();
        var valArray = [];
        if (vals)
            valArray = vals.split(',');

        var self = this;
        setTimeout(function () {
            $(self).val("");
        }, 10);

        if ($.inArray(pk, valArray) != -1)
            return;


        $(this).closest(".guide-collection").children(".models").append("<div class='block guide' data-pk='{1}'>{0} <a href='javascript:void(0);' class='legacy remove-guide'>remove</a></div>".format(selected.Name, pk));





        valArray.push(pk);
        $(this).closest(".guide-collection").find(".to-accept").val(valArray.join(","));

        var removeVals = $(this).closest(".guide-collection").find(".to-remove").val();
        var removeValArray = [];
        if (removeVals)
            removeValArray = removeVals.split(',');
        removeValArray = removeValArray.filter(function (element, index, array) {
            return element.toString() != pk;
        });
        $(this).closest(".guide-collection").find(".to-remove").val(removeValArray.join(","));

    });
    $(".guide-collection .remove-guide", context).live("click", function (e, selected) {
        var pk = $(this).parent().attr("data-pk");
        var hdn = $(this).closest(".guide-collection").find(".to-remove");



        var vals = hdn.val();
        var valArray = [];
        if (vals)
            valArray = vals.split(',');
        valArray.push(pk);
        hdn.val(valArray.join(","));



        var addVals = $(this).closest(".guide-collection").find(".to-accept").val();
        var addValArray = [];
        if (addVals)
            addValArray = addVals.split(',');

        addValArray = addValArray.filter(function (element, index, array) {
            return element.toString() != pk;
        });

        $(this).closest(".guide-collection").find(".to-accept").val(addValArray.join(","));

        $(this).parent().remove();

    });


    $("#filter-guide .jq-auto-complete", context).bind("autocomplete.pk", function (e, selected) {
        $("table.filterable").dataTable().fnDraw(true);
    });
    $(".filter input:checkbox", context).parent().button();

    $(".filter", context).bind("change", function (e, selected) {

        $("table.filterable").dataTable().fnDraw(true);
    });

    $(".filter.picker input", context).bind("pk", function (e, selected) {

        $("table.filterable").dataTable().fnDraw(true);
    });
    $("#filter-guide .jq-auto-complete", context).bind("keydown", function (e) {
        if (e.which == 13 && !$(this).val()) {
            var hdn = $(this).data("pk");
            $(hdn).val(0);
            $(this).trigger("autocomplete.pk", null);
        }
    });



    $(".auto-load", context).each(function () {
        var pk = $(this).attr("data-pk");
        if (!pk)
            return;
        var clientModel = $(this).attr("data-client-model");
        var label = $(this).attr("data-client-label");

        //backbone
        if (clientModel) {
            clientModel = eval(clientModel);
            var isInput = $(this).is(":input");
            var loadThis = new clientModel();
            loadThis.id = pk;
            var self = $(this);
            $(this).fadeTo("FAST", .5);
            loadThis.fetch({ success: function (model, object) {

                self.fadeTo("FAST", 1);
                if (isInput)
                    self.val(model.get(label));
                else
                    self.text(model.get(label));
            },
                error: function () {
                    self.fadeTo("FAST", 1);
                    if (isInput)
                        self.val("There was an error");
                    else
                        self.text("There was an error");
                }
            });
        } else if (clientModel = $(this).attr("data-source")) {
            //odata
            var self = $(this);
            $(this).fadeTo("FAST", .5);
            var url = clientModel.format(pk);
            $.getJSON(url, function (data, status, xhr) {
                self.fadeTo("FAST", 1);
                if (isInput)
                    self.val(data.d[label]);
                else
                    self.text(data.d[label]);
            });
        }
    });

    $(".auto-format-mvc .display-label", context).addClass("inline-block").each(function () {
        var fieldName = $(this).attr("data-field");
        var jq = $("<div class='model-field field-{0}' />".format(fieldName))
        $(this).before(jq);
        jq.prepend($(this).next(".display-field").addClass("inline-block")).prepend(this);
    });

    $(".auto-format-mvc .editor-label", context).addClass("inline-block").each(function () {
        var fieldName = $(this).attr("data-field");
        var jq = $("<div class='model-field field-{0}' />".format(fieldName))
        $(this).before(jq);
        jq.prepend($(this).next(".editor-field").addClass("inline-block")).prepend(this);
    });
    //var params = { LocationNumber: 1 };
    //$.getJSON("/Service/Location.svc/LocationById", JSON.stringify(params), function (d) { console.log(d); });


    $("select.load-on-click", context).each(function () {
        var self = $(this);
        if ($().selectmenu) {

            $(this).selectmenu({ style: 'dropdown' }).next().data("select", self).bind("click", function () {
                var self = $(this).data("select");
                var src = $(self).attr("data-source");
                var val = $(self).attr("data-value");
                var label = $(self).attr("data-label");
                var newSelf = $(this);

                $.getJSON(src, function (data, status, xhr) {
                    var d;
                    if (data.length && !data.d)
                        d = data;
                    else
                        d = data.d;

                    for (var i = 0; i < d.length; i++) {
                        var labelArr = label.split(",");
                        var theLabel = "";
                        for (var j = 0; j < labelArr.length; j++) {
                            theLabel = labelArr[j];
                            if (d[i][theLabel])
                                break;
                        }
                        self.append(
                            $('<option />').val(d[i][val]).html(d[i][theLabel])
                            );
                    }

                    $(self).selectmenu('destroy').selectmenu({ style: 'dropdown' }).selectmenu('open');
                });

            });
        }
    });


    if ($().selectmenu)
        $('.jq-select', context).selectmenu({ style: 'dropdown' });

    $(".validation-summary-errors,.jq-error", context).addClass("ui-widget").wrapInner("<div class='ui-state-error ui-corner-all'><p /></div>").wrap("<div class='admin-table' />").find("p").prepend("<span class='ui-icon ui-icon-alert' style='float: left; margin-right: .3em;'></span>");
    $(".jq-info", context).addClass("ui-widget").wrapInner("<div class='ui-state-highlight ui-corner-all'><p /></div>").wrap("<div class='admin-table' />").find("p").prepend("<span class='ui-icon ui-icon-info' style='float: left; margin-right: .3em;'></span>");


    $("#auth_dd", context).bind("select", function () {
        var href = $(this).val();
        window.location = href;
    });

    /*$("#auth_dd", context).bind("change", function () {
    // alert($(this).val());
    });*/

    $(".jq-date", context).each(function () {


        $(this).datepicker();
    });

    $(".jq-time", context).each(function () {
        var args = { stepMinute: 10, ampm: true };

        args.stepMinute = $(this).attr("data-step-minute") || args.stepMinute;

        $(this).timepicker(args);
    });

    $(".jq-button", context).each(function () {
        var icon = $(this).attr("data-icon");
        if (icon)
            $(this).button({ icons: { secondary: icon} });
        else if (icon = $(this).attr("data-left-icon")) {

            $(this).button({ icons: { primary: icon} });
        } else
            $(this).button();
    });

    $(".jq-accordian", context).accordion({
        autoHeight: false,
        navigation: true
    });

    $(".jq-dialog", context).dialog({
        modal: true,
        width: 500,
        height: 500,
        autoOpen: false

    }).parent().wrap('<div class="admin-table"></div>');

    $(".jq-button-icon", context).each(function () {

        $(this).button({ text: false, icons: { primary: $(this).attr("data-icon")} });
    });

    $(".dash-cta a", context).each(function () {
        $(this).parent().toggleClass("admin-table", true);
        $(this).button({ icons: { primary: "ui-icon-circle-arrow-w"} });
    });
    $(".edit-cta a", context).each(function () {
        $(this).parent().toggleClass("admin-table", true);
        $(this).button({ icons: { secondary: "ui-icon-pencil"} });
    });
    $(".button-cta a", context).each(function () {
        $(this).parent().toggleClass("admin-table", true);
        var icon = $(this).attr("data-icon") || $(this).parent().attr("data-icon");
        if (icon)
            $(this).button({ icons: { secondary: icon} });
        else if (icon = $(this).attr("data-left-icon") || $(this).parent().attr("data-left-icon")) {

            $(this).button({ icons: { primary: icon} });
        } else
            $(this).button();
    });
    $(".jq-date-time", context).each(function () {
        $(this).datetimepicker().addClass("text-box");
    });

    $(".jq-date-range", context).each(function () {
        var name = $(this).attr("name");
        var val = $(this).val();
        var classes = $(this).attr("class");
        var hiddenElement = "<input type='hidden' value='{1}' class='date-range-answer' name='{0}' />".format(name, val);
        var hiddenSibling = $(this).after(hiddenElement).siblings(".date-range-answer").addClass(classes).removeClass("jq-date-range");

        var targetValArr = val.split(',');
        while (targetValArr.length < 2)
            targetValArr.push("");

        var endInput = " <span class='in-daterange-to'>to</span> <input type='text' class='date-range-end inline-block' name='{0}_End' />".format(name);

        var themeify = function (input, inst) {
            if (!$(inst.dpDiv).parent(".admin-grey").length)
                $(inst.dpDiv).wrap("<div />").parent().toggleClass("admin-grey", true);
        }

        var dateEnd = $(this).after(endInput).siblings(".date-range-end").val(targetValArr[1]).data("target", hiddenSibling).datetimepicker({ stepMinute: 10, ampm: true, beforeShow: themeify, onSelect: function (dateText, inst) {

            var target = $(this).data("target");
            var targetValArr = $(target).val().split(',');

            if (targetValArr.length > 1)
                targetValArr[1] = dateText;

            $(target).val(targetValArr.join());

        }
        }).addClass(classes).removeClass("jq-date-range");

        var dateStart = $(this).attr("name", "{0}_Start".format(name)).val(targetValArr[0]).data("target", hiddenSibling).toggleClass("date-range-start", true).datetimepicker({ stepMinute: 10, ampm: true, beforeShow: themeify, onSelect: function (dateText, inst) {
            var target = $(this).data("target");
            var targetValArr = $(target).val().split(',');
            if (targetValArr.length > 0)
                targetValArr[0] = dateText;
            console.log(target);
            $(target).val(targetValArr.join());
        }
        }).addClass(classes).removeClass("jq-date-range");

    });

    $(".jq-auto-complete", context).each(function () {
        var src = $(this).attr("data-src");
        var label = $(this).attr("data-label");


        if (!label)
            label = eval($(this).attr("data-onlabel"));
        else
            label = label.split(",");

        var select = $(this).attr("data-onselect");
        var filter = $(this).attr("data-filter");
        var numericFilter = $(this).attr("data-numeric-filter");
        var columns = $(this).attr("data-columns");

        var pk = $(this).attr("data-pk");

        var self = this;

        //if the input is a front for an id picker
        if (pk) {

            var name = $(self).attr("name");
            var hdn = $("<input type='hidden' />").insertAfter(self).val($(self).val()).attr("name", name);

            $(self).attr("name", name + "-label");

            $(self).data("pk", hdn);

            var initLabel = $(self).attr("data-default-label");
            if (initLabel) {
                $(self).val(initLabel);
            }
        }

        if (select)
            select = eval(select);
        $(self).autocomplete({
            source: function (req, add) {

                /*
                Build Odata Filter from Query
                Process and identify Return Value
                */
                var args = [];
                var displayLength = 10;

                //var valueColumn = "LocationNumber";
                var labelColumns;
                if (!$.isFunction(label))
                    labelColumns = label;

                var selectArr = columns.split(",");
                for (var i = 0; i < labelColumns.length; i++)
                    if (labelColumns[i] && selectArr.indexOf(labelColumns[i]) == -1)
                        selectArr.push(labelColumns[i]);

                args.push({ name: "$top", "value": displayLength });
                args.push({ name: "$select", "value": selectArr.join(",") });

                var filterOutArr = [];
                //TODO - test this number search
                if (filter) {
                    filterArr = filter.split(",");
                    for (var i = 0; i < filterArr.length; i++) {

                        var filterVar = filterArr[i];
                        var typeExp = /[DLFI]{1}$/;
                        var typeMatch = filterVar.match(typeExp);
                        if (typeMatch) {

                            if (!isNumber(req.term)) {
                                continue;
                            }

                            filterVar = filterVar.replace(typeExp, "");

                            var type = typeMatch[0] == "I" ? "" : typeMatch[0];

                            filterVar = "" + filterVar + " eq (" + req.term + "" + type + ")";
                        } else {
                            filterVar = "indexof(" + filterVar + ",'" + req.term + "') gt -1";
                        }
                        filterOutArr.push(filterVar);
                    }
                }






                args.push({ name: "$filter", "value": filterOutArr.join(" or ") });



                $.ajax({
                    url: src,
                    data: args,
                    method: "GET",
                    dataType: 'json',
                    success: function (json) {
                        //create array for response objects  
                        var suggestions = [];

                        //process response  
                        $.each(json.d, function (i, val) {
                            if (labelColumns && labelColumns.length) {
                                var theLabel = "";
                                for (var j = 0; j < labelColumns.length; j++) {
                                    if (theLabel = val[labelColumns[j]]) {
                                        break;
                                    }
                                }
                                suggestions.push({ label: theLabel, value: theLabel, data: val });
                            } else {
                                var toLabel = label(val, req.term);
                                suggestions.push({ label: toLabel, value: toLabel, data: val });
                            }

                        });

                        //pass array to callback  
                        add(suggestions);
                    }
                });


            },
            minLength: 2,
            select: function (event, ui) {
                //console.log(ui.item);
                //console.log(this);
                if (pk) {
                    ui.item.data.pk = ui.item.data[pk];
                    var hdn = $(this).data("pk");
                    $(hdn).val(ui.item.data.pk);
                    $(this).trigger("autocomplete.pk", ui.item.data);
                }
                if (select)
                    select.call(this, event, ui.item.data);
            }
        }).bind("click", function () { $(this).select(); });


    });

    $(".clean-timezone", context).each(function () {
        $(this).trigger("timezone.preconvert");
        var ival = $(this).text() || $(this).val();

        var del = "-";
        if (ival.indexOf(',') >= 0)
            del = ",";

        var vals = ival.split(del);
        var newValue = "";
        var newValues = [];
        
        for (var i = 0; i < vals.length; i++) {
            var val = vals[i];
            var date = Date.parse(val);
            var defaultFormat = "MM/dd/yyyy h:mmtt";
            var baseDate = date;
            if (date == "Invalid Date") {
                date = Date.parse((new Date()).toString("MM/dd/yyyy") + " " + val);
                defaultFormat = "h:mmtt";



            }
            if ($(this).attr("data-base-date")) {
                baseDate = Date.parse($(this).attr("data-base-date"));
            } else if ($(this).attr("data-date-base")) {
                baseDate = Date.parse($(this).attr("data-date-base"));
            } else {
                baseDate = null;
            }
            if (!date || date == "Invalid Date")
                return;


            if (window.Global && window.Global.TimeZoneContext) {
                //figure out if falls within DST
                if (!baseDate && !window.Global.TimeZoneInfo.SupportsDaylightSavingTime) {
                    date = date.addHours(window.Global.TimeZoneContextOffset);
                } else {
                    if (baseDate && baseDate.isDST())
                        date = date.addHours(window.Global.TimeZoneContextOffset + 1);
                    else
                        date = date.addHours(window.Global.TimeZoneContextOffset);
                }
            }

            var newFormat = "";
            if (newFormat = $(this).attr('data-time-format'))
                defaultFormat = newFormat;
            var newDate = date.toString(defaultFormat).replace("AM", "am").replace("PM", "pm");
            newValues.push(newDate);
        }
        if (del == "-")
            del = " - ";
        //console.log(newValues);
        newValue = newValues.join(del);
        $(this).text(newValue) && $(this).val(newValue);
        $(this).trigger("timezone.convert");
        $(this).removeClass("clean-timezone");
    });

    $("#ui-datepicker-div", context).wrap('<div class="admin-table" />');
}


