<div align="center">

# Rohit Suryaa Portfolio

### Reliable software. Intelligent data systems. Built with C# and Blazor.

[![Live Site](https://img.shields.io/badge/live-rohitsuryaa.com-7c3aed?style=for-the-badge)](https://rohitsuryaa.com)
![Blazor](https://img.shields.io/badge/Blazor-WebAssembly-512bd4?style=for-the-badge&logo=blazor)
![.NET](https://img.shields.io/badge/.NET-9-512bd4?style=for-the-badge&logo=dotnet)
![GitHub Pages](https://img.shields.io/badge/deploy-GitHub%20Pages-222?style=for-the-badge&logo=github)

</div>

Modern personal portfolio for [rohitsuryaa.com](https://rohitsuryaa.com), rebuilt from a static HTML site into a C# Blazor WebAssembly application.

This portfolio is designed to position Rohit Suryaa Saravanan for software development, C#/.NET, backend engineering, data engineering, data science, machine learning, and research-focused roles. The site moves away from a plain resume-style layout and presents the work as an interactive product experience.

## The pitch

This is not a generic portfolio template. It is a focused technical identity system:

| Goal | Implementation |
| --- | --- |
| Recruiter clarity | Strong headline, direct role targeting, focused contact section |
| Technical proof | Case studies with challenge, approach, outcome, links, and metrics |
| Visual differentiation | Dark-first interface, animated data visuals, interaction polish |
| Engineering credibility | Blazor WebAssembly app with C#, JS interop, and GitHub Actions deployment |
| Global positioning | English/German language toggle and Germany-focused opportunity messaging |

## What changed

The original static website was replaced with a production-ready Blazor WebAssembly portfolio.

- Rebuilt the portfolio in C# using Blazor WebAssembly
- Removed the old root `index.html` static site
- Added a modern dark-first visual system with optional light mode
- Added Rohit's real profile photo as the hero portrait
- Removed the resume download section to keep the site focused and direct
- Added role-specific positioning for C#/.NET, backend, data, ML, and research opportunities
- Added GitHub Pages deployment through GitHub Actions
- Preserved the custom domain setup for `rohitsuryaa.com`

## Live site

[rohitsuryaa.com](https://rohitsuryaa.com)

## Tech stack

- C#
- Blazor WebAssembly
- .NET 9
- ASP.NET Core Minimal APIs
- EF Core
- PostgreSQL via Npgsql
- Markdig Markdown rendering
- JavaScript interop
- HTML/CSS
- GitHub Actions
- GitHub Pages

## Main features

- Dark mode by default, with persisted light/dark theme switching
- English/German content toggle
- Responsive navigation for desktop and mobile
- Scroll progress indicator
- Active-section navigation
- Animated reveal effects with reduced-motion support
- Interactive command palette with `Ctrl/Cmd + K`
- Filterable project case studies
- Interactive role-fit scanner built in Blazor
- Animated metric counters and visual data panels
- Pointer glow, magnetic buttons, and tilt-card interactions
- Contact section focused on real target roles
- Custom domain deployment using GitHub Pages

## Portfolio positioning

The current headline is:

> I build reliable software  
> and intelligent data systems.

That positioning is intentionally broader than a narrow data-science tagline. It works across early-career roles such as:

- C#/.NET Software Developer
- Backend Developer
- Data Engineer
- Data Scientist
- Machine Learning Engineer
- Research Assistant / Applied AI roles

The site avoids locking the profile into only one path and instead presents Rohit as someone who can build both the system and the intelligence inside it.

## Project structure

```text
.
├── .github/
│   └── workflows/
│       └── deploy.yml
├── RohitPortfolio.Blazor/
│   ├── Pages/
│   │   ├── Blog.razor
│   │   ├── BlogPost.razor
│   │   ├── Home.razor
│   │   ├── ProjectCard.razor
│   │   └── SectionHeading.razor
│   ├── Services/
│   │   └── BlogApiClient.cs
│   ├── wwwroot/
│   │   ├── css/
│   │   │   └── app.css
│   │   ├── images/
│   │   │   └── rohit.jpg
│   │   ├── js/
│   │   │   └── portfolio.js
│   │   ├── CNAME
│   │   └── index.html
│   ├── PortfolioProject.cs
│   └── RohitPortfolio.Blazor.csproj
├── RohitPortfolio.Api/
│   ├── Data/
│   │   ├── BlogDbContext.cs
│   │   └── Migrations/
│   ├── Models/
│   │   └── BlogPost.cs
│   ├── Services/
│   │   ├── MarkdownService.cs
│   │   └── SlugService.cs
│   └── Program.cs
├── RohitPortfolio.Shared/
│   └── BlogDtos.cs
├── BLOG_BACKEND.md
├── CNAME
├── RohitPortfolio.Blazor.slnx
└── README.md
```

## Key files

- `RohitPortfolio.Blazor/Pages/Home.razor`  
  Main portfolio content, bilingual state, project data, role-fit scanner, theme state, command palette, and contact logic.

- `RohitPortfolio.Api/Program.cs`  
  ASP.NET Core Minimal API backend for public blog reads and admin-protected writes.

- `RohitPortfolio.Api/Data/BlogDbContext.cs`  
  EF Core PostgreSQL schema for published/draft posts, tags, SEO fields, and related project metadata.

- `BLOG_BACKEND.md`  
  Backend setup, migration, API, PostgreSQL, and deployment notes.

- `RohitPortfolio.Blazor/Pages/ProjectCard.razor`  
  Reusable project case-study card with challenge, approach, outcome, links, and project visuals.

- `RohitPortfolio.Blazor/wwwroot/css/app.css`  
  Full visual system: dark/light themes, responsive layout, animation, cards, hero, project sections, lab UI, and contact design.

- `RohitPortfolio.Blazor/wwwroot/js/portfolio.js`  
  Browser-side interaction layer: theme persistence, scroll progress, active nav, reveal observer, metric counters, magnetic buttons, tilt cards, and command shortcut handling.

- `.github/workflows/deploy.yml`  
  GitHub Actions workflow that publishes the Blazor app and deploys it to GitHub Pages.

## Run locally

Requirements:

- .NET 9 SDK
- Visual Studio 2022 with the ASP.NET and web development workload, or any editor with the .NET CLI

From the repository root:

```powershell
dotnet restore
dotnet run --project RohitPortfolio.Blazor
```

Then open the local URL printed by the CLI, usually:

```text
http://localhost:5000
```

or

```text
http://localhost:5188
```

## Build

```powershell
dotnet publish RohitPortfolio.Blazor/RohitPortfolio.Blazor.csproj -c Release -o publish
```

The static site output is generated in:

```text
publish/wwwroot
```

## Deployment

Deployment is handled by GitHub Actions.

On every push to `main`, the workflow:

1. Checks out the repository
2. Installs .NET 9
3. Publishes the Blazor WebAssembly project
4. Copies `index.html` to `404.html` for SPA routing support
5. Adds `.nojekyll`
6. Writes the `CNAME` file for `rohitsuryaa.com`
7. Deploys the final `publish/wwwroot` folder to GitHub Pages

The custom domain is configured through:

```text
CNAME
RohitPortfolio.Blazor/wwwroot/CNAME
```

Both point to:

```text
rohitsuryaa.com
```

## Design direction

The site is built to feel closer to a technical product page than a traditional personal website.

The design priorities are:

- Strong first impression
- Dark-first premium visual identity
- Fast scanning for recruiters
- Clear role targeting
- Proof through measurable project outcomes
- Smooth but controlled interactions
- No unnecessary resume clutter

## Current contact focus

The contact section is tuned for:

- C# / .NET
- Software Development
- Backend Engineering
- Data Engineering
- Data Science
- Machine Learning
- Research opportunities

## Repository status

The current production version has been pushed to `main` and deployed to GitHub Pages.
