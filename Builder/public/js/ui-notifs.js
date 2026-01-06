const notifs = document.querySelector('.notifs');

const notif = (text, caption = 'builder', type = 'info', time = 5) => {
    const notif = document.createElement('div');
    notif.className = `notif ${type}`;

    const close = document.createElement('button');
    close.className = 'notif-close';
    close.textContent = 'x';

    const theCaption = document.createElement('div');
    theCaption.className = 'notif-caption';
    theCaption.textContent = caption;

    const theText = document.createElement('div');
    theText.className = 'notif-text';
    theText.textContent = text;

    notif.appendChild(close);
    notif.appendChild(theCaption);
    notif.appendChild(theText);
    notifs.appendChild(notif);

    setTimeout(() => {
        notif.classList.add('show');
    }, 10);

    setTimeout(() => {
        delNotif(notif);
    }, time * 1000);

    close.addEventListener('click', () => {
        delNotif(notif);
    });

    return notif;
};

const delNotif = (notif) => {
    notif.classList.remove('show');

    if (!notif.parentNode) {
        return;
    }

    notif.parentNode.removeChild(notif);
};