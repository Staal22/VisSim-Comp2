using System;
using System.IO;
using UnityEngine;

public static class Utilities
{
    public static string ReadUntil(StreamReader reader, char terminator)
    {
        var text = "";
        while (!reader.EndOfStream)
        {
            var nextChar = (char)reader.Peek(); // Peek next char, don't advance
            if (nextChar == terminator)
            {
                reader.Read(); // Consume the terminator before breaking
                break;
            }
            text += (char)reader.Read(); // Now read, advancing position
        }

        return text.Trim();
    }

    public static Vector3 ParseVector3(string text)
    {
        string[] parts = text.Trim('(', ')').Split(',');
        return new Vector3(float.Parse(parts[0]), float.Parse(parts[1]), float.Parse(parts[2]));
    }

    public static Vector2 ParseVector2(string text)
    {
        string[] parts = text.Trim('(', ')').Split(',');
        return new Vector2(float.Parse(parts[0]), float.Parse(parts[1]));
    }
    
    // public static float ReadNextFloat(StreamReader reader)
    // {
    //     string text = ReadUntil(reader, ',');
    //     if (!float.TryParse(text, out float result))
    //     {
    //         throw new FormatException($"Unable to parse '{text}' as a float.");
    //     }
    //     return result;
    // }

    // public static Vector3 ReadNextVector3(StreamReader reader)
    // {
    //     try
    //     {
    //         reader.Read();  // Skip '(' before reading a vector3
    //         Vector3 vector = new Vector3(ReadNextFloat(reader), ReadNextFloat(reader), ReadNextFloat(reader));
    //         reader.Read();  // Reads the ')' after reading a vector3
    //         reader.Read();  // Reads the ' ' whitespace after reading a vector3
    //         return vector;
    //     }
    //     catch (FormatException fe)
    //     {
    //         throw new FormatException($"Failed to parse a Vector3: {fe.Message}");
    //     }
    // }
}