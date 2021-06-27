using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[AddComponentMenu("EasyVehicleSystem/ESCameraManager")]

public class ESCameraManager : MonoBehaviour
{

    [System.Serializable]

    public class KeyboardInput
    {
        // keyboard inputs,this can changed in the inspector
        [SerializeField]
        public KeyCode camera1 = KeyCode.Alpha1;
        [SerializeField]
        public KeyCode camera2 = KeyCode.Alpha2;
        [SerializeField]
        public KeyCode camera3 = KeyCode.Alpha3;
        [SerializeField]
        public KeyCode camera4 = KeyCode.Alpha4;
        [SerializeField]
        public KeyCode carmera5 = KeyCode.Alpha5;
    }
    public enum defaultcamera
    {
        camera1,
        camera2,
        camera3,
        camera4,
        camera5
    }
    public defaultcamera defaultcarmera = defaultcamera.camera1;
    public KeyboardInput keyboardinput;
    public GameObject camera1;
    public GameObject camera2;
    public GameObject camera3;
    public GameObject camera4;
    public GameObject camera5;
    private void Start()
    {
        switch (defaultcarmera)
        {
            case defaultcamera.camera1:
                {
                    camerasuffle(camera1, true);
                    camerasuffle(camera2, false);
                    camerasuffle(camera3, false);
                    camerasuffle(camera4, false);
                    camerasuffle(camera5, false);
                }
                break;
            case defaultcamera.camera2:
                {
                    camerasuffle(camera1, false);
                    camerasuffle(camera2, true);
                    camerasuffle(camera3, false);
                    camerasuffle(camera4, false);
                    camerasuffle(camera5, false);
                }
                break;
            case defaultcamera.camera3:
                {
                    camerasuffle(camera1, false);
                    camerasuffle(camera2, false);
                    camerasuffle(camera3, true);
                    camerasuffle(camera4, false);
                    camerasuffle(camera5, false);
                }
                break;
            case defaultcamera.camera4:
                {
                    camerasuffle(camera1, false);
                    camerasuffle(camera2, false);
                    camerasuffle(camera3, false);
                    camerasuffle(camera4, true);
                    camerasuffle(camera5, false);
                }
                break;
            case defaultcamera.camera5:
                {
                    camerasuffle(camera1, false);
                    camerasuffle(camera2, false);
                    camerasuffle(camera3, false);
                    camerasuffle(camera4, false);
                    camerasuffle(camera5, true);
                }
                break;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(keyboardinput.camera1))
        {
            camerasuffle(camera1, true);
            camerasuffle(camera2, false);
            camerasuffle(camera3, false);
            camerasuffle(camera4, false);
            camerasuffle(camera5, false);
        }
        if (Input.GetKeyDown(keyboardinput.camera2))
        {
            camerasuffle(camera1, false);
            camerasuffle(camera2, true);
            camerasuffle(camera3, false);
            camerasuffle(camera4, false);
            camerasuffle(camera5, false);
        }
        if (Input.GetKeyDown(keyboardinput.camera3))
        {
            camerasuffle(camera1, false);
            camerasuffle(camera2, false);
            camerasuffle(camera3, true);
            camerasuffle(camera4, false);
            camerasuffle(camera5, false);
        }
        if (Input.GetKeyDown(keyboardinput.camera4))
        {
            camerasuffle(camera1, false);
            camerasuffle(camera2, false);
            camerasuffle(camera3, false);
            camerasuffle(camera4, true);
            camerasuffle(camera5, false);
        }
        if (Input.GetKeyDown(keyboardinput.carmera5))
        {
            camerasuffle(camera1, false);
            camerasuffle(camera2, false);
            camerasuffle(camera3, false);
            camerasuffle(camera4, false);
            camerasuffle(camera5, true);
        }
    }
    private void camerasuffle(GameObject myobject, bool mybool)
    {
        myobject.GetComponent<Camera>().enabled = mybool;
        myobject.GetComponent<AudioListener>().enabled = mybool;
        myobject.GetComponent<FlareLayer>().enabled = mybool;
    }


}
