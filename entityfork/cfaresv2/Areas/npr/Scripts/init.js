

$(".button-icon-container").click(function() {
    var form = $(this).closest("form");
    form.submit();
});



//========================================================
//  GUEST LIST

    //bundle names on form submit
    $("form").submit(function () { BundleNames(); });

    //update number of name fields on guest count change
    $("#GroupSize").change(function () {
        UpdateGuestListCount();
        $("#slotdatepicker").val('');

    });




    //bundle all first and last name fields into comma seperated string 
    function BundleNames() {
        var list = $(".guest-list");
        if (list.length <= 0) {return false;}

        var guests = "";
        var names = $(".guest-list .name");
        for (var i = 0; i < names.length; i++) {
            var first = $(names[i]).find(".first").val();
            var last = $(names[i]).find(".last").val();
            if(i != names.length-1)
                guests += first + " " + last + ",";
            else
                guests += first + " " + last;
        }
        $("#guestlist-field").val(guests);
    }


    // add fields for one name
    function UpdateGuestListCount() {
        var container = $(".guest-list");
        var currentGuestCount = $(".name").length;
        var newGuestCount = parseInt($("#GroupSize").val(), 10);
        if (isNaN(newGuestCount)) { return false; }
        var diff = Math.abs(newGuestCount - currentGuestCount);

        var template = "<div class='name'><span class='asterisk'>* </span><span class='guest-label'> Guest {0}: </span>\
                             <input type='text' class='first required' name='first{1}' id='first{1}'/><input type='text' class='last required' name='last{2}' id='last{2}'/></div>";
                
        //to larger size
        if (currentGuestCount < newGuestCount) {
            for(var i=0;i<diff;i++)
            {
                container.append(template.format(i + currentGuestCount + 1, i + currentGuestCount + 1, i + currentGuestCount + 1));
            }
        }
        //to smaller size
        else {
            $(".name:gt("+ (currentGuestCount - diff - 1) +")").remove();
        }
    }    

    var ownerNameSync = function() {
    	$("input[name='first1']").val($("#Owner_FirstName").val());
    	$("input[name='last1']").val($("#Owner_LastName").val());
    };

    $("#Owner_FirstName, #Owner_LastName, #Owner_Email").keyup(ownerNameSync).blur(ownerNameSync);

//========================================================
//  FORM VALIDATION

    $.validator.addClassRules('first', {
        required: true ,
        alpha: true
    });
    
    $.validator.addClassRules('last', {
        required: true,
        alpha: true
    });


 

    $("form").validate({
        rules: {
            slotdatepicker: "required",
            "Owner.FirstName": {
                alpha: true
            },
            "Owner.LastName": {
                alpha: true
            },
            "Owner.Username": {
                user_email_not_same: true
            },
            "Owner.HomePhoneString": {
                required: true,
                minimum: true
            },
            "Email": {
                email: true
            },
            "ConfirmNumber": {

            }
        },
        messages: {
            slotdatepicker: "Please select a date",
            "Owner.FirstName": "Please enter a valid first name.",
            "Owner.LastName": "Please enter a valid last name.",
            "Owner.Email": 'Please enter an email address',
            "Owner.Username": 'Please enter a matching email address',
            "Owner.HomePhoneString": "Please enter a valid phone number.",
            "Dates[0]": "Select a date",
            "Email": "Please enter an email address",
            "ConfirmNumber": "Please enter an 8 digit confirmation number"
        },
        success: function(e){
            
        }
    });
    
    // custom validateion methods
    $.validator.addMethod("alphanumeric", function (value, element) {
        return this.optional(element) || /^\w+$/.test(value);
    }, "No special characters");
    
    $.validator.addMethod("alpha", function (value, element) {
        return this.optional(element) || /^[a-zA-Z\s]+$/.test(value);
    }, "No special characters");
    
    $.validator.addMethod("user_email_not_same", function (value, element) {
        return $('#Owner_Email').val() == $('#Owner_Username').val();
    }, "Emails must match.");
    
    $.validator.addMethod("phoneUS", function (phone_number, element) {
        return this.optional(element) || phone_number.match(/[0-9,]+/);
    }, "Please specify a valid phone number");
    
    $.validator.addMethod("minimum", function (phone, element) {
        return this.optional(element) || phone.length > 9;
    }, "A 10 digit phone number is required");
    
   
    
    
    
    //phone field on public tours registration
    $("input[name=ContactPreference]").click(function() {
        if ($(this).val() == "Phone") {
            $("#Owner_HomePhoneString").addClass("required");
            $("#Owner_HomePhoneString").prop('disabled', false);
        }
        else {
            $("#Owner_HomePhoneString").removeClass("required error");
            $("#Owner_HomePhoneString").prop('disabled', true);
        }
    });



    
    
    
//========================================================
//  DATEPICKER

    var legendTemplate = "<div class='legend'><h3>Calendar Key</h3><div class='item active'>selected date</div><div class='item cut-off'>no longer accepting reservations</div><div class='item no-vacancy'>not enough vacancy</div><div class='item closed'>NPR Closed</div></div>";
    var requestLegendTemplate = "<div class='legend'><h3>Calendar Key</h3><div class='item active'>selected date</div><div class='item closed'>NPR Closed</div></div>";
    var slotInfoDict = {};
    
//create generic datepickers
$(".slotpicker").datepicker({ format: 'mm-dd-yyyy', autoclose: true }).on("render", function (e) {

    var newMonth = e.date.getUTCMonth();
    var newYear = e.date.getUTCFullYear();

    $("td", ".datepicker-days:visible").each(function () {
        var cell = $(this);
        var day = Number($(this).text());
        var calDay = new Date();
        calDay.setDate(day);
        calDay.setMonth(newMonth);
        calDay.setYear(newYear);
        
        //legend
        $(".datepicker-days .legend").remove();
        $(".datepicker-days").append(requestLegendTemplate);
        
        //weekends
        $(this).parent().find(".day").eq(0).addClass("closed").click(false);
        $(this).parent().find(".day").eq(6).addClass("closed").click(false);
        
        //past dates or another month
        if ( calDay < new Date() || cell.hasClass("old") || cell.hasClass("new")) {
            cell.addClass("no-slots").click(false);
            return;
        }
    });

});



//create slot datepickers
$("#slotdatepicker").datepicker({
        autoclose: true,
        format: "MM dd, yyyy"

    }).on("render", function (e) {

        var newMonth = e.date.getUTCMonth() + 1;
        var newYear = e.date.getUTCFullYear();
        var key = "{0}_{1}".format(newMonth, newYear);

        $(".datepicker-days .legend").remove();
        $(".datepicker-days").append(legendTemplate);
        $(".datepicker").prepend("<div class='cal-cover'></div>");


        var coverDatePicker = function (slotCollection) {
            var slots = true;
            if (slotCollection == "loading")
                return;
            if (slotCollection.length < 1)
                slots = false;
            else {
                var first = slotCollection.at(0);
                var startstring = first.get("Start");
                var startDate = Date.fromISO(startstring);
                var month = startDate.getUTCMonth() + 1;
                var year = startDate.getUTCFullYear();
                var newKey = "{0}_{1}".format(month, year);
                if (newKey != key)
                    return;
            }
            
            $("td", ".datepicker-days:visible").not(".old,.new").each(function () {
                var cell =$(this);
                var day = Number($(this).text());
                var slotsOnThisDay = slotCollection.filter(function (slot) {
                    var start = Date.fromISO(slot.get("Start"));
                    return start.getUTCDate() == day;
                });

                //weekends
                $(this).parent().find(".day").eq(0).addClass("closed").click(false);
                $(this).parent().find(".day").eq(6).addClass("closed").click(false);
                
                //slots exist
                if (slotsOnThisDay.length > 0) {
                    var slot = slotsOnThisDay[0];
                    var cutoff = Date.fromISO(slot.attributes["Cutoff"]);
                    var ticketsAvailable = Number(slot.attributes["TicketsAvailable"]);
                    var start = Date.fromISO(slot.attributes["Start"]);
                    var members = Number($("#GroupSize").val());
                    var type = slot.attributes["ReservationTypeId"];
                    var isevent = false;
                    
                    //date ie fix
                    var now = Date.now || function () { return +new Date; };

                    //slot is in the past
                    if (start < now || isevent) {
                        cell.addClass("no-slots").click(false);
                        return;
                    }
                    // slots past cutoff
                    else if (cutoff.getTime() < (new Date()).getTime()) {
                        cell.addClass("cut-off").click(false);ReservationTypeId: ""

                        return;
                    }
                    // slots full
                    else if (ticketsAvailable < members) {
                        cell.addClass("no-vacancy").click(false);
                        return;
                    }
                    // regular slot, assign id to hidden field on click
                    else {
                        cell.click(function () {
                            $("#slotId").val(slotsOnThisDay[0].id);
                            $("#slotdatepicker").removeClass("error");
                            $("label[for='slotdatepicker']").css("display", "none");
                            
                        });
                    }
                }
                //no slots
                else {
                    cell.addClass("no-slots").click(false);
                }
                $(".cal-cover").remove();
            });
            $("td.old, td.new").click(false);
        };


        //GET SLOTS
        if (slotInfoDict[key]) {
            coverDatePicker(slotInfoDict[key]);
        } else {
            //slotInfoDict[key] = "loading";
            var col = DomainModel.SlotCollection.NPRByMonth({ month: newMonth, year: newYear, type: "Tour" }, {
                success: function (collection, data, httpcode) {
                    
                    slotInfoDict[collection.key] = collection;
                    coverDatePicker(collection);                    
                }
            });
            col.key = key;
        }
    });





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