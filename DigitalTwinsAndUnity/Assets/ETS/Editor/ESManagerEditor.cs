using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CanEditMultipleObjects]
[CustomEditor(typeof(ESGateWaySpawnSetup))]
public class ESManagerEditor : Editor
{
    public ESGateWaySpawnSetup scripts;

    public void OnSceneGUI()
    {
        scripts = target as ESGateWaySpawnSetup;
        Event e = Event.current;

        if (Event.current.type == EventType.KeyDown)
        {
            if (e.keyCode == KeyCode.M)
            {

                CallGenericMenu(e.mousePosition);
            }
            //
            if (e.keyCode == KeyCode.O)
            {

                CallGenericMenuDetach(e.mousePosition);
            }
        }


        if (Event.current.type == EventType.KeyUp)
        {
            scripts.done = false;
        }
    }

    private void CallGenericMenu(Vector2 mousepos)
    {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("MakeTarget"), false, () => Performadd());

        genericMenu.ShowAsContext();
    }
    //
    private void CallGenericMenuDetach(Vector2 mousepos)
    {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("RemoveTarget"), false, () => Performremove());

        genericMenu.ShowAsContext();
    }
    //
    private void Performadd()
    {
        //  Debug.Log("wow");
        scripts.add = true;
    }
    //
    //
    private void Performremove()
    {
        // Debug.Log("wow");
        scripts.remove = true;
    }
    //
}
