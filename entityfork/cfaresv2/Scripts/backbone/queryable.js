/* 
 * backbone.queryable.js v0.9 
 */

(function (window) {
    "use strict";

    // Alias backbone, underscore and jQuery.
    var Backbone = window.Backbone,
        _ = window._,
        $ = window.$;

    // Define the pagination enale method under the Pagination namespace.
    Backbone.Queryable = {

        // Called when enabling pagination on a Backbone.Collection.
        enable: function (collection, model, config) {

            _.extend(collection, Backbone.Queryable.Queryable)

            
            
            if (config) {
                _.extend(collection.paginationConfig, config);
            }
        }
    };

    // Define all the queryable methods available.
    Backbone.Queryable.Queryable = {

        // The current where clause
        _where: null,
        _whereCache:"",

        // Search is always like $or
        setSearch: function (q, fields) {
            var query = { $or: [] };
            _.each(fields, function (field, i) {
                var o = {};
                o[field] = { $like: q };
                query.$or.push(o);
            });
            return this.setWhere("search",query);
        },

        // Set the where clause
        setWhere: function (bucket, where) {
        	if (where === undefined) {
                where = bucket;
                bucket = "default";
            }
            if ($.isArray(where) && where.length == 0)
                where = null;
            else if ($.isPlainObject(where) && _.size(where) == 0)
                where = null;

            var oldWhere = this.getWhere();
            if (!this._where)
                this._where = {};
            this._where[bucket] = where;
            
            if (this._whereCache != this.oDataFilter()) {
                this.trigger("where:change", oldWhere);
            }   
                
            return this._where;
        },
        // Gets the where object
        getWhere: function () {
            if (this.query)
                this._where = this.query;
            this.query = null;
            

            return this._where;
        },
        mongoArrayToData: function (arr,joiner,options,typing) {

            //of format [{},{}]
            var filterArr = [];
            
                var filterSubArr = [];
                var q = arr;
                
                var cleaned_q = [];
                for (var i = 0; i < q.length; i++) {
                    var param = q[i];
                    var c = 0;
                    for (var k in param)
                        c++;
                    if (c > 1) {
                        var arr = [];
                        for (var k in param) {
                            var obj = {};
                            obj[k] = param[k];
                            cleaned_q.push(obj);
                        }                        
                    } else {
                        cleaned_q.push(param);
                    }
                }
                q = cleaned_q;
                for (var i = 0; i < q.length; i++) {
                    var k, val;
                    for (var kk in q[i]) {
                        k = kk;
                        val = q[i][kk];
                        break;
                    }

                    if (q[i].$or || q[i].$and) {
                        var _joiner = !!q[i].$or ? "or" : "and";
                        var toval = q[i]["$" + _joiner];
                        if (!$.isArray(toval))
                            toval = [q[i]["$" + _joiner]];

                        var d = this.mongoArrayToData(toval, _joiner, options, typing);
                        if(d)
                            filterSubArr.push(d);
                    } else {
                        var line = this.formatOdataLine(typing, k, val);
                        if(line)
                            filterSubArr.push(line);
                    }

                    
                }
                if (filterSubArr && filterSubArr.length)
                filterArr.push("({0})".format(filterSubArr.join(" {0} ".format(joiner.toLowerCase()))));

            
            if(filterArr&&filterArr.length)
            return filterArr.join(" {0} ".format(joiner));

        },
        toFilteredJSON: function () {
            var inst = this;
            return _.filter(inst.toJSON(), inst.queryableFilter, inst);
        },
        filteredSlice: function () {
            var inst = this;
            return inst.filter(inst.queryableFilter,inst);
        },
        queryableFilter:function(child){
            if (child.toJSON)
                child = child.toJSON();
            var query = this.getWhere();
            if (!query)
                return true;
            query = this.normalizeQuery(query);
            var model = this;
            

            var joiner = !!query[0].$or ? "or" : "and";

            if (!query[0]["$"+joiner].length)
                return true;
           
            var typing = this.typing;


            typing = typing ? typing.call(this) : null;

            
            
            for (var i = 0; i < query.length; i++) {
                var op = "";
                var q = query[i];
                for (var k in q) {
                    op = k.replace(/^\$/gi, "");
                    q = q[k];
                    break;
                }
                var success = op!="or";
                for (var j = 0; j < q.length; j++) {
                    
                    var statement = q[j];
                    var args = this.readMongo(statement);
                    var key = args[0];
                    if (!child[key])
                        continue;
                    statement = args[1];
                    if (op == "or")
                        success = this.processMongoFilter(child[key], statement) || success;
                    else
                        success = this.processMongoFilter(child[key], statement) && success;
                    
                 
                }
            }
            return success;
        },
        processMongoFilter: function (param, statement) {
            
            if (!$.isPlainObject(statement)) {
                statement = { "$eq": statement };
            }
            var args = this.readMongo(statement);
            if (args[0] == "$not")
                return !this.processMongoFilter(param,args[1]);
            var q = (args[1] && args[1].toLowerCase) ? args[1].toLowerCase() : args[1];
            if (!param)
                return true;
            if (param.toLowerCase)
            param = param.toLowerCase();
            

            switch(args[0].replace(/^\$/gi,"")){
                default:
                case "eq":
                    return param == q;
                case "neq":
                    return param != q;
                case "gt":
                    return q > param;
                case "gte":
                    return q >= param;
                case "lte":
                    return q <= param;
                case "lt":
                    return q < param;
                case "like":
                    if (!param.indexOf)
                        return param == q;

                    if (q.lastIndexOf("%") == 0) {
                        //return q.startswith
                        return param.indexOf(q.replace("%", ""), param.length - q.length) !== -1;
                    } else if (q.indexOf("%") == q.length - 1) {                        
                        return param.indexOf(q.replace("%", "")) == 0;
                    } else {
                        return param.indexOf(q.replace("%", "")) != -1;
                    }
                    return param < q;
            }
            
        },
        readMongo: function (q) {
            for (var i in q) {
                return [i, q[i]];
            }
        },
        normalizeQuery: function (query) {
            if (!query)
                return false;            

            //query is likely kvp of querysets "default","manager","etc" - these automatically get joined by and
            if (!$.isArray(query)) {
                var qarr = [];
                for (var q in query) {
                    qarr.push(query[q]);
                }
                query = qarr;
            }
            //query is a set of arrays that have op as param
            //these arrays are of format {$op:[{param:filter}]}
            var qarr2 = [];
            for (var i = 0; i < query.length; i++) {
                if (!query[i])
                    continue;
                if (!query[i].$or && !query[i].$and) {
                    var q2 = query[i];
                    if (!$.isArray(q2))
                        q2 = [q2];
                    qarr2.push({ $and: q2 });
                } else {
                    qarr2.push(query[i]);
                }
                
            }
            
            return qarr2;
        },
        oDataFilter: function (options) {
            if (!options)
                options = {};
           
            var query = options.where||this.getWhere();
            var model = this;
            query = this.normalizeQuery(query);

            if (!query) {
                this._whereCache = null;
                return null;
            }

            var joiner = !!query[0].$or ? "or" : "and";

            options = options || {};
            var typing = options.typing||this.typing ;


            typing = typing ? typing.call(this) : null;

            var finalArr = [];

            for (var i = 0; i < query.length; i++) {
                var op = "";
                var q = query[i];
                for (var k in q) {
                    op = k.replace(/^\$/gi, "");
                    q = q[k];
                    break;
                }
                var d = this.mongoArrayToData(q, op, options, typing);
                if(d)
                    finalArr.push(d);
            }
            
            
            
            this._whereCache = "{0}".format(finalArr.join(" and "));
            return this._whereCache;

        },
        getBaseType: function (type, key, param) {
            //if(key)
            //  key = key.replace(/Id$/gi, "");
            var isEnum = false;
            if (window.Enum && (Enum[type] || Enum[key.replace(/Id$/gi, "")])) {
                isEnum = Enum[type];
                type = "enum";
                
            } else if (!type) {

            	type = "string";

            	// SH - Numeric values w/o column designations ( for example, manual setWhere clauses )
            	// .. would result in string type searches and subsequently errors .. below fixes
            	for (var k in param)
            		if (param.hasOwnProperty(k)) {
            			if (this.isInt(param[k])) type = "int";
            			break;
            		}
            }
            else {
                type = type.toLowerCase().replace(/[0-9]/gi, "");
            }

            return { type: type, key: key, isEnum: isEnum };
        },
        mongoOpToOdata: function (param,type,not) {
            var op = "";
            if (!$.isPlainObject(param))
            {
            	if ((typeof param) == "string")
            	{
            		param = { "$like": param };
            	}

            	else
            	{
            		param = { "$eq": param };
            	}
            } 

            for (var i in param) {
                op = i;
                param = param[i];
                break;
            }

            
            op=op.replace(/^\$/gi,"").toLowerCase();
            switch (op) {
                case "gte":
                    op = "ge";
                    break;
                case "lte":
                    op = "le";
                    break;
                case "neq":
                    op = "ne";
                    break;
                case "not":
                    return this.mongoOpToOdata(param,type,true);
                default:
                    op = op;
                    break;
            }
            return {op: op,param:param,not:not};
        },
        isInt: function (n) {
        	return !isNaN(parseInt(n)) && isFinite(n);
        },
        isNumeric: function(n) {
        	return !isNaN(parseFloat(n)) && isFinite(n);
        },
        formatOdataLine: function (typing, key, param) {
        	var type = typing ? typing[key] : null;
            var baseTypeArgs = this.getBaseType(type,key,param);
            
            var baseType = baseTypeArgs.type;
            var isEnum = baseTypeArgs.isEnum;
            
            var op = this.mongoOpToOdata(param, baseType);
	        var origParam = param;
            param = op.param;
            var not = op.not;
            op = op.op;
            
            param = this.formatOdataValue(typing, key, param, origParam);

            
            if (param==null) {
                return null;
            }

            key = baseTypeArgs.key;

            var result = null;

            if (isEnum) {
                var enumParam = param.replace(/[^0-9_A-Za-z]/gi,"");
                if (isEnum[enumParam] != undefined) {
	                // SH - Operator fix ( original line below - needed for queries other than 'eq' )
                    // return "{0} eq '{1}'".format(key, enumParam);
	                return "{0} {2} '{1}'".format( key, enumParam, op );
                } else {
                    return null;
                }
                
            }

            switch (baseType) {
                
                case "int":
                case "float":
                case "double":
                    if(!isNaN(param))
                        result = "{0} {2} {1}".format(key, param, op);
                    break;
                case "date":
                case "datetime":
                case "datetimeoffset":
                    if (op == "year") {
                        result = "year({0}) eq {1}".format(key, param);
                    }else if (op == "month") {
                        result = "month({0}) eq {1}".format(key, param);
                    } else if (op == "day") {
                        result = "day({0}) eq {1}".format(key, param);
                    } else {
                        result = "{0} {2} {3}{1}".format(key, param, op, baseType);
                    }
                    break;
                case "long":
                    if (!isNaN(param))
                        result = "{0} {2} {1}L".format(key, param, op);
                    break;
                case "bool":
                case "boolean":
                    result = "{0} {2} {1}".format(key, param ? "true" : "false", op);
                    break;
                case "string":
                default:
                    if (op.indexOf("like") >= 0) {
                        param = param.substring(1,param.length-1);
                        var not = op.indexOf("not") >= 0;
                        
                        if (param.lastIndexOf("%") == 0) {
                        	result = "endswith(tolower({0}),tolower('{1}')) eq {2}".format(key, param.replace("%", ""), !not ? "true" : "false");

                        } else if (param.indexOf("%") == param.length - 1) {
                        	result = "startswith(tolower({0}),tolower('{1}')) eq {2}".format(key, param.replace("%", ""), !not ? "true" : "false");
                        } else {
                        	result = "indexof(tolower({0}),tolower('{1}')) {2} -1".format(key, param.replace("%", ""), not ? "eq" : "gt");
                        }
                    } else {
                        result = "{0} {2} {1}".format(key, param, op);
                    }
                    break;
            }
            if (not)
                return "not ({0})".format(result);
            else
                return result;
        },
        formatOdataValue: function (typing, key, param, origParam) {
            if (param == null)
                return "null";

            var type = typing ? typing[key] : null;
            var baseType = this.getBaseType(type, key, origParam);
            key = baseType.key;
            type = baseType.type;

            switch (type) {
                case "int":
                case "float":
                case "double":                    
                    return "{0}".format(param);
                case "date":
                case "datetime":
                case "datetimeoffset":
                    if (param.getTime)
                        return "'{0}'".format(param.toString("yyyy-MM-ddThh:mm:ss"));
                    else if (!isNaN(param))
                        return param;
                    else
                        return "'{0}'".format(param);
                case "long":
                    return "{0}L".format(param);
                case "bool":
                case "boolean":
                    return "{0}".format(param ? "true" : "false");
                case "string":
                default:
                    return "'{0}'".format(param);
            }
        }

    }
        

    // Provide a PaginatedCollection constructor that extends Backbone.Collection.
    Backbone.QueryableCollection = Backbone.Collection.extend(Backbone.Queryable.Queryable);

})(this);