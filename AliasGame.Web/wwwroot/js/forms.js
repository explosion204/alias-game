function initForms() {
    document.getElementById('signupButton').onclick = async function() {
        this.disabled = true;
        let isValid = true;

        let signupPassword = document.getElementById('signupPassword');
        let signupConfirmPassword = document.getElementById('signupConfirmPassword');

        let signupPasswordError = document.querySelector('#signupPassword + span.error');
        let signupConfirmPasswordError = document.querySelector('#signupConfirmPassword + span.error');

        signupPasswordError.style.display = 'none';
        signupConfirmPasswordError.style.display = 'none';

        if (signupPassword.value !== signupConfirmPassword.value) {
            signupPasswordError.style.display = 'block';
            signupPasswordError.textContent = 'Passwords do not match';

            signupConfirmPasswordError.style.display = 'block';
            signupConfirmPasswordError.textContent = 'Passwords do not match';

            isValid = false;
        }

        if (isValid) {
            let formData = new FormData(document.forms.signupForm);

            await signUp(
                formData.get('nickname'),
                formData.get('password'),
                formData.get('confirmPassword'),
                function () {
                    document.getElementById('signUpModal').style.display = 'none';
                    document.forms.signupForm.reset();
                },
                function () {
                    document.querySelector('#signupNickname + span.error').style.display = 'block';
                    document.querySelector('#signupNickname + span.error').textContent = 'Duplicate nickname, choose another one';
                }
            );
        }

        this.disabled = false;

        return false;
    }

    document.getElementById('signinButton').onclick = async function() {
        this.disabled = true;
        let formData = new FormData(document.forms.signinForm);

        await signIn(
            formData.get('nickname'),
            formData.get('password'),
            async function () {
                document.getElementById('signInModal').style.display = 'none';
                document.forms.signinForm.reset();
                await isAuthenticated(onAuthenticated, onNotAuthenticated);
            },
            function () {
                document.querySelector('#signinPassword + span.error').style.display = 'block';
                document.querySelector('#signinPassword + span.error').textContent = 'Incorret login or password';
            }
        );
    
        this.disabled = false;
        return false;
    }

    document.getElementById('changePasswordButton').onclick = async function() {
        this.disabled = true;
        let isValid = true;

        let profileNewPassword = document.getElementById('profileNewPassword');
        let profileConfirmPassword = document.getElementById('profileConfirmPassword');

        let profileNewPasswordError = document.querySelector('#profileNewPassword + span.error');
        let profileConfirmPasswordError = document.querySelector('#profileConfirmPassword + span.error');

        profileNewPasswordError.style.display = 'none';
        profileConfirmPasswordError.style.display = 'none';

        if (profileNewPassword.value !== profileConfirmPassword.value) {
            profileNewPasswordError.style.display = 'block';
            profileNewPasswordError.textContent = 'Passwords do not match';

            profileConfirmPasswordError.style.display = 'block';
            profileConfirmPasswordError.textContent = 'Passwords do not match';

            isValid = false;
        } 
        
        if (isValid) {
            let formData = new FormData(document.forms.changePasswordForm);

            await changePassword(
                formData.get('currentPassword'),
                formData.get('newPassword'),
                formData.get('confirmPassword'),
                function () {
                    document.getElementById('profileModal').style.display = 'none';
                    document.forms.changePasswordForm.reset();
                },
                function () {
                    document.querySelector('#profileCurrentPassword + span.error').style.display = 'block';
                    document.querySelector('#profileCurrentPassword + span.error').textContent = 'Current password is wrong';
                }
            )
        }

        this.disabled = false;
        return false;
    }
}

function initValidation() {
    const INVALID_NICKNAME = 'Nickname length must be between 4 and 12 characters';
    const INVALID_PASSWORD = 'Password must contain at least 8 characters';
    const INVALID_ID = 'Game ID cannot be empty'

    let signupNickname = document.getElementById('signupNickname');
    let signupPassword = document.getElementById('signupPassword');
    let signupConfirmPassword = document.getElementById('signupConfirmPassword');

    let signinNickname = document.getElementById('signinNickname');
    let signinPassword = document.getElementById('signinPassword'); 

    let profileCurrentPassword = document.getElementById('profileCurrentPassword');
    let profileNewPassword = document.getElementById('profileNewPassword');
    let profileConfirmPassword = document.getElementById('profileConfirmPassword');

    let gameId = document.getElementById('gameId');

    let signupNicknameError = document.querySelector('#signupNickname + span.error');
    let signupPasswordError = document.querySelector('#signupPassword + span.error');
    let signupConfirmPasswordError = document.querySelector('#signupConfirmPassword + span.error');

    let signinNicknameError = document.querySelector('#signinNickname + span.error');
    let signinPasswordError = document.querySelector('#signinPassword + span.error');

    let profileCurrentPasswordError = document.querySelector('#profileCurrentPassword + span.error');
    let profileNewPasswordError = document.querySelector('#profileNewPassword + span.error');
    let profileConfirmPasswordError = document.querySelector('#profileConfirmPassword + span.error');

    let gameIdError = document.querySelector('#gameId + span.error');

    let signupButton = document.getElementById('signupButton');
    let signinButton = document.getElementById('signinButton');
    let changePasswordButton = document.getElementById('changePasswordButton');
    let joinGameButton = document.getElementById('joinGameButton');

    signupButton.disabled = true;
    signinButton.disabled = true;
    changePasswordButton.disabled = true;
    joinGameButton.disabled = true;

    validateInput(signupNickname, signupNicknameError, INVALID_NICKNAME, [signupNickname, signupPassword, signupConfirmPassword], signupButton);
    validateInput(signupPassword, signupPasswordError, INVALID_PASSWORD, [signupNickname, signupPassword, signupConfirmPassword], signupButton);
    validateInput(signupConfirmPassword, signupConfirmPasswordError, INVALID_PASSWORD, [signupNickname, signupPassword, signupConfirmPassword], signupButton);

    validateInput(signinNickname, signinNicknameError, INVALID_NICKNAME, [signinNickname, signinPassword], signinButton);
    validateInput(signinPassword, signinPasswordError, INVALID_PASSWORD, [signinNickname, signinPassword], signinButton);

    validateInput(profileCurrentPassword, profileCurrentPasswordError, INVALID_PASSWORD, [profileCurrentPassword, profileNewPassword, profileConfirmPassword], changePasswordButton);
    validateInput(profileNewPassword, profileNewPasswordError, INVALID_PASSWORD, [profileCurrentPassword, profileNewPassword, profileConfirmPassword], changePasswordButton);
    validateInput(profileConfirmPassword, profileConfirmPasswordError, INVALID_PASSWORD, [profileCurrentPassword, profileNewPassword, profileConfirmPassword], changePasswordButton);

    validateInput(gameId, gameIdError, INVALID_ID, [gameId], joinGameButton);
}

function validateInput(input, errorSpan, errorMsg, inputs, submitButton) {
    let onInput = function () {
        input.required = true;
        submitButton.disabled = !formIsValid(inputs);

        if (input.validity.valid) {
            errorSpan.style.display = 'none';
        } else {
            errorSpan.style.display = 'block';
            errorSpan.textContent = errorMsg;
        }
    }

    input.addEventListener('focusout', onInput);
    input.addEventListener('input', onInput);
}

function formIsValid(inputs) {
    let isValid = true;
    inputs.forEach(i => isValid &= i.validity.valid && i.value.length > 0);

    return isValid;
}