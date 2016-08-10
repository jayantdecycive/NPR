$(function () {

    var eventRenderLogic = function (event, element) {
        var startTime = event.start.toString("h:mm");
		
		var endTime = event.end.toString("h:mm tt");
		var timeLabel = $('<span class="timeString"></span>');
		timeLabel.text(startTime + " - " + endTime);
		element.append(timeLabel);
		
		var numberOfAvailableTickets = event.numberOfAvailable;
		var ticketLabel = $('<div class="ticketLabel"></div>');
		ticketLabel.text(numberOfAvailableTickets + " reservations available");
		element.append(ticketLabel);
        

    }

    $("#calendar").fullCalendar({
        defaultView: "basicWeek",
        theme: true,
        weekends: false,
        contentHeight: 300,
		theme: true,
        events: [
        { title: 'Event 1', start: "2012-06-04T09:30:00Z", end: "2012-06-04T12:30:00Z",
		numberOfTickets: 40, numberOfAvailable: 15 },
        { title: 'Event 2', start: "2012-06-04T12:30:00Z", end: "2012-06-04T14:30:00Z",
		numberOfTickets: 40, numberOfAvailable: 15 },
        { title: 'Event 3', start: "2012-06-05T09:30:00Z", end: "2012-06-05T12:30:00Z",
		numberOfTickets: 40, numberOfAvailable: 15 },
        { title: 'Event 4', start: "2012-06-05T12:30:00Z", end: "2012-06-05T14:30:00Z" ,
		numberOfTickets: 40, numberOfAvailable: 15 }
        ],
        eventRender: eventRenderLogic
    });

    var today = new Date();
    today.setDate(today.getDate() + 4);
    $("#calendar").fullCalendar('gotoDate', today);

});