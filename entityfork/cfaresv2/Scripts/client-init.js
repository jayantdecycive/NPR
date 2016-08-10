$(function () {
    $(".carousel").hammer().on("swiperight", function (e) {
        $(this).carousel('pause').carousel('prev');
        
    });
    $(".carousel").hammer().on("swipeleft", function () {
        
        $(this).carousel('pause').carousel('next');
    });
    $("a[data-location-favorite]").click(function (e) {
        e.preventDefault();
        var id = $(this).attr("data-location-favorite");
        if ($(".add-as-favorite[data-location='" + id + "']") != null)
        {
            $(".add-as-favorite[data-location='" + id + "']").modal("show");
        }
        
        return false; 
    });
}); 