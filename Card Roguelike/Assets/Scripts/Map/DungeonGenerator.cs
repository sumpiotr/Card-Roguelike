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


        }

        HexTilemap.Instance.LoadMap(map);

    }

}
