# Rohit Portfolio — Blazor

This is the upgraded C#/Blazor WebAssembly portfolio: an editorial, interaction-led experience built around measurable project outcomes rather than a conventional resume layout.

## Experience features

- Responsive light/dark visual system with persisted preference
- English/German content switching
- Scroll progress and active-section navigation
- Accessible reveal motion with reduced-motion support
- Pointer glow, tilt cards, magnetic calls to action, and animated data visuals
- Filterable project case studies with challenge/approach/outcome details
- Interactive Blazor ML trade-off simulator
- Command palette (`Ctrl/Cmd + K`)
- Clipboard contact action
- Mobile navigation and mobile-specific layouts

## Requirements

- Visual Studio 2022 17.12 or later with the **ASP.NET and web development** workload, or
- .NET 9 SDK or later

## Run

From Visual Studio, open `RohitPortfolio.Blazor.sln`, set `RohitPortfolio.Blazor` as the startup project, and press `F5`.

From a terminal:

```powershell
dotnet restore
dotnet run --project RohitPortfolio.Blazor
```

## Main files

- `Pages/Home.razor` — content, filters, command palette, bilingual state, ML simulator, theme state, and portfolio data
- `Pages/ProjectCard.razor` — reusable measurable case-study card
- `Pages/SectionHeading.razor` — reusable section heading
- `wwwroot/css/app.css` — complete responsive styling and dark theme
- `wwwroot/js/portfolio.js` — scroll observers, active navigation, counters, tilt/magnetic interactions, theme persistence, and keyboard shortcuts
- `wwwroot/images/rohit.jpg` — profile image

## Deployment

The app is Blazor WebAssembly and can be deployed as static files. Run:

```powershell
dotnet publish -c Release
```

Publish output is generated under `bin/Release/net9.0/publish/wwwroot`.
