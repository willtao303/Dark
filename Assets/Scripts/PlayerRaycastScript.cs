using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRaycastScript : MonoBehaviour
{

    [SerializeField] private MinimapScript Minimap;

    [SerializeField]
    public int viewDist = 5;
    [SerializeField]
    private float flickerDist = 0.1f;
    private float flicker = 0.0f;
    private float flickerChange = 0.0125f;
    private int flickerSpeed = 0;

    [SerializeField]
    [Range(0, 360)]
    private int viewAngle = 360;
    
    [SerializeField]
    private float raysPerDeg = 1.0f;

    [SerializeField]
    private int itersPerEdge=5;
    [SerializeField]
    private float rangePerEdge=1.0f;
    

    [SerializeField]
    private bool runRaycast = true;
    

    
    [SerializeField] private LayerMask viewMask;
    [SerializeField] private MeshFilter FovMeshFilter;
    [SerializeField] private MeshFilter FovShadowMesh;
    [SerializeField] private MeshRenderer FovShadowRenderer;

    [SerializeField]
    private Color FovShadowColor = new Color(0.6f, 0.4f, 0.0f, 0.02f);

    private Mesh FovMesh;
    private Mesh ShadowMesh;

    private float OilAmount = 0.9F;

    void Start()
    {
        FovMesh = new Mesh();
        ShadowMesh = new Mesh();
        FovMeshFilter.mesh = FovMesh;
        FovShadowMesh.mesh = ShadowMesh;
        FovShadowRenderer.material.color = FovShadowColor;
    }

    void Update()
    {
        if (flickerSpeed > 2) {
            if (((int)Random.Range(0, 15) != 3)&&(flickerChange > 0 && flicker < flickerDist || (flickerChange < 0 && flicker > 0))){
                flicker += flickerChange * Random.Range(0, 3) * (Mathf.Min(OilAmount, 0.9f)*0.6f + 0.4f);
            } else {
                flickerChange *= -1;
            }
            flickerSpeed = 0 - (int)((1.0f-OilAmount)*3);
        } else {
            flickerSpeed++;
        }

        int raysMax;
        float raysIncr;
        float rayDist = viewDist*Mathf.Min(OilAmount, 0.9f)+flicker;

        if (rayDist > 0.7) {
            OilAmount -= 0.00015f;
        } else {

        }

        if (rayDist < 1.5){
            raysMax = Mathf.RoundToInt(viewAngle * 0.05f);
            raysIncr = 1.0f / 0.05f;
        } else {
            raysMax = Mathf.RoundToInt(viewAngle * raysPerDeg);
            raysIncr = 1.0f / raysPerDeg;
        }

        float angle;


        List<Vector3> vertexRaw = new List<Vector3>();
        RayInfo prevRay = new RayInfo();

        for (int i = 0; i <= raysMax; i++){
            angle = i * raysIncr;
            RayInfo newRay = castRay(angle, rayDist);
            if (i != 0 && rayDist > 0.9) {
                if (hitDiffer(prevRay, newRay)) {
                    EdgeInfo newEdge = edgeRay(prevRay, newRay, rayDist);
                    if (newEdge.pointA != Vector2.zero) {
                        vertexRaw.Add(newEdge.pointA);
                    }
                    if (newEdge.pointB != Vector2.zero) {
                        vertexRaw.Add(newEdge.pointB);
                    }
                }
            }
            prevRay = newRay;
            vertexRaw.Add(newRay.colPoint);

            /*Vector3 DatatypeConversionsAreStupid = DirVector(angle);
            Color a = Color.red;
            if (newRay.hit){
                a = Color.cyan;
            }
            Debug.DrawLine(transform.position, transform.position + DatatypeConversionsAreStupid*newRay.dist, a);*/
        }

        Vector3[] vertex = new Vector3[vertexRaw.Count + 1];
        vertex[0] = Vector2.zero;
        int[] triangles = new int[(vertexRaw.Count - 1) * 3];

        for (int i = 0; i < vertexRaw.Count; i++) {
            vertex[i+1] = vertexRaw[i]; 
            if ((i*3 + 3) < (vertexRaw.Count)*3){
                triangles[i*3] = 0;
                triangles[i*3 + 1] = i+1;
                triangles[i*3 + 2] = i+2;
            }
        }

        FovMesh.Clear();
        FovMesh.vertices = vertex;
        FovMesh.triangles = triangles;

        ShadowMesh.Clear();
        ShadowMesh.vertices = vertex;
        ShadowMesh.triangles = triangles;
    }

    RayInfo castRay(float angle, float dist){
        RaycastHit2D ray = Physics2D.Raycast(transform.position, DirVector(angle), dist, viewMask);
        if (ray.collider != null && runRaycast){
            Minimap.SetTileByRaycast(ray);
            return new RayInfo(ray.point - (Vector2)transform.position, true, ray.distance, angle);
        } else {
            return new RayInfo(DirVector(angle)*(dist), false, dist, angle);
        }

    }
    EdgeInfo edgeRay(RayInfo rayA, RayInfo rayB, float dist){
        RayInfo rayMin = rayA;
        RayInfo rayMax = rayB;
        float angle;
        
        for (int i = 0; i < itersPerEdge; i++){
            angle = (rayMin.angle + rayMax.angle)/2.0f;
            RayInfo ray = castRay(angle, dist);
            
            //Color a = Color.magenta;
            if (!hitDiffer(ray, rayMin)) {
                rayMin = ray;
            } else {
                rayMax = ray;
                //a = Color.blue;
            }
            /*Vector3 DatatypeConversionsAreStupid = DirVector(angle);
            Debug.DrawLine(transform.position, transform.position + DatatypeConversionsAreStupid*ray.dist, a);*/
            if (i + 1== itersPerEdge){
                Vector3 DatatypeConversionsAreStupid = DirVector(angle);
            Debug.DrawLine(transform.position, transform.position + DatatypeConversionsAreStupid*ray.dist, Color.cyan);
            }
        }
        return new EdgeInfo(rayMin.colPoint, rayMax.colPoint);
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
    private struct EdgeInfo {
        public Vector2 pointA;
        public Vector2 pointB;

        public EdgeInfo(Vector2 _pointA, Vector2 _pointB){
            pointA = _pointA;
            pointB = _pointB;
        }
    }

    private Vector2 DirVector(float angle){
        return new Vector2(Mathf.Sin(angle * Mathf.Deg2Rad), Mathf.Cos(angle * Mathf.Deg2Rad));
    }

    private bool hitDiffer(RayInfo rayA, RayInfo rayB){
        return (rayA.hit ^ rayB.hit) || (Mathf.Abs(rayA.dist - rayB.dist) > rangePerEdge);
    }

    public void AddOil(float val) {
        OilAmount += val;
        if (OilAmount > 1.5f) {
            OilAmount = 1.5f;
        }
    }
    public float GetOilPercent() {
        return OilAmount / 1.5f;
    }
}
