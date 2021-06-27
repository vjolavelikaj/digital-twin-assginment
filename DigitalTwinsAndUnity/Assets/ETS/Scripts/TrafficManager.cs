using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficManager : MonoBehaviour
{
    public List<ESTrafficLghtCtrl> trafficlights;
    public bool Stop = false;
    public bool play =  false;
    public float S_time;
    public List<ESTrafficLghtCtrl> Lastveh;

    private void Awake()
    {
        Lastveh = new List<ESTrafficLghtCtrl>();
    }
    // Update is called once per frame
    private void Update()
    {
        if (trafficlights.Count > 0)
        {
           for (int i = 0; i < trafficlights.Count; i++)
           {
               if (trafficlights[i].GetComponent<ESTrafficLghtCtrl>().LastVeh != null)
               {
                   //might be a suspect
                   if(trafficlights[i].GetComponent<ESTrafficLghtCtrl>().addtoarray == false)
                   {
                      Lastveh.Add(trafficlights[i].GetComponent<ESTrafficLghtCtrl>());
                      trafficlights[i].GetComponent<ESTrafficLghtCtrl>().addtoarray = true;
                   }
               }
               //
               if (play == true)
               {
                   trafficlights[i].GetComponent<ESTrafficLghtCtrl>().simulate = true;
                   trafficlights[i].GetComponent<ESTrafficLghtCtrl>().slowdowntime = S_time;
                   
                   //trafficlights[i].GetComponent<ESTrafficLghtCtrl>().reset = true;
               }
               else
               {
                   trafficlights[i].GetComponent<ESTrafficLghtCtrl>().simulate = false;
                   //trafficlights[i].GetComponent<ESTrafficLghtCtrl>().reset = true;
               }
               //
               
           }
        }
        //
        if (Lastveh.Count > 0)
        {
            for (int j = 0; j < Lastveh.Count; j++)
            {
                if(Lastveh[j].LastVeh == null)
                Lastveh.RemoveAt(j);
            }
        }
    }
}
