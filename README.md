# PMRS · Patient Medical Records System

A web app for a small clinic to manage patients, doctors, appointments, medical records and prescriptions in one place. Built with ASP.NET Core MVC on .NET 9.

## Features

- **Patients:** register, search, edit and view full patient profiles with address and blood type
- **Doctors:** keep a list of doctors with their specialisation and licence number
- **Appointments:** book, reschedule and track each visit from scheduled to confirmed, in progress, completed, cancelled or no show
- **Medical records:** consultations, lab results, imaging and discharge notes on each patient's history
- **Prescriptions:** write prescriptions with multiple medicine items
- **Dashboard:** totals for patients, doctors, today's and pending appointments, plus recent patients, appointments and prescriptions
- **Login and roles:** ASP.NET Core Identity with Admin and Receptionist roles
- **Search and pagination** on the main lists

## Tech stack

| Layer | Technology |
| --- | --- |
| Framework | ASP.NET Core MVC (.NET 9) |
| Data | Entity Framework Core |
| Database | SQLite by default, SQL Server supported |
| Auth | ASP.NET Core Identity (cookie auth) |
| UI | Razor views |

## Project structure

```
Controllers/   request handling for each module
Services/      business logic (patients, doctors, appointments, records, prescriptions, dashboard)
Entities/      EF Core models with shared audit fields
Data/          DbContext and the seed data
ViewModels/    form models for login, register and patient forms
Views/         Razor pages
```

## Getting started

You need the [.NET 9 SDK](https://dotnet.microsoft.com/download).

```bash
git clone https://github.com/ManasvibaRana/PMRS.git
cd PMRS
dotnet run
```

Then open the URL shown in the terminal. The SQLite database `pmrs.db` is created and filled with sample data on the first run.

### Demo logins

| Role | Email | Password |
| --- | --- | --- |
| Admin | admin@pmrs.com | Admin@123 |
| Receptionist | reception@pmrs.com | Reception@123 |

These accounts are for local testing only.

### Using SQL Server

Change `ConnectionStrings:DefaultConnection` in `appsettings.json` to a SQL Server connection string. The app picks the provider from the connection string automatically.
