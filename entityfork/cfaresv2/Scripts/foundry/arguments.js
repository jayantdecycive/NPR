var $arg = $arg || {};
(function ($arg, $) {
    //Sample use: $arg.go(arguments)

    $arg.go = function (args) {
        var isObject = $.isObject || $.isSimpleObject;
        var isNumber = $.isNumber || $.isNumeric;
        var isBoolean = $.isBoolean || function (o) {
            return typeof o === 'boolean';
        };
        var results = { f: [],a:[],s:[],n:[],o:[],b:[]};

        for (var i = 0, j = args.length; i < j; i++) {
            var arg = args[i];

            if ($.isFunction(arg))
                results.f.push(arg);
            else if (isObject(arg))
                results.o.push(arg);
            else if ($.isArray(arg))
                results.a.push(arg);
            else if ($.isString(arg))
                results.s.push(arg);
            else if (isBoolean(arg))
                results.b.push(arg);
            else if(isNumber(arg))
                results.n.push(arg)
            
            results[j]=(arg);

        }
        return results;
    };
})($arg,jquery||_);