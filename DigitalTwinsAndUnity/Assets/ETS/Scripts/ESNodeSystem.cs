using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//[ExecuteAlways]
public class ESNodeSystem : MonoBehaviour
{
    [HideInInspector]public bool done;
    [HideInInspector]public GameObject nodeprefab;
    [HideInInspector]public Transform LastcreatedNode, Pnode;
    [HideInInspector]public List<Transform> nodelist;
    [HideInInspector]public Vector3 lastnodepos;
    [HideInInspector]public Transform[] g;
    //[HideInInspector]
    public int max;
    public enum AlignAxis
    {
        X,
        Z,
        free
    }
    [HideInInspector]
    public AlignAxis GetAlign = AlignAxis.free;
    
    // Update is called once per frame
    void Update()
    {
        if (Application.isPlaying)
        {
            if (nodelist.Count > 0)
            {
                for (int i = 0; i < nodelist.Count; ++i)
                {
                    //nodelist[i].name = "Node" + i.ToString();
                    if (nodelist[i].GetComponent<MeshRenderer>() != null)
                        nodelist[i].GetComponent<MeshRenderer>().enabled = false;
                }
            }       
        }
      
    }
    //

    private void OnDrawGizmos()
    {
        Transform[] nodes = GetComponentsInChildren<Transform>();

        nodelist = new List<Transform>();
        for (int i = 0; i < nodes.Length; ++i)
        {
            if (nodes[i] != this.transform)
            {
                nodelist.Add(nodes[i]);
            }
        }
        //
        if (max != nodes.Length)
        {
            if (nodelist.Count > 0)
            {
                for (int i = 0; i < nodelist.Count; ++i)
                {
                    nodelist[i].name = "Node" + i.ToString();

                }
            }
            if (nodelist.Count > 0)
            {
                lastnodepos = nodelist[nodelist.Count - 1].position;
            }
            if (nodelist.Count == 0)
            {
                lastnodepos = Vector3.zero;
            }

            max = nodes.Length;
        }
    }
}
