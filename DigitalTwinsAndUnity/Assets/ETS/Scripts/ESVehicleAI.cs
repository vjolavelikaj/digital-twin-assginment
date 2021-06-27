using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ESVehicleAI : MonoBehaviour 
{

    public Transform TargetNode;
    [Header("WheelSettings")]
    public WheelCollider[] frontwheel;
    public WheelCollider[] rearwheel;
    public Transform[] frontwheelmeshes;
    public Transform[] rearwheelmeshes;
  
    //
    [Header("EngineSettings")]
    public float EngineTorque;
    public float Brakefroce = 50000f;
    //
    [Header("SteerSettings")]
    public float maxsteerangle = 35f;
    public float targetangle;
    public float turnspeed =100;

    //
    [Header("AI Settings")]
    public float SteerBalanceFactor = 0.5f;
    public float topspeed = 150.0f;
    public float smoothtargetspeed = 75.0f;
    public float CautiosAngle = 20f;
    public float CautiosSpeed = 15f;
    public float RealseBrakeTime = 0.5f;
    public float closerangspeed = 5.5f;
    public bool AvoidObstacles = true;
    public bool SmartDelect = true;
    public float SpawnDistance = 400f;
    [HideInInspector]public Transform player;
    //
    /*
    [Header("SensorSettings")]
    public Vector3 sensorposition = Vector3.zero;
    public float sensorsize = 1.5f;
    public float fowarddist = 5.5f;
    public float spacing = 2;
    public float sensorangle = 10f;
    public float length = 10f;
    public float angledist = 10f;
    public float TurnOffSensorDist = 30f;
     */
    //
    [Header("PathSettings")]
    public float DistanceApart = 10f;
    public float overtakedistapart = 50f;
    public float overtakespeed = 200f;
    public bool forcereturnspeed;
    //
    [Header("Debug")]
    public float currentspeed;
    public float Brakemul;
    public float backuptopspeed;
    [HideInInspector]
    public float nextnodeangle, angle, tempangle;
    public Rigidbody CarRb;
    [HideInInspector]
    public float BrakeCounter;
    [HideInInspector]
    public bool doRange = false;
    [HideInInspector]
    public Transform NextTarget;
    [HideInInspector]
    public bool returnmotor;
    [HideInInspector]
    public bool SpeedClamped = false;
    [HideInInspector]
    public bool Stop;
    [HideInInspector]
    public bool callsensor;
    [HideInInspector]
    public ESTrafficLghtCtrl trafficlightctrl;
    [HideInInspector]
    public Transform TriggerObject;
    [HideInInspector]
    public bool increasedistcheck;
    [HideInInspector]
    public float backupdistapart;
    [HideInInspector]
    public bool AngleBraking = false;
    [HideInInspector]
    public bool Trafficlightbraking = false,TriggerObjectBraking = false;
    [HideInInspector]
    public bool returntriggerstay = false;
    private float sensorwaittime;
    private float returnspeedrout;
    private float OldRot;
    private float disttimer;
   //
    private void Awake()
    {
        backuptopspeed = topspeed;
    }
    //
    private void Start()
    {
        /*
        int NextNodeIndex = TargetNode.GetComponent<ESNodeManager>().NextNode != null ?
                       Random.Range(0, TargetNode.GetComponent<ESNodeManager>().ConnectedNode.Count + 1) :
                       Random.Range(0, TargetNode.GetComponent<ESNodeManager>().ConnectedNode.Count);
                       */
        CarRb = this.GetComponent<Rigidbody>();
        backupdistapart = DistanceApart;
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
  
    //
    private void FixedUpdate()
    {
        ControlAI();
       
        if (!Trafficlightbraking && !TriggerObjectBraking && !AngleBraking &&!Stop)
        {
            if (topspeed == 0)
            {
                forcereturnspeed = true;
            }
            if (returnmotor == true)
            {
                returnmotor = false;
            }
            CarRb.drag = 0.0f;
            Brakemul = 0.0f;
           
        }
        else
        {
            CarRb.drag = 500f;
            Brakemul = 1;
        }

    }
    //
    private void LateUpdate()
    {
        CheckDist();
        Behaviour();
    }
    //
    private void ControlAI()
    {
        AI();
        RegulateAI();
    }
    //
    //
    private void AI()
    {
        ApplySteer();
        SteerBalance();
        CarSpeed();
        Motor();
        WheelAlignment();
        for (int i = 0; i < frontwheel.Length; ++i)
        {
             frontwheel[i].brakeTorque = Brakefroce * Brakemul;
        }
        for (int i = 0; i < rearwheel.Length; ++i)
        {
             rearwheel[i].brakeTorque = Brakefroce * Brakemul;  
        } 
    }
    //
    private void Motor()
    {
        if (returnmotor)
        {
            for (int i = 0; i < frontwheel.Length; ++i)
            {
                frontwheel[i].motorTorque = 0;
                //

            }
            for (int i = 0; i < rearwheel.Length; ++i)
            {
                rearwheel[i].motorTorque = 0;
                //
            }
        }
        else
        {
            for (int i = 0; i < frontwheel.Length; ++i)
            {
                frontwheel[i].motorTorque = EngineTorque;
                //

            }
            for (int i = 0; i < rearwheel.Length; ++i)
            {
                rearwheel[i].motorTorque = EngineTorque;
                //
            }
        }
      
    }
    //
    public void SteerBalance()
    {
        for (int i = 0; i < frontwheel.Length; i++)
        {
            WheelHit wheelhit;
            frontwheel[i].GetGroundHit(out wheelhit);
            if (wheelhit.normal == Vector3.zero)
                return;
        }
        for (int i = 0; i < rearwheel.Length; i++)
        {
            WheelHit wheelhit;
            rearwheel[i].GetGroundHit(out wheelhit);
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
    //
    private void ApplySteer()
    {

        LerpToSteerAngle();
        Vector3 relativevec = transform.InverseTransformPoint(TargetNode.position);
        relativevec = relativevec / relativevec.magnitude;
        float newsteer = (relativevec.x / relativevec.magnitude) * maxsteerangle;
        targetangle = newsteer;
       
    }
    //
    private float CalculateDistance(Vector3 currentposition, Vector3 targetposition)
    {
        Vector3 offset = targetposition - currentposition;
        float sqrlen = offset.sqrMagnitude;
        return sqrlen;
    }
    //
    private void CheckDist()
    { 
        if (TargetNode != null)
        {
            //
            if (CalculateDistance(this.transform.position, TargetNode.position) < DistanceApart * DistanceApart)
            {
                if (TargetNode.GetComponent<ESNodeManager>().ConnectedNode.Count == 0)
                {
                    if (TargetNode.GetComponent<ESNodeManager>().NextNode != null)
                    {
                        TargetNode = TargetNode.GetComponent<ESNodeManager>().NextNode;
                    }
                }
                else
                {
                   // print("Suck it");
                    TargetNode = NextTarget;
                }
               
            }
            //
            if (CalculateDistance(this.transform.position, TargetNode.position) < 50f * 50f)
            {
                doRange = true;
                int NextNodeIndex = 0;
                if (doRange)
                {
                   NextNodeIndex = Random.Range(0, TargetNode.GetComponent<ESNodeManager>().ConnectedNode.Count + 1);
                   //NextNodeIndex = 0;
                    doRange = false;
                }

                //print(NextNodeIndex);
                if (NextNodeIndex != TargetNode.GetComponent<ESNodeManager>().ConnectedNode.Count)
                {
                    if(TargetNode.GetComponent<ESNodeManager>().ConnectedNode.Count > 0)
                    NextTarget = TargetNode.GetComponent<ESNodeManager>().ConnectedNode[NextNodeIndex];

                }
                else
                {
                    if (TargetNode.GetComponent<ESNodeManager>().NextNode != null)
                    {
                        NextTarget = TargetNode.GetComponent<ESNodeManager>().NextNode;
                    }
                    else
                    {
                        if (TargetNode.GetComponent<ESNodeManager>().ConnectedNode.Count > 0)
                        NextTarget = TargetNode.GetComponent<ESNodeManager>().ConnectedNode[NextNodeIndex-1];
                    }
                }

            }
        }
       
    }
    //
    private void LerpToSteerAngle()
    {
        for (int i = 0; i < frontwheel.Length; i++)
        {
            frontwheel[i].steerAngle = Mathf.Lerp(frontwheel[i].steerAngle, targetangle, Time.deltaTime * turnspeed);
        }
    }
    //
    private void RegulateAI()
    {
        if (player == null) return;

        if (CalculateDistance(player.position, this.transform.position) > SpawnDistance * SpawnDistance)
        {
           this.gameObject.SetActive(false);
        }
    }
    //
    private void Behaviour()
    {
        /*
               NextTarget = TargetNode.GetComponent<ESNodeManager>().NextNode
              != null ? TargetNode.GetComponent<ESNodeManager>().NextNode : null;
              */
        Vector3 direction = TargetNode != null ? TargetNode.position - transform.position : new Vector3();
        //
        //cautious ai

        //check for curve,
        if (TargetNode.GetComponent<ESNodeManager>().NextNode != null && NextTarget != null)
        {
            Vector3 Nextdirection = NextTarget.position - transform.position;
            float checksharpang = Vector3.Angle(transform.forward, Nextdirection);
            nextnodeangle = checksharpang;
            if (nextnodeangle > 5 && currentspeed > smoothtargetspeed && !increasedistcheck)
            {
                //print("naa angle not sharp ");
                //AngleBraking = true;
              
            }
            else
            {
                //AngleBraking = false;
            }
            if (nextnodeangle > CautiosAngle && currentspeed > CautiosSpeed && !increasedistcheck)
            {
                // print("next angle sharp ");
                SpeedClamped = true;
                topspeed = CautiosSpeed;
            }
            else
            {
                if (SpeedClamped)
                {
                    ReturnTopseed();
                    SpeedClamped = false;
                }
            }
            angle = tempangle;
        }
        //
        if (TargetNode != null)
        {
            float tempangle = Vector3.Angle(transform.forward, direction);
            angle = tempangle;
            //
            if (angle > 5 && currentspeed > smoothtargetspeed && !increasedistcheck)
            {
                //remove comments if u prefer Ai to Check Angle before applying brakes;
                //AngleBraking = true;
            }
            else
            {
               // AngleBraking = false;
            }
            if (angle > CautiosAngle && currentspeed > CautiosSpeed && !increasedistcheck)
            {
                //print("naa angle sharp ");
                SpeedClamped = true;
                topspeed = CautiosSpeed;
            }
            else
            {
                if (SpeedClamped)
                {
                    ReturnTopseed();
                    SpeedClamped = false;
                }
            }
            //
            if (TargetNode.GetComponent<ESNodeManager>().ConnectedNode.Count > 0 && currentspeed > CautiosSpeed && !increasedistcheck)
            {
                SpeedClamped = true;
                topspeed = CautiosSpeed;
            }
            else
            {
                if (SpeedClamped)
                {
                    ReturnTopseed();
                    SpeedClamped = false;
                }
            }
            //
            if (SpeedClamped == false)
            {
                if (currentspeed != backuptopspeed)
                    ReturnTopseed();
            }
            //
            //
            if (SmartDelect)
            {
                if (CalculateDistance(this.transform.position, TargetNode.position) < 5f * 5f)
                {
                    if (TargetNode.GetComponent<ESNodeManager>().NextNode == null && TargetNode.GetComponent<ESNodeManager>().ConnectedNode.Count == 0)
                    {
                        //Destroy(this.gameObject);
                        this.gameObject.SetActive(false);
                    }
                }
                //
            }
            //
            if (TriggerObject != null)
            {
                if (TriggerObject.tag == "AIVehicle")
                {
                    TriggerObject = null;
                    returntriggerstay = false;
                }
            }
            //
            if (TriggerObject != null)
            {
                if (TriggerObject.root.name == "AI" || TriggerObject.root.tag == "Player")
                {
                    forcereturnspeed = true;
                    TriggerObjectBraking = true;
                    topspeed = 0f;
                    BrakeCounter = 0;
                }
                //
                if (CalculateDistance(TriggerObject.transform.position, this.transform.position) > 20 * 20)
                {
                    TriggerObject = null;
                }
            }
            else
            {
                if (forcereturnspeed)
                {
                    TriggerObjectBraking = false;
                    ReturnTopseed();
                    if(topspeed == backuptopspeed)
                     forcereturnspeed = false;
                }
                TriggerObjectBraking = false;
            }
            
        }
       
        //
        if (Stop)
        {
            topspeed = 0f;
            BrakeCounter = 0;
            Trafficlightbraking = true;
        }
        else
        {
            Trafficlightbraking = false;
        }
        //
        if (trafficlightctrl != null)
        {
            if (trafficlightctrl.transform.GetComponent<ESTrafficLghtCtrl>().red)
            {
                Stop = true;
            }
            else if (trafficlightctrl.transform.GetComponent<ESTrafficLghtCtrl>().green)
            {
                callsensor = false;
                ReturnTopseed();
                Stop = false;
            }
        }
        //
       
        //
      
    }
    //
 
    //
    public void CarSpeed()
    {
        //km/h
        float Pi = Mathf.PI * 1.15f;
        currentspeed = CarRb.velocity.magnitude * Pi;
            if (currentspeed > topspeed)
                CarRb.velocity = (topspeed / Pi) * CarRb.velocity.normalized;
    }
    //
 
    private void ReturnTopseed()
    {
        if (Application.isPlaying == false) return;
        if (topspeed != backuptopspeed)
        {
            returnspeedrout += Time.deltaTime;
            if (returnspeedrout >= 1.5f)
            {
                returnspeedrout = 0;
                topspeed = backuptopspeed; 
                //;)
            }
        }
    }
    //
 
    //
   
    //
    public void WheelAlignment()
    {
        // make tyre meshes follow wheels;
        if (currentspeed < 0.09f) return;
        // align front wheel meshes
        Vector3 frontwheelposition;
        Quaternion frontwheelrotation;
        if (frontwheelmeshes.Length > 0)
        {
            for (int i = 0; i < frontwheel.Length; i++)
            {
                if (frontwheelmeshes[i] == null)
                {
                    return;
                }
                frontwheel[i].GetWorldPose(out frontwheelposition, out frontwheelrotation);
                frontwheelmeshes[i].transform.position = frontwheelposition;
                frontwheelmeshes[i].transform.rotation = frontwheelrotation;
            }
        }
        // align rear wheel meshes
        Vector3 rearwheelposition;
        Quaternion rearwheelrotation;
        if (rearwheelmeshes.Length > 0)
        {
            for (int i = 0; i <rearwheel.Length; i++)
            {
                if (rearwheelmeshes[i] == null)
                {
                    return;
                }
                rearwheel[i].GetWorldPose(out rearwheelposition, out rearwheelrotation);
                rearwheelmeshes[i].transform.position = rearwheelposition;
                rearwheelmeshes[i].transform.rotation = rearwheelrotation;
                // Rpm = m_wheelsettings.frontwheels.frontwheelcols[i].rpm;
            }
        }
    }
    // 
    //
    void OnTriggerEnter(Collider Other)
    {
        if (Other.CompareTag("TrafficLight"))
        {
            //trafficlight ahead
            if (Other.transform.GetComponent<ESTrafficLghtCtrl>()!= null)
                trafficlightctrl = Other.transform.GetComponent<ESTrafficLghtCtrl>();
            if (Other.transform.root.GetComponent<ESTrafficLghtCtrl>() != null)
               trafficlightctrl =  Other.transform.root.GetComponent<ESTrafficLghtCtrl>();
            if (Other.transform.parent.GetComponent<ESTrafficLghtCtrl>() != null)
                trafficlightctrl = Other.transform.parent.GetComponent<ESTrafficLghtCtrl>();        
        }
        //
        if (Other.transform.root.name == "AI" || Other.transform.root.tag == "Player")
        {
            TriggerObjectBraking = true;
            TriggerObject = Other.gameObject.transform;
        }
       // this.GetComponent<SphereCollider>().enabled = false;
        callsensor = true;
    }
    //
    void OnTriggerStay(Collider Other)
    {
        //
        if (returntriggerstay) return;
        if (Other.GetComponent<SphereCollider>() == null)
        {
            if (Other.transform.root.name == "AI" || Other.transform.root.tag == "Player")
            {
                TriggerObjectBraking = true;
                TriggerObject = Other.gameObject.transform;
                returntriggerstay = true;
            }
        }
        // this.GetComponent<SphereCollider>().enabled = false;
        callsensor = true;
    }
    //
    void OnTriggerExit()
    {
        //
        returntriggerstay = false;
        TriggerObject = null;
        if (trafficlightctrl != null)
        {
          trafficlightctrl.LastVeh = this.transform;
        }
        trafficlightctrl = null;
        callsensor = false;
        Stop = false;
       
        ReturnTopseed();
    }
}
 