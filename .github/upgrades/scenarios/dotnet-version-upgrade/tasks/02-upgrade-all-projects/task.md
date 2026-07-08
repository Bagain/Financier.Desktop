# 02-upgrade-all-projects: Update frameworks and packages

Update all project files to target .NET 11, bump NuGet packages to compatible versions, and resolve any API breaking changes. This is the core upgrade task covering all 8 projects across all dependency levels simultaneously.

**Scope**: All 8 projects:
- **Adapter layer**: Financier.Adapter (net11.0)
- **Data & common**: Financier.Common (net11.0-windows), Financier.DataAccess (net11.0)
- **Desktop apps**: Financier.Desktop (net11.0-windows), Financier.Reports (net11.0-windows)
- **MAUI app**: Financier.MAUI (currently net10.0, needs update to net11.0 targets)
- **Tests**: Financier.Desktop.Tests, Financier.Tests.Common (net11.0-windows, net11.0)

**Assessment context**: 
- **Package upgrades**: 2 packages need updates (Microsoft.EntityFrameworkCore.Sqlite 10.0.3 → 10.0.9, Microsoft.Extensions.Logging.Debug 10.0.3 → 10.0.9)
- **API issues**: 0 binary incompatible APIs, 0 source incompatible APIs detected
- **Risk**: Low — most projects have no issues; MAUI has 2 package upgrades to resolve
- **Known breaking changes to check**: MAUI platform-specific TFM changes (net10.0-android → net11.0-android, etc.)

**Research starting points**:
- Verify all TargetFramework entries across project files
- Review MAUI's platform-specific TFM targets (android, ios, maccatalyst, windows)
- Confirm EF Core compatibility with .NET 11
- Check for any platform-specific API deprecations in updated packages

**Operation sequence**:
1. Update all TargetFramework values to net11.0 or net11.0-{platform} as applicable
2. Update package references (Directory.Packages.props or direct csproj entries)
3. Restore dependencies (`dotnet restore`)
4. Build solution and fix all compilation errors in a single bounded pass
5. Verify solution builds with 0 errors and 0 warnings

**Done when**:
- All 8 projects target .NET 11 (or net11.0-{platform} for platform-specific projects)
- All recommended package upgrades applied (EF Core 10.0.9, Extensions.Logging 10.0.9)
- Solution builds successfully with 0 errors
- No new compilation warnings introduced
