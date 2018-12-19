# Translation 3

## Background
T3 is my third attempt at a gameplay concept that I have been messing with for three years now. The initial
aim was to make a kinematics simulation program, but I quickly got carried away with the idea of a platformer
featuring complex translational motion and gravity.

## Gameplay
### Objective
The player(s) (1-2) take control of red squares. The aim of each level is to land on and crush all of the sentries:
the other colour squares that patrol the stage platforms. If all players fall off the stage (either down or up),
the level is reset.

### Movement
In addition to side-to-side lateral motion, Translation 3 has three main movement systems:

  1. Jumping & Dropping: The player can `JUMP` when supported by a platform. When supported by a platform,
  the player can `DROP` to fall from it. The player can also `DROP` in the air to accelerate their descent.
  2. Teleportation: The player can `TELEPORT` laterally for short distances. The distance is determined by the duration of
  the key press (this is capped).
  3. Saving & Loading: The player can set a saved position on the stage at any time. When it is loaded, the player instantly
  teleports to the saved location. The player's motion prior to the teleport is preserved.

### Sentries
Sentries are the enemies in Translation 3. Each sentry is rendered in a unique colour and has a unique ability. 
Abilities are either sight-dependent (SD) or sight-independent (SI), meaning that their triggering may be determinant on whether
they can "see" the player. Abilities include:

  - `PUSH`: Pushes the player away from their position laterally (SD)
  - `SPAWN`: Spawns a new sentry on the stage every 100 ticks (capped at 5 per spawner; SI)
  - `NECROMANCER`: Resurrects a crushed sentry every 100 ticks (transitively closed
  [all sentries resurrected by `NECROMANCER` A die when A dies]; SI)
  - `DECAY`: Shrinks the patrolling platform (SD)
  - `GRAV_FLIP`: Multiplies the player's grav. acceleration by -1 (SD)
  - `GRAV_RED`: Reduces the gravitational acceleration (SI)
  - And many more...

## This Repository
I am developing the game as a C# project Visual Studio solution. The source code consists of the `.cs`
files in the [`/TRANSLATION3/`](https://github.com/jbunke/translation3/tree/master/TRANSLATION3) directory.
Resource files (font reference images & levels) are in
[`/TRANSLATION3/Resources`](https://github.com/jbunke/translation3/tree/master/TRANSLATION3/Resources).

## Updates
Will be playable once level editor is implemented, core campaigns are fully designed, and menus are properly linked.

Email me at jtb17@ic.ac.uk if you have any questions.
