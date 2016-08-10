

$(function () {
    $("*[data-scaffold='editor-for']").find(".editor-label").each(function () {
        console.log(this);
        var field = $(this).next(".editor-field");
        $(this).wrap("<div class='model-field' />").parent().append(field).children().addClass("inline-block");
    });

});


$(document).ready(function() {
    //nav event tour select box
    $("#admin-section-picker").on('change', function (e) {
        if ($(this).val() != "" && $(this).val() != null) {
            window.location = $(this).val();
        }
    });
    

    //npr ticket phone field
    $("#ContactPreference").change(function () {
        if ($(this).val() == "Phone") {
            $(".field-HomePhone").removeClass("invisible");
        }
        else {
            $(".field-HomePhone").addClass("invisible");
        }
    });
    


    //event url validation
    jQuery.validator.addMethod("urlmatch", function (value, element) {
        return (/http:\/\//i.test(value));
    }, "Please enter a valid url");

    $('form').validate({
        rules: {
            "Event.Urls": {
                urlmatch: true,
                required: true
            }
        },
        messages: {
            "Event.Urls": "Please enter a valid URL"
        }

    });

    var ItemsView = function(items,cat) {
        var canvas = cat ? $("#food_canvas .category[data-pk='{0}']".format(cat.pid)).fadeIn() : $("#food_canvas").fadeIn();
        canvas.empty();
        _.each(items, function(item) {
            if (item.NoImage) return;
            var template = $("#item_template").html(); // SH - Changing from '.text()' to '.html()' for IE8 fix
            canvas.append(template.format(item.DomId, item.Name.replace(/<(?:.|\n)*?>/gm, ''), item.ImgReduced, item.ImgXLarge, item.local ? "local" : "remote"));

            if (foodConditions[item.DomId] != null) {
                $("select", "#food_canvas .food-selection[data-pk='{0}']".format(item.DomId)).val(foodConditions[item.DomId]);
            }
            if (cat) {
                if (cat.pid > 1) {
                    $("#food_canvas .category[data-pk='{0}']".format(cat.pid)).hide();
                } else {
                    //$("#food_canvas .category[data-pk='{0}']".format(cat.pid)).slideDown();
                    $("#food_canvas").slideDown();
                }
            }
            if (foodConditions.mapItem) {
                foodConditions.mapItem(item, $("#food_canvas .food-selection[data-pk='{0}']".format(item.DomId)));
            }
        });
    };

    var CategoriesView = function(cats) {
        _.each(cats, function(cat) {
            $("#food_categories").append("<option value='{0}'>{1}</option>".format(cat.pid, cat.Name));
            $("#food_canvas").append("<div class='category' data-pk='{0}'></div>".format(cat.pid));
            $("#food_canvas").hide();

	        console.log('ItemsByGroupIncludingDraftItems - ' + cat.pid);
            $cfa.Menu("ItemsByGroupIncludingDraftItems", cat.pid, function(items) {
            	console.log(items);
            	ItemsView(items, cat);
            });

        });
        $("#food_canvas .category:gt(0)").hide();
        $("#food_categories").change(function() {
            var val = $(this).val();
            if (!val) {
                $("#food_canvas .category").hide();
                return;
            }
            $("#food_canvas .category").fadeOut();
            if ($("#food_canvas .category[data-pk='{0}'] .food-selection".format(val)).length) {
                $("#food_canvas .category[data-pk='{0}']".format(val)).fadeIn();
                return;
            }

        });
        $("#food_categories").selectmenu({ style: 'dropdown' }, "widget").addClass("");
    };

    $("#food_canvas[data-load]").each(function () {
        if (window.foodConditions && window.foodConditions.ignoreOthers) {
            var items = _.map(foodConditions, function (v,k) {
                return {DomId:k, Allowance:v, ImgReduced:"", ImgXLarge:"",local:true};
            });
            ItemsView(items);
        } else {
            $cfa.Menu("LoadFoodCategories", function(cats) {
                CategoriesView(cats);
            });
        }
    });

});

