$('[data-role=page]').live('pageshow', function (event, ui) {

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

    function toggleFieldOnCheck(field, checkbox) {
        if ($(checkbox).hasClass("")) {
            $(textBox).removeAttr("disabled");
        }
        else {
            $(textBox).attr("disabled", "true");
        }
    }

    //________________________________________________________________________________________________
    //Special Needs

    if ($('.needs-no:checked').val()) {
        $(".needs-wrapper").hide();
    }

    $('.needs-yes').bind("click", function () { $(".needs-wrapper").show(); });
    $('.needs-no').bind("click", function () { $(".needs-wrapper").hide(); });

    //________________________________________________________________________________________________
    //Lunch

    var lunch = false

    if ($(".lunch-no:checked").val()) {
        $(".lunch-wrapper").hide();
    }

    $('.lunch-yes').bind("click", function () { $(".lunch-wrapper").show(); lunch = true; });
    $('.lunch-no').bind("click", function () { $(".lunch-wrapper").hide(); lunch = false; });


    var time = new Date($('.slotTime').toString());
    if (time.getHours() > 9 || time.getHours() < 10) {
        $(".lunch-section").hide();
    }

    $("#guest-count").bind("change", function () {

    });

    // limit lunches to group size
    /*
    function checkCounters() {
    var a_lunches = parseInt($("#tourTicket_NumberOfAdultLunches").val());
    var k_lunches = parseInt($("#tourTicket_NumberOfKidLunches").val());
    var s_lunches = parseInt($("#tourTicket_NumberOfSpecialNeedLunches").val());
    var size = parseInt($("#guest-count").val());
    var total = a_lunches + k_lunches + s_lunches;

    if (total >= size) {
    $(".number-picker").each(function () {
    $(this).attr("max", $(this).val());
    });
    }
    else {
    $(".number-picker").attr("max", size);
    }
    }

    $("#tourTicket_NumberOfAdultLunches").bind("change", function () { checkCounters() });
    $("#tourTicket_NumberOfKidLunches").bind("change", function () { checkCounters() });
    $("#tourTicket_NumberOfSpecialNeedLunches").bind("change", function () { checkCounters() });
    */

    //________________________________________________________________________________________________
    //Group Size

    var groupSizeListener = function () {
        var groupSize = $("#tourTicket_GuestCount").val()
        var x = document.URL.substring(7, 8);
        if (groupSize > 40 && x == "s") {
            $groupSizeDialog.dialog('open');
        }
        if (groupSize < 41 && x == "l") {
            $groupSizeDialog.dialog('open');
        }
    }


    $("#tourTicket_GuestCount").change(groupSizeListener);
    groupSizeListener();
    $("#register-form").attr("data-ajax", "false");

    //________________________________________________________________________________________________
    //Group type other
    if ($('#tourTicket_IsOtherTypeOfGroup').is(':checked')) {
        $("#tourTicket_OtherTypeDescription").removeAttr("disabled");
    } else {
        $("#tourTicket_OtherTypeDescription").attr("disabled", "disabled");
    }

    $("#tourTicket_IsOtherTypeOfGroup").bind("change", function () {
        if ($('#tourTicket_IsOtherTypeOfGroup').is(':checked')) {
            $("#tourTicket_OtherTypeDescription").removeAttr("disabled");
        } else {
            $("#tourTicket_OtherTypeDescription").attr("disabled", "disabled");
        }

    });

    //________________________________________________________________________________________________
    //Guest List

    bindRemove();


    $("#guest-add-button").unbind('click');
    $("#guest-add-button").click(function () {
        var currentGuestCount = 1;
        var currentGuestCount = parseInt($("#guest-count").val());
        var listLength = $("#guest-list li").length;

        if ((listLength - 2) < currentGuestCount) {
            var name = $("#group-member-name").val();
            if (!name == null || !name == "") {
                $("#guest-list").append($('<li/>', {
                    'data-role': "list-divider"
                }).append($('<a/>', {
                    'class': 'guest-name', 'text': name
                })).append($('<a/>', {
                    'href': 'javascript:void(0)', 'class': 'guest-delete-button'
                })));
            }
            $("#group-member-name").val(''); //clear name field
            $("#group-member-name").focus(); //refocus name field
            refreshList()
        }
    });


    function refreshList() {
        //build model
        var guestList = "";
        var names = $(".guest-name");
        for (var i = 0; i < names.length; i++) {
            if (i < 1)
                guestList += (names[i].innerText);
            else
                guestList += ("," + names[i].innerText);
        }
        //set model
        $("#tourTicket_GuestList").val(guestList);
        //refresh view
        $("#guest-list").listview('refresh');
        //bind delete buttons
        bindRemove();

        //remove css 
        $('#guest-list li').bind('mouseover', function () {
            return false;
        });
        $('#guest-list li').bind('mousedown', function () {
            return false;
        });
    }


    //bind delete buttons
    function bindRemove() {
        $(".guest-delete-button").bind("click", function () {
            $(this).closest('li').remove();
            refreshList()
        });
    }


    var validateGuests = function () {
        var guestArray = $("#tourTicket_GuestList").val().split(",");
        var currentGuestCount = 1;
        if (!(isNaN(parseInt($("#guest-count").val())))) {
            currentGuestCount = parseInt($("#guest-count").val());
        }
        if (guestArray.length < currentGuestCount || (guestArray.length == 1 && (guestArray[0] == "" || guestArray[0] == null))) {   
            //$("#tourticket_GuestList_Validation").removeClass("field-validation-valid").addClass("field-validation-error").text("Please enter the first and last names of each group member.");
            //$("#form-validation").removeClass("field-validation-valid").addClass("field-validation-error").text("Registration was unsuccesful. Please re-enter your password, correct the errors and try again. ");
            //return false;
        } else if (guestArray.length > currentGuestCount) {
            $("#tourticket_GuestList_Validation").removeClass("field-validation-valid").addClass("field-validation-error").text("The number of names entered does not match the stated group size.");
            $("#form-validation").removeClass("field-validation-valid").addClass("field-validation-error").text("Registration was unsuccesful. Please re-enter your password, correct the errors and try again. ");
            return false;
        } else {
            $("#tourticket_GuestList_Validation").removeClass("field-validation-error").addClass("field-validation-valid").text("");
            $("#form-validation").removeClass("field-validation-error").addClass("field-validation-valid").text("");
            return true;
        }
    }



    $("#register-form, #edit-form").submit(function () {
        var check = validateGuests();
        if (!check) {
            $("body").scrollTop(0);
            $("#userForm_Password").val("");
            $("#userForm_ConfirmPassword").val("");
        }
        return check;
    });


});
