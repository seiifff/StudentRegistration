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



---

## 🗄️ Database Schema (Tables)

### **Students**
| Column | Type | Notes |
|--------|------|-------|
| Id | int (PK) | Identity |
| FullName | nvarchar | Required |
| Email | nvarchar | Unique |
| PasswordHash | nvarchar | SHA-256 hash |
| RegistrationNumber | nvarchar | Auto-generated |

### **Courses**
| Column | Type | Notes |
|--------|------|-------|
| Id | int (PK) | Identity |
| CourseCode | nvarchar | Unique |
| CourseName | nvarchar | Required |
| Lecturer | nvarchar | Required |
| StartDate | datetime | Required |

### **StudentCourses (Join Table)**
| Column | Type | Notes |
|--------|------|-------|
| Id | int (PK) |
| StudentId | int (FK → Students.Id) |
| CourseId | int (FK → Courses.Id) |

---

## 🚀 How to Run the Project

### 1️⃣ Install Requirements
- .NET 6 or .NET 7 SDK  
- SQL Server Express  
- SQL Server Management Studio (SSMS)  

### 2️⃣ Clone the Repo
```bash
git clone https://github.com/USERNAME/StudentRegistration.git
cd StudentRegistration



Configure Database Connection

In Program.cs or appsettings.json, update:

Server=YOUR_MACHINE\SQLEXPRESS;
Database=StudentRegistrationDb;
Trusted_Connection=True;
TrustServerCertificate=True;

Run the App
dotnet run
