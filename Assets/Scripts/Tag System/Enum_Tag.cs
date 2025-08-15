using System;

[Flags]
public enum Enum_Tag
{
    canRespawn = 1 << 0,
    enemy = 1 << 1,
    player = 1 << 2,

    interactable = 1 << 3,      // For objects that can be used (e.g., buttons, doors)
    collectible = 1 << 4,       // For pickups like health, ammo, or keys
    hazard = 1 << 5,            // For environmental dangers (e.g., spikes, lava)
    destructible = 1 << 6,      // For objects that can be destroyed
    trigger_volume = 1 << 7,    // For invisible triggers that start events or cutscenes

    projectile = 1 << 8,        // For bullets, rockets, etc. (from player or enemy)
    cover = 1 << 9,             // For AI to identify cover points
    friendly_npc = 1 << 10,     // For allied or neutral characters

    slippery_surface = 1 << 11, // For surfaces like ice that affect movement
    no_wall_run_surface = 1 << 12, // To prevent wall running on specific surfaces
    climbable_surface = 1 << 13 // For ledges or ladders
}