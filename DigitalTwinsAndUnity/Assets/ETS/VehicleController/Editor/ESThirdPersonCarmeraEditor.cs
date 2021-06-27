using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(ESThirdPersonCarmera))]
public class ESThirdPersonCarmeraEditor : Editor 
{
    public ESThirdPersonCarmera myscript;

   
    
    public override void OnInspectorGUI()
    {
        myscript = target as ESThirdPersonCarmera;

        EditorGUI.BeginChangeCheck();
        GameObject FollowObject = null;
        float followspeed = new float();
        float distancefromtarget = new float();
        float heightfromtarget =   new float();
        float heightdamping      = new float();
        float rotationdamping =    new float();
        
        string GameobjectName = "";
        bool FindByName = new bool();
        ESThirdPersonCarmera.LerpType _lerptype = new ESThirdPersonCarmera.LerpType();

        FindByName = EditorGUILayout.Toggle("FindByName", myscript.findbyname);
        if (myscript.findbyname)
        {
            GameobjectName = EditorGUILayout.TextField("GameobjectName", myscript.Gameobjname);
        }
        else
        {
            FollowObject = EditorGUILayout.ObjectField("FollowObject", myscript.Target, typeof(GameObject), true) as GameObject;
        }
        _lerptype = (ESThirdPersonCarmera.LerpType)EditorGUILayout.EnumPopup("LerpType", myscript._lerptype);
        followspeed = EditorGUILayout.FloatField("FollowSpeed", myscript.FollowSpeed);
        distancefromtarget  = EditorGUILayout.FloatField("DistanceFromTarget", myscript.DistanceFromTarget);
        heightfromtarget = EditorGUILayout.FloatField("HeightFromTarget", myscript.HeightFromTarget);
        heightdamping     = EditorGUILayout.FloatField("HeightDamping", myscript.HeightDamping);
        rotationdamping = EditorGUILayout.FloatField("RotatinDamping", myscript.RotationDamping);
       

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(myscript, "Changes");
            myscript.findbyname = FindByName;
            myscript.Gameobjname = GameobjectName;
            myscript.Target = FollowObject;
            myscript.FollowSpeed = followspeed;
            myscript.DistanceFromTarget = distancefromtarget;
            myscript.HeightFromTarget = heightfromtarget;
            myscript.HeightDamping = heightdamping;
            myscript.RotationDamping = rotationdamping;
            myscript._lerptype = _lerptype;
        }


        if (GUI.changed)
        {
            EditorUtility.SetDirty(myscript);
        }
    }
}
