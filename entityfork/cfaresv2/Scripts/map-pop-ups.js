var RWDDimensions={
    mobileWidth:768,
    tabletWidth:979
}

function reverseGeocodeToPostalCode() {

    // Set user's zip code via reverse geoLocation if not already present
    if ($('#SelectedLocation').val() == '' && navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(function (a) {
            if (a.coords) {
                geocoder = new google.maps.Geocoder();
                var lat = a.coords.latitude;
                var lng = a.coords.longitude;
                var latlng = new google.maps.LatLng(lat, lng);
                geocoder.geocode({ 'latLng': latlng }, function (results, status) {
                    if (status == google.maps.GeocoderStatus.OK) {
                        if (results[1]) {

                            var addressElements = results[1].address_components;
                            var postalCode = '';

                            for (var i = 0; i < addressElements.length; i++)
                                if (addressElements[i]['types'][0] == "postal_code")
                                    postalCode = addressElements[i]['short_name'];

                            if (postalCode && postalCode != '')
                                $('#SelectedLocation').val(postalCode);
                        }
                    } else {
                        console.log('Reverse geocode failed [ Code = ' +
                            status + ', Latitude = ' + lat + ', Longitude = ' + lng + ' ]');
                    }
                });
            }
        });
    } else {
        console.log('navigator.geoLocation not supported.');
    }
}

function bindViewMapLinks() {

    $('*[data-poload]').bind('click', function (evnt) {
        var isMobile = $(window).width() <= RWDDimensions.mobileWidth;
        var e = $(this);
        if (isMobile) {
            evnt.preventDefault();
            window.open(e.data('poload'), '_blank');
            return false;
        }
        

        if (e.attr('maptoggle') == 'true')
            e.removeAttr('maptoggle');
        else
            e.attr('maptoggle', 'true');

        var mapToggle = (e.attr('maptoggle') == 'true');
        var mapLoaded = (e.attr('maploaded') == 'true');

        if (mapToggle) {

            $('*[maptoggle]').each(function () {
                if (this.id != e[0].id)
                    $(this).popover('hide');
            });

            if (!mapLoaded) {

                e.popover({
                    html: true,
                    trigger: 'manual',

                    title: function () { return "TITLE"; },

                    content: '<iframe width="620" height="400" frameborder="0" scrolling="no" marginheight="0" marginwidth="0" src="' + e.data('poload') + '&amp;output=embed"></iframe>'

                }).popover('show');
                e.attr('mapLoaded', 'true');
            }
            else
                e.popover('show');

            $('.popover-title').append(function () {
                return $('<button class="btn small default viewMapCloseButton" value="Close">Close</button>').click(function () {
                    e.popover('hide');
                });
            });
        }
        else
            e.popover('hide');

        return false;
    });
}

$(function () {
    reverseGeocodeToPostalCode();
    bindViewMapLinks();
});