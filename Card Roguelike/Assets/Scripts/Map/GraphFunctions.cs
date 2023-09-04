using System;
using System.Collections.Generic;
using UnityEngine;

public class GraphFunctions
{
    // A utility function to find
    // the subset of an element i
    private static Vector2Int find(Dictionary<Vector2Int, Vector2Int> parent, Vector2Int i)
    {
        if (parent[i] == i)
            return i;
        return find(parent, parent[i]);
    }

    // A utility function to do
    // union of two subsets
    private static void Union(Dictionary<Vector2Int, Vector2Int> parent, Vector2Int x, Vector2Int y)
    {
        parent[x] = y;
    }

    // The main function to check
    // whether a given graph
    // contains cycle or not
    public static bool ContainsCycle(Graph graph)
    {
        // Allocate memory for
        // creating V subsets
        Dictionary<Vector2Int, Vector2Int> parent = new Dictionary<Vector2Int, Vector2Int>();

        // Initialize all subsets as
        // single element sets
        for (int i = 0; i < graph.verticesCount; ++i)
            parent.Add(graph.vertices[i], graph.vertices[i]);

        // Iterate through all edges of graph,
        // find subset of both vertices of every
        // edge, if both subsets are same, then
        // there is cycle in graph.
        for (int i = 0; i < graph.edgesCount; ++i)
        {
            Vector2Int x = find(parent, graph.edges[i].src);
            Vector2Int y = find(parent, graph.edges[i].destination);

            if (x == y)
                return true;

            Union(parent, x, y);
        }
        return false;
    }


    public static Graph GetMinimumSpanningTree(Graph graph)
    {
        Graph newGraph = new Graph(graph.vertices);

        foreach(GraphEdge edge in graph.edges)
        {
            newGraph.AddEdge(edge);
            if (ContainsCycle(newGraph)) newGraph.RemoveEdge(edge);
        }

        return newGraph;
    }

    public static int CalculateShortestPathLength(Vector2Int startVertex, Vector2Int endVertex, List<Room> rooms)
    {
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        Dictionary<Vector2Int, int> distance = new Dictionary<Vector2Int, int>();

        queue.Enqueue(startVertex);
        visited.Add(startVertex);
        distance[startVertex] = 0;

        while (queue.Count > 0)
        {
            Vector2Int currentVertex = queue.Dequeue();

            if (currentVertex == endVertex)
            {
                // Znaleziono docelowy wierzcho³ek, zwracamy jego odleg³oœæ.
                return distance[currentVertex];
            }

            foreach (Room room in rooms)
            {
                if (room.center == currentVertex)
                {
                    foreach (Room connectedRoom in room.connectedRooms)
                    {
                        Vector2Int neighborVertex = connectedRoom.center;
                        if (!visited.Contains(neighborVertex))
                        {
                            queue.Enqueue(neighborVertex);
                            visited.Add(neighborVertex);
                            distance[neighborVertex] = distance[currentVertex] + 1;
                        }
                    }
                }
            }
        }

        // Nie znaleziono œcie¿ki, zwracamy -1 lub inn¹ wartoœæ oznaczaj¹c¹ brak po³¹czenia.
        return -1;
    }
}


public class GraphEdge : IEquatable<GraphEdge>, IComparable<GraphEdge>
{
    public Vector2Int src;
    public Vector2Int destination;
    public int weight;

    public GraphEdge(Vector2Int point1, Vector2Int point2)
    {
        this.src = point1;
        this.destination = point2;
        weight = (int)Vector2Int.Distance(point1, point2);
    }

    public int CompareTo(GraphEdge other)
    {
        return weight - other.weight;
    }

    public bool Equals(GraphEdge edge)
    {
        return edge.src == src && edge.destination == destination || edge.destination == src && edge.src == destination;
    }
}


public class Graph
{
    public int verticesCount { get { return vertices.Count; } }
    public int edgesCount { get { return edges.Count; } }
    public List<GraphEdge> edges = new List<GraphEdge>();
    public List<Vector2Int> vertices = new List<Vector2Int>();

    public Graph(List<GraphEdge> edges)
    {
        foreach(GraphEdge edge in edges)
        {
            if (!this.edges.Contains(edge))this.edges.Add(edge);
        }

        foreach(GraphEdge edge in this.edges)
        {
            if(!this.vertices.Contains(edge.src))this.vertices.Add(edge.src);
            if (!this.vertices.Contains(edge.destination)) this.vertices.Add(edge.destination);
        }

        this.edges.Sort();
        
    }


    public Graph(List<Vector2Int> vertices)
    {
      
       foreach(Vector2Int vertex in vertices)
        {
            this.vertices.Add(vertex);
        }

    }

    public void AddEdge(GraphEdge graphEdge)
    {
        this.edges.Add(graphEdge);
    }

    public void RemoveEdge(GraphEdge graphEdge)
    {
        this.edges.Remove(graphEdge);
    }
}