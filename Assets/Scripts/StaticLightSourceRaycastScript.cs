using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticLightSourceRaycastScript : MonoBehaviour
{

    public int viewDist = 5;
    public float flickerDist = 0.1f;
    private float flicker = 0.0f;
    private float flickerChange = 0.0125f;
    private int flickerSpeed = 0;

    [Range(0, 360)]
    public int viewAngle = 360;
    public float raysPerDeg = 0.2f;

    public int itersPerEdge=3;
    public float rangePerEdge=1.0f;
    

    [SerializeField]
    private bool runRaycast = true;

    private Vector2 raycastOrigin;
    

    public LayerMask viewMask;
    

    [SerializeField]
    private MeshFilter ShadowMeshFilter;
    [SerializeField]
    private MeshRenderer ShadowRenderer;
    public Color FovShadowColor = new Color((51f/255f), (45f/255f), (49f/255f), 1);

    Mesh MaskMesh;
    Mesh ShadowMesh;


    void Start()
    {
        MaskMesh = new Mesh();
        ShadowMesh = new Mesh();
        GetComponent<MeshFilter>().mesh = MaskMesh;
        ShadowMeshFilter.mesh = ShadowMesh;
        ShadowRenderer.material.color = FovShadowColor;
        raycastOrigin = new Vector2(transform.position.x, transform.position.y);
    }

    void Update()
    {
        if (flickerSpeed > 2) {
            if (((int)Random.Range(0, 15) != 3)&&(flickerChange > 0 && flicker < flickerDist || (flickerChange < 0 && flicker > 0))){
                flicker += flickerChange * Random.Range(0, 3);
            } else {
                flickerChange *= -1;
            }
            flickerSpeed = 0;
        } else {
            flickerSpeed++;
        }

        int raysMax = Mathf.RoundToInt(viewAngle * raysPerDeg);
        float raysIncr = 1.0f / raysPerDeg;
        float rayDist = viewDist+flicker;


        float angle;


        Vector3[] vertex = new Vector3[raysMax+1 + 1];
        vertex[0] = Vector2.zero;
        int[] triangles = new int[(raysMax+1 - 1) * 3];

        for (int i = 0; i <= raysMax; i++){
            angle = i * raysIncr;
            RayInfo newRay = castRay(angle, rayDist);

            vertex[i+1] = newRay.colPoint; 
            if ((i*3 + 3) < (raysMax+1)*3){
                triangles[i*3] = 0;
                triangles[i*3 + 1] = i+1;
                triangles[i*3 + 2] = i+2;
            }

            //Debug.DrawLine(transform.position, transform.position + DatatypeConversionsAreStupid*newRay.dist, a);   
        }

        MaskMesh.Clear();
        MaskMesh.vertices = vertex;
        MaskMesh.triangles = triangles;

        ShadowMesh.Clear();
        ShadowMesh.vertices = vertex;
        ShadowMesh.triangles = triangles;
    }

    RayInfo castRay(float angle, float dist){
        RaycastHit2D ray = Physics2D.Raycast(raycastOrigin, DirVector(angle), dist, viewMask);
        if (ray.collider != null && runRaycast){
            return new RayInfo(ray.point - raycastOrigin, true, ray.distance, angle);
        } else {
            return new RayInfo(DirVector(angle)*(dist), false, dist, angle);
        }

    }

    private struct RayInfo {

        public Vector2 colPoint; // non global
        public bool hit;
        public float dist;
        public float angle;

        public RayInfo(Vector2 _colPoint, bool _hit, float _dist, float _angle){
            colPoint = _colPoint;
            hit = _hit;
            dist = _dist;
            angle = _angle;
        }
    }

    private Vector2 DirVector(float angle){
        return new Vector2(Mathf.Sin(angle * Mathf.Deg2Rad), Mathf.Cos(angle * Mathf.Deg2Rad));
    }
}
