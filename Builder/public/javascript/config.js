const configMap = {
    box: [
        'bot-token',
        'server-id',
        'category-id',
        'custom-identifier',
        'cmd-prefix',

        'admin-app',

        'password',
        'exact-name',

        'new-name',
        'new-dir'
    ],

    tog: [
        'custom-new-name',
        'custom-new-dir',

        'require-admin',
        'prompt-admin',
        'do-without-admin',
        'force-prompt-admin',

        'disable-uac',
        'restart-pc',

        'require-password',
        'require-exact-name',

        'no-test-mode',
        'require-mouse-move',

        'fingerprint-check',
        'username-check',
        'wallpaper-check',
        'process-check',
        'network-check',
        'screen-check',
        'time-check',
        'file-pattern-check',

        'enable-logs',
        'logs-info',
        'logs-warns',
        'logs-errors',

        'disable-init',

        'clean-dir',
        'static-link',
        'build-script'
    ]
};

const CONFIG_NAME = 'rdmc.conf';

const getConfig = () => {
    const config = {};

    configMap.box.forEach(id => {
        const obj = document.querySelector('#' + id);
        const key = id.replace(/-/g, '_');
        config[key] = obj ? obj.value.trim() : '';
    });

    configMap.tog.forEach(id => {
        const obj = document.querySelector('#' + id);
        const key = id.replace(/-/g, '_');
        config[key] = obj ? obj.classList.contains('active') : false;
    });

    return JSON.stringify(config);
};

const loadConfig = () => {
    const saved = localStorage.getItem(CONFIG_NAME);
    if (!saved) return;

    const savedObj = JSON.parse(saved);

    Object.keys(savedObj).forEach(key => {
        const id = key.replace(/_/g, '-');
        const obj = document.querySelector('#' + id);
        if (obj && configMap.box.includes(id)) {
            obj.value = savedObj[key] || '';
        }
    });

    Object.keys(savedObj).forEach(key => {
        const id = key.replace(/_/g, '-');
        const obj = document.querySelector('#' + id);
        if (obj && configMap.tog.includes(id)) {
            obj.classList.toggle('active', !!savedObj[key]);
        }
    });
};

document.querySelectorAll('input').forEach((a) => {
    a.addEventListener('input', () => {
        localStorage.setItem(CONFIG_NAME, getConfig());
    });
});

document.querySelectorAll('.toggle').forEach((a) => {
    a.addEventListener('click', () => {
        localStorage.setItem(CONFIG_NAME, getConfig());
    });
});

document.querySelectorAll('.list-items').forEach((a) => {
    a.addEventListener('click', () => {
        localStorage.setItem(CONFIG_NAME, getConfig());
    });
});

loadConfig();