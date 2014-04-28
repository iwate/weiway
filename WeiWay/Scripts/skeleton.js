var Skeleton;
(function (Skeleton) {
    var _extends = function (props, staticProps) {
        var base = this, properties, statics, klass, _constructor;
        properties = props || {};
        statics = staticProps || {};
        klass = _constructor = props.constructor != Object ? props.constructor : function () {
            // this must be declared as "function"
            this.base().apply(this, arguments);
        };
        klass.extends = _extends;
        for (var p in base)
            if (base.hasOwnProperty(p))
                klass[p] = base[p];
        function __() {
            this.constructor = klass;
        }
        __.prototype = base.prototype;
        klass.prototype = new __();
        Object.keys(properties).forEach(function (key) {
            klass.prototype[key] = properties[key];
        });
        Object.keys(statics).forEach(function (key) {
            klass[key] = statics[key];
        });
        klass.prototype.base = function (propName) {
            var _this = this;
            if (typeof propName === "undefined") { propName = "constructor"; }
            var prop = this.klass.base.prototype[propName];
            if (typeof prop == "function") {
                return function () {
                    var base = _this.klass.base;
                    if (!base) {
                        throw "It will be not here.";
                    }
                    _this.klass.base = _this.klass.base.base || null;
                    prop.apply(_this, arguments);
                    _this.klass.base = base;
                };
            } else {
                return prop;
            }
        };
        klass.prototype.klass = klass;
        klass["base"] = base;
        return klass;
    };
    function add(listeners, key, listener) {
        var i = listeners.length;
        while (i--) {
            if (listeners[i] === listener) {
                return;
            }
        }
        listeners.push(listener);
    }
    function remove(listeners, key, listener) {
        var i = listeners.length;
        while (i--) {
            if (listeners[i] === listener) {
                listeners.splice(i, 1);
                return;
            }
        }
    }
    function explorer(obj, keys) {
        var head = keys[0], tails = keys.slice(1, keys.length);
        if (typeof obj[head] === "undefind") {
            throw "miss match in explorer";
        }
        if (tails.length == 0) {
            return {
                object: obj,
                key: head
            };
        }
        return explorer(obj[head], tails);
    }
    var Class = (function () {
        function Class() {
        }
        Class.extends = _extends;
        return Class;
    })();
    Skeleton.Class = Class;
    Skeleton.Eventable = Class.extends({
        constructor: function () {
            this.base()();
            this.__skeletonEventListeners__ = {};
        },
        addEventListener: function (key, listener) {
            if (!Array.isArray(this.__skeletonEventListeners__[key])) {
                this.__skeletonEventListeners__[key] = [];
            }
            add(this.__skeletonEventListeners__[key], key, listener);
        },
        removeEventListener: function (key, listener) {
            if (!Array.isArray(this.__skeletonEventListeners__[key])) {
                return;
            }
            remove(this.__skeletonEventListeners__[key], key, listener);
        },
        dispatch: function (key) {
            var that, params;
            if (!Array.isArray(this.__skeletonEventListeners__[key])) {
                return;
            }
            that = this;
            params = [].slice.call(arguments, 1);
            this.__skeletonEventListeners__[key].forEach(function (listener) {
                listener.apply(that, params);
            });
        },
        delegate: function (method) {
            var that = this;
            return function () {
                method.apply(that, arguments);
            };
        }
    }, {});
    Skeleton.Model = Skeleton.Eventable.extends({
        constructor: function (props) {
            var _this = this;
            this.base()();
            this.__properties__ = {};
            this.properties.forEach(function (key) {
                _this.__properties__[key] = {
                    value: null,
                    listeners: []
                };
            });
            props = props || {};
            Object.keys(props).forEach(function (key) {
                _this[key] = props[key];
            });
            this.autoSync = false;
        },
        properties: [],
        bind: function (key, listener) {
            add(this.__properties__[key].listeners, key, listener);
        },
        unbind: function (key, listener) {
            remove(this.__properties__[key].listeners, key, listener);
        },
        unbindAll: function () {
            var that = this;
            Object.keys(this.propertie).forEach(function (key) {
                var listeners = that.__properties__[key].listeners;
                listeners.splice(0, listeners.length);
            });
        },
        sync: function () {
        },
        delete: function () {
        },
        export: function () {
            var that = this, res = {};
            this.properties.forEach(function (key) {
                res[key] = that[key];
            });
            return res;
        }
    }, {
        extends: function (props, staticProps) {
            var klass = this.base.extends.apply(this, arguments);
            klass.prototype.properties.forEach(function (key) {
                klass.prototype.__defineGetter__(key, function () {
                    return this.__properties__[key].value;
                });
                klass.prototype.__defineSetter__(key, function (val) {
                    var that = this;
                    this.__properties__[key].value = val;
                    this.__properties__[key].listeners.forEach(function (listener) {
                        listener.apply(that, [val]);
                    });
                    this.dispatch("change", key, val);
                });
            });
            return klass;
        }
    });
    Skeleton.Template = Skeleton.Eventable.extends({
        render: function () {
            return document.createElement("div");
        }
    });
    Skeleton.JsonMLTemplate = Skeleton.Template.extends({
        constructor: function (jsonml) {
            this.base()();
            this.jsonml = jsonml;
        },
        decode: function (jsonml) {
            var _decode = function (jsonml) {
                var tag = jsonml[0], ext = jsonml.slice(1, jsonml.length);
                var elem;
                if (typeof tag === "string") {
                    elem = document.createElement(tag);
                    for (var i = 0, len = ext.length; i < len; i++) {
                        if (Array.isArray(ext[i])) {
                            elem.appendChild(_decode(ext[i]));
                        } else if (ext[i].constructor == Object) {
                            Object.keys(ext[i]).forEach(function (attr) {
                                elem.setAttribute(attr, ext[i][attr]);
                            });
                        } else if (typeof ext[i] === "string") {
                            elem.innerText = ext[i];
                        }
                    }
                }
                return elem;
            };
            return _decode(jsonml);
        },
        render: function () {
            return this.decode(this.jsonml);
        }
    });
    Skeleton.HTMLTemplate = Skeleton.Template.extends({
        constructor: function (elem) {
            this.base()();
            if (typeof elem == "string") {
                this.id = elem;
            } else {
                this.element = elem;
            }
            this.html = null;
        },
        render: function () {
            var elem = document.createElement("div");
            if (!this.html) {
                if (!this.element) {
                    this.element = document.querySelector(this.id);
                }
                if (!this.element) {
                    throw "It is need DOM id or DOM element.";
                }
                this.html = this.element.innerHTML;
            }
            elem.innerHTML = this.html;
            return elem.firstElementChild;
        }
    });
    Skeleton.View = Skeleton.Eventable.extends({
        constructor: function (props) {
            var _this = this;
            this.base()();
            props = props || {};
            this.__binds__ = [];
            this.__bindAttrs__ = [];
            this.__skeletonEvents__.forEach(function (event) {
                _this.addEventListener(event.trigger, _this[event.callback]);
            });
            if (!!props.model && !this.model) {
                this.model = props.model;
            }
            if (!!this.model) {
                this.__modelEvents__.forEach(function (event) {
                    this.model.addEventListener(event.trigger, this[event.callback]);
                });
            }
        },
        appendTypes: {
            Append: "append",
            Prepend: "prepend",
            Before: "before",
            After: "after"
        },
        appends: {
            "append": function (parent, elem) {
                parent.appendChild(elem);
                return elem;
            },
            "prepend": function (parent, elem) {
                parent.insertBefore(elem, parent.firstElementChild);
                return elem;
            },
            "before": function (target, elem) {
                target.parentElement.insertBefore(elem, target);
                return elem;
            },
            "after": function (target, elem) {
                target.parentElement.insertAfter(elem, target);
                return elem;
            }
        },
        renderTo: function (target, appendType) {
            if (!this.template) {
                return;
            }
            appendType = appendType || this.appendTypes.Append;
            this.element = this.appends[appendType](target, this.template.render());
            this.bindAll();
            this.element.skeleton = this;
            this.dispatch("rendered");
            return this;
        },
        bindAll: function () {
            var _this = this;
            var binds = this.element.querySelectorAll("*[data-skl-bind]");
            var attrs = this.element.querySelectorAll("*[data-skl-bind-attr]");
            var i, len;
            for (i = 0, len = binds.length; i < len; i++) {
                this.__binds__ = this.__binds__.concat(this.decodeBinds(binds[i], binds[i].getAttribute("data-skl-bind")));
            }
            var bind = this.element.getAttribute("data-skl-bind");
            if (!!bind) {
                this.__binds__ = this.__binds__.concat(this.decodeBinds(this.element, bind));
            }
            for (i = 0, len = attrs.length; i < len; i++) {
                this.__bindAttrs__ = this.__bindAttrs__.concat(this.decodeBinds(attrs[i], attrs[i].getAttribute("data-skl-attr")));
            }
            var attr = this.element.getAttribute("data-skl-attr");
            if (!!attr) {
                this.__bindAttrs__ = this.__bindAttrs__.concat(this.decodeBinds(this.element, attr));
            }
            this.__binds__.forEach(function (b) {
                var raw = b.values[0].match(/"(.*)"/) || b.values[0].match(/'(.*)'/);
                if (!!raw) {
                    var target = explorer(b.element, b.keys);
                    target.object[target.key] = raw[1];
                } else {
                    _this.model.bind(b.values[0], function (val) {
                        var target, value;
                        target = explorer(b.element, b.keys);
                        value = explorer(_this.model, b.values);
                        target.object[target.key] = value.object[value.key];
                    });
                    var value = explorer(_this.model, b.values);
                    value.object[value.key] = value.object[value.key];
                }
            });
            this.__bindAttrs__.forEach(function (b) {
                _this.model.bind(b.values[0], function (val) {
                    var value;
                    value = explorer(_this.model, b.values);
                    b.element.setAttribute(b.keys[0], value.object[value.key]);
                });
                var value = explorer(_this.model, b.values);
                value.object[value.key] = value.object[value.key];
            });
            this.__domEvents__.forEach(function (event) {
                var targets = _this.findAll(event.target);
                if (targets.length == 0) {
                    throw "Target element is not found: " + event.target;
                }
                targets.forEach(function (target) {
                    target.addEventListener(event.trigger, function () {
                        _this[event.callback].apply(_this, arguments);
                    });
                });
            });
        },
        decodeBinds: function (elem, strBinds) {
            var binds = [];
            strBinds.replace(/\s*/g, "").split(";").forEach(function (b) {
                var keyval = b.split(":");
                if (keyval.length !== 2) {
                    throw "miss match";
                }
                binds.push({
                    element: elem,
                    keys: keyval[0].split("."),
                    values: keyval[1].split(".")
                });
            });
            return binds;
        },
        find: function (query) {
            if (this.element === null) {
                throw "not element";
            }
            var elem = this.element.querySelector(query);
            if (!!elem)
                return elem;
            elem = this.element.parentElement.querySelector(query);
            if (!!elem && elem === this.element)
                return elem;
            return null;
        },
        findAll: function (query) {
            var founds = [], elems, elem, i, len;
            if (this.element === null) {
                throw "not element";
            }
            elems = this.element.querySelectorAll(query);
            if (elems.length > 0) {
                for (i = 0, len = elems.length; i < len; i++) {
                    founds.push(elems[i]);
                }
                return founds;
            }
            elem = this.element.parentElement.querySelector(query);
            if (!!elem && elem === this.element) {
                founds.push(elem);
                return founds;
            }
            return founds;
        }
    }, {
        extends: function (props, statics) {
            var klass = Skeleton.Eventable.extends.apply(this, arguments);
            klass.prototype.__modelEvents__ = [];
            klass.prototype.__domEvents__ = [];
            klass.prototype.__skeletonEvents__ = [];
            if (!!props.events && props.events.constructor === Object) {
                Object.keys(props.events).forEach(function (key) {
                    var trigger, array;
                    array = key.split(/\s+/);
                    trigger = array.shift();
                    if (array.length > 0) {
                        if (array[0] === "model") {
                            klass.prototype.__modelEvents__.push({
                                trigger: trigger,
                                callback: props.events[key]
                            });
                        } else {
                            klass.prototype.__domEvents__.push({
                                trigger: trigger,
                                target: array.join(" "),
                                callback: props.events[key]
                            });
                        }
                    } else {
                        klass.prototype.__skeletonEvents__.push({
                            trigger: trigger,
                            callback: props.events[key]
                        });
                    }
                });
            }
            return klass;
        }
    });
})(Skeleton || (Skeleton = {}));
window.Skeleton = Skeleton;
//# sourceMappingURL=skeleton.js.map