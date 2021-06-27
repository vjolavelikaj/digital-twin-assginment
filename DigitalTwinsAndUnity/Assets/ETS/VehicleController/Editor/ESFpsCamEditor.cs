using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(ESFpsCamera))]
public class ESFpsCamEditor :Editor
{

    public ESFpsCamera myscript;

    public override void OnInspectorGUI()
    {
        myscript = target as ESFpsCamera;

        EditorGUI.BeginChangeCheck();
        GameObject FollowObject = null;
        float smoothspeed = new float();
        float MouseXclamp = new float();
   
        string GameobjectName = "";
        bool FindByName = new bool();

        FindByName = EditorGUILayout.Toggle("FindByName", myscript.findbyname);
        if (myscript.findbyname)
        {
            GameobjectName = EditorGUILayout.TextField("GameobjectName", myscript.Gameobjname);
        }
        else
        {
            FollowObject = EditorGUILayout.ObjectField("FollowObject", myscript.Target, typeof(GameObject), true) as GameObject;
        }
        smoothspeed = EditorGUILayout.FloatField("SmoothSpeed", myscript.SmoothSpeed);
        MouseXclamp = EditorGUILayout.FloatField("MouseXclamp", myscript.mouseXclamp);
       
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(myscript, "Changes");
            myscript.findbyname = FindByName;
            myscript.Gameobjname = GameobjectName;
            myscript.Target = FollowObject;
            myscript.SmoothSpeed = smoothspeed;
           
        }


        if (GUI.changed)
        {
            EditorUtility.SetDirty(myscript);
        }
    }
}
