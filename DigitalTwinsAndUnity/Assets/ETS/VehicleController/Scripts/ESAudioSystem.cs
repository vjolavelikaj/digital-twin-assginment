using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[AddComponentMenu("EasyVehicleSystem/ESAudioSystem")]
[RequireComponent(typeof(AudioSource))]
public class ESAudioSystem : MonoBehaviour
{

    public AudioSource audiosource;
    [HideInInspector]
    public ESVehicleController vehiclecontroller;
    [HideInInspector]
    public ESGearShift gearshift;
    [Header("PitchSettings")]
    public float PitchModifier = 0.5f;
    [Tooltip("To Get Best sound make value same as MaxEngineRpm")]
    public float PitchMultiplier = 100f;
    [Header("VolumeSettings")]
    [Tooltip("To Get Best sound make value same as MaxEngineRpm")]
    public float VolumeMultiplier = 100f;
    public float StartVolume = 0.6f;
    [Header("AudioSettings")]
    [Tooltip("Reduce if sound shutter")][Range(0.0f,1.0f)]public float reverb = 0;
    public AudioClip enginesound;
    public AudioClip acceleratehigh;
    public AudioClip acceleratelow;
    public AudioClip deccelarate;
    public AudioClip idle;

    [Range(0.0f, 1.0f)]
    [Tooltip("Reduce if sound shutter")]public float spatialblend = 0.173f;
    public enum SoundType
    {
        simple,
        Advanced
    }
    //
    public SoundType _soundtype = SoundType.simple; 
    // Use this for initialization
    private void Start()
    {
        gearshift = GetComponent<ESGearShift>();
        vehiclecontroller = GetComponent<ESVehicleController>();
        audiosource = GetComponent<AudioSource>();
        audiosource.loop = true;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        AudioPlay();
        if (vehiclecontroller.usefuel)
        {
            if (vehiclecontroller.fuelmanager.Empty && audiosource.isPlaying)
            {
                audiosource.Stop();
            }
            else if (!vehiclecontroller.fuelmanager.Empty && !audiosource.isPlaying)
            {
                audiosource.Play();
            }
        }
        audiosource.pitch = Mathf.Abs(vehiclecontroller.Rpm) > 0 && Mathf.Abs(gearshift.forwardSlip) > (gearshift.sliplimit + 0.1f) && vehiclecontroller.CurrentSpeed < 0.3f && Mathf.Abs(Input.GetAxis("Vertical")) > 0 ?
            gearshift.forwardSlip : (gearshift.EngineRpm / PitchMultiplier) + PitchModifier;
        //audiosource.volume = (gearshift.EngineRpm / VolumeMultiplier) + StartVolume;

        //
        audiosource.spatialBlend = spatialblend;
        audiosource.reverbZoneMix = reverb;
      

    }
    //
    private void AudioPlay()
    {
        if (_soundtype == SoundType.Advanced)
        {
            if (vehiclecontroller.Neutral == false)
            {
                if (Mathf.Abs(vehiclecontroller.Accel) >= 0.15)
                {
                    audiosource.clip = acceleratehigh;
                }
                if (Mathf.Abs(vehiclecontroller.Accel) <= 0.15)
                {
                    if (vehiclecontroller.CurrentSpeed > 5f)
                        audiosource.clip = deccelarate;
                    else
                        audiosource.clip = acceleratelow;
                }
               
            }
            if (vehiclecontroller.Neutral == true)
            {
                if (gearshift.EngineRpm <= 0.5f && vehiclecontroller.Accel == 0)
                {
                    audiosource.clip = idle;
                }
                else
                {
                    audiosource.clip = acceleratehigh;
                }
            }  
        }
        else
        {
            audiosource.clip = enginesound;
            if (audiosource.isPlaying == false)
            {
                audiosource.Play();
            }
        }
        //

        if (audiosource.isPlaying == false)
        {
            audiosource.Play();
        }
    }
}
