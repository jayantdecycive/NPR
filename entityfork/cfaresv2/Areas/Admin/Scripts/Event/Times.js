

(function (global) {
    /*
    ==========Declaration==========
    */
    var ViewModel = {}, View = {};
    window.Model = window.Model || {};
    /*
    -----------Models-----------
    */
    window.toDelete = [];

    var siteMode = "date";

    var SlotAggregate = DomainModel.Slot.extend({
        initialize: function () {
            this.bind("destroy", this.onDestroy, this);
            this.bind("create", this.onCreate, this);
            this.bind("change", this.onChange, this);
            this.constructor.__super__.initialize.apply(this, arguments);
            this.trigger("create",this);
        },
        SetStartTime: function (time,silent) {
            this.reconcileTime(time, "Start", silent);
        },
        reconcileTime: function (time, key, silent) {
            var aggregate = this.get("parent");
            var aggregateDate = aggregate.get("value");
            var date = aggregateDate.getDate();
            var month = aggregateDate.getMonth() + 1;
            var year = aggregateDate.getFullYear();

            var newDate = new Date("{0}/{1}/{2} {3}".format(month, date, year, time));
            var toSet = {};
            toSet[key] = newDate;
            //, 
            this.set(toSet, { silent: silent });
        },
        SetEndTime: function (time, silent) {
            this.reconcileTime(time, "End", silent);
        },
        getPlucked: function () {
            var m = this;
            var start = m.get("Start");
            var end = m.get("End");
            var aggregate = m.get("parent");
            var oldPlucked = _.clone(m.get("OldAggregate"));
            if (oldPlucked) {
                oldPlucked.StartMonth;// += 1;
                oldPlucked.EndMonth;// += 1;
            }
            
            if (aggregate.mode == "dow")
                dayOffset = 1;
            return {
                NewValue: {
                    StartDay: aggregate.GetDay(),
                    StartYear: aggregate.GetYear(),
                    StartMonth: aggregate.GetMonth()-1,
                    StartMinute: start.getMinutes(),
                    StartHour: start.getHours(),
                    EndDay: aggregate.GetDay(),
                    EndYear: aggregate.GetYear(),
                    EndMonth: aggregate.GetMonth()-1,
                    EndMinute: end.getMinutes(),
                    EndHour: end.getHours(),
                    Capacity: m.get("Capacity")
                },
                OldValue:oldPlucked
            };
        },
        onChange: function () {
            $(document).trigger("refresh.viewmodel");
        },
        onDestroy: function (model) {
            if (this.get("OldAggregate")) {
                var pluck = { Aggregates: [this.getPlucked()] };
                var m = this.get("parent");
                pluck.Grouping = m.GroupingId(),
                pluck.BaseDate = m.get("value");
                
                window.toDelete.push(pluck);
                
            }
            $(document).trigger("refresh.viewmodel");
        },
        onCreate: function () {
            
        }
    });
    var SlotAggregateCollection = DomainModel.SlotCollection.extend({
        initialize: function () {
            this.bind("add", this.onAdd, this);
            this.bind("remove", this.onRemove, this);
        },
        onAdd: function () {
            $(document).trigger("refresh.viewmodel");
        },
        onRemove: function () {
            $(document).trigger("refresh.viewmodel");
        },
        pluckAggregate: function () {
            return this.map(function (m) {
                return m.getPlucked();
            });
        },
        
        model: SlotAggregate
    });

    var SlotAggregateDate = Backbone.Model.extend({
            initialize: function () {
                this.set("slots", new SlotAggregateCollection());
                this.bind("destroy", this.onDestroy, this);
            },
            onDestroy: function () {
                this.get("slots").each(function (m) {
                    m.destroy();
                });
            },
            GroupingId: function () {
                switch(this.get("mode").toLowerCase()){
                    case "all":
                        return 2;
                    case "dow":
                        return 1;
                    case "date":
                        default:
                        return 0;
                }
            },
            GetDay: function () {
                var value = this.get("value");
                var mode = this.get("mode");                
                if (mode == "date")
                    return value.getDate();
                if (mode == "dow")
                    return value.getDay();
                return 1;
            },
            GetYear: function () {
                var value = this.get("value");
                var mode = this.get("mode");
                if (mode == "date")
                    return value.getFullYear();
                return 1989;
            },
            GetMonth: function (forServer) {
                var value = this.get("value");
                var mode = this.get("mode");
                if (mode == "date") {
                    if (forServer)
                        return value.getMonth() - 1;
                    else
                        return value.getMonth();
                }
                return 0;
            },
            defaults: {
                mode: "all",
                value: 1,
                orderBy:0,
                label: "All",
                slots: []
            }
        });
    var SlotAggregateDateCollection = Backbone.Collection.extend({
        initialize: function () {
            this.bind("add", this.onAdd, this);
            this.bind("remove", this.onRemove, this);
        },
        onAdd: function () {
            $(document).trigger("refresh.viewmodel");
        },
        onRemove: function () {
            $(document).trigger("refresh.viewmodel");
        },
        pluckAllAggregates: function () {
            return this.map(function (m) {
                    return {
                        Aggregates: m.get("slots").pluckAggregate(),
                        Grouping: m.GroupingId(),
                        BaseDate:m.get("value")
                    };
            });
        },
        model: SlotAggregateDate,
        comparator: function (modelA, modelB) {
            return modelA.get("orderBy") - modelB.get("orderBy");
        }
    });

        
                
    

    /*
    -----------Collections-----------
    */    
    var UpdatingCollectionView = Backbone.View.extend({
        initialize : function(options) {
            _(this).bindAll('add', 'remove');
 
            if (!options.childViewConstructor) throw "no child view constructor provided";
            if (!options.childViewTagName) throw "no child view tag name provided";
 
            this._childViewConstructor = options.childViewConstructor;
            this._childViewTagName = options.childViewTagName;
 
            this._childViews = [];
 
            this.collection.each(this.add);
 
            this.collection.bind('add', this.add);
            this.collection.bind('remove', this.remove);
            
            
        },
 
        add : function(model) {
            var childView = new this._childViewConstructor({
                tagName : this._childViewTagName,
                model : model
            });
            var index = model.collection.indexOf(model);
            
            this._childViews.splice(index,0,childView);
 
            this.trigger("add:collection:pre", this.el, childView);
            if (this._rendered) {
                

                if (index === 0) {
                    $(this.el).prepend(childView.render().el);
                } else if (index === this._childViews.length-1) {
                    $(this.el).append(childView.render().el);
                } else {
                    var el = $(this.el).children()[index-1];
                    $(childView.render().el).insertAfter(el);
                }
                this.trigger("add:model", this.el, childView);

            }
            this.trigger("add:collection:post", this.el);
        },
 
        remove : function(model) {
            var viewToRemove = _(this._childViews).select(function(cv) { return cv.model === model; })[0];
            this._childViews = _(this._childViews).without(viewToRemove);
 
            if (this._rendered) $(viewToRemove.el).remove();
            this.trigger("remove:model", this.el, viewToRemove.el);
            if(this._childViews.length==0)
                this.trigger("remove:collection", this.el);
        },
 
        render : function() {
            var that = this;
            this._rendered = true;
 
            $(this.el).empty();
 
            _(this._childViews).each(function(childView) {
                $(that.el).append(childView.render().el);
            });
            
            this.trigger("render", this.el);
            return this;
        }
    });

    var UpdatingCollectionAccordionView = UpdatingCollectionView.extend({
        add: function (model) {
            var childView = new this._childViewConstructor({
                tagName: this._childViewTagName,
                model: model
            });
            var index = model.collection.indexOf(model);

            this._childViews.splice(index, 0, childView);

            this.trigger("add:collection:pre", this.el, childView);
            if (this._rendered) {


                if (index === 0) {
                    $(this.el).prepend(childView.render().el);
                } else if (index === this._childViews.length - 1) {
                    $(this.el).append(childView.render().el);
                } else {
                    var el = $(this.el).children()[index * 2 - 1];
                    $(childView.render().el).insertAfter(el);
                }
                this.trigger("add:model", this.el, childView);

            }
            this.trigger("add:collection:post", this.el);
        }
    });

    var SlotAggregateDateSummaryView = Backbone.View.extend({
        template: _.template($("#slot_summary_template").html()),
        events: {
            "click .remove-summary": "remove"
        },
        render: function () {
            var self = this;
            $(self.el).empty();
         
            $(this.template(this.model.toJSON())).appendTo(self.el);
            Page.init(self.el);
            
            //$(self.el).addClass("inline-block");
            return this;
        },
        remove: function () {
            this.model.destroy();
        }
    });


        var SlotAggregateDateView = Backbone.View.extend({
            
            template: _.template($("#accordion_slot_template").html()),
            events:{
                "click .add-timeslot": "add"
            },
            render: function () {
                var self = this;

               

                $(self.el).empty();

                $(this.template(this.model.toJSON())).appendTo(self.el);
                self.SlotAggregateCollectionView.el = $(".slot-rows", $(self.el));
                                
                self.SlotAggregateCollectionView.render();
                Page.init(self.el);
                //console.log(self);
                
                return this;
            },
            cleanup: function () {
                this.$el.append(this.$el.prev("h3"));
            },
            onModelRemove: function () {
                
                if (this.model.get("slots").size() == 0 && this.model.get("mode")=="date") {
                    
                    this.cleanup();
                    this.model.destroy();
                    //this.remove();
                }
            },
            onModelDestroy: function () {

                
                this.cleanup();
                
                this.remove();
                
            },
            initialize: function () {
                this.SlotAggregateCollectionView = new UpdatingCollectionView({
                    collection: this.model.get("slots"),
                    childViewConstructor: SlotAggregateView,
                    childViewTagName: 'li'
                });
                
                this.model.get("slots").bind("remove", this.onModelRemove, this);
                this.model.bind("destroy", this.onModelDestroy, this);
            },
            add: function () {
                var model = new SlotAggregate({parent:this.model});
                console.log("add");
                this.model.get("slots").add(model);
                $("#slot_accordion").accordion("refresh");
                

            }
        });

       /* var SlotAggregateCollectionView = Backbone.View.extend({
                aggregateDateViews: [],                
                events: {
                    'click .remove-slot': 'remove'                    
                },
                render: function () {
                    var self = this;            
                    return this;
                },
                initialize: function () {
                    this.listenTo(this.collection, "add remove", this.render);            
                }
            });*/

                var SlotAggregateView = Backbone.View.extend({
                    template: _.template($("#slot_row_template").html()),
                    events: {
                        'click .remove-slot': 'removeClick',
                        "change input": "changed"
                    },
                    changed: function (evt) {
                        var changed = $(evt.currentTarget);
                        var val = changed.val();
                        if (changed.hasClass("in-capacity")) {
                            this.model.set("Capacity", new Number(val));
                        } else {
                            //if timepicker is out, it'll get messed up if focus changes
                            var silent = !!$(".ui-timepicker-div:visible").length;
                            if (changed.hasClass("in-start")) {
                                this.model.SetStartTime(val, silent);
                            } else if (changed.hasClass("in-end")) {
                                this.model.SetEndTime(val, silent);
                            }
                        }
                        //this.model.trigger("change");
                    },
                    removeClick: function () {
                        this.model.destroy();
                    },
                    render: function () {
                        var self = this;
                        $(self.el).empty();
                        var model = this.model.toJSON();
                        model.cid = this.model.id || this.model.cid;
                        $(this.template(model)).appendTo(self.el);
                        Page.init(self.el);
                        
                        return this;
                    },
                    onModelRemove: function () {
                        //this.remove();
                    },
                    initialize: function () {
                        this.listenTo(this.model, "change", this.render);
                        this.listenTo(this.model, "remove", this.onModelRemove);
                    }
                });

    

    /*
    ============Model============
    */
    


    /*
    ============Views============
    */
    Views = {};

    


    /*
    ============Controller============
    */



    /*
    ============jQuery============
    */
    $(document).bind("render", function () {

        /*MDRAKE - HERE IS WHERE YOU WOULD LOAD IN DATA*/

        ViewModel.dateAggregates = new SlotAggregateDateCollection();
        
        //var slotAggsDate = new SlotAggregateCollection();
        var configs = [
            { key: "Date", mode: "date", label: function (date) { return date.toString("dddd, MMMM d, yyyy"); } },
            { key: "Day", mode: "dow", label: function (date) { return date.toString("dddd"); } },
            { key: "All", mode: "all", label: function (date) { return "All"; } }
        ];
        var alled = false;
        _.each(configs, function (config) {
            console.log(config);
            if (CurrentAggregates)
            _.each(CurrentAggregates[config.key], function (serverModel) {
                var aggDate = new Date(serverModel.StartYear, serverModel.StartMonth-1 , serverModel.StartDay);
                var startDate = new Date(serverModel.StartYear, serverModel.StartMonth-1 , serverModel.StartDay, serverModel.StartHour, serverModel.StartMinute);
                var endDate = new Date(serverModel.EndYear, serverModel.EndMonth+1 , serverModel.EndDay, serverModel.EndHour, serverModel.EndMinute);
                var aggModel = {};
                aggModel.mode = config.mode;

                /*if (aggModel.mode != "date") {
                    aggDate = new Date(serverModel.StartYear, serverModel.StartMonth, serverModel.StartDay );
                    startDate = new Date(serverModel.StartYear, serverModel.StartMonth, serverModel.StartDay , serverModel.StartHour, serverModel.StartMinute);
                    endDate = new Date(serverModel.EndYear, serverModel.EndMonth, serverModel.EndDay , serverModel.EndHour, serverModel.EndMinute);
                }*/

                aggModel.label = config.label(aggDate);

                var orderBy;
                if (config.mode == "all") {
                    orderBy = 0;
                    alled = true;
                } else if (config.mode == "dow") {
                    orderBy = startDate.getDay();
                } else {
                    orderBy = startDate.getTime();
                }

                var aggregate = ViewModel.dateAggregates.where(aggModel);
                
                if (!aggregate || !aggregate.length) {
                    aggregate = new SlotAggregateDate(aggModel);
                    ViewModel.dateAggregates.add({ label: aggModel.label, mode: aggModel.mode, value: aggDate, orderBy: orderBy });
                }
                aggregate = ViewModel.dateAggregates.where(aggModel);                
                aggregate = aggregate[0];
                
                var slotModel = {
                    Start: startDate,
                    End: endDate,
                    Capacity: serverModel.Capacity,
                    parent: aggregate
                };

                var oldModel = {
                    StartDay: aggregate.GetDay(),
                    StartYear: aggregate.GetYear(),
                    StartMonth: aggregate.GetMonth(),
                    StartMinute: slotModel.Start.getMinutes(),
                    StartHour: slotModel.Start.getHours(),
                    EndDay: aggregate.GetDay(),
                    EndYear: aggregate.GetYear(),
                    EndMonth: aggregate.GetMonth(),
                    EndMinute: slotModel.End.getMinutes(),
                    EndHour: slotModel.End.getHours(),
                    Capacity: slotModel.Capacity
                };
                
                

                var slotAggregate = new SlotAggregate(slotModel);
                slotAggregate.set("OldAggregate", oldModel);
                var aggregateSlotCollection = aggregate.get("slots");
                aggregateSlotCollection.add(slotAggregate);
            });
        });

        if (!alled) {

            //bug here with slots being automatically added
            var model = {};
            model.mode = "all";
            model.value = (new Date());
            model.label = "All";
            model.orderBy = 0;
            ViewModel.dateAggregates.add(model);
        }
        for (var i = 1; i <= 7;i++) {
            var d = new Date("1/{0}/1989".format(i));
            var model = {label:d.toString("dddd"),mode:"dow"};
            var aggregate = ViewModel.dateAggregates.where(model);
            var orderBy = d.getDay();
            if (!aggregate || !aggregate.length) {
                aggregate = new SlotAggregateDate(model);
                ViewModel.dateAggregates.add({ label: model.label, mode: model.mode, value: d, orderBy: orderBy });
            }
        }

        /*MDRAKE - HERE IS WHERE YOU WOULD LOAD IN DATA*/

        Views.dateAggregates = new UpdatingCollectionAccordionView({
            collection: ViewModel.dateAggregates,
            childViewConstructor: SlotAggregateDateView,
            childViewTagName: 'div',
            el: $("#slot_accordion")[0]
        });

        Views.dateSummaryAggregates = new UpdatingCollectionView({
            collection: ViewModel.dateAggregates,
            childViewConstructor: SlotAggregateDateSummaryView,
            childViewTagName: 'li',
            el: $("#date_summary_list")[0]
        });
        

        

        Views.dateAggregates.bind("add:collection:pre", function (el) {
        	
        	var isAccordion = !!$("#slot_accordion").data("ui-accordion");
        	if (isAccordion) { $("#slot_accordion").accordion("destroy"); }  //rclark: commented out due to error thrown on page load
        	
        });

        Views.dateAggregates.bind("add:collection:post", function (el) {

            var pre = $(el).children("div");
            pre.each(function () {
                $(this).children("h3").insertBefore(this);
            });
            $("#slot_accordion").accordion({ /*autoHeight: false*/ heightStyle: "content" });
        });

        Views.dateAggregates.bind("render", function (el) {
            Views.dateAggregates.trigger("add:collection:pre", el);
            Views.dateAggregates.trigger("add:collection:post", el);
        });

        Views.dateAggregates.render();
        Views.dateSummaryAggregates.render();

    });

    $(document).bind("redraw", function () {
        if (siteMode == "all") {
            $("#button_group_week").hide();
        } else {
            $("#button_group_week").show();
        }
        $("*[data-mode]").filter("*[data-mode!='{0}']".format(siteMode)).each(function () {
            console.log("FILTERING UNUSED");
            if ($(this).hasClass("ui-accordion-header")) {
                $(this).next().slideUp("FAST");
                $(this).slideUp("FAST");
            } else {
                $(this).parent().slideUp("FAST");
            }
        });
        $("*[data-mode]").filter("*[data-mode='{0}']".format(siteMode)).each(function () {
            if ($(this).hasClass("ui-accordion-header")) {
                //$(this).next().show();
                $(this).slideDown("SLOW", function () {

                    var firstVisible = $(".ui-accordion-header:visible", ".slot-accordion").index(".ui-accordion-header");
                    if ($(this).is(".ui-accordion-header-active"))
                    $(this).next(".ui-accordion-content").show();
                    console.log(firstVisible);
                    $("#slot_accordion").accordion({ active: firstVisible });
                    
                });
            } else {
                $(this).parent().slideDown("FAST");
            }
        });
        
        //$("ui-accordion-header[data-mode]:visible:eq(0)").next().show();


    });

    $(document).bind("init", function () {
        $("#day_style_radios,#all_style_radios").buttonset();
        
        $("input[name='day_style']").bind("change", function() {
            var day_mode = $("input[name='day_style']:checked").val();
            siteMode = siteMode == "all" ? "all" : day_mode;
            $(document).trigger("redraw");
        });
        $("input[name='all_style']").bind("change", function () {
            //day_style
            var all_mode = $("input[name='all_style']:checked").val();
            siteMode = all_mode == "all" ? "all" : $("input[name='day_style']:checked").val();
            $(document).trigger("redraw");
        });

        $("#inline_cal").datepicker({
            onSelect: function (date) {

                date = new Date(date);
                var model = {};
                
                var dayMode = $("input[name='day_style']:checked").val();

                if (siteMode != "date") {
                    return;
                }                


                var label;
                if (dayMode == "dow") {
                    label = date.toString("dddd");
                    model.orderBy = date.getDay();                        
                } else {
                    label = date.toString("dddd, MMMM d, yyyy");
                    model.orderBy = date.getTime();                        
                }
                    
                model.mode = dayMode.toLowerCase();
                model.label = label;
                model.value = date;

                
                var existing = ViewModel.dateAggregates.where({ mode: model.mode, label: model.label });
                if (!existing || !existing.length) {
                    
                    ViewModel.dateAggregates.add(model);
                }

                var firstVisible = $(".ui-accordion-header:visible", ".slot-accordion").index(".ui-accordion-header");
                
                $("#slot_accordion").accordion({ active: firstVisible });

            },
            defaultDate: Model.RegistrationStart,
            beforeShow: function () {

            }
        });

        $(document).trigger("render");
        
    });

    $(document).bind("refresh.viewmodel", function () {

        var aggs = ViewModel.dateAggregates.pluckAllAggregates();
        $("#SaveAggregate").val(JSON.stringify(aggs));
        $("#DeleteAggregate").val(JSON.stringify(toDelete));
    });

    var aggThresholdValidation = function (item, iterator) {
        return !window.CapacityThreshold || _.every(item.Aggregates, function (agg) {
            agg = agg.NewValue;
            return agg.Capacity < window.CapacityThreshold;
        });
    };
    
    var aggTimeSpanValidation = function (item, iterator) {
        return _.every(item.Aggregates, function (agg) {
            agg = agg.NewValue;
            var start = new Date(agg.StartYear, agg.StartMonth, agg.StartDay, agg.StartHour, agg.StartMinute, 0, 0);
            var end = new Date(agg.EndYear, agg.EndMonth, agg.EndDay, agg.EndHour, agg.EndMinute, 0, 0);
            return start.getTime() < end.getTime();
        });
    };
    
    var aggTimeThresholdValidation = function (item, iterator) {
        return !window.Model.RegistrationStart || _.every(item.Aggregates, function (agg) {
            agg = agg.NewValue;
            if (agg.StartYear==1989) {
                return true;
            }
            var start = new Date(agg.StartYear, agg.StartMonth+1, agg.StartDay, agg.StartHour, agg.StartMinute, 0, 0);
            var registrationStart = new Date(window.Model.RegistrationStart.getTime());
            var registrationEnd = new Date(window.Model.RegistrationEnd.getTime());
            registrationStart.setDate(registrationStart.getDate() - 1);
            var end = new Date(agg.EndYear, agg.EndMonth+1, agg.EndDay, agg.EndHour, agg.EndMinute, 0, 0);
            registrationEnd.setDate(registrationEnd.getDate() + 1);
            var toCheck = start.getTime() > window.Model.RegistrationStart.getTime() && new Date(end.getTime()) < new Date(window.Model.RegistrationEnd.getTime()).setHours(23);
            console.log("Checking Slot:");
            console.log("Reg Start: " + new Date(window.Model.RegistrationStart.getTime()));
            console.log("Slot Start: " + new Date(start.getTime()));
            console.log("Slot End: " + new Date(end.getTime()));
            console.log("Reg Start: " + new Date(window.Model.RegistrationEnd.getTime()).setHours(23));
            console.log("\n");
            return toCheck;
        });
    };

    $(function() {

        $("form").submit(function (e) {
            debugger;
            $(document).trigger("refresh.viewmodel");
            var success = true;
            if (toDelete&&toDelete.length) {
                success = confirm("You have decided to delete a large number of slots for this event. Is that ok?");
            }
            if (!_.every(ViewModel.dateAggregates.pluckAllAggregates(), aggThresholdValidation)) {
                success = confirm("You have decided to use a capacity that is bigger than this event's location. Do you want to continue?");
            }
            if (!_.every(ViewModel.dateAggregates.pluckAllAggregates(), aggTimeThresholdValidation)) {
                success = confirm("You have some time slots that are outside of your registration period ({0} to {1}). Are you sure you want to save them?".format(window.Model.RegistrationStart.toString("MM/dd/yyyy"), window.Model.RegistrationEnd.toString("MM/dd/yyyy")));
            }
            if (!_.every(ViewModel.dateAggregates.pluckAllAggregates(), aggTimeSpanValidation)) {
                success = false;
                //alert("You have an error in one of your slots. The begining date is after the end date.");
                alert("There is a problem with one of the time slots. The start time is after the end time.");
            }
            if (success) {
                return true;
            } else {
                e.preventDefault();
                return false;
            }
        });
    });

})(window);

$(function () {
    $(document).trigger("init");
    
    $(document).trigger("redraw");
});
$(window).load(function () {
    
    
});