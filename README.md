# Candy Customizer

Candy Customizer is an EXILED plugin for SCP: Secret Laboratory that lets server owners control SCP-330 candy behavior without hardcoding one-off candy effects.

It targets EXILED `9.14.0`.

## Features

- Configure every supported SCP-330 candy: `Rainbow`, `Yellow`, `Purple`, `Red`, `Green`, `Blue`, `Pink`, `Orange`, `White`, `Gray`, `Black`, `Brown`, and `Evil`.
- Choose per-candy mode:
  - `VanillaOnly`: leave the candy unchanged.
  - `Additive`: run vanilla candy behavior, then apply custom behavior.
  - `OverrideVanilla`: skip vanilla candy behavior and apply only custom behavior.
- Use reusable weighted outcomes from `outcomes.yml`.
- Configure effects, health, max health, artificial health, max artificial health, hints, explosions, and kill behavior.
- Gate outcomes by role, team, and round elapsed time.
- Control random SCP-330 candy spawn weights.
- Reload config without restarting the server.
- Inspect candy config, outcome config, and effective spawn pool from Remote Admin or server console.
- Track custom death reasons for explicit kills, lethal negative health, tracked lethal candy effects, and tracked candy explosions.

## Requirements

- SCP: Secret Laboratory server
- EXILED `9.14.0`
- .NET Framework `net48` plugin environment

## Install

1. Download `CandyCustomizer.dll` from the latest release.
2. Place it in your EXILED plugins folder.
3. Start the server once so Candy Customizer can generate its config files.
4. Edit the generated config and `outcomes.yml`.
5. Run `candyreload` or restart the server.

## Config Files

Candy Customizer uses two files:

- Main EXILED config: controls candy mode, eating, spawn weight, and outcome weights.
- `outcomes.yml`: controls the actual behavior that can be reused by one or more candies.

The main config decides what each candy can roll:

```yml
candies:
  Rainbow:
    mode: Additive
    can_eat: true
    spawn_weight: -1
    outcomes:
      defensive_heal: 3
      risky_bite: 1
```

`outcomes.yml` defines what those outcomes do:

```yml
outcomes:
  defensive_heal:
    effects:
    - name: DamageReduction
      intensity: 1
      duration: 12
      add_duration_if_active: true
    health: 20
    artificial_health: 25
    hint: You feel protected.
    hint_duration: 3
  risky_bite:
    health: -20
    kill_reason: An unstable candy
    hint: That candy bites back.
```

## Main Fields

- `mode`: `VanillaOnly`, `Additive`, or `OverrideVanilla`.
- `can_eat`: when `false`, blocks eating for custom-mode candies.
- `spawn_weight`: random SCP-330 pool weight. `-1` uses the game's native weight, `0` removes that candy from random rolls, positive values use custom weighting.
- `outcomes`: weighted map of outcome names from `outcomes.yml`.

## Outcome Fields

- `effects`: list of EXILED effect names with intensity, duration, and add-duration behavior.
- `health`: instant health change. Positive heals, negative hurts.
- `max_health`: set max health. `-1` leaves it unchanged.
- `artificial_health`: instant artificial health change. Positive adds AHP, negative removes AHP.
- `max_artificial_health`: set max artificial health. `-1` leaves it unchanged.
- `kill`: explicitly kill the player after applying other configured fields.
- `kill_reason`: death reason used for explicit kills, lethal negative health, tracked lethal effects, and tracked explosions.
- `explode`: trigger `Player.Explode`.
- `hint`: hint shown after custom behavior applies.
- `hint_duration`: hint duration in seconds.
- `conditions`: optional role, team, and round-time conditions.

## Conditions

Outcome conditions are checked before weights are rolled. If an outcome fails its conditions, it is skipped for that roll.

```yml
conditions:
  allowed_roles:
  - ClassD
  denied_roles: []
  allowed_teams:
  - ClassD
  denied_teams: []
  min_round_elapsed_seconds: -1
  max_round_elapsed_seconds: 420
```

Use `-1` for time fields to disable that check.

## Commands

| Command | Aliases | Permission | Purpose |
| --- | --- | --- | --- |
| `candyreload` | `ccreload`, `reloadcandy` | `candycustomizer.reload` | Reloads Candy Customizer config and `outcomes.yml`. |
| `candyinspect <candy>` | `ccinspect`, `inspectcandy` | `candycustomizer.inspect` | Shows one candy's configured behavior. |
| `outcomeinspect <name>` | `ccoutcome`, `inspectoutcome` | `candycustomizer.inspect` | Shows one outcome from `outcomes.yml`. |
| `candypool` | `candyweights`, `ccpool` | `candycustomizer.inspect` | Shows effective random SCP-330 spawn pool weights. |
| `candytest <candy> [player]` | `cctest`, `testcandy` | `candycustomizer.test` | Applies the plugin's custom behavior for testing. Vanilla candy behavior is not simulated. |

## Build

The project can build standalone or as part of the larger solution, as long as the dependency folders exist beside the project or at the workspace root.

```bash
dotnet build CandyCustomizer.sln -c Release
```

The output DLL is:

```text
bin/Release/net48/CandyCustomizer.dll
```
