
(function (window) {

    var Backbone = window.Backbone,
        _ = window._,
        $ = window.$;
    // Backbone namespace
    Backbone.Res = {        
        enable: function (model, config) {
            /// <summary>Applies plugin to backbone model instance.</summary>            
            /// <param name="model" type="BackboneModel">The backbone model.</param>
            /// <param name="config" type="object">A configuration object.</param>
            /// <returns type="BackboneModel">self</returns>
            _.extend(model, Backbone.Res.Model);
            model._initialize = model.initialize;
            model.initialize = function (option) {
                this.set("__checked", false);
                this._initialize(option);
            }
            
        }        
    };
    // Backbone res namespace
    Backbone.Res.Model = {
        setType: function (m) {
            $.extend(this.typingObject,m);
        },
        CheckBox: function() {
            var id = this.id || this.cid;
            var data_state = !!this.isNew() ? "client" : "server";
            var bucket = "default";
            return "<input type='checkbox' data-bucket='{2}' data-state='{1}' value='{0}' />".format(id, data_state, bucket);
        }
    };
    // Backbone namespace
    Backbone.ResCollection = {
        enable: function (collection, model, config) {
            /// <summary>Applies plugin to backbone collection instance.</summary>
            /// <param name="collection" type="BackboneCollection">The backbone collection.</param>
            /// <param name="model" type="BackboneModel">The backbone model.</param>
            /// <param name="config" type="object">A configuration object.</param>
            /// <returns type="BackboneCollection">self</returns>
            _.extend(collection, Backbone.Res.Collection);
            collection._initialize = collection.initialize;
            collection.initialize = function(option) {

                this.bind("table:checkbox", function(checkbox) {
                    var bucket = $(checkbox).attr("data-bucket") || "default";
                    var model = this.get($(checkbox).val());
                    if (model) {
                        var buckets = model.get("__checked") || {};
                        buckets[bucket] = $(checkbox).is(":checked");
                        model.set("__checked", buckets);
                    }
                });
                this.bind("table:draw", function(table) {


                    this.each(function(model) {
                        var buckets = model.get("__checked");
                        if (!buckets)
                            return;
                        for (var i in buckets) {
                            console.log(i);
                            if (buckets[i])
                                $("input:checkbox[value='{0}'][data-bucket='{1}']".format(model.id || model.cid, i), table).attr("checked", "checked");
                        }
                    });
                });
                this._initialize(option);
            };
            return this;
        }
    };
    // Backbone res namespace
    Backbone.Res.Collection = {
        setType: function(m) {
            $.extend(this.typingObject, m);
        },
        typing: function () {
        	//if (typeof (this.model) == "undefined") return;
            var _model = (new this.model());
            console.log(this);
            var modelTypeing = _model.typing();

            if ($.isFunction(modelTypeing))
                modelTypeing = modelTypeing();

            return $.extend({}, modelTypeing || {}, this.typingObject);
        },
        search: function (q, pageSize, callback) {
            if (!callback) {
                callback = pageSize;
                pageSize = 10;
            }
            var model = this;
            model.SetTextSearch(q);
            model.setPageSize(pageSize);
            model.fetch({
                success: function (collection, data, result) {
                    callback.apply(this, arguments);
                }
            });
            return this;
        },
        checkedQuery: function(bucket, val) {
            /// <summary>Rigs into checkbox flags and returns slice of collection.</summary>
            /// <param name="bucket" type="string">The string bucket for comparison.</param>
            /// <param name="val" type="bool">The value asserted for the bucket (is it checked or not).</param>            
            /// <returns type="BackboneCollection">collection of models that match the list</returns>            
            if (bucket == undefined && val == undefined) {
                val = true;
                bucket = "default";
            } else if (bucket == undefined) {
                val = bucket;
                bucket = "default";
            }

            var q = {};
            q[bucket] = val;
            return this.filter(function(m) {
                var b = m.get("__checked");
                if (!b)
                    return;
                return b[bucket];
            });
        },
        _queryParams: {},
        setQueryParam: function(param, val) {
            this._queryParams[param] = val;
        },
        removeQueryParam: function(param) {
            this._queryParams[param] = null;
            delete this._queryParams[param];
        },
        getQueryParams: function() {
            return this._queryParams;
        },

        /// <summary>Rigs into checkbox flags and returns slice of collection, also clears the checked values.</summary>
        /// <param name="bucket" type="string">The string bucket for comparison.</param>
        /// <param name="val" type="bool">The value asserted for the bucket (is it checked or not).</param>            
        /// <returns type="BackboneCollection">collection of models that match the list</returns>            
        popChecked: function(bucket, val) {
            var checked = this.checkedQuery(bucket, val);
            //this.set("__checked", false);
            this.each(function(m) { m.set("__checked", false) });
            return checked;
        }
    };
    Backbone.ResCollectionStatic = {
        enable: function (collection) {
            _.extend(collection, Backbone.ResCollectionStatic.fields);
        },
        fields:{
            Search: function (q, pageSize, callback) {
                if (!callback) {
                    callback = pageSize;
                    pageSize = 10;
                }
                var model = new this();
                model.SetTextSearch(q);
                model.setPageSize(pageSize);
                model.fetch({
                    success: function (collection, data, result) {
                        callback.apply(this, arguments);
                    }
                });
                return this;
            }
        }
    }
})(this);


/*
====================
Model Functions
====================
*/
Backbone.Res.extensions = {

    ResStoreCollection:{
        SetTextSearch: function (q) {
            this.setSearch(q, ["Name", "LocationNumber"]);
        }
    },
    ResEventCollection: {
        SetTextSearch: function (q) {
            this.setSearch(q, ["Name"]);
        }
    },
    

    ResStore: {
        Link: function () {
            return "http://www.chick-fil-a.com/{0}".format(this.get("MarketableName"));
        },
        Cta: function () {
            //return "[{0}]({1})".format(this.get("Name"),this.Link());
            // carson: changed to open cta in new window
            return "<a href='{1}' target='_blank'>{0}</a>".format(this.get("Name"), this.Link());
        },
        City: function () {
            return this.get("StreetAddress").City;
        },
        CityOrNull: function () {
            return this.get("StreetAddress") ? this.get("StreetAddress").City : "";
        }
        
    },
    ResEvent: {
        Link: function () {
            return "/Admin/Event/Details/{0}".format(this.get("ResEventId"));
        },
        Cta: function () {
            return "[{0}]({1})".format(this.get("Name"), this.Link());
        },
        ViewCta: function () {
            return "[{0}]({1})".format("View Event", this.Link());
        }
    },
    Slot: {
        
        //TicketsAvailable: function () {
        //    return this.get("Capacity") - this.get("TicketsReserved");
        //},
    	Link: function () {
    		return "/Admin/Slot/Details/{0}".format(this.get("SlotId"));
    	},
    	Cta: function () {
    		return "[{0}]({1})".format("View", this.Link());
    	},
    	ViewCta: function () {
    		return "[{0}]({1})".format("View", this.Link());
    	}
    },
    TourSlot: {
        TicketsAvailable: function () {
            return this.get("Capacity") - this.get("TicketsReserved");
        },
        Link: function () {
            return "/Admin/TourSlot/Details/{0}".format(this.get("SlotId"));
        },
        Cta: function () {
            return "[{0}]({1})".format("View", this.Link());
        },
        ViewCta: function () {
            return "[{0}]({1})".format("View", this.Link());
        }
    },
    NPRSlot: {
    	Link: function () {
    		return "/Admin/NPRSlot/Details/{0}".format(this.get("SlotId"));
    	},
    	Cta: function () {
    		return "[{0}]({1})".format("View", this.Link());
    	},
    	ViewCta: function () {
    		return "[{0}]({1})".format("View", this.Link());
    	}
    },
    NPRTicket: {
        Link: function () {
            return "/Admin/NPRTicket/Details/{0}".format(this.get("TicketId"));
        },
        DateArray: function () {
        	var datesToSplit = this.get("DatesString");
        	if( datesToSplit == null ) datesToSplit = "";
            var dats = datesToSplit.split(",");
            if (dats)
            {
				/*
            	return _.sortBy(dats, function (d)
            	{
                    return new Date(d).getTime();
                });
				*/
            }
            return dats;
        },
        DateRequest: function (n) {
            var dates = this.DateArray();
            if (dates && dates.length>n) {
                return dates[n];
            }
            return null;
        },
        DateOne: function () {
            return this.DateRequest(0);
        },
        DateTwo: function () {
            return this.DateRequest(1);
        },
        DateThree: function () {
            return this.DateRequest(2);
        },
        RequestLink: function () {
            return "/Admin/NPRTicket/Request/{0}".format(this.get("TicketId"));
        },
        ViewCta: function () {
            return "[{0}]({1})".format("View", this.Link());
        },
        RequestCta: function () {
            return "[{0}]({1})".format("View", this.RequestLink());
        }
    },
    ReservationType: {
        Link: function () {
            return "/Admin/ReservationType/Details/{0}".format(this.get("ReservationTypeId"));
        },
        Cta: function () {
            return "[{0}]({1})".format(this.get("Name"), this.Link());
        }
    },
    ResUser: {
        Link: function () {
            return "/Admin/User/Details/{0}".format(this.get("ResUserId"));
        },
        Cta: function () {
            return "[{0}]({1})".format(this.get("NameString"), this.Link());
        },
        Name: function () {
            return "{0}".format(this.get("NameString"));
        }
    },
    ResTemplate: {
        Link: function () {
            return "/Admin/ResTemplate/Details/{0}".format(this.get("ResTemplateId"));
        },
        Cta: function () {
            return "[{0}]({1})".format(this.get("Name"), this.Link());
        }
    },
    Media: {
        AdminDeleteLink: function () {
            return "/Admin/Media/Delete/" + this.get("MediaId");
        },
        AdminDeleteCta: function () {
            return "[{0}]({1})".format("Delete", this.AdminDeleteLink());
        },
        AdminDetailsLink: function () {
            return "/Admin/Media/Details/" + this.get("MediaId");
        },
        AdminDetailsCta: function () {
            return "[{0}]({1})".format(this.get("Name"), this.AdminDetailsLink());
        },
        Icon: function () {
            return " <img class='table-icon' src='{0}' width='60px' /> ".format(this.get("MediaUriStr"));
        },
        IconLink: function () {
            return " <a href='{0}' target='_blank'><img class='table-icon' src='{0}' width='60px' /></a> ".format(this.get("MediaUriStr"));
        },
        SetImage: function (target, options) {
            options = options || {};
            $.get("/api/WebMedia/GetS3Thumb/{0}".format(this.get("MediaId")), options, function (d) {
                if ($("img", target).length)
                    $("img", target).attr("src", d);
                else
                    $(target).append("<img src='{0}' />".format(d));
                if (options.success) {
                    options.success.call($("img", target),d);
                }
            });
        }
    }

};



Backbone.Res.staticExtensions = {

    ResStoreCollection: {        
        
    },


    SlotCollection: {
        
        ByStore: function (data, options) {
            options = options || {};
            options.url = "/api/SlotService/ByStore";
            options.sync = Backbone.sync;
            options.data = $.param(data);
            var col = new DomainModel.SlotCollection();
            return col.fetch(options);
        },


        NPRByMonth: function (data, options) {
            options = options || {};
            options.url = "/api/SlotService/NPRByMonth";
            options.sync = Backbone.sync;
            options.data = $.param(data);
            var col = new DomainModel.SlotCollection();
            return col.fetch(options);
        },

    	ByMonth: function (data, options) {
    		options = options || {};
    		options.url = "/api/SlotService/ByMonth";
    		options.sync = Backbone.sync;
    		options.data = $.param(data);
    		var col = new DomainModel.SlotCollection();
    		return col.fetch(options);
    	}
    },
    
    NPRTicketCollection: {
    	BySlot: function (data, options) {
    		options = options || {};
    		options.url = "/api/TicketService/BySlot";
    		options.sync = Backbone.sync;
    		options.data = $.param(data);
    		var col = new DomainModel.NPRTicketCollection();
    		return col.fetch(options);
    	}
    },

    FoodTicketCollection: {
    	GetProductSummary: function (data, options) {
			options = options || {};
			options.url = "/api/FoodTicketProductSummary/GetProductSummary";
			options.sync = Backbone.sync;
			options.async = false;
			options.data = $.param(data);
			var col = new DomainModel.FoodTicketCollection();
			//return col.fetch(options);
			col.fetch(options);
    		return col;
    	}
	}

};

var ProductSummaryCollection = Backbone.Collection.extend({
	//initialize: function (models, options) { }
});


/*
====================
Applied
====================
*/
for (var m in DomainModel) {
    var model = DomainModel[m];
    if (model.__super__) {
        if (model.__super__.add) {
            //collection
            Backbone.ResCollection.enable(model.prototype);
            Backbone.ResCollectionStatic.enable(model);
        } else {
            //model
            Backbone.Res.enable(model.prototype);
        }
        //all
        if (Backbone.Res.extensions[m]) {
            _.extend(model.prototype, Backbone.Res.extensions[m]);
        }

        if (Backbone.Res.staticExtensions[m]) {
            _.extend(model, Backbone.Res.staticExtensions[m]);
        }
    }
};


