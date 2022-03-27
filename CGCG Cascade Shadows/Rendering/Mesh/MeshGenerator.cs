using SharpDX;
using SharpDX.Direct3D;

using System;
using System.Collections.Generic;
using System.Linq;

namespace FR.CascadeShadows.Rendering.Meshes;

public static class MeshGenerator
{
    public static Mesh GenerateSphere(int rings, int segments)
    {
        static Vector3[] GenerateSphereNormals(int rings, int segments)
        {
            // 2 .. VERTICES (top and bottom singular)
            // (rings - 2) * segments .. VERTICES (body)
            int vertexCount = 2 + (rings - 2) * segments;

            int idx = 0;
            Vector3[] normals = new Vector3[vertexCount];

            // Top vertex
            normals[idx++] = Vector3.Up;

            // Body
            //float anglePerSegment = 2 * Math.PI / segments
            for (int y = 1; y <= rings - 2; y++)
            {
                float vecY = (float)Math.Cos(Math.PI * y / (rings - 1)); // in 0: y / (rings - 1) = 0; in last ring: y / (rings - 1) = 1;
                float remains = (float)Math.Sqrt(1 - vecY * vecY);
                for (int k = 0; k < segments; k++)
                {
                    float vecX = -(float)Math.Cos(2 * Math.PI * k / (segments - 1)) * remains;
                    float vecZ = (float)Math.Sin(2 * Math.PI * k / (segments - 1)) * remains;
                    normals[idx++] = new Vector3(vecX, vecY, vecZ);
                }
            }

            // Bottom vertex
            normals[idx++] = Vector3.Down;

            //Debug.Assert(vertices.Length == idx, "Sphere generated wrong amount of vertices!");
            return normals;
        }

        static Vector3[] GenerateSpherePositions(int rings, int segments, float radius)
        {
            // 2 .. VERTICES (top and bottom singular)
            // (rings - 2) * segments .. VERTICES (body)
            int vertexCount = 2 + (rings - 2) * segments;

            int idx = 0;
            Vector3[] vertices = new Vector3[vertexCount];

            // Top vertex
            vertices[idx++] = Vector3.Up * radius;

            // Body
            //float anglePerSegment = 2 * Math.PI / segments
            for (int y = 1; y <= rings - 2; y++)
            {
                float vecY = (float)Math.Cos(Math.PI * y / (rings - 1)); // in 0: y / (rings - 1) = 0; in last ring: y / (rings - 1) = 1;
                float remains = (float)Math.Sqrt(1 - vecY * vecY);
                for (int k = 0; k < segments; k++)
                {
                    float vecX = -(float)Math.Cos(2 * Math.PI * k / (segments - 1)) * remains;
                    float vecZ = (float)Math.Sin(2 * Math.PI * k / (segments - 1)) * remains;
                    vertices[idx++] = new Vector3(vecX, vecY, vecZ) * radius;
                }
            }

            // Bottom vertex
            vertices[idx++] = Vector3.Down * radius;

            //Debug.Assert(vertices.Length == idx, "Sphere generated wrong amount of vertices!");
            return vertices;
        }

        static uint[] GenerateSphereIndices(int rings, int segments)
        {
            // #segment * 2 .. TRIANGLES (at the top and bottom)
            // #rings - 3 = #squareRings (rings - top ring - bottom ring - 1 ring that ends the last squareRing)
            // #squareRings * #segments * 2 .. TRIANGLES (each ring has #segment squares; each square has 2 triangles)
            int idxCount = ((rings - 2) * segments * 2) * 3; // == (2 * segments + (rings - 3) * segments * 2) * 3;
                                                             //idxCount =
                                                             //    ((rings - 2) * segments * 2) * 3 // rings - 2 = stripes; segments * 2 = triangles per stripe; *3 because 3 indices per triangle
                                                             //    + segments * 2 * 3; // top and bottom copula

            int idx = 0;
            uint[] indices = new uint[idxCount];

            uint A(int y, int x) // Return vertex index of x'th segment on current (y'th) ring
                => (uint)(1          // Top vertex
                + (y - 1) * segments // + each passed ring (except for top 1-element one)
                + (x % segments));   // + segment (wrapping around)

            // Top copula
            for (int k = 0; k < segments; k++)
            {
                indices[idx++] = 0;
                indices[idx++] = A(1, k);
                indices[idx++] = A(1, k + 1);
            }

            // Horizontal stripes
            for (int y = 1; y <= rings - 3; y++) // rings - 1 = last = single vertex
                for (int k = 0; k < segments; k++)
                {
                    // Top triangle
                    indices[idx++] = A(y, k);
                    indices[idx++] = A(y + 1, k + 1);
                    indices[idx++] = A(y, k + 1);

                    // Bottom triangle
                    indices[idx++] = A(y, k);
                    indices[idx++] = A(y + 1, k);
                    indices[idx++] = A(y + 1, k + 1);
                }

            // Bottom copula
            for (int k = 0; k < segments; k++)
            {
                indices[idx++] = A(rings - 2, k);
                indices[idx++] = A(rings - 2, k - 1);
                indices[idx++] = A(rings - 1, 0); // Bottom single vertex
            }

            //Debug.Assert(indices.Length == idx, "Sphere generated wrong amount of indices!");
            return indices;
        }

        return new()
        {
            Indices = GenerateSphereIndices(rings, segments),
            Positions = GenerateSpherePositions(rings, segments, 1),
            Normals = GenerateSphereNormals(rings, segments),
        };
    }

    public static Mesh GenerateQuad()
        => new()
        {
            Positions = new Vector3[]
            {
                new(-1, +1, 0),
                new(-1, -1, 0),
                new(+1, +1, 0),
                new(+1, -1, 0)
            },
            Normals = new Vector3[]
            {
                new(0, 0, 1),
                new(0, 0, 1),
                new(0, 0, 1),
                new(0, 0, 1),
            },
            Indices = new uint[] { 0, 1, 2, 2, 1, 3 }
        };
}