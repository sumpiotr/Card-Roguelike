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
using Random = UnityEngine.Random;

public class DungeonGenerator
{
    


    public static void  GenerateDungeon(int dungeonWidth, int dungeonHeight, int minRoomWidth, int minRoomHeight, int roomOffset)
    {
        List<Room> rooms = new List<Room>();
        List<BoundsInt> roomsBounds = ProcedularDungeonGenerationAlgorithms.BinarySpacePartitioning(new BoundsInt(new Vector3Int(0, 0, 0), new Vector3Int(dungeonWidth, dungeonHeight, 0)), minRoomWidth, minRoomHeight);

        int[,] map = new int[dungeonWidth, dungeonHeight];
        for(int i = 0; i < dungeonWidth; i++)
        {
            for(int j = 0; j < dungeonHeight; j++)
            {
                map[i, j] = 1;
            }
        }


        List<Vector2Int> roomCenters = new List<Vector2Int>();

        foreach (BoundsInt room in roomsBounds)
        {
            for(int col = roomOffset; col < room.size.x-roomOffset; col++)
            {
                for(int row = roomOffset; row < room.size.y-roomOffset; row++)
                {
                    Vector2Int floorPosition = (Vector2Int)room.min + new Vector2Int(col, row);
                    map[floorPosition.x, floorPosition.y] = 0;
                }
            }
            Vector2Int center = new Vector2Int(room.min.x + (room.size.x / 2), room.min.y + (room.size.y / 2));
            roomCenters.Add(center);
            Debug.Log(center);
            rooms.Add(new Room(center, room));
        }





        List<DelaunayTriangulation.Triangle> triangles = DelaunayTriangulation.Triangulation(roomCenters);
        List<GraphEdge> graphEdges = new List<GraphEdge>();
        foreach (DelaunayTriangulation.Triangle triangle in triangles)
        {
            graphEdges.Add(triangle.edge1);
             graphEdges.Add(triangle.edge2);
             graphEdges.Add(triangle.edge3);
        }

        Graph optimalGraph = GraphFunctions.GetMinimumSpanningTree(new Graph(graphEdges));


        foreach(GraphEdge edge in graphEdges)
        {
            if (optimalGraph.edges.Contains(edge)) continue;
            if(UnityEngine.Random.Range(0, 100) < 10   )optimalGraph.edges.Add(edge);   
        }

        //List<Room> rooms;

        foreach (GraphEdge edge in optimalGraph.edges)
        {
            TileObject object1 = HexTilemap.Instance.GetTileByIndexPosition(edge.src);
            TileObject object2 = HexTilemap.Instance.GetTileByIndexPosition(edge.destination);
            Debug.DrawLine(object1.transform.position, object2.transform.position, Color.red, 60);


            Vector2Int collidor = new Vector2Int(edge.src.x, edge.src.y);
            while(collidor != edge.destination)
            {
                int xDifference = collidor.x - edge.destination.x;
                int yDifference = collidor.y - edge.destination.y;
                Vector2Int directionVector = new Vector2Int(0, 0);
                if(Math.Abs(xDifference) > Math.Abs(yDifference))
                {
                    directionVector = xDifference > 0 ? new Vector2Int(-1, 0) : new Vector2Int(1, 0);
                }
                else
                {
                    directionVector = yDifference > 0 ? new Vector2Int(0, -1) : new Vector2Int(0, 1);
                }
                collidor += directionVector;
                map[collidor.x, collidor.y] = 0;
            }


            Room room1 = rooms.Find(x => x.center == edge.src);
            Room room2 = rooms.Find(x => x.center == edge.destination);

            room1.AddConnectedRoom(room2);
            room2.AddConnectedRoom(room1);

        }

        HexTilemap.Instance.LoadMap(map);

        FillRooms(rooms);

    }

    private static void FillRooms(List<Room> rooms) {
        List<Room> emptyRooms = new List<Room>(rooms);
        Room startRoom = emptyRooms[Random.Range(0, rooms.Count - 1)];
        emptyRooms.Remove(startRoom);
        Room endRoom = emptyRooms[Random.Range(0, rooms.Count - 1)];
        emptyRooms.Remove(endRoom);

        SpawnManager.Instance.SpawnPlayer(startRoom.center);

        SpawnManager.Instance.SpawnExit(endRoom.center);

       
        foreach(Room room in emptyRooms)
        {
            SpawnManager.Instance.SpawnEnemies(room);
        }
    }

}

public enum RoomType
{
    Start,
    Exit,
    Encounter,
    Treasure
}

public class Room : IEquatable<Room>
{
    public Vector2Int center;
    public BoundsInt size;
    public List<Room> connectedRooms;


    public Room(Vector2Int center, BoundsInt size)
    {
        this.center = center;
        connectedRooms = new List<Room>();
        this.size = size;
    }

    public void AddConnectedRoom(Room connectedRoom)
    {
        foreach(Room room in connectedRooms)
        {
            if(room.center == connectedRoom.center)
            {
                return;
            }
        }
        connectedRooms.Add(connectedRoom);
    }

    public bool Equals(Room other)
    {
        return center == other.center;
    }
}
