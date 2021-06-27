using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(ESThirdPersonCamera360))]
public class ESThirdPerson360CamEditor : Editor
{


    public ESThirdPersonCamera360 myscript;

    public override void OnInspectorGUI()
    {
        myscript = target as ESThirdPersonCamera360;

        EditorGUI.BeginChangeCheck();
        GameObject FollowObject = null;
        float followspeed = new float();
        float distancefromtarget = new float();
        float heightdamping = new float();
        float rotationdamping = new float();
        float Sensitivity = new float();
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
        followspeed = EditorGUILayout.FloatField("FollowSpeed", myscript.FollowSpeed);
        distancefromtarget = EditorGUILayout.FloatField("DistanceFromTarget", myscript.DistanceFromTarget);
        heightdamping = EditorGUILayout.FloatField("HeightDamping", myscript.HeightDamping);
        rotationdamping = EditorGUILayout.FloatField("RotatinDamping", myscript.RotationDamping);
        Sensitivity = EditorGUILayout.FloatField("Sensitivity", myscript.Sensitivity);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(myscript, "Changes");
            myscript.findbyname = FindByName;
            myscript.Gameobjname = GameobjectName;
            myscript.Target = FollowObject;
            myscript.FollowSpeed = followspeed;
            myscript.DistanceFromTarget = distancefromtarget;
            myscript.HeightDamping = heightdamping;
            myscript.RotationDamping = rotationdamping;
            myscript.Sensitivity = Sensitivity;
        }


        if (GUI.changed)
        {
            EditorUtility.SetDirty(myscript);
        }
    }
}
