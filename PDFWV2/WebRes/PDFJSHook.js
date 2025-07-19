Object.defineProperty(URL.prototype, 'origin', {
    get: ()=> {
        return "null";
    },
    set: ()=> { },
    configurable: false
});