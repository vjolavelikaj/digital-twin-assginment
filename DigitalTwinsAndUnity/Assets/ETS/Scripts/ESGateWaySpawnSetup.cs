using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ESGateWaySpawnSetup : MonoBehaviour
{
    public bool CanSpawn = true;
    [HideInInspector]
    public float distanceapart = 20f,m_DistApartFromPlayer = 50f, m_SpawnAngle = 50f, LineOfSight = 200f;
    public bool findplayer = true;
    public Transform playertransform;
    [HideInInspector]
    public Transform obj;
    [HideInInspector]public bool add, remove;
    [HideInInspector]
    public bool done;
    [HideInInspector]
    public Transform temptrans;
    [HideInInspector]
    public bool greater;
    //
    public Transform TargetNode;
    private void Awake()
    {
        CanSpawn = true;
        playertransform = GameObject.FindGameObjectWithTag("Player").transform;
    }
    //
    private void OnDrawGizmos()
    {
        AddTarget();
    }
    //
    private void OnDrawGizmosSelected()
    {
        if (TargetNode != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(TargetNode.position, 6f);
        }
    }
    private void FixedUpdate()
    {
        CallSpawn();
    }
    //
    private float CalculateDistance(Vector3 currentposition, Vector3 targetposition)
    {
        Vector3 offset = targetposition - currentposition;
        float sqrlen = offset.sqrMagnitude;
        return sqrlen;
    }
    //
    void CallSpawn()
    {
        if (playertransform != null)
        {
            if (Vector3.Angle(playertransform.forward, transform.position - playertransform.position) > m_SpawnAngle)
            {
                greater = true;
            }
            else
            {
                greater = false;
            }
           
        }
        //
        if (greater)
        {
            if (obj != null)
            {
                spawnme();
            }
            else
            {
                if (CalculateDistance(this.transform.position, playertransform.position) > m_DistApartFromPlayer*m_DistApartFromPlayer)
                {
                    CanSpawn = true;
                }
                else
                {
                    CanSpawn = false;
                }
            }
        }
        else
        {
            if (playertransform != null)
            {

                if (obj != null)
                {
                    if (CalculateDistance(this.transform.position, obj.position) > (distanceapart * distanceapart) && CalculateDistance(this.transform.position, playertransform.position) > LineOfSight * LineOfSight)
                    {
                        CanSpawn = true;
                    }
                    else
                    {
                        CanSpawn = false;
                    }
                }
                else
                {

                    if (CalculateDistance(this.transform.position, playertransform.position) > LineOfSight *LineOfSight)
                    {
                        CanSpawn = true;
                    }
                    else
                    {
                        CanSpawn = false;
                    }
                }
            }
            else
            {
                CanSpawn = false;
            }
        }
    }
    //
    void spawnme()
    {
        if (playertransform != null)
        {
            if (CalculateDistance(this.transform.position, obj.position) > (distanceapart*distanceapart) && CalculateDistance(this.transform.position, playertransform.position) > m_DistApartFromPlayer * m_DistApartFromPlayer)
            {
                CanSpawn = true;
            }
            else
            {
                CanSpawn = false;
            }

        }
        else
        {
            if (CalculateDistance(this.transform.position, obj.position) > distanceapart * distanceapart)
            {
                CanSpawn = true;
            }
            else
            {
                CanSpawn = false;
            }
        }
    }
    //
    void OnTriggerEnter(Collider other)
    {
        obj = other.transform;
    }
   //
    void AddTarget() {
        if (add)
        {
#if UNITY_EDITOR

            temptrans = Selection.activeTransform;
            if (temptrans != null)
            {
                if(temptrans != this)
                TargetNode = temptrans;
            }
            add = false;
            //
#endif
        }
        else if (remove)
        {
            if (TargetNode != null)
            {
                TargetNode = null;
            }
            remove = false;
        }
    }
}
