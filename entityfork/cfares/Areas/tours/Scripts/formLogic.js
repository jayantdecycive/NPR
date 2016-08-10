$(document).ready(function () {

    
    $(".checkboxWithExplanation").each(
        function () {
            var checkbox = $(this).find('input[type="checkbox"]');
            var textBox = $(this).find('input[type="text"]');

            var updateBoxFunction = function () {

                if ($(checkbox).is(":checked")) {
                    $(textBox).removeAttr("disabled");
                } else {
                    $(textBox).attr("disabled", "true");
                }
            };
            $(checkbox).change(updateBoxFunction);

            //now call the update function once for init
            updateBoxFunction();

        }
    );

    /*------------------------Multi Name Entry App---------------------*/


    var newGuestCreated = function () {
        var guestNamesCSV = collectNames();
        $("#hiddenGuestList").val(guestNamesCSV);
        localStorage.setItem("guestList", guestNamesCSV);
    };

    var guestListEditBegin = function () {
        //$("#guest-list").find(".display-form").find(".button").attr("disabled", true);
    };

    var guestListEditComplete = function () {
        //$("#guest-list").find(".display-form").find(".button").removeAttr("disabled");
    };

    var guestDeleted = function () {
        var guestNamesCSV = collectNames();
        $("#hiddenGuestList").val(guestNamesCSV);
        localStorage.setItem("guestList", guestNamesCSV);
    };

    var createNewName = function (model, initialCreate) {
        var newListInput = $('<li></li>');
        $("#guest-list").append(newListInput);

        if (initialCreate) {
            newListInput.InputListEntry({
                model: model,
                onEditBegin: guestListEditBegin,
                onEditComplete: guestListEditComplete,
                onDelete: guestDeleted
            });
        } else {
            newListInput.InputListEntry({
                model: model,
                onCreateComplete: newGuestCreated,
                onEditBegin: guestListEditBegin,
                onEditComplete: guestListEditComplete,
                onDelete: guestDeleted
            });

        }
        return newListInput;
    };

    var updateCounters = function (model) {
        var countNumber;
        var requirementNumber;

        if (model.requirement == null) {
            requirementNumber = parseInt($("#groupSize").val());
        } else {
            requirementNumber = parseInt(model.requirement);
        }
        if (model.count == null) {
            countNumber = parseInt($("#guest-list-counter").text());
        } else {
            countNumber = parseInt(model.count);
        }


        $("#guest-count-requirement").text(requirementNumber);
        $("#guest-list-counter").text(countNumber);

        if (countNumber < requirementNumber) {
            $("#guest-counter-message").addClass("red");

            $(".guest-list-widget").find(".addNameButton").show();

        } else if (countNumber > requirementNumber) {
            $("#guest-counter-message").addClass("red");
        } else {
            $(".guest-list-widget").find(".addNameButton").hide();

            $("#guest-counter-message").removeClass("red");

            $("#guest-count-validation").removeClass("field-validation-error");
            $("#guest-count-validation").hide();
        }

    }

    var addNameButton = $('<span class="addNameButton"><span class="small">Add another guest</span><a href="javascript: void(0)" class="button ui-icon ui-icon-circle-plus" /> </span>');
    addNameButton.find(".button").click(function () {
        var count = parseInt($("#guest-list-counter").text());
        var requirement = parseInt($("#groupSize").val());
        if (count >= requirement) {
            return false;
        }
        var newNameInput = createNewName();
        $(newNameInput).find("input").focus();
        return false;
    });
    $(".guest-list-widget").find("#guest-counter-message").before(addNameButton);

    var collectNames = function () {
        var names = "";
        var count = 0;
        $("#guest-list").find(".display-form").each(function () {
            if (!$(this).parent().attr("data-deleted")) {
                count++;
                var fullName = $(this).find(".list-text-display").text();

                if (names.length == 0) {
                    names = fullName;
                } else {
                    names += "," + fullName;
                }
            }
        });
        $("#guest-count-requirement").text($("#groupSize").val());
        $("#guest-list-counter").text(count);
        updateCounters({ count: count, requirement: $("#groupSize").val() });
        return names;
    };


    var rawCSVList = "";
    if ($("#hiddenGuestList").val()&&$("#hiddenGuestList").val().length > 0) {
        rawCSVList = $("#hiddenGuestList").val();
        localStorage.setItem("guestList", rawCSVList);
    } else if (localStorage.getItem("guestList") != null) {
        rawCSVList = localStorage.getItem("guestList");
        $("#hiddenGuestList").val(localStorage.getItem("guestList"));
    }
    if (rawCSVList.length > 0) {

        var nameArray = rawCSVList.split(",");

        for (index = 0; index < nameArray.length; index++) {
            createNewName({ textValue: nameArray[index], exists: true }, true);

        }
        updateCounters({ count: nameArray.length, requirement: $("#groupSize").val() });
    } else {
        updateCounters({ count: 0, requirement: $("#groupSize").val() });

    }
    if (parseInt($("#guest-list-counter").text()) == 0) {
        createNewName();

    }

    //email checker

    $("#reservationUser_Email").blur(function () {
        if ($("#reservationUser_Email").val().length > 0) {

            var data = { value: $("#reservationUser_Email").val() };

            $.ajax({
                url: '/Reservation/CheckEmail',
                type: "POST",
                data: data,
                dataType: "json",
                success: function (data) {
                    if (data.userExists) {
                        var emailUsedDialog = $('<div></div>')
		                    .html('This email has already been used to create a reservation. Please click <a href="/Account/LogOn/' + $("#slotIdInput").val() + '">here</a> to log in.')
		                    .dialog({
		                        autoOpen: true,
		                        title: 'User Exists'
		                    });

                    }
                },
                error: function () {
                    console.log('email check failed!');
                }
            });
        }

    });


    //Listeners for special needs guests

    var hasSpecialNeedsRadio = $("#specialNeedsRadio").find('input[value="True"]');
    var hasSpecialNeedsRadio2 = $("#specialNeedsRadio").find('input[value="False"]');
    var updateSpecialNeedsAddOn = function () {
        if ($(hasSpecialNeedsRadio).is(":checked")) {
            $("#specialNeedsForm").show();
        } else {
            $("#specialNeedsForm").hide();
        }
    };
    hasSpecialNeedsRadio.change(updateSpecialNeedsAddOn);
    hasSpecialNeedsRadio2.change(updateSpecialNeedsAddOn);

    //once during init
    updateSpecialNeedsAddOn();

    //Add Datepicker jquery ui functionality

    $(".datepickerinput").datepicker({ beforeShowDay: $.datepicker.noWeekends, minDate: (4) });

    $(".timeRequestForm").find(".calendar-icon").click(function (e) {
        $(this).parent().find('.datepickerinput').datepicker("show");

        return false;
    });

    if ($("#lunchOptInSection").length != 0) {

        //Listener for lunch opt in
        var lunchRadios = $("#lunchOptInSection").find('[name="tourTicket.OptInForLunch"]');

        var updateLunchOrders = function () {
            $("#lunchOptInSection").find(".lunchCounter").each(
            function () {
                if (lunchRadios.filter('[value="True"]').is(":checked")) {
                    $(this).removeAttr("disabled");
                    // $("#tourTicket_GroupName").removeAttr("disabled");
                } else {
                    $(this).attr("disabled", "true");
                    //$("#tourTicket_GroupName").attr("disabled", true);
                }
            });

        };

        lunchRadios.change(updateLunchOrders);
        updateLunchOrders();


        //Listeners for special dietary needs description box

        var updateSpecialFoodDescriptionBox = function () {
            if (($("#tourTicket_NumberOfSpecialNeedLunches").val()) != "0" && ($("#NumberOfSpecialNeedLunches").val() != "")) {
                $("#tourTicket_SpecialDietNeedsDescription").removeAttr("disabled");
            } else {
                $("#tourTicket_SpecialDietNeedsDescription").attr("disabled", true);

            }

        }

        $("#tourTicket_NumberOfSpecialNeedLunches").keyup(updateSpecialFoodDescriptionBox);
        updateSpecialFoodDescriptionBox();

        //Listeners for all lunch counters
        var updateTotalforLunch = function () {

            var adultLunches = getLunchValue($("#tourTicket_NumberOfAdultLunches"));
            var kidLunches = getLunchValue($("#tourTicket_NumberOfKidLunches"));
            var specialLunches = getLunchValue($("#tourTicket_NumberOfSpecialNeedLunches"));

            function getLunchValue(obj) {
                var number = 0;
                if (obj.val() != null) {
                    if (!isNaN(parseInt(obj.val()))) {
                        number += obj.val();
                    }
                }
                return (number);
            }
            numberOfLunches = adultLunches + kidLunches + specialLunches;

            //update lunchTotal
            if (numberOfLunches > 0) {
                $("#lunchPriceSection").show();

                var totalPrice = (adultLunches * 7.0) + (kidLunches * 6.0) + (specialLunches * 7.0);
                var priceString = "$" + totalPrice.toFixed(2);
                $("#totalLunchAmount").text(priceString);
            } else {
                $("#lunchPriceSection").hide();
                $("#totalLunchAmount").text("-");
            }

        };
        $(".lunchCounter").keyup(updateTotalforLunch);
        updateTotalforLunch();
    }



    var groupSizeListener = function () {

        var groupSize = parseInt($("#groupSize").val());
        var ticketsAvailable = parseInt($("#ticketsAvailable").val());

        var x = document.URL.substring(7, 8);

        if (groupSize > ticketsAvailable) {
            lackOfSpaceDialog.find(".tickets-available-message").text(ticketsAvailable);
            lackOfSpaceDialog.dialog('open');
        }

        updateCounters({ requirement: $("#groupSize").val() });
    }

    var lackOfSpaceDialog = $('<div></div>')
		    .html('Unfortunately, this tour time only has <span class="tickets-available-message"></span> tickets available, Please <a href="http://story.tours.chick-fil-a.com/Reservation/Date">click here</a> to choose a new tour date and time.')
		    .dialog({
		        autoOpen: false,
		        title: 'Group Too Large'
		    });

    if ($(".form-body").attr("data-tourSize") == "Large") {
        lackOfSpaceDialog = $('<div></div>')
		    .html('Unfortunately, this tour time only has <span class="tickets-available-message"></span> tickets available, Please <a href="http://lgstory.tours.chick-fil-a.com/Reservation/Date">click here</a> to choose a new tour date and time.')
		    .dialog({
		        autoOpen: false,
		        title: 'Group Too Large'
		    });

    }


    $("#groupSize").change(groupSizeListener);
    groupSizeListener();


    $("form").submit(function () {
        $("#hiddenGuestList").val(collectNames());
        var count = parseInt($("#guest-list-counter").text());
        var requirement = parseInt($("#groupSize").val());
        if (count < requirement) {

            //12-19-2012 removed validation on guest names because names can be entered later

            //$("#guest-count-validation").text("You must enter the first and last names of every guest attending the tour.");
            //$("#guest-count-validation").addClass("field-validation-error");
            //$("#guest-count-validation").show();
            //return false;
        } else if (count > requirement) {
            $("#guest-count-validation").text("Please adjust your group size to reflect the number of guest names provided.");
            $("#guest-count-validation").addClass("field-validation-error");
            $("#guest-count-validation").show();
            return false;
        }
        localStorage.removeItem("guestList");
        return true;
    });

});

