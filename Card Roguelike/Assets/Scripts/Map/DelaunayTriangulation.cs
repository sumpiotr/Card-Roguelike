using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DelaunayTriangulation
{
    public class Triangle 
    {
        public Vector2Int a;
        public Vector2Int b;
        public Vector2Int c;

        public Edge edge1;
        public Edge edge2;
        public Edge edge3;

        public Triangle(Vector2Int a, Vector2Int b, Vector2Int c)
        {
            this.a = a;
            this.b = b;
            this.c = c;

            edge1 = new Edge(a, b);
            edge2 = new Edge(a, c);
            edge3 = new Edge(b, c);
        }

        public bool ContainsEdge(Edge edge)
        {
            return edge1.Equals(edge) || edge2.Equals(edge) || edge3.Equals(edge);
        }

        public bool ContainsVertex(Vector2Int vertex)
        {
            return vertex == a || vertex == b || vertex == c;
        }
    }

    public class Edge 
    {
        public Vector2Int point1;
        public Vector2Int point2;

        public Edge(Vector2Int point1, Vector2Int point2)
        {
            this.point1 = point1;
            this.point2 = point2;
        }

        public bool Equals(Edge edge)
        {
            return edge.point1 == point1 && edge.point2 == point2 || edge.point2 == point1 && edge.point1 == point2;
        }
        
    }

    public static List<Triangle> Triangulation(List<Vector2Int> points)
    {
        
        Triangle superTriangle = new Triangle(new Vector2Int(-10000, -10000), new Vector2Int(10000, -10000), new Vector2Int(0, 10000));
        List<Triangle> triangles = new List<Triangle>();
        List<Triangle> badTriangles = new List<Triangle>();
        triangles.Add(superTriangle);

        foreach (Vector2Int point in points)
        {
            badTriangles.Clear();
            foreach (Triangle triangle in triangles)
            {
                if (!IsValidTriangle(triangle, point)) badTriangles.Add(triangle);
            }
            List<Edge> polygon = new List<Edge>();
            foreach (Triangle badTriangle in badTriangles)
            {
                bool unique1 = true;
                bool unique2 = true;
                bool unique3 = true;
                foreach (Triangle checkTriangle in badTriangles)
                {
                    if (checkTriangle == badTriangle) continue;
                    if (checkTriangle.ContainsEdge(badTriangle.edge1))
                    {
                        unique1 = false;
                    }
                    if (checkTriangle.ContainsEdge(badTriangle.edge2))
                    {
                        unique2 = false;
                    }
                    if (checkTriangle.ContainsEdge(badTriangle.edge3))
                    {
                        unique3 = false;
                    }
                }
                if (unique1) polygon.Add(badTriangle.edge1);
                if (unique2) polygon.Add(badTriangle.edge2);
                if (unique3) polygon.Add(badTriangle.edge3);
            }
            foreach (Triangle badTriangle in badTriangles)
            {
                triangles.Remove(badTriangle);
            }
            foreach(Edge edge in polygon)
            {
                Triangle newTriangle = new Triangle(edge.point1, edge.point2, point);
                triangles.Add(newTriangle);
            }
        }

        List<Triangle> toRemove = new List<Triangle>();
        foreach(Triangle triangle in triangles)
        {
            if(triangle.ContainsVertex(superTriangle.a) || triangle.ContainsVertex(superTriangle.b) || triangle.ContainsVertex(superTriangle.c))
            {
                toRemove.Add(triangle);
            }
        }

        Debug.Log(triangles.Count);

        foreach(Triangle triangle in toRemove)
        {
            triangles.Remove(triangle);
        }
        return triangles;
    }

    public static bool IsValidTriangle(Triangle triangle, Vector2Int point) 
    {

        float d = (triangle.a.x * (triangle.b.y - triangle.c.y) + triangle.b.x * (triangle.c.y - triangle.a.y) + triangle.c.x * (triangle.a.y - triangle.b.y)) * 2;
        float circleX = 1 / d * ((Mathf.Pow(triangle.a.x, 2) + Mathf.Pow(triangle.a.y, 2)) * (triangle.b.y - triangle.c.y) + (Mathf.Pow(triangle.b.x, 2) + Mathf.Pow(triangle.b.y, 2)) * (triangle.c.y - triangle.a.y) + (Mathf.Pow(triangle.c.x, 2) + Mathf.Pow(triangle.c.y, 2)) * (triangle.a.y - triangle.b.y));
        float circleY = 1 / d * ((Mathf.Pow(triangle.a.x, 2) + Mathf.Pow(triangle.a.y, 2)) * (triangle.c.x - triangle.b.x) + (Mathf.Pow(triangle.b.x, 2) + Mathf.Pow(triangle.b.y, 2)) * (triangle.a.x - triangle.c.x) + (Mathf.Pow(triangle.c.x, 2) + Mathf.Pow(triangle.c.y, 2)) * (triangle.b.x - triangle.a.x));
        Vector2 circleCenter = new Vector2(circleX, circleY);
        float radius = Vector2.Distance(triangle.a, circleCenter);
        float distance = Vector2.Distance(point, circleCenter);
        return distance > radius;
    }
}


