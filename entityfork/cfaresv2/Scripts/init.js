Date.UserTimeZoneOffset= function () { 
        return (new Date().getTimezoneOffset());
};

var Supported = { date: false, number: false, time: false, month: false, week: false },
    tester = document.createElement('input');

for (var i in Supported) {
    if(navigator.appVersion.indexOf("MSIE")==-1){
        tester.type = String(i);
        Supported[i] = tester.type === i;
    }
    else{
        Supported[i] = false
    }
}

/*==================BEGIN API ADDITIONS=================*/

    /*
    * Location - Store Methods and Searches
    */

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
    
    //NOTE: using Date.fromISO(val) instead of new Date(val) to format correctly in ie8-

	date: function (sourceObject, val, column) {
	    //var d = new Date(val);
	    var d = Date.fromISO(val);
		return d.toString("M/d/yyyy ").toLowerCase() + "<strong class='time'>" +
			d.toString("h:mmtt").toLowerCase().replace(/^0:/, '12:') + "</strong>";
	},
	dateWithoutDaylightDavings: function (sourceObject, val, column) {
	    var d = Date.fromISO(val);
	    d.setHours(val.slice(11, 13))
	    return d.toString("M/d/yyyy ").toLowerCase() + "<strong class='time'>" +
			d.toString("h:mmtt").toLowerCase().replace(/^0:/, '12:') + "</strong>";
	},
	dateOnly: function (sourceObject, val, column) {
	    var d = Date.fromISO(val);
	    var _userOffset = d.getTimezoneOffset() * 60 * 1000; // user's offset time
	    var _gmtOffset = 0 * 60 * 60 * 1000;
	    d = new Date(d.getTime() + _userOffset + _gmtOffset); // redefine variable
	    
	    return d.toString("M/d/yyyy").toLowerCase();
	},
	day: function (sourceObject, val, column) {
	    return Date.fromISO(val).toString("d").toLowerCase();
		// SH - Commenting out below ( ineffective w/ values such as '2013-05-27T15:00:00-04:00' ), see new above
		//var re = /-?\d+/;
        //var m = re.exec(val);
        //var d = new Date(parseInt(m[0]));

        //return d.toString("d").toLowerCase();
    },
    defaultToZero: function (sourceObject, val, column) {
        if (!val)
        	return "0";
	    return val;
    },
    weekday: function (sourceObject, val, column) {
        return Date.fromISO(val).toString("dddd");
    	// SH - Commenting out below ( ineffective w/ values such as '2013-05-27T15:00:00-04:00' ), see new above
    	//var re = /-?\d+/;
        //var m = re.exec(val);
        //var d = new Date(parseInt(m[0]));
        //return d.toString("dddd");
    },
    weekdayByInt: function (sourceObject, val, column) {

        return Date.getDayNameFromNumber(val);
    },
    year: function (sourceObject, val, column) {
        return Date.fromISO(val).toString("yyyy");
    	// SH - Commenting out below ( ineffective w/ values such as '2013-05-27T15:00:00-04:00' ), see new above
    	//var re = /-?\d+/;
        //var m = re.exec(val);
        //var d = new Date(parseInt(m[0]));

        //return d.toString("yyyy");
    },
    month: function (sourceObject, val, column) {
        return Date.fromISO(val).toString("MMMM");
    	// SH - Commenting out below ( ineffective w/ values such as '2013-05-27T15:00:00-04:00' ), see new above
    	//var re = /-?\d+/;
        //var m = re.exec(val);
        //var d = new Date(parseInt(m[0]));

        //return d.toString("MMMM");
    },
    time: function (sourceObject, val, column) {
        var d = Date.fromISO(val).toString("h:mm tt");
	    if( d.indexOf( '0:' ) == 0 ) d = d.replace('0:', '12:');
	    return d;
    	// SH - Commenting out below ( ineffective w/ values such as '2013-05-27T15:00:00-04:00' ), see new above
    	//var re = /-?\d+/;
        //var m = re.exec(val);
        //var d = new Date(parseInt(m[0]));

        //return d.toString("h:mmtt");
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
    NPRTicketUrl: function (sourceObject, val, column) {
    	if (val == null)
    		return "No Ticket";
    	return "<a href='/Admin/NPRTicket/Details/{0}'>View</a>".format(val);
    },
    DateTicketUrl: function (sourceObject, val, column) {
        if (val == null)
            return "No Ticket";
        return "<a href='/Admin/DateTicket/Details/{0}'>View</a>".format(val);
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
    NPRSlotUrl: function (sourceObject, val, column) {
    	if (val == null)
    		return "No Slot";
    	return "<a href='/Admin/NPRSlot/Details/{0}'>View</a>".format(val);
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
        return "<a href='/Admin/Event/Details/{0}'>Details</a>".format(val);
    },
    ResEventUrlSummary: function (sourceObject, val, column) {
        if (val == null)
            return "No Event";
        return "<a href='/Admin/Event/Summary/{0}'>View</a>".format(val);
    },
    ResEventUrlStart: function (sourceObject, val, column) {
        if (val == null)
            return "No Event";
        return "<a href='/Admin/Event/Start/{0}'>Details</a>".format(val);
    },
    ResEventSlotDash: function (sourceObject, val, column) {
        if (val == null)
            return "No Dash";
        return "<a href='/Admin/Event/Edit-Dash/{0}'>Slots Dash</a>".format(val);
    },
    OccurrenceCapacityUrl: function (sourceObject, val, column) {
        if (val == null)
            return "No Occurrence";
        return "<a href='/Admin/Occurrence/Capacity/{0}'>Change Capacity</a>".format(val);
    },
    OccurrenceUrl: function (sourceObject, val, column) {
        if (val == null)
            return "No Occurrence";
        return "<a href='/Admin/Occurrence/Details/{0}'>View</a>".format(val);
    },
    StoreUrl: function (sourceObject, val, column) {
        if (val == null)
            return "No Store";
        return "<a href='/Admin/Store/Details/{0}' target='_blank'>View</a>".format(val);
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



//Date parsing helper for ie8-
(function () {
    var D = new Date('2011-06-02T09:34:29+02:00');
    if (!D || +D !== 1307000069000) {
        Date.fromISO = function (s) {
            var day, tz,
            rx = /^(\d{4}\-\d\d\-\d\d([tT ][\d:\.]*)?)([zZ]|([+\-])(\d\d):(\d\d))?$/,
            p = rx.exec(s) || [];
            if (p[1]) {
                day = p[1].split(/\D/);
                for (var i = 0, L = day.length; i < L; i++) {
                    day[i] = parseInt(day[i], 10) || 0;
                };
                day[1] -= 1;
                day = new Date(Date.UTC.apply(Date, day));
                if (!day.getDate()) return NaN;
                if (p[5]) {
                    tz = (parseInt(p[5], 10) * 60);
                    if (p[6]) tz += parseInt(p[6], 10);
                    if (p[4] == '+') tz *= -1;
                    if (tz) day.setUTCMinutes(day.getUTCMinutes() + tz);
                }
                return day;
            }
            return NaN;
        }
    }
    else {
        Date.fromISO = function (s) {
            return new Date(s);
        }
    }
})()



var Filter = {
    

};


$(function () {
    //$.getJSON(requestUri, function (d) {
    //  console.log(d);
    //});
    


});

$(function () {
    Page.init();
});
Page.init = function (context) {
    if (!context)
        context = document;
    
    $(".auto-model", context).each(function () {
        var pk = $(this).attr("data-pk");
        var typ = $(this).attr("data-model");
        $(this).prepend("<div class='legend inline-block'>{0}</div>".format(typ));
        $(".editor-label", this).each(function () {
            var fieldName = $("label", this).attr("for");
            $(this).next(".editor-field").andSelf().addClass("inline-block");
            $(this).next(".editor-field").andSelf().wrapAll("<div class='model-field field-{0}'>".format(fieldName));
            
            
        });
        $(this).removeClass("auto-model");
    });
    
    $(".auto-display-model", context).each(function () {
        var pk = $(this).attr("data-pk");
        var typ = $(this).attr("data-model");
        $(this).prepend("<div class='legend inline-block'>{0}</div>".format(typ));
        $(".display-label", this).each(function () {
            $(this).next(".display-field").andSelf().addClass("inline-block");
            $(this).next(".display-field").andSelf().wrapAll("<div class='model-field'>");


        });
        $(this).removeClass("auto-display-model");
    });

    $('.data-table', context).each(function () {
        if ($(this).data("rendered"))
            return;
        DataTableRes.parseAndRender(this);
        $(this).data("rendered", true);
        
        //$(this).removeClass("data-table");
    });

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
            $(this).trigger("pk", null);
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
            console.log(clientModel);
            var isInput = $(this).is(":input");
            var loadThis = new clientModel();
            loadThis.id = pk;
            var self = $(this);
            $(this).fadeTo("FAST", .5);
            loadThis.fetch({
                success: function (model, object) {

                    self.fadeTo("FAST", 1);
                    var val = model.get(label);
                    if (!val && $.isFunction(model[label])) {
                        val = model[label]();
                    }
                    if (isInput)
                        self.val(val);
                    else
                        self.text(val);
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
                    var d = data.d||data.value;
                   

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
    	$('.jq-select', context).selectmenu({ style: 'dropdown' }, "widget").addClass("");

    var errorSummary = $(".validation-summary-errors,.jq-error", context);
		errorSummary.addClass("ui-widget")
			.wrapInner("<div class='ui-state-error ui-corner-all'><p /></div>")
			.wrap("<div class='admin-table' />")
			.find("p")
			.prepend("<span class='ui-icon ui-icon-alert' style='float: left; margin-right: 1.3em; position: relative; top: 5px; left: 8px;'></span>");

	if (errorSummary.find("li").length === 1 && errorSummary.find("li").text() === "")
		errorSummary.hide();

    $(".jq-info", context).addClass("ui-widget").wrapInner("<div class='ui-state-highlight ui-corner-all'><p /></div>").wrap("<div class='admin-table' />").find("p").prepend("<span class='ui-icon ui-icon-info' style='float: left; margin-right: .3em;'></span>");


    $("#auth_dd", context).selectmenu({
        "change": function() {
            var href = $(this).val();
            window.location = href;
        }
    });

    /*$("#auth_dd", context).bind("change", function () {
    // alert($(this).val());
    });*/
    
    $(".jq-crop", context).each(function () {
        var self = this;
        var hdn = $("<input type='hidden' />").insertAfter(self).attr("name", $(self).attr("data-crop-name"));
        var croppingGuide = ["x", "y", "x2", "y2"];
        var trueSize = null;
        if ($(self).attr("data-original-width"))
            trueSize = [Number($(self).attr("data-original-width")), Number($(self).attr("data-original-height"))];
        var cropping = [];
        for (var i = 0; i < croppingGuide.length; i++) {
            var attr = $(self).attr("data-crop-rect-" + croppingGuide[i]);
            if (!attr) {
                cropping = null;
                break;
            }
            var val = Number(attr);
            cropping.push(val);
        }
        
        var act = function(coords) {
            $(hdn).val(JSON.stringify({ X: Math.round(coords.x), Y: Math.round(coords.y), Width: Math.round(coords.w), Height: Math.round(coords.h) }));
        };
        $(self).Jcrop({
            onSelect: act,
            onChange:act,
            setSelect: cropping,
            trueSize:trueSize

        });
        
        
    });

    $(".jq-date", context).each(function () {
       // if (Supported["date"])
       //     return;
        $(this).datepicker();
    });

    $(".jq-date-time", context).each(function () {
      //  if (Supported["datetime"])
       //     return;
		var self = this;
        var dateFmt = "MM/dd/yyyy";
        var timeFmt = "h:mm tt";
        var val = $(this).val();
        if (val) {
            var d = new Date(val);
            var valStr = d.toString("{0} {1} {2}".format(dateFmt, timeFmt, d.getUTCOffset().replace(/00^/gi, ":00")));
            $(this).val(valStr.toLowerCase().replace(' 0:', ' 12:'));
            
        }
        timeFmt = timeFmt + " Z";
        $(this).datetimepicker({
    		stepMinute: 15,
    		ampm: true,
    		parse: function (timeFormat, timeString, options) {
    		    var val = $(self).val();
    		    
    		    var d = new Date(val);
    		    
    		    var tz = (d.getUTCOffset());
    		    var minIndex = tz.length - 2;
    		    var mTz = { hours: Number(tz.substring(0, minIndex)), minutes: Number(tz.substring(minIndex)) };
    		    var minutes = mTz.hours * 60;
    		    //tz = "-240";
    		    return {
    		        hour: d.getHours(),
    		        minute: d.getMinutes(),
    		        second: 0,
    		        millisec: 0,
    		        microsec: 0,
    		        timezone: minutes
    		    };
    		},
    		format: dateFmt,
    		timeFormat :timeFmt,
    		controlType: 'select'
    	}); //.addClass("text-box");
    });

    $(".jq-time", context).each(function () {
       // if (Supported["time"])
      //      return;
        //var args = { stepMinute: 15, ampm: true };

        //args.stepMinute = $(this).attr("data-step-minute") || args.stepMinute;

    	//$(this).timepicker(args);
        var timeFmt = "h:mm tt";
    	$(this).timepicker({
    		stepMinute: 15,
    		ampm: true,
    		timeFormat: timeFmt,
			controlType: 'select'
    	});
    });

    $(".jq-button", context).each(function () {
        var icon = $(this).attr("data-icon");
        if (icon)
            $(this).button({ icons: { secondary: icon } });
        else if (icon = $(this).attr("data-left-icon")) {

            $(this).button({ icons: { primary: icon } });
        } else
            $(this).button();
    });

    $(".jq-accordian", context).accordion({
    	//autoHeight: false, //deprecated in latest jquery ui update
    	heightStyle: "content"
    	//collapsible: "true"
    	//navigation: true //deprecated in latest jquery ui update
    });

    $(".jq-dialog", context).dialog({
        modal: true,
        width: 500,
        height: 500,
        autoOpen: false

    }).parent().wrap('<div class="admin-table"></div>');

    $("*[data-toggle='modal']", context).each(function () {
        var anch = this;
        
        $(this).data("href",$(this).attr("href"));
        $(this).attr("href", "javascript:void(0);");
        
        var dia = $('<div class="auto-dialog"></div>');
        dia.dialog({
            modal: true,
            title: $(this).attr("data-title") || "Pop-up",
            show: 'clip',
            width: 700,
            hide: 'clip',
            autoOpen: false,
            buttons: [
                {
                	text: $(this).attr("data-submit-title") || "Submit",
                    click: function () {
                        dia.trigger("dia.submit");
                    }
                },
                { text: "Cancel", click: function () { $(this).dialog("close"); } }
            ]
        }).parents('.ui-dialog:eq(0)').addClass('admin-table');

        $(this).bind("dialog-success", function (e, data) {
        	console.log(data);
            if (data.success.message) {
                alert(data.success.message);
            }
            if ($(this).attr("data-success") && data.success.id) {
            	var key = $(this).attr("data-success");
            	$("#" + key).trigger("pk", [data.success.id, data.success.label || null]);
            	// For scenarios of LocationNumber_LocationNumber vs. just LocationNumber
            	if (key.indexOf('_') > 0) key = key.substring(0, key.indexOf('_'));
            	$("#" + key).trigger("pk", [data.success.id, data.success.label || null]);
            }
        });

        dia.bind("dia.submit", function () {
            var form = ($("form", this));
            var $this = this;
            if ($($this).data("loading")) {
                return;
            }
            $($this).fadeTo("FAST", .5).data("loading", true);
            
            var success = function (data, textStatus, xhr) {
                $($this).fadeTo("FAST", 1).data("loading", false);
                if (data.success && data.success.continued && data.success.url) {
                    var title = null;
                    if (data.success.message) {
                        title = data.success.message;
                        //alert(data.success.message);
                    }
                    data.success.url = data.success.url + (data.success.url.indexOf("?") >= 0 ? "&" : "?") + "snip=true";
                    handleClick(dia, data.success.url,title);
                } else {

                    $(anch).trigger("dialog-success", data);
                    dia.dialog("close");
                }
            };

            var error = function (xhr, data, msg) {
                $($this).fadeTo("FAST", 1).data("loading", false);
                $("form .validation", dia).remove();
                $("form .legend", dia).after("<ul class='validation'></ul>");
                $("[data-valmsg-for]", dia).html("").hide().removeClass("field-validation-error");
                var errors = JSON.parse(xhr.responseText);
                for (var err in errors) {
                    if (err) {
                        $("[data-valmsg-for='{0}']".format(err), dia).html(errors[err]).show().addClass("field-validation-error");
                    } else {
                        $("form .validation", dia).prepend("<li>" + errors[err] + "</li>").show();
                    }
                }

                console.log(errors);
            };
            
            if (form.attr("enctype") == "multipart/form-data") {
                form.fileUpload({
                    form: form,
                    actionURL: form.attr("action"),
                    success: success,
                    error: error
                });
            } else {
                $.post(form.attr("action"), form.serialize(), success, "json").fail(error);
            }
        });

        var handleClick = function(dia, href,title) {
            var remote = href.search(/^#/) == -1;

            if (remote) {

                $(dia).load(href, function() {
                    $("h1,h2", dia).hide();
                    $("input[type='submit']", dia).hide();
                    
                    dia.dialog("open");
                    Page.init(dia);
                });
            }

            if (title) {
                dia.dialog( "option", "title", title );
            }

        };

        if ($(this).attr("data-modal") == "lazy") {
            $(this).bind("modal",function (e,href) {
                
                console.log(href);
                handleClick(dia, href);

                return false;
            });
        } else {

            $(this).click(function(e) {

                var href = $(this).data("href");
                console.log(href);
                handleClick(dia, href);

                return false;
            });
        }
    });

    $(".jq-button-icon,[data-icon]", context).each(function () {
        if ($(this).hasClass("jq-button") || $(this).hasClass("button-cta") || $(this).text().replace(/\s/gi, "")) {
            return;
        }
        var icon = $(this).attr("data-icon");
        
        if (icon && icon.indexOf("ui-icon-") != 0) {
            icon = "ui-icon-" + icon;
        }
        $(this).button({ text: false, icons: { primary: icon } });
    });
    
    $(".dash-cta a", context).each(function () {
        $(this).parent().toggleClass("admin-table", true);
        $(this).button({ icons: { primary: "ui-icon-circle-arrow-w" } });
    });
    $(".edit-cta a", context).each(function () {
        $(this).parent().toggleClass("admin-table", true);
        $(this).button({ icons: { secondary: "ui-icon-pencil" } });
    });
    $(".button-cta a", context).each(function () {
        //$(this).parent().toggleClass("admin-table-2", true); /* rclark: commented out this line as too disruptive */
        var icon = $(this).attr("data-icon") || $(this).parent().attr("data-icon");
        if (icon)
            $(this).button({ icons: { secondary: icon } });
        else if (icon = $(this).attr("data-left-icon") || $(this).parent().attr("data-left-icon")) {

            $(this).button({ icons: { primary: icon } });
        } else
            $(this).button();
    });
    
    

    $(".button-cta", context).each(function () {
        if ($(this).attr("class").indexOf("admin") == -1)
            $(this).addClass("admin-table");
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

        var dateEnd = $(this).after(endInput).siblings(".date-range-end").val(targetValArr[1]).data("target", hiddenSibling).datetimepicker({
            stepMinute: 10, ampm: true, beforeShow: themeify, onSelect: function (dateText, inst) {

                var target = $(this).data("target");
                var targetValArr = $(target).val().split(',');

                if (targetValArr.length > 1)
                    targetValArr[1] = dateText;

                $(target).val(targetValArr.join());

            }
        }).addClass(classes).removeClass("jq-date-range");

        var dateStart = $(this).attr("name", "{0}_Start".format(name)).val(targetValArr[0]).data("target", hiddenSibling).toggleClass("date-range-start", true).datetimepicker({
            stepMinute: 10, ampm: true, beforeShow: themeify, onSelect: function (dateText, inst) {
                var target = $(this).data("target");
                var targetValArr = $(target).val().split(',');
                if (targetValArr.length > 0)
                    targetValArr[0] = dateText;
                console.log(target);
                $(target).val(targetValArr.join());
            }
        }).addClass(classes).removeClass("jq-date-range");

    });

    $(".jq-auto-complete[data-model]", context).each(function () {
        var self = this;
        var label = $(this).attr("data-label");


        if (!label)
            label = eval($(this).attr("data-onlabel"));
        else
            label = label.split(",");

        //label = label.push ? label[0] : label;

        var select = $(this).attr("data-onselect");
        var filter = $(this).attr("data-filter").split(",");

        
        
        var collection = eval($(this).attr("data-model"));
        var name = $(self).attr("name");
        var hdn = $("<input type='hidden' />").insertAfter(self).val($(self).val()).attr("name", name);
        $(self).bind("pk", function (e, id) {
            if (!$(this).val())
                $(this).val(id);
            if (id == 0)
                id = null;
            hdn.val(id);
        });
        console.log(hdn);
        $(self).attr("name",name+"__typeahead");
        
        
        
        $(self).autocomplete({
            source: function (req, add) {

                
                var q = req.term;
                collection.setSearch(q,filter);
                collection.fetch({
                    success: function (method, models) {
                        var result = [];
                        _.each(models, function (model) {
                            var idAttribute = "id";
                            if (collection._idAttr)
                                idAttribute = collection._idAttr;
                            var rowlabel = [];
                            
                            _.each(label,function(l) {
                                rowlabel.push(model[l]);
                            });
                            

                            result.push({label:rowlabel.join(", "),value:model[idAttribute],data:model});
                        });
                        add(result);
                    }
                });
                

            },
            minLength: 2,
            select: function (event, ui) {
                
                collection.trigger("autocomplete:select", ui.item);
                $(self).trigger("typeahead.select", ui.item);
                window.setTimeout(function () {
                    $(self).val(ui.item.label);
                }, 1);
                $(self).trigger("pk", ui.item.value);
                
                
                if (select)
                    select.call(this, event, ui.item.data);
            }
        });
        $(self).bind("click", function () {
            if($(this).val())
             $(this).select();
        });
        collection.bind("autocomplete:select", function (d) {
            
            
            
        });
    });

    $(".jq-auto-complete[data-src]", context).each(function () {
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
        return;
        
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
            var defaultFormat = "MM/dd/yyyy h:mm tt";
            var baseDate = date;
            if (date == "Invalid Date") {
                date = Date.parse((new Date()).toString("MM/dd/yyyy") + " " + val);
                defaultFormat = "h:mm tt";
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
            /*
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
            }*/

            var newFormat = "";
            if (newFormat = $(this).attr('data-time-format'))
                defaultFormat = newFormat;
            var newDate = date.toString(defaultFormat).replace("AM", "am").replace("PM", "pm").replace(' 0:', ' 12:');
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

	// SH - Currently disabled - Not finished - Adding to many instances to page
	//$('<input>').attr('type', 'hidden')
	//	.attr('id', 'ClientTimezoneOffset').attr('name', 'ClientTimezoneOffset')
	//	.attr('value', '0').appendTo('form');
    //$('#ClientTimezoneOffset').val( new Date().getTimezoneOffset() / 60 );

	//Admin - Calendar icons triggering date picker
    $(".npr").on("click", ".calendar-icon-big", function(event) {
    	$(this).parent().find('input').focus();
    });

	//NPR Admin - if event private disable featured event checkbox
    var privateCheck = function() {
    	var res_type = $("#Event_Visibility").val();
    	if (res_type === "Private") {
    		$('#Event_IsFeatured').attr('checked', false);
    		$("#Event_IsFeatured").attr("disabled", true);
		} else {
    		$("#Event_IsFeatured").removeAttr("disabled");
		}
	}

    $('#Event_Visibility').change(function() {
    	privateCheck();
    });

	privateCheck();
};