using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ESLightSystem : MonoBehaviour
{

    public GameObject BrakeLightObject;
    public GameObject HeadLighthighObject;
    public GameObject HeadLightLowObject;
    public GameObject ReverseLight;
    [System.Serializable]
    public class BrakeLightMaterial
    {
        [Tooltip("what meshrenderer to this materials onto")]
        public MeshRenderer BrakeLightMeshRenderer;
        [Header("Emits light")]
        public Material BrakeLightEmission;
        [Header("don't Emit light")]
        public Material BrakeLightAbsorption;
    }

    [System.Serializable]
    public class ReverseLightMaterial
    {
        [Tooltip("what meshrenderer to this materials onto")]
        public MeshRenderer ReverseLightMeshRenderer;
        [Header("Emits light")]
        public Material ReverseLightEmission;
        [Header("don't Emit light")]
        public Material ReverseLightAbsorption;
    }

    [System.Serializable]
    public class KeyBoardInput
    {
        [Header("this has no effect on AI :)")]
        public KeyCode headLampKey = KeyCode.L;
        public KeyCode ReverseLightKey = KeyCode.R;
        public KeyCode High_low_BeamKey = KeyCode.H;
    }
    public KeyBoardInput keyboardinput;
    public BrakeLightMaterial brakelightmaterial;
    public ReverseLightMaterial reverselightmaterial;
    //private 
    private bool ToggleHeadLamp = false;
    private bool ToggleReverseLight = false;
    private bool ToggleHighBeam = false;
    private ESGearShift gearshift;


    private void Start()
    {
        gearshift = this.GetComponent<ESGearShift>();
    }

    // controls vehicle controller
    public void LightSystem(float handbrake, float vertical, float shoebrake)
    {
        if (handbrake > 0)
        {
            BrakeLightObject.SetActive(true);
            brakelightmaterial.BrakeLightMeshRenderer.material = brakelightmaterial.BrakeLightEmission;
        }
        else
        {
            if (shoebrake > 0 && !ToggleReverseLight)
            {
                BrakeLightObject.SetActive(true);
                brakelightmaterial.BrakeLightMeshRenderer.material = brakelightmaterial.BrakeLightEmission;
            }
            else
            {
                BrakeLightObject.SetActive(false);
                brakelightmaterial.BrakeLightMeshRenderer.material = brakelightmaterial.BrakeLightAbsorption;
            }
        }
        //headlamp   
        if (ToggleHeadLamp)
        {
            if (ToggleHighBeam)
            {
                HeadLighthighObject.SetActive(true);
                HeadLightLowObject.SetActive(false);
            }
            if (!ToggleHighBeam)
            {
                HeadLightLowObject.SetActive(true);
                HeadLighthighObject.SetActive(false);
            }
        }
        if (!ToggleHeadLamp)
        {
            HeadLighthighObject.SetActive(false);
            HeadLightLowObject.SetActive(false);
        }


        //reverselight;
        if (ToggleReverseLight || vertical < 0)
        {
            ReverseLight.SetActive(true);
            reverselightmaterial.ReverseLightMeshRenderer.material = reverselightmaterial.ReverseLightEmission;
        }
        if (vertical > 0 && ToggleReverseLight)
        {
            ReverseLight.SetActive(false);
            ToggleReverseLight = false;
            reverselightmaterial.ReverseLightMeshRenderer.material = reverselightmaterial.ReverseLightAbsorption;
        }
        if (!ToggleReverseLight)
        {
            ReverseLight.SetActive(false);
            reverselightmaterial.ReverseLightMeshRenderer.material = reverselightmaterial.ReverseLightAbsorption;
        }

    }
    //
    public void KeyInput()
    {
        if (Input.GetKeyDown(keyboardinput.headLampKey))
        {
            ToggleHeadLamp = !ToggleHeadLamp;
        }
        if (gearshift.AutoReverse)
        {
            if (gearshift.isreverse)
            {
                if (gearshift.GetComponent<Rigidbody>().velocity.magnitude < 5f && Input.GetAxis("Vertical") < 0)
                {
                    ToggleReverseLight = true;
                }
            }
            else
            {
                if (gearshift.GetComponent<Rigidbody>().velocity.magnitude > 2f)
                {
                    ToggleReverseLight = false;
                }
            }
        }
        else
        {
            if (Input.GetKeyDown(keyboardinput.ReverseLightKey))
            {
                ToggleReverseLight = !ToggleReverseLight;
            }
        }

        if (Input.GetKeyDown(keyboardinput.High_low_BeamKey))
        {
            ToggleHighBeam = !ToggleHighBeam;
        }
    }
    // controls ai lightsystem
    public void LightSystemAI(bool UseHeadLamp, bool isbraking, float accel, bool isreverse, bool High)
    {
        if (High)
        {
            HeadLighthighObject.SetActive(UseHeadLamp);
            HeadLightLowObject.SetActive(false);
        }
        if (!High)
        {
            HeadLighthighObject.SetActive(false);
            HeadLightLowObject.SetActive(UseHeadLamp);
        }
        if (isbraking)
        {
            BrakeLightObject.SetActive(true);
            brakelightmaterial.BrakeLightMeshRenderer.material = brakelightmaterial.BrakeLightEmission;
        }
        else
        {
            BrakeLightObject.SetActive(false);
            brakelightmaterial.BrakeLightMeshRenderer.material = brakelightmaterial.BrakeLightAbsorption;
        }

        if (isreverse || accel < 0)
        {
            ReverseLight.SetActive(true);
            reverselightmaterial.ReverseLightMeshRenderer.material = reverselightmaterial.ReverseLightEmission;
        }
        if (accel > 0 && isreverse)
        {
            ReverseLight.SetActive(false);
            reverselightmaterial.ReverseLightMeshRenderer.material = reverselightmaterial.ReverseLightAbsorption;
        }
        if (!isreverse)
        {
            ReverseLight.SetActive(false);
            reverselightmaterial.ReverseLightMeshRenderer.material = reverselightmaterial.ReverseLightAbsorption;
        }


    }

}
