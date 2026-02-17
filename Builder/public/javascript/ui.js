const tabs = document.querySelectorAll('.tab-btn');
const contents = document.querySelectorAll('.content');

const inputs = document.querySelectorAll('input');
const toggles = document.querySelectorAll('.toggle');

const notifs = document.querySelector('.notifs');

const notif = (msg) => {
    const notif = document.createElement('div');
    notif.className = 'notif';
    notif.textContent = msg;

    notifs.appendChild(notif);
    notif.classList.add('show')

    setTimeout(() => notif.remove(), 4000);

    notif.addEventListener('click', () => {
        notif.classList.remove('show');
        notif.classList.add('hide');
        setTimeout(() => notif.remove(), 100);
    });
};

inputs.forEach(a => {
    a.setAttribute('autocomplete', 'off');
    a.setAttribute('autocorrect', 'off');
    a.setAttribute('autocapitalize', 'off');
    a.setAttribute('spellcheck', 'false');
});

tabs.forEach(a => {
    a.addEventListener('click', () => {
        tabs.forEach(a => a.classList.remove('active'));
        contents.forEach(a => a.classList.remove('active'));

        a.classList.add('active');

        const target = a.getAttribute('data-tab');
        document.getElementById(`${target}-tab`).classList.add('active');
    });
});

toggles.forEach(a => {
    a.addEventListener('click', () => {
        a.classList.toggle('active');
    });
});