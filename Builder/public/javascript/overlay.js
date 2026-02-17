const overlay = document.querySelector('.build-overlay');
const stats = document.querySelector('.overlay-stats');

const statsText = document.querySelector('.stats-text');
const statsDots = document.querySelector('.stats-dots');
const statsTimer = document.querySelector('.stats-timer');

const info = document.querySelector('.overlay-info');
const hide = document.querySelector('.overlay-close');

const output = document.querySelector('.output');
const linking = document.querySelector('.linking');
const version = document.querySelector('.version');
const powershell = document.querySelector('.powershell');

let dotLoop = null;
let timerLoop = null;
let startTime = null;

const showOverlay = () => {
    overlay.classList.add('show');
    info.classList.remove('show');
    hide.classList.remove('show');

    setStatus('building', '#e0e0e0');
};

const setStatus = (text, color, noloop = false) => {
    statsText.textContent = text;
    stats.style.color = color;

    if (dotLoop) clearInterval(dotLoop);
    if (timerLoop) clearInterval(timerLoop);

    if (noloop) {
        stopStatus(text);
        return;
    }

    let dotCount = 0;
    statsDots.textContent = '.';

    dotLoop = setInterval(() => {
        dotCount = (dotCount % 3) + 1;
        statsDots.textContent = '.'.repeat(dotCount);
    }, 500);

    startTime = Date.now();
    statsTimer.textContent = '(0s)';

    timerLoop = setInterval(() => {
        const elapsed = Math.floor((Date.now() - startTime) / 1000);
        statsTimer.textContent = `(${elapsed}s)`;
    }, 1000);
};

const stopStatus = (text) => {
    if (dotLoop) {
        clearInterval(dotLoop);
        dotLoop = null;
    }

    if (timerLoop) {
        clearInterval(timerLoop);
        timerLoop = null;
    }

    statsText.textContent = text;
    statsDots.textContent = '';
    statsTimer.textContent = '';
};

const setInfo = (options = {}) => {
    stopStatus('build successful');

    if (options.script !== '') {
        powershell.value = options.script ?? '';
        powershell.classList.add('show');
    }

    output.textContent = options.output ?? '';
    linking.textContent = options.linking ?? '';
    version.textContent = options.version ?? '';

    info.classList.add('show');
    hide.classList.add('show');
};

hide.addEventListener('click', () => {
    overlay.classList.remove('show');
    stopStatus();
});