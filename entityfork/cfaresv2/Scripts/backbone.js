﻿// Underscore.js 1.3.3
// (c) 2009-2012 Jeremy Ashkenas, DocumentCloud Inc.
// Underscore is freely distributable under the MIT license.
// Portions of Underscore are inspired or borrowed from Prototype,
// Oliver Steele's Functional, and John Resig's Micro-Templating.
// For all details and documentation:
// http://documentcloud.github.com/underscore
(function () {
    function r(a, c, d) {
        if (a === c) return 0 !== a || 1 / a == 1 / c; if (null == a || null == c) return a === c; a._chain && (a = a._wrapped); c._chain && (c = c._wrapped); if (a.isEqual && b.isFunction(a.isEqual)) return a.isEqual(c); if (c.isEqual && b.isFunction(c.isEqual)) return c.isEqual(a); var e = l.call(a); if (e != l.call(c)) return !1; switch (e) {
            case "[object String]": return a == "" + c; case "[object Number]": return a != +a ? c != +c : 0 == a ? 1 / a == 1 / c : a == +c; case "[object Date]": case "[object Boolean]": return +a == +c; case "[object RegExp]": return a.source ==
c.source && a.global == c.global && a.multiline == c.multiline && a.ignoreCase == c.ignoreCase
        } if ("object" != typeof a || "object" != typeof c) return !1; for (var f = d.length; f--; ) if (d[f] == a) return !0; d.push(a); var f = 0, g = !0; if ("[object Array]" == e) { if (f = a.length, g = f == c.length) for (; f-- && (g = f in a == f in c && r(a[f], c[f], d)); ); } else {
            if ("constructor" in a != "constructor" in c || a.constructor != c.constructor) return !1; for (var h in a) if (b.has(a, h) && (f++, !(g = b.has(c, h) && r(a[h], c[h], d)))) break; if (g) {
                for (h in c) if (b.has(c, h) && !f--) break;
                g = !f
            } 
        } d.pop(); return g
    } var s = this, I = s._, o = {}, k = Array.prototype, p = Object.prototype, i = k.slice, J = k.unshift, l = p.toString, K = p.hasOwnProperty, y = k.forEach, z = k.map, A = k.reduce, B = k.reduceRight, C = k.filter, D = k.every, E = k.some, q = k.indexOf, F = k.lastIndexOf, p = Array.isArray, L = Object.keys, t = Function.prototype.bind, b = function (a) { return new m(a) }; "undefined" !== typeof exports ? ("undefined" !== typeof module && module.exports && (exports = module.exports = b), exports._ = b) : s._ = b; b.VERSION = "1.3.3"; var j = b.each = b.forEach = function (a,
c, d) { if (a != null) if (y && a.forEach === y) a.forEach(c, d); else if (a.length === +a.length) for (var e = 0, f = a.length; e < f; e++) { if (e in a && c.call(d, a[e], e, a) === o) break } else for (e in a) if (b.has(a, e) && c.call(d, a[e], e, a) === o) break }; b.map = b.collect = function (a, c, b) { var e = []; if (a == null) return e; if (z && a.map === z) return a.map(c, b); j(a, function (a, g, h) { e[e.length] = c.call(b, a, g, h) }); if (a.length === +a.length) e.length = a.length; return e }; b.reduce = b.foldl = b.inject = function (a, c, d, e) {
    var f = arguments.length > 2; a == null && (a = []); if (A &&
a.reduce === A) { e && (c = b.bind(c, e)); return f ? a.reduce(c, d) : a.reduce(c) } j(a, function (a, b, i) { if (f) d = c.call(e, d, a, b, i); else { d = a; f = true } }); if (!f) throw new TypeError("Reduce of empty array with no initial value"); return d
}; b.reduceRight = b.foldr = function (a, c, d, e) { var f = arguments.length > 2; a == null && (a = []); if (B && a.reduceRight === B) { e && (c = b.bind(c, e)); return f ? a.reduceRight(c, d) : a.reduceRight(c) } var g = b.toArray(a).reverse(); e && !f && (c = b.bind(c, e)); return f ? b.reduce(g, c, d, e) : b.reduce(g, c) }; b.find = b.detect = function (a,
c, b) { var e; G(a, function (a, g, h) { if (c.call(b, a, g, h)) { e = a; return true } }); return e }; b.filter = b.select = function (a, c, b) { var e = []; if (a == null) return e; if (C && a.filter === C) return a.filter(c, b); j(a, function (a, g, h) { c.call(b, a, g, h) && (e[e.length] = a) }); return e }; b.reject = function (a, c, b) { var e = []; if (a == null) return e; j(a, function (a, g, h) { c.call(b, a, g, h) || (e[e.length] = a) }); return e }; b.every = b.all = function (a, c, b) {
    var e = true; if (a == null) return e; if (D && a.every === D) return a.every(c, b); j(a, function (a, g, h) {
        if (!(e = e && c.call(b,
a, g, h))) return o
    }); return !!e
}; var G = b.some = b.any = function (a, c, d) { c || (c = b.identity); var e = false; if (a == null) return e; if (E && a.some === E) return a.some(c, d); j(a, function (a, b, h) { if (e || (e = c.call(d, a, b, h))) return o }); return !!e }; b.include = b.contains = function (a, c) { var b = false; if (a == null) return b; if (q && a.indexOf === q) return a.indexOf(c) != -1; return b = G(a, function (a) { return a === c }) }; b.invoke = function (a, c) { var d = i.call(arguments, 2); return b.map(a, function (a) { return (b.isFunction(c) ? c || a : a[c]).apply(a, d) }) }; b.pluck =
function (a, c) { return b.map(a, function (a) { return a[c] }) }; b.max = function (a, c, d) { if (!c && b.isArray(a) && a[0] === +a[0]) return Math.max.apply(Math, a); if (!c && b.isEmpty(a)) return -Infinity; var e = { computed: -Infinity }; j(a, function (a, b, h) { b = c ? c.call(d, a, b, h) : a; b >= e.computed && (e = { value: a, computed: b }) }); return e.value }; b.min = function (a, c, d) {
    if (!c && b.isArray(a) && a[0] === +a[0]) return Math.min.apply(Math, a); if (!c && b.isEmpty(a)) return Infinity; var e = { computed: Infinity }; j(a, function (a, b, h) {
        b = c ? c.call(d, a, b, h) : a; b < e.computed &&
(e = { value: a, computed: b })
    }); return e.value
}; b.shuffle = function (a) { var b = [], d; j(a, function (a, f) { d = Math.floor(Math.random() * (f + 1)); b[f] = b[d]; b[d] = a }); return b }; b.sortBy = function (a, c, d) { var e = b.isFunction(c) ? c : function (a) { return a[c] }; return b.pluck(b.map(a, function (a, b, c) { return { value: a, criteria: e.call(d, a, b, c)} }).sort(function (a, b) { var c = a.criteria, d = b.criteria; return c === void 0 ? 1 : d === void 0 ? -1 : c < d ? -1 : c > d ? 1 : 0 }), "value") }; b.groupBy = function (a, c) {
    var d = {}, e = b.isFunction(c) ? c : function (a) { return a[c] };
    j(a, function (a, b) { var c = e(a, b); (d[c] || (d[c] = [])).push(a) }); return d
}; b.sortedIndex = function (a, c, d) { d || (d = b.identity); for (var e = 0, f = a.length; e < f; ) { var g = e + f >> 1; d(a[g]) < d(c) ? e = g + 1 : f = g } return e }; b.toArray = function (a) { return !a ? [] : b.isArray(a) || b.isArguments(a) ? i.call(a) : a.toArray && b.isFunction(a.toArray) ? a.toArray() : b.values(a) }; b.size = function (a) { return b.isArray(a) ? a.length : b.keys(a).length }; b.first = b.head = b.take = function (a, b, d) { return b != null && !d ? i.call(a, 0, b) : a[0] }; b.initial = function (a, b, d) {
    return i.call(a,
0, a.length - (b == null || d ? 1 : b))
}; b.last = function (a, b, d) { return b != null && !d ? i.call(a, Math.max(a.length - b, 0)) : a[a.length - 1] }; b.rest = b.tail = function (a, b, d) { return i.call(a, b == null || d ? 1 : b) }; b.compact = function (a) { return b.filter(a, function (a) { return !!a }) }; b.flatten = function (a, c) { return b.reduce(a, function (a, e) { if (b.isArray(e)) return a.concat(c ? e : b.flatten(e)); a[a.length] = e; return a }, []) }; b.without = function (a) { return b.difference(a, i.call(arguments, 1)) }; b.uniq = b.unique = function (a, c, d) {
    var d = d ? b.map(a, d) : a,
e = []; a.length < 3 && (c = true); b.reduce(d, function (d, g, h) { if (c ? b.last(d) !== g || !d.length : !b.include(d, g)) { d.push(g); e.push(a[h]) } return d }, []); return e
}; b.union = function () { return b.uniq(b.flatten(arguments, true)) }; b.intersection = b.intersect = function (a) { var c = i.call(arguments, 1); return b.filter(b.uniq(a), function (a) { return b.every(c, function (c) { return b.indexOf(c, a) >= 0 }) }) }; b.difference = function (a) { var c = b.flatten(i.call(arguments, 1), true); return b.filter(a, function (a) { return !b.include(c, a) }) }; b.zip = function () {
    for (var a =
i.call(arguments), c = b.max(b.pluck(a, "length")), d = Array(c), e = 0; e < c; e++) d[e] = b.pluck(a, "" + e); return d
}; b.indexOf = function (a, c, d) { if (a == null) return -1; var e; if (d) { d = b.sortedIndex(a, c); return a[d] === c ? d : -1 } if (q && a.indexOf === q) return a.indexOf(c); d = 0; for (e = a.length; d < e; d++) if (d in a && a[d] === c) return d; return -1 }; b.lastIndexOf = function (a, b) { if (a == null) return -1; if (F && a.lastIndexOf === F) return a.lastIndexOf(b); for (var d = a.length; d--; ) if (d in a && a[d] === b) return d; return -1 }; b.range = function (a, b, d) {
    if (arguments.length <=
1) { b = a || 0; a = 0 } for (var d = arguments[2] || 1, e = Math.max(Math.ceil((b - a) / d), 0), f = 0, g = Array(e); f < e; ) { g[f++] = a; a = a + d } return g
}; var H = function () { }; b.bind = function (a, c) { var d, e; if (a.bind === t && t) return t.apply(a, i.call(arguments, 1)); if (!b.isFunction(a)) throw new TypeError; e = i.call(arguments, 2); return d = function () { if (!(this instanceof d)) return a.apply(c, e.concat(i.call(arguments))); H.prototype = a.prototype; var b = new H, g = a.apply(b, e.concat(i.call(arguments))); return Object(g) === g ? g : b } }; b.bindAll = function (a) {
    var c =
i.call(arguments, 1); c.length == 0 && (c = b.functions(a)); j(c, function (c) { a[c] = b.bind(a[c], a) }); return a
}; b.memoize = function (a, c) { var d = {}; c || (c = b.identity); return function () { var e = c.apply(this, arguments); return b.has(d, e) ? d[e] : d[e] = a.apply(this, arguments) } }; b.delay = function (a, b) { var d = i.call(arguments, 2); return setTimeout(function () { return a.apply(null, d) }, b) }; b.defer = function (a) { return b.delay.apply(b, [a, 1].concat(i.call(arguments, 1))) }; b.throttle = function (a, c) {
    var d, e, f, g, h, i, j = b.debounce(function () {
        h =
g = false
    }, c); return function () { d = this; e = arguments; f || (f = setTimeout(function () { f = null; h && a.apply(d, e); j() }, c)); g ? h = true : i = a.apply(d, e); j(); g = true; return i } 
}; b.debounce = function (a, b, d) { var e; return function () { var f = this, g = arguments; d && !e && a.apply(f, g); clearTimeout(e); e = setTimeout(function () { e = null; d || a.apply(f, g) }, b) } }; b.once = function (a) { var b = false, d; return function () { if (b) return d; b = true; return d = a.apply(this, arguments) } }; b.wrap = function (a, b) {
    return function () {
        var d = [a].concat(i.call(arguments, 0));
        return b.apply(this, d)
    } 
}; b.compose = function () { var a = arguments; return function () { for (var b = arguments, d = a.length - 1; d >= 0; d--) b = [a[d].apply(this, b)]; return b[0] } }; b.after = function (a, b) { return a <= 0 ? b() : function () { if (--a < 1) return b.apply(this, arguments) } }; b.keys = L || function (a) { if (a !== Object(a)) throw new TypeError("Invalid object"); var c = [], d; for (d in a) b.has(a, d) && (c[c.length] = d); return c }; b.values = function (a) { return b.map(a, b.identity) }; b.functions = b.methods = function (a) {
    var c = [], d; for (d in a) b.isFunction(a[d]) &&
c.push(d); return c.sort()
}; b.extend = function (a) { j(i.call(arguments, 1), function (b) { for (var d in b) a[d] = b[d] }); return a }; b.pick = function (a) { var c = {}; j(b.flatten(i.call(arguments, 1)), function (b) { b in a && (c[b] = a[b]) }); return c }; b.defaults = function (a) { j(i.call(arguments, 1), function (b) { for (var d in b) a[d] == null && (a[d] = b[d]) }); return a }; b.clone = function (a) { return !b.isObject(a) ? a : b.isArray(a) ? a.slice() : b.extend({}, a) }; b.tap = function (a, b) { b(a); return a }; b.isEqual = function (a, b) { return r(a, b, []) }; b.isEmpty =
function (a) { if (a == null) return true; if (b.isArray(a) || b.isString(a)) return a.length === 0; for (var c in a) if (b.has(a, c)) return false; return true }; b.isElement = function (a) { return !!(a && a.nodeType == 1) }; b.isArray = p || function (a) { return l.call(a) == "[object Array]" }; b.isObject = function (a) { return a === Object(a) }; b.isArguments = function (a) { return l.call(a) == "[object Arguments]" }; b.isArguments(arguments) || (b.isArguments = function (a) { return !(!a || !b.has(a, "callee")) }); b.isFunction = function (a) { return l.call(a) == "[object Function]" };
    b.isString = function (a) { return l.call(a) == "[object String]" }; b.isNumber = function (a) { return l.call(a) == "[object Number]" }; b.isFinite = function (a) { return b.isNumber(a) && isFinite(a) }; b.isNaN = function (a) { return a !== a }; b.isBoolean = function (a) { return a === true || a === false || l.call(a) == "[object Boolean]" }; b.isDate = function (a) { return l.call(a) == "[object Date]" }; b.isRegExp = function (a) { return l.call(a) == "[object RegExp]" }; b.isNull = function (a) { return a === null }; b.isUndefined = function (a) { return a === void 0 }; b.has = function (a,
b) { return K.call(a, b) }; b.noConflict = function () { s._ = I; return this }; b.identity = function (a) { return a }; b.times = function (a, b, d) { for (var e = 0; e < a; e++) b.call(d, e) }; b.escape = function (a) { return ("" + a).replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;").replace(/"/g, "&quot;").replace(/'/g, "&#x27;").replace(/\//g, "&#x2F;") }; b.result = function (a, c) { if (a == null) return null; var d = a[c]; return b.isFunction(d) ? d.call(a) : d }; b.mixin = function (a) { j(b.functions(a), function (c) { M(c, b[c] = a[c]) }) }; var N = 0; b.uniqueId =
function (a) { var b = N++; return a ? a + b : b }; b.templateSettings = { evaluate: /<%([\s\S]+?)%>/g, interpolate: /<%=([\s\S]+?)%>/g, escape: /<%-([\s\S]+?)%>/g }; var u = /.^/, n = { "\\": "\\", "'": "'", r: "\r", n: "\n", t: "\t", u2028: "\u2028", u2029: "\u2029" }, v; for (v in n) n[n[v]] = v; var O = /\\|'|\r|\n|\t|\u2028|\u2029/g, P = /\\(\\|'|r|n|t|u2028|u2029)/g, w = function (a) { return a.replace(P, function (a, b) { return n[b] }) }; b.template = function (a, c, d) {
    d = b.defaults(d || {}, b.templateSettings); a = "__p+='" + a.replace(O, function (a) { return "\\" + n[a] }).replace(d.escape ||
u, function (a, b) { return "'+\n_.escape(" + w(b) + ")+\n'" }).replace(d.interpolate || u, function (a, b) { return "'+\n(" + w(b) + ")+\n'" }).replace(d.evaluate || u, function (a, b) { return "';\n" + w(b) + "\n;__p+='" }) + "';\n"; d.variable || (a = "with(obj||{}){\n" + a + "}\n"); var a = "var __p='';var print=function(){__p+=Array.prototype.join.call(arguments, '')};\n" + a + "return __p;\n", e = new Function(d.variable || "obj", "_", a); if (c) return e(c, b); c = function (a) { return e.call(this, a, b) }; c.source = "function(" + (d.variable || "obj") + "){\n" + a + "}"; return c
};
    b.chain = function (a) { return b(a).chain() }; var m = function (a) { this._wrapped = a }; b.prototype = m.prototype; var x = function (a, c) { return c ? b(a).chain() : a }, M = function (a, c) { m.prototype[a] = function () { var a = i.call(arguments); J.call(a, this._wrapped); return x(c.apply(b, a), this._chain) } }; b.mixin(b); j("pop,push,reverse,shift,sort,splice,unshift".split(","), function (a) {
        var b = k[a]; m.prototype[a] = function () {
            var d = this._wrapped; b.apply(d, arguments); var e = d.length; (a == "shift" || a == "splice") && e === 0 && delete d[0]; return x(d,
this._chain)
        } 
    }); j(["concat", "join", "slice"], function (a) { var b = k[a]; m.prototype[a] = function () { return x(b.apply(this._wrapped, arguments), this._chain) } }); m.prototype.chain = function () { this._chain = true; return this }; m.prototype.value = function () { return this._wrapped } 
}).call(this);


// Backbone.js 0.9.2

// (c) 2010-2012 Jeremy Ashkenas, DocumentCloud Inc.
// Backbone may be freely distributed under the MIT license.
// For all details and documentation:
// http://backbonejs.org
(function () {
    var l = this, y = l.Backbone, z = Array.prototype.slice, A = Array.prototype.splice, g; g = "undefined" !== typeof exports ? exports : l.Backbone = {}; g.VERSION = "0.9.2"; var f = l._; !f && "undefined" !== typeof require && (f = require("underscore")); var i = l.jQuery || l.Zepto || l.ender; g.setDomLibrary = function (a) { i = a }; g.noConflict = function () { l.Backbone = y; return this }; g.emulateHTTP = !1; g.emulateJSON = !1; var p = /\s+/, k = g.Events = { on: function (a, b, c) {
        var d, e, f, g, j; if (!b) return this; a = a.split(p); for (d = this._callbacks || (this._callbacks =
{}); e = a.shift(); ) f = (j = d[e]) ? j.tail : {}, f.next = g = {}, f.context = c, f.callback = b, d[e] = { tail: g, next: j ? j.next : f }; return this
    }, off: function (a, b, c) { var d, e, h, g, j, q; if (e = this._callbacks) { if (!a && !b && !c) return delete this._callbacks, this; for (a = a ? a.split(p) : f.keys(e); d = a.shift(); ) if (h = e[d], delete e[d], h && (b || c)) for (g = h.tail; (h = h.next) !== g; ) if (j = h.callback, q = h.context, b && j !== b || c && q !== c) this.on(d, j, q); return this } }, trigger: function (a) {
        var b, c, d, e, f, g; if (!(d = this._callbacks)) return this; f = d.all; a = a.split(p); for (g =
z.call(arguments, 1); b = a.shift(); ) { if (c = d[b]) for (e = c.tail; (c = c.next) !== e; ) c.callback.apply(c.context || this, g); if (c = f) { e = c.tail; for (b = [b].concat(g); (c = c.next) !== e; ) c.callback.apply(c.context || this, b) } } return this
    } 
    }; k.bind = k.on; k.unbind = k.off; var o = g.Model = function (a, b) {
        var c; a || (a = {}); b && b.parse && (a = this.parse(a)); if (c = n(this, "defaults")) a = f.extend({}, c, a); b && b.collection && (this.collection = b.collection); this.attributes = {}; this._escapedAttributes = {}; this.cid = f.uniqueId("c"); this.changed = {}; this._silent =
{}; this._pending = {}; this.set(a, { silent: !0 }); this.changed = {}; this._silent = {}; this._pending = {}; this._previousAttributes = f.clone(this.attributes); this.initialize.apply(this, arguments)
    }; f.extend(o.prototype, k, { changed: null, _silent: null, _pending: null, idAttribute: "id", initialize: function () { }, toJSON: function () { return f.clone(this.attributes) }, get: function (a) { return this.attributes[a] }, escape: function (a) {
        var b; if (b = this._escapedAttributes[a]) return b; b = this.get(a); return this._escapedAttributes[a] = f.escape(null ==
b ? "" : "" + b)
    }, has: function (a) { return null != this.get(a) }, set: function (a, b, c) {
        var d, e; f.isObject(a) || null == a ? (d = a, c = b) : (d = {}, d[a] = b); c || (c = {}); if (!d) return this; d instanceof o && (d = d.attributes); if (c.unset) for (e in d) d[e] = void 0; if (!this._validate(d, c)) return !1; this.idAttribute in d && (this.id = d[this.idAttribute]); var b = c.changes = {}, h = this.attributes, g = this._escapedAttributes, j = this._previousAttributes || {}; for (e in d) {
            a = d[e]; if (!f.isEqual(h[e], a) || c.unset && f.has(h, e)) delete g[e], (c.silent ? this._silent :
b)[e] = !0; c.unset ? delete h[e] : h[e] = a; !f.isEqual(j[e], a) || f.has(h, e) != f.has(j, e) ? (this.changed[e] = a, c.silent || (this._pending[e] = !0)) : (delete this.changed[e], delete this._pending[e])
        } c.silent || this.change(c); return this
    }, unset: function (a, b) { (b || (b = {})).unset = !0; return this.set(a, null, b) }, clear: function (a) { (a || (a = {})).unset = !0; return this.set(f.clone(this.attributes), a) }, fetch: function (a) {
        var a = a ? f.clone(a) : {}, b = this, c = a.success; a.success = function (d, e, f) { if (!b.set(b.parse(d, f), a)) return !1; c && c(b, d) };
        a.error = g.wrapError(a.error, b, a); return (this.sync || g.sync).call(this, "read", this, a)
    }, save: function (a, b, c) {
        var d, e; f.isObject(a) || null == a ? (d = a, c = b) : (d = {}, d[a] = b); c = c ? f.clone(c) : {}; if (c.wait) { if (!this._validate(d, c)) return !1; e = f.clone(this.attributes) } a = f.extend({}, c, { silent: !0 }); if (d && !this.set(d, c.wait ? a : c)) return !1; var h = this, i = c.success; c.success = function (a, b, e) { b = h.parse(a, e); if (c.wait) { delete c.wait; b = f.extend(d || {}, b) } if (!h.set(b, c)) return false; i ? i(h, a) : h.trigger("sync", h, a, c) }; c.error = g.wrapError(c.error,
h, c); b = this.isNew() ? "create" : "update"; b = (this.sync || g.sync).call(this, b, this, c); c.wait && this.set(e, a); return b
    }, destroy: function (a) { var a = a ? f.clone(a) : {}, b = this, c = a.success, d = function () { b.trigger("destroy", b, b.collection, a) }; if (this.isNew()) return d(), !1; a.success = function (e) { a.wait && d(); c ? c(b, e) : b.trigger("sync", b, e, a) }; a.error = g.wrapError(a.error, b, a); var e = (this.sync || g.sync).call(this, "delete", this, a); a.wait || d(); return e }, url: function () {
        var a = n(this, "urlRoot") || n(this.collection, "url") || t();
        return this.isNew() ? a : a + ("/" == a.charAt(a.length - 1) ? "" : "/") + encodeURIComponent(this.id)
    }, parse: function (a) { return a }, clone: function () { return new this.constructor(this.attributes) }, isNew: function () { return null == this.id }, change: function (a) {
        a || (a = {}); var b = this._changing; this._changing = !0; for (var c in this._silent) this._pending[c] = !0; var d = f.extend({}, a.changes, this._silent); this._silent = {}; for (c in d) this.trigger("change:" + c, this, this.get(c), a); if (b) return this; for (; !f.isEmpty(this._pending); ) {
            this._pending =
{}; this.trigger("change", this, a); for (c in this.changed) !this._pending[c] && !this._silent[c] && delete this.changed[c]; this._previousAttributes = f.clone(this.attributes)
        } this._changing = !1; return this
    }, hasChanged: function (a) { return !arguments.length ? !f.isEmpty(this.changed) : f.has(this.changed, a) }, changedAttributes: function (a) { if (!a) return this.hasChanged() ? f.clone(this.changed) : !1; var b, c = !1, d = this._previousAttributes, e; for (e in a) if (!f.isEqual(d[e], b = a[e])) (c || (c = {}))[e] = b; return c }, previous: function (a) {
        return !arguments.length ||
!this._previousAttributes ? null : this._previousAttributes[a]
    }, previousAttributes: function () { return f.clone(this._previousAttributes) }, isValid: function () { return !this.validate(this.attributes) }, _validate: function (a, b) { if (b.silent || !this.validate) return !0; var a = f.extend({}, this.attributes, a), c = this.validate(a, b); if (!c) return !0; b && b.error ? b.error(this, c, b) : this.trigger("error", this, c, b); return !1 } 
    }); var r = g.Collection = function (a, b) {
        b || (b = {}); b.model && (this.model = b.model); b.comparator && (this.comparator = b.comparator);
        this._reset(); this.initialize.apply(this, arguments); a && this.reset(a, { silent: !0, parse: b.parse })
    }; f.extend(r.prototype, k, { model: o, initialize: function () { }, toJSON: function (a) { return this.map(function (b) { return b.toJSON(a) }) }, add: function (a, b) {
        var c, d, e, g, i, j = {}, k = {}, l = []; b || (b = {}); a = f.isArray(a) ? a.slice() : [a]; c = 0; for (d = a.length; c < d; c++) {
            if (!(e = a[c] = this._prepareModel(a[c], b))) throw Error("Can't add an invalid model to a collection"); g = e.cid; i = e.id; j[g] || this._byCid[g] || null != i && (k[i] || this._byId[i]) ?
l.push(c) : j[g] = k[i] = e
        } for (c = l.length; c--; ) a.splice(l[c], 1); c = 0; for (d = a.length; c < d; c++) (e = a[c]).on("all", this._onModelEvent, this), this._byCid[e.cid] = e, null != e.id && (this._byId[e.id] = e); this.length += d; A.apply(this.models, [null != b.at ? b.at : this.models.length, 0].concat(a)); this.comparator && this.sort({ silent: !0 }); if (b.silent) return this; c = 0; for (d = this.models.length; c < d; c++) if (j[(e = this.models[c]).cid]) b.index = c, e.trigger("add", e, this, b); return this
    }, remove: function (a, b) {
        var c, d, e, g; b || (b = {}); a = f.isArray(a) ?
a.slice() : [a]; c = 0; for (d = a.length; c < d; c++) if (g = this.getByCid(a[c]) || this.get(a[c])) delete this._byId[g.id], delete this._byCid[g.cid], e = this.indexOf(g), this.models.splice(e, 1), this.length--, b.silent || (b.index = e, g.trigger("remove", g, this, b)), this._removeReference(g); return this
    }, push: function (a, b) { a = this._prepareModel(a, b); this.add(a, b); return a }, pop: function (a) { var b = this.at(this.length - 1); this.remove(b, a); return b }, unshift: function (a, b) { a = this._prepareModel(a, b); this.add(a, f.extend({ at: 0 }, b)); return a },
        shift: function (a) { var b = this.at(0); this.remove(b, a); return b }, get: function (a) { return null == a ? void 0 : this._byId[null != a.id ? a.id : a] }, getByCid: function (a) { return a && this._byCid[a.cid || a] }, at: function (a) { return this.models[a] }, where: function (a) { return f.isEmpty(a) ? [] : this.filter(function (b) { for (var c in a) if (a[c] !== b.get(c)) return !1; return !0 }) }, sort: function (a) {
            a || (a = {}); if (!this.comparator) throw Error("Cannot sort a set without a comparator"); var b = f.bind(this.comparator, this); 1 == this.comparator.length ?
this.models = this.sortBy(b) : this.models.sort(b); a.silent || this.trigger("reset", this, a); return this
        }, pluck: function (a) { return f.map(this.models, function (b) { return b.get(a) }) }, reset: function (a, b) { a || (a = []); b || (b = {}); for (var c = 0, d = this.models.length; c < d; c++) this._removeReference(this.models[c]); this._reset(); this.add(a, f.extend({ silent: !0 }, b)); b.silent || this.trigger("reset", this, b); return this }, fetch: function (a) {
            a = a ? f.clone(a) : {}; void 0 === a.parse && (a.parse = !0); var b = this, c = a.success; a.success = function (d,
e, f) { b[a.add ? "add" : "reset"](b.parse(d, f), a); c && c(b, d) }; a.error = g.wrapError(a.error, b, a); return (this.sync || g.sync).call(this, "read", this, a)
        }, create: function (a, b) { var c = this, b = b ? f.clone(b) : {}, a = this._prepareModel(a, b); if (!a) return !1; b.wait || c.add(a, b); var d = b.success; b.success = function (e, f) { b.wait && c.add(e, b); d ? d(e, f) : e.trigger("sync", a, f, b) }; a.save(null, b); return a }, parse: function (a) { return a }, chain: function () { return f(this.models).chain() }, _reset: function () {
            this.length = 0; this.models = []; this._byId =
{}; this._byCid = {}
        }, _prepareModel: function (a, b) { b || (b = {}); a instanceof o ? a.collection || (a.collection = this) : (b.collection = this, a = new this.model(a, b), a._validate(a.attributes, b) || (a = !1)); return a }, _removeReference: function (a) { this == a.collection && delete a.collection; a.off("all", this._onModelEvent, this) }, _onModelEvent: function (a, b, c, d) {
            ("add" == a || "remove" == a) && c != this || ("destroy" == a && this.remove(b, d), b && a === "change:" + b.idAttribute && (delete this._byId[b.previous(b.idAttribute)], this._byId[b.id] = b), this.trigger.apply(this,
arguments))
        } 
    }); f.each("forEach,each,map,reduce,reduceRight,find,detect,filter,select,reject,every,all,some,any,include,contains,invoke,max,min,sortBy,sortedIndex,toArray,size,first,initial,rest,last,without,indexOf,shuffle,lastIndexOf,isEmpty,groupBy".split(","), function (a) { r.prototype[a] = function () { return f[a].apply(f, [this.models].concat(f.toArray(arguments))) } }); var u = g.Router = function (a) { a || (a = {}); a.routes && (this.routes = a.routes); this._bindRoutes(); this.initialize.apply(this, arguments) }, B = /:\w+/g,
C = /\*\w+/g, D = /[-[\]{}()+?.,\\^$|#\s]/g; f.extend(u.prototype, k, { initialize: function () { }, route: function (a, b, c) { g.history || (g.history = new m); f.isRegExp(a) || (a = this._routeToRegExp(a)); c || (c = this[b]); g.history.route(a, f.bind(function (d) { d = this._extractParameters(a, d); c && c.apply(this, d); this.trigger.apply(this, ["route:" + b].concat(d)); g.history.trigger("route", this, b, d) }, this)); return this }, navigate: function (a, b) { g.history.navigate(a, b) }, _bindRoutes: function () {
    if (this.routes) {
        var a = [], b; for (b in this.routes) a.unshift([b,
this.routes[b]]); b = 0; for (var c = a.length; b < c; b++) this.route(a[b][0], a[b][1], this[a[b][1]])
    } 
}, _routeToRegExp: function (a) { a = a.replace(D, "\\$&").replace(B, "([^/]+)").replace(C, "(.*?)"); return RegExp("^" + a + "$") }, _extractParameters: function (a, b) { return a.exec(b).slice(1) } 
}); var m = g.History = function () { this.handlers = []; f.bindAll(this, "checkUrl") }, s = /^[#\/]/, E = /msie [\w.]+/; m.started = !1; f.extend(m.prototype, k, { interval: 50, getHash: function (a) {
    return (a = (a ? a.location : window.location).href.match(/#(.*)$/)) ? a[1] :
""
}, getFragment: function (a, b) { if (null == a) if (this._hasPushState || b) { var a = window.location.pathname, c = window.location.search; c && (a += c) } else a = this.getHash(); a.indexOf(this.options.root) || (a = a.substr(this.options.root.length)); return a.replace(s, "") }, start: function (a) {
    if (m.started) throw Error("Backbone.history has already been started"); m.started = !0; this.options = f.extend({}, { root: "/" }, this.options, a); this._wantsHashChange = !1 !== this.options.hashChange; this._wantsPushState = !!this.options.pushState; this._hasPushState =
!(!this.options.pushState || !window.history || !window.history.pushState); var a = this.getFragment(), b = document.documentMode; if (b = E.exec(navigator.userAgent.toLowerCase()) && (!b || 7 >= b)) this.iframe = i('<iframe src="javascript:0" tabindex="-1" />').hide().appendTo("body")[0].contentWindow, this.navigate(a); this._hasPushState ? i(window).bind("popstate", this.checkUrl) : this._wantsHashChange && "onhashchange" in window && !b ? i(window).bind("hashchange", this.checkUrl) : this._wantsHashChange && (this._checkUrlInterval = setInterval(this.checkUrl,
this.interval)); this.fragment = a; a = window.location; b = a.pathname == this.options.root; if (this._wantsHashChange && this._wantsPushState && !this._hasPushState && !b) return this.fragment = this.getFragment(null, !0), window.location.replace(this.options.root + "#" + this.fragment), !0; this._wantsPushState && this._hasPushState && b && a.hash && (this.fragment = this.getHash().replace(s, ""), window.history.replaceState({}, document.title, a.protocol + "//" + a.host + this.options.root + this.fragment)); if (!this.options.silent) return this.loadUrl()
},
    stop: function () { i(window).unbind("popstate", this.checkUrl).unbind("hashchange", this.checkUrl); clearInterval(this._checkUrlInterval); m.started = !1 }, route: function (a, b) { this.handlers.unshift({ route: a, callback: b }) }, checkUrl: function () { var a = this.getFragment(); a == this.fragment && this.iframe && (a = this.getFragment(this.getHash(this.iframe))); if (a == this.fragment) return !1; this.iframe && this.navigate(a); this.loadUrl() || this.loadUrl(this.getHash()) }, loadUrl: function (a) {
        var b = this.fragment = this.getFragment(a); return f.any(this.handlers,
function (a) { if (a.route.test(b)) return a.callback(b), !0 })
    }, navigate: function (a, b) {
        if (!m.started) return !1; if (!b || !0 === b) b = { trigger: b }; var c = (a || "").replace(s, ""); this.fragment != c && (this._hasPushState ? (0 != c.indexOf(this.options.root) && (c = this.options.root + c), this.fragment = c, window.history[b.replace ? "replaceState" : "pushState"]({}, document.title, c)) : this._wantsHashChange ? (this.fragment = c, this._updateHash(window.location, c, b.replace), this.iframe && c != this.getFragment(this.getHash(this.iframe)) && (b.replace ||
this.iframe.document.open().close(), this._updateHash(this.iframe.location, c, b.replace))) : window.location.assign(this.options.root + a), b.trigger && this.loadUrl(a))
    }, _updateHash: function (a, b, c) { c ? a.replace(a.toString().replace(/(javascript:|#).*$/, "") + "#" + b) : a.hash = b } 
}); var v = g.View = function (a) { this.cid = f.uniqueId("view"); this._configure(a || {}); this._ensureElement(); this.initialize.apply(this, arguments); this.delegateEvents() }, F = /^(\S+)\s*(.*)$/, w = "model,collection,el,id,attributes,className,tagName".split(",");
    f.extend(v.prototype, k, { tagName: "div", $: function (a) { return this.$el.find(a) }, initialize: function () { }, render: function () { return this }, remove: function () { this.$el.remove(); return this }, make: function (a, b, c) { a = document.createElement(a); b && i(a).attr(b); c && i(a).html(c); return a }, setElement: function (a, b) { this.$el && this.undelegateEvents(); this.$el = a instanceof i ? a : i(a); this.el = this.$el[0]; !1 !== b && this.delegateEvents(); return this }, delegateEvents: function (a) {
        if (a || (a = n(this, "events"))) {
            this.undelegateEvents();
            for (var b in a) { var c = a[b]; f.isFunction(c) || (c = this[a[b]]); if (!c) throw Error('Method "' + a[b] + '" does not exist'); var d = b.match(F), e = d[1], d = d[2], c = f.bind(c, this), e = e + (".delegateEvents" + this.cid); "" === d ? this.$el.bind(e, c) : this.$el.delegate(d, e, c) } 
        } 
    }, undelegateEvents: function () { this.$el.unbind(".delegateEvents" + this.cid) }, _configure: function (a) { this.options && (a = f.extend({}, this.options, a)); for (var b = 0, c = w.length; b < c; b++) { var d = w[b]; a[d] && (this[d] = a[d]) } this.options = a }, _ensureElement: function () {
        if (this.el) this.setElement(this.el,
!1); else { var a = n(this, "attributes") || {}; this.id && (a.id = this.id); this.className && (a["class"] = this.className); this.setElement(this.make(this.tagName, a), !1) } 
    } 
    }); o.extend = r.extend = u.extend = v.extend = function (a, b) { var c = G(this, a, b); c.extend = this.extend; return c }; var H = { create: "POST", update: "PUT", "delete": "DELETE", read: "GET" }; g.sync = function (a, b, c) {
        var d = H[a]; c || (c = {}); var e = { type: d, dataType: "json" }; c.url || (e.url = n(b, "url") || t()); if (!c.data && b && ("create" == a || "update" == a)) e.contentType = "application/json",
e.data = JSON.stringify(b.toJSON()); g.emulateJSON && (e.contentType = "application/x-www-form-urlencoded", e.data = e.data ? { model: e.data} : {}); if (g.emulateHTTP && ("PUT" === d || "DELETE" === d)) g.emulateJSON && (e.data._method = d), e.type = "POST", e.beforeSend = function (a) { a.setRequestHeader("X-HTTP-Method-Override", d) }; "GET" !== e.type && !g.emulateJSON && (e.processData = !1); return i.ajax(f.extend(e, c))
    }; g.wrapError = function (a, b, c) { return function (d, e) { e = d === b ? e : d; a ? a(b, e, c) : b.trigger("error", b, e, c) } }; var x = function () { }, G = function (a,
b, c) { var d; d = b && b.hasOwnProperty("constructor") ? b.constructor : function () { a.apply(this, arguments) }; f.extend(d, a); x.prototype = a.prototype; d.prototype = new x; b && f.extend(d.prototype, b); c && f.extend(d, c); d.prototype.constructor = d; d.__super__ = a.prototype; return d }, n = function (a, b) { return !a || !a[b] ? null : f.isFunction(a[b]) ? a[b]() : a[b] }, t = function () { throw Error('A "url" property or function must be specified'); } 
}).call(this);

