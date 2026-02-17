const build = document.querySelector('#build');

let fullPath = '';

const onKey = (t) => {
    build.style.borderColor = t ? '#000' : '#555';
    build.innerHTML = t ? 'update config' : 'build';
};

document.addEventListener('keydown', (e) => {
    if (e.ctrlKey) onKey(true);
});

document.addEventListener('keyup', (e) => {
    if (!e.ctrlKey) onKey(false);
});

build.addEventListener('click', async (e) => {
    const config = JSON.parse(getConfig());

    if (e.ctrlKey) {
        config.update_only = true;
    } else {
        showOverlay();
    }

    const build = await fetch('/start-build', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(config)
    });

    if (!build.ok) {
        const text = await build.text();
        console.warn(text);

        setStatus(text, '#ce5858ff', true);
        return;
    }

    const data = await build.json();

    fullPath = data.output;

    setInfo({
        output: formatPath(data.output),
        linking: config.static_link ? 'static' : 'dynamic',
        version: `${data.version} [${config.custom_identifier}]`,
        script: data.script
    });
});

output.addEventListener('click', async () => {
    const copy = fullPath.substring(0, fullPath.lastIndexOf('\\'));
    await navigator.clipboard.writeText(copy);

    notif('build directory copied to clipboard');
});

powershell.addEventListener('click', async () => {
    await navigator.clipboard.writeText(powershell.value);

    notif('script copied to clipboard');
});

const formatPath = (path) => {
    const index = path.indexOf('redamancy');
    if (index === -1) {
        return path.replaceAll('\\', '/');
    }

    return path
        .substring(index)
        .replaceAll('\\', '/');
};