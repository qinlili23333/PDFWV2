Object.defineProperty(URL.prototype, 'origin', {
    get: ()=> {
        console.log('URL.origin getter was called, returning "null"');
        return "null";
    },
    set: ()=> { },
    configurable: false
});