using System.Collections;
using System.Collections.Generic;
using Tilemap;
using Tilemap.Tile;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class AStar 
{
    public static List<Node> findPath(HexTilemap grid, Vector2Int startPosition, Vector2Int targetPosition, int maxPathLength = -1)
    {
        Node currentNode = null;
        List<Node> openList = new List<Node>();
        List<Node> closedList = new List<Node>();


        openList.Add(new Node(startPosition, targetPosition, startPosition));

        while (openList.Count > 0)
        {
            Node lowest = openList[0];
            foreach (Node node in openList)
            {
                if (node.fCost < lowest.fCost) lowest = node;
            }
            currentNode = lowest;

            closedList.Add(currentNode);
            openList.Remove(currentNode);

            if (getPathLength(currentNode) - 1 > maxPathLength && maxPathLength > 0)
            {
                return null;
            }
            if (currentNode.nodePosition == targetPosition)
            {
                return GetPathFromNode(currentNode);
            }

            List<Vector2Int> adjacentedNodes = grid.GetWalkableNeighboursPositions(currentNode.nodePosition);

            foreach (Vector2Int nodePosition in adjacentedNodes)
            {
                TileObject tile = grid.GetHex(nodePosition);
                if (getNodeByPosition(closedList, nodePosition) != null) continue;
                if (!tile.IsEmpty() && nodePosition != targetPosition) continue;
                Node adjacentedNode = getNodeByPosition(openList, nodePosition);
                if (adjacentedNode == null) adjacentedNode = new Node(nodePosition, targetPosition, startPosition);
                if (openList.Contains(adjacentedNode))
                {
                    float tentativeGCost = currentNode.gCost + Vector2Int.Distance(currentNode.nodePosition, adjacentedNode.nodePosition);
                    if (tentativeGCost < adjacentedNode.gCost)
                    {
                        adjacentedNode.previousNode = currentNode;
                        adjacentedNode.gCost = tentativeGCost;
                        adjacentedNode.fCost = adjacentedNode.hCost + adjacentedNode.gCost;

                    }
                }
                else
                {
                    adjacentedNode.previousNode = currentNode;
                    openList.Add(adjacentedNode);
                }

            }
        }
        return null;
    }

    private static Node getNodeByPosition(List<Node> nodeList, Vector2Int position)
    {
        foreach (Node node in nodeList)
        {
            if (node.nodePosition == position) return node;
        }
        return null;
    }

    private static List<Node> GetPathFromNode(Node endNode)
    {
        if (endNode.previousNode == null) return null;
        List<Node> path = new List<Node>();
        Node currentNode = endNode;
        while (currentNode.previousNode != null)
        {
            path.Add(currentNode);
            currentNode = currentNode.previousNode;
        }
        path.Reverse();
        return path;
    }

    private static int getPathLength(Node endNode)
    {
        if (endNode.previousNode == null) return 0;
        List<Node> path = new List<Node>();
        path.Add(endNode);
        Node currentNode = endNode;
        while (currentNode.previousNode != null)
        {
            path.Add(currentNode);
            currentNode = currentNode.previousNode;
        }
        return path.Count;
    }

    /*public static List<Vector2Int> findShortestPath(Grid grid, Vector2Int startPosition, Vector2Int targetPosition, List<Vector2Int> tiles = null)
    {
        //zwraca najkrotsza sciezke bez uwzgledniania zadnych zmiennych
        if (tiles == null) tiles = new List<Vector2Int>();

        int dirx = (targetPosition.x - startPosition.x) == 0 ? 0 : (int)((targetPosition.x - startPosition.x) / Math.Abs(targetPosition.x - startPosition.x));
        int diry = (targetPosition.y - startPosition.y) == 0 ? 0 : (int)((targetPosition.y - startPosition.y) / Math.Abs(targetPosition.y - startPosition.y));
        Vector2Int newStartPos = new Vector2Int(startPosition.x + dirx, startPosition.y + diry);
        if (grid.GetTileByPosition(newStartPos) == grid.GetTileByPosition(targetPosition)) return tiles;
        tiles.Add(newStartPos);
        return findShortestPath(grid, newStartPos, targetPosition, tiles);
    }*/
}

public class Node
{
    public float fCost;
    public float gCost;
    public float hCost;

    public Vector2Int nodePosition;

    public Node previousNode;

    public Node(Vector2Int nodePosition) { this.nodePosition = nodePosition; }

    public Node(Vector2Int nodePosition, Vector2Int targetNodePosition, Vector2Int startNodePosition, Node previousNode = null)
    {
        this.gCost = HexTilemap.AxialDistance(startNodePosition, nodePosition);
        this.hCost = HexTilemap.AxialDistance(nodePosition, targetNodePosition);
        this.nodePosition = nodePosition;
        fCost = gCost + hCost;
        this.previousNode = previousNode;
    }
}
