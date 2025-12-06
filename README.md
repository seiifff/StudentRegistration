# StudentRegistrationApp  
(SQL Express + ASP.NET MVC + Dashboard UI)

A modern, user-friendly Student Registration Web Application built using **ASP.NET Core MVC**, **SQL Express**, and **Entity Framework Core**.  
The system allows students to register, log in, enroll in courses, view upcoming lessons, manage profiles, and interact with a clean dashboard-style UI.

---

## ✨ Features

### 👤 Student Management
- Student registration & login  
- Secure password hashing (SHA-256)  
- Profile page with student info  
- Ability to edit email and password  
- Profile avatar displayed on dashboard  

### 📚 Course Enrollment System
- View all available courses  
- Enroll or un-enroll from multiple courses  
- “My Courses” dashboard widget  
- Automatic upcoming lessons section  

### 🖥 Dashboard UI (Modern Design)
- Four main dashboard cards:
  - **Profile**
  - **Total Lessons**
  - **Quick Study Tools / Upcoming Exams**
  - **Overview**
- Floating card effects  
- Light mode interface  
- Sidebar navigation  
- Clean typography and spacing  

### 🗄 Database (SQL Express)
- Students table  
- Courses table  
- StudentCourses (Many-to-Many)  
- Migrations enabled  
- Auto-seeding sample data  

---

## 🏗️ Tech Stack

| Layer | Technology |
|-------|------------|
| Backend | ASP.NET Core MVC 7 |
| Database | SQL Server Express |
| ORM | Entity Framework Core |
| Frontend | Razor Views, HTML, CSS |
| Authentication | Custom login (SHA-256 hashing) |
| Hosting | Local development / IIS-ready |
| Version Control | Git + GitHub |

---

## 📁 Project Structure
StudentRegistrationApp/
├── Controllers/
│ ├── AccountController.cs
│ ├── CoursesController.cs
│ └── StudentsController.cs
│
├── Models/
│ ├── Student.cs
│ ├── Course.cs
│ └── StudentCourse.cs
│
├── Views/
│ ├── Account/
│ ├── Courses/
│ ├── Students/
│ └── Shared/
│
├── wwwroot/
│ ├── css/
│ └── images/
│
├── Program.cs
├── appsettings.json
├── StudentRegistrationApp.csproj
└── DatabaseSchema.sql

