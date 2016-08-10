jQuery.fn.dataTableExt.oApi.fnSetFilteringDelay = function (oSettings, iDelay) {
    /*
    * Inputs:      object:oSettings - dataTables settings object - automatically given
    *              integer:iDelay - delay in milliseconds
    * Usage:       $('#example').dataTable().fnSetFilteringDelay(250);
    * Author:      Zygimantas Berziunas (www.zygimantas.com) and Allan Jardine
    * License:     GPL v2 or BSD 3 point style
    * Contact:     zygimantas.berziunas /AT\ hotmail.com
    */
    var 
        _that = this,
        iDelay = (typeof iDelay == 'undefined') ? 250 : iDelay;

    this.each(function (i) {
        $.fn.dataTableExt.iApiIndex = i;
	    var $this = this,
	        oTimerId = null,
	        sPreviousSearch = null,
	        // SH - Fix for identifying datatable filter inputs
	        filterId = _that.attr('id') + '_filter',
			anControl = $('#' + filterId + ' input'); 
			//anControl = $('.dataTables_filter input', _that);

    	//console.log('anControl ( ' + filterId + ' ) - ' + anControl.length);

        anControl.unbind('keyup').bind('keyup', function () {
            //console.log('keyup - ' + anControl.val());
            if (sPreviousSearch === null || sPreviousSearch != anControl.val()) {
                window.clearTimeout(oTimerId);
                sPreviousSearch = anControl.val();
                //console.log("TIMEOUT");
                oTimerId = window.setTimeout(function () {
                    $.fn.dataTableExt.iApiIndex = i;
                	//console.log('fnFilter - ' + anControl.val());
	                _that.fnFilter(anControl.val());
                }, iDelay);
            }
        });

        return this;
    });
    return this;
}

jQuery.fn.dataTableExt.oApi.fnSetJqUiHover = function (oSettings) {
    /*
    * Inputs:      object:oSettings - dataTables settings object - automatically given
    *              integer:iDelay - delay in milliseconds
    * Usage:       $('#example').dataTable().fnSetFilteringDelay(250);
    * Author:      Zygimantas Berziunas (www.zygimantas.com) and Allan Jardine
    * License:     GPL v2 or BSD 3 point style
    * Contact:     zygimantas.berziunas /AT\ hotmail.com
    */
    var 
        _that = this;

    this.each(function (i) {
        $.fn.dataTableExt.iApiIndex = i;
        var 
            $this = this;

        
        $(_that).dataTable().bind("processing page",function () {
            $("thead th,.dataTables_paginate .ui-button", $(_that).closest(".dataTables_wrapper")).bind(
            {
                mouseenter:
                function () {
                    if (!$(this).hasClass("ui-state-disabled"))
                        $(this).toggleClass("ui-state-hover", true);
                },
                mouseleave:
                function () {
                    $(this).toggleClass("ui-state-hover", false);
                }
            });
        });

        return this;
    });
    return this;
}
var Convert = {
    toInt: function (arg) {
        return Number(toInt);
    },
    toFunction: function (arg) {
        return eval(arg);
    },
    toBoolean: function (arg) {
        return arg.toLowerCase() == 'true';
    },
    toDefault: function (arg) {
        return arg;
    }
};

var DataTableRes = {};

DataTableRes.getIndexFromId = function (id, table) {
    var index = -1;
    if ($(table).is('th')) {
        var colArgs = $(table).data("args");
        if (colArgs)
            table = colArgs.table;
        else
            table = $(table).closest("table");
    }
    var args = $(table).data("args");

    if (args) {
        $(args.aoColumnDefs).each(function (i) {
            var thid = this.id;
            if (thid == id)
                index = i;
        });
    } else {
        $(table).find("th").each(function (i) {
            var thid = DataTableRes.getIdFromTh(this);
            if (thid == id)
                index = i;
        });
    }
    return index;
}

DataTableRes.getIdFromTh = function (th) {
	var id = $(th).attr("data-id") || $(th).attr("data-column-name") || $(th).attr("id") || ($(th).html() && $(th).html().replace(/[^0-9A-Z]/gi, ""));
    var type = id.match(/[A-Z]$/);
    
    if (type)
        return id.replace(/[A-Z]$/, "");
    return id;
}

DataTableRes.getTypeFromTh = function (th) {
    var id = $(th).attr("data-id") || $(th).attr("data-column-name") || $(th).attr("id") || ($(th).html() && $(th).html().replace(/[^0-9A-Z]/gi, ""));
    return DataTableRes.getTypeFromId(id);
}

DataTableRes.getTypeFromId = function (id) {
    
    var type = id.match(/[A-Z]$/);
    
    if (type&&type.length>0&&type[0])
        return type[0];
    return "S";
}

DataTableRes.getThFromId = function (table, id) {
    if ($(table).is('th')) {
        var colArgs = $(table).data("args");
        if (colArgs)
            table = colArgs.table;
        else
            table = $(table).closest("table");
    }
    
    var th = null;
    $(table).find("th").each(function () {
        var thid = DataTableRes.getIdFromTh(this);
        if (thid == id)
            th = this;
    });
    return th;
}


DataTableRes.DomToTableArgs = function ($that) {
    $that = $($that);
    var outputArgs = {};
    var argTemplate = {
        mData: { key: "data-data" },
        sCrud: { key: "data-crud" },
        sDom: { key: "data-dom" },
        bJQueryUI: { key: "data-jquery-ui", parse: Convert.toBoolean },
        fnODataFilter: { key: "data-filter", parse: Convert.toFunction },
        bOData: { key: "data-odata", parse: Convert.toBoolean },
        sMode: { key: "data-mode"},
        iDisplayLength: { key: "data-display-length", parse: Convert.toInt },
        iDisplayStart: { key: "data-display-start", parse: Convert.toInt },
        sServerMethod: { key: "data-server-method" },
        aLengthMenu: { key: "data-length-menu" }

    };

    var argColTemplate = {
        bVisible: { key: "data-visible", parse: Convert.toBoolean },
        bSearchable: { key: "data-searchable", parse: Convert.toBoolean },
        bSortable: { key: "data-sortable", parse: Convert.toBoolean },
        asSorting: { key: "data-default-sort", parse: Convert.toFunction },
        bEditable: { key: "data-editable", parse: Convert.toBoolean },
        sQueryName: { key: "data-query-name" },
        sClass: { key: "data-css" },
        sDefaultContent: { key: "data-default-content" },
        mEditType: { key: "data-edit-type", parse: function (arg) { return Convert.toFunction(arg) || arg; } },
        mEditData: { key: "data-edit-data", parse: function (arg) { return Convert.toFunction(arg) || arg; } },
        fnOnEdit: { key: "data-on-edit", parse: Convert.toFunction },
        fnOnSubmit: { key: "data-on-submit", parse: Convert.toFunction },
        sCrudOverride: { key: "data-crud" },
        oIdOverride: { key: "data-id-override" },
        bIdColumn: { key: "data-id-column", parse: Convert.toBoolean },

        fnODataColumnFilter: { key: "data-filter", parse: Convert.toFunction },
        mData: {    //row is an object with each element dictionaried BY ID
            fnOnSet: { key: "data-on-set", parse: Convert.toFunction },
            fnOnDisplay: { key: "data-on-display", parse: Convert.toFunction },
            fnOnFilter: { key: "data-on-filter", parse: Convert.toFunction },
            fnOnSort: { key: "data-on-sort", parse: Convert.toFunction },
            fnOnType: { key: "data-on-type", parse: Convert.toFunction }
        }
    };



    for (var i in argTemplate) {
        var attrIndex = argTemplate[i];
        var attr = $that.attr(attrIndex.key);
        if (!attrIndex.parse)
            attrIndex.parse = Convert.toDefault;
        if (attr)
            outputArgs[i] = attrIndex.parse(attr);
    }

    var isUrl = outputArgs.mData.indexOf('/') != -1;

    if (isUrl || outputArgs.sMode=="backbone") {
        outputArgs.sAjaxSource = outputArgs.mData;
        outputArgs.sPaginationType = "full_numbers";
    } else {
        outputArgs.mData = eval(outputArgs.mData);
        outputArgs.bOData = false;
        outputArgs.bServerSide = false;
        outputArgs.sDom = '<"H"lr>t<"F"ip>';
    }
    if (outputArgs.sMode == "backbone") {
        outputArgs.bOData = false;
    }

    outputArgs.aColumns = {};

    $that.find("th").each(function () {
        var id = DataTableRes.getIdFromTh(this);
        var type = DataTableRes.getTypeFromTh(this);
        var colObject = {};
        for (var i in argColTemplate) {
            var attrIndex = argColTemplate[i];

            if (attrIndex.key) {
                if (!attrIndex.parse)
                    attrIndex.parse = Convert.toDefault;
                var attr = $(this).attr(attrIndex.key);
                if (attr)
                    colObject[i] = attrIndex.parse(attr);
            }
        }
        colObject.mData = {};
        for (var i in argColTemplate.mData) {
            var attrIndex = argColTemplate.mData[i];
            var attrData = $(this).attr("data-data");
            var attrEdit = $(this).attr("data-edit");
            if (attrData) {
                colObject.mData = attrData;
                continue;
            }
            if (attrEdit) {
                colObject.mEdit = attrEdit;
                continue;
            }

            if (attrIndex.key) {
                if (!attrIndex.parse)
                    attrIndex.parse = Convert.toDefault;
                var attr = $(this).attr(attrIndex.key);
                if (attr)
                    colObject.mData[i] = attrIndex.parse(attr);
            }
        }

        var par = $(this).attr("data-column-parent");
        if (par)
            colObject.oParent = par;
        if (id && type)
            colObject.sSearchType = type;
        outputArgs.aColumns[id] = colObject;
    });



    return outputArgs;
}

DataTableRes.defaultInputArgs =
{
    bOData: true,
    sMode:"url",
    iCount: 10000,
    bProcessing: true,
    aLengthMenu: [[10, 25, 50, 100, -1], [10, 25, 50, 100, "All"]],
    bServerSide: true,
    bJQueryUI: true,
    sDom: '<"H"TClfr>t<"F"ip>',

    oTableTools: {
        "sSwfPath": "/Scripts/datatables/TableTools-2.0.3/media/swf/copy_csv_xls_pdf.swf",
        "aButtons": [
        /*{ "type": "copy", "buttonText": "Copy" },
        { "type": "csv", "buttonText": "CSV" },
        { "type": "xls", "buttonText": "Excel" }*/
            "copy", "csv", {
                "sExtends": "ajax",
                "sButtonText": "Excel",
                "fnClick": function (nButton, oConfig) {
                    oConfig.sFieldBoundary = "'";
                    oConfig.sFieldSeperator = ",";
                    oConfig.mColumns = "visible";
                    oConfig.fnCellRender = function (cell) {
                        var newCell = ("" + cell).replace(/\r/gi, "").replace(/<(?:.|\n)*?>/gm, '').replace(/'/g, "\\'");
                        //console.log(newCell);
                        return newCell;
                    }
                    
                    var sData = this.fnGetTableData(oConfig);
                    //console.log(sData);
                    sData = sData.replace(/\\''/g, "\\'");
                    //console.log(sData);
                    var id = $(this.dom.table).attr("data-filename") || $(this.dom.table).attr("id");
                    $.ajax({
                        "url": "/Admin/Excel/Table/" + id,
                        "data": { "data": sData },
                        "success": function (d, xhr) {
                            var href = d.href;
                            $("#xcel_src").attr("src", href);
                        },
                        "dataType": "json",
                        "type": "POST",
                        "cache": false,
                        "error": function () {
                            alert("Error detected when sending table data to server");
                        }
                    });
                }

            }
        /*{
        "sExtends": "xls",
        "sButtonText": "Excel"
                
        }*/
        ]

    }
};

DataTableRes.defaultEditArgs =
{
    bOData: true,
    iCount: 10000,
    bProcessing: true,
    bServerSide: true,
    bJQueryUI: true
};

DataTableRes.defaultColInputArgs =
{
    bSortable: true,
    bVisible: true,
    bEditable: true,
    sQueryName:"",
    bIdColumn: false,
    sCrudOverride: null,
    oIdOverride: null,
    fnODataColumnFilter: function (sourceObject, val, row, column) {
        return null;
    }
};

DataTableRes.defaultColEditArgs =
{
    
    type: "text",
    data: null
};

DataTableRes.defaultMData =
{
    fnOnSet: function (sourceObject, val, column) {//return object to set
        
        return val;
    },
    fnOnDisplay: function (sourceObject, val, column) {//return string to display
        return val;
    },
    fnOnFilter: function (sourceObject, val, column) {//return value to sort
        return val;
    },
    fnOnSort: function (sourceObject, val, column) {//return value to sort
        return val;
    },
    fnOnType: function (sourceObject, val, column) {//return value to sort
        return val;
    }
};

DataTableRes.buildEditPluginArguments = function(table, args) {

    $this = $(table);

    var tableArgs = $.extend(true, {}, DataTableRes.defaultEditArgs, args);
    if (tableArgs.sMode == "backbone") {
        tableArgs.sUpdateURL = DataTableRes.fnProcessUpdate;
    } else {
        tableArgs.sUpdateURL = DataTableRes.fnProcessUpdate;
    }

    var colArr = [];
    $this.find("th").each(function() {

        var colEditObject = null;
        for (var i = 0; i < args.aoColumnDefs.length; i++) {
            var colArgObject = args.aoColumnDefs[i];
            var dtId = DataTableRes.getIdFromTh(this);

            if (colArgObject.id == dtId) {
                colEditObject = $.extend(true, {}, DataTableRes.defaultColEditArgs, colArgObject);

                if (!colArgObject.bEditable || colArgObject.bIdColumn)
                    colEditObject = null;
                else {
                    if (colArgObject.mEditType)
                        colEditObject.type = colArgObject.mEditType;
                    if (colArgObject.mEditData)
                        colEditObject.type = colArgObject.mEditData;
                }
            }
        }
        colArr.push(colEditObject);
    });

    tableArgs.aoColumns = colArr;

    $this.data("editArgs", tableArgs);
    return tableArgs;
};


DataTableRes.buildPluginArguments = function(table, inputArgs) {

    $this = $(table);


    var tableArgs = $.extend(true, {}, DataTableRes.defaultInputArgs, inputArgs);

    tableArgs.oLanguage = { sInfoFiltered: "" };
    var colArr = [];
    for (var i in tableArgs.aColumns) {
        var colInput = tableArgs.aColumns[i];
        var th = DataTableRes.getThFromId($this, i);
        var type = DataTableRes.getTypeFromTh(th);
        var colIndex = DataTableRes.getIndexFromId(i, $this);
        var colObject = $.extend(true, {}, DataTableRes.defaultColInputArgs, colInput);


        colObject.id = i;
        colObject.sSearchType = type;
        colObject.index = colIndex;


        if (typeof(colObject.mData) == 'object') {


            colObject.mDataProp = function(source, type, val) {


                /*TODO SORT THIS OUT - mDataProp*/
                var th = this;
                var args = $(th).data("args");

                var table;
                var colArgs = $(th).data("args");
                if (colArgs)
                    table = colArgs.table;
                else
                    table = $(table).closest("table");

                var tableArgs = $(table).data("args");


                var sourceObject = {};

                for (var i = 0; i < tableArgs.aoColumnDefs.length; i++) {
                    var colDef = tableArgs.aoColumnDefs[i];
                    if (!colDef.bVisible) {

                    }
                }

                for (var i = 0; i < tableArgs.aoColumnDefs.length; i++) {
                    var colDef = tableArgs.aoColumnDefs[i];

                    var id = colDef.id;

                    sourceObject[colDef.id] = source[i];

                }

                var colMData = $.extend(true, {}, DataTableRes.defaultMData, args.mData);

                var oParent = args.oParent;

                if (oParent) {
                    var newTh = oParent.th;
                    if (newTh)
                        th = newTh;
                }

                var id = DataTableRes.getIdFromTh(th);
                var col = DataTableRes.getIndexFromId(id, table);

                var defaultVal = source[col];

                switch (type) {
                case "set":
                    var setted = colMData.fnOnSet.call(th, sourceObject, val, col);
                    return setted;
                case "filter":
                    return colMData.fnOnFilter.call(th, sourceObject, defaultVal, col);
                case "type":
                    return colMData.fnOnType.call(th, sourceObject, defaultVal, col);
                case "sort":
                    return colMData.fnOnSort.call(th, sourceObject, defaultVal, col);
                case "display":
                default:
                    return colMData.fnOnDisplay.call(th, sourceObject, defaultVal, col);
                }
            };
        } else {
            colObject.mDataProp = colObject.mData;
        }

        colObject.aTargets = [colIndex];


        colObject.th = th;
        colObject.table = $this;
        $(th).data("args", colObject);
        colArr.push(colObject);
    }

    for (var i = 0; i < colArr.length; i++) {
        var colObject = colArr[i];
        if (colObject.oParent && typeof(colObject.oParent) == 'string') {
            for (var j = 0; j < colArr.length; j++) {
                if (colArr[j].id == colObject.oParent) {
                    colObject.oParent = colArr[j];
                    colObject.bSortable = false;
                    colObject.bSearchable = false;
                    colObject.bEditable = false;
                }
            }

        }
        if (colObject.oIdOverride && typeof(colObject.oIdOverride) == 'string') {
            for (var j = 0; j < colArr.length; j++) {
                if (colArr[j].id == colObject.oIdOverride) {
                    colObject.oIdOverride = colArr[j];
                }
            }

        }
        if (colObject.bIdColumn)
            tableArgs.idColumn = colObject;
    }

    if (tableArgs.bOData) {
        tableArgs.fnServerData = DataTableRes.fnServerData;
    } else if (tableArgs.sMode == "backbone") {
        tableArgs.fnServerData = DataTableRes.fnBackboneData;
    } else {

        tableArgs.aaData = DataTableRes.bindObjectToTable.call($this, tableArgs.mData, tableArgs, colArr);
    }

    if (colArr.length)
        tableArgs.aoColumnDefs = colArr;


    $this.data("args", tableArgs);
    return tableArgs;
};

DataTableRes.DELAY = 700;

DataTableRes.addObjectToTable = function(table, obj) {
    $(table).dataTable().fnDestroy();
    $(table).attr("style", "");
    var self = table;
    var inputArgs = DataTableRes.DomToTableArgs(self);
    inputArgs.mData.push(obj);
    var args = DataTableRes.buildPluginArguments(self, inputArgs);
    var editArgs = DataTableRes.buildEditPluginArguments(self, args);


    if (args.sCrud)
        $(self).dataTable(args).fnSetFilteringDelay(DataTableRes.DELAY).fnSetJqUiHover().makeEditable(editArgs);
    else
        $(self).dataTable(args).fnSetFilteringDelay(DataTableRes.DELAY).fnSetJqUiHover();
};

DataTableRes.bindObjectToTable = function(mData, tableArgs, colArr) {
    var aaData = [];
    for (var j = 0; j < mData.length; j++) {
        var row = [];
        for (var i = 0; i < colArr.length; i++) {
            var colObject = colArr[i];
            var id = colObject.oParent ? colObject.oParent.id : colObject.id;
            var td = mData[j][id];

            if (td) {
                row.push(td);
                continue;
            }
        }
        aaData.push(row);
    }
    $(this).data("data-target", mData);
    tableArgs.fnDrawCallback = function() {
        var dataTarget = $(this).data("data-target");

        var data = $(this).dataTable().fnGetData();
        var dataArrObject = [];

        var tableArgs = $(this).data("args");


        for (var j = 0; j < data.length; j++) {
            var row = data[j];
            var rowObj = {};
            for (var i = 0; i < tableArgs.aoColumnDefs.length; i++) {
                var col = tableArgs.aoColumnDefs[i];
                rowObj[col.id] = row[i];
            }
            dataArrObject.push(rowObj);
        }


        dataTarget = dataArrObject;
        $(this).data("table-data", dataTarget);
    };

    return aaData;
};

function deepFind(obj, path) {
    var paths = path
      , current = obj
      , i;
    var val;
    for (i = 0; i < paths.length; ++i) {
        val = current[paths[i]] || (current.get ? current.get(paths[i]) : undefined);
        if (val == undefined || val == null) {
            return undefined;
        } else {
            return val;
        }
    }
    return current;
}

;

DataTableRes.fnBackboneData = function(sSource, aoData, fnCallback) {
    var md = new Markdown.Converter();
    md.hooks.chain("postConversion", function (text) {
        
        return text.replace(/^<p>/, "").replace(/<\/p>$/, "");
    });
    md.hooks.chain("preConversion", function (text) {
        return text;
    });
    /*

    =========PREVIOUS CRAP FROM DATATABLES THAT I CAN'T SORT OUT==========

    */
    var inputData = $(this).data("args");

    var params = {};
    for (var i = 0; i < aoData.length; i++) {
        var param = aoData[i];
        params[param.name] = param.value;
    }


    var columns = [];
    if (inputData) {
        $(inputData.aoColumnDefs).each(function() {
            var id = (this.id);
            if (id)
                columns.push(id);
        });
    } else {

        $(this).find("th").each(function() {
            var id = DataTableRes.getIdFromTh(this);
            if (id)
                columns.push(id);
        });
    }

    var tableData = DataTableRes.BuildTableData(columns, inputData, params);

    /*
    =========GET BACKBONE INSTANCE==========
    */
    var inst = $(this).data("backbone") || eval($(this).data("data"));
    $(this).data("backbone", inst);


    /*
    =========PAGINATION DATA==========
    */
    var displayLength = params.iDisplayLength || DEFAULT_LENGTH;
    var displayStart = params.iDisplayStart || 0;

    /*
    =========ORDERING AND FILTERING==========
    */
	// SH - Only set order if not already set by prior manual designations
    // MD - this causes problems for sortable tables
	// TODO - revisit this ( SH = Revisited, issue before was determination be initial load vs. post-load sorting )
	// .. New system allows definition of initial sort via setOrder, while also allowing subsequent sorts w/o breaking queries
    if (typeof (DataTableRes.OrderInitialized) == 'undefined') DataTableRes.OrderInitialized = {};
    if( typeof( DataTableRes.OrderInitialized[ sSource ] ) != 'undefined' || ! inst._order || inst._order.length == 0 ) {
        var orderQuery = DataTableRes.BuildOrderQuery(tableData, inputData);
        console.log('orderQuery = ' + orderQuery);
        inst.setOrder(orderQuery);
    }
    if( typeof( DataTableRes.OrderInitialized[ sSource ] ) == 'undefined' )
    	DataTableRes.OrderInitialized[sSource] = true;

	// SH - Set where clause only if explicit query not already defined ( mongoQuery )
	// .. Enhanced such that when a search term is specified, the mongo query overrides the filter
	// .. and when no search term is specified, the default where condition ( if specified is used )
	if( ! inst._where || params.sSearch != '' ) { 
		var mongoQuery = DataTableRes.BuildMongoQuery(tableData, inputData, params);
		inst.setWhere(mongoQuery);
	}

    /*
    =========SETS PAGE, this is OK even if we don't know max page, we should order and filter first, though==========
    */
    if (displayLength != -1) {
        inst.setPageSize(displayLength);
        inst.setPage(Math.floor(displayStart / displayLength));
    } else {
        inst.setPageSize(inst.getCount());
        inst.setPage(0);
    }


    var self = this;

    //bind to model and redraw table if it changes!


    var filtered = false;
    //console.log(inst.toJSON());
    if (inst.filteredSlice)
        filtered = inst.filteredSlice();
    //console.log(filtered);

    var results;
    if (filtered)
        results = inst.paginatedSlice(filtered);
    else
        results = inst.paginatedSlice();


    var c = (filtered ? filtered.length : false) || inst.getCount();
    if (!inst.abstract) {
        /*
        =========IF MODEL IS LOCAL, NEED TO MANUALLY FILTER RESULTS AND RESET MAX PAGE SIZE==========
        */
        inst.setCount();

        var result = {
            "sEcho": params.sEcho,
            "iTotalRecords": inputData.iCount,
            "iTotalDisplayRecords": c,
            "aaData": []
        };
        _.each(results, function(model, i) {
            var row = [];
            for (var j = 0; j < columns.length; j++) {
                var col = columns[j];
                var dat;
                if (model[col] && $.isFunction(model[col])) {
                    dat = (model[col](i, inst));
                } else {
                    var cols = col.split(".");
                    if (cols.length == 1) {
                        dat = (model.get(col));
                    } else {
                        var obj = model.get(cols[0]);
                        dat = deepFind(obj, cols.slice(1));
                    }
                }

                if ((typeof dat) == "string") {
                    
                    if (!/<[a-z][\s\S]*>/i.test(dat)) {
                        dat = md.makeHtml(dat);
                    }
                }
                row.push(dat);
            }
            result.aaData.push(row);
        });
        fnCallback(result);


    } else {
        /*
        =========IF MODEL IS REMOTE, WEB DOES EVERYTHING FOR US==========
        -jk that won't work anymore
        */

        inst.fetch({
            error: function (response, code, xhr) {
                var result;
                if (xhr) {
                    result = JSON.parse(xhr.responseText);
                    result = result["odata.error"].message;
                } else {
                    result = response;
                }
               
                var message = result;
                var table_result = {
                    "sEcho": params.sEcho,
                    "iTotalRecords": inputData.iCount,
                    "iTotalDisplayRecords": c,
                    "sEmptyTable":message,
                    "aaData": []
                };
                fnCallback(table_result);
                $(".dataTables_empty", self).html(message);
            },
            success: function(m, result) {
                c = m.getCount();
                /* Do whatever additional processing you want on the callback, then tell DataTables */

                var table_result = {
                    "sEcho": params.sEcho,
                    "iTotalRecords": inputData.iCount,
                    "iTotalDisplayRecords": c,
                    "aaData": []
                };
                var idAttr = m.idAttribute || m.idAttr || m._idAttr;
                var ids = _.pluck(result, idAttr);
                var resultsFiltered = m.filter(function (m) {
                    return _.contains(ids, m.id);
                });

                _.each(resultsFiltered, function (me, i) {
                    var row = [];
                    for (var j = 0; j < columns.length; j++) {
                        var col = columns[j];
                        var dat;
                        if (me[col] && $.isFunction(me[col])) {
                        	dat = (me[col](i, inst));
                        } else {
                            var cols = col.split(".");
                            if (cols.length == 1) {
                            	dat = (me.get(col));
                            } else {
                            	var obj = me.get(cols[0]);
                                dat = deepFind(obj, cols.slice(1));
                            }
                        }

                        if ((typeof dat) == "string") {
                            
                            if (!/<[a-z][\s\S]*>/i.test(dat)) {
                                
                                dat = md.makeHtml(dat);
                            }
                        }
                        row.push(dat);
                    }
                    table_result.aaData.push(row);
                });

                fnCallback(table_result);

            }
        });
    }

};

DataTableRes.BuildOrderQuery = function(tableData, inputData) {

    var orderArr = [];
    for (var i in tableData) {
        var dat = tableData[i];
        var queryName = i;
        var inp = inputData.aColumns[i];
        if (inp.sQueryName)
            queryName = inp.sQueryName;
        
        if (dat.oParent)
            continue;

        if (dat.bSortable && dat.sSortDir) {
            var o = {};
            o[queryName] = dat.sSortDir == "asc" ? 1 : -1;
            //if (inp.sQueryName)
              //  o.alt = true;
            orderArr.push(o);
        }
    }

    return orderArr;
};

DataTableRes.BuildMongoQuery = function(tableData, inputData, params) {
    var filterParams = [];
    for (var i in tableData) {
        var dat = tableData[i];
        var queryName = i;
        var inp = inputData.aColumns[i];
        if (inp.sQueryName)
            queryName = inp.sQueryName;
        if (dat.oParent)
            continue;


        if (dat.bSearchable && params.sSearch) {
            var searchBroken = params.sSearch.split(":");
            var q = params.sSearch;
            var fil = {};
            if (searchBroken.length > 1 && searchBroken[1]) {
                var col = searchBroken[0].trim();
                if (col.toLowerCase() == i.toLowerCase()) {
                    q = searchBroken[1].trim();

                } else {
                    continue;
                }
            }
	        
            if (isNaN(q)) {
            	// SH - Default behavior should be contains ( indexof ) instead of startswith .. commenting line below
            	// fil[queryName] = { "$like": q + "%" };
            	fil[queryName] = { "$like": q };
			} else
				fil[queryName] = q;

            if (fil)
                filterParams.push(fil);
        }
    }

    return { $or: filterParams };
};

/*
This is not a plug-in, however, it does the job we need for communicating with an OData Service
*/
DataTableRes.BuildTableData = function(columns, inputData, params) {
    var tableData = {};
    for (var i = 0; i < columns.length; i++) {
        var col = columns[i];
        tableData[col] = {};
        var colInputObject = inputData.aColumns[col];

        tableData[col].id = col;


        if (colInputObject && colInputObject.bSearchable) {
            tableData[col].bGlobalSearch = colInputObject.bSearchable;
            tableData[col].sSearchType = colInputObject.sSearchType;

        }

        if (colInputObject && colInputObject.fnODataColumnFilter)
            tableData[col].fnODataColumnFilter = colInputObject.fnODataColumnFilter;

        if (colInputObject && colInputObject.oParent)
            tableData[col].oParent = colInputObject.oParent;


        if (params["bRegex_" + i] != null) {
            tableData[col].bRegex = params["bRegex_" + i];
        }
        if (params["bSearchable_" + i] != null) {
            tableData[col].bSearchable = params["bSearchable_" + i];
        }
        if (params["mDataProp_" + i] != null) {
            tableData[col].mDataProp = params["mDataProp_" + i];
        }

        if (params["bSortable_" + i] != null) {
            tableData[col].bSortable = params["bSortable_" + i];
        }

        for (var j = 0; j < params.iSortingCols; j++) {
            var sortColNum = params["iSortCol_" + j];
            var sortColDir = params["sSortDir_" + j];
            if (sortColNum == i) {
                //tableData[col].iSortCol = params["iSortCol_" + i];

                tableData[col].sSortDir = sortColDir;
            }
        }
    }

    return tableData;
};

DataTableRes.fnServerData = function(sSource, aoData, fnCallback) {

    /* Add some extra data to the sender */
    //aoData.push({ "name": "more_data", "value": "my_value" });
    var inputData = $(this).data("args");

    var params = {};
    for (var i = 0; i < aoData.length; i++) {
        var param = aoData[i];
        params[param.name] = param.value;
    }


    var columns = [];
    if (inputData) {
        $(inputData.aoColumnDefs).each(function() {
            var id = (this.id);
            if (id)
                columns.push(id);
        });
    } else {

        $(this).find("th").each(function() {
            var id = DataTableRes.getIdFromTh(this);
            if (id)
                columns.push(id);
        });
    }

    var tableCount = inputData.iCount;


    var tableData = DataTableRes.BuildTableData(columns, inputData, params);


    $(this).data("tableData", tableData);


    oDataParams = DataTableRes.BuildOdataQuery(params, inputData, tableData);
    var totalTable = tableCount;
    var splitUrl = sSource.split("?");
    if (splitUrl.length > 1 && splitUrl[1]) {
        sSource = splitUrl[0];
        var qParams = $.parseParams(splitUrl[1]);
        for (var i in qParams) {
            if (!oDataParams[i]) {
                oDataParams[i] = qParams[i];
            } else if (i.indexOf("filter") != -1) {
                oDataParams[i] = "({0}) and ({1})".format(oDataParams[i], qParams[i]);
            }
        }
    }

    var queryParams =
        $.ajax({
            url: sSource,
            data: oDataParams,
            method: "GET",
            dataType: 'json',
            success: function(json) {
                /* Do whatever additional processing you want on the callback, then tell DataTables */
                var c = json.d ? json.d.__count : json["odata.count"];
                var result = {
                    "sEcho": params.sEcho,
                    "iTotalRecords": totalTable,
                    "iTotalDisplayRecords": c,
                    "aaData": []
                };
                var results = json.d ? json.d.results : json.value;
                for (var i = 0; i < results.length; i++) {
                    var row = [];
                    for (var j = 0; j < columns.length; j++) {
                        var col = columns[j];
                        row.push(results[i][col]);
                    }
                    result.aaData.push(row);
                }

                fnCallback(result);
            }
        });


};

    DataTableRes.BuildOdataFilter = function (col, type, q) {
        switch (type) {
            case "D":
                var date = Date.parse(q);
                if (date) {
                    var s = "{0} gt datetime'{1}'".format(col, date.toString("yyyy-MM-dd"));
                    var e = "{0} lt datetime'{1}'".format(col, date.addDays(1).toString("yyyy-MM-dd"));
                    return "({0} and {1})".format(s,e);
                }
                return null;
            case "L":
            case "F":
            case "I":
                if (type == "I")
                    type = "";
                if (isNumber(q))
                    return "{0} eq {1}{2}".format(col, q, type);
                return null;
            case "B":
                return null;
            case "S":
            default:
                return "indexof(" + col + ",'" + q + "') gt -1";
        }

    };

    DataTableRes.BuildOdataQuery = function(params, inputData, tableData) {

        var args = [];

        var DEFAULT_LENGTH = 10;

        var displayLength = params.iDisplayLength || DEFAULT_LENGTH;
        if (displayLength != -1)
            args.push({ name: "$top", "value": displayLength });

        var displayStart = params.iDisplayStart || 0;

        args.push({ name: "$skip", "value": displayStart });


        var orderParams = [];
        var selectParams = [];
        var filterParams = [];
        var cFilterParams = [];

        if (inputData.fnODataFilter) {
            cFilterParams.push(inputData.fnODataFilter());
        }

        for (var i in tableData) {
            var dat = tableData[i];
            var inp = inputData.aColumns[i];
            if (dat.oParent)
                continue;

            if (dat.bSortable && dat.sSortDir) {
                orderParams.push(i + " " + dat.sSortDir);
            }

            if (dat.bSearchable && params.sSearch) {
                var searchBroken = params.sSearch.split(":");
                var q = params.sSearch;
                var fil = "";
                if (searchBroken.length > 1 && searchBroken[1]) {
                    var col = searchBroken[0].trim();
                    if (col.toLowerCase() == i.toLowerCase()) {
                        q = searchBroken[1].trim();
                        fil = DataTableRes.BuildOdataFilter(i, inp.sSearchType, q);
                    }
                } else {
                    fil = DataTableRes.BuildOdataFilter(i, inp.sSearchType, q);
                }

                if (fil)
                    filterParams.push(fil);
            }


            if (dat.fnODataColumnFilter) {

                var filterresult = dat.fnODataColumnFilter(i);
                if (filterresult)
                    cFilterParams.push(filterresult);
            }

            selectParams.push(i);
        }

        var filterArr = [];
        if (cFilterParams.length) {
            cFilterParams = cFilterParams.filter(function(element, index, array) { return !!element; });
            if (cFilterParams.length)
                filterArr.push("(" + cFilterParams.join(' and ') + ")");
        }


        if (filterParams.length) {
            filterParams = filterParams.filter(function(element, index, array) { return !!element; });
            if (filterParams.length)
                filterArr.push("(" + filterParams.join(' or ') + ")");
        }

        if (filterArr && filterArr.length) {
            filterArr = filterArr.filter(function(element, index, array) { return !!element; });
            if (filterArr.length)
                args.push({ name: "$filter", "value": filterArr.join(' and ') });
        }

        if (orderParams.length) {
            args.push({ name: "$orderby", "value": orderParams.join(',') });
        }
        if (selectParams.length) {
            args.push({ name: "$select", "value": selectParams.join(',') });
        }

        args.push({ name: "$inlinecount", "value": "allpages" });

        return args;

    };

    /*
    Return string, null if invalid
    */
    DataTableRes.fnProcessUpdate = function(val, e, callback, callbackTarget, callbackArguments) {

        //table cell

        var table = $(this).closest("table");
        var i = $(this).getNonColSpanIndex();
        var oTable = $(table).dataTable();
        var tr = $(this).closest("tr");
        var header = $(table).find("th:eq(" + i + ")");
        var colArgs = $(header).data("args");

        var tableArgs = $(table).data("args");
        var tableEditArgs = $(table).data("editArgs");


        var sUpdateSource = tableArgs.sCrud;
        if (colArgs.sCrudOverride)
            sUpdateSource = colArgs.sCrudOverride;


        var idColumn = tableArgs.idColumn;
        if (colArgs.oIdOverride)
            idColumn = colArgs.oIdOverride;
        var idColumnIndex = idColumn.index;


        if (idColumnIndex == null || idColumnIndex == undefined) {
            return;
        }


        var columnname = DataTableRes.getIdFromTh(header);
        /*
        TODO: Make this better
        */

        var rowData = oTable.fnGetData(tr.get(0));
        var pk = rowData[idColumnIndex];

        sUpdateSource = sUpdateSource.format(pk);


        var oDataUpdateParams = {};


        oDataUpdateParams[columnname] = val;


        $.ajax({
            url: sUpdateSource,
            data: JSON.stringify(oDataUpdateParams),
            type: "MERGE",
            context: this,
            contentType: "application/json",
            dataType: 'json',
            success: function(json) {
                /* Do whatever additional processing you want on the callback, then tell DataTables */

                callback.apply(callbackTarget, callbackArguments);
            }
        });

        return val;
    };

    DataTableRes.parseAndRender = function (jq) {
        var self = jq;
        var inputArgs = DataTableRes.DomToTableArgs(self);
        var args = DataTableRes.buildPluginArguments(self, inputArgs);
        var editArgs = DataTableRes.buildEditPluginArguments(self, args);
        //{ sUpdateURL: DataTable.fnProcessUpdate }
        args.bFilter=true;
        
        var tools;
        if (tools = $(self).data("tools")) {
            args.oTableTools.aButtons = tools;
        }
        var sortcol = false;        
        for (var q in args.aColumns) {            
            if (args.aColumns.sSortDir) {
                sortcol = q;
                break;
            }
        }
        if(!sortcol)
            for (var q in args.aColumns) {
                if (args.aColumns[q].bIdColumn) {
                    args.aColumns[q].sSortDir = "asc";
                    sortcol = q;
                }
            }
        for (var i = 0; i < args.aoColumnDefs.length; i++) {
            if (args.aoColumnDefs[i].id == sortcol) {
                args.aoColumnDefs[i].sSortDir = "asc";
            }
        }
        var inst;
        if (args.sMode == "backbone") {
            inst = $(self).data("backbone") || eval($(self).data("data"));
            $(self).data("backbone", inst);
            if (!inst.abstract) {
                inst.bind("add remove refresh where:change", function () {

                    var dt = $(self).dataTable().fnDraw();

                });
            } else {
	            try {
                inst.bind("add remove refresh where:change", function () {

                    //var dt = $(self).dataTable().fnDraw();

                });
	            } catch ( ex ) {}
            }
            inst.bind("redraw", function () {
                
                var dt = $(self).dataTable().fnDraw();
                //var dt = $(self).dataTable();
            });
        }
        ;
        args.fnPreDrawCallback = function() {
            $(this).trigger("predraw", this);
            if (inst)
                inst.trigger("table:predraw", this);
        };
        args.fnDrawCallback = function() {
            $(this).trigger("draw");
            if (inst)
                inst.trigger("table:draw", this);
            if ($("td input[type='checkbox']", this).length && !$("th input", this).length) {
                var i = $("td input", this).closest("td").index();
                $("th:eq({0}) div".format(i), this).html("<input type='checkbox' />");
            }

            if (!$("td input:checked", this).length)
                $("th input:checkbox", this).removeAttr('checked');
            else
                $("th input:checkbox", this).attr('checked', 'checked');

            $("th input:checkbox", this).unbind('change').change(function() {
                var i = $(this).closest("th").index();
                if ($(this).attr("checked")) {
                    $(this).closest("table").find("tr").find("td:eq({0}) input:checkbox".format(i)).attr('checked', "checked").trigger("change");

                } else {
                    $(this).closest("table").find("tr").find("td:eq({0}) input:checkbox".format(i)).removeAttr('checked').trigger("change");
                }
            });
            $('input:checkbox', this).change(function() {
                if (inst)
                    inst.trigger("table:checkbox", this);
            });
            $(".select-wrapper").each(function() {
                $("tr", this).each(function() {

                    $(this).click(function() {
                        $(this).find("input").each(function() {
                            if ($(this).attr("type") == "radio") {
                                $(this).attr("checked", "checked");
                            } else if ($(this).attr("type") == "checkbox") {
                                /*var ischecked = $(this).is(":checked");
                                if (ischecked) {
                                    $(this).removeAttr("checked");
                                } else {
                                    $(this).attr("checked", "checked");
                                }*/
                            }
                        });
                    });
                });
            });
        };
        
        if (args.sCrud)
            $(self).dataTable(args).fnSetFilteringDelay(DataTableRes.DELAY).fnSetJqUiHover().makeEditable(editArgs);
        else
            $(self).dataTable(args).fnSetFilteringDelay(DataTableRes.DELAY).fnSetJqUiHover();
    };