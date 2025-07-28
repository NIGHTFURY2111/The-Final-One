using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class ST_debug : MonoBehaviour
{
    private static ST_debug instance;
    [SerializeField] TMP_Text debugText;
    [SerializeField] TMP_Text stateText;

    private static readonly List<(Vector3 position, float radius, Color color)> spheres = new();
    private static string _displayString = "";
    private static string _stateString = "";

    private void Awake()
    {
        instance = this;
    }

    public static void Log(string msg) => _displayString += msg + "\n";
    public static void LogState(string msg) => _stateString += msg + "\n";
    public static void DrawSphere(Vector3 position, float radius, Color? color = null) => spheres.Add((position, radius, color ?? Color.red));
    public static void ClearSpheres() => spheres.Clear();

    private void Update()
    {
        ClearSpheres();
        if (stateText) stateText.text = _stateString;
        if (debugText) debugText.text = _displayString;
        _displayString = "";
        _stateString = "";
    }

    private void OnDrawGizmos()
    {
        foreach (var (position, radius, color) in spheres)
        {
            Gizmos.color = color;
            Gizmos.DrawSphere(position, radius);
        }
        // Do NOT clear spheres here!
    }
}
