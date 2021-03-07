async function getAccessToken() {
    let accessToken = window.localStorage.getItem('accessToken');

    if (accessToken == null) return null;

    let decodedAccessToken = decodeJwt(accessToken);

    if (Date.now() >= decodedAccessToken['exp'] * 1000) {
        let response = await fetch('/api/account/refresh_token', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
        });

        if (response.ok) {
            let result = await response.json();

            if (result['status'] === true) {
                return result['accessToken']
            }

            return null;
        }
    }

    return accessToken;
}

function decodeJwt(token) {
    let base64Url = token.split('.')[1];
    let base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    let jsonPayload = decodeURIComponent(atob(base64).split('').map(function (c) {
        return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
    }).join(''));

    return JSON.parse(jsonPayload);
}

async function isAuthenticated(onSuccess, onFailure) {
    let accessToken = await getAccessToken();

    if (accessToken !== null) {
        let response = await fetch(`/api/account/is_authenticated/${accessToken}`);

        if (response.ok) {
            let result = await response.json();

            if (result['status'] === true) {
                onSuccess(result);
            } else {
                onFailure();
            }
        }
    } else {
        onFailure();
    }
}

async function signUp(nickname, password, confirmPassword, onSuccess, onFailure) {
    let response = await fetch('/api/account/sign_up', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(
            {
                'Nickname': nickname,
                'Password': password,
                'ConfirmPassword': confirmPassword
            }
        )
    })

    if (response.ok) {
        let result = await response.json();

        if (result['status'] === true) {
            onSuccess();
        } else {
            onFailure();
        }
    }
}

async function signIn(nickname, password, onSuccess, onFailure) {
    let response = await fetch('/api/account/sign_in', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(
            {
                'Nickname': nickname,
                'Password': password
            }
        )
    });

    if (response.ok) {
        let result = await response.json();
        if (result['status'] === true) {
            window.localStorage.setItem('accessToken', result['body']['accessToken']);

            onSuccess();
        } else {
            onFailure();
        }
    }
} 

async function signOut(onComplete) {
    removeCookie('refreshToken');
    window.localStorage.removeItem('accessToken');

    let response = await fetch('/api/account/sign_out');

    if (response.ok) {
        onComplete();
    }
}

async function changePassword(currentPassword, newPassword, confirmPassword, onSuccess, onFailure) {
    let response = await fetch('/api/account/change_password', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            'AccessToken': await getAccessToken(),
            'CurrentPassword': currentPassword,
            'NewPassword': newPassword,
            'ConfirmNewPassword': confirmPassword
        })
    });

    if (response.ok) {
        let result = await response.json();

        if (result['status'] === true) {
            onSuccess();
        } else {
            onFailure();
        }
    }
}

function getCookie(name) {
    let value = `; ${document.cookie}`;
    let parts = value.split(`; ${name}=`);
    if (parts.length === 2) {
        return parts.pop().split(';').shift();  
    } else {
        return null;
    }
}

function removeCookie(name) {
    document.cookie = `${name}=""; -1; path=/`;
}