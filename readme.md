# Customer Support Ticket System

## Project Overview
A robust, decoupled Customer Support Ticket System featuring a Windows Desktop Application (Frontend) and an ASP.NET Web API (Backend). The system allows standard users to submit and track support tickets, while granting Administrators the ability to assign tickets, update statuses, and log internal comments. The desktop client communicates exclusively with the MySQL database via RESTful JSON HTTP requests, ensuring strict adherence to multi-tier architecture principles.

## Tech Stack Used
* **Frontend:** C# WPF (Windows Presentation Foundation)
* **UI Framework:** MaterialDesignInXAML
* **Backend:** ASP.NET Core Web API (RESTful)
* **Database:** MySQL
* **ORM:** Entity Framework Core (EF Core)
* **Authentication:** JWT (JSON Web Tokens) for secure session management


## Steps to Run the Project Locally

### 1. Database Setup
1. Open your MySQL server (via Workbench or CLI).
2. Execute the provided `SupportTicketDB_Script.sql` file to create the schema and populate the initial test data.
3. Verify that the `Users`, `Tickets`, `TicketComments`, and `TicketStatusHistory` tables are successfully created.

### 2. Backend Setup (ASP.NET Web API)
1. Open the Web API project in Visual Studio.
2. Open `appsettings.json` and update the `DefaultConnection` string with your local MySQL credentials (User/Password).
3. Run the API project and note the localhost port it runs on (e.g., `https://localhost:7123`).

### 3. Frontend Setup (WPF Desktop App)
1. Open the WPF Desktop project in Visual Studio.
2. Locate the `Session.cs` file (or API Service class) and ensure the `BaseAddress` matches the exact port your Web API is running on.
3. Build and Run the WPF application.

## Assumptions & Design Decisions
* **Decoupled Architecture:** Strict separation of concerns was maintained. The WPF client has zero direct connection to MySQL; all data flows asynchronously through the Web API.
* **Security:** Implemented JWT-based authentication. User roles (Admin vs. User) are securely verified at the API endpoint level to prevent unauthorized access to administrative actions.
* **Dynamic UI:** Hardcoding was avoided. Dropdowns (like the Admin Assignment list) are dynamically populated via API endpoints fetching real-time database records.
* **Modern UX:** Bypassed standard Win32 dialogs and basic styling in favor of asynchronous, non-blocking UI threads and a custom-built Material Design interface with floating modal alerts.
