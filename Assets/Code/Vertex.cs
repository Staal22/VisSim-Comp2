using System;
using System.IO;
using UnityEngine;

public class Vertex
{
    public Vector3 Position { get; private set; }
    public Vector3 Normal { get; private set; }
    public Vector2 Uv { get; private set; }
    

    public Vertex(Vector3 pos, Vector3 norm, Vector2 uv)
    {
        Position = pos;
        Normal = norm;
        Uv = uv;
    }

    public Vertex(Vector3 pos)
    {
        Position = pos;
    }

    public static Vertex LoadFromFile(StreamReader reader)
    {
        string line = reader.ReadLine();
        // Split the source string at ') (' to get separate vectors
        string[] parts = line.Trim('(', ')').Split(new string[] { ") (" }, StringSplitOptions.None);

        if (parts.Length != 3)
        {
            throw new FormatException($"The line '{line}' is invalid. It should contain exactly 2 Vector3s and 1 Vector2.");
        }

        // Parse the vectors, now that they are properly separated
        var position = Utilities.ParseVector3(parts[0]);
        var normal = Utilities.ParseVector3(parts[1]);
        var uv = Utilities.ParseVector2(parts[2]);

        return new Vertex(position, normal, uv);
    }
    
    public override string ToString()
    {
        return $"({Position.x}, {Position.y}, {Position.z}) " +
               $"({Normal.x}, {Normal.y}, {Normal.z}) " +
               $"({Uv.x}, {Uv.y})";
    }
}