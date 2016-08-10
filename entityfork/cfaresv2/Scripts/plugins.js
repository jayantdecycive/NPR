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
* jQuery UI Selectmenu version 1.4.0pre
*
* Copyright (c) 2009-2010 filament group, http://filamentgroup.com
* Copyright (c) 2010-2012 Felix Nagel, http://www.felixnagel.com
* Licensed under the MIT (MIT-LICENSE.txt)
*
* https://github.com/fnagel/jquery-ui/wiki/Selectmenu
*/

(function ($) {

	$.widget("ui.selectmenu", {
		options: {
			appendTo: "body",
			typeAhead: 1000,
			style: 'dropdown',
			positionOptions: null,
			width: null,
			menuWidth: null,
			handleWidth: 26,
			maxHeight: null,
			icons: null,
			format: null,
			escapeHtml: false,
			bgImage: function () { }
		},

		_create: function () {
			var self = this, o = this.options;

			// make / set unique id
			var selectmenuId = this.element.uniqueId().attr("id");

			// quick array of button and menu id's
			this.ids = [selectmenuId, selectmenuId + '-button', selectmenuId + '-menu'];

			// define safe mouseup for future toggling
			this._safemouseup = true;
			this.isOpen = false;

			// create menu button wrapper
			this.newelement = $('<a />', {
				'class': 'ui-selectmenu ui-widget ui-state-default ui-corner-all',
				'id': this.ids[1],
				'role': 'button',
				'href': '#nogo',
				'tabindex': this.element.attr('disabled') ? 1 : 0,
				'aria-haspopup': true,
				'aria-owns': this.ids[2]
			});
			this.newelementWrap = $("<span />")
				.append(this.newelement)
				.insertAfter(this.element);

			// transfer tabindex
			var tabindex = this.element.attr('tabindex');
			if (tabindex) {
				this.newelement.attr('tabindex', tabindex);
			}

			// save reference to select in data for ease in calling methods
			this.newelement.data('selectelement', this.element);

			// menu icon
			this.selectmenuIcon = $('<span class="ui-selectmenu-icon ui-icon"></span>')
				.prependTo(this.newelement);

			// append status span to button
			this.newelement.prepend('<span class="ui-selectmenu-status" />');

			// make associated form label trigger focus
			this.element.bind({
				'click.selectmenu': function (event) {
					self.newelement.focus();
					event.preventDefault();
				}
			});

			// click toggle for menu visibility
			this.newelement
				.bind('mousedown.selectmenu', function (event) {
					self._toggle(event, true);
					// make sure a click won't open/close instantly
					if (o.style == "popup") {
						self._safemouseup = false;
						setTimeout(function () { self._safemouseup = true; }, 300);
					}

					event.preventDefault();
				})
				.bind('click.selectmenu', function (event) {
					event.preventDefault();
				})
				.bind("keydown.selectmenu", function (event) {
					var ret = false;
					switch (event.keyCode) {
						case $.ui.keyCode.ENTER:
							ret = true;
							break;
						case $.ui.keyCode.SPACE:
							self._toggle(event);
							break;
						case $.ui.keyCode.UP:
							if (event.altKey) {
								self.open(event);
							} else {
								self._moveSelection(-1);
							}
							break;
						case $.ui.keyCode.DOWN:
							if (event.altKey) {
								self.open(event);
							} else {
								self._moveSelection(1);
							}
							break;
						case $.ui.keyCode.LEFT:
							self._moveSelection(-1);
							break;
						case $.ui.keyCode.RIGHT:
							self._moveSelection(1);
							break;
						case $.ui.keyCode.TAB:
							ret = true;
							break;
						case $.ui.keyCode.PAGE_UP:
						case $.ui.keyCode.HOME:
							self.index(0);
							break;
						case $.ui.keyCode.PAGE_DOWN:
						case $.ui.keyCode.END:
							self.index(self._optionLis.length);
							break;
						default:
							ret = true;
					}
					return ret;
				})
				.bind('keypress.selectmenu', function (event) {
					if (event.which > 0) {
						self._typeAhead(event.which, 'mouseup');
					}
					return true;
				})
				.bind('mouseover.selectmenu', function () {
					if (!o.disabled) $(this).addClass('ui-state-hover');
				})
				.bind('mouseout.selectmenu', function () {
					if (!o.disabled) $(this).removeClass('ui-state-hover');
				})
				.bind('focus.selectmenu', function () {
					if (!o.disabled) $(this).addClass('ui-state-focus');
				})
				.bind('blur.selectmenu', function () {
					if (!o.disabled) $(this).removeClass('ui-state-focus');
				});

			// document click closes menu
			$(document).bind("mousedown.selectmenu-" + this.ids[0], function (event) {
				//check if open and if the clicket targes parent is the same
				if (self.isOpen && self.ids[1] != event.target.offsetParent.id) {
					self.close(event);
				}
			});

			// change event on original selectmenu
			this.element
				.bind("click.selectmenu", function () {
					self._refreshValue();
				})
				// FIXME: newelement can be null under unclear circumstances in IE8
				// TODO not sure if this is still a problem (fnagel 20.03.11)
				.bind("focus.selectmenu", function () {
					if (self.newelement) {
						self.newelement[0].focus();
					}
				});

			// set width when not set via options
			if (!o.width) {
				o.width = this.element.outerWidth();
			}
			// set menu button width
			this.newelement.width(o.width);

			// hide original selectmenu element
			this.element.hide();

			// create menu portion, append to body
			this.list = $('<ul />', {
				'class': 'ui-widget ui-widget-content',
				'aria-hidden': true,
				'role': 'listbox',
				'aria-labelledby': this.ids[1],
				'id': this.ids[2]
			});
			this.listWrap = $("<div />", {
				'class': 'ui-selectmenu-menu admin-table-2' //rclark: added admin-table-2 to get around issues
			}).append(this.list).appendTo(o.appendTo);

			// transfer menu click to menu button
			this.list
				.bind("keydown.selectmenu", function (event) {
					var ret = false;
					switch (event.keyCode) {
						case $.ui.keyCode.UP:
							if (event.altKey) {
								self.close(event, true);
							} else {
								self._moveFocus(-1);
							}
							break;
						case $.ui.keyCode.DOWN:
							if (event.altKey) {
								self.close(event, true);
							} else {
								self._moveFocus(1);
							}
							break;
						case $.ui.keyCode.LEFT:
							self._moveFocus(-1);
							break;
						case $.ui.keyCode.RIGHT:
							self._moveFocus(1);
							break;
						case $.ui.keyCode.HOME:
							self._moveFocus(':first');
							break;
						case $.ui.keyCode.PAGE_UP:
							self._scrollPage('up');
							break;
						case $.ui.keyCode.PAGE_DOWN:
							self._scrollPage('down');
							break;
						case $.ui.keyCode.END:
							self._moveFocus(':last');
							break;
						case $.ui.keyCode.ENTER:
						case $.ui.keyCode.SPACE:
							self.close(event, true);
							$(event.target).parents('li:eq(0)').trigger('mouseup');
							break;
						case $.ui.keyCode.TAB:
							ret = true;
							self.close(event, true);
							$(event.target).parents('li:eq(0)').trigger('mouseup');
							break;
						case $.ui.keyCode.ESCAPE:
							self.close(event, true);
							break;
						default:
							ret = true;
					}
					return ret;
				})
				.bind('keypress.selectmenu', function (event) {
					if (event.which > 0) {
						self._typeAhead(event.which, 'focus');
					}
					return true;
				})
				// this allows for using the scrollbar in an overflowed list
				.bind('mousedown.selectmenu mouseup.selectmenu', function () { return false; });

			// needed when window is resized
			$(window).bind("resize.selectmenu-" + this.ids[0], $.proxy(self.close, this));
		},

		_init: function () {
			var self = this, o = this.options;

			// serialize selectmenu element options
			var selectOptionData = [];
			this.element.find('option').each(function () {
				var opt = $(this);
				selectOptionData.push({
					value: opt.attr('value'),
					text: self._formatText(opt.text(), opt),
					selected: opt.attr('selected'),
					disabled: opt.attr('disabled'),
					classes: opt.attr('class'),
					typeahead: opt.attr('typeahead'),
					parentOptGroup: opt.parent('optgroup'),
					bgImage: o.bgImage.call(opt)
				});
			});

			// active state class is only used in popup style
			var activeClass = (self.options.style == "popup") ? " ui-state-active" : "";

			// empty list so we can refresh the selectmenu via selectmenu()
			this.list.html("");

			// write li's
			if (selectOptionData.length) {
				for (var i = 0; i < selectOptionData.length; i++) {
					var thisLiAttr = { role: 'presentation' };
					if (selectOptionData[i].disabled) {
						thisLiAttr['class'] = 'ui-state-disabled';
					}
					var thisAAttr = {
						html: selectOptionData[i].text || '&nbsp;',
						href: '#nogo',
						tabindex: -1,
						role: 'option',
						'aria-selected': false
					};
					if (selectOptionData[i].disabled) {
						thisAAttr['aria-disabled'] = selectOptionData[i].disabled;
					}
					if (selectOptionData[i].typeahead) {
						thisAAttr['typeahead'] = selectOptionData[i].typeahead;
					}
					var thisA = $('<a/>', thisAAttr)
						.bind('focus.selectmenu', function () {
							$(this).parent().mouseover();
						})
						.bind('blur.selectmenu', function () {
							$(this).parent().mouseout();
						});
					var thisLi = $('<li/>', thisLiAttr)
						.append(thisA)
						.data('index', i)
						.addClass(selectOptionData[i].classes)
						.data('optionClasses', selectOptionData[i].classes || '')
						.bind("mouseup.selectmenu", function (event) {
							if (self._safemouseup && !self._disabled(event.currentTarget) && !self._disabled($(event.currentTarget).parents("ul > li.ui-selectmenu-group "))) {
								self.index($(this).data('index'));
								self.select(event);
								self.close(event, true);
							}
							return false;
						})
						.bind("click.selectmenu", function () {
							return false;
						})
						.bind('mouseover.selectmenu', function (e) {
							// no hover if diabled
							if (!$(this).hasClass('ui-state-disabled') && !$(this).parent("ul").parent("li").hasClass('ui-state-disabled')) {
								e.optionValue = self.element[0].options[$(this).data('index')].value;
								self._trigger("hover", e, self._uiHash());
								self._selectedOptionLi().addClass(activeClass);
								self._focusedOptionLi().removeClass('ui-selectmenu-item-focus ui-state-hover');
								$(this).removeClass('ui-state-active').addClass('ui-selectmenu-item-focus ui-state-hover');
							}
						})
						.bind('mouseout.selectmenu', function (e) {
							if ($(this).is(self._selectedOptionLi())) {
								$(this).addClass(activeClass);
							}
							e.optionValue = self.element[0].options[$(this).data('index')].value;
							self._trigger("blur", e, self._uiHash());
							$(this).removeClass('ui-selectmenu-item-focus ui-state-hover');
						});

					// optgroup or not...
					if (selectOptionData[i].parentOptGroup.length) {
						var optGroupName = 'ui-selectmenu-group-' + this.element.find('optgroup').index(selectOptionData[i].parentOptGroup);
						if (this.list.find('li.' + optGroupName).length) {
							this.list.find('li.' + optGroupName + ':last ul').append(thisLi);
						} else {
							$('<li role="presentation" class="ui-selectmenu-group ' + optGroupName + (selectOptionData[i].parentOptGroup.attr("disabled") ? ' ' + 'ui-state-disabled" aria-disabled="true"' : '"') + '><span class="ui-selectmenu-group-label">' + selectOptionData[i].parentOptGroup.attr('label') + '</span><ul></ul></li>')
								.appendTo(this.list)
								.find('ul')
								.append(thisLi);
						}
					} else {
						thisLi.appendTo(this.list);
					}

					// append icon if option is specified
					if (o.icons) {
						for (var j in o.icons) {
							if (thisLi.is(o.icons[j].find)) {
								thisLi
									.data('optionClasses', selectOptionData[i].classes + ' ui-selectmenu-hasIcon')
									.addClass('ui-selectmenu-hasIcon');
								var iconClass = o.icons[j].icon || "";
								thisLi
									.find('a:eq(0)')
									.prepend('<span class="ui-selectmenu-item-icon ui-icon ' + iconClass + '"></span>');
								if (selectOptionData[i].bgImage) {
									thisLi.find('span').css('background-image', selectOptionData[i].bgImage);
								}
							}
						}
					}
				}
			} else {
				$(' <li role="presentation"><a href="#nogo" tabindex="-1" role="option"></a></li>').appendTo(this.list);
			}
			// we need to set and unset the CSS classes for dropdown and popup style
			var isDropDown = (o.style == 'dropdown');
			this.newelement
				.toggleClass('ui-selectmenu-dropdown', isDropDown)
				.toggleClass('ui-selectmenu-popup', !isDropDown);
			this.list
				.toggleClass('ui-selectmenu-menu-dropdown ui-corner-bottom', isDropDown)
				.toggleClass('ui-selectmenu-menu-popup ui-corner-all', !isDropDown)
				// add corners to top and bottom menu items
				.find('li:first')
				.toggleClass('ui-corner-top', !isDropDown)
				.end().find('li:last')
				.addClass('ui-corner-bottom');
			this.selectmenuIcon
				.toggleClass('ui-icon-triangle-1-s', isDropDown)
				.toggleClass('ui-icon-triangle-2-n-s', !isDropDown);

			// set menu width to either menuWidth option value, width option value, or select width
			if (o.style == 'dropdown') {
				this.list.width(o.menuWidth ? o.menuWidth : o.width);
			} else {
				this.list.width(o.menuWidth ? o.menuWidth : o.width - o.handleWidth);
			}

			// reset height to auto
			this.list.css('height', 'auto');
			var listH = this.listWrap.height();
			var winH = $(window).height();
			// calculate default max height
			/*var maxH = o.maxHeight ? Math.min(o.maxHeight, winH) : winH / 3;
			if (listH > maxH) this.list.height(maxH);*/

			// save reference to actionable li's (not group label li's)
			this._optionLis = this.list.find('li:not(.ui-selectmenu-group)');

			// transfer disabled state
			if (this.element.attr('disabled')) {
				this.disable();
			} else {
				this.enable();
			}

			// update value
			this._refreshValue();

			// set selected item so movefocus has intial state
			this._selectedOptionLi().addClass('ui-selectmenu-item-focus');

			// needed when selectmenu is placed at the very bottom / top of the page
			clearTimeout(this.refreshTimeout);
			this.refreshTimeout = window.setTimeout(function () {
				self._refreshPosition();
			}, 200);
		},

		destroy: function () {
			this.element.removeData(this.widgetName)
				.removeClass('ui-selectmenu-disabled' + ' ' + 'ui-state-disabled')
				.removeAttr('aria-disabled')
				.unbind(".selectmenu");

			$(window).unbind(".selectmenu-" + this.ids[0]);
			$(document).unbind(".selectmenu-" + this.ids[0]);

			this.newelementWrap.remove();
			this.listWrap.remove();

			// unbind click event and show original select
			this.element
				.unbind(".selectmenu")
				.show();

			// call widget destroy function
			$.Widget.prototype.destroy.apply(this, arguments);
		},

		_typeAhead: function (code, eventType) {
			var self = this,
				c = String.fromCharCode(code).toLowerCase(),
				matchee = null,
				nextIndex = null;

			// Clear any previous timer if present
			if (self._typeAhead_timer) {
				window.clearTimeout(self._typeAhead_timer);
				self._typeAhead_timer = undefined;
			}
			// Store the character typed
			self._typeAhead_chars = (self._typeAhead_chars === undefined ? "" : self._typeAhead_chars).concat(c);
			// Detect if we are in cyciling mode or direct selection mode
			if (self._typeAhead_chars.length < 2 || (self._typeAhead_chars.substr(-2, 1) === c && self._typeAhead_cycling)) {
				self._typeAhead_cycling = true;
				// Match only the first character and loop
				matchee = c;
			} else {
				// We won't be cycling anymore until the timer expires
				self._typeAhead_cycling = false;
				// Match all the characters typed
				matchee = self._typeAhead_chars;
			}

			// We need to determine the currently active index, but it depends on
			// the used context: if it's in the element, we want the actual
			// selected index, if it's in the menu, just the focused one
			var selectedIndex = (eventType !== 'focus' ? this._selectedOptionLi().data('index') : this._focusedOptionLi().data('index')) || 0;
			for (var i = 0; i < this._optionLis.length; i++) {
				var thisText = this._optionLis.eq(i).text().substr(0, matchee.length).toLowerCase();
				if (thisText === matchee) {
					if (self._typeAhead_cycling) {
						if (nextIndex === null)
							nextIndex = i;
						if (i > selectedIndex) {
							nextIndex = i;
							break;
						}
					} else {
						nextIndex = i;
					}
				}
			}

			if (nextIndex !== null) {
				// Why using trigger() instead of a direct method to select the index? Because we don't what is the exact action to do,
				// it depends if the user is typing on the element or on the popped up menu
				this._optionLis.eq(nextIndex).find("a").trigger(eventType);
			}

			self._typeAhead_timer = window.setTimeout(function () {
				self._typeAhead_timer = undefined;
				self._typeAhead_chars = undefined;
				self._typeAhead_cycling = undefined;
			}, self.options.typeAhead);
		},

		// returns some usefull information, called by callbacks only
		_uiHash: function () {
			var index = this.index();
			return {
				index: index,
				option: $("option", this.element).get(index),
				value: this.element[0].value
			};
		},

		open: function (event) {
			if (this.newelement.attr("aria-disabled") != 'true') {
				var self = this,
					o = this.options,
					selected = this._selectedOptionLi(),
					link = selected.find("a");

				self._closeOthers(event);
				self.newelement.addClass('ui-state-active');
				self.list.attr('aria-hidden', false);
				self.listWrap.addClass('ui-selectmenu-open');

				if (o.style == "dropdown") {
					//self.newelement.removeClass('ui-corner-all').addClass('ui-corner-top');
				} else {
					// center overflow and avoid flickering
					this.list
						.css("left", -5000)
						.scrollTop(this.list.scrollTop() + selected.position().top - this.list.outerHeight() / 2 + selected.outerHeight() / 2)
						.css("left", "auto");
				}

				self._refreshPosition();

				if (link.length) {
					link[0].focus();
				}

				self.isOpen = true;
				self._trigger("open", event, self._uiHash());
			}
		},

		close: function (event, retainFocus) {
			if (this.newelement.is('.ui-state-active')) {
				this.newelement.removeClass('ui-state-active');
				this.listWrap.removeClass('ui-selectmenu-open');
				this.list.attr('aria-hidden', true);
				if (this.options.style == "dropdown") {
					this.newelement.removeClass('ui-corner-top').addClass('ui-corner-all');
				}
				if (retainFocus) {
					this.newelement.focus();
				}
				this.isOpen = false;
				this._trigger("close", event, this._uiHash());
			}
		},

		change: function (event) {
			this.element.trigger("change");
			this._trigger("change", event, this._uiHash());
		},

		select: function (event) {
			if (this._disabled(event.currentTarget)) { return false; }
			this._trigger("select", event, this._uiHash());
		},

		widget: function () {
			return this.listWrap.add(this.newelementWrap);
		},

		_closeOthers: function (event) {
			$('.ui-selectmenu.ui-state-active').not(this.newelement).each(function () {
				$(this).data('selectelement').selectmenu('close', event);
			});
			$('.ui-selectmenu.ui-state-hover').trigger('mouseout');
		},

		_toggle: function (event, retainFocus) {
			if (this.isOpen) {
				this.close(event, retainFocus);
			} else {
				this.open(event);
			}
		},

		_formatText: function (text, opt) {
			if (this.options.format) {
				text = this.options.format(text, opt);
			} else if (this.options.escapeHtml) {
				text = $('<div />').text(text).html();
			}
			return text;
		},

		_selectedIndex: function () {
			return this.element[0].selectedIndex;
		},

		_selectedOptionLi: function () {
			return this._optionLis.eq(this._selectedIndex());
		},

		_focusedOptionLi: function () {
			return this.list.find('.ui-selectmenu-item-focus');
		},

		_moveSelection: function (amt, recIndex) {
			// do nothing if disabled
			if (!this.options.disabled) {
				var currIndex = parseInt(this._selectedOptionLi().data('index') || 0, 10);
				var newIndex = currIndex + amt;
				// do not loop when using up key
				if (newIndex < 0) {
					newIndex = 0;
				}
				if (newIndex > this._optionLis.size() - 1) {
					newIndex = this._optionLis.size() - 1;
				}
				// Occurs when a full loop has been made
				if (newIndex === recIndex) {
					return false;
				}

				if (this._optionLis.eq(newIndex).hasClass('ui-state-disabled')) {
					// if option at newIndex is disabled, call _moveFocus, incrementing amt by one
					(amt > 0) ? ++amt : --amt;
					this._moveSelection(amt, newIndex);
				} else {
					this._optionLis.eq(newIndex).trigger('mouseover').trigger('mouseup');
				}
			}
		},

		_moveFocus: function (amt, recIndex) {
			if (!isNaN(amt)) {
				var currIndex = parseInt(this._focusedOptionLi().data('index') || 0, 10);
				var newIndex = currIndex + amt;
			} else {
				var newIndex = parseInt(this._optionLis.filter(amt).data('index'), 10);
			}

			if (newIndex < 0) {
				newIndex = 0;
			}
			if (newIndex > this._optionLis.size() - 1) {
				newIndex = this._optionLis.size() - 1;
			}

			//Occurs when a full loop has been made
			if (newIndex === recIndex) {
				return false;
			}

			var activeID = 'ui-selectmenu-item-' + Math.round(Math.random() * 1000);

			this._focusedOptionLi().find('a:eq(0)').attr('id', '');

			if (this._optionLis.eq(newIndex).hasClass('ui-state-disabled')) {
				// if option at newIndex is disabled, call _moveFocus, incrementing amt by one
				(amt > 0) ? ++amt : --amt;
				this._moveFocus(amt, newIndex);
			} else {
				this._optionLis.eq(newIndex).find('a:eq(0)').attr('id', activeID).focus();
			}

			this.list.attr('aria-activedescendant', activeID);
		},

		_scrollPage: function (direction) {
			var numPerPage = Math.floor(this.list.outerHeight() / this._optionLis.first().outerHeight());
			numPerPage = (direction == 'up' ? -numPerPage : numPerPage);
			this._moveFocus(numPerPage);
		},

		_setOption: function (key, value) {
			this.options[key] = value;
			// set
			if (key == 'disabled') {
				if (value) this.close();
				this.element
					.add(this.newelement)
					.add(this.list)[value ? 'addClass' : 'removeClass']('ui-selectmenu-disabled ' + 'ui-state-disabled')
					.attr("aria-disabled", value);
			}
		},

		disable: function (index, type) {
			// if options is not provided, call the parents disable function
			if (typeof (index) == 'undefined') {
				this._setOption('disabled', true);
			} else {
				if (type == "optgroup") {
					this._toggleOptgroup(index, false);
				} else {
					this._toggleOption(index, false);
				}
			}
		},

		enable: function (index, type) {
			// if options is not provided, call the parents enable function
			if (typeof (index) == 'undefined') {
				this._setOption('disabled', false);
			} else {
				if (type == "optgroup") {
					this._toggleOptgroup(index, true);
				} else {
					this._toggleOption(index, true);
				}
			}
		},

		_disabled: function (elem) {
			return $(elem).hasClass('ui-state-disabled');
		},

		_toggleOption: function (index, flag) {
			var optionElem = this._optionLis.eq(index);
			if (optionElem) {
				optionElem
					.toggleClass('ui-state-disabled', flag)
					.find("a").attr("aria-disabled", !flag);
				if (flag) {
					this.element.find("option").eq(index).attr("disabled", "disabled");
				} else {
					this.element.find("option").eq(index).removeAttr("disabled");
				}
			}
		},

		// true = enabled, false = disabled
		_toggleOptgroup: function (index, flag) {
			var optGroupElem = this.list.find('li.ui-selectmenu-group-' + index);
			if (optGroupElem) {
				optGroupElem
					.toggleClass('ui-state-disabled', flag)
					.attr("aria-disabled", !flag);
				if (flag) {
					this.element.find("optgroup").eq(index).attr("disabled", "disabled");
				} else {
					this.element.find("optgroup").eq(index).removeAttr("disabled");
				}
			}
		},

		index: function (newIndex) {
			if (arguments.length) {
				if (!this._disabled($(this._optionLis[newIndex])) && newIndex != this._selectedIndex()) {
					this.element[0].selectedIndex = newIndex;
					this._refreshValue();
					this.change();
				} else {
					return false;
				}
			} else {
				return this._selectedIndex();
			}
		},

		value: function (newValue) {
			if (arguments.length && newValue != this.element[0].value) {
				this.element[0].value = newValue;
				this._refreshValue();
				this.change();
			} else {
				return this.element[0].value;
			}
		},

		_refreshValue: function () {
			var activeClass = (this.options.style == "popup") ? " ui-state-active" : "";
			var activeID = 'ui-selectmenu-item-' + Math.round(Math.random() * 1000);
			// deselect previous
			this.list
				.find('.ui-selectmenu-item-selected')
				.removeClass("ui-selectmenu-item-selected" + activeClass)
				.find('a')
				.attr('aria-selected', 'false')
				.attr('id', '');
			// select new
			this._selectedOptionLi()
				.addClass("ui-selectmenu-item-selected" + activeClass)
				.find('a')
				.attr('aria-selected', 'true')
				.attr('id', activeID);

			// toggle any class brought in from option
			var currentOptionClasses = (this.newelement.data('optionClasses') ? this.newelement.data('optionClasses') : "");
			var newOptionClasses = (this._selectedOptionLi().data('optionClasses') ? this._selectedOptionLi().data('optionClasses') : "");
			this.newelement
				.removeClass(currentOptionClasses)
				.data('optionClasses', newOptionClasses)
				.addClass(newOptionClasses)
				.find('.ui-selectmenu-status')
				.html(this._selectedOptionLi().find('a:eq(0)').html());

			this.list.attr('aria-activedescendant', activeID);
		},

		_refreshPosition: function () {
			var o = this.options,
				positionDefault = {
					of: this.newelement,
					my: "left top",
					at: "left bottom",
					collision: 'flip'
				};

			// if its a pop-up we need to calculate the position of the selected li
			if (o.style == "popup") {
				var selected = this._selectedOptionLi();
				positionDefault.my = "left top" + (this.list.offset().top - selected.offset().top - (this.newelement.outerHeight() + selected.outerHeight()) / 2);
				positionDefault.collision = "fit";
			}

			this.listWrap
				.removeAttr('style')
				.zIndex(this.element.zIndex() + 1)
				.position($.extend(positionDefault, o.positionOptions));
		}
	});

})(jQuery);


/*
 * jQuery timepicker addon
 * By: Trent Richardson [http://trentrichardson.com]
 * Version 1.3
 * Last Modified: 05/05/2013
 *
 * Copyright 2013 Trent Richardson
 * You may use this project under MIT or GPL licenses.
 * http://trentrichardson.com/Impromptu/GPL-LICENSE.txt
 * http://trentrichardson.com/Impromptu/MIT-LICENSE.txt
 */

/*jslint evil: true, white: false, undef: false, nomen: false */

(function ($) {

    /*
	* Lets not redefine timepicker, Prevent "Uncaught RangeError: Maximum call stack size exceeded"
	*/
    $.ui.timepicker = $.ui.timepicker || {};
    if ($.ui.timepicker.version) {
        return;
    }

    /*
	* Extend jQueryUI, get it started with our version number
	*/
    $.extend($.ui, {
        timepicker: {
            version: "1.3"
        }
    });

    /* 
	* Timepicker manager.
	* Use the singleton instance of this class, $.timepicker, to interact with the time picker.
	* Settings for (groups of) time pickers are maintained in an instance object,
	* allowing multiple different settings on the same page.
	*/
    var Timepicker = function () {
        this.regional = []; // Available regional settings, indexed by language code
        this.regional[''] = { // Default regional settings
            currentText: 'Now',
            closeText: 'Done',
            amNames: ['AM', 'A'],
            pmNames: ['PM', 'P'],
            timeFormat: 'HH:mm',
            timeSuffix: '',
            timeOnlyTitle: 'Choose Time',
            timeText: 'Time',
            hourText: 'Hour',
            minuteText: 'Minute',
            secondText: 'Second',
            millisecText: 'Millisecond',
            microsecText: 'Microsecond',
            timezoneText: 'Time Zone',
            isRTL: false
        };
        this._defaults = { // Global defaults for all the datetime picker instances
            showButtonPanel: true,
            timeOnly: false,
            showHour: null,
            showMinute: null,
            showSecond: null,
            showMillisec: null,
            showMicrosec: null,
            showTimezone: null,
            showTime: true,
            stepHour: 1,
            stepMinute: 1,
            stepSecond: 1,
            stepMillisec: 1,
            stepMicrosec: 1,
            hour: 0,
            minute: 0,
            second: 0,
            millisec: 0,
            microsec: 0,
            timezone: null,
            hourMin: 0,
            minuteMin: 0,
            secondMin: 0,
            millisecMin: 0,
            microsecMin: 0,
            hourMax: 23,
            minuteMax: 59,
            secondMax: 59,
            millisecMax: 999,
            microsecMax: 999,
            minDateTime: null,
            maxDateTime: null,
            onSelect: null,
            hourGrid: 0,
            minuteGrid: 0,
            secondGrid: 0,
            millisecGrid: 0,
            microsecGrid: 0,
            alwaysSetTime: true,
            separator: ' ',
            altFieldTimeOnly: true,
            altTimeFormat: null,
            altSeparator: null,
            altTimeSuffix: null,
            pickerTimeFormat: null,
            pickerTimeSuffix: null,
            showTimepicker: true,
            timezoneList: null,
            addSliderAccess: false,
            sliderAccessArgs: null,
            controlType: 'slider',
            defaultValue: null,
            parse: 'strict'
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
        microsec_slider: null,
        timezone_select: null,
        hour: 0,
        minute: 0,
        second: 0,
        millisec: 0,
        microsec: 0,
        timezone: null,
        hourMinOriginal: null,
        minuteMinOriginal: null,
        secondMinOriginal: null,
        millisecMinOriginal: null,
        microsecMinOriginal: null,
        hourMaxOriginal: null,
        minuteMaxOriginal: null,
        secondMaxOriginal: null,
        millisecMaxOriginal: null,
        microsecMaxOriginal: null,
        ampm: '',
        formattedDate: '',
        formattedTime: '',
        formattedDateTime: '',
        timezoneList: null,
        units: ['hour', 'minute', 'second', 'millisec', 'microsec'],
        support: {},
        control: null,

        /* 
		* Override the default settings for all instances of the time picker.
		* @param  settings  object - the new settings to use as defaults (anonymous object)
		* @return the manager object
		*/
        setDefaults: function (settings) {
            extendRemove(this._defaults, settings || {});
            return this;
        },

        /*
		* Create a new Timepicker instance
		*/
        _newInst: function ($input, o) {
            var tp_inst = new Timepicker(),
				inlineSettings = {},
            	fns = {},
		    	overrides, i;

            for (var attrName in this._defaults) {
                if (this._defaults.hasOwnProperty(attrName)) {
                    var attrValue = $input.attr('time:' + attrName);
                    if (attrValue) {
                        try {
                            inlineSettings[attrName] = eval(attrValue);
                        } catch (err) {
                            inlineSettings[attrName] = attrValue;
                        }
                    }
                }
            }

            overrides = {
                beforeShow: function (input, dp_inst) {
                    if ($.isFunction(tp_inst._defaults.evnts.beforeShow)) {
                        return tp_inst._defaults.evnts.beforeShow.call($input[0], input, dp_inst, tp_inst);
                    }
                },
                onChangeMonthYear: function (year, month, dp_inst) {
                    // Update the time as well : this prevents the time from disappearing from the $input field.
                    tp_inst._updateDateTime(dp_inst);
                    if ($.isFunction(tp_inst._defaults.evnts.onChangeMonthYear)) {
                        tp_inst._defaults.evnts.onChangeMonthYear.call($input[0], year, month, dp_inst, tp_inst);
                    }
                },
                onClose: function (dateText, dp_inst) {
                    if (tp_inst.timeDefined === true && $input.val() !== '') {
                        tp_inst._updateDateTime(dp_inst);
                    }
                    if ($.isFunction(tp_inst._defaults.evnts.onClose)) {
                        tp_inst._defaults.evnts.onClose.call($input[0], dateText, dp_inst, tp_inst);
                    }
                }
            };
            for (i in overrides) {
                if (overrides.hasOwnProperty(i)) {
                    fns[i] = o[i] || null;
                }
            }

            tp_inst._defaults = $.extend({}, this._defaults, inlineSettings, o, overrides, {
                evnts: fns,
                timepicker: tp_inst // add timepicker as a property of datepicker: $.datepicker._get(dp_inst, 'timepicker');
            });
            tp_inst.amNames = $.map(tp_inst._defaults.amNames, function (val) {
                return val.toUpperCase();
            });
            tp_inst.pmNames = $.map(tp_inst._defaults.pmNames, function (val) {
                return val.toUpperCase();
            });

            // detect which units are supported
            tp_inst.support = detectSupport(
					tp_inst._defaults.timeFormat +
					(tp_inst._defaults.pickerTimeFormat ? tp_inst._defaults.pickerTimeFormat : '') +
					(tp_inst._defaults.altTimeFormat ? tp_inst._defaults.altTimeFormat : ''));

            // controlType is string - key to our this._controls
            if (typeof (tp_inst._defaults.controlType) === 'string') {
                if (tp_inst._defaults.controlType == 'slider' && typeof (jQuery.ui.slider) === 'undefined') {
                    tp_inst._defaults.controlType = 'select';
                }
                tp_inst.control = tp_inst._controls[tp_inst._defaults.controlType];
            }
                // controlType is an object and must implement create, options, value methods
            else {
                tp_inst.control = tp_inst._defaults.controlType;
            }

            // prep the timezone options
            var timezoneList = [-720, -660, -600, -570, -540, -480, -420, -360, -300, -270, -240, -210, -180, -120, -60,
					0, 60, 120, 180, 210, 240, 270, 300, 330, 345, 360, 390, 420, 480, 525, 540, 570, 600, 630, 660, 690, 720, 765, 780, 840];
            if (tp_inst._defaults.timezoneList !== null) {
                timezoneList = tp_inst._defaults.timezoneList;
            }
            var tzl = timezoneList.length, tzi = 0, tzv = null;
            if (tzl > 0 && typeof timezoneList[0] !== 'object') {
                for (; tzi < tzl; tzi++) {
                    tzv = timezoneList[tzi];
                    timezoneList[tzi] = { value: tzv, label: $.timepicker.timezoneOffsetString(tzv, tp_inst.support.iso8601) };
                }
            }
            tp_inst._defaults.timezoneList = timezoneList;

            // set the default units
            tp_inst.timezone = tp_inst._defaults.timezone !== null ? $.timepicker.timezoneOffsetNumber(tp_inst._defaults.timezone) :
							((new Date()).getTimezoneOffset() * -1);
            tp_inst.hour = tp_inst._defaults.hour < tp_inst._defaults.hourMin ? tp_inst._defaults.hourMin :
							tp_inst._defaults.hour > tp_inst._defaults.hourMax ? tp_inst._defaults.hourMax : tp_inst._defaults.hour;
            tp_inst.minute = tp_inst._defaults.minute < tp_inst._defaults.minuteMin ? tp_inst._defaults.minuteMin :
							tp_inst._defaults.minute > tp_inst._defaults.minuteMax ? tp_inst._defaults.minuteMax : tp_inst._defaults.minute;
            tp_inst.second = tp_inst._defaults.second < tp_inst._defaults.secondMin ? tp_inst._defaults.secondMin :
							tp_inst._defaults.second > tp_inst._defaults.secondMax ? tp_inst._defaults.secondMax : tp_inst._defaults.second;
            tp_inst.millisec = tp_inst._defaults.millisec < tp_inst._defaults.millisecMin ? tp_inst._defaults.millisecMin :
							tp_inst._defaults.millisec > tp_inst._defaults.millisecMax ? tp_inst._defaults.millisecMax : tp_inst._defaults.millisec;
            tp_inst.microsec = tp_inst._defaults.microsec < tp_inst._defaults.microsecMin ? tp_inst._defaults.microsecMin :
							tp_inst._defaults.microsec > tp_inst._defaults.microsecMax ? tp_inst._defaults.microsecMax : tp_inst._defaults.microsec;
            tp_inst.ampm = '';
            tp_inst.$input = $input;

            if (o.altField) {
                tp_inst.$altInput = $(o.altField).css({
                    cursor: 'pointer'
                }).focus(function () {
                    $input.trigger("focus");
                });
            }

            if (tp_inst._defaults.minDate === 0 || tp_inst._defaults.minDateTime === 0) {
                tp_inst._defaults.minDate = new Date();
            }
            if (tp_inst._defaults.maxDate === 0 || tp_inst._defaults.maxDateTime === 0) {
                tp_inst._defaults.maxDate = new Date();
            }

            // datepicker needs minDate/maxDate, timepicker needs minDateTime/maxDateTime..
            if (tp_inst._defaults.minDate !== undefined && tp_inst._defaults.minDate instanceof Date) {
                tp_inst._defaults.minDateTime = new Date(tp_inst._defaults.minDate.getTime());
            }
            if (tp_inst._defaults.minDateTime !== undefined && tp_inst._defaults.minDateTime instanceof Date) {
                tp_inst._defaults.minDate = new Date(tp_inst._defaults.minDateTime.getTime());
            }
            if (tp_inst._defaults.maxDate !== undefined && tp_inst._defaults.maxDate instanceof Date) {
                tp_inst._defaults.maxDateTime = new Date(tp_inst._defaults.maxDate.getTime());
            }
            if (tp_inst._defaults.maxDateTime !== undefined && tp_inst._defaults.maxDateTime instanceof Date) {
                tp_inst._defaults.maxDate = new Date(tp_inst._defaults.maxDateTime.getTime());
            }
            tp_inst.$input.bind('focus', function () {
                tp_inst._onFocus();
            });

            return tp_inst;
        },

        /*
		* add our sliders to the calendar
		*/
        _addTimePicker: function (dp_inst) {
            var currDT = (this.$altInput && this._defaults.altFieldTimeOnly) ? this.$input.val() + ' ' + this.$altInput.val() : this.$input.val();

            this.timeDefined = this._parseTime(currDT);
            this._limitMinMaxDateTime(dp_inst, false);
            this._injectTimePicker();
        },

        /*
		* parse the time string from input value or _setTime
		*/
        _parseTime: function (timeString, withDate) {
            if (!this.inst) {
                this.inst = $.datepicker._getInst(this.$input[0]);
            }

            if (withDate || !this._defaults.timeOnly) {
                var dp_dateFormat = $.datepicker._get(this.inst, 'dateFormat');
                try {
                    var parseRes = parseDateTimeInternal(dp_dateFormat, this._defaults.timeFormat, timeString, $.datepicker._getFormatConfig(this.inst), this._defaults);
                    if (!parseRes.timeObj) {
                        return false;
                    }
                    $.extend(this, parseRes.timeObj);
                } catch (err) {
                    $.timepicker.log("Error parsing the date/time string: " + err +
									"\ndate/time string = " + timeString +
									"\ntimeFormat = " + this._defaults.timeFormat +
									"\ndateFormat = " + dp_dateFormat);
                    return false;
                }
                return true;
            } else {
                var timeObj = $.datepicker.parseTime(this._defaults.timeFormat, timeString, this._defaults);
                if (!timeObj) {
                    return false;
                }
                $.extend(this, timeObj);
                return true;
            }
        },

        /*
		* generate and inject html for timepicker into ui datepicker
		*/
        _injectTimePicker: function () {
            var $dp = this.inst.dpDiv,
				o = this.inst.settings,
				tp_inst = this,
				litem = '',
				uitem = '',
				show = null,
				max = {},
				gridSize = {},
				size = null,
				i = 0,
				l = 0;

            // Prevent displaying twice
            if ($dp.find("div.ui-timepicker-div").length === 0 && o.showTimepicker) {
                var noDisplay = ' style="display:none;"',
					html = '<div class="ui-timepicker-div' + (o.isRTL ? ' ui-timepicker-rtl' : '') + '"><dl>' + '<dt class="ui_tpicker_time_label"' + ((o.showTime) ? '' : noDisplay) + '>' + o.timeText + '</dt>' +
								'<dd class="ui_tpicker_time"' + ((o.showTime) ? '' : noDisplay) + '></dd>';

                // Create the markup
                for (i = 0, l = this.units.length; i < l; i++) {
                    litem = this.units[i];
                    uitem = litem.substr(0, 1).toUpperCase() + litem.substr(1);
                    show = o['show' + uitem] !== null ? o['show' + uitem] : this.support[litem];

                    // Added by Peter Medeiros:
                    // - Figure out what the hour/minute/second max should be based on the step values.
                    // - Example: if stepMinute is 15, then minMax is 45.
                    max[litem] = parseInt((o[litem + 'Max'] - ((o[litem + 'Max'] - o[litem + 'Min']) % o['step' + uitem])), 10);
                    gridSize[litem] = 0;

                    html += '<dt class="ui_tpicker_' + litem + '_label"' + (show ? '' : noDisplay) + '>' + o[litem + 'Text'] + '</dt>' +
								'<dd class="ui_tpicker_' + litem + '"><div class="ui_tpicker_' + litem + '_slider"' + (show ? '' : noDisplay) + '></div>';

                    if (show && o[litem + 'Grid'] > 0) {
                        html += '<div style="padding-left: 1px"><table class="ui-tpicker-grid-label"><tr>';

                        if (litem == 'hour') {
                            for (var h = o[litem + 'Min']; h <= max[litem]; h += parseInt(o[litem + 'Grid'], 10)) {
                                gridSize[litem]++;
                                var tmph = $.datepicker.formatTime(this.support.ampm ? 'hht' : 'HH', { hour: h }, o);
                                html += '<td data-for="' + litem + '">' + tmph + '</td>';
                            }
                        }
                        else {
                            for (var m = o[litem + 'Min']; m <= max[litem]; m += parseInt(o[litem + 'Grid'], 10)) {
                                gridSize[litem]++;
                                html += '<td data-for="' + litem + '">' + ((m < 10) ? '0' : '') + m + '</td>';
                            }
                        }

                        html += '</tr></table></div>';
                    }
                    html += '</dd>';
                }

                // Timezone
                var showTz = o.showTimezone !== null ? o.showTimezone : this.support.timezone;
                html += '<dt class="ui_tpicker_timezone_label"' + (showTz ? '' : noDisplay) + '>' + o.timezoneText + '</dt>';
                html += '<dd class="ui_tpicker_timezone" ' + (showTz ? '' : noDisplay) + '></dd>';

                // Create the elements from string
                html += '</dl></div>';
                var $tp = $(html);

                // if we only want time picker...
                if (o.timeOnly === true) {
                    $tp.prepend('<div class="ui-widget-header ui-helper-clearfix ui-corner-all">' + '<div class="ui-datepicker-title">' + o.timeOnlyTitle + '</div>' + '</div>');
                    $dp.find('.ui-datepicker-header, .ui-datepicker-calendar').hide();
                }

                // add sliders, adjust grids, add events
                for (i = 0, l = tp_inst.units.length; i < l; i++) {
                    litem = tp_inst.units[i];
                    uitem = litem.substr(0, 1).toUpperCase() + litem.substr(1);
                    show = o['show' + uitem] !== null ? o['show' + uitem] : this.support[litem];

                    // add the slider
                    tp_inst[litem + '_slider'] = tp_inst.control.create(tp_inst, $tp.find('.ui_tpicker_' + litem + '_slider'), litem, tp_inst[litem], o[litem + 'Min'], max[litem], o['step' + uitem]);

                    // adjust the grid and add click event
                    if (show && o[litem + 'Grid'] > 0) {
                        size = 100 * gridSize[litem] * o[litem + 'Grid'] / (max[litem] - o[litem + 'Min']);
                        $tp.find('.ui_tpicker_' + litem + ' table').css({
                            width: size + "%",
                            marginLeft: o.isRTL ? '0' : ((size / (-2 * gridSize[litem])) + "%"),
                            marginRight: o.isRTL ? ((size / (-2 * gridSize[litem])) + "%") : '0',
                            borderCollapse: 'collapse'
                        }).find("td").click(function (e) {
                            var $t = $(this),
                                h = $t.html(),
                                n = parseInt(h.replace(/[^0-9]/g), 10),
                                ap = h.replace(/[^apm]/ig),
                                f = $t.data('for'); // loses scope, so we use data-for

                            if (f == 'hour') {
                                if (ap.indexOf('p') !== -1 && n < 12) {
                                    n += 12;
                                }
                                else {
                                    if (ap.indexOf('a') !== -1 && n === 12) {
                                        n = 0;
                                    }
                                }
                            }

                            tp_inst.control.value(tp_inst, tp_inst[f + '_slider'], litem, n);

                            tp_inst._onTimeChange();
                            tp_inst._onSelectHandler();
                        }).css({
                            cursor: 'pointer',
                            width: (100 / gridSize[litem]) + '%',
                            textAlign: 'center',
                            overflow: 'hidden'
                        });
                    } // end if grid > 0
                } // end for loop

                // Add timezone options
                this.timezone_select = $tp.find('.ui_tpicker_timezone').append('<select></select>').find("select");
                $.fn.append.apply(this.timezone_select,
				$.map(o.timezoneList, function (val, idx) {
				    return $("<option />").val(typeof val == "object" ? val.value : val).text(typeof val == "object" ? val.label : val);
				}));
                if (typeof (this.timezone) != "undefined" && this.timezone !== null && this.timezone !== "") {
                    var local_timezone = (new Date(this.inst.selectedYear, this.inst.selectedMonth, this.inst.selectedDay, 12)).getTimezoneOffset() * -1;
                    if (local_timezone == this.timezone) {
                        selectLocalTimezone(tp_inst);
                    } else {
                        this.timezone_select.val(this.timezone);
                    }
                } else {
                    if (typeof (this.hour) != "undefined" && this.hour !== null && this.hour !== "") {
                        this.timezone_select.val(o.timezone);
                    } else {
                        selectLocalTimezone(tp_inst);
                    }
                }
                this.timezone_select.change(function () {
                    tp_inst._onTimeChange();
                    tp_inst._onSelectHandler();
                });
                // End timezone options

                // inject timepicker into datepicker
                var $buttonPanel = $dp.find('.ui-datepicker-buttonpane');
                if ($buttonPanel.length) {
                    $buttonPanel.before($tp);
                } else {
                    $dp.append($tp);
                }

                this.$timeObj = $tp.find('.ui_tpicker_time');

                if (this.inst !== null) {
                    var timeDefined = this.timeDefined;
                    this._onTimeChange();
                    this.timeDefined = timeDefined;
                }

                // slideAccess integration: http://trentrichardson.com/2011/11/11/jquery-ui-sliders-and-touch-accessibility/
                if (this._defaults.addSliderAccess) {
                    var sliderAccessArgs = this._defaults.sliderAccessArgs,
						rtl = this._defaults.isRTL;
                    sliderAccessArgs.isRTL = rtl;

                    setTimeout(function () { // fix for inline mode
                        if ($tp.find('.ui-slider-access').length === 0) {
                            $tp.find('.ui-slider:visible').sliderAccess(sliderAccessArgs);

                            // fix any grids since sliders are shorter
                            var sliderAccessWidth = $tp.find('.ui-slider-access:eq(0)').outerWidth(true);
                            if (sliderAccessWidth) {
                                $tp.find('table:visible').each(function () {
                                    var $g = $(this),
										oldWidth = $g.outerWidth(),
										oldMarginLeft = $g.css(rtl ? 'marginRight' : 'marginLeft').toString().replace('%', ''),
										newWidth = oldWidth - sliderAccessWidth,
										newMarginLeft = ((oldMarginLeft * newWidth) / oldWidth) + '%',
										css = { width: newWidth, marginRight: 0, marginLeft: 0 };
                                    css[rtl ? 'marginRight' : 'marginLeft'] = newMarginLeft;
                                    $g.css(css);
                                });
                            }
                        }
                    }, 10);
                }
                // end slideAccess integration

            }
        },

        /*
		* This function tries to limit the ability to go outside the
		* min/max date range
		*/
        _limitMinMaxDateTime: function (dp_inst, adjustSliders) {
            var o = this._defaults,
				dp_date = new Date(dp_inst.selectedYear, dp_inst.selectedMonth, dp_inst.selectedDay);

            if (!this._defaults.showTimepicker) {
                return;
            } // No time so nothing to check here

            if ($.datepicker._get(dp_inst, 'minDateTime') !== null && $.datepicker._get(dp_inst, 'minDateTime') !== undefined && dp_date) {
                var minDateTime = $.datepicker._get(dp_inst, 'minDateTime'),
					minDateTimeDate = new Date(minDateTime.getFullYear(), minDateTime.getMonth(), minDateTime.getDate(), 0, 0, 0, 0);

                if (this.hourMinOriginal === null || this.minuteMinOriginal === null || this.secondMinOriginal === null || this.millisecMinOriginal === null || this.microsecMinOriginal === null) {
                    this.hourMinOriginal = o.hourMin;
                    this.minuteMinOriginal = o.minuteMin;
                    this.secondMinOriginal = o.secondMin;
                    this.millisecMinOriginal = o.millisecMin;
                    this.microsecMinOriginal = o.microsecMin;
                }

                if (dp_inst.settings.timeOnly || minDateTimeDate.getTime() == dp_date.getTime()) {
                    this._defaults.hourMin = minDateTime.getHours();
                    if (this.hour <= this._defaults.hourMin) {
                        this.hour = this._defaults.hourMin;
                        this._defaults.minuteMin = minDateTime.getMinutes();
                        if (this.minute <= this._defaults.minuteMin) {
                            this.minute = this._defaults.minuteMin;
                            this._defaults.secondMin = minDateTime.getSeconds();
                            if (this.second <= this._defaults.secondMin) {
                                this.second = this._defaults.secondMin;
                                this._defaults.millisecMin = minDateTime.getMilliseconds();
                                if (this.millisec <= this._defaults.millisecMin) {
                                    this.millisec = this._defaults.millisecMin;
                                    this._defaults.microsecMin = minDateTime.getMicroseconds();
                                } else {
                                    if (this.microsec < this._defaults.microsecMin) {
                                        this.microsec = this._defaults.microsecMin;
                                    }
                                    this._defaults.microsecMin = this.microsecMinOriginal;
                                }
                            } else {
                                this._defaults.millisecMin = this.millisecMinOriginal;
                                this._defaults.microsecMin = this.microsecMinOriginal;
                            }
                        } else {
                            this._defaults.secondMin = this.secondMinOriginal;
                            this._defaults.millisecMin = this.millisecMinOriginal;
                            this._defaults.microsecMin = this.microsecMinOriginal;
                        }
                    } else {
                        this._defaults.minuteMin = this.minuteMinOriginal;
                        this._defaults.secondMin = this.secondMinOriginal;
                        this._defaults.millisecMin = this.millisecMinOriginal;
                        this._defaults.microsecMin = this.microsecMinOriginal;
                    }
                } else {
                    this._defaults.hourMin = this.hourMinOriginal;
                    this._defaults.minuteMin = this.minuteMinOriginal;
                    this._defaults.secondMin = this.secondMinOriginal;
                    this._defaults.millisecMin = this.millisecMinOriginal;
                    this._defaults.microsecMin = this.microsecMinOriginal;
                }
            }

            if ($.datepicker._get(dp_inst, 'maxDateTime') !== null && $.datepicker._get(dp_inst, 'maxDateTime') !== undefined && dp_date) {
                var maxDateTime = $.datepicker._get(dp_inst, 'maxDateTime'),
					maxDateTimeDate = new Date(maxDateTime.getFullYear(), maxDateTime.getMonth(), maxDateTime.getDate(), 0, 0, 0, 0);

                if (this.hourMaxOriginal === null || this.minuteMaxOriginal === null || this.secondMaxOriginal === null || this.millisecMaxOriginal === null) {
                    this.hourMaxOriginal = o.hourMax;
                    this.minuteMaxOriginal = o.minuteMax;
                    this.secondMaxOriginal = o.secondMax;
                    this.millisecMaxOriginal = o.millisecMax;
                    this.microsecMaxOriginal = o.microsecMax;
                }

                if (dp_inst.settings.timeOnly || maxDateTimeDate.getTime() == dp_date.getTime()) {
                    this._defaults.hourMax = maxDateTime.getHours();
                    if (this.hour >= this._defaults.hourMax) {
                        this.hour = this._defaults.hourMax;
                        this._defaults.minuteMax = maxDateTime.getMinutes();
                        if (this.minute >= this._defaults.minuteMax) {
                            this.minute = this._defaults.minuteMax;
                            this._defaults.secondMax = maxDateTime.getSeconds();
                            if (this.second >= this._defaults.secondMax) {
                                this.second = this._defaults.secondMax;
                                this._defaults.millisecMax = maxDateTime.getMilliseconds();
                                if (this.millisec >= this._defaults.millisecMax) {
                                    this.millisec = this._defaults.millisecMax;
                                    this._defaults.microsecMax = maxDateTime.getMicroseconds();
                                } else {
                                    if (this.microsec > this._defaults.microsecMax) {
                                        this.microsec = this._defaults.microsecMax;
                                    }
                                    this._defaults.microsecMax = this.microsecMaxOriginal;
                                }
                            } else {
                                this._defaults.millisecMax = this.millisecMaxOriginal;
                                this._defaults.microsecMax = this.microsecMaxOriginal;
                            }
                        } else {
                            this._defaults.secondMax = this.secondMaxOriginal;
                            this._defaults.millisecMax = this.millisecMaxOriginal;
                            this._defaults.microsecMax = this.microsecMaxOriginal;
                        }
                    } else {
                        this._defaults.minuteMax = this.minuteMaxOriginal;
                        this._defaults.secondMax = this.secondMaxOriginal;
                        this._defaults.millisecMax = this.millisecMaxOriginal;
                        this._defaults.microsecMax = this.microsecMaxOriginal;
                    }
                } else {
                    this._defaults.hourMax = this.hourMaxOriginal;
                    this._defaults.minuteMax = this.minuteMaxOriginal;
                    this._defaults.secondMax = this.secondMaxOriginal;
                    this._defaults.millisecMax = this.millisecMaxOriginal;
                    this._defaults.microsecMax = this.microsecMaxOriginal;
                }
            }

            if (adjustSliders !== undefined && adjustSliders === true) {
                var hourMax = parseInt((this._defaults.hourMax - ((this._defaults.hourMax - this._defaults.hourMin) % this._defaults.stepHour)), 10),
					minMax = parseInt((this._defaults.minuteMax - ((this._defaults.minuteMax - this._defaults.minuteMin) % this._defaults.stepMinute)), 10),
					secMax = parseInt((this._defaults.secondMax - ((this._defaults.secondMax - this._defaults.secondMin) % this._defaults.stepSecond)), 10),
					millisecMax = parseInt((this._defaults.millisecMax - ((this._defaults.millisecMax - this._defaults.millisecMin) % this._defaults.stepMillisec)), 10);
                microsecMax = parseInt((this._defaults.microsecMax - ((this._defaults.microsecMax - this._defaults.microsecMin) % this._defaults.stepMicrosec)), 10);

                if (this.hour_slider) {
                    this.control.options(this, this.hour_slider, 'hour', { min: this._defaults.hourMin, max: hourMax });
                    this.control.value(this, this.hour_slider, 'hour', this.hour - (this.hour % this._defaults.stepHour));
                }
                if (this.minute_slider) {
                    this.control.options(this, this.minute_slider, 'minute', { min: this._defaults.minuteMin, max: minMax });
                    this.control.value(this, this.minute_slider, 'minute', this.minute - (this.minute % this._defaults.stepMinute));
                }
                if (this.second_slider) {
                    this.control.options(this, this.second_slider, 'second', { min: this._defaults.secondMin, max: secMax });
                    this.control.value(this, this.second_slider, 'second', this.second - (this.second % this._defaults.stepSecond));
                }
                if (this.millisec_slider) {
                    this.control.options(this, this.millisec_slider, 'millisec', { min: this._defaults.millisecMin, max: millisecMax });
                    this.control.value(this, this.millisec_slider, 'millisec', this.millisec - (this.millisec % this._defaults.stepMillisec));
                }
                if (this.microsec_slider) {
                    this.control.options(this, this.microsec_slider, 'microsec', { min: this._defaults.microsecMin, max: microsecMax });
                    this.control.value(this, this.microsec_slider, 'microsec', this.microsec - (this.microsec % this._defaults.stepMicrosec));
                }
            }

        },

        /*
		* when a slider moves, set the internal time...
		* on time change is also called when the time is updated in the text field
		*/
        _onTimeChange: function () {
            var hour = (this.hour_slider) ? this.control.value(this, this.hour_slider, 'hour') : false,
				minute = (this.minute_slider) ? this.control.value(this, this.minute_slider, 'minute') : false,
				second = (this.second_slider) ? this.control.value(this, this.second_slider, 'second') : false,
				millisec = (this.millisec_slider) ? this.control.value(this, this.millisec_slider, 'millisec') : false,
				microsec = (this.microsec_slider) ? this.control.value(this, this.microsec_slider, 'microsec') : false,
				timezone = (this.timezone_select) ? this.timezone_select.val() : false,
				o = this._defaults,
				pickerTimeFormat = o.pickerTimeFormat || o.timeFormat,
				pickerTimeSuffix = o.pickerTimeSuffix || o.timeSuffix;

            if (typeof (hour) == 'object') {
                hour = false;
            }
            if (typeof (minute) == 'object') {
                minute = false;
            }
            if (typeof (second) == 'object') {
                second = false;
            }
            if (typeof (millisec) == 'object') {
                millisec = false;
            }
            if (typeof (microsec) == 'object') {
                microsec = false;
            }
            if (typeof (timezone) == 'object') {
                timezone = false;
            }

            if (hour !== false) {
                hour = parseInt(hour, 10);
            }
            if (minute !== false) {
                minute = parseInt(minute, 10);
            }
            if (second !== false) {
                second = parseInt(second, 10);
            }
            if (millisec !== false) {
                millisec = parseInt(millisec, 10);
            }
            if (microsec !== false) {
                microsec = parseInt(microsec, 10);
            }

            var ampm = o[hour < 12 ? 'amNames' : 'pmNames'][0];

            // If the update was done in the input field, the input field should not be updated.
            // If the update was done using the sliders, update the input field.
            var hasChanged = (hour != this.hour || minute != this.minute || second != this.second || millisec != this.millisec || microsec != this.microsec
								|| (this.ampm.length > 0 && (hour < 12) != ($.inArray(this.ampm.toUpperCase(), this.amNames) !== -1))
								|| (this.timezone !== null && timezone != this.timezone));

            if (hasChanged) {

                if (hour !== false) {
                    this.hour = hour;
                }
                if (minute !== false) {
                    this.minute = minute;
                }
                if (second !== false) {
                    this.second = second;
                }
                if (millisec !== false) {
                    this.millisec = millisec;
                }
                if (microsec !== false) {
                    this.microsec = microsec;
                }
                if (timezone !== false) {
                    this.timezone = timezone;
                }

                if (!this.inst) {
                    this.inst = $.datepicker._getInst(this.$input[0]);
                }

                this._limitMinMaxDateTime(this.inst, true);
            }
            if (this.support.ampm) {
                this.ampm = ampm;
            }

            // Updates the time within the timepicker
            this.formattedTime = $.datepicker.formatTime(o.timeFormat, this, o);
            if (this.$timeObj) {
                if (pickerTimeFormat === o.timeFormat) {
                    this.$timeObj.text(this.formattedTime + pickerTimeSuffix);
                }
                else {
                    this.$timeObj.text($.datepicker.formatTime(pickerTimeFormat, this, o) + pickerTimeSuffix);
                }
            }

            this.timeDefined = true;
            if (hasChanged) {
                this._updateDateTime();
            }
        },

        /*
		* call custom onSelect.
		* bind to sliders slidestop, and grid click.
		*/
        _onSelectHandler: function () {
            var onSelect = this._defaults.onSelect || this.inst.settings.onSelect;
            var inputEl = this.$input ? this.$input[0] : null;
            if (onSelect && inputEl) {
                onSelect.apply(inputEl, [this.formattedDateTime, this]);
            }
        },

        /*
		* update our input with the new date time..
		*/
        _updateDateTime: function (dp_inst) {
            dp_inst = this.inst || dp_inst;
            var dt = $.datepicker._daylightSavingAdjust(new Date(dp_inst.selectedYear, dp_inst.selectedMonth, dp_inst.selectedDay)),
				dateFmt = $.datepicker._get(dp_inst, 'dateFormat'),
				formatCfg = $.datepicker._getFormatConfig(dp_inst),
				timeAvailable = dt !== null && this.timeDefined;
            this.formattedDate = $.datepicker.formatDate(dateFmt, (dt === null ? new Date() : dt), formatCfg);
            var formattedDateTime = this.formattedDate;

            // if a slider was changed but datepicker doesn't have a value yet, set it
            if (dp_inst.lastVal === "") {
                dp_inst.currentYear = dp_inst.selectedYear;
                dp_inst.currentMonth = dp_inst.selectedMonth;
                dp_inst.currentDay = dp_inst.selectedDay;
            }

            /*
			* remove following lines to force every changes in date picker to change the input value
			* Bug descriptions: when an input field has a default value, and click on the field to pop up the date picker. 
			* If the user manually empty the value in the input field, the date picker will never change selected value.
			*/
            //if (dp_inst.lastVal !== undefined && (dp_inst.lastVal.length > 0 && this.$input.val().length === 0)) {
            //	return;
            //}

            if (this._defaults.timeOnly === true) {
                formattedDateTime = this.formattedTime;
            } else if (this._defaults.timeOnly !== true && (this._defaults.alwaysSetTime || timeAvailable)) {
                formattedDateTime += this._defaults.separator + this.formattedTime + this._defaults.timeSuffix;
            }

            this.formattedDateTime = formattedDateTime;

            if (!this._defaults.showTimepicker) {
                this.$input.val(this.formattedDate);
            } else if (this.$altInput && this._defaults.timeOnly === false && this._defaults.altFieldTimeOnly === true) {
                this.$altInput.val(this.formattedTime);
                this.$input.val(this.formattedDate);
            } else if (this.$altInput) {
                this.$input.val(formattedDateTime);
                var altFormattedDateTime = '',
					altSeparator = this._defaults.altSeparator ? this._defaults.altSeparator : this._defaults.separator,
					altTimeSuffix = this._defaults.altTimeSuffix ? this._defaults.altTimeSuffix : this._defaults.timeSuffix;

                if (!this._defaults.timeOnly) {
                    if (this._defaults.altFormat) {
                        altFormattedDateTime = $.datepicker.formatDate(this._defaults.altFormat, (dt === null ? new Date() : dt), formatCfg);
                    }
                    else {
                        altFormattedDateTime = this.formattedDate;
                    }

                    if (altFormattedDateTime) {
                        altFormattedDateTime += altSeparator;
                    }
                }

                if (this._defaults.altTimeFormat) {
                    altFormattedDateTime += $.datepicker.formatTime(this._defaults.altTimeFormat, this, this._defaults) + altTimeSuffix;
                }
                else {
                    altFormattedDateTime += this.formattedTime + altTimeSuffix;
                }
                this.$altInput.val(altFormattedDateTime);
            } else {
                this.$input.val(formattedDateTime);
            }

            this.$input.trigger("change");
        },

        _onFocus: function () {
            if (!this.$input.val() && this._defaults.defaultValue) {
                this.$input.val(this._defaults.defaultValue);
                var inst = $.datepicker._getInst(this.$input.get(0)),
					tp_inst = $.datepicker._get(inst, 'timepicker');
                if (tp_inst) {
                    if (tp_inst._defaults.timeOnly && (inst.input.val() != inst.lastVal)) {
                        try {
                            $.datepicker._updateDatepicker(inst);
                        } catch (err) {
                            $.timepicker.log(err);
                        }
                    }
                }
            }
        },

        /*
		* Small abstraction to control types
		* We can add more, just be sure to follow the pattern: create, options, value
		*/
        _controls: {
            // slider methods
            slider: {
                create: function (tp_inst, obj, unit, val, min, max, step) {
                    var rtl = tp_inst._defaults.isRTL; // if rtl go -60->0 instead of 0->60
                    return obj.prop('slide', null).slider({
                        orientation: "horizontal",
                        value: rtl ? val * -1 : val,
                        min: rtl ? max * -1 : min,
                        max: rtl ? min * -1 : max,
                        step: step,
                        slide: function (event, ui) {
                            tp_inst.control.value(tp_inst, $(this), unit, rtl ? ui.value * -1 : ui.value);
                            tp_inst._onTimeChange();
                        },
                        stop: function (event, ui) {
                            tp_inst._onSelectHandler();
                        }
                    });
                },
                options: function (tp_inst, obj, unit, opts, val) {
                    if (tp_inst._defaults.isRTL) {
                        if (typeof (opts) == 'string') {
                            if (opts == 'min' || opts == 'max') {
                                if (val !== undefined) {
                                    return obj.slider(opts, val * -1);
                                }
                                return Math.abs(obj.slider(opts));
                            }
                            return obj.slider(opts);
                        }
                        var min = opts.min,
							max = opts.max;
                        opts.min = opts.max = null;
                        if (min !== undefined) {
                            opts.max = min * -1;
                        }
                        if (max !== undefined) {
                            opts.min = max * -1;
                        }
                        return obj.slider(opts);
                    }
                    if (typeof (opts) == 'string' && val !== undefined) {
                        return obj.slider(opts, val);
                    }
                    return obj.slider(opts);
                },
                value: function (tp_inst, obj, unit, val) {
                    if (tp_inst._defaults.isRTL) {
                        if (val !== undefined) {
                            return obj.slider('value', val * -1);
                        }
                        return Math.abs(obj.slider('value'));
                    }
                    if (val !== undefined) {
                        return obj.slider('value', val);
                    }
                    return obj.slider('value');
                }
            },
            // select methods
            select: {
                create: function (tp_inst, obj, unit, val, min, max, step) {
                    var sel = '<select class="ui-timepicker-select" data-unit="' + unit + '" data-min="' + min + '" data-max="' + max + '" data-step="' + step + '">',
						format = tp_inst._defaults.pickerTimeFormat || tp_inst._defaults.timeFormat;

                    for (var i = min; i <= max; i += step) {
                        sel += '<option value="' + i + '"' + (i == val ? ' selected' : '') + '>';
                        if (unit == 'hour') {
                            sel += $.datepicker.formatTime($.trim(format.replace(/[^ht ]/ig, '')), { hour: i }, tp_inst._defaults);
                        }
                        else if (unit == 'millisec' || unit == 'microsec' || i >= 10) { sel += i; }
                        else { sel += '0' + i.toString(); }
                        sel += '</option>';
                    }
                    sel += '</select>';

                    obj.children('select').remove();

                    $(sel).appendTo(obj).change(function (e) {
                        tp_inst._onTimeChange();
                        tp_inst._onSelectHandler();
                    });

                    return obj;
                },
                options: function (tp_inst, obj, unit, opts, val) {
                    var o = {},
						$t = obj.children('select');
                    if (typeof (opts) == 'string') {
                        if (val === undefined) {
                            return $t.data(opts);
                        }
                        o[opts] = val;
                    }
                    else { o = opts; }
                    return tp_inst.control.create(tp_inst, obj, $t.data('unit'), $t.val(), o.min || $t.data('min'), o.max || $t.data('max'), o.step || $t.data('step'));
                },
                value: function (tp_inst, obj, unit, val) {
                    var $t = obj.children('select');
                    if (val !== undefined) {
                        return $t.val(val);
                    }
                    return $t.val();
                }
            }
        } // end _controls

    });

    $.fn.extend({
        /*
		* shorthand just to use timepicker..
		*/
        timepicker: function (o) {
            o = o || {};
            var tmp_args = Array.prototype.slice.call(arguments);

            if (typeof o == 'object') {
                tmp_args[0] = $.extend(o, {
                    timeOnly: true
                });
            }

            return $(this).each(function () {
                $.fn.datetimepicker.apply($(this), tmp_args);
            });
        },

        /*
		* extend timepicker to datepicker
		*/
        datetimepicker: function (o) {
            o = o || {};
            var tmp_args = arguments;

            if (typeof (o) == 'string') {
                if (o == 'getDate') {
                    return $.fn.datepicker.apply($(this[0]), tmp_args);
                } else {
                    return this.each(function () {
                        var $t = $(this);
                        $t.datepicker.apply($t, tmp_args);
                    });
                }
            } else {
                return this.each(function () {
                    var $t = $(this);
                    $t.datepicker($.timepicker._newInst($t, o)._defaults);
                });
            }
        }
    });

    /*
	* Public Utility to parse date and time
	*/
    $.datepicker.parseDateTime = function (dateFormat, timeFormat, dateTimeString, dateSettings, timeSettings) {
        var parseRes = parseDateTimeInternal(dateFormat, timeFormat, dateTimeString, dateSettings, timeSettings);
        if (parseRes.timeObj) {
            var t = parseRes.timeObj;
            parseRes.date.setHours(t.hour, t.minute, t.second, t.millisec);
            parseRex.date.setMicroseconds(t.microsec);
        }

        return parseRes.date;
    };

    /*
	* Public utility to parse time
	*/
    $.datepicker.parseTime = function (timeFormat, timeString, options) {
        var o = extendRemove(extendRemove({}, $.timepicker._defaults), options || {}),
			iso8601 = (timeFormat.replace(/\'.*?\'/g, '').indexOf('Z') !== -1);

        // Strict parse requires the timeString to match the timeFormat exactly
        var strictParse = function (f, s, o) {

            // pattern for standard and localized AM/PM markers
            var getPatternAmpm = function (amNames, pmNames) {
                var markers = [];
                if (amNames) {
                    $.merge(markers, amNames);
                }
                if (pmNames) {
                    $.merge(markers, pmNames);
                }
                markers = $.map(markers, function (val) {
                    return val.replace(/[.*+?|()\[\]{}\\]/g, '\\$&');
                });
                return '(' + markers.join('|') + ')?';
            };

            // figure out position of time elements.. cause js cant do named captures
            var getFormatPositions = function (timeFormat) {
                var finds = timeFormat.toLowerCase().match(/(h{1,2}|m{1,2}|s{1,2}|l{1}|c{1}|t{1,2}|z|'.*?')/g),
					orders = {
					    h: -1,
					    m: -1,
					    s: -1,
					    l: -1,
					    c: -1,
					    t: -1,
					    z: -1
					};

                if (finds) {
                    for (var i = 0; i < finds.length; i++) {
                        if (orders[finds[i].toString().charAt(0)] == -1) {
                            orders[finds[i].toString().charAt(0)] = i + 1;
                        }
                    }
                }
                return orders;
            };

            var regstr = '^' + f.toString()
					.replace(/([hH]{1,2}|mm?|ss?|[tT]{1,2}|[zZ]|[lc]|'.*?')/g, function (match) {
					    var ml = match.length;
					    switch (match.charAt(0).toLowerCase()) {
					        case 'h': return ml === 1 ? '(\\d?\\d)' : '(\\d{' + ml + '})';
					        case 'm': return ml === 1 ? '(\\d?\\d)' : '(\\d{' + ml + '})';
					        case 's': return ml === 1 ? '(\\d?\\d)' : '(\\d{' + ml + '})';
					        case 'l': return '(\\d?\\d?\\d)';
					        case 'c': return '(\\d?\\d?\\d)';
					        case 'z': return '(z|[-+]\\d\\d:?\\d\\d|\\S+)?';
					        case 't': return getPatternAmpm(o.amNames, o.pmNames);
					        default:    // literal escaped in quotes
					            return '(' + match.replace(/\'/g, "").replace(/(\.|\$|\^|\\|\/|\(|\)|\[|\]|\?|\+|\*)/g, function (m) { return "\\" + m; }) + ')?';
					    }
					})
					.replace(/\s/g, '\\s?') +
					o.timeSuffix + '$',
				order = getFormatPositions(f),
				ampm = '',
				treg;

            treg = s.match(new RegExp(regstr, 'i'));

            var resTime = {
                hour: 0,
                minute: 0,
                second: 0,
                millisec: 0,
                microsec: 0
            };

            if (treg) {
                if (order.t !== -1) {
                    if (treg[order.t] === undefined || treg[order.t].length === 0) {
                        ampm = '';
                        resTime.ampm = '';
                    } else {
                        ampm = $.inArray(treg[order.t].toUpperCase(), o.amNames) !== -1 ? 'AM' : 'PM';
                        resTime.ampm = o[ampm == 'AM' ? 'amNames' : 'pmNames'][0];
                    }
                }

                if (order.h !== -1) {
                    if (ampm == 'AM' && treg[order.h] == '12') {
                        resTime.hour = 0; // 12am = 0 hour
                    } else {
                        if (ampm == 'PM' && treg[order.h] != '12') {
                            resTime.hour = parseInt(treg[order.h], 10) + 12; // 12pm = 12 hour, any other pm = hour + 12
                        } else {
                            resTime.hour = Number(treg[order.h]);
                        }
                    }
                }

                if (order.m !== -1) {
                    resTime.minute = Number(treg[order.m]);
                }
                if (order.s !== -1) {
                    resTime.second = Number(treg[order.s]);
                }
                if (order.l !== -1) {
                    resTime.millisec = Number(treg[order.l]);
                }
                if (order.c !== -1) {
                    resTime.microsec = Number(treg[order.c]);
                }
                if (order.z !== -1 && treg[order.z] !== undefined) {
                    resTime.timezone = $.timepicker.timezoneOffsetNumber(treg[order.z]);
                }


                return resTime;
            }
            return false;
        };// end strictParse

        // First try JS Date, if that fails, use strictParse
        var looseParse = function (f, s, o) {
            try {
                var d = new Date('2012-01-01 ' + s);
                if (isNaN(d.getTime())) {
                    d = new Date('2012-01-01T' + s);
                    if (isNaN(d.getTime())) {
                        d = new Date('01/01/2012 ' + s);
                        if (isNaN(d.getTime())) {
                            throw "Unable to parse time with native Date: " + s;
                        }
                    }
                }

                return {
                    hour: d.getHours(),
                    minute: d.getMinutes(),
                    second: d.getSeconds(),
                    millisec: d.getMilliseconds(),
                    microsec: d.getMicroseconds(),
                    timezone: d.getTimezoneOffset() * -1
                };
            }
            catch (err) {
                try {
                    return strictParse(f, s, o);
                }
                catch (err2) {
                    $.timepicker.log("Unable to parse \ntimeString: " + s + "\ntimeFormat: " + f);
                }
            }
            return false;
        }; // end looseParse

        if (typeof o.parse === "function") {
            return o.parse(timeFormat, timeString, o);
        }
        if (o.parse === 'loose') {
            return looseParse(timeFormat, timeString, o);
        }
        return strictParse(timeFormat, timeString, o);
    };

    /*
	* Public utility to format the time
	* format = string format of the time
	* time = a {}, not a Date() for timezones
	* options = essentially the regional[].. amNames, pmNames, ampm
	*/
    $.datepicker.formatTime = function (format, time, options) {
        options = options || {};
        options = $.extend({}, $.timepicker._defaults, options);
        time = $.extend({
            hour: 0,
            minute: 0,
            second: 0,
            millisec: 0,
            timezone: 0
        }, time);

        var tmptime = format,
			ampmName = options.amNames[0],
			hour = parseInt(time.hour, 10);

        if (hour > 11) {
            ampmName = options.pmNames[0];
        }

        tmptime = tmptime.replace(/(?:HH?|hh?|mm?|ss?|[tT]{1,2}|[zZ]|[lc]|('.*?'|".*?"))/g, function (match) {
            switch (match) {
                case 'HH':
                    return ('0' + hour).slice(-2);
                case 'H':
                    return hour;
                case 'hh':
                    return ('0' + convert24to12(hour)).slice(-2);
                case 'h':
                    return convert24to12(hour);
                case 'mm':
                    return ('0' + time.minute).slice(-2);
                case 'm':
                    return time.minute;
                case 'ss':
                    return ('0' + time.second).slice(-2);
                case 's':
                    return time.second;
                case 'l':
                    return ('00' + time.millisec).slice(-3);
                case 'c':
                    return ('00' + time.microsec).slice(-3);
                case 'z':
                    return $.timepicker.timezoneOffsetString(time.timezone === null ? options.timezone : time.timezone, false);
                case 'Z':
                    return $.timepicker.timezoneOffsetString(time.timezone === null ? options.timezone : time.timezone, true);
                case 'T':
                    return ampmName.charAt(0).toUpperCase();
                case 'TT':
                    return ampmName.toUpperCase();
                case 't':
                    return ampmName.charAt(0).toLowerCase();
                case 'tt':
                    return ampmName.toLowerCase();
                default:
                    return match.replace(/\'/g, "") || "'";
            }
        });

        tmptime = $.trim(tmptime);
        return tmptime;
    };

    /*
	* the bad hack :/ override datepicker so it doesnt close on select
	// inspired: http://stackoverflow.com/questions/1252512/jquery-datepicker-prevent-closing-picker-when-clicking-a-date/1762378#1762378
	*/
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
        } else {
            this._base_selectDate(id, dateStr);
        }
    };

    /*
	* second bad hack :/ override datepicker so it triggers an event when changing the input field
	* and does not redraw the datepicker on every selectDate event
	*/
    $.datepicker._base_updateDatepicker = $.datepicker._updateDatepicker;
    $.datepicker._updateDatepicker = function (inst) {

        // don't popup the datepicker if there is another instance already opened
        var input = inst.input[0];
        if ($.datepicker._curInst && $.datepicker._curInst != inst && $.datepicker._datepickerShowing && $.datepicker._lastInput != input) {
            return;
        }

        if (typeof (inst.stay_open) !== 'boolean' || inst.stay_open === false) {

            this._base_updateDatepicker(inst);

            // Reload the time control when changing something in the input text field.
            var tp_inst = this._get(inst, 'timepicker');
            if (tp_inst) {
                tp_inst._addTimePicker(inst);
            }
        }
    };

    /*
	* third bad hack :/ override datepicker so it allows spaces and colon in the input field
	*/
    $.datepicker._base_doKeyPress = $.datepicker._doKeyPress;
    $.datepicker._doKeyPress = function (event) {
        var inst = $.datepicker._getInst(event.target),
			tp_inst = $.datepicker._get(inst, 'timepicker');

        if (tp_inst) {
            if ($.datepicker._get(inst, 'constrainInput')) {
                var ampm = tp_inst.support.ampm,
					tz = tp_inst._defaults.showTimezone !== null ? tp_inst._defaults.showTimezone : tp_inst.support.timezone,
					dateChars = $.datepicker._possibleChars($.datepicker._get(inst, 'dateFormat')),
					datetimeChars = tp_inst._defaults.timeFormat.toString()
											.replace(/[hms]/g, '')
											.replace(/TT/g, ampm ? 'APM' : '')
											.replace(/Tt/g, ampm ? 'AaPpMm' : '')
											.replace(/tT/g, ampm ? 'AaPpMm' : '')
											.replace(/T/g, ampm ? 'AP' : '')
											.replace(/tt/g, ampm ? 'apm' : '')
											.replace(/t/g, ampm ? 'ap' : '') +
											" " + tp_inst._defaults.separator +
											tp_inst._defaults.timeSuffix +
											(tz ? tp_inst._defaults.timezoneList.join('') : '') +
											(tp_inst._defaults.amNames.join('')) + (tp_inst._defaults.pmNames.join('')) +
											dateChars,
					chr = String.fromCharCode(event.charCode === undefined ? event.keyCode : event.charCode);
                return event.ctrlKey || (chr < ' ' || !dateChars || datetimeChars.indexOf(chr) > -1);
            }
        }

        return $.datepicker._base_doKeyPress(event);
    };

    /*
	* Fourth bad hack :/ override _updateAlternate function used in inline mode to init altField
	*/
    $.datepicker._base_updateAlternate = $.datepicker._updateAlternate;
    /* Update any alternate field to synchronise with the main field. */
    $.datepicker._updateAlternate = function (inst) {
        var tp_inst = this._get(inst, 'timepicker');
        if (tp_inst) {
            var altField = tp_inst._defaults.altField;
            if (altField) { // update alternate field too
                var altFormat = tp_inst._defaults.altFormat || tp_inst._defaults.dateFormat,
					date = this._getDate(inst),
					formatCfg = $.datepicker._getFormatConfig(inst),
					altFormattedDateTime = '',
					altSeparator = tp_inst._defaults.altSeparator ? tp_inst._defaults.altSeparator : tp_inst._defaults.separator,
					altTimeSuffix = tp_inst._defaults.altTimeSuffix ? tp_inst._defaults.altTimeSuffix : tp_inst._defaults.timeSuffix,
					altTimeFormat = tp_inst._defaults.altTimeFormat !== null ? tp_inst._defaults.altTimeFormat : tp_inst._defaults.timeFormat;

                altFormattedDateTime += $.datepicker.formatTime(altTimeFormat, tp_inst, tp_inst._defaults) + altTimeSuffix;
                if (!tp_inst._defaults.timeOnly && !tp_inst._defaults.altFieldTimeOnly && date !== null) {
                    if (tp_inst._defaults.altFormat) {
                        altFormattedDateTime = $.datepicker.formatDate(tp_inst._defaults.altFormat, date, formatCfg) + altSeparator + altFormattedDateTime;
                    }
                    else {
                        altFormattedDateTime = tp_inst.formattedDate + altSeparator + altFormattedDateTime;
                    }
                }
                $(altField).val(altFormattedDateTime);
            }
        }
        else {
            $.datepicker._base_updateAlternate(inst);
        }
    };

    /*
	* Override key up event to sync manual input changes.
	*/
    $.datepicker._base_doKeyUp = $.datepicker._doKeyUp;
    $.datepicker._doKeyUp = function (event) {
        var inst = $.datepicker._getInst(event.target),
			tp_inst = $.datepicker._get(inst, 'timepicker');

        if (tp_inst) {
            if (tp_inst._defaults.timeOnly && (inst.input.val() != inst.lastVal)) {
                try {
                    $.datepicker._updateDatepicker(inst);
                } catch (err) {
                    $.timepicker.log(err);
                }
            }
        }

        return $.datepicker._base_doKeyUp(event);
    };

    /*
	* override "Today" button to also grab the time.
	*/
    $.datepicker._base_gotoToday = $.datepicker._gotoToday;
    $.datepicker._gotoToday = function (id) {
        var inst = this._getInst($(id)[0]),
			$dp = inst.dpDiv;
        this._base_gotoToday(id);
        var tp_inst = this._get(inst, 'timepicker');
        selectLocalTimezone(tp_inst);
        var now = new Date();
        this._setTime(inst, now);
        $('.ui-datepicker-today', $dp).click();
    };

    /*
	* Disable & enable the Time in the datetimepicker
	*/
    $.datepicker._disableTimepickerDatepicker = function (target) {
        var inst = this._getInst(target);
        if (!inst) {
            return;
        }

        var tp_inst = this._get(inst, 'timepicker');
        $(target).datepicker('getDate'); // Init selected[Year|Month|Day]
        if (tp_inst) {
            tp_inst._defaults.showTimepicker = false;
            tp_inst._updateDateTime(inst);
        }
    };

    $.datepicker._enableTimepickerDatepicker = function (target) {
        var inst = this._getInst(target);
        if (!inst) {
            return;
        }

        var tp_inst = this._get(inst, 'timepicker');
        $(target).datepicker('getDate'); // Init selected[Year|Month|Day]
        if (tp_inst) {
            tp_inst._defaults.showTimepicker = true;
            tp_inst._addTimePicker(inst); // Could be disabled on page load
            tp_inst._updateDateTime(inst);
        }
    };

    /*
	* Create our own set time function
	*/
    $.datepicker._setTime = function (inst, date) {
        var tp_inst = this._get(inst, 'timepicker');
        if (tp_inst) {
            var defaults = tp_inst._defaults;

            // calling _setTime with no date sets time to defaults
            tp_inst.hour = date ? date.getHours() : defaults.hour;
            tp_inst.minute = date ? date.getMinutes() : defaults.minute;
            tp_inst.second = date ? date.getSeconds() : defaults.second;
            tp_inst.millisec = date ? date.getMilliseconds() : defaults.millisec;
            tp_inst.microsec = date ? date.getMicroseconds() : defaults.microsec;

            //check if within min/max times.. 
            tp_inst._limitMinMaxDateTime(inst, true);

            tp_inst._onTimeChange();
            tp_inst._updateDateTime(inst);
        }
    };

    /*
	* Create new public method to set only time, callable as $().datepicker('setTime', date)
	*/
    $.datepicker._setTimeDatepicker = function (target, date, withDate) {
        var inst = this._getInst(target);
        if (!inst) {
            return;
        }

        var tp_inst = this._get(inst, 'timepicker');

        if (tp_inst) {
            this._setDateFromField(inst);
            var tp_date;
            if (date) {
                if (typeof date == "string") {
                    tp_inst._parseTime(date, withDate);
                    tp_date = new Date();
                    tp_date.setHours(tp_inst.hour, tp_inst.minute, tp_inst.second, tp_inst.millisec);
                    tp_date.setMicroseconds(tp_inst.microsec);
                } else {
                    tp_date = new Date(date.getTime());
                }
                if (tp_date.toString() == 'Invalid Date') {
                    tp_date = undefined;
                }
                this._setTime(inst, tp_date);
            }
        }

    };

    /*
	* override setDate() to allow setting time too within Date object
	*/
    $.datepicker._base_setDateDatepicker = $.datepicker._setDateDatepicker;
    $.datepicker._setDateDatepicker = function (target, date) {
        var inst = this._getInst(target);
        if (!inst) {
            return;
        }

        var tp_inst = this._get(inst, 'timepicker'),
			tp_date = (date instanceof Date) ? new Date(date.getTime()) : date;

        // This is important if you are using the timezone option, javascript's Date 
        // object will only return the timezone offset for the current locale, so we 
        // adjust it accordingly.  If not using timezone option this won't matter..
        // If a timezone is different in tp, keep the timezone as is
        if (tp_inst && tp_inst.timezone != null) {
            date = $.timepicker.timezoneAdjust(date, tp_inst.timezone);
            tp_date = $.timepicker.timezoneAdjust(tp_date, tp_inst.timezone);
        }

        this._updateDatepicker(inst);
        this._base_setDateDatepicker.apply(this, arguments);
        this._setTimeDatepicker(target, tp_date, true);
    };

    /*
	* override getDate() to allow getting time too within Date object
	*/
    $.datepicker._base_getDateDatepicker = $.datepicker._getDateDatepicker;
    $.datepicker._getDateDatepicker = function (target, noDefault) {
        var inst = this._getInst(target);
        if (!inst) {
            return;
        }

        var tp_inst = this._get(inst, 'timepicker');

        if (tp_inst) {
            // if it hasn't yet been defined, grab from field
            if (inst.lastVal === undefined) {
                this._setDateFromField(inst, noDefault);
            }

            var date = this._getDate(inst);
            if (date && tp_inst._parseTime($(target).val(), tp_inst.timeOnly)) {
                date.setHours(tp_inst.hour, tp_inst.minute, tp_inst.second, tp_inst.millisec);
                date.setMicroseconds(tp_inst.microsec);

                // This is important if you are using the timezone option, javascript's Date 
                // object will only return the timezone offset for the current locale, so we 
                // adjust it accordingly.  If not using timezone option this won't matter..
                if (tp_inst.timezone != null) {
                    date = $.timepicker.timezoneAdjust(date, tp_inst.timezone);
                }
            }
            return date;
        }
        return this._base_getDateDatepicker(target, noDefault);
    };

    /*
	* override parseDate() because UI 1.8.14 throws an error about "Extra characters"
	* An option in datapicker to ignore extra format characters would be nicer.
	*/
    $.datepicker._base_parseDate = $.datepicker.parseDate;
    $.datepicker.parseDate = function (format, value, settings) {
        var date;
        try {
            date = this._base_parseDate(format, value, settings);
        } catch (err) {
            // Hack!  The error message ends with a colon, a space, and
            // the "extra" characters.  We rely on that instead of
            // attempting to perfectly reproduce the parsing algorithm.
            if (err.indexOf(":") >= 0) {
                date = this._base_parseDate(format, value.substring(0, value.length - (err.length - err.indexOf(':') - 2)), settings);
                $.timepicker.log("Error parsing the date string: " + err + "\ndate string = " + value + "\ndate format = " + format);
            } else {
                throw err;
            }
        }
        return date;
    };

    /*
	* override formatDate to set date with time to the input
	*/
    $.datepicker._base_formatDate = $.datepicker._formatDate;
    $.datepicker._formatDate = function (inst, day, month, year) {
        var tp_inst = this._get(inst, 'timepicker');
        if (tp_inst) {
            tp_inst._updateDateTime(inst);
            return tp_inst.$input.val();
        }
        return this._base_formatDate(inst);
    };

    /*
	* override options setter to add time to maxDate(Time) and minDate(Time). MaxDate
	*/
    $.datepicker._base_optionDatepicker = $.datepicker._optionDatepicker;
    $.datepicker._optionDatepicker = function (target, name, value) {
        var inst = this._getInst(target),
	        name_clone;
        if (!inst) {
            return null;
        }

        var tp_inst = this._get(inst, 'timepicker');
        if (tp_inst) {
            var min = null,
				max = null,
				onselect = null,
				overrides = tp_inst._defaults.evnts,
				fns = {},
				prop;
            if (typeof name == 'string') { // if min/max was set with the string
                if (name === 'minDate' || name === 'minDateTime') {
                    min = value;
                } else if (name === 'maxDate' || name === 'maxDateTime') {
                    max = value;
                } else if (name === 'onSelect') {
                    onselect = value;
                } else if (overrides.hasOwnProperty(name)) {
                    if (typeof (value) === 'undefined') {
                        return overrides[name];
                    }
                    fns[name] = value;
                    name_clone = {}; //empty results in exiting function after overrides updated
                }
            } else if (typeof name == 'object') { //if min/max was set with the JSON
                if (name.minDate) {
                    min = name.minDate;
                } else if (name.minDateTime) {
                    min = name.minDateTime;
                } else if (name.maxDate) {
                    max = name.maxDate;
                } else if (name.maxDateTime) {
                    max = name.maxDateTime;
                }
                for (prop in overrides) {
                    if (overrides.hasOwnProperty(prop) && name[prop]) {
                        fns[prop] = name[prop];
                    }
                }
            }
            for (prop in fns) {
                if (fns.hasOwnProperty(prop)) {
                    overrides[prop] = fns[prop];
                    if (!name_clone) { name_clone = $.extend({}, name); }
                    delete name_clone[prop];
                }
            }
            if (name_clone && isEmptyObject(name_clone)) { return; }
            if (min) { //if min was set
                if (min === 0) {
                    min = new Date();
                } else {
                    min = new Date(min);
                }
                tp_inst._defaults.minDate = min;
                tp_inst._defaults.minDateTime = min;
            } else if (max) { //if max was set
                if (max === 0) {
                    max = new Date();
                } else {
                    max = new Date(max);
                }
                tp_inst._defaults.maxDate = max;
                tp_inst._defaults.maxDateTime = max;
            } else if (onselect) {
                tp_inst._defaults.onSelect = onselect;
            }
        }
        if (value === undefined) {
            return this._base_optionDatepicker.call($.datepicker, target, name);
        }
        return this._base_optionDatepicker.call($.datepicker, target, name_clone || name, value);
    };

    /*
	* jQuery isEmptyObject does not check hasOwnProperty - if someone has added to the object prototype,
	* it will return false for all objects
	*/
    var isEmptyObject = function (obj) {
        var prop;
        for (prop in obj) {
            if (obj.hasOwnProperty(obj)) {
                return false;
            }
        }
        return true;
    };

    /*
	* jQuery extend now ignores nulls!
	*/
    var extendRemove = function (target, props) {
        $.extend(target, props);
        for (var name in props) {
            if (props[name] === null || props[name] === undefined) {
                target[name] = props[name];
            }
        }
        return target;
    };

    /*
	* Determine by the time format which units are supported
	* Returns an object of booleans for each unit
	*/
    var detectSupport = function (timeFormat) {
        var tf = timeFormat.replace(/\'.*?\'/g, '').toLowerCase(), // removes literals
			isIn = function (f, t) { // does the format contain the token?
			    return f.indexOf(t) !== -1 ? true : false;
			};
        return {
            hour: isIn(tf, 'h'),
            minute: isIn(tf, 'm'),
            second: isIn(tf, 's'),
            millisec: isIn(tf, 'l'),
            microsec: isIn(tf, 'c'),
            timezone: isIn(tf, 'z'),
            ampm: isIn('t') && isIn(timeFormat, 'h'),
            iso8601: isIn(timeFormat, 'Z')
        };
    };

    /*
	* Converts 24 hour format into 12 hour
	* Returns 12 hour without leading 0
	*/
    var convert24to12 = function (hour) {
        if (hour > 12) {
            hour = hour - 12;
        }

        if (hour === 0) {
            hour = 12;
        }

        return String(hour);
    };

    /*
	* Splits datetime string into date ans time substrings.
	* Throws exception when date can't be parsed
	* Returns [dateString, timeString]
	*/
    var splitDateTime = function (dateFormat, dateTimeString, dateSettings, timeSettings) {
        try {
            // The idea is to get the number separator occurances in datetime and the time format requested (since time has 
            // fewer unknowns, mostly numbers and am/pm). We will use the time pattern to split.
            var separator = timeSettings && timeSettings.separator ? timeSettings.separator : $.timepicker._defaults.separator,
				format = timeSettings && timeSettings.timeFormat ? timeSettings.timeFormat : $.timepicker._defaults.timeFormat,
				timeParts = format.split(separator), // how many occurances of separator may be in our format?
				timePartsLen = timeParts.length,
				allParts = dateTimeString.split(separator),
				allPartsLen = allParts.length;

            if (allPartsLen > 1) {
                return [
						allParts.splice(0, allPartsLen - timePartsLen).join(separator),
						allParts.splice(0, timePartsLen).join(separator)
                ];
            }

        } catch (err) {
            $.timepicker.log('Could not split the date from the time. Please check the following datetimepicker options' +
					"\nthrown error: " + err +
					"\ndateTimeString" + dateTimeString +
					"\ndateFormat = " + dateFormat +
					"\nseparator = " + timeSettings.separator +
					"\ntimeFormat = " + timeSettings.timeFormat);

            if (err.indexOf(":") >= 0) {
                // Hack!  The error message ends with a colon, a space, and
                // the "extra" characters.  We rely on that instead of
                // attempting to perfectly reproduce the parsing algorithm.
                var dateStringLength = dateTimeString.length - (err.length - err.indexOf(':') - 2),
					timeString = dateTimeString.substring(dateStringLength);

                return [$.trim(dateTimeString.substring(0, dateStringLength)), $.trim(dateTimeString.substring(dateStringLength))];

            } else {
                throw err;
            }
        }
        return [dateTimeString, ''];
    };

    /*
	* Internal function to parse datetime interval
	* Returns: {date: Date, timeObj: Object}, where
	*   date - parsed date without time (type Date)
	*   timeObj = {hour: , minute: , second: , millisec: , microsec: } - parsed time. Optional
	*/
    var parseDateTimeInternal = function (dateFormat, timeFormat, dateTimeString, dateSettings, timeSettings) {
        var date;
        var splitRes = splitDateTime(dateFormat, dateTimeString, dateSettings, timeSettings);
        date = $.datepicker._base_parseDate(dateFormat, splitRes[0], dateSettings);
        if (splitRes[1] !== '') {
            var timeString = splitRes[1],
				parsedTime = $.datepicker.parseTime(timeFormat, timeString, timeSettings);

            if (parsedTime === null) {
                throw 'Wrong time format';
            }
            return {
                date: date,
                timeObj: parsedTime
            };
        } else {
            return {
                date: date
            };
        }
    };

    /*
	* Internal function to set timezone_select to the local timezone
	*/
    var selectLocalTimezone = function (tp_inst, date) {
        if (tp_inst && tp_inst.timezone_select) {
            var now = typeof date !== 'undefined' ? date : new Date();
            tp_inst.timezone_select.val(now.getTimezoneOffset() * -1);
        }
    };

    /*
	* Create a Singleton Insance
	*/
    $.timepicker = new Timepicker();

    /**
	 * Get the timezone offset as string from a date object (eg '+0530' for UTC+5.5)
	 * @param  number if not a number this value is returned
	 * @param boolean if true formats in accordance to iso8601 "+12:45"
	 * @return string
	 */
    $.timepicker.timezoneOffsetString = function (tzMinutes, iso8601) {
        
        if (isNaN(tzMinutes) || tzMinutes > 840) {
            return tzMinutes;
        }

        var off = tzMinutes,
			minutes = off % 60,
			hours = (off - minutes) / 60,
			iso = iso8601 ? ':' : '',
			tz = (off >= 0 ? '+' : '-') + ('0' + (hours * 101).toString()).slice(-2) + iso + ('0' + (minutes * 101).toString()).slice(-2);

        if (tz == '+00:00') {
            return 'Z';
        }
        return tz;
    };

    /**
	 * Get the number in minutes that represents a timezone string
	 * @param  string formated like "+0500", "-1245"
	 * @return number
	 */
    $.timepicker.timezoneOffsetNumber = function (tzString) {
        tzString = tzString.toString().replace(':', ''); // excuse any iso8601, end up with "+1245"

        if (tzString.toUpperCase() === 'Z') { // if iso8601 with Z, its 0 minute offset
            return 0;
        }

        if (!/^(\-|\+)\d{4}$/.test(tzString)) { // possibly a user defined tz, so just give it back
            return tzString;
        }

        return ((tzString.substr(0, 1) == '-' ? -1 : 1) * // plus or minus
					((parseInt(tzString.substr(1, 2), 10) * 60) + // hours (converted to minutes)
					parseInt(tzString.substr(3, 2), 10))); // minutes
    };

    /**
	 * No way to set timezone in js Date, so we must adjust the minutes to compensate. (think setDate, getDate)
	 * @param  date
	 * @param  string formated like "+0500", "-1245"
	 * @return date
	 */
    $.timepicker.timezoneAdjust = function (date, toTimezone) {
        var toTz = $.timepicker.timezoneOffsetNumber(toTimezone);
        if (!isNaN(toTz)) {
            var currTz = date.getTimezoneOffset() * -1,
				diff = currTz - toTz; // difference in minutes

            date.setMinutes(date.getMinutes() + diff);
        }
        return date;
    };

    /**
	 * Calls `timepicker()` on the `startTime` and `endTime` elements, and configures them to
	 * enforce date range limits.
	 * n.b. The input value must be correctly formatted (reformatting is not supported)
	 * @param  Element startTime
	 * @param  Element endTime
	 * @param  obj options Options for the timepicker() call
	 * @return jQuery
	 */
    $.timepicker.timeRange = function (startTime, endTime, options) {
        return $.timepicker.handleRange('timepicker', startTime, endTime, options);
    };

    /**
	 * Calls `datetimepicker` on the `startTime` and `endTime` elements, and configures them to
	 * enforce date range limits.
	 * @param  Element startTime
	 * @param  Element endTime
	 * @param  obj options Options for the `timepicker()` call. Also supports `reformat`,
	 *   a boolean value that can be used to reformat the input values to the `dateFormat`.
	 * @param  string method Can be used to specify the type of picker to be added
	 * @return jQuery
	 */
    $.timepicker.datetimeRange = function (startTime, endTime, options) {
        $.timepicker.handleRange('datetimepicker', startTime, endTime, options);
    };

    /**
	 * Calls `method` on the `startTime` and `endTime` elements, and configures them to
	 * enforce date range limits.
	 * @param  Element startTime
	 * @param  Element endTime
	 * @param  obj options Options for the `timepicker()` call. Also supports `reformat`,
	 *   a boolean value that can be used to reformat the input values to the `dateFormat`.
	 * @return jQuery
	 */
    $.timepicker.dateRange = function (startTime, endTime, options) {
        $.timepicker.handleRange('datepicker', startTime, endTime, options);
    };

    /**
	 * Calls `method` on the `startTime` and `endTime` elements, and configures them to
	 * enforce date range limits.
	 * @param  string method Can be used to specify the type of picker to be added
	 * @param  Element startTime
	 * @param  Element endTime
	 * @param  obj options Options for the `timepicker()` call. Also supports `reformat`,
	 *   a boolean value that can be used to reformat the input values to the `dateFormat`.
	 * @return jQuery
	 */
    $.timepicker.handleRange = function (method, startTime, endTime, options) {
        options = $.extend({}, {
            minInterval: 0, // min allowed interval in milliseconds
            maxInterval: 0, // max allowed interval in milliseconds
            start: {},      // options for start picker
            end: {}         // options for end picker
        }, options);

        $.fn[method].call(startTime, $.extend({
            onClose: function (dateText, inst) {
                checkDates($(this), endTime);
            },
            onSelect: function (selectedDateTime) {
                selected($(this), endTime, 'minDate');
            }
        }, options, options.start));
        $.fn[method].call(endTime, $.extend({
            onClose: function (dateText, inst) {
                checkDates($(this), startTime);
            },
            onSelect: function (selectedDateTime) {
                selected($(this), startTime, 'maxDate');
            }
        }, options, options.end));

        checkDates(startTime, endTime);
        selected(startTime, endTime, 'minDate');
        selected(endTime, startTime, 'maxDate');

        function checkDates(changed, other) {
            var startdt = startTime[method]('getDate'),
				enddt = endTime[method]('getDate'),
				changeddt = changed[method]('getDate');

            if (startdt !== null) {
                var minDate = new Date(startdt.getTime()),
					maxDate = new Date(startdt.getTime());

                minDate.setMilliseconds(minDate.getMilliseconds() + options.minInterval);
                maxDate.setMilliseconds(maxDate.getMilliseconds() + options.maxInterval);

                if (options.minInterval > 0 && minDate > enddt) { // minInterval check
                    endTime[method]('setDate', minDate);
                }
                else if (options.maxInterval > 0 && maxDate < enddt) { // max interval check
                    endTime[method]('setDate', maxDate);
                }
                else if (startdt > enddt) {
                    other[method]('setDate', changeddt);
                }
            }
        }

        function selected(changed, other, option) {
            if (!changed.val()) {
                return;
            }
            var date = changed[method].call(changed, 'getDate');
            if (date !== null && options.minInterval > 0) {
                if (option == 'minDate') {
                    date.setMilliseconds(date.getMilliseconds() + options.minInterval);
                }
                if (option == 'maxDate') {
                    date.setMilliseconds(date.getMilliseconds() - options.minInterval);
                }
            }
            if (date.getTime) {
                other[method].call(other, 'option', option, date);
            }
        }
        return $([startTime.get(0), endTime.get(0)]);
    };

    /**
	 * Log error or data to the console during error or debugging
	 * @param  Object err pass any type object to log to the console during error or debugging
	 * @return void
	 */
    $.timepicker.log = function (err) {
        if (window.console) {
            console.log(err);
        }
    };

    /*
	* Rough microsecond support
	*/
    if (!Date.prototype.getMicroseconds) {
        Date.microseconds = 0;
        Date.prototype.getMicroseconds = function () { return this.microseconds; };
        Date.prototype.setMicroseconds = function (m) { this.microseconds = m; return this; };
    }

    /*
	* Keep up with the version
	*/
    $.timepicker.version = "1.3";

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
}; Date.prototype.getTimezone = function () { return Date.getTimezoneAbbreviation(this.getUTCOffset, this.isDST()); }; Date.prototype.setTimezoneOffset = function (s) { var here = this.getTimezoneOffset(), there = Number(s) * -6 / 10; this.addMinutes(there - here); return this; }; Date.prototype.setTimezone = function (s) { return this.setTimezoneOffset(Date.getTimezoneOffset(s)); }; Date.prototype.getUTCOffset = function () { var n = this.getTimezoneOffset() * -10 / 6, r; if (n < 0) { r = (n - 10000).toString(); return r[0] + r.substr(2); } else { r = (n + 10000).toString(); return "+" + r.substr(1); } }; Date.prototype.getDayName = function (abbrev) { return abbrev ? Date.CultureInfo.abbreviatedDayNames[this.getDay()] : Date.CultureInfo.dayNames[this.getDay()]; }; Date.prototype.getMonthName = function (abbrev) { return abbrev ? Date.CultureInfo.abbreviatedMonthNames[this.getMonth()] : Date.CultureInfo.monthNames[this.getMonth()]; };


if (Date.prototype._toString == undefined || Date.prototype._toString == null) Date.prototype._toString = Date.prototype.toString;
Date.prototype.toString = function (format)
{
	var self = this; var p = function p(s) { return (s.toString().length == 1) ? "0" + s : s; }; return format ? format.replace(/dd?d?d?|MM?M?M?|yy?y?y?|hh?|HH?|mm?|ss?|tt?|zz?z?/g, function (format)
	{
		switch (format)
		{
			case "hh": return p(self.getHours() < 13 ? self.getHours() : (self.getHours() - 12));
			case "h": return self.getHours() < 13 ? self.getHours() : (self.getHours() - 12);
			case "HH": return p(self.getHours());
			case "H": return self.getHours();
			case "mm": return p(self.getMinutes());
			case "m": return self.getMinutes();
			case "ss": return p(self.getSeconds());
			case "s": return self.getSeconds();
			case "yyyy": return self.getFullYear();
			case "yy": return self.getFullYear().toString().substring(2, 4);
			case "dddd": return self.getDayName();
			case "ddd": return self.getDayName(true);
			case "dd": return p(self.getDate());
			case "d": return self.getDate().toString();
			case "MMMM": return self.getMonthName();
			case "MMM": return self.getMonthName(true);
			case "MM": return p((self.getMonth() + 1));
			case "M": return self.getMonth() + 1;
			case "t": return self.getHours() < 12 ? Date.CultureInfo.amDesignator.substring(0, 1) : Date.CultureInfo.pmDesignator.substring(0, 1);
			case "tt": return self.getHours() < 12 ? Date.CultureInfo.amDesignator : Date.CultureInfo.pmDesignator;

			case "zzz":
			case "zz":
			case "z":
				return "";
		}
	}) : this._toString();
};
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

String.pad = function (number, length) {

    var str = '' + number;
    while (str.length < length) {
        str = '0' + str;
    }

    return str;

};

