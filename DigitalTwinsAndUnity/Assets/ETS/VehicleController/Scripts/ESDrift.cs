using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[AddComponentMenu("EasyVehicleSystem/ESDrift")]

public class ESDrift : MonoBehaviour
{
    [System.Serializable]
    public class sideswaysslip
    {
        public float asymptotevalue = 2.99f, asymptoteslip = 2.95f, extremumvalue = 3.83f, extremumslip = 5.75f, Stiffness = 1f;
    }
    [System.Serializable]
    public class fowradslip
    {
        public float asymptotevalue = 0.5f, asymptoteslip = 0.8f, extremumvalue = 1, extremumslip = 0.4f, Stiffness = 1f;
    }
    [System.Serializable]
    public class AdvanceDrift
    {
        public bool Isdrifting;
        [System.Serializable]
        public class sideswaysslip
        {
            public float asymptotevalue = 2.99f, asymptoteslip = 2.95f, extremumvalue = 3.83f, extremumslip = 5.75f, Stiffness = 1f;
        }
        [System.Serializable]
        public class fowradslip
        {
            public float asymptotevalue = 0.5f, asymptoteslip = 0.8f, extremumvalue = 1, extremumslip = 0.4f, Stiffness = 1f;
        }
        public sideswaysslip sidewaysslip;
        public fowradslip fowardslip;
    }
    //public 
    public bool m_dodrift;
    public bool m_donut;
    [Header("DriftCurve")]
    public sideswaysslip m_sideswaysslip;
    public fowradslip m_fowardslip;
    public WheelFrictionCurve fowardfriction, sidefriction, returnfowardfriction, returnsidewaysfriction;
    //private
    [SerializeField]
    private WheelFrictionCurve scurve, fcurve;
    private WheelCollider m_mywheel;

    void Awake()
    {
        m_mywheel = GetComponent<WheelCollider>();

    }
    // Use this for initialization
    void Start()
    {
        fowardfriction = m_mywheel.forwardFriction;
        sidefriction = m_mywheel.sidewaysFriction;
        returnfowardfriction = m_mywheel.forwardFriction;
        returnsidewaysfriction = m_mywheel.sidewaysFriction;
    }

    void Update()
    {
        drift(m_dodrift);
        PerformDonut();
    }

    public void PerformDonut()
    {
        if (m_donut)
        {
            scurve = sidefriction;
            scurve.extremumSlip = 1;
            scurve.asymptoteSlip = 1;
            scurve.asymptoteValue = 1;
            scurve.extremumValue = 1;
            scurve.stiffness = 1;
            m_mywheel.sidewaysFriction = scurve;
        }
        else
        {
            if (!m_dodrift)
                ReturnFriction();
        }
    }

    public void drift(bool DoDrift)
    {
        if (DoDrift)
        {
            scurve = sidefriction;
            scurve.extremumSlip = m_sideswaysslip.extremumslip;
            scurve.asymptoteSlip = m_sideswaysslip.asymptoteslip;
            scurve.asymptoteValue = m_sideswaysslip.asymptotevalue;
            scurve.extremumValue = m_sideswaysslip.extremumvalue;
            scurve.stiffness = m_sideswaysslip.Stiffness;
            m_mywheel.sidewaysFriction = scurve;
            //
            fcurve = fowardfriction;
            fcurve.extremumSlip = m_fowardslip.extremumslip;
            fcurve.asymptoteSlip = m_fowardslip.asymptoteslip;
            fcurve.asymptoteValue = m_fowardslip.asymptotevalue;
            fcurve.extremumValue = m_fowardslip.extremumvalue;
            fcurve.stiffness = m_fowardslip.Stiffness;
            m_mywheel.forwardFriction = fcurve;
        }
        else
        {
            if (!m_donut)
                ReturnFriction();
        }
    }

    private void ReturnFriction()
    {
        scurve.extremumSlip = returnsidewaysfriction.extremumSlip;
        scurve.asymptoteSlip = returnsidewaysfriction.asymptoteSlip;
        scurve.asymptoteValue = returnsidewaysfriction.asymptoteValue;
        scurve.extremumValue = returnsidewaysfriction.extremumValue;
        scurve.stiffness = returnsidewaysfriction.stiffness;
        m_mywheel.sidewaysFriction = scurve;
        //
        fcurve.extremumSlip = returnfowardfriction.extremumSlip;
        fcurve.asymptoteSlip = returnfowardfriction.asymptoteSlip;
        fcurve.asymptoteValue = returnfowardfriction.asymptoteValue;
        fcurve.extremumValue = returnfowardfriction.extremumValue;
        fcurve.stiffness = returnfowardfriction.stiffness;
        m_mywheel.forwardFriction = fcurve;
    }
}

