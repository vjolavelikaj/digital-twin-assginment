using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[AddComponentMenu("EasyVehicleSystem/ESPath")]

public class ESPath : MonoBehaviour
{
    // update this next;
    public Color linecolor;
    private List<Transform> nodes = new List<Transform>();

    private void OnDrawGizmos()
    {
        Gizmos.color = linecolor;

        Transform[] pathtrans = GetComponentsInChildren<Transform>();
        nodes = new List<Transform>();
        for (int i = 0; i < pathtrans.Length; i++)
        {
            if (pathtrans[i] != transform)
            {
                nodes.Add(pathtrans[i]);
            }
        }
        for (int i = 0; i < nodes.Count; i++)
        {
            Vector3 curnode = nodes[i].position;
            Vector3 prevnode = Vector3.zero;
            if (i > 0)
            {
                prevnode = nodes[i - 1].position;
            }
            else if (i == 0 && nodes.Count > 1)
            {
                prevnode = nodes[nodes.Count - 1].position;
            }
            Gizmos.DrawLine(prevnode, curnode);
            Gizmos.DrawWireSphere(curnode, 0.5f);
        }
    }
}

