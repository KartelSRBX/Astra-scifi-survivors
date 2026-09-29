# GamingApp — SciFi survivors-like (Android)

Unity skeleton for the roadmap in `SciFi рогалик с волнами (Android) — Roadmap.md`.
Scripts only: no scene, prefabs or art yet. Nothing here has been opened in Unity or compiled.

## Open the project
1. Install Unity 6 (6000.0 LTS) with the Android Build Support module.
2. Unity Hub > Add > select this folder. `ProjectSettings/ProjectVersion.txt` is a placeholder; Unity will offer to upgrade/fix it.
3. Input uses the legacy `UnityEngine.Input` (touch, mouse fallback in the editor), so keep *Active Input Handling* on "Input Manager (Old)" or "Both".

## Layout (`Assets/Scripts`)
| Folder | Scripts |
|---|---|
| Core | `GameManager` (run timer, state, best time), `Health`, `TopDownCamera` |
| Player | `PlayerController` (drag movement), `PlayerExperience` (XP/level), `PlayerHealth` |
| Weapons | `AutoWeapon` (auto-target), `Projectile`, `UpgradeData` (ScriptableObject), `UpgradeManager` |
| Enemies | `EnemyData` (ScriptableObject), `Enemy`, `WaveSpawner`, `XpGem` |
| UI | `Hud`, `LevelUpPanel`, `ResultPanel` |

## Building the first scene (manual, in the editor)
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
