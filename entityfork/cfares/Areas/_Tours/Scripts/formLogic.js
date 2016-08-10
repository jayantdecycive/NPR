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



    

    
    if ($("#lunchOptInSection").data("priceModel") == "Customer") {
        //Listener for lunch opt in
        var lunchRadios = $("#lunchOptInSection").find('[name="lunchForm.optInForLunch"]');

        var updateLunchOrders = function () {
            $("#lunchOptInSection").find(".lunchCounter").each(
            function () {
                if (lunchRadios.filter('[value="True"]').is(":checked")) {
                    $(this).removeAttr("disabled");
                } else {
                    $(this).attr("disabled", "true");
                }
            }
        );

        };
        lunchRadios.change(updateLunchOrders);
        updateLunchOrders();

        //Listeners for special dietary needs description box

        var updateSpecialFoodDescriptionBox = function () {
            if (($("#lunchForm_numberOfSpecialNeeds").val()) != "0" && ($("#numberOfSpecialNeeds").val() != "")) {
                $("#lunchForm_descriptionOfSpecialNeeds").removeAttr("disabled");
            } else {
                $("#lunchForm_descriptionOfSpecialNeeds").attr("disabled", true);

            }

        }
        $("#lunchForm_numberOfSpecialNeeds").keyup(updateSpecialFoodDescriptionBox);
        updateSpecialFoodDescriptionBox();

        //Listeners for all lunch counters
        var updateTotalforLunch = function () {
            var numberOfLunches = 0;
            $(".lunchCounter").each(
                function () {
                    if (($(this).val()) != "0" && ($(this).val() != "")) {
                        var thisLunches = parseInt($(this).val());
                        if (!isNaN(thisLunches)) {
                            numberOfLunches += thisLunches;

                        }
                    }

                });
            //update lunchTotal
            if (numberOfLunches > 0) {
                $("#lunchPriceSection").show();
                var totalPrice = numberOfLunches * 6.5;
                var priceString = "$" + totalPrice.toFixed(2);
                $("#totalLunchAmount").text(priceString);
            } else {
                $("#lunchPriceSection").hide();
                $("#totalLunchAmount").text("-");
            }

        };
        $(".lunchCounter").keyup(updateTotalforLunch);
        updateTotalforLunch();
    } else {
        $("#lunchPriceSection").hide();
    }


    //Have time picker hide or show lunch opt in form
    var tourTimeListener = function () {
        var lunchValidTimeSlot = false;
        $(".timeselectinput").each(function () {
            if ($(this).find(":selected").val() == "9:30 AM") {
                lunchValidTimeSlot = true;
            }

        });
        if (lunchValidTimeSlot) {
            $("#lunchOptInSection").show();

        } else {
            $("#lunchOptInSection").hide();
        }
    };

    $(".timeselectinput").change(tourTimeListener);
    tourTimeListener();
});

