(function ($) {
    $.fn.extend({

        fileUpload: function (options) {

            var defaults = {
                form: null,
                actionURL: null,
                success: function () { },
                error: function () { }
            };

            var options = $.extend(defaults, options);

            return this.each(function () {

                $('<iframe />', {
                    id: 'upload_iframe',
                    name: 'upload_iframe',
                    style: 'width:0; height:0; border:none;'
                }).appendTo('body');

                var form = $(options.form);
                var url = options.actionURL+(options.actionURL.indexOf("?")>=0?"&":"?")+"iframe=true";
                form.attr("action", url);
                form.attr("method", "post");
                form.attr("enctype", "multipart/form-data");
                form.attr("encoding", "multipart/form-data");
                form.attr("target", "upload_iframe");
                form.submit();


                $("#upload_iframe").load(function () {
                    html = $($("#upload_iframe")[0].contentWindow.document).text();
                    var response = JSON.parse(html);
                    
                    response.success ? options.success.call(this, response) : options.error.call(this, { responseText: JSON.stringify(response) });

                    $("iframe#upload_iframe").remove();

                });


            });
        }
    });
})(jQuery);