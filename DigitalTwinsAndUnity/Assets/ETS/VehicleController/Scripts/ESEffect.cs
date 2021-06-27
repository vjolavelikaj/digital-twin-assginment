using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[AddComponentMenu("EasyVehicleSystem/ESEffects")]

[RequireComponent(typeof(AudioSource))]
public class ESEffect : MonoBehaviour
{
    [System.Serializable]
    public class GenerateSkid
    {
        // Class that generates the SkidMark:}
        public float width = 0.13f;
        public float groundoffset = 0.01f;
        public float destroyskidmarkstime = 5f;
        public bool isskidding;
        public Material material;
        public float Resolution = 0.25f;
        [HideInInspector]
        public Vector3 lastwheelpos;
        [HideInInspector]
        public Transform skidtrashholder;
        private Vector3[] lastposl_r = new Vector3[2];
        private Vector3 currentpositon, lastposition;

        public void Addskidmarks(WheelCollider collider, Transform Curtransform)
        {
            WheelHit hit;
            collider.GetGroundHit(out hit);
            GameObject skidgameobject = new GameObject("SkidInstance");
            skidgameobject.transform.parent = skidtrashholder;
            skidgameobject.AddComponent<MeshFilter>();
            skidgameobject.AddComponent<MeshRenderer>();
            MeshFilter mf = skidgameobject.GetComponent<MeshFilter>();
            MeshRenderer mr = skidgameobject.GetComponent<MeshRenderer>();

            Mesh mesh = new Mesh();

            Vector3[] vertices = new Vector3[4];
            // Vector3[] normals = new Vector3[4];
            Vector2[] uvs = new Vector2[4];
            int[] traingles = new int[6] 
            {
                0,
                1,
                2,
                2, 
                3,
                0 
            };

            //
            if (!isskidding)
            {
                currentpositon = hit.point + hit.normal * groundoffset;
                Vector3 direction = (currentpositon - lastposition);
                Vector3 Xdirection = Vector3.Cross(direction, hit.normal).normalized;
                Vector3 positiontoleft = currentpositon + Xdirection * width;
                Vector3 positiontoright = currentpositon - Xdirection * width;

                vertices[0] = positiontoleft;
                vertices[1] = positiontoright;
                vertices[2] = positiontoleft;
                vertices[3] = positiontoright;

                lastposition = currentpositon;
                lastposl_r[0] = positiontoleft;
                lastposl_r[1] = positiontoright;
                isskidding = true;
            }
            if (isskidding)
            {
                currentpositon = hit.point + hit.normal * groundoffset;
                Vector3 direction = (currentpositon - lastposition);
                Vector3 Xdirection = Vector3.Cross(direction, hit.normal).normalized;
                Vector3 positiontoleft = currentpositon + Xdirection * width;
                Vector3 positiontoright = currentpositon - Xdirection * width;

                lastwheelpos = currentpositon;
                vertices[0] = lastposl_r[1];
                vertices[1] = lastposl_r[0];
                vertices[2] = positiontoleft;
                vertices[3] = positiontoright;

                lastposition = currentpositon;
                lastposl_r[0] = positiontoleft;
                lastposl_r[1] = positiontoright;
            }
            uvs[0] = new Vector3(1, 0);
            uvs[1] = new Vector3(0, 0);
            uvs[2] = new Vector3(0, 1);
            uvs[3] = new Vector3(1, 1);
            // normals = new Vector3[] { Vector3.up, Vector3.up, Vector3.up, Vector3.up };
            mesh.MarkDynamic();
            mesh.vertices = vertices;
            mesh.triangles = traingles;
            mesh.uv = uvs;
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();
            // mesh.normals = normals;
            mr.material = material;
            mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            mf.mesh = mesh;

            Destroy(skidgameobject, destroyskidmarkstime);
        }
    }

    [System.Serializable]
    public class GenerateSkidSounds
    {
        // Class that generates the SkidSound:}
        public bool isplayingskidsound;

        public void AddSkidSounds(AudioSource source, bool playskidsound)
        {
            source.loop = true;
            if (playskidsound)
            {
                if (!isplayingskidsound)
                {
                    source.Play();
                    isplayingskidsound = true;
                }
            }
            else
            {
                source.Stop();
                isplayingskidsound = false;
            }
        }
    }
    public GenerateSkid generateskid;
    public GenerateSkidSounds generateskidsounds;
    public bool DebugMode;
    public ParticleSystem burnoutparticle;
    public float currentFrictionsidewaysslip, currentfrictionfowardslip;
    public float fowardsliplimit = 0.988f, sidesliplimit = 0.2f;
    public WheelCollider wheelcollider;
    public AudioSource audiosource;

    void Start()
    {
        if (generateskid.skidtrashholder == null)
        {
            generateskid.skidtrashholder = new GameObject("SkidTrashHolder").transform;
            generateskid.skidtrashholder.tag = "SkidTrashHolder";
        }
        audiosource = GetComponent<AudioSource>();
        wheelcollider = GetComponent<WheelCollider>();
        burnoutparticle.Stop();
    }

    void LateUpdate()
    {
        WheelHit hit;
        wheelcollider.GetGroundHit(out hit);
        currentFrictionsidewaysslip = Mathf.Abs(hit.sidewaysSlip);
        currentfrictionfowardslip = Mathf.Abs(hit.forwardSlip);
        if (burnoutparticle != null)
        {
            burnoutparticle.transform.position = transform.position - transform.up * wheelcollider.radius;
        }


        if (currentFrictionsidewaysslip >= sidesliplimit || currentfrictionfowardslip >= fowardsliplimit)
        {
            if (Vector3.Distance(generateskid.lastwheelpos, this.transform.position) > generateskid.Resolution)
            {
                Doskid();
            }
            DoSkidSound(true);
            if (burnoutparticle != null)
            {
                burnoutparticle.Emit(1);
            }
        }
        else
        {
            DoSkidSound(false);
            generateskid.isskidding = false;
            if (burnoutparticle != null)
                burnoutparticle.Stop();
        }
    }

    public void Doskid()
    {
        generateskid.Addskidmarks(wheelcollider, transform);
    }

    public void DoSkidSound(bool play)
    {
        generateskidsounds.AddSkidSounds(audiosource, play);
    }
}
