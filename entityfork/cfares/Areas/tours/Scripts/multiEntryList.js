(function ($) {

    var methods = {
        init: function (options) {
            if (options == null) { options = {}; }
            $(this).data("displayForm", $('<span class="display-form"><span class="list-text-display"></span><a href="javascript: void(0)" class="button ui-icon  ui-icon-pencil" /> <a href="javascript: void(0)" class="button ui-icon  ui-icon-trash" /></span>'));
            $(this).data("editForm", $('<span class="edit-form"><input placeholder="Guest Name" type="text" /><a href="javascript: void(0)" class="button ui-icon ui-icon-circle-check" /> <a href="javascript: void(0)" class="button ui-icon ui-icon-closethick" /></span>'));

            var url = window.location.href;
            if (options.model == null) {
                options.model = { exists: false, textValue: "" };
            }
            $(this).data("model", options.model);
            if (options.onCreateComplete) { $(this).one("onCreateComplete", options.onCreateComplete); }
            if (options.onEditBegin) { $(this).one("onEditBegin", options.onEditBegin); }
            if (options.onEditComplete) { $(this).one("onEditComplete", options.onEditComplete); }
            if (options.onDelete) { $(this).one("onDelete", options.onDelete); }

            var self = this;
            $(this).data("displayForm").find(".ui-icon-pencil").click(function () {
                $(self).InputListEntry("beginEdit");
                $(this).data("editForm").find("input").focus();
                return false;
            });

            $(this).data("displayForm").find(".ui-icon-trash").click(function () {
                $(self).InputListEntry("deleteEntry");
                return false;
            });

            var submitData = function () {
                if ($(self).data("editForm").find("input").val().length > 0) {
                    var rawTextValue = $(self).data("editForm").find("input").val();
                    var scrubbedValue = $.trim(rawTextValue.replace(/,/gi, ""));

                    $(self).data("model").textValue = scrubbedValue;
                    if ($(self).data("model").exists) {

                        $(self).InputListEntry("beginDisplay");
                        $(self).trigger("onEditComplete");
                    } else {

                        $(self).data("model").exists = true;
                        $(self).InputListEntry("beginDisplay");
                        $(self).trigger("onCreateComplete");
                    }

                }

            };

            $(this).data("editForm").find(".ui-icon-circle-check").click(function () {
                submitData();
                return false;
            });

            $(this).data("editForm").find(".ui-icon-closethick").click(function () {
                if ($(self).data("model").exists) {

                    $(self).InputListEntry("beginDisplay");
                    $(self).trigger("onEditComplete");
                } else {
                    $(self).InputListEntry("deleteEntry");

                }
                return false;
            });


            $(this).data("editForm").find('input').bind('keypress', function (e) {
                if (e.keyCode == 13) {
                    submitData();
                    return false;
                }
            });

            $(this).data("editForm").find('input').blur(function () {
                submitData();
            });

            $(this).append($(this).data("editForm"));
            $(this).append($(this).data("displayForm"));

            if ($(this).data("model").exists) {
                $(this).InputListEntry("beginDisplay");
                $(this).trigger("onCreateComplete");
            } else {
                $(this).InputListEntry("beginEdit");

            }

        },
        beginEdit: function () {
            $(this).data("displayForm").hide();
            $(this).trigger("editBegin");
            $(this).data("editForm").show().find("input").val($(this).data("model").textValue);

        },
        beginDisplay: function () {
            $(this).data("editForm").hide();
            $(this).data("displayForm").show().find(".list-text-display").text($(this).data("model").textValue); ;
        },
        deleteEntry: function (content) {
            $(this).attr("data-deleted", "true");
            $(this).trigger("onDelete");
            $(this).remove();
        }
    };

    $.fn.InputListEntry = function (method) {

        // Method calling logic
        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on jQuery.tooltip');
        }

    };

})(jQuery);
