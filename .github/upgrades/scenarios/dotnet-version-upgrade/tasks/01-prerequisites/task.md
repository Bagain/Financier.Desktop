# 01-prerequisites: Verify SDK and toolchain compatibility

Ensure the development environment is prepared for .NET 11 upgrade. Verify that .NET 11 SDK is installed, project files have correct structure, and any global.json constraints are compatible.

**Scope**: All projects, solution-wide configuration

**Assessment context**: All 8 projects are SDK-style on modern .NET. The solution uses Directory.Packages.props for centralized package management (good practice).

**Done when**:
- .NET 11 SDK is installed and verified (`dotnet --version`)
- global.json (if present) is updated for .NET 11 compatibility
- Solution builds successfully with current state as baseline
