using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[AddComponentMenu("EasyVehicleSystem/ESVehicleController")]
public class ESVehicleController : MonoBehaviour
{
    [Serializable]
    public class WheelSettings
    {
        [Serializable]
        public class FrontWheels
        {
            public WheelCollider[] frontwheelcols = new WheelCollider[2];
            public Transform[] frontwheelmeshes = new Transform[2];
        }
        [Serializable]
        public class Rearwheels
        {
            public WheelCollider[] rearwheelcols = new WheelCollider[2];
            public Transform[] rearwheelmeshes = new Transform[2];
        }
        public FrontWheels frontwheels;
        public Rearwheels rearwheels;
        public ESDrift[] m_drift;
        public float sidewaysslip = 0.13f;
    }
    [Serializable]
    public class EngineSettings
    {
        public enum BrakeType
        {
            front,
            Rear,
            All
        }

        public float SteerAngle = 25f;
        public float HighSpeedSteerAngle = 15f;
        public float TargetSpeed = 50f;
        public bool UseMaxBrakeForce = true;
        public BrakeType braketype = BrakeType.All;
        public bool AirBrake = false;
        public bool PlayAirSound;
        public float timecounter;
        public float DelayTime = 2f;
        public float DestroyAirSound = 2f;
        public AudioSource AirBrakeSourcePrefab;
        public Transform AIrBrakeTransform;
        public float MaxBrakeForce;
        public float ReversePower;
        public bool OptimizeForBurnOut;
        public float steersensitivity= 0.3f;
        public float ForceFeedBack = 0.3f;
        public enum CarDriveType
        {
            FourWheel,
            RearWheel,
            FrontWheel
        }

        public CarDriveType m_cardrivetype = CarDriveType.FourWheel;
    }
    public EngineSettings m_enginesettings;
    public WheelSettings m_wheelsettings;
    public bool Neutral;
    public bool ShowReadOnly;
    public Vector3 m_ridigbodycenterofmass;
    public float TopSpeed, SteerBalanceFactor = 0.5f;
    public float CurrentSpeed;
    public float GripForce =50.0f, RotDiffLimit = 0.9f;
    [Tooltip("to apply make sure value is greater 0")]
    public float AirDrag = 5f;
    [Tooltip("to apply make sure value is greater 0")]
    public float Drag;
    public float KillDriftSpeed =10f;
    public float mul = 1;

    public float Shoebrake, Rpm;
    [HideInInspector]
    public bool ShowCursor = false;
    [HideInInspector]
    public bool lockcursor = true;
    [HideInInspector]
    public float Steer;
    [HideInInspector]
    public float Handbrake;
    [HideInInspector]
    public float M;
    [HideInInspector]
    public Quaternion oldrot;
    public float forceAppliedToWheels;
    [HideInInspector]
    public float Accel;
    [HideInInspector]
    public float RotationDifference;
    [HideInInspector]
    public Rigidbody CarRb;
    [HideInInspector]
    public ESFuelManager fuelmanager;
    [HideInInspector]
    public ESIgnition ignition;
    [HideInInspector]
    public bool usefuel;
    [HideInInspector]
    public bool uselight;
    public int currentnode;
    //private
    private Vector3 angvel, eularvec;
    private ESGearShift m_gearshift;
    private ESLightSystem light;
    private WheelHit wheelhit;
    private Vector3 CarRot;
    private Quaternion CarRotation;
    private float forwardrot, OldRot;
    private float m_Tsteer;
    private bool m_isDrifting;
    private bool m_isgrounded;
    private float m_startangdrag;
    private float mytime;
    
    // Use this for initialization
    void Awake()
    {
        if (m_enginesettings.UseMaxBrakeForce)
            m_enginesettings.MaxBrakeForce = float.MaxValue;
    }

    void Start()
    {
        CarRb = GetComponent<Rigidbody>();
        m_gearshift = GetComponent<ESGearShift>();
        m_startangdrag = CarRb.angularDrag;
       
        if (this.GetComponent<ESLightSystem>() != null)
        {
            light = GetComponent<ESLightSystem>();
            uselight = true;
        }
        else
        {
            uselight = false;
        }
        if (this.GetComponent<ESFuelManager>() != null)
        {
            fuelmanager = GetComponent<ESFuelManager>();
            usefuel = true;
        }
        else
        {
            usefuel = false;
        }
        if (this.GetComponent<ESIgnition>() != null)
        {
            ignition = GetComponent<ESIgnition>();
        }
        CarRb.centerOfMass += m_ridigbodycenterofmass;
        m_Tsteer = Steer;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Engine();
    }

    void Update()
    {

        Drifting();
        Cursor();
        if (uselight)
            light.KeyInput();
    }
    //show gizmos
    public void OnDrawGizmos()
    {
        if (Application.isPlaying == false)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position + m_ridigbodycenterofmass, Vector3.up);
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position + m_ridigbodycenterofmass, Vector3.forward);
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position + m_ridigbodycenterofmass, Vector3.right);
        }
    }
    //Drifting
    public void Drifting()
    {
       
            RotationDifference = Quaternion.Angle(oldrot, transform.rotation);
        if (CurrentSpeed < KillDriftSpeed || RotationDifference < RotDiffLimit && Mathf.Abs(M) < m_wheelsettings.sidewaysslip && m_isDrifting)
        {
            m_isDrifting = false;
            for (int i = 0; i < m_wheelsettings.m_drift.Length; i++)
            {
                m_wheelsettings.m_drift[i].m_dodrift = m_isDrifting;
            }
        }
        oldrot = transform.rotation;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            m_isDrifting = true;
        }
        if (CurrentSpeed > KillDriftSpeed)
        {
            for (int i = 0; i < m_wheelsettings.m_drift.Length; i++)
            {
                m_wheelsettings.m_drift[i].m_dodrift = m_isDrifting;
            }
        }
        // if drifting takes too long kill drift.
        if (m_isDrifting)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                mytime += Time.deltaTime;
            }
            if (Input.GetKeyUp(KeyCode.Space))
            {
                mytime = 0f;
            }
            if (mytime > 0.3f)
            {
                m_isDrifting = false;
                // reset mytime
                mytime = 0f;
            }
        }
        else
        {
            mytime = 0f;
        }
        //donut
        if (m_enginesettings.OptimizeForBurnOut && m_enginesettings.braketype == EngineSettings.BrakeType.front)
        {
            if (CurrentSpeed < KillDriftSpeed && m_wheelsettings.rearwheels.rearwheelcols[0].GetComponent<ESEffect>().currentfrictionfowardslip >
                m_wheelsettings.rearwheels.rearwheelcols[0].GetComponent<ESEffect>().fowardsliplimit && Handbrake > 0)
            {
                for (int i = 0; i < m_wheelsettings.rearwheels.rearwheelcols.Length; i++)
                    m_wheelsettings.rearwheels.rearwheelcols[i].GetComponent<ESDrift>().m_donut = true;
            }
            //
            if (RotationDifference < RotDiffLimit && m_wheelsettings.rearwheels.rearwheelcols[0].GetComponent<ESEffect>().currentfrictionfowardslip <
                m_wheelsettings.rearwheels.rearwheelcols[0].GetComponent<ESEffect>().fowardsliplimit)
            {
                for (int i = 0; i < m_wheelsettings.rearwheels.rearwheelcols.Length; i++)
                    m_wheelsettings.rearwheels.rearwheelcols[i].GetComponent<ESDrift>().m_donut = false;
            }
        }

    }
    //Engine
   
    public void Engine()
    {
        // check for fuel consumption
        if (fuelmanager != null)
        {
            if (fuelmanager.Empty)
            {
                usefuel = false;
            }
            else
            {
                usefuel = true;
            }
        }
        //
        if (ignition != null)
        {
            if (usefuel || ignition.On)
            {
                Accel = Mathf.Clamp(Input.GetAxis("Vertical") * mul, 0, 1);
                Shoebrake = -1f * Mathf.Clamp(Input.GetAxis("Vertical") * mul, -1, 0);
            }
            if (!usefuel || !ignition.On)
            {
                Accel = 0f;
                Shoebrake = 0f;
            }
        }
        else
        {
            if (fuelmanager != null)
            {
                if (usefuel)
                {
                    Accel = Mathf.Clamp(Input.GetAxis("Vertical") * mul, 0, 1);
                    Shoebrake = -1f * Mathf.Clamp(Input.GetAxis("Vertical") * mul, -1, 0);
                }
                if (!usefuel)
                {
                    Accel = 0f;
                    Shoebrake = 0f;
                }
            }
            else
            {
                Accel = Mathf.Clamp(Input.GetAxis("Vertical") * mul, 0, 1);
                Shoebrake = -1f * Mathf.Clamp(Input.GetAxis("Vertical") * mul, -1, 0);
            }
        }
        TopSpeed = m_gearshift.TopSpeed;
        Steer = Input.GetAxis("Horizontal");
        Handbrake = Input.GetAxis("Jump");
        AddTorqueToEngine(Accel, Shoebrake);
        BrakeSystem(Handbrake);
        if (Handbrake == 0 && m_enginesettings.PlayAirSound)
        {
            if (m_enginesettings.AirBrakeSourcePrefab != null)
            {
                if (m_enginesettings.AIrBrakeTransform == null)
                {
                    m_enginesettings.AIrBrakeTransform = Instantiate(m_enginesettings.AirBrakeSourcePrefab.gameObject.transform);
                }
            }
            m_enginesettings.PlayAirSound = false;
            m_enginesettings.timecounter = 0;
        }
        if (m_enginesettings.AIrBrakeTransform != null)
        {
            Destroy(m_enginesettings.AIrBrakeTransform.gameObject, m_enginesettings.DestroyAirSound);
        }
        if (uselight)
        {
            light.LightSystem(Handbrake, Accel, Shoebrake);
        }
       SteeringControl(Steer);
       SteerBalance();
         WheelAlignment();
        CarSpeed();
        GetRpm();
        Vector3 ang = CarRb.angularVelocity;
        angvel.y = ang.y;
        eularvec.y = transform.eulerAngles.y;
    }
    // AddTorque;
    public void AddTorqueToEngine(float Accel, float Shoebrake)
    {

        switch (m_enginesettings.m_cardrivetype)
        {
            case EngineSettings.CarDriveType.FourWheel:
                {
                    if (m_gearshift.isneutral || m_gearshift.isparking || m_gearshift.isclutching)
                    {
                        for (int i = 0; i < m_wheelsettings.frontwheels.frontwheelcols.Length; i++)
                        {
                            //TorqueControl(wheelhit.forwardSlip);
                            m_wheelsettings.frontwheels.frontwheelcols[i].GetGroundHit(out wheelhit);
                            M = wheelhit.sidewaysSlip;
                            m_wheelsettings.frontwheels.frontwheelcols[i].motorTorque = 0;
                            if (Shoebrake > 0 && m_gearshift.isreverse)
                            {
                                m_wheelsettings.frontwheels.frontwheelcols[i].motorTorque = 0;
                            }
                        }
                        // apply force rearwheels
                        for (int i = 0; i < m_wheelsettings.rearwheels.rearwheelcols.Length; i++)
                        {
                            //TorqueControl(wheelhit.forwardSlip);
                            m_wheelsettings.rearwheels.rearwheelcols[i].GetGroundHit(out wheelhit);
                            M = wheelhit.sidewaysSlip;

                            m_wheelsettings.rearwheels.rearwheelcols[i].motorTorque = 0;

                            if (Shoebrake > 0 && m_gearshift.isreverse)
                            {
                                m_wheelsettings.rearwheels.rearwheelcols[i].motorTorque = 0;
                            }
                        }
                        return;
                    }
                    // apply for to frontwheels
                    for (int i = 0; i < m_wheelsettings.frontwheels.frontwheelcols.Length; i++)
                    {
                        //TorqueControl(wheelhit.forwardSlip);
                        m_wheelsettings.frontwheels.frontwheelcols[i].GetGroundHit(out wheelhit);
                        M = wheelhit.sidewaysSlip;
                        m_wheelsettings.frontwheels.frontwheelcols[i].motorTorque = m_gearshift.do_nitro == false ? (Accel * forceAppliedToWheels) / 4f :
                            (Accel * m_gearshift.NitroTorque) / 4f;
                        //
                        if (CurrentSpeed > 5 && Vector3.Angle(transform.forward, CarRb.velocity) < 50f)
                        {
                            m_wheelsettings.frontwheels.frontwheelcols[i].brakeTorque = m_enginesettings.MaxBrakeForce * Shoebrake;
                        }
                        //
                        if (Shoebrake > 0 && m_gearshift.isreverse)
                        {
                            m_wheelsettings.frontwheels.frontwheelcols[i].motorTorque -= forceAppliedToWheels * Shoebrake;
                        }

                    }
                    // apply force rearwheels
                    for (int i = 0; i < m_wheelsettings.rearwheels.rearwheelcols.Length; i++)
                    {
                        //TorqueControl(wheelhit.forwardSlip);
                        m_wheelsettings.rearwheels.rearwheelcols[i].GetGroundHit(out wheelhit);
                        M = wheelhit.sidewaysSlip;
                        m_wheelsettings.rearwheels.rearwheelcols[i].motorTorque = m_gearshift.do_nitro == false ? (Accel * forceAppliedToWheels) / 4f :
                            (Accel * m_gearshift.NitroTorque) / 4f;
                        //
                        if (CurrentSpeed > 5 && Vector3.Angle(transform.forward, CarRb.velocity) < 50f)
                        {
                            m_wheelsettings.rearwheels.rearwheelcols[i].brakeTorque = m_enginesettings.MaxBrakeForce * Shoebrake;
                        }
                        //
                        if (Shoebrake > 0 && m_gearshift.isreverse)
                        {
                            m_wheelsettings.rearwheels.rearwheelcols[i].motorTorque -= forceAppliedToWheels * Shoebrake;
                        }
                    }
                }
                break;
            case EngineSettings.CarDriveType.FrontWheel:
                {
                    if (m_gearshift.isneutral || m_gearshift.isparking || m_gearshift.isclutching)
                    {
                        for (int i = 0; i < m_wheelsettings.frontwheels.frontwheelcols.Length; i++)
                        {
                            //TorqueControl(wheelhit.forwardSlip);
                            m_wheelsettings.frontwheels.frontwheelcols[i].GetGroundHit(out wheelhit);
                            M = wheelhit.sidewaysSlip;
                            m_wheelsettings.frontwheels.frontwheelcols[i].motorTorque = 0;
                            if (Shoebrake > 0 && m_gearshift.isreverse)
                            {
                                m_wheelsettings.frontwheels.frontwheelcols[i].motorTorque = 0;
                            }
                        }
                        return;
                    }
                    // apply force  to frontwheels

                    for (int i = 0; i < m_wheelsettings.frontwheels.frontwheelcols.Length; i++)
                    {
                        //TorqueControl(wheelhit.forwardSlip);
                        m_wheelsettings.frontwheels.frontwheelcols[i].GetGroundHit(out wheelhit);
                        M = wheelhit.sidewaysSlip;
                        m_wheelsettings.frontwheels.frontwheelcols[i].motorTorque = m_gearshift.do_nitro == false ? (Accel * forceAppliedToWheels) / 2f :
                            (Accel * m_gearshift.NitroTorque) / 2f;
                        //
                        if (CurrentSpeed > 5 && Vector3.Angle(transform.forward, CarRb.velocity) < 50f)
                        {
                            m_wheelsettings.frontwheels.frontwheelcols[i].brakeTorque = m_enginesettings.MaxBrakeForce * Shoebrake;
                        }
                        //
                        if (Shoebrake > 0 && m_gearshift.isreverse)
                        {
                            m_wheelsettings.frontwheels.frontwheelcols[i].motorTorque -= forceAppliedToWheels * Shoebrake;
                        }
                        //

                    }
                }
                break;
            case EngineSettings.CarDriveType.RearWheel:
                {
                    if (m_gearshift.isneutral || m_gearshift.isparking || m_gearshift.isclutching)
                    {
                        for (int i = 0; i < m_wheelsettings.rearwheels.rearwheelcols.Length; i++)
                        {
                            // TorqueControl(wheelhit.forwardSlip);
                            // m_wheelsettings.rearwheels.rearwheelcols[i].GetGroundHit(out wheelhit);
                            M = wheelhit.sidewaysSlip;
                            m_wheelsettings.rearwheels.rearwheelcols[i].motorTorque = 0;
                            if (Shoebrake > 0 && m_gearshift.isreverse)
                            {
                                m_wheelsettings.rearwheels.rearwheelcols[i].motorTorque = 0;
                            }
                        }
                        return;
                    }
                    // apply force  to rearwheels
                    for (int i = 0; i < m_wheelsettings.rearwheels.rearwheelcols.Length; i++)
                    {
                        // TorqueControl(wheelhit.forwardSlip);
                        // m_wheelsettings.rearwheels.rearwheelcols[i].GetGroundHit(out wheelhit);
                        M = wheelhit.sidewaysSlip;
                        m_wheelsettings.rearwheels.rearwheelcols[i].motorTorque = m_gearshift.do_nitro == false ? (Accel * forceAppliedToWheels) / 2f :
                            (Accel * m_gearshift.NitroTorque) / 2f;
                        //
                        if (CurrentSpeed > 5 && Vector3.Angle(transform.forward, CarRb.velocity) < 50f)
                        {
                            m_wheelsettings.rearwheels.rearwheelcols[i].brakeTorque = m_enginesettings.MaxBrakeForce * Shoebrake;
                        }
                        //
                        if (Shoebrake > 0 && m_gearshift.isreverse)
                        {
                            m_wheelsettings.rearwheels.rearwheelcols[i].motorTorque -= forceAppliedToWheels * Shoebrake;
                        }
                    }
                }
                break;
        }
        // adds more grip to CarRb
        m_wheelsettings.rearwheels.rearwheelcols[1].attachedRigidbody.AddForce(-transform.up * GripForce * CarRb.velocity.magnitude);
        if (Drag > 0)
        {
            if (Accel == 0)
            {
                CarRb.drag = Drag;
            }
            else
            {
                CarRb.drag = 0.01f;
            }
        }
        CarRb.angularDrag = m_isgrounded == false ? AirDrag : m_startangdrag;

    }
    //Brakesystem
    public void BrakeSystem(float HandBrake)
    {
        if (m_gearshift.isparking) return;
        // apply brake force to front wheels
        if (m_enginesettings.braketype == EngineSettings.BrakeType.All)
        {
            for (int i = 0; i < m_wheelsettings.frontwheels.frontwheelcols.Length; i++)
            {
                m_wheelsettings.frontwheels.frontwheelcols[i].brakeTorque = HandBrake * m_enginesettings.MaxBrakeForce;

            }
            // apply brake force to rear wheels
            for (int i = 0; i < m_wheelsettings.rearwheels.rearwheelcols.Length; i++)
            {
                m_wheelsettings.rearwheels.rearwheelcols[i].brakeTorque = HandBrake * m_enginesettings.MaxBrakeForce;

            }
        }
        else if (m_enginesettings.braketype == EngineSettings.BrakeType.front)
        {
            for (int i = 0; i < m_wheelsettings.frontwheels.frontwheelcols.Length; i++)
            {
                m_wheelsettings.frontwheels.frontwheelcols[i].brakeTorque = HandBrake * m_enginesettings.MaxBrakeForce;

            }
        }
        else if (m_enginesettings.braketype == EngineSettings.BrakeType.Rear)
        {
            for (int i = 0; i < m_wheelsettings.rearwheels.rearwheelcols.Length; i++)
            {
                m_wheelsettings.rearwheels.rearwheelcols[i].brakeTorque = HandBrake * m_enginesettings.MaxBrakeForce;

            }
        }
        if (m_enginesettings.AirBrake)
        {
            if (HandBrake > 0 && CarRb.velocity.magnitude > 0.2f)
            {
                if (m_enginesettings.AIrBrakeTransform != null)
                {
                    Destroy(m_enginesettings.AIrBrakeTransform.gameObject);
                }
                Destroy(m_enginesettings.AIrBrakeTransform, m_enginesettings.DestroyAirSound);
                m_enginesettings.timecounter += Time.deltaTime;
                if (m_enginesettings.timecounter > m_enginesettings.DelayTime)
                {
                    m_enginesettings.PlayAirSound = true;
                }
            }
        }
    }
    //steeringcontrol
    public void SteeringControl(float steer)
    {
        float changeangtospeed = 0;
        if (CurrentSpeed < m_enginesettings.TargetSpeed)
        {
            changeangtospeed = m_enginesettings.SteerAngle;
        }
        else if (CurrentSpeed > m_enginesettings.TargetSpeed)
        {
            changeangtospeed = m_enginesettings.HighSpeedSteerAngle;
        }
        //
        for (int j = 0; j < m_wheelsettings.frontwheels.frontwheelcols.Length; j++)
        {
            m_wheelsettings.frontwheels.frontwheelcols[j].steerAngle = changeangtospeed * steer;
            m_isgrounded = m_wheelsettings.frontwheels.frontwheelcols[j].isGrounded;
        }
        // m_wheelsettings.m_mywheelcols[0].steerAngle = steer * m_enginesettings.SteerAngle;
        //m_wheelsettings.m_mywheelcols[1].steerAngle = steer * m_enginesettings.SteerAngle;
    }
    //wheelAlignment
    public void GetRpm()
    {
        for (int i = 0; i < m_wheelsettings.frontwheels.frontwheelcols.Length; i++)
        {
            Rpm = m_wheelsettings.frontwheels.frontwheelcols[i].rpm;
        }
        for (int i = 0; i < m_wheelsettings.rearwheels.rearwheelcols.Length; i++)
        {
            Rpm = m_wheelsettings.rearwheels.rearwheelcols[i].rpm;
        }

    }
    public void WheelAlignment()
    {
        // make tyre meshes follow wheels;

        // align front wheel meshes
        Vector3 frontwheelposition;
        Quaternion frontwheelrotation;
        for (int i = 0; i < m_wheelsettings.frontwheels.frontwheelcols.Length; i++)
        {
            if (m_wheelsettings.frontwheels.frontwheelmeshes[i] == null)
            {
                return;
            }
            m_wheelsettings.frontwheels.frontwheelcols[i].GetWorldPose(out frontwheelposition, out frontwheelrotation);
            m_wheelsettings.frontwheels.frontwheelmeshes[i].transform.position = frontwheelposition;
            m_wheelsettings.frontwheels.frontwheelmeshes[i].transform.rotation = frontwheelrotation;
        }

        // align rear wheel meshes
        Vector3 rearwheelposition;
        Quaternion rearwheelrotation;
        for (int i = 0; i < m_wheelsettings.rearwheels.rearwheelcols.Length; i++)
        {
            if (m_wheelsettings.rearwheels.rearwheelmeshes[i] == null)
            {
                return;
            }
            m_wheelsettings.rearwheels.rearwheelcols[i].GetWorldPose(out rearwheelposition, out rearwheelrotation);
            m_wheelsettings.rearwheels.rearwheelmeshes[i].transform.position = rearwheelposition;
            m_wheelsettings.rearwheels.rearwheelmeshes[i].transform.rotation = rearwheelrotation;
            // Rpm = m_wheelsettings.frontwheels.frontwheelcols[i].rpm;
        }
    }
    //TorqueControl;

    //CarSpeed
    public void CarSpeed()
    {
        //km/h
        float Pi = Mathf.PI * 1.15f;
        CurrentSpeed = CarRb.velocity.magnitude * Pi;
        if (m_gearshift.do_nitro == false)
        {
            if (CurrentSpeed > TopSpeed)
                CarRb.velocity = (TopSpeed / Pi) * CarRb.velocity.normalized;
        }
        else
        {
            if (CurrentSpeed > m_gearshift.NitroTopSpeed)
                CarRb.velocity = (m_gearshift.NitroTopSpeed / Pi) * CarRb.velocity.normalized;
        }
    }
    //gimbal lock
    public void SteerBalance()
    {
        for (int i = 0; i < m_wheelsettings.frontwheels.frontwheelcols.Length; i++)
        {
            WheelHit wheelhit;
            m_wheelsettings.frontwheels.frontwheelcols[i].GetGroundHit(out wheelhit);
            if (wheelhit.normal == Vector3.zero)
                return;
        }
        for (int i = 0; i < m_wheelsettings.rearwheels.rearwheelcols.Length; i++)
        {
            WheelHit wheelhit;
            m_wheelsettings.rearwheels.rearwheelcols[i].GetGroundHit(out wheelhit);
            if (wheelhit.normal == Vector3.zero)
                return;
        }
        if (Mathf.Abs(OldRot - transform.eulerAngles.y) < 10f)
        {
            var alignturn = (transform.eulerAngles.y - OldRot) * SteerBalanceFactor;
            Quaternion angvelocity = Quaternion.AngleAxis(alignturn, Vector3.up);
            CarRb.velocity = angvelocity * CarRb.velocity;
        }
        OldRot = transform.eulerAngles.y;
    }

    public void Cursor()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            lockcursor = !lockcursor;
            ShowCursor = !ShowCursor;
        }
        ESCursorManger.Hide_ShowCursor(lockcursor, ShowCursor);
    }
}