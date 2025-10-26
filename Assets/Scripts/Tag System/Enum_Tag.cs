using System;

[Flags]
public enum Enum_Tag
{
    canRespawn = 1 << 1,
    entity = 1 << 2,
    enemy = 1 << 3,
    player = 1 << 4,

    interactable = 1 << 5,      // For objects that can be used (e.g., buttons, doors)
    collectible = 1 << 6,       // For pickups like health, ammo, or keys

    projectile = 1 << 7,        // For bullets, rockets, etc. (from player or enemy)
    cover = 1 << 8             // For AI to identify cover points

}