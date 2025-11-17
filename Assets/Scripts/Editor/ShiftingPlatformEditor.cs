using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ShiftingPlatform))]
public class ShiftingPlatformEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if (targets.Length > 1)
        {
            EditorGUILayout.HelpBox("Multi-object editing not supported for transform recording.", MessageType.Info);
            DrawDefaultInspector();
            return;
        }

        ShiftingPlatform platform = (ShiftingPlatform)target;

        DrawRecordingButtons(platform);
        DrawPreviewButtons(platform);
        
        EditorGUILayout.Space(10);
        DrawPropertiesExcluding(serializedObject);
        
        serializedObject.ApplyModifiedProperties();
    }

    private void DrawRecordingButtons(ShiftingPlatform platform)
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
                serializedObject.Update();
            }

            GUI.backgroundColor = new Color(1f, 0.9f, 0.6f);
            if (GUILayout.Button("Record Target", GUILayout.Height(35)))
            {
                Undo.RecordObject(platform, "Record Target");
                platform.RecordTargetTransform();
                EditorUtility.SetDirty(platform);
                serializedObject.Update();
            }
            GUI.backgroundColor = Color.white;
        }
    }

    private void DrawPreviewButtons(ShiftingPlatform platform)
    {
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
    }
}
