# PVE Voxel Hit Calculation Service

This project complements the Dual Universe PVE mod by adding the ability correctly calculate dynamic voxel meshes hits from a raycast.

The challenge with modding in Dual Universe, is that these calculations are done on the client (Game client) side and the NPCs don't run on a game client.

When building the PVE mod, the AI needs to behave the same as a player would, and that include hits.

This project then performs optimized calculations based on where the player and NPC ships to hit another construct or ship.

The service holds a maximum of a large cube of voxel data stored and voxel coordinates are bit compressed for better memory efficiency.

NPCs then use this service's API to calculate where to hit on a target ship.

## Sequence of events using this project:

Ship and construct words are used interchangeably here but mean the same on the context of the game.

- NPC targets a ship/construct.
- NPC requests target ship voxelized mesh to be cached by PVE Voxel Service.
  - Target ship is enqueued for mesh updates and Voxelization caching.
- NPC shoots at ship.
- PVE MOD Backend calculates hit probability.
- NPC hits a ship.
- NPC requests damage area of target ship by calling PVE Voxel Service.
- PVE Voxel Service loads or retrieves from memory the voxelized mesh of target ship
- PVE Voxel Service calculates hit location based on parameters provided by shooter ship (positions, distance - raycast).
- PVE Voxel Service returns hit position to affect voxel damage.
- PVE Mod Backend affects the damage area and consume voxel chunks.
- Game Backend edits the construct's voxels to match damage.
- (After a significant amount of time has elapsed and the target construct is not being targeted)
- PVE Voxel Service discards memory data of the target ship (Cache invalidation).
