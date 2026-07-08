# Financier.Desktop .NET 11 Upgrade Tasks

## Overview

This document lists executable tasks to upgrade `Financier.Desktop.sln` to .NET 11 (net11.0) using an atomic all-at-once approach: prerequisites, a single atomic upgrade pass, test execution, and a final commit. Tasks are self-contained and reference the plan for specifics.

**Progress**: 2/4 tasks complete (50%) ![0%](https://progress-bar.xyz/50)

---

## Tasks

### [✓] TASK-001: Verify prerequisites *(Completed: 2026-03-03 19:37)*
**References**: Plan §Preconditions, Plan §Migration Strategy

- [✓] (1) Verify required .NET 11 SDK is installed on the execution environment per Plan §Preconditions
- [✓] (2) Runtime/SDK version meets minimum requirements for net11.0 (**Verify**)
- [✓] (3) Check `global.json` compatibility and update if required per Plan §Preconditions (do not perform VCS actions)
- [✓] (4) `global.json` is compatible with .NET 11 or updated as required (**Verify**)

---

### [✓] TASK-002: Atomic framework and package upgrade with compilation fixes *(Completed: 2026-03-03 21:44)*
**References**: Plan §Migration Strategy, Plan §Project-by-Project Plans, Plan §Package Update Reference, Plan §Breaking Changes Catalog

- [✓] (1) Update TargetFramework/TargetFrameworks in all project files listed in Plan §Project-by-Project Plans to the proposed targets (net11.0 / net11.0-windows as applicable)
- [✓] (2) Update package references across all projects per Plan §Package Update Reference (including EF Core, logging, `Newtonsoft.Json`, and noted WPF/MAUI package adjustments)
- [✓] (3) Restore dependencies (`dotnet restore`) at repository root per Plan §Testing & Validation Strategy
- [✓] (4) Build solution and fix all compilation errors resulting from framework/package upgrades, following remediation patterns in Plan §Breaking Changes Catalog
- [✓] (5) Solution builds with 0 errors (**Verify**)

---

### [▶] TASK-003: Run test suite and validate upgrade
**References**: Plan §Testing & Validation Strategy, Plan §Project-by-Project Plans, Plan §Breaking Changes Catalog

- [✓] (1) Run tests for the test projects: `src\Tests\Financier.Desktop.Tests\Financier.Desktop.Tests.csproj` and `src\Tests\Financier.Tests.Common\Financier.Tests.Common.csproj` per Plan §Testing & Validation Strategy
- [▶] (2) Fix any test failures (reference Plan §Breaking Changes Catalog for common issues and remediation patterns)
- [ ] (3) Re-run tests after fixes
- [ ] (4) All tests pass with 0 failures (**Verify**)

---

### [ ] TASK-004: Final commit
**References**: Plan §Source Control Strategy

- [ ] (1) Commit all changes with message: "TASK-004: Complete upgrade of `Financier.Desktop.sln` to net11.0 and update packages"





