using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ESScenemanager : MonoBehaviour
{
    [HideInInspector]public Transform Pnode;
    [HideInInspector]public Transform[] t;
    [HideInInspector]public List<Transform> connectednode;
    [HideInInspector]public bool connect;
    [HideInInspector]public bool disconnect;
    [HideInInspector]public bool done;
    public bool shownodes = true;
    public bool UseGizmosSelected = true;
    [HideInInspector]private GameObject[] nm;
    private void OnDrawGizmos()
    {
        nm = GameObject.FindGameObjectsWithTag("Nodes");
        if (nm.Length > 0)
        {
            for (int i = 0; i < nm.Length; i++)
            {
                nm[i].GetComponent<MeshRenderer>().enabled = shownodes;
                nm[i].GetComponent<ESNodeManager>().drawline = shownodes;
                nm[i].GetComponent<ESNodeManager>().UseGizmosSelected = UseGizmosSelected;
            }
        }
        #if UNITY_EDITOR

        t = Selection.transforms;
        if (Selection.transforms.Length == 1)
        {
            if (Selection.transforms[0] != this.transform && Selection.transforms[0].GetComponent<ESNodeManager>() != null)
                Pnode = Selection.transforms[0];
           
        }
        //
        #endif
        if (t.Length > 1)
        {
            connectednode = new List<Transform>();
            for (int i = 0; i < t.Length; i++)
            {
                if (t[i] !=Pnode && t[i].GetComponent<ESNodeManager>() != null)
                {
                    connectednode.Add(t[i]);
                }
            }
        }
        //
        if (connect)
        {
            if (connectednode.Count > 0)
            {
                if (Pnode != null)
                {
                    if (Pnode.GetComponent<ESNodeManager>().ConnectedNode == null || Pnode.GetComponent<ESNodeManager>().ConnectedNode.Count == 0)
                    {
                        Pnode.GetComponent<ESNodeManager>().ConnectedNode = new List<Transform>();
                    }
                    for (int i = 0; i < connectednode.Count; ++i)
                    {
                        if (Pnode.GetComponent<ESNodeManager>().ConnectedNode.Count == 0)
                            Pnode.GetComponent<ESNodeManager>().ConnectedNode.Add(connectednode[i]);
                        for (int j = 0; j < Pnode.GetComponent<ESNodeManager>().ConnectedNode.Count; ++j)
                        {
                            if (connectednode[i] != Pnode.GetComponent<ESNodeManager>().ConnectedNode[j])
                                Pnode.GetComponent<ESNodeManager>().ConnectedNode.Add(connectednode[i]);
                        }
                        
                        
                    }
                   
                }
            }
            Pnode = null;
            connect = false;
        }
        //
        if (disconnect == true)
        {
            if (Pnode != null) 
            {
                Pnode.GetComponent<ESNodeManager>().ConnectedNode.Clear();
                connectednode.Clear();
            }
            disconnect = false;
        }
      //

    }
    //
  
}
