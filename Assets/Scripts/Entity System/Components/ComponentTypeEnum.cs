using System;

[Flags]

public enum ComponentType
{
    Health = 1 << 1,
    Movement = 1 << 2,
    Animation = 1 << 3,
    Audio = 1 << 4
}