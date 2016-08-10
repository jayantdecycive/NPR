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
    Backbone.Orderable = {

        // Called when enabling pagination on a Backbone.Collection.
        enable: function (collection, model, config) {

            _.extend(collection, Backbone.Orderable.Orderable)

            if (!collection.comparator)
            collection.comparator = collection.orderableComparator;

            if (config) {
                _.extend(collection.orderableConfig, config);
            }
        }
    };

    // Define all the pagination methods available.
    Backbone.Orderable.Orderable = {
        // The current page displayed -- defaults to page 1.
        _order: [],
        _orderCache: "",

        orderableComparator: function(attr) {
            var order = this.getOrder();
            var typing = this.typing||(this.model ? (new this.model()).typing : null) || this.typing;
            if ($.isFunction(typing)) {
                typing = typing.call(this);
            }
            //TODO:order
            if (!$.isArray(order)) {
                order = [order];
            }
            var weight = 0;
            for (var i = 0; i < order.length; i++) {
                var power = (order.length) - i;
                var key, keyOrder;
                for (var k in order[i]) {
                    key = k;
                    keyOrder = order[i][k];
                    break;
                }
                var modelValue = attr.get(key);
                if (!modelValue) {
                    continue;
                }
                
                var baseType = typing[key] || (isNaN(modelValue) ? "string" : "int");
                console.log(typing);
                console.log(key);
                console.log(baseType);
                baseType = baseType.replace(/[0-9]/gi, "").toLowerCase();
                
                switch (baseType) {
                case "int":
                case "long":
                case "number":
                case "float":
                case "short":
                    break;
                case "datetime":
                case "datetimeoffset":
                case "date":
                        
                        modelValue = (new Date(modelValue)).getTime();
                    break;
                default:
                    modelValue = modelValue.toLowerCase();
                    modelValue = modelValue.split("");
                    var c = modelValue.length;
                    modelValue = _.map(modelValue, function(letter) {
                        return letter.charCodeAt(0);
                    });
                    var modelValueWeight = 0;
                    for (var j = 0; j < modelValue.length; j++) {
                        modelValueWeight += modelValue[j] / (Math.max(512 * j, 1));
                    }
                    modelValue = modelValueWeight;
                    break;
                }

                weight += (modelValue * keyOrder) * (100000 << power);
            }
            return weight;
        },
        // Set the page number given.
        setOrder: function(order) {
            var oldOrder = this.getOrder();
            this._order = order;

            this.order = null;

            if (this._orderCache != this.oDataOrder()) {
                this.trigger("order:change", oldOrder);

            }

            return this._order;
        },
        getOrder: function() {
            if (this.order)
                this._order = this.order;
            this.order = null;
            if (!$.isArray(this._order)) {
                this._order = [this._order];
            }

            return this._order;
        },
        oDataOrderLine: function(key, val) {
            return "{0} {1}".format(key, val == 1 ? "asc" : "desc");
        },
        oDataOrder: function(options) {

            
            if (!options)
                options = {};
            var alt = !!options.alt;
            var order = options.order || this.getOrder();
            if (!order)
                return false;
            var model = this;

            if (order.$orderby)
                order = order.$orderby;

            var orderLines = [];
            if (!$.isArray(order)) {
                order = [order];
            }
            for (var i = 0; i < order.length; i++) {
                for (var k in order[i]) {
                    if (alt == !!order[i].alt)
                        orderLines.push(this.oDataOrderLine(k, order[i][k]));
                    break;
                }
            }

            this._orderCache = orderLines.join(",");

            return this._orderCache;

        }
    };
    // Provide a PaginatedCollection constructor that extends Backbone.Collection.
    Backbone.OrderableCollection = Backbone.Collection.extend(Backbone.Orderable.Orderable);

})(this);