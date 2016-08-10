$(function () {

    var action = $("form").attr("action");
    $("form").attr("action", "javascript:void(0);");
    $("form").attr("data-action", action);
    $("form").submit(function () {
        if ($(this).hasClass("fire"))
            return true;

        var storeId = $("#store_id").val();
        $cfa.Location("LocationById", storeId, function (store) {

            var peopleId = $("#person_id").val();

            var operator = store.OperatorContact;
            var location = store.LocationContact;
            if (!operator.Name || operator.PersonID.toString() != peopleId.toString()) {
                alert("There was an error: {0}".format(!operator.Name?"Store Does not Exist":"Incorrect Operator ID"));
                return false;
            }
            var data = { Name: operator.Name, Email: location.EmailAddress, AuthorityUID: operator.PersonID };
            $("form .store-print").remove();
            for (var i in data)
                $("form").append("<input type='hidden' class='store-print' value='{0}' name='{1}' />".format(data[i], i));

            var action = $("form").attr("data-action");
            $("form").addClass("fire").attr("action", action).submit();

        }, function () {

        });
    });

});