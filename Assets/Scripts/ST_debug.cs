using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

/// <summary>
/// A singleton-based debug utility for drawing Gizmos and displaying text in the UI.
/// Supports both single-frame (cleared on FixedUpdate) and timed Gizmos.
/// </summary>
public class ST_debug : MonoBehaviour
{
    // --- Singleton ---
    private static ST_debug _instance;

    // --- Inspector References ---
    [Header("UI References")]
    [SerializeField] private TMP_Text _debugText;
    [SerializeField] private TMP_Text _stateText;

    // --- Private Fields ---
    private readonly StringBuilder _debugStringBuilder = new StringBuilder(1024);
    private readonly StringBuilder _stateStringBuilder = new StringBuilder(512);

    private struct SphereGizmo
    {
        public Vector3 Position;
        public float Radius;
        public Color Color;
        // A nullable float. If it has a value, it's a timed sphere. If null, it's a single-frame sphere.
        public float? ExpirationTime;
    }
    private static readonly List<SphereGizmo> _spheresToDraw = new List<SphereGizmo>();

    #region Unity Lifecycle
    private void Awake()
    {
        // Enforce the singleton pattern.
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }

    private void OnDestroy()
    {
        // Clear the instance if this object is destroyed.
        if (_instance == this)
        {
            _instance = null;
            _spheresToDraw.Clear(); // Clear static list on destroy
        }
    }

    private void Update()
    {
        // 1. Remove any timed spheres that have expired.
        _spheresToDraw.RemoveAll(sphere => sphere.ExpirationTime.HasValue && Time.time > sphere.ExpirationTime.Value);

        // 2. Update UI text elements.
        if (_debugText != null)
        {
            _debugText.text = _debugStringBuilder.ToString();
        }
        if (_stateText != null)
        {
            _stateText.text = _stateStringBuilder.ToString();
        }

        // 3. Clear the string builders for the next frame.
        _debugStringBuilder.Clear();
        _stateStringBuilder.Clear();
    }

    private void FixedUpdate()
    {
        // As requested, clear all single-frame (non-timed) spheres every FixedUpdate cycle.
        _spheresToDraw.RemoveAll(sphere => !sphere.ExpirationTime.HasValue);
    }

    // OnDrawGizmos is called by the editor to draw gizmos.
    private void OnDrawGizmos()
    {
        if (_spheresToDraw == null) return;

        // Draw all spheres currently in the list (both timed and single-frame).
        foreach (var sphere in _spheresToDraw)
        {
            Gizmos.color = sphere.Color;
            Gizmos.DrawSphere(sphere.Position, sphere.Radius);
        }
    }
    #endregion

    #region Public Static API
    /// <summary>
    /// Appends a message to the main debug text display for one frame.
    /// </summary>
    public static void Log(object message)
    {
        if (_instance != null)
        {
            _instance._debugStringBuilder.AppendLine(message.ToString());
        }
    }

    /// <summary>
    /// Appends a message to the state debug text display for one frame.
    /// </summary>
    public static void LogState(object message)
    {
        if (_instance != null)
        {
            _instance._stateStringBuilder.AppendLine(message.ToString());
        }
    }

    /// <summary>
    /// Schedules a sphere to be drawn by Gizmos.
    /// If duration is null, it will be drawn for one frame (cleared on FixedUpdate).
    /// If duration has a value, it will be drawn for that many seconds.
    /// </summary>
    public static void DrawSphere(Vector3 position, float radius, Color? color = null, float? duration = null)
    {
        if (Application.isPlaying && _instance != null)
        {
            _spheresToDraw.Add(new SphereGizmo
            {
                Position = position,
                Radius = radius,
                Color = color ?? Color.red,
                ExpirationTime = duration.HasValue ? Time.time + duration.Value : (float?)null
            });
        }
    }
    #endregion
}