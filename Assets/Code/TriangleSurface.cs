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
    private List<Vector3[]> _triangles;
    
    private void Awake()
    {
        _meshFilter = GetComponent<MeshFilter>();
        _meshCollider = GetComponent<MeshCollider>();
        _triangles = new List<Vector3[]>();
    }
    private void Start()
    {
        using var stream = new StreamReader(new MemoryStream(file.bytes));
        using var reader = new StreamReader(stream.BaseStream);
        GeometryFromStream(reader);
    }
    
    private void GeometryFromStream(StreamReader reader)
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
        }
        
        // Color[] colors = new Color[newMesh.vertices.Length];
        // for (int i = 0; i < newMesh.vertices.Length; i++)
        // {
        //     colors[i] = Color.Lerp(Color.red, Color.green, newMesh.vertices[i].y);
        // }
        
        _meshFilter.mesh = newMesh;
        // _meshFilter.mesh.colors = colors;
        
        UpdateTriangles();
    }
    
    public int FindTriangle(Vector3 point)
    {
        if (_triangles.Count == 0)
            return -1;
        for (int i = 0; i < _triangles.Count; i++)
        {
            Vector3 barycentricCoordinates = Utilities.Barycentric(
                _triangles[i][0],
                _triangles[i][1],
                _triangles[i][2],
                point
            );
            if (Utilities.IsInsideTriangle(barycentricCoordinates))
            {
                return i; // return the index of the triangle the point is in
            }
        }
        return -1; // point is not within any triangle
    }

    private void UpdateTriangles()
    {
        Mesh mesh = _meshFilter.mesh;
        for (int i = 0; i < mesh.triangles.Length; i+=3)
        {
            _triangles.Add(new []{ mesh.vertices[mesh.triangles[i]], mesh.vertices[mesh.triangles[i+1]], mesh.vertices[mesh.triangles[i+2]]});
        }
    }
}
