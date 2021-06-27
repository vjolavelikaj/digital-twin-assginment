using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ESPath))]
public class ESPathEditor : Editor
{
    public ESPath myscript;

    public override void OnInspectorGUI()
    {
        myscript = target as ESPath;
        if (GUI.changed)
        {
            EditorUtility.SetDirty(myscript);
        }
        EditorGUILayout.HelpBox("Inorder to add more nodes please duplicate a child of this game object,and drag it's desired position ", MessageType.Info);
        myscript.linecolor = EditorGUILayout.ColorField("LineColor", myscript.linecolor);
    }
}
