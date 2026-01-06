export default {
    _chars: 'qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM1234567890',

    genStr(len) {
        if (!len) {
            console.error('cannot gen string due to no length provided');
            return '';
        }

        let str = '';

        for (let i = 0; i < len; i++) {
            str += this._chars.charAt(Math.floor(Math.random() * this._chars.length));
        }

        return str;
    }
};