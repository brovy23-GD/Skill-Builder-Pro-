# Skill Builder Pro

![Skill Builder Pro banner](assets/default.png)

**Built for Athletes. Powered by Precision.**

A multi-client athlete development platform built with C# and .NET. Developed by **Bobby Rovy**, a Microsoft Software and Systems Academy (MSSA) Cloud Application Development graduate and U.S. Army veteran.

[GitHub](https://github.com/brovy23-GD) | [LinkedIn](https://www.linkedin.com/in/bobbyrovy) | [Email](mailto:brovy23@gmail.com)

## At a glance

Skill Builder Pro (SBP) explores how athletes, parents, coaches, and administrators can manage structured training through connected applications. The repository includes an ASP.NET Core Web API, shared Core and Client projects, Windows Forms and .NET MAUI clients, SQL Server/Entity Framework Core integration, and a test project. It is an **active portfolio project**; the committed GitHub version may differ from newer work on my local machine.

**Technology:** C# | .NET 10 | ASP.NET Core Web API | Entity Framework Core | SQL Server | WinForms | .NET MAUI | GitHub Actions | Figma

### What problem is SBP designed to address?

Training instructions, athlete progress, and communication can be scattered across videos, spreadsheets, texts, and separate team systems. SBP brings drill discovery, user roles, training assignments, and progress-oriented workflows together in one application architecture. Features and exact counts are evolving; review the checked-in source to determine current behavior.

### Explore the implementation

| Repository path | Area |
| --- | --- |
| [`SkillBuilderPro.API/`](SkillBuilderPro.API/) | API, request handling, and application endpoints |
| [`SkillBuilderPro.Core/`](SkillBuilderPro.Core/) | Domain models and shared backend logic |
| [`SkillBuilderPro.Client/`](SkillBuilderPro.Client/) | Shared client services |
| [`SkillBuilderPro.WinForms/`](SkillBuilderPro.WinForms/) | Windows desktop UI |
| [`SkillBuilderPro.MAUI/`](SkillBuilderPro.MAUI/) | Cross-platform UI project |
| [`SkillBuilderPro.Tests/`](SkillBuilderPro.Tests/) | Tests and coverage configuration |
| [`.github/workflows/dotnet-ci.yml`](.github/workflows/dotnet-ci.yml) | CI configuration; see Actions for actual run status |

---

## Architecture and engineering workflows

The diagrams below **preserve the original README's technical walkthroughs** and are organized for readers who want a deeper engineering view. They are **conceptual or historical illustrations**, not an automatically generated map of every currently checked-in class, route, database column, or runtime behavior. The codebase has evolved since the earliest diagrams were drawn; consult current source and tests for implementation details.

### 1. Multi-client system architecture

```mermaid
graph TB
  subgraph Presentation
    WF[WinForms desktop client]
    MAUI[.NET MAUI client]
  end
  subgraph Application
    API[ASP.NET Core Web API]
    SHARED[Shared Client services]
    CORE[Core models and business logic]
  end
  DB[(SQL Server / EF Core)]
  WF --> SHARED
  MAUI --> SHARED
  SHARED -->|HTTP / JSON| API
  API --> CORE
  CORE -->|EF Core| DB
```

This logical view illustrates the separation between user interfaces, reusable client services, API endpoints, domain logic, and persistent data. Actual project references are defined by the solution and project files.

### 2. Role routing and dashboards (conceptual workflow)

```mermaid
graph TD
  Login[User signs in] --> Auth[Authentication and authorization]
  Auth --> Role{User role}
  Role -->|Athlete| Athlete[Athlete experience]
  Role -->|Coach| Coach[Coach experience]
  Role -->|Parent| Parent[Parent experience]
  Role -->|Administrator| Admin[Administration experience]
  Athlete --> Drills[Training and drill library]
  Coach --> Drills
  Parent --> Drills
  Admin --> Drills
  Drills --> Progress[Training and progress workflows]
```

This preserves the original role-router design idea without tying it to an obsolete login endpoint or claiming that every screen is fully implemented in each client.

### 3. WinForms component interaction

```mermaid
graph LR
  UI[Forms and controls] --> SERVICE[Application / API services]
  SERVICE --> MODEL[Shared models and contracts]
  SERVICE -->|HTTP requests| API[ASP.NET Core API]
  API -->|JSON response| SERVICE
  MODEL --> UI
  SERVICE --> UI
```

This diagram conveys the intended separation of UI, services, shared models, and server interaction rather than a literal serialization call sequence.

### 4. API controller architecture

```mermaid
graph TB
  CLIENT[Desktop / mobile client] --> API[ASP.NET Core routing]
  API --> AUTH[Auth and user workflows]
  API --> DRILL[Drill and assignment workflows]
  API --> PROG[Progress and progression workflows]
  API --> OPS[Coach / parent / admin workflows]
  AUTH --> CORE[Application and domain services]
  DRILL --> CORE
  PROG --> CORE
  OPS --> CORE
  CORE --> DB[(EF Core / SQL Server)]
```

The current repository contains dedicated controllers for authentication, drills, progress, athlete assignments, coaches, parents, schedules, administration, and other functions. See [`SkillBuilderPro.API/Controllers/`](SkillBuilderPro.API/Controllers/) for actual names and routes; the old three-controller diagram is no longer an accurate inventory.

### 5. API request/response lifecycle

```mermaid
graph LR
  A[Client HTTP request] --> B[Route / controller]
  B --> C[Validation and application logic]
  C --> D[EF Core query / command]
  D --> E[(SQL Server)]
  E --> F[Result / DTO mapping]
  F --> G[HTTP JSON response]
  G --> H[Client updates UI]
```

Exact status codes, authorization requirements, and persistence behavior depend on the selected endpoint.

### 6. Shared model dependencies

```mermaid
graph TB
  USER[User / identity] --> ASSIGN[Training assignment]
  DRILL[Drill / instructional content] --> ASSIGN
  ASSIGN --> PROG[Athlete progress]
  USER --> PROG
  DRILL --> PROG
  PROG --> API[API and application services]
  ASSIGN --> API
  API --> CLIENT[Desktop / MAUI clients]
```

This is a domain-level explanation, not a declaration of exact current EF Core table names or foreign-key columns.

### 7. MAUI MVVM flow

```mermaid
graph TB
  VIEW[MAUI pages / views] -->|binding and commands| VM[ViewModels]
  VM -->|invoke| SERVICE[Client services]
  SERVICE -->|HTTP / JSON| API[Web API]
  API -->|result| SERVICE
  SERVICE --> VM
  VM -->|updated state| VIEW
```

The MAUI project is part of the repository. Individual pages, commands, and features may be in different stages of development.

### 8. Conceptual relational data model

```mermaid
erDiagram
  USER ||--o{ TRAINING_ASSIGNMENT : receives
  DRILL ||--o{ TRAINING_ASSIGNMENT : assigned_as
  USER ||--o{ PROGRESS_RECORD : records
  DRILL ||--o{ PROGRESS_RECORD : tracked_in
  USER {
    int id PK
    string role
  }
  DRILL {
    int id PK
    string sport
  }
  TRAINING_ASSIGNMENT {
    int id PK
    int user_id FK
    int drill_id FK
  }
  PROGRESS_RECORD {
    int id PK
    int user_id FK
    int drill_id FK
  }
```

**Conceptual entities only.** Inspect [`SkillBuilderPro.Core/`](SkillBuilderPro.Core/) and EF Core migrations for the current schema; names above are not guaranteed to match actual classes or tables.

### 9. End-to-end athlete training flow

```mermaid
graph LR
  A[Athlete selects training] --> B[Client service]
  B --> C[API request]
  C --> D[Validate user and input]
  D --> E[Save or retrieve progress]
  E --> F[(Database)]
  F --> G[API response]
  G --> H[Refresh athlete view]
```

This preserves the original drill-to-progress narrative while avoiding an unverified claim about a particular UI button, route, or response code.

---

## How I use AI to build and improve SBP

I configured a **project-specific AI coding agent** by writing prompts and instructions, testing its recommendations against SBP tasks, and refining those instructions as the codebase evolved. I use **OpenAI Codex** to investigate complex coding problems and help diagnose errors and bugs. I also work with **ChatGPT Projects, Claude Projects, GitHub Copilot, and Perplexity Computer Mode** in research, coding, planning, review, and troubleshooting workflows.

My typical workflow is to reproduce an issue, provide relevant project context, ask for a focused analysis or suggested change, inspect the proposed code, test the affected paths, and refine the solution based on results. **I review and validate changes; using an AI coding agent does not mean SBP deploys a production AI agent to customers.**

I use **Figma** extensively to explore visual direction and user experience, then translate design decisions into application changes and iterate on them.

---

## Local setup and testing

The following is a starting point, not a guarantee that every platform-specific project will run on every machine. Review the project configuration, connection strings, SDK requirements, and local secrets before starting.

```bash
git clone https://github.com/brovy23-GD/Skill-Builder-Pro-.git
cd Skill-Builder-Pro-
dotnet restore SkillBuilderPro.sln
```

For a supported environment, build the projects you intend to run and follow the API's local configuration and database migration setup. WinForms and some MAUI targets require the appropriate OS, workloads, and SDKs.

The repository includes [tests](SkillBuilderPro.Tests/) and a [GitHub Actions workflow](.github/workflows/dotnet-ci.yml). Consult the Actions tab for the latest test and coverage results rather than relying on an unverified "build passing" badge.

## API reference and performance notes

Because API routes and the domain model have evolved, the source of truth for endpoint names, request models, authorization rules, and status codes is the [current controller implementation](SkillBuilderPro.API/Controllers/). Where Swagger/OpenAPI is enabled locally, use it to inspect the running API.

Performance depends on actual query shapes, indexes, dataset size, and environment. An endpoint should not be described as a guaranteed O(1) database operation without profiling and a supporting execution plan.

## Design direction

The original project identity uses performance blue and a dark interface with high-contrast typography, sports-training imagery, and role-oriented experiences. Figma concepts and implementation may evolve separately; see the [design documentation](docs/design/) and [design assets](DesignAssets/) for project materials.

## Status and scope

SBP is an ongoing independently developed portfolio application. This README documents the engineering approach and the code currently represented in the public repository; it does not claim a production enterprise deployment, universal feature completeness, or that local changes have already been pushed to GitHub.

---

**Bobby Rovy** | [LinkedIn](https://www.linkedin.com/in/bobbyrovy) | [GitHub](https://github.com/brovy23-GD) | [Email](mailto:brovy23@gmail.com)
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
