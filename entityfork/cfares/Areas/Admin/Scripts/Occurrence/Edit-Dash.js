
/*
================================MODEL===================================
*/

Model.toDelete = {};
Model.toSave = {};

Model.save = function (model) {
    Model.toSave[model.id || model.cid] = model;
    delete Model.toDelete[model.id || model.cid];
};

Model.remove = function (model) {
    delete Model.toSave[model.id];
    Model.toDelete[model.id]=model;
};

Model.purgeFromSave = function (model) {
    delete Model.toSave[model.id || model.cid];    
};

// Variables dealing with Occurrence
Model.dowForOccurrence = function (dowArray) {
    var result = [];

    //milli epoch
    var minutes = 1000 * 60;
    var hours = minutes * 60;
    var days = hours * 24;
    var years = days * 365;

    var start = new Date(Model.OccurrenceModel.attributes.SlotRangeStart.getTime());
    var end = new Date(Model.OccurrenceModel.attributes.SlotRangeEnd.getTime());

    var startDay = Math.floor(start / days);
    var endDay = Math.ceil(end / days);

    var range = endDay - startDay;

    // FOR loop: create new Date for each day in range.
    // Amount of time between new dates depends on 'days' variable
    for (var i = 0; i < range; i++) {
        var newDay = new Date((i + startDay) * days);
        for (var j in dowArray) {
            // new Date created equals the "day number", then newDay is added to end of array
            if (newDay.getDay() == Date.getDayNumberFromName(dowArray[j])) {
                result.push(newDay);                
            }
        }

    }
    return result;
}

Model.allDaysChosen = function (dowArray) {
    var result = [];

    //milli epoch
    var minutes = 1000 * 60;
    var hours = minutes * 60;
    var days = hours * 24;
    var years = days * 365;

    var start = new Date(Model.OccurrenceModel.attributes.SlotRangeStart.getTime());
    var end = new Date(Model.OccurrenceModel.attributes.SlotRangeEnd.getTime());

    var startDay = Math.floor(start / days);
    var endDay = Math.ceil(end / days);

    var range = endDay - startDay;

    // FOR loop: create new Date for each day in range.
    // Amount of time between new dates depends on 'days' variable
    for (var i = 0; i < range; i++) {
        var newDay = new Date((i + startDay) * days);

        Model.slotRules.each(function (rule) {
            if (rule.attributes.date == "All") {

            } else if (!rule.attributes.date.getDay) {
                if (newDay.getDay() == Model.DOW[rule.attributes.date])
                    result = result.concat(Model.dowForOccurrence([rule.attributes.date]));
            } else {
                if (rule.attributes.date.toString("Mdyyyy") == newDay.toString("Mdyyyy"))
                    result.push(newDay);
            }
        });

    }

    var sorted_arr = result.sort(function (a, b) { return a.getTime() - b.getTime() }); // You can define the comparing function here. 

    var results2 = [], o = {};
    for (var i = 0; i < sorted_arr.length; i++) {
        o["_" + sorted_arr[i].getTime()] = sorted_arr[i];
    }

    for (var i in o) {
        results2.push(o[i]);
    }


    return results2;
}

Model.allDaysForOccurrence = function () {
    var NORMAL_DAYS = ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday"];
    return Model.dowForOccurrence(NORMAL_DAYS);
}

// DEPRECIATED: Use Date.js instead.
Model.DOW = {
    "Sunday":0,
    "Monday":1,
    "Tuesday":2,
    "Wednesday":3,
    "Thursday":4,
    "Friday":5,
    "Saturday":6
};


/*
* SlotCollection and associated functions
******************************************
*/
var SlotCollection = Backbone.Collection.extend({
initialize: function () {
        
},
    
model: DomainModel.TourSlot

});


/*
* SlotRuleCollection and associated functions
**********************************************
*/
var SlotRuleCollection = Backbone.Collection.extend({
    initialize: function () {
    },
    model: SlotRule
,
    comparator: function (slotRuleA, slotRuleB) {
        if (slotRuleA.attributes.date == "All") {
            return -1;
        }
        if (slotRuleB.attributes.date == "All") {
            return 1;
        }
        if (slotRuleA.attributes.date.getTime && !slotRuleB.attributes.date.getTime) {
            return 1;
        }
        if (!slotRuleA.attributes.date.getTime && slotRuleB.attributes.date.getTime) {
            return -1;
        }
        if (!slotRuleA.attributes.date.getTime && !slotRuleB.attributes.date.getTime) {
            var a, b;
            a = Model.DOW[slotRuleA.attributes.date];
            b = Model.DOW[slotRuleB.attributes.date];
            return a - b;
        }
        if (slotRuleA.attributes.date.getTime() != slotRuleB.attributes.date.getTime()) {
            return slotRuleA.attributes.date.getTime() - slotRuleB.attributes.date.getTime();
        } else {
            return slotRuleA.cid - slotRuleB.cid;
        }
    }

});

// SlotRule Model and associated functions
//*****************************************
var SlotRule = Backbone.Model.extend({
    initialize: function () {

        //this.on('remove', Model.remove);
        this.on('remove', this.removeModel, this);

    },

    removeModel: function (model) {
        this.attributes.slots.each(function (model) {
            //model.destroy();
            Model.purgeFromSave(model);
            Model.remove(model);
        });
    },

    defaults: {
        date: (null)

    },

    allowedToEdit: function (account) {
        return true;
    }

});

// SlotDate (extends SlotRule)
var SlotDate = SlotRule.extend({
    defaults: {
        date: (new Date())
    }
});

// SlotAll (extends SlotRule)
var SlotAll = SlotRule.extend({
    defaults: {
        date: "All"
    }
});

// SlotDOW (extends SlotRule)
var SlotDOW = SlotRule.extend({
    defaults: {
        date: "Monday"
    }
});

        
/*
================================VIEWS===================================
*/

/*
===MODEL VIEWS===
*/

// SlotView and associated functions
//**********************************
var SlotView = Backbone.View.extend({

    // Template for SlotView
    template: _.template($("#slot_row_template").html()),
    // Events
    events: {
        'click .remove-slot': 'remove',
        "change input[data-model-property]": 'valueUpdate',
        "click .edit-cameos": "cameoDialog"
    },

    valueUpdate: function (e) {

        var prop = $(e.currentTarget).attr("data-model-property");
        var val = $(e.currentTarget).val();
        if (prop == "Start" || prop == "End") {
            var sibDate = $(e.currentTarget).siblings("input[data-model-property='Date']").val();
            val = new Date("{0} {1}".format(sibDate, val));
        }

        this.model.set(prop, val);
        return true;
    },
    valueUpdateFixContext: function (e) {

        return e.data.valueUpdate.call(e.data, e);

    },
    cameoDialog: function () {
        $(".ui-dialog-content:visible").dialog('close');


        $(".dialog-cameos", this.$el.data("cameo-dialog")).dialog("open");

    },

    remove: function () {

        this.model.destroy();

    },

    render: function () {

        var self = this;

        var model = this.model.toJSON();
        model.cid = this.cid;

        this.setElement($(this.template(model)));


        var jq = $(".dialog-cameos", this.$el).dialog({
            modal: true,
            width: 400,
            autoOpen: false,
            open: function () {
                $(".cameos", this).trigger("sync");
            },
            buttons: {
                Ok: function () {
                    $(this).dialog("close");
                }
            }
        }).parents('.ui-dialog:eq(0)').wrap('<div class="admin-table"></div>');

        $("input[data-model-property]:not(.jq-auto-complete),select[data-model-property]:not(.jq-auto-complete)", jq)
            .bind("change", this, this.valueUpdateFixContext);

        $("input.jq-auto-complete.cameo-input", jq).bind("click", function () {
            $(this).removeClass("dormant");
            $(this).val("");
        }).bind("reset", function () {
            $(this).addClass("dormant");
            $(this).val("Click to Add");
        }).bind("blur", function () {
            var setLength = $(this).closest(".subform-input").find(".cameo-set").length;
            if (setLength)
                $(this).hide();
            else
                $(this).trigger("reset");

            $(this).closest(".subform-input").trigger("reset-wrapper");
        }).trigger("blur");


        var initView = function (jq) {
            $(".add-cameo", jq).click(function () {
                $(this).closest(".subform-input").find(".cameo-input").show().trigger("reset");
            });
            $(".remove-cameo", jq).click(function () {
                var view = $(this).closest(".cameos").data("view");
                var model = view.model;
                var pk = $(this).attr("data-pk");

                var cameoType = $(this).closest(".subform-input").find(".cameo-input").attr("data-cameo-type");
                var cameos = model.attributes.Cameos || (new DomainModel.ResUserCollection());

                model.attributes.RemoveCameos = model.attributes.RemoveCameos || (new DomainModel.ResUserCollection());

                cameos.each(function (child) {

                    if (child.attributes.UserId == pk) {
                        model.attributes.RemoveCameos.add(child);
                        cameos.remove(child);                        
                        return false;
                    }
                });



                $(this).closest(".cameos .subform-input").trigger("remove-from-view", pk);
            });
        };
        initView(jq);

        $("input.jq-auto-complete.cameo-input", jq).bind("autocomplete.pk", this, function (e, selected) {
            var pk = selected.pk;
            var view = e.data;
            var model = view.model;
            var cameoType = $(e.currentTarget).attr("data-cameo-type");

            $(e.currentTarget).closest(".cameos .subform-input").trigger("add-to-view", selected);

            if (!model.attributes.Cameos)
                model.set("Cameos", new DomainModel.ResUserCollection());

            model.attributes.Cameos.add(new DomainModel.ResUser({ UserId: selected.pk, Type: cameoType }));

            $(e.currentTarget).hide();
        });

        $(".cameos .subform-input", jq).bind("reset-wrapper", function () {
            $(this).find(".add-cameo:not(:last)").hide();
            $(this).find(".cameo-input").show().trigger("reset");
        }).trigger("reset-wrapper");

        $(".cameos .subform-input", jq).bind("add-to-view", function (e, selected) {
            if (!selected) {
                console.log("No data");
                return;
            }
            var name = selected.Name;
            var pk = selected.pk;
            var template = $("#slot_cameo_template").html();
            var html = template.format(name, pk);
            var jq = $(html).appendTo($(this).find(".cameo-canvas"));
            Page.init(jq);
            initView(jq);
            $(this).trigger("reset-wrapper");
        }).bind("remove-from-view", function (e, pk) {
            $(".cameo-set[data-pk='" + pk + "']", this).remove();
            $(this).trigger("reset-wrapper");
        });

        $("input.jq-auto-complete:not(.cameo-input)", jq).bind("autocomplete.pk", this, function (e, selected) {
            var pk = selected.pk;
            var view = e.data;
            var prop = $(e.currentTarget).attr("data-model-property");

            view.model.set(prop, pk);
        });

        $(".cameos", jq).bind("sync", function () {
            var view = $(this).data("view");
            var model = view.model;
            var self = this;
            ModelTools.local = false;
            model.LoadCameos({ success: function (children, d) {
                $(".cameo-canvas", this).empty();

                console.log(arguments);
                children.each(function (child) {
                    var cameoStr = Enum.CameoType[child.get("Type")];
                    var jqTarget = $(".cameo-input[data-cameo-type='" + cameoStr + "']", self).closest(".cameos .subform-input");
                    jqTarget.trigger("add-to-view", { Name: child.get("Name"), pk: child.get("UserId") });
                });

                ModelTools.local = true;
            }, error: function () {

                ModelTools.local = true;
            }
            });

        }).data("view", this).bind("clear", function () {
            //$(".cameo-canvas", this).empty();
        });

        Page.init(jq);

        this.$el.data("cameo-dialog", jq);
        Page.init(this.$el);
        return this;
    },

    // REFACTOR: remove if unnecessary
    unrender: function () {

    },

    // REFACTOR: remove if unnecessary
    initialize: function () {

    }
});

// SlotRuleView and associated functions
//***************************************
var SlotRuleView = Backbone.View.extend({
    template: _.template($("#date_menu_template").html()),

    unrender: function () {
        this.$el.remove();
        this.unbind();
        this.remove();
    },

    events: {
        'click .remove-date': 'removeModel'
    },

    removeModel: function () {
        
        this.model.destroy();
    },

    render: function () {
        var self = this;

        var model = this.model.toJSON();


        model.cid = this.cid;

        this.$el.html(this.template(model));

        Page.init(this.$el);

        return this;
    },

    initialize: function () {
        this.model.bind('remove', this.unrender, this);
    }
});


// SlotRuleAccordionView and associated functions
// **********************************************

var SlotRuleAccordionView = Backbone.View.extend({
    template: _.template($("#accordion_slot_template").html()),
    model: SlotRule,

    unrender: function () {
        this.$el.prev().remove();
        this.$el.remove();
        this.unbind();
        this.remove();

    },

    events: {
        'click .add-timeslot': 'addSlot'
    },

    render: function () {
        var self = this;

        this.$(".slot-rows").empty();
        var slots = this.model.attributes.slots;
        var c = slots.length;
        slots.each(function (item, i) { // in case collection is not empty
            var last = i == (c - 1);
            self.appendSlot(item, {silent:last});
        }, this);

        Page.init(this.$el);
        return this;
    },

    addSlot: function (slot) {
        var slot = new DomainModel.TourSlot({ OccurrenceId: Model.OccurrenceModel.id });
        if (this.model.attributes.date.getTime) {
            slot.set("Start", new Date(this.model.attributes.date.addHours(7)));
            slot.set("End", new Date(this.model.attributes.date.addHours(8)));
        }
        slot.on("change", Model.save);
        Model.save(slot);

        slot.on("remove", Model.purgeFromSave);

        this.model.attributes.slots.push(slot);

    },

    appendSlot: function (slot) {
        //TODO: FIX THIS

        var itemView = new SlotView({
            model: slot
        });
        slot.parent = this;
        this.$(".slot-rows").append(itemView.render().el);

    },

    initialize: function () {
        var slots = this.model.attributes.slots;


        slots.bind('add', this.appendSlot, this);
        slots.bind('remove', this.render, this);
    }
});


/*
===COLLECTION VIEWS===
*/

// SlotRuleCollectionView and associated functions
//************************************************

var SlotRuleCollectionView = Backbone.View.extend({
    //collection: SlotRuleCollection,
    unrender: function () {
        this.unbind();
        this.remove();
    },

    render: function () {

        var self = this;
        $(this.el).empty();
        var c = this.collection.length;
        this.collection.each(function (item, i) { // in case collection is not empty
            var last = i == (c - 1);
            self.appendSlot(item, {silent:last});
        }, this);

        return this;
    },

    addSlot: function (slot) {

        var slot = new SlotDate({ slots: new SlotCollection() });
        this.collection.add(slot);

    },

    appendSlot: function (slot) {

        var itemView = new SlotRuleView({
            model: slot
        });

        $(this.el).prepend(itemView.render().el);
    },

    initialize: function () {

        this.collection.bind('add', this.appendSlot, this);
        //this.collection.bind('remove', this.render, this);
    }
});


// SlotRuleAccordionCollectionView and associated functions
//*********************************************************

var SlotRuleAccordionCollectionView = Backbone.View.extend({
    collection: SlotRuleCollection,
    unrender: function () {
        this.unbind();
        this.remove();
    },

    render: function () {

        var self = this;
        this.$el.accordion("destroy");

        this.$el.empty();
        this.collection.each(function (item) { // in case collection is not empty

            self.appendSlot(item);
        }, this);


        this.$el.accordion({ autoHeight: false });
        return this;
    },

    appendSlot: function (slotRule) {

        var itemView = new SlotRuleAccordionView({
            model: slotRule
        });

        var model = itemView.model.toJSON();
        model.cid = itemView.cid;


        $(this.el).append(itemView.template(model));
        var el = $(this.el).children("div:last");
        itemView.setElement(el);
        itemView.render();

        //$(this.el).append(itemView.render().el);

    },

    initialize: function () {
        this.collection.bind('add', this.render, this);
        this.collection.bind('remove', this.render, this);        
    }
});




/*
================================CONTROLLERS===================================
*/

// SlotDashRouter Controller
//***************************

var SlotDashRouter = Backbone.Router.extend({
    routes: {
        "confirm-delete": "confirmDelete",
        "delete/:val": "deleteSlot",
        "save/:val": "saveSlot",
        "save-new/:val": "saveNewSlot",
        "next": "next",
        "next-delete": "nextDelete",
        "finished": "finished",
        "begin-save": "beginSaveSlot"
    },
    next: function () {
        if (!Model.FinalSlotCollection.length)
            return slotDashRouter.navigate("finished", { trigger: true });

        var model = Model.FinalSlotCollection.first();
        var id = model.id || model.cid || null;
        if (!model.id)
            slotDashRouter.navigate("save-new/" + id, { trigger: true });
        else
            slotDashRouter.navigate("save/" + id, { trigger: true });

    },
    nextDelete: function () {
        if (!Model.deleteIds.length)
            return slotDashRouter.navigate("next", { trigger: true });


        var id = Model.deleteIds.pop();

        slotDashRouter.navigate("delete/" + id, { trigger: true });


    },
    confirmDelete: function () {
        $(".ui-dialog-content:visible").dialog('close');
        $("#dialog-confirm").dialog('open');
    },

    deleteSlot: function (val) {
        (new DomainModel.TourSlot({ SlotId: val })).destroy({ success: function (model, msg, e) {

            var html = "Deleted slot: {0}<br />".format(model.id);
            $("#save-dash-console").append(html);
            slotDashRouter.navigate("next-delete", { trigger: true });
        }, error: function (model, msg, e) {
            var html = "Error deleting slot: {0}<br />".format(msg);
            $("#save-dash-console").append(html);
            slotDashRouter.navigate("next-delete", { trigger: true });
        }
        });
    },
    finished: function (val) {
        
        setTimeout(function () {
            $(".ui-dialog-content:visible").dialog('close');
            //TODO post to next page with controller arguments
            slotDashRouter.navigate("");
            window.location = "/Admin/Event/Summary/" + Model.OccurrenceModel.get("ResEvent").id;


        }, 4000);


    },
    saveSlot: function (val) {
        var slot = Model.FinalSlotCollection.get(val);
        Model.FinalSlotCollection.remove(slot);

        var startDate = slot.get("Start");

        var cutoffAddTo = JSON.parse($("#cutoff_day").val());
        var cutoff = startDate.clone().add(cutoffAddTo); ;
        var cutoffTime = $("#cutoff_time").val().replace(" ", "").toLowerCase();
        if (cutoffTime == "12:00pm")
            cutoffTime = "12:00";
        else if (cutoffTime == "12:00am")
            cutoffTime = "00:00";
        cutoff = Date.parse(cutoff.toString("MM/dd/yyyy") + " " + cutoffTime);

        slot.set("Cutoff", cutoff);
        //return;
        slot.save({}, { success: function (model, msg, e) {

            var html = "Saved slot: {0}<br />".format(model.id);
            $("#save-dash-console").append(html);

            /*model.SaveCameos();

            if (model.cameoDeleteQueue && model.cameoDeleteQueue.length)
            for (var i = 0; i < model.cameoDeleteQueue.length; i++) {
            var deleteId = model.cameoDeleteQueue[i];
            model.DeleteCameo(deleteId);
            }*/
            if ((slot.attributes.RemoveCameos && slot.attributes.RemoveCameos.length)
            || (slot.attributes.Cameos && slot.attributes.Cameos.length)) {
                var Cameos;

                var ids = [], types = [], SlotId = model.attributes.SlotId;

                if (slot.attributes.RemoveCameos && slot.attributes.RemoveCameos.length) {
                    Cameos = slot.attributes.RemoveCameos.toJSON();
                    for (var i = 0; i < Cameos.length; i++) {

                        ids.push(Cameos[i].UserId);

                        var typ;
                        if (isNumber(Cameos[i].Type))
                            typ = Number(Cameos[i].Type);
                        else
                            typ = Enum.CameoType[Cameos[i].Type];

                        types.push(typ);


                    }
                    //TODO FINISH CAMEO SAVING
                    $.ajax({
                        dataType: "json",
                        url: "/Service/Slot.svc/RemoveCameosForTourSlot({0}L)".format(SlotId),
                        type: "DELETE",
                        data: JSON.stringify({ UserIds: ids, CameoTypes: types }),
                        contentType: "application/json",
                        success: function (result) {
                            //slotDashRouter.navigate("next", { trigger: true });

                        },
                        error: function (result) {
                            //slotDashRouter.navigate("next", { trigger: true });
                        }

                    });
                }
                ids = []; types = [];

                if (slot.attributes.Cameos && slot.attributes.Cameos.length) {
                    Cameos = slot.attributes.Cameos.toJSON();
                    for (var i = 0; i < Cameos.length; i++) {

                        ids.push(Cameos[i].UserId);

                        var typ;
                        if (isNumber(Cameos[i].Type))
                            typ = Number(Cameos[i].Type);
                        else
                            typ = Enum.CameoType[Cameos[i].Type];

                        types.push(typ);


                    }
                    //TODO FINISH CAMEO SAVING
                    $.ajax({
                        dataType: "json",
                        url: "/Service/Slot.svc/SaveCameosForTourSlot({0}L)".format(SlotId),
                        type: "POST",
                        data: JSON.stringify({ UserIds: ids, CameoTypes: types }),
                        contentType: "application/json",
                        success: function (result) {
                            slotDashRouter.navigate("next", { trigger: true });

                        },
                        error: function (result) {
                            slotDashRouter.navigate("next", { trigger: true });
                        }

                    });
                } else {
                    slotDashRouter.navigate("next", { trigger: true });
                }
            } else {
                slotDashRouter.navigate("next", { trigger: true });
            }
        },
            error: function (model, msg, e) {
                var html = "Error updating old slot: {0}<br />".format(msg);
                $("#save-dash-console").append(html);
                slotDashRouter.navigate("next", { trigger: true });
            }

        });

    },

    saveNewSlot: function (val) {
        var slot = Model.FinalSlotCollection.getByCid(val);
        Model.FinalSlotCollection.remove(slot);
        slot.save({}, { success: function (model, msg, e) {
            var html = "Saved new slot: {0}<br />".format(model.get("Start").toString("MM/dd/yyyy hh:mm tt"));
            $("#save-dash-console").append(html);


            if ((slot.attributes.RemoveCameos && slot.attributes.RemoveCameos.length)
            || (slot.attributes.Cameos && slot.attributes.Cameos.length)) {
                var Cameos;

                var ids = [], types = [], SlotId = model.attributes.SlotId;

                if (slot.attributes.RemoveCameos && slot.attributes.RemoveCameos.length) {
                    Cameos = slot.attributes.RemoveCameos.toJSON();
                    for (var i = 0; i < Cameos.length; i++) {

                        ids.push(Cameos[i].UserId);

                        var typ;
                        if (isNumber(Cameos[i].Type))
                            typ = Number(Cameos[i].Type);
                        else
                            typ = Enum.CameoType[Cameos[i].Type];

                        types.push(typ);


                    }
                    //TODO FINISH CAMEO SAVING
                    $.ajax({
                        dataType: "json",
                        url: "/Service/Slot.svc/RemoveCameosForTourSlot({0}L)".format(SlotId),
                        type: "DELETE",
                        data: JSON.stringify({ UserIds: ids, CameoTypes: types }),
                        contentType: "application/json",
                        success: function (result) {
                            //slotDashRouter.navigate("next", { trigger: true });

                        },
                        error: function (result) {
                            //slotDashRouter.navigate("next", { trigger: true });
                        }

                    });
                }
                ids = []; types = [];

                if (slot.attributes.Cameos && slot.attributes.Cameos.length) {
                    Cameos = slot.attributes.Cameos.toJSON();
                    for (var i = 0; i < Cameos.length; i++) {

                        ids.push(Cameos[i].UserId);

                        var typ;
                        if (isNumber(Cameos[i].Type))
                            typ = Number(Cameos[i].Type);
                        else
                            typ = Enum.CameoType[Cameos[i].Type];

                        types.push(typ);


                    }
                    //TODO FINISH CAMEO SAVING
                    $.ajax({
                        dataType: "json",
                        url: "/Service/Slot.svc/SaveCameosForTourSlot({0}L)".format(SlotId),
                        type: "POST",
                        data: JSON.stringify({ UserIds: ids, CameoTypes: types }),
                        contentType: "application/json",
                        success: function (result) {
                            slotDashRouter.navigate("next", { trigger: true });

                        },
                        error: function (result) {
                            slotDashRouter.navigate("next", { trigger: true });
                        }

                    });
                } else {
                    slotDashRouter.navigate("next", { trigger: true });
                }
            } else {
                slotDashRouter.navigate("next", { trigger: true });
            }


        }, error: function (model, msg, e) {
            var html = "Error saving new slot: {0}<br />".format(msg);
            $("#save-dash-console").append(html);
            slotDashRouter.navigate("next", { trigger: true });
        }
        });

    },

    beginSaveSlot: function () {
        $(".ui-dialog-content:visible").dialog('close');
        $("#dialog-save").dialog('open');

        ModelTools.local = false;

        Model.FinalSlotCollection = new DomainModel.SlotCollection();

        //SlotModels - Build Greylist
        var greyListedDates = [];
        var greyListedDOW = [];
        var greyListedDatesFull = [];
        var greyListedDOWFull = [];

        // MODEL REPLICATION LOGIC
        //greylisted entries should block less specific entries
        //iterate all queued models        
        for (var i in Model.toSave) {
            var self = Model.toSave[i];
            var parent = self.parent.model;

            if (parent.attributes.date == "All") {
                //all rule - no priority
            } else if (Model.DOW[parent.attributes.date] != undefined) {
                //get all dates that will be copied
                replicationDates = Model.dowForOccurrence([parent.attributes.date]);
                for (var j = 0; j < replicationDates.length; j++) {
                    var repDate = replicationDates[j];
                    var dayObj = { day: repDate.getDate(), month: repDate.getMonth(), year: repDate.getFullYear() };
                    //day of week, medium priority
                    greyListedDOW.push(dayObj);
                    greyListedDOWFull.push(repDate);
                }
            } else {
                var repDate = self.attributes.Start;
                var dayObj = { day: repDate.getDate(), month: repDate.getMonth(), year: repDate.getFullYear() };
                //date, highest priority
                greyListedDates.push(dayObj);
                greyListedDatesFull.push(repDate);
            }
        } // END MODEL REPLICATION LOGIC

        //function to see if slot clashes with DOW greylist item
        var fnGreyDOW = function (element, index, array) {
            var newDay = { day: element.getDate(), month: element.getMonth(), year: element.getFullYear() };
            for (var j = 0; j < greyListedDOW.length; j++)
                if (JSON.stringify(greyListedDOW[j]) == JSON.stringify(newDay))
                    return false;
            return true;
        };

        //function to see if slot clashes with date greylist item
        var fnGreyDate = function (element, index, array) {
            var newDay = { day: element.getDate(), month: element.getMonth(), year: element.getFullYear() };
            for (var j = 0; j < greyListedDates.length; j++)
                if (JSON.stringify(greyListedDates[j]) == JSON.stringify(newDay))
                    return false;
            return true;
        };

        //MODEL SAVE LOGIC
        //iterate all queued models
        for (var i in Model.toSave) {
            var self = Model.toSave[i];
            var parent = self.parent.model;

            var replicationDates = [];
            if (parent.attributes.date == "All") {

                //replicationDates = Model.allDaysForOccurrence();
                replicationDates = Model.allDaysChosen();
                //get all DOW and remove greylisted items
                //replicationDates = replicationDates.filter(fnGreyDOW);
                //do it again all dates and remove greylisted items
                //replicationDates = replicationDates.filter(fnGreyDate);
                //replicationDates = replicationDates.concat(greyListedDatesFull).concat(greyListedDOWFull);

            } else if (Model.DOW[parent.attributes.date] != undefined) {

                replicationDates = Model.dowForOccurrence([parent.attributes.date]);
                //get all dates and remove greylisted dates
                //replicationDates = replicationDates.filter(fnGreyDate);
            } else {
                //dates get added
                Model.FinalSlotCollection.push(self);
            }

            for (var j = 0; j < replicationDates.length; j++) {
                var repDate = replicationDates[j];
                var newDay = { day: repDate.getDate(), month: repDate.getMonth(), year: repDate.getFullYear() };


                var json = $.extend(true, {}, self.attributes);
                json.Start = self.attributes.Start.clone();
                json.End = self.attributes.End.clone();
                var slotSpan = json.End.getTime() - json.Start.getTime();
                json.Start = json.Start.set(newDay);
                json.End = new Date(json.Start.getTime() + slotSpan);
                json.OccurrenceId = Model.OccurrenceModel.id;
                //CREATE NEW SLOT
                Model.FinalSlotCollection.push(new DomainModel.TourSlot(json));

            }

        } // END MODEL SAVE LOGIC

        console.log(Model.FinalSlotCollection.toJSON());
        //return;
        //MODEL DELETE LOGIC
        var deleteIds = [];
        for (var i in Model.toDelete) {
            var self = Model.toDelete[i];
            var parent = self.parent.model;

            var replicationDates = [];
            if (parent.attributes.date == "All") {

            } else if (Model.DOW[parent.attributes.date] != undefined) {

            } else {
                deleteIds.push(self.id);
            }
        }


        Model.deleteIds = deleteIds;
        for (var i = 0; i < Model.deleteIds.length; i++) {
            //slotDashRouter.navigate("delete/" + Model.deleteIds[i], { trigger: true });
        }

        slotDashRouter.navigate("next-delete", { trigger: true });

        /*Model.FinalSlotCollection.each(function (model) {
        if (!model.id)
        slotDashRouter.navigate("save-new/" + id, { trigger: true });
        else
        slotDashRouter.navigate("save/" + id, { trigger: true });
        });*/

        // END MODEL DELETE
    }

});
var slotDashRouter = new SlotDashRouter();


/*
================================INSTANCE===================================
*/



        
var Views = {
    
};

//JQ INIT
$(function () {


    Model.OccurrenceModel.fetch({ success: function () {

        var slotsUrl = "/DataService/ResEvent.svc/Occurrences({0}L)/Slots?$expand=Slot_Tour/User&$orderby=Start".format(Model.OccurrenceModel.id);


        var slots = new DomainModel.SlotCollection();
        Model.OccurrenceModel.set({ "Slots": slots });
        Model.OccurrenceModel.set("ResEvent", new DomainModel.ResEvent({ ResEventId: Model.OccurrenceModel.get("ResEventId") }));
        //lets do ajax instead
        /*
        slots.url = slotsUrl;
        slots.fetch({
        */


        $.ajax({
            dataType: ("json"),
            url: slotsUrl,
            type: "GET",
            data: null,
            contentType: "application/json",
            success: function (result) {
                if (result.d.length) {
                    for (var i = 0; i < result.d.length; i++) {
                        result.d[i] = ModelTools.FixDate(result.d[i]);
                    }
                }
                var dateTargets = {};

                var targetFormatString = "MMM_d_yyyy";
                var parseableFormatString = "MM/dd/yyyy";
                var setCutoff = false;
                $(result.d).each(function (i) {
                    var d = this;
                    var startDate = d["Start"];

                    if (!setCutoff)
                        $("#cutoff_time").val(d["Cutoff"].toString("hh:mm tt"));
                    setCutoff = true;
                    var dateStr = startDate.toString(targetFormatString);
                    if (!dateTargets[dateStr])
                        dateTargets[dateStr] = { date: new Date(startDate.toString(parseableFormatString)), models: [] };


                    //MANAGE incoming data

                    $.extend(d, d.Slot_Tour[0]);
                    delete d.Slot_Tour;
                    d.Guide = new DomainModel.ResUser(d.User);
                    delete d.User;
                    //console.log(d.Guide,"Guide");

                    //END MANAGE incoming data

                    dateTargets[dateStr].models.push(d);
                });


                Backbone.Model.prototype.localStorage = new window.Store("edit-dash");
                var storedModels = Backbone.Model.prototype.localStorage.findAll();
                $(storedModels).each(function (i) {
                    var d = this;
                    var startDate = d["Start"];
                    var dateStr = startDate.toString(targetFormatString);
                    if (!dateTargets[dateStr])
                        dateTargets[dateStr] = { date: new Date(startDate.toString(parseableFormatString)), models: [] };

                    dateTargets[dateStr].models.push(d);
                    dateTargets[dateStr].fromLocal = true;
                });


                ModelTools.local = true;

                var dateArr = [];
                for (var i in dateTargets) {
                    dateArr.push(dateTargets[i]);
                }
                dateArr.sort(function (a, b) {

                    return a.date.getTime() - b.date.getTime();
                });

                Model.slotRules = new SlotRuleCollection();



                Views = {
                    slotSummary: new SlotRuleCollectionView({ collection: Model.slotRules }),
                    slotAccordions: new SlotRuleAccordionCollectionView({ collection: Model.slotRules })
                };

                Views.slotAccordions.setElement($("#slot_accordion"));
                Views.slotSummary.setElement($("#date_summary_list"));


                


                for (var i = 0; i < dateArr.length; i++) {
                    var slotDate = new SlotDate({ date: dateArr[i].date, slots: new SlotCollection() });

                    for (var j = 0; j < dateArr[i].models.length; j++) {
                        dateArr[i].models[j].OccurrenceId = Model.OccurrenceModel.id;
                        var mod = dateArr[i].models[j];
                        var slot = new DomainModel.TourSlot(mod);
                        slot.attributes.Guide = mod.Guide;
                        if (!dateArr[i].fromLocal)
                            slot.bind('remove', Model.remove, slot);


                        Model.save(slot);
                        slotDate.attributes.slots.add(slot);
                        slot.bind('change', Model.save, slot);
                    }

                    Model.slotRules.add(slotDate, { silent: true });
                    //console.log(slotDate);
                }

                for (var i in Views)
                    Views[i].render();


                /*
                EXAMPLE: Use this for adding days
                var slotDay = new SlotDOW({ date: "Tuesday", slots: new SlotCollection() });
                Model.slotRules.add(slotDay);
                */
                var slotAll = new SlotAll({ slots: new SlotCollection() });
                Model.slotRules.add(slotAll);


                //DomainModel.Slot.prototype.bind('change', Model.change);



                Backbone.history.start();
                //slotDashRouter.navigate("hello/foo", {trigger:true});


            }
        });

        $("#inline_cal").datepicker({
            onSelect: function (date) {

                date = new Date(date);
                var at = 0;

                //date.toString("MMMM d, yyyy")

                var abort = false;

                var allMode = $("input[name='all_style']:checked").val();
                if (allMode == "all") {
                    //$("#day_style_radios").
                    console.log($("input[name='all_style',value='day']"));
                    $("input[name='all_style'][value='day']").attr("checked", "checked").trigger("change");
                    $("#all_style_radios").buttonset("refresh");
                }

                var dayMode = $("input[name='day_style']:checked").val();
                if (dayMode == "DOW") {
                    /*Model.slotRules.each(function (model, i) {
                    var modelDate = new Date(model.get("date"));
                    if (!modelDate.getTime&&modelDate.date!="All") {
                            
                    at = i;
                    return false;
                    }
                    });*/

                    Model.slotRules.each(function (model, i) {
                        if (model.get("date").toString("MMMM d, yyyy") == date.toString("dddd")) {
                            abort = true;
                        }
                    });
                    if (abort)
                        return false;
                    var slotDay = new SlotDOW({ date: date.toString("dddd"), slots: new SlotCollection(), OccurrenceId: Model.OccurrenceModel.id });

                } else {

                    Model.slotRules.each(function (model, i) {
                        if (model.get("date").toString("MMMM d, yyyy") == date.toString("MMMM d, yyyy")) {
                            abort = true;
                        }
                    });

                    if (abort) {
                        return false;
                    }

                    Model.slotRules.each(function (model, i) {
                        var modelDate = new Date(model.get("date"));
                        if (modelDate.getTime && modelDate.getTime() > date.getTime()) {

                            at = i;
                            return false;
                        }
                    });

                    var slotDay = new SlotDate({ date: date, slots: new SlotCollection(), at: at, OccurrenceId: Model.OccurrenceModel.id });

                }

                Model.slotRules.add(slotDay);

            },
            defaultDate:Model.Start,
            beforeShow: function () {

            }
        });

        // SAVE
        $("#save").click(function () {

            if (!$("#cutoff_time").val()) {
                return alert("You must select a cuttoff time first.");
            }

            if (_.size(Model.toDelete)) {

                slotDashRouter.navigate("confirm-delete", { trigger: true });
            } else if (_.size(Model.toSave)) {
                slotDashRouter.navigate("begin-save", { trigger: true });
            }


        });


    }
    });



    // Dialog Save
    //Model.sampleAddress = new DomainModel.Address({ AddressId: 1 });
    $("#dialog-save").dialog("option", {
        modal: true,
        width: 500,
        height: 500,
        autoOpen: false
        /*,
        beforeClose: function () {
        if (confirm("The slot process has started, are you sure you want to abort?")) {
        slotDashRouter.navigate("", { trigger: true });
        return true;
        }
        }*/
    });

    // Dialog Delete
    $("#dialog-delete").dialog("option",{
        modal: true,
        width: 500,
        height:"auto",
        autoOpen: false
        /*,
        beforeClose: function () {
        if (confirm("The slot process has started, are you sure you want to abort?")) {
        slotDashRouter.navigate("", { trigger: true });
        return true;
        }
        }*/
    });


    //Dialog Complete
    $("#dialog-complete").dialog("option", {
        modal: true,
        autoOpen: false,
        width: "auto",
        height: "auto",
        buttons: {
            "Ok": function () {
                $(this).dialog("close");

            }
        }
    });

    // Dialog Confirm
    $("#dialog-confirm").dialog("option", {
        resizable: false,
        height: 190,
        width: "auto",
        height: "auto",
        modal: true,
        autoOpen: false,
        buttons: {
            "Delete": function () {
                $(this).dialog("close");
                slotDashRouter.navigate("begin-save", { trigger: true });
            },
            Cancel: function () {
                $(this).dialog("close");
                slotDashRouter.navigate("", { trigger: true });
            }
        }
    });

    $("#day_style_radios,#all_style_radios").buttonset();

    $("#all_style_radios input[name='all_style']").change(function () {
        var allMode = $(this).val();


        if (allMode == "all") {
            $(".slot-accordion").addClass("mode-all").removeClass("mode-date");
            $(".slot-accordion").accordion("activate", 0);
            //$(".slot-accordion .mode-date").hide();
            //$(".slot-accordion .mode-all").show();
        } else {
            $(".slot-accordion").accordion("activate", 1);
            $(".slot-accordion").removeClass("mode-all").addClass("mode-date");
            //$(".slot-accordion .mode-all").hide();
            //$(".slot-accordion .mode-date").show();
        }
    });
    $("#all_style_radios input:checked").trigger("change");

});

$(document).bind("render", function () {

});


