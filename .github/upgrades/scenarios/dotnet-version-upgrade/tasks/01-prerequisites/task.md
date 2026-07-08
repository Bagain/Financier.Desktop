# 01-prerequisites: Verify SDK and toolchain compatibility

Ensure the development environment is prepared for .NET 11 upgrade. Verify that .NET 11 SDK is installed, project files have correct structure, and any global.json constraints are compatible.

**Scope**: All projects, solution-wide configuration

**Assessment context**: All 8 projects are SDK-style on modern .NET. The solution uses Directory.Packages.props for centralized package management (good practice).

**Done when**:
- .NET 11 SDK is installed and verified (`dotnet --version`)
- global.json (if present) is updated for .NET 11 compatibility
- Solution builds successfully with current state as baseline

## Research Findings

### Environment Status
- ✅ .NET 11 SDK installed: 11.0.100-preview.5.26302.115
- ✅ No global.json file found (not constraining SDK selection)
- ❌ Solution does NOT build currently

### Projects Affected
All 8 projects in solution (no new SDK-style conversion needed)

### Build Failure Analysis
Initial build attempt revealed 5 NuGet restore errors:
- 3 test projects have mismatched target frameworks causing incompatibility with upgraded dependencies:
  1. `src/Tests/Financier.DataAccess.Tests/Financier.DataAccess.Tests.csproj` - targets net10.0 (should be net11.0)
  2. `src/Tests/Financier.Adapter.Tests/Financier.Adapter.Tests.csproj` - targets net10.0 (should be net11.0)
  3. `src/Tests/Financier.Converter.Test/Financier.Common.Test.csproj` - targets net10.0-windows7.0 (should be net11.0-windows7.0)

These test projects reference main projects that have already been upgraded to net11.0:
- Financier.DataAccess (net11.0) - referenced by Financier.DataAccess.Tests
- Financier.Adapter (net11.0) - referenced by Financier.Adapter.Tests
- Financier.Reports (net11.0-windows7.0) - referenced by Financier.Common.Test

### Files to Modify
- `src/Tests/Financier.DataAccess.Tests/Financier.DataAccess.Tests.csproj` - Update TargetFramework from net10.0 to net11.0
- `src/Tests/Financier.Adapter.Tests/Financier.Adapter.Tests.csproj` - Update TargetFramework from net10.0 to net11.0
- `src/Tests/Financier.Converter.Test/Financier.Common.Test.csproj` - Update TargetFramework from net10.0-windows7.0 to net11.0-windows7.0

### Package Issues
- SQLitePCLRaw.lib.e_sqlite3 2.1.11 has a known high severity vulnerability (GHSA-2m69-gcr7-jv3q)
  - This is a transitive dependency, will be addressed in task 02-upgrade-all-projects
- All packages are compatible with .NET 11 according to assessment

### Decisions Made
- Task scope: Limited to making the solution restorable as a prerequisite for task 02-upgrade-all-projects
- Update only test project target frameworks to enable build/restore to proceed
- Vulnerability remediation deferred to subsequent task where package updates are applied
