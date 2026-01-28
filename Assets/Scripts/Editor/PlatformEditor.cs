using UnityEngine;
using UnityEditor;

/// <summary>
/// Custom editor for EC_ShiftingPlatform component
/// </summary>
[CustomEditor(typeof(EC_ShiftingPlatform))]
public class EC_ShiftingPlatformEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EC_ShiftingPlatform platform = (EC_ShiftingPlatform)target;
        
        DrawDefaultInspector();

        EditorGUILayout.Space(10);
        DrawShiftingPlatformControls(platform);
    }

    private void DrawShiftingPlatformControls(EC_ShiftingPlatform platform)
    {
        EditorGUILayout.LabelField("Transform Recording", EditorStyles.boldLabel);
        EditorGUILayout.Space(3);

        using (new EditorGUILayout.HorizontalScope())
        {
            GUI.backgroundColor = new Color(0.7f, 1f, 0.7f);
            if (GUILayout.Button("Record Original", GUILayout.Height(35)))
            {
                Undo.RecordObject(platform, "Record Original");
                platform.RecordOriginalTransform();
                EditorUtility.SetDirty(platform);
            }

            GUI.backgroundColor = new Color(1f, 0.9f, 0.6f);
            if (GUILayout.Button("Record Target", GUILayout.Height(35)))
            {
                Undo.RecordObject(platform, "Record Target");
                platform.RecordTargetTransform();
                EditorUtility.SetDirty(platform);
            }
            GUI.backgroundColor = Color.white;
        }

        EditorGUILayout.Space(3);
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Preview Original", GUILayout.Height(25)))
            {
                Undo.RecordObject(platform.transform, "Preview Original");
                platform.PreviewOriginalTransform();
                SceneView.RepaintAll();
            }

            if (GUILayout.Button("Preview Target", GUILayout.Height(25)))
            {
                Undo.RecordObject(platform.transform, "Preview Target");
                platform.PreviewTargetTransform();
                SceneView.RepaintAll();
            }
        }

        if (Application.isPlaying)
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Runtime Controls", EditorStyles.miniBoldLabel);
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Shift to Target"))
                {
                    platform.ShiftToTarget();
                }

                if (GUILayout.Button("Shift to Original"))
                {
                    platform.ShiftToOriginal();
                }

                if (GUILayout.Button("Toggle"))
                {
                    platform.Toggle();
                }
            }
        }
    }
}

/// <summary>
/// Custom editor for EC_CyclicPlatform component
/// </summary>
[CustomEditor(typeof(EC_CyclicPlatform))]
public class EC_CyclicPlatformEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EC_CyclicPlatform platform = (EC_CyclicPlatform)target;
        
        DrawDefaultInspector();

        EditorGUILayout.Space(10);
        DrawCyclicPlatformControls(platform);
    }

    private void DrawCyclicPlatformControls(EC_CyclicPlatform platform)
    {
        EditorGUILayout.LabelField("Transform Recording", EditorStyles.boldLabel);
        EditorGUILayout.Space(3);

        using (new EditorGUILayout.HorizontalScope())
        {
            GUI.backgroundColor = new Color(0.7f, 1f, 0.7f);
            if (GUILayout.Button("Record Point A", GUILayout.Height(35)))
            {
                Undo.RecordObject(platform, "Record Point A");
                platform.RecordPointA();
                EditorUtility.SetDirty(platform);
            }

            GUI.backgroundColor = new Color(1f, 0.9f, 0.6f);
            if (GUILayout.Button("Record Point B", GUILayout.Height(35)))
            {
                Undo.RecordObject(platform, "Record Point B");
                platform.RecordPointB();
                EditorUtility.SetDirty(platform);
            }
            GUI.backgroundColor = Color.white;
        }

        EditorGUILayout.Space(3);
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Preview Point A", GUILayout.Height(25)))
            {
                Undo.RecordObject(platform.transform, "Preview Point A");
                platform.PreviewPointA();
                SceneView.RepaintAll();
            }

            if (GUILayout.Button("Preview Point B", GUILayout.Height(25)))
            {
                Undo.RecordObject(platform.transform, "Preview Point B");
                platform.PreviewPointB();
                SceneView.RepaintAll();
            }
        }

        if (Application.isPlaying)
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Runtime Controls", EditorStyles.miniBoldLabel);
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Start Cycle"))
                {
                    platform.StartCycle();
                }

                if (GUILayout.Button("Stop Cycle"))
                {
                    platform.StopCycle();
                }

                if (GUILayout.Button("Reset to Point A"))
                {
                    platform.ResetToPointA();
                }
            }
        }
    }
}

/// <summary>
/// Custom editor for EC_DisappearingPlatform component
/// </summary>
[CustomEditor(typeof(EC_DisappearingPlatform))]
public class EC_DisappearingPlatformEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EC_DisappearingPlatform platform = (EC_DisappearingPlatform)target;
        
        DrawDefaultInspector();

        if (Application.isPlaying)
        {
            EditorGUILayout.Space(10);
            DrawDisappearingPlatformControls(platform);
        }
    }

    private void DrawDisappearingPlatformControls(EC_DisappearingPlatform platform)
    {
        EditorGUILayout.LabelField("Runtime Controls", EditorStyles.boldLabel);
        EditorGUILayout.Space(5);

        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Trigger Disappear"))
            {
                platform.TriggerDisappear();
            }

            if (GUILayout.Button("Trigger Respawn"))
            {
                platform.TriggerRespawn();
            }

            if (GUILayout.Button("Reset"))
            {
                platform.ResetPlatform();
            }
        }
    }
}
