var extendedDataTables = (function () {
    var abstractFactory = function () {

        this.createTable = function (params) {

            var abstractExtendedDataTable = function () {

                this.tableProfile = {};
                /*
                You can create three types of column profiles
                1. auto-mapped: use the data-column-name attribute to simply 
                pull data from the source column
                -optionally use the sType attribute to format and edit common data types
                2. custom-data: use the requested-columns, onRender, onEdit and customOData
                attributes to get, render, edit and query composite data types (date range,
                full name, etc.)
                3. utility: use the fnRender and onClick attributes to create a utility
                column (expand/contract details, delete, export, etc.)
                */
                this.columnProfiles = [];
                this.createColumnProfile = function (columnName, type, params) {
                    this.columnProfiles[columnName] = { "columnName": columnName, "type": type };
                    return columnProfiles[columnName];
                };

                this.dataProfiles = [];
                this.createDataProfile = function (dataName, params) {
                    return this.dataProfiles[dataName] = { "dataName": dataName };
                }

                this.filterControlForm = null;

                this.dataTable = null;
            };

            return new abstractExtendedDataTable();
        };

        this.renderFunction = [];

        this.onEditFunction = [];

    };

    //------------------------------------------------
    //EVERYTHING BELOW THIS POINT IS AN IMPLEMENTATION OF ABOVE abstractFactory class and abstractExtendedDataTable
    //------------------------------------------------
    var implementedFactory = function () { };
    implementedFactory.prototype = new abstractFactory();
    implementedFactory.constructor = implementedFactory;
    implementedFactory.prototype.superClass = abstractFactory.protype;

    //Private static methods

    var setUpDefaultDataTableValues = function () { };
    var createTableProfile = function (domTableRef, argumentProfile, destinationProfile) { };
    var createColumnProfiles = function (domTableRef, argumentProfiles, destinationProfiles) { };
    var createDataProfiles = function (rawData, dataProfile) { };
    var setUpAutoMappedColumns = function (dataProfiles, columnProfiles) { };
    var setUpCustomDataColumns = function (dataProfiles, columnProfiles) { };
    var setUpUtilityDataColumns = function (extendedDataTable) { };
    var buildOdataQuery = function (interceptedData, columnProfiles) { };

    //this is where our factory gives birth to a new extendedDataTable object
    implementedFactory.prototype.createTable = function (params) {
        var implementedTableClass = function () { };

        //inherit from the abstract extendedTable stub-class created by our abstract factory
        var abstractTable = this.superClass.createTable.call(this);
        implementedTableClass.prototype = abstractTable;
        implementedTableClass.constructor = abstractTable;
        implementedTableClass.prototype.superClass = abstractTable;

        var implementedTableObj = new implementedTableClass();


        var dataTableArguments = setupDefaultDataTableValues();

        createTableProfile(params.tableDom, params.tableProfile, implementedTableObj.tableProfile);



        createColumnProfiles(params.tableDom, params.columnProfiles, implementedTableObj.columnProfiles);

        setUpAutoMappedColumns(implementedTableObj.dataProfiles, implementedTableObj.columnProfiles);
        setUpCustomDatacolumns(implementedTableObj.dataProfiles, implementedTableObj.columnProfiles);
    };

    setUpDefaultDataTableValues = function () {
        var dataTableArguments = {};
        dataTableArguments.bProcessing = true;
        dataTableArguments.bServerSide = true;
        dataTableArguments.bJQueryUI = true;
        dataTableArguments.sPaginationType = "full_numbers";

    }

    createTableProfile = function (domTableRef, argumentProfile, destinationProfile) {
        
        var ajaxSource = $(domTableRef).attr("data-sAjaxSource");
        if ($(domTableRef).attr("data-useOData") == "false") {
            useOData = false;
        }

    };

    //static methods buildODataQuery, build
    buildOdataQuery = function (rawData, columnParams) {

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

    };

    var setUpColumnMapper = function (params, columnMapper) {
        //init
        columnMapper = {
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
    };

    return new implementedFactory();
})();