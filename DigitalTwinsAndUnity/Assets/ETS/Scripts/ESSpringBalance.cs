using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[AddComponentMenu("EasyVehicleSystem/ESSpringBalance")]

public class ESSpringBalance : MonoBehaviour {

    public WheelCollider m_mywheelleft, m_mywheelright;
    public Rigidbody CarRigidBody;
    public float AntiRollFactor;
    // Use this for initialization
    void Start()
    {
        CarRigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    public void FixedUpdate()
    {
        WheelHit Hit;
        var WheelCompressionleft = 1.0f;
        var WheelCompressionright = 1.0f;

        var CheckGroundStateleft = m_mywheelleft.GetGroundHit(out Hit);
        if (CheckGroundStateleft)
        {
            WheelCompressionleft = (-m_mywheelleft.transform.InverseTransformPoint(Hit.point).y - m_mywheelleft.radius)
                / m_mywheelleft.suspensionDistance;
        }

        var CheckGroundStateright = m_mywheelright.GetGroundHit(out Hit);
        if (CheckGroundStateright)
        {
            WheelCompressionright = (-m_mywheelright.transform.InverseTransformPoint(Hit.point).y - m_mywheelright.radius)
                / m_mywheelright.suspensionDistance;
        }


        if (Hit.normal == Vector3.zero)
            return;
        //Determine the antirollforce
        var antirollforce0_1 = (WheelCompressionleft - WheelCompressionright) * AntiRollFactor;
        if (CheckGroundStateleft)
            CarRigidBody.AddForceAtPosition(m_mywheelleft.transform.up * -antirollforce0_1, m_mywheelleft.transform.position);
        if (CheckGroundStateright)
            CarRigidBody.AddForceAtPosition(m_mywheelright.transform.up * antirollforce0_1, m_mywheelright.transform.position);
    }



}