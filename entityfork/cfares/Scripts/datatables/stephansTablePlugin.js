function dataTableFactory(factoryInitVars) {

    var factory = this;
    this.customMDataProp = [];
    this.customOData = [];


    var setUpColumnMapper = function (params) {
        var columnMapper = {
            "_initialized": false
        };


        for (i = 0; i < params.aoColumns.length; i++) {


            params.aoColumns[i].mDataProp = function (existingMDataProp) {

                var mDataPropAnonymous;
                if (existingMDataProp) {
                    mDataPropAnonymous = function (source, type, val) {
                        source.columnMapper = columnMapper;
                        return existingMDataProp(source, type, val);
                    }
                } else {
                    var columnName = params.aoColumns[i].dataName;

                    if (columnName) {
                        mDataPropAnonymous = function (source, type, val) {

                            var mappedColumnIndex = columnMapper[columnName];
                            if (mappedColumnIndex == undefined) {
                                return null;
                            }
                            
                            return source[mappedColumnIndex];
                        }

                    } else {

                        mDataPropAnonymous = function (source, type, val) {
                            return null;
                        };
                    }
                }

                return mDataPropAnonymous;

            } (params.aoColumns[i].mDataProp);
        }

        return columnMapper;
    }

    var buildOdataQuery = function (rawData, columnParams) {

        var mappedaoData = {};
        for (var nameValueIndex in rawData) {
            mappedaoData[rawData[nameValueIndex].name] = rawData[nameValueIndex].value;
        }
        console.log(mappedaoData);
        var args = [];

        var DEFAULT_LENGTH = 10;

        var displayLength = mappedaoData.iDisplayLength || DEFAULT_LENGTH;

        args.push({ name: "$top", "value": displayLength });

        var displayStart = mappedaoData.iDisplayStart || 0;

        args.push({ name: "$skip", "value": displayStart });

        var orderParams = [];
        var selectParams = [];
        var filterParams = [];
        var cFilterParams = [];

        var numberOfSortingColumns = mappedaoData["iSortingCols"];
        if (numberOfSortingColumns) {
            for (i = 0; i < numberOfSortingColumns; i++) {
                var columnIndex = mappedaoData["iSortCol_" + i];
                var columnData = columnParams.aoColumns[columnIndex];
                if (columnData.dataName) {
                    orderParams.push(columnData.dataName + " " + mappedaoData["sSortDir_" + i]);
                }
                
            }
        }

        for (i = 0; i < columnParams.aoColumns.length; i++) {
            var columnData = columnParams.aoColumns[i];


            if (columnData.bSortable && mappedaoData["sSortDir_" + i]) {
                orderParams.push(columnData.dataName + " " + mappedaoData["sSortDir_" + i]);
            }
            if (columnData.bSearchable && mappedaoData["sSearch_" + i]) {
                filterParams.push("indexof(" + columnData.dataName + ",'" + mappedaoData["sSearch_" + i] + "') gt -1");
            }
            if (columnData.bGlobalSearchable && mappedaoData.sSearch) {
                if (columnData.dataName) {
                    filterParams.push("indexof(" + columnData.dataName + ",'" + mappedaoData.sSearch + "') gt -1");
                }
            }

            // This may not be efficient, but we need a way of accessing non displayed columns
           // if (columnData.dataName) {
           //     selectParams.push(columnData.dataName);
           // }

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
        return args;

    }

    this.createParams = function (paramOptions) {


        if (!paramOptions.params) {
            paramOptions.params = {
                "aoColumns": []
            };
        } else if (!paramOptions.params.aoColumns) {
            paramOptions.params.aoColumns = [];
        }
        //This must be set to these values for this plugin
        paramOptions.params.bProcessing = true;
        paramOptions.params.bServerSide = true;
        paramOptions.params.bJQueryUI = true;

        var useOData = true;
        var ajaxSource;
        if (paramOptions.tableTemplate) {
            ajaxSource = $(paramOptions.tableTemplate).attr("data-sAjaxSource");
            if ($(paramOptions.tableTemplate).attr("data-useOData") == "false") {
                useOData = false;
            }
        }

        if (!(paramOptions.useOData == undefined)) {
            useOData = paramOptions.useOData;
        }

        if (!paramOptions.params.sAjaxSource) {
            paramOptions.params.sAjaxSource = ajaxSource;
        }

        if (paramOptions.tableTemplate) {
            var columnIndex = 0;
            $(paramOptions.tableTemplate).find("th").each(function () {
                if (!paramOptions.params.aoColumns[columnIndex]) {
                    paramOptions.params.aoColumns[columnIndex] = {};
                }

                var convertToBoolean = function (string) {
                    if (!(string == undefined)) {
                        if (string == "true") {
                            return true;
                        } else {
                            return false;
                        }
                    }
                    
                }


                var dataName = $(this).attr("data-column-name");
                var bSortable = convertToBoolean($(this).attr("data-bSortable"));
                var bVisible = convertToBoolean($(this).attr("data-bVisible"));
                var bSearchable = convertToBoolean($(this).attr("data-bSearchable"));
                var bGlobalSearchable = convertToBoolean($(this).attr("data-bGlobalSearchable"));
                var sClass = $(this).attr("data-sClass");
                var sType = $(this).attr("data-sType");
                var sDefaultContent = $(this).attr("data-sDefaultContent");
                var mDataProp = factory.customMDataProp[$(this).attr("data-mDataProp")];
                var customOData = factory.customMDataProp[$(this).attr("data-customOData")];

                if (!(dataName == undefined)) {
                    paramOptions.params.aoColumns[columnIndex].dataName = dataName;
                }
                if (!(bSortable == undefined)) {
                    paramOptions.params.aoColumns[columnIndex].bSortable = bSortable;
                } else {
                    paramOptions.params.aoColumns[columnIndex].bSortable = false;
                }
                if (!(bVisible == undefined)) {
                    paramOptions.params.aoColumns[columnIndex].bVisible = bVisible;
                } else {
                    paramOptions.params.aoColumns[columnIndex].bVisible = true;
                }
                if (!(bSearchable == undefined)) {
                    paramOptions.params.aoColumns[columnIndex].bSearchable = bSearchable;
                } else {
                    paramOptions.params.aoColumns[columnIndex].bSearchable = false;
                }
                if (!(bGlobalSearchable == undefined)) {
                    paramOptions.params.aoColumns[columnIndex].bGlobalSearchable = bGlobalSearchable;
                } else {
                    paramOptions.params.aoColumns[columnIndex].bGlobalSearchable = false;
                }

                if (!(sClass == undefined)) {
                    paramOptions.params.aoColumns[columnIndex].sClass = sClass;
                }
                if (!(sType == undefined)) {
                    paramOptions.params.aoColumns[columnIndex].sType = sType;
                }
                if (!(sDefaultContent == undefined)) {
                    paramOptions.params.aoColumns[columnIndex].sDefaultContent = sDefaultContent;
                }
                if (!(mDataProp == undefined)) {
                    paramOptions.params.aoColumns[columnIndex].mDataProp = mDataProp;
                }

                if (!(customOData == undefined)) {
                    paramOptions.params.aoColumns[columnIndex].customOData = customOData;
                }

                columnIndex++;
            });
        }

        var columnMapper = setUpColumnMapper(paramOptions.params);

        var mapColumns = function (data) {
            if (!columnMapper._initialized) {
                columnMapper._initialized = true;
                var columnNameStrings = data.sColumns.split(",");
                for (i = 0; i < columnNameStrings.length; i++) {
                    columnMapper[columnNameStrings[i]] = i;
                }
            }
        };
        var overridenServerCall;
        if (useOData) {

            overridenServerCall = function (sSource, aoData, fnCallback) {
                oDataParams = buildOdataQuery(aoData, paramOptions.params);
                var totalTable = 10000;

                $.ajax({
                    url: sSource,
                    data: oDataParams,
                    method: "GET",
                    dataType: 'json',
                    success: function (json) {
                        /* Do whatever additional processing you want on the callback, then tell DataTables */
                        console.log(json);

                        var result = {
                            "sEcho": aoData.sEcho,
                            "iTotalRecords": totalTable,
                            "iTotalDisplayRecords": json.d.__count,
                            "aaData": []
                        };

                        var totalColumns = 0;
                        if (json.d.results[0]) {
                            var nameIndex = 0;
                            for (var name in json.d.results[0]) {
                                columnMapper[name] = nameIndex;
                                nameIndex++;
                            }
                            totalColumns = nameIndex + 1;
                        }

                        for (var i = 0; i < json.d.results.length; i++) {
                            var row = [];
                            for (var name in json.d.results[i]) {
                                row.push(json.d.results[i][name]);
                            }
                            result.aaData.push(row);
                        }
                        console.log(result);
                        fnCallback(result);
                    }
                });

            }
        } else {

            overridenServerCall = function (sSource, aoData, fnCallback) {
                $.ajax({
                    "dataType": 'json',
                    "type": "GET",
                    "url": sSource,
                    "data": aoData,
                    "success": function (data, textStatus, jqXHR) {
                        console.log(data);
                        mapColumns(data);
                        fnCallback(data, textStatus, jqXHR);
                    }
                });
            };



        }
        paramOptions.params.fnServerData = overridenServerCall;
        return paramOptions.params;

    };

}