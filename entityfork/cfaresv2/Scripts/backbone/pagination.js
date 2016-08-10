/* 
 * backbone.pagination.js v0.9
 * Copyright (C) 2012 Philipp Nolte
 * backbone.pagination.js may be freely distributed under the MIT license.
 */

(function (window) {
    "use strict";

    // Alias backbone, underscore and jQuery.
    var Backbone = window.Backbone,
        _ = window._,
        $ = window.$;

    // Define the pagination enale method under the Pagination namespace.
    Backbone.Pagination = {

        // Called when enabling pagination on a Backbone.Collection.
        enable: function (collection, model, config) {
            
            _.extend(collection, Backbone.Pagination.Paginator)
            
            collection.on("order:change where:change pagination:change", collection.resetPagination);
            
            if (config) {
                _.extend(collection.paginationConfig, config);
            }
        }
    };

    // Define all the pagination methods available.
    Backbone.Pagination.Paginator = {

        // The current page displayed -- defaults to page 1.
        currentPage: 0,

        totalCount:1000,

        // Pagination configuration can be overwritten anytime.
        paginationConfig: {
            pretty: false,  // enable pretty urls url/page/2/ipp/20
            ipp: 20,     // items per page
            page_attr: 'page',
            ipp_attr: 'ipp',  // will result in a query like page=4&ipp=20            
            fetchOptions: {}      // any options handed over to the fetch method
        },
        _pageCache:{},
        refreshModelPages: function () {
        },
        
        pullCache: function () {
            return this._pageCache["$" + this.currentPage];
        },
        paginatedSlice: function (data) {
            var start = this.currentPage * this.paginationConfig.ipp;
            return (data || this.toArray()).slice(start, start + this.paginationConfig.ipp);
        },
        paginatedJSON: function (data) {
            var start = this.currentPage * this.paginationConfig.ipp;
            return (data || this.toJSON()).slice(start, start + this.paginationConfig.ipp);
        },
        
        // Load the page number given.
        fetchPage: function (page, options) {
            
            page=this.setPage(page);
            
            
            return this.fetch(this,options);
        },
        _oldPageSize:null,
        setPageSize: function (s) {            
            this.paginationConfig.ipp = s;
            if(this._oldPageSize!=s)
                this.trigger("pagination:change", this.paginationConfig);
            this._oldPageSize = s;
            return this.paginationConfig.ipp;
        },
        fetchNextPage: function (options) {
            return this.fetchPage(this.nextPage(), options);
        },
        fetchPrevPage: function (options) {
            return this.fetchPage(this.prevPage(),options);
        },
        maxPage: function () {
            var p = Math.floor(this.totalCount / this.paginationConfig.ipp);
            console.log(this);
            return p;
        },
        // Set the page number given.
        setPage: function (page) {
            if (page == null || page == undefined)
                page = this.currentPage;
            if (page == "next") {
                return this.nextPage();
            }else if(page=="prev"){
                return this.prevPage();
            }


            var toPage = (page >= 0) ? (page <= this.maxPage() ? page : this.maxPage()) : 0;

            if (toPage != this.currentPage) {
                this._pageCache["$"+this.currentPage]=this.toJSON();
            }

            this.currentPage = toPage;
            return this.currentPage;
        },
        resetPagination: function (e) {
            console.log("reset");
            if(this.comparator)
                this.sort();
            this._pageCache = {};
            this.totalCount = 1000;
            //this.each(function (m) { m.set("__page", false); });
            this.currentPage = 0;

        },
        // Set the page number given.
        setCount: function (c) {
            //if (c != this.totalCount)
              //  this.resetPagination();
            if (!c)
                this.totalCount = this.size();
            else
                this.totalCount = c;
            
           
        },
        getCount: function () {
            return this.totalCount;
        },
        // Load the next page.
        nextPage: function () {
            return this.setPage(this.currentPage + 1);
             
        },
        // Load the page number given.
        lastPage: function () {
            return Math.ceil(this.totalCount / this.currentPage);
        },

        // Load the previous page.
        previousPage: function () {
            return this.setPage(this.currentPage - 1);
        },
        // Load the previous page.
        prevPage: function () {
            return this.previousPage();
        },

        // The url function will append the page and ipp attribute to the result
        // of an baseUrl property or function (if it exists). Note, that
        // this url function will override any previous defined url function.
        paginator: function (options) {
            var url = options.url;
            
            if (options.page) {
                switch (options.page) {
                    case "next":
                        this.nextPage();
                        break;
                    case "prev":
                        this.previousPage();
                        break;
                    default:
                        this.setPage(options.page);
                        break;
                }
            }
            
            $.extend(this.paginationConfig,options)
            var pagination = { page: this.currentPage, page_size: this.paginationConfig.ipp };
            var params = { "$inlinecount": "allpages", "$top": (pagination.page_size), "$skip": (pagination.page_size * pagination.page) }
            return url + ((url.indexOf('?') === -1) ? '?' : '&') + $.param(params);
        }

    }

    // Provide a PaginatedCollection constructor that extends Backbone.Collection.
    Backbone.PaginatedCollection = Backbone.Collection.extend(Backbone.Pagination.Paginator);

})(this);