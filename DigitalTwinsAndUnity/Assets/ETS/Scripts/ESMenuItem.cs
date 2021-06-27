using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

public class ESMenuItem : MonoBehaviour
{
    [MenuItem("GameObject/Easy Traffic System/Vol2/AddPathParent", false, 10)]
    static void CreatePathParentGameObject(MenuCommand menuCommand)
    {
        //creates a parent
        GameObject goparent = new GameObject("NodeParent");
        // Create a custom game object
        GameObject go = new GameObject("Path");
        go.AddComponent<ESNodeSystem>();
 
      
        // Ensure it gets reparented if this was a context click (otherwise does nothing)
        GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
        GameObjectUtility.SetParentAndAlign(goparent, menuCommand.context as GameObject);
        // Register the creation in the undo system
        Undo.RegisterCreatedObjectUndo(goparent, "Create " + goparent.name);
        Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
        Selection.activeObject = go;
        go.transform.parent = goparent.transform;

    }
    //
   
    [MenuItem("GameObject/Easy Traffic System/Vol2/TrafficManager", false, 10)]
    static void CreateManager(MenuCommand menuCommand)
    {
        //creates a parent
        GameObject go = new GameObject("Manager");
        // Create a custom game object
       
        go.AddComponent<TrafficManager>();


        // Ensure it gets reparented if this was a context click (otherwise does nothing)
        GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
        Undo.RegisterCreatedObjectUndo(go, "Create manager" + go.name);
        Selection.activeObject = go;
     

    }
    //
    [MenuItem("GameObject/Easy Traffic System/Vol2/TrafficControlPoint", false, 10)]
    static void CreateCP(MenuCommand menuCommand)
    {
        //creates a parent
        GameObject go = new GameObject("ControlPoint");
        // Create a custom game object

        go.AddComponent<ESTrafficControlPoint>();


        // Ensure it gets reparented if this was a context click (otherwise does nothing)
        GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
        Undo.RegisterCreatedObjectUndo(go, "Create CP_i" + go.name);
        Selection.activeObject = go;


    }
    //
   
    //
    /*
    [MenuItem("GameObject/Easy Traffic System/AddTrafficLightManager", false, 10)]
    static void CreateTrafficLightManagerGameObject(MenuCommand menuCommand)
    {
        // Create a custom game object
        GameObject go = new GameObject("TrafficLightManager");
        go.AddComponent<ES_TrafficLight_Manager>();

        // Ensure it gets reparented if this was a context click (otherwise does nothing)
        GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
        // Register the creation in the undo system
        Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
        Selection.activeObject = go;
    }
    */
}
#endif
