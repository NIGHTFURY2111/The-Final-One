# Platform System - Quick Start Guide

## ✅ What Was Fixed/Added

### 1. **Fixed Rotation Tweening** ✓
- The original `ShiftingPlatform.cs` now properly tweens rotation and scale
- Uses DOTween Sequence to animate position, rotation, and scale together
- All transforms are smoothly interpolated

### 2. **Created Modular Platform System** ✓
Following your project's coding patterns using **MonoBehaviour components**:

```
Platform System Architecture:
├── AC_PlatformBehaviour (Abstract MonoBehaviour Base Class)
└── Platform Components (MonoBehaviours - each instance has unique data):
    ├── EC_ShiftingPlatform
    ├── EC_CyclicPlatform
    └── EC_DisappearingPlatform
```

**Key Feature**: Each platform instance has its own component with **unique transform data** - no shared state issues!

---

## 🎮 Platform Types Overview

### EC_ShiftingPlatform
**What it does**: Moves between two positions when triggered
- ✓ Each instance records its own positions
- ✓ Manual or auto-start
- ✓ Can toggle between positions
- ✓ Great for: doors, gates, activated platforms

### EC_CyclicPlatform
**What it does**: Continuously moves between two points
- ✓ Each instance has unique points and cycle state
- ✓ Two modes: PingPong (A→B→A) or Loop (A→B→snap to A)
- ✓ Configurable wait time at points
- ✓ Start/stop/pause controls
- ✓ Great for: moving platforms, elevators, patrol routes

### EC_DisappearingPlatform
**What it does**: Disappears when player jumps off
- ✓ Each instance tracks its own state
- ✓ Detects player automatically
- ✓ Warning flash before disappearing
- ✓ Scale + fade animations
- ✓ Auto-respawn with delay
- ✓ Great for: breakable platforms, timed challenges

---

## 🚀 Quick Setup (2 Steps!)

### Step 1: Create Platform GameObject
```
1. Create Cube (or any mesh with collider)
2. Add Rigidbody component
3. Set Rigidbody to Kinematic
```

### Step 2: Add Platform Component
```
Add Component > Platform > [Choose Type]:
- Shifting Platform
- Cyclic Platform
- Disappearing Platform
```

**That's it!** Each component has its own inspector with recording buttons.

---

## 📝 Example Workflows

### Making a Moving Platform (Cyclic)
```
1. Create platform GameObject with Rigidbody (kinematic)
2. Add Component > Platform > Cyclic Platform
3. Position platform at point A
4. Click "Record Point A" in inspector
5. Move platform to point B
6. Click "Record Point B"
7. Set "Start Immediately" to true
8. Play! Platform now loops between points
```

### Making a Breakable Platform (Disappearing)
```
1. Create platform GameObject with Rigidbody (kinematic)
2. Add Component > Platform > Disappearing Platform
3. Configure in inspector:
   - Player Layer: "Player"
   - Disappear Delay: 0.5s
   - Respawn: true
   - Respawn Delay: 3s
4. Play! Platform breaks when player leaves
```

### Making an Activated Door (Shifting)
```
1. Create door GameObject with Rigidbody (kinematic)
2. Add Component > Platform > Shifting Platform
3. Position door closed
4. Click "Record Original"
5. Move door to open position
6. Click "Record Target"
7. Set "Auto Shift On Start" to false
8. Call GetComponent<EC_ShiftingPlatform>().ShiftToTarget() from script
```

---

## 🎨 Editor Features

### Recording Buttons (Shifting/Cyclic)
- **Green "Record Original/Point A"**: Save current position as first transform
- **Yellow "Record Target/Point B"**: Save current position as second transform

### Preview Buttons
- **Preview Original/Point A**: Instantly move to first position (no animation)
- **Preview Target/Point B**: Instantly move to second position (no animation)

### Runtime Controls (Play Mode Only)
- Test your platforms while game is running
- Buttons appear in inspector during play mode

---

## 🔧 Accessing from Code

### Direct Component Access
```csharp
EC_ShiftingPlatform shifting = GetComponent<EC_ShiftingPlatform>();
shifting.ShiftToTarget();
```

### Control Cyclic Platform
```csharp
EC_CyclicPlatform cyclic = GetComponent<EC_CyclicPlatform>();
cyclic.StartCycle();
cyclic.StopCycle();
cyclic.PauseCycle();
```

### Trigger Disappearing Platform
```csharp
EC_DisappearingPlatform disappearing = GetComponent<EC_DisappearingPlatform>();
disappearing.TriggerDisappear();
disappearing.TriggerRespawn();
```

---

## 📂 File Locations

### Core System
```
Assets/Scripts/GamePlay Scripts/Platform System/
├── AC_PlatformBehaviour.cs          (Base class)
├── EC_ShiftingPlatform.cs           (Component)
├── EC_CyclicPlatform.cs             (Component)
├── EC_DisappearingPlatform.cs       (Component)
└── README.md                        (Full docs)
```

### Editor Scripts
```
Assets/Scripts/Editor/
├── PlatformEditor.cs                (Custom inspectors)
└── ShiftingPlatformEditor.cs        (Legacy support)
```

### Legacy
```
Assets/Scripts/GamePlay Scripts/
└── Shifting Platform.cs             (LEGACY - kept for compatibility)
```

---

## ⚙️ Common Settings

### Movement Settings (All Types)
- **Move Duration**: 1.0 (seconds to complete movement)
- **Ease Type**: InOutQuad (smooth acceleration/deceleration)

### Cyclic Platform
- **Cycle Mode**: PingPong (most common)
- **Wait Time At Points**: 0.5 (brief pause at ends)
- **Start Immediately**: true

### Disappearing Platform
- **Disappear Delay**: 0.5 (reaction time for player)
- **Disappear Duration**: 0.3 (fade out time)
- **Respawn**: true
- **Respawn Delay**: 3.0 (time until respawn)
- **Detection Box Size**: (1, 0.2, 1) (matches platform size)

---

## 🎯 Best Practices

1. **Always use Kinematic Rigidbody** for moving platforms
2. **Record transforms in edit mode** before play testing
3. **Use Preview buttons** to visualize movement
4. **Test detection areas** with Scene view gizmos (Disappearing)
5. **Match detection box** to platform size (Disappearing)
6. **Set proper layer mask** for player detection (Disappearing)
7. **Each platform is independent** - no shared state between instances!

---

## ✨ Why MonoBehaviour Components?

### Problem with ScriptableObjects:
- ❌ Shared state across all instances
- ❌ All platforms using same SO would have same recorded positions
- ❌ Can't have unique settings per platform

### Solution with MonoBehaviour:
- ✅ Each platform has its own component instance
- ✅ Each platform can have unique recorded positions
- ✅ Each platform can have different settings
- ✅ Simple workflow: just add component!

---

## 🔄 Migration from Old ShiftingPlatform

### Option 1: Keep Using Legacy (No Changes Required)
- Your existing ShiftingPlatform scripts still work
- Rotation is now fixed
- No migration needed

### Option 2: Upgrade to New System
1. Note your old settings (positions, duration, ease)
2. Remove old ShiftingPlatform component
3. Add EC_ShiftingPlatform component
4. Apply noted settings
5. Record original and target transforms

**Note**: The new system is simpler - just add the component directly to your GameObject!

---

## 📞 Need Help?

See full documentation: `Assets/Scripts/GamePlay Scripts/Platform System/README.md`

Happy platforming! 🎮
