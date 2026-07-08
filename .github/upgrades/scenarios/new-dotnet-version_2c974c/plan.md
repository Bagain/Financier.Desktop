# .NET 11 Upgrade Plan for Financier.Desktop

## Table of Contents
- Executive Summary
- Migration Strategy
- Detailed Dependency Analysis
- Project-by-Project Plans
- Package Update Reference
- Breaking Changes Catalog
- Testing & Validation Strategy
- Risk Management
- Complexity & Effort Assessment
- Source Control Strategy
- Success Criteria


## 1. Executive Summary

### Scenario
Upgrade solution `Financier.Desktop.sln` to target .NETCoreApp, Version=11.0 (net11.0) using an All-At-Once Strategy: all projects will be updated in a single coordinated operation.

### High-level Findings (from assessment)
- Projects: 8; all require target framework updates to net11.0 or net11.0-windows for Windows-specific projects.
- Major compatibility hotspots: WPF-based projects (`Financier.Common`, `Financier.Desktop`, `Financier.Reports`) and the .NET MAUI project (`Financier.MAUI`) that has extensive source-incompatible API usages.
- NuGet issues: 5 packages flagged for upgrade/incompatibility (including `Microsoft.EntityFrameworkCore.Sqlite`, `Microsoft.Extensions.Logging.Debug`, `DataGridExtensions`, `Microsoft.Xaml.Behaviors.Wpf`, `Newtonsoft.Json`).
- API analysis: many binary and source incompatibilities expected in WPF and MAUI layers; compilation and code changes will be required.

### Selected Strategy
**All-At-Once Strategy** — Upgrade all projects simultaneously in a single atomic operation.

**Rationale:**
- Medium-sized repository (8 projects) which fits All-At-Once constraints.
- Project SDK-style consistency and available package upgrade paths.
- Team accepts short coordinated instability window for faster completion.


## 2. Migration Strategy

### Scope
- All projects in `Financier.Desktop.sln` will be updated to their proposed target frameworks in assessment.md (net11.0 for cross-platform and net11.0-windows for WPF/Windows-specific projects). The MAUI project will append `net11.0-windows` as an additional target framework.

### Key Principles
- Atomic operation: update all project TargetFramework(s) and package versions in a single commit/PR.
- Respect dependency graph when reasoning about code changes, but do not split the upgrade per-project; changes and compilation fixes happen in the same atomic pass.
- Include all package updates suggested by assessment; particularly address packages with security or incompatibility flags.
- Validation: full solution restore, build (zero errors), and run test suite.

### Preconditions
- Developer machines and CI must have .NET 11 SDK installed and available. Update global.json if required.
- Ensure pending changes are handled: commit pending changes on `FinancistoMAUI` before branch creation (per assessment defaults).
- Create and switch to upgrade branch `upgrade-to-NET11` before making changes.



## 3. Detailed Dependency Analysis

### Summary
- Total projects: 8
- Topological dependency depth: 3 (DataAccess is a leaf, Desktop/Reports/Common depend on DataAccess)
- No cycles detected in project-level dependencies.
- Migration will respect dependency relationships when applying code fixes, but the actual TargetFramework updates are applied atomically across all projects.

### Migration Ordering (for reasoning and troubleshooting)
- Leaf-first conceptual ordering (for understanding where compilation errors may originate):
  1. `Financier.DataAccess` (leaf)
  2. `Financier.Adapter`, `Financier.Tests.Common` (depend on DataAccess)
  3. `Financier.Common`, `Financier.Reports` (depend on DataAccess/Common)
  4. `Financier.Desktop` (depends on Adapter, Common, Reports)
  5. `Financier.MAUI` and test projects (MAUI has no project dependencies)

### Critical path
- `Financier.DataAccess` -> `Financier.Common` -> `Financier.Desktop`

### Notes
- Although All-At-Once is used, the critical path helps prioritize troubleshooting when build errors occur after the atomic changes.
- Test projects should be executed after the atomic upgrade completes and the solution builds.



## 4. Project-by-Project Plans

Below are per-project specifications. Each project will be updated as part of the single atomic upgrade pass.

### Common per-project steps (applies to all projects)
1. Update `TargetFramework` or `TargetFrameworks` to the proposed value from assessment.md (net11.0 or net11.0-windows). For multi-targeted projects (MAUI), append `net11.0-windows` per assessment.
2. Update `PackageReference` items to versions recommended in the assessment.
3. Restore packages (`dotnet restore`) and build solution to identify compilation errors.
4. Fix compilation errors caused by framework/package updates (API changes, obsolete members).
5. Rebuild solution and ensure zero build errors.
6. Run test projects and address test failures.


---

### Project: src\Financier.DataAccess\Financier.DataAccess.csproj
**Current**: net10.0
**Target**: net11.0
**Type**: ClassLibrary
**Key packages**: `Microsoft.EntityFrameworkCore.Sqlite` (9.0.4 -> 10.0.3 suggested)
**Risk**: Low

Migration steps:
- Update project TargetFramework to `net11.0`.
- Update `Microsoft.EntityFrameworkCore.Sqlite` to `10.0.3`.
- Verify EF Core provider compatibility; update migrations if necessary.
- Build and validate.


---

### Project: src\Financier.Adapter\Financier.Adapter.csproj
**Current**: net10.0
**Target**: net11.0
**Type**: ClassLibrary
**Risk**: Low

Migration steps:
- Update TargetFramework to `net11.0`.
- Restore, build, and fix any compile issues.


---

### Project: src\Financier.Tests.Common\Financier.Tests.Common.csproj
**Current**: net10.0
**Target**: net11.0
**Type**: ClassLibrary
**Risk**: Low

Migration steps:
- Update TargetFramework to `net11.0`.
- Ensure test helpers and dependencies are compatible with net11.0.


---

### Project: src\Financier.Common\Financier.Common.csproj
**Current**: net10.0-windows
**Target**: net11.0-windows
**Type**: WPF library
**Key packages**: (WPF-compatible packages; none flagged for upgrade in this project specifically)
**Risk**: Medium (WPF API incompatibilities)

Migration steps:
- Update TargetFramework to `net11.0-windows`.
- Verify and adapt WPF-specific APIs (DependencyProperty patterns, Application lifecycle, XAML component loading).
- Address binary-incompatible members identified in the assessment (see Breaking Changes Catalog).


---

### Project: src\Financier.Reports\Financier.Reports.csproj
**Current**: net10.0-windows
**Target**: net11.0-windows
**Type**: WPF application/library
**Risk**: Medium

Migration steps:
- Update TargetFramework to `net11.0-windows`.
- Update references to `Financier.Common` and `Financier.DataAccess` as needed.
- Fix WPF API incompatibilities.


---

### Project: src\Financier.Desktop\Financier.Desktop.csproj
**Current**: net10.0-windows
**Target**: net11.0-windows
**Type**: WPF application
**Key packages**: `DataGridExtensions` (incompatible), `Microsoft.Xaml.Behaviors.Wpf` (incompatible), `Newtonsoft.Json` (recommended update)
**Risk**: Medium-High

Migration steps:
- Update TargetFramework to `net11.0-windows`.
- Replace or downgrade incompatible packages if newer compatible versions are unavailable (see Package Update Reference).
- Address WPF binary incompatible APIs (see Breaking Changes Catalog).


---

### Project: src\Financier.MAUI\Financier.MAUI.csproj
**Current**: multi-target net10.0-android;net10.0-ios;net10.0-maccatalyst;net10.0-windows10.0.19041.0
**Target**: append `net11.0-windows` to TargetFrameworks
**Key packages**: `CommunityToolkit.Maui`, `Microsoft.EntityFrameworkCore.Sqlite`, `Microsoft.Extensions.Logging.Debug`, `sqlite-net-pcl`
**Risk**: Medium (many source-incompatible MAUI API usages expected)

Migration steps:
- Append `net11.0-windows` to the existing `TargetFrameworks` entry (no removal of existing targets).
- Update `Microsoft.EntityFrameworkCore.Sqlite` to `10.0.3` and `Microsoft.Extensions.Logging.Debug` to `10.0.3`.
- Reconcile MAUI API source incompatibilities (see Breaking Changes Catalog) across XAML and code-behind.


---

### Project: src\Tests\Financier.Desktop.Tests\Financier.Desktop.Tests.csproj
**Current**: net10.0-windows7.0
**Target**: net11.0-windows7.0
**Type**: Test project
**Risk**: Low

Migration steps:
- Update TargetFramework to `net11.0-windows7.0` (match assessment proposed target).
- Update test SDKs if necessary.
- Run test suite after atomic upgrade.



## 5. Package Update Reference

### Common Package Updates (affecting multiple projects)
- `Microsoft.EntityFrameworkCore.Sqlite` 9.0.4 → 10.0.3 (Affected: `Financier.DataAccess`, `Financier.MAUI`) — EF Core provider upgrade required for net11.0 compatibility and security updates.
- `Microsoft.Extensions.Logging.Debug` 9.0.1 → 10.0.3 (Affected: `Financier.MAUI`) — Update to align with framework runtime.
- `Newtonsoft.Json` 13.0.4-beta1 → 13.0.4 (Affected: `Financier.DataAccess`, `Financier.Desktop`) — Stabilize to release version.

### Incompatible packages to address
- `DataGridExtensions` 2.6.0 → recommended 2.5.15 or find alternative (Affected: `Financier.Desktop`) — assessment flagged incompatibility; consider replacing or locking to compatible version.
- `Microsoft.Xaml.Behaviors.Wpf` 1.1.135 → 1.1.39 (Affected: `Financier.Desktop`) — use compatible package version for WPF on net11.0.

### Other packages (no change required per assessment)
- `CommunityToolkit.Maui` 11.2.0 — compatible (Financier.MAUI)
- `CommunityToolkit.MVVM` 8.4.0 — compatible (Financier.MAUI)
- `sqlite-net-pcl` 1.9.172 — compatible (Financier.MAUI)
- `OxyPlot.Wpf`, `Prism.Core`, `NLog`, `CsvHelper`, `Moq`, `xunit`, etc. — compatible

### Guidance
- Apply all package updates in the atomic pass alongside TargetFramework changes.
- For incompatible packages where recommended older versions are suggested, evaluate whether upstream updates exist; if not, select known-compatible version and document rationale.
- Flag any package where no compatible version exists as blocking and document mitigation (replace package or adjust code to remove dependency).



## 6. Breaking Changes Catalog

This catalog lists expected breaking changes discovered during analysis and indicates which projects are likely to be affected. The list is not exhaustive — additional issues will be discovered during compilation.

### High-impact breaking changes (examples)
- WPF binary incompatible types and members (e.g., `System.Windows.Application`, `UserControl` constructors, `IComponentConnector`) — Affects `Financier.Common`, `Financier.Desktop`, `Financier.Reports`.
- WPF XAML loading differences (LoadComponent behavior) — verify `InitializeComponent` and XAML resource URIs.
- MAUI source-incompatible API changes in animations, bindings and controls (e.g., `Microsoft.Maui.Controls.Animation`, `BindableObject.BindingContext`, `InputView.TextColor`) — Affects `Financier.MAUI`.
- `System.Uri` constructor behavioral changes — Search and review any code that constructs `Uri` with ambiguous UriKind.
- EF Core provider API changes between EF Core 9 → 10 — migration of DbContext options or provider-specific APIs may be required.

### Representative code-level remediation patterns
- Replace removed/renamed types with new equivalents; use modern MAUI/WPF patterns where available.
- For XAML-related issues: ensure XAML namespaces are updated, verify `x:Class` and `x:Name` declarations, and re-generate InitializeComponent where needed.
- For animation and binding API changes in MAUI: adapt to new overloads, replace deprecated properties (e.g., TextColor usage) with new property patterns.
- For package incompatibilities: prefer upgrading to suggested versions; if unavailable, consider replacing the package with an alternative library.

### Discovery during build
- Expect source-compile errors in WPF and MAUI layers. Use the critical path ordering to triage root causes beginning with `Financier.DataAccess` then `Financier.Common`.



## 7. Testing & Validation Strategy

### Validation checkpoints
- Preconditions: .NET 11 SDK installed and global.json validated.
- Post-upgrade build: Full solution restores and builds with zero errors.
- Unit tests: All unit test projects run and pass.
- Integration checks: Where automated integration tests exist, they should be executed.

### Test projects to run
- `src\Tests\Financier.Desktop.Tests\Financier.Desktop.Tests.csproj`
- `src\Tests\Financier.Tests.Common\Financier.Tests.Common.csproj`

### Automated steps (recommended during execution)
1. `dotnet restore` (root)
2. `dotnet build` (root solution)
3. `dotnet test` for discovered test projects

### Validation criteria
- Solution builds with 0 errors
- Unit test runs report zero failed tests
- No unresolved package dependency conflicts remain
- No critical runtime exceptions for main application startup (if automated smoke tests exist)

### Manual verification (document only)
- Run desktop application on a Windows machine and verify core UI flows (login, listing, reports) — manual step, not part of atomic automation



## 8. Risk Management

### High-risk areas
- WPF projects (`Financier.Common`, `Financier.Desktop`, `Financier.Reports`) — large counts of binary-incompatible APIs. Mitigation: allocate focused troubleshooting after atomic upgrade; prefer code fixes that preserve public API where possible.
- MAUI project (`Financier.MAUI`) — many source-incompatible API usages. Mitigation: update MAUI toolkit packages first and adapt XAML/code-behind accordingly.
- Incompatible NuGet packages (`DataGridExtensions`, `Microsoft.Xaml.Behaviors.Wpf`) — Mitigation: locate compatible versions or replacements; document any regressions.

### Risk mitigation strategies
- Preinstall .NET 11 SDK on build agents and developer machines.
- Commit pending changes and use a single upgrade branch `upgrade-to-NET11` for the atomic change.
- Run `dotnet build` immediately after package and TF updates to gather deterministic list of compile errors and address them in the same atomic pass.
- Prepare fallback plan: if a package has no compatible version, revert to previous commit and open a focused remediation PR for that package.

### Rollback plan
- If atomic upgrade introduces blocking issues that cannot be fixed within the upgrade branch window, revert the upgrade branch and resume work on a per-issue basis outside of the atomic pass.
- Keep detailed commit history in `upgrade-to-NET11` branch; use PR review to gate merge into main.



## 9. Complexity & Effort Assessment

Per-project complexity classification (relative):

- `Financier.DataAccess` — Low
- `Financier.Adapter` — Low
- `Financier.Tests.Common` — Low
- `Financier.Tests.Desktop` — Low
- `Financier.Common` — Medium (WPF API changes)
- `Financier.Reports` — Medium (WPF)
- `Financier.Desktop` — Medium-High (WPF + incompatible packages)
- `Financier.MAUI` — Medium (many source-incompatible usages)

Notes:
- Expect the majority of code fixes in WPF and MAUI projects. DataAccess and helper libraries have minimal changes.
- Complexity classification guides reviewer priorities but does not imply phased execution; the All-At-Once strategy applies.



## 10. Source Control Strategy

- Branching:
  - Start from source branch `FinancistoMAUI` (current branch per assessment) after committing pending changes.
  - Create and switch to upgrade branch `upgrade-to-NET11` and perform all upgrade changes there.
- Commit strategy:
  - Single atomic commit or small set of logically grouped commits for TargetFramework and package updates followed by compilation fixes. Prefer a single PR that contains the full upgrade for easier review.
  - Ensure commit messages clearly reference this upgrade (e.g., `chore(upgrade): migrate projects to net11.0 and update packages`).
- Pull Request:
  - Create PR from `upgrade-to-NET11` into the main branch.
  - Include link to this `plan.md` and `assessment.md` in PR description.


## 11. Success Criteria

Migration is complete when the following technical criteria are met:
- All projects target their proposed framework from assessment (net11.0 or net11.0-windows where applicable).
- All package updates listed in §5 applied and no package compatibility conflicts remain.
- Solution restores and builds with 0 errors.
- All unit tests pass in test projects.
- No outstanding known critical security vulnerabilities remain in project dependencies.


