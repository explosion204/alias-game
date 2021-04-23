window.addEventListener('load', async function (e) {
    document.getElementById('leaveGameBtn').onclick = async function () {
        document.getElementById('leaveGameBtn').setAttribute('disabled', 'disabled');
        await leaveGame();
        location.reload();
    };
});

async function setupGameLayout(gameLayout, sessionId, userId) {
    await fetchWords();

    document.getElementsByTagName('main')[0].innerHTML = gameLayout;
    document.getElementById('footer').style.display = 'none';
    document.getElementById('mainContent').style.margin = 0;
    document.getElementById('signOutBtn').style.display = 'none';
    document.getElementById('leaveGameBtn').style.display = 'block';
    hideAcceptRejectButtons();

    document.getElementById('join-first-team-btn').onclick = async function (e) {
        document.querySelectorAll('.join-team-btn').forEach(x => x.setAttribute('disabled', 'disabled'));
        await joinGame(1);
    };

    document.getElementById('join-second-team-btn').onclick = async function (e) {
        document.querySelectorAll('.join-team-btn').forEach(x => x.setAttribute('disabled', 'disabled'));
        await joinGame(2);
    };

    document.getElementById('start-round-button').onclick = async function (e) {
        let userId = localStorage.getItem('userId');
        let sessionId = localStorage.getItem('sessionId');
        localStorage.setItem('timerOwner', true);
        document.getElementById('word').innerText = await getWord();

        showAcceptRejectButtons();
        startTimer();
        notifyTimerStarted(userId, sessionId);
    };

    document.getElementById('accept-btn').onclick = onAcceptButtonClick;
    document.getElementById('reject-btn').onclick = onRejectButtonClick;

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

    if (!localStorage.getItem('questioning') && !localStorage.getItem('answering')) {
        localStorage.setItem('questioning', 1);
        localStorage.setItem('answering', 2);
    }

    if (totalPlayers == 4) {
        document.getElementById('lobby-players-span').style.display = 'none';
        document.getElementById('lobby-id').style.display = 'none';
    
        if (localStorage.getItem('position') == localStorage.getItem('questioning')) {
            document.getElementById('start-round-button').style.display = 'block';
        }
    }

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

function startTimer() {
    let progressBar = document.querySelector('.lobby-id-container');
    let lobbyPlayersSpan = document.getElementById('lobby-players-span');
    let startRoundButton = document.getElementById('start-round-button');

    lobbyPlayersSpan.style.display = 'none';
    startRoundButton.style.display = 'none';

    if (window.Worker) {
        let timerWorker = new Worker('/js/game.timer.js');
        timerWorker.postMessage([5]);
        timerWorker.onmessage = function (e) {
            let message = e.data[0];

            switch (message) {
                case 'update':
                    let width = e.data[1];
                    progressBar.style.width = width + "%";
                    break;
                case 'finished':
                    if (localStorage.getItem('timerOwner') === 'true') {
                        localStorage.setItem('timerOwner', false);
                        nextRound();
                    }

                    progressBar.style.width = '100%';
                    break;
            }
        }
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

function updateScore(teamCode, delta) {
    switch (teamCode) {
        case 1: {
            let scoreDiv = document.getElementById('score1');
            let score = Number.parseInt(scoreDiv.innerText);
            score += delta;

            if (score >= 0) {
                scoreDiv.innerText = score;
            }

            if (score >= 30 && localStorage.getItem('timerOwner') === 'true') {
                let userId = localStorage.getItem('userId');
                let sessionId = localStorage.getItem('sessionId');

                notifyGameFinished(userId, sessionId, 1);
                onGameFinished(1);
            }

            break;
        }
        case 2: {
            let scoreDiv = document.getElementById('score2');
            let score = Number.parseInt(scoreDiv.innerText);
            score += delta;

            if (score >= 0) {
                scoreDiv.innerText = score;
            }

            if (score >= 30 && localStorage.getItem('timerOwner') === 'true') {
                let userId = localStorage.getItem('userId');
                let sessionId = localStorage.getItem('sessionId');

                notifyGameFinished(userId, sessionId, 1);
                onGameFinished(2);
            }

            break;
        }
    }
}

function onSessionClosed() {
    alert('Session is closed');
    clearGameData();
    location.reload();
}

function onRoundFinished(currentTeamCode) {
    let questioning;
    let answering;

    if (currentTeamCode == 1) {
        let playedAsQuestioner3 = localStorage.getItem('playedAsQuestioner3');
        let playedAsQuestioner4 = localStorage.getItem('playedAsQuestioner4');

        if (playedAsQuestioner3 < playedAsQuestioner4) {
            questioning = 3;
            answering = 4;
            playedAsQuestioner3++;
            localStorage.setItem('playedAsQuestioner3', playedAsQuestioner3);
        } else {
            questioning = 4;
            answering = 3;
            playedAsQuestioner4++;
            localStorage.setItem('playedAsQuestioner4', playedAsQuestioner4);
        }
    } else {
        let playedAsQuestioner1 = localStorage.getItem('playedAsQuestioner1');
        let playedAsQuestioner2 = localStorage.getItem('playedAsQuestioner2');

        if (playedAsQuestioner1 < playedAsQuestioner2) {
            questioning = 1;
            answering = 2;
            playedAsQuestioner1++;
            localStorage.setItem('playedAsQuestioner1', playedAsQuestioner1);
        } else {
            questioning = 2;
            answering = 1;
            playedAsQuestioner2++;
            localStorage.setItem('playedAsQuestioner2', playedAsQuestioner2);
        }
    }

    localStorage.setItem('questioning', questioning);
    localStorage.setItem('answering', answering);

    updateGameField();
    hideAcceptRejectButtons();
}

// word area & start button
function updateGameField() {
    let startRoundButton = document.getElementById('start-round-button');

    let position = localStorage.getItem('position');
    let questioning = localStorage.getItem('questioning');
    let answering = localStorage.getItem('answering');

    if (position == questioning) {
        startRoundButton.style.display = 'block';
    } else if (position == answering) {
        startRoundButton.style.display = 'none';
        // TODO: word area
    } else {
        startRoundButton.style.display = 'none';
    }

    document.querySelectorAll('.player').forEach(x => x.classList.remove('active'));
    document.getElementById('word').innerText = 'some word...';

    switch (questioning) {
        case '1': {
            document.getElementById('first-player').classList.add('active');
            break;
        }
        case '2': {
            document.getElementById('second-player').classList.add('active');
            break;
        }
        case '3': {
            document.getElementById('third-player').classList.add('active');
            break;
        }
        case '4': {
            document.getElementById('fourth-player').classList.add('active');
            break;
        }
    }
}

function showAcceptRejectButtons() {
    let acceptButton = document.getElementById('accept-btn');
    let rejectButton = document.getElementById('reject-btn');

    let position = localStorage.getItem('position');
    let questioning = localStorage.getItem('questioning');
    let answering = localStorage.getItem('answering');

    if (position == questioning) {
        acceptButton.style.display = 'inherit';
        rejectButton.style.display = 'inherit';
    } else if (position == answering) {
        acceptButton.style.display = 'none';
        rejectButton.style.display = 'none';
    } else {
        acceptButton.style.display = 'none';
        rejectButton.style.display = 'none';
    }
}

function hideAcceptRejectButtons() {
    document.getElementById('accept-btn').style.display = 'none';
    document.getElementById('reject-btn').style.display = 'none';
}

async function updateBacklog(teamCode, word, status) {
    let backlogElement = document.createElement('div');
    backlogElement.classList.add('expr');

    if (status === 'accepted') {
        backlogElement.innerHTML =            
            `<span class="accepted">${word}</span>`;

        switch (teamCode) {
            case 1: {
                updateScore(1, 1);
                break;
            }
            case 2: {
                updateScore(2, 1);
                break;
            }
        }
    } else {
        backlogElement.innerHTML =            
            `<span class="rejected">${word}</span>`;
    }

    
    document.getElementById('backlog').appendChild(backlogElement)
}

// TODO: update statistics in db
function onGameFinished(winner) {
    switch (winner) {
        case 1: {
            alert('team 1 wins');
            clearGameData();
            location.reload();
            break;
        }
        case 2: {
            alert('team 2 wins');
            clearGameData();
            location.reload();
            break;
        }
    }
}

async function onAcceptButtonClick() {
    document.getElementById('accept-btn').onclick = function() {}

    let userId = localStorage.getItem('userId');
    let sessionId = localStorage.getItem('sessionId');
    let position = localStorage.getItem('position');
    let word = document.getElementById('word').innerText;
    let teamCode = (position === '1' || position === '2') ? 1 : 2;
    document.getElementById('word').innerText = await getWord();

    notifyBacklogUpdated(userId, sessionId, teamCode, word, 'accepted');
    await updateBacklog(teamCode, word, 'accepted');

    document.getElementById('accept-btn').onclick = onAcceptButtonClick;
}

async function onRejectButtonClick() {
    document.getElementById('reject-btn').onclick = function() {}
    let userId = localStorage.getItem('userId');
    let sessionId = localStorage.getItem('sessionId');
    let position = localStorage.getItem('position');
    let word = document.getElementById('word').innerText;
    let teamCode = (position === '1' || position === '2') ? 1 : 2;
    document.getElementById('word').innerText = await getWord();

    notifyBacklogUpdated(userId, sessionId, teamCode, word, 'rejected');
    await updateBacklog(teamCode, word, 'rejected');

    document.getElementById('reject-btn').onclick = onRejectButtonClick;
}