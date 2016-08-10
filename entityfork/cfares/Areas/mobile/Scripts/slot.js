$(document).ready(function () {

    var myCollection = new DomainModel.SlotCollection();
    var url = window.location.href;
    var groupSize = parseInt(url.substring(url.lastIndexOf('/') + 1));
    var tourType;
    if (groupSize == null || isNaN(groupSize)) {
        groupSize = 1;
    }
    if (groupSize > 60)
        tourType = "lgstory";
    else
        tourType = "story";

    var slotCollection;

    function getEventsNearDate(startDate, endDate) {
        myCollection.GetByEventTypeWithDateRange({ EventType: tourType, Start: startDate.toString("yyyy-MM-dd"), End: endDate.toString("yyyy-MM-dd") },
        {
            success: function (collection, response) {
                var events = [];

                for (i = 0; i < collection.models.length; i++) {

                    events.push({
                        start: collection.models[i].attributes.Start.toString(),
                        end: collection.models[i].attributes.End.toString(),
                        slotStart: collection.models[i].attributes.Start,
                        slotEnd: collection.models[i].attributes.End,
                        title: collection.models[i].attributes.SlotId,
                        ticketsAvailable: collection.models[i].attributes.TicketsAvailable,
                        ticketsUsed: collection.models[i].attributes.TotalTickets,
                        occurrenceId: collection.models[i].attributes.OccurrenceId,
                        cutOff: collection.models[i].attributes.Cutoff
                    });
                }
                addSlots(events);
                //eventDataCallback(events);
            },
            error: function () { console.log(arguments); console.log(this); }
        });
    }


    function populateMonths() {
        if ($('#select-month').data("primed"))
            return;

        $('#select-month').data("primed", true);
        $('#select-month').empty();
        $('#select-month').append('<option selected="selected">Choose a Month</option>');

        $cfa.TourData("SlotMonthRangeCheckGroupSize", tourType, groupSize, function (data) {
            var now = new Date();
            $.each(data, function (index) {
                var eventMonthNumber = data[index].Month;
                var eventYearNumber = data[index].Year;
                var eventMonthObj = Date.parse("{0}/01/{1}".format(eventMonthNumber, eventYearNumber));
                if (now <= eventMonthObj) {
                    var month = (eventMonthObj).toString("MMMM")
                    if ($("#month-option-" + index).length == 0) {
                        $('#select-month').append(
                        $('<option></option>').val(data[index].SlotYearMonth).html(month + " " + (data[index].Year)).attr('id', "month-option-" + index)
                    );
                    }

                }


            });
        });
    }


    function populateDays() {
        $('#select-date').empty();
        $('#select-date').append('<option selected="selected">Choose a Day</option>');
        $('#select-date').selectmenu("refresh");


        var year = $('#select-month').val().substring(0, 4);
        var month = $('#select-month').val().substring(4, ($('#select-month').val().length));

        var checksum = JSON.stringify([year, month]);
        if ($('#select-date').data("model") == checksum)
            return;
        $('#select-date').data("model", checksum);
        console.log(checksum);

        $cfa.TourData("SlotDateRangeCheckGroupSize", tourType, year, month, groupSize, function (data) {

            var today = new Date();
            $.each(data, function (index) {
                var date = data[index].Date;
                var day = new Date(year, month - 1, date, 0, 0, 0, 0);

                if (today <= day) {
                    if ($("#date-option-" + index).length == 0) {
                        $('#select-date').append(
                        $('<option></option>').val(date).html(day.toString("dddd MMMM d")).attr('id', "date-option-" + index)
                    );
                    }
                }

            });
        });
    }


    function addSlots(collection) {
        $.each(collection, function (index) {
            var slotStart = collection[index].slotStart.toString("h:mm tt");
            var id = collection[index].title.toString();

            var x = collection[index].ticketsAvailable;
            var y = collection[index].ticketsUsed;

            if ($("#time-option-" + index).length == 0 && (collection[index].slotStart < collection[index].slotEnd) && (collection[index].ticketsAvailable >= groupSize)) {
                $('#select-slot').append(
                    $('<option></option>').val(id).html(slotStart).attr('id', "time-option-" + index)
                );
            }
        });
    }


    function populateSlots() {
        $('#select-slot').empty();
        $('#select-slot').append('<option selected="selected">Choose a Time</option>');
        $('#select-month').selectmenu("refresh");
        var year = $('#select-month').val().substring(0, 4);
        var month = $('#select-month').val().substring(4, ($('#select-month').val().length));
        month -= 1;
        var day = $('#select-date').val();
        var start = new Date(year, month, day, 0, 0, 0, 0);
        var end = new Date(year, month, day, 0, 0, 0, 0);
        end.setDate(end.getDate() + 1);
        getEventsNearDate(start, end);
    }


    function changeSubmitLink() {
        $("#button-submit").attr("href", "/mobile/ticket/register/" + ($("#select-slot").val()));
    }

    populateMonths();
    $('#select-month').bind("change", populateDays);
    $('#select-date').bind("change", populateSlots);
    $('#select-slot').bind("change", changeSubmitLink);

});