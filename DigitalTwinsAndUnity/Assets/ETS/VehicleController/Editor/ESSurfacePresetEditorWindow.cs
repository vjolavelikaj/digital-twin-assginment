using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ESSurfacePresetEditorWindow : EditorWindow
{
    public static ESSurfacePreset my_preset;
    public ESSurfacePresetEditor editor;
    private SerializedObject serlizeme;
    Vector2 scrollPos;

    // Add menu named "My Window" to the Window menu
    [MenuItem("Easy Vehicle System/Surface/SurfacePreset")]
    public static void Init()
    {
        // Get existing open window or if none, make a new one:
        ESSurfacePresetEditorWindow window = (ESSurfacePresetEditorWindow)EditorWindow.GetWindow(typeof(ESSurfacePresetEditorWindow), false, "SurfacePreset");
        window.autoRepaintOnSceneChange = true;
        //forces editor refresh 
        EditorApplication.modifierKeysChanged += window.Repaint;
        window.Show();

    }
   
    private void OnFocus()
    {
        GameObject go = Resources.Load("Surfaces/SurfacePreset") as GameObject;
        my_preset = go.GetComponent<ESSurfacePreset>();
        serlizeme = new SerializedObject(my_preset);

        System.Array.Resize(ref my_preset.m_surafaceFoldout, my_preset.addsurface.Count);
        System.Array.Resize(ref my_preset.wheelcolliderpreset, my_preset.addsurface.Count);
        for (int i = 0; i < my_preset.wheelcolliderpreset.Length; ++i)
        {
            my_preset.wheelcolliderpreset[i] = my_preset.addsurface[i].usewheelfrictincurve;
        }
       
    }

   
    private void OnGUI()
    {
        GameObject go = Resources.Load("Surfaces/SurfacePreset") as GameObject;
        my_preset = go.GetComponent<ESSurfacePreset>();
        serlizeme = new SerializedObject(my_preset);
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        GUILayout.Label("Surface Settings", EditorStyles.boldLabel);

        Surfaces();

        EditorGUILayout.EndScrollView();
        serlizeme.Update();
        if (GUI.changed)
        {
            EditorUtility.SetDirty(my_preset);
        }
    }
    private void DuplicateLast(int lastindex)
    {
        GameObject go = Resources.Load("Surfaces/SurfacePreset") as GameObject;
        my_preset = go.GetComponent<ESSurfacePreset>();
        serlizeme = new SerializedObject(my_preset);

        my_preset.addsurface[my_preset.surfaceindex - 1].IsTerrain = my_preset.addsurface[lastindex].IsTerrain;
        my_preset.addsurface[my_preset.surfaceindex - 1].SurfaceName = my_preset.addsurface[lastindex].SurfaceName;
        my_preset.addsurface[my_preset.surfaceindex - 1].TerrainName = my_preset.addsurface[lastindex].TerrainName;
        my_preset.addsurface[my_preset.surfaceindex - 1].TerrainTextureIndex = my_preset.addsurface[lastindex].TerrainTextureIndex;
        my_preset.addsurface[my_preset.surfaceindex - 1].TagName = my_preset.addsurface[lastindex].TagName;
        //wheelpreset
        //fowardslip
        my_preset.addsurface[my_preset.surfaceindex - 1].sideslip.asymptoteslip = my_preset.addsurface[lastindex].sideslip.asymptoteslip;
        my_preset.addsurface[my_preset.surfaceindex - 1].sideslip.asymptotevalue = my_preset.addsurface[lastindex].sideslip.asymptotevalue;
        my_preset.addsurface[my_preset.surfaceindex - 1].sideslip.extremumslip = my_preset.addsurface[lastindex].sideslip.extremumslip;
        my_preset.addsurface[my_preset.surfaceindex - 1].sideslip.extremumvalue = my_preset.addsurface[lastindex].sideslip.extremumvalue;
        my_preset.addsurface[my_preset.surfaceindex - 1].sideslip.Stiffness = my_preset.addsurface[lastindex].sideslip.Stiffness;
        //sidealip
        my_preset.addsurface[my_preset.surfaceindex - 1].forwardslip.asymptoteslip = my_preset.addsurface[lastindex].forwardslip.asymptoteslip;
        my_preset.addsurface[my_preset.surfaceindex - 1].forwardslip.asymptotevalue = my_preset.addsurface[lastindex].forwardslip.asymptotevalue;
        my_preset.addsurface[my_preset.surfaceindex - 1].forwardslip.extremumslip = my_preset.addsurface[lastindex].forwardslip.extremumslip;
        my_preset.addsurface[my_preset.surfaceindex - 1].forwardslip.extremumvalue = my_preset.addsurface[lastindex].forwardslip.extremumvalue;
        my_preset.addsurface[my_preset.surfaceindex - 1].forwardslip.Stiffness = my_preset.addsurface[lastindex].forwardslip.Stiffness;
        
    }

    private void Surfaces()
    {
        EditorGUILayout.HelpBox("Here you can Set Behiavour for vehicle wheel based on different surfaces", MessageType.Info);
        #region loop
        for (int i = 0; i < my_preset.addsurface.Count; i++)
        {
            GUI.color = Color.gray;
            EditorGUILayout.BeginVertical("Box");

            my_preset.m_surafaceFoldout[i] = EditorGUILayout.Foldout(my_preset.m_surafaceFoldout[i], my_preset.addsurface[i].SurfaceName);
            
          
            EditorGUI.BeginChangeCheck();
            //
            string _SurfaceName = "";
            //bool _EmitParticle = new bool();
            bool _IsTerrain = new bool();
            int _TerrainTextureIndex = new int();
            string _TerrainName = "";
            string _TagName = "";
            string _WheelColliderTagName = "";
            //
            float F_extremumslip   = new float();
            float F_extremumvalue  = new float();
            float F_asymptoteslip  = new float();
            float F_asymptotevalue = new float();
            float F_Stiffness = new float();
            //
            float S_extremumslip      = new float();
            float S_extremumvalue     = new float();
            float S_asymptoteslip     = new float();
            float S_asymptotevalue    = new float();
            float S_Stiffness = new float();
            bool _EmitParticle = new bool();
            ParticleSystem _ParticlePrefab = new ParticleSystem();
            AudioSource _ParticleSoundPrefab = new AudioSource();
            AudioClip clip = null;

            //
            if (my_preset.m_surafaceFoldout[i] == true)
            {
                GUILayout.Space(4);
               
                 _SurfaceName = EditorGUILayout.TextField("SurfaceName", my_preset.addsurface[i].SurfaceName);
                 _IsTerrain = EditorGUILayout.Toggle("IsTerrain", my_preset.addsurface[i].IsTerrain);
                
                if (my_preset.addsurface[i].IsTerrain)
                {
                    _TerrainTextureIndex = EditorGUILayout.IntField("TerrainTextureIndex", my_preset.addsurface[i].TerrainTextureIndex);
                    
                    _TerrainName = EditorGUILayout.TextField("TerrainName", my_preset.addsurface[i].TerrainName);
                }
                else
                {
                   
                     _TagName = EditorGUILayout.TextField("TagName", my_preset.addsurface[i].TagName);
                }
                
              
                 _EmitParticle = EditorGUILayout.Toggle("EmitParticles", my_preset.addsurface[i].EmitParticle);
                if (my_preset.addsurface[i].EmitParticle)
                {
                    EditorGUILayout.HelpBox("Must be a Prefab, inorder to avoid loss of data", MessageType.Info);
                    _ParticlePrefab = (ParticleSystem)EditorGUILayout.ObjectField("ParticleSystem", my_preset.addsurface[i].ParticlePrefab,
                        typeof(ParticleSystem), true) as ParticleSystem;
                    clip = EditorGUILayout.ObjectField("Clip", my_preset.addsurface[i].clip, typeof(AudioClip), true) as AudioClip;
                }
             
                my_preset.wheelcolliderpreset[i] = EditorGUILayout.BeginToggleGroup("Wheel Friction Settings", my_preset.wheelcolliderpreset[i]);
                
                GUILayout.Space(10);


                my_preset.addsurface[i].usewheelfrictincurve = my_preset.wheelcolliderpreset[i];
              
                EditorGUILayout.HelpBox("The current wheelcollider's tagname", MessageType.Info);
                 _WheelColliderTagName = EditorGUILayout.TextField("WheelColliderTagName", my_preset.addsurface[i].forwardslip.WheelColliderTagName);
               
                GUILayout.Label("Forward Friction", EditorStyles.boldLabel);
                F_extremumslip = EditorGUILayout.FloatField("Extremum Slip", my_preset.addsurface[i].forwardslip.extremumslip);
                F_extremumvalue = EditorGUILayout.FloatField("Extremum Value", my_preset.addsurface[i].forwardslip.extremumvalue);
                F_asymptoteslip = EditorGUILayout.FloatField("Asymptote Slip", my_preset.addsurface[i].forwardslip.asymptoteslip);
                F_asymptotevalue = EditorGUILayout.FloatField("Asymptote Value", my_preset.addsurface[i].forwardslip.asymptotevalue);
                F_Stiffness = EditorGUILayout.FloatField("Stiffness", my_preset.addsurface[i].forwardslip.Stiffness);
              
                GUILayout.Space(10f);

                GUILayout.Label("SideWay Friction", EditorStyles.boldLabel);

                S_extremumslip = EditorGUILayout.FloatField("Extremum Slip", my_preset.addsurface[i].sideslip.extremumslip);
                S_extremumvalue = EditorGUILayout.FloatField("Extremum Value", my_preset.addsurface[i].sideslip.extremumvalue);
                S_asymptoteslip = EditorGUILayout.FloatField("Asymptote Slip", my_preset.addsurface[i].sideslip.asymptoteslip);
                S_asymptotevalue = EditorGUILayout.FloatField("Asymptote Value", my_preset.addsurface[i].sideslip.asymptotevalue);
                S_Stiffness = EditorGUILayout.FloatField("Stiffness", my_preset.addsurface[i].sideslip.Stiffness);
             
                EditorGUILayout.EndToggleGroup();
            }
            //
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(my_preset, "Changed Field");
                my_preset.addsurface[i].SurfaceName = _SurfaceName;
                my_preset.addsurface[i].IsTerrain = _IsTerrain;
                my_preset.addsurface[i].EmitParticle = _EmitParticle;
                my_preset.addsurface[i].TerrainTextureIndex = _TerrainTextureIndex;
                my_preset.addsurface[i].TerrainName = _TerrainName;
                my_preset.addsurface[i].TagName = _TagName;
                my_preset.addsurface[i].clip = clip;
                my_preset.addsurface[i].ParticlePrefab = _ParticlePrefab;
               // my_preset.addsurface[i].ParticleSoundPrefab = _ParticleSoundPrefab;
                //
                my_preset.addsurface[i].forwardslip.WheelColliderTagName = _WheelColliderTagName;
                my_preset.addsurface[i].forwardslip.extremumslip = F_extremumslip;
                my_preset.addsurface[i].forwardslip.extremumvalue = F_extremumvalue;
                my_preset.addsurface[i].forwardslip.asymptoteslip = F_asymptoteslip;
                my_preset.addsurface[i].forwardslip.asymptotevalue = F_asymptotevalue;
                my_preset.addsurface[i].forwardslip.Stiffness = F_Stiffness;
                //
                my_preset.addsurface[i].sideslip.extremumslip = S_extremumslip;
                my_preset.addsurface[i].sideslip.extremumvalue = S_extremumvalue;
                my_preset.addsurface[i].sideslip.asymptoteslip = S_asymptoteslip;
                my_preset.addsurface[i].sideslip.asymptotevalue = S_asymptotevalue;
                my_preset.addsurface[i].sideslip.Stiffness = S_Stiffness;
            }

            if (my_preset.m_surafaceFoldout[i] == false)
            {
                
                EditorGUILayout.BeginHorizontal();
                
                if (GUILayout.Button("-", GUILayout.Width(30), GUILayout.Height(15)))
                {
                    EditorGUI.BeginChangeCheck();
                    if (my_preset.surfaceindex > 1)
                    {
                        Undo.RecordObject(my_preset, "-");
                        my_preset.addsurface.RemoveAt(i);
                        my_preset.surfaceindex--;
                        System.Array.Resize(ref my_preset.m_surafaceFoldout, my_preset.addsurface.Count);
                        System.Array.Resize(ref  my_preset.wheelcolliderpreset, my_preset.addsurface.Count);
                        EditorUtility.SetDirty(my_preset);        
                    }
                }
                EditorGUILayout.EndHorizontal();
               
            }
            EditorGUILayout.EndVertical();
        }
        #endregion
        GUILayout.Space(20);
        //
        GUI.color = Color.white;
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Suface Preset", GUILayout.Width(200), GUILayout.Height(20)))
        {
            Undo.RecordObject(my_preset, "Add Suface Preset");
            ESSurfacePreset.AddSurface ad = new ESSurfacePreset.AddSurface();
            my_preset.surfaceindex++;
            my_preset.addsurface.Add(ad);
            my_preset.SetDefualts(my_preset.surfaceindex - 1);
            System.Array.Resize(ref my_preset.m_surafaceFoldout, my_preset.addsurface.Count);
            System.Array.Resize(ref my_preset.wheelcolliderpreset, my_preset.addsurface.Count);
            EditorUtility.SetDirty(my_preset);   
        }
        if (GUILayout.Button("Duplicate Last", GUILayout.Width(200), GUILayout.Height(20)))
        {
            Undo.RecordObject(my_preset, "Duplicate Last");
            ESSurfacePreset.AddSurface ad = new ESSurfacePreset.AddSurface();
            my_preset.surfaceindex++;
            my_preset.addsurface.Add(ad);
            my_preset.SetDefualts(my_preset.surfaceindex - 1);
           
            System.Array.Resize(ref my_preset.m_surafaceFoldout, my_preset.addsurface.Count);
            System.Array.Resize(ref my_preset.wheelcolliderpreset, my_preset.addsurface.Count);
            DuplicateLast(my_preset.surfaceindex - 2);
            EditorUtility.SetDirty(my_preset);   
        }
        if (GUILayout.Button("Clear", GUILayout.Width(150), GUILayout.Height(20)))
        {
            Undo.RecordObject(my_preset, "Clear");
            my_preset.addsurface.Clear();
            ESSurfacePreset.AddSurface ad = new ESSurfacePreset.AddSurface();
            my_preset.surfaceindex = 1;
            my_preset.addsurface.Add(ad);
            
            System.Array.Resize(ref my_preset.m_surafaceFoldout, my_preset.addsurface.Count);
            System.Array.Resize(ref my_preset.wheelcolliderpreset, my_preset.addsurface.Count);
            EditorUtility.SetDirty(my_preset);   
            //
        }
        EditorGUILayout.EndHorizontal();
    }
}
