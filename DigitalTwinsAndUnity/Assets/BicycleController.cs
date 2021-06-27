
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BicycleController : MonoBehaviour
{
    public GameObject bicycle;

    // Start is called before the first frame update
    void Start()
    {

    }


  // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision c) {
        if(c.collider.tag == "Exit"){
            Destroy(bicycle);
        }
    }

}