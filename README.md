# SkillBuilderPro

![SkillBuilderPro Banner](assets/default.png)

> Built for Athletes. Powered by Precision.

MSSA Capstone Project — Cloud Application Development | Cohort PCAD20 | July 2026

[![.NET](https://img.shields.io/badge/.NET-10-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com)
[![C#](https://img.shields.io/badge/C%23-12-239120?style=flat-square&logo=csharp)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-2022-CC2927?style=flat-square&logo=microsoftsqlserver)](https://www.microsoft.com/en-us/sql-server/)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-Web%20API-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/apps/aspnet)
[![WinForms](https://img.shields.io/badge/WinForms-Desktop-512BD4?style=flat-square&logo=windows)](https://github.com/dotnet/winforms)
[![MAUI](https://img.shields.io/badge/MAUI-Mobile-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/maui)
[![Build](https://img.shields.io/badge/Build-Passing-brightgreen?style=flat-square)](https://github.com)

[GitHub](https://github.com/brovy23-GD) • [LinkedIn](https://www.linkedin.com/in/bobby-rovy/) • [Email](mailto:brovy23@gmail.com)

---

## Executive Summary

SkillBuilderPro is a full-stack, multi-role athletic development platform built with C#/.NET 10 across four integrated projects: a RESTful Web API backend, WinForms desktop frontend, shared Core library, and MAUI mobile scaffold. It delivers a professional-grade drill library (60 drills, 6 sports), YouTube video integration, multi-role authentication, and analytics dashboards in a single cohesive solution.

---

## Problem / Solution

### The Problem

Athletes at every level — youth through semi-pro — lack a centralized, measurable system for structured training. Coaches assign drills informally, parents have zero visibility, and athletes have no way to track progress or access quality instructional video. The result: inconsistent development, wasted sessions, and missed potential.

### The Solution

SkillBuilderPro replaces fragmented coaching with a precision-engineered platform featuring:

- A seeded drill library (60 drills, 6 sports) with embedded YouTube instruction
- Role-specific dashboards tailored to Athletes, Coaches, Parents, and Admins
- REST API backend powering structured data access and progress tracking
- Enterprise-grade UI with professional brand standards — not a hobby project aesthetic
- Fullstack architecture across desktop, web, and mobile platforms

---

## Features

| Feature | Description | Status |
|---------|-------------|--------|
| **Multi-Role Authentication** | Athlete / Coach / Parent / Admin login with role-based routing | ✅ Complete |
| **Drill Library** | 60 seeded drills across 6 sports with categorization & difficulty levels | ✅ Complete |
| **Video Player** | Embedded YouTube playback via WebView2, full-screen capable, centered layout | ✅ Complete |
| **REST API** | 3 ASP.NET Core controllers (Drils, Users, Progress) on localhost:5000 | ✅ Complete |
| **SQL Server Database** | SkillBuilderDb with EF Core code-first migrations | ✅ Complete |
| **Role Dashboards** | Athlete, Coach, Parent, Admin view-specific dashboards | ✅ Complete |
| **Progress Tracking** | Athlete performance logging with historical analytics | ✅ Complete |
| **Coach Analytics** | Drill completion rates, athlete performance insights | ✅ Complete |
| **Elite UI Design** | Performance Blue brand palette, dark elite aesthetic, responsive layout | ✅ Complete |
| **MAUI Mobile Client** | Athletes-only mobile drill list + video player (scaffold) | 🚧 Planned |

---

## System Architecture

### Four-Tier Integration

```
┌─────────────────────────────────────────────────────┐
│           PRESENTATION LAYER                        │
│  ┌──────────────────┐      ┌──────────────────┐    │
│  │ WinForms Desktop │      │  MAUI Mobile     │    │
│  │   (4 Dashboards)│      │   (Scaffold)     │    │
│  └──────────────────┘      └──────────────────┘    │
└────────────┬──────────────────────────────┬─────────┘
             │ HTTP REST Calls              │
             └──────────────┬───────────────┘
                            │
         ┌──────────────────▼───────────────────┐
         │     APPLICATION LAYER                │
         │   ASP.NET Core Web API               │
         │   localhost:5000                     │
         │  ┌─────────────────────────────────┐ │
         │  │ DrilsController (60 drills)     │ │
         │  │ UsersController (Auth + Roles)  │ │
         │  │ ProgressController (Tracking)   │ │
         │  └─────────────────────────────────┘ │
         └──────────────────┬────────────────────┘
                            │
         ┌──────────────────▼────────────────────┐
         │      SHARED LAYER                    │
         │   SkillBuilderPro.Core               │
         │  ┌─────────────────────────────────┐ │
         │  │ Models: User, Drill, Progress   │ │
         │  │ Interfaces: IAuthService, IApi  │ │
         │  └─────────────────────────────────┘ │
         └──────────────────┬────────────────────┘
                            │
         ┌──────────────────▼────────────────────┐
         │      DATA LAYER                      │
         │   SQL Server SkillBuilderDb          │
         │  ┌─────────────────────────────────┐ │
         │  │ Users Table (4 roles)           │ │
         │  │ Drills Table (60 drills)        │ │
         │  │ Progress Table (history)        │ │
         │  └─────────────────────────────────┘ │
         └──────────────────────────────────────┘
```

---

## Project 1: WinForms Desktop Client

### Login & Role Router Flow

```
LoginForm (Username + Password)
    ↓
POST /api/users/login
    ↓
API Returns: { userId, role, token }
    ↓
Role Router (Switch Statement)
    ├─ role="Athlete" → AthleteForm (drill assignments, personal progress)
    ├─ role="Coach" → CoachForm (manage athletes, assign drills, analytics)
    ├─ role="Parent" → ParentForm (child progress, drill history)
    └─ role="Admin" → AdminForm (system config, user management)
    ↓
DrillLibraryForm (GET /api/drils → Browse all 60 drills)
    ↓
VideoPlayerForm (WebView2 + YouTube IFrame)
    ↓
Log Completion (POST /api/progress)
```

### WinForms Component Layers

```
UI Layer (Forms & Controls)
    ↓
Service Layer (ApiService, AuthService, DrillService)
    ↓
Model Layer (User, Drill, Progress classes)
    ↓
Web API Endpoints (localhost:5000)
    ↓
EF Core ORM
    ↓
SQL Server Database
```

---

## Project 2: ASP.NET Core Web API

### REST Controllers (3 Total)

**DrilsController**
- GET /api/drils → List all 60 drills (O(n))
- GET /api/drils/{id} → Get drill by ID (O(1))
- GET /api/drils/sport/{sport} → Filter by sport (O(n))

**UsersController**
- POST /api/users/login → Authenticate user, return role (O(1))
- GET /api/users → List all users (O(n))

**ProgressController**
- POST /api/progress → Log completed drill (O(1))
- GET /api/progress/{userId} → Get athlete progress history (O(n))

### API Data Flow

```
Client Request
    ↓
Route to Controller (DrilsController | UsersController | ProgressController)
    ↓
Business Logic (validation, filtering)
    ↓
Entity Framework Core DbContext
    ↓
SQL Query (SELECT | INSERT | UPDATE)
    ↓
SQL Server (SkillBuilderDb)
    ↓
Serialize Result to JSON
    ↓
HTTP Response (200 OK | 201 Created | 400 Bad Request)
    ↓
Client receives JSON
```

---

## Project 3: SkillBuilderPro.Core (Shared)

### Model Dependencies

```
User Model
├─ UserId (PK)
├─ Username
├─ PasswordHash
├─ Email
├─ Role (Athlete | Coach | Parent | Admin)
└─ CreatedDate
    ↓ (1:N relationship)
    ↓
Progress Model
├─ ProgressId (PK)
├─ UserId (FK)
├─ DrillId (FK)
├─ CompletedDate
└─ RepetitionsCompleted

Drill Model
├─ DrillId (PK)
├─ Name
├─ Sport (Basketball, Football, Soccer, Baseball, Volleyball, Tennis)
├─ Description
├─ YoutubeUrl
├─ DifficultyLevel (1-5)
└─ CreatedDate
    ↓ (1:N relationship)
    ↓
Progress Model (tracks drill completion per athlete)
```

**Shared across all 3 projects:** WinForms, API, MAUI

---

## Project 4: MAUI Mobile Client (Scaffold)

### MVVM Architecture

```
View Layer (XAML)
├─ LoginPage.xaml (username, password fields)
├─ DrillListPage.xaml (CollectionView of drills)
└─ VideoPlayerPage.xaml (WebView2 + YouTube)
    ↓
ViewModel Layer (C#, MVVM Toolkit)
├─ LoginViewModel (AuthCommand, handles login)
├─ DrillListViewModel (LoadDrillsCommand, SelectDrillCommand, ObservableCollection)
└─ VideoPlayerViewModel (CurrentDrill, LogProgressCommand)
    ↓
Service Layer (Shared ApiService)
├─ GetDrillsAsync()
├─ LoginAsync()
└─ LogProgressAsync()
    ↓
Web API (localhost:5000)
```

---

## Project Structure

```
SkillBuilderPro/
├── SkillBuilderPro.sln
│
├── SkillBuilderPro.WinForms/
│   ├── Forms/
│   │   ├── LoginForm.cs
│   │   ├── AthleteForm.cs
│   │   ├── CoachForm.cs
│   │   ├── ParentForm.cs
│   │   ├── AdminForm.cs
│   │   ├── DrillLibraryForm.cs
│   │   └── VideoPlayerForm.cs (WebView2 YouTube embed)
│   ├── Services/
│   │   ├── ApiService.cs
│   │   ├── AuthService.cs
│   │   └── DrillService.cs
│   ├── Models/
│   └── Utils/
│
├── SkillBuilderPro.API/
│   ├── Controllers/
│   │   ├── DrilsController.cs
│   │   ├── UsersController.cs
│   │   └── ProgressController.cs
│   ├── Data/
│   │   ├── AppDbContext.cs
│   │   └── Migrations/
│   ├── Program.cs (API startup)
│   └── appsettings.json (connection string)
│
├── SkillBuilderPro.Core/
│   ├── Models/
│   │   ├── User.cs
│   │   ├── Drill.cs
│   │   └── Progress.cs
│   └── Interfaces/
│
├── SkillBuilderPro.MAUI/
│   ├── Views/
│   │   ├── LoginPage.xaml
│   │   ├── DrillListPage.xaml
│   │   └── VideoPlayerPage.xaml
│   ├── ViewModels/
│   │   ├── LoginViewModel.cs
│   │   ├── DrillListViewModel.cs
│   │   └── VideoPlayerViewModel.cs
│   ├── Services/
│   │   └── ApiService.cs
│   └── MauiProgram.cs
│
└── assets/
    ├── default.png (1200×400 elite banner)
    └── screenshots/ (login, dashboards, video player)
```

---

## Technologies

| Layer | Technology | Version | Purpose |
|-------|-----------|---------|---------|
| **Language** | C# | 12 | Primary language |
| **Runtime** | .NET | 10 | Core runtime |
| **Desktop UI** | Windows Forms | .NET 10 | Desktop framework |
| **Mobile UI** | MAUI | Latest | Cross-platform mobile |
| **Backend** | ASP.NET Core Web API | .NET 10 | REST API framework |
| **ORM** | Entity Framework Core | 8.x | Database object mapping |
| **Database** | SQL Server | 2022 | Relational data store |
| **Cloud DB** | Azure SQL | — | Production deployment |
| **Video Embed** | WebView2 | Latest | YouTube integration |
| **HTTP Client** | HttpClient | .NET 10 | REST communication |
| **Version Control** | Git / GitHub | — | Source control |

---

## Installation & Setup

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server 2022 (or LocalDB)
- Visual Studio 2022 (recommended)
- WebView2 Runtime (bundled with Windows 11+)

### Clone & Restore

```bash
git clone https://github.com/brovy23-GD/Skill-Builder-Pro.git
cd SkillBuilderPro
dotnet restore SkillBuilderPro.sln
```

### Database Setup

```bash
cd SkillBuilderPro.API
dotnet ef database update
```

Update `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=SkillBuilderDb;Trusted_Connection=True;"
  }
}
```

### Build

```bash
dotnet build SkillBuilderPro.sln
```

---

## Usage — Running All 4 Projects

### Terminal 1: Start Web API

```bash
cd SkillBuilderPro.API
dotnet run
# API running at http://localhost:5000
```

### Terminal 2: Start WinForms Desktop Client

```bash
cd SkillBuilderPro.WinForms
dotnet run
# Desktop app launches, connects to localhost:5000
```

### Terminal 3: Start MAUI Mobile Client

```bash
cd SkillBuilderPro.MAUI
dotnet run
# Mobile app scaffolds, connects to localhost:5000
```

---

## API Endpoints — Quick Reference

| Method | Endpoint | Purpose | Complexity |
|--------|----------|---------|-----------|
| GET | `/api/drils` | List all 60 drills | O(n) |
| GET | `/api/drils/{id}` | Get drill by ID | O(1) — Primary key lookup |
| GET | `/api/drils/sport/{sport}` | Filter drills by sport | O(n) — Full scan |
| POST | `/api/users/login` | Authenticate user, return role | O(1) |
| GET | `/api/users` | List all users (admin only) | O(n) |
| POST | `/api/progress` | Log completed drill session | O(1) — Insert |
| GET | `/api/progress/{userId}` | Get athlete progress history | O(n) — Query |

---

## Key Concepts — Advanced Patterns

### 1. Multi-Role Architecture (WinForms)

After login, the API returns a role string. WinForms uses a switch statement to instantiate the appropriate dashboard form. This mirrors enterprise Role-Based Access Control (RBAC) patterns.

```csharp
switch(user.Role)
{
    case "Athlete":
        new AthleteForm().Show();
        break;
    case "Coach":
        new CoachForm().Show();
        break;
    // etc.
}
```

### 2. WebView2 Video Integration

The `VideoPlayerForm` uses Microsoft.Web.WebView2 to embed YouTube via an IFrame. This avoids requiring local media codecs while providing professional-grade video instruction for every drill. Full-screen capable.

### 3. EF Core Code-First Migrations

The entire database schema is defined in C# model classes. Running `dotnet ef database update` applies all migrations in sequence and seeds the drill library. Fully reproducible from a clean SQL Server instance in one command.

### 4. REST API Design

The Web API follows REST conventions: noun-based endpoints (`/api/drils`, `/api/progress`), standard HTTP verbs (GET, POST, PUT), and JSON request/response bodies. Extensible architecture supports future resource types.

### 5. MVVM Pattern (MAUI)

MAUI pages bind to ViewModels. ViewModels expose `ObservableCollection` and `RelayCommand` properties. Services handle API calls. When data updates, the UI automatically refreshes via data bindings.

---

## Brand Standards — Locked Design System

| Element | Hex | Usage | Purpose |
|---------|-----|-------|---------|
| **Performance Blue** | `#0078D4` | Primary buttons, brand accent | Signals action, elite tier |
| **Hover Blue** | `#168FE5` | Button hover state | Visual feedback, interactive |
| **Pressed Blue** | `#005A9E` | Button pressed/active state | Depth, active confirmation |
| **Elite Black** | `#0A0F1E` | Application background | Premium, professional, minimal |
| **Charcoal Black** | `#121212` | Panel surfaces, cards | Hierarchy, separation |
| **Soft White** | `#F5F7FA` | Body text, UI text | Readability, contrast |

**Personality:** Elite · Professional · Disciplined · Motivational · Precision-Focused

**Never:** Childish, cartoonish, generic fitness app aesthetic

---

## Interview Talking Points

1. **Problem & Solution:** Replaced fragmented coaching with centralized, measurable athletic platform
2. **Full-Stack Architecture:** 4 projects spanning desktop, web API, mobile, and shared models
3. **Technical Depth:** Multi-role RBAC, REST API design, EF Core migrations, MVVM patterns, WebView2 integration
4. **Scale & Polish:** 60 seeded drills, 4 user roles, YouTube integration, locked brand system, enterprise-grade UI
5. **Versatility:** Desktop (WinForms), backend (ASP.NET Core), mobile (MAUI) — demonstrates cross-platform thinking
6. **Production Quality:** Migrations tracked, code organized, error handling in place, Git versioned

---

## Performance Analysis

### API Endpoint Complexity

| Endpoint | Operation | Big-O | Index |
|----------|-----------|-------|-------|
| GET /api/drils/{id} | Primary key lookup | **O(1)** | PK index on DrillId |
| GET /api/drils/sport/{sport} | Filtered scan | **O(n)** | Sport index (optional) |
| POST /api/progress | Insert with FK | **O(1)** | Auto-increment PK |
| GET /api/progress/{userId} | Range query | **O(n)** | FK index on UserId |

### Role Router

```
Login → API POST (O(1))
     → Deserialize role (O(1))
     → Switch statement (O(1))
     → Form instantiation (O(1))
     Total: O(1) constant time
```

---

## Deployment Ready

- ✅ Solution compiles cleanly
- ✅ All EF Core migrations tested and applied
- ✅ 60 drills seeded with YouTube URLs
- ✅ Brand colors locked and consistent
- ✅ API endpoints verified and documented
- ✅ MVVM pattern implemented (MAUI)
- ✅ Error handling and logging in place
- ✅ Git tracked, committed, and pushed
- ✅ Professional README with architecture diagrams
- ✅ Contact information linked (GitHub, LinkedIn, email)

---

## Author

**Bobby Rovy**

MSSA Graduate — Cloud Application Development | Cohort PCAD20 | July 2026

📍 Oak Lawn, IL

🔗 [GitHub](https://github.com/brovy23-GD)

🔗 [LinkedIn](https://www.linkedin.com/in/bobby-rovy/)

✉️ [brovy23@gmail.com](mailto:brovy23@gmail.com)

---

<div align="center">

**SkillBuilderPro — Built for Athletes. Powered by Precision.**

*A full-stack, multi-role athletic development platform across desktop, web, and mobile.*

*Engineered for performance. Designed for scale.*

</div>
