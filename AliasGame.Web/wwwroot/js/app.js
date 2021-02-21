window.onload = function() {
    initModals();
    document.getElementById("startGame").onclick = onBoxClick;
    document.getElementById("joinGame").onclick = onBoxClick;
}

function initModals() {
    modalIds = [['signInBtn', 'signInModal'], ['signUpBtn', 'signUpModal'], ['profile', 'profileModal']]
    modals = []

    modalIds.forEach(data => {
        btnId = data[0];
        modalId = data[1];

        let modal = document.getElementById(modalId);
        let btn = document.getElementById(btnId);
        
        btn.onclick = function() {
            modal.style.display = 'block';
        }

        modals.push(modal);
    });

    window.onclick = function(event) {
        modals.forEach(modal => {
            if (event.target == modal) {
                modal.style.display = 'none';
            }
        });
    }
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