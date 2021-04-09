window.addEventListener('load', async function (e) {
    document.getElementById('leaveGameBtn').onclick = async function () {
        document.getElementById('leaveGameBtn').setAttribute('disabled', 'disabled');
        await leaveGame();
        location.reload();
    };
});

async function setupGameLayout(gameLayout, sessionId, userId) {
    document.getElementsByTagName('main')[0].innerHTML = gameLayout;
    document.getElementById('footer').style.display = 'none';
    document.getElementById('mainContent').style.margin = 0;
    document.getElementById('signOutBtn').style.display = 'none';
    document.getElementById('leaveGameBtn').style.display = 'block';

    document.getElementById('join-first-team-btn').onclick = async function (e) {
        document.querySelectorAll('.join-team-btn').forEach(x => x.setAttribute('disabled', 'disabled'));
        await joinGame(1);
    }

    document.getElementById('join-second-team-btn').onclick = async function (e) {
        document.querySelectorAll('.join-team-btn').forEach(x => x.setAttribute('disabled', 'disabled'));
        await joinGame(2);
    }

    await initSignalRConnection();
    subscribe(sessionId, userId);
    // window.onbeforeunload = function (e) {
    //     return '';
    // }
    window.onunload = async function (e) {
        if (localStorage.getItem('position')) {
            console.log('leave');
            await leaveGame();
            unsubscribe(sessionId, userId);
        }

        return '';
    }
}


async function fetchGameLayout(sessionId, firstPlayer, secondPlayer = "", thirdPlayer = "", fourthPlayer = "") {
    let response = await fetch('/fetch_game_html');

    if (response.ok) {
        let htmlString = await response.text();
        let gameDoc = new DOMParser().parseFromString(htmlString, "text/html");
        gameDoc.getElementById('lobby-id').innerText = 'Game ID: ' + sessionId;
        let totalPlayers = 0;

        if (firstPlayer) {
            gameDoc.getElementById('first-player').innerText = firstPlayer;
            localStorage.setItem('firstPlayer', firstPlayer);
            totalPlayers++;
        }

        if (secondPlayer) {
            gameDoc.getElementById('second-player').innerText = secondPlayer;
            localStorage.setItem('secondPlayer', secondPlayer);
            totalPlayers++;
        }

        if (thirdPlayer) {
            gameDoc.getElementById('third-player').innerText = thirdPlayer;
            localStorage.setItem('thirdPlayer', thirdPlayer);
            totalPlayers++;
        }

        if (fourthPlayer) {
            gameDoc.getElementById('fourth-player').innerText = fourthPlayer;
            localStorage.setItem('fourthPlayer', fourthPlayer);
            totalPlayers++;
        }

        gameDoc.getElementById('totalPlayers').innerText = totalPlayers;

        if (firstPlayer && secondPlayer) {
            gameDoc.getElementById('join-first-team-btn').setAttribute('disabled', 'disabled');
        }

        if (thirdPlayer && fourthPlayer) {
            gameDoc.getElementById('join-second-team-btn').setAttribute('disabled', 'disabled');
        }

        return new XMLSerializer().serializeToString(gameDoc);
    }

    return "NO CONTENT";
}

function onUserConnected(userNickname, position) {
    console.log(userNickname);
    let totalPlayersSpan = document.getElementById('totalPlayers');
    let totalPlayers = Number.parseInt(totalPlayersSpan.innerText);
    totalPlayersSpan.innerText = ++totalPlayers;

    
    switch (position) {
        case 2:
            document.getElementById('second-player').innerText = userNickname;
            localStorage.setItem('secondPlayer', userNickname);
            break;
        case 3:
            document.getElementById('third-player').innerText = userNickname;
            localStorage.setItem('thirdPlayer', userNickname);
            break;
        case 4:
            document.getElementById('fourth-player').innerText = userNickname;
            localStorage.setItem('fourthPlayer', userNickname);
            break;
    }

    if (!localStorage.getItem('position')) {
        updateJoinButtons(position);
    }
}

function onUserDisconnected(position) {
    let totalPlayersSpan = document.getElementById('totalPlayers');
    let totalPlayers = Number.parseInt(totalPlayersSpan.innerText);
    totalPlayersSpan.innerText = --totalPlayers;
    
    const EMPTY_SLOT = "Empty slot";
    console.log(position);

    switch (position) {
        case '2': {
            document.getElementById('second-player').innerText = EMPTY_SLOT;
            localStorage.removeItem('secondPlayer');
            break;
        }
        case '3': {
            document.getElementById('third-player').innerText = EMPTY_SLOT;
            localStorage.removeItem('thirdPlayer');
            break;
        }
        case '4': {
            document.getElementById('fourth-player').innerText = EMPTY_SLOT;
            localStorage.removeItem('fourthPlayer');
            break;
        }
    }

    if (localStorage.getItem('position') != null) {
        updateJoinButtons(position);
    }
}


function updateJoinButtons(position) {
    let predicate = function (teamCode) {
        switch (teamCode) {
            case 1: 
                let firstPlayer = localStorage.getItem('firstPlayer');
                let secondPlayer = localStorage.getItem('secondPlayer');
                return !(firstPlayer && secondPlayer);
            case 2:
                let thirdPlayer = localStorage.getItem('thirdPlayer');
                let fourthPlayer = localStorage.getItem('fourthPlayer');
                return !(thirdPlayer && fourthPlayer);
        }
    
        return false;
    }

    switch (position) {
        case 2:
            if (predicate(1)) {
                document.getElementById('join-first-team-btn').removeAttribute('disabled', 'disabled');
            } else {
                document.getElementById('join-first-team-btn').setAttribute('disabled', 'disabled');
            }
            break;
        case 3:
        case 4:
            if (predicate(2)) {
                document.getElementById('join-second-team-btn').removeAttribute('disabled', 'disabled');
            } else {
                document.getElementById('join-second-team-btn').setAttribute('disabled', 'disabled');
            }
            break;
    }
}

function onSessionClosed() {
    alert('Session is closed by creator.');
    location.reload();
}