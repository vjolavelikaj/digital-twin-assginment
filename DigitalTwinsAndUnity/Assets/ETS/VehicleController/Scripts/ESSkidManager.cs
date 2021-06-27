using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[AddComponentMenu("EasyVehicleSystem/ESSkidManager")]

public class ESSkidManager : MonoBehaviour
{

    private GameObject[] skidinscene;
    //
    private void Update()
    {
        ManageSkid();
    }
    //
    private void ManageSkid()
    {
        skidinscene = GameObject.FindGameObjectsWithTag("SkidTrashHolder");
        for (int i = 0; i < skidinscene.Length; i++)
        {
            skidinscene[i].transform.parent = this.transform;
        }
    }
}
