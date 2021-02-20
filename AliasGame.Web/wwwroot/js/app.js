window.onload = function() {
    initModals();
}

function initModals() {
    modalIds = [['signInBtn', 'signInModal'], ['signUpBtn', 'profileModal']]
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