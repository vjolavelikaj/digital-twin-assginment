using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CanEditMultipleObjects]
[CustomEditor(typeof(ESNodeManager))]
public class ESSceneManagerEditor : Editor
{
    public ESNodeManager scripts;
    
    // Update is called once per frame
    public  void OnSceneGUI()
    {
        scripts = target as ESNodeManager;
        Event e = Event.current;

        if (Event.current.type == EventType.KeyDown)
        {
            if (e.keyCode == KeyCode.J)
            {

                CallGenericMenu(e.mousePosition);
            }
            //
            if (e.keyCode == KeyCode.L)
            {
             
                CallGenericMenuDetach(e.mousePosition);
            }
        }


        if (Event.current.type == EventType.KeyUp)
        {
           scripts.scenemanager.GetComponent<ESScenemanager>().done = false;
        }
        //


    }
    //
    private void CallGenericMenu(Vector2 mousepos)
    {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Join"), false, () => PerformConnect());

        genericMenu.ShowAsContext();
    }
    //
    private void CallGenericMenuDetach(Vector2 mousepos)
    {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Detach"), false, () => PerformDetach());

        genericMenu.ShowAsContext();
    }
    //
    private void PerformConnect()
    {
       // Debug.Log("wow");
        scripts.scenemanager.GetComponent<ESScenemanager>().connect = true;
    }
    //
    //
    private void PerformDetach()
    {
       // Debug.Log("wow");
        scripts.scenemanager.GetComponent<ESScenemanager>().disconnect = true;
    }
    //
}
