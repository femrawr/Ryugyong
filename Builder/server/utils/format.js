export default {
    fromPascal(str) {
        return str
            .replace(/([A-Z])/g, ' $1')
            .trim();
    }
};