using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Tilemap.Tile;
using Tilemap;
using UnityEngine;
using Unity.VisualScripting;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class DungeonGenerator
{
    


    public static void  GenerateDungeon(int dungeonWidth, int dungeonHeight, int minRoomWidth, int minRoomHeight, int roomOffset)
    {
        List<BoundsInt> rooms = ProcedularDungeonGenerationAlgorithms.BinarySpacePartitioning(new BoundsInt(new Vector3Int(0, 0, 0), new Vector3Int(dungeonWidth, dungeonHeight, 0)), minRoomWidth, minRoomHeight);

        int[,] map = new int[dungeonWidth, dungeonHeight];
        for(int i = 0; i < dungeonWidth; i++)
        {
            for(int j = 0; j < dungeonHeight; j++)
            {
                map[i, j] = 1;
            }
        }


        List<Vector2Int> roomCenters = new List<Vector2Int>();

        foreach (BoundsInt room in rooms)
        {
            for(int col = roomOffset; col < room.size.x-roomOffset; col++)
            {
                for(int row = roomOffset; row < room.size.y-roomOffset; row++)
                {
                    Vector2Int floorPosition = (Vector2Int)room.min + new Vector2Int(col, row);
                    map[floorPosition.x, floorPosition.y] = 0;
                }
            }

            roomCenters.Add(new Vector2Int(room.min.x + room.size.x / 2, room.min.y + room.size.y / 2));
        }

        HexTilemap.Instance.LoadMap(map);



        List<DelaunayTriangulation.Triangle> triangles = DelaunayTriangulation.Triangulation(roomCenters);
        Debug.Log(triangles.Count);

        foreach(DelaunayTriangulation.Triangle triangle in triangles)
        {
            TileObject object1 = HexTilemap.Instance.GetTileByIndexPosition(triangle.a);
            TileObject object2 = HexTilemap.Instance.GetTileByIndexPosition(triangle.b);
            TileObject object3 = HexTilemap.Instance.GetTileByIndexPosition(triangle.c);

      
            Debug.DrawLine(object1.transform.position, object2.transform.position, Color.red, 60);
            Debug.DrawLine(object1.transform.position, object3.transform.position, Color.red, 60);
            Debug.DrawLine(object2.transform.position, object3.transform.position, Color.red, 60);
        }


      
    }

}
