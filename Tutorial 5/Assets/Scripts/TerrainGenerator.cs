﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public int Width;
    public int Depth;
    [Header("Vertex visualization")]
    public GameObject VertexObject;
    public bool VisualizeVertices;

    private Vector3[] vertices;
    private int[] trianglePoints;
    Vector2[] uvs;
    private Mesh mesh;
    private MeshFilter meshFilter;
    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        mesh.name = "Procedural Terrain";
        meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        CreateMesh();
        UpdateMesh();
        if (VisualizeVertices)
        {
            DrawVertices();
        }
    }

    private void DrawVertices()
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            Instantiate(VertexObject, vertices[i], Quaternion.identity, transform);
        }
    }

    private void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = trianglePoints;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
    }

    void CreateMesh()
    {
        //Vertices
        vertices = new Vector3[(Width + 1) * (Depth + 1)];

        int i = 0;
        for (int z = 0; z <= Depth; z++)
        {
            for (int x = 0; x <= Width; x++)
            {
                float y = Mathf.PerlinNoise(x * 0.4f, z * 0.4f) * 3f;
                vertices[i] = new Vector3(x, y, z);
                i++;
            }
        }

        //Triangles
        trianglePoints = new int[Width * Depth * 6];
        int currentTrianglePoint = 0;
        int currentVertexPoint = 0;

        for (int z = 0; z < Depth; z++)
        {
            for (int x = 0; x < Width; x++)
            {
                trianglePoints[currentTrianglePoint + 0] = currentVertexPoint + 0;
                trianglePoints[currentTrianglePoint + 1] = currentVertexPoint + Width + 1;
                trianglePoints[currentTrianglePoint + 2] = currentVertexPoint + 1;
                trianglePoints[currentTrianglePoint + 3] = currentVertexPoint + 1;
                trianglePoints[currentTrianglePoint + 4] = currentVertexPoint + Width + 1;
                trianglePoints[currentTrianglePoint + 5] = currentVertexPoint + Width + 2;

                currentVertexPoint++;
                currentTrianglePoint += 6;
            }
            currentVertexPoint++;
        }

        //UVs
        uvs = new Vector2[vertices.Length];
        i = 0;
        for (int z = 0; z <= Depth; z++)
        {
            for (int x = 0; x <= Width; x++)
            {
                uvs[i] = new Vector2((float)x / Width, (float)z / Depth);
                i++;
            }
        }
    }
}
