var ModelTools = {
    /*===JQUI===*/
    AutoCompleteSelect: function (e, data) {



    },
    local: false,
    /*===Backbone===*/
    Sync: function (method, model, options) {
        
        if (options.sync)
            return options.sync(method, model, options);
        if (!ModelTools.local)
            return ModelTools.ODataSyncModel(method, model, options);

    },
    CollectionSync: function (method, model, options) {
        
        if (options.sync)
            return options.sync(method, model, options);
        if (!ModelTools.local)
            return ModelTools.ODataSyncCollection(method, model, options);

    },
    ApplyAllAssociations: function () {

        var associations = this.associations;
        for (var modelKey in associations) {
            var thisKey = associations[modelKey].ThisKey;
            var thisModel = associations[modelKey].Model;

            var pk = this.get(thisKey);
            if (pk) {
                var pkArgs = {};
                var idAttribute = (new DomainModel[thisModel]()).idAttribute;

                if (idAttribute) {

                    pkArgs[idAttribute] = pk;
                    if (!this.attributes)
                        this.attributes = {};

                    this.attributes[modelKey] = new DomainModel[thisModel](pkArgs);
                }

            }
        }
    },
    ApplyAssociation: function (model, value) {
        console.log(arguments);
        console.log(this);
        var pkArgs = {};
        var idAttribute = (new DomainModel[this.associated]()).idAttribute;
        if (idAttribute) {
            pkArgs[idAttribute] = value;

            this.model.attributes[this.modelKey] = new DomainModel[this.associated](pkArgs);
        }

    },
    InitializeAssociations: function () {
        ModelTools.ApplyAllAssociations.call(this);
        var associations = this.associations;
        for (var modelKey in associations) {
            var thisKey = associations[modelKey].ThisKey;
            var thisModel = associations[modelKey].Model;

            this.on("change:" + thisKey, ModelTools.ApplyAssociation, { model: this, modelKey: modelKey, associated: thisModel, thisKey: thisKey });
        }
    },

    GetKeySuffix: function (keyType, key) {
        var keyTypeArgs = keyType.split(':');
        var formatTemplate;
        switch(keyTypeArgs[0]){
            case "string":
                formatTemplate = "('{0}')";
                if (keyTypeArgs.length > 1) {
                    key = String.pad(key,Number(keyTypeArgs[1]));
                }
                break;
            case "long":
            default:
                formatTemplate = "({0}L)";
                break;
        }

        return formatTemplate.format(key);
    },

    ProcessRow: function (method, result, model) {
        if (result) {
            var data = result.d || result;
            if (!data) {
                var props = (
                function (a) {
                    var props = [];

                    for (i in a)
                        if (a.hasOwnProperty(i))
                            props.push(i);

                    return props;
                }
                )(data);
                if (props.length == 1) {
                    data = result[props[0]];
                }
            }

            if (data.length) {
                for (var i = 0; i < data.length; i++) {
                    data[i] = ModelTools.FixDate(data[i]);

                }
                console.log("DEBUG HERE");
                //model.add(result.d, { silent: true });
            } else {
                result.d = ModelTools.FixDate(result.d);

                //model.set(result.d, { silent: true });
            }

        }
        if (data) {
            for (var i in data) {
                if (i.indexOf("__") == 0 || i.indexOf("odata") == 0)
                    delete data[i];
            }
            if (method != 'read')
                model.set(data);
        }
        //if (model.currentPage != null && model.currentPage != undefined)
        //  data.__page = model.currentPage;

        return data;
    },

    ODataSyncModel: function (method, model, options) {
        
        var args = this.ODataSync(method, model, options);
        var url = args[0];
        var jsonp = args[2];
        var requestType = args[3];
        var self = this;
        args = args[1];
        $.ajax({
            dataType: (jsonp ? "jsonp" : "json"),
            url: url,
            type: requestType,
            beforeSend: ModelTools.LegacyMerge,
            data: args ? JSON.stringify(args) : null,
            contentType: "application/json",
            success: function (result) {
                console.log(result);
                var data = self.ProcessRow(method, result, model);
                model.set(data);
                options.success(model);

            },
            error: function (result) {
                options.error('' + result.statusText);
            }

        });
    },

    ODataSyncCollection: function (method, model, options) {
        
        var args = this.ODataSync(method, model, options, model.model.prototype);
        var url = args[0];
        var jsonp = args[2];
        var requestType = args[3];
        var self=this;
        args = args[1];

        if (method == 'read'&&model.pullCache && model.pullCache()) {
            //return options.success(model.pullCache())
        }

        

        $.ajax({
            dataType: (jsonp ? "jsonp" : "json"),
            url: url,
            type: requestType,
            beforeSend: ModelTools.LegacyMerge,
            data: args ? JSON.stringify(args) : null,
            contentType: "application/json",
            success: function (result) {
                
                if (result["odata.count"] && model.setCount)
                    model.setCount(Number(result["odata.count"]));
                var data = result.value || result.d;
                for (var i = 0; i < data.length; i++) {
                    data[i] = self.ProcessRow(method, data[i], model);
                }
                var m = model.abstract ? "update" : "reset";
                var remove = !model.abstract;
                options.success(model,method != 'read' ? model : data,  {update:m,remove:remove});
                model.trigger("fetch",[data]);
            },
            error: function (result) {
                options.error('' + result.statusText);
            }

        });
    },

    ODataSync: function (method, singleModel, options, collectionModel) {

        var query = singleModel.viewUrl||(collectionModel ? collectionModel.urlRoot : singleModel.urlRoot);
        var model = collectionModel || singleModel;
        if ($.isFunction(query))
            query = query(method);
        //console.log(query);
        if (collectionModel && method != "read")
            return null;

        var requestType = "GET";
        var args;

        if (method != "create" && !collectionModel) {
            query = query + ModelTools.GetKeySuffix(model.idAttributeType,model.id);
        }

        if (method != "read" && method != "delete") {
            args = model.toJSON();
        }

        switch (method) {
            case 'create': requestType = "POST"; break;
            case 'update': requestType = ModelTools.LegacyMergeType(); break;
            case 'delete': requestType = "DELETE"; break;
            case 'read': default: requestType = "GET"; break;
        }

        var jsonp = false;

        var url = query + "";

        if (model.url && typeof (model.url) == 'string')
            url = model.url;

        if (jsonp) {
            args = args || {};
            args.format = "json";
            //args.callback= "displayResults";
        }

        if ((method == "create" || method == "update") && model.associations) {
            for (var i in model.associations) {
                if (args[i])
                    delete args[i];
            }
        }

        
        if (singleModel.paginator) {
            var poptions = { url: url };
            
            $.extend(poptions, options);
            
            url = singleModel.paginator(poptions);
        }
        
        
            
        var targetModel = singleModel;

        
        if (options.where && targetModel.setWhere) {            
            targetModel.setWhere(options.where);
        }
        if (options.order && targetModel.setOrder) {
            targetModel.setOrder(options.order);
        }

        if (targetModel.oDataFilter) {
            var where = targetModel.oDataFilter();
            if (where && where.replace(/[\(\)\s]/gi, ""))
                url = url + ((url.indexOf('?') === -1) ? '?' : '&') + "$filter={0}".format(encodeURIComponent(where));
        }
        if (targetModel.oDataOrder) {
            var order = targetModel.oDataOrder();
            if (order && order.replace(/[\(\)\s]/gi, ""))
                url = url + ((url.indexOf('?') === -1) ? '?' : '&') + "$orderby={0}".format(encodeURIComponent(order));
            
            var altorder = targetModel.oDataOrder({ alt: true });
            if (altorder && altorder.replace(/[\(\)\s]/gi, ""))
                url = url + ((url.indexOf('?') === -1) ? '?' : '&') + "orderby={0}".format(encodeURIComponent(altorder));
        }
        
        if (targetModel.getQueryParams && $.param(targetModel.getQueryParams())) {
            url = url + ((url.indexOf('?') === -1) ? '?' : '&') + $.param(targetModel.getQueryParams());
        }
            
        return [url,args,jsonp,requestType];
    },
    
    FixDate: function (obj) {
        for (var i in obj) {
            if (obj[i] && obj[i].search && obj[i].search(/Date\(/) != -1) {
                var dateStr = obj[i].match(/-?\d+/)[0];
                var date = (new Date(Number(dateStr)));

                if (window.Global && window.Global.TimeZoneContext) {

                    //date = date.addHours(window.Global.TimeZoneContextOffset);

                }
                obj[i] = date;
            }
        }
        return obj;
    },

    ODataUrl: function () {
        var query = this.urlRoot;
        if (this.id)
            query = query + "({0}L)".format(this.id);
        return query;
    },
    TestWcf: function () {
        $.ajax({
            dataType: "json",
            url: "/Service/Slot.svc/TestWork",
            type: "POST",
            data: JSON.stringify({ testbool: true }),
            contentType: "application/json",
            success: function (result) {
                console.log("success");
                console.log(result);
            },
            error: function (result) {
                console.log("fail");
                console.log(result);
            }

        });
    },
    TestDelete: function () {
        $.ajax({
            dataType: "json",
            url: "/Service/Slot.svc/TestDelete",
            type: "DELETE",
            data: null,
            contentType: "application/json",
            success: function (result) {
                console.log("success");
                console.log(result);
            },
            error: function (result) {
                console.log("fail");
                console.log(result);
            }

        });
    },
    TestPut: function () {
        $.ajax({
            dataType: "json",
            url: "/Service/Slot.svc/TestPut",
            type: "PUT",
            data: null,
            contentType: "application/json",
            success: function (result) {
                console.log("success");
                console.log(result);
            },
            error: function (result) {
                console.log("fail");
                console.log(result);
            }

        });
    },
    TestMerge: function () {
        $.ajax({
            dataType: "json",
            url: "/Service/Slot.svc/TestMerge",
            type: ModelTools.LegacyMergeType(),
            data: null,
            beforeSend: ModelTools.LegacyMerge,
            contentType: "application/json",
            success: function (result) {
                console.log("success");
                console.log(result);
            },
            error: function (result) {
                console.log("fail");
                console.log(result);
            }

        });
    },
    LegacyMerge: function (xhr, settings) {
        console.log(this);
        console.log(arguments);
        if ($("body").hasClass("flag-ie7") || $("body").hasClass("flag-ie8")) {
            xhr.setRequestHeader("X-HTTP-Method-Override", 'MERGE');
        }
    },
    LegacyMergeType: function () {

        if ($("body").hasClass("flag-ie7") || $("body").hasClass("flag-ie8")) {
            return "POST";
        }
        return "MERGE";
    }
};