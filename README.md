# PVE Voxel Hit Calculation Service

This project complements the PVE mod by adding the ability correctly calculate dynamic voxel meshes hits from a raycast.

The challenge with modding in Dual Universe, is that these calculations are done on the client side. 

When building the PVE mod, the AI needs to behave the same as a player would, and that include hits.

This project then performs optimized calculations based on where the player and NPC ships to hit another construct or ship.

The service holds a maximum of a large cube of voxel data stored and voxel coordinates are bit compressed for better memory efficiency.

NPCs then use this service's API to calculate where to hit on a target ship.
