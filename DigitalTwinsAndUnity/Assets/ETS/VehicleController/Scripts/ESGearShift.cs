using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[AddComponentMenu("EasyVehicleSystem/ESGearshit")]
public class ESGearShift : MonoBehaviour
{
    // public
    #region public
    public enum ShiftTpye
    {
        Automatic,
        Manual
    }
    public enum EngineMode
    {
        Nitro,
        Normal
    }
    public enum GearRatioCalculationType
    {
        Automatic,
        Manual
    }
    public EngineMode enginemode;
    public ShiftTpye shifttype;
    public bool GearTransmissionEffect;
    public float EngineRpm, MaxEngineRpm, RaiseModifier = 15f;
    public float MaxEngineTorque;
    public float EngineTorqueMultiplier = 1000f;
    public float MaxSpeedMultiplier = 100f;
    public float CurrentEngineTorque;
    public float[] enginetorquearray;
    public float TopSpeed;
    public float ReverseTopSpeed = 100f;
    public float[] topspeedarray;
    public float MaxSpeed = 150f;
    public bool AutoReverse = true;
    public bool NeutralOnAwake = false;
    public bool Debug = false;
    public GameObject[] ExhaustFlame;
    public GameObject[] HeavyEngineSmoke;
    public AudioSource TransmissionSoundPrefab;
    public AudioSource TransmissionSoud;

    public float EffectTime = 2f;
    public int MaxGear = 7;
    public GearRatioCalculationType gearcalculationtype = GearRatioCalculationType.Automatic;
    public float[] GearRatios = { 500f, 450, 400, 350, 300, 250, 200, 150, 100, 50, 45, 40 };
    [Range(0, 1)]
    public float TractionFactor = 1;
    public float TractionMultiplier = 10f;
    [Tooltip("do not edit this ")]
    public int CurrentGear = 1;
    [Tooltip("do not edit this ")]
    public float forwardSlip;
    public float sliplimit;
    [Header("Nitro Settings")]
    [Header("to use this change engine mode to nitro")]
    public KeyCode NitroKey = KeyCode.V;
    public float MaxJerkForce;
    [Header("this must be greater than the max engine torque")]
    public float NitroTorque;
    public float NitroValue;
    public float NitroTopSpeed = 500f;
    [Range(0, 100)]
    public float MaxNitroValue;
    [Range(0, 10)]
    public float NitroExpense;
    public AudioSource PickUpSource;
    public AudioSource NitroSoundSource;
    public float tempforce;
    public bool DontShiftDown;
    [HideInInspector]
    public float mytime = 0;
    [HideInInspector]
    public bool do_nitro;
    //[HideInInspector]
    public float oldTopSpeed, OldGearRatio;
    //[HideInInspector]
    public float GearRatio, inverseGearRatio;
    //[HideInInspector]
    public int oldgear;
    [HideInInspector]
    public bool isparking;
    [HideInInspector]
    public bool isneutral;
    [HideInInspector]
    public bool isclutching;
    [HideInInspector]
    public bool isreverse;
    private bool r_mode;
    #endregion
    //private
    #region private
    private float ReturnTorque;
    private int Inversecurrentgear;
    private float ParkingBrakeTorque;
    private bool dojerk;
    private float initialforce;
    private ESVehicleController vehiclecontroller;
    private float h;
    public bool resetgear;
    [SerializeField]
    private float effecttime;
    [SerializeField]
    private bool ResetEffect;
    [SerializeField]
    private bool CallGearEffect;
    [SerializeField]
    private float BacOldTopSeed;
    [SerializeField]
    private bool has_return_oldtopspeed;
    #endregion

    private void Start()
    {
        vehiclecontroller = GetComponent<ESVehicleController>();
        if (gearcalculationtype == GearRatioCalculationType.Automatic)
        {
            CurrentEngineTorque = MaxEngineTorque;
            CurrentGear = 1;
            GearRatio = (float)CurrentGear / (float)MaxGear;
            Inversecurrentgear = MaxGear;
            oldgear = CurrentGear;
            inverseGearRatio = (float)Inversecurrentgear / (float)MaxGear;
            TopSpeed = MaxSpeed * GearRatio;
            vehiclecontroller.forceAppliedToWheels = MaxEngineTorque * inverseGearRatio;
            tempforce = vehiclecontroller.forceAppliedToWheels;

        }
        else if (gearcalculationtype == GearRatioCalculationType.Manual)
        {
            CurrentGear = 1;

            System.Array.Resize(ref topspeedarray, GearRatios.Length);
            System.Array.Resize(ref enginetorquearray, GearRatios.Length);
            for (int i = GearRatios.Length; i > 0; i--)
            {
                topspeedarray[(topspeedarray.Length) - i] = MaxSpeedMultiplier * GearRatios[i - 1];
                enginetorquearray[i - 1] = EngineTorqueMultiplier * GearRatios[i - 1];
            }

            CurrentEngineTorque = enginetorquearray[CurrentGear - 1];
            tempforce = CurrentEngineTorque;
            vehiclecontroller.forceAppliedToWheels = tempforce;
            TopSpeed = topspeedarray[CurrentGear - 1];
        }
        ParkingBrakeTorque = float.MaxValue;
        isneutral = NeutralOnAwake;
        NitroValue = MaxNitroValue;
        ReturnTorque = MaxEngineTorque;
        for (int i = 0; i < ExhaustFlame.Length; i++)
        {
            ExhaustFlame[i].GetComponent<ParticleSystem>().Stop();
        }
    }

    private void FixedUpdate()
    {
        GoNeutral_Parking();
        ApplyClutch();
        Clutching();
        TorqueControl();
        Reverse();
        Gear();
        EmitHeavyEngineSmoke();
        NitroEngine();
        if (CallGearEffect)
        {
            CreateGearEffect(EffectTime, true);
        }
        //makes sure engine rpm stays stable at all time.
        if (EngineRpm > MaxEngineRpm)
        {
            EngineRpm = MaxEngineRpm;
        }

    }
    #region CalculateEngineRpm
    private void CalcEngineRpm()
    {
        if (!isclutching)
        {
            if (!vehiclecontroller.Neutral)
            {
                if (shifttype == ShiftTpye.Automatic)
                {
                    if (gearcalculationtype == GearRatioCalculationType.Automatic)
                    {
                        EngineRpm = EngineRpm <= MaxEngineRpm ? EngineRpm = vehiclecontroller.CurrentSpeed * inverseGearRatio : EngineRpm;
                    }
                    else
                    {
                        EngineRpm = EngineRpm <= MaxEngineRpm ? EngineRpm = vehiclecontroller.CurrentSpeed * GearRatios[CurrentGear - 1] : EngineRpm;
                    }
                }
                if (shifttype == ShiftTpye.Manual)
                {
                    if (vehiclecontroller.CurrentSpeed < vehiclecontroller.TopSpeed)
                    {
                        if (gearcalculationtype == GearRatioCalculationType.Automatic)
                        {
                            EngineRpm = EngineRpm <= MaxEngineRpm ? EngineRpm = vehiclecontroller.CurrentSpeed * inverseGearRatio : EngineRpm;
                        }
                        else
                        {
                            EngineRpm = EngineRpm <= MaxEngineRpm ? EngineRpm = vehiclecontroller.CurrentSpeed * GearRatios[CurrentGear - 1] : EngineRpm;
                        }
                    }
                    if ((vehiclecontroller.Handbrake > 0 && vehiclecontroller.CurrentSpeed < 0.3f) || vehiclecontroller.CurrentSpeed >= vehiclecontroller.TopSpeed)
                    {
                        if (vehiclecontroller.Accel > 0)
                        {
                            if (gearcalculationtype == GearRatioCalculationType.Automatic)
                            {
                                if (CurrentGear == 1)
                                {

                                    EngineRpm += 1 * RaiseModifier * inverseGearRatio * 0.02f;
                                }
                                else
                                {
                                    EngineRpm += 1 * RaiseModifier * inverseGearRatio * 0.02f;
                                }
                            }
                            else
                            {
                                if (CurrentGear == 1)
                                {

                                    EngineRpm += 1 * RaiseModifier * GearRatios[CurrentGear - 1] * 0.02f;
                                }
                                else
                                {
                                    EngineRpm += 1 * RaiseModifier * GearRatios[CurrentGear - 1] * 0.02f;
                                }
                            }
                        }
                        else
                        {
                            if (gearcalculationtype == GearRatioCalculationType.Automatic)
                            {
                                if (EngineRpm > vehiclecontroller.TopSpeed)
                                {
                                    EngineRpm -= 1 * RaiseModifier * inverseGearRatio * 0.2f;
                                }
                            }
                            else
                            {
                                if (EngineRpm > vehiclecontroller.TopSpeed)
                                {
                                    EngineRpm -= 1 * RaiseModifier * GearRatios[CurrentGear - 1] * 0.2f;
                                }
                            }
                        }
                        if (vehiclecontroller.Accel < 0)
                        {
                            if (gearcalculationtype == GearRatioCalculationType.Automatic)
                            {
                                EngineRpm = EngineRpm <= MaxEngineRpm ? EngineRpm = vehiclecontroller.CurrentSpeed * inverseGearRatio : EngineRpm;
                            }
                            else
                            {
                                EngineRpm = EngineRpm <= MaxEngineRpm ? EngineRpm = vehiclecontroller.CurrentSpeed * GearRatios[CurrentGear - 1] : EngineRpm;
                            }
                        }
                    }
                }
            }
        }
    }
    #endregion
    #region Clutching
    private void Clutching()
    {
        if (!isneutral)
        {
            if (isclutching)
            {
                if (vehiclecontroller.Accel > 0)
                {
                    //EngineRpm = EngineRpm < MaxEngineRpm ? EngineRpm += Time.deltaTime * 60f : EngineRpm;
                    if (EngineRpm < MaxEngineRpm)
                        EngineRpm += Time.deltaTime * 60f;
                }
                else if (vehiclecontroller.Accel <= 0)
                {
                    //EngineRpm = EngineRpm > 0f ? EngineRpm -= Time.deltaTime * 60f : EngineRpm;
                    if (EngineRpm > 0f)
                        EngineRpm -= Time.deltaTime * 60f;
                }
                if (EngineRpm > MaxEngineRpm)
                {
                    EngineRpm = MaxEngineRpm;
                }

            }
        }
    }
    #endregion
    #region Reverse
    private void Reverse()
    {
        if (AutoReverse)
        {
            if (Input.GetAxis("Vertical") < 0)
            {
                if (!isreverse)
                {
                    resetgear = true;
                }
                isreverse = true;
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                isreverse = !isreverse;
                resetgear = true;
            }
        }


        if (vehiclecontroller.Accel > 0)
        {
            isreverse = false;
        }
        if (isreverse && vehiclecontroller.CurrentSpeed < 5f)
        {
            if (resetgear)
            {
                CallResetGear();
            }

        }
        if (isreverse && vehiclecontroller.CarRb.velocity.magnitude < 5f)
        {
            if (TopSpeed != ReverseTopSpeed)
            {
                TopSpeed = ReverseTopSpeed;
                tempforce = vehiclecontroller.m_enginesettings.ReversePower;
                CurrentEngineTorque = tempforce;
                vehiclecontroller.forceAppliedToWheels = tempforce;

            }

        }
        if (isreverse)
        {
            CalcEngineRpm();
            r_mode = true;
        }
        if (!isreverse && r_mode)
        {
           

            if (TopSpeed == ReverseTopSpeed)
            {
               CallResetGear();
               r_mode = false;
            }
        }
    }
    #endregion
    #region TorqueControl
    private void TorqueControl()
    {
        if (isneutral) return;
        WheelHit wheelhit = new WheelHit();
        switch (vehiclecontroller.m_enginesettings.m_cardrivetype)
        {
            case ESVehicleController.EngineSettings.CarDriveType.FourWheel:
                {
                    for (int i = 0; i < vehiclecontroller.m_wheelsettings.frontwheels.frontwheelcols.Length; i++)
                    {
                        vehiclecontroller.m_wheelsettings.frontwheels.frontwheelcols[i].GetGroundHit(out wheelhit);
                    }
                    for (int i = 0; i < vehiclecontroller.m_wheelsettings.rearwheels.rearwheelcols.Length; i++)
                    {
                        vehiclecontroller.m_wheelsettings.rearwheels.rearwheelcols[i].GetGroundHit(out wheelhit);
                    }
                }
                break;
            case ESVehicleController.EngineSettings.CarDriveType.FrontWheel:
                {
                    for (int i = 0; i < vehiclecontroller.m_wheelsettings.frontwheels.frontwheelcols.Length; i++)
                    {
                        vehiclecontroller.m_wheelsettings.frontwheels.frontwheelcols[i].GetGroundHit(out wheelhit);
                    }
                }
                break;
            case ESVehicleController.EngineSettings.CarDriveType.RearWheel:
                {
                    for (int i = 0; i < vehiclecontroller.m_wheelsettings.rearwheels.rearwheelcols.Length; i++)
                    {
                        vehiclecontroller.m_wheelsettings.rearwheels.rearwheelcols[i].GetGroundHit(out wheelhit);
                    }
                }
                break;
        }
        forwardSlip = wheelhit.forwardSlip;

        if (Mathf.Abs(forwardSlip) >= sliplimit && vehiclecontroller.forceAppliedToWheels >= 0)
        {
            vehiclecontroller.forceAppliedToWheels -= TractionMultiplier * TractionFactor;
        }
        else
        {
            if (vehiclecontroller.forceAppliedToWheels < tempforce)
            {
                vehiclecontroller.forceAppliedToWheels += TractionMultiplier * TractionFactor;
                //  vehiclecontroller.forceAppliedToWheels = currentengineforce;
            }
        }

    }
    #endregion
    #region Gear
    private void Gear()
    {
        if (vehiclecontroller.Neutral || isparking) return;
        #region Manual Transmission
        if (shifttype == ShiftTpye.Manual && !isreverse)
        {
            CalcEngineRpm();
            Clutching();
            if (Input.GetKeyDown(KeyCode.LeftShift) && isclutching)
            {
                if (!vehiclecontroller.Neutral)
                {
                    GearTransmissionUp();
                }
                // vehiclecontroller.Neutral = false;
            }
            if (Input.GetKeyDown(KeyCode.RightShift) && isclutching)
            {
                if (!vehiclecontroller.Neutral)
                {
                    if (CurrentGear > 1)
                    {
                        if (gearcalculationtype == GearRatioCalculationType.Automatic)
                        {
                            EngineRpm = vehiclecontroller.CurrentSpeed * inverseGearRatio;
                        }
                        else
                        {
                            EngineRpm = vehiclecontroller.CurrentSpeed * GearRatios[CurrentGear - 1];
                        }
                    }
                    GearTransmissionDown();
                }
                vehiclecontroller.Neutral = false;
            }
        }
        #endregion
        //
        #region automatic transmission
        if (shifttype == ShiftTpye.Automatic && !isreverse)
        {
            if (vehiclecontroller.Neutral)
            {
                if (Input.GetKeyDown(KeyCode.LeftShift))
                {
                    vehiclecontroller.Neutral = false;
                }
            }
            if (gearcalculationtype == GearRatioCalculationType.Automatic)
            {
                EngineRpm = vehiclecontroller.CurrentSpeed * inverseGearRatio;
            }
            else
            {
                EngineRpm = vehiclecontroller.CurrentSpeed * GearRatios[CurrentGear - 1];
            }
            if (vehiclecontroller.CurrentSpeed >= vehiclecontroller.TopSpeed)
            {
                if(CurrentGear < MaxGear)
                GearTransmissionUp();
            }
            if (vehiclecontroller.CurrentSpeed <= oldTopSpeed)
            {
                 if (!DontShiftDown)
                   GearTransmissionDown();
            }
            //
            if (vehiclecontroller.Accel == 0 && has_return_oldtopspeed == false)
            {
                oldTopSpeed = BacOldTopSeed;
                has_return_oldtopspeed = true;
            }
        }
        #endregion
    }
    #endregion
    //
    private IEnumerator ShiftDelay(float shiftdelaytime)
    {
        //vehiclecontroller.Neutral = true;
        has_return_oldtopspeed = false;
        DontShiftDown = true;
        isneutral = true;
        // oldTopSpeed = float.MaxValue;
        BacOldTopSeed = MaxSpeed * OldGearRatio;
        oldTopSpeed = oldTopSpeed - 10f;
        yield return new WaitForSeconds(shiftdelaytime);
        oldTopSpeed = (MaxSpeed * OldGearRatio) * 0.5f;
        isneutral = false;
        DontShiftDown = false;
    }
    //
    #region GearTransmissionUp
    private void GearTransmissionUp()
    {
        if (gearcalculationtype == GearRatioCalculationType.Automatic)
        {
            if (CurrentGear < MaxGear)
            {
                CurrentGear++;

                if (GearTransmissionEffect)
                {
                    ResetEffect = true;
                    CallGearEffect = true;
                }
                if (CurrentGear > 1)
                {
                    oldgear = CurrentGear - 1;
                }
                Inversecurrentgear--;
                inverseGearRatio = (float)Inversecurrentgear / (float)MaxGear;
                tempforce = MaxEngineTorque * inverseGearRatio;

                vehiclecontroller.forceAppliedToWheels = tempforce;
                GearRatio = (float)CurrentGear / (float)MaxGear;
                TopSpeed = MaxSpeed * GearRatio;
                //
                OldGearRatio = (float)oldgear / (float)MaxGear;
                StartCoroutine(ShiftDelay(0.5f));
                //oldTopSpeed = MaxSpeed * OldGearRatio;
            }
        }
        else if (gearcalculationtype == GearRatioCalculationType.Manual)
        {
            if (CurrentGear < MaxGear)
            {
                oldTopSpeed = TopSpeed;
                CurrentGear++;
                if (GearTransmissionEffect)
                {
                    ResetEffect = true;
                    CallGearEffect = true;
                }

                if (CurrentGear > 1)
                {
                    oldgear = CurrentGear - 1;
                }

                CurrentEngineTorque = enginetorquearray[CurrentGear - 1];

                tempforce = CurrentEngineTorque;
                vehiclecontroller.forceAppliedToWheels = tempforce;

                TopSpeed = topspeedarray[CurrentGear - 1];
                oldTopSpeed = topspeedarray[oldgear - 1];
            }
        }
    }
    #endregion
    #region GearTransmissionDown
    private void GearTransmissionDown()
    {
        if (gearcalculationtype == GearRatioCalculationType.Automatic)
        {
            if (CurrentGear > 1)
            {
                CurrentGear--;
                Inversecurrentgear++;
                inverseGearRatio = (float)Inversecurrentgear / (float)MaxGear;
                //
                if (CurrentGear > 1)
                {
                    oldgear = CurrentGear - 1;
                }
                GearRatio = (float)CurrentGear / (float)MaxGear;
                tempforce = MaxEngineTorque * inverseGearRatio;

                vehiclecontroller.forceAppliedToWheels = tempforce;
                TopSpeed = MaxSpeed * GearRatio;
                OldGearRatio = (float)oldgear / (float)MaxGear;
                oldTopSpeed = MaxSpeed * OldGearRatio;
            }
        }
        else if (gearcalculationtype == GearRatioCalculationType.Manual)
        {
            if (CurrentGear > 1)
            {

                CurrentGear--;

                if (CurrentGear > 1)
                {
                    oldgear = CurrentGear - 1;
                }
                //
                CurrentEngineTorque = enginetorquearray[CurrentGear - 1];
                tempforce = CurrentEngineTorque;
                vehiclecontroller.forceAppliedToWheels = tempforce;
                TopSpeed = topspeedarray[CurrentGear - 1];
                oldTopSpeed = topspeedarray[oldgear - 1];
            }
        }

    }
    #endregion
    #region Neutral_Parking
    // Neutral
    private void GoNeutral_Parking()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            isneutral = !isneutral;
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            isparking = !isparking;
        }

        if (isneutral)
        {
            vehiclecontroller.Neutral = true;
            if (vehiclecontroller.CarRb.velocity.magnitude < 0.01 || vehiclecontroller.CarRb.velocity.magnitude > 0.01 && vehiclecontroller.Neutral)
            {
                if (vehiclecontroller.Accel > 0)
                {
                    EngineRpm = EngineRpm < MaxEngineRpm ? EngineRpm += Time.deltaTime * RaiseModifier : EngineRpm;
                }
                else
                {
                    EngineRpm = EngineRpm > 0f ? EngineRpm -= Time.deltaTime * RaiseModifier : EngineRpm;
                }
            }
            if (EngineRpm > MaxEngineRpm)
            {
                EngineRpm = MaxEngineRpm;
            }
        }
        else
        {
            vehiclecontroller.Neutral = false;
        }
        //apply parking brake

        if (isparking)
        {
            if (vehiclecontroller.Accel > 0)
            {
                EngineRpm = EngineRpm < MaxEngineRpm ? EngineRpm += Time.deltaTime * RaiseModifier : EngineRpm;
            }
            else
            {
                EngineRpm = EngineRpm > 0f ? EngineRpm -= Time.deltaTime * RaiseModifier : EngineRpm;
            }
            for (int i = 0; i < vehiclecontroller.m_wheelsettings.frontwheels.frontwheelcols.Length; i++)
            {
                vehiclecontroller.m_wheelsettings.frontwheels.frontwheelcols[i].brakeTorque = ParkingBrakeTorque;
            }
            for (int i = 0; i < vehiclecontroller.m_wheelsettings.rearwheels.rearwheelcols.Length; i++)
            {
                vehiclecontroller.m_wheelsettings.rearwheels.rearwheelcols[i].brakeTorque = ParkingBrakeTorque;
            }
        }
        if (!isparking)
        {
            for (int i = 0; i < vehiclecontroller.m_wheelsettings.frontwheels.frontwheelcols.Length; i++)
            {
                vehiclecontroller.m_wheelsettings.frontwheels.frontwheelcols[i].brakeTorque = 0f;
            }
            for (int i = 0; i < vehiclecontroller.m_wheelsettings.rearwheels.rearwheelcols.Length; i++)
            {
                vehiclecontroller.m_wheelsettings.rearwheels.rearwheelcols[i].brakeTorque = 0f;
            }
        }
    }
    #endregion
    #region ApplyClutch
    private void ApplyClutch()
    {
        if (Input.GetKey(KeyCode.C))
        {
            isclutching = true;
        }
        else
        {
            isclutching = false;
        }

    }
    #endregion
    #region NitroEngine
    public void NitroEngine()
    {
        if (enginemode == EngineMode.Nitro)
        {
            if (isneutral || isclutching || isparking)
            {
                if (vehiclecontroller.Accel > 0)
                {
                    NeutralNitroEffect(EngineRpm, 50f, false);
                }

            }
            if (CurrentGear == 1)
            {
                if (isneutral || isclutching)
                {
                    if (vehiclecontroller.Accel > 0)
                    {
                        if (initialforce < MaxJerkForce)
                        {
                            initialforce += 10;
                        }
                        dojerk = true;
                    }
                    else
                    {
                        if (initialforce > 0)
                        {
                            initialforce -= 5;
                        }
                    }
                }
                else
                {
                    if (dojerk)
                    {
                        mytime += 1.5f * Time.deltaTime;
                        if (mytime > 0.5f)
                        {
                            Dojerk(initialforce);
                            initialforce = 0;
                            mytime = 0;
                            dojerk = false;
                        }
                    }
                }
            }
            if (Input.GetKeyDown(NitroKey))
            {
                if (NitroValue > 1 && vehiclecontroller.CarRb.velocity.magnitude > 10f)
                {
                    do_nitro = true;
                }
            }
            if (do_nitro)
            {
                if (NitroValue > 1)
                {
                    //AddNitroForce.
                    NitroValue -= NitroExpense;
                    for (int i = 0; i < ExhaustFlame.Length; i++)
                    {
                        ExhaustFlame[i].GetComponent<ParticleSystem>().Emit(1);
                    }
                }
                else
                {

                    for (int i = 0; i < ExhaustFlame.Length; i++)
                    {
                        ExhaustFlame[i].GetComponent<ParticleSystem>().Stop();
                    }

                    do_nitro = false;

                }
            }
            if (vehiclecontroller.Handbrake > 0 || vehiclecontroller.Shoebrake > 0)
            {
                if (do_nitro)
                {
                    for (int i = 0; i < ExhaustFlame.Length; i++)
                    {
                        ExhaustFlame[i].GetComponent<ParticleSystem>().Stop();
                    }
                    do_nitro = false;
                }
            }
            if (vehiclecontroller.CarRb.velocity.magnitude < 10f)
            {
                if (do_nitro)
                {
                    do_nitro = false;
                }
            }
            if (!do_nitro)
            {
                MaxEngineTorque = ReturnTorque;
                if (NitroSoundSource != null)
                {
                    if (NitroSoundSource.isPlaying)
                    {
                        NitroSoundSource.Stop();
                    }
                }
            }
            if (do_nitro)
            {
                if (NitroSoundSource != null)
                {
                    if (!NitroSoundSource.isPlaying)
                    {
                        NitroSoundSource.Play();
                    }
                }
            }

        }

    }
    #endregion
    #region PickUpNitro
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<ESNitroSetup>() != null)
        {
            if (NitroValue < MaxNitroValue)
            {
                PickUpSource.clip = other.gameObject.GetComponent<ESNitroSetup>().PickUpSound;
                PickUpSource.Play();
                //print("Picked Up" + other.gameObject.name);
                float addtonitrovalue = MaxNitroValue - NitroValue;
                float initialnitroval = NitroValue;
                if (addtonitrovalue > other.gameObject.GetComponent<ESNitroSetup>().NitroValue)
                {
                    NitroValue = other.gameObject.GetComponent<ESNitroSetup>().NitroValue + initialnitroval;
                }
                if (addtonitrovalue < other.gameObject.GetComponent<ESNitroSetup>().NitroValue)
                {
                    NitroValue = MaxNitroValue;
                }

                Destroy(other.gameObject);
            }
            else
            {
                // print("Did Not Pick Up" + other.gameObject.name);
            }
        }

    }
    #endregion
    private void Dojerk(float initialforce)
    {
        if (vehiclecontroller.CarRb.velocity.magnitude > 1f)
        {
            //print("done");
            if (vehiclecontroller.m_wheelsettings.frontwheels.frontwheelcols[0] != null)
            {
                vehiclecontroller.CarRb.AddForceAtPosition(vehiclecontroller.m_wheelsettings.frontwheels.frontwheelcols[0].transform.up * initialforce,
                    vehiclecontroller.m_wheelsettings.frontwheels.frontwheelcols[0].transform.position, ForceMode.Impulse);
            }
            if (vehiclecontroller.m_wheelsettings.frontwheels.frontwheelcols[1] != null)
            {
                vehiclecontroller.CarRb.AddForceAtPosition(vehiclecontroller.m_wheelsettings.frontwheels.frontwheelcols[1].transform.up * initialforce,
                    vehiclecontroller.m_wheelsettings.frontwheels.frontwheelcols[1].transform.position, ForceMode.Impulse);
            }

        }

    }
    //
    #region GearRatio
    private void CallResetGear()
    {

        if (gearcalculationtype == GearRatioCalculationType.Automatic)
        {
            CurrentGear = 1;
            oldgear = 1;
            Inversecurrentgear = MaxGear;
            inverseGearRatio = (float)Inversecurrentgear / (float)MaxGear;
            tempforce = MaxEngineTorque * inverseGearRatio;

            vehiclecontroller.forceAppliedToWheels = tempforce;
            GearRatio = (float)CurrentGear / (float)MaxGear;
            TopSpeed = MaxSpeed * GearRatio;
            resetgear = false;
        }
        else if (gearcalculationtype == GearRatioCalculationType.Manual)
        {
            CurrentGear = 1;
            CurrentEngineTorque = enginetorquearray[CurrentGear - 1];
            tempforce = CurrentEngineTorque;

            vehiclecontroller.forceAppliedToWheels = tempforce;
            TopSpeed = topspeedarray[CurrentGear - 1];
            oldTopSpeed = 0f;
        }
    }
    #endregion
    //

    //
    private void NeutralNitroEffect(float val, float maxval, bool usesound)
    {
        if (val < maxval)
        {
            for (int i = 0; i < ExhaustFlame.Length; i++)
            {
                ExhaustFlame[i].GetComponent<ParticleSystem>().Emit(1);
            }
            if (TransmissionSoundPrefab != null)
            {
                if (TransmissionSoud == null && usesound)
                {
                    TransmissionSoud = Instantiate(TransmissionSoundPrefab);
                    if (ExhaustFlame[0] != null)
                    {
                        TransmissionSoud.transform.position = ExhaustFlame[0].transform.position;
                    }
                    TransmissionSoud.transform.parent = this.transform;
                }
            }
        }
        else
        {
            if (TransmissionSoud != null)
            {
                Destroy(TransmissionSoud);
            }
            for (int i = 0; i < ExhaustFlame.Length; i++)
            {
                ExhaustFlame[i].GetComponent<ParticleSystem>().Stop();
            }
        }

    }
    // waittime for transsimision effects
    private void CreateGearEffect(float maxtime, bool usesound)
    {

        if (ResetEffect)
        {

            if (TransmissionSoud != null)
            {
                Destroy(TransmissionSoud);
            }
            for (int i = 0; i < ExhaustFlame.Length; i++)
            {
                ExhaustFlame[i].GetComponent<ParticleSystem>().Stop();
            }
            effecttime = 0f;
            ResetEffect = false;
        }
        if (effecttime < maxtime)
        {
            effecttime += Time.deltaTime;
            for (int i = 0; i < ExhaustFlame.Length; i++)
            {
                ExhaustFlame[i].GetComponent<ParticleSystem>().Emit(1);
            }
            if (TransmissionSoundPrefab != null)
            {
                if (TransmissionSoud == null && usesound)
                {
                    TransmissionSoud = Instantiate(TransmissionSoundPrefab);
                    TransmissionSoud.transform.parent = this.transform;
                }
            }
        }
        if (effecttime > maxtime)
        {
            if (TransmissionSoud != null)
            {
                Destroy(TransmissionSoud);
            }
            for (int i = 0; i < ExhaustFlame.Length; i++)
            {
                ExhaustFlame[i].GetComponent<ParticleSystem>().Stop();
            }
            effecttime = 0f;
            CallGearEffect = false;

        }
    }
    private void EmitHeavyEngineSmoke()
    {
        if (HeavyEngineSmoke.Length > 0)
        {
            for (int i = 0; i < HeavyEngineSmoke.Length; i++)
            {
                if (CurrentGear == 1 && Input.GetAxis("Vertical") > 0)
                    HeavyEngineSmoke[i].GetComponent<ParticleSystem>().Emit(1);
            }
        }
    }
}
