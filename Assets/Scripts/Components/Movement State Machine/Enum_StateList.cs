using System;

//[Flags]
public enum Enum_StateList
{
    jump = 1 << 0,
    idle = 1 << 1,
    walk = 1 << 2,
    fall = 1 << 3,
    dash = 1 << 4,
    wallslide = 1 << 5,
    slide = 1 << 6,
}
[Flags]
public enum Enum_TransitionList
{
    idleToJump = 1 << 0
}