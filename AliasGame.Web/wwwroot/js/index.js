window.addEventListener('load', async function() {
    initModals();
    initForms();
    initValidation();

    document.getElementById('startGame').onclick = async function () {
        startLoadingAnimation();
        document.getElementById('signOutBtn').style.display = 'none';

        await createGame(async function (result) {
            localStorage.setItem('sessionId', result['sessionId']);
            localStorage.setItem('position', 1);
            localStorage.setItem('questioning', 1);
            localStorage.setItem('answering', 2);
            localStorage.setItem('playedAsQuestioner1', 1);
            localStorage.setItem('playedAsQuestioner2', 0);
            localStorage.setItem('playedAsQuestioner3', 0);
            localStorage.setItem('playedAsQuestioner4', 0);
            localStorage.setItem('timerOwner', false); // user becomes a timer owner only when clicks 'start' button

            let sessionId = result['sessionId'];
            let userId = localStorage.getItem('userId');
            let nickname = localStorage.getItem('nickname');
            let gameLayout = await fetchGameLayout(sessionId, nickname);

            await setupGameLayout(gameLayout, sessionId, userId);
            document.querySelectorAll('.join-team-btn').forEach(x => x.setAttribute('disabled', 'disabled'));
            document.getElementById('accept-btn').style.display = 'inherit';
            document.getElementById('reject-btn').style.display = 'inherit';
            stopLoadingAnimation();
        }, function () {
            alert('You are not authorized');
            // document.getElementById('signOutBtn').style.display = 'block';
            stopLoadingAnimation();
        })
    }

    document.getElementById('signOutBtn').onclick = async function () {
        document.getElementById('signOutBtn').disabled = true;
        await signOut(
            async function () {
                document.getElementById('signInBtn').style.display = 'block';
                document.getElementById('signUpBtn').style.display = 'block';
                document.getElementById('signOutBtn').style.display = 'none';
                document.getElementById('profile').style.display = 'none';
                document.getElementById('signOutBtn').disabled = false;
                
                await isAuthenticated(onAuthenticated, onNotAuthenticated);
            }
        )
    };

    console.log('k');
    await isAuthenticated(onAuthenticated, onNotAuthenticated);
    getAccessToken();
});

function startLoadingAnimation() {
    document.getElementById('mainContent').style.display = 'none';
    document.getElementById('spinner').style.display = 'inline-block';
    document.getElementById('spinner').style.animationPlayState = 'running';
}

function stopLoadingAnimation() {
    document.getElementById('mainContent').style.display = 'flex';
    document.getElementById('spinner').style.display = 'none';
    document.getElementById('spinner').style.animationPlayState = 'paused';
}

function onAuthenticated(result) {
    console.log('authenticated');
    document.getElementById('signInBtn').style.display = 'none';
    document.getElementById('signUpBtn').style.display = 'none';
    document.getElementById('signOutBtn').style.display = 'block';
    document.getElementById('profile').style.display = 'block';
    document.getElementById('profile-nickname').innerText = result['nickname'];
    document.getElementById('profile-total-games').innerText = result['totalGames'];
    document.getElementById('profile-wins').innerText = result['wins'];

    localStorage.setItem('userId', result['userId']);
    localStorage.setItem('nickname', result['nickname']);
    stopLoadingAnimation();
}

function onNotAuthenticated() {
    document.getElementById('signInBtn').style.display = 'block';
    document.getElementById('signUpBtn').style.display = 'block';
    document.getElementById('signOutBtn').style.display = 'none';
    document.getElementById('profile').style.display = 'none';
    stopLoadingAnimation();
}