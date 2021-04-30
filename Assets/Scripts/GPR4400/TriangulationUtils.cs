using System.Collections.Generic;
using UnityEngine;

namespace path
{
    public struct Vertex
    {
        public Vector2 position;
        public int index;

        public Vertex(Vector2 position)
        {
            this.position = position;
            index = -1;
        }

        public Vertex(Vector2 position, int index)
        {
            this.position = position;
            this.index = index;
        }

        public static bool operator ==(Vertex v1, Vertex v2)
        {
            if (v1.index != v2.index)
                return false;
            if (v1.index == v2.index && v1.index != -1)
                return true;
            return Mathf.Approximately(v1.position.x, v2.position.x) &&
                   Mathf.Approximately(v1.position.y, v2.position.y);
        }

        public static bool operator !=(Vertex v1, Vertex v2)
        {
            return !(v1 == v2);
        }
    }

    public struct Edge
    {
        public Vertex a;
        public Vertex b;
        public bool isBad;

        public Edge(Vertex a, Vertex b)
        {
            this.a = a;
            this.b = b;
            isBad = false;
        }

        public static bool operator ==(Edge e1, Edge e2)
        {
            return (e1.a == e2.a && e1.b == e2.b) || (e1.a == e2.b && e1.b == e2.a) ;
        }

        public static bool operator !=(Edge e1, Edge e2)
        {
            return !(e1 == e2);
        }
    }
    public struct Triangle
    {
        
        public Vertex a;
        public Vertex b;
        public Vertex c;
        public bool isBad;

        public Triangle(Vertex a, Vertex b, Vertex c)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            isBad = false;
        }

        public bool CircumCircleContains(Vertex v)
        {
            var ab = a.position.sqrMagnitude;
            var cd = b.position.sqrMagnitude;
            var ef = c.position.sqrMagnitude;

            float ax = a.position.x;
            float ay = a.position.y;
            float bx = b.position.x;
            float by = b.position.y;
            float cx = c.position.x;
            float cy = c.position.y;
            
            var circumX = (ab * (cy - by) + cd * (ay - cy) + ef * (by - ay)) / (ax * (cy - by) + bx * (ay - cy) + cx * (by - ay));
            var circumY = (ab * (cx - bx) + cd * (ax - cx) + ef * (bx - ax)) / (ay * (cx - bx) + by * (ax - cx) + cy * (bx - ax));
            Vector2 circum = new Vector2(circumX / 2.0f, circumY / 2.0f);
            var radius2 = (circum - a.position).sqrMagnitude;
            var dist = (circum - v.position).sqrMagnitude;
            return dist <= radius2;
        }
        
    }

    public static class Delaunay
    {
        public static List<Edge> Triangulate(in List<Vector2> points)
        {
            const float deltaMaxFactor = 2.0f;
            var edges = new List<Edge>();
            var pointCount = points.Count;
            if (pointCount < 3)
            {
                return edges;
            }
            var triangles = new List<Triangle>();
            Vector2[] bigPoints = new Vector2[3];
            Vector2 minPoint = points[0];
            Vector2 maxPoint = minPoint;

            foreach (var point in points)
            {
                if (minPoint.x > point.x)
                {
                    minPoint.x = point.x;
                }
                if (minPoint.y > point.y)
                {
                    minPoint.y = point.y;
                }
                if (maxPoint.x < point.x)
                {
                    maxPoint.x = point.x;
                }
                if (maxPoint.y < point.y)
                {
                    maxPoint.y = point.y;
                }
            }

            var delta = maxPoint - minPoint;
            float deltaMax = delta.x > delta.y ? delta.x : delta.y;
            var mid = (minPoint + maxPoint) / 2.0f;

            bigPoints[0] = new Vector2(mid.x - deltaMaxFactor*deltaMax, mid.y - deltaMaxFactor*deltaMax);
            bigPoints[1] = new Vector2(mid.x, mid.y + deltaMaxFactor*deltaMax);
            bigPoints[2] = new Vector2(mid.x + deltaMaxFactor*deltaMax, mid.y - deltaMaxFactor*deltaMax);
            
            triangles.Add(new Triangle(
                new Vertex(bigPoints[0]),
                new Vertex(bigPoints[1]),
                new Vertex(bigPoints[2])
                ));
            for (int pointIndex = 0; pointIndex < pointCount; pointIndex++)
            {
                var point = points[pointIndex];
                var vertex = new Vertex(point, pointIndex);
                List<Edge> polygon = new List<Edge>();

                for (var triangleIndex = 0; triangleIndex < triangles.Count; triangleIndex++)
                {
                    var triangle = triangles[triangleIndex];
                    if (triangle.CircumCircleContains(vertex))
                    {
                        triangle.isBad = true;
                        triangles[triangleIndex] = triangle;
                        polygon.Add(new Edge(triangle.a, triangle.b));
                        polygon.Add(new Edge(triangle.b, triangle.c));
                        polygon.Add(new Edge(triangle.c, triangle.a));
                    }
                }

                triangles.RemoveAll(t => t.isBad);
                for (int edgeIndex = 0; edgeIndex < polygon.Count; edgeIndex++)
                {
                    var edge = polygon[edgeIndex];
                    for (int otherEdgeIndex = edgeIndex + 1; otherEdgeIndex < polygon.Count; otherEdgeIndex++)
                    {
                        var otherEdge = polygon[otherEdgeIndex];
                        if (polygon[edgeIndex] != polygon[otherEdgeIndex]) continue;
                        edge.isBad = true;
                        polygon[edgeIndex] = edge;
                        otherEdge.isBad = true;
                        polygon[otherEdgeIndex] = otherEdge;
                    }
                }

                polygon.RemoveAll(p => p.isBad);
                foreach (var edge in polygon)
                {
                    triangles.Add(new Triangle(edge.a, edge.b, vertex));
                }
            }

            triangles.RemoveAll(t => t.a.index == -1 || t.b.index == -1 || t.c.index == -1);
            foreach (var triangle in triangles)
            {
                edges.Add(new Edge(triangle.a, triangle.b));
                edges.Add(new Edge(triangle.b, triangle.c));
                edges.Add(new Edge(triangle.c, triangle.a));
            }
            return edges;
        }
    }
}