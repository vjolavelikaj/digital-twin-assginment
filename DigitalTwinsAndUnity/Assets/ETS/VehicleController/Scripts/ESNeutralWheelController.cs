using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[AddComponentMenu("EasyVehicleSystem/ESNeutralWheelController")]

public class ESNeutralWheelController : MonoBehaviour
{
    public WheelCollider[] wheelcollider = new WheelCollider[2];
    public Transform[] wheelmeshes = new Transform[2];
    [Tooltip("set value above 0 to applydrag")]
    public float MaxAngularDrag = 100f;
    public bool Isgrounded = false;
    [Tooltip("set value above 0 to applydrag")]
    public float MaxDrag = 500f;
    private float ReturnDrag;
    private float RetuenAngulardrag;
    private Rigidbody m_ridigbody;
    public ESVehicleController evs;
    private void Start()
    {
        m_ridigbody = GetComponent<Rigidbody>();
        ReturnDrag = m_ridigbody.drag;
        RetuenAngulardrag = m_ridigbody.angularDrag;
    }

    private void FixedUpdate()
    {
        WheelAlignment();
        ApplyDrag();
    }

    private void WheelAlignment()
    {
        // align front wheel meshes
        Vector3 wheelposition;
        Quaternion wheelrotation;

        for (int i = 0; i < wheelcollider.Length; i++)
        {
            if (wheelmeshes[i] == null)
            {
                return;
            }
            wheelcollider[i].GetWorldPose(out wheelposition, out wheelrotation);
            wheelmeshes[i].transform.position = wheelposition;
            wheelmeshes[i].transform.rotation = wheelrotation;
        }
    }

    private void ApplyDrag()
    {
        //WheelHit hit;

        for (int i = 0; i < wheelcollider.Length; i++)
        {
           // Isgrounded = wheelcollider[i].GetGroundHit(out hit);
        }

        if (MaxDrag > 0)
        {
            if (evs.CurrentSpeed < 0.5f)
            {
                m_ridigbody.drag = MaxDrag;
            }
            else
            {
                m_ridigbody.drag = ReturnDrag;
            }
        }
        if (MaxAngularDrag > 0)
        {
            if (evs.CurrentSpeed < 0.5f)
            {
                m_ridigbody.angularDrag = MaxAngularDrag;
            }
            else
            {
                m_ridigbody.angularDrag = RetuenAngulardrag;
            }
        }
    }
}
