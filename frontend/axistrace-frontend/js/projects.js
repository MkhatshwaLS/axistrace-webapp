// This file contains functions for managing projects, including creating, updating, and deleting projects.

document.addEventListener('DOMContentLoaded', function() {
    const projectList = document.getElementById('projects');
    const newProjectButton = document.getElementById('new-project-btn');

    function createProjectCard(project) {
        const article = document.createElement('article');
        article.className = 'project-card';
        article.innerHTML = `
            <div class="project-card-header">
                <h3>${project.name}</h3>
                <span class="status-badge ${project.status === 'Active' ? 'status-active' : project.status === 'Review' ? 'status-needs-review' : 'status-complete'}">${project.status}</span>
            </div>
            <p>${project.description}</p>
            <div class="project-meta">
                <span>${project.tasks} tasks</span>
                <span>${project.deadline}</span>
            </div>
        `;
        return article;
    }

    function renderProjects(projects) {
        if (!projectList) {
            return;
        }

        projectList.innerHTML = '';
        projects.forEach(project => {
            projectList.appendChild(createProjectCard(project));
        });
    }

    function loadProjects() {
        const fallbackProjects = [
            { name: 'Customer Portal', status: 'Active', description: 'Restructuring account management and onboarding workflow.', tasks: 9, deadline: 'Due in 3 days' },
            { name: 'AI Assistant Beta', status: 'Review', description: 'Testing pilot workflows and validating adoption metrics.', tasks: 11, deadline: 'Due today' },
            { name: 'Operations Dashboard', status: 'Complete', description: 'Dashboard refresh shipped with improved KPI visibility.', tasks: 16, deadline: 'Closed' }
        ];

        fetch('/api/projects')
            .then(response => {
                if (!response.ok) {
                    throw new Error('Unable to load projects');
                }
                return response.json();
            })
            .then(data => {
                renderProjects(data.length ? data : fallbackProjects);
            })
            .catch(() => {
                renderProjects(fallbackProjects);
            });
    }

    if (newProjectButton) {
        newProjectButton.addEventListener('click', function() {
            const name = prompt('Enter a new project name:');
            if (!name) {
                return;
            }

            const newProject = {
                name,
                status: 'Active',
                description: 'New project created from the workspace overview.',
                tasks: 0,
                deadline: 'To be scheduled'
            };

            const existingProjects = Array.from(projectList.querySelectorAll('.project-card')).length ? Array.from(projectList.querySelectorAll('.project-card')).map(card => ({
                name: card.querySelector('h3')?.textContent || 'Project',
                status: card.querySelector('.status-badge')?.textContent || 'Active',
                description: card.querySelector('p')?.textContent || 'Project summary',
                tasks: 0,
                deadline: 'New'
            })) : [];

            renderProjects([newProject, ...existingProjects]);
        });
    }

    loadProjects();
});