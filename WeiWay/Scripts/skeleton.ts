module Skeleton {
    function add(listeners: Array<(...any) => void>, listener: (...any) => void) {
        var i = listeners.length;
        while (i--) {
            if (listeners[i] === listener) {
                return;
            }
        }
        listeners.push(listener);
    }
    function remove(listeners: Array<(...any) => void>, listener: (...any) => void){
        var i = listeners.length;
        while (i--) {
            if (listeners[i] === listener) {
                listeners.splice(i, 1);
                return;
            }
        }
    }
    export class Class {
    }
    export class Eventable extends Class {
        private __skeletonEventListeners__: Object
        constructor() {
            super();
            this.__skeletonEventListeners__ = {}
        }
        addEventListener(trigger: string, callback:(...any) => void) {
            if (!Array.isArray(this.__skeletonEventListeners__[trigger])){
                this.__skeletonEventListeners__[trigger] = [];
            }
            add(this.__skeletonEventListeners__[trigger], callback);
        }
        removeEventListener(trigger: string, callback: (...any) => void) {
            if (!Array.isArray(this.__skeletonEventListeners__[trigger])) {
                return;
            }
            remove(this.__skeletonEventListeners__[trigger], callback);
        }
        dispatch(trigger: string, ...any) {
            if (!Array.isArray(this.__skeletonEventListeners__[trigger])) {
                return;
            }
            var params = [].slice.call(arguments, 1);
            this.__skeletonEventListeners__[trigger].forEach((listener: (...any) => void) => {
                listener.apply(this, params);
            });
        }
    }
    export class Model extends Eventable {
        autoSyncable: boolean = false
        properties: string[]
        constructor() {
            super();
            this.properties.forEach((key: string) => {
            });
        }
        bind(key: string, callback: (...any) => void) {
        }
        unbind() {
        }
        serialize() {
        }
        sync() {
        }
    }
    export class View extends Eventable {
    }
}