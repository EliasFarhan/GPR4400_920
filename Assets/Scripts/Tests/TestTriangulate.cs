using System.Linq;
using NUnit.Framework;
using path;
using UnityEngine;

public class TriangulateTest
{
    [Test]
    public void TriangleTest()
    {
        Graph graph = new Graph();
        graph.AddNode(new Vector2(0, 0));
        graph.AddNode(new Vector2(1, 1));
        graph.AddNode(new Vector2(2, 0));
        var vertices = graph.Nodes.Select(node => node.position).ToList();
        var edges = Delaunay.Triangulate(vertices);
        foreach (var edge in edges)
        {
            graph.AddNeighborEdge(edge.a.index, edge.b.index, bidirectional:true);
        }
        Assert.True(graph.Nodes[0].neighbors.Any(n => n.nodeIndex == 1));
        Assert.True(graph.Nodes[0].neighbors.Any(n => n.nodeIndex == 2));
        Assert.True(graph.Nodes[1].neighbors.Any(n => n.nodeIndex == 0));
        Assert.True(graph.Nodes[1].neighbors.Any(n => n.nodeIndex == 2));
        Assert.True(graph.Nodes[2].neighbors.Any(n => n.nodeIndex == 0));
        Assert.True(graph.Nodes[2].neighbors.Any(n => n.nodeIndex == 1));
    }
    
    [Test]
    public void DiamondTest()
    {
        Graph graph = new Graph();
        graph.AddNode(new Vector2(1, 1));
        graph.AddNode(new Vector2(4, 4));
        graph.AddNode(new Vector2(5, 0));
        graph.AddNode(new Vector2(0, 5));
        var vertices = graph.Nodes.Select(node => node.position).ToList();
        var edges = Delaunay.Triangulate(vertices);
        foreach (var edge in edges)
        {
            graph.AddNeighborEdge(edge.a.index, edge.b.index, bidirectional:true);
        }
        Assert.True(graph.Nodes[0].neighbors.Any(n => n.nodeIndex == 1));
        Assert.True(graph.Nodes[0].neighbors.Any(n => n.nodeIndex == 2));
        Assert.True(graph.Nodes[0].neighbors.Any(n => n.nodeIndex == 3));
        Assert.True(graph.Nodes[1].neighbors.Any(n => n.nodeIndex == 0));
        Assert.True(graph.Nodes[1].neighbors.Any(n => n.nodeIndex == 2));
        Assert.True(graph.Nodes[1].neighbors.Any(n => n.nodeIndex == 3));
        Assert.True(graph.Nodes[2].neighbors.Any(n => n.nodeIndex == 0));
        Assert.True(graph.Nodes[2].neighbors.Any(n => n.nodeIndex == 1));
        Assert.True(graph.Nodes[3].neighbors.Any(n => n.nodeIndex == 0));
        Assert.True(graph.Nodes[3].neighbors.Any(n => n.nodeIndex == 1));
    }
}