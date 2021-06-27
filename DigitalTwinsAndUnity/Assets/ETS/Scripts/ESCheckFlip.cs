using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ESCheckFlip : MonoBehaviour
{
 [HideInInspector]
    public bool Reset = false;
    [HideInInspector]
    public float mytime = 0;
    public Transform player;
  
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }
    //
    void Update()
    {
        CheckForFlipState();
    }
    //
    void CheckForFlipState()
    {
        //check for upsidedown
        if (player == null) return;
        if (Vector3.Dot(transform.up, Vector3.down) > 0)
        {
            mytime += Time.deltaTime;
            if (mytime > 5)
            {
                if(Vector3.Distance(this.transform.position, player.transform.position ) > 100)
                Destroy(this.gameObject);
            }
        }
        //check for side ways
        else if (Mathf.Abs(Vector3.Dot(transform.up, Vector3.down)) < 0.125f)
        {
            mytime += Time.deltaTime;
            if (mytime > 5)
            {
                if (Vector3.Distance(this.transform.position, player.transform.position) > 100)
                    Destroy(this.gameObject);
            }

        }
        else if (Mathf.Abs(Vector3.Dot(transform.right, Vector3.down)) > 0.825f)
        {
            mytime += Time.deltaTime;
            if (mytime > 5)
            {
                if (Vector3.Distance(this.transform.position, player.transform.position) > 100)
                    Destroy(this.gameObject);
            }
        }
        //
    }
}
