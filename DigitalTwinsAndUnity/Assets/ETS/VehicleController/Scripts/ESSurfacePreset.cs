using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[AddComponentMenu("EasyVehicleSystem/ESurfacePreset")]
public class ESSurfacePreset : MonoBehaviour
{
    [Serializable]
    public class AddSurface
    {
        public string SurfaceName = "NewSurfacePreset";

        public bool usewheelfrictincurve;

        public GameObject ActiveTerrain;
        public String TerrainName;
        public bool IsTerrain = true;
        public ParticleSystem ParticlePrefab;
        public ParticleSystem Particles;
        public bool EmitParticle;
        public int TerrainTextureIndex;
        public string TagName;
        public AudioClip clip;

        [Serializable]
        public class ForwardSlip
        {
            public float asymptoteslip, asymptotevalue, extremumslip, extremumvalue, Stiffness;
            public WheelCollider[] wheelcollider;
            public GameObject[] WheelObject;
            public string WheelColliderTagName;
        }
        [Serializable]
        public class SideSlip
        {
            public float asymptoteslip, asymptotevalue, extremumslip, extremumvalue, Stiffness;
        }
        public ForwardSlip forwardslip;
        public SideSlip sideslip;
    }
    [Serializable]
    public class GrougSurfaces
    {
        public int currentsurface;
    }
    public int surfaceindex = 1;
    public List<AddSurface> addsurface = new List<AddSurface>(1);
    public bool[] m_surafaceFoldout;
    public bool[] wheelcolliderpreset;

    public void SetDefualts(int i)
    {
        addsurface[i].SurfaceName = "NewSurfacePreset" + i.ToString();
    }
}
