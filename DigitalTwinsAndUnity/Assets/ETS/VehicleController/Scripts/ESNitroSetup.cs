using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ESNitroSetup : MonoBehaviour
{
    public float NitroValue;
    public float RotationSpeed;
    public AudioClip PickUpSound;

    public void FixedUpdate()
    {
        Vector3 RotVec = new Vector3(1, 1, 1) * RotationSpeed;
        transform.Rotate(RotVec);
    }
}
