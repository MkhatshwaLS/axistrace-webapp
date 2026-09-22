// This file contains functions related to user authentication, such as login and logout processes.

const apiUrl = 'http://localhost:5000/api'; // Adjust the API URL as needed

// Function to handle user login
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
        localStorage.setItem('token', data.token); // Store the token in local storage
        window.location.href = 'pages/dashboard.html'; // Redirect to dashboard
    } catch (error) {
        alert(error.message);
    }
}

// Function to handle user logout
function logout() {
    localStorage.removeItem('token'); // Remove the token from local storage
    window.location.href = 'index.html'; // Redirect to the main page
}

// Function to check if the user is authenticated
function isAuthenticated() {
    return localStorage.getItem('token') !== null;
}

// Function to get the current user's information
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