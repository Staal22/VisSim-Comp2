using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class TriangleSurface : VisualObject
{
    // file to read from
    [SerializeField] private TextAsset file;
    private MeshFilter _meshFilter;

    private void Awake()
    {
        _meshFilter = GetComponent<MeshFilter>();
    }

    private void Start()
    {
        using var stream = new StreamReader(new MemoryStream(file.bytes));
        using var reader = new StreamReader(stream.BaseStream);
        GeometryFromStream(reader);
    }


    public TriangleSurface(Vertex[] vertices, int[] indices) : base(vertices, indices)
    {
    }
    
    public void GeometryFromStream(StreamReader reader)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }
        int numIndices = int.Parse(Utilities.ReadUntil(reader, '\n').Trim());
        int numVertices = int.Parse(Utilities.ReadUntil(reader, '\n').Trim());

        List<int> indices = new List<int>();
        for (int i = 0; i < numIndices; i++)
        {
            indices.Add(int.Parse(Utilities.ReadUntil(reader, '\n').Trim()));
        }

        List<Vertex> vertices = new List<Vertex>();
        for (int i = 0; i < numVertices; i++)
        {
            vertices.Add(Vertex.LoadFromFile(reader));
        }

        List<int> neighbours = new List<int>();
        for (int i = 0; i < numIndices; i++)
        {
            neighbours.Add(int.Parse(Utilities.ReadUntil(reader, '\n').Trim()));
        }

        reader.Close();
        
        // If there is no meshFilter component attached to the gameObject, add one.
        if (_meshFilter == null)
        {
            _meshFilter = gameObject.AddComponent<MeshFilter>();
        }
        
        Mesh newMesh = new Mesh();

        newMesh.vertices = vertices.Select(v => v.Position).ToArray();
        newMesh.triangles = indices.ToArray();

        // Re-calculate normals for correct shading
        newMesh.RecalculateNormals();
        newMesh.RecalculateBounds();
        
        if (_meshFilter.mesh != null)
        {
            _meshFilter.mesh.Clear();
        }
        _meshFilter.mesh = newMesh;
    }
}
