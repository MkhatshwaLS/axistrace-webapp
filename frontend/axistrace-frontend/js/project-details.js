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
    const fallbackProject = {
        title: 'Customer Portal',
        description: 'Customer account management and onboarding workflow redesign with a stronger self-serve experience.',
        status: 'Active',
        startDate: '2026-08-01',
        endDate: '2026-10-15',
        owner: 'Maya Chen'
    };

    const projectTitle = document.getElementById('project-title');
    const projectDescription = document.getElementById('project-description');
    const projectStatus = document.getElementById('status');
    const startDate = document.getElementById('start-date');
    const endDate = document.getElementById('end-date');
    const owner = document.getElementById('owner');

    if (!projectTitle || !projectDescription || !projectStatus) {
        return;
    }

    const renderProject = (project) => {
        projectTitle.textContent = project.title || fallbackProject.title;
        projectDescription.textContent = project.description || fallbackProject.description;
        projectStatus.textContent = project.status || fallbackProject.status;
        projectStatus.className = 'status-badge ' + (
            project.status === 'Active' ? 'status-active' :
            project.status === 'Review' ? 'status-needs-review' : 'status-complete'
        );

        if (startDate) startDate.textContent = project.startDate || fallbackProject.startDate;
        if (endDate) endDate.textContent = project.endDate || fallbackProject.endDate;
        if (owner) owner.textContent = project.owner || fallbackProject.owner;
    };

    if (!projectId) {
        renderProject(fallbackProject);
        return;
    }

    fetch(`/api/projects/${projectId}`)
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(data => {
            renderProject({
                title: data.name || data.title || fallbackProject.title,
                description: data.description || fallbackProject.description,
                status: data.status || fallbackProject.status,
                startDate: data.startDate || fallbackProject.startDate,
                endDate: data.endDate || fallbackProject.endDate,
                owner: data.owner || fallbackProject.owner
            });
        })
        .catch(() => {
            renderProject(fallbackProject);
        });
}