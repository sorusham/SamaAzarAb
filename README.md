<div align="center">
  <h1>SamaAzarAb: Engineering Document Management System</h1>
  <p>A robust .NET MVC web application for managing engineering documents, revisions, and department workflows.</p>
  <img src="https://img.shields.io/badge/.NET-6.0-blueviolet" alt=".NET 6.0">
  <img src="https://img.shields.io/badge/License-Apache%202.0-brightgreen" alt="Apache 2.0 License">
</div>

## ✨ Features
- 📄 **Document & Revision Management**: Upload, track, and manage engineering documents with revision history.
- 🔔 **Notification System**: Real-time alerts for document updates, revisions, and approvals.
- 🔐 **Authentication & Authorization**: Role-based access control (Admin, User, Designer, Checker, Approver, DCC) using ASP.NET Identity.
- 🏢 **Department & Project Allocation**: Assign projects to departments for streamlined workflows.
- 📎 **File Management**: Secure storage and retrieval for document attachments.
- 🎨 **Themed UI**: Responsive and customizable interface with Bootswatch.
- 📊 **Excel Processing**: Tools for handling Excel-based data.

## 🛠 Tech Stack
<table>
  <tr>
    <th>Category</th>
    <th>Technology</th>
  </tr>
  <tr>
    <td>Backend</td>
    <td>.NET MVC, C#</td>
  </tr>
  <tr>
    <td>Frontend</td>
    <td>Bootswatch, HTML, CSS, JavaScript</td>
  </tr>
  <tr>
    <td>Database</td>
    <td>SQLite (configurable for SQL Server)</td>
  </tr>
  <tr>
    <td>Authentication</td>
    <td>ASP.NET Identity</td>
  </tr>
</table>

## 🚀 Getting Started

### Prerequisites
- .NET SDK 6.0 or later
- SQLite (or another supported database)
- Git

### Installation
1. **Clone the Repository**:
   ```bash
   git clone https://github.com/sorusham/SamaAzarAb.git
   cd SamaAzarAb
   ```
2. **Restore Dependencies**:
   ```bash
   dotnet restore
   ```
3. **Configure Database**:
   - Update `appsettings.json` with your connection string:
     ```json
     "ConnectionStrings": {
       "DefaultConnection": "Data Source=MessageForAzarab.db"
     }
     ```
4. **Apply Migrations**:
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```
5. **Run the Application**:
   ```bash
   dotnet run
   ```
6. Open `https://localhost:5001` in your browser.

### Default Admin Account
- **Email**: `admin@example.com`
- **Password**: `Admin@123`
- **Roles**: Admin, DCC

## 📂 Project Structure
- **`Program.cs`**: Configures services, middleware, and initializes roles/users.
- **`appsettings.json`**: Manages connection strings and logging.
- **`Services`**: Scoped services for Letters, Users, Roles, Files, Attachments, Notifications, Departments, Projects, and Documents.
- **`Models`**: Entities like `ApplicationUser` for identity management.
- **`Data`**: EF Core context (`ApplicationDbContext`) for database operations.

## 🤝 Contributing
1. Fork the repository.
2. Create a branch: `git checkout -b feature-branch`
3. Commit changes: `git commit -m 'Add feature'`
4. Push: `git push origin feature-branch`
5. Open a Pull Request.

## 📜 License
This project is licensed under the [Apache License 2.0](LICENSE).

## 📧 Contact
For questions or feedback, reach out at [sorushmehrad@gmail.com](mailto:sorushmehrad@gmail.com) or open an issue.

<div align="center">
  <p>Built with 💻 and ☕ by <a href="https://github.com/sorusham">Sorush Mehrad</a></p>
</div>
