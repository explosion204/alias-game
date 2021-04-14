async function createGame(onSuccess, onFailure) {
    let accessToken = await getAccessToken();

    if (accessToken) {
        
        let response = await fetch(`/api/game/create_session/${accessToken}`, {
            method: 'POST'
        });

        if (response.ok) {
            let result = await response.json();

            if (result['status'] === true) {
                await onSuccess(result);
            } else {
                onFailure();
            }
        }
    } else {
        onFailure();
    }
}

async function viewGame(sessionId, onSuccess, onFailure) {
    let accessToken = await getAccessToken();

    if (accessToken) {
        let response = await fetch(`/api/game/view_session/${accessToken}/${sessionId}`);

        if (response.ok) {
            let result = await response.json();

            if (result['status'] === true) {
                localStorage.setItem('sessionId', sessionId);
                onSuccess(result);
            } else {
                onFailure();
            }
        }
    } else {
        alert('Access token expired. Please, refresh the page.');
    }
}

async function joinGame(teamCode) {
    let accessToken = await getAccessToken();

    if (accessToken) {
        let userId = localStorage.getItem('userId');
        let sessionId = localStorage.getItem('sessionId');
        let userNickname = localStorage.getItem('nickname');
        let response = await fetch(`/api/game/join_session/${accessToken}/${sessionId}/${teamCode}`, {
            method: 'POST'
        });

        if (response.ok) {
            let result = await response.json();

            if (result['status'] === true) {
                let positionInTeam = result['position'];
                localStorage.setItem('position', positionInTeam);
            
                notifyConnected(userId, sessionId, userNickname, positionInTeam);
            }
        }

    } else {
        alert('Access token expired. Please, refresh the page.');
    }
}

async function leaveGame() {
    let accessToken = await getAccessToken();

    if (accessToken) {
        let sessionId = localStorage.getItem('sessionId');
        let userId = localStorage.getItem('userId');
        let position = localStorage.getItem('position');
        let nickname = localStorage.getItem('nickname');
        await fetch(`/api/game/leave_session/${accessToken}/${sessionId}`, {
            method: 'POST'
        });
        console.log('disconnection');
        notifyDisconnected(userId, sessionId, position);

        if (nickname === localStorage.getItem('firstPlayer')) {
            notifySessionClosed(userId, sessionId);
        }

        clearGameData();
    }
}



/**
 * MESSAGE-BASED INTERACTION FORMAT:
 * 
 * {
 *  "message_type": "type of message",
 *  "arg1": ...,
 *  "arg2": ...,
 *  ...
 * }
 * 
 * MESSAGE TYPES:
 * 
 * (1) user_connected
 * args: user_nickname, position (2, 3 or 4)
 * 
 * (2) user_disconnected
 * args: position(2, 3 or 4)
 * 
 * (3) session_closed
 * 
 * (4) timer_started
 */

/**
 * GAME VARIABLES:
 * 
 */

var connection = null;

async function initSignalRConnection() {
    connection = new signalR.HubConnectionBuilder().withUrl('/MessageHub').build();
    setHandlers();
    await connection.start();
}

function subscribe(sessionId, userId) {
    if (connection != null) {
        connection.invoke('Subscribe', sessionId, userId);
    }
}

function unsubscribe(sessionId, userId) {
    if (connection != null) {
        connection.invoke('Unsubscribe', sessionId, userId);
    }
}

function setHandlers() {
    if (connection != null) {
        connection.on('receive_message', function (message) {
            let json = JSON.parse(message);
            let messageType = json['message_type'];

            console.log(json);

            switch (messageType) {
                case 'user_connected': {
                    let userNickname = json['user_nickname'];
                    let position = json['position'];
                    onUserConnected(userNickname, position);
                    break;
                }
                case 'user_disconnected': {
                    let position = json['position'];
                    onUserDisconnected(position);
                    break;
                }
                case 'session_closed': {
                    onSessionClosed();
                    break;
                }
                case 'timer_started': {
                    onTimerStarted();
                    break;
                }
            }
        });
    }
}

function notifySession(sessionId, senderId, message) {
    if (connection != null) {
        connection.invoke('NotifySession', sessionId, senderId, message);
    }
}

function notifyConnected(senderId, sessionId, userNickname, position) {
    let message = {
        'message_type': 'user_connected',
        'user_nickname': userNickname,
        'position': position
    };
    let json = JSON.stringify(message);
    notifySession(sessionId, senderId, json); 
    onUserConnected(userNickname, position);
}

function notifyDisconnected(senderId, sessionId, position) {
    let message = {
        'message_type': 'user_disconnected',
        'position': position
    };
    let json = JSON.stringify(message);
    notifySession(sessionId, senderId, json);
}

function notifySessionClosed(senderId, sessionId) {
    let message = {
        'message_type': 'session_closed'
    };
    let json = JSON.stringify(message);
    notifySession(sessionId, senderId, json);
    clearGameData();
}

function notifyTimerStarted(senderId, sessionId) {
    let message = {
        'message_type': 'timer_started'
    };
    let json = JSON.stringify(message);
    notifySession(sessionId, senderId, json);
}

// function notifyTimerStopped(senderId, sessionId) {
//     let message = {
//         'message_type': 'timer_started'
//     };
//     let json = JSON.stringify(message);
//     notifySession(sessionId, senderId, json);
// }


function clearGameData() {
    localStorage.removeItem('firstPlayer');
    localStorage.removeItem('secondPlayer');
    localStorage.removeItem('thirdPlayer');
    localStorage.removeItem('fourthPlayer');
    localStorage.removeItem('sessionId');
    localStorage.removeItem('position');
}