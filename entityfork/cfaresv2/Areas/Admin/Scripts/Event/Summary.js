var Model = {
    capacityCache: {}
};

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
        if (!Model.FinalSlotCollection||!Model.FinalSlotCollection.length)
            return slotDashRouter.navigate("finished", { trigger: true });

        

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
        //$(".ui-dialog-content:visible").dialog('close');
        setTimeout(function () {
            $(".ui-dialog-content:visible").dialog('close');            
            slotDashRouter.navigate("");            
            $("#dialog-complete").dialog('open');
            $("table.filterable").dataTable().fnDraw(true);

        }, 4000);
        

    },
    saveSlot: function (val) {
        //not called
    },

    saveNewSlot: function (val) {
        //not called

    },

    beginSaveSlot: function () {
        $(".ui-dialog-content:visible").dialog('close');
        $("#dialog-save").dialog('open');

        
        Model.FinalSlotCollection = new DomainModel.SlotCollection();

        
        //return;
        //MODEL DELETE LOGIC
        
        for (var i = 0; i < Model.deleteIds.length; i++) {
            //slotDashRouter.navigate("delete/" + Model.deleteIds[i], { trigger: true });
        }

        slotDashRouter.navigate("next-delete", { trigger: true });

        
    }

});
var slotDashRouter = new SlotDashRouter();


/*
================================INSTANCE===================================
*/



//JQ INIT
$(function () {

    // Dialog Save    
    $("#dialog-save").dialog("option", {
        modal: true,
        width: 500,
        height: 500,
        autoOpen: false

    });

    // Dialog Delete
    $("#dialog-delete").dialog("option", {
        modal: true,
        width: 500,
        height: "auto",
        autoOpen: false

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

    $("#delete").click(function () {
        //alert("delete");
        Model.deleteIds = [];
        $("#event_summary input[name='tableselect']:checked").each(function () { Model.deleteIds.push($(this).val()); });
        Model.deleteIds.filter(function (item) {
            if (Model.capacityCache["_" + item]) {
                return true;
            } else {
                alert("Slot number " + item + " has tickets and will not be deleted. Please contact an administrator to handle this.");
                return false;
            }
        });
        if (Model.deleteIds.length) {

            slotDashRouter.navigate("confirm-delete", { trigger: true });
        }


    });

    Backbone.history.start();
});