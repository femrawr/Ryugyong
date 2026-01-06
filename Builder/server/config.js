export default {
    port: '1987',
    host: 'localhost',
    protocol: 'http',

    debug: true,

    cachedCryptoKey: '',

    get domain() {
        return this.protocol + '://' + this.host + ':' + this.port;
    }
};