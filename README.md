# StudentRegistrationApp (SQL Express + Dashboard-style UI)

- ASP.NET Core MVC (.NET 8)
- Entity Framework Core with SQL Server provider
- Database: MS SQL Express (Server=.\SQLEXPRESS; Database=StudentRegistrationDb)
- Features:
  - Register students
  - Define courses
  - Map students to multiple courses
  - Modern, card-based layout inspired by course dashboards

To run:

```bash
dotnet restore
dotnet run
```

Ensure SQL Server Express is installed and the SQLEXPRESS instance is running.
