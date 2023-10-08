using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;

public class GridGenScript : MonoBehaviour
{
    [SerializeField]
    private Tilemap Highlights;
    [SerializeField]
    private Tilemap Walls;
    [SerializeField]
    private Tilemap Floor;


    [SerializeField]
    private TileBase wallTile; 
    [SerializeField]
    private TileBase highlightTile;

    
    [SerializeField]
    private TileBase[] WallTileList;
    [SerializeField]
    private TileBase[] HighlightTileList;

    [SerializeField]
    private TileBase[] StairsTileList;

    [SerializeField]
    private BagManagerScript BagManager;

    [SerializeField]
    private FlameManagerScript FlameManager;
    
    [SerializeField]
    private PlayerScript Player;

    [SerializeField]
    private MinimapScript Minimap;


    private Queue<Doorway> unresolvedDoors = new Queue<Doorway>();



    
    private RoomPreset[] roomPresetsCaps;
    private RoomPreset[] roomPresetsPassages;
    private RoomPreset[] roomPresetsSplits;
    private RoomPreset[] roomPresetsMulti;
    void Start()
    {
        
        roomPresetsCaps = new RoomPreset[12];
        roomPresetsCaps[0] = RoomPreset.getMapFromFile("Assets/Scripts/RoomPresets/CapSouth0.txt");
        roomPresetsCaps[1] = RoomPreset.getMapFromFile("Assets/Scripts/RoomPresets/CapEast0.txt");
        roomPresetsCaps[2] = RoomPreset.getMapFromFile("Assets/Scripts/RoomPresets/CapWest0.txt");
        roomPresetsCaps[3] = RoomPreset.getMapFromFile("Assets/Scripts/RoomPresets/CapNorth0.txt");
        roomPresetsCaps[4] = RoomPreset.getMapFromFile("Assets/Scripts/RoomPresets/CapSouth1.txt");
        roomPresetsCaps[5] = RoomPreset.getMapFromFile("Assets/Scripts/RoomPresets/CapSouth3.txt");
        roomPresetsCaps[6] = RoomPreset.getMapFromFile("Assets/Scripts/RoomPresets/CapEast2.txt");
        roomPresetsCaps[7] = RoomPreset.getMapFromFile("Assets/Scripts/RoomPresets/CapNorth1.txt");
        roomPresetsCaps[8] = RoomPreset.getMapFromFile("Assets/Scripts/RoomPresets/CapSouth4.txt");
        roomPresetsCaps[9] = RoomPreset.getMapFromFile("Assets/Scripts/RoomPresets/CapEast4.txt");
        roomPresetsCaps[10] = RoomPreset.getMapFromFile("Assets/Scripts/RoomPresets/CapWest4.txt");
        roomPresetsCaps[11] = RoomPreset.getMapFromFile("Assets/Scripts/RoomPresets/CapNorth4.txt");

        roomPresetsPassages = new RoomPreset[2];
        roomPresetsPassages[0] = RoomPreset.getMapFromFile("Assets/Scripts/RoomPresets/Path0.txt");
        roomPresetsPassages[1] = RoomPreset.getMapFromFile("Assets/Scripts/RoomPresets/Path1.txt");
        
        roomPresetsMulti = new RoomPreset[3];
        roomPresetsMulti[0] = RoomPreset.getMapFromFile("Assets/Scripts/RoomPresets/Center.txt");
        roomPresetsMulti[1] = RoomPreset.getMapFromFile("Assets/Scripts/RoomPresets/Center1.txt");
        roomPresetsMulti[2] = RoomPreset.getMapFromFile("Assets/Scripts/RoomPresets/Multi0.txt");
        
    }

    [SerializeField]
    private SpriteRenderer TransitionScreen;
    private bool transitionDark = true;
    private bool resetRooms = true;
    void Update()
    {   
            
        if (resetRooms){
            if (!transitionDark){
                if (TransitionScreen.color.a < 1){
                    Color newColor = TransitionScreen.color;
                    newColor.a = newColor.a + 0.05f;
                    TransitionScreen.color = newColor;
                } else {
                    transitionDark = true;
                }
            } 
            if (transitionDark) {
                InitNewMap();
                GenerateMap();
                resetRooms = false;
                Player.resetPos();
                Minimap.InitializeNewMinimap();
            }
        } else {
            if (transitionDark){
                if (TransitionScreen.color.a > 0){
                    Color newColor = TransitionScreen.color;
                    newColor.a -= 0.02f;
                    TransitionScreen.color = newColor;
                } else {
                    transitionDark = false;
                }
            }
        }

        if (Input.GetKey(KeyCode.Space) && resetRooms == false){
            Debug.Log("aa");
            GenerateNewGrid();
        }
    }

    private void InitNewMap(){
        Highlights.ClearAllTiles();
        Walls.ClearAllTiles();
        Floor.ClearAllTiles();
        FlameManager.ClearAllFlames();
        BagManager.ClearAllBags();
    }

    public void GenerateNewGrid() {
        resetRooms = true;
    }

    private void GenerateMap(){
        //roomList = new List<Room>();
        Room rootRoom = new Room(roomPresetsMulti[Random.Range(0, 2)], -8, 8, unresolvedDoors, null);
        setRoom(rootRoom);
        //roomList.Add(newRoom);

        while (unresolvedDoors.Count > 0){
            Doorway door = unresolvedDoors.Dequeue();
            bool valid = false;
            
            int rng = Random.Range(0, 10);
            string batch;
            RoomPreset[] roomPresetBatch;
            if (rng == 0){
                batch = "multi";
                roomPresetBatch = roomPresetsMulti;
            } 
            else if (rng < 4) {
                batch = "passage";
                roomPresetBatch = roomPresetsPassages;
            } 
            else {
                batch = "cap";
                roomPresetBatch = roomPresetsCaps;
            }

            HashSet<int> testedRooms = new HashSet<int>();
            while (!valid) {
                if (testedRooms.Count == roomPresetBatch.GetLength(0)){
                    Debug.Log("Possible config not found: " + batch);
                    setDeadend(door);
                    break;
                }
                int index = Random.Range(0, roomPresetBatch.GetLength(0));
                while (testedRooms.Contains(index)){
                    index = Random.Range(0, roomPresetBatch.GetLength(0));
                }
                testedRooms.Add(index);

                RoomPreset roomTemplate = roomPresetBatch[index];
                Doorway newDoor = roomTemplate.getAvailableDoor(Doorway.Invert(door.facing)); 

                if (newDoor != null) {
                    HashSet<int> testedHalls = new HashSet<int>();
                    while (!valid) {
                        if (testedHalls.Count == 4){
                            break;
                        }
                        int hallwayDist =  Random.Range(0, 4)*2;
                        while (testedHalls.Contains(hallwayDist)){
                            hallwayDist = Random.Range(0, 4)*2;
                        }
                        testedHalls.Add(hallwayDist);

                        Vector2Int hallVector = Doorway.DirToVector(door.facing)*hallwayDist;
                        valid = isValid(roomTemplate, door.x - newDoor.x + hallVector.x, door.y + newDoor.y + hallVector.y);
                        if (valid){
                            door.resolved = true;
                            setHallway(door, hallwayDist);
                            Room newRoom = new Room(roomTemplate, (door.x-newDoor.x+hallVector.x), (door.y+newDoor.y+hallVector.y), unresolvedDoors, newDoor);
                            setRoom(newRoom);
                        }
                    }
                }
                
            }
        }
    }
    
    private bool isValid(RoomPreset reference, int pivot_x, int pivot_y) {
        // TODO: change this to validation by ground tilemap
        Vector3Int pos = new Vector3Int(pivot_x, pivot_y, 0);
        
        for (int x_pos = 0; x_pos < reference.width; x_pos++){
            for (int y_pos = 0; y_pos < reference.height; y_pos++) {
                if (Walls.HasTile(pos)) {
                    if (reference.charmap[x_pos, y_pos] != RoomPreset.BORDER && reference.charmap[x_pos, y_pos] != RoomPreset.NULL) {
                        return false;
                    }
                }
                pos.y -= 1;
            }
            pos.x += 1;
            pos.y = pivot_y;
        }
        return true;
    }
    
    private void setHallway (Doorway door, int hallDist) {
        Vector3Int dirVector;
        Vector3Int spaceVector;
        Vector3Int doorVector = new Vector3Int(door.x, door.y, 0);
        if (door.facing == Doorway.Direction.North) {
            dirVector = Vector3Int.up;
            spaceVector = Vector3Int.right*(door.size+1);
            doorVector += Vector3Int.left;
        } else if (door.facing == Doorway.Direction.South) {
            dirVector = Vector3Int.down;
            spaceVector = Vector3Int.right*(door.size+1);
            doorVector += Vector3Int.left;
        } else if (door.facing == Doorway.Direction.East) {
            dirVector = Vector3Int.right;
            spaceVector = Vector3Int.down*(door.size+1);
            doorVector += Vector3Int.up;
        } else if (door.facing == Doorway.Direction.West) {
            dirVector = Vector3Int.left;
            spaceVector = Vector3Int.down*(door.size+1);
            doorVector += Vector3Int.up;
        } else {
            dirVector = Vector3Int.zero;
            spaceVector = Vector3Int.zero;
        }

        for (int i = 1; i < hallDist; i++){
            Walls.SetTile(doorVector + (i*dirVector), wallTile);
            Walls.SetTile(doorVector + spaceVector + (i*dirVector), wallTile);
            Highlights.SetTile(doorVector + (i*dirVector), highlightTile);
            Highlights.SetTile(doorVector + spaceVector + (i*dirVector), highlightTile);
        }
    }
    private void setDeadend(Doorway door) {
        if (door.facing == Doorway.Direction.North || door.facing == Doorway.Direction.South){
            for (int i = 0; i < door.size; i++) {
                Walls.SetTile(new Vector3Int(door.x + i, door.y), wallTile);
                Highlights.SetTile(new Vector3Int(door.x + i, door.y), highlightTile);
            }
        } else {
            for (int i = 0; i < door.size; i++) {
                Walls.SetTile(new Vector3Int(door.x, door.y - i), wallTile);
                Highlights.SetTile(new Vector3Int(door.x, door.y - i), highlightTile);
            }
        }
    }

    private void setRoom(Room room){
        setRoom(room.preset, room.xAnchor, room.yAnchor);
    }
    private void setRoom (RoomPreset reference, int pivot_x, int pivot_y) {
        Vector3Int pos = new Vector3Int(pivot_x, pivot_y, 0);

        for (int x_pos = 0; x_pos < reference.width; x_pos++){
            for (int y_pos = 0; y_pos < reference.height; y_pos++) {
                if (reference.charmap[x_pos, y_pos] == RoomPreset.BORDER || reference.charmap[x_pos, y_pos] == RoomPreset.WALL){
                    Walls.SetTile(pos, wallTile);
                    Highlights.SetTile(pos, highlightTile);
                } else if (reference.charmap[x_pos, y_pos] == RoomPreset.COLUMN) {
                    Walls.SetTile(pos, WallTileList[0]);
                    Highlights.SetTile(pos, HighlightTileList[0]);
                } else if (reference.charmap[x_pos, y_pos] == RoomPreset.BAG) {
                    if (Random.value < 0.5){
                        BagManager.AddBag(x_pos + pivot_x, pivot_y - y_pos);
                    }
                } else if (reference.charmap[x_pos, y_pos] == RoomPreset.FLAME_CENTERED){
                    if (Random.value < 0.5){
                    FlameManager.AddNew(x_pos + pivot_x, pivot_y - y_pos);}
                } else if (reference.charmap[x_pos, y_pos] == RoomPreset.FLAME){
                    if (Random.value < 0.5){
                    FlameManager.AddNew(x_pos + pivot_x + 0.5f, pivot_y - y_pos + 0.5f);}
                } 
                //Stairs
                else if (reference.charmap[x_pos, y_pos] == RoomPreset.STAIRS_U || reference.charmap[x_pos, y_pos] == RoomPreset.STAIRS_U_ROOT){
                    Floor.SetTile(pos, StairsTileList[0]);
                } else if (reference.charmap[x_pos, y_pos] == RoomPreset.STAIRS_D || reference.charmap[x_pos, y_pos] == RoomPreset.STAIRS_D_ROOT){
                    Floor.SetTile(pos, StairsTileList[1]);
                } else if (reference.charmap[x_pos, y_pos] == RoomPreset.STAIRS_L || reference.charmap[x_pos, y_pos] == RoomPreset.STAIRS_L_ROOT){
                    Floor.SetTile(pos, StairsTileList[2]);
                    
                } else if (reference.charmap[x_pos, y_pos] == RoomPreset.STAIRS_R || reference.charmap[x_pos, y_pos] == RoomPreset.STAIRS_R_ROOT){
                    Floor.SetTile(pos, StairsTileList[3]);
                }
                pos.y -= 1;
            }
            pos.x += 1;
            pos.y = pivot_y;
        }
    }

    
}
