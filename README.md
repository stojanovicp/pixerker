# Pixerker

**Play in browser:** https://stojanovicp.itch.io/pixerker

A 2D top-down action RPG built in Unity 6 (URP 2D, Cinemachine 3, Input System). Fight through ten connected areas, defeat four bosses to collect their weapons, and take down the Black Ghost to win.

The project started from a Unity course and was significantly extended afterwards: inventory, new weapons, enemy AI, bosses, world persistence, death drops and menus were all added on top of the course base.

## Controls

| Action | Input |
|---|---|
| Move | `W` `A` `S` `D` |
| Aim | Mouse |
| Attack | Left mouse button |
| Dash (uses stamina) | `Space` |
| Select weapon slot | `1` – `5` |
| Drop active weapon | `Q` |
| Pause | `Esc` or `P` |

## Features

**Weapons** — five physical weapons, each existing once in the world. Pick one up to place it in the first free slot; drop it with `Q` to reorder. Cooldowns are tracked per weapon and shown on the slot.
- **Sword** — melee with knockback; your starting weapon, cannot be dropped.
- **Bow** — fires arrows.
- **Staff** — fires a magic laser.
- **Lightning Bomb** — lobbed toward the cursor; on impact, procedural lightning bolts strike the area with a 2D light flash, damaging and knocking back everything in the blast.
- **Life Steal Grimoire** — fires a glowing red bolt that executes a normal enemy outright (bosses lose a share of their max HP) and heals you for half the damage dealt.

**Enemies** — Slime (contact damage), Grape (lobs grapes that leave a damaging splatter), Ghost and Red Ghost (bullet patterns). Enemies roam, chase the player once detected and attack from their preferred range.

**Bosses** — five boss fights: Slime, Ghost, Grape, Red Ghost and the final Black Ghost.
- Two phases: below 50% HP a boss switches to a harder set of attack patterns and summons more often.
- Bosses periodically summon minions of their type; the Black Ghost summons one of every type.
- Each of the first four drops a weapon and opens its arena exit on death. Defeating the Black Ghost wins the game.

**Death and drops** — on death you respawn at the start with only the Sword. Every other weapon you carried and half of your gold scatter where you fell, waiting for you to come back for them.

**Persistent world** — killed enemies and bosses, broken bushes and dropped items stay that way for the whole session, across scene transitions and deaths. An unfinished boss is back at full health when you return.

**Also** — health, stamina and gold HUD; health, stamina and gold pickups; main menu and victory screen with score; starting a new game fully resets the session.

## Technical highlights

- **Enemy FSM** — a small plain-C# state machine (`IState`, `StateMachine`) drives Roam → Chase → Attack, with per-enemy detection range, lose range (hysteresis) and stopping distance.
- **Data-driven enemies** — `EnemyStats` ScriptableObjects hold health, speed, ranges and contact damage. `AttackPattern` ScriptableObjects describe bursts, spread, oscillation, timing and bullet damage, and are shared by shooters and grape throwers. Bosses hold a list of patterns per phase and pick one at random for each attack.
- **Damage pipeline** — every hit carries a `DamageInfo` (amount, knockback, source position, flags) captured when the attack is spawned, and is delivered through `IDamageable`. Enemies, bosses and destructible bushes share the same path; the `Execute` flag powers the Life Steal Grimoire.
- **World state** — an in-memory `WorldState` records removed objects per scene. Scene-placed objects carry a `PersistentObject` component with a stable ID and remove themselves on load if already destroyed. Dropped weapons and death coins are stored as records and respawned when their scene loads.
- **Event-driven HUD** — health, stamina, gold, boss health and the victory screen subscribe to C# events instead of polling or looking objects up by name.
- **Prefab variants** — Red Ghost is a variant of Ghost, and every boss is a variant of its base enemy with scaled stats and size plus a `Boss` component for phases and summons.

## Requirements

Unity 6 (6000.6.9f1). Open the project and play `Assets/Scenes/Menu/MainMenu.unity`.
