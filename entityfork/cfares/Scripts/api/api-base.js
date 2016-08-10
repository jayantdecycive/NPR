var $api_baseUtils = {
    XD: {},
    S4: function () {
        return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
    },
    guid: function () {
        return ($api_baseUtils.S4() + $api_baseUtils.S4() + "-" + $api_baseUtils.S4() + "-" + $api_baseUtils.S4() + "-" + $api_baseUtils.S4() + "-" + $api_baseUtils.S4() + $api_baseUtils.S4() + $api_baseUtils.S4());
    },
    parseHash: function (key) {
        var hash = window.location.hash.replace(/^#/, "");
        var pid;
        if (hash.indexOf("=") != -1) {
            kvps = hash.split("&");

            for (var i = 0; i < kvps.length; i++) {
                var kvp = kvps[i].split("=");
                if (kvp[0] == key) {
                    pid = kvp[1];
                    break;
                }
            }
        } else {
            pid = hash;
        }

        return pid;
    }
};



if (!String.prototype.format) {
    function _StringFormatInline() {
        var txt = this;
        for (var i = 0; i < arguments.length; i++) {
            var exp = new RegExp('\\{' + (i) + '\\}', 'gm');
            txt = txt.replace(exp, arguments[i]);
        }
        return txt;
    }

    String.prototype.format = _StringFormatInline;
}

if (!String.format) {
    function _StringFormatStatic() {
        for (var i = 1; i < arguments.length; i++) {
            var exp = new RegExp('\\{' + (i - 1) + '\\}', 'gm');
            arguments[0] = arguments[0].replace(exp, arguments[i]);
        }
        return arguments[0];
    }
    String.format = _StringFormatStatic;
}

/*END Base Methods*/

/*api_base JS API*/
(function (window) {



    window.$api_base = function (arg) {


        return new $api_base.fn.init(arg);
    };
    var $api_base = window.$api_base;
    $api_base.fn = $api_base.prototype;
    $api_base.fn.net = $api_base.net = {};


    (function ($api_base) {

        /*Base credentials*/
        $api_base.fn.root = $api_base.root = true;
        if ($api_base.loaded)
            return;

        
        $api_base.guid = $api_baseUtils.guid();
        $api_base.fn.init = function (arg) {
            this.target = jQuery(arg);
            this.guid = $api_baseUtils.guid();
            return this;
        }

        $api_base.fn.init.prototype = $api_base.fn;


        $api_base.Call = $api_base.fn.Call = function (url, args) {
            var self;
            if (this.target) {
                self = this.target;
            } else
                self = $api_base;

            args.target = self;

            if (!args.success)
                args.success = function (e) { console.log(e); }



            if (!args.error)
                args.error = function (e) { console.log(e); }

            for (var q in args.data)
                args.data[q] = JSON.stringify(args.data[q]);

            jQuery.ajax({
                type: "GET",
                url: url,
                context: args,
                dataType: "jsonp",
                success: function (e) {
                    this.success.call(this.target, e.d);

                    if (this.success.after) {
                        this.success.after.call(this.target, e.d);
                    };
                },
                error: function (e) { this.error.call(this.target, e); },
                contentType: "application/json; charset=utf-8",
                data: args.data
            });
            return self;
        }

        /*
        * Card - Card and Member Services
        */
        $api_base.Card = $api_base.fn.Card = function () {

            var self = this.root ? this : $api_base;

            var cmd = arguments[0];


            var args
            var service = false;
            if (cmd == "service") {
                service = true;
                cmd = arguments[1];
                args = [].slice.call(arguments, 2);
            } else
                args = [].slice.call(arguments, 1);

            args = $api_base.Card[cmd].apply(self, args);


            var url = $api_base.net.leftSide($api_base.net) + "/Card.asmx/get" + args.cmd + "?format=json";
            if (service)
                return url;

            return $api_base.Call.call(self, url, args);



        };

        $api_base.fn.Card.IsActive = $api_base.Card.IsActive = function (pid, success, error) {


            var args = {
                success: success,
                error: error,
                data: {
                    card: pid
                },
                cmd: "IsActive"
            };
            return (args);
        };

        $api_base.fn.Card.IsActiveOrProfile = $api_base.Card.IsActiveOrProfile = function (pid, success, error) {


            var args = {
                success: success,
                error: error,
                data: {
                    card: pid
                },
                cmd: "IsActiveOrProfile"
            };
            return (args);
        };

        $api_base.fn.Card.IsProfile = $api_base.Card.IsProfile = function (pid, success, error) {


            var args = {
                success: success,
                error: error,
                data: {
                    card: pid
                },
                cmd: "IsProfile"
            };
            return (args);
        };

        $api_base.fn.Card.IsValidReference = $api_base.Card.IsValidReference = function (pid, success, error) {


            var args = {
                success: success,
                error: error,
                data: {
                    store: pid
                },
                cmd: "IsValidReference"
            };
            return (args);
        };

                
        
        if (!window.JSON) {
            (function () {

                var src = ('https:' == document.location.protocol ? 'https://' : 'http://') + 'ajax.cdnjs.com/ajax/libs/json2/20110223/json2.js';

                document.write('<script type="text/javascript" src="' + src + '"></script>');
            })();
        }

        window.onload = (function () {





        });

        $api_base.loaded = true;

    })(window.$api_base);


})(window);

$api_base.net.local = function(){
    $api_base.setNet($api_base.netArray,"local");
}

$api_base.setNet=function(net,target){
    if(!target)
        target="default";
    $api_base.netArray = net;
    $api_base.net=net[target];
    /*NET METHODS*/
    $api_base.net.leftSide = function (self) {
        if (!self)
            self = this;
        return self.protocal + "://" + self.domain + ":" + self.port + "/" + self.base;
    }
    $api_base.net.cdnLeftSide = function (self) {
        if (!self)
            self = this;
        return self.protocal + "://" + self.cdn;
    }
    $api_base.net.contentLeftSide = function (self) {
        if (!self)
            self = this;
        return self.protocal + "://" + self.domain + ":" + self.port + "/";
    }

    $api_base.cdn = $api_base.net.cdnLeftSide($api_base.net);

    $api_base.fn.net = $api_base.net;

}

$api_base.setNet({
    "default":{
        domain : "www.chick-fil-a-list.com",
        port : "80",

    

        base : "Services",
        protocal : "http",
        cdn : "www.chick-fil-a-list.com"
    },
    "local":{    
        domain : "localhost",
        port : "59141",
        base : "Services",
        protocal : "http",
        cdn : "www.chick-fil-a-list.com"
    }
});