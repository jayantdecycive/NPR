alert("CfaAdmin.js is used for development only. Please use plugins.js for production");

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

var DataTable = {};

DataTable.getIndexFromId = function (id, table) {
    var index = -1;
    if ($(table).is('th'))
        table = $(table).closest("table");
    $(table).find("th").each(function (i) {
        var thid = DataTable.getIdFromTh(this);
        if (thid == id)
            index = i;
    });
    return index;
}

DataTable.getIdFromTh = function (th) {
    return $(th).attr("data-id") || $(th).attr("data-column-name") || $(th).attr("id") || $(th).html().replace(/[^0-9A-Z]/gi, "");
}
DataTable.getThFromId = function (table, id) {
    if ($(table).is('th'))
        table = $(table).closest("table");
    var th = null;
    $(table).find("th").each(function () {
        var thid = DataTable.getIdFromTh(this);
        if (thid == id)
            th = this;
    });
    return th;
}

DataTable.DomToTableArgs = function ($that) {
    $that = $($that);
    var outputArgs = {};
    var argTemplate = {
        mData: { key: "data-data" },
        sCrud: { key: "data-crud" },
        bOData: { key: "data-odata", parse: Convert.toBoolean },        
        iDisplayLength: { key: "data-display-length", parse: Convert.toInt },
        iDisplayStart: { key: "data-display-start", parse: Convert.toInt },
        sServerMethod: { key: "data-server-method"}
    };

    var argColTemplate = {
        bVisible: { key: "data-visible", parse: Convert.toBoolean },
        bSearchable: { key: "data-searchable", parse: Convert.toBoolean },
        bSortable: { key: "data-sortable", parse: Convert.toBoolean },
        bEditable: { key: "data-editable", parse: Convert.toBoolean },

        mEditType: { key: "data-edit-type", parse: function (arg) { return Convert.toFunction(arg) || arg; } },
        mEditData: { key: "data-edit-data", parse: function (arg) { return Convert.toFunction(arg) || arg; } },
        fnOnEdit: { key: "data-on-edit", parse: Convert.toFunction },
        fnOnSubmit: { key: "data-on-submit", parse: Convert.toFunction },

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

    var isUrl = outputArgs.mData.match(/(https?:\/\/)?.+/);

    if (isUrl) {
        outputArgs.sAjaxSource = outputArgs.mData;
        outputArgs.sPaginationType = "full_numbers";
    } else {
        outputArgs.aData = eval(outputArgs.aaData);
    }

    outputArgs.aColumns = {};

    $that.find("th").each(function () {
        var id = DataTable.getIdFromTh(this);
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
        outputArgs.aColumns[id] = colObject;
    });



    return outputArgs;
}

DataTable.defaultInputArgs =
{                
    bOData: true,
    iCount:10000,
    bProcessing: true,
    bServerSide: true,
    bJQueryUI: true
};

DataTable.defaultEditArgs =
{
    bOData: true,
    iCount: 10000,
    bProcessing: true,
    bServerSide: true,
    bJQueryUI: true
};

DataTable.defaultColInputArgs =
{
    bSortable: true,
    bVisible: true,
    bEditable: true,
    bIdColumn: false,
    fnODataColumnFilter: function (sourceObject, val, row, column) {
        return null;
    }
};

DataTable.defaultColEditArgs =
{
    
    type:"text",
    data:null
};

DataTable.defaultMData =
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

DataTable.buildEditPluginArguments = function (table, args) {

    $this = $(table);

    var tableArgs = $.extend(true, {}, DataTable.defaultEditArgs, args);
    tableArgs.sUpdateURL = DataTable.fnProcessUpdate;

    var colArr = [];
    $this.find("th").each(function () {

        var colEditObject = null;
        for (var i = 0; i < args.aoColumnDefs.length; i++) {
            var colArgObject = args.aoColumnDefs[i];
            var dtId = DataTable.getIdFromTh(this);

            if (colArgObject.id == dtId) {
                colEditObject = $.extend(true, {}, DataTable.defaultColEditArgs, colArgObject);
                
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
}

DataTable.buildPluginArguments = function (table, inputArgs) {

    $this = $(table);

    var tableArgs = $.extend(true, {}, DataTable.defaultInputArgs, inputArgs);
    tableArgs.oLanguage = { sInfoFiltered: "" };
    var colArr = [];
    for (var i in tableArgs.aColumns) {
        var colInput = tableArgs.aColumns[i];
        var th = DataTable.getThFromId($this, i);
        var colIndex = DataTable.getIndexFromId(i, $this);
        var colObject = $.extend(true, {}, DataTable.defaultColInputArgs, colInput);


        colObject.id = i;




        if (typeof (colObject.mData) == 'object') {




            colObject.mDataProp = function (source, type, val) {


                /*TODO SORT THIS OUT*/
                var th = this;
                var args = $(th).data("args");
                var colMData = $.extend(true, {}, DataTable.defaultMData, args.mData);

                var oParent = args.oParent;

                if (oParent)
                    th = DataTable.getThFromId(th, oParent.id);

                var id = DataTable.getIdFromTh(th);
                var col = DataTable.getIndexFromId(id, th);

                var defaultVal = source[col];

                switch (type) {
                    case "set":
                        var setted = colMData.fnOnSet.call(th, source, val, col);
                        return setted;
                    case "filter":

                        return colMData.fnOnFilter.call(th, source, defaultVal, col);
                    case "type":

                        return colMData.fnOnType.call(th, source, defaultVal, col);
                    case "sort":

                        return colMData.fnOnSort.call(th, source, defaultVal, col);
                    case "display":
                    default:
                        
                        return colMData.fnOnDisplay.call(th, source, defaultVal, col);
                }
            };
        } else {
            colObject.mDataProp = colObject.mData;
        }

        colObject.aTargets = [colIndex];




        $(th).data("args", colObject);
        colArr.push(colObject);
    }

    for (var i = 0; i < colArr.length; i++) {
        var colObject = colArr[i];
        if (colObject.oParent && typeof (colObject.oParent) == 'string') {
            for (var j = 0; j < colArr.length; j++) {
                if (colArr[j].id == colObject.oParent) {
                    colObject.oParent = colArr[j];
                    colObject.bSortable = false;
                    colObject.bSearchable = false;
                    colObject.bEditable = false;                    
                }
            }

        }
        if (colObject.bIdColumn)
            tableArgs.idColumn = colObject;
    }

    if (tableArgs.bOData)
        tableArgs.fnServerData = DataTable.fnServerData;

    if (colArr.length)
        tableArgs.aoColumnDefs = colArr;

    
    $this.data("args", tableArgs);
    return tableArgs;
}


/*
This is not a plug-in, however, it does the job we need for communicating with an OData Service
*/
DataTable.fnServerData = function (sSource, aoData, fnCallback) {
    /* Add some extra data to the sender */
    //aoData.push({ "name": "more_data", "value": "my_value" });
    var inputData = $(this).data("args");

    var params = {};
    for (var i = 0; i < aoData.length; i++) {
        var param = aoData[i];
        params[param.name] = param.value;
    }


    var columns = [];
    $(this).find("th").each(function () {
        var id = DataTable.getIdFromTh(this);
        if (id)
            columns.push(id);
    });

    var tableCount = inputData.iCount;


    var tableData = {};

    for (var i = 0; i < columns.length; i++) {
        var col = columns[i];
        tableData[col] = {};
        var colInputObject = inputData.aColumns[col];

        tableData[col].id = col;

        if (colInputObject && colInputObject.bSearchable)
            tableData[col].bGlobalSearch = colInputObject.bSearchable;

        if (colInputObject && colInputObject.fnODataColumnFilter)
            tableData[col].fnODataColumnFilter = colInputObject.fnODataColumnFilter;

        if (colInputObject && colInputObject.oParent)
            tableData[col].oParent = colInputObject.oParent;



        if (params["sSearch_" + i] != null) {
            tableData[col].sSearch = params["sSearch_" + i];
        }
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

    $(this).data("tableData", tableData);

    oDataParams = DataTable.BuildOdataQuery(params, tableData);
    var totalTable = tableCount;

    $.ajax({
        url: sSource,
        data: oDataParams,
        method: "GET",
        dataType: 'json',
        success: function (json) {
            /* Do whatever additional processing you want on the callback, then tell DataTables */
            console.log(json);
            console.log(totalTable);
            var result = {
                "sEcho": params.sEcho,
                "iTotalRecords": totalTable,
                "iTotalDisplayRecords": json.d.__count,
                "aaData": []
            };
            for (var i = 0; i < json.d.results.length; i++) {
                var row = [];
                for (var j = 0; j < columns.length; j++) {
                    var col = columns[j];
                    row.push(json.d.results[i][col]);
                }
                result.aaData.push(row);
            }
            console.log(result);
            fnCallback(result);
        }
    });
}

DataTable.BuildOdataQuery = function (params, tableData) {

    var args = [];

    var DEFAULT_LENGTH = 10;

    var displayLength = params.iDisplayLength || DEFAULT_LENGTH;

    args.push({ name: "$top", "value": displayLength });

    var displayStart = params.iDisplayStart || 0;

    args.push({ name: "$skip", "value": displayStart });

    var orderParams = [];
    var selectParams = [];
    var filterParams = [];
    var cFilterParams = [];

    for (var i in tableData) {
        var dat = tableData[i];
        if (dat.oParent)
            continue;

        if (dat.bSortable && dat.sSortDir) {
            orderParams.push(i + " " + dat.sSortDir);
        }

        if (dat.bSearchable && dat.sSearch) {
            filterParams.push("indexof(" + i + ",'" + dat.sSearch + "') gt -1");
        }
        if (dat.bGlobalSearch && params.sSearch)
            filterParams.push("indexof(" + i + ",'" + params.sSearch + "') gt -1");

        if (dat.fnODataColumnFilter) {
            
            var filterresult = dat.fnODataColumnFilter(i);
            if (filterresult)
                cFilterParams.push(filterresult);
        }

        selectParams.push(i);
    }

    var filterArr = [];
    if (cFilterParams.length) {
        filterArr.push("(" + cFilterParams.join(' and ') + ")");
    }


    if (filterParams.length) {
        filterArr.push("(" + filterParams.join(' or ') + ")");
    }

    if (filterArr) {
        args.push({ name: "$filter", "value": filterArr.join(' and ') });
    }

    if (orderParams.length) {
        args.push({ name: "$orderby", "value": orderParams.join(',') });
    }
    if (selectParams.length) {
        args.push({ name: "$select", "value": selectParams.join(',') });
    }

    args.push({ name: "$inlinecount", "value": "allpages" });
    console.log(args);
    return args;

}

/*
Return string, null if invalid
*/
DataTable.fnProcessUpdate = function (val, e, callback, callbackTarget, callbackArguments) {
    console.log(e);
    //table cell


    var table = $(this).closest("table");
    var i = $(this).getNonColSpanIndex();
    var header = $(table).find("th:eq(" + i + ")");

    var tableArgs = $(table).data("args");
    var tableEditArgs = $(table).data("editArgs");

    var sUpdateSource = tableArgs.sCrud;
    var idColumn = tableArgs.idColumn;
    var idColumnIndex = DataTable.getIndexFromId(idColumn.id,table);

    if (idColumnIndex == null || idColumnIndex == undefined) {
        return;
    }

    console.log(idColumnIndex);

    var columnname = DataTable.getIdFromTh(header);
    /*
    TODO: Make this better
    */
    var rowid = $(this).closest("tr").find("td:eq(" + idColumnIndex + ")").html();

    sUpdateSource = sUpdateSource.format(rowid);
    

    var oDataUpdateParams = {};

    

    oDataUpdateParams[columnname] = val;

    //console.log(sUpdateSource);
    //console.log(oDataUpdateParams);

    $.ajax({
        url: sUpdateSource,
        data: JSON.stringify(oDataUpdateParams),
        type: "MERGE",
        context: this,
        contentType: "application/json",
        dataType: 'json',
        success: function (json) {
            /* Do whatever additional processing you want on the callback, then tell DataTables */
            console.log(json);
            callback.apply(callbackTarget, callbackArguments);
        }
    });

    return val;
}