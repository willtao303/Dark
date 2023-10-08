using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class RoomPreset
{
    public int width;
    public int height;

    public const char BORDER = 'W';
    public const char WALL = 'w';
    public const char COLUMN = '1';
    public const char NULL = ' ';
    public const char BAG = 'b';
    public const char FLAME_CENTERED = 'F';
    public const char FLAME = 'f';

    public const char STAIRS_L = 'l';
    public const char STAIRS_L_ROOT = 'L';
    public const char STAIRS_R = 'r';
    public const char STAIRS_R_ROOT = 'R';
    public const char STAIRS_U = 'u';
    public const char STAIRS_U_ROOT = 'U';
    public const char STAIRS_D = 'v';
    public const char STAIRS_D_ROOT = 'V';

    public char[,] charmap;

    public Doorway[] doors;

    public Doorway getAvailableDoor(Doorway.Direction dir){
        foreach (Doorway door in doors) {
            if (door.facing == dir && !door.resolved){
                return door;
            }
        }
        return null;
    }

    public static RoomPreset getMapFromFile(string file_path)
    {
        StreamReader input_stream = new StreamReader(file_path);

        RoomPreset rawMap = new RoomPreset();
        string[] stats = input_stream.ReadLine( ).Split(" ");

        rawMap.width = int.Parse(stats[0]);
        rawMap.height = int.Parse(stats[1]);
        rawMap.charmap = new char[rawMap.width, rawMap.height];
        int num_doorways = int.Parse(stats[2]);
        rawMap.doors = new Doorway[num_doorways];

        for (int i_y = 0; i_y < rawMap.height; i_y++) {
            string rawRow = input_stream.ReadLine( );
            for (int i_x = 0; i_x < rawMap.width; i_x++){
                rawMap.charmap[i_x, i_y] = rawRow[i_x];
            }
        }
        for (int i = 0; i < num_doorways; i++){
            string[] rawLine = input_stream.ReadLine().Split(" ");
            rawMap.doors[i] = new Doorway(rawLine[0][0], int.Parse(rawLine[2]), int.Parse(rawLine[3]), int.Parse(rawLine[1]));
        }
        input_stream.Close( ); 

        return rawMap;
    }
}

public class Room {
    public Room(RoomPreset _preset, int _xAnchor, int _yAnchor, Queue<Doorway> allDoors, Doorway originDoor){
        preset = _preset;
        xAnchor = _xAnchor;
        yAnchor = _yAnchor;

        doors = new Doorway[_preset.doors.GetLength(0)];
        for (int i = 0; i < _preset.doors.GetLength(0); i++){
            doors[i] = _preset.doors[i].Clone();
            doors[i].x += xAnchor;
            doors[i].y = yAnchor - doors[i].y;
            if (_preset.doors[i] == originDoor){
                doors[i].resolved = true;
            } else {
                allDoors.Enqueue(doors[i]);
            }
        }
    }

    public int xAnchor;
    public int yAnchor;

    public RoomPreset preset;
    
    public Doorway[] doors;
}

public class Doorway{
    public enum Direction {
        North,
        East,
        South,
        West,
        NULLDIR
    }

    public static Vector2Int DirToVector(Direction dir){
        
        if (dir == Doorway.Direction.North) {
            return Vector2Int.up;
        } else if (dir == Doorway.Direction.South) {
            return Vector2Int.down;
        } else if (dir == Doorway.Direction.West) {
            return Vector2Int.left;
        } else if (dir == Doorway.Direction.East) {
            return Vector2Int.right;
        } else {
            return Vector2Int.zero;
        }
    }
    public static Direction Invert(Direction dir){
        if (dir == Direction.North){
            return Direction.South;
        }
        if (dir == Direction.South){
            return Direction.North;
        }
        if (dir == Direction.West){
            return Direction.East;
        }
        if (dir == Direction.East){
            return Direction.West;
        }
        return Direction.NULLDIR;
    }

    public Direction facing;
    // Top Right Corner
    public int x;
    public int y;
    public int size;
    
    public bool resolved = false;

    private Doorway(Direction dir, int _x, int _y, int _size, bool res){
        facing = dir;
        x = _x;
        y = _y;
        size = _size;
        resolved = res;
    }
    public Doorway(char dir, int _x, int _y, int _size){
        if (dir == 'N') {
            facing = Direction.North;
        } else if (dir == 'E') {
            facing = Direction.East;
        } else if (dir == 'S') {
            facing = Direction.South;
        } else if (dir == 'W') {
            facing = Direction.West;
        }
        x = _x;
        y = _y;
        size = _size;
    }

    public Doorway Clone(){
        return new Doorway(facing, x, y, size, resolved);
    }
}