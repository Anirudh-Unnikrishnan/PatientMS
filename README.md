PatientMS — Patient Management System

A web app for managing patients, doctors, appointments, and medical reports with role-based access control.

Stack: ASP.NET Core MVC (.NET 10) · C# · Entity Framework Core · SQLite · ASP.NET Core Identity

Features


Login system with 3 roles: Admin, Doctor, Receptionist
 Patient & doctor management (CRUD)
 Appointment booking with interactive calendar (FullCalendar)
 Patient report uploads (PDF, JPG, PNG)
 Admin dashboard with live stats

 Getting Started

git clone https://github.com/YOUR-USERNAME/PatientMS.git
cd PatientMS
dotnet restore
dotnet ef database update
dotnet run

Default Login

Role: Admin 
Email:admin@patientms.com
Password:Admin@123

Roles & Access
Role:Admin- everything
Role:Doctor- Availability,Reports
Role:Receptionist- Patients,Bookings
