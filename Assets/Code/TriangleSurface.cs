using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class TriangleSurface : VisualObject
{
    // file to read from
    [SerializeField] private TextAsset file;
    private MeshFilter _meshFilter;
    private MeshCollider _meshCollider;

    private void Awake()
    {
        _meshFilter = GetComponent<MeshFilter>();
        _meshCollider = GetComponent<MeshCollider>();
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
        
        Mesh newMesh = new Mesh();

        newMesh.vertices = vertices.Select(v => v.Position).ToArray();;
        newMesh.triangles = indices.ToArray();
        
        // Re-calculate normals and bounds
        newMesh.RecalculateNormals();
        newMesh.RecalculateBounds();

        Destroy(_meshCollider);
        List<MeshCollider> colliders = new List<MeshCollider>();
        for (int i = 0; i < newMesh.triangles.Length; i += 3)
        {
            Vector3[] triangleVertices = new Vector3[3]
            {
                newMesh.vertices[newMesh.triangles[i]],
                newMesh.vertices[newMesh.triangles[i + 1]],
                newMesh.vertices[newMesh.triangles[i + 2]]
            };

            Mesh triangleMesh = new Mesh();
            triangleMesh.vertices = triangleVertices;
            triangleMesh.triangles = new int[] { 0, 1, 2 };

            MeshCollider triangleCollider = gameObject.AddComponent<MeshCollider>();
            triangleCollider.sharedMesh = triangleMesh;

            colliders.Add(triangleCollider);
        }

        
        
        _meshFilter.mesh = newMesh;
        // if (_meshCollider != null) 
        // {
        //     _meshCollider.sharedMesh = newMesh;
        // }
        
    }
}
