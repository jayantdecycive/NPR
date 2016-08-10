var sharedPhoto = "@ViewBag.sharedPhoto";
var url = "http://tours.chick-fil-a.com";
var description = "Make a reservation to attend a Chick-fil-A® Home Office Backstage Tour, a behind-the-scenes look at the history and food of Chick-fil-A. Register Now! ";


function facebookShare() {
    FB.ui(
        {
            method: 'feed',
            name: 'Chick-fil-A Home Office Backstage Tours',
            link: url,
            caption: 'Chick-fil-A Home Office Backstage Tours',
            picture: sharedPhoto,
            description: description
        },
        function (response) {
            if (response && response.post_id) {

            } else {

            }
        });

};

function twitterShare() {
    var twitterShareURL = "http://twitter.com/share?text={0}&url={1}";
    var twitterDescription = "Make a reservation to attend a Chick-fil-A® Home Office Backstage Tour";
    twitterShareURL = twitterShareURL.format(encodeURIComponent(twitterDescription), encodeURIComponent(url));

    var width = 575,
        height = 252,
        left = ($(window).width() - width) / 2,
        top = ($(window).height() - height) / 2,
        opts = 'status=1' +
                    ',width=' + width +
                    ',height=' + height +
                    ',top=' + Math.round(top) +
                    ',left=' + left;

    window.open(twitterShareURL, 'twitter', opts);
};


function pinterestShare() {

    var url = "http://pinterest.com/pin/create/button/?url=http%3A%2F%2Ftours.chick-fil-a.com&media=http%3A%2F%2Ftours.chick-fil-a.com%2FAreas%2FTemp%2FContent%2FImg%2Flogo.png&description=Make%20a%20reservation%20to%20attend%20a%20Chick-fil-A%C2%AE%20Home%20Office%20Backstage%20Tour%2C%20a%20behind-the-scenes%20look%20at%20the%20history%20and%20food%20of%20Chick-fil-A.%20Register%20Now!%20"
    window.location.href = url;
}


function mailShare() {
    window.location.href = "mailto:?subject=Make a reservation to attend a Chick-fil-A® Home Office Backstage Tour&body=Hello, your friend wanted to share a link with you,%0D%0A%0D%0AMake a reservation to attend a Chick-fil-A® Home Office Backstage Tour, a behind-the-scenes look at the history and food of Chick-fil-A. Register Now at tours.chick-fil-a.com!"
};


$(document).ready(function () {

    $("#share-fb").bind("click", function () { facebookShare(); });
    $("#share-tw").bind("click", function () { twitterShare(); });
    $("#share-pin").bind("click", function () { pinterestShare(); });
    $("#share-email").bind("click", function () { mailShare(); });

});
