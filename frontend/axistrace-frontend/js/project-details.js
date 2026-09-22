// This file contains functions for handling the project details page, including loading project-specific data and updating the UI.

document.addEventListener("DOMContentLoaded", function() {
    const projectId = getProjectIdFromUrl();
    loadProjectDetails(projectId);
});

function getProjectIdFromUrl() {
    const urlParams = new URLSearchParams(window.location.search);
    return urlParams.get('id');
}

function loadProjectDetails(projectId) {
    fetch(`https://api.example.com/projects/${projectId}`)
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(data => {
            displayProjectDetails(data);
        })
        .catch(error => {
            console.error('There was a problem with the fetch operation:', error);
        });
}

function displayProjectDetails(project) {
    const projectTitle = document.getElementById('project-title');
    const projectDescription = document.getElementById('project-description');
    const projectStatus = document.getElementById('project-status');

    projectTitle.textContent = project.title;
    projectDescription.textContent = project.description;
    projectStatus.textContent = project.status;
}