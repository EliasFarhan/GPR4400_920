using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using path;
using UnityEngine;
using UnityEngine.TestTools;

public class PathTest
{
    // A Test behaves as an ordinary method
    [Test]
    public void StraightLineTest()
    {
        // Use the Assert class to test conditions
        Graph graph = new Graph();
        graph.AddNode(new Vector2(0, 0));
        graph.AddNode(new Vector2(1, 0));
        graph.AddNode(new Vector2(2, 0));
        graph.AddNeighborEdge(0,1);
        graph.AddNeighborEdge(1,2);

        var p1 = graph.CalculatePath(0, 2);
        Assert.AreEqual(p1.Count, 3);
        Assert.AreEqual(p1[0], 0);
        Assert.AreEqual(p1[1], 1);
        Assert.AreEqual(p1[2], 2);
        var p2 = graph.CalculatePath(2, 0);
        Assert.AreEqual(p2.Count, 0);
    }

    [Test]
    public void TriangleTest()
    {
        // Use the Assert class to test conditions
        Graph graph = new Graph();
        graph.AddNode(new Vector2(0, 0));
        graph.AddNode(new Vector2(1, 1));
        graph.AddNode(new Vector2(2, 0));
        graph.AddNeighborEdge(0,1);
        graph.AddNeighborEdge(1,2);
        graph.AddNeighborEdge(0,2);
        var p1 = graph.CalculatePath(0, 2);
        Assert.AreEqual(p1.Count, 2);        
        Assert.AreEqual(p1[0], 0);
        Assert.AreEqual(p1[1], 2);
    }
}
