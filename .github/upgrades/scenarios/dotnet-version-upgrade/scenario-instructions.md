# .NET Version Upgrade

## Preferences
- **Flow Mode**: Automatic
- **Target Framework**: .NET 11 (Preview)

## Upgrade Options
- **Upgrade Strategy**: All-at-Once

## Strategy
**Selected**: All-At-Once
**Rationale**: 8 SDK-style projects all on modern .NET with shallow dependencies and minimal complexity. Atomic upgrade in single pass is most efficient.

### Execution Constraints
- All projects upgraded simultaneously in a single operation — no phasing
- SDK-style conversion not needed (all projects already SDK-style)
- Complete all TFM and package updates before validation
- Build and fix errors in a single bounded pass
- Solution may be temporarily in intermediate state during upgrade

## Source Control
- **Source Branch**: FinancistoMAUI
- **Working Branch**: upgrade-dotnet-11
- **Commit Strategy**: After Each Task
- **Branch Sync**: Auto (Merge)
