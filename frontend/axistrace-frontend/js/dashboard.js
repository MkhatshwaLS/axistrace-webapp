// This file contains functions for managing the dashboard, including loading user data and updating the UI.

document.addEventListener("DOMContentLoaded", function() {
    loadUserData();
});

function loadUserData() {
    // Simulate an API call to fetch user data
    const userData = {
        name: "John Doe",
        email: "john.doe@example.com",
        projects: [
            { id: 1, title: "Project A", status: "In Progress" },
            { id: 2, title: "Project B", status: "Completed" }
        ]
    };

    displayUserData(userData);
}

function displayUserData(userData) {
    const userNameElement = document.getElementById("user-name");
    const userEmailElement = document.getElementById("user-email");
    const projectsListElement = document.getElementById("projects-list");

    userNameElement.textContent = userData.name;
    userEmailElement.textContent = userData.email;

    userData.projects.forEach(project => {
        const projectItem = document.createElement("li");
        projectItem.textContent = `${project.title} - ${project.status}`;
        projectsListElement.appendChild(projectItem);
    });
}