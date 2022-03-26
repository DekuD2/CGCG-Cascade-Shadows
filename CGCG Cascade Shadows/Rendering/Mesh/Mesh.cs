using SharpDX;

using System.Collections.Generic;
using System.Linq;

namespace FR.CascadeShadows.Rendering.Meshes;

public class Mesh
{
    // Mesh properties
    public string Name { get; set; } = "";

    private GeometryData? geometryData;
    public GeometryData GeometryData => geometryData ??= new GeometryData(this);

    public void UpdateGeometryData() => geometryData?.Update(this);

    // Vertex properties
    public Vector3[]? Positions { get; set; }
    public Vector3[]? Normals { get; set; }
    public Vector2[][] TexCoordChannels { get; set; } = System.Array.Empty<Vector2[]>();
    public Color[][] ColorChannels { get; set; } = System.Array.Empty<Color[]>();
    public TangentBase[]? TangentBasis { get; set; }
    public Vector4[]? BoneWeights { get; set; } // Not implemented

    // Indices
    public uint[]? Indices { get; set; }

    // Counts
    public int VertexCount => Positions?.Length ?? 0;
    public int IndexCount => Indices?.Length ?? 0;
    public int TexCoordChannelCount => TexCoordChannels.Length;
    public int ColorChannelCount => ColorChannels.Length;

    // Existential helper properties
    public bool HasPositions => Positions != null;
    public bool HasNormals => Normals != null;
    public bool HasTexCoords => TexCoordChannelCount > 0;
    public bool HasColors => ColorChannelCount > 0;
    public bool HasTangentBasis => TangentBasis != null;
    public bool HasBoneWeights => BoneWeights != null;

    public static Mesh FromAssimp(Assimp.Mesh mesh)
    {
        Mesh res = new();

        // Positions
        res.Positions = (from n in mesh.Vertices
                         select new Vector3(n.X, n.Y, n.Z))
                         .ToArray();
        // Normals
        if (mesh.HasNormals)
            res.Normals = (from n in mesh.Normals
                           select new Vector3(n.X, n.Y, n.Z))
                           .ToArray();
        // Texture channels
        res.TexCoordChannels = new Vector2[mesh.TextureCoordinateChannelCount][];
        for (int i = 0; i < res.TexCoordChannelCount; i++)
            res.TexCoordChannels[i] = (from uv in mesh.TextureCoordinateChannels[i]
                                       select new Vector2(uv.X, 1f - uv.Y))
                                       .ToArray();

        // Color channels
        res.ColorChannels = new Color[mesh.VertexColorChannelCount][];
        for (int i = 0; i < res.ColorChannelCount; i++)
            res.ColorChannels[i] = (from col in mesh.VertexColorChannels[i]
                                    select new Color(col.R, col.G, col.B, col.A))
                                    .ToArray();

        // Normal Complements
        if (mesh.HasTangentBasis)
            res.TangentBasis = (from idx in Enumerable.Range(0, res.Positions.Length)
                                select new TangentBase(tangent: new Vector3(mesh.Tangents[idx].X, mesh.Tangents[idx].Y, mesh.Tangents[idx].Z),
                                                       bitangent: new Vector3(mesh.BiTangents[idx].X, mesh.BiTangents[idx].Y, mesh.BiTangents[idx].Z)))
                                                       .ToArray();
        // Indices
        List<uint> idcs = new();
        foreach (var face in mesh.Faces)
        {
            for (int i = 2; i < face.IndexCount; i++)
            {
                idcs.Add((uint)face.Indices[0]);
                idcs.Add((uint)face.Indices[i - 1]);
                idcs.Add((uint)face.Indices[i]);
            }
            if (face.IndexCount > 3)
                System.Diagnostics.Debugger.Break();
        }
        res.Indices = idcs.ToArray();

        // Name
        res.Name = mesh.Name;
        Debug.WriteLine($"Mesh '{mesh.Name}' loaded; V/T = {mesh.VertexCount}/{mesh.FaceCount}");

        return res;
    }
}
