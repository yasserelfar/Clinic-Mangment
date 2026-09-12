# Clinic Management System

A full-stack ASP.NET Core MVC application for managing a clinic's day-to-day operations, from patient registration to diagnosis, with role-based dashboards for Admin, Doctor, and Reception staff.

## Demo

[Watch the demo video](https://www.linkedin.com/posts/yasser-muhamed-elfar_dotnet-aspnetcore-softwareengineering-ugcPost-7504200971783274498-mS0E/)

## Overview

Clinic Management gives a clinic one place to handle patients, visits, doctors, and specialties instead of paper records and disconnected tools. Reception registers patients and creates visits, the system automatically matches them with an available doctor in the right specialty, and doctors examine patients and record diagnoses directly on screen. Every visit is added to the patient's permanent history, including images, so their full record is available in one place.

## Key Features

- **Role-based access** for Admin, Doctor, and Reception, each with a dedicated dashboard and permissions
- **Patient management**: registration, search by phone, and full visit history with filters (doctor, specialty, status, date range)
- **Automatic doctor matching**: reception selects a specialty and the system lists available doctors
- **Visit lifecycle tracking**: Pending → In Progress → Completed, with timestamps at each stage
- **Full patient history** across all visits, including diagnosis notes and images
- **Admin tools** for managing doctors, specialties, staff accounts, and clinic-wide stats
- **Cookie-based authentication** with role claims (Admin / Doctor / Reception)

## Tech Stack

- **Backend:** ASP.NET Core MVC (.NET), C#
- **Data:** Entity Framework Core, PostgreSQL (Npgsql)
- **Auth:** Cookie authentication with role-based authorization
- **Frontend:** Razor views, Bootstrap, custom "IDH Theme" design system
- **Architecture:** MVC pattern with dependency-injected `DbContext`

## User Roles

| Role | Responsibilities |
|---|---|
| **Admin** | Manage doctors, specialties, and staff accounts; view clinic-wide stats |
| **Reception** | Register patients, create visits, assign specialty and doctor |
| **Doctor** | View pending visits, start examinations, record diagnoses |

## Core Entities

`User` · `Doctor` · `Specialty` · `Patient` · `Visit` · `Diagnosis` · `Prescription`

- A `Doctor` is linked one-to-one with a `User` and belongs to a `Specialty`
- A `Patient` has many `Visits`
- Each `Visit` belongs to a `Patient` and `Doctor`, and produces one `Diagnosis`

## Getting Started

### Prerequisites
- [.NET SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL](https://www.postgresql.org/download/)

### Setup
```bash
git clone <repo-url>
cd clinic-management
```

Update the connection string in `appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Database=ClinicManagement;Username=postgres;Password=yourpassword"
}
```

Apply migrations and run:
```bash
dotnet ef database update
dotnet run
```

On first run, the seeder creates one Specialty and one user per role so you can log in immediately:

| Role | Email | Password |
|---|---|---|
| Admin | admin@clinic.com | Admin123! |
| Doctor | doctor@clinic.com | Doctor123! |
| Reception | reception@clinic.com | Reception123! |

> Change these credentials before deploying to production.

## Project Structure

```
Controllers/    # Account, Admin, Doctors, Patients, Reception, Visits
Models/         # User, Doctor, Specialty, Patient, Visit, Diagnosis, Prescription
ViewModels/     # PatientHistoryViewModel
Views/          # Razor views per controller
Data/           # AppDbContext, DbSeeder
wwwroot/css/    # IDH Theme design system
```

## Roadmap

- Password reset and user onboarding
- Visit cancellation flow
- Async EF Core operations
- Service layer for shared business logic
- Automated tests
