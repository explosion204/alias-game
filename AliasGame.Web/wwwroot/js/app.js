window.onload = async function() {
    initModals();
    initForms();
    initValidation();

    document.getElementById('startGame').onclick = onBoxClick;
    document.getElementById('joinGame').onclick = onBoxClick;
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

    await isAuthenticated(onAuthenticated, onNotAuthenticated);
    getAccessToken();
}

function onPageLoaded() {
    document.getElementsByTagName('body')[0].style.display = 'block';
}

function onAuthenticated(result) {
    document.getElementById('signInBtn').style.display = 'none';
    document.getElementById('signUpBtn').style.display = 'none';
    document.getElementById('signOutBtn').style.display = 'block';
    document.getElementById('profile').style.display = 'block';
    document.getElementById('profile-nickname').innerText = result['body']['nickname'];
    document.getElementById('profile-total-games').innerText = result['body']['totalGames'];
    document.getElementById('profile-wins').innerText = result['body']['wins'];
    onPageLoaded();
}

function onNotAuthenticated() {
    document.getElementById('signInBtn').style.display = 'block';
    document.getElementById('signUpBtn').style.display = 'block';
    document.getElementById('signOutBtn').style.display = 'none';
    document.getElementById('profile').style.display = 'none';
    onPageLoaded(); 
}

// DEV: only for test
async function onBoxClick() {
    let response = await fetch('/dev');

    if (response.ok) {
        response.text().then(text => {
            document.getElementsByTagName('main')[0].innerHTML = text;
        })
    }
}
