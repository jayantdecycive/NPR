/**
* $.parseParams - parse query string paramaters into an object.
*/
(function ($) {
    var re = /([^&=]+)=?([^&]*)/g;
    var decodeRE = /\+/g;  // Regex for replacing addition symbol with a space
    var decode = function (str) { return decodeURIComponent(str.replace(decodeRE, " ")); };
    $.parseParams = function (query) {
        var params = {}, e;
        while (e = re.exec(query)) {
            var k = decode(e[1]), v = decode(e[2]);
            if (k.substring(k.length - 2) === '[]') {
                k = k.substring(0, k.length - 2);
                (params[k] || (params[k] = [])).push(v);
            }
            else params[k] = v;
        }
        return params;
    };
})(jQuery);

if (!Array.prototype.filter) {
    Array.prototype.filter = function (fun /*, thisp*/) {
        var len = this.length;
        if (typeof fun != "function")
            throw new TypeError();

        var res = new Array();
        var thisp = arguments[1];
        for (var i = 0; i < len; i++) {
            if (i in this) {
                var val = this[i]; // in case fun mutates this
                if (fun.call(thisp, val, i, this))
                    res.push(val);
            }
        }

        return res;
    };
}

/*
* jQuery UI selectmenu
*
* Copyright (c) 2009 AUTHORS.txt (http://jqueryui.com/about)
* Dual licensed under the MIT (MIT-LICENSE.txt)
* and GPL (GPL-LICENSE.txt) licenses.
*
* http://docs.jquery.com/UI
*/

(function ($) {

    $.widget("ui.selectmenu", {
        _init: function () {
            var self = this, o = this.options;

            //quick array of button and menu id's
            this.ids = [this.element.attr('id') + '-' + 'button', this.element.attr('id') + '-' + 'menu'];

            //define safe mouseup for future toggling
            this._safemouseup = true;

            //create menu button wrapper
            this.newelement = $('<a class="' + this.widgetBaseClass + ' ui-widget ui-state-default ui-corner-all" id="' + this.ids[0] + '" role="button" href="#" aria-haspopup="true" aria-owns="' + this.ids[1] + '"></a>')
			.insertAfter(this.element);

            //transfer tabindex
            var tabindex = this.element.attr('tabindex');
            if (tabindex) { this.newelement.attr('tabindex', tabindex); }

            //save reference to select in data for ease in calling methods
            this.newelement.data('selectelement', this.element);

            //menu icon
            this.selectmenuIcon = $('<span class="' + this.widgetBaseClass + '-icon ui-icon"></span>')
			.prependTo(this.newelement)
			.addClass((o.style == "popup") ? 'ui-icon-triangle-2-n-s' : 'ui-icon-triangle-1-s');


            //make associated form label trigger focus
            $('label[for=' + this.element.attr('id') + ']')
			.attr('for', this.ids[0])
			.bind('click', function () {
			    self.newelement[0].focus();
			    return false;
			});

            //click toggle for menu visibility
            this.newelement
			.bind('mousedown', function (event) {
			    self._toggle(event);
			    //make sure a click won't open/close instantly
			    if (o.style == "popup") {
			        self._safemouseup = false;
			        setTimeout(function () { self._safemouseup = true; }, 300);
			    }
			    return false;
			})
			.bind('click', function () {
			    return false;
			})
			.keydown(function (event) {
			    var ret = true;
			    switch (event.keyCode) {
			        case $.ui.keyCode.ENTER:
			            ret = true;
			            break;
			        case $.ui.keyCode.SPACE:
			            ret = false;
			            self._toggle(event);
			            break;
			        case $.ui.keyCode.UP:
			        case $.ui.keyCode.LEFT:
			            ret = false;
			            self._moveSelection(-1);
			            break;
			        case $.ui.keyCode.DOWN:
			        case $.ui.keyCode.RIGHT:
			            ret = false;
			            self._moveSelection(1);
			            break;
			        case $.ui.keyCode.TAB:
			            ret = true;
			            break;
			        default:
			            ret = false;
			            self._typeAhead(event.keyCode, 'mouseup');
			            break;
			    }
			    return ret;
			})
			.bind('mouseover focus', function () {
			    $(this).addClass(self.widgetBaseClass + '-focus ui-state-hover');
			})
			.bind('mouseout blur', function () {
			    $(this).removeClass(self.widgetBaseClass + '-focus ui-state-hover');
			});

            //document click closes menu
            $(document)
			.mousedown(function (event) {
			    self.close(event);
			});

            //change event on original selectmenu
            this.element
			.click(function () { this._refreshValue(); })
			.focus(function () { this.newelement[0].focus(); });

            //create menu portion, append to body
            var cornerClass = (o.style == "dropdown") ? " ui-corner-bottom" : " ui-corner-all"
            this.list = $('<ul class="admin-table ' + self.widgetBaseClass + '-menu ui-widget ui-widget-content' + cornerClass + '" aria-hidden="true" role="listbox" aria-labelledby="' + this.ids[0] + '" id="' + this.ids[1] + '"></ul>').appendTo('body');

            //serialize selectmenu element options	
            var selectOptionData = [];
            this.element
			.find('option')
			.each(function () {
			    selectOptionData.push({
			        value: $(this).attr('value'),
			        text: self._formatText(jQuery(this).text()),
			        selected: $(this).attr('selected'),
			        classes: $(this).attr('class'),
			        parentOptGroup: $(this).parent('optgroup').attr('label')
			    });
			});

            //active state class is only used in popup style
            var activeClass = (self.options.style == "popup") ? " ui-state-active" : "";

            //write li's
            for (var i in selectOptionData) {
                var thisLi = $('<li role="presentation"><a href="#" tabindex="-1" role="option" aria-selected="false">' + selectOptionData[i].text + '</a></li>')
				.data('index', i)
				.addClass(selectOptionData[i].classes)
				.data('optionClasses', selectOptionData[i].classes || '')
				.mouseup(function (event) {
				    if (self._safemouseup) {
				        var changed = $(this).data('index') != self._selectedIndex();
				        self.value($(this).data('index'));
				        self.select(event);
				        if (changed) { self.change(event); }
				        self.close(event, true);
				    }
				    return false;
				})
				.click(function () {
				    return false;
				})
				.bind('mouseover focus', function () {
				    self._selectedOptionLi().addClass(activeClass);
				    self._focusedOptionLi().removeClass(self.widgetBaseClass + '-item-focus ui-state-hover');
				    $(this).removeClass('ui-state-active').addClass(self.widgetBaseClass + '-item-focus ui-state-hover');
				})
				.bind('mouseout blur', function () {
				    if ($(this).is(self._selectedOptionLi())) { $(this).addClass(activeClass); }
				    $(this).removeClass(self.widgetBaseClass + '-item-focus ui-state-hover');
				});

                //optgroup or not...
                if (selectOptionData[i].parentOptGroup) {
                    var optGroupName = self.widgetBaseClass + '-group-' + selectOptionData[i].parentOptGroup;
                    if (this.list.find('li.' + optGroupName).size()) {
                        this.list.find('li.' + optGroupName + ':last ul').append(thisLi);
                    }
                    else {
                        $('<li role="presentation" class="' + self.widgetBaseClass + '-group ' + optGroupName + '"><span class="' + self.widgetBaseClass + '-group-label">' + selectOptionData[i].parentOptGroup + '</span><ul></ul></li>')
						.appendTo(this.list)
						.find('ul')
						.append(thisLi);
                    }
                }
                else {
                    thisLi.appendTo(this.list);
                }

                //this allows for using the scrollbar in an overflowed list
                this.list.bind('mousedown mouseup', function () { return false; });

                //append icon if option is specified
                if (o.icons) {
                    for (var j in o.icons) {
                        if (thisLi.is(o.icons[j].find)) {
                            thisLi
							.data('optionClasses', selectOptionData[i].classes + ' ' + self.widgetBaseClass + '-hasIcon')
							.addClass(self.widgetBaseClass + '-hasIcon');
                            var iconClass = o.icons[j].icon || "";

                            thisLi
							.find('a:eq(0)')
							.prepend('<span class="' + self.widgetBaseClass + '-item-icon ui-icon ' + iconClass + '"></span>');
                        }
                    }
                }
            }

            //add corners to top and bottom menu items
            this.list.find('li:last').addClass("ui-corner-bottom");
            if (o.style == 'popup') { this.list.find('li:first').addClass("ui-corner-top"); }

            //transfer classes to selectmenu and list
            if (o.transferClasses) {
                var transferClasses = this.element.attr('class') || '';
                this.newelement.add(this.list).addClass(transferClasses);
            }

            //original selectmenu width
            var selectWidth = this.element.width();

            //set menu button width
            this.newelement.width((o.width) ? o.width : selectWidth);

            //set menu width to either menuWidth option value, width option value, or select width 
            if (o.style == 'dropdown') { this.list.width((o.menuWidth) ? o.menuWidth : ((o.width) ? o.width : selectWidth)); }
            else { this.list.width((o.menuWidth) ? o.menuWidth : ((o.width) ? o.width - o.handleWidth : selectWidth - o.handleWidth)); }

            //set max height from option 
            if (o.maxHeight && o.maxHeight < this.list.height()) { this.list.height(o.maxHeight); }

            //save reference to actionable li's (not group label li's)
            this._optionLis = this.list.find('li:not(.' + self.widgetBaseClass + '-group)');

            //transfer menu click to menu button
            this.list
			.keydown(function (event) {
			    var ret = true;
			    switch (event.keyCode) {
			        case $.ui.keyCode.UP:
			        case $.ui.keyCode.LEFT:
			            ret = false;
			            self._moveFocus(-1);
			            break;
			        case $.ui.keyCode.DOWN:
			        case $.ui.keyCode.RIGHT:
			            ret = false;
			            self._moveFocus(1);
			            break;
			        case $.ui.keyCode.HOME:
			            ret = false;
			            self._moveFocus(':first');
			            break;
			        case $.ui.keyCode.PAGE_UP:
			            ret = false;
			            self._scrollPage('up');
			            break;
			        case $.ui.keyCode.PAGE_DOWN:
			            ret = false;
			            self._scrollPage('down');
			            break;
			        case $.ui.keyCode.END:
			            ret = false;
			            self._moveFocus(':last');
			            break;
			        case $.ui.keyCode.ENTER:
			        case $.ui.keyCode.SPACE:
			            ret = false;
			            self.close(event, true);
			            $(event.target).parents('li:eq(0)').trigger('mouseup');
			            break;
			        case $.ui.keyCode.TAB:
			            ret = true;
			            self.close(event, true);
			            break;
			        case $.ui.keyCode.ESCAPE:
			            ret = false;
			            self.close(event, true);
			            break;
			        default:
			            ret = false;
			            self._typeAhead(event.keyCode, 'focus');
			            break;
			    }
			    return ret;
			});

            //selectmenu style
            if (o.style == 'dropdown') {
                this.newelement
				.addClass(self.widgetBaseClass + "-dropdown");
                this.list
				.addClass(self.widgetBaseClass + "-menu-dropdown");
            }
            else {
                this.newelement
				.addClass(self.widgetBaseClass + "-popup");
                this.list
				.addClass(self.widgetBaseClass + "-menu-popup");
            }

            //append status span to button
            this.newelement.prepend('<span class="' + self.widgetBaseClass + '-status">' + selectOptionData[this._selectedIndex()].text + '</span>');

            //hide original selectmenu element
            this.element.hide();

            //transfer disabled state
            if (this.element.attr('disabled') == true) { this.disable(); }

            //update value
            this.value(this._selectedIndex());
        },
        destroy: function () {
            this.element.removeData(this.widgetName)
			.removeClass(this.widgetBaseClass + '-disabled' + ' ' + this.namespace + '-state-disabled')
			.removeAttr('aria-disabled');

            //unbind click on label, reset its for attr
            $('label[for=' + this.newelement.attr('id') + ']')
			.attr('for', this.element.attr('id'))
			.unbind('click');
            this.newelement.remove();
            this.list.remove();
            this.element.show();
        },
        _typeAhead: function (code, eventType) {
            var self = this;
            //define self._prevChar if needed
            if (!self._prevChar) { self._prevChar = ['', 0]; }
            var C = String.fromCharCode(code);
            c = C.toLowerCase();
            var focusFound = false;
            function focusOpt(elem, ind) {
                focusFound = true;
                $(elem).trigger(eventType);
                self._prevChar[1] = ind;
            };
            this.list.find('li a').each(function (i) {
                if (!focusFound) {
                    var thisText = $(this).text();
                    if (thisText.indexOf(C) == 0 || thisText.indexOf(c) == 0) {
                        if (self._prevChar[0] == C) {
                            if (self._prevChar[1] < i) { focusOpt(this, i); }
                        }
                        else { focusOpt(this, i); }
                    }
                }
            });
            this._prevChar[0] = C;
        },
        _uiHash: function () {
            return {
                value: this.value()
            };
        },
        open: function (event) {
            var self = this;
            var disabledStatus = this.newelement.attr("aria-disabled");
            if (disabledStatus != 'true') {
                this._refreshPosition();
                this._closeOthers(event);
                this.newelement
				.addClass('ui-state-active');

                this.list
				.appendTo('body')
				.addClass(self.widgetBaseClass + '-open')
				.attr('aria-hidden', false)
				.find('li:not(.' + self.widgetBaseClass + '-group):eq(' + this._selectedIndex() + ') a')[0].focus();
                if (this.options.style == "dropdown") { this.newelement.removeClass('ui-corner-all').addClass('ui-corner-top'); }
                this._refreshPosition();
                this._trigger("open", event, this._uiHash());
            }
        },
        close: function (event, retainFocus) {
            if (this.newelement.is('.ui-state-active')) {
                this.newelement
				.removeClass('ui-state-active');
                this.list
				.attr('aria-hidden', true)
				.removeClass(this.widgetBaseClass + '-open');
                if (this.options.style == "dropdown") { this.newelement.removeClass('ui-corner-top').addClass('ui-corner-all'); }
                if (retainFocus) { this.newelement[0].focus(); }
                this._trigger("close", event, this._uiHash());
            }
        },
        change: function (event) {
            this.element.trigger('change');
            this._trigger("change", event, this._uiHash());
        },
        select: function (event) {
            this.element.trigger('select');
            this._trigger("select", event, this._uiHash());
        },
        _closeOthers: function (event) {
            $('.' + this.widgetBaseClass + '.ui-state-active').not(this.newelement).each(function () {
                $(this).data('selectelement').selectmenu('close', event);
            });
            $('.' + this.widgetBaseClass + '.ui-state-hover').trigger('mouseout');
        },
        _toggle: function (event, retainFocus) {
            if (this.list.is('.' + this.widgetBaseClass + '-open')) { this.close(event, retainFocus); }
            else { this.open(event); }
        },
        _formatText: function (text) {
            return this.options.format ? this.options.format(text) : text;
        },
        _selectedIndex: function () {
            return this.element[0].selectedIndex;
        },
        _selectedOptionLi: function () {
            return this._optionLis.eq(this._selectedIndex());
        },
        _focusedOptionLi: function () {
            return this.list.find('.' + this.widgetBaseClass + '-item-focus');
        },
        _moveSelection: function (amt) {
            var currIndex = parseInt(this._selectedOptionLi().data('index'), 10);
            var newIndex = currIndex + amt;
            return this._optionLis.eq(newIndex).trigger('mouseup');
        },
        _moveFocus: function (amt) {
            if (!isNaN(amt)) {
                var currIndex = parseInt(this._focusedOptionLi().data('index'), 10);
                var newIndex = currIndex + amt;
            }
            else { var newIndex = parseInt(this._optionLis.filter(amt).data('index'), 10); }

            if (newIndex < 0) { newIndex = 0; }
            if (newIndex > this._optionLis.size() - 1) {
                newIndex = this._optionLis.size() - 1;
            }
            var activeID = this.widgetBaseClass + '-item-' + Math.round(Math.random() * 1000);

            this._focusedOptionLi().find('a:eq(0)').attr('id', '');
            this._optionLis.eq(newIndex).find('a:eq(0)').attr('id', activeID)[0].focus();
            this.list.attr('aria-activedescendant', activeID);
        },
        _scrollPage: function (direction) {
            var numPerPage = Math.floor(this.list.outerHeight() / this.list.find('li:first').outerHeight());
            numPerPage = (direction == 'up') ? -numPerPage : numPerPage;
            this._moveFocus(numPerPage);
        },
        _setData: function (key, value) {
            this.options[key] = value;
            if (key == 'disabled') {
                this.close();
                this.element
				.add(this.newelement)
				.add(this.list)
					[value ? 'addClass' : 'removeClass'](
						this.widgetBaseClass + '-disabled' + ' ' +
						this.namespace + '-state-disabled')
					.attr("aria-disabled", value);
            }
        },
        value: function (newValue) {
            if (arguments.length) {
                this.element[0].selectedIndex = newValue;
                this._refreshValue();
                this._refreshPosition();
            }
            return this.element[0].selectedIndex;
        },
        _refreshValue: function () {
            var activeClass = (this.options.style == "popup") ? " ui-state-active" : "";
            var activeID = this.widgetBaseClass + '-item-' + Math.round(Math.random() * 1000);
            //deselect previous
            this.list
			.find('.' + this.widgetBaseClass + '-item-selected')
			.removeClass(this.widgetBaseClass + "-item-selected" + activeClass)
			.find('a')
			.attr('aria-selected', 'false')
			.attr('id', '');
            //select new
            this._selectedOptionLi()
			.addClass(this.widgetBaseClass + "-item-selected" + activeClass)
			.find('a')
			.attr('aria-selected', 'true')
			.attr('id', activeID);

            //toggle any class brought in from option
            var currentOptionClasses = this.newelement.data('optionClasses') ? this.newelement.data('optionClasses') : "";
            var newOptionClasses = this._selectedOptionLi().data('optionClasses') ? this._selectedOptionLi().data('optionClasses') : "";
            this.newelement
			.removeClass(currentOptionClasses)
			.data('optionClasses', newOptionClasses)
			.addClass(newOptionClasses)
			.find('.' + this.widgetBaseClass + '-status')
			.html(
				this._selectedOptionLi()
					.find('a:eq(0)')
					.html()
			);

            this.list.attr('aria-activedescendant', activeID)
        },
        _refreshPosition: function () {
            //set left value
            this.list.css('left', this.newelement.offset().left);

            //set top value
            var menuTop = this.newelement.offset().top;
            var scrolledAmt = this.list[0].scrollTop;
            this.list.find('li:lt(' + this._selectedIndex() + ')').each(function () {
                scrolledAmt -= $(this).outerHeight();
            });

            if (this.newelement.is('.' + this.widgetBaseClass + '-popup')) {
                menuTop += scrolledAmt;
                this.list.css('top', menuTop);
            }
            else {
                menuTop += this.newelement.height();
                this.list.css('top', menuTop);
            }
        }
    });

    $.extend($.ui.selectmenu, {
        getter: "value",
        version: "@VERSION",
        eventPrefix: "selectmenu",
        defaults: {
            transferClasses: true,
            style: 'popup',
            width: null,
            menuWidth: null,
            handleWidth: 26,
            maxHeight: null,
            icons: null,
            format: null
        }
    });

})(jQuery);

/*JQ UI DATETIME*/
/*
* jQuery timepicker addon
* By: Trent Richardson [http://trentrichardson.com]
* Version 0.9.9
* Last Modified: 02/05/2012
* 
* Copyright 2012 Trent Richardson
* Dual licensed under the MIT and GPL licenses.
* http://trentrichardson.com/Impromptu/GPL-LICENSE.txt
* http://trentrichardson.com/Impromptu/MIT-LICENSE.txt
* 
* HERES THE CSS:
* .ui-timepicker-div .ui-widget-header { margin-bottom: 8px; }
* .ui-timepicker-div dl { text-align: left; }
* .ui-timepicker-div dl dt { height: 25px; margin-bottom: -25px; }
* .ui-timepicker-div dl dd { margin: 0 10px 10px 65px; }
* .ui-timepicker-div td { font-size: 90%; }
* .ui-tpicker-grid-label { background: none; border: none; margin: 0; padding: 0; }
*/

(function ($) {

    $.extend($.ui, { timepicker: { version: "0.9.9"} });

    /* Time picker manager.
    Use the singleton instance of this class, $.timepicker, to interact with the time picker.
    Settings for (groups of) time pickers are maintained in an instance object,
    allowing multiple different settings on the same page. */

    function Timepicker() {
        this.regional = []; // Available regional settings, indexed by language code
        this.regional[''] = { // Default regional settings
            currentText: 'Now',
            closeText: 'Done',
            ampm: false,
            amNames: ['AM', 'A'],
            pmNames: ['PM', 'P'],
            timeFormat: 'hh:mm tt',
            timeSuffix: '',
            timeOnlyTitle: 'Choose Time',
            timeText: 'Time',
            hourText: 'Hour',
            minuteText: 'Minute',
            secondText: 'Second',
            millisecText: 'Millisecond',
            timezoneText: 'Time Zone'
        };
        this._defaults = { // Global defaults for all the datetime picker instances
            showButtonPanel: true,
            timeOnly: false,
            showHour: true,
            showMinute: true,
            showSecond: false,
            showMillisec: false,
            showTimezone: false,
            showTime: true,
            stepHour: 1,
            stepMinute: 1,
            stepSecond: 1,
            stepMillisec: 1,
            hour: 0,
            minute: 0,
            second: 0,
            millisec: 0,
            timezone: '+0000',
            hourMin: 0,
            minuteMin: 0,
            secondMin: 0,
            millisecMin: 0,
            hourMax: 23,
            minuteMax: 59,
            secondMax: 59,
            millisecMax: 999,
            minDateTime: null,
            maxDateTime: null,
            onSelect: null,
            hourGrid: 0,
            minuteGrid: 0,
            secondGrid: 0,
            millisecGrid: 0,
            alwaysSetTime: true,
            separator: ' ',
            altFieldTimeOnly: true,
            showTimepicker: true,
            timezoneIso8609: false,
            timezoneList: null,
            addSliderAccess: false,
            sliderAccessArgs: null
        };
        $.extend(this._defaults, this.regional['']);
    };

    $.extend(Timepicker.prototype, {
        $input: null,
        $altInput: null,
        $timeObj: null,
        inst: null,
        hour_slider: null,
        minute_slider: null,
        second_slider: null,
        millisec_slider: null,
        timezone_select: null,
        hour: 0,
        minute: 0,
        second: 0,
        millisec: 0,
        timezone: '+0000',
        hourMinOriginal: null,
        minuteMinOriginal: null,
        secondMinOriginal: null,
        millisecMinOriginal: null,
        hourMaxOriginal: null,
        minuteMaxOriginal: null,
        secondMaxOriginal: null,
        millisecMaxOriginal: null,
        ampm: '',
        formattedDate: '',
        formattedTime: '',
        formattedDateTime: '',
        timezoneList: null,

        /* Override the default settings for all instances of the time picker.
        @param  settings  object - the new settings to use as defaults (anonymous object)
        @return the manager object */
        setDefaults: function (settings) {
            extendRemove(this._defaults, settings || {});
            return this;
        },

        //########################################################################
        // Create a new Timepicker instance
        //########################################################################
        _newInst: function ($input, o) {
            var tp_inst = new Timepicker(),
			inlineSettings = {};

            for (var attrName in this._defaults) {
                var attrValue = $input.attr('time:' + attrName);
                if (attrValue) {
                    try {
                        inlineSettings[attrName] = eval(attrValue);
                    } catch (err) {
                        inlineSettings[attrName] = attrValue;
                    }
                }
            }
            tp_inst._defaults = $.extend({}, this._defaults, inlineSettings, o, {
                beforeShow: function (input, dp_inst) {
                    if ($.isFunction(o.beforeShow))
                        return o.beforeShow(input, dp_inst, tp_inst);
                },
                onChangeMonthYear: function (year, month, dp_inst) {
                    // Update the time as well : this prevents the time from disappearing from the $input field.
                    tp_inst._updateDateTime(dp_inst);
                    if ($.isFunction(o.onChangeMonthYear))
                        o.onChangeMonthYear.call($input[0], year, month, dp_inst, tp_inst);
                },
                onClose: function (dateText, dp_inst) {
                    if (tp_inst.timeDefined === true && $input.val() != '')
                        tp_inst._updateDateTime(dp_inst);
                    if ($.isFunction(o.onClose))
                        o.onClose.call($input[0], dateText, dp_inst, tp_inst);
                },
                timepicker: tp_inst // add timepicker as a property of datepicker: $.datepicker._get(dp_inst, 'timepicker');
            });
            tp_inst.amNames = $.map(tp_inst._defaults.amNames, function (val) { return val.toUpperCase() });
            tp_inst.pmNames = $.map(tp_inst._defaults.pmNames, function (val) { return val.toUpperCase() });

            if (tp_inst._defaults.timezoneList === null) {
                var timezoneList = [];
                for (var i = -11; i <= 12; i++)
                    timezoneList.push((i >= 0 ? '+' : '-') + ('0' + Math.abs(i).toString()).slice(-2) + '00');
                if (tp_inst._defaults.timezoneIso8609)
                    timezoneList = $.map(timezoneList, function (val) {
                        return val == '+0000' ? 'Z' : (val.substring(0, 3) + ':' + val.substring(3));
                    });
                tp_inst._defaults.timezoneList = timezoneList;
            }

            tp_inst.hour = tp_inst._defaults.hour;
            tp_inst.minute = tp_inst._defaults.minute;
            tp_inst.second = tp_inst._defaults.second;
            tp_inst.millisec = tp_inst._defaults.millisec;
            tp_inst.ampm = '';
            tp_inst.$input = $input;

            if (o.altField)
                tp_inst.$altInput = $(o.altField)
				.css({ cursor: 'pointer' })
				.focus(function () { $input.trigger("focus"); });

            if (tp_inst._defaults.minDate == 0 || tp_inst._defaults.minDateTime == 0) {
                tp_inst._defaults.minDate = new Date();
            }
            if (tp_inst._defaults.maxDate == 0 || tp_inst._defaults.maxDateTime == 0) {
                tp_inst._defaults.maxDate = new Date();
            }

            // datepicker needs minDate/maxDate, timepicker needs minDateTime/maxDateTime..
            if (tp_inst._defaults.minDate !== undefined && tp_inst._defaults.minDate instanceof Date)
                tp_inst._defaults.minDateTime = new Date(tp_inst._defaults.minDate.getTime());
            if (tp_inst._defaults.minDateTime !== undefined && tp_inst._defaults.minDateTime instanceof Date)
                tp_inst._defaults.minDate = new Date(tp_inst._defaults.minDateTime.getTime());
            if (tp_inst._defaults.maxDate !== undefined && tp_inst._defaults.maxDate instanceof Date)
                tp_inst._defaults.maxDateTime = new Date(tp_inst._defaults.maxDate.getTime());
            if (tp_inst._defaults.maxDateTime !== undefined && tp_inst._defaults.maxDateTime instanceof Date)
                tp_inst._defaults.maxDate = new Date(tp_inst._defaults.maxDateTime.getTime());
            return tp_inst;
        },

        //########################################################################
        // add our sliders to the calendar
        //########################################################################
        _addTimePicker: function (dp_inst) {
            var currDT = (this.$altInput && this._defaults.altFieldTimeOnly) ?
				this.$input.val() + ' ' + this.$altInput.val() :
				this.$input.val();

            this.timeDefined = this._parseTime(currDT);
            this._limitMinMaxDateTime(dp_inst, false);
            this._injectTimePicker();
        },

        //########################################################################
        // parse the time string from input value or _setTime
        //########################################################################
        _parseTime: function (timeString, withDate) {
            var regstr = this._defaults.timeFormat.toString()
				.replace(/h{1,2}/ig, '(\\d?\\d)')
				.replace(/m{1,2}/ig, '(\\d?\\d)')
				.replace(/s{1,2}/ig, '(\\d?\\d)')
				.replace(/l{1}/ig, '(\\d?\\d?\\d)')
				.replace(/t{1,2}/ig, this._getPatternAmpm())
				.replace(/z{1}/ig, '(z|[-+]\\d\\d:?\\d\\d)?')
				.replace(/\s/g, '\\s?') + this._defaults.timeSuffix + '$',
			order = this._getFormatPositions(),
			ampm = '',
			treg;

            if (!this.inst) this.inst = $.datepicker._getInst(this.$input[0]);

            if (withDate || !this._defaults.timeOnly) {
                // the time should come after x number of characters and a space.
                // x = at least the length of text specified by the date format
                var dp_dateFormat = $.datepicker._get(this.inst, 'dateFormat');
                // escape special regex characters in the seperator
                var specials = new RegExp("[.*+?|()\\[\\]{}\\\\]", "g");
                regstr = '^.{' + dp_dateFormat.length + ',}?' + this._defaults.separator.replace(specials, "\\$&") + regstr;
            }

            treg = timeString.match(new RegExp(regstr, 'i'));

            if (treg) {
                if (order.t !== -1) {
                    if (treg[order.t] === undefined || treg[order.t].length === 0) {
                        ampm = '';
                        this.ampm = '';
                    } else {
                        ampm = $.inArray(treg[order.t].toUpperCase(), this.amNames) !== -1 ? 'AM' : 'PM';
                        this.ampm = this._defaults[ampm == 'AM' ? 'amNames' : 'pmNames'][0];
                    }
                }

                if (order.h !== -1) {
                    if (ampm == 'AM' && treg[order.h] == '12')
                        this.hour = 0; // 12am = 0 hour
                    else if (ampm == 'PM' && treg[order.h] != '12')
                        this.hour = (parseFloat(treg[order.h]) + 12).toFixed(0); // 12pm = 12 hour, any other pm = hour + 12
                    else this.hour = Number(treg[order.h]);
                }

                if (order.m !== -1) this.minute = Number(treg[order.m]);
                if (order.s !== -1) this.second = Number(treg[order.s]);
                if (order.l !== -1) this.millisec = Number(treg[order.l]);
                if (order.z !== -1 && treg[order.z] !== undefined) {
                    var tz = treg[order.z].toUpperCase();
                    switch (tz.length) {
                        case 1: // Z
                            tz = this._defaults.timezoneIso8609 ? 'Z' : '+0000';
                            break;
                        case 5: // +hhmm
                            if (this._defaults.timezoneIso8609)
                                tz = tz.substring(1) == '0000'
						   ? 'Z'
						   : tz.substring(0, 3) + ':' + tz.substring(3);
                            break;
                        case 6: // +hh:mm
                            if (!this._defaults.timezoneIso8609)
                                tz = tz == 'Z' || tz.substring(1) == '00:00'
						   ? '+0000'
						   : tz.replace(/:/, '');
                            else if (tz.substring(1) == '00:00')
                                tz = 'Z';
                            break;
                    }
                    this.timezone = tz;
                }

                return true;

            }
            return false;
        },

        //########################################################################
        // pattern for standard and localized AM/PM markers
        //########################################################################
        _getPatternAmpm: function () {
            var markers = [];
            o = this._defaults;
            if (o.amNames)
                $.merge(markers, o.amNames);
            if (o.pmNames)
                $.merge(markers, o.pmNames);
            markers = $.map(markers, function (val) { return val.replace(/[.*+?|()\[\]{}\\]/g, '\\$&') });
            return '(' + markers.join('|') + ')?';
        },

        //########################################################################
        // figure out position of time elements.. cause js cant do named captures
        //########################################################################
        _getFormatPositions: function () {
            var finds = this._defaults.timeFormat.toLowerCase().match(/(h{1,2}|m{1,2}|s{1,2}|l{1}|t{1,2}|z)/g),
			orders = { h: -1, m: -1, s: -1, l: -1, t: -1, z: -1 };

            if (finds)
                for (var i = 0; i < finds.length; i++)
                    if (orders[finds[i].toString().charAt(0)] == -1)
                        orders[finds[i].toString().charAt(0)] = i + 1;

            return orders;
        },

        //########################################################################
        // generate and inject html for timepicker into ui datepicker
        //########################################################################
        _injectTimePicker: function () {
            var $dp = this.inst.dpDiv,
			o = this._defaults,
			tp_inst = this,
            // Added by Peter Medeiros:
            // - Figure out what the hour/minute/second max should be based on the step values.
            // - Example: if stepMinute is 15, then minMax is 45.
			hourMax = parseInt((o.hourMax - ((o.hourMax - o.hourMin) % o.stepHour)), 10),
			minMax = parseInt((o.minuteMax - ((o.minuteMax - o.minuteMin) % o.stepMinute)), 10),
			secMax = parseInt((o.secondMax - ((o.secondMax - o.secondMin) % o.stepSecond)), 10),
			millisecMax = parseInt((o.millisecMax - ((o.millisecMax - o.millisecMin) % o.stepMillisec)), 10),
			dp_id = this.inst.id.toString().replace(/([^A-Za-z0-9_])/g, '');

            // Prevent displaying twice
            //if ($dp.find("div#ui-timepicker-div-"+ dp_id).length === 0) {
            if ($dp.find("div#ui-timepicker-div-" + dp_id).length === 0 && o.showTimepicker) {
                var noDisplay = ' style="display:none;"',
				html = '<div class="ui-timepicker-div" id="ui-timepicker-div-' + dp_id + '"><dl>' +
						'<dt class="ui_tpicker_time_label" id="ui_tpicker_time_label_' + dp_id + '"' +
						((o.showTime) ? '' : noDisplay) + '>' + o.timeText + '</dt>' +
						'<dd class="ui_tpicker_time" id="ui_tpicker_time_' + dp_id + '"' +
						((o.showTime) ? '' : noDisplay) + '></dd>' +
						'<dt class="ui_tpicker_hour_label" id="ui_tpicker_hour_label_' + dp_id + '"' +
						((o.showHour) ? '' : noDisplay) + '>' + o.hourText + '</dt>',
				hourGridSize = 0,
				minuteGridSize = 0,
				secondGridSize = 0,
				millisecGridSize = 0,
				size;

                // Hours
                html += '<dd class="ui_tpicker_hour"><div id="ui_tpicker_hour_' + dp_id + '"' +
						((o.showHour) ? '' : noDisplay) + '></div>';
                if (o.showHour && o.hourGrid > 0) {
                    html += '<div style="padding-left: 1px"><table class="ui-tpicker-grid-label"><tr>';

                    for (var h = o.hourMin; h <= hourMax; h += parseInt(o.hourGrid, 10)) {
                        hourGridSize++;
                        var tmph = (o.ampm && h > 12) ? h - 12 : h;
                        if (tmph < 10) tmph = '0' + tmph;
                        if (o.ampm) {
                            if (h == 0) tmph = 12 + 'a';
                            else if (h < 12) tmph += 'a';
                            else tmph += 'p';
                        }
                        html += '<td>' + tmph + '</td>';
                    }

                    html += '</tr></table></div>';
                }
                html += '</dd>';

                // Minutes
                html += '<dt class="ui_tpicker_minute_label" id="ui_tpicker_minute_label_' + dp_id + '"' +
					((o.showMinute) ? '' : noDisplay) + '>' + o.minuteText + '</dt>' +
					'<dd class="ui_tpicker_minute"><div id="ui_tpicker_minute_' + dp_id + '"' +
							((o.showMinute) ? '' : noDisplay) + '></div>';

                if (o.showMinute && o.minuteGrid > 0) {
                    html += '<div style="padding-left: 1px"><table class="ui-tpicker-grid-label"><tr>';

                    for (var m = o.minuteMin; m <= minMax; m += parseInt(o.minuteGrid, 10)) {
                        minuteGridSize++;
                        html += '<td>' + ((m < 10) ? '0' : '') + m + '</td>';
                    }

                    html += '</tr></table></div>';
                }
                html += '</dd>';

                // Seconds
                html += '<dt class="ui_tpicker_second_label" id="ui_tpicker_second_label_' + dp_id + '"' +
					((o.showSecond) ? '' : noDisplay) + '>' + o.secondText + '</dt>' +
					'<dd class="ui_tpicker_second"><div id="ui_tpicker_second_' + dp_id + '"' +
							((o.showSecond) ? '' : noDisplay) + '></div>';

                if (o.showSecond && o.secondGrid > 0) {
                    html += '<div style="padding-left: 1px"><table><tr>';

                    for (var s = o.secondMin; s <= secMax; s += parseInt(o.secondGrid, 10)) {
                        secondGridSize++;
                        html += '<td>' + ((s < 10) ? '0' : '') + s + '</td>';
                    }

                    html += '</tr></table></div>';
                }
                html += '</dd>';

                // Milliseconds
                html += '<dt class="ui_tpicker_millisec_label" id="ui_tpicker_millisec_label_' + dp_id + '"' +
					((o.showMillisec) ? '' : noDisplay) + '>' + o.millisecText + '</dt>' +
					'<dd class="ui_tpicker_millisec"><div id="ui_tpicker_millisec_' + dp_id + '"' +
							((o.showMillisec) ? '' : noDisplay) + '></div>';

                if (o.showMillisec && o.millisecGrid > 0) {
                    html += '<div style="padding-left: 1px"><table><tr>';

                    for (var l = o.millisecMin; l <= millisecMax; l += parseInt(o.millisecGrid, 10)) {
                        millisecGridSize++;
                        html += '<td>' + ((l < 10) ? '0' : '') + l + '</td>';
                    }

                    html += '</tr></table></div>';
                }
                html += '</dd>';

                // Timezone
                html += '<dt class="ui_tpicker_timezone_label" id="ui_tpicker_timezone_label_' + dp_id + '"' +
					((o.showTimezone) ? '' : noDisplay) + '>' + o.timezoneText + '</dt>';
                html += '<dd class="ui_tpicker_timezone" id="ui_tpicker_timezone_' + dp_id + '"' +
							((o.showTimezone) ? '' : noDisplay) + '></dd>';

                html += '</dl></div>';
                $tp = $(html);

                // if we only want time picker...
                if (o.timeOnly === true) {
                    $tp.prepend(
					'<div class="ui-widget-header ui-helper-clearfix ui-corner-all">' +
						'<div class="ui-datepicker-title">' + o.timeOnlyTitle + '</div>' +
					'</div>');
                    $dp.find('.ui-datepicker-header, .ui-datepicker-calendar').hide();
                }

                this.hour_slider = $tp.find('#ui_tpicker_hour_' + dp_id).slider({
                    orientation: "horizontal",
                    value: this.hour,
                    min: o.hourMin,
                    max: hourMax,
                    step: o.stepHour,
                    slide: function (event, ui) {
                        tp_inst.hour_slider.slider("option", "value", ui.value);
                        tp_inst._onTimeChange();
                    }
                });


                // Updated by Peter Medeiros:
                // - Pass in Event and UI instance into slide function
                this.minute_slider = $tp.find('#ui_tpicker_minute_' + dp_id).slider({
                    orientation: "horizontal",
                    value: this.minute,
                    min: o.minuteMin,
                    max: minMax,
                    step: o.stepMinute,
                    slide: function (event, ui) {
                        tp_inst.minute_slider.slider("option", "value", ui.value);
                        tp_inst._onTimeChange();
                    }
                });

                this.second_slider = $tp.find('#ui_tpicker_second_' + dp_id).slider({
                    orientation: "horizontal",
                    value: this.second,
                    min: o.secondMin,
                    max: secMax,
                    step: o.stepSecond,
                    slide: function (event, ui) {
                        tp_inst.second_slider.slider("option", "value", ui.value);
                        tp_inst._onTimeChange();
                    }
                });

                this.millisec_slider = $tp.find('#ui_tpicker_millisec_' + dp_id).slider({
                    orientation: "horizontal",
                    value: this.millisec,
                    min: o.millisecMin,
                    max: millisecMax,
                    step: o.stepMillisec,
                    slide: function (event, ui) {
                        tp_inst.millisec_slider.slider("option", "value", ui.value);
                        tp_inst._onTimeChange();
                    }
                });

                this.timezone_select = $tp.find('#ui_tpicker_timezone_' + dp_id).append('<select></select>').find("select");
                $.fn.append.apply(this.timezone_select,
				$.map(o.timezoneList, function (val, idx) {
				    return $("<option />")
						.val(typeof val == "object" ? val.value : val)
						.text(typeof val == "object" ? val.label : val);
				})
			);
                this.timezone_select.val((typeof this.timezone != "undefined" && this.timezone != null && this.timezone != "") ? this.timezone : o.timezone);
                this.timezone_select.change(function () {
                    tp_inst._onTimeChange();
                });

                // Add grid functionality
                if (o.showHour && o.hourGrid > 0) {
                    size = 100 * hourGridSize * o.hourGrid / (hourMax - o.hourMin);

                    $tp.find(".ui_tpicker_hour table").css({
                        width: size + "%",
                        marginLeft: (size / (-2 * hourGridSize)) + "%",
                        borderCollapse: 'collapse'
                    }).find("td").each(function (index) {
                        $(this).click(function () {
                            var h = $(this).html();
                            if (o.ampm) {
                                var ap = h.substring(2).toLowerCase(),
								aph = parseInt(h.substring(0, 2), 10);
                                if (ap == 'a') {
                                    if (aph == 12) h = 0;
                                    else h = aph;
                                } else if (aph == 12) h = 12;
                                else h = aph + 12;
                            }
                            tp_inst.hour_slider.slider("option", "value", h);
                            tp_inst._onTimeChange();
                            tp_inst._onSelectHandler();
                        }).css({
                            cursor: 'pointer',
                            width: (100 / hourGridSize) + '%',
                            textAlign: 'center',
                            overflow: 'hidden'
                        });
                    });
                }

                if (o.showMinute && o.minuteGrid > 0) {
                    size = 100 * minuteGridSize * o.minuteGrid / (minMax - o.minuteMin);
                    $tp.find(".ui_tpicker_minute table").css({
                        width: size + "%",
                        marginLeft: (size / (-2 * minuteGridSize)) + "%",
                        borderCollapse: 'collapse'
                    }).find("td").each(function (index) {
                        $(this).click(function () {
                            tp_inst.minute_slider.slider("option", "value", $(this).html());
                            tp_inst._onTimeChange();
                            tp_inst._onSelectHandler();
                        }).css({
                            cursor: 'pointer',
                            width: (100 / minuteGridSize) + '%',
                            textAlign: 'center',
                            overflow: 'hidden'
                        });
                    });
                }

                if (o.showSecond && o.secondGrid > 0) {
                    $tp.find(".ui_tpicker_second table").css({
                        width: size + "%",
                        marginLeft: (size / (-2 * secondGridSize)) + "%",
                        borderCollapse: 'collapse'
                    }).find("td").each(function (index) {
                        $(this).click(function () {
                            tp_inst.second_slider.slider("option", "value", $(this).html());
                            tp_inst._onTimeChange();
                            tp_inst._onSelectHandler();
                        }).css({
                            cursor: 'pointer',
                            width: (100 / secondGridSize) + '%',
                            textAlign: 'center',
                            overflow: 'hidden'
                        });
                    });
                }

                if (o.showMillisec && o.millisecGrid > 0) {
                    $tp.find(".ui_tpicker_millisec table").css({
                        width: size + "%",
                        marginLeft: (size / (-2 * millisecGridSize)) + "%",
                        borderCollapse: 'collapse'
                    }).find("td").each(function (index) {
                        $(this).click(function () {
                            tp_inst.millisec_slider.slider("option", "value", $(this).html());
                            tp_inst._onTimeChange();
                            tp_inst._onSelectHandler();
                        }).css({
                            cursor: 'pointer',
                            width: (100 / millisecGridSize) + '%',
                            textAlign: 'center',
                            overflow: 'hidden'
                        });
                    });
                }

                var $buttonPanel = $dp.find('.ui-datepicker-buttonpane');
                if ($buttonPanel.length) $buttonPanel.before($tp);
                else $dp.append($tp);

                this.$timeObj = $tp.find('#ui_tpicker_time_' + dp_id);

                if (this.inst !== null) {
                    var timeDefined = this.timeDefined;
                    this._onTimeChange();
                    this.timeDefined = timeDefined;
                }

                //Emulate datepicker onSelect behavior. Call on slidestop.
                var onSelectDelegate = function () {
                    tp_inst._onSelectHandler();
                };
                this.hour_slider.bind('slidestop', onSelectDelegate);
                this.minute_slider.bind('slidestop', onSelectDelegate);
                this.second_slider.bind('slidestop', onSelectDelegate);
                this.millisec_slider.bind('slidestop', onSelectDelegate);

                // slideAccess integration: http://trentrichardson.com/2011/11/11/jquery-ui-sliders-and-touch-accessibility/
                if (this._defaults.addSliderAccess) {
                    var sliderAccessArgs = this._defaults.sliderAccessArgs;
                    setTimeout(function () { // fix for inline mode
                        if ($tp.find('.ui-slider-access').length == 0) {
                            $tp.find('.ui-slider:visible').sliderAccess(sliderAccessArgs);

                            // fix any grids since sliders are shorter
                            var sliderAccessWidth = $tp.find('.ui-slider-access:eq(0)').outerWidth(true);
                            if (sliderAccessWidth) {
                                $tp.find('table:visible').each(function () {
                                    var $g = $(this),
									oldWidth = $g.outerWidth(),
									oldMarginLeft = $g.css('marginLeft').toString().replace('%', ''),
									newWidth = oldWidth - sliderAccessWidth,
									newMarginLeft = ((oldMarginLeft * newWidth) / oldWidth) + '%';

                                    $g.css({ width: newWidth, marginLeft: newMarginLeft });
                                });
                            }
                        }
                    }, 0);
                }
                // end slideAccess integration

            }
        },

        //########################################################################
        // This function tries to limit the ability to go outside the
        // min/max date range
        //########################################################################
        _limitMinMaxDateTime: function (dp_inst, adjustSliders) {
            var o = this._defaults,
			dp_date = new Date(dp_inst.selectedYear, dp_inst.selectedMonth, dp_inst.selectedDay);

            if (!this._defaults.showTimepicker) return; // No time so nothing to check here

            if ($.datepicker._get(dp_inst, 'minDateTime') !== null && $.datepicker._get(dp_inst, 'minDateTime') !== undefined && dp_date) {
                var minDateTime = $.datepicker._get(dp_inst, 'minDateTime'),
				minDateTimeDate = new Date(minDateTime.getFullYear(), minDateTime.getMonth(), minDateTime.getDate(), 0, 0, 0, 0);

                if (this.hourMinOriginal === null || this.minuteMinOriginal === null || this.secondMinOriginal === null || this.millisecMinOriginal === null) {
                    this.hourMinOriginal = o.hourMin;
                    this.minuteMinOriginal = o.minuteMin;
                    this.secondMinOriginal = o.secondMin;
                    this.millisecMinOriginal = o.millisecMin;
                }

                if (dp_inst.settings.timeOnly || minDateTimeDate.getTime() == dp_date.getTime()) {
                    this._defaults.hourMin = minDateTime.getHours();
                    if (this.hour <= this._defaults.hourMin) {
                        this.hour = this._defaults.hourMin;
                        this._defaults.minuteMin = minDateTime.getMinutes();
                        if (this.minute <= this._defaults.minuteMin) {
                            this.minute = this._defaults.minuteMin;
                            this._defaults.secondMin = minDateTime.getSeconds();
                        } else if (this.second <= this._defaults.secondMin) {
                            this.second = this._defaults.secondMin;
                            this._defaults.millisecMin = minDateTime.getMilliseconds();
                        } else {
                            if (this.millisec < this._defaults.millisecMin)
                                this.millisec = this._defaults.millisecMin;
                            this._defaults.millisecMin = this.millisecMinOriginal;
                        }
                    } else {
                        this._defaults.minuteMin = this.minuteMinOriginal;
                        this._defaults.secondMin = this.secondMinOriginal;
                        this._defaults.millisecMin = this.millisecMinOriginal;
                    }
                } else {
                    this._defaults.hourMin = this.hourMinOriginal;
                    this._defaults.minuteMin = this.minuteMinOriginal;
                    this._defaults.secondMin = this.secondMinOriginal;
                    this._defaults.millisecMin = this.millisecMinOriginal;
                }
            }

            if ($.datepicker._get(dp_inst, 'maxDateTime') !== null && $.datepicker._get(dp_inst, 'maxDateTime') !== undefined && dp_date) {
                var maxDateTime = $.datepicker._get(dp_inst, 'maxDateTime'),
				maxDateTimeDate = new Date(maxDateTime.getFullYear(), maxDateTime.getMonth(), maxDateTime.getDate(), 0, 0, 0, 0);

                if (this.hourMaxOriginal === null || this.minuteMaxOriginal === null || this.secondMaxOriginal === null) {
                    this.hourMaxOriginal = o.hourMax;
                    this.minuteMaxOriginal = o.minuteMax;
                    this.secondMaxOriginal = o.secondMax;
                    this.millisecMaxOriginal = o.millisecMax;
                }

                if (dp_inst.settings.timeOnly || maxDateTimeDate.getTime() == dp_date.getTime()) {
                    this._defaults.hourMax = maxDateTime.getHours();
                    if (this.hour >= this._defaults.hourMax) {
                        this.hour = this._defaults.hourMax;
                        this._defaults.minuteMax = maxDateTime.getMinutes();
                        if (this.minute >= this._defaults.minuteMax) {
                            this.minute = this._defaults.minuteMax;
                            this._defaults.secondMax = maxDateTime.getSeconds();
                        } else if (this.second >= this._defaults.secondMax) {
                            this.second = this._defaults.secondMax;
                            this._defaults.millisecMax = maxDateTime.getMilliseconds();
                        } else {
                            if (this.millisec > this._defaults.millisecMax) this.millisec = this._defaults.millisecMax;
                            this._defaults.millisecMax = this.millisecMaxOriginal;
                        }
                    } else {
                        this._defaults.minuteMax = this.minuteMaxOriginal;
                        this._defaults.secondMax = this.secondMaxOriginal;
                        this._defaults.millisecMax = this.millisecMaxOriginal;
                    }
                } else {
                    this._defaults.hourMax = this.hourMaxOriginal;
                    this._defaults.minuteMax = this.minuteMaxOriginal;
                    this._defaults.secondMax = this.secondMaxOriginal;
                    this._defaults.millisecMax = this.millisecMaxOriginal;
                }
            }

            if (adjustSliders !== undefined && adjustSliders === true) {
                var hourMax = parseInt((this._defaults.hourMax - ((this._defaults.hourMax - this._defaults.hourMin) % this._defaults.stepHour)), 10),
                minMax = parseInt((this._defaults.minuteMax - ((this._defaults.minuteMax - this._defaults.minuteMin) % this._defaults.stepMinute)), 10),
                secMax = parseInt((this._defaults.secondMax - ((this._defaults.secondMax - this._defaults.secondMin) % this._defaults.stepSecond)), 10),
				millisecMax = parseInt((this._defaults.millisecMax - ((this._defaults.millisecMax - this._defaults.millisecMin) % this._defaults.stepMillisec)), 10);

                if (this.hour_slider)
                    this.hour_slider.slider("option", { min: this._defaults.hourMin, max: hourMax }).slider('value', this.hour);
                if (this.minute_slider)
                    this.minute_slider.slider("option", { min: this._defaults.minuteMin, max: minMax }).slider('value', this.minute);
                if (this.second_slider)
                    this.second_slider.slider("option", { min: this._defaults.secondMin, max: secMax }).slider('value', this.second);
                if (this.millisec_slider)
                    this.millisec_slider.slider("option", { min: this._defaults.millisecMin, max: millisecMax }).slider('value', this.millisec);
            }

        },


        //########################################################################
        // when a slider moves, set the internal time...
        // on time change is also called when the time is updated in the text field
        //########################################################################
        _onTimeChange: function () {
            var hour = (this.hour_slider) ? this.hour_slider.slider('value') : false,
			minute = (this.minute_slider) ? this.minute_slider.slider('value') : false,
			second = (this.second_slider) ? this.second_slider.slider('value') : false,
			millisec = (this.millisec_slider) ? this.millisec_slider.slider('value') : false,
			timezone = (this.timezone_select) ? this.timezone_select.val() : false,
			o = this._defaults;

            if (typeof (hour) == 'object') hour = false;
            if (typeof (minute) == 'object') minute = false;
            if (typeof (second) == 'object') second = false;
            if (typeof (millisec) == 'object') millisec = false;
            if (typeof (timezone) == 'object') timezone = false;

            if (hour !== false) hour = parseInt(hour, 10);
            if (minute !== false) minute = parseInt(minute, 10);
            if (second !== false) second = parseInt(second, 10);
            if (millisec !== false) millisec = parseInt(millisec, 10);

            var ampm = o[hour < 12 ? 'amNames' : 'pmNames'][0];

            // If the update was done in the input field, the input field should not be updated.
            // If the update was done using the sliders, update the input field.
            var hasChanged = (hour != this.hour || minute != this.minute
				|| second != this.second || millisec != this.millisec
				|| (this.ampm.length > 0
				    && (hour < 12) != ($.inArray(this.ampm.toUpperCase(), this.amNames) !== -1))
				|| timezone != this.timezone);

            if (hasChanged) {

                if (hour !== false) this.hour = hour;
                if (minute !== false) this.minute = minute;
                if (second !== false) this.second = second;
                if (millisec !== false) this.millisec = millisec;
                if (timezone !== false) this.timezone = timezone;

                if (!this.inst) this.inst = $.datepicker._getInst(this.$input[0]);

                this._limitMinMaxDateTime(this.inst, true);
            }
            if (o.ampm) this.ampm = ampm;

            //this._formatTime();
            this.formattedTime = $.datepicker.formatTime(this._defaults.timeFormat, this, this._defaults);
            if (this.$timeObj) this.$timeObj.text(this.formattedTime + o.timeSuffix);
            this.timeDefined = true;
            if (hasChanged) this._updateDateTime();
        },

        //########################################################################
        // call custom onSelect. 
        // bind to sliders slidestop, and grid click.
        //########################################################################
        _onSelectHandler: function () {
            var onSelect = this._defaults.onSelect;
            var inputEl = this.$input ? this.$input[0] : null;
            if (onSelect && inputEl) {
                onSelect.apply(inputEl, [this.formattedDateTime, this]);
            }
        },

        //########################################################################
        // left for any backwards compatibility
        //########################################################################
        _formatTime: function (time, format) {
            time = time || { hour: this.hour, minute: this.minute, second: this.second, millisec: this.millisec, ampm: this.ampm, timezone: this.timezone };
            var tmptime = (format || this._defaults.timeFormat).toString();

            tmptime = $.datepicker.formatTime(tmptime, time, this._defaults);

            if (arguments.length) return tmptime;
            else this.formattedTime = tmptime;
        },

        //########################################################################
        // update our input with the new date time..
        //########################################################################
        _updateDateTime: function (dp_inst) {
            dp_inst = this.inst || dp_inst;
            var dt = $.datepicker._daylightSavingAdjust(new Date(dp_inst.selectedYear, dp_inst.selectedMonth, dp_inst.selectedDay)),
			dateFmt = $.datepicker._get(dp_inst, 'dateFormat'),
			formatCfg = $.datepicker._getFormatConfig(dp_inst),
			timeAvailable = dt !== null && this.timeDefined;
            this.formattedDate = $.datepicker.formatDate(dateFmt, (dt === null ? new Date() : dt), formatCfg);
            var formattedDateTime = this.formattedDate;
            if (dp_inst.lastVal !== undefined && (dp_inst.lastVal.length > 0 && this.$input.val().length === 0))
                return;

            if (this._defaults.timeOnly === true) {
                formattedDateTime = this.formattedTime;
            } else if (this._defaults.timeOnly !== true && (this._defaults.alwaysSetTime || timeAvailable)) {
                formattedDateTime += this._defaults.separator + this.formattedTime + this._defaults.timeSuffix;
            }

            this.formattedDateTime = formattedDateTime;

            if (!this._defaults.showTimepicker) {
                this.$input.val(this.formattedDate);
            } else if (this.$altInput && this._defaults.altFieldTimeOnly === true) {
                this.$altInput.val(this.formattedTime);
                this.$input.val(this.formattedDate);
            } else if (this.$altInput) {
                this.$altInput.val(formattedDateTime);
                this.$input.val(formattedDateTime);
            } else {
                this.$input.val(formattedDateTime);
            }

            this.$input.trigger("change");
        }

    });

    $.fn.extend({
        //########################################################################
        // shorthand just to use timepicker..
        //########################################################################
        timepicker: function (o) {
            o = o || {};
            var tmp_args = arguments;

            if (typeof o == 'object') tmp_args[0] = $.extend(o, { timeOnly: true });

            return $(this).each(function () {
                $.fn.datetimepicker.apply($(this), tmp_args);
            });
        },

        //########################################################################
        // extend timepicker to datepicker
        //########################################################################
        datetimepicker: function (o) {
            o = o || {};
            var $input = this,
		tmp_args = arguments;

            if (typeof (o) == 'string') {
                if (o == 'getDate')
                    return $.fn.datepicker.apply($(this[0]), tmp_args);
                else
                    return this.each(function () {
                        var $t = $(this);
                        $t.datepicker.apply($t, tmp_args);
                    });
            }
            else
                return this.each(function () {
                    var $t = $(this);
                    $t.datepicker($.timepicker._newInst($t, o)._defaults);
                });
        }
    });

    //########################################################################
    // format the time all pretty... 
    // format = string format of the time
    // time = a {}, not a Date() for timezones
    // options = essentially the regional[].. amNames, pmNames, ampm
    //########################################################################
    if (!$.datepicker)
        $.datepicker = {};
    $.datepicker.formatTime = function (format, time, options) {
        options = options || {};
        options = $.extend($.timepicker._defaults, options);
        time = $.extend({ hour: 0, minute: 0, second: 0, millisec: 0, timezone: '+0000' }, time);

        var tmptime = format;
        var ampmName = options['amNames'][0];

        var hour = parseInt(time.hour, 10);
        if (options.ampm) {
            if (hour > 11) {
                ampmName = options['pmNames'][0];
                if (hour > 12)
                    hour = hour % 12;
            }
            if (hour === 0)
                hour = 12;
        }
        tmptime = tmptime.replace(/(?:hh?|mm?|ss?|[tT]{1,2}|[lz])/g, function (match) {
            switch (match.toLowerCase()) {
                case 'hh': return ('0' + hour).slice(-2);
                case 'h': return hour;
                case 'mm': return ('0' + time.minute).slice(-2);
                case 'm': return time.minute;
                case 'ss': return ('0' + time.second).slice(-2);
                case 's': return time.second;
                case 'l': return ('00' + time.millisec).slice(-3);
                case 'z': return time.timezone;
                case 't': case 'tt':
                    if (options.ampm) {
                        if (match.length == 1)
                            ampmName = ampmName.charAt(0);
                        return match.charAt(0) == 'T' ? ampmName.toUpperCase() : ampmName.toLowerCase();
                    }
                    return '';
            }
        });

        tmptime = $.trim(tmptime);
        return tmptime;
    }

    //########################################################################
    // the bad hack :/ override datepicker so it doesnt close on select
    // inspired: http://stackoverflow.com/questions/1252512/jquery-datepicker-prevent-closing-picker-when-clicking-a-date/1762378#1762378
    //########################################################################
    $.datepicker._base_selectDate = $.datepicker._selectDate;
    $.datepicker._selectDate = function (id, dateStr) {
        var inst = this._getInst($(id)[0]),
		tp_inst = this._get(inst, 'timepicker');

        if (tp_inst) {
            tp_inst._limitMinMaxDateTime(inst, true);
            inst.inline = inst.stay_open = true;
            //This way the onSelect handler called from calendarpicker get the full dateTime
            this._base_selectDate(id, dateStr);
            inst.inline = inst.stay_open = false;
            this._notifyChange(inst);
            this._updateDatepicker(inst);
        }
        else this._base_selectDate(id, dateStr);
    };

    //#############################################################################################
    // second bad hack :/ override datepicker so it triggers an event when changing the input field
    // and does not redraw the datepicker on every selectDate event
    //#############################################################################################
    $.datepicker._base_updateDatepicker = $.datepicker._updateDatepicker;
    $.datepicker._updateDatepicker = function (inst) {

        // don't popup the datepicker if there is another instance already opened
        var input = inst.input[0];
        if ($.datepicker._curInst &&
	   $.datepicker._curInst != inst &&
	   $.datepicker._datepickerShowing &&
	   $.datepicker._lastInput != input) {
            return;
        }

        if (typeof (inst.stay_open) !== 'boolean' || inst.stay_open === false) {

            this._base_updateDatepicker(inst);

            // Reload the time control when changing something in the input text field.
            var tp_inst = this._get(inst, 'timepicker');
            if (tp_inst) tp_inst._addTimePicker(inst);
        }
    };

    //#######################################################################################
    // third bad hack :/ override datepicker so it allows spaces and colon in the input field
    //#######################################################################################
    $.datepicker._base_doKeyPress = $.datepicker._doKeyPress;
    $.datepicker._doKeyPress = function (event) {
        var inst = $.datepicker._getInst(event.target),
		tp_inst = $.datepicker._get(inst, 'timepicker');

        if (tp_inst) {
            if ($.datepicker._get(inst, 'constrainInput')) {
                var ampm = tp_inst._defaults.ampm,
				dateChars = $.datepicker._possibleChars($.datepicker._get(inst, 'dateFormat')),
				datetimeChars = tp_inst._defaults.timeFormat.toString()
								.replace(/[hms]/g, '')
								.replace(/TT/g, ampm ? 'APM' : '')
								.replace(/Tt/g, ampm ? 'AaPpMm' : '')
								.replace(/tT/g, ampm ? 'AaPpMm' : '')
								.replace(/T/g, ampm ? 'AP' : '')
								.replace(/tt/g, ampm ? 'apm' : '')
								.replace(/t/g, ampm ? 'ap' : '') +
								" " +
								tp_inst._defaults.separator +
								tp_inst._defaults.timeSuffix +
								(tp_inst._defaults.showTimezone ? tp_inst._defaults.timezoneList.join('') : '') +
								(tp_inst._defaults.amNames.join('')) +
								(tp_inst._defaults.pmNames.join('')) +
								dateChars,
				chr = String.fromCharCode(event.charCode === undefined ? event.keyCode : event.charCode);
                return event.ctrlKey || (chr < ' ' || !dateChars || datetimeChars.indexOf(chr) > -1);
            }
        }

        return $.datepicker._base_doKeyPress(event);
    };

    //#######################################################################################
    // Override key up event to sync manual input changes.
    //#######################################################################################
    $.datepicker._base_doKeyUp = $.datepicker._doKeyUp;
    $.datepicker._doKeyUp = function (event) {
        var inst = $.datepicker._getInst(event.target),
		tp_inst = $.datepicker._get(inst, 'timepicker');

        if (tp_inst) {
            if (tp_inst._defaults.timeOnly && (inst.input.val() != inst.lastVal)) {
                try {
                    $.datepicker._updateDatepicker(inst);
                }
                catch (err) {
                    $.datepicker.log(err);
                }
            }
        }

        return $.datepicker._base_doKeyUp(event);
    };

    //#######################################################################################
    // override "Today" button to also grab the time.
    //#######################################################################################
    $.datepicker._base_gotoToday = $.datepicker._gotoToday;
    $.datepicker._gotoToday = function (id) {
        var inst = this._getInst($(id)[0]),
		$dp = inst.dpDiv;
        this._base_gotoToday(id);
        var now = new Date();
        var tp_inst = this._get(inst, 'timepicker');
        if (tp_inst && tp_inst._defaults.showTimezone && tp_inst.timezone_select) {
            var tzoffset = now.getTimezoneOffset(); // If +0100, returns -60
            var tzsign = tzoffset > 0 ? '-' : '+';
            tzoffset = Math.abs(tzoffset);
            var tzmin = tzoffset % 60;
            tzoffset = tzsign + ('0' + (tzoffset - tzmin) / 60).slice(-2) + ('0' + tzmin).slice(-2);
            if (tp_inst._defaults.timezoneIso8609)
                tzoffset = tzoffset.substring(0, 3) + ':' + tzoffset.substring(3);
            tp_inst.timezone_select.val(tzoffset);
        }
        this._setTime(inst, now);
        $('.ui-datepicker-today', $dp).click();
    };

    //#######################################################################################
    // Disable & enable the Time in the datetimepicker
    //#######################################################################################
    $.datepicker._disableTimepickerDatepicker = function (target, date, withDate) {
        var inst = this._getInst(target),
	tp_inst = this._get(inst, 'timepicker');
        $(target).datepicker('getDate'); // Init selected[Year|Month|Day]
        if (tp_inst) {
            tp_inst._defaults.showTimepicker = false;
            tp_inst._updateDateTime(inst);
        }
    };

    $.datepicker._enableTimepickerDatepicker = function (target, date, withDate) {
        var inst = this._getInst(target),
	tp_inst = this._get(inst, 'timepicker');
        $(target).datepicker('getDate'); // Init selected[Year|Month|Day]
        if (tp_inst) {
            tp_inst._defaults.showTimepicker = true;
            tp_inst._addTimePicker(inst); // Could be disabled on page load
            tp_inst._updateDateTime(inst);
        }
    };

    //#######################################################################################
    // Create our own set time function
    //#######################################################################################
    $.datepicker._setTime = function (inst, date) {
        var tp_inst = this._get(inst, 'timepicker');
        if (tp_inst) {
            var defaults = tp_inst._defaults,
            // calling _setTime with no date sets time to defaults
			hour = date ? date.getHours() : defaults.hour,
			minute = date ? date.getMinutes() : defaults.minute,
			second = date ? date.getSeconds() : defaults.second,
			millisec = date ? date.getMilliseconds() : defaults.millisec;

            //check if within min/max times..
            if ((hour < defaults.hourMin || hour > defaults.hourMax) || (minute < defaults.minuteMin || minute > defaults.minuteMax) || (second < defaults.secondMin || second > defaults.secondMax) || (millisec < defaults.millisecMin || millisec > defaults.millisecMax)) {
                hour = defaults.hourMin;
                minute = defaults.minuteMin;
                second = defaults.secondMin;
                millisec = defaults.millisecMin;
            }

            tp_inst.hour = hour;
            tp_inst.minute = minute;
            tp_inst.second = second;
            tp_inst.millisec = millisec;

            if (tp_inst.hour_slider) tp_inst.hour_slider.slider('value', hour);
            if (tp_inst.minute_slider) tp_inst.minute_slider.slider('value', minute);
            if (tp_inst.second_slider) tp_inst.second_slider.slider('value', second);
            if (tp_inst.millisec_slider) tp_inst.millisec_slider.slider('value', millisec);

            tp_inst._onTimeChange();
            tp_inst._updateDateTime(inst);
        }
    };

    //#######################################################################################
    // Create new public method to set only time, callable as $().datepicker('setTime', date)
    //#######################################################################################
    $.datepicker._setTimeDatepicker = function (target, date, withDate) {
        var inst = this._getInst(target),
		tp_inst = this._get(inst, 'timepicker');

        if (tp_inst) {
            this._setDateFromField(inst);
            var tp_date;
            if (date) {
                if (typeof date == "string") {
                    tp_inst._parseTime(date, withDate);
                    tp_date = new Date();
                    tp_date.setHours(tp_inst.hour, tp_inst.minute, tp_inst.second, tp_inst.millisec);
                }
                else tp_date = new Date(date.getTime());
                if (tp_date.toString() == 'Invalid Date') tp_date = undefined;
                this._setTime(inst, tp_date);
            }
        }

    };

    //#######################################################################################
    // override setDate() to allow setting time too within Date object
    //#######################################################################################
    $.datepicker._base_setDateDatepicker = $.datepicker._setDateDatepicker;
    $.datepicker._setDateDatepicker = function (target, date) {
        var inst = this._getInst(target),
	tp_date = (date instanceof Date) ? new Date(date.getTime()) : date;

        this._updateDatepicker(inst);
        this._base_setDateDatepicker.apply(this, arguments);
        this._setTimeDatepicker(target, tp_date, true);
    };

    //#######################################################################################
    // override getDate() to allow getting time too within Date object
    //#######################################################################################
    $.datepicker._base_getDateDatepicker = $.datepicker._getDateDatepicker;
    $.datepicker._getDateDatepicker = function (target, noDefault) {
        var inst = this._getInst(target),
		tp_inst = this._get(inst, 'timepicker');

        if (tp_inst) {
            this._setDateFromField(inst, noDefault);
            var date = this._getDate(inst);
            if (date && tp_inst._parseTime($(target).val(), tp_inst.timeOnly)) date.setHours(tp_inst.hour, tp_inst.minute, tp_inst.second, tp_inst.millisec);
            return date;
        }
        return this._base_getDateDatepicker(target, noDefault);
    };

    //#######################################################################################
    // override parseDate() because UI 1.8.14 throws an error about "Extra characters"
    // An option in datapicker to ignore extra format characters would be nicer.
    //#######################################################################################
    $.datepicker._base_parseDate = $.datepicker.parseDate;
    $.datepicker.parseDate = function (format, value, settings) {
        var date;
        try {
            date = this._base_parseDate(format, value, settings);
        } catch (err) {
            if (err.indexOf(":") >= 0) {
                // Hack!  The error message ends with a colon, a space, and
                // the "extra" characters.  We rely on that instead of
                // attempting to perfectly reproduce the parsing algorithm.
                date = this._base_parseDate(format, value.substring(0, value.length - (err.length - err.indexOf(':') - 2)), settings);
            } else {
                // The underlying error was not related to the time
                throw err;
            }
        }
        return date;
    };

    //#######################################################################################
    // override formatDate to set date with time to the input
    //#######################################################################################
    $.datepicker._base_formatDate = $.datepicker._formatDate;
    $.datepicker._formatDate = function (inst, day, month, year) {
        var tp_inst = this._get(inst, 'timepicker');
        if (tp_inst) {
            if (day)
                var b = this._base_formatDate(inst, day, month, year);
            tp_inst._updateDateTime(inst);
            return tp_inst.$input.val();
        }
        return this._base_formatDate(inst);
    };

    //#######################################################################################
    // override options setter to add time to maxDate(Time) and minDate(Time). MaxDate
    //#######################################################################################
    $.datepicker._base_optionDatepicker = $.datepicker._optionDatepicker;
    $.datepicker._optionDatepicker = function (target, name, value) {
        var inst = this._getInst(target),
		tp_inst = this._get(inst, 'timepicker');
        if (tp_inst) {
            var min, max, onselect;
            if (typeof name == 'string') { // if min/max was set with the string
                if (name === 'minDate' || name === 'minDateTime')
                    min = value;
                else if (name === 'maxDate' || name === 'maxDateTime')
                    max = value;
                else if (name === 'onSelect')
                    onselect = value;
            } else if (typeof name == 'object') { //if min/max was set with the JSON
                if (name.minDate)
                    min = name.minDate;
                else if (name.minDateTime)
                    min = name.minDateTime;
                else if (name.maxDate)
                    max = name.maxDate;
                else if (name.maxDateTime)
                    max = name.maxDateTime;
            }
            if (min) { //if min was set
                if (min == 0)
                    min = new Date();
                else
                    min = new Date(min);

                tp_inst._defaults.minDate = min;
                tp_inst._defaults.minDateTime = min;
            } else if (max) { //if max was set
                if (max == 0)
                    max = new Date();
                else
                    max = new Date(max);
                tp_inst._defaults.maxDate = max;
                tp_inst._defaults.maxDateTime = max;
            }
            else if (onselect)
                tp_inst._defaults.onSelect = onselect;
        }
        if (value === undefined)
            return this._base_optionDatepicker(target, name);
        return this._base_optionDatepicker(target, name, value);
    };

    //#######################################################################################
    // jQuery extend now ignores nulls!
    //#######################################################################################
    function extendRemove(target, props) {
        $.extend(target, props);
        for (var name in props)
            if (props[name] === null || props[name] === undefined)
                target[name] = props[name];
        return target;
    };

    $.timepicker = new Timepicker(); // singleton instance
    $.timepicker.version = "0.9.9";

})(jQuery);


/*END JQ UI DATETIME*/

/**
* Version: 1.0 Alpha-1 
* Build Date: 13-Nov-2007
* Copyright (c) 2006-2007, Coolite Inc. (http://www.coolite.com/). All rights reserved.
* License: Licensed under The MIT License. See license.txt and http://www.datejs.com/license/. 
* Website: http://www.datejs.com/ or http://www.coolite.com/datejs/
*/
Date.CultureInfo = { name: "en-US", englishName: "English (United States)", nativeName: "English (United States)", dayNames: ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"], abbreviatedDayNames: ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"], shortestDayNames: ["Su", "Mo", "Tu", "We", "Th", "Fr", "Sa"], firstLetterDayNames: ["S", "M", "T", "W", "T", "F", "S"], monthNames: ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"], abbreviatedMonthNames: ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"], amDesignator: "AM", pmDesignator: "PM", firstDayOfWeek: 0, twoDigitYearMax: 2029, dateElementOrder: "mdy", formatPatterns: { shortDate: "M/d/yyyy", longDate: "dddd, MMMM dd, yyyy", shortTime: "h:mm tt", longTime: "h:mm:ss tt", fullDateTime: "dddd, MMMM dd, yyyy h:mm:ss tt", sortableDateTime: "yyyy-MM-ddTHH:mm:ss", universalSortableDateTime: "yyyy-MM-dd HH:mm:ssZ", rfc1123: "ddd, dd MMM yyyy HH:mm:ss GMT", monthDay: "MMMM dd", yearMonth: "MMMM, yyyy" }, regexPatterns: { jan: /^jan(uary)?/i, feb: /^feb(ruary)?/i, mar: /^mar(ch)?/i, apr: /^apr(il)?/i, may: /^may/i, jun: /^jun(e)?/i, jul: /^jul(y)?/i, aug: /^aug(ust)?/i, sep: /^sep(t(ember)?)?/i, oct: /^oct(ober)?/i, nov: /^nov(ember)?/i, dec: /^dec(ember)?/i, sun: /^su(n(day)?)?/i, mon: /^mo(n(day)?)?/i, tue: /^tu(e(s(day)?)?)?/i, wed: /^we(d(nesday)?)?/i, thu: /^th(u(r(s(day)?)?)?)?/i, fri: /^fr(i(day)?)?/i, sat: /^sa(t(urday)?)?/i, future: /^next/i, past: /^last|past|prev(ious)?/i, add: /^(\+|after|from)/i, subtract: /^(\-|before|ago)/i, yesterday: /^yesterday/i, today: /^t(oday)?/i, tomorrow: /^tomorrow/i, now: /^n(ow)?/i, millisecond: /^ms|milli(second)?s?/i, second: /^sec(ond)?s?/i, minute: /^min(ute)?s?/i, hour: /^h(ou)?rs?/i, week: /^w(ee)?k/i, month: /^m(o(nth)?s?)?/i, day: /^d(ays?)?/i, year: /^y((ea)?rs?)?/i, shortMeridian: /^(a|p)/i, longMeridian: /^(a\.?m?\.?|p\.?m?\.?)/i, timezone: /^((e(s|d)t|c(s|d)t|m(s|d)t|p(s|d)t)|((gmt)?\s*(\+|\-)\s*\d\d\d\d?)|gmt)/i, ordinalSuffix: /^\s*(st|nd|rd|th)/i, timeContext: /^\s*(\:|a|p)/i }, abbreviatedTimeZoneStandard: { GMT: "-000", EST: "-0400", CST: "-0500", MST: "-0600", PST: "-0700" }, abbreviatedTimeZoneDST: { GMT: "-000", EDT: "-0500", CDT: "-0600", MDT: "-0700", PDT: "-0800"} };
Date.getMonthNumberFromName = function (name) {
    var n = Date.CultureInfo.monthNames, m = Date.CultureInfo.abbreviatedMonthNames, s = name.toLowerCase(); for (var i = 0; i < n.length; i++) { if (n[i].toLowerCase() == s || m[i].toLowerCase() == s) { return i; } }
    return -1;
}; Date.getDayNumberFromName = function (name) {
    var n = Date.CultureInfo.dayNames, m = Date.CultureInfo.abbreviatedDayNames, o = Date.CultureInfo.shortestDayNames, s = name.toLowerCase(); for (var i = 0; i < n.length; i++) { if (n[i].toLowerCase() == s || m[i].toLowerCase() == s) { return i; } }
    return -1;
}; Date.isLeapYear = function (year) { return (((year % 4 === 0) && (year % 100 !== 0)) || (year % 400 === 0)); }; Date.getDaysInMonth = function (year, month) { return [31, (Date.isLeapYear(year) ? 29 : 28), 31, 30, 31, 30, 31, 31, 30, 31, 30, 31][month]; }; Date.getTimezoneOffset = function (s, dst) { return (dst || false) ? Date.CultureInfo.abbreviatedTimeZoneDST[s.toUpperCase()] : Date.CultureInfo.abbreviatedTimeZoneStandard[s.toUpperCase()]; }; Date.getTimezoneAbbreviation = function (offset, dst) {
    var n = (dst || false) ? Date.CultureInfo.abbreviatedTimeZoneDST : Date.CultureInfo.abbreviatedTimeZoneStandard, p; for (p in n) { if (n[p] === offset) { return p; } }
    return null;
}; Date.prototype.clone = function () { return new Date(this.getTime()); }; Date.prototype.compareTo = function (date) {
    if (isNaN(this)) { throw new Error(this); }
    if (date instanceof Date && !isNaN(date)) { return (this > date) ? 1 : (this < date) ? -1 : 0; } else { throw new TypeError(date); }
}; Date.prototype.equals = function (date) { return (this.compareTo(date) === 0); }; Date.prototype.between = function (start, end) { var t = this.getTime(); return t >= start.getTime() && t <= end.getTime(); }; Date.prototype.addMilliseconds = function (value) { this.setMilliseconds(this.getMilliseconds() + value); return this; }; Date.prototype.addSeconds = function (value) { return this.addMilliseconds(value * 1000); }; Date.prototype.addMinutes = function (value) { return this.addMilliseconds(value * 60000); }; Date.prototype.addHours = function (value) { return this.addMilliseconds(value * 3600000); }; Date.prototype.addDays = function (value) { return this.addMilliseconds(value * 86400000); }; Date.prototype.addWeeks = function (value) { return this.addMilliseconds(value * 604800000); }; Date.prototype.addMonths = function (value) { var n = this.getDate(); this.setDate(1); this.setMonth(this.getMonth() + value); this.setDate(Math.min(n, this.getDaysInMonth())); return this; }; Date.prototype.addYears = function (value) { return this.addMonths(value * 12); }; Date.prototype.add = function (config) {
    if (typeof config == "number") { this._orient = config; return this; }
    var x = config; if (x.millisecond || x.milliseconds) { this.addMilliseconds(x.millisecond || x.milliseconds); }
    if (x.second || x.seconds) { this.addSeconds(x.second || x.seconds); }
    if (x.minute || x.minutes) { this.addMinutes(x.minute || x.minutes); }
    if (x.hour || x.hours) { this.addHours(x.hour || x.hours); }
    if (x.month || x.months) { this.addMonths(x.month || x.months); }
    if (x.year || x.years) { this.addYears(x.year || x.years); }
    if (x.day || x.days) { this.addDays(x.day || x.days); }
    return this;
}; Date._validate = function (value, min, max, name) {
    if (typeof value != "number") { throw new TypeError(value + " is not a Number."); } else if (value < min || value > max) { throw new RangeError(value + " is not a valid value for " + name + "."); }
    return true;
}; Date.validateMillisecond = function (n) { return Date._validate(n, 0, 999, "milliseconds"); }; Date.validateSecond = function (n) { return Date._validate(n, 0, 59, "seconds"); }; Date.validateMinute = function (n) { return Date._validate(n, 0, 59, "minutes"); }; Date.validateHour = function (n) { return Date._validate(n, 0, 23, "hours"); }; Date.validateDay = function (n, year, month) { return Date._validate(n, 1, Date.getDaysInMonth(year, month), "days"); }; Date.validateMonth = function (n) { return Date._validate(n, 0, 11, "months"); }; Date.validateYear = function (n) { return Date._validate(n, 1, 9999, "seconds"); }; Date.prototype.set = function (config) {
    var x = config; if (!x.millisecond && x.millisecond !== 0) { x.millisecond = -1; }
    if (!x.second && x.second !== 0) { x.second = -1; }
    if (!x.minute && x.minute !== 0) { x.minute = -1; }
    if (!x.hour && x.hour !== 0) { x.hour = -1; }
    if (!x.day && x.day !== 0) { x.day = -1; }
    if (!x.month && x.month !== 0) { x.month = -1; }
    if (!x.year && x.year !== 0) { x.year = -1; }
    if (x.millisecond != -1 && Date.validateMillisecond(x.millisecond)) { this.addMilliseconds(x.millisecond - this.getMilliseconds()); }
    if (x.second != -1 && Date.validateSecond(x.second)) { this.addSeconds(x.second - this.getSeconds()); }
    if (x.minute != -1 && Date.validateMinute(x.minute)) { this.addMinutes(x.minute - this.getMinutes()); }
    if (x.hour != -1 && Date.validateHour(x.hour)) { this.addHours(x.hour - this.getHours()); }
    if (x.month !== -1 && Date.validateMonth(x.month)) { this.addMonths(x.month - this.getMonth()); }
    if (x.year != -1 && Date.validateYear(x.year)) { this.addYears(x.year - this.getFullYear()); }
    if (x.day != -1 && Date.validateDay(x.day, this.getFullYear(), this.getMonth())) { this.addDays(x.day - this.getDate()); }
    if (x.timezone) { this.setTimezone(x.timezone); }
    if (x.timezoneOffset) { this.setTimezoneOffset(x.timezoneOffset); }
    return this;
}; Date.prototype.clearTime = function () { this.setHours(0); this.setMinutes(0); this.setSeconds(0); this.setMilliseconds(0); return this; }; Date.prototype.isLeapYear = function () { var y = this.getFullYear(); return (((y % 4 === 0) && (y % 100 !== 0)) || (y % 400 === 0)); }; Date.prototype.isWeekday = function () { return !(this.is().sat() || this.is().sun()); }; Date.prototype.getDaysInMonth = function () { return Date.getDaysInMonth(this.getFullYear(), this.getMonth()); }; Date.prototype.moveToFirstDayOfMonth = function () { return this.set({ day: 1 }); }; Date.prototype.moveToLastDayOfMonth = function () { return this.set({ day: this.getDaysInMonth() }); }; Date.prototype.moveToDayOfWeek = function (day, orient) { var diff = (day - this.getDay() + 7 * (orient || +1)) % 7; return this.addDays((diff === 0) ? diff += 7 * (orient || +1) : diff); }; Date.prototype.moveToMonth = function (month, orient) { var diff = (month - this.getMonth() + 12 * (orient || +1)) % 12; return this.addMonths((diff === 0) ? diff += 12 * (orient || +1) : diff); }; Date.prototype.getDayOfYear = function () { return Math.floor((this - new Date(this.getFullYear(), 0, 1)) / 86400000); }; Date.prototype.getWeekOfYear = function (firstDayOfWeek) {
    var y = this.getFullYear(), m = this.getMonth(), d = this.getDate(); var dow = firstDayOfWeek || Date.CultureInfo.firstDayOfWeek; var offset = 7 + 1 - new Date(y, 0, 1).getDay(); if (offset == 8) { offset = 1; }
    var daynum = ((Date.UTC(y, m, d, 0, 0, 0) - Date.UTC(y, 0, 1, 0, 0, 0)) / 86400000) + 1; var w = Math.floor((daynum - offset + 7) / 7); if (w === dow) { y--; var prevOffset = 7 + 1 - new Date(y, 0, 1).getDay(); if (prevOffset == 2 || prevOffset == 8) { w = 53; } else { w = 52; } }
    return w;
}; Date.prototype.isDST = function () {
    console.log('isDST');
    var mat = this.toString().match(/(E|C|M|P)(S|D)T/);
    return (mat && mat[2] == "D") || this.toString().match(/daylight/i);
}; Date.prototype.getTimezone = function () { return Date.getTimezoneAbbreviation(this.getUTCOffset, this.isDST()); }; Date.prototype.setTimezoneOffset = function (s) { var here = this.getTimezoneOffset(), there = Number(s) * -6 / 10; this.addMinutes(there - here); return this; }; Date.prototype.setTimezone = function (s) { return this.setTimezoneOffset(Date.getTimezoneOffset(s)); }; Date.prototype.getUTCOffset = function () { var n = this.getTimezoneOffset() * -10 / 6, r; if (n < 0) { r = (n - 10000).toString(); return r[0] + r.substr(2); } else { r = (n + 10000).toString(); return "+" + r.substr(1); } }; Date.prototype.getDayName = function (abbrev) { return abbrev ? Date.CultureInfo.abbreviatedDayNames[this.getDay()] : Date.CultureInfo.dayNames[this.getDay()]; }; Date.prototype.getMonthName = function (abbrev) { return abbrev ? Date.CultureInfo.abbreviatedMonthNames[this.getMonth()] : Date.CultureInfo.monthNames[this.getMonth()]; }; Date.prototype._toString = Date.prototype.toString; Date.prototype.toString = function (format) { var self = this; var p = function p(s) { return (s.toString().length == 1) ? "0" + s : s; }; return format ? format.replace(/dd?d?d?|MM?M?M?|yy?y?y?|hh?|HH?|mm?|ss?|tt?|zz?z?/g, function (format) { switch (format) { case "hh": return p(self.getHours() < 13 ? self.getHours() : (self.getHours() - 12)); case "h": return self.getHours() < 13 ? self.getHours() : (self.getHours() - 12); case "HH": return p(self.getHours()); case "H": return self.getHours(); case "mm": return p(self.getMinutes()); case "m": return self.getMinutes(); case "ss": return p(self.getSeconds()); case "s": return self.getSeconds(); case "yyyy": return self.getFullYear(); case "yy": return self.getFullYear().toString().substring(2, 4); case "dddd": return self.getDayName(); case "ddd": return self.getDayName(true); case "dd": return p(self.getDate()); case "d": return self.getDate().toString(); case "MMMM": return self.getMonthName(); case "MMM": return self.getMonthName(true); case "MM": return p((self.getMonth() + 1)); case "M": return self.getMonth() + 1; case "t": return self.getHours() < 12 ? Date.CultureInfo.amDesignator.substring(0, 1) : Date.CultureInfo.pmDesignator.substring(0, 1); case "tt": return self.getHours() < 12 ? Date.CultureInfo.amDesignator : Date.CultureInfo.pmDesignator; case "zzz": case "zz": case "z": return ""; } }) : this._toString(); };
Date.now = function () { return new Date(); }; Date.today = function () { return Date.now().clearTime(); }; Date.prototype._orient = +1; Date.prototype.next = function () { this._orient = +1; return this; }; Date.prototype.last = Date.prototype.prev = Date.prototype.previous = function () { this._orient = -1; return this; }; Date.prototype._is = false; Date.prototype.is = function () { this._is = true; return this; }; Number.prototype._dateElement = "day"; Number.prototype.fromNow = function () { var c = {}; c[this._dateElement] = this; return Date.now().add(c); }; Number.prototype.ago = function () { var c = {}; c[this._dateElement] = this * -1; return Date.now().add(c); }; (function () {
    var $D = Date.prototype, $N = Number.prototype; var dx = ("sunday monday tuesday wednesday thursday friday saturday").split(/\s/), mx = ("january february march april may june july august september october november december").split(/\s/), px = ("Millisecond Second Minute Hour Day Week Month Year").split(/\s/), de; var df = function (n) {
        return function () {
            if (this._is) { this._is = false; return this.getDay() == n; }
            return this.moveToDayOfWeek(n, this._orient);
        };
    }; for (var i = 0; i < dx.length; i++) { $D[dx[i]] = $D[dx[i].substring(0, 3)] = df(i); }
    var mf = function (n) {
        return function () {
            if (this._is) { this._is = false; return this.getMonth() === n; }
            return this.moveToMonth(n, this._orient);
        };
    }; for (var j = 0; j < mx.length; j++) { $D[mx[j]] = $D[mx[j].substring(0, 3)] = mf(j); }
    var ef = function (j) {
        return function () {
            if (j.substring(j.length - 1) != "s") { j += "s"; }
            return this["add" + j](this._orient);
        };
    }; var nf = function (n) { return function () { this._dateElement = n; return this; }; }; for (var k = 0; k < px.length; k++) { de = px[k].toLowerCase(); $D[de] = $D[de + "s"] = ef(px[k]); $N[de] = $N[de + "s"] = nf(de); }
} ()); Date.prototype.toJSONString = function () { return this.toString("yyyy-MM-ddThh:mm:ssZ"); }; Date.prototype.toShortDateString = function () { return this.toString(Date.CultureInfo.formatPatterns.shortDatePattern); }; Date.prototype.toLongDateString = function () { return this.toString(Date.CultureInfo.formatPatterns.longDatePattern); }; Date.prototype.toShortTimeString = function () { return this.toString(Date.CultureInfo.formatPatterns.shortTimePattern); }; Date.prototype.toLongTimeString = function () { return this.toString(Date.CultureInfo.formatPatterns.longTimePattern); }; Date.prototype.getOrdinal = function () { switch (this.getDate()) { case 1: case 21: case 31: return "st"; case 2: case 22: return "nd"; case 3: case 23: return "rd"; default: return "th"; } };
(function () {
    Date.Parsing = { Exception: function (s) { this.message = "Parse error at '" + s.substring(0, 10) + " ...'"; } }; var $P = Date.Parsing; var _ = $P.Operators = { rtoken: function (r) { return function (s) { var mx = s.match(r); if (mx) { return ([mx[0], s.substring(mx[0].length)]); } else { throw new $P.Exception(s); } }; }, token: function (s) { return function (s) { return _.rtoken(new RegExp("^\s*" + s + "\s*"))(s); }; }, stoken: function (s) { return _.rtoken(new RegExp("^" + s)); }, until: function (p) {
        return function (s) {
            var qx = [], rx = null; while (s.length) {
                try { rx = p.call(this, s); } catch (e) { qx.push(rx[0]); s = rx[1]; continue; }
                break;
            }
            return [qx, s];
        };
    }, many: function (p) {
        return function (s) {
            var rx = [], r = null; while (s.length) {
                try { r = p.call(this, s); } catch (e) { return [rx, s]; }
                rx.push(r[0]); s = r[1];
            }
            return [rx, s];
        };
    }, optional: function (p) {
        return function (s) {
            var r = null; try { r = p.call(this, s); } catch (e) { return [null, s]; }
            return [r[0], r[1]];
        };
    }, not: function (p) {
        return function (s) {
            try { p.call(this, s); } catch (e) { return [null, s]; }
            throw new $P.Exception(s);
        };
    }, ignore: function (p) { return p ? function (s) { var r = null; r = p.call(this, s); return [null, r[1]]; } : null; }, product: function () {
        var px = arguments[0], qx = Array.prototype.slice.call(arguments, 1), rx = []; for (var i = 0; i < px.length; i++) { rx.push(_.each(px[i], qx)); }
        return rx;
    }, cache: function (rule) {
        var cache = {}, r = null; return function (s) {
            try { r = cache[s] = (cache[s] || rule.call(this, s)); } catch (e) { r = cache[s] = e; }
            if (r instanceof $P.Exception) { throw r; } else { return r; }
        };
    }, any: function () {
        var px = arguments; return function (s) {
            var r = null; for (var i = 0; i < px.length; i++) {
                if (px[i] == null) { continue; }
                try { r = (px[i].call(this, s)); } catch (e) { r = null; }
                if (r) { return r; }
            }
            throw new $P.Exception(s);
        };
    }, each: function () {
        var px = arguments; return function (s) {
            var rx = [], r = null; for (var i = 0; i < px.length; i++) {
                if (px[i] == null) { continue; }
                try { r = (px[i].call(this, s)); } catch (e) { throw new $P.Exception(s); }
                rx.push(r[0]); s = r[1];
            }
            return [rx, s];
        };
    }, all: function () { var px = arguments, _ = _; return _.each(_.optional(px)); }, sequence: function (px, d, c) {
        d = d || _.rtoken(/^\s*/); c = c || null; if (px.length == 1) { return px[0]; }
        return function (s) {
            var r = null, q = null; var rx = []; for (var i = 0; i < px.length; i++) {
                try { r = px[i].call(this, s); } catch (e) { break; }
                rx.push(r[0]); try { q = d.call(this, r[1]); } catch (ex) { q = null; break; }
                s = q[1];
            }
            if (!r) { throw new $P.Exception(s); }
            if (q) { throw new $P.Exception(q[1]); }
            if (c) { try { r = c.call(this, r[1]); } catch (ey) { throw new $P.Exception(r[1]); } }
            return [rx, (r ? r[1] : s)];
        };
    }, between: function (d1, p, d2) { d2 = d2 || d1; var _fn = _.each(_.ignore(d1), p, _.ignore(d2)); return function (s) { var rx = _fn.call(this, s); return [[rx[0][0], r[0][2]], rx[1]]; }; }, list: function (p, d, c) { d = d || _.rtoken(/^\s*/); c = c || null; return (p instanceof Array ? _.each(_.product(p.slice(0, -1), _.ignore(d)), p.slice(-1), _.ignore(c)) : _.each(_.many(_.each(p, _.ignore(d))), px, _.ignore(c))); }, set: function (px, d, c) {
        d = d || _.rtoken(/^\s*/); c = c || null; return function (s) {
            var r = null, p = null, q = null, rx = null, best = [[], s], last = false; for (var i = 0; i < px.length; i++) {
                q = null; p = null; r = null; last = (px.length == 1); try { r = px[i].call(this, s); } catch (e) { continue; }
                rx = [[r[0]], r[1]]; if (r[1].length > 0 && !last) { try { q = d.call(this, r[1]); } catch (ex) { last = true; } } else { last = true; }
                if (!last && q[1].length === 0) { last = true; }
                if (!last) {
                    var qx = []; for (var j = 0; j < px.length; j++) { if (i != j) { qx.push(px[j]); } }
                    p = _.set(qx, d).call(this, q[1]); if (p[0].length > 0) { rx[0] = rx[0].concat(p[0]); rx[1] = p[1]; }
                }
                if (rx[1].length < best[1].length) { best = rx; }
                if (best[1].length === 0) { break; }
            }
            if (best[0].length === 0) { return best; }
            if (c) {
                try { q = c.call(this, best[1]); } catch (ey) { throw new $P.Exception(best[1]); }
                best[1] = q[1];
            }
            return best;
        };
    }, forward: function (gr, fname) { return function (s) { return gr[fname].call(this, s); }; }, replace: function (rule, repl) { return function (s) { var r = rule.call(this, s); return [repl, r[1]]; }; }, process: function (rule, fn) { return function (s) { var r = rule.call(this, s); return [fn.call(this, r[0]), r[1]]; }; }, min: function (min, rule) {
        return function (s) {
            var rx = rule.call(this, s); if (rx[0].length < min) { throw new $P.Exception(s); }
            return rx;
        };
    }
    }; var _generator = function (op) {
        return function () {
            var args = null, rx = []; if (arguments.length > 1) { args = Array.prototype.slice.call(arguments); } else if (arguments[0] instanceof Array) { args = arguments[0]; }
            if (args) { for (var i = 0, px = args.shift(); i < px.length; i++) { args.unshift(px[i]); rx.push(op.apply(null, args)); args.shift(); return rx; } } else { return op.apply(null, arguments); }
        };
    }; var gx = "optional not ignore cache".split(/\s/); for (var i = 0; i < gx.length; i++) { _[gx[i]] = _generator(_[gx[i]]); }
    var _vector = function (op) { return function () { if (arguments[0] instanceof Array) { return op.apply(null, arguments[0]); } else { return op.apply(null, arguments); } }; }; var vx = "each any all".split(/\s/); for (var j = 0; j < vx.length; j++) { _[vx[j]] = _vector(_[vx[j]]); }
} ()); (function () {
    var flattenAndCompact = function (ax) {
        var rx = []; for (var i = 0; i < ax.length; i++) { if (ax[i] instanceof Array) { rx = rx.concat(flattenAndCompact(ax[i])); } else { if (ax[i]) { rx.push(ax[i]); } } }
        return rx;
    }; Date.Grammar = {}; Date.Translator = { hour: function (s) { return function () { this.hour = Number(s); }; }, minute: function (s) { return function () { this.minute = Number(s); }; }, second: function (s) { return function () { this.second = Number(s); }; }, meridian: function (s) { return function () { this.meridian = s.slice(0, 1).toLowerCase(); }; }, timezone: function (s) { return function () { var n = s.replace(/[^\d\+\-]/g, ""); if (n.length) { this.timezoneOffset = Number(n); } else { this.timezone = s.toLowerCase(); } }; }, day: function (x) { var s = x[0]; return function () { this.day = Number(s.match(/\d+/)[0]); }; }, month: function (s) { return function () { this.month = ((s.length == 3) ? Date.getMonthNumberFromName(s) : (Number(s) - 1)); }; }, year: function (s) { return function () { var n = Number(s); this.year = ((s.length > 2) ? n : (n + (((n + 2000) < Date.CultureInfo.twoDigitYearMax) ? 2000 : 1900))); }; }, rday: function (s) { return function () { switch (s) { case "yesterday": this.days = -1; break; case "tomorrow": this.days = 1; break; case "today": this.days = 0; break; case "now": this.days = 0; this.now = true; break; } }; }, finishExact: function (x) {
        x = (x instanceof Array) ? x : [x]; var now = new Date(); this.year = now.getFullYear(); this.month = now.getMonth(); this.day = 1; this.hour = 0; this.minute = 0; this.second = 0; for (var i = 0; i < x.length; i++) { if (x[i]) { x[i].call(this); } }
        this.hour = (this.meridian == "p" && this.hour < 13) ? this.hour + 12 : this.hour; if (this.day > Date.getDaysInMonth(this.year, this.month)) { throw new RangeError(this.day + " is not a valid value for days."); }
        var r = new Date(this.year, this.month, this.day, this.hour, this.minute, this.second); if (this.timezone) { r.set({ timezone: this.timezone }); } else if (this.timezoneOffset) { r.set({ timezoneOffset: this.timezoneOffset }); }
        return r;
    }, finish: function (x) {
        x = (x instanceof Array) ? flattenAndCompact(x) : [x]; if (x.length === 0) { return null; }
        for (var i = 0; i < x.length; i++) { if (typeof x[i] == "function") { x[i].call(this); } }
        if (this.now) { return new Date(); }
        var today = Date.today(); var method = null; var expression = !!(this.days != null || this.orient || this.operator); if (expression) {
            var gap, mod, orient; orient = ((this.orient == "past" || this.operator == "subtract") ? -1 : 1); if (this.weekday) { this.unit = "day"; gap = (Date.getDayNumberFromName(this.weekday) - today.getDay()); mod = 7; this.days = gap ? ((gap + (orient * mod)) % mod) : (orient * mod); }
            if (this.month) { this.unit = "month"; gap = (this.month - today.getMonth()); mod = 12; this.months = gap ? ((gap + (orient * mod)) % mod) : (orient * mod); this.month = null; }
            if (!this.unit) { this.unit = "day"; }
            if (this[this.unit + "s"] == null || this.operator != null) {
                if (!this.value) { this.value = 1; }
                if (this.unit == "week") { this.unit = "day"; this.value = this.value * 7; }
                this[this.unit + "s"] = this.value * orient;
            }
            return today.add(this);
        } else {
            if (this.meridian && this.hour) { this.hour = (this.hour < 13 && this.meridian == "p") ? this.hour + 12 : this.hour; }
            if (this.weekday && !this.day) { this.day = (today.addDays((Date.getDayNumberFromName(this.weekday) - today.getDay()))).getDate(); }
            if (this.month && !this.day) { this.day = 1; }
            return today.set(this);
        }
    }
    }; var _ = Date.Parsing.Operators, g = Date.Grammar, t = Date.Translator, _fn; g.datePartDelimiter = _.rtoken(/^([\s\-\.\,\/\x27]+)/); g.timePartDelimiter = _.stoken(":"); g.whiteSpace = _.rtoken(/^\s*/); g.generalDelimiter = _.rtoken(/^(([\s\,]|at|on)+)/); var _C = {}; g.ctoken = function (keys) {
        var fn = _C[keys]; if (!fn) {
            var c = Date.CultureInfo.regexPatterns; var kx = keys.split(/\s+/), px = []; for (var i = 0; i < kx.length; i++) { px.push(_.replace(_.rtoken(c[kx[i]]), kx[i])); }
            fn = _C[keys] = _.any.apply(null, px);
        }
        return fn;
    }; g.ctoken2 = function (key) { return _.rtoken(Date.CultureInfo.regexPatterns[key]); }; g.h = _.cache(_.process(_.rtoken(/^(0[0-9]|1[0-2]|[1-9])/), t.hour)); g.hh = _.cache(_.process(_.rtoken(/^(0[0-9]|1[0-2])/), t.hour)); g.H = _.cache(_.process(_.rtoken(/^([0-1][0-9]|2[0-3]|[0-9])/), t.hour)); g.HH = _.cache(_.process(_.rtoken(/^([0-1][0-9]|2[0-3])/), t.hour)); g.m = _.cache(_.process(_.rtoken(/^([0-5][0-9]|[0-9])/), t.minute)); g.mm = _.cache(_.process(_.rtoken(/^[0-5][0-9]/), t.minute)); g.s = _.cache(_.process(_.rtoken(/^([0-5][0-9]|[0-9])/), t.second)); g.ss = _.cache(_.process(_.rtoken(/^[0-5][0-9]/), t.second)); g.hms = _.cache(_.sequence([g.H, g.mm, g.ss], g.timePartDelimiter)); g.t = _.cache(_.process(g.ctoken2("shortMeridian"), t.meridian)); g.tt = _.cache(_.process(g.ctoken2("longMeridian"), t.meridian)); g.z = _.cache(_.process(_.rtoken(/^(\+|\-)?\s*\d\d\d\d?/), t.timezone)); g.zz = _.cache(_.process(_.rtoken(/^(\+|\-)\s*\d\d\d\d/), t.timezone)); g.zzz = _.cache(_.process(g.ctoken2("timezone"), t.timezone)); g.timeSuffix = _.each(_.ignore(g.whiteSpace), _.set([g.tt, g.zzz])); g.time = _.each(_.optional(_.ignore(_.stoken("T"))), g.hms, g.timeSuffix); g.d = _.cache(_.process(_.each(_.rtoken(/^([0-2]\d|3[0-1]|\d)/), _.optional(g.ctoken2("ordinalSuffix"))), t.day)); g.dd = _.cache(_.process(_.each(_.rtoken(/^([0-2]\d|3[0-1])/), _.optional(g.ctoken2("ordinalSuffix"))), t.day)); g.ddd = g.dddd = _.cache(_.process(g.ctoken("sun mon tue wed thu fri sat"), function (s) { return function () { this.weekday = s; }; })); g.M = _.cache(_.process(_.rtoken(/^(1[0-2]|0\d|\d)/), t.month)); g.MM = _.cache(_.process(_.rtoken(/^(1[0-2]|0\d)/), t.month)); g.MMM = g.MMMM = _.cache(_.process(g.ctoken("jan feb mar apr may jun jul aug sep oct nov dec"), t.month)); g.y = _.cache(_.process(_.rtoken(/^(\d\d?)/), t.year)); g.yy = _.cache(_.process(_.rtoken(/^(\d\d)/), t.year)); g.yyy = _.cache(_.process(_.rtoken(/^(\d\d?\d?\d?)/), t.year)); g.yyyy = _.cache(_.process(_.rtoken(/^(\d\d\d\d)/), t.year)); _fn = function () { return _.each(_.any.apply(null, arguments), _.not(g.ctoken2("timeContext"))); }; g.day = _fn(g.d, g.dd); g.month = _fn(g.M, g.MMM); g.year = _fn(g.yyyy, g.yy); g.orientation = _.process(g.ctoken("past future"), function (s) { return function () { this.orient = s; }; }); g.operator = _.process(g.ctoken("add subtract"), function (s) { return function () { this.operator = s; }; }); g.rday = _.process(g.ctoken("yesterday tomorrow today now"), t.rday); g.unit = _.process(g.ctoken("minute hour day week month year"), function (s) { return function () { this.unit = s; }; }); g.value = _.process(_.rtoken(/^\d\d?(st|nd|rd|th)?/), function (s) { return function () { this.value = s.replace(/\D/g, ""); }; }); g.expression = _.set([g.rday, g.operator, g.value, g.unit, g.orientation, g.ddd, g.MMM]); _fn = function () { return _.set(arguments, g.datePartDelimiter); }; g.mdy = _fn(g.ddd, g.month, g.day, g.year); g.ymd = _fn(g.ddd, g.year, g.month, g.day); g.dmy = _fn(g.ddd, g.day, g.month, g.year); g.date = function (s) { return ((g[Date.CultureInfo.dateElementOrder] || g.mdy).call(this, s)); }; g.format = _.process(_.many(_.any(_.process(_.rtoken(/^(dd?d?d?|MM?M?M?|yy?y?y?|hh?|HH?|mm?|ss?|tt?|zz?z?)/), function (fmt) { if (g[fmt]) { return g[fmt]; } else { throw Date.Parsing.Exception(fmt); } }), _.process(_.rtoken(/^[^dMyhHmstz]+/), function (s) { return _.ignore(_.stoken(s)); }))), function (rules) { return _.process(_.each.apply(null, rules), t.finishExact); }); var _F = {}; var _get = function (f) { return _F[f] = (_F[f] || g.format(f)[0]); }; g.formats = function (fx) {
        if (fx instanceof Array) {
            var rx = []; for (var i = 0; i < fx.length; i++) { rx.push(_get(fx[i])); }
            return _.any.apply(null, rx);
        } else { return _get(fx); }
    }; g._formats = g.formats(["yyyy-MM-ddTHH:mm:ss", "ddd, MMM dd, yyyy H:mm:ss tt", "ddd MMM d yyyy HH:mm:ss zzz", "d"]); g._start = _.process(_.set([g.date, g.time, g.expression], g.generalDelimiter, g.whiteSpace), t.finish); g.start = function (s) {
        try { var r = g._formats.call({}, s); if (r[1].length === 0) { return r; } } catch (e) { }
        return g._start.call({}, s);
    };
} ()); Date._parse = Date.parse; Date.parse = function (s) {
    var r = null; if (!s) { return null; }
    try { r = Date.Grammar.start.call({}, s); } catch (e) { return null; }
    return ((r[1].length === 0) ? r[0] : null);
}; Date.getParseFunction = function (fx) {
    var fn = Date.Grammar.formats(fx); return function (s) {
        var r = null; try { r = fn.call({}, s); } catch (e) { return null; }
        return ((r[1].length === 0) ? r[0] : null);
    };
}; Date.parseExact = function (s, fx) { return Date.getParseFunction(fx)(s); };


// Backbone.Validation v0.4.0
//
// Copyright (C)2011 Thomas Pedersen
// Distributed under MIT License
//
// Documentation and full license availabe at:
// http://github.com/thedersen/backbone.validation
if (!window._)
    window._ = {};
if (!window.Backbone)
    window.Backbone = {};

_.isInt = function (obj) {
    if (_.isNumber(obj) && (parseFloat(obj) == parseInt(obj, 10))) {
        return true;
    } else {
        return false;
    }
};

negate_func = function (func) {
    return function (value, attr) { return !func(value, attr); };
};

Backbone.Validation = function (model) {
    var self = this;
    this.defaultOptions = {
        forceUpdate: false,
        selector: 'name'
    };

    this.getValidatedAttrs = function () {
        return _.reduce(_.keys(model.validation), function (memo, key) {
            memo[key] = undefined;
            return memo;
        }, {});
    };

    this.getValidators = function (attr) {
        var validation = model.validation[attr] || {};

        if (_.isFunction(validation)) {
            return validation;
        } else if (_.isString(validation)) {
            return model[validation];
        } else if (!_.isArray(validation)) {
            validation = [validation];
        }

        return _.reduce(validation, function (memo, validation) {
            _.each(_.without(_.keys(validation), 'msg'), function (validator) {
                memo.push({
                    fn: Backbone.Validation.validators[validator],
                    val: validation[validator],
                    msg: validation.msg
                });
            });
            return memo;
        }, []);
    };

    this.validateAttr = function (attr, value) {
        var validators = self.getValidators(attr);

        if (_.isFunction(validators)) {
            return validators.call(model, value, attr);
        }

        return _.reduce(validators, function (memo, validator) {
            var result = validator.fn(value, attr, validator.val, model);
            if (result === false && memo === '') {
                return false;
            }
            if (result && !memo) {
                return validator.msg || result;
            }
            return memo;
        }, '');
    };

    if (model.validate) {
        model._saved_validate = model.validate;
    }
    /**
    * Instead of a simple string we always return an array of error messages if validation fails
    */
    return function (attrs) {
        if (!model.validation) {
            console.log('no validation found in model ', model);
            return false;
        }

        var result = [],
        invalidAttrs = [];

        if (model._saved_validate) {
            var res = model._saved_validate(attrs) || false;
            if (res) result.push(res);
        }

        for (var changedAttr in attrs) {
            var error = self.validateAttr(changedAttr, attrs[changedAttr]);
            if (error) {
                result.push(error);
                invalidAttrs.push(changedAttr);
            }
        }

        if (!_.isEmpty(result)) return result;
    };
};

Backbone.Validation.OldModel = Backbone.Model;
Backbone.Validation.Model = Backbone.Model.extend({
    initialize: function () {
        this.validate = new Backbone.Validation(this);
        Backbone.Validation.OldModel.prototype.initialize.apply(this, arguments);
    }
});
Backbone.Model = Backbone.Validation.Model;

Backbone.Validation.patterns = {
    digits: /^\d+$/,
    number: /^-?(?:\d+|\d{1,3}(?:,\d{3})+)(?:\.\d+)?$/,
    email: /^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))$/i,
    url: /^(https?|ftp):\/\/(((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:)*@)?(((\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5]))|((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?)(:\d*)?)(\/((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:|@)+(\/(([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:|@)*)*)?)?(\?((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:|@)|[\uE000-\uF8FF]|\/|\?)*)?(\#((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:|@)|\/|\?)*)?$/i
};

Backbone.Validation.messages = {
    required: '{0} is required',
    acceptance: '{0} must be accepted',
    min: '{0} must be grater than or equal to {1}',
    max: '{0} must be less than or equal to {1}',
    range: '{0} must be between {1} and {2}',
    length: '{0} must be {1} characters',
    minLength: '{0} must be at least {1} characters',
    maxLength: '{0} must be at most {1} characters',
    rangeLength: '{0} must be between {1} and {2} characters',
    oneOf: '{0} must be one of: {1}',
    equalTo: '{0} must be the same as {1}',
    pattern: '{0} must be a valid {1}'
};

Backbone.Validation.validators = (function (patterns, messages, _) {
    var trim = String.prototype.trim ?
                function (text) {
                    return text === null ? '' : String.prototype.trim.call(text);
                } :
                function (text) {
                    var trimLeft = /^\s+/,
                        trimRight = /\s+$/;

                    return text === null ? '' : text.toString().replace(trimLeft, '').replace(trimRight, '');
                };
    var format = function () {
        var args = Array.prototype.slice.call(arguments);
        var text = args.shift();
        return text.replace(/\{(\d+)\}/g, function (match, number) {
            return typeof args[number] != 'undefined' ? args[number] : match;
        });
    };
    var isNumber = function (value) {
        return _.isNumber(value) || (_.isString(value) && value.match(patterns.number));
    };
    var hasValue = function (value) {
        return !(_.isNull(value) || _.isUndefined(value) || (_.isString(value) && trim(value) === ''));
    };

    return {
        fn: function (value, attr, fn, model) {
            if (_.isString(fn)) {
                fn = model[fn];
            }
            return fn.call(model, value, attr);
        },
        required: function (value, attr, required, model) {
            var isRequired = _.isFunction(required) ? required.call(model) : required;
            if (!isRequired && !hasValue(value)) {
                return false; // overrides all other validators
            }
            if (isRequired && !hasValue(value)) {
                return format(messages.required, attr);
            }
        },
        acceptance: function (value, attr) {
            if (value !== 'true' && (!_.isBoolean(value) || value === false)) {
                return format(messages.acceptance, attr);
            }
        },
        min: function (value, attr, minValue) {
            if (!isNumber(value) || value < minValue) {
                return format(messages.min, attr, minValue);
            }
        },
        max: function (value, attr, maxValue) {
            if (!isNumber(value) || value > maxValue) {
                return format(messages.max, attr, maxValue);
            }
        },
        range: function (value, attr, range) {
            if (!isNumber(value) || value < range[0] || value > range[1]) {
                return format(messages.range, attr, range[0], range[1]);
            }
        },
        length: function (value, attr, length) {
            if (!hasValue(value) || trim(value).length !== length) {
                return format(messages.length, attr, length);
            }
        },
        minLength: function (value, attr, minLength) {
            if (!hasValue(value) || trim(value).length < minLength) {
                return format(messages.minLength, attr, minLength);
            }
        },
        maxLength: function (value, attr, maxLength) {
            if (!hasValue(value) || trim(value).length > maxLength) {
                return format(messages.maxLength, attr, maxLength);
            }
        },
        rangeLength: function (value, attr, range) {
            if (!hasValue(value) || trim(value).length < range[0] || trim(value).length > range[1]) {
                return format(messages.rangeLength, attr, range[0], range[1]);
            }
        },
        oneOf: function (value, attr, values) {
            if (!_.include(values, value)) {
                return format(messages.oneOf, attr, values.join(', '));
            }
        },
        equalTo: function (value, attr, equalTo, model) {
            if (value !== model.get(equalTo)) {
                return format(messages.equalTo, attr, equalTo);
            }
        },
        pattern: function (value, attr, pattern) {
            if (!hasValue(value) || !value.toString().match(patterns[pattern] || pattern)) {
                return format(messages.pattern, attr, pattern);
            }
        }
    };
} (Backbone.Validation.patterns, Backbone.Validation.messages, _));


var ModelTools = {
    /*===JQUI===*/
    AutoCompleteSelect: function (e, data) {



    },
    local: false,
    /*===Backbone===*/
    Sync: function (method, model, options) {
        if (!ModelTools.local)
            return ModelTools.ODataSync(method, model, options);

    },
    ApplyAllAssociations: function () {

        var associations = this.associations;
        for (var modelKey in associations) {
            var thisKey = associations[modelKey].ThisKey;
            var thisModel = associations[modelKey].Model;

            var pk = this.get(thisKey);
            if (pk) {
                var pkArgs = {};
                var idAttribute = (new DomainModel[thisModel]()).idAttribute;

                if (idAttribute) {

                    pkArgs[idAttribute] = pk;
                    if (!this.attributes)
                        this.attributes = {};

                    this.attributes[modelKey] = new DomainModel[thisModel](pkArgs);
                }

            }
        }
    },
    ApplyAssociation: function (model, value) {
        console.log(arguments);
        console.log(this);
        var pkArgs = {};
        var idAttribute = (new DomainModel[this.associated]()).idAttribute;
        if (idAttribute) {
            pkArgs[idAttribute] = value;

            this.model.attributes[this.modelKey] = new DomainModel[this.associated](pkArgs);
        }

    },
    InitializeAssociations: function () {
        ModelTools.ApplyAllAssociations.call(this);
        var associations = this.associations;
        for (var modelKey in associations) {
            var thisKey = associations[modelKey].ThisKey;
            var thisModel = associations[modelKey].Model;

            this.on("change:" + thisKey, ModelTools.ApplyAssociation, { model: this, modelKey: modelKey, associated: thisModel, thisKey: thisKey });
        }
    },

    ODataSync: function (method, model, options) {


        var query = model.urlRoot;
        if ($.isFunction(query))
            query = query(method);

        var requestType = "GET";
        var args;

        if (method != "create")
            query = query + "({0}L)".format(model.id);

        if (method != "read" && method != "delete") {
            args = model.toJSON();
        }

        switch (method) {
            case 'create': requestType = "POST"; break;
            case 'update': requestType = ModelTools.LegacyMergeType(); break;
            case 'delete': requestType = "DELETE"; break;
            case 'read': default: requestType = "GET"; break;
        }

        var jsonp = false;

        var url = query + "";

        if (model.url && typeof (model.url) == 'string')
            url = model.url;

        if (jsonp) {
            args = args || {};
            args.format = "json";
            //args.callback= "displayResults";
        }

        if ((method == "create" || method == "update") && model.associations) {
            for (var i in model.associations) {
                if (args[i])
                    delete args[i];
            }
        }

        console.log(url);
        $.ajax({
            dataType: (jsonp ? "jsonp" : "json"),
            url: url,
            type: requestType,
            beforeSend: ModelTools.LegacyMerge,
            data: args ? JSON.stringify(args) : null,
            contentType: "application/json",
            success: function (result) {
                console.log(model);
                if (result) {
                    if (!result.d) {
                        var props = (
                        function (a) {
                            var props = [];

                            for (i in a)
                                if (a.hasOwnProperty(i))
                                    props.push(i);

                            return props;
                        }
                        )(result);
                        if (props.length == 1) {
                            result.d = result[props[0]];
                        }
                    }

                    if (result.d.length) {
                        for (var i = 0; i < result.d.length; i++) {
                            result.d[i] = ModelTools.FixDate(result.d[i]);

                        }
                        console.log("DEBUG HERE");
                        //model.add(result.d, { silent: true });
                    } else {
                        result.d = ModelTools.FixDate(result.d);

                        //model.set(result.d, { silent: true });
                    }

                }
                if (result && result.d) {
                    for (var i in result.d) {
                        if (i.indexOf("__") == 0)
                            delete result.d[i];
                    }
                    if (method != 'read')
                        model.set(result.d);
                }
                options.success(method != 'read' ? model : result.d);

            },
            error: function (result) {
                options.error('' + result.statusText);
            }

        });

    },
    FixDate: function (obj) {
        for (var i in obj) {
            if (obj[i] && obj[i].search && obj[i].search(/Date\(/) != -1) {
                var dateStr = obj[i].match(/-?\d+/)[0];
                var date = (new Date(Number(dateStr)));

                if (window.Global && window.Global.TimeZoneContext) {

                    //date = date.addHours(window.Global.TimeZoneContextOffset);

                }
                obj[i] = date;
            }
        }
        return obj;
    },

    ODataUrl: function () {
        var query = this.urlRoot;
        if (this.id)
            query = query + "({0}L)".format(this.id);
        return query;
    },
    TestWcf: function () {
        $.ajax({
            dataType: "json",
            url: "/Service/Slot.svc/TestWork",
            type: "POST",
            data: JSON.stringify({ testbool: true }),
            contentType: "application/json",
            success: function (result) {
                console.log("success");
                console.log(result);
            },
            error: function (result) {
                console.log("fail");
                console.log(result);
            }

        });
    },
    TestDelete: function () {
        $.ajax({
            dataType: "json",
            url: "/Service/Slot.svc/TestDelete",
            type: "DELETE",
            data: null,
            contentType: "application/json",
            success: function (result) {
                console.log("success");
                console.log(result);
            },
            error: function (result) {
                console.log("fail");
                console.log(result);
            }

        });
    },
    TestPut: function () {
        $.ajax({
            dataType: "json",
            url: "/Service/Slot.svc/TestPut",
            type: "PUT",
            data: null,
            contentType: "application/json",
            success: function (result) {
                console.log("success");
                console.log(result);
            },
            error: function (result) {
                console.log("fail");
                console.log(result);
            }

        });
    },
    TestMerge: function () {
        $.ajax({
            dataType: "json",
            url: "/Service/Slot.svc/TestMerge",
            type: ModelTools.LegacyMergeType(),
            data: null,
            beforeSend: ModelTools.LegacyMerge,
            contentType: "application/json",
            success: function (result) {
                console.log("success");
                console.log(result);
            },
            error: function (result) {
                console.log("fail");
                console.log(result);
            }

        });
    },
    LegacyMerge: function (xhr, settings) {
        console.log(this);
        console.log(arguments);
        if ($("body").hasClass("flag-ie7") || $("body").hasClass("flag-ie8")) {
            xhr.setRequestHeader("X-HTTP-Method-Override", 'MERGE');
        }
    },
    LegacyMergeType: function () {

        if ($("body").hasClass("flag-ie7") || $("body").hasClass("flag-ie8")) {
            return "POST";
        }
        return "MERGE";
    }
};

Date.getOctalFromTimezoneOffset = function (d) {
    if (!d)
        d = new Date();
    var off = (d.getTimezoneOffset() / 60) + 1;
    if (off == 0)
        return "GMT";
    return "GMT" + (off > 0 ? "-" : "+") + "0" + off + "00";
}

Date.getDayNameFromNumber = function (d) {
    switch (Number(d)) {
        case 1:
            return "Sunday";
        case 2:
            return "Monday";
        case 3:
            return "Tuesday";
        case 4:
            return "Wednesday";
        case 5:
            return "Thursday";
        case 6:
            return "Friday";
        case 7:
            return "Saturday";
        default:
            return "Out of bounds";
    }

}

Date.parseWithTimezone = function (value) {
    return Date.parse(value + " " + Date.getOctalFromTimezoneOffset());
};