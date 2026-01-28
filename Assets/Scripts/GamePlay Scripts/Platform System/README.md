# Platform System Documentation

## Overview
This modular platform system follows the project's established coding patterns using **MonoBehaviour components**. Each platform instance has its own unique data - no shared state issues!

## Architecture

### Core Classes

#### `AC_PlatformBehaviour` (Abstract MonoBehaviour Base)
- **Location**: `Assets/Scripts/GamePlay Scripts/Platform System/AC_PlatformBehaviour.cs`
- **Purpose**: Abstract base class for all platform behaviors
- **Type**: MonoBehaviour (each instance has unique data)
- **Key Features**:
  - TransformData class for storing position, rotation, and scale
  - DOTween-based movement with physics support
  - Lifecycle hooks: OnBehaviourStart, OnBehaviourUpdate, OnBehaviourFixedUpdate, OnBehaviourDisable, OnBehaviourDestroy
  - RequireComponent(typeof(Rigidbody))

---

## Platform Types

### 1. EC_ShiftingPlatform
**File**: `Assets/Scripts/GamePlay Scripts/Platform System/EC_ShiftingPlatform.cs`

**Description**: Platform that shifts between two recorded positions when triggered. Each instance stores its own unique transform data.

**Setup**:
1. Add component to GameObject: `Add Component > Platform > Shifting Platform`
2. Ensure Rigidbody is present (added automatically)
3. Set Rigidbody to Kinematic

**Features**:
- Record original and target transforms (unique per instance)
- Shift on command or auto-shift on start
- Preview transforms in editor
- Toggle between positions

**Inspector Settings**:
- `Original Transform`: First position/rotation/scale (instance-specific)
- `Target Transform`: Second position/rotation/scale (instance-specific)
- `Auto Shift On Start`: Whether to automatically shift when the game starts
- `Shift To Target On Start`: Which direction to shift first
- `Move Duration`: Time to complete the movement
- `Ease Type`: DOTween ease curve

**Public Methods**:
```csharp
void RecordOriginalTransform()    // Record current position as original
void RecordTargetTransform()      // Record current position as target
void PreviewOriginalTransform()   // Preview original in editor
void PreviewTargetTransform()     // Preview target in editor
void ShiftToTarget()              // Move to target position
void ShiftToOriginal()            // Move to original position
void Toggle()                     // Toggle between positions
```

**Use Cases**:
- Doors/gates that open and close
- Platforms that move when activated
- Moving walls
- Rotating bridges

---

### 2. EC_CyclicPlatform
**File**: `Assets/Scripts/GamePlay Scripts/Platform System/EC_CyclicPlatform.cs`

**Description**: Platform that continuously moves between two points in a loop. Each instance has its own unique points and cycle state.

**Setup**:
1. Add component to GameObject: `Add Component > Platform > Cyclic Platform`
2. Ensure Rigidbody is present (added automatically)
3. Set Rigidbody to Kinematic

**Features**:
- Ping-pong mode (A → B → A → B)
- Loop mode (A → B → snap to A → B)
- Configurable wait time at each point
- Start/stop/pause/resume control
- Auto-start option
- Each instance tracks its own cycle state

**Inspector Settings**:
- `Point A`: First position/rotation/scale (instance-specific)
- `Point B`: Second position/rotation/scale (instance-specific)
- `Cycle Mode`: PingPong or Loop
- `Wait Time At Points`: Delay at each endpoint
- `Start Immediately`: Begin cycling on start
- `Start At Point A`: Initial position
- `Move Duration`: Time to move between points
- `Ease Type`: DOTween ease curve

**Public Methods**:
```csharp
void RecordPointA()               // Record current position as point A
void RecordPointB()               // Record current position as point B
void PreviewPointA()              // Preview point A in editor
void PreviewPointB()              // Preview point B in editor
void StartCycle()                 // Begin cycling
void StopCycle()                  // Stop cycling
void PauseCycle()                 // Pause movement
void ResumeCycle()                // Resume movement
void ResetToPointA()              // Reset to starting position
```

**Use Cases**:
- Continuously moving platforms
- Patrol routes for moving obstacles
- Pendulum-style traps
- Elevator systems

---

### 3. EC_DisappearingPlatform
**File**: `Assets/Scripts/GamePlay Scripts/Platform System/EC_DisappearingPlatform.cs`

**Description**: Platform that disappears after the player leaves it. Each instance maintains its own state and settings.

**Setup**:
1. Add component to GameObject: `Add Component > Platform > Disappearing Platform`
2. Ensure Rigidbody and MeshRenderer are present
3. Set Rigidbody to Kinematic
4. Configure Player Layer for detection

**Features**:
- Player detection via Physics.CheckBox
- Configurable disappear delay
- Visual warning (color flash) before disappearing
- Scale and fade animations
- Automatic respawn with delay
- Manual trigger support
- Each instance tracks its own disappear/respawn state

**Inspector Settings**:
- `Disappear Delay`: Time before platform disappears after player leaves
- `Disappear Duration`: Animation time for disappearing
- `Respawn`: Whether platform respawns
- `Respawn Delay`: Time before respawn
- `Use Scale Animation`: Animate scale to zero
- `Use Fade Animation`: Fade out alpha
- `Disappear Curve`: Animation curve for disappearing
- `Warning Color`: Color to flash before disappearing
- `Warning Flash Speed`: Speed of warning flash
- `Player Layer`: LayerMask for player detection
- `Detection Distance`: Height above platform to check
- `Detection Box Size`: Size of detection area

**Public Methods**:
```csharp
void TriggerDisappear()           // Manually trigger disappear
void TriggerRespawn()             // Manually trigger respawn
void ResetPlatform()              // Reset to initial state
```

**Use Cases**:
- Breakable ice platforms
- Crumbling platforms in platformers
- Temporary footholds
- Timed challenge platforms

---

## Setup Guide

### Creating a New Platform

1. **Create Platform GameObject**:
   - Add a GameObject with a mesh (cube, custom model, etc.)
   - Add a Collider component if not present

2. **Add Platform Component**:
   - Click `Add Component > Platform > [Platform Type]`
   - Rigidbody will be added automatically if missing
   - Set Rigidbody to **Kinematic**

3. **Configure in Editor**:
   - Position the platform where you want it to start
   - For shifting/cyclic platforms, use the "Record" buttons in the inspector
   - Adjust movement settings (duration, ease type, etc.)

### Example: Shifting Platform

```
1. Create a cube GameObject named "DoorPlatform"
2. Add Component > Platform > Shifting Platform
3. Set Rigidbody to Kinematic (added automatically)
4. Position door in closed position
5. Click "Record Original" in inspector
6. Move door to open position
7. Click "Record Target"
8. Configure Auto Shift settings
```

---

## Editor Features

### Component-Specific Editors
Each platform type has its own custom editor:
- **EC_ShiftingPlatformEditor**: Recording and preview buttons for shifting platforms
- **EC_CyclicPlatformEditor**: Recording and preview buttons for cyclic platforms
- **EC_DisappearingPlatformEditor**: Runtime control buttons for disappearing platforms

### Recording Buttons
- **Recording buttons** for transform positions (per instance)
- **Preview buttons** to visualize positions in edit mode
- **Runtime controls** for testing in play mode

### Gizmos
- **Disappearing Platform**: Shows green detection box in scene view when selected

---

## Integration with Existing Systems

### Component Pattern
Platforms use MonoBehaviour components similar to your Entity system:
- Each instance has unique data (no shared state)
- Lifecycle methods (Awake, Start, Update, etc.)
- Modular and reusable
- Can be added via Component menu

### Physics
Platforms use Rigidbody for physics interactions:
- Moving platforms affect player velocity
- Proper collision detection
- Supports moving platforms in platformer games
- RequireComponent ensures Rigidbody is present

### DOTween Integration
All platforms use DOTween for smooth animations:
- Position, rotation, and scale tweening
- Customizable ease curves
- Sequence support for complex animations

---

## Why MonoBehaviour Instead of ScriptableObject?

### The Problem with ScriptableObjects
ScriptableObjects are **shared assets** - all instances reference the same data:
- ❌ Multiple platforms using same SO would share recorded positions
- ❌ Changing settings on one platform affects all others using same SO
- ❌ State is shared across all instances
- ❌ Can't have unique transform data per platform

### The MonoBehaviour Solution
Each platform gets its own component instance:
- ✅ Each platform has unique recorded positions
- ✅ Each platform can have different settings
- ✅ State is per-instance, not shared
- ✅ Simpler workflow: just add component!
- ✅ Follows Unity's standard component pattern

---

## Legacy Support

### ShiftingPlatform.cs (LEGACY)
The original `ShiftingPlatform` MonoBehaviour is kept for backwards compatibility:
- **Location**: `Assets/Scripts/GamePlay Scripts/Shifting Platform.cs`
- **Status**: Marked as LEGACY in comments
- **Recommendation**: Use new EC_ShiftingPlatform for new platforms
- **Reason**: New system is more modular and follows project patterns

---

## Advanced Usage

### Creating Custom Platform Behaviors

1. **Inherit from AC_PlatformBehaviour**:
```csharp
[AddComponentMenu("Platform/My Platform")]
public class EC_MyPlatform : AC_PlatformBehaviour
{
    protected override void OnBehaviourStart()
    {
        // Initialize your platform
    }
    
    protected override void OnBehaviourUpdate()
    {
        // Update logic
    }
}
```

2. **Use Built-in Methods**:
   - `MoveTo(TransformData target, Action onComplete)`: Smooth movement with callback
   - `ApplyTransformImmediate(TransformData data)`: Instant position change
   - Access `rb`, `activeSequence` protected members

3. **Add Custom Editor**:
```csharp
[CustomEditor(typeof(EC_MyPlatform))]
public class EC_MyPlatformEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        // Add custom buttons/controls
    }
}
```

### Accessing Platform Components
```csharp
// Direct component access
EC_ShiftingPlatform platform = GetComponent<EC_ShiftingPlatform>();
platform.ShiftToTarget();

// Or find in scene
EC_CyclicPlatform[] allCyclicPlatforms = FindObjectsOfType<EC_CyclicPlatform>();
```

---

## Troubleshooting

### Platform doesn't move
- Check if Rigidbody is attached and set to Kinematic
- Verify movement is triggered (auto-start or manual call)
- Check if transforms are recorded

### Player doesn't move with platform
- Ensure Rigidbody is NOT static
- Check collision layers
- For disappearing platforms, verify player layer is correct

### Animations are jerky
- Increase Move Duration
- Try different Ease Types
- Ensure Rigidbody is Kinematic

### Platform rotation not working
- Verify rotation is recorded in TransformData
- Check if rotation tween is working
- Ensure DOTween is properly imported

### Multiple platforms sharing same data
- This shouldn't happen with MonoBehaviour components!
- Each component instance has unique data
- If you're seeing shared state, check you're not using static variables

---

## Performance Considerations

- **Instance Components**: Each platform has its own component (no shared state)
- **Kill Sequences**: All tweens are properly killed on disable/destroy
- **Physics Updates**: Position changes use FixedUpdate for physics consistency
- **Detection**: Disappearing platform uses Physics.CheckBox (efficient)
- **Memory**: Each instance stores its own TransformData (minimal overhead)

---

## Version History

### v2.0 (Current)
- **BREAKING CHANGE**: Converted from ScriptableObject to MonoBehaviour
- Each platform instance now has unique data
- Simplified workflow: just add component
- Better Unity integration
- No more shared state issues

### v1.0
- Initial modular platform system
- Used ScriptableObject-based behaviors (had shared state issue)
- Three platform types: Shifting, Cyclic, Disappearing

---

## Future Enhancements

Potential additions:
- EC_PathPlatform (follows waypoint path)
- EC_PhysicsPlatform (responds to weight/triggers)
- EC_RotatingPlatform (continuous rotation)
- EC_BouncePlatform (bounces player)
- Animation Events integration
- Sound effect hooks
- Particle effect support
