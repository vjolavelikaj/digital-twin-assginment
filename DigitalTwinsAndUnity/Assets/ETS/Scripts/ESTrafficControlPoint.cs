using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ESTrafficControlPoint : MonoBehaviour
{
    public TrafficManager[] managers;
    public bool FullStop = false;
    public float slowdowntime = 2;
    public float cooldowntime = 30.0f;
    public float cooldowntimer = 0.0f;
    public int selectedindex = 0;
    public float S_time;
    //
    private void Start()
    {
        cooldowntimer = 0;
        S_time = cooldowntime - slowdowntime;
        if (managers.Length > 0)
        {
            managers[selectedindex].play = true;
            managers[selectedindex].S_time = S_time;
        }
    }
    //
    private void Update()
    {
        if (cooldowntimer < (cooldowntime + 5))
        {
            cooldowntimer += Time.deltaTime;
        }
        if (cooldowntimer >= cooldowntime)
        {
            if (managers.Length > 0)
            {
                if (selectedindex <= managers.Length)
                {
                    if (selectedindex != 0)
                    {
                        if (managers[selectedindex - 1].Lastveh.Count > 0)
                        {
                            return;
                        }
                        managers[selectedindex - 1].play = false;
                    }
                    //
                    if (selectedindex == 0)
                    {
                        if (managers[0].Lastveh.Count > 0)
                        {
                            return;
                        }
                        managers[0].play = false;
                    }
                    //
                    if (selectedindex == managers.Length)
                    {
                        selectedindex = 0;
                    }
                    //
                    selectedindex++;
                    managers[selectedindex - 1].play = true;
                    managers[selectedindex - 1].S_time = S_time;
                }
            }
            cooldowntimer = 0.0f;
        }
        //
    }
    //

}
