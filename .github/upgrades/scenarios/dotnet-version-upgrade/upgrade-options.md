# Upgrade Options — Financier.Desktop

Assessment: 8 projects, all SDK-style modern .NET (net11.0, net10.0), 2 package upgrades recommended (EF Core, Extensions.Logging), no incompatibilities

## Strategy

### Upgrade Strategy

All projects are already on modern .NET with shallow dependencies and low complexity. A single atomic upgrade pass across all projects is the most efficient approach.

| Value | Description |
|-------|-------------|
| **All-at-Once** (selected) | Upgrade all projects simultaneously in a single atomic pass. Fastest approach, no multi-targeting overhead. |
| Top-Down | Upgrade applications first, multi-target libraries temporarily. Adds complexity for minimal benefit at this scale. |
