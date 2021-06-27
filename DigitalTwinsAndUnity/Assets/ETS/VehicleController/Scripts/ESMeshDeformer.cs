using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ESMeshDeformer : MonoBehaviour
{
    public MeshFilter[] meshes;
    public Collider[] colliders;
    public Rigidbody myrigidbody;
    [System.Serializable]
    public class DeformSettings
    {
        public float offsetforce = 0.09f;
        public float damageintensity = 0.2f;
        public float Maxdistance = 0.1f;
        [Header("Enable this to recalculate Normals and Bounds :}")]
        public bool Recalute = true;
    }

    public DeformSettings deformsettings;
    //priavte
    private int impactcount = 0;
    private float impactposition = 0.0f;
    private Vector3 pointtoverts = Vector3.zero;
    private Vector3 vertexvelocity = Vector3.zero;
    private Vector3[][] OriginalMeshes;

    void start()
    {
        myrigidbody = GetComponent<Rigidbody>();
    }

    void Awake()
    {
        OriginalMeshes = new Vector3[meshes.Length][];
        for (int i = 0; i < meshes.Length; i++)
        {
            Mesh mesh = meshes[i].mesh;
            OriginalMeshes[i] = mesh.vertices;
            mesh.MarkDynamic();
        }
    }

    void OnApplicationQuit()
    {
        for (int i = 0; i < meshes.Length; i++)
        {
            Mesh mesh = meshes[i].mesh;
            mesh.vertices = OriginalMeshes[i];
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
        }
    }

    void OnCollisionEnter(Collision mycol)
    {
        if (meshes.Length > 0 && colliders.Length > 0)
        {
            int impactpoint = 0;
            Vector3 impactpos = Vector3.zero;
            Vector3 impactvel = Vector3.zero;
            foreach (ContactPoint contacts in mycol.contacts)
            {
                float dragratio = Vector3.Dot(myrigidbody.GetPointVelocity(contacts.point), contacts.normal);
                if (dragratio < -0.6f || mycol.relativeVelocity.sqrMagnitude > 3.0f)
                {
                    impactpoint++;
                    impactpos += contacts.point;
                    impactvel += mycol.relativeVelocity;
                }
            }

            if (impactpoint > 0)
            {
                float inversecount = 1.0f / impactpoint;
                impactpos *= inversecount;
                impactvel *= inversecount;
                impactcount++;
                pointtoverts += transform.InverseTransformPoint(impactpos);
                vertexvelocity += transform.InverseTransformDirection(impactvel);
            }
        }
    }


    void FixedUpdate()
    {
        if (meshes.Length > 0 && colliders.Length > 0)
        {
            if (Time.time - impactposition >= 0.2f && impactcount > 0)
            {
                float inversecount = 1.0f / impactcount;
                pointtoverts *= inversecount;
                vertexvelocity *= inversecount;
                Vector3 impactvelocity = Vector3.zero;
                if (vertexvelocity.sqrMagnitude > 1.5f)
                {
                    impactvelocity = transform.TransformDirection(vertexvelocity) * 0.02f;
                }
                if (impactvelocity.sqrMagnitude > 0.0f)
                {
                    Vector3 contactpoint = transform.TransformPoint(pointtoverts);
                    for (int i = 0; i < meshes.Length; i++)
                    {
                        ApplyDeformingForce(meshes[i].mesh, OriginalMeshes[i], meshes[i].transform, contactpoint, impactvelocity);
                    }
                }
                impactcount = 0;
                pointtoverts = Vector3.zero;
                vertexvelocity = Vector3.zero;
                impactposition = Time.time + 0.2f * Random.Range(-0.4f, 0.4f);
            }
        }
    }

    float ApplyDeformingForce(Mesh mesh, Vector3[] OriginalMesh, Transform LocalTransform, Vector3 ContactPoint, Vector3 ContactVel)
    {
        Vector3[] vertices = mesh.vertices;
        Vector3 localcontactpoint = LocalTransform.InverseTransformPoint(ContactPoint);
        Vector3 localcontactforce = LocalTransform.InverseTransformDirection(ContactVel);
        float sqrmaxdist = deformsettings.Maxdistance * deformsettings.Maxdistance;
        float maxdamage = 0.0f;
        int damagedverts = 0;
        for (int i = 0; i < vertices.Length; i++)
        {
            UpdateVerts(i, vertices, localcontactpoint, localcontactforce, maxdamage, damagedverts, sqrmaxdist, OriginalMesh);
        }
        mesh.vertices = vertices;
        if (deformsettings.Recalute)
        {
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
        }
        return damagedverts > 0 ? maxdamage / damagedverts : 0.0f;
    }

    public void UpdateVerts(int i, Vector3[] vertices, Vector3 localcollisionpoint, Vector3 localcollisionforce, float overalldamage,
        int damagedverts, float sqrmaxdist, Vector3[] OriginalMesh)
    {

        float dist = (localcollisionpoint - vertices[i]).sqrMagnitude;
        if (dist < deformsettings.damageintensity)
        {
            Vector3 damage = (localcollisionforce * ((deformsettings.damageintensity * 2.0f) - Mathf.Sqrt(dist)) / (deformsettings.damageintensity * 2.0f)) * (deformsettings.offsetforce * (1 + myrigidbody.velocity.magnitude * 0.2f));
            vertices[i] += damage;
            Vector3 deform = vertices[i] - OriginalMesh[i];
            if (damage.sqrMagnitude > sqrmaxdist)
            {
                vertices[i] = OriginalMesh[i] + deform.normalized * deformsettings.Maxdistance;
            }
            overalldamage += damage.magnitude;
            damagedverts++;
        }
    }
}
