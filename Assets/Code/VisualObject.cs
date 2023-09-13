using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Don't use directly
public class VisualObject : MonoBehaviour
{
    public Vertex[] Vertices { get; }
    public int[] Indices { get; private set; }
    public int[] Neighbours { get; private set; }
    
    VisualObject()
    {
        Vertices = Array.Empty<Vertex>();
        Indices = Array.Empty<int>();
    }
    public VisualObject(Vertex[] vertices, int[] indices)
    {
        Vertices = vertices;
        Indices = indices;
    }
    
    // public void Draw()
    // {
    //     var mesh = new Mesh();
    //
    //     // Use vertices positions from Vertices array
    //     mesh.vertices = Vertices.Select(v => v.Position).ToArray();
    //
    //     // Use triangles indices from Indices array
    //     mesh.triangles = Indices;
    //
    //     mesh.RecalculateNormals();
    //     GetComponent<MeshFilter>().mesh = mesh;
    // }
}
