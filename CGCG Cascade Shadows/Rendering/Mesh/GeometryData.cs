using SharpDX;
using SharpDX.Direct3D11;

using System;
using System.Linq;

using Buffer = SharpDX.Direct3D11.Buffer;

namespace FR.CascadeShadows.Rendering.Meshes;

public class GeometryData
{
    public string Name { get; protected set; }

    // Vertex properties
    public VertexBufferBinding? Positions { get; protected set; }
    public VertexBufferBinding? Normals { get; protected set; }
    public VertexBufferBinding[] TexCoordChannels { get; protected set; } = Array.Empty<VertexBufferBinding>();
    public VertexBufferBinding[] ColorChannels { get; protected set; } = Array.Empty<VertexBufferBinding>();
    public VertexBufferBinding? Tangents { get; protected set; }
    public VertexBufferBinding? Binormals { get; protected set; }
    public VertexBufferBinding? BoneWeights { get; protected set; } // Not implemented yet

    // Indices
    public Buffer? Indices { get; protected set; }

    // Counts
    public int VertexCount { get; protected set; } = 0;
    public int IndexCount { get; protected set; } = 0;
    public int TexCoordChannelCount => TexCoordChannels.Length;
    public int ColorChannelCount => ColorChannels.Length;

    // Useful shortcuts
    public bool HasPositions => Positions != null;
    public bool HasNormals => Normals != null;
    public bool HasTexCoords => TexCoordChannelCount > 0;
    public bool HasColors => ColorChannelCount > 0;
    public bool HasTangentBasis => Tangents != null;
    public bool HasBoneWeights => BoneWeights != null;
    public bool HasVertices => VertexCount > 0;

    public event Action? Updated;

    // Probably will turn to Buffer (constant buffer)
    public Matrix[]? boneOffsets; // Not implemented

    public OrientedBoundingBox? BoundingBox;
    public bool HasBoundingBox => BoundingBox != null;

    public GeometryData(Mesh mesh)
        => Update(mesh);

    public void Update(Mesh mesh)
    {
        static VertexBufferBinding MakeVbb<T>(T[] ts) where T : struct
            => new(Buffer.Create(Devices.Device3D, BindFlags.VertexBuffer, ts), SharpDX.Utilities.SizeOf<T>(), 0);

        Name = mesh.Name;

        VertexCount = mesh.VertexCount;
        IndexCount = mesh.IndexCount;
        if (mesh.HasPositions)
            Positions = MakeVbb(mesh.Positions!);
        if (mesh.HasNormals)
            Normals = MakeVbb(mesh.Normals!);

        TexCoordChannels = new VertexBufferBinding[mesh.TexCoordChannelCount];
        for (int i = 0; i < TexCoordChannelCount; i++)
            TexCoordChannels[i] = MakeVbb(mesh.TexCoordChannels[i]);

        ColorChannels = new VertexBufferBinding[mesh.ColorChannelCount];
        for (int i = 0; i < ColorChannelCount; i++)
            ColorChannels[i] = MakeVbb(mesh.ColorChannels[i].Select(x => x.ToColor4()).ToArray());

        if (mesh.HasTangentBasis)
        {
            Tangents = MakeVbb(mesh.TangentBasis!.Select(b => b.Tangent).ToArray());
            Binormals = MakeVbb(mesh.TangentBasis!.Select(b => b.BiTangent).ToArray());
        }

        if (mesh.HasBoneWeights)
            BoneWeights = MakeVbb(mesh.BoneWeights!);

        Indices = Buffer.Create(Devices.Device3D, BindFlags.IndexBuffer, mesh.Indices);

        if (HasVertices)
        {
            Vector3 min = mesh.Positions![0];
            Vector3 max = min;

            foreach (var pos in mesh.Positions)
            {
                min = Vector3.Min(min, pos);
                max = Vector3.Max(max, pos);
            }

            BoundingBox = new OrientedBoundingBox(min, max);
        }

        Updated?.Invoke();
    }
}
