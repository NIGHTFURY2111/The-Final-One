using System;

//[Flags]

public enum Enum_ComponentType
{
    Movement = 1 << 0,
    Camera = 1 << 1,
    Camera_Effects = 1 << 2,
    RigidBody = 1 << 3,
    Detector = 1 << 4,
    Behaviour_Tree = 1 << 5,
    Health = 1 << -1,
    Animation = 1 << -1,
    Audio = 1 << -1
}