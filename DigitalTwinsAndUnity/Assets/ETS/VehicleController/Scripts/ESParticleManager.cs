using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ESParticleManager : MonoBehaviour
{
    private GameObject[] Particles;
    //
    private void Update()
    {
        ManagerParticles();
    }
    //
    public void ManagerParticles()
    {
        Particles = GameObject.FindGameObjectsWithTag("Particles");
        for (int i = 0; i < Particles.Length; i++)
        {
            Particles[i].transform.parent = this.transform;
        }
    }
}