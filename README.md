# SkillBuilderPro

**By Bobby Rovy**

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

```mermaid
graph TB
    subgraph "Presentation Layer"
        WF["WinForms Desktop Client<br/>(SkillBuilderPro.WinForms)"]
        MAUI["MAUI Mobile Client<br/>(SkillBuilderPro.MAUI)"]
    end
    
    subgraph "Application Layer"
        API["ASP.NET Core Web API<br/>(SkillBuilderPro.API)<br/>localhost:5000"]
    end
    
    subgraph "Shared Layer"
        CORE["SkillBuilderPro.Core<br/>Models & Contracts"]
    end
    
    subgraph "Data Layer"
        DB["SQL Server<br/>SkillBuilderDb<br/>60 Drills, 4 Roles"]
    end
    
    WF -->|HTTP REST| API
    MAUI -->|HTTP REST| API
    
    WF -.->|References| CORE
    MAUI -.->|References| CORE
    API -.->|References| CORE
    
    API -->|EF Core ORM| DB
    
    style WF fill:#0078D4,stroke:#F5F7FA,color:#F5F7FA
    style MAUI fill:#0078D4,stroke:#F5F7FA,color:#F5F7FA
    style API fill:#168FE5,stroke:#F5F7FA,color:#F5F7FA
    style CORE fill:#121212,stroke:#0078D4,color:#F5F7FA
    style DB fill:#005A9E,stroke:#F5F7FA,color:#F5F7FA
```

---

## Project 1: SkillBuilderPro.WinForms — Desktop Client

### Role Router & Dashboard Architecture

```mermaid
graph TD
    LF["LoginForm<br/>Username + Password"]
    
    LF -->|POST /api/users/login| API["Web API<br/>Authenticate User"]
    
    API -->|Returns User + Role| RR["Role Router<br/>Switch Statement"]
    
    RR -->|role=Athlete| AF["AthleteForm<br/>Drill Assignments<br/>Personal Progress"]
    RR -->|role=Coach| CF["CoachForm<br/>Manage Athletes<br/>Assign Drills<br/>View Analytics"]
    RR -->|role=Parent| PF["ParentForm<br/>Child Progress<br/>Drill History"]
    RR -->|role=Admin| ADMF["AdminForm<br/>System Config<br/>User Management"]
    
    AF --> DLF["DrillLibraryForm<br/>GET /api/drils<br/>Browse All 60 Drills"]
    CF --> DLF
    PF --> DLF
    ADMF --> DLF
    
    DLF --> VP["VideoPlayerForm<br/>WebView2 + YouTube IFrame"]
    
    VP -->|POST /api/progress| PROG["Log Completion<br/>Update SQL"]
    
    style LF fill:#0078D4,color:#F5F7FA
    style API fill:#168FE5,color:#F5F7FA
    style RR fill:#0078D4,color:#F5F7FA
    style AF fill:#121212,stroke:#0078D4,color:#F5F7FA
    style CF fill:#121212,stroke:#0078D4,color:#F5F7FA
    style PF fill:#121212,stroke:#0078D4,color:#F5F7FA
    style ADMF fill:#121212,stroke:#0078D4,color:#F5F7FA
    style DLF fill:#121212,stroke:#0078D4,color:#F5F7FA
    style VP fill:#0078D4,color:#F5F7FA
    style PROG fill:#168FE5,color:#F5F7FA
```

### WinForms Component Interaction

```mermaid
graph LR
    UI["UI Layer<br/>Forms & Controls"]
    SERVICE["Service Layer<br/>ApiService<br/>AuthService<br/>DrillService"]
    MODEL["Model Layer<br/>User<br/>Drill<br/>Progress"]
    API["Web API<br/>Endpoints"]
    
    UI -->|Calls| SERVICE
    SERVICE -->|Uses| MODEL
    MODEL -->|Serializes| API
    API -->|Returns| MODEL
    MODEL -->|Binds| UI
    
    style UI fill:#0078D4,color:#F5F7FA
    style SERVICE fill:#168FE5,color:#F5F7FA
    style MODEL fill:#121212,stroke:#0078D4,color:#F5F7FA
    style API fill:#005A9E,color:#F5F7FA
```

---

## Project 2: SkillBuilderPro.API — Web API Backend

### REST Controller Architecture

### REST Controller Architecture

```mermaid
graph TB
    CLIENT["Client Request<br/>WinForms / MAUI"]
    
    CLIENT -->|GET /api/drils| DC["DrilsController"]
    CLIENT -->|POST login| UC["UsersController"]
    CLIENT -->|POST progress| PC["ProgressController"]
    
    DC -->|EF Core| DB["SQL Server<br/>SkillBuilderDb"]
    UC -->|EF Core| DB
    PC -->|EF Core| DB
    
    DB -->|JSON| CLIENT
    
    style CLIENT fill:#0078D4,color:#F5F7FA
    style DC fill:#168FE5,color:#F5F7FA
    style UC fill:#168FE5,color:#F5F7FA
    style PC fill:#168FE5,color:#F5F7FA
    style DB fill:#005A9E,color:#F5F7FA
```

### API Data Flow — Request/Response Cycle

### API Data Flow

```mermaid
graph LR
    A["Request<br/>GET /api/drils"]
    B["DrilsController<br/>Route"]
    C["Entity Framework<br/>Query Builder"]
    D["SQL Server<br/>Execute"]
    E["Serialize<br/>JSON"]
    F["Response<br/>200 OK"]
    
    A --> B --> C --> D --> E --> F
    
    style A fill:#0078D4,color:#F5F7FA
    style B fill:#168FE5,color:#F5F7FA
    style C fill:#121212,stroke:#0078D4,color:#F5F7FA
    style D fill:#005A9E,color:#F5F7FA
    style E fill:#168FE5,color:#F5F7FA
    style F fill:#0078D4,color:#F5F7FA
```
How to use it:
Copy the text above (everything between the lines)
Go to GitHub → Edit README.md
Find the section "### REST Controller Architecture"
Replace it with the code above
Commit

That's it. ✅

---

## Project 3: SkillBuilderPro.Core — Shared Models & Contracts

### Model Dependency Graph

```mermaid
graph TB
    USER["User Model<br/>Id, Username<br/>PasswordHash, Role<br/>Email"]
    
    DRILL["Drill Model<br/>Id, Name, Sport<br/>Description<br/>YoutubeUrl<br/>DifficultyLevel"]
    
    PROGRESS["Progress Model<br/>Id, UserId<br/>DrillId, CompletedDate<br/>RepetitionsCompleted"]
    
    USER -->|1:N| PROGRESS
    DRILL -->|1:N| PROGRESS
    
    USER -->|References in| WF["WinForms<br/>LoginForm"]
    DRILL -->|References in| WF
    PROGRESS -->|References in| WF
    
    USER -->|References in| API["Web API<br/>Controllers"]
    DRILL -->|References in| API
    PROGRESS -->|References in| API
    
    USER -->|References in| MAUI["MAUI Client<br/>ViewModels"]
    DRILL -->|References in| MAUI
    PROGRESS -->|References in| MAUI
    
    style USER fill:#0078D4,color:#F5F7FA
    style DRILL fill:#0078D4,color:#F5F7FA
    style PROGRESS fill:#0078D4,color:#F5F7FA
    style WF fill:#121212,stroke:#0078D4,color:#F5F7FA
    style API fill:#121212,stroke:#168FE5,color:#F5F7FA
    style MAUI fill:#121212,stroke:#0078D4,color:#F5F7FA
```

---

## Project 4: SkillBuilderPro.MAUI — Mobile Client (Scaffold)

### MVVM Architecture — Mobile

```mermaid
graph TB
    subgraph "View Layer"
        LP["LoginPage.xaml<br/>Username field<br/>Password field"]
        DLP["DrillListPage.xaml<br/>CollectionView<br/>Drill list binding"]
        VP["VideoPlayerPage.xaml<br/>WebView2<br/>YouTube embed"]
    end
    
    subgraph "ViewModel Layer"
        LVM["LoginViewModel<br/>Username property<br/>AuthCommand"]
        DLVM["DrillListViewModel<br/>ObservableCollection<br/>LoadDrillsCommand"]
        VVM["VideoPlayerViewModel<br/>CurrentDrill<br/>LogProgressCommand"]
    end
    
    subgraph "Model & Service Layer"
        MODEL["Models<br/>User, Drill<br/>Progress"]
        SERVICE["ApiService<br/>GetDrillsAsync<br/>LoginAsync<br/>LogProgressAsync"]
    end
    
    subgraph "Data Layer"
        API["Web API<br/>localhost:5000"]
    end
    
    LP -->|Binding| LVM
    DLP -->|Binding| DLVM
    VP -->|Binding| VVM
    
    LVM -->|Calls| SERVICE
    DLVM -->|Calls| SERVICE
    VVM -->|Calls| SERVICE
    
    SERVICE -->|Uses| MODEL
    SERVICE -->|HTTP Requests| API
    
    style LP fill:#0078D4,color:#F5F7FA
    style DLP fill:#0078D4,color:#F5F7FA
    style VP fill:#0078D4,color:#F5F7FA
    style LVM fill:#168FE5,color:#F5F7FA
    style DLVM fill:#168FE5,color:#F5F7FA
    style VVM fill:#168FE5,color:#F5F7FA
    style MODEL fill:#121212,stroke:#0078D4,color:#F5F7FA
    style SERVICE fill:#121212,stroke:#168FE5,color:#F5F7FA
    style API fill:#005A9E,color:#F5F7FA
```

---

## Database Schema — SQL Server

### Entity Relationship Diagram

```mermaid
erDiagram
    USERS ||--o{ PROGRESS : logs
    DRILLS ||--o{ PROGRESS : contains
    
    USERS {
        int UserId PK
        string Username
        string PasswordHash
        string Email
        string Role
        datetime CreatedDate
    }
    
    DRILLS {
        int DrillId PK
        string Name
        string Sport
        string Description
        string YoutubeUrl
        int DifficultyLevel
        datetime CreatedDate
    }
    
    PROGRESS {
        int ProgressId PK
        int UserId FK
        int DrillId FK
        datetime CompletedDate
        int RepetitionsCompleted
    }
```

---

## Full Solution Data Flow

### End-to-End: Athlete Logs a Completed Drill

```mermaid
graph LR
    A["Athlete<br/>Clicks Log Drill<br/>VideoPlayerForm"]
    
    B["WinForms Service<br/>ProgressService<br/>.LogDrillAsync"]
    
    C["HTTP POST<br/>localhost:5000<br/>/api/progress"]
    
    D["Web API<br/>ProgressController<br/>.Post"]
    
    E["EF Core<br/>Add to DbSet<br/>SaveChangesAsync"]
    
    F["SQL Server<br/>INSERT Progress<br/>SkillBuilderDb"]
    
    G["Return 201 Created<br/>to WinForms"]
    
    H["Update UI<br/>Show Confirmation<br/>Refresh Stats"]
    
    A --> B --> C --> D --> E --> F --> G --> H
    
    style A fill:#0078D4,color:#F5F7FA
    style B fill:#168FE5,color:#F5F7FA
    style C fill:#0078D4,color:#F5F7FA
    style D fill:#168FE5,color:#F5F7FA
    style E fill:#121212,stroke:#0078D4,color:#F5F7FA
    style F fill:#005A9E,color:#F5F7FA
    style G fill:#168FE5,color:#F5F7FA
    style H fill:#0078D4,color:#F5F7FA
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
│   ├── Program.cs
│   └── appsettings.json
│
├── SkillBuilderPro.Core/
│   ├── Models/
│   │   ├── User.cs
│   │   ├── Drill.cs
│   │   └── Progress.cs
│   └── Interfaces/
│
└── SkillBuilderPro.MAUI/
    ├── Views/
    │   ├── LoginPage.xaml
    │   ├── DrillListPage.xaml
    │   └── VideoPlayerPage.xaml
    ├── ViewModels/
    │   ├── LoginViewModel.cs
    │   ├── DrillListViewModel.cs
    │   └── VideoPlayerViewModel.cs
    ├── Services/
    │   └── ApiService.cs
    └── MauiProgram.cs
```

---

## Technologies

| Layer | Technology | Version | Purpose |
|-------|-----------|---------|---------|
| Language | C# | 12 | Primary language |
| Runtime | .NET | 10 | Application runtime |
| Desktop UI | Windows Forms | .NET 10 | Desktop framework |
| Mobile UI | MAUI | Latest | Cross-platform mobile |
| Backend | ASP.NET Core Web API | .NET 10 | REST API framework |
| ORM | Entity Framework Core | 8.x | Database access |
| Database | SQL Server | 2022 | Data store |
| Video Embed | WebView2 | Latest | YouTube integration |

---

## Installation & Setup

### Prerequisites

- .NET 10 SDK
- SQL Server 2022 (or LocalDB)
- Visual Studio 2022
- WebView2 Runtime

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
```

### Terminal 3: Start MAUI Mobile Client

```bash
cd SkillBuilderPro.MAUI
dotnet run
```

---

## API Endpoints — Quick Reference

| Method | Endpoint | Purpose | Complexity |
|--------|----------|---------|-----------|
| GET | /api/drils | List all 60 drills | O(n) |
| GET | /api/drils/{id} | Get drill by ID | O(1) |
| GET | /api/drils/sport/{sport} | Filter by sport | O(n) |
| POST | /api/users/login | Authenticate user | O(1) |
| GET | /api/users | List all users | O(n) |
| POST | /api/progress | Log completion | O(1) |
| GET | /api/progress/{userId} | Get history | O(n) |

---

## Key Concepts

**Multi-Role Architecture:** After login, API returns role. WinForms switches to appropriate dashboard form using role-based routing.

**WebView2 Video Integration:** Embeds YouTube IFrame without local codec dependencies. Full-screen capable for professional instruction.

**EF Core Code-First Migrations:** Entire database schema defined in C# models. Fully reproducible via migrations in one command.

**REST API Design:** Noun-based endpoints, standard HTTP verbs, JSON responses. Extensible for future resources.

**MVVM Pattern (MAUI):** Views bind to ViewModels. Services call API. UI updates automatically via ObservableCollection.

---

## Brand Standards — Locked Design System

| Color | Hex | Usage |
|-------|-----|-------|
| Primary Blue | #0078D4 | Buttons, accents |
| Hover Blue | #168FE5 | Button hover state |
| Pressed Blue | #005A9E | Button pressed state |
| Elite Black | #0A0F1E | App background |
| Charcoal | #121212 | Panels, surfaces |
| Soft White | #F5F7FA | Body text |

**Personality:** Elite · Professional · Disciplined · Motivational · Precision-Focused

**Never:** Childish, cartoonish, generic fitness app look

---

## Interview Talking Points

1. **Problem & Solution:** Replaced fragmented coaching with centralized, measurable athletic platform
2. **Full-Stack Architecture:** 4 integrated projects spanning desktop (WinForms), web API, mobile (MAUI), and shared core
3. **Technical Depth:** Multi-role RBAC, REST API design, EF Core migrations, MVVM patterns, WebView2 integration
4. **Scale & Polish:** 60 seeded drills, 4 user roles, YouTube video integration, locked brand system, enterprise-grade UI
5. **Versatility:** Desktop, backend, and mobile platforms — demonstrates cross-platform thinking
6. **Production Quality:** Migrations tracked, code organized, error handling, Git versioned

---

## Performance Analysis

### API Endpoint Complexity

| Endpoint | Operation | Big-O | Index |
|----------|-----------|-------|-------|
| GET /api/drils/{id} | Primary key lookup | O(1) | PK index |
| GET /api/drils/sport/{sport} | Filtered scan | O(n) | Sport index |
| POST /api/progress | Insert with FK | O(1) | Auto-increment PK |
| GET /api/progress/{userId} | Range query | O(n) | FK index |

---

## Deployment Ready

- ✅ Solution compiles cleanly
- ✅ All EF Core migrations tested
- ✅ 60 drills seeded with YouTube URLs
- ✅ Brand colors locked and consistent
- ✅ API endpoints verified and documented
- ✅ MVVM pattern implemented
- ✅ Error handling in place
- ✅ Git tracked and pushed
- ✅ Professional README with diagrams
- ✅ Contact information linked

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
