using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAi : MonoBehaviour
{
  
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "AIVehicle")
        Destroy(other.gameObject);
    }
}
