using System;

//[Flags]
public enum Enum_StateList
{
    jump = 1 << 0,
    idle = 1 << 1,
    walk = 1 << 2,
    fall = 1 << 3,
    dash = 1 << 4,
    slide = 1 << 6,
    wallEnter = 1 << 7,
    wallSlide = 1 << 5,
    wallRun = 1 << 8,
    wallJump = 1 << 9,
}
[Flags]
public enum Enum_TransitionList
{
    idleToJump = 1 << 0
}