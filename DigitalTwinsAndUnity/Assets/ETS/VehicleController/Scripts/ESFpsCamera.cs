using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[AddComponentMenu("EasyVehicleSystem/ESFpsCarmera")]

public class ESFpsCamera : MonoBehaviour
{
    //public
    public GameObject Target;
    public float SmoothSpeed = 0.125f;
    public float mouseXclamp = 60f;
    public bool findbyname;
    public string Gameobjname;
    //private
    private float mouseX;
    private float mouseY;
    private float Xorientationvelocity;
    private float Yorientationvelocity;
    private float vehiclerotation;

    void Update()
    {

        if (Target == null && findbyname)
        {
            Target = GameObject.Find(Gameobjname);
        }
        if (Target != null)
        {
            vehiclerotation = Target.transform.rotation.y;
            mouseX += Input.GetAxis("Mouse X");
            mouseY -= Input.GetAxis("Mouse Y");

            mouseX = Mathf.Clamp(mouseX, -mouseXclamp, mouseXclamp);

            mouseY = Mathf.SmoothDamp(mouseY, vehiclerotation, ref Yorientationvelocity, Time.deltaTime * SmoothSpeed);
            transform.localRotation = Quaternion.Euler(0f, mouseX, mouseY);
        }

    }
}
