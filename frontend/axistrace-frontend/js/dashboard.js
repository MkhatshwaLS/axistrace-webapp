// This file contains functions for managing the dashboard, including loading user data and updating the UI.

document.addEventListener("DOMContentLoaded", function() {
    loadUserData();
});

function loadUserData() {
    const userData = {
        name: "Alex Morgan",
        email: "alex.morgan@axistrace.io",
        projects: [
            { id: 1, title: "Website Redesign", status: "Active" },
            { id: 2, title: "Mobile App Launch", status: "Review" },
            { id: 3, title: "CRM Migration", status: "Complete" }
        ]
    };

    displayUserData(userData);
}

function displayUserData(userData) {
    const userNameElement = document.getElementById("username");
    const projectsContainer = document.getElementById("projects-container");
    const activityList = document.getElementById("activity-list");

    if (userNameElement) {
        userNameElement.textContent = userData.name;
    }

    if (projectsContainer) {
        projectsContainer.innerHTML = userData.projects.map(project => `
            <article class="project-card">
                <div class="project-card-header">
                    <h3>${project.title}</h3>
                    <span class="status-badge ${project.status === 'Active' ? 'status-active' : project.status === 'Review' ? 'status-needs-review' : 'status-complete'}">${project.status}</span>
                </div>
                <p>Progress is moving steadily with the current milestone set.</p>
                <div class="project-meta">
                    <span>${project.status === 'Complete' ? 'Closed' : 'In progress'}</span>
                    <span>Updated today</span>
                </div>
            </article>
        `).join("");
    }

    if (activityList) {
        activityList.innerHTML = `
            <li>
                <div class="activity-item">
                    <strong>Client review sent</strong>
                    <small>Marketing launch plan</small>
                </div>
                <span class="activity-time">2h ago</span>
            </li>
            <li>
                <div class="activity-item">
                    <strong>Milestone updated</strong>
                    <small>Product design sprint</small>
                </div>
                <span class="activity-time">4h ago</span>
            </li>
            <li>
                <div class="activity-item">
                    <strong>Task completed</strong>
                    <small>QA regression pass</small>
                </div>
                <span class="activity-time">Yesterday</span>
            </li>
        `;
    }
}