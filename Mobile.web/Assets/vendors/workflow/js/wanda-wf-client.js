try {
    if (jQuery) {
        // jQuery 已加载 
    } else {
    }
}
catch (kqv) {
    /*! jQuery v1.8.3 jquery.com | jquery.org/license */
    (function (e, t) { function _(e) { var t = M[e] = {}; return v.each(e.split(y), function (e, n) { t[n] = !0 }), t } function H(e, n, r) { if (r === t && e.nodeType === 1) { var i = "data-" + n.replace(P, "-$1").toLowerCase(); r = e.getAttribute(i); if (typeof r == "string") { try { r = r === "true" ? !0 : r === "false" ? !1 : r === "null" ? null : +r + "" === r ? +r : D.test(r) ? v.parseJSON(r) : r } catch (s) { } v.data(e, n, r) } else r = t } return r } function B(e) { var t; for (t in e) { if (t === "data" && v.isEmptyObject(e[t])) continue; if (t !== "toJSON") return !1 } return !0 } function et() { return !1 } function tt() { return !0 } function ut(e) { return !e || !e.parentNode || e.parentNode.nodeType === 11 } function at(e, t) { do e = e[t]; while (e && e.nodeType !== 1); return e } function ft(e, t, n) { t = t || 0; if (v.isFunction(t)) return v.grep(e, function (e, r) { var i = !!t.call(e, r, e); return i === n }); if (t.nodeType) return v.grep(e, function (e, r) { return e === t === n }); if (typeof t == "string") { var r = v.grep(e, function (e) { return e.nodeType === 1 }); if (it.test(t)) return v.filter(t, r, !n); t = v.filter(t, r) } return v.grep(e, function (e, r) { return v.inArray(e, t) >= 0 === n }) } function lt(e) { var t = ct.split("|"), n = e.createDocumentFragment(); if (n.createElement) while (t.length) n.createElement(t.pop()); return n } function Lt(e, t) { return e.getElementsByTagName(t)[0] || e.appendChild(e.ownerDocument.createElement(t)) } function At(e, t) { if (t.nodeType !== 1 || !v.hasData(e)) return; var n, r, i, s = v._data(e), o = v._data(t, s), u = s.events; if (u) { delete o.handle, o.events = {}; for (n in u) for (r = 0, i = u[n].length; r < i; r++) v.event.add(t, n, u[n][r]) } o.data && (o.data = v.extend({}, o.data)) } function Ot(e, t) { var n; if (t.nodeType !== 1) return; t.clearAttributes && t.clearAttributes(), t.mergeAttributes && t.mergeAttributes(e), n = t.nodeName.toLowerCase(), n === "object" ? (t.parentNode && (t.outerHTML = e.outerHTML), v.support.html5Clone && e.innerHTML && !v.trim(t.innerHTML) && (t.innerHTML = e.innerHTML)) : n === "input" && Et.test(e.type) ? (t.defaultChecked = t.checked = e.checked, t.value !== e.value && (t.value = e.value)) : n === "option" ? t.selected = e.defaultSelected : n === "input" || n === "textarea" ? t.defaultValue = e.defaultValue : n === "script" && t.text !== e.text && (t.text = e.text), t.removeAttribute(v.expando) } function Mt(e) { return typeof e.getElementsByTagName != "undefined" ? e.getElementsByTagName("*") : typeof e.querySelectorAll != "undefined" ? e.querySelectorAll("*") : [] } function _t(e) { Et.test(e.type) && (e.defaultChecked = e.checked) } function Qt(e, t) { if (t in e) return t; var n = t.charAt(0).toUpperCase() + t.slice(1), r = t, i = Jt.length; while (i--) { t = Jt[i] + n; if (t in e) return t } return r } function Gt(e, t) { return e = t || e, v.css(e, "display") === "none" || !v.contains(e.ownerDocument, e) } function Yt(e, t) { var n, r, i = [], s = 0, o = e.length; for (; s < o; s++) { n = e[s]; if (!n.style) continue; i[s] = v._data(n, "olddisplay"), t ? (!i[s] && n.style.display === "none" && (n.style.display = ""), n.style.display === "" && Gt(n) && (i[s] = v._data(n, "olddisplay", nn(n.nodeName)))) : (r = Dt(n, "display"), !i[s] && r !== "none" && v._data(n, "olddisplay", r)) } for (s = 0; s < o; s++) { n = e[s]; if (!n.style) continue; if (!t || n.style.display === "none" || n.style.display === "") n.style.display = t ? i[s] || "" : "none" } return e } function Zt(e, t, n) { var r = Rt.exec(t); return r ? Math.max(0, r[1] - (n || 0)) + (r[2] || "px") : t } function en(e, t, n, r) { var i = n === (r ? "border" : "content") ? 4 : t === "width" ? 1 : 0, s = 0; for (; i < 4; i += 2) n === "margin" && (s += v.css(e, n + $t[i], !0)), r ? (n === "content" && (s -= parseFloat(Dt(e, "padding" + $t[i])) || 0), n !== "margin" && (s -= parseFloat(Dt(e, "border" + $t[i] + "Width")) || 0)) : (s += parseFloat(Dt(e, "padding" + $t[i])) || 0, n !== "padding" && (s += parseFloat(Dt(e, "border" + $t[i] + "Width")) || 0)); return s } function tn(e, t, n) { var r = t === "width" ? e.offsetWidth : e.offsetHeight, i = !0, s = v.support.boxSizing && v.css(e, "boxSizing") === "border-box"; if (r <= 0 || r == null) { r = Dt(e, t); if (r < 0 || r == null) r = e.style[t]; if (Ut.test(r)) return r; i = s && (v.support.boxSizingReliable || r === e.style[t]), r = parseFloat(r) || 0 } return r + en(e, t, n || (s ? "border" : "content"), i) + "px" } function nn(e) { if (Wt[e]) return Wt[e]; var t = v("<" + e + ">").appendTo(i.body), n = t.css("display"); t.remove(); if (n === "none" || n === "") { Pt = i.body.appendChild(Pt || v.extend(i.createElement("iframe"), { frameBorder: 0, width: 0, height: 0 })); if (!Ht || !Pt.createElement) Ht = (Pt.contentWindow || Pt.contentDocument).document, Ht.write("<!doctype html><html><body>"), Ht.close(); t = Ht.body.appendChild(Ht.createElement(e)), n = Dt(t, "display"), i.body.removeChild(Pt) } return Wt[e] = n, n } function fn(e, t, n, r) { var i; if (v.isArray(t)) v.each(t, function (t, i) { n || sn.test(e) ? r(e, i) : fn(e + "[" + (typeof i == "object" ? t : "") + "]", i, n, r) }); else if (!n && v.type(t) === "object") for (i in t) fn(e + "[" + i + "]", t[i], n, r); else r(e, t) } function Cn(e) { return function (t, n) { typeof t != "string" && (n = t, t = "*"); var r, i, s, o = t.toLowerCase().split(y), u = 0, a = o.length; if (v.isFunction(n)) for (; u < a; u++) r = o[u], s = /^\+/.test(r), s && (r = r.substr(1) || "*"), i = e[r] = e[r] || [], i[s ? "unshift" : "push"](n) } } function kn(e, n, r, i, s, o) { s = s || n.dataTypes[0], o = o || {}, o[s] = !0; var u, a = e[s], f = 0, l = a ? a.length : 0, c = e === Sn; for (; f < l && (c || !u) ; f++) u = a[f](n, r, i), typeof u == "string" && (!c || o[u] ? u = t : (n.dataTypes.unshift(u), u = kn(e, n, r, i, u, o))); return (c || !u) && !o["*"] && (u = kn(e, n, r, i, "*", o)), u } function Ln(e, n) { var r, i, s = v.ajaxSettings.flatOptions || {}; for (r in n) n[r] !== t && ((s[r] ? e : i || (i = {}))[r] = n[r]); i && v.extend(!0, e, i) } function An(e, n, r) { var i, s, o, u, a = e.contents, f = e.dataTypes, l = e.responseFields; for (s in l) s in r && (n[l[s]] = r[s]); while (f[0] === "*") f.shift(), i === t && (i = e.mimeType || n.getResponseHeader("content-type")); if (i) for (s in a) if (a[s] && a[s].test(i)) { f.unshift(s); break } if (f[0] in r) o = f[0]; else { for (s in r) { if (!f[0] || e.converters[s + " " + f[0]]) { o = s; break } u || (u = s) } o = o || u } if (o) return o !== f[0] && f.unshift(o), r[o] } function On(e, t) { var n, r, i, s, o = e.dataTypes.slice(), u = o[0], a = {}, f = 0; e.dataFilter && (t = e.dataFilter(t, e.dataType)); if (o[1]) for (n in e.converters) a[n.toLowerCase()] = e.converters[n]; for (; i = o[++f];) if (i !== "*") { if (u !== "*" && u !== i) { n = a[u + " " + i] || a["* " + i]; if (!n) for (r in a) { s = r.split(" "); if (s[1] === i) { n = a[u + " " + s[0]] || a["* " + s[0]]; if (n) { n === !0 ? n = a[r] : a[r] !== !0 && (i = s[0], o.splice(f--, 0, i)); break } } } if (n !== !0) if (n && e["throws"]) t = n(t); else try { t = n(t) } catch (l) { return { state: "parsererror", error: n ? l : "No conversion from " + u + " to " + i } } } u = i } return { state: "success", data: t } } function Fn() { try { return new e.XMLHttpRequest } catch (t) { } } function In() { try { return new e.ActiveXObject("Microsoft.XMLHTTP") } catch (t) { } } function $n() { return setTimeout(function () { qn = t }, 0), qn = v.now() } function Jn(e, t) { v.each(t, function (t, n) { var r = (Vn[t] || []).concat(Vn["*"]), i = 0, s = r.length; for (; i < s; i++) if (r[i].call(e, t, n)) return }) } function Kn(e, t, n) { var r, i = 0, s = 0, o = Xn.length, u = v.Deferred().always(function () { delete a.elem }), a = function () { var t = qn || $n(), n = Math.max(0, f.startTime + f.duration - t), r = n / f.duration || 0, i = 1 - r, s = 0, o = f.tweens.length; for (; s < o; s++) f.tweens[s].run(i); return u.notifyWith(e, [f, i, n]), i < 1 && o ? n : (u.resolveWith(e, [f]), !1) }, f = u.promise({ elem: e, props: v.extend({}, t), opts: v.extend(!0, { specialEasing: {} }, n), originalProperties: t, originalOptions: n, startTime: qn || $n(), duration: n.duration, tweens: [], createTween: function (t, n, r) { var i = v.Tween(e, f.opts, t, n, f.opts.specialEasing[t] || f.opts.easing); return f.tweens.push(i), i }, stop: function (t) { var n = 0, r = t ? f.tweens.length : 0; for (; n < r; n++) f.tweens[n].run(1); return t ? u.resolveWith(e, [f, t]) : u.rejectWith(e, [f, t]), this } }), l = f.props; Qn(l, f.opts.specialEasing); for (; i < o; i++) { r = Xn[i].call(f, e, l, f.opts); if (r) return r } return Jn(f, l), v.isFunction(f.opts.start) && f.opts.start.call(e, f), v.fx.timer(v.extend(a, { anim: f, queue: f.opts.queue, elem: e })), f.progress(f.opts.progress).done(f.opts.done, f.opts.complete).fail(f.opts.fail).always(f.opts.always) } function Qn(e, t) { var n, r, i, s, o; for (n in e) { r = v.camelCase(n), i = t[r], s = e[n], v.isArray(s) && (i = s[1], s = e[n] = s[0]), n !== r && (e[r] = s, delete e[n]), o = v.cssHooks[r]; if (o && "expand" in o) { s = o.expand(s), delete e[r]; for (n in s) n in e || (e[n] = s[n], t[n] = i) } else t[r] = i } } function Gn(e, t, n) { var r, i, s, o, u, a, f, l, c, h = this, p = e.style, d = {}, m = [], g = e.nodeType && Gt(e); n.queue || (l = v._queueHooks(e, "fx"), l.unqueued == null && (l.unqueued = 0, c = l.empty.fire, l.empty.fire = function () { l.unqueued || c() }), l.unqueued++, h.always(function () { h.always(function () { l.unqueued--, v.queue(e, "fx").length || l.empty.fire() }) })), e.nodeType === 1 && ("height" in t || "width" in t) && (n.overflow = [p.overflow, p.overflowX, p.overflowY], v.css(e, "display") === "inline" && v.css(e, "float") === "none" && (!v.support.inlineBlockNeedsLayout || nn(e.nodeName) === "inline" ? p.display = "inline-block" : p.zoom = 1)), n.overflow && (p.overflow = "hidden", v.support.shrinkWrapBlocks || h.done(function () { p.overflow = n.overflow[0], p.overflowX = n.overflow[1], p.overflowY = n.overflow[2] })); for (r in t) { s = t[r]; if (Un.exec(s)) { delete t[r], a = a || s === "toggle"; if (s === (g ? "hide" : "show")) continue; m.push(r) } } o = m.length; if (o) { u = v._data(e, "fxshow") || v._data(e, "fxshow", {}), "hidden" in u && (g = u.hidden), a && (u.hidden = !g), g ? v(e).show() : h.done(function () { v(e).hide() }), h.done(function () { var t; v.removeData(e, "fxshow", !0); for (t in d) v.style(e, t, d[t]) }); for (r = 0; r < o; r++) i = m[r], f = h.createTween(i, g ? u[i] : 0), d[i] = u[i] || v.style(e, i), i in u || (u[i] = f.start, g && (f.end = f.start, f.start = i === "width" || i === "height" ? 1 : 0)) } } function Yn(e, t, n, r, i) { return new Yn.prototype.init(e, t, n, r, i) } function Zn(e, t) { var n, r = { height: e }, i = 0; t = t ? 1 : 0; for (; i < 4; i += 2 - t) n = $t[i], r["margin" + n] = r["padding" + n] = e; return t && (r.opacity = r.width = e), r } function tr(e) { return v.isWindow(e) ? e : e.nodeType === 9 ? e.defaultView || e.parentWindow : !1 } var n, r, i = e.document, s = e.location, o = e.navigator, u = e.jQuery, a = e.$, f = Array.prototype.push, l = Array.prototype.slice, c = Array.prototype.indexOf, h = Object.prototype.toString, p = Object.prototype.hasOwnProperty, d = String.prototype.trim, v = function (e, t) { return new v.fn.init(e, t, n) }, m = /[\-+]?(?:\d*\.|)\d+(?:[eE][\-+]?\d+|)/.source, g = /\S/, y = /\s+/, b = /^[\s\uFEFF\xA0]+|[\s\uFEFF\xA0]+$/g, w = /^(?:[^#<]*(<[\w\W]+>)[^>]*$|#([\w\-]*)$)/, E = /^<(\w+)\s*\/?>(?:<\/\1>|)$/, S = /^[\],:{}\s]*$/, x = /(?:^|:|,)(?:\s*\[)+/g, T = /\\(?:["\\\/bfnrt]|u[\da-fA-F]{4})/g, N = /"[^"\\\r\n]*"|true|false|null|-?(?:\d\d*\.|)\d+(?:[eE][\-+]?\d+|)/g, C = /^-ms-/, k = /-([\da-z])/gi, L = function (e, t) { return (t + "").toUpperCase() }, A = function () { i.addEventListener ? (i.removeEventListener("DOMContentLoaded", A, !1), v.ready()) : i.readyState === "complete" && (i.detachEvent("onreadystatechange", A), v.ready()) }, O = {}; v.fn = v.prototype = { constructor: v, init: function (e, n, r) { var s, o, u, a; if (!e) return this; if (e.nodeType) return this.context = this[0] = e, this.length = 1, this; if (typeof e == "string") { e.charAt(0) === "<" && e.charAt(e.length - 1) === ">" && e.length >= 3 ? s = [null, e, null] : s = w.exec(e); if (s && (s[1] || !n)) { if (s[1]) return n = n instanceof v ? n[0] : n, a = n && n.nodeType ? n.ownerDocument || n : i, e = v.parseHTML(s[1], a, !0), E.test(s[1]) && v.isPlainObject(n) && this.attr.call(e, n, !0), v.merge(this, e); o = i.getElementById(s[2]); if (o && o.parentNode) { if (o.id !== s[2]) return r.find(e); this.length = 1, this[0] = o } return this.context = i, this.selector = e, this } return !n || n.jquery ? (n || r).find(e) : this.constructor(n).find(e) } return v.isFunction(e) ? r.ready(e) : (e.selector !== t && (this.selector = e.selector, this.context = e.context), v.makeArray(e, this)) }, selector: "", jquery: "1.8.3", length: 0, size: function () { return this.length }, toArray: function () { return l.call(this) }, get: function (e) { return e == null ? this.toArray() : e < 0 ? this[this.length + e] : this[e] }, pushStack: function (e, t, n) { var r = v.merge(this.constructor(), e); return r.prevObject = this, r.context = this.context, t === "find" ? r.selector = this.selector + (this.selector ? " " : "") + n : t && (r.selector = this.selector + "." + t + "(" + n + ")"), r }, each: function (e, t) { return v.each(this, e, t) }, ready: function (e) { return v.ready.promise().done(e), this }, eq: function (e) { return e = +e, e === -1 ? this.slice(e) : this.slice(e, e + 1) }, first: function () { return this.eq(0) }, last: function () { return this.eq(-1) }, slice: function () { return this.pushStack(l.apply(this, arguments), "slice", l.call(arguments).join(",")) }, map: function (e) { return this.pushStack(v.map(this, function (t, n) { return e.call(t, n, t) })) }, end: function () { return this.prevObject || this.constructor(null) }, push: f, sort: [].sort, splice: [].splice }, v.fn.init.prototype = v.fn, v.extend = v.fn.extend = function () { var e, n, r, i, s, o, u = arguments[0] || {}, a = 1, f = arguments.length, l = !1; typeof u == "boolean" && (l = u, u = arguments[1] || {}, a = 2), typeof u != "object" && !v.isFunction(u) && (u = {}), f === a && (u = this, --a); for (; a < f; a++) if ((e = arguments[a]) != null) for (n in e) { r = u[n], i = e[n]; if (u === i) continue; l && i && (v.isPlainObject(i) || (s = v.isArray(i))) ? (s ? (s = !1, o = r && v.isArray(r) ? r : []) : o = r && v.isPlainObject(r) ? r : {}, u[n] = v.extend(l, o, i)) : i !== t && (u[n] = i) } return u }, v.extend({ noConflict: function (t) { return e.$ === v && (e.$ = a), t && e.jQuery === v && (e.jQuery = u), v }, isReady: !1, readyWait: 1, holdReady: function (e) { e ? v.readyWait++ : v.ready(!0) }, ready: function (e) { if (e === !0 ? --v.readyWait : v.isReady) return; if (!i.body) return setTimeout(v.ready, 1); v.isReady = !0; if (e !== !0 && --v.readyWait > 0) return; r.resolveWith(i, [v]), v.fn.trigger && v(i).trigger("ready").off("ready") }, isFunction: function (e) { return v.type(e) === "function" }, isArray: Array.isArray || function (e) { return v.type(e) === "array" }, isWindow: function (e) { return e != null && e == e.window }, isNumeric: function (e) { return !isNaN(parseFloat(e)) && isFinite(e) }, type: function (e) { return e == null ? String(e) : O[h.call(e)] || "object" }, isPlainObject: function (e) { if (!e || v.type(e) !== "object" || e.nodeType || v.isWindow(e)) return !1; try { if (e.constructor && !p.call(e, "constructor") && !p.call(e.constructor.prototype, "isPrototypeOf")) return !1 } catch (n) { return !1 } var r; for (r in e); return r === t || p.call(e, r) }, isEmptyObject: function (e) { var t; for (t in e) return !1; return !0 }, error: function (e) { throw new Error(e) }, parseHTML: function (e, t, n) { var r; return !e || typeof e != "string" ? null : (typeof t == "boolean" && (n = t, t = 0), t = t || i, (r = E.exec(e)) ? [t.createElement(r[1])] : (r = v.buildFragment([e], t, n ? null : []), v.merge([], (r.cacheable ? v.clone(r.fragment) : r.fragment).childNodes))) }, parseJSON: function (t) { if (!t || typeof t != "string") return null; t = v.trim(t); if (e.JSON && e.JSON.parse) return e.JSON.parse(t); if (S.test(t.replace(T, "@").replace(N, "]").replace(x, ""))) return (new Function("return " + t))(); v.error("Invalid JSON: " + t) }, parseXML: function (n) { var r, i; if (!n || typeof n != "string") return null; try { e.DOMParser ? (i = new DOMParser, r = i.parseFromString(n, "text/xml")) : (r = new ActiveXObject("Microsoft.XMLDOM"), r.async = "false", r.loadXML(n)) } catch (s) { r = t } return (!r || !r.documentElement || r.getElementsByTagName("parsererror").length) && v.error("Invalid XML: " + n), r }, noop: function () { }, globalEval: function (t) { t && g.test(t) && (e.execScript || function (t) { e.eval.call(e, t) })(t) }, camelCase: function (e) { return e.replace(C, "ms-").replace(k, L) }, nodeName: function (e, t) { return e.nodeName && e.nodeName.toLowerCase() === t.toLowerCase() }, each: function (e, n, r) { var i, s = 0, o = e.length, u = o === t || v.isFunction(e); if (r) { if (u) { for (i in e) if (n.apply(e[i], r) === !1) break } else for (; s < o;) if (n.apply(e[s++], r) === !1) break } else if (u) { for (i in e) if (n.call(e[i], i, e[i]) === !1) break } else for (; s < o;) if (n.call(e[s], s, e[s++]) === !1) break; return e }, trim: d && !d.call("\ufeff\u00a0") ? function (e) { return e == null ? "" : d.call(e) } : function (e) { return e == null ? "" : (e + "").replace(b, "") }, makeArray: function (e, t) { var n, r = t || []; return e != null && (n = v.type(e), e.length == null || n === "string" || n === "function" || n === "regexp" || v.isWindow(e) ? f.call(r, e) : v.merge(r, e)), r }, inArray: function (e, t, n) { var r; if (t) { if (c) return c.call(t, e, n); r = t.length, n = n ? n < 0 ? Math.max(0, r + n) : n : 0; for (; n < r; n++) if (n in t && t[n] === e) return n } return -1 }, merge: function (e, n) { var r = n.length, i = e.length, s = 0; if (typeof r == "number") for (; s < r; s++) e[i++] = n[s]; else while (n[s] !== t) e[i++] = n[s++]; return e.length = i, e }, grep: function (e, t, n) { var r, i = [], s = 0, o = e.length; n = !!n; for (; s < o; s++) r = !!t(e[s], s), n !== r && i.push(e[s]); return i }, map: function (e, n, r) { var i, s, o = [], u = 0, a = e.length, f = e instanceof v || a !== t && typeof a == "number" && (a > 0 && e[0] && e[a - 1] || a === 0 || v.isArray(e)); if (f) for (; u < a; u++) i = n(e[u], u, r), i != null && (o[o.length] = i); else for (s in e) i = n(e[s], s, r), i != null && (o[o.length] = i); return o.concat.apply([], o) }, guid: 1, proxy: function (e, n) { var r, i, s; return typeof n == "string" && (r = e[n], n = e, e = r), v.isFunction(e) ? (i = l.call(arguments, 2), s = function () { return e.apply(n, i.concat(l.call(arguments))) }, s.guid = e.guid = e.guid || v.guid++, s) : t }, access: function (e, n, r, i, s, o, u) { var a, f = r == null, l = 0, c = e.length; if (r && typeof r == "object") { for (l in r) v.access(e, n, l, r[l], 1, o, i); s = 1 } else if (i !== t) { a = u === t && v.isFunction(i), f && (a ? (a = n, n = function (e, t, n) { return a.call(v(e), n) }) : (n.call(e, i), n = null)); if (n) for (; l < c; l++) n(e[l], r, a ? i.call(e[l], l, n(e[l], r)) : i, u); s = 1 } return s ? e : f ? n.call(e) : c ? n(e[0], r) : o }, now: function () { return (new Date).getTime() } }), v.ready.promise = function (t) { if (!r) { r = v.Deferred(); if (i.readyState === "complete") setTimeout(v.ready, 1); else if (i.addEventListener) i.addEventListener("DOMContentLoaded", A, !1), e.addEventListener("load", v.ready, !1); else { i.attachEvent("onreadystatechange", A), e.attachEvent("onload", v.ready); var n = !1; try { n = e.frameElement == null && i.documentElement } catch (s) { } n && n.doScroll && function o() { if (!v.isReady) { try { n.doScroll("left") } catch (e) { return setTimeout(o, 50) } v.ready() } }() } } return r.promise(t) }, v.each("Boolean Number String Function Array Date RegExp Object".split(" "), function (e, t) { O["[object " + t + "]"] = t.toLowerCase() }), n = v(i); var M = {}; v.Callbacks = function (e) { e = typeof e == "string" ? M[e] || _(e) : v.extend({}, e); var n, r, i, s, o, u, a = [], f = !e.once && [], l = function (t) { n = e.memory && t, r = !0, u = s || 0, s = 0, o = a.length, i = !0; for (; a && u < o; u++) if (a[u].apply(t[0], t[1]) === !1 && e.stopOnFalse) { n = !1; break } i = !1, a && (f ? f.length && l(f.shift()) : n ? a = [] : c.disable()) }, c = { add: function () { if (a) { var t = a.length; (function r(t) { v.each(t, function (t, n) { var i = v.type(n); i === "function" ? (!e.unique || !c.has(n)) && a.push(n) : n && n.length && i !== "string" && r(n) }) })(arguments), i ? o = a.length : n && (s = t, l(n)) } return this }, remove: function () { return a && v.each(arguments, function (e, t) { var n; while ((n = v.inArray(t, a, n)) > -1) a.splice(n, 1), i && (n <= o && o--, n <= u && u--) }), this }, has: function (e) { return v.inArray(e, a) > -1 }, empty: function () { return a = [], this }, disable: function () { return a = f = n = t, this }, disabled: function () { return !a }, lock: function () { return f = t, n || c.disable(), this }, locked: function () { return !f }, fireWith: function (e, t) { return t = t || [], t = [e, t.slice ? t.slice() : t], a && (!r || f) && (i ? f.push(t) : l(t)), this }, fire: function () { return c.fireWith(this, arguments), this }, fired: function () { return !!r } }; return c }, v.extend({ Deferred: function (e) { var t = [["resolve", "done", v.Callbacks("once memory"), "resolved"], ["reject", "fail", v.Callbacks("once memory"), "rejected"], ["notify", "progress", v.Callbacks("memory")]], n = "pending", r = { state: function () { return n }, always: function () { return i.done(arguments).fail(arguments), this }, then: function () { var e = arguments; return v.Deferred(function (n) { v.each(t, function (t, r) { var s = r[0], o = e[t]; i[r[1]](v.isFunction(o) ? function () { var e = o.apply(this, arguments); e && v.isFunction(e.promise) ? e.promise().done(n.resolve).fail(n.reject).progress(n.notify) : n[s + "With"](this === i ? n : this, [e]) } : n[s]) }), e = null }).promise() }, promise: function (e) { return e != null ? v.extend(e, r) : r } }, i = {}; return r.pipe = r.then, v.each(t, function (e, s) { var o = s[2], u = s[3]; r[s[1]] = o.add, u && o.add(function () { n = u }, t[e ^ 1][2].disable, t[2][2].lock), i[s[0]] = o.fire, i[s[0] + "With"] = o.fireWith }), r.promise(i), e && e.call(i, i), i }, when: function (e) { var t = 0, n = l.call(arguments), r = n.length, i = r !== 1 || e && v.isFunction(e.promise) ? r : 0, s = i === 1 ? e : v.Deferred(), o = function (e, t, n) { return function (r) { t[e] = this, n[e] = arguments.length > 1 ? l.call(arguments) : r, n === u ? s.notifyWith(t, n) : --i || s.resolveWith(t, n) } }, u, a, f; if (r > 1) { u = new Array(r), a = new Array(r), f = new Array(r); for (; t < r; t++) n[t] && v.isFunction(n[t].promise) ? n[t].promise().done(o(t, f, n)).fail(s.reject).progress(o(t, a, u)) : --i } return i || s.resolveWith(f, n), s.promise() } }), v.support = function () { var t, n, r, s, o, u, a, f, l, c, h, p = i.createElement("div"); p.setAttribute("className", "t"), p.innerHTML = "  <link/><table></table><a href='/a'>a</a><input type='checkbox'/>", n = p.getElementsByTagName("*"), r = p.getElementsByTagName("a")[0]; if (!n || !r || !n.length) return {}; s = i.createElement("select"), o = s.appendChild(i.createElement("option")), u = p.getElementsByTagName("input")[0], r.style.cssText = "top:1px;float:left;opacity:.5", t = { leadingWhitespace: p.firstChild.nodeType === 3, tbody: !p.getElementsByTagName("tbody").length, htmlSerialize: !!p.getElementsByTagName("link").length, style: /top/.test(r.getAttribute("style")), hrefNormalized: r.getAttribute("href") === "/a", opacity: /^0.5/.test(r.style.opacity), cssFloat: !!r.style.cssFloat, checkOn: u.value === "on", optSelected: o.selected, getSetAttribute: p.className !== "t", enctype: !!i.createElement("form").enctype, html5Clone: i.createElement("nav").cloneNode(!0).outerHTML !== "<:nav></:nav>", boxModel: i.compatMode === "CSS1Compat", submitBubbles: !0, changeBubbles: !0, focusinBubbles: !1, deleteExpando: !0, noCloneEvent: !0, inlineBlockNeedsLayout: !1, shrinkWrapBlocks: !1, reliableMarginRight: !0, boxSizingReliable: !0, pixelPosition: !1 }, u.checked = !0, t.noCloneChecked = u.cloneNode(!0).checked, s.disabled = !0, t.optDisabled = !o.disabled; try { delete p.test } catch (d) { t.deleteExpando = !1 } !p.addEventListener && p.attachEvent && p.fireEvent && (p.attachEvent("onclick", h = function () { t.noCloneEvent = !1 }), p.cloneNode(!0).fireEvent("onclick"), p.detachEvent("onclick", h)), u = i.createElement("input"), u.value = "t", u.setAttribute("type", "radio"), t.radioValue = u.value === "t", u.setAttribute("checked", "checked"), u.setAttribute("name", "t"), p.appendChild(u), a = i.createDocumentFragment(), a.appendChild(p.lastChild), t.checkClone = a.cloneNode(!0).cloneNode(!0).lastChild.checked, t.appendChecked = u.checked, a.removeChild(u), a.appendChild(p); if (p.attachEvent) for (l in { submit: !0, change: !0, focusin: !0 }) f = "on" + l, c = f in p, c || (p.setAttribute(f, "return;"), c = typeof p[f] == "function"), t[l + "Bubbles"] = c; return v(function () { var n, r, s, o, u = "padding:0;margin:0;border:0;display:block;overflow:hidden;", a = i.getElementsByTagName("body")[0]; if (!a) return; n = i.createElement("div"), n.style.cssText = "visibility:hidden;border:0;width:0;height:0;position:static;top:0;margin-top:1px", a.insertBefore(n, a.firstChild), r = i.createElement("div"), n.appendChild(r), r.innerHTML = "<table><tr><td></td><td>t</td></tr></table>", s = r.getElementsByTagName("td"), s[0].style.cssText = "padding:0;margin:0;border:0;display:none", c = s[0].offsetHeight === 0, s[0].style.display = "", s[1].style.display = "none", t.reliableHiddenOffsets = c && s[0].offsetHeight === 0, r.innerHTML = "", r.style.cssText = "box-sizing:border-box;-moz-box-sizing:border-box;-webkit-box-sizing:border-box;padding:1px;border:1px;display:block;width:4px;margin-top:1%;position:absolute;top:1%;", t.boxSizing = r.offsetWidth === 4, t.doesNotIncludeMarginInBodyOffset = a.offsetTop !== 1, e.getComputedStyle && (t.pixelPosition = (e.getComputedStyle(r, null) || {}).top !== "1%", t.boxSizingReliable = (e.getComputedStyle(r, null) || { width: "4px" }).width === "4px", o = i.createElement("div"), o.style.cssText = r.style.cssText = u, o.style.marginRight = o.style.width = "0", r.style.width = "1px", r.appendChild(o), t.reliableMarginRight = !parseFloat((e.getComputedStyle(o, null) || {}).marginRight)), typeof r.style.zoom != "undefined" && (r.innerHTML = "", r.style.cssText = u + "width:1px;padding:1px;display:inline;zoom:1", t.inlineBlockNeedsLayout = r.offsetWidth === 3, r.style.display = "block", r.style.overflow = "visible", r.innerHTML = "<div></div>", r.firstChild.style.width = "5px", t.shrinkWrapBlocks = r.offsetWidth !== 3, n.style.zoom = 1), a.removeChild(n), n = r = s = o = null }), a.removeChild(p), n = r = s = o = u = a = p = null, t }(); var D = /(?:\{[\s\S]*\}|\[[\s\S]*\])$/, P = /([A-Z])/g; v.extend({ cache: {}, deletedIds: [], uuid: 0, expando: "jQuery" + (v.fn.jquery + Math.random()).replace(/\D/g, ""), noData: { embed: !0, object: "clsid:D27CDB6E-AE6D-11cf-96B8-444553540000", applet: !0 }, hasData: function (e) { return e = e.nodeType ? v.cache[e[v.expando]] : e[v.expando], !!e && !B(e) }, data: function (e, n, r, i) { if (!v.acceptData(e)) return; var s, o, u = v.expando, a = typeof n == "string", f = e.nodeType, l = f ? v.cache : e, c = f ? e[u] : e[u] && u; if ((!c || !l[c] || !i && !l[c].data) && a && r === t) return; c || (f ? e[u] = c = v.deletedIds.pop() || v.guid++ : c = u), l[c] || (l[c] = {}, f || (l[c].toJSON = v.noop)); if (typeof n == "object" || typeof n == "function") i ? l[c] = v.extend(l[c], n) : l[c].data = v.extend(l[c].data, n); return s = l[c], i || (s.data || (s.data = {}), s = s.data), r !== t && (s[v.camelCase(n)] = r), a ? (o = s[n], o == null && (o = s[v.camelCase(n)])) : o = s, o }, removeData: function (e, t, n) { if (!v.acceptData(e)) return; var r, i, s, o = e.nodeType, u = o ? v.cache : e, a = o ? e[v.expando] : v.expando; if (!u[a]) return; if (t) { r = n ? u[a] : u[a].data; if (r) { v.isArray(t) || (t in r ? t = [t] : (t = v.camelCase(t), t in r ? t = [t] : t = t.split(" "))); for (i = 0, s = t.length; i < s; i++) delete r[t[i]]; if (!(n ? B : v.isEmptyObject)(r)) return } } if (!n) { delete u[a].data; if (!B(u[a])) return } o ? v.cleanData([e], !0) : v.support.deleteExpando || u != u.window ? delete u[a] : u[a] = null }, _data: function (e, t, n) { return v.data(e, t, n, !0) }, acceptData: function (e) { var t = e.nodeName && v.noData[e.nodeName.toLowerCase()]; return !t || t !== !0 && e.getAttribute("classid") === t } }), v.fn.extend({ data: function (e, n) { var r, i, s, o, u, a = this[0], f = 0, l = null; if (e === t) { if (this.length) { l = v.data(a); if (a.nodeType === 1 && !v._data(a, "parsedAttrs")) { s = a.attributes; for (u = s.length; f < u; f++) o = s[f].name, o.indexOf("data-") || (o = v.camelCase(o.substring(5)), H(a, o, l[o])); v._data(a, "parsedAttrs", !0) } } return l } return typeof e == "object" ? this.each(function () { v.data(this, e) }) : (r = e.split(".", 2), r[1] = r[1] ? "." + r[1] : "", i = r[1] + "!", v.access(this, function (n) { if (n === t) return l = this.triggerHandler("getData" + i, [r[0]]), l === t && a && (l = v.data(a, e), l = H(a, e, l)), l === t && r[1] ? this.data(r[0]) : l; r[1] = n, this.each(function () { var t = v(this); t.triggerHandler("setData" + i, r), v.data(this, e, n), t.triggerHandler("changeData" + i, r) }) }, null, n, arguments.length > 1, null, !1)) }, removeData: function (e) { return this.each(function () { v.removeData(this, e) }) } }), v.extend({ queue: function (e, t, n) { var r; if (e) return t = (t || "fx") + "queue", r = v._data(e, t), n && (!r || v.isArray(n) ? r = v._data(e, t, v.makeArray(n)) : r.push(n)), r || [] }, dequeue: function (e, t) { t = t || "fx"; var n = v.queue(e, t), r = n.length, i = n.shift(), s = v._queueHooks(e, t), o = function () { v.dequeue(e, t) }; i === "inprogress" && (i = n.shift(), r--), i && (t === "fx" && n.unshift("inprogress"), delete s.stop, i.call(e, o, s)), !r && s && s.empty.fire() }, _queueHooks: function (e, t) { var n = t + "queueHooks"; return v._data(e, n) || v._data(e, n, { empty: v.Callbacks("once memory").add(function () { v.removeData(e, t + "queue", !0), v.removeData(e, n, !0) }) }) } }), v.fn.extend({ queue: function (e, n) { var r = 2; return typeof e != "string" && (n = e, e = "fx", r--), arguments.length < r ? v.queue(this[0], e) : n === t ? this : this.each(function () { var t = v.queue(this, e, n); v._queueHooks(this, e), e === "fx" && t[0] !== "inprogress" && v.dequeue(this, e) }) }, dequeue: function (e) { return this.each(function () { v.dequeue(this, e) }) }, delay: function (e, t) { return e = v.fx ? v.fx.speeds[e] || e : e, t = t || "fx", this.queue(t, function (t, n) { var r = setTimeout(t, e); n.stop = function () { clearTimeout(r) } }) }, clearQueue: function (e) { return this.queue(e || "fx", []) }, promise: function (e, n) { var r, i = 1, s = v.Deferred(), o = this, u = this.length, a = function () { --i || s.resolveWith(o, [o]) }; typeof e != "string" && (n = e, e = t), e = e || "fx"; while (u--) r = v._data(o[u], e + "queueHooks"), r && r.empty && (i++, r.empty.add(a)); return a(), s.promise(n) } }); var j, F, I, q = /[\t\r\n]/g, R = /\r/g, U = /^(?:button|input)$/i, z = /^(?:button|input|object|select|textarea)$/i, W = /^a(?:rea|)$/i, X = /^(?:autofocus|autoplay|async|checked|controls|defer|disabled|hidden|loop|multiple|open|readonly|required|scoped|selected)$/i, V = v.support.getSetAttribute; v.fn.extend({ attr: function (e, t) { return v.access(this, v.attr, e, t, arguments.length > 1) }, removeAttr: function (e) { return this.each(function () { v.removeAttr(this, e) }) }, prop: function (e, t) { return v.access(this, v.prop, e, t, arguments.length > 1) }, removeProp: function (e) { return e = v.propFix[e] || e, this.each(function () { try { this[e] = t, delete this[e] } catch (n) { } }) }, addClass: function (e) { var t, n, r, i, s, o, u; if (v.isFunction(e)) return this.each(function (t) { v(this).addClass(e.call(this, t, this.className)) }); if (e && typeof e == "string") { t = e.split(y); for (n = 0, r = this.length; n < r; n++) { i = this[n]; if (i.nodeType === 1) if (!i.className && t.length === 1) i.className = e; else { s = " " + i.className + " "; for (o = 0, u = t.length; o < u; o++) s.indexOf(" " + t[o] + " ") < 0 && (s += t[o] + " "); i.className = v.trim(s) } } } return this }, removeClass: function (e) { var n, r, i, s, o, u, a; if (v.isFunction(e)) return this.each(function (t) { v(this).removeClass(e.call(this, t, this.className)) }); if (e && typeof e == "string" || e === t) { n = (e || "").split(y); for (u = 0, a = this.length; u < a; u++) { i = this[u]; if (i.nodeType === 1 && i.className) { r = (" " + i.className + " ").replace(q, " "); for (s = 0, o = n.length; s < o; s++) while (r.indexOf(" " + n[s] + " ") >= 0) r = r.replace(" " + n[s] + " ", " "); i.className = e ? v.trim(r) : "" } } } return this }, toggleClass: function (e, t) { var n = typeof e, r = typeof t == "boolean"; return v.isFunction(e) ? this.each(function (n) { v(this).toggleClass(e.call(this, n, this.className, t), t) }) : this.each(function () { if (n === "string") { var i, s = 0, o = v(this), u = t, a = e.split(y); while (i = a[s++]) u = r ? u : !o.hasClass(i), o[u ? "addClass" : "removeClass"](i) } else if (n === "undefined" || n === "boolean") this.className && v._data(this, "__className__", this.className), this.className = this.className || e === !1 ? "" : v._data(this, "__className__") || "" }) }, hasClass: function (e) { var t = " " + e + " ", n = 0, r = this.length; for (; n < r; n++) if (this[n].nodeType === 1 && (" " + this[n].className + " ").replace(q, " ").indexOf(t) >= 0) return !0; return !1 }, val: function (e) { var n, r, i, s = this[0]; if (!arguments.length) { if (s) return n = v.valHooks[s.type] || v.valHooks[s.nodeName.toLowerCase()], n && "get" in n && (r = n.get(s, "value")) !== t ? r : (r = s.value, typeof r == "string" ? r.replace(R, "") : r == null ? "" : r); return } return i = v.isFunction(e), this.each(function (r) { var s, o = v(this); if (this.nodeType !== 1) return; i ? s = e.call(this, r, o.val()) : s = e, s == null ? s = "" : typeof s == "number" ? s += "" : v.isArray(s) && (s = v.map(s, function (e) { return e == null ? "" : e + "" })), n = v.valHooks[this.type] || v.valHooks[this.nodeName.toLowerCase()]; if (!n || !("set" in n) || n.set(this, s, "value") === t) this.value = s }) } }), v.extend({ valHooks: { option: { get: function (e) { var t = e.attributes.value; return !t || t.specified ? e.value : e.text } }, select: { get: function (e) { var t, n, r = e.options, i = e.selectedIndex, s = e.type === "select-one" || i < 0, o = s ? null : [], u = s ? i + 1 : r.length, a = i < 0 ? u : s ? i : 0; for (; a < u; a++) { n = r[a]; if ((n.selected || a === i) && (v.support.optDisabled ? !n.disabled : n.getAttribute("disabled") === null) && (!n.parentNode.disabled || !v.nodeName(n.parentNode, "optgroup"))) { t = v(n).val(); if (s) return t; o.push(t) } } return o }, set: function (e, t) { var n = v.makeArray(t); return v(e).find("option").each(function () { this.selected = v.inArray(v(this).val(), n) >= 0 }), n.length || (e.selectedIndex = -1), n } } }, attrFn: {}, attr: function (e, n, r, i) { var s, o, u, a = e.nodeType; if (!e || a === 3 || a === 8 || a === 2) return; if (i && v.isFunction(v.fn[n])) return v(e)[n](r); if (typeof e.getAttribute == "undefined") return v.prop(e, n, r); u = a !== 1 || !v.isXMLDoc(e), u && (n = n.toLowerCase(), o = v.attrHooks[n] || (X.test(n) ? F : j)); if (r !== t) { if (r === null) { v.removeAttr(e, n); return } return o && "set" in o && u && (s = o.set(e, r, n)) !== t ? s : (e.setAttribute(n, r + ""), r) } return o && "get" in o && u && (s = o.get(e, n)) !== null ? s : (s = e.getAttribute(n), s === null ? t : s) }, removeAttr: function (e, t) { var n, r, i, s, o = 0; if (t && e.nodeType === 1) { r = t.split(y); for (; o < r.length; o++) i = r[o], i && (n = v.propFix[i] || i, s = X.test(i), s || v.attr(e, i, ""), e.removeAttribute(V ? i : n), s && n in e && (e[n] = !1)) } }, attrHooks: { type: { set: function (e, t) { if (U.test(e.nodeName) && e.parentNode) v.error("type property can't be changed"); else if (!v.support.radioValue && t === "radio" && v.nodeName(e, "input")) { var n = e.value; return e.setAttribute("type", t), n && (e.value = n), t } } }, value: { get: function (e, t) { return j && v.nodeName(e, "button") ? j.get(e, t) : t in e ? e.value : null }, set: function (e, t, n) { if (j && v.nodeName(e, "button")) return j.set(e, t, n); e.value = t } } }, propFix: { tabindex: "tabIndex", readonly: "readOnly", "for": "htmlFor", "class": "className", maxlength: "maxLength", cellspacing: "cellSpacing", cellpadding: "cellPadding", rowspan: "rowSpan", colspan: "colSpan", usemap: "useMap", frameborder: "frameBorder", contenteditable: "contentEditable" }, prop: function (e, n, r) { var i, s, o, u = e.nodeType; if (!e || u === 3 || u === 8 || u === 2) return; return o = u !== 1 || !v.isXMLDoc(e), o && (n = v.propFix[n] || n, s = v.propHooks[n]), r !== t ? s && "set" in s && (i = s.set(e, r, n)) !== t ? i : e[n] = r : s && "get" in s && (i = s.get(e, n)) !== null ? i : e[n] }, propHooks: { tabIndex: { get: function (e) { var n = e.getAttributeNode("tabindex"); return n && n.specified ? parseInt(n.value, 10) : z.test(e.nodeName) || W.test(e.nodeName) && e.href ? 0 : t } } } }), F = { get: function (e, n) { var r, i = v.prop(e, n); return i === !0 || typeof i != "boolean" && (r = e.getAttributeNode(n)) && r.nodeValue !== !1 ? n.toLowerCase() : t }, set: function (e, t, n) { var r; return t === !1 ? v.removeAttr(e, n) : (r = v.propFix[n] || n, r in e && (e[r] = !0), e.setAttribute(n, n.toLowerCase())), n } }, V || (I = { name: !0, id: !0, coords: !0 }, j = v.valHooks.button = { get: function (e, n) { var r; return r = e.getAttributeNode(n), r && (I[n] ? r.value !== "" : r.specified) ? r.value : t }, set: function (e, t, n) { var r = e.getAttributeNode(n); return r || (r = i.createAttribute(n), e.setAttributeNode(r)), r.value = t + "" } }, v.each(["width", "height"], function (e, t) { v.attrHooks[t] = v.extend(v.attrHooks[t], { set: function (e, n) { if (n === "") return e.setAttribute(t, "auto"), n } }) }), v.attrHooks.contenteditable = { get: j.get, set: function (e, t, n) { t === "" && (t = "false"), j.set(e, t, n) } }), v.support.hrefNormalized || v.each(["href", "src", "width", "height"], function (e, n) { v.attrHooks[n] = v.extend(v.attrHooks[n], { get: function (e) { var r = e.getAttribute(n, 2); return r === null ? t : r } }) }), v.support.style || (v.attrHooks.style = { get: function (e) { return e.style.cssText.toLowerCase() || t }, set: function (e, t) { return e.style.cssText = t + "" } }), v.support.optSelected || (v.propHooks.selected = v.extend(v.propHooks.selected, { get: function (e) { var t = e.parentNode; return t && (t.selectedIndex, t.parentNode && t.parentNode.selectedIndex), null } })), v.support.enctype || (v.propFix.enctype = "encoding"), v.support.checkOn || v.each(["radio", "checkbox"], function () { v.valHooks[this] = { get: function (e) { return e.getAttribute("value") === null ? "on" : e.value } } }), v.each(["radio", "checkbox"], function () { v.valHooks[this] = v.extend(v.valHooks[this], { set: function (e, t) { if (v.isArray(t)) return e.checked = v.inArray(v(e).val(), t) >= 0 } }) }); var $ = /^(?:textarea|input|select)$/i, J = /^([^\.]*|)(?:\.(.+)|)$/, K = /(?:^|\s)hover(\.\S+|)\b/, Q = /^key/, G = /^(?:mouse|contextmenu)|click/, Y = /^(?:focusinfocus|focusoutblur)$/, Z = function (e) { return v.event.special.hover ? e : e.replace(K, "mouseenter$1 mouseleave$1") }; v.event = { add: function (e, n, r, i, s) { var o, u, a, f, l, c, h, p, d, m, g; if (e.nodeType === 3 || e.nodeType === 8 || !n || !r || !(o = v._data(e))) return; r.handler && (d = r, r = d.handler, s = d.selector), r.guid || (r.guid = v.guid++), a = o.events, a || (o.events = a = {}), u = o.handle, u || (o.handle = u = function (e) { return typeof v == "undefined" || !!e && v.event.triggered === e.type ? t : v.event.dispatch.apply(u.elem, arguments) }, u.elem = e), n = v.trim(Z(n)).split(" "); for (f = 0; f < n.length; f++) { l = J.exec(n[f]) || [], c = l[1], h = (l[2] || "").split(".").sort(), g = v.event.special[c] || {}, c = (s ? g.delegateType : g.bindType) || c, g = v.event.special[c] || {}, p = v.extend({ type: c, origType: l[1], data: i, handler: r, guid: r.guid, selector: s, needsContext: s && v.expr.match.needsContext.test(s), namespace: h.join(".") }, d), m = a[c]; if (!m) { m = a[c] = [], m.delegateCount = 0; if (!g.setup || g.setup.call(e, i, h, u) === !1) e.addEventListener ? e.addEventListener(c, u, !1) : e.attachEvent && e.attachEvent("on" + c, u) } g.add && (g.add.call(e, p), p.handler.guid || (p.handler.guid = r.guid)), s ? m.splice(m.delegateCount++, 0, p) : m.push(p), v.event.global[c] = !0 } e = null }, global: {}, remove: function (e, t, n, r, i) { var s, o, u, a, f, l, c, h, p, d, m, g = v.hasData(e) && v._data(e); if (!g || !(h = g.events)) return; t = v.trim(Z(t || "")).split(" "); for (s = 0; s < t.length; s++) { o = J.exec(t[s]) || [], u = a = o[1], f = o[2]; if (!u) { for (u in h) v.event.remove(e, u + t[s], n, r, !0); continue } p = v.event.special[u] || {}, u = (r ? p.delegateType : p.bindType) || u, d = h[u] || [], l = d.length, f = f ? new RegExp("(^|\\.)" + f.split(".").sort().join("\\.(?:.*\\.|)") + "(\\.|$)") : null; for (c = 0; c < d.length; c++) m = d[c], (i || a === m.origType) && (!n || n.guid === m.guid) && (!f || f.test(m.namespace)) && (!r || r === m.selector || r === "**" && m.selector) && (d.splice(c--, 1), m.selector && d.delegateCount--, p.remove && p.remove.call(e, m)); d.length === 0 && l !== d.length && ((!p.teardown || p.teardown.call(e, f, g.handle) === !1) && v.removeEvent(e, u, g.handle), delete h[u]) } v.isEmptyObject(h) && (delete g.handle, v.removeData(e, "events", !0)) }, customEvent: { getData: !0, setData: !0, changeData: !0 }, trigger: function (n, r, s, o) { if (!s || s.nodeType !== 3 && s.nodeType !== 8) { var u, a, f, l, c, h, p, d, m, g, y = n.type || n, b = []; if (Y.test(y + v.event.triggered)) return; y.indexOf("!") >= 0 && (y = y.slice(0, -1), a = !0), y.indexOf(".") >= 0 && (b = y.split("."), y = b.shift(), b.sort()); if ((!s || v.event.customEvent[y]) && !v.event.global[y]) return; n = typeof n == "object" ? n[v.expando] ? n : new v.Event(y, n) : new v.Event(y), n.type = y, n.isTrigger = !0, n.exclusive = a, n.namespace = b.join("."), n.namespace_re = n.namespace ? new RegExp("(^|\\.)" + b.join("\\.(?:.*\\.|)") + "(\\.|$)") : null, h = y.indexOf(":") < 0 ? "on" + y : ""; if (!s) { u = v.cache; for (f in u) u[f].events && u[f].events[y] && v.event.trigger(n, r, u[f].handle.elem, !0); return } n.result = t, n.target || (n.target = s), r = r != null ? v.makeArray(r) : [], r.unshift(n), p = v.event.special[y] || {}; if (p.trigger && p.trigger.apply(s, r) === !1) return; m = [[s, p.bindType || y]]; if (!o && !p.noBubble && !v.isWindow(s)) { g = p.delegateType || y, l = Y.test(g + y) ? s : s.parentNode; for (c = s; l; l = l.parentNode) m.push([l, g]), c = l; c === (s.ownerDocument || i) && m.push([c.defaultView || c.parentWindow || e, g]) } for (f = 0; f < m.length && !n.isPropagationStopped() ; f++) l = m[f][0], n.type = m[f][1], d = (v._data(l, "events") || {})[n.type] && v._data(l, "handle"), d && d.apply(l, r), d = h && l[h], d && v.acceptData(l) && d.apply && d.apply(l, r) === !1 && n.preventDefault(); return n.type = y, !o && !n.isDefaultPrevented() && (!p._default || p._default.apply(s.ownerDocument, r) === !1) && (y !== "click" || !v.nodeName(s, "a")) && v.acceptData(s) && h && s[y] && (y !== "focus" && y !== "blur" || n.target.offsetWidth !== 0) && !v.isWindow(s) && (c = s[h], c && (s[h] = null), v.event.triggered = y, s[y](), v.event.triggered = t, c && (s[h] = c)), n.result } return }, dispatch: function (n) { n = v.event.fix(n || e.event); var r, i, s, o, u, a, f, c, h, p, d = (v._data(this, "events") || {})[n.type] || [], m = d.delegateCount, g = l.call(arguments), y = !n.exclusive && !n.namespace, b = v.event.special[n.type] || {}, w = []; g[0] = n, n.delegateTarget = this; if (b.preDispatch && b.preDispatch.call(this, n) === !1) return; if (m && (!n.button || n.type !== "click")) for (s = n.target; s != this; s = s.parentNode || this) if (s.disabled !== !0 || n.type !== "click") { u = {}, f = []; for (r = 0; r < m; r++) c = d[r], h = c.selector, u[h] === t && (u[h] = c.needsContext ? v(h, this).index(s) >= 0 : v.find(h, this, null, [s]).length), u[h] && f.push(c); f.length && w.push({ elem: s, matches: f }) } d.length > m && w.push({ elem: this, matches: d.slice(m) }); for (r = 0; r < w.length && !n.isPropagationStopped() ; r++) { a = w[r], n.currentTarget = a.elem; for (i = 0; i < a.matches.length && !n.isImmediatePropagationStopped() ; i++) { c = a.matches[i]; if (y || !n.namespace && !c.namespace || n.namespace_re && n.namespace_re.test(c.namespace)) n.data = c.data, n.handleObj = c, o = ((v.event.special[c.origType] || {}).handle || c.handler).apply(a.elem, g), o !== t && (n.result = o, o === !1 && (n.preventDefault(), n.stopPropagation())) } } return b.postDispatch && b.postDispatch.call(this, n), n.result }, props: "attrChange attrName relatedNode srcElement altKey bubbles cancelable ctrlKey currentTarget eventPhase metaKey relatedTarget shiftKey target timeStamp view which".split(" "), fixHooks: {}, keyHooks: { props: "char charCode key keyCode".split(" "), filter: function (e, t) { return e.which == null && (e.which = t.charCode != null ? t.charCode : t.keyCode), e } }, mouseHooks: { props: "button buttons clientX clientY fromElement offsetX offsetY pageX pageY screenX screenY toElement".split(" "), filter: function (e, n) { var r, s, o, u = n.button, a = n.fromElement; return e.pageX == null && n.clientX != null && (r = e.target.ownerDocument || i, s = r.documentElement, o = r.body, e.pageX = n.clientX + (s && s.scrollLeft || o && o.scrollLeft || 0) - (s && s.clientLeft || o && o.clientLeft || 0), e.pageY = n.clientY + (s && s.scrollTop || o && o.scrollTop || 0) - (s && s.clientTop || o && o.clientTop || 0)), !e.relatedTarget && a && (e.relatedTarget = a === e.target ? n.toElement : a), !e.which && u !== t && (e.which = u & 1 ? 1 : u & 2 ? 3 : u & 4 ? 2 : 0), e } }, fix: function (e) { if (e[v.expando]) return e; var t, n, r = e, s = v.event.fixHooks[e.type] || {}, o = s.props ? this.props.concat(s.props) : this.props; e = v.Event(r); for (t = o.length; t;) n = o[--t], e[n] = r[n]; return e.target || (e.target = r.srcElement || i), e.target.nodeType === 3 && (e.target = e.target.parentNode), e.metaKey = !!e.metaKey, s.filter ? s.filter(e, r) : e }, special: { load: { noBubble: !0 }, focus: { delegateType: "focusin" }, blur: { delegateType: "focusout" }, beforeunload: { setup: function (e, t, n) { v.isWindow(this) && (this.onbeforeunload = n) }, teardown: function (e, t) { this.onbeforeunload === t && (this.onbeforeunload = null) } } }, simulate: function (e, t, n, r) { var i = v.extend(new v.Event, n, { type: e, isSimulated: !0, originalEvent: {} }); r ? v.event.trigger(i, null, t) : v.event.dispatch.call(t, i), i.isDefaultPrevented() && n.preventDefault() } }, v.event.handle = v.event.dispatch, v.removeEvent = i.removeEventListener ? function (e, t, n) { e.removeEventListener && e.removeEventListener(t, n, !1) } : function (e, t, n) { var r = "on" + t; e.detachEvent && (typeof e[r] == "undefined" && (e[r] = null), e.detachEvent(r, n)) }, v.Event = function (e, t) { if (!(this instanceof v.Event)) return new v.Event(e, t); e && e.type ? (this.originalEvent = e, this.type = e.type, this.isDefaultPrevented = e.defaultPrevented || e.returnValue === !1 || e.getPreventDefault && e.getPreventDefault() ? tt : et) : this.type = e, t && v.extend(this, t), this.timeStamp = e && e.timeStamp || v.now(), this[v.expando] = !0 }, v.Event.prototype = { preventDefault: function () { this.isDefaultPrevented = tt; var e = this.originalEvent; if (!e) return; e.preventDefault ? e.preventDefault() : e.returnValue = !1 }, stopPropagation: function () { this.isPropagationStopped = tt; var e = this.originalEvent; if (!e) return; e.stopPropagation && e.stopPropagation(), e.cancelBubble = !0 }, stopImmediatePropagation: function () { this.isImmediatePropagationStopped = tt, this.stopPropagation() }, isDefaultPrevented: et, isPropagationStopped: et, isImmediatePropagationStopped: et }, v.each({ mouseenter: "mouseover", mouseleave: "mouseout" }, function (e, t) { v.event.special[e] = { delegateType: t, bindType: t, handle: function (e) { var n, r = this, i = e.relatedTarget, s = e.handleObj, o = s.selector; if (!i || i !== r && !v.contains(r, i)) e.type = s.origType, n = s.handler.apply(this, arguments), e.type = t; return n } } }), v.support.submitBubbles || (v.event.special.submit = { setup: function () { if (v.nodeName(this, "form")) return !1; v.event.add(this, "click._submit keypress._submit", function (e) { var n = e.target, r = v.nodeName(n, "input") || v.nodeName(n, "button") ? n.form : t; r && !v._data(r, "_submit_attached") && (v.event.add(r, "submit._submit", function (e) { e._submit_bubble = !0 }), v._data(r, "_submit_attached", !0)) }) }, postDispatch: function (e) { e._submit_bubble && (delete e._submit_bubble, this.parentNode && !e.isTrigger && v.event.simulate("submit", this.parentNode, e, !0)) }, teardown: function () { if (v.nodeName(this, "form")) return !1; v.event.remove(this, "._submit") } }), v.support.changeBubbles || (v.event.special.change = { setup: function () { if ($.test(this.nodeName)) { if (this.type === "checkbox" || this.type === "radio") v.event.add(this, "propertychange._change", function (e) { e.originalEvent.propertyName === "checked" && (this._just_changed = !0) }), v.event.add(this, "click._change", function (e) { this._just_changed && !e.isTrigger && (this._just_changed = !1), v.event.simulate("change", this, e, !0) }); return !1 } v.event.add(this, "beforeactivate._change", function (e) { var t = e.target; $.test(t.nodeName) && !v._data(t, "_change_attached") && (v.event.add(t, "change._change", function (e) { this.parentNode && !e.isSimulated && !e.isTrigger && v.event.simulate("change", this.parentNode, e, !0) }), v._data(t, "_change_attached", !0)) }) }, handle: function (e) { var t = e.target; if (this !== t || e.isSimulated || e.isTrigger || t.type !== "radio" && t.type !== "checkbox") return e.handleObj.handler.apply(this, arguments) }, teardown: function () { return v.event.remove(this, "._change"), !$.test(this.nodeName) } }), v.support.focusinBubbles || v.each({ focus: "focusin", blur: "focusout" }, function (e, t) { var n = 0, r = function (e) { v.event.simulate(t, e.target, v.event.fix(e), !0) }; v.event.special[t] = { setup: function () { n++ === 0 && i.addEventListener(e, r, !0) }, teardown: function () { --n === 0 && i.removeEventListener(e, r, !0) } } }), v.fn.extend({ on: function (e, n, r, i, s) { var o, u; if (typeof e == "object") { typeof n != "string" && (r = r || n, n = t); for (u in e) this.on(u, n, r, e[u], s); return this } r == null && i == null ? (i = n, r = n = t) : i == null && (typeof n == "string" ? (i = r, r = t) : (i = r, r = n, n = t)); if (i === !1) i = et; else if (!i) return this; return s === 1 && (o = i, i = function (e) { return v().off(e), o.apply(this, arguments) }, i.guid = o.guid || (o.guid = v.guid++)), this.each(function () { v.event.add(this, e, i, r, n) }) }, one: function (e, t, n, r) { return this.on(e, t, n, r, 1) }, off: function (e, n, r) { var i, s; if (e && e.preventDefault && e.handleObj) return i = e.handleObj, v(e.delegateTarget).off(i.namespace ? i.origType + "." + i.namespace : i.origType, i.selector, i.handler), this; if (typeof e == "object") { for (s in e) this.off(s, n, e[s]); return this } if (n === !1 || typeof n == "function") r = n, n = t; return r === !1 && (r = et), this.each(function () { v.event.remove(this, e, r, n) }) }, bind: function (e, t, n) { return this.on(e, null, t, n) }, unbind: function (e, t) { return this.off(e, null, t) }, live: function (e, t, n) { return v(this.context).on(e, this.selector, t, n), this }, die: function (e, t) { return v(this.context).off(e, this.selector || "**", t), this }, delegate: function (e, t, n, r) { return this.on(t, e, n, r) }, undelegate: function (e, t, n) { return arguments.length === 1 ? this.off(e, "**") : this.off(t, e || "**", n) }, trigger: function (e, t) { return this.each(function () { v.event.trigger(e, t, this) }) }, triggerHandler: function (e, t) { if (this[0]) return v.event.trigger(e, t, this[0], !0) }, toggle: function (e) { var t = arguments, n = e.guid || v.guid++, r = 0, i = function (n) { var i = (v._data(this, "lastToggle" + e.guid) || 0) % r; return v._data(this, "lastToggle" + e.guid, i + 1), n.preventDefault(), t[i].apply(this, arguments) || !1 }; i.guid = n; while (r < t.length) t[r++].guid = n; return this.click(i) }, hover: function (e, t) { return this.mouseenter(e).mouseleave(t || e) } }), v.each("blur focus focusin focusout load resize scroll unload click dblclick mousedown mouseup mousemove mouseover mouseout mouseenter mouseleave change select submit keydown keypress keyup error contextmenu".split(" "), function (e, t) { v.fn[t] = function (e, n) { return n == null && (n = e, e = null), arguments.length > 0 ? this.on(t, null, e, n) : this.trigger(t) }, Q.test(t) && (v.event.fixHooks[t] = v.event.keyHooks), G.test(t) && (v.event.fixHooks[t] = v.event.mouseHooks) }), function (e, t) { function nt(e, t, n, r) { n = n || [], t = t || g; var i, s, a, f, l = t.nodeType; if (!e || typeof e != "string") return n; if (l !== 1 && l !== 9) return []; a = o(t); if (!a && !r) if (i = R.exec(e)) if (f = i[1]) { if (l === 9) { s = t.getElementById(f); if (!s || !s.parentNode) return n; if (s.id === f) return n.push(s), n } else if (t.ownerDocument && (s = t.ownerDocument.getElementById(f)) && u(t, s) && s.id === f) return n.push(s), n } else { if (i[2]) return S.apply(n, x.call(t.getElementsByTagName(e), 0)), n; if ((f = i[3]) && Z && t.getElementsByClassName) return S.apply(n, x.call(t.getElementsByClassName(f), 0)), n } return vt(e.replace(j, "$1"), t, n, r, a) } function rt(e) { return function (t) { var n = t.nodeName.toLowerCase(); return n === "input" && t.type === e } } function it(e) { return function (t) { var n = t.nodeName.toLowerCase(); return (n === "input" || n === "button") && t.type === e } } function st(e) { return N(function (t) { return t = +t, N(function (n, r) { var i, s = e([], n.length, t), o = s.length; while (o--) n[i = s[o]] && (n[i] = !(r[i] = n[i])) }) }) } function ot(e, t, n) { if (e === t) return n; var r = e.nextSibling; while (r) { if (r === t) return -1; r = r.nextSibling } return 1 } function ut(e, t) { var n, r, s, o, u, a, f, l = L[d][e + " "]; if (l) return t ? 0 : l.slice(0); u = e, a = [], f = i.preFilter; while (u) { if (!n || (r = F.exec(u))) r && (u = u.slice(r[0].length) || u), a.push(s = []); n = !1; if (r = I.exec(u)) s.push(n = new m(r.shift())), u = u.slice(n.length), n.type = r[0].replace(j, " "); for (o in i.filter) (r = J[o].exec(u)) && (!f[o] || (r = f[o](r))) && (s.push(n = new m(r.shift())), u = u.slice(n.length), n.type = o, n.matches = r); if (!n) break } return t ? u.length : u ? nt.error(e) : L(e, a).slice(0) } function at(e, t, r) { var i = t.dir, s = r && t.dir === "parentNode", o = w++; return t.first ? function (t, n, r) { while (t = t[i]) if (s || t.nodeType === 1) return e(t, n, r) } : function (t, r, u) { if (!u) { var a, f = b + " " + o + " ", l = f + n; while (t = t[i]) if (s || t.nodeType === 1) { if ((a = t[d]) === l) return t.sizset; if (typeof a == "string" && a.indexOf(f) === 0) { if (t.sizset) return t } else { t[d] = l; if (e(t, r, u)) return t.sizset = !0, t; t.sizset = !1 } } } else while (t = t[i]) if (s || t.nodeType === 1) if (e(t, r, u)) return t } } function ft(e) { return e.length > 1 ? function (t, n, r) { var i = e.length; while (i--) if (!e[i](t, n, r)) return !1; return !0 } : e[0] } function lt(e, t, n, r, i) { var s, o = [], u = 0, a = e.length, f = t != null; for (; u < a; u++) if (s = e[u]) if (!n || n(s, r, i)) o.push(s), f && t.push(u); return o } function ct(e, t, n, r, i, s) { return r && !r[d] && (r = ct(r)), i && !i[d] && (i = ct(i, s)), N(function (s, o, u, a) { var f, l, c, h = [], p = [], d = o.length, v = s || dt(t || "*", u.nodeType ? [u] : u, []), m = e && (s || !t) ? lt(v, h, e, u, a) : v, g = n ? i || (s ? e : d || r) ? [] : o : m; n && n(m, g, u, a); if (r) { f = lt(g, p), r(f, [], u, a), l = f.length; while (l--) if (c = f[l]) g[p[l]] = !(m[p[l]] = c) } if (s) { if (i || e) { if (i) { f = [], l = g.length; while (l--) (c = g[l]) && f.push(m[l] = c); i(null, g = [], f, a) } l = g.length; while (l--) (c = g[l]) && (f = i ? T.call(s, c) : h[l]) > -1 && (s[f] = !(o[f] = c)) } } else g = lt(g === o ? g.splice(d, g.length) : g), i ? i(null, o, g, a) : S.apply(o, g) }) } function ht(e) { var t, n, r, s = e.length, o = i.relative[e[0].type], u = o || i.relative[" "], a = o ? 1 : 0, f = at(function (e) { return e === t }, u, !0), l = at(function (e) { return T.call(t, e) > -1 }, u, !0), h = [function (e, n, r) { return !o && (r || n !== c) || ((t = n).nodeType ? f(e, n, r) : l(e, n, r)) }]; for (; a < s; a++) if (n = i.relative[e[a].type]) h = [at(ft(h), n)]; else { n = i.filter[e[a].type].apply(null, e[a].matches); if (n[d]) { r = ++a; for (; r < s; r++) if (i.relative[e[r].type]) break; return ct(a > 1 && ft(h), a > 1 && e.slice(0, a - 1).join("").replace(j, "$1"), n, a < r && ht(e.slice(a, r)), r < s && ht(e = e.slice(r)), r < s && e.join("")) } h.push(n) } return ft(h) } function pt(e, t) { var r = t.length > 0, s = e.length > 0, o = function (u, a, f, l, h) { var p, d, v, m = [], y = 0, w = "0", x = u && [], T = h != null, N = c, C = u || s && i.find.TAG("*", h && a.parentNode || a), k = b += N == null ? 1 : Math.E; T && (c = a !== g && a, n = o.el); for (; (p = C[w]) != null; w++) { if (s && p) { for (d = 0; v = e[d]; d++) if (v(p, a, f)) { l.push(p); break } T && (b = k, n = ++o.el) } r && ((p = !v && p) && y--, u && x.push(p)) } y += w; if (r && w !== y) { for (d = 0; v = t[d]; d++) v(x, m, a, f); if (u) { if (y > 0) while (w--) !x[w] && !m[w] && (m[w] = E.call(l)); m = lt(m) } S.apply(l, m), T && !u && m.length > 0 && y + t.length > 1 && nt.uniqueSort(l) } return T && (b = k, c = N), x }; return o.el = 0, r ? N(o) : o } function dt(e, t, n) { var r = 0, i = t.length; for (; r < i; r++) nt(e, t[r], n); return n } function vt(e, t, n, r, s) { var o, u, f, l, c, h = ut(e), p = h.length; if (!r && h.length === 1) { u = h[0] = h[0].slice(0); if (u.length > 2 && (f = u[0]).type === "ID" && t.nodeType === 9 && !s && i.relative[u[1].type]) { t = i.find.ID(f.matches[0].replace($, ""), t, s)[0]; if (!t) return n; e = e.slice(u.shift().length) } for (o = J.POS.test(e) ? -1 : u.length - 1; o >= 0; o--) { f = u[o]; if (i.relative[l = f.type]) break; if (c = i.find[l]) if (r = c(f.matches[0].replace($, ""), z.test(u[0].type) && t.parentNode || t, s)) { u.splice(o, 1), e = r.length && u.join(""); if (!e) return S.apply(n, x.call(r, 0)), n; break } } } return a(e, h)(r, t, s, n, z.test(e)), n } function mt() { } var n, r, i, s, o, u, a, f, l, c, h = !0, p = "undefined", d = ("sizcache" + Math.random()).replace(".", ""), m = String, g = e.document, y = g.documentElement, b = 0, w = 0, E = [].pop, S = [].push, x = [].slice, T = [].indexOf || function (e) { var t = 0, n = this.length; for (; t < n; t++) if (this[t] === e) return t; return -1 }, N = function (e, t) { return e[d] = t == null || t, e }, C = function () { var e = {}, t = []; return N(function (n, r) { return t.push(n) > i.cacheLength && delete e[t.shift()], e[n + " "] = r }, e) }, k = C(), L = C(), A = C(), O = "[\\x20\\t\\r\\n\\f]", M = "(?:\\\\.|[-\\w]|[^\\x00-\\xa0])+", _ = M.replace("w", "w#"), D = "([*^$|!~]?=)", P = "\\[" + O + "*(" + M + ")" + O + "*(?:" + D + O + "*(?:(['\"])((?:\\\\.|[^\\\\])*?)\\3|(" + _ + ")|)|)" + O + "*\\]", H = ":(" + M + ")(?:\\((?:(['\"])((?:\\\\.|[^\\\\])*?)\\2|([^()[\\]]*|(?:(?:" + P + ")|[^:]|\\\\.)*|.*))\\)|)", B = ":(even|odd|eq|gt|lt|nth|first|last)(?:\\(" + O + "*((?:-\\d)?\\d*)" + O + "*\\)|)(?=[^-]|$)", j = new RegExp("^" + O + "+|((?:^|[^\\\\])(?:\\\\.)*)" + O + "+$", "g"), F = new RegExp("^" + O + "*," + O + "*"), I = new RegExp("^" + O + "*([\\x20\\t\\r\\n\\f>+~])" + O + "*"), q = new RegExp(H), R = /^(?:#([\w\-]+)|(\w+)|\.([\w\-]+))$/, U = /^:not/, z = /[\x20\t\r\n\f]*[+~]/, W = /:not\($/, X = /h\d/i, V = /input|select|textarea|button/i, $ = /\\(?!\\)/g, J = { ID: new RegExp("^#(" + M + ")"), CLASS: new RegExp("^\\.(" + M + ")"), NAME: new RegExp("^\\[name=['\"]?(" + M + ")['\"]?\\]"), TAG: new RegExp("^(" + M.replace("w", "w*") + ")"), ATTR: new RegExp("^" + P), PSEUDO: new RegExp("^" + H), POS: new RegExp(B, "i"), CHILD: new RegExp("^:(only|nth|first|last)-child(?:\\(" + O + "*(even|odd|(([+-]|)(\\d*)n|)" + O + "*(?:([+-]|)" + O + "*(\\d+)|))" + O + "*\\)|)", "i"), needsContext: new RegExp("^" + O + "*[>+~]|" + B, "i") }, K = function (e) { var t = g.createElement("div"); try { return e(t) } catch (n) { return !1 } finally { t = null } }, Q = K(function (e) { return e.appendChild(g.createComment("")), !e.getElementsByTagName("*").length }), G = K(function (e) { return e.innerHTML = "<a href='#'></a>", e.firstChild && typeof e.firstChild.getAttribute !== p && e.firstChild.getAttribute("href") === "#" }), Y = K(function (e) { e.innerHTML = "<select></select>"; var t = typeof e.lastChild.getAttribute("multiple"); return t !== "boolean" && t !== "string" }), Z = K(function (e) { return e.innerHTML = "<div class='hidden e'></div><div class='hidden'></div>", !e.getElementsByClassName || !e.getElementsByClassName("e").length ? !1 : (e.lastChild.className = "e", e.getElementsByClassName("e").length === 2) }), et = K(function (e) { e.id = d + 0, e.innerHTML = "<a name='" + d + "'></a><div name='" + d + "'></div>", y.insertBefore(e, y.firstChild); var t = g.getElementsByName && g.getElementsByName(d).length === 2 + g.getElementsByName(d + 0).length; return r = !g.getElementById(d), y.removeChild(e), t }); try { x.call(y.childNodes, 0)[0].nodeType } catch (tt) { x = function (e) { var t, n = []; for (; t = this[e]; e++) n.push(t); return n } } nt.matches = function (e, t) { return nt(e, null, null, t) }, nt.matchesSelector = function (e, t) { return nt(t, null, null, [e]).length > 0 }, s = nt.getText = function (e) { var t, n = "", r = 0, i = e.nodeType; if (i) { if (i === 1 || i === 9 || i === 11) { if (typeof e.textContent == "string") return e.textContent; for (e = e.firstChild; e; e = e.nextSibling) n += s(e) } else if (i === 3 || i === 4) return e.nodeValue } else for (; t = e[r]; r++) n += s(t); return n }, o = nt.isXML = function (e) { var t = e && (e.ownerDocument || e).documentElement; return t ? t.nodeName !== "HTML" : !1 }, u = nt.contains = y.contains ? function (e, t) { var n = e.nodeType === 9 ? e.documentElement : e, r = t && t.parentNode; return e === r || !!(r && r.nodeType === 1 && n.contains && n.contains(r)) } : y.compareDocumentPosition ? function (e, t) { return t && !!(e.compareDocumentPosition(t) & 16) } : function (e, t) { while (t = t.parentNode) if (t === e) return !0; return !1 }, nt.attr = function (e, t) { var n, r = o(e); return r || (t = t.toLowerCase()), (n = i.attrHandle[t]) ? n(e) : r || Y ? e.getAttribute(t) : (n = e.getAttributeNode(t), n ? typeof e[t] == "boolean" ? e[t] ? t : null : n.specified ? n.value : null : null) }, i = nt.selectors = { cacheLength: 50, createPseudo: N, match: J, attrHandle: G ? {} : { href: function (e) { return e.getAttribute("href", 2) }, type: function (e) { return e.getAttribute("type") } }, find: { ID: r ? function (e, t, n) { if (typeof t.getElementById !== p && !n) { var r = t.getElementById(e); return r && r.parentNode ? [r] : [] } } : function (e, n, r) { if (typeof n.getElementById !== p && !r) { var i = n.getElementById(e); return i ? i.id === e || typeof i.getAttributeNode !== p && i.getAttributeNode("id").value === e ? [i] : t : [] } }, TAG: Q ? function (e, t) { if (typeof t.getElementsByTagName !== p) return t.getElementsByTagName(e) } : function (e, t) { var n = t.getElementsByTagName(e); if (e === "*") { var r, i = [], s = 0; for (; r = n[s]; s++) r.nodeType === 1 && i.push(r); return i } return n }, NAME: et && function (e, t) { if (typeof t.getElementsByName !== p) return t.getElementsByName(name) }, CLASS: Z && function (e, t, n) { if (typeof t.getElementsByClassName !== p && !n) return t.getElementsByClassName(e) } }, relative: { ">": { dir: "parentNode", first: !0 }, " ": { dir: "parentNode" }, "+": { dir: "previousSibling", first: !0 }, "~": { dir: "previousSibling" } }, preFilter: { ATTR: function (e) { return e[1] = e[1].replace($, ""), e[3] = (e[4] || e[5] || "").replace($, ""), e[2] === "~=" && (e[3] = " " + e[3] + " "), e.slice(0, 4) }, CHILD: function (e) { return e[1] = e[1].toLowerCase(), e[1] === "nth" ? (e[2] || nt.error(e[0]), e[3] = +(e[3] ? e[4] + (e[5] || 1) : 2 * (e[2] === "even" || e[2] === "odd")), e[4] = +(e[6] + e[7] || e[2] === "odd")) : e[2] && nt.error(e[0]), e }, PSEUDO: function (e) { var t, n; if (J.CHILD.test(e[0])) return null; if (e[3]) e[2] = e[3]; else if (t = e[4]) q.test(t) && (n = ut(t, !0)) && (n = t.indexOf(")", t.length - n) - t.length) && (t = t.slice(0, n), e[0] = e[0].slice(0, n)), e[2] = t; return e.slice(0, 3) } }, filter: { ID: r ? function (e) { return e = e.replace($, ""), function (t) { return t.getAttribute("id") === e } } : function (e) { return e = e.replace($, ""), function (t) { var n = typeof t.getAttributeNode !== p && t.getAttributeNode("id"); return n && n.value === e } }, TAG: function (e) { return e === "*" ? function () { return !0 } : (e = e.replace($, "").toLowerCase(), function (t) { return t.nodeName && t.nodeName.toLowerCase() === e }) }, CLASS: function (e) { var t = k[d][e + " "]; return t || (t = new RegExp("(^|" + O + ")" + e + "(" + O + "|$)")) && k(e, function (e) { return t.test(e.className || typeof e.getAttribute !== p && e.getAttribute("class") || "") }) }, ATTR: function (e, t, n) { return function (r, i) { var s = nt.attr(r, e); return s == null ? t === "!=" : t ? (s += "", t === "=" ? s === n : t === "!=" ? s !== n : t === "^=" ? n && s.indexOf(n) === 0 : t === "*=" ? n && s.indexOf(n) > -1 : t === "$=" ? n && s.substr(s.length - n.length) === n : t === "~=" ? (" " + s + " ").indexOf(n) > -1 : t === "|=" ? s === n || s.substr(0, n.length + 1) === n + "-" : !1) : !0 } }, CHILD: function (e, t, n, r) { return e === "nth" ? function (e) { var t, i, s = e.parentNode; if (n === 1 && r === 0) return !0; if (s) { i = 0; for (t = s.firstChild; t; t = t.nextSibling) if (t.nodeType === 1) { i++; if (e === t) break } } return i -= r, i === n || i % n === 0 && i / n >= 0 } : function (t) { var n = t; switch (e) { case "only": case "first": while (n = n.previousSibling) if (n.nodeType === 1) return !1; if (e === "first") return !0; n = t; case "last": while (n = n.nextSibling) if (n.nodeType === 1) return !1; return !0 } } }, PSEUDO: function (e, t) { var n, r = i.pseudos[e] || i.setFilters[e.toLowerCase()] || nt.error("unsupported pseudo: " + e); return r[d] ? r(t) : r.length > 1 ? (n = [e, e, "", t], i.setFilters.hasOwnProperty(e.toLowerCase()) ? N(function (e, n) { var i, s = r(e, t), o = s.length; while (o--) i = T.call(e, s[o]), e[i] = !(n[i] = s[o]) }) : function (e) { return r(e, 0, n) }) : r } }, pseudos: { not: N(function (e) { var t = [], n = [], r = a(e.replace(j, "$1")); return r[d] ? N(function (e, t, n, i) { var s, o = r(e, null, i, []), u = e.length; while (u--) if (s = o[u]) e[u] = !(t[u] = s) }) : function (e, i, s) { return t[0] = e, r(t, null, s, n), !n.pop() } }), has: N(function (e) { return function (t) { return nt(e, t).length > 0 } }), contains: N(function (e) { return function (t) { return (t.textContent || t.innerText || s(t)).indexOf(e) > -1 } }), enabled: function (e) { return e.disabled === !1 }, disabled: function (e) { return e.disabled === !0 }, checked: function (e) { var t = e.nodeName.toLowerCase(); return t === "input" && !!e.checked || t === "option" && !!e.selected }, selected: function (e) { return e.parentNode && e.parentNode.selectedIndex, e.selected === !0 }, parent: function (e) { return !i.pseudos.empty(e) }, empty: function (e) { var t; e = e.firstChild; while (e) { if (e.nodeName > "@" || (t = e.nodeType) === 3 || t === 4) return !1; e = e.nextSibling } return !0 }, header: function (e) { return X.test(e.nodeName) }, text: function (e) { var t, n; return e.nodeName.toLowerCase() === "input" && (t = e.type) === "text" && ((n = e.getAttribute("type")) == null || n.toLowerCase() === t) }, radio: rt("radio"), checkbox: rt("checkbox"), file: rt("file"), password: rt("password"), image: rt("image"), submit: it("submit"), reset: it("reset"), button: function (e) { var t = e.nodeName.toLowerCase(); return t === "input" && e.type === "button" || t === "button" }, input: function (e) { return V.test(e.nodeName) }, focus: function (e) { var t = e.ownerDocument; return e === t.activeElement && (!t.hasFocus || t.hasFocus()) && !!(e.type || e.href || ~e.tabIndex) }, active: function (e) { return e === e.ownerDocument.activeElement }, first: st(function () { return [0] }), last: st(function (e, t) { return [t - 1] }), eq: st(function (e, t, n) { return [n < 0 ? n + t : n] }), even: st(function (e, t) { for (var n = 0; n < t; n += 2) e.push(n); return e }), odd: st(function (e, t) { for (var n = 1; n < t; n += 2) e.push(n); return e }), lt: st(function (e, t, n) { for (var r = n < 0 ? n + t : n; --r >= 0;) e.push(r); return e }), gt: st(function (e, t, n) { for (var r = n < 0 ? n + t : n; ++r < t;) e.push(r); return e }) } }, f = y.compareDocumentPosition ? function (e, t) { return e === t ? (l = !0, 0) : (!e.compareDocumentPosition || !t.compareDocumentPosition ? e.compareDocumentPosition : e.compareDocumentPosition(t) & 4) ? -1 : 1 } : function (e, t) { if (e === t) return l = !0, 0; if (e.sourceIndex && t.sourceIndex) return e.sourceIndex - t.sourceIndex; var n, r, i = [], s = [], o = e.parentNode, u = t.parentNode, a = o; if (o === u) return ot(e, t); if (!o) return -1; if (!u) return 1; while (a) i.unshift(a), a = a.parentNode; a = u; while (a) s.unshift(a), a = a.parentNode; n = i.length, r = s.length; for (var f = 0; f < n && f < r; f++) if (i[f] !== s[f]) return ot(i[f], s[f]); return f === n ? ot(e, s[f], -1) : ot(i[f], t, 1) }, [0, 0].sort(f), h = !l, nt.uniqueSort = function (e) { var t, n = [], r = 1, i = 0; l = h, e.sort(f); if (l) { for (; t = e[r]; r++) t === e[r - 1] && (i = n.push(r)); while (i--) e.splice(n[i], 1) } return e }, nt.error = function (e) { throw new Error("Syntax error, unrecognized expression: " + e) }, a = nt.compile = function (e, t) { var n, r = [], i = [], s = A[d][e + " "]; if (!s) { t || (t = ut(e)), n = t.length; while (n--) s = ht(t[n]), s[d] ? r.push(s) : i.push(s); s = A(e, pt(i, r)) } return s }, g.querySelectorAll && function () { var e, t = vt, n = /'|\\/g, r = /\=[\x20\t\r\n\f]*([^'"\]]*)[\x20\t\r\n\f]*\]/g, i = [":focus"], s = [":active"], u = y.matchesSelector || y.mozMatchesSelector || y.webkitMatchesSelector || y.oMatchesSelector || y.msMatchesSelector; K(function (e) { e.innerHTML = "<select><option selected=''></option></select>", e.querySelectorAll("[selected]").length || i.push("\\[" + O + "*(?:checked|disabled|ismap|multiple|readonly|selected|value)"), e.querySelectorAll(":checked").length || i.push(":checked") }), K(function (e) { e.innerHTML = "<p test=''></p>", e.querySelectorAll("[test^='']").length && i.push("[*^$]=" + O + "*(?:\"\"|'')"), e.innerHTML = "<input type='hidden'/>", e.querySelectorAll(":enabled").length || i.push(":enabled", ":disabled") }), i = new RegExp(i.join("|")), vt = function (e, r, s, o, u) { if (!o && !u && !i.test(e)) { var a, f, l = !0, c = d, h = r, p = r.nodeType === 9 && e; if (r.nodeType === 1 && r.nodeName.toLowerCase() !== "object") { a = ut(e), (l = r.getAttribute("id")) ? c = l.replace(n, "\\$&") : r.setAttribute("id", c), c = "[id='" + c + "'] ", f = a.length; while (f--) a[f] = c + a[f].join(""); h = z.test(e) && r.parentNode || r, p = a.join(",") } if (p) try { return S.apply(s, x.call(h.querySelectorAll(p), 0)), s } catch (v) { } finally { l || r.removeAttribute("id") } } return t(e, r, s, o, u) }, u && (K(function (t) { e = u.call(t, "div"); try { u.call(t, "[test!='']:sizzle"), s.push("!=", H) } catch (n) { } }), s = new RegExp(s.join("|")), nt.matchesSelector = function (t, n) { n = n.replace(r, "='$1']"); if (!o(t) && !s.test(n) && !i.test(n)) try { var a = u.call(t, n); if (a || e || t.document && t.document.nodeType !== 11) return a } catch (f) { } return nt(n, null, null, [t]).length > 0 }) }(), i.pseudos.nth = i.pseudos.eq, i.filters = mt.prototype = i.pseudos, i.setFilters = new mt, nt.attr = v.attr, v.find = nt, v.expr = nt.selectors, v.expr[":"] = v.expr.pseudos, v.unique = nt.uniqueSort, v.text = nt.getText, v.isXMLDoc = nt.isXML, v.contains = nt.contains }(e); var nt = /Until$/, rt = /^(?:parents|prev(?:Until|All))/, it = /^.[^:#\[\.,]*$/, st = v.expr.match.needsContext, ot = { children: !0, contents: !0, next: !0, prev: !0 }; v.fn.extend({ find: function (e) { var t, n, r, i, s, o, u = this; if (typeof e != "string") return v(e).filter(function () { for (t = 0, n = u.length; t < n; t++) if (v.contains(u[t], this)) return !0 }); o = this.pushStack("", "find", e); for (t = 0, n = this.length; t < n; t++) { r = o.length, v.find(e, this[t], o); if (t > 0) for (i = r; i < o.length; i++) for (s = 0; s < r; s++) if (o[s] === o[i]) { o.splice(i--, 1); break } } return o }, has: function (e) { var t, n = v(e, this), r = n.length; return this.filter(function () { for (t = 0; t < r; t++) if (v.contains(this, n[t])) return !0 }) }, not: function (e) { return this.pushStack(ft(this, e, !1), "not", e) }, filter: function (e) { return this.pushStack(ft(this, e, !0), "filter", e) }, is: function (e) { return !!e && (typeof e == "string" ? st.test(e) ? v(e, this.context).index(this[0]) >= 0 : v.filter(e, this).length > 0 : this.filter(e).length > 0) }, closest: function (e, t) { var n, r = 0, i = this.length, s = [], o = st.test(e) || typeof e != "string" ? v(e, t || this.context) : 0; for (; r < i; r++) { n = this[r]; while (n && n.ownerDocument && n !== t && n.nodeType !== 11) { if (o ? o.index(n) > -1 : v.find.matchesSelector(n, e)) { s.push(n); break } n = n.parentNode } } return s = s.length > 1 ? v.unique(s) : s, this.pushStack(s, "closest", e) }, index: function (e) { return e ? typeof e == "string" ? v.inArray(this[0], v(e)) : v.inArray(e.jquery ? e[0] : e, this) : this[0] && this[0].parentNode ? this.prevAll().length : -1 }, add: function (e, t) { var n = typeof e == "string" ? v(e, t) : v.makeArray(e && e.nodeType ? [e] : e), r = v.merge(this.get(), n); return this.pushStack(ut(n[0]) || ut(r[0]) ? r : v.unique(r)) }, addBack: function (e) { return this.add(e == null ? this.prevObject : this.prevObject.filter(e)) } }), v.fn.andSelf = v.fn.addBack, v.each({ parent: function (e) { var t = e.parentNode; return t && t.nodeType !== 11 ? t : null }, parents: function (e) { return v.dir(e, "parentNode") }, parentsUntil: function (e, t, n) { return v.dir(e, "parentNode", n) }, next: function (e) { return at(e, "nextSibling") }, prev: function (e) { return at(e, "previousSibling") }, nextAll: function (e) { return v.dir(e, "nextSibling") }, prevAll: function (e) { return v.dir(e, "previousSibling") }, nextUntil: function (e, t, n) { return v.dir(e, "nextSibling", n) }, prevUntil: function (e, t, n) { return v.dir(e, "previousSibling", n) }, siblings: function (e) { return v.sibling((e.parentNode || {}).firstChild, e) }, children: function (e) { return v.sibling(e.firstChild) }, contents: function (e) { return v.nodeName(e, "iframe") ? e.contentDocument || e.contentWindow.document : v.merge([], e.childNodes) } }, function (e, t) { v.fn[e] = function (n, r) { var i = v.map(this, t, n); return nt.test(e) || (r = n), r && typeof r == "string" && (i = v.filter(r, i)), i = this.length > 1 && !ot[e] ? v.unique(i) : i, this.length > 1 && rt.test(e) && (i = i.reverse()), this.pushStack(i, e, l.call(arguments).join(",")) } }), v.extend({ filter: function (e, t, n) { return n && (e = ":not(" + e + ")"), t.length === 1 ? v.find.matchesSelector(t[0], e) ? [t[0]] : [] : v.find.matches(e, t) }, dir: function (e, n, r) { var i = [], s = e[n]; while (s && s.nodeType !== 9 && (r === t || s.nodeType !== 1 || !v(s).is(r))) s.nodeType === 1 && i.push(s), s = s[n]; return i }, sibling: function (e, t) { var n = []; for (; e; e = e.nextSibling) e.nodeType === 1 && e !== t && n.push(e); return n } }); var ct = "abbr|article|aside|audio|bdi|canvas|data|datalist|details|figcaption|figure|footer|header|hgroup|mark|meter|nav|output|progress|section|summary|time|video", ht = / jQuery\d+="(?:null|\d+)"/g, pt = /^\s+/, dt = /<(?!area|br|col|embed|hr|img|input|link|meta|param)(([\w:]+)[^>]*)\/>/gi, vt = /<([\w:]+)/, mt = /<tbody/i, gt = /<|&#?\w+;/, yt = /<(?:script|style|link)/i, bt = /<(?:script|object|embed|option|style)/i, wt = new RegExp("<(?:" + ct + ")[\\s/>]", "i"), Et = /^(?:checkbox|radio)$/, St = /checked\s*(?:[^=]|=\s*.checked.)/i, xt = /\/(java|ecma)script/i, Tt = /^\s*<!(?:\[CDATA\[|\-\-)|[\]\-]{2}>\s*$/g, Nt = { option: [1, "<select multiple='multiple'>", "</select>"], legend: [1, "<fieldset>", "</fieldset>"], thead: [1, "<table>", "</table>"], tr: [2, "<table><tbody>", "</tbody></table>"], td: [3, "<table><tbody><tr>", "</tr></tbody></table>"], col: [2, "<table><tbody></tbody><colgroup>", "</colgroup></table>"], area: [1, "<map>", "</map>"], _default: [0, "", ""] }, Ct = lt(i), kt = Ct.appendChild(i.createElement("div")); Nt.optgroup = Nt.option, Nt.tbody = Nt.tfoot = Nt.colgroup = Nt.caption = Nt.thead, Nt.th = Nt.td, v.support.htmlSerialize || (Nt._default = [1, "X<div>", "</div>"]), v.fn.extend({ text: function (e) { return v.access(this, function (e) { return e === t ? v.text(this) : this.empty().append((this[0] && this[0].ownerDocument || i).createTextNode(e)) }, null, e, arguments.length) }, wrapAll: function (e) { if (v.isFunction(e)) return this.each(function (t) { v(this).wrapAll(e.call(this, t)) }); if (this[0]) { var t = v(e, this[0].ownerDocument).eq(0).clone(!0); this[0].parentNode && t.insertBefore(this[0]), t.map(function () { var e = this; while (e.firstChild && e.firstChild.nodeType === 1) e = e.firstChild; return e }).append(this) } return this }, wrapInner: function (e) { return v.isFunction(e) ? this.each(function (t) { v(this).wrapInner(e.call(this, t)) }) : this.each(function () { var t = v(this), n = t.contents(); n.length ? n.wrapAll(e) : t.append(e) }) }, wrap: function (e) { var t = v.isFunction(e); return this.each(function (n) { v(this).wrapAll(t ? e.call(this, n) : e) }) }, unwrap: function () { return this.parent().each(function () { v.nodeName(this, "body") || v(this).replaceWith(this.childNodes) }).end() }, append: function () { return this.domManip(arguments, !0, function (e) { (this.nodeType === 1 || this.nodeType === 11) && this.appendChild(e) }) }, prepend: function () { return this.domManip(arguments, !0, function (e) { (this.nodeType === 1 || this.nodeType === 11) && this.insertBefore(e, this.firstChild) }) }, before: function () { if (!ut(this[0])) return this.domManip(arguments, !1, function (e) { this.parentNode.insertBefore(e, this) }); if (arguments.length) { var e = v.clean(arguments); return this.pushStack(v.merge(e, this), "before", this.selector) } }, after: function () { if (!ut(this[0])) return this.domManip(arguments, !1, function (e) { this.parentNode.insertBefore(e, this.nextSibling) }); if (arguments.length) { var e = v.clean(arguments); return this.pushStack(v.merge(this, e), "after", this.selector) } }, remove: function (e, t) { var n, r = 0; for (; (n = this[r]) != null; r++) if (!e || v.filter(e, [n]).length) !t && n.nodeType === 1 && (v.cleanData(n.getElementsByTagName("*")), v.cleanData([n])), n.parentNode && n.parentNode.removeChild(n); return this }, empty: function () { var e, t = 0; for (; (e = this[t]) != null; t++) { e.nodeType === 1 && v.cleanData(e.getElementsByTagName("*")); while (e.firstChild) e.removeChild(e.firstChild) } return this }, clone: function (e, t) { return e = e == null ? !1 : e, t = t == null ? e : t, this.map(function () { return v.clone(this, e, t) }) }, html: function (e) { return v.access(this, function (e) { var n = this[0] || {}, r = 0, i = this.length; if (e === t) return n.nodeType === 1 ? n.innerHTML.replace(ht, "") : t; if (typeof e == "string" && !yt.test(e) && (v.support.htmlSerialize || !wt.test(e)) && (v.support.leadingWhitespace || !pt.test(e)) && !Nt[(vt.exec(e) || ["", ""])[1].toLowerCase()]) { e = e.replace(dt, "<$1></$2>"); try { for (; r < i; r++) n = this[r] || {}, n.nodeType === 1 && (v.cleanData(n.getElementsByTagName("*")), n.innerHTML = e); n = 0 } catch (s) { } } n && this.empty().append(e) }, null, e, arguments.length) }, replaceWith: function (e) { return ut(this[0]) ? this.length ? this.pushStack(v(v.isFunction(e) ? e() : e), "replaceWith", e) : this : v.isFunction(e) ? this.each(function (t) { var n = v(this), r = n.html(); n.replaceWith(e.call(this, t, r)) }) : (typeof e != "string" && (e = v(e).detach()), this.each(function () { var t = this.nextSibling, n = this.parentNode; v(this).remove(), t ? v(t).before(e) : v(n).append(e) })) }, detach: function (e) { return this.remove(e, !0) }, domManip: function (e, n, r) { e = [].concat.apply([], e); var i, s, o, u, a = 0, f = e[0], l = [], c = this.length; if (!v.support.checkClone && c > 1 && typeof f == "string" && St.test(f)) return this.each(function () { v(this).domManip(e, n, r) }); if (v.isFunction(f)) return this.each(function (i) { var s = v(this); e[0] = f.call(this, i, n ? s.html() : t), s.domManip(e, n, r) }); if (this[0]) { i = v.buildFragment(e, this, l), o = i.fragment, s = o.firstChild, o.childNodes.length === 1 && (o = s); if (s) { n = n && v.nodeName(s, "tr"); for (u = i.cacheable || c - 1; a < c; a++) r.call(n && v.nodeName(this[a], "table") ? Lt(this[a], "tbody") : this[a], a === u ? o : v.clone(o, !0, !0)) } o = s = null, l.length && v.each(l, function (e, t) { t.src ? v.ajax ? v.ajax({ url: t.src, type: "GET", dataType: "script", async: !1, global: !1, "throws": !0 }) : v.error("no ajax") : v.globalEval((t.text || t.textContent || t.innerHTML || "").replace(Tt, "")), t.parentNode && t.parentNode.removeChild(t) }) } return this } }), v.buildFragment = function (e, n, r) { var s, o, u, a = e[0]; return n = n || i, n = !n.nodeType && n[0] || n, n = n.ownerDocument || n, e.length === 1 && typeof a == "string" && a.length < 512 && n === i && a.charAt(0) === "<" && !bt.test(a) && (v.support.checkClone || !St.test(a)) && (v.support.html5Clone || !wt.test(a)) && (o = !0, s = v.fragments[a], u = s !== t), s || (s = n.createDocumentFragment(), v.clean(e, n, s, r), o && (v.fragments[a] = u && s)), { fragment: s, cacheable: o } }, v.fragments = {}, v.each({ appendTo: "append", prependTo: "prepend", insertBefore: "before", insertAfter: "after", replaceAll: "replaceWith" }, function (e, t) { v.fn[e] = function (n) { var r, i = 0, s = [], o = v(n), u = o.length, a = this.length === 1 && this[0].parentNode; if ((a == null || a && a.nodeType === 11 && a.childNodes.length === 1) && u === 1) return o[t](this[0]), this; for (; i < u; i++) r = (i > 0 ? this.clone(!0) : this).get(), v(o[i])[t](r), s = s.concat(r); return this.pushStack(s, e, o.selector) } }), v.extend({ clone: function (e, t, n) { var r, i, s, o; v.support.html5Clone || v.isXMLDoc(e) || !wt.test("<" + e.nodeName + ">") ? o = e.cloneNode(!0) : (kt.innerHTML = e.outerHTML, kt.removeChild(o = kt.firstChild)); if ((!v.support.noCloneEvent || !v.support.noCloneChecked) && (e.nodeType === 1 || e.nodeType === 11) && !v.isXMLDoc(e)) { Ot(e, o), r = Mt(e), i = Mt(o); for (s = 0; r[s]; ++s) i[s] && Ot(r[s], i[s]) } if (t) { At(e, o); if (n) { r = Mt(e), i = Mt(o); for (s = 0; r[s]; ++s) At(r[s], i[s]) } } return r = i = null, o }, clean: function (e, t, n, r) { var s, o, u, a, f, l, c, h, p, d, m, g, y = t === i && Ct, b = []; if (!t || typeof t.createDocumentFragment == "undefined") t = i; for (s = 0; (u = e[s]) != null; s++) { typeof u == "number" && (u += ""); if (!u) continue; if (typeof u == "string") if (!gt.test(u)) u = t.createTextNode(u); else { y = y || lt(t), c = t.createElement("div"), y.appendChild(c), u = u.replace(dt, "<$1></$2>"), a = (vt.exec(u) || ["", ""])[1].toLowerCase(), f = Nt[a] || Nt._default, l = f[0], c.innerHTML = f[1] + u + f[2]; while (l--) c = c.lastChild; if (!v.support.tbody) { h = mt.test(u), p = a === "table" && !h ? c.firstChild && c.firstChild.childNodes : f[1] === "<table>" && !h ? c.childNodes : []; for (o = p.length - 1; o >= 0; --o) v.nodeName(p[o], "tbody") && !p[o].childNodes.length && p[o].parentNode.removeChild(p[o]) } !v.support.leadingWhitespace && pt.test(u) && c.insertBefore(t.createTextNode(pt.exec(u)[0]), c.firstChild), u = c.childNodes, c.parentNode.removeChild(c) } u.nodeType ? b.push(u) : v.merge(b, u) } c && (u = c = y = null); if (!v.support.appendChecked) for (s = 0; (u = b[s]) != null; s++) v.nodeName(u, "input") ? _t(u) : typeof u.getElementsByTagName != "undefined" && v.grep(u.getElementsByTagName("input"), _t); if (n) { m = function (e) { if (!e.type || xt.test(e.type)) return r ? r.push(e.parentNode ? e.parentNode.removeChild(e) : e) : n.appendChild(e) }; for (s = 0; (u = b[s]) != null; s++) if (!v.nodeName(u, "script") || !m(u)) n.appendChild(u), typeof u.getElementsByTagName != "undefined" && (g = v.grep(v.merge([], u.getElementsByTagName("script")), m), b.splice.apply(b, [s + 1, 0].concat(g)), s += g.length) } return b }, cleanData: function (e, t) { var n, r, i, s, o = 0, u = v.expando, a = v.cache, f = v.support.deleteExpando, l = v.event.special; for (; (i = e[o]) != null; o++) if (t || v.acceptData(i)) { r = i[u], n = r && a[r]; if (n) { if (n.events) for (s in n.events) l[s] ? v.event.remove(i, s) : v.removeEvent(i, s, n.handle); a[r] && (delete a[r], f ? delete i[u] : i.removeAttribute ? i.removeAttribute(u) : i[u] = null, v.deletedIds.push(r)) } } } }), function () { var e, t; v.uaMatch = function (e) { e = e.toLowerCase(); var t = /(chrome)[ \/]([\w.]+)/.exec(e) || /(webkit)[ \/]([\w.]+)/.exec(e) || /(opera)(?:.*version|)[ \/]([\w.]+)/.exec(e) || /(msie) ([\w.]+)/.exec(e) || e.indexOf("compatible") < 0 && /(mozilla)(?:.*? rv:([\w.]+)|)/.exec(e) || []; return { browser: t[1] || "", version: t[2] || "0" } }, e = v.uaMatch(o.userAgent), t = {}, e.browser && (t[e.browser] = !0, t.version = e.version), t.chrome ? t.webkit = !0 : t.webkit && (t.safari = !0), v.browser = t, v.sub = function () { function e(t, n) { return new e.fn.init(t, n) } v.extend(!0, e, this), e.superclass = this, e.fn = e.prototype = this(), e.fn.constructor = e, e.sub = this.sub, e.fn.init = function (r, i) { return i && i instanceof v && !(i instanceof e) && (i = e(i)), v.fn.init.call(this, r, i, t) }, e.fn.init.prototype = e.fn; var t = e(i); return e } }(); var Dt, Pt, Ht, Bt = /alpha\([^)]*\)/i, jt = /opacity=([^)]*)/, Ft = /^(top|right|bottom|left)$/, It = /^(none|table(?!-c[ea]).+)/, qt = /^margin/, Rt = new RegExp("^(" + m + ")(.*)$", "i"), Ut = new RegExp("^(" + m + ")(?!px)[a-z%]+$", "i"), zt = new RegExp("^([-+])=(" + m + ")", "i"), Wt = { BODY: "block" }, Xt = { position: "absolute", visibility: "hidden", display: "block" }, Vt = { letterSpacing: 0, fontWeight: 400 }, $t = ["Top", "Right", "Bottom", "Left"], Jt = ["Webkit", "O", "Moz", "ms"], Kt = v.fn.toggle; v.fn.extend({ css: function (e, n) { return v.access(this, function (e, n, r) { return r !== t ? v.style(e, n, r) : v.css(e, n) }, e, n, arguments.length > 1) }, show: function () { return Yt(this, !0) }, hide: function () { return Yt(this) }, toggle: function (e, t) { var n = typeof e == "boolean"; return v.isFunction(e) && v.isFunction(t) ? Kt.apply(this, arguments) : this.each(function () { (n ? e : Gt(this)) ? v(this).show() : v(this).hide() }) } }), v.extend({ cssHooks: { opacity: { get: function (e, t) { if (t) { var n = Dt(e, "opacity"); return n === "" ? "1" : n } } } }, cssNumber: { fillOpacity: !0, fontWeight: !0, lineHeight: !0, opacity: !0, orphans: !0, widows: !0, zIndex: !0, zoom: !0 }, cssProps: { "float": v.support.cssFloat ? "cssFloat" : "styleFloat" }, style: function (e, n, r, i) { if (!e || e.nodeType === 3 || e.nodeType === 8 || !e.style) return; var s, o, u, a = v.camelCase(n), f = e.style; n = v.cssProps[a] || (v.cssProps[a] = Qt(f, a)), u = v.cssHooks[n] || v.cssHooks[a]; if (r === t) return u && "get" in u && (s = u.get(e, !1, i)) !== t ? s : f[n]; o = typeof r, o === "string" && (s = zt.exec(r)) && (r = (s[1] + 1) * s[2] + parseFloat(v.css(e, n)), o = "number"); if (r == null || o === "number" && isNaN(r)) return; o === "number" && !v.cssNumber[a] && (r += "px"); if (!u || !("set" in u) || (r = u.set(e, r, i)) !== t) try { f[n] = r } catch (l) { } }, css: function (e, n, r, i) { var s, o, u, a = v.camelCase(n); return n = v.cssProps[a] || (v.cssProps[a] = Qt(e.style, a)), u = v.cssHooks[n] || v.cssHooks[a], u && "get" in u && (s = u.get(e, !0, i)), s === t && (s = Dt(e, n)), s === "normal" && n in Vt && (s = Vt[n]), r || i !== t ? (o = parseFloat(s), r || v.isNumeric(o) ? o || 0 : s) : s }, swap: function (e, t, n) { var r, i, s = {}; for (i in t) s[i] = e.style[i], e.style[i] = t[i]; r = n.call(e); for (i in t) e.style[i] = s[i]; return r } }), e.getComputedStyle ? Dt = function (t, n) { var r, i, s, o, u = e.getComputedStyle(t, null), a = t.style; return u && (r = u.getPropertyValue(n) || u[n], r === "" && !v.contains(t.ownerDocument, t) && (r = v.style(t, n)), Ut.test(r) && qt.test(n) && (i = a.width, s = a.minWidth, o = a.maxWidth, a.minWidth = a.maxWidth = a.width = r, r = u.width, a.width = i, a.minWidth = s, a.maxWidth = o)), r } : i.documentElement.currentStyle && (Dt = function (e, t) { var n, r, i = e.currentStyle && e.currentStyle[t], s = e.style; return i == null && s && s[t] && (i = s[t]), Ut.test(i) && !Ft.test(t) && (n = s.left, r = e.runtimeStyle && e.runtimeStyle.left, r && (e.runtimeStyle.left = e.currentStyle.left), s.left = t === "fontSize" ? "1em" : i, i = s.pixelLeft + "px", s.left = n, r && (e.runtimeStyle.left = r)), i === "" ? "auto" : i }), v.each(["height", "width"], function (e, t) { v.cssHooks[t] = { get: function (e, n, r) { if (n) return e.offsetWidth === 0 && It.test(Dt(e, "display")) ? v.swap(e, Xt, function () { return tn(e, t, r) }) : tn(e, t, r) }, set: function (e, n, r) { return Zt(e, n, r ? en(e, t, r, v.support.boxSizing && v.css(e, "boxSizing") === "border-box") : 0) } } }), v.support.opacity || (v.cssHooks.opacity = { get: function (e, t) { return jt.test((t && e.currentStyle ? e.currentStyle.filter : e.style.filter) || "") ? .01 * parseFloat(RegExp.$1) + "" : t ? "1" : "" }, set: function (e, t) { var n = e.style, r = e.currentStyle, i = v.isNumeric(t) ? "alpha(opacity=" + t * 100 + ")" : "", s = r && r.filter || n.filter || ""; n.zoom = 1; if (t >= 1 && v.trim(s.replace(Bt, "")) === "" && n.removeAttribute) { n.removeAttribute("filter"); if (r && !r.filter) return } n.filter = Bt.test(s) ? s.replace(Bt, i) : s + " " + i } }), v(function () { v.support.reliableMarginRight || (v.cssHooks.marginRight = { get: function (e, t) { return v.swap(e, { display: "inline-block" }, function () { if (t) return Dt(e, "marginRight") }) } }), !v.support.pixelPosition && v.fn.position && v.each(["top", "left"], function (e, t) { v.cssHooks[t] = { get: function (e, n) { if (n) { var r = Dt(e, t); return Ut.test(r) ? v(e).position()[t] + "px" : r } } } }) }), v.expr && v.expr.filters && (v.expr.filters.hidden = function (e) { return e.offsetWidth === 0 && e.offsetHeight === 0 || !v.support.reliableHiddenOffsets && (e.style && e.style.display || Dt(e, "display")) === "none" }, v.expr.filters.visible = function (e) { return !v.expr.filters.hidden(e) }), v.each({ margin: "", padding: "", border: "Width" }, function (e, t) { v.cssHooks[e + t] = { expand: function (n) { var r, i = typeof n == "string" ? n.split(" ") : [n], s = {}; for (r = 0; r < 4; r++) s[e + $t[r] + t] = i[r] || i[r - 2] || i[0]; return s } }, qt.test(e) || (v.cssHooks[e + t].set = Zt) }); var rn = /%20/g, sn = /\[\]$/, on = /\r?\n/g, un = /^(?:color|date|datetime|datetime-local|email|hidden|month|number|password|range|search|tel|text|time|url|week)$/i, an = /^(?:select|textarea)/i; v.fn.extend({ serialize: function () { return v.param(this.serializeArray()) }, serializeArray: function () { return this.map(function () { return this.elements ? v.makeArray(this.elements) : this }).filter(function () { return this.name && !this.disabled && (this.checked || an.test(this.nodeName) || un.test(this.type)) }).map(function (e, t) { var n = v(this).val(); return n == null ? null : v.isArray(n) ? v.map(n, function (e, n) { return { name: t.name, value: e.replace(on, "\r\n") } }) : { name: t.name, value: n.replace(on, "\r\n") } }).get() } }), v.param = function (e, n) { var r, i = [], s = function (e, t) { t = v.isFunction(t) ? t() : t == null ? "" : t, i[i.length] = encodeURIComponent(e) + "=" + encodeURIComponent(t) }; n === t && (n = v.ajaxSettings && v.ajaxSettings.traditional); if (v.isArray(e) || e.jquery && !v.isPlainObject(e)) v.each(e, function () { s(this.name, this.value) }); else for (r in e) fn(r, e[r], n, s); return i.join("&").replace(rn, "+") }; var ln, cn, hn = /#.*$/, pn = /^(.*?):[ \t]*([^\r\n]*)\r?$/mg, dn = /^(?:about|app|app\-storage|.+\-extension|file|res|widget):$/, vn = /^(?:GET|HEAD)$/, mn = /^\/\//, gn = /\?/, yn = /<script\b[^<]*(?:(?!<\/script>)<[^<]*)*<\/script>/gi, bn = /([?&])_=[^&]*/, wn = /^([\w\+\.\-]+:)(?:\/\/([^\/?#:]*)(?::(\d+)|)|)/, En = v.fn.load, Sn = {}, xn = {}, Tn = ["*/"] + ["*"]; try { cn = s.href } catch (Nn) { cn = i.createElement("a"), cn.href = "", cn = cn.href } ln = wn.exec(cn.toLowerCase()) || [], v.fn.load = function (e, n, r) { if (typeof e != "string" && En) return En.apply(this, arguments); if (!this.length) return this; var i, s, o, u = this, a = e.indexOf(" "); return a >= 0 && (i = e.slice(a, e.length), e = e.slice(0, a)), v.isFunction(n) ? (r = n, n = t) : n && typeof n == "object" && (s = "POST"), v.ajax({ url: e, type: s, dataType: "html", data: n, complete: function (e, t) { r && u.each(r, o || [e.responseText, t, e]) } }).done(function (e) { o = arguments, u.html(i ? v("<div>").append(e.replace(yn, "")).find(i) : e) }), this }, v.each("ajaxStart ajaxStop ajaxComplete ajaxError ajaxSuccess ajaxSend".split(" "), function (e, t) { v.fn[t] = function (e) { return this.on(t, e) } }), v.each(["get", "post"], function (e, n) { v[n] = function (e, r, i, s) { return v.isFunction(r) && (s = s || i, i = r, r = t), v.ajax({ type: n, url: e, data: r, success: i, dataType: s }) } }), v.extend({ getScript: function (e, n) { return v.get(e, t, n, "script") }, getJSON: function (e, t, n) { return v.get(e, t, n, "json") }, ajaxSetup: function (e, t) { return t ? Ln(e, v.ajaxSettings) : (t = e, e = v.ajaxSettings), Ln(e, t), e }, ajaxSettings: { url: cn, isLocal: dn.test(ln[1]), global: !0, type: "GET", contentType: "application/x-www-form-urlencoded; charset=UTF-8", processData: !0, async: !0, accepts: { xml: "application/xml, text/xml", html: "text/html", text: "text/plain", json: "application/json, text/javascript", "*": Tn }, contents: { xml: /xml/, html: /html/, json: /json/ }, responseFields: { xml: "responseXML", text: "responseText" }, converters: { "* text": e.String, "text html": !0, "text json": v.parseJSON, "text xml": v.parseXML }, flatOptions: { context: !0, url: !0 } }, ajaxPrefilter: Cn(Sn), ajaxTransport: Cn(xn), ajax: function (e, n) { function T(e, n, s, a) { var l, y, b, w, S, T = n; if (E === 2) return; E = 2, u && clearTimeout(u), o = t, i = a || "", x.readyState = e > 0 ? 4 : 0, s && (w = An(c, x, s)); if (e >= 200 && e < 300 || e === 304) c.ifModified && (S = x.getResponseHeader("Last-Modified"), S && (v.lastModified[r] = S), S = x.getResponseHeader("Etag"), S && (v.etag[r] = S)), e === 304 ? (T = "notmodified", l = !0) : (l = On(c, w), T = l.state, y = l.data, b = l.error, l = !b); else { b = T; if (!T || e) T = "error", e < 0 && (e = 0) } x.status = e, x.statusText = (n || T) + "", l ? d.resolveWith(h, [y, T, x]) : d.rejectWith(h, [x, T, b]), x.statusCode(g), g = t, f && p.trigger("ajax" + (l ? "Success" : "Error"), [x, c, l ? y : b]), m.fireWith(h, [x, T]), f && (p.trigger("ajaxComplete", [x, c]), --v.active || v.event.trigger("ajaxStop")) } typeof e == "object" && (n = e, e = t), n = n || {}; var r, i, s, o, u, a, f, l, c = v.ajaxSetup({}, n), h = c.context || c, p = h !== c && (h.nodeType || h instanceof v) ? v(h) : v.event, d = v.Deferred(), m = v.Callbacks("once memory"), g = c.statusCode || {}, b = {}, w = {}, E = 0, S = "canceled", x = { readyState: 0, setRequestHeader: function (e, t) { if (!E) { var n = e.toLowerCase(); e = w[n] = w[n] || e, b[e] = t } return this }, getAllResponseHeaders: function () { return E === 2 ? i : null }, getResponseHeader: function (e) { var n; if (E === 2) { if (!s) { s = {}; while (n = pn.exec(i)) s[n[1].toLowerCase()] = n[2] } n = s[e.toLowerCase()] } return n === t ? null : n }, overrideMimeType: function (e) { return E || (c.mimeType = e), this }, abort: function (e) { return e = e || S, o && o.abort(e), T(0, e), this } }; d.promise(x), x.success = x.done, x.error = x.fail, x.complete = m.add, x.statusCode = function (e) { if (e) { var t; if (E < 2) for (t in e) g[t] = [g[t], e[t]]; else t = e[x.status], x.always(t) } return this }, c.url = ((e || c.url) + "").replace(hn, "").replace(mn, ln[1] + "//"), c.dataTypes = v.trim(c.dataType || "*").toLowerCase().split(y), c.crossDomain == null && (a = wn.exec(c.url.toLowerCase()), c.crossDomain = !(!a || a[1] === ln[1] && a[2] === ln[2] && (a[3] || (a[1] === "http:" ? 80 : 443)) == (ln[3] || (ln[1] === "http:" ? 80 : 443)))), c.data && c.processData && typeof c.data != "string" && (c.data = v.param(c.data, c.traditional)), kn(Sn, c, n, x); if (E === 2) return x; f = c.global, c.type = c.type.toUpperCase(), c.hasContent = !vn.test(c.type), f && v.active++ === 0 && v.event.trigger("ajaxStart"); if (!c.hasContent) { c.data && (c.url += (gn.test(c.url) ? "&" : "?") + c.data, delete c.data), r = c.url; if (c.cache === !1) { var N = v.now(), C = c.url.replace(bn, "$1_=" + N); c.url = C + (C === c.url ? (gn.test(c.url) ? "&" : "?") + "_=" + N : "") } } (c.data && c.hasContent && c.contentType !== !1 || n.contentType) && x.setRequestHeader("Content-Type", c.contentType), c.ifModified && (r = r || c.url, v.lastModified[r] && x.setRequestHeader("If-Modified-Since", v.lastModified[r]), v.etag[r] && x.setRequestHeader("If-None-Match", v.etag[r])), x.setRequestHeader("Accept", c.dataTypes[0] && c.accepts[c.dataTypes[0]] ? c.accepts[c.dataTypes[0]] + (c.dataTypes[0] !== "*" ? ", " + Tn + "; q=0.01" : "") : c.accepts["*"]); for (l in c.headers) x.setRequestHeader(l, c.headers[l]); if (!c.beforeSend || c.beforeSend.call(h, x, c) !== !1 && E !== 2) { S = "abort"; for (l in { success: 1, error: 1, complete: 1 }) x[l](c[l]); o = kn(xn, c, n, x); if (!o) T(-1, "No Transport"); else { x.readyState = 1, f && p.trigger("ajaxSend", [x, c]), c.async && c.timeout > 0 && (u = setTimeout(function () { x.abort("timeout") }, c.timeout)); try { E = 1, o.send(b, T) } catch (k) { if (!(E < 2)) throw k; T(-1, k) } } return x } return x.abort() }, active: 0, lastModified: {}, etag: {} }); var Mn = [], _n = /\?/, Dn = /(=)\?(?=&|$)|\?\?/, Pn = v.now(); v.ajaxSetup({ jsonp: "callback", jsonpCallback: function () { var e = Mn.pop() || v.expando + "_" + Pn++; return this[e] = !0, e } }), v.ajaxPrefilter("json jsonp", function (n, r, i) { var s, o, u, a = n.data, f = n.url, l = n.jsonp !== !1, c = l && Dn.test(f), h = l && !c && typeof a == "string" && !(n.contentType || "").indexOf("application/x-www-form-urlencoded") && Dn.test(a); if (n.dataTypes[0] === "jsonp" || c || h) return s = n.jsonpCallback = v.isFunction(n.jsonpCallback) ? n.jsonpCallback() : n.jsonpCallback, o = e[s], c ? n.url = f.replace(Dn, "$1" + s) : h ? n.data = a.replace(Dn, "$1" + s) : l && (n.url += (_n.test(f) ? "&" : "?") + n.jsonp + "=" + s), n.converters["script json"] = function () { return u || v.error(s + " was not called"), u[0] }, n.dataTypes[0] = "json", e[s] = function () { u = arguments }, i.always(function () { e[s] = o, n[s] && (n.jsonpCallback = r.jsonpCallback, Mn.push(s)), u && v.isFunction(o) && o(u[0]), u = o = t }), "script" }), v.ajaxSetup({ accepts: { script: "text/javascript, application/javascript, application/ecmascript, application/x-ecmascript" }, contents: { script: /javascript|ecmascript/ }, converters: { "text script": function (e) { return v.globalEval(e), e } } }), v.ajaxPrefilter("script", function (e) { e.cache === t && (e.cache = !1), e.crossDomain && (e.type = "GET", e.global = !1) }), v.ajaxTransport("script", function (e) { if (e.crossDomain) { var n, r = i.head || i.getElementsByTagName("head")[0] || i.documentElement; return { send: function (s, o) { n = i.createElement("script"), n.async = "async", e.scriptCharset && (n.charset = e.scriptCharset), n.src = e.url, n.onload = n.onreadystatechange = function (e, i) { if (i || !n.readyState || /loaded|complete/.test(n.readyState)) n.onload = n.onreadystatechange = null, r && n.parentNode && r.removeChild(n), n = t, i || o(200, "success") }, r.insertBefore(n, r.firstChild) }, abort: function () { n && n.onload(0, 1) } } } }); var Hn, Bn = e.ActiveXObject ? function () { for (var e in Hn) Hn[e](0, 1) } : !1, jn = 0; v.ajaxSettings.xhr = e.ActiveXObject ? function () { return !this.isLocal && Fn() || In() } : Fn, function (e) { v.extend(v.support, { ajax: !!e, cors: !!e && "withCredentials" in e }) }(v.ajaxSettings.xhr()), v.support.ajax && v.ajaxTransport(function (n) { if (!n.crossDomain || v.support.cors) { var r; return { send: function (i, s) { var o, u, a = n.xhr(); n.username ? a.open(n.type, n.url, n.async, n.username, n.password) : a.open(n.type, n.url, n.async); if (n.xhrFields) for (u in n.xhrFields) a[u] = n.xhrFields[u]; n.mimeType && a.overrideMimeType && a.overrideMimeType(n.mimeType), !n.crossDomain && !i["X-Requested-With"] && (i["X-Requested-With"] = "XMLHttpRequest"); try { for (u in i) a.setRequestHeader(u, i[u]) } catch (f) { } a.send(n.hasContent && n.data || null), r = function (e, i) { var u, f, l, c, h; try { if (r && (i || a.readyState === 4)) { r = t, o && (a.onreadystatechange = v.noop, Bn && delete Hn[o]); if (i) a.readyState !== 4 && a.abort(); else { u = a.status, l = a.getAllResponseHeaders(), c = {}, h = a.responseXML, h && h.documentElement && (c.xml = h); try { c.text = a.responseText } catch (p) { } try { f = a.statusText } catch (p) { f = "" } !u && n.isLocal && !n.crossDomain ? u = c.text ? 200 : 404 : u === 1223 && (u = 204) } } } catch (d) { i || s(-1, d) } c && s(u, f, c, l) }, n.async ? a.readyState === 4 ? setTimeout(r, 0) : (o = ++jn, Bn && (Hn || (Hn = {}, v(e).unload(Bn)), Hn[o] = r), a.onreadystatechange = r) : r() }, abort: function () { r && r(0, 1) } } } }); var qn, Rn, Un = /^(?:toggle|show|hide)$/, zn = new RegExp("^(?:([-+])=|)(" + m + ")([a-z%]*)$", "i"), Wn = /queueHooks$/, Xn = [Gn], Vn = { "*": [function (e, t) { var n, r, i = this.createTween(e, t), s = zn.exec(t), o = i.cur(), u = +o || 0, a = 1, f = 20; if (s) { n = +s[2], r = s[3] || (v.cssNumber[e] ? "" : "px"); if (r !== "px" && u) { u = v.css(i.elem, e, !0) || n || 1; do a = a || ".5", u /= a, v.style(i.elem, e, u + r); while (a !== (a = i.cur() / o) && a !== 1 && --f) } i.unit = r, i.start = u, i.end = s[1] ? u + (s[1] + 1) * n : n } return i }] }; v.Animation = v.extend(Kn, { tweener: function (e, t) { v.isFunction(e) ? (t = e, e = ["*"]) : e = e.split(" "); var n, r = 0, i = e.length; for (; r < i; r++) n = e[r], Vn[n] = Vn[n] || [], Vn[n].unshift(t) }, prefilter: function (e, t) { t ? Xn.unshift(e) : Xn.push(e) } }), v.Tween = Yn, Yn.prototype = { constructor: Yn, init: function (e, t, n, r, i, s) { this.elem = e, this.prop = n, this.easing = i || "swing", this.options = t, this.start = this.now = this.cur(), this.end = r, this.unit = s || (v.cssNumber[n] ? "" : "px") }, cur: function () { var e = Yn.propHooks[this.prop]; return e && e.get ? e.get(this) : Yn.propHooks._default.get(this) }, run: function (e) { var t, n = Yn.propHooks[this.prop]; return this.options.duration ? this.pos = t = v.easing[this.easing](e, this.options.duration * e, 0, 1, this.options.duration) : this.pos = t = e, this.now = (this.end - this.start) * t + this.start, this.options.step && this.options.step.call(this.elem, this.now, this), n && n.set ? n.set(this) : Yn.propHooks._default.set(this), this } }, Yn.prototype.init.prototype = Yn.prototype, Yn.propHooks = { _default: { get: function (e) { var t; return e.elem[e.prop] == null || !!e.elem.style && e.elem.style[e.prop] != null ? (t = v.css(e.elem, e.prop, !1, ""), !t || t === "auto" ? 0 : t) : e.elem[e.prop] }, set: function (e) { v.fx.step[e.prop] ? v.fx.step[e.prop](e) : e.elem.style && (e.elem.style[v.cssProps[e.prop]] != null || v.cssHooks[e.prop]) ? v.style(e.elem, e.prop, e.now + e.unit) : e.elem[e.prop] = e.now } } }, Yn.propHooks.scrollTop = Yn.propHooks.scrollLeft = { set: function (e) { e.elem.nodeType && e.elem.parentNode && (e.elem[e.prop] = e.now) } }, v.each(["toggle", "show", "hide"], function (e, t) { var n = v.fn[t]; v.fn[t] = function (r, i, s) { return r == null || typeof r == "boolean" || !e && v.isFunction(r) && v.isFunction(i) ? n.apply(this, arguments) : this.animate(Zn(t, !0), r, i, s) } }), v.fn.extend({ fadeTo: function (e, t, n, r) { return this.filter(Gt).css("opacity", 0).show().end().animate({ opacity: t }, e, n, r) }, animate: function (e, t, n, r) { var i = v.isEmptyObject(e), s = v.speed(t, n, r), o = function () { var t = Kn(this, v.extend({}, e), s); i && t.stop(!0) }; return i || s.queue === !1 ? this.each(o) : this.queue(s.queue, o) }, stop: function (e, n, r) { var i = function (e) { var t = e.stop; delete e.stop, t(r) }; return typeof e != "string" && (r = n, n = e, e = t), n && e !== !1 && this.queue(e || "fx", []), this.each(function () { var t = !0, n = e != null && e + "queueHooks", s = v.timers, o = v._data(this); if (n) o[n] && o[n].stop && i(o[n]); else for (n in o) o[n] && o[n].stop && Wn.test(n) && i(o[n]); for (n = s.length; n--;) s[n].elem === this && (e == null || s[n].queue === e) && (s[n].anim.stop(r), t = !1, s.splice(n, 1)); (t || !r) && v.dequeue(this, e) }) } }), v.each({ slideDown: Zn("show"), slideUp: Zn("hide"), slideToggle: Zn("toggle"), fadeIn: { opacity: "show" }, fadeOut: { opacity: "hide" }, fadeToggle: { opacity: "toggle" } }, function (e, t) { v.fn[e] = function (e, n, r) { return this.animate(t, e, n, r) } }), v.speed = function (e, t, n) { var r = e && typeof e == "object" ? v.extend({}, e) : { complete: n || !n && t || v.isFunction(e) && e, duration: e, easing: n && t || t && !v.isFunction(t) && t }; r.duration = v.fx.off ? 0 : typeof r.duration == "number" ? r.duration : r.duration in v.fx.speeds ? v.fx.speeds[r.duration] : v.fx.speeds._default; if (r.queue == null || r.queue === !0) r.queue = "fx"; return r.old = r.complete, r.complete = function () { v.isFunction(r.old) && r.old.call(this), r.queue && v.dequeue(this, r.queue) }, r }, v.easing = { linear: function (e) { return e }, swing: function (e) { return .5 - Math.cos(e * Math.PI) / 2 } }, v.timers = [], v.fx = Yn.prototype.init, v.fx.tick = function () { var e, n = v.timers, r = 0; qn = v.now(); for (; r < n.length; r++) e = n[r], !e() && n[r] === e && n.splice(r--, 1); n.length || v.fx.stop(), qn = t }, v.fx.timer = function (e) { e() && v.timers.push(e) && !Rn && (Rn = setInterval(v.fx.tick, v.fx.interval)) }, v.fx.interval = 13, v.fx.stop = function () { clearInterval(Rn), Rn = null }, v.fx.speeds = { slow: 600, fast: 200, _default: 400 }, v.fx.step = {}, v.expr && v.expr.filters && (v.expr.filters.animated = function (e) { return v.grep(v.timers, function (t) { return e === t.elem }).length }); var er = /^(?:body|html)$/i; v.fn.offset = function (e) { if (arguments.length) return e === t ? this : this.each(function (t) { v.offset.setOffset(this, e, t) }); var n, r, i, s, o, u, a, f = { top: 0, left: 0 }, l = this[0], c = l && l.ownerDocument; if (!c) return; return (r = c.body) === l ? v.offset.bodyOffset(l) : (n = c.documentElement, v.contains(n, l) ? (typeof l.getBoundingClientRect != "undefined" && (f = l.getBoundingClientRect()), i = tr(c), s = n.clientTop || r.clientTop || 0, o = n.clientLeft || r.clientLeft || 0, u = i.pageYOffset || n.scrollTop, a = i.pageXOffset || n.scrollLeft, { top: f.top + u - s, left: f.left + a - o }) : f) }, v.offset = { bodyOffset: function (e) { var t = e.offsetTop, n = e.offsetLeft; return v.support.doesNotIncludeMarginInBodyOffset && (t += parseFloat(v.css(e, "marginTop")) || 0, n += parseFloat(v.css(e, "marginLeft")) || 0), { top: t, left: n } }, setOffset: function (e, t, n) { var r = v.css(e, "position"); r === "static" && (e.style.position = "relative"); var i = v(e), s = i.offset(), o = v.css(e, "top"), u = v.css(e, "left"), a = (r === "absolute" || r === "fixed") && v.inArray("auto", [o, u]) > -1, f = {}, l = {}, c, h; a ? (l = i.position(), c = l.top, h = l.left) : (c = parseFloat(o) || 0, h = parseFloat(u) || 0), v.isFunction(t) && (t = t.call(e, n, s)), t.top != null && (f.top = t.top - s.top + c), t.left != null && (f.left = t.left - s.left + h), "using" in t ? t.using.call(e, f) : i.css(f) } }, v.fn.extend({ position: function () { if (!this[0]) return; var e = this[0], t = this.offsetParent(), n = this.offset(), r = er.test(t[0].nodeName) ? { top: 0, left: 0 } : t.offset(); return n.top -= parseFloat(v.css(e, "marginTop")) || 0, n.left -= parseFloat(v.css(e, "marginLeft")) || 0, r.top += parseFloat(v.css(t[0], "borderTopWidth")) || 0, r.left += parseFloat(v.css(t[0], "borderLeftWidth")) || 0, { top: n.top - r.top, left: n.left - r.left } }, offsetParent: function () { return this.map(function () { var e = this.offsetParent || i.body; while (e && !er.test(e.nodeName) && v.css(e, "position") === "static") e = e.offsetParent; return e || i.body }) } }), v.each({ scrollLeft: "pageXOffset", scrollTop: "pageYOffset" }, function (e, n) { var r = /Y/.test(n); v.fn[e] = function (i) { return v.access(this, function (e, i, s) { var o = tr(e); if (s === t) return o ? n in o ? o[n] : o.document.documentElement[i] : e[i]; o ? o.scrollTo(r ? v(o).scrollLeft() : s, r ? s : v(o).scrollTop()) : e[i] = s }, e, i, arguments.length, null) } }), v.each({ Height: "height", Width: "width" }, function (e, n) { v.each({ padding: "inner" + e, content: n, "": "outer" + e }, function (r, i) { v.fn[i] = function (i, s) { var o = arguments.length && (r || typeof i != "boolean"), u = r || (i === !0 || s === !0 ? "margin" : "border"); return v.access(this, function (n, r, i) { var s; return v.isWindow(n) ? n.document.documentElement["client" + e] : n.nodeType === 9 ? (s = n.documentElement, Math.max(n.body["scroll" + e], s["scroll" + e], n.body["offset" + e], s["offset" + e], s["client" + e])) : i === t ? v.css(n, r, i, u) : v.style(n, r, i, u) }, n, o ? i : t, o, null) } }) }), e.jQuery = e.$ = v, typeof define == "function" && define.amd && define.amd.jQuery && define("jquery", [], function () { return v }) })(window);
}
try {
    if (Enumerable) {
    }
    else {
    }
}
catch (eee) {
    Enumerable = function () { var m = "Single:sequence contains more than one element.", e = true, b = null, a = false, c = function (a) { this.GetEnumerator = a }; c.Choice = function () { var a = arguments[0] instanceof Array ? arguments[0] : arguments; return new c(function () { return new f(g.Blank, function () { return this.Yield(a[Math.floor(Math.random() * a.length)]) }, g.Blank) }) }; c.Cycle = function () { var a = arguments[0] instanceof Array ? arguments[0] : arguments; return new c(function () { var b = 0; return new f(g.Blank, function () { if (b >= a.length) b = 0; return this.Yield(a[b++]) }, g.Blank) }) }; c.Empty = function () { return new c(function () { return new f(g.Blank, function () { return a }, g.Blank) }) }; c.From = function (j) { if (j == b) return c.Empty(); if (j instanceof c) return j; if (typeof j == i.Number || typeof j == i.Boolean) return c.Repeat(j, 1); if (typeof j == i.String) return new c(function () { var b = 0; return new f(g.Blank, function () { return b < j.length ? this.Yield(j.charAt(b++)) : a }, g.Blank) }); if (typeof j != i.Function) { if (typeof j.length == i.Number) return new h(j); if (!(j instanceof Object) && d.IsIEnumerable(j)) return new c(function () { var c = e, b; return new f(function () { b = new Enumerator(j) }, function () { if (c) c = a; else b.moveNext(); return b.atEnd() ? a : this.Yield(b.item()) }, g.Blank) }) } return new c(function () { var b = [], c = 0; return new f(function () { for (var a in j) !(j[a] instanceof Function) && b.push({ Key: a, Value: j[a] }) }, function () { return c < b.length ? this.Yield(b[c++]) : a }, g.Blank) }) }, c.Return = function (a) { return c.Repeat(a, 1) }; c.Matches = function (h, e, d) { if (d == b) d = ""; if (e instanceof RegExp) { d += e.ignoreCase ? "i" : ""; d += e.multiline ? "m" : ""; e = e.source } if (d.indexOf("g") === -1) d += "g"; return new c(function () { var b; return new f(function () { b = new RegExp(e, d) }, function () { var c = b.exec(h); return c ? this.Yield(c) : a }, g.Blank) }) }; c.Range = function (e, d, a) { if (a == b) a = 1; return c.ToInfinity(e, a).Take(d) }; c.RangeDown = function (e, d, a) { if (a == b) a = 1; return c.ToNegativeInfinity(e, a).Take(d) }; c.RangeTo = function (d, e, a) { if (a == b) a = 1; return d < e ? c.ToInfinity(d, a).TakeWhile(function (a) { return a <= e }) : c.ToNegativeInfinity(d, a).TakeWhile(function (a) { return a >= e }) }; c.Repeat = function (d, a) { return a != b ? c.Repeat(d).Take(a) : new c(function () { return new f(g.Blank, function () { return this.Yield(d) }, g.Blank) }) }; c.RepeatWithFinalize = function (a, e) { a = d.CreateLambda(a); e = d.CreateLambda(e); return new c(function () { var c; return new f(function () { c = a() }, function () { return this.Yield(c) }, function () { if (c != b) { e(c); c = b } }) }) }; c.Generate = function (a, e) { if (e != b) return c.Generate(a).Take(e); a = d.CreateLambda(a); return new c(function () { return new f(g.Blank, function () { return this.Yield(a()) }, g.Blank) }) }; c.ToInfinity = function (d, a) { if (d == b) d = 0; if (a == b) a = 1; return new c(function () { var b; return new f(function () { b = d - a }, function () { return this.Yield(b += a) }, g.Blank) }) }; c.ToNegativeInfinity = function (d, a) { if (d == b) d = 0; if (a == b) a = 1; return new c(function () { var b; return new f(function () { b = d + a }, function () { return this.Yield(b -= a) }, g.Blank) }) }; c.Unfold = function (h, b) { b = d.CreateLambda(b); return new c(function () { var d = e, c; return new f(g.Blank, function () { if (d) { d = a; c = h; return this.Yield(c) } c = b(c); return this.Yield(c) }, g.Blank) }) }; c.prototype = { CascadeBreadthFirst: function (g, b) { var h = this; g = d.CreateLambda(g); b = d.CreateLambda(b); return new c(function () { var i, k = 0, j = []; return new f(function () { i = h.GetEnumerator() }, function () { while (e) { if (i.MoveNext()) { j.push(i.Current()); return this.Yield(b(i.Current(), k)) } var f = c.From(j).SelectMany(function (a) { return g(a) }); if (!f.Any()) return a; else { k++; j = []; d.Dispose(i); i = f.GetEnumerator() } } }, function () { d.Dispose(i) }) }) }, CascadeDepthFirst: function (g, b) { var h = this; g = d.CreateLambda(g); b = d.CreateLambda(b); return new c(function () { var j = [], i; return new f(function () { i = h.GetEnumerator() }, function () { while (e) { if (i.MoveNext()) { var f = b(i.Current(), j.length); j.push(i); i = c.From(g(i.Current())).GetEnumerator(); return this.Yield(f) } if (j.length <= 0) return a; d.Dispose(i); i = j.pop() } }, function () { try { d.Dispose(i) } finally { c.From(j).ForEach(function (a) { a.Dispose() }) } }) }) }, Flatten: function () { var h = this; return new c(function () { var j, i = b; return new f(function () { j = h.GetEnumerator() }, function () { while (e) { if (i != b) if (i.MoveNext()) return this.Yield(i.Current()); else i = b; if (j.MoveNext()) if (j.Current() instanceof Array) { d.Dispose(i); i = c.From(j.Current()).SelectMany(g.Identity).Flatten().GetEnumerator(); continue } else return this.Yield(j.Current()); return a } }, function () { try { d.Dispose(j) } finally { d.Dispose(i) } }) }) }, Pairwise: function (b) { var e = this; b = d.CreateLambda(b); return new c(function () { var c; return new f(function () { c = e.GetEnumerator(); c.MoveNext() }, function () { var d = c.Current(); return c.MoveNext() ? this.Yield(b(d, c.Current())) : a }, function () { d.Dispose(c) }) }) }, Scan: function (i, g, j) { if (j != b) return this.Scan(i, g).Select(j); var h; if (g == b) { g = d.CreateLambda(i); h = a } else { g = d.CreateLambda(g); h = e } var k = this; return new c(function () { var b, c, j = e; return new f(function () { b = k.GetEnumerator() }, function () { if (j) { j = a; if (!h) { if (b.MoveNext()) return this.Yield(c = b.Current()) } else return this.Yield(c = i) } return b.MoveNext() ? this.Yield(c = g(c, b.Current())) : a }, function () { d.Dispose(b) }) }) }, Select: function (b) { var e = this; b = d.CreateLambda(b); return new c(function () { var c, g = 0; return new f(function () { c = e.GetEnumerator() }, function () { return c.MoveNext() ? this.Yield(b(c.Current(), g++)) : a }, function () { d.Dispose(c) }) }) }, SelectMany: function (g, e) { var h = this; g = d.CreateLambda(g); if (e == b) e = function (b, a) { return a }; e = d.CreateLambda(e); return new c(function () { var j, i = undefined, k = 0; return new f(function () { j = h.GetEnumerator() }, function () { if (i === undefined) if (!j.MoveNext()) return a; do { if (i == b) { var f = g(j.Current(), k++); i = c.From(f).GetEnumerator() } if (i.MoveNext()) return this.Yield(e(j.Current(), i.Current())); d.Dispose(i); i = b } while (j.MoveNext()); return a }, function () { try { d.Dispose(j) } finally { d.Dispose(i) } }) }) }, Where: function (b) { b = d.CreateLambda(b); var e = this; return new c(function () { var c, g = 0; return new f(function () { c = e.GetEnumerator() }, function () { while (c.MoveNext()) if (b(c.Current(), g++)) return this.Yield(c.Current()); return a }, function () { d.Dispose(c) }) }) }, OfType: function (c) { var a; switch (c) { case Number: a = i.Number; break; case String: a = i.String; break; case Boolean: a = i.Boolean; break; case Function: a = i.Function; break; default: a = b } return a === b ? this.Where(function (a) { return a instanceof c }) : this.Where(function (b) { return typeof b === a }) }, Zip: function (e, b) { b = d.CreateLambda(b); var g = this; return new c(function () { var i, h, j = 0; return new f(function () { i = g.GetEnumerator(); h = c.From(e).GetEnumerator() }, function () { return i.MoveNext() && h.MoveNext() ? this.Yield(b(i.Current(), h.Current(), j++)) : a }, function () { try { d.Dispose(i) } finally { d.Dispose(h) } }) }) }, Join: function (m, i, h, k, j) { i = d.CreateLambda(i); h = d.CreateLambda(h); k = d.CreateLambda(k); j = d.CreateLambda(j); var l = this; return new c(function () { var n, q, o = b, p = 0; return new f(function () { n = l.GetEnumerator(); q = c.From(m).ToLookup(h, g.Identity, j) }, function () { while (e) { if (o != b) { var c = o[p++]; if (c !== undefined) return this.Yield(k(n.Current(), c)); c = b; p = 0 } if (n.MoveNext()) { var d = i(n.Current()); o = q.Get(d).ToArray() } else return a } }, function () { d.Dispose(n) }) }) }, GroupJoin: function (l, h, e, j, i) { h = d.CreateLambda(h); e = d.CreateLambda(e); j = d.CreateLambda(j); i = d.CreateLambda(i); var k = this; return new c(function () { var m = k.GetEnumerator(), n = b; return new f(function () { m = k.GetEnumerator(); n = c.From(l).ToLookup(e, g.Identity, i) }, function () { if (m.MoveNext()) { var b = n.Get(h(m.Current())); return this.Yield(j(m.Current(), b)) } return a }, function () { d.Dispose(m) }) }) }, All: function (b) { b = d.CreateLambda(b); var c = e; this.ForEach(function (d) { if (!b(d)) { c = a; return a } }); return c }, Any: function (c) { c = d.CreateLambda(c); var b = this.GetEnumerator(); try { if (arguments.length == 0) return b.MoveNext(); while (b.MoveNext()) if (c(b.Current())) return e; return a } finally { d.Dispose(b) } }, Concat: function (e) { var g = this; return new c(function () { var i, h; return new f(function () { i = g.GetEnumerator() }, function () { if (h == b) { if (i.MoveNext()) return this.Yield(i.Current()); h = c.From(e).GetEnumerator() } return h.MoveNext() ? this.Yield(h.Current()) : a }, function () { try { d.Dispose(i) } finally { d.Dispose(h) } }) }) }, Insert: function (h, b) { var g = this; return new c(function () { var j, i, l = 0, k = a; return new f(function () { j = g.GetEnumerator(); i = c.From(b).GetEnumerator() }, function () { if (l == h && i.MoveNext()) { k = e; return this.Yield(i.Current()) } if (j.MoveNext()) { l++; return this.Yield(j.Current()) } return !k && i.MoveNext() ? this.Yield(i.Current()) : a }, function () { try { d.Dispose(j) } finally { d.Dispose(i) } }) }) }, Alternate: function (a) { a = c.Return(a); return this.SelectMany(function (b) { return c.Return(b).Concat(a) }).TakeExceptLast() }, Contains: function (f, b) { b = d.CreateLambda(b); var c = this.GetEnumerator(); try { while (c.MoveNext()) if (b(c.Current()) === f) return e; return a } finally { d.Dispose(c) } }, DefaultIfEmpty: function (b) { var g = this; return new c(function () { var c, h = e; return new f(function () { c = g.GetEnumerator() }, function () { if (c.MoveNext()) { h = a; return this.Yield(c.Current()) } else if (h) { h = a; return this.Yield(b) } return a }, function () { d.Dispose(c) }) }) }, Distinct: function (a) { return this.Except(c.Empty(), a) }, Except: function (e, b) { b = d.CreateLambda(b); var g = this; return new c(function () { var h, i; return new f(function () { h = g.GetEnumerator(); i = new n(b); c.From(e).ForEach(function (a) { i.Add(a) }) }, function () { while (h.MoveNext()) { var b = h.Current(); if (!i.Contains(b)) { i.Add(b); return this.Yield(b) } } return a }, function () { d.Dispose(h) }) }) }, Intersect: function (e, b) { b = d.CreateLambda(b); var g = this; return new c(function () { var h, i, j; return new f(function () { h = g.GetEnumerator(); i = new n(b); c.From(e).ForEach(function (a) { i.Add(a) }); j = new n(b) }, function () { while (h.MoveNext()) { var b = h.Current(); if (!j.Contains(b) && i.Contains(b)) { j.Add(b); return this.Yield(b) } } return a }, function () { d.Dispose(h) }) }) }, SequenceEqual: function (h, f) { f = d.CreateLambda(f); var g = this.GetEnumerator(); try { var b = c.From(h).GetEnumerator(); try { while (g.MoveNext()) if (!b.MoveNext() || f(g.Current()) !== f(b.Current())) return a; return b.MoveNext() ? a : e } finally { d.Dispose(b) } } finally { d.Dispose(g) } }, Union: function (e, b) { b = d.CreateLambda(b); var g = this; return new c(function () { var j, h, i; return new f(function () { j = g.GetEnumerator(); i = new n(b) }, function () { var b; if (h === undefined) { while (j.MoveNext()) { b = j.Current(); if (!i.Contains(b)) { i.Add(b); return this.Yield(b) } } h = c.From(e).GetEnumerator() } while (h.MoveNext()) { b = h.Current(); if (!i.Contains(b)) { i.Add(b); return this.Yield(b) } } return a }, function () { try { d.Dispose(j) } finally { d.Dispose(h) } }) }) }, OrderBy: function (b) { return new j(this, b, a) }, OrderByDescending: function (a) { return new j(this, a, e) }, Reverse: function () { var b = this; return new c(function () { var c, d; return new f(function () { c = b.ToArray(); d = c.length }, function () { return d > 0 ? this.Yield(c[--d]) : a }, g.Blank) }) }, Shuffle: function () { var b = this; return new c(function () { var c; return new f(function () { c = b.ToArray() }, function () { if (c.length > 0) { var b = Math.floor(Math.random() * c.length); return this.Yield(c.splice(b, 1)[0]) } return a }, g.Blank) }) }, GroupBy: function (i, h, e, g) { var j = this; i = d.CreateLambda(i); h = d.CreateLambda(h); if (e != b) e = d.CreateLambda(e); g = d.CreateLambda(g); return new c(function () { var c; return new f(function () { c = j.ToLookup(i, h, g).ToEnumerable().GetEnumerator() }, function () { while (c.MoveNext()) return e == b ? this.Yield(c.Current()) : this.Yield(e(c.Current().Key(), c.Current())); return a }, function () { d.Dispose(c) }) }) }, PartitionBy: function (j, i, g, h) { var l = this; j = d.CreateLambda(j); i = d.CreateLambda(i); h = d.CreateLambda(h); var k; if (g == b) { k = a; g = function (b, a) { return new o(b, a) } } else { k = e; g = d.CreateLambda(g) } return new c(function () { var b, n, o, m = []; return new f(function () { b = l.GetEnumerator(); if (b.MoveNext()) { n = j(b.Current()); o = h(n); m.push(i(b.Current())) } }, function () { var d; while ((d = b.MoveNext()) == e) if (o === h(j(b.Current()))) m.push(i(b.Current())); else break; if (m.length > 0) { var f = k ? g(n, c.From(m)) : g(n, m); if (d) { n = j(b.Current()); o = h(n); m = [i(b.Current())] } else m = []; return this.Yield(f) } return a }, function () { d.Dispose(b) }) }) }, BufferWithCount: function (e) { var b = this; return new c(function () { var c; return new f(function () { c = b.GetEnumerator() }, function () { var b = [], d = 0; while (c.MoveNext()) { b.push(c.Current()); if (++d >= e) return this.Yield(b) } return b.length > 0 ? this.Yield(b) : a }, function () { d.Dispose(c) }) }) }, Aggregate: function (c, b, a) { return this.Scan(c, b, a).Last() }, Average: function (a) { a = d.CreateLambda(a); var c = 0, b = 0; this.ForEach(function (d) { c += a(d); ++b }); return c / b }, Count: function (a) { a = a == b ? g.True : d.CreateLambda(a); var c = 0; this.ForEach(function (d, b) { if (a(d, b))++c }); return c }, Max: function (a) { if (a == b) a = g.Identity; return this.Select(a).Aggregate(function (a, b) { return a > b ? a : b }) }, Min: function (a) { if (a == b) a = g.Identity; return this.Select(a).Aggregate(function (a, b) { return a < b ? a : b }) }, MaxBy: function (a) { a = d.CreateLambda(a); return this.Aggregate(function (b, c) { return a(b) > a(c) ? b : c }) }, MinBy: function (a) { a = d.CreateLambda(a); return this.Aggregate(function (b, c) { return a(b) < a(c) ? b : c }) }, Sum: function (a) { if (a == b) a = g.Identity; return this.Select(a).Aggregate(0, function (a, b) { return a + b }) }, ElementAt: function (d) { var c, b = a; this.ForEach(function (g, f) { if (f == d) { c = g; b = e; return a } }); if (!b) throw new Error("index is less than 0 or greater than or equal to the number of elements in source."); return c }, ElementAtOrDefault: function (f, d) { var c, b = a; this.ForEach(function (g, d) { if (d == f) { c = g; b = e; return a } }); return !b ? d : c }, First: function (c) { if (c != b) return this.Where(c).First(); var f, d = a; this.ForEach(function (b) { f = b; d = e; return a }); if (!d) throw new Error("First:No element satisfies the condition."); return f }, FirstOrDefault: function (c, d) { if (d != b) return this.Where(d).FirstOrDefault(c); var g, f = a; this.ForEach(function (b) { g = b; f = e; return a }); return !f ? c : g }, Last: function (c) { if (c != b) return this.Where(c).Last(); var f, d = a; this.ForEach(function (a) { d = e; f = a }); if (!d) throw new Error("Last:No element satisfies the condition."); return f }, LastOrDefault: function (c, d) { if (d != b) return this.Where(d).LastOrDefault(c); var g, f = a; this.ForEach(function (a) { f = e; g = a }); return !f ? c : g }, Single: function (d) { if (d != b) return this.Where(d).Single(); var f, c = a; this.ForEach(function (a) { if (!c) { c = e; f = a } else throw new Error(m); }); if (!c) throw new Error("Single:No element satisfies the condition."); return f }, SingleOrDefault: function (d, f) { if (f != b) return this.Where(f).SingleOrDefault(d); var g, c = a; this.ForEach(function (a) { if (!c) { c = e; g = a } else throw new Error(m); }); return !c ? d : g }, Skip: function (e) { var b = this; return new c(function () { var c, g = 0; return new f(function () { c = b.GetEnumerator(); while (g++ < e && c.MoveNext()); }, function () { return c.MoveNext() ? this.Yield(c.Current()) : a }, function () { d.Dispose(c) }) }) }, SkipWhile: function (b) { b = d.CreateLambda(b); var g = this; return new c(function () { var c, i = 0, h = a; return new f(function () { c = g.GetEnumerator() }, function () { while (!h) if (c.MoveNext()) { if (!b(c.Current(), i++)) { h = e; return this.Yield(c.Current()) } continue } else return a; return c.MoveNext() ? this.Yield(c.Current()) : a }, function () { d.Dispose(c) }) }) }, Take: function (e) { var b = this; return new c(function () { var c, g = 0; return new f(function () { c = b.GetEnumerator() }, function () { return g++ < e && c.MoveNext() ? this.Yield(c.Current()) : a }, function () { d.Dispose(c) }) }) }, TakeWhile: function (b) { b = d.CreateLambda(b); var e = this; return new c(function () { var c, g = 0; return new f(function () { c = e.GetEnumerator() }, function () { return c.MoveNext() && b(c.Current(), g++) ? this.Yield(c.Current()) : a }, function () { d.Dispose(c) }) }) }, TakeExceptLast: function (e) { if (e == b) e = 1; var g = this; return new c(function () { if (e <= 0) return g.GetEnumerator(); var b, c = []; return new f(function () { b = g.GetEnumerator() }, function () { while (b.MoveNext()) { if (c.length == e) { c.push(b.Current()); return this.Yield(c.shift()) } c.push(b.Current()) } return a }, function () { d.Dispose(b) }) }) }, TakeFromLast: function (e) { if (e <= 0 || e == b) return c.Empty(); var g = this; return new c(function () { var j, h, i = []; return new f(function () { j = g.GetEnumerator() }, function () { while (j.MoveNext()) { i.length == e && i.shift(); i.push(j.Current()) } if (h == b) h = c.From(i).GetEnumerator(); return h.MoveNext() ? this.Yield(h.Current()) : a }, function () { d.Dispose(h) }) }) }, IndexOf: function (c) { var a = b; this.ForEach(function (d, b) { if (d === c) { a = b; return e } }); return a !== b ? a : -1 }, LastIndexOf: function (b) { var a = -1; this.ForEach(function (d, c) { if (d === b) a = c }); return a }, ToArray: function () { var a = []; this.ForEach(function (b) { a.push(b) }); return a }, ToLookup: function (c, b, a) { c = d.CreateLambda(c); b = d.CreateLambda(b); a = d.CreateLambda(a); var e = new n(a); this.ForEach(function (g) { var f = c(g), a = b(g), d = e.Get(f); if (d !== undefined) d.push(a); else e.Add(f, [a]) }); return new q(e) }, ToObject: function (b, a) { b = d.CreateLambda(b); a = d.CreateLambda(a); var c = {}; this.ForEach(function (d) { c[b(d)] = a(d) }); return c }, ToDictionary: function (c, b, a) { c = d.CreateLambda(c); b = d.CreateLambda(b); a = d.CreateLambda(a); var e = new n(a); this.ForEach(function (a) { e.Add(c(a), b(a)) }); return e }, ToJSON: function (a, b) { return JSON.stringify(this.ToArray(), a, b) }, ToString: function (a, c) { if (a == b) a = ""; if (c == b) c = g.Identity; return this.Select(c).ToArray().join(a) }, Do: function (b) { var e = this; b = d.CreateLambda(b); return new c(function () { var c, g = 0; return new f(function () { c = e.GetEnumerator() }, function () { if (c.MoveNext()) { b(c.Current(), g++); return this.Yield(c.Current()) } return a }, function () { d.Dispose(c) }) }) }, ForEach: function (c) { c = d.CreateLambda(c); var e = 0, b = this.GetEnumerator(); try { while (b.MoveNext()) if (c(b.Current(), e++) === a) break } finally { d.Dispose(b) } }, Write: function (c, f) { if (c == b) c = ""; f = d.CreateLambda(f); var g = e; this.ForEach(function (b) { if (g) g = a; else document.write(c); document.write(f(b)) }) }, WriteLine: function (a) { a = d.CreateLambda(a); this.ForEach(function (b) { document.write(a(b)); document.write("<br />") }) }, Force: function () { var a = this.GetEnumerator(); try { while (a.MoveNext()); } finally { d.Dispose(a) } }, Let: function (b) { b = d.CreateLambda(b); var e = this; return new c(function () { var g; return new f(function () { g = c.From(b(e)).GetEnumerator() }, function () { return g.MoveNext() ? this.Yield(g.Current()) : a }, function () { d.Dispose(g) }) }) }, Share: function () { var e = this, d; return new c(function () { return new f(function () { if (d == b) d = e.GetEnumerator() }, function () { return d.MoveNext() ? this.Yield(d.Current()) : a }, g.Blank) }) }, MemoizeAll: function () { var h = this, e, d; return new c(function () { var c = -1; return new f(function () { if (d == b) { d = h.GetEnumerator(); e = [] } }, function () { c++; return e.length <= c ? d.MoveNext() ? this.Yield(e[c] = d.Current()) : a : this.Yield(e[c]) }, g.Blank) }) }, Catch: function (b) { b = d.CreateLambda(b); var e = this; return new c(function () { var c; return new f(function () { c = e.GetEnumerator() }, function () { try { return c.MoveNext() ? this.Yield(c.Current()) : a } catch (d) { b(d); return a } }, function () { d.Dispose(c) }) }) }, Finally: function (b) { b = d.CreateLambda(b); var e = this; return new c(function () { var c; return new f(function () { c = e.GetEnumerator() }, function () { return c.MoveNext() ? this.Yield(c.Current()) : a }, function () { try { d.Dispose(c) } finally { b() } }) }) }, Trace: function (c, a) { if (c == b) c = "Trace"; a = d.CreateLambda(a); return this.Do(function (b) { console.log(c, ":", a(b)) }) } }; var g = { Identity: function (a) { return a }, True: function () { return e }, Blank: function () { } }, i = { Boolean: typeof e, Number: typeof 0, String: typeof "", Object: typeof {}, Undefined: typeof undefined, Function: typeof function () { } }, d = { CreateLambda: function (a) { if (a == b) return g.Identity; if (typeof a == i.String) if (a == "") return g.Identity; else if (a.indexOf("=>") == -1) return new Function("$,$$,$$$,$$$$", "return " + a); else { var c = a.match(/^[(\s]*([^()]*?)[)\s]*=>(.*)/); return new Function(c[1], "return " + c[2]) } return a }, IsIEnumerable: function (b) { if (typeof Enumerator != i.Undefined) try { new Enumerator(b); return e } catch (c) { } return a }, Compare: function (a, b) { return a === b ? 0 : a > b ? 1 : -1 }, Dispose: function (a) { a != b && a.Dispose() } }, k = { Before: 0, Running: 1, After: 2 }, f = function (d, f, g) { var c = new p, b = k.Before; this.Current = c.Current; this.MoveNext = function () { try { switch (b) { case k.Before: b = k.Running; d(); case k.Running: if (f.apply(c)) return e; else { this.Dispose(); return a } case k.After: return a } } catch (g) { this.Dispose(); throw g; } }; this.Dispose = function () { if (b != k.Running) return; try { g() } finally { b = k.After } } }, p = function () { var a = b; this.Current = function () { return a }; this.Yield = function (b) { a = b; return e } }, j = function (f, b, c, e) { var a = this; a.source = f; a.keySelector = d.CreateLambda(b); a.descending = c; a.parent = e }; j.prototype = new c; j.prototype.CreateOrderedEnumerable = function (a, b) { return new j(this.source, a, b, this) }; j.prototype.ThenBy = function (b) { return this.CreateOrderedEnumerable(b, a) }; j.prototype.ThenByDescending = function (a) { return this.CreateOrderedEnumerable(a, e) }; j.prototype.GetEnumerator = function () { var h = this, d, c, e = 0; return new f(function () { d = []; c = []; h.source.ForEach(function (b, a) { d.push(b); c.push(a) }); var a = l.Create(h, b); a.GenerateKeys(d); c.sort(function (b, c) { return a.Compare(b, c) }) }, function () { return e < c.length ? this.Yield(d[c[e++]]) : a }, g.Blank) }; var l = function (c, d, e) { var a = this; a.keySelector = c; a.descending = d; a.child = e; a.keys = b }; l.Create = function (a, d) { var c = new l(a.keySelector, a.descending, d); return a.parent != b ? l.Create(a.parent, c) : c }; l.prototype.GenerateKeys = function (d) { var a = this; for (var f = d.length, g = a.keySelector, e = new Array(f), c = 0; c < f; c++) e[c] = g(d[c]); a.keys = e; a.child != b && a.child.GenerateKeys(d) }; l.prototype.Compare = function (e, f) { var a = this, c = d.Compare(a.keys[e], a.keys[f]); if (c == 0) { if (a.child != b) return a.child.Compare(e, f); c = d.Compare(e, f) } return a.descending ? -c : c }; var h = function (a) { this.source = a }; h.prototype = new c; h.prototype.Any = function (a) { return a == b ? this.source.length > 0 : c.prototype.Any.apply(this, arguments) }; h.prototype.Count = function (a) { return a == b ? this.source.length : c.prototype.Count.apply(this, arguments) }; h.prototype.ElementAt = function (a) { return 0 <= a && a < this.source.length ? this.source[a] : c.prototype.ElementAt.apply(this, arguments) }; h.prototype.ElementAtOrDefault = function (a, b) { return 0 <= a && a < this.source.length ? this.source[a] : b }; h.prototype.First = function (a) { return a == b && this.source.length > 0 ? this.source[0] : c.prototype.First.apply(this, arguments) }; h.prototype.FirstOrDefault = function (a, d) { return d != b ? c.prototype.FirstOrDefault.apply(this, arguments) : this.source.length > 0 ? this.source[0] : a }; h.prototype.Last = function (d) { var a = this; return d == b && a.source.length > 0 ? a.source[a.source.length - 1] : c.prototype.Last.apply(a, arguments) }; h.prototype.LastOrDefault = function (d, e) { var a = this; return e != b ? c.prototype.LastOrDefault.apply(a, arguments) : a.source.length > 0 ? a.source[a.source.length - 1] : d }; h.prototype.Skip = function (d) { var b = this.source; return new c(function () { var c; return new f(function () { c = d < 0 ? 0 : d }, function () { return c < b.length ? this.Yield(b[c++]) : a }, g.Blank) }) }; h.prototype.TakeExceptLast = function (a) { if (a == b) a = 1; return this.Take(this.source.length - a) }; h.prototype.TakeFromLast = function (a) { return this.Skip(this.source.length - a) }; h.prototype.Reverse = function () { var b = this.source; return new c(function () { var c; return new f(function () { c = b.length }, function () { return c > 0 ? this.Yield(b[--c]) : a }, g.Blank) }) }; h.prototype.SequenceEqual = function (d, e) { return (d instanceof h || d instanceof Array) && e == b && c.From(d).Count() != this.Count() ? a : c.prototype.SequenceEqual.apply(this, arguments) }; h.prototype.ToString = function (a, d) { if (d != b || !(this.source instanceof Array)) return c.prototype.ToString.apply(this, arguments); if (a == b) a = ""; return this.source.join(a) }; h.prototype.GetEnumerator = function () { var b = this.source, c = 0; return new f(g.Blank, function () { return c < b.length ? this.Yield(b[c++]) : a }, g.Blank) }; var n = function () { var h = function (a, b) { return Object.prototype.hasOwnProperty.call(a, b) }, d = function (a) { return a === b ? "null" : a === undefined ? "undefined" : typeof a.toString === i.Function ? a.toString() : Object.prototype.toString.call(a) }, l = function (d, c) { var a = this; a.Key = d; a.Value = c; a.Prev = b; a.Next = b }, j = function () { this.First = b; this.Last = b }; j.prototype = { AddLast: function (c) { var a = this; if (a.Last != b) { a.Last.Next = c; c.Prev = a.Last; a.Last = c } else a.First = a.Last = c }, Replace: function (c, a) { if (c.Prev != b) { c.Prev.Next = a; a.Prev = c.Prev } else this.First = a; if (c.Next != b) { c.Next.Prev = a; a.Next = c.Next } else this.Last = a }, Remove: function (a) { if (a.Prev != b) a.Prev.Next = a.Next; else this.First = a.Next; if (a.Next != b) a.Next.Prev = a.Prev; else this.Last = a.Prev } }; var k = function (c) { var a = this; a.count = 0; a.entryList = new j; a.buckets = {}; a.compareSelector = c == b ? g.Identity : c }; k.prototype = { Add: function (i, j) { var a = this, g = a.compareSelector(i), f = d(g), c = new l(i, j); if (h(a.buckets, f)) { for (var b = a.buckets[f], e = 0; e < b.length; e++) if (a.compareSelector(b[e].Key) === g) { a.entryList.Replace(b[e], c); b[e] = c; return } b.push(c) } else a.buckets[f] = [c]; a.count++; a.entryList.AddLast(c) }, Get: function (i) { var a = this, c = a.compareSelector(i), g = d(c); if (!h(a.buckets, g)) return undefined; for (var e = a.buckets[g], b = 0; b < e.length; b++) { var f = e[b]; if (a.compareSelector(f.Key) === c) return f.Value } return undefined }, Set: function (k, m) { var b = this, g = b.compareSelector(k), j = d(g); if (h(b.buckets, j)) for (var f = b.buckets[j], c = 0; c < f.length; c++) if (b.compareSelector(f[c].Key) === g) { var i = new l(k, m); b.entryList.Replace(f[c], i); f[c] = i; return e } return a }, Contains: function (j) { var b = this, f = b.compareSelector(j), i = d(f); if (!h(b.buckets, i)) return a; for (var g = b.buckets[i], c = 0; c < g.length; c++) if (b.compareSelector(g[c].Key) === f) return e; return a }, Clear: function () { this.count = 0; this.buckets = {}; this.entryList = new j }, Remove: function (g) { var a = this, f = a.compareSelector(g), e = d(f); if (!h(a.buckets, e)) return; for (var b = a.buckets[e], c = 0; c < b.length; c++) if (a.compareSelector(b[c].Key) === f) { a.entryList.Remove(b[c]); b.splice(c, 1); if (b.length == 0) delete a.buckets[e]; a.count--; return } }, Count: function () { return this.count }, ToEnumerable: function () { var d = this; return new c(function () { var c; return new f(function () { c = d.entryList.First }, function () { if (c != b) { var d = { Key: c.Key, Value: c.Value }; c = c.Next; return this.Yield(d) } return a }, g.Blank) }) } }; return k }(), q = function (a) { var b = this; b.Count = function () { return a.Count() }; b.Get = function (b) { return c.From(a.Get(b)) }; b.Contains = function (b) { return a.Contains(b) }; b.ToEnumerable = function () { return a.ToEnumerable().Select(function (a) { return new o(a.Key, a.Value) }) } }, o = function (b, a) { this.Key = function () { return b }; h.call(this, a) }; o.prototype = new h; return c }()
}

try {
    var guid = uuid.v4();
}
catch (mvm) {
    (function () { var _global = this; var _rng; if (typeof (_global.require) == 'function') { try { var _rb = _global.require('crypto').randomBytes; _rng = _rb && function () { return _rb(16) } } catch (e) { } } if (!_rng && _global.crypto && crypto.getRandomValues) { var _rnds8 = new Uint8Array(16); _rng = function whatwgRNG() { crypto.getRandomValues(_rnds8); return _rnds8 } } if (!_rng) { var _rnds = new Array(16); _rng = function () { for (var i = 0, r; i < 16; i++) { if ((i & 0x03) === 0) r = Math.random() * 0x100000000; _rnds[i] = r >>> ((i & 0x03) << 3) & 0xff } return _rnds } } var BufferClass = typeof (_global.Buffer) == 'function' ? _global.Buffer : Array; var _byteToHex = []; var _hexToByte = {}; for (var i = 0; i < 256; i++) { _byteToHex[i] = (i + 0x100).toString(16).substr(1); _hexToByte[_byteToHex[i]] = i } function parse(s, buf, offset) { var i = (buf && offset) || 0, ii = 0; buf = buf || []; s.toLowerCase().replace(/[0-9a-f]{2}/g, function (oct) { if (ii < 16) { buf[i + ii++] = _hexToByte[oct] } }); while (ii < 16) { buf[i + ii++] = 0 } return buf } function unparse(buf, offset) { var i = offset || 0, bth = _byteToHex; return bth[buf[i++]] + bth[buf[i++]] + bth[buf[i++]] + bth[buf[i++]] + '-' + bth[buf[i++]] + bth[buf[i++]] + '-' + bth[buf[i++]] + bth[buf[i++]] + '-' + bth[buf[i++]] + bth[buf[i++]] + '-' + bth[buf[i++]] + bth[buf[i++]] + bth[buf[i++]] + bth[buf[i++]] + bth[buf[i++]] + bth[buf[i++]] } var _seedBytes = _rng(); var _nodeId = [_seedBytes[0] | 0x01, _seedBytes[1], _seedBytes[2], _seedBytes[3], _seedBytes[4], _seedBytes[5]]; var _clockseq = (_seedBytes[6] << 8 | _seedBytes[7]) & 0x3fff; var _lastMSecs = 0, _lastNSecs = 0; function v1(options, buf, offset) { var i = buf && offset || 0; var b = buf || []; options = options || {}; var clockseq = options.clockseq != null ? options.clockseq : _clockseq; var msecs = options.msecs != null ? options.msecs : new Date().getTime(); var nsecs = options.nsecs != null ? options.nsecs : _lastNSecs + 1; var dt = (msecs - _lastMSecs) + (nsecs - _lastNSecs) / 10000; if (dt < 0 && options.clockseq == null) { clockseq = clockseq + 1 & 0x3fff } if ((dt < 0 || msecs > _lastMSecs) && options.nsecs == null) { nsecs = 0 } if (nsecs >= 10000) { throw new Error('uuid.v1(): Can\'t create more than 10M uuids/sec') } _lastMSecs = msecs; _lastNSecs = nsecs; _clockseq = clockseq; msecs += 12219292800000; var tl = ((msecs & 0xfffffff) * 10000 + nsecs) % 0x100000000; b[i++] = tl >>> 24 & 0xff; b[i++] = tl >>> 16 & 0xff; b[i++] = tl >>> 8 & 0xff; b[i++] = tl & 0xff; var tmh = (msecs / 0x100000000 * 10000) & 0xfffffff; b[i++] = tmh >>> 8 & 0xff; b[i++] = tmh & 0xff; b[i++] = tmh >>> 24 & 0xf | 0x10; b[i++] = tmh >>> 16 & 0xff; b[i++] = clockseq >>> 8 | 0x80; b[i++] = clockseq & 0xff; var node = options.node || _nodeId; for (var n = 0; n < 6; n++) { b[i + n] = node[n] } return buf ? buf : unparse(b) } function v4(options, buf, offset) { var i = buf && offset || 0; if (typeof (options) == 'string') { buf = options == 'binary' ? new BufferClass(16) : null; options = null } options = options || {}; var rnds = options.random || (options.rng || _rng)(); rnds[6] = (rnds[6] & 0x0f) | 0x40; rnds[8] = (rnds[8] & 0x3f) | 0x80; if (buf) { for (var ii = 0; ii < 16; ii++) { buf[i + ii] = rnds[ii] } } return buf || unparse(rnds) } var uuid = v4; uuid.v1 = v1; uuid.v4 = v4; uuid.parse = parse; uuid.unparse = unparse; uuid.BufferClass = BufferClass; if (typeof (module) != 'undefined' && module.exports) { module.exports = uuid } else if (typeof define === 'function' && define.amd) { define(function () { return uuid }) } else { var _previousRoot = _global.uuid; uuid.noConflict = function () { _global.uuid = _previousRoot; return uuid }; _global.uuid = uuid } }).call(this);
}

//生成Token，格式（guid+"_"+math.random()+"_"+时间），例如：54732d49-195a-4e7f-ac09-0ad63ae2ee40_0.7057493212632835_20150731104317443
function GeneralToken() {
    var guid = uuid.v4();

    var myDate = new Date();
    var year = myDate.getFullYear();
    var month = ("0" + (myDate.getMonth() + 1)).slice(-2);
    var day = ("0" + myDate.getDate()).slice(-2);
    var h = ("0" + myDate.getHours()).slice(-2);
    var m = ("0" + myDate.getMinutes()).slice(-2);
    var s = ("0" + myDate.getSeconds()).slice(-2);
    var mi = ("00" + myDate.getMilliseconds()).slice(-3);
    var timeInt = year + month + day + h + m + s + mi;
    return guid + "_" + Math.random() + "_" + timeInt;
}

//添加样式文件到界面
function AddCssFileToHead(fileUrl) {
    var fileref = document.createElement("link");
    fileref.setAttribute("rel", "stylesheet");
    fileref.setAttribute("type", "text/css");
    fileref.setAttribute("href", fileUrl);
    document.getElementsByTagName("head")[0].appendChild(fileref);
}

//添加样式到Head
function AddCssByStyle(cssString) {
    var doc = document;
    var style = doc.createElement("style");
    style.setAttribute("type", "text/css");

    if (style.styleSheet) {// IE
        style.styleSheet.cssText = cssString;
    } else {// w3c
        var cssText = doc.createTextNode(cssString);
        style.appendChild(cssText);
    }

    var heads = doc.getElementsByTagName("head");
    if (heads.length)
        heads[0].appendChild(style);
    else
        doc.documentElement.appendChild(style);
}

//弹出层
jQuery.fn.extend(
{
    Wanda_OpenDiv: function (options) {
        var $self = $(this);
        var _setting = {
            title: "",
            level: 1, //弹出层级别
            onInit: function (context) { },  //在对话框弹出时， 初始化对话框内容

            onCancel: function (context) { /*$(context).Wanda_ContextValidation(false);*/ return true; }, //在对话框取消/关闭时， 触发的事件； 未返回为true，则无法关闭对话框
            onSubmit: function (context) { return true; }, //在对话框确定提交时， 触发的事件； 未返回为true，则无法关闭对话框
            btns: [], //自定义的按钮及事件 btns: [{ name: "test", cssClass: "", onclick: function (context) { } }] ,这是参数格式
            mode: "confirm", //or "alert" or "info"
            widthMode: "standard", //thin/small= 400px, standard=700px, wide/large=1000px
            scrollY: false, //如果弹出内容很长或者需要增长， 则设为scrollY:true
            other: "",
            isShowExit: true,
            isShowButton: false
        }
        WANDA_OPENDIV_OBJARRAY.push({
            Obj: $self, CancelFunc: function () {
                if (!!_setting.onCancel($popupFrame)) {
                    $self.ClearInputs();
                    $self.ClearFloatDiv($self);
                    $self.CloseDiv();
                }
            }
        });
        if (!window.popupZindex) {
            window.popupZindex = 10000;
        }

        $.extend(_setting, options);


        var sWidth, sHeight;
        sWidth = window.screen.availWidth;
        if (window.screen.availHeight > document.body.scrollHeight) {
            sHeight = window.screen.availHeight;

        } else {
            sHeight = document.body.scrollHeight + 20;
        }
        var maskObj = $("<div  class='wanda_user_maskDiv'></div>");
        maskObj.css({ width: sWidth, height: sHeight, zIndex: window.popupZindex++ });
        maskObj.appendTo($("body"));
        $("body").attr("scroll", "no");

        //$("#BigDiv").data("divbox_selectlist", $("select:visible"));
        //$("select:visible").hide();
        maskObj.data("divbox_scrolltop", $.Wanda_ScrollPosition().Top);
        maskObj.data("divbox_scrollleft", $.Wanda_ScrollPosition().Left);
        if ($(".wanda_user_maskDiv").length == 1) {
            $("html").data("overflow", $("html").css("overflow"))
                    .css("overflow", "hidden");
        }
        //window.scrollTo($("#BigDiv").attr("divbox_scrollleft"), maskObj.attr("divbox_scrolltop"));

        if (_setting.level < 1) {
            _setting.level = 1
        }
        if (_setting.level > 4) {
            _setting.level = 4
        }

        var popupFrameHtml = [
            // style="width:1018px;height:620px;"
      '<div class="wanda_user_popup context">',
      ' <div class="wanda_user_t_l"> ',
      ' </div>            ',
      ' <div class="wanda_user_t_m"> ',
      ' </div>            ',
      ' <div class="wanda_user_t_r"> ',
      ' </div>            ',
      ' <div class="wanda_user_m_l dnrHandler"> ',
      ' </div>            ',
      ' <div class="wanda_user_m_m"> ',
      '     <div class="wanda_user_pop_titile wanda_user_dnrHandler">',
      '         <span class="wanda_user_pop_icon wanda_user_pop_icon_0' + _setting.level + '"></span>',
      '         <div class="wanda_user_pop_txt">',
      '             <input type="button" style="width:0px;border:none;height:0px; border:0px;padding:0px;margin:0px;" id="closepicxxxxx"/>',
      '             <span class="wanda_user_pop_txt_title"></span>',
      //'             <img src="/images/popup/help.png" class="help" />',
      //'             <img src="/images/popup/exit.png" class="exit" style="display:none" />',
      '             <span class="wanda_user_exit" style="">关闭</span>',
      '         </div>',
      '     </div>',
      '     <!--pop_titile-->',
      '     <div class="wanda_user_padding_10  wanda_user_popup_content">', //class overflow
      '     </div>',
      '     <div class="wanda_user_pop_btn line_t" style="padding: 10px; display:none">',
      '         <div class="wanda_user_btn01 wanda_user_btn_fr wanda_user_btn_cancel"  ><a><span>取消</span></a></div>',
      '         <div class="wanda_user_btn01 wanda_user_btn_fr wanda_user_btn_submit"><a><span>确定</span></a></div>',
      '     </div>',
      ' </div>',
      ' <div class="wanda_user_m_r wanda_user_dnrHandler">',
      ' </div>             ',
      ' <div class="wanda_user_b_l">  ',
      ' </div>             ',
      ' <div class="wanda_user_b_m wanda_user_dnrHandler">  ',
      ' </div>             ',
      ' <div class="wanda_user_b_r wanda_user_resizeHandler ">  ',
      ' <div class="wanda_user_resizeHandler"></div>  ',
      ' </div>             ',
      '</div>'
        ];
        var $popupFrame;//= $(popupFrameHtml.join(""));
        if ($self.parents(".wanda_user_popup").length > 0) {
            $popupFrame = $self.parents(".wanda_user_popup:first");
            $popupFrame.css({ zIndex: window.popupZindex++ });
        }
        else {
            $popupFrame = $(popupFrameHtml.join(""));
            $popupFrame.css({ zIndex: window.popupZindex++ });
            $popupFrame.appendTo($("body"));
            $popupFrame.find(".wanda_user_popup_content").append($self);  //将内容附属到弹出层上
            $popupFrame.find(".wanda_user_exit, .wanda_user_btn_cancel").click(function () {

                if (!!_setting.onCancel($popupFrame)) {
                    $self.ClearInputs();
                    $self.ClearFloatDiv($self);
                    $self.CloseDiv();
                }
            });
            $popupFrame.find(".wanda_user_help").click(function () {
                //alert("TODO 显示帮助内容");
            });
            $popupFrame.find(".wanda_user_btn_submit").click(function () {
                if (!!_setting.onSubmit($popupFrame)) {
                    $self.ClearInputs();
                    $self.ClearFloatDiv($self);
                    $self.CloseDiv();
                }
            });
            $popupFrame.bind("submitContext", function () {
                $popupFrame.find(".wanda_user_btn_submit").click();
            });
            $popupFrame.bind("cancelContext", function () { $popupFrame.find(".wanda_user_exit").click(); });
            $popupFrame.bind("hideContext", function () { $popupFrame.hide(); });
            $popupFrame.bind("showContext", function () { $popupFrame.show(); });
            // 设置高度
            if (_setting.scrollY) {
                $popupFrame.find(".wanda_user_popup_content").addClass("overflow");
            }
            // 设置宽度
            if (_setting.widthMode) {
                switch (_setting.widthMode) {
                    case "small":
                    case "thin": $popupFrame.css({ width: 300 }); break;
                    case "large":
                    case "wide": $popupFrame.css({ width: 800 }); break;
                    default: break;
                }
            }

            var btnContainer = $popupFrame.find(".wanda_user_pop_btn");
            if (_setting.isShowButton) {
                btnContainer.show();
            }
            if (_setting.mode) {
                if (_setting.mode == "info") {
                    btnContainer.find(".wanda_user_btn_submit").remove();
                    btnContainer.find(".wanda_user_btn_cancel").remove();
                }
                else if (_setting.mode == "alert") {
                    btnContainer.find(".wanda_user_btn_cancel").remove();
                }
            }
            if (_setting.isShowExit) {
                $(".wanda_user_exit").show();
            }

            // 额外的自定义按钮
            if (_setting.btns && $.isArray(_setting.btns)) {
                $.each(_setting.btns, function (i, btnDesc) {
                    btnDesc.cssClass = btnDesc.cssClass || "";
                    var btn = $('<div class="wanda_user_btn_blue01 wanda_user_btn_fr ' + btnDesc.cssClass + '"  ><a><span>' + btnDesc.name + '</span></a></div>');
                    if (btnDesc.onclick && $.isFunction(btnDesc.onclick)) {
                        btn.click(function () {
                            var r = btnDesc.onclick($popupFrame);
                            if (r == true) {
                                $self.CloseDiv();
                            }
                        });
                    }
                    btn.appendTo(btnContainer);
                });
            }
        }
        $popupFrame.data("level", _setting.level);//缓存当前级别，如果需要多次弹出， 则需要取此值。
        $popupFrame
            .Wanda_jqDrag($popupFrame.find(".wanda_user_dnrHandler"), $self.ClearFloatDiv) // 位置可拖拽, 第2个参数表示拖动时要“清理”的动作
            .Wanda_jqResize($popupFrame.find(".wanda_user_resizeHandler"))      // 右下角拖拉可Resize
            .find(".wanda_user_pop_txt_title").text(_setting.title);
        $self.data("mask", maskObj); //关联遮罩
        if (_setting.onInit && $.isFunction(_setting.onInit)) {
            _setting.onInit($popupFrame);
        }
        var MyDiv_w = $popupFrame.width();
        var MyDiv_h = $popupFrame.height() + 15;
        MyDiv_w = parseInt(MyDiv_w, 10);
        MyDiv_h = parseInt(MyDiv_h, 10);
        var width = $.Wanda_PageSize().Width;
        var height = $.Wanda_PageSize().Height;
        var left = $.Wanda_ScrollPosition().Left;
        var top = $.Wanda_ScrollPosition().Top;
        var Div_topposition = top + (height / 2) - (MyDiv_h / 2);
        var Div_leftposition = left + (width / 2) - (MyDiv_w / 2) + _setting.level * 26
        $popupFrame.css("left", Div_leftposition + "px");
        $popupFrame.css("top", Div_topposition + "px");
        if (Wanda_browser.versions.gecko) {
            $popupFrame.show();
            return;
        }
        $popupFrame.fadeIn("fast");
        document.getElementById("closepicxxxxx").focus();
        return $self;
    }
    , CloseDiv: function () {
        var $self = $(this);
        var mask = $self.data("mask")
        var $popupFrame = $self.parents(".wanda_user_popup");
        var destroy = true;
        if (destroy) {
            $popupFrame.remove();

        } else {

            if (Wanda_browser.versions.gecko) {
                $popupFrame.hide();

            } else {
                $popupFrame.fadeOut("fast");
            }
        }


        //$("#maskDiv").data("divbox_selectlist").show();
        if (typeof (mask) != 'undefined') {
            if ($(".wanda_user_maskDiv").length == 1) { //由于是多层弹出， 只有在最后关闭的时候才重新显示滚动
                $("html").css("overflow", $("html").data("overflow"));
                $("html").css("overflow", "auto");
                window.scrollTo(mask.data("divbox_scrollleft"),
                                mask.data("divbox_scrolltop"));

                //myDebugger.log("$self.mask top" + mask.data("divbox_scrolltop"));
                //myDebugger.log("$self.mask left" + mask.data("divbox_scrollleft"));
            }
            mask.remove();
        }
        // $(this).appendTo($("body"));
        $popupFrame.remove(); //删除外框
    }
    , ClearInputs: function () {
        var container = $(this);
        container.find("table[name='toEmpty']").find("tbody").empty();
        container.find("input:text, textarea, select").val("");
        container.find(":checkbox, :radio").attr("checked", "");
    }
    , ClearFloatDiv: function (handler) {
        //return; // 怀疑与473bug有关
        var context = $(handler).parents(".wanda_user_popup:first");
        if ($.fn.tipsy) {
            // tipsy
            //context.find("[title]").tipsy("hide");
            //context.find("[original-title]").tipsy("hide");
        }
        // autocomplete
        $(".wanda_user_ac_results").hide();
    }
});



//================================Jquery 插件==================================
//以下实现拖拽与缩放
// h 表示句柄
// c 表示拖拽的时候清理浮动菜单的方法
// k 表示命令，
var WANDA_OPENDIV_OBJARRAY = [];
(function ($) {
    $.fn.Wanda_jqDrag = function (h, c) {
        return i(this, h, c, 'd');
    };
    $.fn.Wanda_jqResize = function (h) {
        return i(this, h, 'r');
    };
    $.Wanda_jqDnR = {
        dnr: {}, e: 0,
        drag: function (v) {

            if (M.k == 'd') E.css({ left: M.X + v.pageX - M.pX, top: M.Y + v.pageY - M.pY });
            else {
                E.css({ width: Math.max(v.pageX - M.pX + M.W, 0), height: Math.max(v.pageY - M.pY + M.H, 0) });
            }
            return false;
        },
        stop: function () {
            E.css('opacity', M.o); $("body").unbind('mousemove', J.drag).unbind('mouseup', J.stop);
        }
    };
    var J = $.Wanda_jqDnR, M = J.dnr, E = J.e,
    i = function (e, h, c, k) {
        return e.each(function () {
            h = (h) ? $(h, e) : e;
            h.bind('mousedown', {
                e: e, k: k
            }, function (v) {
                if (c && typeof (c) == "function") {
                    c(h);
                }
                var d = v.data, p = {}; E = d.e;
                // attempt utilization of dimensions plugin to fix IE issues
                if (E.css('position') != 'relative') {
                    try { E.position(p); } catch (e) { }
                }
                M = { X: p.left || f('left') || 0, Y: p.top || f('top') || 0, W: f('width') || E[0].scrollWidth || 0, H: f('height') || E[0].scrollHeight || 0, pX: v.pageX, pY: v.pageY, k: d.k, o: E.css('opacity') };
                E.css({ opacity: 0.8 }); $("body").mousemove($.Wanda_jqDnR.drag).mouseup($.Wanda_jqDnR.stop);
                return false;
            });
        });
    },
    f = function (k) {
        return parseInt(E.css(k)) || false;
    };
    //使用Esc键关闭弹出窗口 
    $(document).keyup(function (event) {
        if (event.which == '27') {
            if (WANDA_OPENDIV_OBJARRAY.length > 0) {
                var $self = WANDA_OPENDIV_OBJARRAY[WANDA_OPENDIV_OBJARRAY.length - 1];
                $self.CancelFunc();
                WANDA_OPENDIV_OBJARRAY.pop();
            }
        }
    });
})(jQuery);

var Wanda_browser = {
    versions: function () {
        var u = navigator.userAgent, app = navigator.appVersion;
        return {//移动终端浏览器版本信息 
            trident: u.indexOf('Trident') > -1, //IE内核
            presto: u.indexOf('Presto') > -1, //opera内核
            webKit: u.indexOf('AppleWebKit') > -1, //苹果、谷歌内核
            gecko: u.indexOf('Gecko') > -1 && u.indexOf('KHTML') == -1, //火狐内核
            mobile: (!!u.match(/AppleWebKit.*Mobile.*/) || !!u.match(/AppleWebKit/)) && u.indexOf('Windows NT') < 0, //是否为移动终端
            ios: !!u.match(/\(i[^;]+;( U;)? CPU.+Mac OS X/), //ios终端
            android: u.indexOf('Android') > -1 || u.indexOf('Linux') > -1, //android终端或者uc浏览器
            iPhone: u.indexOf('iPhone') > -1,// || u.indexOf('Mac') > -1, //是否为iPhone或者QQHD浏览器
            iPad: u.indexOf('iPad') > -1, //是否iPad
            webApp: u.indexOf('Safari') == -1 //是否web应该程序，没有头部与底部
        };
    }(),
    language: (navigator.browserLanguage || navigator.language).toLowerCase()
}

//滚动和获取页面大小
$.extend(
{
    Wanda_PageSize: function () {
        var width = 0;
        var height = 0;
        width = window.innerWidth != null ? window.innerWidth : document.documentElement && document.documentElement.clientWidth ? document.documentElement.clientWidth : document.body != null ? document.body.clientWidth : null;
        height = window.innerHeight != null ? window.innerHeight : document.documentElement && document.documentElement.clientHeight ? document.documentElement.clientHeight : document.body != null ? document.body.clientHeight : null;
        return { Width: width, Height: height };
    },
    Wanda_ScrollPosition: function () {
        var top = 0, left = 0;
        if (Wanda_browser.versions.gecko) {
            top = window.pageYOffset;
            left = window.pageXOffset;
        }
        else if (Wanda_browser.versions.trident) {
            top = document.documentElement.scrollTop;
            left = document.documentElement.scrollLeft;
        }
        else if (document.body) {
            top = document.body.scrollTop;
            left = document.body.scrollLeft;
        }
        return { Top: top, Left: left };
    }
});

$.fn.extend({
    // 通过ruleModel绑定的方法来给Context中的输入内容设置错误提示tips
    // 绑定的方法中带一个参数context, 表示上下文； 方法中的this表示输入控件
    // gravity 表示tooltip方向， 默认为向下； 如果是string类型， 则为默认方向 ； 如果是对象， 则表示对指定控件分别设置
    Wanda_ContextValidation: function (ruleModel, gravity) {
        var inputs = "input[type=text],input[type=checkbox],textarea,select".split(","); // 输入框
        var $context = $(this);

        if (arguments[0] == false || $.type(arguments[0]) == "undefined" || arguments[0] == null) {
            $context.find("input[data-field],textarea[data-field]").removeClass("validate_error");
            $context.find("[data-field]").each(function () { $(this).tipsy("hide") });
            return;
        }

        if ($.isPlainObject(ruleModel) == false) {
            ruleModel = {};
        }

        var result = true;
        // 
        if ($.type(gravity) == "string") {
            gravity = { base: gravity };
        }
        else {
            gravity = $.extend({ base: "w" }, gravity);
        }
        for (var property in ruleModel) {
            if (ruleModel.hasOwnProperty(property)) {
                var gv = gravity[property] || gravity.base; //设置tooltip方向
                var sender = $context.FindFieldCtrl(property);
                if (sender.length == 0) {
                    continue;
                }
                //var val = $context.val() || $context.attr["data-fieldvalue"] || "";
                if ($.isFunction(ruleModel[property])) {
                    var func = ruleModel[property];
                    if (!controlValidation($context, sender, func, gv)) {
                        result = false; /**/
                    };
                }
                else if ($.isPlainObject(ruleModel[property])) { // 多个方法
                    for (var funcName in ruleModel[property]) {
                        var func = ruleModel[property][funcName];
                        if ($.isFunction(func)) {
                            if (!controlValidation($context, sender, func, gv)) { result = false; };//验证， 一旦有错， 退出
                        }
                    }
                }
                else if ($.isArray(ruleModel[property])) { // 多个方法数组
                    for (var i = 0; i < ruleModel[property].length; i++) {
                        var func = ruleModel[property][i];
                        if ($.isFunction(func)) {
                            if (!controlValidation($context, sender, func, gv)) { result = false; }; //验证， 一旦有错， 退出
                        }
                    }
                }
                // clear
                sender.each(function () {
                    $(this).data("errMsgList", null);
                });
            }
        }
        return result;
        function controlValidation(context, controls, validateFunc, gravity) {
            // var val = control.val() || control.attr["data-fieldvalue"] || "";
            var result = true;
            $(controls).each(function () {
                var control = $(this);
                $.each(inputs, function (i, selector) {
                    if (control.is(selector)) {
                        var errMsg = validateFunc.apply(control, [context]);
                        var tips = control.data("errMsgList");
                        if ($.isArray(tips) == false) {
                            tips = [];
                        }
                        if ($.type(errMsg) == "string" || errMsg != true) {
                            tips.push(errMsg);
                            control.data("errMsgList", tips);
                        }
                        //console.log(control.val() + tips);
                        if (tips.length > 0) {
                            result = false;
                        }
                        else {
                            $(control).tipsy("disable");
                            $(control).removeClass("validate_error");
                        }



                        if (tips.length > 0) { //显示tipsy 做成异步
                            var timer = control.data("tipsTimer");
                            if (timer) {
                                clearTimeout(timer);
                            }

                            timer = setTimeout(function () {
                                var tipsHtml = tips.join("</br>");
                                $(control).addClass("validate_error");
                                $(control).attr("title", tipsHtml)
                                    .tipsy({
                                        theme: "warnTheme",
                                        opacity: 0.95,
                                        trigger: "focus",
                                        html: true,
                                        gravity: gravity || "n", //w,n,s,e...
                                        fade: false
                                    })
                                    .tipsy("enable");
                                $(control).tipsy("show");

                                setTimeout(function () { $(control).tipsy("hide") }, 2000, null);

                            }, 100, null);
                        }

                        control.data("tipsTimer", timer);
                    }
                });
            });
            return result;
        }
    }
    // 目前暂未考虑多级的情况
    , FindFieldCtrl: function (field) {
        var context = $(this);
        var ctrl = context.find("[data-field='" + field + "']");
        return ctrl;
    }
});


(function ($) {

    var menu, shadow, trigger, content, hash, currentTarget;
    var defaults = {
        menuStyle: {
            listStyle: 'none',
            padding: '1px',
            margin: '0px',
            backgroundColor: '#fff',
            border: '1px solid #999',
            width: '100px'
        },
        itemStyle: {
            margin: '0px',
            color: '#000',
            display: 'block',
            cursor: 'default',
            padding: '3px',
            border: '1px solid #fff',
            backgroundColor: 'transparent'
        },
        itemHoverStyle: {
            border: '1px solid #0a246a',
            backgroundColor: '#b6bdd2'
        },
        eventPosX: 'pageX',
        eventPosY: 'pageY',
        shadow: true,
        onContextMenu: null,
        onShowMenu: null
    };

    $.fn.contextMenu = function (id, options) {
        if (!menu) {                                      // Create singleton menu
            menu = $('<div id="jqContextMenu"></div>')
                     .hide()
                     .css({ position: 'absolute', zIndex: '500' })
                     .appendTo('body')
                     .bind('click', function (e) {
                         e.stopPropagation();
                     });
        }
        if (!shadow) {
            shadow = $('<div></div>')
                       .css({ backgroundColor: '#000', position: 'absolute', opacity: 0.2, zIndex: 499 })
                       .appendTo('body')
                       .hide();
        }
        hash = hash || [];
        hash.push({
            id: id,
            menuStyle: $.extend({}, defaults.menuStyle, options.menuStyle || {}),
            itemStyle: $.extend({}, defaults.itemStyle, options.itemStyle || {}),
            itemHoverStyle: $.extend({}, defaults.itemHoverStyle, options.itemHoverStyle || {}),
            bindings: options.bindings || {},
            shadow: options.shadow || options.shadow === false ? options.shadow : defaults.shadow,
            onContextMenu: options.onContextMenu || defaults.onContextMenu,
            onShowMenu: options.onShowMenu || defaults.onShowMenu,
            eventPosX: options.eventPosX || defaults.eventPosX,
            eventPosY: options.eventPosY || defaults.eventPosY
        });

        var index = hash.length - 1;
        $(this).bind('contextmenu', function (e) {
            // Check if onContextMenu() defined
            var bShowContext = (!!hash[index].onContextMenu) ? hash[index].onContextMenu(e) : true;
            if (bShowContext) display(index, this, e, options);
            return false;
        });
        return this;
    };

    function display(index, trigger, e, options) {
        var cur = hash[index];
        content = $('#' + cur.id).find('ul:first').clone(true);
        content.css(cur.menuStyle).find('li').css(cur.itemStyle).hover(
          function () {
              $(this).css(cur.itemHoverStyle);
          },
          function () {
              $(this).css(cur.itemStyle);
          }
        ).find('img').css({ verticalAlign: 'middle', paddingRight: '2px' });

        // Send the content to the menu
        menu.html(content);

        // if there's an onShowMenu, run it now -- must run after content has been added
        // if you try to alter the content variable before the menu.html(), IE6 has issues
        // updating the content
        if (!!cur.onShowMenu) menu = cur.onShowMenu(e, menu);

        $.each(cur.bindings, function (id, func) {
            $('#' + id, menu).bind('click', function (e) {
                hide();
                func(trigger, currentTarget);
            });
        });

        menu.css({ 'left': e[cur.eventPosX], 'top': e[cur.eventPosY] }).show();
        if (cur.shadow) shadow.css({ width: menu.width(), height: menu.height(), left: e.pageX + 2, top: e.pageY + 2 }).show();
        $(document).one('click', hide);
    }

    function hide() {
        menu.hide();
        shadow.hide();
    }

    // Apply defaults
    $.contextMenu = {
        defaults: function (userDefaults) {
            $.each(userDefaults, function (i, val) {
                if (typeof val == 'object' && defaults[i]) {
                    $.extend(defaults[i], val);
                }
                else defaults[i] = val;
            });
        }
    };

})(jQuery);

$(function () {
    $('div.contextMenu').hide();
});

(function ($) {
    $.fn.Wanda_UserSelect = function (options) {
        //脚本中定义的变量
        var _constData = {
            WebUrl: "",
            Token: "",
            IFrameWidth: 845,
            IFrameHeight: 570
        };

        var windowWidth = $(window).width() * 0.9;
        var windowHeight = $(window).height() * 0.9;

        if (windowWidth < _constData.IFrameWidth) {
            _constData.IFrameWidth = windowWidth;
        }
        if (windowHeight < _constData.IFrameHeight) {
            _constData.IFrameHeight = windowHeight;
        }
        //客户端可设置的变量
        var _settings = {
            AppCode: "",
            OnSubmit: function (users) { return true; },
            AllowMulti: true,
            AllowAll: true,
            WaitUser: [],
            CheckedUser: [],
            RequestTime: 5000,
            RequestIntervalTime: 200,
            Param: "",
            BizAppCode: "",
            WebUrl: ""
        };
        //设置属性，
        var options = $.extend(_settings, options);
        if (_settings.AppCode == "") {
            _settings.AppCode = wanda_wf_tool.getApplicationCode();
        }
        if (_settings.AppCode == "") {
            alert("AppCode不能为空");
            return;
        }
        if (_settings.WebUrl != "") {
            if (_settings.WebUrl.charAt(_settings.WebUrl.length - 1) == "/") {
                _settings.WebUrl = _settings.WebUrl.substr(0, _settings.WebUrl.length - 1);
            }
            _constData.WebUrl = _settings.WebUrl;
        }
        else {
            _constData.WebUrl = wanda_wf_tool.getWorkFlowServerURL();
        }
        if (!_settings.AllowAll) {
            _constData.IFrameHeight = 380;
        }
        var requestFlag = false;//轮询请求标识，为True表示轮询到数据
        var requestIntervalInt;//setInterval返回的值，用于清除轮询
        var requestIntervalTime = _settings.RequestIntervalTime;//setInterval轮询频率
        var divIframeTemp;//用于关闭窗口时使用

        ////注册需要的样式到页面Head中
        //var cssStr = '.wanda_user_popup{overflow:hidden;position:absolute;width:1000px;margin:auto auto;padding:7px;background:#fff;z-index:10001;background:#fff;}.wanda_user_t_l,.wanda_user_t_m,.wanda_user_t_r{position:absolute;top:0;z-index:2;height:7px;font-size:0%;}.wanda_user_t_l{left:0;width:7px;}.wanda_user_t_m{z-index:1;width:100%;}.wanda_user_t_r{right:0;width:7px;}.wanda_user_m_l,.wanda_user_m_r{position:absolute;z-index:2;width:7px;}.wanda_user_m_l{top:0px;left:0;z-index:1;}.wanda_user_m_r{top:0px;right:0;z-index:1;}.wanda_user_b_l,.wanda_user_b_m,.wanda_user_b_r{position:absolute;bottom:0;z-index:2;height:7px;font-size:0%;}.wanda_user_b_l{left:0;width:7px;}.wanda_user_b_m{z-index:1;width:100%;}.wanda_user_b_r{right:0;width:7px;}.wanda_user_m_m{width:100%;font-size:12px;}.wanda_user_pop_titile{color:blue;font-size:14px;margin:0;position:relative;background:#999;}.wanda_user_pop_tx_titile{}.wanda_user_pop_txt{margin-left:6px;margin-right:10px;height:35px;line-height:35px;font-size:14px;font-weight:bold;color:#fff;}.wanda_user_pop_titile .wanda_user_help{position:absolute;right:20px;top:5px;padding-right:10px;cursor:pointer;}.wanda_user_pop_titile .wanda_user_exit{position:absolute;right:0;top:3px;padding-right:10px;cursor:pointer;}.wanda_user_pop_bottom{height:50px;line-height:50px;margin:0;text-align:right;padding-right:5px;border-top:1px solid #ccc;clear:both;}.wanda_user_pop_icon{width:38px;height:54px;display:inline-block;position:absolute;left:8px;top:-10px;z-index:1;}.wanda_user_pop_icon_02{background:url(/images/popup/pop_icon02.png) no-repeat;height:64px;}.wanda_user_pop_icon_03{background:url(/images/popup/pop_icon03.png) no-repeat;height:72px;}.wanda_user_popup_content{max-height:480px;padding:10px;border:1px solid #dcdcdc;}.wanda_user_pop_btn{margin:0 10px;}.wanda_user_btn_fr{display:inline;float:right;margin:0px 10px 10px 10px;}.wanda_user_btn_fr01{display:inline !important;margin:0px !important;}.wanda_user_btn01{color:#fff;width:90px;background:rgb(213,0,12);margin:0px 3px;padding:0px;border-radius:3px;line-height:26px;border:1px solid rgb(158,0,0);border-image:none;height:26px;text-align:center;color:rgb(255,255,255);text-indent:0px;font-family:"Microsoft YaHei";font-size:14px;display:inline-block;cursor:pointer;}.wanda_user_btn01 a{color:#fff;font-size:14px;font-weight:bold;text-decoration:none;display:inline-block;cursor:pointer;}.wanda_user_btn_blue01 a{background:url(/images/btn_blue_l.png) left no-repeat !important;height:24px !important;line-height:24px !important;font-size:12px !important;font-weight:bold !important;text-decoration:none !important;display:inline-block !important;cursor:pointer !important;padding:0 !important;}.wanda_user_btn_blue01 a span{background:url(/images/btn_blue_r.png) right no-repeat !important;height:24px !important;line-height:24px !important;margin-left:5px !important;padding:0 20px 0 15px !important;display:inline-block !important;color:#fff !important;cursor:pointer !important;}.wanda_user_input205{width:120px;height:15px;}.wanda_user_btn_search{text-decoration:none;color:#333;}div.wanda_user_maskDiv{position:absolute;top:0;left:0;background-color:#111;filter:Alpha(opacity=70);opacity:0.7;z-index:10000;}.wanda_user_chkItemRole{TEXT-ALIGN:left;WIDTH:400px;DISPLAY:inline;FLOAT:left;}.wanda_user_dnrHandler{cursor:move;}.wanda_user_resizeHandler{cursor:se-resize;}.wanda_user_clearfix:after{content:".";display:block;height:0;clear:both;visibility:hidden;}.wanda_user_overflow{overflow-y:scroll;}.wanda_user_clearfix{display:inline-block;}';
        //AddCssByStyle(cssStr);

        //注册点击事件
        var $ctrl = $(this);
        $ctrl.unbind("click").bind("click", function () {
            requestFlag = false;
            _constData.Token = GeneralToken();
            if ($ctrl.attr("valid") == "false") return;
            if ($ctrl.is(":disabled")) {
                return;
            }

            //添加需要的DOM元素
            var div = $("<div>", { style: "display: block" });
            var divIFrame = $("<div>", { id: "wanda_user_div_iframe", width: _constData.IFrameWidth, height: _constData.IFrameHeight });
            var iframe;
            try { // for I.E.
                iframe = document.createElement('<iframe name="wanda_user_iframe" id="wanda_user_iframe" frameborder="no">');
            } catch (ex) { //for other browsers, an exception will be thrown
                iframe = $("<iframe>", { id: "wanda_user_iframe", name: "wanda_user_iframe", src: "", width: _constData.IFrameWidth, height: _constData.IFrameHeight, border: 0, frameborder: "no" });
            }

            iframe.id = 'wanda_user_iframe';
            iframe.name = 'wanda_user_iframe';
            iframe.width = _constData.IFrameWidth;
            iframe.height = _constData.IFrameHeight;
            iframe.marginHeight = 0;
            iframe.marginWidth = 0;
            iframe.frameborder = "no";
            iframe.border = 0;
            iframe.src = "";
            div.append(divIFrame.append(iframe));
            $("body").append(div);

            //弹出窗口
            $(divIFrame).Wanda_OpenDiv({
                title: '选择用户',
                isShowExit: true,
                onCancel: function () {
                    RequestError("", "OK");
                    return true;
                }
            });
            divIframeTemp = divIFrame;
            //设置需要POST的参数
            var Params = {
                AppCode: _settings.AppCode,
                Token: _constData.Token,
                Param: _settings.Param,
                WaitUser: JSON.stringify(_settings.WaitUser),
                CheckedUser: JSON.stringify(_settings.CheckedUser),
                AllowMulti: _settings.AllowMulti,
                AllowAll: _settings.AllowAll
            };
            //POST数据到选人控件页面
            PostForm(_constData.WebUrl + "/RuntimeService/UserSelect/SelectUser.aspx", Params, "wanda_user_iframe");
            CallDataPolling();
        });

        function RequestError(msg, type) {
            requestFlag = true;
            window.clearInterval(requestIntervalInt);
            divIframeTemp.CloseDiv();
            type = type || "Error";

            if (type == "Error") {
                alert("Client-Error:" + msg);
            }
            else if (type == "Info") {
                alert("Client-Info:" + msg);
            }
            else if (type == "Cancel") {

            }
        }
        //生成Token，格式（guid+"_"+math.random()+"_"+时间），例如：54732d49-195a-4e7f-ac09-0ad63ae2ee40_0.7057493212632835_20150731104317443
        function GeneralToken() {
            var guid = "";
            try {
                guid = uuid.v4();
            }
            catch (mvm) {
                (function () { var _global = this; var _rng; if (typeof (_global.require) == 'function') { try { var _rb = _global.require('crypto').randomBytes; _rng = _rb && function () { return _rb(16) } } catch (e) { } } if (!_rng && _global.crypto && crypto.getRandomValues) { var _rnds8 = new Uint8Array(16); _rng = function whatwgRNG() { crypto.getRandomValues(_rnds8); return _rnds8 } } if (!_rng) { var _rnds = new Array(16); _rng = function () { for (var i = 0, r; i < 16; i++) { if ((i & 0x03) === 0) r = Math.random() * 0x100000000; _rnds[i] = r >>> ((i & 0x03) << 3) & 0xff } return _rnds } } var BufferClass = typeof (_global.Buffer) == 'function' ? _global.Buffer : Array; var _byteToHex = []; var _hexToByte = {}; for (var i = 0; i < 256; i++) { _byteToHex[i] = (i + 0x100).toString(16).substr(1); _hexToByte[_byteToHex[i]] = i } function parse(s, buf, offset) { var i = (buf && offset) || 0, ii = 0; buf = buf || []; s.toLowerCase().replace(/[0-9a-f]{2}/g, function (oct) { if (ii < 16) { buf[i + ii++] = _hexToByte[oct] } }); while (ii < 16) { buf[i + ii++] = 0 } return buf } function unparse(buf, offset) { var i = offset || 0, bth = _byteToHex; return bth[buf[i++]] + bth[buf[i++]] + bth[buf[i++]] + bth[buf[i++]] + '-' + bth[buf[i++]] + bth[buf[i++]] + '-' + bth[buf[i++]] + bth[buf[i++]] + '-' + bth[buf[i++]] + bth[buf[i++]] + '-' + bth[buf[i++]] + bth[buf[i++]] + bth[buf[i++]] + bth[buf[i++]] + bth[buf[i++]] + bth[buf[i++]] } var _seedBytes = _rng(); var _nodeId = [_seedBytes[0] | 0x01, _seedBytes[1], _seedBytes[2], _seedBytes[3], _seedBytes[4], _seedBytes[5]]; var _clockseq = (_seedBytes[6] << 8 | _seedBytes[7]) & 0x3fff; var _lastMSecs = 0, _lastNSecs = 0; function v1(options, buf, offset) { var i = buf && offset || 0; var b = buf || []; options = options || {}; var clockseq = options.clockseq != null ? options.clockseq : _clockseq; var msecs = options.msecs != null ? options.msecs : new Date().getTime(); var nsecs = options.nsecs != null ? options.nsecs : _lastNSecs + 1; var dt = (msecs - _lastMSecs) + (nsecs - _lastNSecs) / 10000; if (dt < 0 && options.clockseq == null) { clockseq = clockseq + 1 & 0x3fff } if ((dt < 0 || msecs > _lastMSecs) && options.nsecs == null) { nsecs = 0 } if (nsecs >= 10000) { throw new Error('uuid.v1(): Can\'t create more than 10M uuids/sec') } _lastMSecs = msecs; _lastNSecs = nsecs; _clockseq = clockseq; msecs += 12219292800000; var tl = ((msecs & 0xfffffff) * 10000 + nsecs) % 0x100000000; b[i++] = tl >>> 24 & 0xff; b[i++] = tl >>> 16 & 0xff; b[i++] = tl >>> 8 & 0xff; b[i++] = tl & 0xff; var tmh = (msecs / 0x100000000 * 10000) & 0xfffffff; b[i++] = tmh >>> 8 & 0xff; b[i++] = tmh & 0xff; b[i++] = tmh >>> 24 & 0xf | 0x10; b[i++] = tmh >>> 16 & 0xff; b[i++] = clockseq >>> 8 | 0x80; b[i++] = clockseq & 0xff; var node = options.node || _nodeId; for (var n = 0; n < 6; n++) { b[i + n] = node[n] } return buf ? buf : unparse(b) } function v4(options, buf, offset) { var i = buf && offset || 0; if (typeof (options) == 'string') { buf = options == 'binary' ? new BufferClass(16) : null; options = null } options = options || {}; var rnds = options.random || (options.rng || _rng)(); rnds[6] = (rnds[6] & 0x0f) | 0x40; rnds[8] = (rnds[8] & 0x3f) | 0x80; if (buf) { for (var ii = 0; ii < 16; ii++) { buf[i + ii] = rnds[ii] } } return buf || unparse(rnds) } var uuid = v4; uuid.v1 = v1; uuid.v4 = v4; uuid.parse = parse; uuid.unparse = unparse; uuid.BufferClass = BufferClass; if (typeof (module) != 'undefined' && module.exports) { module.exports = uuid } else if (typeof define === 'function' && define.amd) { define(function () { return uuid }) } else { var _previousRoot = _global.uuid; uuid.noConflict = function () { _global.uuid = _previousRoot; return uuid }; _global.uuid = uuid } }).call(this);
                guid = uuid.v4();
            }
            var myDate = new Date();
            var year = myDate.getFullYear();
            var month = ("0" + (myDate.getMonth() + 1)).slice(-2);
            var day = ("0" + myDate.getDate()).slice(-2);
            var h = ("0" + myDate.getHours()).slice(-2);
            var m = ("0" + myDate.getMinutes()).slice(-2);
            var s = ("0" + myDate.getSeconds()).slice(-2);
            var mi = ("00" + myDate.getMilliseconds()).slice(-3);
            var timeInt = year + month + day + h + m + s + mi;
            return guid + "_" + Math.random() + "_" + timeInt;
        }

        //轮询数据
        function CallDataPolling() {
            requestIntervalInt = setInterval(function () {
                if (!requestFlag) {
                    requestFlag = true;

                    var paramData = { RequestTime: _settings.RequestTime };
                    if (_settings.BizAppCode != "") {
                        paramData.BizAppCode = _settings.BizAppCode;
                    }
                    if (_settings.WebUrl != "") {
                        paramData.BizWFURL = _settings.WebUrl;
                    }
                    $.ajax({
                        type: "POST",
                        url: "CommonHandler.ashx?RequestType=Request&Action=UserSelect&Method=GetUserList&Token=" + _constData.Token + "&t=" + new Date().getMilliseconds(),
                        dataType: "json",
                        async: true,
                        data: paramData,
                        success: function (data) {
                            requestFlag = false;
                            if (data.IsSuccess) {
                                requestFlag = true;
                                window.clearInterval(requestIntervalInt);
                                divIframeTemp.CloseDiv();
                                try {
                                    //$("input[type='text']")[0].focus();
                                }
                                catch (ex) {
                                }
                                var result = JSON.parse(data.Data);
                                _settings.OnSubmit(result);
                            }
                            else {
                                if (data.StatusCode > 0 && data.StatusCode <= 200) {
                                    RequestError(data.StatusMessage, "Info");
                                }
                                else if (data.StatusCode > 200) {
                                    RequestError(data.StatusMessage);
                                }
                            }
                        },
                        error: function (XMLHttpRequest, textStatus, errorThrown) {
                            RequestError(errorThrown);
                        }
                    });
                }
            }, requestIntervalTime);
        }
        //POST数据到选人页面
        function PostForm(URL, PARAMS, IFRAMEID) {
            var temp = document.createElement("form");
            temp.action = URL;
            temp.method = "post";
            temp.target = IFRAMEID;
            temp.style.display = "none";
            for (var x in PARAMS) {
                var opt = document.createElement("textarea");
                opt.name = x;
                opt.value = PARAMS[x];
                temp.appendChild(opt);
            }
            document.body.appendChild(temp);
            temp.submit();
            return temp;
        }
    }
})(jQuery)

var wanda_wf_data = {
    BizContext: {
        AppCode: "",
        FlowCode: "",
        BusinessID: "",
        WFToken: "",
        FormParams: null,
        DynamicRoleUserList: null,
        CurrentUser: null,
        CheckUserInProcess: true,
        ProcessRunningNodeID: "",
        ProcessTitle: "",
        ProcessURL: "",
        ProcessMobileURL: "",
        NodeInstanceList: null,
        CcNodeInstanceList: null,
        ApprovalContent: "",
        ExtensionCommond: {}
    },
    WorkFlowContext: null,
    ErrorCode: {
        ProcessNotExist: 211
    },
    ClientContext: {
        CcUserList: [],
        ForwardUserList: [],
        AddNodeUserList: [],
        CurrentAddNode: {}
    },
    OtherContext: {
        NodeInstanceArray: [],
        StartNode: null,
        NavNodeDom: {}
    },
    _buildAjaxData: function (methodName, step) {
        this._buildCommonData();
        if (step == undefined) step = 0;
        return { MethodName: methodName, MethodMode: step, Version: '1.0', BizContext: JSON.stringify(wanda_wf_data.BizContext), BizWFURL: this._bizWFURL };
    },
    _bizWFURL: "",
    _buildPostData: function (methodName, operatorType) {
        this._buildCommonData();
        var executeParam = {
            OperatorType: operatorType,
            MethodName: methodName,
            Version: '1.0',
            BizContext: wanda_wf_data.BizContext,
            WorkflowContext: wanda_wf_data.WorkFlowContext
        }
        return executeParam;
    },
    _clearContext: function () {
        wanda_wf_data.ClientContext.CcUserList = [];
        wanda_wf_data.ClientContext.ForwardUserList = [];
        wanda_wf_data.ClientContext.CurrentAddNode = {};
        wanda_wf_data.OtherContext.NodeInstanceArray = [];
        wanda_wf_data.OtherContext.StartNode = null;
    },
    _getRunningNode: function () {
        //return wanda_wf_data.BizContext.NodeInstanceList[wanda_wf_data.BizContext.ProcessRunningNodeID];
        return wanda_wf_data.WorkFlowContext.NodeInstanceList[wanda_wf_data.WorkFlowContext.ProcessInstance.RunningNodeID];
    },
    _getCloneNode: function (selectCloneNode) {
        var runningNode;
        if (selectCloneNode != undefined) {
            runningNode = selectCloneNode;
        }
        else {
            runningNode = wanda_wf_data._getRunningNode();
        }
        var runningNodeType = runningNode.NodeType;
        var cloneNode = null;
        if (runningNodeType == 0) {
            //发起节点使用下一节点来作为克隆节点
            var nextNode = wanda_wf_data.BizContext.NodeInstanceList[runningNode.NextNodeID];
            while (nextNode != null && (nextNode.NodeType == 5 || nextNode.NodeType == 6 || wanda_wf_data.ClientContext.CurrentAddNode[nextNode.NodeID] != null)) {
                if (nextNode.NextNodeID != "") {
                    nextNode = wanda_wf_data.BizContext.NodeInstanceList[nextNode.NextNodeID];
                }
                else {
                    nextNode = null;
                }
            }
            if (nextNode != null) {
                cloneNode = $.extend(true, {}, nextNode);
            }
        }
        else {
            //其它节点使用当前节点作为克隆节点
            cloneNode = $.extend(true, {}, runningNode);
        }
        return cloneNode;
    },
    _getNodeNameAndUserFormateText: function (node, nodeDataArray) {
        //返回的格式：发起人【XXX】，会签活动【XXX,XX】,
        var result = "";
        if (node.NodeType == 2 || node.NodeType == 3) {
            result = result + node.NodeName + "【";
            if (nodeDataArray == undefined || nodeDataArray == null) {
                nodeDataArray = wanda_wf_data.OtherContext.NodeInstanceArray;
            }
            var itemNodeList = Enumerable.From(nodeDataArray).Where("$.ParentNodeID == '" + node.NodeID + "'").OrderBy("$.NodeOrder").ToArray();
            var len = itemNodeList.length;
            $.each(itemNodeList, function (i, item) {
                result = result + item.User.UserName;
                if ((len - 1) != i) {
                    result = result + ",";
                }
            })
            result = result + "】";
        }
        else {
            if (node.User != null) {
                result = result + node.NodeName + "【" + node.User.UserName + "】";
            }
            else {
                result = result + node.NodeName;
            }
        }
        return result;
    },
    _getReturnNodeList: function (currentNode) {

    },
    _getAddNodeList: function (currentNode) {
        var nextNodeList = [];
        if (currentNode == null) return nextNodeList;
        nextNodeList.push(currentNode);
        var nextNode = wanda_wf_data.WorkFlowContext.NodeInstanceList[currentNode.NextNodeID];
        while (nextNode != null) {
            if (nextNode.NodeType == 1 || nextNode.NodeType == 2 || nextNode.NodeType == 0 || nextNode.NodeType == 3 || nextNode.NodeType == 7) {
                nextNodeList.push(nextNode);
            }
            if (nextNode.NextNodeID != "") {
                nextNode = wanda_wf_data.WorkFlowContext.NodeInstanceList[nextNode.NextNodeID]
            }
            else {
                nextNode = null;
            }
        }
        return nextNodeList;
    },
    _initBizContextByWorkFlowContext: function (workflowContext) {
        wanda_wf_data.BizContext.AppCode = workflowContext.AppCode;
        wanda_wf_data.BizContext.BusinessID = workflowContext.BusinessID;
        wanda_wf_data.BizContext.NodeInstanceList = $.extend(true, {}, workflowContext.NodeInstanceList);
        wanda_wf_data.BizContext.CcNodeInstanceList = $.extend(true, {}, workflowContext.CcNodeInstanceList);
        if (workflowContext.ProcessInstance != null) {
            wanda_wf_data.BizContext.FlowCode = workflowContext.ProcessInstance.FlowCode;
            wanda_wf_data.BizContext.ProcessTitle = workflowContext.ProcessInstance.ProcessTitle;
            wanda_wf_data.BizContext.ProcessURL = workflowContext.ProcessInstance.ProcessURL;
            wanda_wf_data.BizContext.ProcessMobileURL = workflowContext.ProcessInstance.ProcessMobileURL;
            wanda_wf_data.BizContext.ProcessRunningNodeID = workflowContext.ProcessInstance.RunningNodeID;
        }
    },
    _initOtherContext: function (workflowContext) {
        //处理其它Context信息
        var nodeDataObject = workflowContext.NodeInstanceList;
        wanda_wf_data.OtherContext.NodeInstanceArray = [];
        wanda_wf_data.OtherContext.StartNode = null;
        if (nodeDataObject != null) {
            for (var key in nodeDataObject) {
                var nodeTemp = nodeDataObject[key];
                wanda_wf_data.OtherContext.NodeInstanceArray.push(nodeTemp);
                if (wanda_wf_data.OtherContext.StartNode == null) {
                    //如果服务器端的StartNodeID错误，则重新计算获取startNode
                    if (nodeTemp.NodeType == 0) {
                        wanda_wf_data.OtherContext.StartNode = nodeDataObject[key];
                    }
                }
            }
        }
    },
    _buildCcNode: function (user, nodeID) {
        return {
            NodeID: nodeID,
            NodeName: "抄送节点",
            NodeType: 4,
            Status: 0,
            ExtendProperties: {
                IsStartUser: "False",
                AllowMergeNode: "False",
                RejectNeedRepeatRun: "True",
                AllowChooseAnyUser: "True"
            },
            User: user,
            CreateDateTime: new Date(),
            CreateUser: wanda_wf_data.BizContext.CurrentUser,
            UpdateDateTime: new Date(),
            UpdateUser: wanda_wf_data.BizContext.CurrentUser
        };
    },
    _buildCommonData: function () {
        if (wanda_wf_data.WorkFlowContext != null) {
            //构建抄送节点信息
            wanda_wf_data.BizContext.CcNodeInstanceList = $.extend(true, {}, wanda_wf_data.WorkFlowContext.CcNodeInstanceList);
            $.each(wanda_wf_data.ClientContext.CcUserList, function (i, item) {
                var nodeID = uuid.v4();
                wanda_wf_data.BizContext.CcNodeInstanceList[nodeID] = wanda_wf_data._buildCcNode(item, nodeID);
            });
        }
        if (wanda_wf_data.BizContext.CcNodeInstanceList == {
        }) {
            wanda_wf_data.BizContext.CcNodeInstanceList = null;
        }
        if (wanda_wf_data.BizContext.NodeInstanceList == {
        }) {
            wanda_wf_data.BizContext.NodeInstanceList = null;
        }
    },
    _setWFURL: function (url) {
        this._bizWFURL = url;
    }
}

var wanda_wf_tool = {
    log: function (data, isEnable) {
        isEnable = isEnable == undefined ? false : isEnable;
        if (isEnable) {
            try {
                if (typeof data == "object") {
                    console.log(data);
                }
                else {
                    console.log(data);
                }
            }
            catch (ee) {
            }
        }
    },
    getQueryString: function (name) {
        var result = location.search.match(new RegExp("[\?\&]" + name + "=([^\&]+)", "i"));
        if (result == null || result.length < 1) {
            return "";
        }
        return result[1];
    },
    ajax: function (func, data) {
        $.ajax({
            type: "POST",
            url: "ProcessHandler.ashx?t=" + new Date().getMilliseconds(),
            dataType: "json",
            async: true,
            data: data,
            beforeSend: function () {
                wanda_wf_tool.showLoading();
            },
            success: function (dataTemp) {
                wanda_wf_tool.hideLoading();
                //try {
                func(dataTemp);
                //}
                //catch (ee) {
                //    wanda_wf_tool.alert(ee.name + ": " + ee.message);
                //}
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                wanda_wf_tool.alert(errorThrown);
                wanda_wf_tool.hideLoading();
                throw (errorThrown);
            }
        });
    },
    _workFlowServerURL: "",
    _applicationCode: "",
    getApplicationCode: function () {
        //返回的是不带/的地址
        if (wanda_wf_tool._applicationCode != "") {
            return wanda_wf_tool._applicationCode;
        }
        this.ajaxCommonHandler({
            RequestType: "AppSetting",
            Action: "ApplicationCode",
            SuccessReturn: function (data) {
                if (data != "") {
                    if (data.charAt(data.length - 1) == "/") {
                        data = data.substr(0, data.length - 1);
                    }
                    wanda_wf_tool._applicationCode = data;
                }
            }
        });
        return wanda_wf_tool._applicationCode;
    },
    getWorkFlowServerURL: function () {
        //返回的是不带/的地址
        if (wanda_wf_tool._workFlowServerURL != "") {
            return wanda_wf_tool._workFlowServerURL;
        }
        this.ajaxCommonHandler({
            RequestType: "AppSetting",
            Action: "WorkflowServerUrl",
            SuccessReturn: function (data) {
                if (data != "") {
                    if (data.charAt(data.length - 1) == "/") {
                        data = data.substr(0, data.length - 1);
                    }
                    wanda_wf_tool._workFlowServerURL = data;
                }
            }
        });
        return wanda_wf_tool._workFlowServerURL;
    },
    ajaxMaintenanace: function (func, methodName, data, isAsync) {
        this.ajaxCommon(func, "ProcessMaintenance", methodName, data, isAsync, undefined, "ProcessMaintenanceHandler");
    },
    ajaxCommon: function (func, action, methodName, data, isAsync, token, handler) {
        var defaultOption = {
            SuccessReturn: func,//回调函数
            Action: action,//动作类别
            Method: methodName,//方法名称
            Token: token,//标识
            IsAsync: isAsync,//是否异步
            Data: data,//POST的数据
            Handler: handler//请求的Handler
        }
        this.ajaxCommonHandler(defaultOption);
    },
    ajaxCommonHandler: function (option) {
        var defaultOption = {
            SuccessReturn: function () { },//回调函数
            RequestType: "Request",//请求类型
            Action: "",//动作类别
            Method: "",//方法名称
            Token: "",//标识
            IsAsync: false,//是否异步
            Data: {},//POST的数据
            Handler: "CommonHandler"//请求的Handler
        }
        $.extend(true, defaultOption, option);
        if (defaultOption.Token == "") {
            defaultOption.Token = GeneralToken();
        }
        $.ajax({
            type: "POST",
            url: defaultOption.Handler + ".ashx?RequestType=" + defaultOption.RequestType + "&Action=" + defaultOption.Action + "&Method=" + defaultOption.Method + "&Token=" + defaultOption.Token + "&t=" + new Date().getMilliseconds(),
            dataType: "json",
            async: defaultOption.IsAsync,
            data: defaultOption.Data,
            beforeSend: function () {
                wanda_wf_tool.showLoading();
            },
            success: function (dataTemp) {
                wanda_wf_tool.hideLoading();
                if (dataTemp.IsSuccess) {
                    if (defaultOption.SuccessReturn != undefined) {
                        defaultOption.SuccessReturn(dataTemp.Data);
                    }
                }
                else {
                    if (dataTemp.StatusCode > 0 && dataTemp.StatusCode <= 200) {
                        wanda_wf_tool.tips(dataTemp.StatusMessage);
                    }
                    else if (dataTemp.StatusCode > 200) {
                        wanda_wf_tool.alert(dataTemp.StatusMessage);
                    }
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                wanda_wf_tool.alert(errorThrown);
                wanda_wf_tool.hideLoading();
                throw (errorThrown);
            }
        });

    },
    // 将json对象转换为js string
    jsonToString: function (data, replacer, space) {// replacer and space are optional
        if ((typeof JSON == "undefined") || (typeof JSON.stringify == "undefined"))
            throw new Error("Cannot find JSON.stringify(). Some browsers (e.g., IE < 8) don't support it natively, but you can overcome this by adding a script reference to json2.js, downloadable from http://www.json.org/json2.js");
        return JSON.stringify(data, replacer, space);
    },
    // 将简单数组对象转换为js string
    // 注：未处理双引号
    arrayToString: function (array) {
        if (arguments.length == 0)
            throw new Error("no arguments, pass the object you want to convert.");
        if (!array || typeof array == "undefined" || array.length < 1) {
            return "";
        }
        return "[\"" + array.join("\",\"") + "\"]";
    },
    formateError: function (workflowContext, type, is) {
        type = (type == undefined || type == null) ? "error" : type;
        var message = "错误信息：";
        if (type == "info") {
            message = "提示信息：" + workflowContext.StatusMessage;
        }
        else {
            message = message + workflowContext.StatusMessage;
            if (workflowContext.LastException != null) {
                message = message + "\n" + workflowContext.LastException.Message;
            }
        }
        return message;
    },
    alert: function (value) {
        alert(value);
    },
    tips: function (value) {
        //if ($(".alert-msg").length) return;
        var container = $("<div>").addClass("alert-msg"), inner = $("<span>").addClass("alert-msg-content");
        value = value.replace("\r\n", "<br/>");
        $(inner).text(value);
        $(container).append($(inner)).appendTo($("body"));
        setTimeout(function () {
            $(container).css({ opacity: 0 });
            setTimeout(function () { $(container).remove() }, 500);
        }, 3000);
    },
    operation: {
        timeTest: null,                     //延时器
        loadingCount: 0,                    //计数器 当同时被多次调用的时候 记录次数
        loadingImgUrl: "../img/loading.gif",  //默认load图地址
        loadingImgHeight: 24,               //Loading图的高
        loadingImgWidth: 24                 //Loading图的宽
    },
    //显示全屏Loading图
    showLoading: function (msg) {
        if (msg === undefined) {
            msg = "操作中, 请稍等...";
        }
        if ($("#div_loadingImg").length == 0) {
            $("body").append("<div id='div_loadingImg'></div>");
        }
        $("#div_loadingImg").append("<div id='loadingPage_bg' class='loadingPage_bg1'></div><div id='loadingPage'>" + msg + "</div>");
        $("#loadingPage_bg").height($(top.window.document).height()).width($(top.window.document).width());
    },
    //隐藏全屏Loading图
    hideLoading: function () {
        $("#div_loadingImg").empty();
        $("#div_loadingImg").remove();
    },
    bindUserSelect: function (obj, func, appcode, allowMulti, allowAll, waitUser, checkedUser, exceptArray) {
        allowMulti = (allowMulti == undefined || allowMulti == null) ? true : allowMulti;
        allowAll = (allowAll == undefined || allowAll == null) ? true : allowAll;
        appcode = (appcode == undefined || appcode == null) ? wanda_wf_data.BizContext.AppCode : appcode;
        waitUser = (waitUser == undefined || waitUser == null) ? [] : waitUser;
        checkedUser = (checkedUser == undefined || checkedUser == null) ? [] : checkedUser;
        obj.Wanda_UserSelect({
            AppCode: appcode,
            AllowMulti: allowMulti,
            AllowAll: allowAll,
            WaitUser: waitUser,
            CheckedUser: checkedUser,
            OnSubmit: function (data) {
                if (func != undefined) {
                    data = Enumerable.From(data).Distinct("$.UserCode").ToArray();
                    if (exceptArray != null && exceptArray != undefined) {
                        data = Enumerable.From(data).Except(exceptArray, "$.UserCode").ToArray();
                    }
                    func(data)
                }
            }
        })
    },
    appendUserListToDom: function (domObj, data, isClearDom, funcAdd, funcDelete) {
        isClearDom = (isClearDom == undefined || isClearDom == null) ? false : isClearDom;
        $.each(data, function (i, item) {
            var span = $("<span>", { "class": "" });
            var spDel = $("<span>", { "class": "" }).append("<img class='imgDelete' alt='删除'>");
            span.append($("<span>").text(item.UserName).attr("title", wanda_wf_tool.formateUserTips(item)));
            span.append(spDel);
            spDel.click(function () {
                $(this).parent().remove();
                if (funcDelete != undefined) {
                    funcDelete(item);
                }
            });
            span.append($("<span>").text("；"));
            domObj.append(span);
            if (funcAdd != undefined) {
                funcAdd(item);
            }
        })
    },
    deleteArrayItem: function (array, item) {
        if (array != undefined && item != undefined) {
            array.splice(jQuery.inArray(item, array), 1);
        }
    },
    formateUserTips: function (userInfo) {
        if (userInfo == null) {
            return "";
        }
        //格式化用户显示
        return "姓名：" + userInfo.UserName + "(" + userInfo.UserLoginID + ")\r\n"
        + "职务：" + userInfo.UserJobName + "\r\n"
        + "组织机构：" + userInfo.UserOrgPathName + "\r\n";
    },
    formateNodeTips: function (node) {
        if (node == null || node == undefined) { return ""; }
        var nodeType = node.NodeType;
        switch (nodeType) {
            case 0:
                return "发起节点";
            case 1:
                return "审批节点";
            case 2:
                return "会签节点";
            case 3:
                return "通知节点";
            case 7:
                return "虚拟节点";
            default:
                return "";
        }
    },
    formatDate: function (date) {
        if (date.indexOf(".") >= 0) {
            return date.replace("T", " ").substr(0, date.indexOf(".")).replace(" ", "<br/>");
        }
        else {
            return date.replace("T", " ").replace(" ", "<br/>");
        }
    },
    convertToDate: function (value) {
        value = value.toString();
        if (value == "") { return null; }
        return new Date(value);
    },
    convertToInt: function (value) {
        value = value.toString();
        if (value == "") { return null; }
        return parseInt(value);
    },
    convertToFloat: function (value) {
        value = value.toString();
        if (value == "") { return null; }
        return parseFloat(value);
    },
    convertToBool: function (value) {
        value = value.toString().toLowerCase();
        if (value == "") { return false; }
        return value == "true" || value == "1";
    },
    convertToString: function (value) {
        value = value.toString();
        return value;
    },
    convertToType: function (value, type) {
        /*0:string,1:bool,2:number,3:date*/
        type = type.toString();
        switch (type) {
            case "1":
                return this.convertToBool(value);
            case "2":
                return this.convertToFloat(value);
            case "3":
                return this.convertToDate(value);
            default:
                return this.convertToString(value);

        }
    },
    getNodeNameAndUserFormateText: function (node, nodeDataArray, isDictionary) {
        //返回的格式：发起人【XXX】，会签活动【XXX,XX】,
        var result = "";
        if (node.NodeType == 2 || node.NodeType == 3) {
            result = result + node.NodeName + "【";
            var whereFilter = "$.ParentNodeID == '" + node.NodeID + "'";
            if (isDictionary == true) {
                whereFilter = "$.Value.ParentNodeID == '" + node.NodeID + "'";
            }

            var itemNodeList = Enumerable.From(nodeDataArray).Where(whereFilter).OrderBy("$.NodeOrder").ToArray();
            var len = itemNodeList.length;
            $.each(itemNodeList, function (i, item) {
                if (isDictionary == true) {
                    result = result + item.Value.User.UserName;
                }
                else {
                    result = result + item.User.UserName;
                }

                if ((len - 1) != i) {
                    result = result + ",";
                }
            })
            result = result + "】";
        }
        else {
            if (node.User != null) {
                result = result + node.NodeName + "【" + node.User.UserName + "】";
            }
            else {
                result = result + node.NodeName;
            }
        }
        return result;
    },
    initNodeByCloneNode: function (runningNode, cloneNode, nodeType, userInfo, preNodeID, nodeName) {
        var nodeID = uuid.v4();
        var nodeTemp = $.extend(true, {}, cloneNode);
        nodeTemp.NodeType = nodeType;
        nodeTemp.ParentNodeID = "";
        nodeTemp.CloneNodeID = cloneNode.NodeID;
        nodeTemp.CloneNodeName = cloneNode.NodeName;
        nodeTemp.CloneNodeType = cloneNode.NodeType;
        nodeTemp.User = userInfo;
        nodeTemp.NodeID = nodeID;
        nodeTemp.NodeName = nodeName;
        nodeTemp.Status = 0;
        nodeTemp.PrevNodeID = preNodeID;
        nodeTemp.NextNodeID = "";
        nodeTemp.StartDateTime = "00001-01-01 00:00:00";
        nodeTemp.FinishDateTime = "00001-01-01 00:00:00";
        return nodeTemp;
    },
    initAddNodeOrderList: function (runningNode, cloneNode, nodeName, addNodeUserList) {
        var preNodeID = "";
        var firstNodeID = "";
        var lastNodeID = "";
        var addNodeList = {};
        //顺序审批
        $.each(addNodeUserList, function (i, item) {
            var nodeTemp = wanda_wf_tool.initNodeByCloneNode(runningNode, cloneNode, 1, item, preNodeID, nodeName);
            addNodeList[nodeTemp.NodeID] = nodeTemp;
            if (preNodeID != "") {
                addNodeList[preNodeID].NextNodeID = nodeTemp.NodeID;
            }
            if (firstNodeID == "") {
                firstNodeID = nodeTemp.NodeID;
            }
            lastNodeID = nodeTemp.NodeID;
            preNodeID = nodeTemp.NodeID;
        })
        return {
            FirstNodeID: firstNodeID,
            LastNodeID: lastNodeID,
            AddNodeList: addNodeList,
            AddNodeArray: []
        }
    },
    initAddNodeCosignerList: function (runningNode, cloneNode, nodeName, addNodeUserList) {
        //同时审批
        var addNodeList = {};
        var addNodeArray = [];
        var nodeTemp = wanda_wf_tool.initNodeByCloneNode(runningNode, cloneNode, 2, null, "", nodeName);
        addNodeList[nodeTemp.NodeID] = nodeTemp;

        var firstNodeID = nodeTemp.NodeID;

        $.each(addNodeUserList, function (i, item) {
            var nodeItemTemp = wanda_wf_tool.initNodeByCloneNode(runningNode, cloneNode, 2, item, "", nodeName);
            nodeItemTemp.ParentNodeID = nodeTemp.NodeID;
            nodeItemTemp.PrevNodeID = "";
            nodeItemTemp.NextNodeID = "";
            addNodeList[nodeItemTemp.NodeID] = nodeItemTemp;
            addNodeArray.push(nodeItemTemp);
        })
        return {
            FirstNodeID: firstNodeID,
            LastNodeID: firstNodeID,
            AddNodeList: addNodeList,
            AddNodeArray: addNodeArray
        }
    },
    initAddNodeAutoInformList: function (runningNode, cloneNode, nodeName, addNodeUserList) {
        //通知
        var addNodeList = {};
        var addNodeArray = [];
        var nodeTemp = wanda_wf_tool.initNodeByCloneNode(runningNode, cloneNode, 3, null, "", nodeName);
        addNodeList[nodeTemp.NodeID] = nodeTemp;

        var firstNodeID = nodeTemp.NodeID;

        $.each(addNodeUserList, function (i, item) {
            var nodeItemTemp = wanda_wf_tool.initNodeByCloneNode(runningNode, cloneNode, 3, item, "", nodeName);
            nodeItemTemp.ParentNodeID = nodeTemp.NodeID;
            nodeItemTemp.PrevNodeID = "";
            nodeItemTemp.NextNodeID = "";
            addNodeList[nodeItemTemp.NodeID] = nodeItemTemp;
            addNodeArray.push(nodeItemTemp);
        })
        return {
            FirstNodeID: firstNodeID,
            LastNodeID: firstNodeID,
            AddNodeList: addNodeList,
            AddNodeArray: addNodeArray
        }
    }
}

/*
可以调用的公开方法有：createProcess，getProcess
initSetting只允许调用一次。
*/

var wanda_wf_client = {
    _setting: {
        AppCode: "",
        BusinessID: "",
        FlowCode: "",
        ProcessTitle: "",
        ProcessURL: "",
        ProcessMobileURL: "",
        DynamicRoleUserList: {},
        FormParams: {},
        CurrentUser: null,
        CheckUserInProcess: true
    },
    _otherSetting: {
        ContainerDomID: "",
        ExecuteType: 1,//1：JS请求处理，2：POST处理
        IsAsync: false,//是否为异步请求。true表示异步请求，false表示同步请求
        IsShowContextMenu: false,//是否显示右键菜单
        PageContextMenu: true,//是否为页面级右键菜单
        IsSaveDraftToDoWork: true,
        EnableDebug: false,
        ShowNodeName: true,
        IsView: false,//是否仅查看
        CustomerProcessLog: null,//自定义审批日志
        CustomerSceneSetting: {
            ShowCc: true,//是否显示抄送
            ShowFowardButton: true,//是否显示转发按钮
            AlwaysReturnToStart: false//不再弹出选择退回节点，始终退回发起节点
        },
        ButtonCssType: "default",//default:审批按钮居于底部,middle:"审批按钮居于审批日志和审批意见框中间"
        OnBeforeExecute: function (args, func) { if (func != undefined) { func(); } else { return true; } },
        OnSaveApplicationData: function (args, func) { if (func != undefined) { func(); } else { return true; } },
        OnAfterExecute: function (args) { },
        OnExecuteCheck: function (operatorType) { return true; }
    },
    _constSetting: {
        WFURL: "",
        hasInit: false,
        resourceHostPath: function () {
            if (wanda_wf_client._constSetting.WFURL != "") {
                return wanda_wf_client._constSetting.WFURL + "/RuntimeService/img/";
            }
            else {
                return wanda_wf_tool.getWorkFlowServerURL() + "/RuntimeService/img/";
            }
        },
        okImgSrc: function () { return wanda_wf_client._constSetting.resourceHostPath() + "ok.png" },
        upImgSrc: function () { return wanda_wf_client._constSetting.resourceHostPath() + "up.png" },
        downImgSrc: function () { return wanda_wf_client._constSetting.resourceHostPath() + "down.png" },
        arrowImgSrc: function () { return wanda_wf_client._constSetting.resourceHostPath() + "arrow.png" },
        deleteImgSrc: function () { return wanda_wf_client._constSetting.resourceHostPath() + "delete.png" }
    },
    _setWFURL: function (url) {
        wanda_wf_client._constSetting.WFURL = url;
        wanda_wf_data._setWFURL(url);
    },
    initPostSetting: function (domID, otherSetting) {
        if (otherSetting != null && otherSetting != undefined) {
            if (typeof otherSetting.OnExecuteCheck != "function") {
                otherSetting.OnExecuteCheck = eval(otherSetting.OnExecuteCheck);
            }
        }
        this._initSetting(domID, 2, undefined, otherSetting);
    },
    initAjaxSetting: function (domID, isAsync, otherSetting) {
        this._initSetting(domID, 1, isAsync, otherSetting);
    },
    _initSetting: function (domID, executeType, isAsync, otherSetting) {
        //wanda_wf_tool.getWorkFlowServerURL();
        if (!this._constSetting.hasInit) {
            if (domID == "" || domID == undefined) {
                wanda_wf_tool.alert("未配置流程显示的Dom元素");
                return;
            }
            if (domID.substr(0, 1) != "#") {
                domID = "#" + domID;
            }
            this._otherSetting.ExecuteType = executeType;
            this._otherSetting.ContainerDomID = domID;
            if (isAsync != undefined && isAsync != null) {
                this._otherSetting.IsAsync = isAsync;
            }
            if (otherSetting != null && otherSetting != undefined) {
                //初始化所需数据FlowCode，BusinessID,
                $.extend(true, this._otherSetting, otherSetting);
            }
            if (this._otherSetting.IsSaveDraftToDoWork) {
                wanda_wf_data.BizContext.ExtensionCommond.DrafterAddTodoTask = "True";
            }
            else {
                wanda_wf_data.BizContext.ExtensionCommond.DrafterAddTodoTask = "False";
            }
            this._constSetting.hasInit = true;
        }
        else {
            wanda_wf_tool.alert("已经初始化设置，请勿重复初始化！");
            throw ("已经初始化设置，请勿重复初始化！");
        }
    },
    getProcess: function (option, func, isRefresh) {
        this._initOption(option);
        if (isRefresh == null || isRefresh == undefined) {
            isRefresh = false;
        }
        //如果需要刷新，且当前节点为发起节点，当前用户为发起人，则允许刷新
        if (isRefresh
            //&& wanda_wf_data.WorkFlowContext.ProcessInstance.StartNodeID == wanda_wf_data.WorkFlowContext.ProcessInstance.RunningNodeID
            //&& wanda_wf_data.WorkFlowContext.CurrentUser.UserCode == wanda_wf_data.WorkFlowContext.ProcessInstance.CreateUser.UserCode
            ) {
            wanda_wf_data.BizContext.GetProcessForceRefresh = "True";
        }
        //AJAX请求Handler获取数据
        wanda_wf_data.BizContext.WFToken = GeneralToken();
        wanda_wf_tool.ajax(function (data) {
            wanda_wf_data.BizContext.GetProcessForceRefresh = "False";
            wanda_wf_client._processAjaxReturn(data);
            if (func != undefined && func != null) {
                func(data);
            }
        }, wanda_wf_data._buildAjaxData("GetProcess")
       );
    },
    createProcess: function (option, func) {
        this._initOption(option);
        this._createProcess(func);
    },
    refreshProcess: function (option) {
        this._initOption(option);
        wanda_wf_data.BizContext.WFToken = GeneralToken();
        wanda_wf_tool.ajax(function (data) {
            wanda_wf_client._processAjaxReturn(data);
        }, wanda_wf_data._buildAjaxData("RefreshProcess")
       );
    },
    exist: function (businessID, trueFunc, falseFunc) {
        try {
            var result;
            wanda_wf_tool.ajaxCommon(function (data) {
                result = data.toString().toLowerCase() == "true";
                if (trueFunc != undefined && falseFunc != undefined) {
                    if (result) {
                        trueFunc();
                    }
                    else {
                        falseFunc();
                    }
                }
            }, "ProcessOperator", "ExistProcess", { BusinessID: businessID });
            return result;
        }
        catch (e) {
            return undefined;
        }
    },
    showProcess: function (workFlowContext) {
        if (workFlowContext != undefined && workFlowContext != null) {
            wanda_wf_client._processAjaxReturn(workFlowContext);
        }
        else {
            //此方法仅提供给POST方式调用，用于在GetProcess，CreateProcess，及POST后注册到前端显示
            if (wanda_wf_client._otherSetting.ExecuteType != 2) {
                wanda_wf_tool.alert("调用失败，该方法为POST接入方式调用！");
                return;
            }
            //如果是POST形式，则直接使用SDK注册的JSON对象MCS_WF_WORKFLOWCONTEXT_JSON显示数据
            wanda_wf_client._processAjaxReturn(MCS_WF_WORKFLOWCONTEXT_JSON);
        }
    },
    _initOption: function (option) {
        this._setting.BusinessID = "";
        this._setting.DynamicRoleUserList = {};
        this._setting.FormParams = {};
        this._setting.CurrentUser = null;
        this._setting.CheckUserInProcess = true;

        if (option != undefined && option != null) {
            if (typeof option == "object") {
                //初始化所需数据FlowCode，BusinessID,
                $.extend(true, this._setting, option);
            }
            else {
                this._setting.BusinessID = option;
            }
        }
        this._repaireSetting();
    },
    _createProcess: function (func) {
        var isCanCreate = this._checkCanCreateProcess();
        if (isCanCreate) {
            wanda_wf_data.BizContext.WFToken = GeneralToken();
            wanda_wf_tool.ajax(function (data) {
                wanda_wf_client._processAjaxReturn(data);
                if (func != undefined && func != null) {
                    func(data);
                }
            }, wanda_wf_data._buildAjaxData("CreateProcess")
            );
        }
    },
    _checkCanCreateProcess: function () {
        var msg = "";
        if ($.trim(this._setting.FlowCode) == "") {
            msg = msg + "FlowCode不能为空\r";
        }
        //暂确定校验，如果没有赋值，使用流程定义的名称和url
        //if ($.trim(this._setting.ProcessTitle) == "") {
        //    msg = msg + "ProcessTitle不能为空\r";
        //}
        //if ($.trim(this._setting.ProcessURL) == "") {
        //    msg = msg + "ProcessURL不能为空\r";
        //}
        //if ($.trim(this._setting.ProcessMobileURL) == "") {
        //    msg = msg + "ProcessMobileURL不能为空\r";
        //}
        if (msg != "") {
            wanda_wf_tool.tips(msg);
        }
        else {
            return true;
        }
    },
    _processAjaxReturn: function (workflowContext) {
        wanda_wf_tool.log(JSON.stringify(workflowContext), wanda_wf_client._otherSetting.EnableDebug);
        var code = workflowContext.StatusCode;
        //处理null数据，改为[]，可以减少后边的判断
        if (workflowContext.CcNodeInstanceList == null) {
            workflowContext.CcNodeInstanceList = {};
        }
        if (workflowContext.ProcessLogList == null) {
            workflowContext.ProcessLogList = [];
        }
        if (workflowContext.ExtensionInfos == null) {
            workflowContext.ExtensionInfos = {};
        }
        if (workflowContext.CurrentUserSceneSetting != null && workflowContext.CurrentUserSceneSetting.ActionButtonList == null) {
            workflowContext.CurrentUserSceneSetting.ActionButtonList = [];
        }

        wanda_wf_data._clearContext();
        wanda_wf_data.BizContext.CurrentUser = workflowContext.CurrentUser;
        if (code >= 0 && code < 200) {
            if (code == 11 || code == 21 || code == 0) {
                if (workflowContext.NodeInstanceList != null) {
                    wanda_wf_data._initBizContextByWorkFlowContext(workflowContext);
                    wanda_wf_data._initOtherContext(workflowContext);
                    wanda_wf_data.WorkFlowContext = workflowContext;
                    //try {
                    wanda_wf_client._initProcessControl();
                    //}
                    //catch (a) {
                    //    alert(a);
                    //}
                }
                if (code == 11 || code == 21) {
                    //wanda_wf_tool.log(workflowContext,wanda_wf_client._otherSetting.EnableDebug);
                    wanda_wf_tool.alert(wanda_wf_tool.formateError(workflowContext, "info"));
                }
            }
            else {
                //wanda_wf_tool.log(workflowContext, wanda_wf_client._otherSetting.EnableDebug);
                wanda_wf_tool.alert(wanda_wf_tool.formateError(workflowContext, "info"));
            }
        }
        else if (code >= 200) {
            //wanda_wf_tool.log(workflowContext, wanda_wf_client._otherSetting.EnableDebug);
            wanda_wf_tool.alert(wanda_wf_tool.formateError(workflowContext));
        }

    },
    _repaireSetting: function () {
        wanda_wf_data.BizContext.CurrentUser = null;
        $.extend(true, wanda_wf_data.BizContext, this._setting);
        if (wanda_wf_data.WorkFlowContext != null && wanda_wf_data.WorkFlowContext.ProcessInstance != null) {
            var processInstance = wanda_wf_data.WorkFlowContext.ProcessInstance;
            if (wanda_wf_data.BizContext.FlowCode == "") {
                wanda_wf_data.BizContext.FlowCode = processInstance.FlowCode;
            }
            if (wanda_wf_data.BizContext.ProcessTitle == "") {
                wanda_wf_data.BizContext.ProcessTitle = processInstance.ProcessTitle;
            }
            if (wanda_wf_data.BizContext.ProcessURL == "") {
                wanda_wf_data.BizContext.ProcessURL = processInstance.ProcessURL;
            }
            if (wanda_wf_data.BizContext.ProcessMobileURL == "") {
                wanda_wf_data.BizContext.ProcessMobileURL = processInstance.ProcessMobileURL;
            }
        }
    },
    _initProcessControl: function () {
        var div = $(this._otherSetting.ContainerDomID);
        //加载整体Dom内容
        div.html(this.template.content());

        //加载局部内容
        var scene = wanda_wf_data.WorkFlowContext.CurrentUserSceneSetting;
        if (scene == null) return;
        if (this._otherSetting.IsView) {
            scene.ShowApprovalTextArea = false;
            scene.AllowNewCC = false;
            scene.ActionButtonList = [];
        }
        //是否显示抄送
        if (!this._otherSetting.CustomerSceneSetting.ShowCc) {
            scene.ShowCCBar = false;
        }
        //是否显示加签按钮
        if (!this._otherSetting.CustomerSceneSetting.ShowFowardButton) {
            wanda_wf_data.WorkFlowContext.CurrentUserSceneSetting.ActionButtonList = Enumerable.From(wanda_wf_data.WorkFlowContext.CurrentUserSceneSetting.ActionButtonList).Where("$.ButtonType!=2").ToArray();
        }

        if (scene.ShowNavigationBar) {
            //加载导航
            this._initItemControl.initNav("#wanda-wfclient-nav-content");
        }
        else {
            $("#wanda-wfclient-nav-content").closest("tr").hide();
        }
        if (scene.ShowCCBar) {
            //加载抄送
            this._initItemControl.initCc("#wanda-wfclient-cc-content");
        }
        else {
            $("#wanda-wfclient-cc-content").closest("tr").hide();
        }

        if (scene.ShowApprovalTextArea) {
            //绑定事件,快捷意见
            $(".wanda-wfclient-log-quicklog").click(function () {
                var currentLogDom = $("#wanda-wfclient-log-textarea");
                var currentLogValue = currentLogDom.val();
                if ($.trim(currentLogValue) != "") {
                    currentLogValue += "\n";
                }
                currentLogValue += $(this).html();
                currentLogDom.val(currentLogValue)
            })
        }
        else {
            $(".wanda-wfclient-log-quicklog").closest("tr").hide();
            $("#wanda-wfclient-log-textarea").closest("tr").hide();
        }
        if (scene.ShowApprovalLog) {  //加载审批日志
            this._initItemControl.initProcessLog("#wanda-wfclient-log-content");
        }
        else {
            $("#wanda-wfclient-log-content").closest("tr").hide();
        }

        switch (wanda_wf_client._otherSetting.ButtonCssType) {
            case "middle":
                $("#wanda-wfclient-button-content").addClass("wanda-wfclient-button-middle");
                $("#wanda-wfclient-log-content").css("margin-bottom", 0);
                break;
            default:
                $("#wanda-wfclient-button-content").addClass("wanda-wfclient-button-default");
                break;
        }

        //加载按钮
        this._initItemControl.initButton("#wanda-wfclient-button-content");
    },
    _initItemControl: {
        initProcessLog: function (logDomID) {
            //加载审批日志
            $(logDomID).html(wanda_wf_client.template.opinion());
            var table = $("#wanda-wfclient-log-logtable");
            var logDataList = [];
            if (wanda_wf_client._otherSetting.CustomerProcessLog != null) {
                logDataList = wanda_wf_client._otherSetting.CustomerProcessLog;
            }
            else {
                //倒序排列审批日志
                wanda_wf_data.WorkFlowContext.ProcessLogList = Enumerable.From(wanda_wf_data.WorkFlowContext.ProcessLogList).OrderByDescending("$.FinishDateTime").ToArray();
                logDataList = wanda_wf_data.WorkFlowContext.ProcessLogList;
            }
            $.each(logDataList, function (i, item) {
                if (item.OpertationName != "保存") {
                    var tr = $("<tr>");
                    tr.append($("<td>").append($("<div>").text(item.NodeName)));
                    tr.append($("<td>", { style: "text-align:left" }).append($("<span>").html("<pre>" + item.LogContent + "</pre>")));
                    var userName = item.User.UserOrgPathName + "<br/>" + item.User.UserJobName + "<br>" + item.User.UserName;
                    tr.append($("<td>", { style: "text-align:left" }).append($("<span>", { style: "line-height:20px;" }).html(userName).attr("title", item.User.UserName + "(" + item.User.UserLoginID + ")")));
                    tr.append($("<td>").append($("<div>").html(wanda_wf_tool.formatDate(item.FinishDateTime))));
                    tr.append($("<td>").append($("<div>").text(item.OpertationName)));
                    if (i > 4) {
                        tr.hide();
                    }
                    table.append(tr);
                }
                else {
                    //草稿状态下，将审批日志中的内容写入到审批意见框中
                    if (wanda_wf_data.WorkFlowContext.ProcessInstance.Status == 0) {
                        $("#wanda-wfclient-log-textarea").val(item.LogContent);
                    }
                }
            });
            if (logDataList.length > 5) {
                //隐藏/显示审批日志
                var showmoretr = $("#wanda-wfclient-log-showmore");
                showmoretr.show();
                showmoretr.find("span").click(function () {
                    var obj = $(this);
                    var img = obj.find("img");
                    var a = obj.find("a");
                    if (a.text() == "显示更多") {
                        a.html("收起");
                        img.attr("src", wanda_wf_client._constSetting.upImgSrc());
                        table.find("tr").show();
                    }
                    else {
                        a.html("显示更多");
                        img.attr("src", wanda_wf_client._constSetting.downImgSrc());
                        table.find("tr:gt(5)").hide();
                    }
                });
            }
        },
        initNav: function (navDomID) {
            var navDiv = $("<div>", { id: "wanda-wfclient-nav-div" });
            $(navDomID).append(navDiv);

            var nodeDataArray = wanda_wf_data.OtherContext.NodeInstanceArray;
            var startNode = wanda_wf_data.WorkFlowContext.NodeInstanceList[wanda_wf_data.WorkFlowContext.ProcessInstance.StartNodeID];
            if (startNode == null) {
                startNode = wanda_wf_data.OtherContext.StartNode;
            }
            var isCanSelectUser = wanda_wf_data.WorkFlowContext.ProcessInstance.RunningNodeID == wanda_wf_data.WorkFlowContext.ProcessInstance.StartNodeID && wanda_wf_data.WorkFlowContext.CurrentUserNodeID == wanda_wf_data.WorkFlowContext.ProcessInstance.RunningNodeID;
            var node = startNode;
            while (node != null) {
                if (node.NodeType != 5 && node.NodeType != 6) {
                    var spDom = wanda_wf_client._initItemControl.initNavItem(node, isCanSelectUser, nodeDataArray);
                    navDiv.append(spDom).append(wanda_wf_client._domBuild.buildNavArrowSpDom());
                }
                var nextNodeID = node.NextNodeID;
                if (nextNodeID != "") {
                    node = wanda_wf_data.WorkFlowContext.NodeInstanceList[nextNodeID];
                }
                else {
                    node = null;
                }
            }
            navDiv.find(".wanda-wfclient-nav-sparrrow:last").hide();
        },
        initNavItem: function (node, isCanSelectUser, nodeDataArray, isAddNode) {
            var spDom = $("<span>", { "class": "wanda-wfclient-nav-item " + (node.NodeType == 3 ? "wanda-wfclient-nav-node-autoinform" : "") });
            var spNode = $("<span>").text(node.NodeName).attr("title", wanda_wf_tool.formateNodeTips(node));
            if (!wanda_wf_client._otherSetting.ShowNodeName && node.NodeType != 0) {
                spNode.css("display", "none");
            }
            spDom.append(spNode);
            if (node.Status == 1 && wanda_wf_data.WorkFlowContext.ProcessInstance.Status != -1) {
                //正在执行中
                spDom.css("font-weight", 600);
            }
            if (node.NodeType == 2 || node.NodeType == 3) {
                var itemNodeList = Enumerable.From(nodeDataArray).Where("$.ParentNodeID == '" + node.NodeID + "'").OrderBy("$.NodeOrder").ToArray();
                var len = itemNodeList.length;
                if (len == 1) {
                    spDom.append($("<span>").text("【" + itemNodeList[0].User.UserName + "】").attr("title", wanda_wf_tool.formateUserTips(itemNodeList[0].User)));
                }
                else if (len > 1) {
                    spDom.append($("<span>").text("【"));
                    var isAllProcess = node.Status == 2;
                    $.each(itemNodeList, function (i, item) {
                        spDom.append($("<span>", {}).text(item.User.UserName).attr("title", wanda_wf_tool.formateUserTips(item.User)));
                        if (item.Status == 2 && !isAllProcess) {
                            spDom.append(wanda_wf_client._domBuild.buildNavOkImgDom());
                        }
                        if (len != (i + 1)) {
                            spDom.append($("<span>").text("，"));
                        }
                    });
                    spDom.append($("<span>").text("】"));
                }
            }
            else if (node.NodeType == 0) {
                if (wanda_wf_data.WorkFlowContext.ProcessInstance.Status != 0) {
                    if (node.User != null) {
                        spDom.append($("<span>").text("【" + node.User.UserName + "】").attr("title", wanda_wf_tool.formateUserTips(node.User)));
                    }
                }
            }
            else {
                if (isCanSelectUser) {
                    var allowChooseAnyUser = node.ExtendProperties.AllowChooseAnyUser == "True";
                    var nomineeUser = [];
                    if (node.NomineeList != null && node.NomineeList.length > 0) {
                        nomineeUser = node.NomineeList;
                    }
                    else {
                        //如果只存在一个候选用户时，则取node.User。
                        if (node.User != null) {
                            nomineeUser.push(node.User);
                        }
                    }
                    if (nomineeUser.length == 0 && !allowChooseAnyUser) {
                        spDom.css("color", "red");
                    }
                    var nomineeUserLen = nomineeUser.length;
                    if (nomineeUserLen == 1 && !allowChooseAnyUser) {
                        spDom.append($("<span>").text("【" + nomineeUser[0].UserName + "】").attr("title", wanda_wf_tool.formateUserTips(nomineeUser[0])));
                        wanda_wf_data.BizContext.NodeInstanceList[node.NodeID].User = nomineeUser[0];
                    }
                    else {
                        var selectPerson = $("<select>", { "class": "wanda-wfclient-nav-select", id: node.NodeID });
                        selectPerson.append('<option value="">请选择</option>');
                        $.each(nomineeUser, function (i, info) {
                            if (i > 9) {
                                return false;
                            }
                            selectPerson.append('<option value="' + info.UserCode + '" title="' + wanda_wf_tool.formateUserTips(info) + '">' + info.UserName + '</option>');
                        });
                        if (node.User != null) {
                            selectPerson.val(node.User.UserCode);
                        }

                        if (allowChooseAnyUser || nomineeUserLen > 10) {
                            selectPerson.append('<option value="...">...</option>');
                        }

                        spDom.append(selectPerson);

                        selectPerson.change(function () {
                            var nodeID = $(this).attr("ID");
                            var userCode = $(this).val();
                            var nodeSelect = wanda_wf_data.BizContext.NodeInstanceList[node.NodeID];

                            if (userCode == "") {
                                nodeSelect.User = null;
                                $(this).attr("title", "");
                            }
                            else if (userCode == "...") {
                                var allowChooseAnyUserSelect = node.ExtendProperties["AllowChooseAnyUser"] == "True";
                                var checkedUser = nodeSelect.User == null ? [] : [nodeSelect.User];
                                wanda_wf_tool.bindUserSelect(selectPerson, function (data) {
                                    selectPerson.unbind("click");
                                    if (data.length == 1) {
                                        var user = data[0];
                                        if (selectPerson.find("option[value='" + user.UserCode + "']").length > 0) {
                                            selectPerson.val(user.UserCode);
                                        }
                                        else {
                                            selectPerson.find("option:last").before('<option value="' + user.UserCode + '" title="' + wanda_wf_tool.formateUserTips(user) + '">' + user.UserName + '</option>');
                                            selectPerson.val(user.UserCode);
                                            if (nodeSelect.NomineeList == null) {
                                                nodeSelect.NomineeList = [];
                                            }
                                            nodeSelect.NomineeList.push(user);
                                        }
                                        nodeSelect.User = user;
                                    }
                                    else {
                                        selectPerson.val("");
                                    }
                                }, wanda_wf_data.BizContext.AppCode, false, allowChooseAnyUserSelect, nodeSelect.NomineeList, checkedUser);
                            }
                            else {
                                var nodeNomineeList = wanda_wf_data.BizContext.NodeInstanceList[nodeID].NomineeList;
                                var userInfo = Enumerable.From(nodeNomineeList).First("$.UserCode == '" + userCode + "'");
                                $(this).attr("title", wanda_wf_tool.formateUserTips(userInfo));
                                wanda_wf_data.BizContext.NodeInstanceList[node.NodeID].User = userInfo;
                            }
                        })
                    }
                }
                else {
                    spDom.append($("<span>").text("【" + node.User.UserName + "】").attr("title", wanda_wf_tool.formateUserTips(node.User)));
                }
            }
            if (node.Status == 2) {
                spDom.append(wanda_wf_client._domBuild.buildNavOkImgDom());
            }
            var nodeID = node.NodeID;
            wanda_wf_data.OtherContext.NavNodeDom[nodeID] = spDom;
            return spDom;
        },
        initCc: function (ccDomID) {
            var ccControl = $(ccDomID).html("");
            var ccDataList = wanda_wf_data.WorkFlowContext.CcNodeInstanceList;
            var runningNode = wanda_wf_data.WorkFlowContext.NodeInstanceList[wanda_wf_data.WorkFlowContext.ProcessInstance.RunningNodeID]
            var isShowDel = wanda_wf_data.WorkFlowContext.CurrentUserSceneSetting.AllowNewCC;
            //if (runningNode != null) {
            //    isShowDel = runningNode.NodeType == 0 && wanda_wf_data.WorkFlowContext.CurrentUserNodeID == runningNode.NodeID;
            //}

            $.each(ccDataList, function (i, item) {
                var span = $("<span>", { "class": "wanda-wfclient-cc-item" });
                var spDel = $("<span>", { "class": "wanda-wfclient-cc-deleteuser" }).append(wanda_wf_client._domBuild.buildDeleteImgDom());
                span.append($("<span>").text(item.User.UserName).attr("title", wanda_wf_tool.formateUserTips(item.User)));
                if (isShowDel) {
                    span.append(spDel);
                    spDel.click(function () { $(this).parent().remove(); delete wanda_wf_data.WorkFlowContext.CcNodeInstanceList[i] });
                }
                if (item.Status == 1) {
                    if (wanda_wf_data.WorkFlowContext.ProcessInstance.Status != -1) {
                        span.css("font-weight", 600);
                    }
                }
                else if (item.Status == 2) {
                    var ok = $("<img>", { alt: "", src: wanda_wf_client._constSetting.okImgSrc() });
                    span.append(ok);
                }
                span.append($("<span>").text("；"));
                ccControl.append(span);
            });
            var allowNewCc = wanda_wf_data.WorkFlowContext.CurrentUserSceneSetting.AllowNewCC;
            if (allowNewCc) {
                var aselect = $("<a>", { "class": "wanda-wfclient-cc-selectuser", href: "javascript:void(0)" }).text("请选择");
                wanda_wf_tool.bindUserSelect(aselect, function (data) {
                    $.each(data, function (i, item) {
                        wanda_wf_data.ClientContext.CcUserList.push(item);
                        var span = $("<span>", { "class": "wanda-wfclient-cc-item" });
                        var spDel = $("<span>", { "class": "wanda-wfclient-cc-deleteuser" }).append(wanda_wf_client._domBuild.buildDeleteImgDom());
                        span.append($("<span>").text(item.UserName).attr("title", wanda_wf_tool.formateUserTips(item)));
                        if (isShowDel) {
                            span.append(spDel);
                            spDel.click(function () { $(this).parent().remove(); wanda_wf_tool.deleteArrayItem(wanda_wf_data.ClientContext.CcUserList, item); });
                        }
                        span.append($("<span>").text("；"));
                        aselect.before(span);
                    })
                }, null, null, null, null, null, wanda_wf_data.ClientContext.CcUserList);
                ccControl.append(aselect);
            }
        },
        initButton: function (btnDomID) {
            $(btnDomID).html(wanda_wf_client.template.button());
            var div = $("#wanda-wfclient-button-div");
            var buttonDataList = wanda_wf_data.WorkFlowContext.CurrentUserSceneSetting.ActionButtonList;

            var divContextMenu = $("<div>", { "class": "contextMenu", id: "divContextMenu", "style": "display:none;" });
            var ul = $("<ul>");
            divContextMenu.append(ul);
            var divContainer = $(wanda_wf_client._otherSetting.ContainerDomID);
            divContainer.append(divContextMenu);

            $.each(buttonDataList, function (i, item) {
                var liItem = $("<li>").text(item.ButtonDisplayName);
                liItem.click(function () { wanda_wf_client._initItemControl.initButtonEvent($(this), item); });
                ul.append(liItem);

                var btnItem = $("<div>", { "class": "wanda-wfclient-button-btn", "optype": item.ButtonType });
                if (item.ButtonType == 1) {
                    btnItem.addClass("wanda-wfclient-button-submit");
                }
                else {
                    btnItem.addClass("wanda-wfclient-button-save");
                }
                btnItem.click(function () { wanda_wf_client._initItemControl.initButtonEvent($(this), item); });
                btnItem.text(item.ButtonDisplayName);
                div.append(btnItem);
            });

            if (buttonDataList.length == 0) {
                $(btnDomID).hide();
            }
            else {
                if (wanda_wf_client._otherSetting.IsShowContextMenu) {
                    if (wanda_wf_client._otherSetting.PageContextMenu) {
                        $("body").contextMenu('divContextMenu', {
                        });
                    }
                    else {
                        divContainer.contextMenu('divContextMenu', {
                        });
                    }
                }
            }
        },
        initButtonEvent: function (btnItem, actionButtonItem) {
            var methodName = actionButtonItem.ButtonMethodName;
            var isSubmit = true;
            /*
                actionButtonItem.ButtonType含义
                0：保存，1：提交，7：撤回，9：作废
                2：转发，5：加签，6：退回
            */
            switch (actionButtonItem.ButtonType) {
                case 0:
                case 1:
                case 7:
                    break;
                case 2://转发
                    isSubmit = false;
                    wanda_wf_client._initItemControl.executeForward(actionButtonItem)
                    break;
                case 5://加签
                    isSubmit = false;
                    wanda_wf_client._initItemControl.executeAddNode(actionButtonItem)
                    break;
                case 6://退回
                    if (wanda_wf_client._otherSetting.CustomerSceneSetting.AlwaysReturnToStart) {
                        wanda_wf_data.BizContext.ExtensionCommond["RejectNode"] = "00000000-0000-0000-0000-000000000000";
                    }
                    else {
                        isSubmit = false;
                        wanda_wf_client._initItemControl.executeReturn(actionButtonItem)
                    }
                    break;
                case 9://作废
                    if (!confirm("确定“作废”当前流程吗？")) {
                        isSubmit = false;
                    }
                    break;
                default:
                    isSubmit = false;
                    wanda_wf_tool.alert("不支持的按钮类型");
                    break;
            }
            if (isSubmit) {
                var isPass = wanda_wf_client._execute.checkSubmit(actionButtonItem);
                if (!isPass) {
                    return false;
                }
                wanda_wf_client._execute.execute(actionButtonItem);
            }
        },
        executeAddNode: function (actionButtonItem) {
            var runningNode = wanda_wf_data._getRunningNode();
            if (runningNode.NodeType != 0 && runningNode.NodeType != 1) {
                wanda_wf_tool.alert("只有发起节点和审批节点允许加签");
                return;
            }
            var divContainerAddNode = $(wanda_wf_client.template.controlTemplate.AddNode());
            divContainerAddNode.Wanda_OpenDiv({
                title: "加签",
                isShowExit: true,
                isShowButton: true,
                onInit: function (context) {
                    var select = context.find("#wanda-wfclient-addnode-currentnode");
                    var nodeNameDom = context.find("#wanda-wfclient-addnode-nodename");
                    nodeNameDom.val(runningNode.User.UserName + "加签");
                    if (runningNode.NodeType == 0) {
                        context.find("#wanda-wfclient-addnode-type_before").hide();//发起节点只有后加签
                        context.find("#wanda-wfclient-addnode-type_before").next().hide();
                        var nextNodeList = wanda_wf_data._getAddNodeList(runningNode);
                        //从发起节点开始顺序添加可以加签的节点
                        var len = nextNodeList.length;
                        for (var i = 0 ; i < len; i++) {
                            var nodeTemp = nextNodeList[i];
                            var nodeText = wanda_wf_data._getNodeNameAndUserFormateText(nodeTemp);
                            select.append('<option value="' + nodeTemp.NodeID + '" title="' + nodeText + '" nodetype="' + nodeTemp.NodeType + '">' + nodeText + '</option>');
                        }
                    }
                    else {
                        var runningNameText = wanda_wf_data._getNodeNameAndUserFormateText(runningNode);
                        select.append('<option value="' + runningNode.NodeID + '" title="' + runningNameText + '" nodetype="' + runningNode.NodeType + '">' + runningNameText + '</option>');
                    }
                    select.change(function () {
                        var nodeType = $(this).find("option:selected").attr("nodetype");
                        if (nodeType == 3) {
                            context.find("#wanda-wfclient-addnode-audittype_autoinform").click();
                        }
                        else {
                            context.find("#wanda-wfclient-addnode-audittype-order").click();
                        }
                    })
                    select.change();
                    context.find("#wanda-wfclient-addnode-logtextarea").val($("#wanda-wfclient-log-textarea").val());

                    var aselect = context.find("#wanda-wfclient-addnode-aselect");
                    wanda_wf_tool.bindUserSelect(aselect, function (data) {
                        $.each(data, function (i, item) {
                            if (item.UserCode != wanda_wf_data.WorkFlowContext.CurrentUser.UserCode) {
                                var span = $("<span>", { "class": "wanda-wfclient-addnode-item" });
                                var spDel = $("<span>", { "class": "wanda-wfclient-deleteuser" }).append(wanda_wf_client._domBuild.buildDeleteImgDom());
                                span.append($("<span>").text(item.UserName).attr("title", wanda_wf_tool.formateUserTips(item))).append(spDel).append($("<span>").text(";"));
                                wanda_wf_data.ClientContext.AddNodeUserList.push(item);
                                spDel.click(function () {
                                    $(this).parent().remove();
                                    wanda_wf_tool.deleteArrayItem(wanda_wf_data.ClientContext.AddNodeUserList, item);
                                });
                                $(aselect).before(span);
                            }
                        })
                    }, null, null, null, null, null, wanda_wf_data.ClientContext.AddNodeUserList);

                    context.find("input[type='radio'][name='wanda-wfclient-addnode-type']").change(function () {
                        var value = $(this).val();
                        if (value == 0) {
                            context.find("#wanda-wfclient-addnode-content").height(190);
                            context.find("#wanda-wfclient-addnode-trlog").show();
                            if (context.find("input[type='radio'][name='wanda-wfclient-addnode-audittype']:checked").val() == 3) {
                                context.find("#wanda-wfclient-addnode-audittype_order").click();
                            }
                            context.find("#wanda-wfclient-addnode-audittype_autoinform").hide();
                            context.find("#wanda-wfclient-addnode-audittype_autoinform").next().hide();
                            $("#wanda-wfclient-addnode-logtextarea").focus();
                        }
                        else {
                            context.find("#wanda-wfclient-addnode-content").height(120);
                            context.find("#wanda-wfclient-addnode-trlog").hide();
                            context.find("#wanda-wfclient-addnode-audittype_autoinform").show();
                            context.find("#wanda-wfclient-addnode-audittype_autoinform").next().show();
                            $("#wanda-wfclient-addnode-nodename").focus();
                        }

                    })
                    context.find("input[type='radio'][name='wanda-wfclient-addnode-audittype']").change(function () {
                        var value = $(this).val();
                        if (value == 3) {
                            nodeNameDom.val(runningNode.User.UserName + "加通知");
                        }
                        else if (value == 2) {
                            nodeNameDom.val(runningNode.User.UserName + "加会签");
                        }
                        else {
                            nodeNameDom.val(runningNode.User.UserName + "加签");
                        }
                        $("#wanda-wfclient-addnode-nodename").focus();
                    })
                },
                onSubmit: function (context) {
                    var cloneNodeListObj = null;
                    var selectNodeID = $("#wanda-wfclient-addnode-currentnode").val();
                    var selectNode = wanda_wf_data.BizContext.NodeInstanceList[selectNodeID];
                    var cloneNode = wanda_wf_data._getCloneNode(wanda_wf_data.BizContext.NodeInstanceList[selectNodeID]);
                    if (cloneNode == null) {
                        wanda_wf_tool.alert("克隆节点为空，不允许加签！");
                        return;
                    }
                    if (wanda_wf_data.ClientContext.AddNodeUserList.length == 0) {
                        wanda_wf_tool.alert("请选择处理人");
                        return false;
                    }

                    var addNodeType = $("input[type='radio'][name='wanda-wfclient-addnode-type']:checked").val();//加签类型（1：后加签，2：前加签）
                    var approvalContent = $.trim(context.find("#wanda-wfclient-addnode-logtextarea").val());
                    if (addNodeType == 0) {
                        if (approvalContent == "") {
                            wanda_wf_tool.alert("请填写加签意见");
                            return false;
                        }
                    }
                    var nodeNameCustomer = context.find("#wanda-wfclient-addnode-nodename").val();
                    if (nodeNameCustomer == "") {
                        wanda_wf_tool.alert("请填写加签节点名称");
                        return false;
                    }
                    var addNodeAuditType = $("input[type='radio'][name='wanda-wfclient-addnode-audittype']:checked").val();//审批类型（1：顺序审批，2：同时审批）
                    var copyBizContextNodeInstanceList = $.extend(true, {}, wanda_wf_data.BizContext.NodeInstanceList);//拷贝一份节点数据，保证加签出错时，数据可以正常
                    if (wanda_wf_data.ClientContext.AddNodeUserList.length < 2 && addNodeAuditType == 2) {
                        wanda_wf_tool.alert("同时审批的处理人必须为两人及以上！");
                        return false;
                    }

                    if (addNodeAuditType == 1) {
                        cloneNodeListObj = wanda_wf_tool.initAddNodeOrderList(runningNode, cloneNode, nodeNameCustomer, wanda_wf_data.ClientContext.AddNodeUserList);
                    }
                    else if (addNodeAuditType == 2) {
                        cloneNodeListObj = wanda_wf_tool.initAddNodeCosignerList(runningNode, cloneNode, nodeNameCustomer, wanda_wf_data.ClientContext.AddNodeUserList);
                    }
                    else if (addNodeAuditType == 3) {
                        cloneNodeListObj = wanda_wf_tool.initAddNodeAutoInformList(runningNode, cloneNode, nodeNameCustomer, wanda_wf_data.ClientContext.AddNodeUserList);
                    }
                    var addNodeList = cloneNodeListObj.AddNodeList;
                    var addNodeArray = cloneNodeListObj.AddNodeArray;
                    var firstNode = addNodeList[cloneNodeListObj.FirstNodeID];
                    var lastNode = addNodeList[cloneNodeListObj.LastNodeID];

                    if (addNodeType == 0) {//前加签
                        var nodeName = wanda_wf_data._getNodeNameAndUserFormateText(firstNode, addNodeArray);
                        if (confirm("流程将提交至“" + nodeName + "”\r确定提交吗？")) {
                            firstNode.PrevNodeID = runningNode.PrevNodeID;
                            wanda_wf_data.BizContext.NodeInstanceList[runningNode.PrevNodeID].NextNodeID = firstNode.NodeID;
                            lastNode.NextNodeID = runningNode.NodeID;
                            runningNode.PrevNodeID = lastNode.NodeID;
                            wanda_wf_data.BizContext.NodeInstanceList[runningNode.NodeID] = $.extend(true, {}, runningNode);//防止出现加签节点前后关联关闭不对
                            $.each(addNodeList, function (i, item) {
                                wanda_wf_data.BizContext.NodeInstanceList[i] = item;
                            })

                            var isPass = wanda_wf_client._execute.checkSubmit(actionButtonItem, approvalContent);
                            if (!isPass) {
                                return false;
                            }
                            wanda_wf_client._execute.execute(actionButtonItem, function (success) {
                                if (success) {
                                    divContainerAddNode.CloseDiv();
                                }
                            });
                        }
                        else {
                            return false;
                        }
                    }
                    else {
                        var navAddNodeDom = [];
                        var node = firstNode;
                        while (node != null) {
                            var spDelNav = $("<span>", { "class": "wanda-wfclient-deletenav", "nodeid": node.NodeID }).append(wanda_wf_client._domBuild.buildDeleteImgDom());
                            var spDom = wanda_wf_client._initItemControl.initNavItem(node, false, addNodeArray);
                            spDom.append(spDelNav);

                            spDelNav.click(function () {
                                var nodeID = $(this).attr("nodeid");
                                var nodeDelete = wanda_wf_data.BizContext.NodeInstanceList[nodeID];
                                if (nodeDelete.NextNodeID == "") {
                                    wanda_wf_data.BizContext.NodeInstanceList[nodeDelete.PrevNodeID].NextNodeID = "";
                                    $(this).parent().prev().remove();
                                }
                                else {
                                    var deletePreNode = wanda_wf_data.BizContext.NodeInstanceList[nodeDelete.PrevNodeID];
                                    var deleteNextNode = wanda_wf_data.BizContext.NodeInstanceList[nodeDelete.NextNodeID];
                                    deletePreNode.NextNodeID = deleteNextNode.NodeID;
                                    deleteNextNode.PrevNodeID = deletePreNode.NodeID;
                                }
                                delete wanda_wf_data.BizContext.NodeInstanceList[nodeID];
                                delete wanda_wf_data.OtherContext.NavNodeDom[nodeID];
                                delete wanda_wf_data.ClientContext.CurrentAddNode[nodeID];
                                $(this).parent().next().remove();
                                $(this).parent().remove();
                            })

                            navAddNodeDom.push({ Dom: spDom, Arrow: wanda_wf_client._domBuild.buildNavArrowSpDom(), NodeID: node.NodeID });
                            wanda_wf_data.ClientContext.CurrentAddNode[node.NodeID] = node;
                            var nextNodeID = node.NextNodeID;
                            if (nextNodeID != "") {
                                node = addNodeList[nextNodeID];
                            }
                            else {
                                node = null;
                            }
                        }
                        var navCurrentDom = wanda_wf_data.OtherContext.NavNodeDom[selectNode.NodeID];
                        var nowDom = navCurrentDom;
                        $.each(navAddNodeDom, function (i, item) {
                            nowDom.after(item.Arrow);
                            item.Arrow.after(item.Dom);
                            wanda_wf_data.OtherContext.NavNodeDom[item.NodeID] = item.Dom;
                            nowDom = item.Dom;
                        })

                        var nextNode = wanda_wf_data.BizContext.NodeInstanceList[selectNode.NextNodeID];
                        if (selectNode.NodeType == 0) {
                            while (nextNode != null && (nextNode.NodeType == 5 || nextNode.NodeType == 6)) {
                                if (nextNode.NextNodeID != "") {
                                    nextNode = wanda_wf_data.BizContext.NodeInstanceList[nextNode.NextNodeID];
                                }
                                else {
                                    nextNode = null;
                                }
                            }
                        }
                        if (nextNode != null) {
                            var preNodeID = nextNode.PrevNodeID;
                            lastNode.NextNodeID = nextNode.NodeID;
                            nextNode.PrevNodeID = lastNode.NodeID;
                            var preNode = wanda_wf_data.BizContext.NodeInstanceList[preNodeID]
                            firstNode.PrevNodeID = preNode.NodeID;
                            preNode.NextNodeID = firstNode.NodeID;
                        }
                        else {
                            firstNode.PrevNodeID = selectNode.NodeID;
                            selectNode.NextNodeID = firstNode.NodeID;
                        }

                        $.each(addNodeList, function (i, item) {
                            wanda_wf_data.BizContext.NodeInstanceList[i] = item;
                        })
                        wanda_wf_data.BizContext.ExtensionCommond["AddAfterNode"] = "True";
                    }
                    wanda_wf_data.ClientContext.AddNodeUserList = [];
                    return true;
                },
                onCancel: function () {
                    wanda_wf_data.ClientContext.AddNodeUserList = [];
                    return true;
                }
            });
        },

        executeReturn: function (actionButtonItem) {
            var divContainerReturn = $(wanda_wf_client.template.controlTemplate.Return());
            divContainerReturn.Wanda_OpenDiv({
                title: "退回",
                isShowExit: true,
                isShowButton: true,
                onInit: function (context) {
                    var approvalContent = $("#wanda-wfclient-log-textarea").val();
                    context.find("#wanda-wfclient-return-logtextarea").val(approvalContent);

                    var select = context.find("#wanda-wfclient-return-select");
                    var currentNode = wanda_wf_data.WorkFlowContext.NodeInstanceList[wanda_wf_data.WorkFlowContext.ProcessInstance.RunningNodeID];
                    var beforeNodeList = [];
                    var beforeNode = wanda_wf_data.WorkFlowContext.NodeInstanceList[currentNode.PrevNodeID];
                    while (beforeNode != null) {
                        if (beforeNode.NodeType == 1 || beforeNode.NodeType == 2 || beforeNode.NodeType == 0 || beforeNode.NodeType == 7) {
                            beforeNodeList.push(beforeNode);
                        }
                        if (beforeNode.PrevNodeID != "") {
                            beforeNode = wanda_wf_data.WorkFlowContext.NodeInstanceList[beforeNode.PrevNodeID]
                        }
                        else {
                            beforeNode = null;
                        }
                    }
                    //从发起节点开始顺序添加可以退回的节点
                    var len = beforeNodeList.length;
                    for (var i = (len - 1) ; i >= 0; i--) {
                        var nodeTemp = beforeNodeList[i];
                        var nodeText = wanda_wf_data._getNodeNameAndUserFormateText(nodeTemp);
                        select.append('<option value="' + nodeTemp.NodeID + '" title="' + nodeText + '">' + nodeText + '</option>');
                    }
                },
                onSubmit: function (context) {
                    var rejectNodeID = context.find("#wanda-wfclient-return-select").val();
                    var rejectNode = wanda_wf_data.WorkFlowContext.NodeInstanceList[rejectNodeID];
                    var msg = "确定退回到“";
                    if (rejectNode.NodeType == 0 || rejectNode.NodeType == 1 || rejectNode.NodeType == 7 || rejectNode.NodeType == 2) {
                        msg = msg + context.find("#wanda-wfclient-return-select").find("option:selected").text();
                    }
                    else {
                        wanda_wf_tool.alert("退回的节点类型不正确");
                        return false;
                    }
                    msg = msg + "”节点吗？";
                    if (confirm(msg)) {
                        var approvalContent = context.find("#wanda-wfclient-return-logtextarea").val();
                        var isPass = wanda_wf_client._execute.checkSubmit(actionButtonItem, approvalContent);
                        if (!isPass) {
                            return false;
                        }
                        wanda_wf_data.BizContext.ExtensionCommond["RejectNode"] = rejectNodeID;
                        wanda_wf_client._execute.execute(actionButtonItem, function (success) {
                            if (success) {
                                divContainerReturn.CloseDiv();
                            }
                        });
                        return true;
                    }
                    else {
                        return false;
                    }
                }
            });
        },
        executeForward: function (actionButtonItem) {
            var divContainerForward = $(wanda_wf_client.template.controlTemplate.Forward());
            divContainerForward.Wanda_OpenDiv({
                title: "转发",
                isShowExit: true,
                isShowButton: true,
                onInit: function (context) {
                    var approvalContent = $("#wanda-wfclient-log-textarea").val();
                    context.find("#wanda-wfclient-forward-logtextarea").val(approvalContent);
                    var aselect = context.find("#wanda-wfclient-forward-aselect");
                    wanda_wf_tool.bindUserSelect(aselect, function (data) {
                        $.each(data, function (i, item) {
                            if (item.UserCode != wanda_wf_data.WorkFlowContext.CurrentUser.UserCode) {
                                var span = $("<span>", { "class": "wanda-wfclient-cc-item" });
                                var spDel = $("<span>", { "class": "wanda-wfclient-deleteuser" }).append(wanda_wf_client._domBuild.buildDeleteImgDom());
                                span.append($("<span>").text(item.UserName).attr("title", wanda_wf_tool.formateUserTips(item))).append(spDel).append($("<span>").text(";"));
                                wanda_wf_data.ClientContext.ForwardUserList.push(item);
                                spDel.click(function () {
                                    $(this).parent().remove();
                                    wanda_wf_tool.deleteArrayItem(wanda_wf_data.ClientContext.ForwardUserList, item);
                                });
                                $(aselect).before(span);
                            }
                        })
                        context.find("#wanda-wfclient-forward-logtextarea").focus();
                    }, null, null, null, null, null, wanda_wf_data.ClientContext.ForwardUserList);
                },
                onSubmit: function (context) {
                    if (wanda_wf_data.ClientContext.ForwardUserList.length == 0) {
                        wanda_wf_tool.alert("请选择接收人");
                        return false;
                    }
                    var approvalContent = context.find("#wanda-wfclient-forward-logtextarea").val();
                    var isPass = wanda_wf_client._execute.checkSubmit(actionButtonItem, approvalContent);
                    if (!isPass) {
                        return false;
                    }
                    wanda_wf_data.BizContext.ExtensionCommond["ForwardUser"] = JSON.stringify(wanda_wf_data.ClientContext.ForwardUserList);
                    wanda_wf_client._execute.execute(actionButtonItem, function (success) {
                        if (success) {
                            divContainerForward.CloseDiv();
                        }
                    });
                },
                onCancel: function (context) {
                    wanda_wf_data.ClientContext.ForwardUserList = [];
                    return true;
                }
            });
        }
    },
    _execute: {
        execute: function (actionButtonItem, func) {
            var result = true;
            try {
                result = wanda_wf_client._otherSetting.OnExecuteCheck(actionButtonItem.ButtonType);
            }
            catch (e) {
                wanda_wf_tool.alert(e);
                return;
            }
            if (result == true) {
                var methodName = actionButtonItem.ButtonMethodName;
                wanda_wf_data.BizContext.WFToken = GeneralToken();
                if (wanda_wf_client._otherSetting.ExecuteType == 1) {
                    wanda_wf_client._execute.executeAjax(methodName, actionButtonItem, func);
                }
                else if (wanda_wf_client._otherSetting.ExecuteType == 2) {
                    wanda_wf_client._execute.executePost(methodName, actionButtonItem, func);
                }
                else {
                    wanda_wf_tool.tips("不支持的请求方式");
                }
            }
        },
        executePost: function (methodName, actionButtonItem, func) {
            var execureParam = wanda_wf_data._buildPostData(methodName, actionButtonItem.ButtonType);
            var hfJsonDom = $("<input type='hidden'/>")
                        .attr("id", "MCS_WF_OPERATIONJSON")
                        .attr("name", "MCS_WF_OPERATIONJSON").insertBefore($(wanda_wf_client._otherSetting.ContainerDomID));
            hfJsonDom.val(JSON.stringify(execureParam));
            wanda_wf_tool.showLoading();
            document.getElementById("MCS_WF_OPERATIONJSON").form.submit();
        },
        executeAjax: function (methodName, actionButtonItem, func) {
            var executeParam = {
                BizContext: wanda_wf_data.BizContext,
                OperatorType: actionButtonItem.ButtonType,
                MethodName: methodName,
                Version: '1.0',
                WorkflowContext: wanda_wf_data.WorkFlowContext
            };
            //如果执行的是撤回操作，则直接执行工作流方法，然后调用After。
            if (executeParam.OperatorType == 7) {
                wanda_wf_client._execute.executeAjaxOnlyCallAfter(executeParam, methodName, func);
                return;
            }
            if (wanda_wf_client._otherSetting.IsAsync) {
                wanda_wf_client._execute.executeAjaxAsync(executeParam, methodName, func);
            }
            else {
                wanda_wf_client._execute.executeAjaxNoAsync(executeParam, methodName, func);
            }
        },
        executeAjaxOnlyCallAfter: function (executeParam, methodName, func) {
            wanda_wf_tool.ajax(function (data) {
                wanda_wf_client._processAjaxReturn(data);
                if (data.StatusCode == 0) {
                    if (func != undefined) {
                        func(true);
                    }
                }
                else {
                    if (func != undefined) {
                        func(false);
                    }
                }
                executeParam.WorkflowContext = data;
                wanda_wf_client._otherSetting.OnAfterExecute(executeParam);
            }, wanda_wf_data._buildAjaxData(methodName, 0))
        },
        executeAjaxAsync: function (executeParam, methodName, func) {
            //BeforeExecute
            wanda_wf_client._otherSetting.OnBeforeExecute(executeParam, function () {
                if (executeParam.BizContext.BusinessID == "") {
                    wanda_wf_tool.alert("业务ID不能为空！");
                    return;
                }
                //执行Step1
                wanda_wf_tool.ajax(function (data) {
                    if (data.StatusCode == 0) {
                        //调用SaveData
                        wanda_wf_client._otherSetting.OnSaveApplicationData(executeParam, function () {
                            wanda_wf_tool.ajax(function (data) {
                                wanda_wf_client._processAjaxReturn(data);
                                if (data.StatusCode == 0) {
                                    if (func != undefined) {
                                        func(true);
                                    }
                                }
                                else {
                                    if (func != undefined) {
                                        func(false);
                                    }
                                }
                                executeParam.WorkflowContext = data;
                                wanda_wf_client._otherSetting.OnAfterExecute(executeParam);
                            }, wanda_wf_data._buildAjaxData(methodName, 2))
                        });
                    }
                    else {
                        wanda_wf_tool.hideLoading();
                        wanda_wf_client._processAjaxReturn(data);
                    }
                }, wanda_wf_data._buildAjaxData(methodName, 1));
            });

        },
        executeAjaxNoAsync: function (executeParam, methodName, func) {
            //BeforeExecute
            var result = wanda_wf_client._otherSetting.OnBeforeExecute(executeParam);
            if (result == false) { return; }
            if (executeParam.BizContext.BusinessID == "") {
                wanda_wf_tool.alert("业务ID不能为空！");
                return;
            }

            //执行Step1
            wanda_wf_tool.ajax(function (data) {
                if (data.StatusCode == 0) {
                    //调用SaveData
                    var isSuccess = wanda_wf_client._otherSetting.OnSaveApplicationData(executeParam);
                    //Step2
                    if (isSuccess || isSuccess == undefined) {
                        wanda_wf_tool.ajax(function (data) {
                            wanda_wf_client._processAjaxReturn(data);
                            if (data.StatusCode == 0) {
                                if (func != undefined) {
                                    func(true);
                                }
                            }
                            else {
                                if (func != undefined) {
                                    func(false);
                                }
                            }
                            executeParam.WorkflowContext = data;
                            wanda_wf_client._otherSetting.OnAfterExecute(executeParam);
                        }, wanda_wf_data._buildAjaxData(methodName, 2))
                    }
                    else {
                        wanda_wf_tool.hideLoading();
                    }
                }
                else {
                    wanda_wf_tool.hideLoading();
                    wanda_wf_client._processAjaxReturn(data);
                }
            }, wanda_wf_data._buildAjaxData(methodName, 1));
        },
        checkSubmit: function (actionButtonItem, approvalContent) {
            var msg = "";
            if (actionButtonItem.ButtonType != 0) {
                $.each(wanda_wf_data.BizContext.NodeInstanceList, function (i, item) {
                    if (item.NodeType == 1 && (item.User == null || (item.User.UserCode == "" && item.User.UserLoginID == ""))) {
                        //校验节点中的人是否选择
                        msg = msg + "节点【" + item.NodeName + "】未选择审批人\r";
                    }
                })
            }
            if (approvalContent == undefined) {
                approvalContent = $("#wanda-wfclient-log-textarea").val();
            }
            wanda_wf_data.BizContext.ApprovalContent = approvalContent;
            if ($.trim(wanda_wf_data.BizContext.ApprovalContent) == "" && actionButtonItem.ButtonType != 0 && actionButtonItem.ButtonType != 7) {
                msg = msg + "请输入相关说明\r"
            }

            if (msg != "") {
                wanda_wf_tool.alert(msg);
                return false;
            }
            else {
                return true;
            }
        }
    },
    _domBuild: {
        buildDeleteImgDom: function () {
            return $("<img>", { src: wanda_wf_client._constSetting.deleteImgSrc(), title: "删除", "class": "wanda-wfclient-deleteimg" });
        },
        buildNavOkImgDom: function () {
            return $("<img>", { alt: "", src: wanda_wf_client._constSetting.okImgSrc() });
        },
        buildNavArrowSpDom: function () {
            return $("<span>", { "class": "wanda-wfclient-nav-sparrrow" }).append($("<img>", {
                alt: "", "class": "wanda-wfclient-nav-arrrow", src: wanda_wf_client._constSetting.arrowImgSrc()
            }));
        }
    },
    template: {
        content: function () {
            return [
            '<div class="wanda-wf-content" style="">',
                '<table class="wanda-wfclient-table">',
                    '<tr>',
                        '<td style="width:100px">审批流程</td>',
                        '<td>',
                            '<div id="wanda-wfclient-nav-content"></div>',
                        '</td>',
                    '</tr>',
                    '<tr>',
                        '<td>抄送</td>',
                        '<td>',
                            '<div id="wanda-wfclient-cc-content"></div>',
                        '</td>',
                    '</tr>',
                    '<tr>',
                        '<td>快捷意见</td>',
                        '<td>',
                            '<span class="wanda-wfclient-log-quicklog">同意</span><span class="spmarginright">&nbsp;</span>',
                            '<span class="wanda-wfclient-log-quicklog">不同意</span><span class="spmarginright">&nbsp;</span>',
                            '<span class="wanda-wfclient-log-quicklog">收到</span>',
                        '</td>',
                    '</tr>',
                    '<tr>',
                        '<td>审批意见</td>',
                        '<td>',
                            '<textarea id="wanda-wfclient-log-textarea"></textarea>',
                        '</td>',
                    '</tr>',
                '</table>',
                '<div id="wanda-wfclient-button-content" class="wanda-wfclient"></div>',
                '<!--审批日志-->',
                '<div id="wanda-wfclient-log-content">',
                '</div>',
            '</div>',

            ].join("");
        },
        button: function () {
            return [
                        '<table id="wanda-wfclient-button-table">',
                            '<tbody>',
                                '<tr style="height: 40px;">',
                                    '<td>&nbsp;</td>',
                                    '<td>',
                                        '<div id="wanda-wfclient-button-div">',
                                            '<!--按钮区域-->',
                                        '</div>',
                                    '</td>',
                                    '<td>&nbsp;</td>',
                                '</tr>',
                            '</tbody>',
                        '</table>'
            ].join("");
        },
        opinion: function () {
            return [
         '<table style="width: 100%; border-spacing: 0;">',
            '<tr>',
                '<td style="text-align: center;">',
                    '<table id="wanda-wfclient-log-logtable" class="wanda-wfclient-table">',
                        '<tbody>',
                            '<tr>',
                                '<th class="wanda-wfclient-log-logtable-thnode">节点</th>',
                                '<th>审批意见</th>',
                                '<th class="wanda-wfclient-log-logtable-approver">审批人</th>',
                                '<th class="wanda-wfclient-log-logtable-approvedate">审批时间</th>',
                                '<th class="wanda-wfclient-log-logtable-operator">操作</th>',
                            '</tr>',
                         '</tbody> ',
                     '</table> ',
                 '</td> ',
             '</tr> ',
             '<tr> ',
                 '<td id="wanda-wfclient-log-showmore"> ',
                     '<span><img alt="\" src="' + wanda_wf_client._constSetting.downImgSrc() + '"> ',
                     '<a>显示更多</a></span>',
                 '</td> ',
             '</tr> ',
           '</table> ', ].join("");
        },
        controlTemplate: {
            AddNode: function () {
                return [
                '<div id="wanda-wfclient-addnode-content">',
                    '<table class="wanda-wfclient-table">',
                        '<tr>',
                            '<th style="width: 100px;">加签<span style="color: red;">*</span>:</th>',
                            '<td>',
                                '<a id="wanda-wfclient-addnode-aselect">请选择</a>',
                            '</td>',
                        '</tr>',
                        '<tr>',
                            '<th>节点:</th>',
                            '<td>',
                                '<select id="wanda-wfclient-addnode-currentnode"></select>',
                                '<input id="wanda-wfclient-addnode-type_after" name="wanda-wfclient-addnode-type" type="radio" value="1" checked="checked" /><label for="wanda-wfclient-addnode-type_after">之后</label>',
                                '<input id="wanda-wfclient-addnode-type_before" name="wanda-wfclient-addnode-type" type="radio" value="0" /><label for="wanda-wfclient-addnode-type_before">之前</label>',
                            '</td>',
                        '</tr>',
                        '<tr>',
                            '<th>审批类型:</th>',
                            '<td>',
                                '<input id="wanda-wfclient-addnode-audittype_order" name="wanda-wfclient-addnode-audittype" type="radio" value="1"  checked="checked" ><label for="wanda-wfclient-addnode-audittype-order">顺序审批</label>',
                                '<input id="wanda-wfclient-addnode-audittype_cosigner" name="wanda-wfclient-addnode-audittype" type="radio" value="2"><label for="wanda-wfclient-addnode-audittype_cosigner">同时审批</label>',
                                '<input id="wanda-wfclient-addnode-audittype_autoinform" name="wanda-wfclient-addnode-audittype" type="radio" value="3"><label for="wanda-wfclient-addnode-audittype_autoinform">通知</label>',
                            '</td>',
                        '</tr>',
                        '<tr>',
                            '<th>加签节点名称:</th>',
                            '<td>',
                                '<input id="wanda-wfclient-addnode-nodename" type="text" value="1" >',
                            '</td>',
                        '</tr>',
                        '<tr style="display: none;" id="wanda-wfclient-addnode-trlog">',
                            '<th>加签意见<span style="color: red;">*</span>:</th>',
                            '<td>',
                                '<textarea id="wanda-wfclient-addnode-logtextarea"></textarea>',
                            '</td>',
                        '</tr>',
                    '</table>',
                '</div>'
                ].join("");
            },
            Return: function () {
                return [
                '<div id="wanda-wfclient-return-content">',
                    '<table class="wanda-wfclient-table">',
                        '<tr>',
                            '<th style="width: 100px;">退回到<span style="color: red;">*</span>：</th>',
                            '<td><select id="wanda-wfclient-return-select"></select></td>',
                        '</tr>',
                        '<tr>',
                            '<th>退回意见<span style="color: red;">*</span>：</th>',
                            '<td>',
                                '<textarea id="wanda-wfclient-return-logtextarea"></textarea>',
                            '</td>',
                        '</tr>',
                    '</table>',
                '</div>'
                ].join("");
            },
            Forward: function () {
                return [
             '<div id="wanda-wfclient-forward-content">',
                '<table class="wanda-wfclient-table">',
                    '<tr>',
                        '<th style="width:100px;">接收人<span style="color: red;">*</span>：</th>',
                        '<td><a id="wanda-wfclient-forward-aselect">请选择</a></td>',
                    '</tr>',
                    '<tr>',
                        '<th>转发意见<span style="color: red;">*</span>：</th>',
                        '<td>',
                            '<textarea id="wanda-wfclient-forward-logtextarea"></textarea>',
                        '</td>',
                    '</tr>',
                '</table>',
            '</div>'
                ].join("");
            }
        }
    }
}