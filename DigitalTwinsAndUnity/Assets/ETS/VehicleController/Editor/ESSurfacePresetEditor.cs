using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(ESSurfacePreset))]
public class ESSurfacePresetEditor : Editor
{

    public  ESSurfacePreset preset;
 
    public override void OnInspectorGUI()
    {
        preset = target as ESSurfacePreset;
       
        if (GUILayout.Button("Launch SurfacePresetWindow", GUILayout.MinWidth(150), GUILayout.MinHeight(25)))
        {
            ESSurfacePresetEditorWindow.Init();
        }
        if (GUI.changed)
        {
            EditorUtility.SetDirty(preset);
        }
        
    }
}
