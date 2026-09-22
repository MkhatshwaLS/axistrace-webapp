// This file contains functions for managing projects, including creating, updating, and deleting projects.

document.addEventListener('DOMContentLoaded', function() {
    const projectList = document.getElementById('project-list');
    const createProjectForm = document.getElementById('create-project-form');

    // Fetch and display projects
    function fetchProjects() {
        fetch('/api/projects')
            .then(response => response.json())
            .then(data => {
                projectList.innerHTML = '';
                data.forEach(project => {
                    const projectItem = document.createElement('li');
                    projectItem.textContent = project.name;
                    projectItem.appendChild(createEditButton(project.id));
                    projectItem.appendChild(createDeleteButton(project.id));
                    projectList.appendChild(projectItem);
                });
            })
            .catch(error => console.error('Error fetching projects:', error));
    }

    // Create edit button
    function createEditButton(projectId) {
        const editButton = document.createElement('button');
        editButton.textContent = 'Edit';
        editButton.onclick = () => editProject(projectId);
        return editButton;
    }

    // Create delete button
    function createDeleteButton(projectId) {
        const deleteButton = document.createElement('button');
        deleteButton.textContent = 'Delete';
        deleteButton.onclick = () => deleteProject(projectId);
        return deleteButton;
    }

    // Edit project
    function editProject(projectId) {
        const newName = prompt('Enter new project name:');
        if (newName) {
            fetch(`/api/projects/${projectId}`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ name: newName })
            })
            .then(response => {
                if (response.ok) {
                    fetchProjects();
                } else {
                    console.error('Error updating project:', response.statusText);
                }
            });
        }
    }

    // Delete project
    function deleteProject(projectId) {
        if (confirm('Are you sure you want to delete this project?')) {
            fetch(`/api/projects/${projectId}`, {
                method: 'DELETE'
            })
            .then(response => {
                if (response.ok) {
                    fetchProjects();
                } else {
                    console.error('Error deleting project:', response.statusText);
                }
            });
        }
    }

    // Create new project
    createProjectForm.addEventListener('submit', function(event) {
        event.preventDefault();
        const formData = new FormData(createProjectForm);
        const projectName = formData.get('project-name');

        fetch('/api/projects', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ name: projectName })
        })
        .then(response => {
            if (response.ok) {
                createProjectForm.reset();
                fetchProjects();
            } else {
                console.error('Error creating project:', response.statusText);
            }
        });
    });

    // Initial fetch of projects
    fetchProjects();
});