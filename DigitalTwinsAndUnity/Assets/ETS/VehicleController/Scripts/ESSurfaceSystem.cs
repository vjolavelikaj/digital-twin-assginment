using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[AddComponentMenu("EasyVehicleSystem/ESurfaceSystem")]

public class ESSurfaceSystem : MonoBehaviour
{
    [HideInInspector]
    public GameObject Surface;
    public ESSurfacePreset surfacepreset;
    private WheelCollider wheelcollider;
    private WheelHit hit;
    //[SerializeField ]
    private bool NotOnTerrain = false;
    //[SerializeField]
    private int currentterrainindex;
    //[SerializeField]
    private int cursurfindex;
    private ESEffect _effect;
    //[SerializeField]
    private GameObject Source;
    //[SerializeField]
    private Terrain ActiveTerrain;
    //private ESDrift drift;
    private void Awake()
    {
        Surface = Resources.Load("Surfaces/SurfacePreset") as GameObject;
        wheelcollider = GetComponent<WheelCollider>();
        //drift = GetComponent<ESDrift>();
        surfacepreset = Surface.GetComponent<ESSurfacePreset>();
        _effect = GetComponent<ESEffect>();
        //create a gameobject

        GameObject go = new GameObject("SurfaceSoundSource");
        go.AddComponent<AudioSource>();
        go.GetComponent<AudioSource>().playOnAwake = false;
        go.GetComponent<AudioSource>().loop = true;
        go.GetComponent<AudioSource>().spatialBlend = 0.9f;
        go.transform.position = transform.position - transform.up * wheelcollider.radius;
        go.transform.parent = this.transform;

        Source = go;
    }

    private void Update()
    {
        CheckCurrentGroundIndex();
        ApplyEffectToSurface();
    }
    private void OnDisable()
    {
        for (int i = 0; i < surfacepreset.addsurface.Count; ++i)
        {
            // surfacepreset.addsurface[i].Particleinstance = false;
        }

    }

    private void CheckCurrentGroundIndex()
    {
        wheelcollider.GetGroundHit(out hit);
        //
        if (hit.collider != null)
        {
            if (hit.collider.GetComponent<Terrain>() != null)
            {
                ActiveTerrain = hit.collider.gameObject.GetComponent<Terrain>();
            }
        }
        if (ActiveTerrain != null)
        {
            currentterrainindex = GetMainTexture(hit.point, ActiveTerrain);
        }
        //
        if (hit.collider != null)
        {
            if (hit.collider.gameObject.GetComponent<Terrain>() != null)
            {
                NotOnTerrain = false;
            }
            else
            {
                NotOnTerrain = true;
            }
        }

        if (wheelcollider.isGrounded)
        {
            for (int i = 0; i < surfacepreset.addsurface.Count; ++i)
            {
                if (NotOnTerrain == true)
                {
                    if (surfacepreset.addsurface[i].TagName == hit.collider.tag && surfacepreset.addsurface[i].forwardslip.WheelColliderTagName == this.tag)
                    {
                        // surfacepreset.addsurface[i].ActiveTerrain = GameObject.Find(surfacepreset.addsurface[i].TerrainName);
                        //if(wheelcollider == )
                        cursurfindex = i;

                    }
                }
                if (NotOnTerrain == false)
                {

                    if (surfacepreset.addsurface[i].IsTerrain)
                    {
                        if (surfacepreset.addsurface[i].TerrainTextureIndex == currentterrainindex && surfacepreset.addsurface[i].forwardslip.WheelColliderTagName == this.tag)
                        {
                            cursurfindex = i;
                        }
                    }
                }
                //end loop
            }


            if (surfacepreset.addsurface[cursurfindex].IsTerrain == false)
            {
                ApplyWheelFriction(surfacepreset, cursurfindex);
            }
            if (surfacepreset.addsurface[cursurfindex].IsTerrain == true)
            {

                if (surfacepreset.addsurface[cursurfindex].TerrainTextureIndex == currentterrainindex)
                {
                    ApplyWheelFriction(surfacepreset, cursurfindex);
                }
            }
        }

    }
    //
    private void ApplyEffectToSurface()
    {
        if (wheelcollider.isGrounded)
        {
            if (surfacepreset.addsurface[cursurfindex].EmitParticle == false)
            {
                for (int i = 0; i < surfacepreset.addsurface.Count; ++i)
                {
                    if (surfacepreset.addsurface[i].Particles != null)
                    {
                        surfacepreset.addsurface[i].Particles.Stop();
                    }
                    //this.GetComponent<AudioSource>().clip = surfacepreset.addsurface[cursurfindex].clip;
                    if (Source.GetComponent<AudioSource>().isPlaying)
                    {
                        Source.GetComponent<AudioSource>().Stop();
                    }
                }
            }
            if (wheelcollider.attachedRigidbody.velocity.magnitude > 0.2f)
            {
                //play particles
                if (surfacepreset.addsurface[cursurfindex].EmitParticle)
                {
                    if (surfacepreset.addsurface[cursurfindex].Particles == null)
                    {
                        surfacepreset.addsurface[cursurfindex].Particles = Instantiate(surfacepreset.addsurface[cursurfindex].ParticlePrefab) as ParticleSystem;
                        surfacepreset.addsurface[cursurfindex].Particles.gameObject.tag = "Particles";

                    }
                    surfacepreset.addsurface[cursurfindex].Particles.transform.position = transform.position - transform.up * wheelcollider.radius;
                    surfacepreset.addsurface[cursurfindex].Particles.Emit(1);
                    Source.GetComponent<AudioSource>().clip = surfacepreset.addsurface[cursurfindex].clip;
                    if (!Source.GetComponent<AudioSource>().isPlaying)
                    {
                        Source.GetComponent<AudioSource>().Play();
                    }
                }

            }
            else
            {
                //stopparticles;
                //surfacepreset.addsurface[cursurfindex].Particles.transform.position = transform.position - transform.up * wheelcollider.radius;
                for (int i = 0; i < surfacepreset.addsurface.Count; ++i)
                {
                    if (surfacepreset.addsurface[i].Particles != null)
                    {
                        surfacepreset.addsurface[i].Particles.Stop();
                    }
                    //this.GetComponent<AudioSource>().clip = surfacepreset.addsurface[cursurfindex].clip;
                    if (Source.GetComponent<AudioSource>().isPlaying)
                    {
                        Source.GetComponent<AudioSource>().Stop();
                    }
                }
            }
        }
        else if (!wheelcollider.isGrounded)
        {
            for (int i = 0; i < surfacepreset.addsurface.Count; ++i)
            {
                if (surfacepreset.addsurface[i].Particles != null)
                {
                    surfacepreset.addsurface[i].Particles.Stop();
                }
                //this.GetComponent<AudioSource>().clip = surfacepreset.addsurface[cursurfindex].clip;
                if (Source.GetComponent<AudioSource>().isPlaying)
                {
                    Source.GetComponent<AudioSource>().Stop();
                }
            }
        }
    }
    //
    private void ApplyWheelFriction(ESSurfacePreset surf, int index)
    {
        if (!surf.addsurface[index].usewheelfrictincurve) return;
        //get all gameobjects with specified tagname
        surf.addsurface[index].forwardslip.WheelObject = GameObject.FindGameObjectsWithTag(surf.addsurface[index].forwardslip.WheelColliderTagName);
        //resize wheelcollider array
        System.Array.Resize(ref  surf.addsurface[index].forwardslip.wheelcollider, surf.addsurface[index].forwardslip.WheelObject.Length);


        for (int j = 0; j < surf.addsurface[index].forwardslip.wheelcollider.Length; ++j)
        {
            surf.addsurface[index].forwardslip.wheelcollider[j] = surf.addsurface[index].forwardslip.WheelObject[j].GetComponent<WheelCollider>();
            if (wheelcollider.gameObject == surf.addsurface[index].forwardslip.wheelcollider[j].gameObject)
            {

                //foward
                surf.addsurface[index].forwardslip.wheelcollider[j].GetComponent<ESDrift>().returnfowardfriction.asymptoteSlip = surf.addsurface[index].forwardslip.asymptoteslip;
                surf.addsurface[index].forwardslip.wheelcollider[j].GetComponent<ESDrift>().returnfowardfriction.asymptoteValue = surf.addsurface[index].forwardslip.asymptotevalue;
                surf.addsurface[index].forwardslip.wheelcollider[j].GetComponent<ESDrift>().returnfowardfriction.extremumSlip = surf.addsurface[index].forwardslip.extremumslip;
                surf.addsurface[index].forwardslip.wheelcollider[j].GetComponent<ESDrift>().returnfowardfriction.extremumValue = surf.addsurface[index].forwardslip.extremumvalue;
                surf.addsurface[index].forwardslip.wheelcollider[j].GetComponent<ESDrift>().returnfowardfriction.stiffness = surf.addsurface[index].forwardslip.Stiffness;
                //side                                         
                surf.addsurface[index].forwardslip.wheelcollider[j].GetComponent<ESDrift>().returnsidewaysfriction.asymptoteSlip = surf.addsurface[index].sideslip.asymptoteslip;
                surf.addsurface[index].forwardslip.wheelcollider[j].GetComponent<ESDrift>().returnsidewaysfriction.asymptoteValue = surf.addsurface[index].sideslip.asymptotevalue;
                surf.addsurface[index].forwardslip.wheelcollider[j].GetComponent<ESDrift>().returnsidewaysfriction.extremumSlip = surf.addsurface[index].sideslip.extremumslip;
                surf.addsurface[index].forwardslip.wheelcollider[j].GetComponent<ESDrift>().returnsidewaysfriction.extremumValue = surf.addsurface[index].sideslip.extremumvalue;
                surf.addsurface[index].forwardslip.wheelcollider[j].GetComponent<ESDrift>().returnsidewaysfriction.stiffness = surf.addsurface[index].sideslip.Stiffness;
            }
        }
    }

    //
    // terrain surface
    public float[] GetTextureMix(Vector3 worldPos, Terrain terrain)
    {
        terrain = Terrain.activeTerrain;
        TerrainData terrainData = terrain.terrainData;
        Vector3 terrainPos = terrain.transform.position;
        int mapX = (int)(((worldPos.x - terrainPos.x) / terrainData.size.x) * terrainData.alphamapWidth);
        int mapZ = (int)(((worldPos.z - terrainPos.z) / terrainData.size.z) * terrainData.alphamapHeight);
        float[, ,] splatmapData = terrainData.GetAlphamaps(mapX, mapZ, 1, 1);
        float[] cellMix = new float[splatmapData.GetUpperBound(2) + 1];
        for (int n = 0; n < cellMix.Length; ++n)
        {
            cellMix[n] = splatmapData[0, 0, n];
        }
        return cellMix;
    }
    public int GetMainTexture(Vector3 worldPos, Terrain terrain)
    {
        float[] mix = GetTextureMix(worldPos, terrain);
        float maxMix = 0;
        int maxIndex = 0;
        for (int n = 0; n < mix.Length; ++n)
        {
            if (mix[n] > maxMix)
            {
                maxIndex = n;
                maxMix = mix[n];
            }
        }
        return maxIndex;
    }

}
