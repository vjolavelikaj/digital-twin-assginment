using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//[CanEditMultipleObjects]
[CustomEditor(typeof(ESGearShift))]
public class ESGearShiftEditor : Editor 
{

    public ESGearShift myscript;

    public override void OnInspectorGUI()
    {
        myscript = target as ESGearShift;
        ESGearShiftCustomInspector(myscript);
        if (GUI.changed)
        {
            EditorUtility.SetDirty(myscript);
        }

    }

    public void ESGearShiftCustomInspector(ESGearShift ESG)
    {
        ESGearShift.ShiftTpye _shifttype = new ESGearShift.ShiftTpye();
        ESGearShift.EngineMode _enginemode = new ESGearShift.EngineMode();
        ESGearShift.GearRatioCalculationType _gearcaltype =  new ESGearShift.GearRatioCalculationType();
        KeyCode NitroKey = new KeyCode();

        float MaxEngineRpm = new float();
        float RaiseModifier = new float();
        float EngineRpm = new float();
        float EffectTime = new float();
        int MaxGear = new int();
        float TractionFactor = new float();
        float TractionMultiplier = new float();
        float sliplimit = new float();
        float MaxJerkForce = new float();
        float MaxNitroValue = new float();
        float NitroExpense = new float();
        float NitroVal = new float();
        float NitroTorque = new float();
        float MaxEngineTorqueMul = new float();
        float MaxSpeedMul = new float();
        float MaxEngineTorque = new float();
        float MaxSpeed = new float();
        float TopSpeed = new float();
        float CurentTorque = new float();
        int CurrentGear = new int();
        float ReverseTopSpeed = new float();
        float NitroSpeed = new float();
        bool Debug = new bool();
        //bool ResetGear =  new bool();
        //bool IsReverse = new bool();
        bool auto = new bool();
        bool NeutralOnAwake = new bool();
        //int curgear = new int();

        AudioSource PickupSource = new AudioSource();
        AudioSource GearShiftSound = new AudioSource();
        AudioSource NitroSound = new AudioSource();

        EditorGUI.BeginChangeCheck();
        _shifttype = (ESGearShift.ShiftTpye)EditorGUILayout.EnumPopup("ShiftType", myscript.shifttype);
        _enginemode = (ESGearShift.EngineMode)EditorGUILayout.EnumPopup("Engine", myscript.enginemode);
        GUI.color = Color.gray;
        EditorGUILayout.BeginVertical("Box");
       
        myscript.GearTransmissionEffect = EditorGUILayout.BeginToggleGroup("Transmission Effect Settings",myscript.GearTransmissionEffect);
        GUILayout.Space(15);
        ArrayEditor("ExhaustFlame");
        GUILayout.Space(8);
        EffectTime = EditorGUILayout.FloatField("EffectTime", myscript.EffectTime);
        EditorGUILayout.HelpBox("Please leave empty if you dont want to use transmission sounds", MessageType.Info);
        EditorGUILayout.HelpBox("Sound Prefab",MessageType.None);
        GearShiftSound = (AudioSource)EditorGUILayout.ObjectField("TransmissionSoundPrefab", myscript.TransmissionSoundPrefab, typeof(AudioSource), true);
        ArrayEditor("HeavyEngineSmoke");
        EditorGUILayout.EndToggleGroup();
        EditorGUILayout.EndVertical();
        GUI.color = Color.white;

            GUILayout.Space(15);
        //
        MaxEngineRpm = EditorGUILayout.FloatField("MaxEngineRpm", myscript.MaxEngineRpm);
        RaiseModifier = EditorGUILayout.FloatField("RaiseModifier", myscript.RaiseModifier);
        //gearsettings
        EditorGUILayout.BeginVertical("Box");
        EditorGUILayout.HelpBox("Gear Setings", MessageType.None);
        GUILayout.Space(10f);
        EditorGUILayout.HelpBox("automatic lets  EVS do the gear ratio calculation while manual lets you fill in  gear ratio", MessageType.Info);
        _gearcaltype = (ESGearShift.GearRatioCalculationType)EditorGUILayout.EnumPopup("GearCalculationType", myscript.gearcalculationtype);
        if (myscript.gearcalculationtype == ESGearShift.GearRatioCalculationType.Manual)
        {
            MaxEngineTorqueMul = EditorGUILayout.FloatField("MaxEngineMultiplier", myscript.EngineTorqueMultiplier);
            MaxSpeedMul = EditorGUILayout.FloatField("MaxSpeedMultiplier", myscript.MaxSpeedMultiplier);
            //
            EditorGUILayout.HelpBox("Gear Limit : Infinity", MessageType.Info);
            System.Array.Resize(ref myscript.GearRatios, myscript.MaxGear);
            MaxGear = EditorGUILayout.IntField("MaxGear", myscript.MaxGear);
            ArrayEditor("GearRatios");
            Debug = EditorGUILayout.Toggle("Debug", myscript.Debug);
            if (myscript.Debug)
            {
                ArrayEditor("enginetorquearray");
                ArrayEditor("topspeedarray");
            }
        }
        else
        {
            EditorGUILayout.HelpBox("Gear Limit : 10", MessageType.Info);
            MaxGear = EditorGUILayout.IntSlider("MaxGear", myscript.MaxGear, 3, 10);
            if (myscript.MaxSpeed == myscript.ReverseTopSpeed)
            {
                GUI.color = Color.red;
                EditorGUILayout.HelpBox("MaxSpeed value cannot be thesame with ReverseTopSpeed, this will result to gearshift error, make sure ReverseTopSpeed is lesser than the actual top speed ", MessageType.Warning);
            }
            GUI.color = Color.white;
            MaxSpeed = EditorGUILayout.FloatField("MaxSpeed", myscript.MaxSpeed);
           
            //checking if maxspeed is qual to reversetopspee
            MaxEngineTorque = EditorGUILayout.FloatField("MaxEngineTorque",myscript.MaxEngineTorque);
        }
        ReverseTopSpeed = EditorGUILayout.FloatField("ReverseTopSpeed", myscript.ReverseTopSpeed);
        TractionFactor = EditorGUILayout.FloatField("TractionFactor", myscript.TractionFactor);
        TractionMultiplier = EditorGUILayout.FloatField("TractionMultiplier", myscript.TractionMultiplier);
        EditorGUILayout.EndVertical();
        sliplimit = EditorGUILayout.FloatField("SlipLimit",myscript.sliplimit);
        //IsReverse = EditorGUILayout.Toggle("Isrevsers", myscript.isreverse);
        //ResetGear = EditorGUILayout.Toggle("ResetGear", myscript.resetgear);
        auto = EditorGUILayout.Toggle("AutoReverse", myscript.AutoReverse);
        //curgear = EditorGUILayout.IntField("CurGear", myscript.CurrentGear);
        NeutralOnAwake = EditorGUILayout.Toggle("NeutralOnAwake", myscript.NeutralOnAwake);

        if (_enginemode == ESGearShift.EngineMode.Nitro)
        {
            GUILayout.Space(15);
            EditorGUILayout.BeginVertical("Box");
            NitroKey = (KeyCode)EditorGUILayout.EnumPopup("NitroKey", myscript.NitroKey);
            MaxJerkForce = EditorGUILayout.FloatField("MaxJerkForce", myscript.MaxJerkForce);
            NitroExpense = EditorGUILayout.Slider("NitroExpense", myscript.NitroExpense,0.0f, 10f);
            MaxNitroValue = EditorGUILayout.Slider("MaxNitroValue", myscript.MaxNitroValue, 0, 100);
            NitroTorque = EditorGUILayout.FloatField("NitroTorque", myscript.NitroTorque);
            NitroSpeed = EditorGUILayout.FloatField("NitroTopSpeed", myscript.NitroTopSpeed);
            PickupSource = (AudioSource)EditorGUILayout.ObjectField("PickUpAudioSource", myscript.PickUpSource, typeof(AudioSource), true) as AudioSource;
            NitroSound = (AudioSource)EditorGUILayout.ObjectField("NitroSoundSource", myscript.NitroSoundSource, typeof(AudioSource), true);
            EditorGUILayout.EndVertical();
        }

        GUILayout.Space(10);
        EditorGUILayout.BeginVertical("Box");
        EditorGUILayout.HelpBox("ReadOnly", MessageType.None);
        EngineRpm = EditorGUILayout.FloatField("EngineRpm", myscript.EngineRpm);
        NitroVal = EditorGUILayout.FloatField("NitroValue", myscript.NitroValue);
        if (myscript.gearcalculationtype == ESGearShift.GearRatioCalculationType.Manual)
        {
            CurentTorque = EditorGUILayout.FloatField("CurrentTorque", myscript.CurrentEngineTorque);
        }
        else
        {
            CurentTorque = EditorGUILayout.FloatField("CurrentTorque", myscript.tempforce);
        }
        TopSpeed = EditorGUILayout.FloatField("TopSpeed", myscript.TopSpeed);
        CurrentGear = EditorGUILayout.IntField("CurrentGear",myscript.CurrentGear);
        EditorGUILayout.EndVertical();


        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(myscript, "Changes");

            myscript.enginemode = _enginemode;
            myscript.shifttype = _shifttype;
            myscript.gearcalculationtype = _gearcaltype;
            myscript.MaxEngineRpm = MaxEngineRpm; 
            myscript.RaiseModifier = RaiseModifier;
            myscript.EngineRpm = EngineRpm;
            myscript.MaxGear = MaxGear;
            myscript.TractionFactor = TractionFactor;
            myscript.TractionMultiplier = TractionMultiplier;
            myscript.sliplimit = sliplimit;
            myscript.MaxJerkForce = MaxJerkForce;
            myscript.MaxNitroValue = MaxNitroValue;
            myscript.NitroTorque = NitroTorque;
            myscript.NitroExpense =  NitroExpense;
            myscript.EffectTime = EffectTime;
            myscript.MaxSpeed = MaxSpeed;
            myscript.MaxEngineTorque = MaxEngineTorque;
            myscript.MaxSpeedMultiplier = MaxSpeedMul;
            myscript.ReverseTopSpeed = ReverseTopSpeed;
            myscript.EngineTorqueMultiplier = MaxEngineTorqueMul;
            myscript.PickUpSource = PickupSource;
            myscript.NitroSoundSource = NitroSound;
            myscript.NitroTopSpeed = NitroSpeed;
            myscript.Debug = Debug;
          //  myscript.resetgear = ResetGear;
            //myscript.isreverse = IsReverse;
            myscript.AutoReverse = auto;
            //myscript.CurrentGear = curgear;
            myscript.NeutralOnAwake = NeutralOnAwake;
            myscript.TransmissionSoundPrefab = GearShiftSound;
          
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
