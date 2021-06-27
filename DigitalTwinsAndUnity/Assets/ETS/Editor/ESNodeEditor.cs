
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
//[CanEditMultipleObjects]
[CustomEditor(typeof(ESNodeSystem))]
public class ESNodeEditor : Editor
{

    ESNodeSystem scripts;
    public override void OnInspectorGUI()
    {
        scripts = target as ESNodeSystem;
        EditorGUI.BeginChangeCheck();


        if (EditorGUI.EndChangeCheck())
        {
            Undo.RegisterCompleteObjectUndo(scripts, "fuck");
        }
        //
        if (GUI.changed)
        EditorUtility.SetDirty(scripts);
        base.OnInspectorGUI();
    }

    public void OnSceneGUI()
    {
        scripts = target as ESNodeSystem;

        CreatePathSettings();
        Event e = Event.current;
        scripts.nodeprefab = scripts.nodeprefab == null ? Resources.Load("Node/Node") as GameObject : scripts.nodeprefab;
        if (Event.current.type == EventType.KeyDown)
        {
            if (e.keyCode == KeyCode.A && !scripts.done)
            {
                CallNodes(scripts);
                scripts.done = true;
            }
        }


        if (Event.current.type == EventType.KeyUp)
        {
            scripts.done = false;
        }
        //



        if (GUI.changed)
        EditorUtility.SetDirty(scripts);
    }
    //
    private void CreatePathSettings()
    {
        Handles.BeginGUI();
        EditorGUI.BeginChangeCheck();

        Rect boxrect = new Rect(0, 0, 150, 100);
        GUIStyle boxstyle = new GUIStyle();
        boxstyle.border = GUI.skin.box.border;
        boxstyle.alignment = TextAnchor.UpperCenter;
        boxstyle.normal.textColor = Color.white;
        boxstyle.normal.background = Resources.Load("stuff/settingspan") as Texture2D;
      
        GUI.Box(boxrect, "PathSettings",boxstyle);
        //
        GUIStyle L_style = new GUIStyle();
        L_style.normal.textColor = Color.white;
        EditorGUI.LabelField(new Rect(5, 20, 20, 20), "Align:",L_style);
        GUI.Box(new Rect(5, 40, 140, 5), "");
        ESNodeSystem.AlignAxis alignAxis = new ESNodeSystem.AlignAxis();
        alignAxis = (ESNodeSystem.AlignAxis)EditorGUI.EnumPopup(new Rect(53, 20, 50, 20), scripts.GetAlign);
        EditorGUI.LabelField(new Rect(10, 47, 50, 20), "sub", L_style);
        if (GUI.Button(new Rect(5, 67, 50, 15),"-"))
        {
            Undo.RegisterFullObjectHierarchyUndo(scripts.gameObject, "Undo last");
            if (scripts.nodelist.Count > 0)
            {
                DestroyImmediate(scripts.nodelist[scripts.nodelist.Count - 1].gameObject);
            }
        
        }
        EditorGUI.LabelField(new Rect(70, 47, 50, 20), "clear", L_style);
        if (GUI.Button(new Rect(58, 67, 80, 15), "ClearNode"))
        {
            Undo.RegisterFullObjectHierarchyUndo(scripts.gameObject, "Undo full delete");
            if (scripts.nodelist.Count > 0)
            {
                for (int i = 0; i < scripts.nodelist.Count; ++i)
                {
                    DestroyImmediate(scripts.nodelist[i].gameObject);
                }
            }
            
            scripts.nodelist.Clear();
        }
        Handles.EndGUI();

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(scripts, "Changes to scripts");
            scripts.GetAlign = alignAxis;
        }
    }
    //
    private void CallNodes(ESNodeSystem es)
    {
       
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray.origin, ray.direction, out hit, Mathf.Infinity))
        {
            // Transform t = Instantiate(es.nodeprefab.transform, hit.point, Quaternion.identity);
            //creates a parent
            GameObject go = new GameObject("Node");
            // Create a custom game object
            go.AddComponent<MeshFilter>();
            go.AddComponent<MeshRenderer>();
            go.AddComponent<ESNodeManager>();
            Transform[] nodes = scripts.GetComponentsInChildren<Transform>();
            List<Transform> nodelist = new List<Transform>();
            for (int i = 0; i < nodes.Length; ++i)
            {
                if (nodes[i] != scripts.transform)
                {
                    nodelist.Add(nodes[i]);
                }
            }
            //
            if (nodelist.Count > 0)
            {
                scripts.LastcreatedNode = nodelist[nodelist.Count - 1];
            }
          
            if (scripts.LastcreatedNode != null)
            {
                scripts.nodelist[scripts.nodelist.Count - 1].GetComponent<ESNodeManager>().NextNode = go.transform;
            }
            go.GetComponent<MeshFilter>().sharedMesh = es.nodeprefab.GetComponent<MeshFilter>().sharedMesh;
            go.GetComponent<MeshRenderer>().sharedMaterial = es.nodeprefab.GetComponent<MeshRenderer>().sharedMaterial;
            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            go.transform.position = hit.point;
            switch (scripts.GetAlign)
            {
                case ESNodeSystem.AlignAxis.X:
                    Vector3 v = go.transform.position;

                    v.x = scripts.lastnodepos.x;
                    go.transform.position = v;
                    break;
                case ESNodeSystem.AlignAxis.Z:
                    Vector3 v1 = go.transform.position;

                    v1.z = scripts.lastnodepos.z;
                    go.transform.position = v1;
                    break;
                
            }
            go.transform.parent = scripts.transform;
        }
    }
}
