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

    window.onclick = function() {
        modals.forEach(modal => {
            if (event.target == modal) {
                modal.style.display = 'none';

                let form = document.querySelector(`#${modal.id} form`);
                if (form != null) {
                    document.querySelectorAll(`#${modal.id} span.error`).forEach(x => x.style.display = 'none');
                    document.querySelectorAll(`#${modal.id} input`).forEach(x => x.required = false);
                    document.querySelector(`#${modal.id} input[type=submit]`).disabled = true;
                    form.reset();
                }
            }
        });
    }
}