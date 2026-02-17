export default {
    stringCS(str, key, val) {
        const regex = new RegExp(`public\\s+const\\s+string\\s+${key}\\s*=\\s*".*?";`);
        return str.replace(regex, `public const string ${key} = "${val}";`);
    },

    boolCS(str, key, val) {
        const regex = new RegExp(`public\\s+const\\s+bool\\s+${key}\\s*=\\s*(true|false);`);
        return str.replace(regex, `public const bool ${key} = ${val};`);
    },

    enumCS(str, key, type, val) {
        const regex = new RegExp(`public\\s+const\\s+${type}\\s+${key}\\s*=\\s*${type}.*?;`);
        return str.replace(regex, `public const ${type} ${key} = ${type}.${val};`);
    },

    stringRS(str, key, val) {
        const regex = new RegExp(`pub\\s+const\\s+${key}:\\s*&\\s*str\\s*=\\s*".*?";`);
        return str.replace(regex, `pub const ${key}: &str = "${val}";`);
    },

    boolRS(str, key, val) {
        const regex = new RegExp(`pub\\s+const\\s+${key}:\\s*bool\\s*=\\s*(true|false);`);
        return str.replace(regex, `pub const ${key}: bool = ${val};`);
    },
};