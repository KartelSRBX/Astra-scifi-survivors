# GamingApp — SciFi survivors-like (Android)

Unity skeleton for the roadmap in `SciFi рогалик с волнами (Android) — Roadmap.md`.
Scripts plus an editor tool that generates a playable placeholder scene. No real art yet.

## Open the project
1. Install Unity 6000.6.3f1 with the Android Build Support module.
2. Unity Hub > Add > select this folder.
3. Input uses the legacy `UnityEngine.Input` (touch, mouse fallback in the editor), so keep *Active Input Handling* on "Input Manager (Old)" or "Both".

## Quick start: playable test scene
1. Open the project in Unity and wait for the import to finish. If `Assets/Scripts` throws compile errors, fix them first (see "Status").
2. Menu **Tools > GamingApp > Build Test Scene**. It creates placeholder prefabs (primitives), enemy/upgrade data in `Assets/Data`, and `Assets/Scenes/Main.unity`, and applies Android player settings.
3. Open `Main.unity` and press Play. Hold the mouse (or a finger on device) and drag to move; the character auto-fires at the nearest enemy. Kill enemies, pick up blue XP gems, choose an upgrade on level-up. The run lasts 5 minutes in the test build (`TestRunSeconds` in `Editor/SceneBuilder.cs`; the roadmap target is 10-15).
4. To test on a phone: File > Build Profiles > Android > Build And Run (needs the Android module + a device with USB debugging).

The builder is safe to re-run; it regenerates the scene and assets. The UI uses legacy uGUI `Text` (no TextMeshPro import needed); switch to TMP together with the localization work.

## Status
Compiles and the test scene builds in Unity 6000.6.3f1 (the project's current editor). Play-mode behaviour and balance are still being playtested. Android Build Support must be installed for the same editor version to build APK/AAB.

## Layout (`Assets/Scripts`)
| Folder | Scripts |
|---|---|
| Core | `GameManager` (run timer, state, best time), `Health`, `TopDownCamera` |
| Player | `PlayerController` (drag movement), `PlayerExperience` (XP/level), `PlayerHealth` |
| Weapons | `AutoWeapon` (auto-target), `Projectile`, `UpgradeData` (ScriptableObject), `UpgradeManager` |
| Enemies | `EnemyData` (ScriptableObject), `Enemy`, `WaveSpawner`, `XpGem` |
| UI | `Hud`, `LevelUpPanel`, `ResultPanel` |
| Editor | `SceneBuilder` (menu entry), `AssetBuilder`, `UiBuilder`, `AndroidSettings`, `BuilderUtil` |

## Building the scene by hand (only if you skip the builder)
1. Ground plane + Directional Light. Camera with `TopDownCamera` targeting the player.
2. Player: capsule with `CharacterController`, `Health`, `PlayerHealth`, `PlayerController`, `PlayerExperience`, and a child/same-object `AutoWeapon` (assign a projectile prefab).
3. Projectile prefab: small sphere, trigger `SphereCollider`, kinematic `Rigidbody`, `Projectile`.
4. Enemy prefab: capsule, trigger collider, kinematic `Rigidbody`, `Health`, `Enemy`. Create 3 `EnemyData` assets (swarmer, tank, ranged) plus a mini-boss (`isMiniBoss`, `unlockAtProgress` 0.5 and 1.0-ish). Create an `XpGem` prefab and assign it on the Enemy prefab.
5. Empty `GameManager` object; empty `WaveSpawner` object (assign player + EnemyData list).
6. `UpgradeManager` with a few `UpgradeData` assets (damage, fire rate, +projectile, move speed, max HP, pickup radius).
7. Canvas with `Hud`, `LevelUpPanel` (3 buttons), `ResultPanel` (Restart button -> `Restart()`).
8. Player Settings: Android, IL2CPP + ARM64, target API level 36, build AAB.

## Not done yet
Localization (EN/RU), ads + consent, privacy policy, Data Safety, art, sound, second map, real balance.

## UI (dark theme)
Menu scene (Play / Settings / best time), in-game HUD, pause with volume, level-up cards,
result screen with menu button, floating on-screen joystick. Palette lives in `Assets/Scripts/UI/UiTheme.cs`.
Rebuild everything with Tools > GamingApp > Build Test Scene, then Play from the Menu scene.
All menu art is procedural (`Assets/Scripts/Editor/UiArt.cs`, engine-independent) and written to
`Assets/Art/UI/Generated` by `UiSprites` on each build.

## Ships, enemies, items
- Player is a warship. Ships (Corvette -> Frigate -> Destroyer -> Cruiser -> Dreadnought) are `ShipData`; bought and upgraded (Lv 1-10) in the Hangar.
- 10 enemy tiers, black/red: the darker the hull, the stronger the enemy. Tier 9 (Warden) and 10 (Leviathan) are bosses.
- Items (`ItemData`): Weapon / Hull / Engine / Module slots, 4 rarities. Equip in Inventory. Runs award Biomass, Cores and a chance for an item.
- Everything is listed in `Assets/Resources/GameDatabase.asset`; progress is saved by `Profile` (PlayerPrefs JSON).

## Weapons
- 8 weapons picked before a run (START -> Choose weapon): Blaster, Laser, CIWS, Heavy Missiles, Swarm Missiles, Plasma Cannon, Drone Swarm, Heavy Drones (`WeaponDef`, built by `WeaponBuilder`).
- Player shots home in on the nearest enemy; missiles explode (AoE), plasma pierces, drones orbit and fire on their own.
- Each weapon has 5 in-run steps offered on level-up (marked WEAPON), on top of generic upgrades like +% damage.
- Weapon-slot items now tune whichever gun is picked (damage, fire rate, +projectile).
