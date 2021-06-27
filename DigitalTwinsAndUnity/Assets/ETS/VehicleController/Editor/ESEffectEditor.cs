using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(ESEffect))]
public class ESEffectEditor : Editor {

 
    public ESEffect _effect;

    public override void OnInspectorGUI()
    {
        _effect = target as ESEffect;

        ArrayEditor("generateskid");
        ArrayEditor("generateskidsounds");
       
        MyCustomInspector(_effect);
     
        if (GUI.changed)
        {
            EditorUtility.SetDirty(_effect);
        }
    }

    private void MyCustomInspector( ESEffect _effect)
    {
        

        GUILayout.Space(10f);
        //
        EditorGUI.BeginChangeCheck();
        ParticleSystem _burnoutparticle = new ParticleSystem();
            _burnoutparticle = (ParticleSystem)EditorGUILayout.ObjectField("BurnOutParticle", 
                _effect.burnoutparticle, typeof(ParticleSystem), true) as ParticleSystem;
            
        //
        float _fowardsliplimit = EditorGUILayout.FloatField("FowardSlipLimit", _effect.fowardsliplimit);
        float _sidesliplimit = EditorGUILayout.FloatField("SideSlipLimit", _effect.sidesliplimit);
        //
   
        bool _DebugMode = EditorGUILayout.Toggle("DebugMode", _effect.DebugMode);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(_effect, "Set Field");
            _effect.DebugMode = _DebugMode;
            _effect.fowardsliplimit = _fowardsliplimit;
            _effect.sidesliplimit = _sidesliplimit;
            _effect.burnoutparticle = _burnoutparticle;
        }
       

        if (_effect.DebugMode)
        {
            EditorGUI.BeginChangeCheck();
        
            float f1 = EditorGUILayout.FloatField("currentfrictionfowardslip ", _effect.currentfrictionfowardslip);
            float f2 = EditorGUILayout.FloatField("currentFrictionsidewaysslip", _effect.currentFrictionsidewaysslip);
           
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_effect,"Set Field Value");
                _effect.currentfrictionfowardslip = f1;
                _effect.currentFrictionsidewaysslip = f2;
            }
            
        }
    }

   

    private void ArrayEditor(string word)
    {
        SerializedProperty MyArrayprop = serializedObject.FindProperty(word);
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(MyArrayprop, true);
        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
            //EditorGUIUtility.LookLikeControls();
        }
        serializedObject.ApplyModifiedProperties();
    }	

}
