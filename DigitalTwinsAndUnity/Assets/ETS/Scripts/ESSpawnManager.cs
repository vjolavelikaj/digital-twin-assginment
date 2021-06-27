using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ESSpawnManager : MonoBehaviour
{
   
    //gateway
    [HideInInspector]public ESGateWaySpawnSetup[] spawnpoints;
    public float checkspawndist = 10;
    public Transform[] vehicles;
    public float Delaytime = 5f;
    [Tooltip("dont use above 100 vehicles if this for mobile")]
    [Range(1,100)]
    public int MaxAllowedVehicles = 10;
    public float distanceapart = 20f, m_DistApartFromPlayer = 50f, m_SpawnAngle = 50f, LineOfSight = 200f;
    [Tooltip("Dont mess around with this ;}{")]
    [Range(0, 15)]
    public float SpawnRate = 0.19f;
    [Tooltip("Dont mess around with this ;}{")]
    [Range(0, 15)]
    public float SpawnStartTime = 1.5f;
    //private
    private int spawnindex;
    private int vehicleindex;
    private float counter;
    private float starter;
    public Transform player;
    [HideInInspector]
    public List<GameObject> SpawnedVeh = new List<GameObject>();
    //
    [HideInInspector]
    public Transform veh;
    [HideInInspector]
    public bool parentspawnedvehicles = true;
    private GameObject Pvehicles;
    [SerializeField]private int numofveh;
    List<ESGateWaySpawnSetup> nearestspawn; 
    //
    private void Awake()
    {
        Spawn();
        spawnpoints = GameObject.FindObjectsOfType<ESGateWaySpawnSetup>();
    }
    //
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
     
        if (spawnpoints.Length > 0)
        {
            for (int i = 0; i < spawnpoints.Length; i++)
            {
                spawnpoints[i].LineOfSight = LineOfSight;
                spawnpoints[i].m_DistApartFromPlayer = m_DistApartFromPlayer;
                spawnpoints[i].m_SpawnAngle = m_SpawnAngle;
                spawnpoints[i].distanceapart = distanceapart;
            }
        }
        //
        InvokeRepeating("ManageSpawnAI", SpawnStartTime, SpawnRate);
    }
    //
    //
    private float CalculateDistance(Vector3 currentposition, Vector3 targetposition)
    {
        Vector3 offset = targetposition - currentposition;
        float sqrlen = offset.sqrMagnitude;
        return sqrlen;
    }
    //
    //
    private void ManageSpawnAI()
    { 
            nearestspawn  = new List<ESGateWaySpawnSetup>();
            for (int i = 0; i < spawnpoints.Length; ++i)
            {
               if (CalculateDistance(player.position, spawnpoints[i].transform.position) < 400 * 400)
               {
                   nearestspawn.Add(spawnpoints[i]);
               }
            }
                vehicleindex = Random.Range(0, SpawnedVeh.Count);
                spawnindex = Random.Range(0, nearestspawn.Count);

                if (SpawnedVeh[vehicleindex].gameObject.activeSelf == false)
                {
                    veh = SpawnedVeh[vehicleindex].transform;
                    if (!nearestspawn[spawnindex].CanSpawn)
                    {
                        return;
                    }
                    if (nearestspawn[spawnindex].CanSpawn)
                    {
                        //set vehicles position
                        veh.localPosition = nearestspawn[spawnindex].transform.position;
                        veh.transform.LookAt(nearestspawn[spawnindex].TargetNode.position);
                        //
                        veh.GetComponent<ESVehicleAI>().TargetNode = nearestspawn[spawnindex].TargetNode;
                        veh.GetComponent<ESVehicleAI>().trafficlightctrl = null;
                        veh.GetComponent<ESVehicleAI>().TriggerObject = null;
                        veh.GetComponent<ESVehicleAI>().callsensor = false;
                        veh.GetComponent<ESVehicleAI>().Stop = false;
                        veh.GetComponent<ESVehicleAI>().Trafficlightbraking = false;
                        veh.GetComponent<ESVehicleAI>().AngleBraking = false;
                        veh.GetComponent<ESVehicleAI>().topspeed = veh.GetComponent<ESVehicleAI>().backuptopspeed;
                        veh.GetComponent<ESVehicleAI>().Brakemul = 0.0f;
                        veh.GetComponent<ESVehicleAI>().returntriggerstay = false;
                        SpawnedVeh[vehicleindex].SetActive(true);
                    }
                }
                else
                {
                    return;
                }
                //
    }
    //
    private void Spawn()
    {
        if (Pvehicles == null)
        {
            GameObject parentveh = new GameObject("AI");
            Pvehicles = parentveh;
        }
        SpawnedVeh = new List<GameObject>();
        //
        for (int i = 0; i < MaxAllowedVehicles; ++i)
        {
          vehicleindex = Random.Range(0,vehicles.Length);
          GameObject spawned = Instantiate(vehicles[vehicleindex].gameObject, Vector3.zero, Quaternion.identity, Pvehicles.transform);
          SpawnedVeh.Add(spawned);
          spawned.SetActive(false);
        }
        numofveh = SpawnedVeh.Count;
    }
}
