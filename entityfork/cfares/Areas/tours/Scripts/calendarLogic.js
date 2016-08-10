$(function () {

    var myCollection = new DomainModel.SlotCollection();


    var initialRender = true;
    function renderCalendar() {
        var calendarObj = $("#calendarDiv");

        var updateMonthHeader = function () {
            var currentCalendarDate = calendarObj.fullCalendar('getDate');
            calendarObj.find(".monthHeader").text(currentCalendarDate.toString("MMMM yyyy"));
        };

        if (initialRender) {
            initialRender = false;

            var headerDiv = $("<div class=\"calendarHeader\"></div>");
            var monthHeader = $("<span class=\"monthHeader\"></span>");


            //initialize the header with initial date
            updateMonthHeader();


            var previousMonthLink = $("<a class=\"month-button previous\" href=\"#\"></a>");
            previousMonthLink.click(function () {
                calendarObj.fullCalendar('incrementDate', 0, -1, 0);
                return false;
            });
            var nextMonthLink = $("<a class=\"month-button next\" href=\"#\"></a>");
            nextMonthLink.click(function () {
                calendarObj.fullCalendar('incrementDate', 0, 1, 0);
                return false;
            });


            headerDiv.append(previousMonthLink);
            headerDiv.append(monthHeader);
            headerDiv.append(nextMonthLink);

            calendarObj.prepend(headerDiv);

            var nextWeekButton = $("<a class=\"week-button next-week\" href=\"#\"></a>");
            nextWeekButton.click(function () {
                calendarObj.fullCalendar('incrementDate', 0, 0, 7);
                return false;
            });
            var previousWeekButton = $("<a class=\"week-button previous-week\"  href=\"#\"></a>");
            previousWeekButton.click(function () {
                calendarObj.fullCalendar('incrementDate', 0, 0, -7);
                return false;
            });

            calendarObj.find(".fc-view").before(nextWeekButton);
            calendarObj.find(".fc-view").before(previousWeekButton);
        }

        //Go through and style date headers
        $("#calendarDiv").find("thead").find("th").each(function () {
            var splitDate = $(this).text().split(" ");
            if (splitDate.length < 3) {
                splitDate[2] = splitDate[1];
                splitDate[1] = splitDate[0].substring((splitDate[0].length - 3), splitDate[0].length);
                splitDate[0] = splitDate[0].substring(0, (splitDate[0].length - 3));
            }
            var dayOfWeek = $("<span class=\"dayOfWeek\">" + splitDate[0] + "</span>");
            var date = $("<span class=\"date\">" + splitDate[1] + " " + splitDate[2] + "</span>");
            $(this).empty();
            $(this).append(dayOfWeek);
            $(this).append(date);

            $(this).css("width", "205");
        });

        updateMonthHeader();
    }

    function getEventsNearDate(startDate, endDate, eventDataCallback) {
        myCollection.GetByEventTypeWithDateRange({ EventType: Application.EventId, Start: startDate.toString("yyyy-MM-dd"), End: endDate.toString("yyyy-MM-dd") },
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
                eventDataCallback(events);
            },
            error: function () { console.log(arguments); console.log(this); }
        });

    }


    var eventRenderLogic = function (event, element) {
        $(element).empty();

        var today = new Date().getTime();
        var groupSize = $("#calendarDiv").data("groupSize");

        var ticketID = $("#calendarDiv").attr("data-ticket");

        var hrefTemplate = "<a href='/Reservation/Begin/{0}' class='eventBody'></a>";
        if (ticketID)
            hrefTemplate = "<a href='/Reservation/Modify/" + ticketID + "?slot={0}' class='eventBody'></a>";

        var ticketsLeft = event.ticketsAvailable;
        var ticketLabel = $('<div class="ticketLabel"></div>');
        if (today < event.cutOff.getTime() && ticketsLeft >= groupSize && today < event.slotStart.getTime()) {
            var eventContent = $(hrefTemplate.format(event.title));

            ticketLabel.text(ticketsLeft + " reservations available");
        }
        else {
            var eventContent = $("<div class=\"eventBodyPast\"></div>");

            if (today > event.slotStart.getTime()) {
                ticketLabel.text("Event has already occurred");
            }
            else if (today > event.cutOff.getTime()) {
                ticketLabel.text("No longer accepting reservations");
                ticketLabel.addClass("condensed");
            } else if (ticketsLeft < groupSize) {
                ticketLabel.text(ticketsLeft + " reservations available");

            }
        }
        eventContent.append(ticketLabel);

        eventContent.data("id", event.title);
        eventContent.data("ticketsUsed", event.ticketsUsed);

        var startTime = event.slotStart.toString("h:mm");
        var endTime = event.slotEnd.toString("h:mm tt");


        var timeLabel = $('<span class="timeString clean-timezone"></span>');
        timeLabel.text(startTime + " - " + endTime);
        eventContent.append(timeLabel);





        eventContent.append(ticketLabel);

        /*eventContent.click(function () {
        window.location = "/Reservation/Begin/"+event.title;
        return false;
        });*/

        $(element).append(eventContent);
    }

    $("#calendarDiv").fullCalendar({
        defaultView: "basicWeek",
        theme: false,
        weekends: false,
        header: false,
        contentHeight: 420,
        theme: true,
        events: getEventsNearDate,
        eventRender: eventRenderLogic,
        viewDisplay: renderCalendar,
        columnFormat: { basicWeek: "dddd MMM dd" }
    });

    $("#calendarDiv").fullCalendar('gotoDate', Date.today().add({ day: 4 }));

});