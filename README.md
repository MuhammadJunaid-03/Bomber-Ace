# Bomber Ace - WW2 War Plane Game

A WW2 arcade bomber game built with **Unity 6** (6000.0.58f2) featuring top-down aerial combat, bomb dropping, and ground target destruction.

## Gameplay

Fly your bomber plane over enemy territory, drop bombs on ground targets, shoot down enemy aircraft, and complete missions to earn coins and upgrades.

- Auto-forward flight with arcade-style steering
- Drop bombs on tanks, trucks, AA guns, buildings, and radars
- Shoot enemy fighter planes and helicopters
- Avoid anti-aircraft fire and homing missiles
- Upgrade your plane's engine, armor, wings, weapons, and bombs
- Progress through missions with increasing difficulty

## Controls

| Key | Action |
|-----|--------|
| A/D or Left/Right Arrow | Steer left/right |
| W/S or Up/Down Arrow | Climb/dive |
| Mouse Drag | Steer (mobile-style) |
| Space / Right-click | Drop bombs |
| F | Shoot guns |
| R | Reload ammo |
| Esc | Pause game |

## Project Structure

```
Assets/
├── Materials/          # URP Lit materials (terrain, plane, enemies, weapons)
├── Prefabs/
│   ├── Effects/        # Explosion prefab
│   ├── Enemies/        # Tank, Truck, AAGun, Building, Radar, EnemyPlane, EnemyHelicopter
│   └── Weapons/        # Bomb, Bullet, HomingMissile
├── Scenes/
│   ├── SampleScene.unity   # Main game scene
│   └── MainMenu.unity      # Menu scene
├── Scripts/
│   ├── Camera/         # BomberCamera (top-down chase), CameraShake
│   ├── Core/           # GameManager, CurrencyManager, UpgradeManager, MissionData
│   ├── Editor/         # Build settings & prefab material setup utilities
│   ├── Effects/        # Explosion effect
│   ├── Enemies/        # GroundTarget, EnemyPlane, EnemyHelicopter, EnemyHealth
│   ├── Environment/    # TerrainGenerator (procedural), MissionSpawner
│   ├── Player/         # PlaneController, PlaneHealth, BombDropper, PlaneGun, PlaneSetup
│   ├── UI/             # HUDController (Canvas-based), GameOverUI, UpgradeUI, MainMenuUI
│   └── Weapons/        # Bomb, Bullet, HomingMissile
└── UI/                 # UI Toolkit assets (UXML/USS)
```

## Features

### Core Systems
- **Flight Controller** - Auto-forward arcade flight with smooth banking and altitude control
- **Bomb System** - Physics-based bombs with area damage and explosion effects
- **Gun System** - Dual-mounted machine guns with ammo and reload
- **Camera** - Top-down angled chase camera (Bomber Ace style)

### Combat
- **Ground Targets** - Tanks, trucks, AA guns, radars, buildings with varied health and score values
- **AA Guns** - Enemy anti-aircraft guns that shoot at the player with lead prediction
- **Enemy Planes** - AI fighters with patrol/chase/attack/disengage behavior states
- **Enemy Helicopters** - Hovering enemies with guns and homing missiles
- **Homing Missiles** - Track the player with configurable turn speed

### Environment
- **Procedural Terrain** - Infinite scrolling desert terrain using Perlin noise with mesh colliders
- **Mission Spawner** - Spawns convoys and scattered targets based on mission configuration

### Progression
- **Upgrade System** - 5-level upgrades for engine, armor, wings, bombs, and guns
- **Currency** - Coins earned from score, gems from mission completion
- **Plane Data** - ScriptableObject-based plane stats and upgrade costs
- **Mission Data** - ScriptableObject-based mission configuration

### UI
- **HUD** - Score, health bar, bomb/ammo count, target counter, speed, altitude
- **Game Over Panel** - Score display with Retry and Main Menu buttons
- **Mission Complete Panel** - Score, destruction %, coins earned, Next Mission button
- **Pause Panel** - Resume, Retry, Main Menu buttons
- **Damage Overlay** - Red flash on hit, persistent tint at low health
- **Missile Warning** - Alert when homing missiles are nearby

## Tech Stack

- **Unity 6** (6000.0.58f2)
- **Universal Render Pipeline** (URP 17.0.4)
- **Input System** (1.14.2)
- **Cinemachine** (3.1.6)
- **TextMeshPro** for UI text
- **C#** with Unity MonoBehaviour architecture

## Setup

1. Clone this repository
2. Open with **Unity 6** (6000.0.58f2 or compatible)
3. Open `Assets/Scenes/SampleScene.unity`
4. Press **Play**

## Status

Work in progress - core gameplay loop is functional with plane flight, bombing, target spawning, and UI. Further polish and gameplay matching planned.

## License

This project is for educational and personal use.
