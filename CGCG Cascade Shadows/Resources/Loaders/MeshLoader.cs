
using Assimp;

using Mesh = FR.CascadeShadows.Rendering.Meshes.Mesh;

namespace FR.CascadeShadows.Resources.Loaders;

public class MeshLoader : FileLoader<Mesh>
{
    public override Mesh? Load(string uri)
    {
        //return Model.FromAssimpFile(uri).Meshes[0].meshData;

        using var importer = new AssimpContext();
        var scene = importer.ImportFile(uri,
            PostProcessSteps.Triangulate
            | PostProcessSteps.GenerateNormals
            | PostProcessSteps.CalculateTangentSpace
            | PostProcessSteps.OptimizeGraph
            | PostProcessSteps.OptimizeMeshes);

        if(scene.Meshes.Count != 0)
            Debug.WriteLine($"File '{uri}' has {scene.Meshes.Count} meshes.");

        return Mesh.FromAssimp(scene.Meshes[0]);
    }

}
