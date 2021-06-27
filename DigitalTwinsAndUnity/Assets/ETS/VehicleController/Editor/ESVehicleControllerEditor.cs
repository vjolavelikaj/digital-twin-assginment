using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(ESVehicleController))]
public class ESVehicleControllerEditor : Editor
{

    public ESVehicleController myscript;
    public int tab;

    public override void OnInspectorGUI()
    {
        myscript = target as ESVehicleController;
        ESVehicleCtrlCustomInspector(myscript);
        if (GUI.changed)
        {
            EditorUtility.SetDirty(myscript);
        }
    }

    public void ESVehicleCtrlCustomInspector(ESVehicleController ESVC)
    {
        tab = GUILayout.Toolbar(tab, new string[] { "Welcome","EngineSettings", "WheelSettings", "Balance"});
        switch (tab)
        {

            case 0:
                {

                    EditorGUILayout.HelpBox("NOTE: \n -MaxSpeed and MaxEngineTorque has been moved to ESGearShift", MessageType.Info);
                    //
                    GUILayout.Space(6);
                    GUILayout.Label("Thanks for Choosing Easy Vehicle System(Upgrade),\n  now you can enjoy real vehcle physics in your game ;)", EditorStyles.boldLabel);
                }
             break;
            case 1:
                {
                    
                    float SteerAngle = new float();
                    bool UseMaxBrakeForce = new bool();
                    float MaxBrakeForce = new float();
                    float ReversePower = new float();
                    //float MaxSpeed = new float();
                    float TargetSpeed = new float();
                    float HighSpeedSteerAngle = new float();
                    bool donut = new bool();
                    bool AirBrake = new bool();
                    float DestroyAirSound = new float();
                    float Delaytime = new float();
                    //float steersensity = new float();
                    //float forcefeedback = new float();

                    AudioSource AirbrakeSourcePrefab = new AudioSource();

                    ESVehicleController.EngineSettings.CarDriveType _cardrivetype = new ESVehicleController.EngineSettings.CarDriveType();
                    ESVehicleController.EngineSettings.BrakeType _braketype = new ESVehicleController.EngineSettings.BrakeType();
                    EditorGUI.BeginChangeCheck();

                    _cardrivetype = (ESVehicleController.EngineSettings.CarDriveType)EditorGUILayout.EnumPopup("CarDriveType", myscript.m_enginesettings.m_cardrivetype);
                 
                    SteerAngle = EditorGUILayout.FloatField("SteerAngle", myscript.m_enginesettings.SteerAngle);
                   // steersensity = EditorGUILayout.Slider("SteerSensitivity",myscript.m_enginesettings.steersensitivity,0.005f,50f);
                    //forcefeedback = EditorGUILayout.Slider("ForceFeedBack", myscript.m_enginesettings.ForceFeedBack, 0.1f, 50f);
                    TargetSpeed = EditorGUILayout.FloatField("TargetSpeed", myscript.m_enginesettings.TargetSpeed);
                    HighSpeedSteerAngle = EditorGUILayout.FloatField("HighSpeedSteerAngle", myscript.m_enginesettings.HighSpeedSteerAngle);
                    EditorGUILayout.BeginVertical("Box");
                    _braketype = (ESVehicleController.EngineSettings.BrakeType)EditorGUILayout.EnumPopup("BrakeType", myscript.m_enginesettings.braketype);
                    AirBrake = EditorGUILayout.Toggle("AirBrake", myscript.m_enginesettings.AirBrake);
                    if (myscript.m_enginesettings.AirBrake)
                    {
                        Delaytime = EditorGUILayout.FloatField("AirBrakeDelayTime", myscript.m_enginesettings.DelayTime);
                        DestroyAirSound = EditorGUILayout.FloatField("DestroyAirSound", myscript.m_enginesettings.DestroyAirSound);
                        AirbrakeSourcePrefab = EditorGUILayout.ObjectField("AirBrakeSourcePrefab", myscript.m_enginesettings.AirBrakeSourcePrefab, typeof(AudioSource), true) as AudioSource; 
                    }
                    donut = EditorGUILayout.Toggle("OptimizeForBurout", myscript.m_enginesettings.OptimizeForBurnOut);
                    UseMaxBrakeForce = EditorGUILayout.Toggle("UseMaxBrakeForce", myscript.m_enginesettings.UseMaxBrakeForce);
                    if (!UseMaxBrakeForce)
                    {
                        MaxBrakeForce = EditorGUILayout.FloatField("BrakeForce", myscript.m_enginesettings.MaxBrakeForce);
                    }
                    EditorGUILayout.EndVertical();
                   
                    ReversePower = EditorGUILayout.FloatField("ReversePower", myscript.m_enginesettings.ReversePower);
                    //MaxSpeed = EditorGUILayout.FloatField("MaxSpeed", myscript.MaxSpeed);
                  

                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(myscript, "Engine Changes");
                        myscript.m_enginesettings.m_cardrivetype = _cardrivetype;
                        myscript.m_enginesettings.braketype = _braketype;
                     
                        myscript.m_enginesettings.SteerAngle = SteerAngle;
                        myscript.m_enginesettings.UseMaxBrakeForce = UseMaxBrakeForce;
                        myscript.m_enginesettings.MaxBrakeForce = MaxBrakeForce;
                        myscript.m_enginesettings.ReversePower = ReversePower;
                       // myscript.MaxSpeed = MaxSpeed;
                        myscript.m_enginesettings.OptimizeForBurnOut = donut;
                        myscript.m_enginesettings.TargetSpeed = TargetSpeed;
                        myscript.m_enginesettings.HighSpeedSteerAngle = HighSpeedSteerAngle;
                        myscript.m_enginesettings.DelayTime = Delaytime;
                        myscript.m_enginesettings.AirBrake = AirBrake;
                        myscript.m_enginesettings.AirBrakeSourcePrefab = AirbrakeSourcePrefab;
                        myscript.m_enginesettings.DestroyAirSound = DestroyAirSound;
                        //myscript.m_enginesettings.steersensitivity = steersensity;
                       // myscript.m_enginesettings.ForceFeedBack = forcefeedback;
                    }

                }
                break;
            case 2:
                {
                   
                    float RotDiffLimit = new float();
                    float KillDriftSpeed = new float();

                    ArrayEditor("m_wheelsettings");
                    EditorGUI.BeginChangeCheck();
               
                    RotDiffLimit = EditorGUILayout.FloatField("RotDiffLimit", myscript.RotDiffLimit);
                    KillDriftSpeed = EditorGUILayout.FloatField("KillDriftSpeed", myscript.KillDriftSpeed);

                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(myscript, "wheel Changes");
                        myscript.RotDiffLimit = RotDiffLimit;
                        myscript.KillDriftSpeed = KillDriftSpeed;
                    }
                    GUILayout.Space(5);

                }
                break;
            case 3:
                {
                    float Drag = new float();
                    float AngDrag = new float();
                    float SteerBalanceFactor = new float();
                    float GripForce = new float();
                    Vector3 CenterOfMass = Vector3.zero;

                    EditorGUI.BeginChangeCheck();
                    SteerBalanceFactor = EditorGUILayout.FloatField("SteerBalnceFactor", myscript.SteerBalanceFactor);
                    Drag = EditorGUILayout.FloatField("Drag", myscript.Drag);
                    AngDrag = EditorGUILayout.FloatField("AirDrag",myscript .AirDrag);
                    GripForce = EditorGUILayout.FloatField("GripForce", myscript.GripForce);
                    CenterOfMass = EditorGUILayout.Vector3Field("CenterOfMass", myscript.m_ridigbodycenterofmass);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(myscript, "balance Changes");
                        myscript.SteerBalanceFactor = SteerBalanceFactor;
                        myscript.Drag = Drag;
                        myscript.GripForce = GripForce;
                        myscript.m_ridigbodycenterofmass = CenterOfMass;
                        myscript.AirDrag = AngDrag;
                    }
                }
                break;
        }
        bool ShowReadOnly = new bool();
        bool Neutral = new bool();
        float TopSpeed = new float();
        float CurrentSpeed = new float();
        float ForceAppliedToWheels = new float();
        EditorGUI.BeginChangeCheck();

        GUILayout.Space(5);
        ShowReadOnly = EditorGUILayout.Toggle("ShowReadOnlyValues", myscript.ShowReadOnly);

        if (ShowReadOnly)
        {
            EditorGUILayout.BeginVertical("Box");
            EditorGUILayout.HelpBox("ReadOnly", MessageType.None);
            TopSpeed = EditorGUILayout.FloatField("TopSpeed", myscript.TopSpeed);
            Neutral = EditorGUILayout.Toggle("Nuetral", myscript.Neutral);
            CurrentSpeed = EditorGUILayout.FloatField("CurrentSpeed", myscript.CurrentSpeed);
            ForceAppliedToWheels = EditorGUILayout.FloatField("ForceAppliedToWheels", myscript.forceAppliedToWheels);

            EditorGUILayout.EndVertical();
        }

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(myscript, "Engine Changes");
          
            myscript.ShowReadOnly = ShowReadOnly;
            myscript.TopSpeed = TopSpeed;
            myscript.Neutral = Neutral;
            myscript.CurrentSpeed = CurrentSpeed;
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
