# AxisTrace Frontend

## Overview
AxisTrace Frontend is a web application that provides a user interface for managing projects, user authentication, and displaying user-specific information. This application is built using basic HTML, CSS, and JavaScript.

## Project Structure
```
axistrace-frontend
├── index.html          # Main entry point of the application
├── pages               # Contains different pages of the application
│   ├── login.html      # Login page for user authentication
│   ├── dashboard.html   # Dashboard page displaying user information
│   ├── projects.html    # Projects page for managing user projects
│   └── project-details.html # Detailed view of a specific project
├── css                 # Contains stylesheets
│   ├── styles.css      # Main styles for the application
│   └── responsive.css   # Responsive styles for various screen sizes
├── js                  # Contains JavaScript files
│   ├── api.js          # Functions for API calls to the backend
│   ├── auth.js         # Functions related to user authentication
│   ├── dashboard.js     # Functions for managing the dashboard
│   ├── projects.js      # Functions for managing projects
│   └── project-details.js # Functions for handling project details
└── README.md           # Documentation for the project
```

## Setup Instructions
1. Clone the repository to your local machine.
2. Open the `index.html` file in your web browser to view the application.
3. Ensure that you have a backend service running to handle API requests.

## Usage
- Navigate to the login page to authenticate users.
- Once logged in, users can access the dashboard to view their information.
- Users can manage their projects through the projects page.
- Click on a project to view detailed information on the project details page.

## Contributing
Feel free to submit issues or pull requests for improvements or bug fixes. 

## License
This project is licensed under the MIT License.