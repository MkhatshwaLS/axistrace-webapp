// This file contains functions related to user authentication, such as login and logout processes.

const apiUrl = 'http://localhost:5000/api';

function getDashboardPath() {
    return window.location.pathname.includes('/pages/') ? 'dashboard.html' : 'pages/dashboard.html';
}

function getHomePath() {
    return window.location.pathname.includes('/pages/') ? '../index.html' : 'index.html';
}

async function login(username, password) {
    try {
        const response = await fetch(`${apiUrl}/auth/login`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({ username, password }),
        });

        if (!response.ok) {
            throw new Error('Login failed. Please check your credentials.');
        }

        const data = await response.json();
        localStorage.setItem('token', data.token || 'demo-token');
        window.location.href = getDashboardPath();
    } catch (error) {
        const errorMessage = document.getElementById('error-message');
        if (errorMessage) {
            errorMessage.textContent = error.message;
            return;
        }

        alert(error.message);
    }
}

async function register(username, email, password, confirmPassword) {
    try {
        if (!username || !email || !password || !confirmPassword) {
            throw new Error('Please fill in all fields.');
        }

        if (password !== confirmPassword) {
            throw new Error('Passwords do not match.');
        }

        const response = await fetch(`${apiUrl}/auth/register`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({ username, email, password }),
        });

        if (!response.ok) {
            throw new Error('Registration failed. Please try again.');
        }

        const data = await response.json();
        localStorage.setItem('token', data.token || 'demo-token');
        window.location.href = getDashboardPath();
    } catch (error) {
        const errorMessage = document.getElementById('error-message');
        if (errorMessage) {
            errorMessage.textContent = error.message;
            return;
        }

        alert(error.message);
    }
}

function logout() {
    localStorage.removeItem('token');
    window.location.href = getHomePath();
}

function isAuthenticated() {
    return localStorage.getItem('token') !== null;
}

async function getCurrentUser() {
    if (!isAuthenticated()) {
        return null;
    }

    try {
        const response = await fetch(`${apiUrl}/auth/me`, {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${localStorage.getItem('token')}`,
            },
        });

        if (!response.ok) {
            throw new Error('Failed to fetch user information.');
        }

        return await response.json();
    } catch (error) {
        console.error(error);
        return null;
    }
}

document.addEventListener('DOMContentLoaded', function() {
    const loginForm = document.getElementById('login-form');
    if (loginForm) {
        loginForm.addEventListener('submit', function(event) {
            event.preventDefault();
            const username = document.getElementById('username')?.value.trim();
            const password = document.getElementById('password')?.value.trim();

            if (!username || !password) {
                const errorMessage = document.getElementById('error-message');
                if (errorMessage) {
                    errorMessage.textContent = 'Please enter both username and password.';
                }
                return;
            }

            login(username, password);
        });
    }

    const registerForm = document.getElementById('register-form');
    if (registerForm) {
        registerForm.addEventListener('submit', function(event) {
            event.preventDefault();
            const username = document.getElementById('register-username')?.value.trim();
            const email = document.getElementById('register-email')?.value.trim();
            const password = document.getElementById('register-password')?.value.trim();
            const confirmPassword = document.getElementById('confirm-password')?.value.trim();

            if (!username || !email || !password || !confirmPassword) {
                const errorMessage = document.getElementById('error-message');
                if (errorMessage) {
                    errorMessage.textContent = 'Please fill in all registration fields.';
                }
                return;
            }

            register(username, email, password, confirmPassword);
        });
    }
});