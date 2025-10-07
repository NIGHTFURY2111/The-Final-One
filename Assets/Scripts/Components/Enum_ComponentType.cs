using System;

//[Flags]

public enum Enum_ComponentType
{
    Movement = 1 << 0,
    Camera = 1 << 1,
    CameraEffects = 1 << 2,
    RigidBody = 1 << 3,
    Detector = 1 << 4,
    Health = 1 << -1,
    Animation = 1 << -1,
    Audio = 1 << -1
}