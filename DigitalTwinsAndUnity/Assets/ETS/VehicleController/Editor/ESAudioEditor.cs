using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CanEditMultipleObjects]
[CustomEditor(typeof(ESAudioSystem))]
public class ESAudioEditor :Editor
{
    public ESAudioSystem scripts;


    public override void OnInspectorGUI()
    {
        scripts = target as ESAudioSystem;
        float pitchmodifier;
        float pitchmultipler;
        float volumemul;
        float startvol;
        float reverb;
        float spartialblend;
        AudioClip Highacc = null;
        AudioClip lowacc= null;
        AudioClip idle = null;
        AudioClip enginesound  = null;
        AudioClip deccel = null;    
        //
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.LabelField("PitchSettings",EditorStyles.boldLabel);
        GUILayout.Space(1.5f);
        pitchmodifier =  EditorGUILayout.FloatField("pitchmodifier",scripts.PitchModifier);
        pitchmultipler = EditorGUILayout.FloatField("ptchmultiplier",scripts.PitchMultiplier);
        EditorGUILayout.LabelField("VolumeSettings", EditorStyles.boldLabel);
        GUILayout.Space(1.5f);
        volumemul = EditorGUILayout.FloatField("VolumeMultiplier",scripts.VolumeMultiplier);
        startvol = EditorGUILayout.FloatField("MaxVolume",scripts.StartVolume);
        EditorGUILayout.LabelField("AudioSettings", EditorStyles.boldLabel);
        GUILayout.Space(1.5f);
        reverb = EditorGUILayout.FloatField("Reverb", scripts.reverb);
        spartialblend = EditorGUILayout.FloatField( "SpatialBlend",scripts.spatialblend);
        GUILayout.Space(0.7f);
        ESAudioSystem.SoundType ST = new ESAudioSystem.SoundType();
        ST = (ESAudioSystem.SoundType)EditorGUILayout.EnumPopup("SoundType", scripts._soundtype);
        if (ST == ESAudioSystem.SoundType.Advanced)
        {
            Highacc = EditorGUILayout.ObjectField("AccelerationHigh", scripts.acceleratehigh, typeof(AudioClip), true) as AudioClip;
            lowacc = EditorGUILayout.ObjectField("AccelerationLow", scripts.acceleratelow, typeof(AudioClip), true) as AudioClip;
            deccel = EditorGUILayout.ObjectField("Decelerate", scripts.deccelarate, typeof(AudioClip), true) as AudioClip;
            idle = EditorGUILayout.ObjectField("Idle", scripts.idle, typeof(AudioClip), true) as AudioClip;
        }
        else if (ST == ESAudioSystem.SoundType.simple)
        {
            enginesound = EditorGUILayout.ObjectField("EngineSound", scripts.enginesound, typeof(AudioClip), true) as AudioClip;
        }
        //
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RegisterCompleteObjectUndo(scripts, "Undo audio settings");
            scripts.PitchMultiplier = pitchmultipler;
            scripts.PitchModifier = pitchmodifier;
            scripts.reverb = reverb;
            scripts.spatialblend = spartialblend;
            scripts.StartVolume = startvol;
            scripts.VolumeMultiplier = volumemul;
            scripts._soundtype = ST;
            if (ST == ESAudioSystem.SoundType.Advanced)
            {
               scripts.acceleratehigh =  Highacc; 
               scripts.acceleratelow =  lowacc;
               scripts.deccelarate =  deccel ;
               scripts.idle = idle;
            }
            else if (ST == ESAudioSystem.SoundType.simple)
            {
                scripts.enginesound = enginesound;
            }
        }
        if (GUI.changed)
        {
            EditorUtility.SetDirty(scripts);
        }
    }
    
}
