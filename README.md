# SocialMotive Platform

## Project Description

**SocialMotive** is a comprehensive social platform designed to connect **volunteers** with **event organizers** and volunteer opportunities. The platform enables volunteers to discover and participate in community events while organizers can efficiently manage volunteers, track impact, and create professional promotional content.

## Vision & Goals

### Primary Goals
- **Empower volunteers** to easily find and participate in meaningful community events
- **Support organizers** with tools to manage volunteers, track hours, and measure impact
- **Streamline event promotion** with an integrated image/poster generator for social media
- **Enable data-driven decisions** through comprehensive admin tools and analytics

### Success Metrics
- v1.0 Beta (March 31, 2026): MVP launch with core volunteer discovery + event registration + canvas editor
- v3.0+ (Q4 2026): 5,000+ active volunteers, marketplace & monetization launch
- v4.0+ (2027): Mobile apps, international scale, AI-powered volunteer matching

## Technology Stack

- **Runtime**: .NET 10.0 ASP.NET Core
- **UI Framework**: Blazor Web App (Auto/Interactive rendering)
- **UI Components**: Telerik UI for Blazor
- **Database**: Microsoft SQL Server (two databases: `socialmotive`, `trekkergenerator`)
- **Authentication**: OIDC/OAuth2 SSO
- **ORM**: Entity Framework Core
- **Testing**: xUnit, Moq
- **API Documentation**: Swagger/OpenAPI

## Project Structure

```
SocialMotive/
├── planning/                          # Project planning & design docs
│   ├── README.md                      # This file
│   ├── functional-design.md/.html     # Detailed feature specifications
│   ├── technical-design.md            # Architecture & infrastructure
│   ├── roadmap.md                     # v1.0 → v4.0+ product roadmap
│   └── plan-socialMotiveTrekkerGenerator.prompt.md
│
├── SQL/                               # Database scripts
│   └── socialmotive-create-v1.sql     # Schema & initial data
│
└── src/ (to be created)               # Application source code
    ├── SocialMotive.Core/             # Shared library (DTOs, contracts, auth)
    │
    ├── SocialMotive.Frontend.Web/     # Volunteer portal
    │   ├── Controllers/
    │   ├── Services/
    │   └── Components/
    │
    ├── SocialMotive.TrekkerGenerator.Web/  # Canvas editor & promo tool
    │   ├── Controllers/
    │   ├── Services/
    │   └── Components/
    │
    ├── SocialMotive.TrekkerGenerator.Core/ # Business logic
    │   ├── Domain/
    │   ├── Services/
    │   └── Infrastructure/
    │
    ├── SocialMotive.AdminBackend.Web/ # Admin data management
    │   ├── Controllers/
    │   ├── Services/
    │   └── Components/
    │
    └── SocialMotive.TrekkerGenerator.Web.Tests/
```

## Key Features (MVP - v1.0)

### Frontend.Web (Volunteer Portal)
- 🔐 OIDC/OAuth2 authentication
- 🔍 Event discovery with filtering & search
- 📋 Event registration & management
- ⏱️ Volunteer hours tracking
- 👤 User profile management
- 📧 Email notifications (basic)

### TrekkerGenerator.Web (Canvas Editor)
- 🎨 Interactive WYSIWYG canvas editor
- 🖼️ Layer management (text & images)
- 📐 Canvas presets (social media sizes)
- 💾 Template save/load system
- 📥 Background image upload
- 📤 PNG export (transparency support)

### AdminBackend.Web (Data Management)
- 📊 Metadata-driven CRUD grids
- 🔍 Advanced filtering & sorting
- 💾 CSV/Excel export
- 🗂️ Multi-database support (SocialMotive + TrekkerGenerator)
- 📋 Table whitelist controls
- 📝 Audit logging (v2+)

## Getting Started

### Prerequisites
- .NET 10.0 SDK
- SQL Server 2019+ (SQL Server Express supported)
- Node.js 18+ (for frontend tooling, optional)

### Setup (Coming Soon)
```bash
# Clone repository
git clone https://github.com/BoomRepublic/SocialMotive.git
cd SocialMotive

# Install SQL Server schema
sqlcmd -S (local) -i SQL/socialmotive-create-v1.sql

# Create .NET projects (see technical-design.md)
dotnet new globaljson --sdk-version 10.0.0
dotnet sln create SocialMotive.sln

# Restore & build
dotnet restore
dotnet build
```

## Documentation

All planning and design documentation is in the `planning/` folder:

- **[Functional Design](planning/functional-design.md)** - Feature specs, workflows, API endpoints, business rules
- **[Technical Design](planning/technical-design.md)** - Architecture, project structure, database schema, deployment
- **[Product Roadmap](planning/roadmap.md)** - v1.0 → v4.0+, timeline, resource allocation, success metrics
- **[DTO Catalog](planning/plan-socialMotiveTrekkerGenerator.prompt.md)** - Complete DTO specifications with namespaces

## Architecture Overview

```
┌─────────────────────────────────────────────────────────────┐
│                    SocialMotive.Core                        │
│         (Shared DTOs, Contracts, Auth Helpers)             │
└─────────────────────────────────────────────────────────────┘
    ▲              ▲              ▲
    │              │              │
┌───┴──────┐  ┌────┴─────┐  ┌────┴──────┐
│ Frontend │  │ Trekker  │  │   Admin   │
│   Web    │  │Generator │  │  Backend  │
│  (Portal)│  │  (Canvas)│  │   (CRUD)  │
└──────────┘  └──────────┘  └───────────┘
    │              │              │
    └──────────────┴──────────────┘
         OIDC/OAuth2 SSO Provider
```

Each app:
- Runs independently on its own subdomain
- Has dedicated REST API controllers
- Uses typed HttpClient API services
- Accesses separate databases with shared schemas

## Development Status

- ✅ Architecture & planning (complete)
- ✅ Database schema design (complete)
- ✅ API specifications (complete)
- ⏳ Project scaffolding (pending)
- ⏳ Implementation (pending)

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md) (coming soon)

## License

License information coming soon

## Contact & Support

- **Project Lead**: SocialMotive Dev Team
- **Repository**: https://github.com/BoomRepublic/SocialMotive
- **Status**: Early Development (v1.0 Beta Target: March 31, 2026)

---

**Last Updated**: March 11, 2026
