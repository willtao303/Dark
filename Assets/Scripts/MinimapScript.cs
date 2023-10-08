using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Tilemaps;

public class MinimapScript : MonoBehaviour
{
    [SerializeField]
    Transform PlayerTransform;

    [SerializeField]
    Transform CameraTransform;

    Texture2D mapTex;
    Sprite mapSprite;

    [SerializeField]
    GameObject MinimapRenderer;

    [SerializeField]
    float MinimapPPU = 10f;


    bool changeOccured = false;

    Vector2Int MinimapCenter;

    bool MinimapMaximized = false;

    // Start is called before the first frame update
    void Start()
    {
        InitializeNewMinimap();
    }

    // Update is called once per frame
    void Update()
    {
        if (changeOccured){
            mapTex.Apply();
            changeOccured = false;
        }

        if (!MinimapMaximized){
            MinimapRenderer.transform.SetLocalPositionAndRotation(PlayerTransform.position/MinimapPPU*(-1), Quaternion.identity);
        } else {
            MinimapRenderer.transform.position = CameraTransform.position + (Vector3.forward*10);
        }
        
        if (Input.GetKeyDown("m")){
            MinimapMaximized = !MinimapMaximized;
            if (MinimapMaximized) {
                MinimapRenderer.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.None;
                MinimapRenderer.transform.localScale = new Vector3(1.6f, 1.6f, 0);
            } else {
                MinimapRenderer.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
                MinimapRenderer.transform.localScale = new Vector3(1, 1, 0);
            }
        }
    }

    public void SetPixel(Vector2Int coords, Color color) {
        mapTex.SetPixel(coords.x, coords.y, color);
        changeOccured = true;
    }

    
    public void SetTileByRaycast(RaycastHit2D ray) {
        int x, y;
        if (ray.transform.gameObject.TryGetComponent(typeof(Tilemap), out Component component)) {
            Vector2Int TileVector = (Vector2Int) ((Tilemap) component).layoutGrid.WorldToCell(ray.point - new Vector2(0, 0.47f));
            SetPixel(MinimapCenter + TileVector, Color.white);
            
        }
        //mapTex.SetPixel(coords.x, coords.y, color);
    }

    public void InitializeNewMinimap() {
        mapTex = new Texture2D(512, 512);
        mapSprite = Sprite.Create(mapTex, new Rect(0, 0, mapTex.width, mapTex.height), new Vector2(0.5f, 0.5f), MinimapPPU);
        MinimapRenderer.GetComponent<SpriteRenderer>().sprite = mapSprite;
        Color MapBgColor = new Color(41f/255, 40f/255, 46f/255);
        for(int i = 0; i < mapTex.width; i++){
            for(int j = 0; j < mapTex.height; j++){
                SetPixel(new Vector2Int(i, j), MapBgColor);
            }
        }
        MinimapCenter = new Vector2Int(mapTex.width/2, mapTex.height/2);
    }
}
