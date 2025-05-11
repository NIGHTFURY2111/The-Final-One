using System;

[Flags]

public enum ComponentType
{
    Movement = 1 << 0,
    Camera = 1 << 1,
    RigidBody = 1 << 2,
    Health = 1 << -1,
    Animation = 1 << -1,
    Audio = 1 << -1
}