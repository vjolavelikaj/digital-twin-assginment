using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ESNodeManager : MonoBehaviour
{
    public Transform NextNode;
    public List<Transform> ConnectedNode;
    public GameObject mesh;
    public float targetangle;
    public bool CanSpawn;
    [HideInInspector]public bool drawline;
    public bool UseGizmosSelected = true;
    public GameObject scenemanager;
    void Awake()
    {
        CanSpawn = true;
    }
  
    //
    private void OnDrawGizmosSelected()
    {
        if (scenemanager == null)
        {
            scenemanager = GameObject.Find("SceneManager");
        }
        if (UseGizmosSelected)
        {
            Gizmos_Me();
        }
    }
    //
    private void OnDrawGizmos()
    {
        if (scenemanager == null)
        {
            scenemanager = GameObject.Find("SceneManager");
        }
        if (!UseGizmosSelected)
        {
            Gizmos_Me();
        }
    }
    //
    private void Gizmos_Me()
    {
        if (scenemanager == null)
        {
            scenemanager = GameObject.Find("SceneManager");
            if (scenemanager == null)
            {
                GameObject sm = new GameObject("SceneManager");
                sm.AddComponent<ESScenemanager>();
                scenemanager = sm;
            }
        }
        //scenemanager = GameObject.Find("SceneManager");
        if (gameObject.tag != "Nodes")
        {
            this.gameObject.tag = "Nodes";
        }
        //Gizmos.DrawMesh(mesh, 1,this.transform.position,this.transform.rotation  )
        if (NextNode != null)
        {
            transform.LookAt(NextNode);
        }
        //
        //m.a = 0.5f;
        if (mesh == null)
        {
            mesh = Resources.Load("stuff/Arrow") as GameObject;
        }
        Vector4 c = new Vector4(1, 0, 0, 0.8f);
        Gizmos.color = c;

        if (NextNode != null && drawline == true)
        {
            Gizmos.DrawMesh(mesh.GetComponent<MeshFilter>().sharedMesh, -1, (this.transform.position + NextNode.position) * 0.5f, this.transform.rotation, new Vector3(1.5f, 0.125f, 2f));

            // Vector4 d = new Vector4(1, 1, 1, 0.035f);
            Gizmos.color = Color.grey;
            Debug.DrawLine(this.transform.position, NextNode.position);
        }
        //
        if (ConnectedNode != null)
        {
            if (ConnectedNode.Count > 0)
            {
                CanSpawn = false;
                Gizmos.color = Color.black;
                Gizmos.DrawWireSphere(this.transform.position, 3f);
                if (drawline)
                {
                    for (int i = 0; i < ConnectedNode.Count; ++i)
                    {
                        if (ConnectedNode[i] == null)
                        {
                            ConnectedNode.RemoveAt(i);
                        }
                        Gizmos.color = Color.red;
                        if (ConnectedNode[i].position != null)
                        {
                            Gizmos.DrawLine(this.transform.position, ConnectedNode[i].position);

                            Vector4 c1 = new Vector4(1, 0, 5, 0.8f);
                            Gizmos.color = c1;
                            Vector3 dir = ConnectedNode[i].transform.position - this.transform.position;
                            Quaternion rot = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 5);
                            Gizmos.DrawMesh(mesh.GetComponent<MeshFilter>().sharedMesh, -1, (this.transform.position + ConnectedNode[i].position) * 0.5f, rot, new Vector3(1.5f, 0.125f, 2f));
                        }
                    }
                }

            }
        }

    }
    //
    void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Untagged")
        {
            CanSpawn = false;
        }
    }
    //
    void OnTriggerExit(Collider other)
    {
        if (other.tag != "Untagged")
        {
            CanSpawn = true;
        }
    }
}
