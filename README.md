Engineering Document Management System
A robust .NET MVC web application for managing engineering documents, revisions, notifications, and department-specific project assignments. Built with a focus on scalability, security, and user-friendly UI using Bootswatch themes.
Features

Document & Revision Management: Upload, track, and manage engineering documents with revision history.
Notification System: Real-time alerts for document updates, revisions, and approvals.
User Authentication & Authorization: Role-based access control with ASP.NET Identity (Admin, User, Designer, Checker, Approver, DCC roles).
Department & Project Allocation: Assign projects to specific departments for streamlined workflows.
File Management: Secure file storage and retrieval for document attachments.
Themed UI: Responsive and customizable interface using Bootswatch.
Excel Processing: Tools for handling Excel-based data within the system.

Tech Stack

Backend: .NET MVC, C#
Frontend: Bootswatch, HTML, CSS, JavaScript
Database: SQLite (configurable for other databases like SQL Server)
Authentication: ASP.NET Identity
Services: Scoped services for Letters, Users, Roles, Files, Attachments, Notifications, Departments, Projects, and Documents
Environment: Configured for development and production

Prerequisites

.NET SDK 6.0 or later
SQLite (or another supported database)
Git (for cloning the repository)
Visual Studio or any IDE supporting .NET development

Getting Started

Clone the Repository:git clone https://github.com/your-username/your-repo.git


Navigate to Project Directory:cd your-repo


Restore Dependencies:dotnet restore


Configure Database:
Update the connection string in appsettings.json if using a database other than SQLite:"ConnectionStrings": {
  "DefaultConnection": "Data Source=MessageForAzarab.db"
}




Apply Migrations:dotnet ef migrations add InitialCreate
dotnet ef database update


Run the Application:dotnet run


Open your browser and navigate to https://localhost:5001.

Default Admin Account

Email: admin@example.com
Password: Admin@123
Roles: Admin, DCC

Project Structure

Program.cs: Configures services, middleware, and initializes roles/users.
appsettings.json: Contains connection strings and logging configurations.
Services: Scoped services for managing letters, users, roles, files, attachments, notifications, departments, projects, and documents.
Models: Defines entities like ApplicationUser for identity management.
Data: Entity Framework Core context (ApplicationDbContext) for database operations.

Contributing
Contributions are welcome! Please follow these steps:

Fork the repository.
Create a new branch (git checkout -b feature-branch).
Commit your changes (git commit -m 'Add new feature').
Push to the branch (git push origin feature-branch).
Open a Pull Request.

License
This project is licensed under the MIT License - see the LICENSE file for details.
Contact
For questions or feedback, feel free to open an issue or contact me at [your-email@example.com].
