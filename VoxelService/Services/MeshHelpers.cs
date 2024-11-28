using System.Numerics;
using SharpGLTF.Geometry;
using SharpGLTF.Geometry.VertexTypes;
using SharpGLTF.Materials;
using SharpGLTF.Scenes;
using SharpGLTF.Schema2;
using VoxelService.Data;

namespace VoxelService.Services;

public static class MeshHelpers
{
    public static List<Triangle> ExtractTrianglesFromModel(ModelRoot model)
    {
        var triangles = new List<Triangle>();

        foreach (var mesh in model.LogicalMeshes)
        {
            foreach (var primitive in mesh.Primitives)
            {
                var vertices = primitive.VertexAccessors["POSITION"].AsVector3Array();
                var indices = primitive.IndexAccessor.AsIndicesArray();

                for (var i = 0; i < indices.Count; i += 3)
                {
                    triangles.Add(new Triangle(
                        vertices[(int)indices[i]],
                        vertices[(int)indices[i + 1]],
                        vertices[(int)indices[i + 2]]
                    ));
                }
            }
        }

        return triangles;
    }

    public static void BuildVoxelScene(
        SceneBuilder sceneBuilder,
        HashSet<Voxel> voxels, 
        float voxelSize, 
        float scale
    )
    {
        // Create material for the cubes
        var material = new MaterialBuilder()
            .WithDoubleSide(true)
            .WithMetallicRoughnessShader()
            .WithChannelParam(KnownChannel.BaseColor, KnownProperty.RGBA, new Vector4(0.8f, 0.1f, 0.1f, 1f));

        // Create a mesh builder for the GLB
        var mesh = new MeshBuilder<VertexPosition>();

        // Add a cube for each voxel
        foreach (var voxel in voxels)
        {
            var voxelCenter = new Vector3(
                voxel.X * voxelSize + voxelSize / 2,
                voxel.Y * voxelSize + voxelSize / 2,
                voxel.Z * voxelSize + voxelSize / 2
            );

            AddCubeToMesh(mesh, voxelCenter, voxelSize, scale, material);
        }

        // Create a scene and add the voxel mesh
        sceneBuilder.AddRigidMesh(mesh, Matrix4x4.Identity);
    }

    public static void AddCubeToMesh(
        MeshBuilder<VertexPosition> meshBuilder,
        Vector3 center,
        float size,
        float scale,
        MaterialBuilder material)
    {
        float scaledSize = size * scale;
        float halfSize = scaledSize / 2;

        // Define cube vertices
        Vector3[] vertices =
        [
            new(center.X - halfSize, center.Y - halfSize, center.Z - halfSize),
            new(center.X + halfSize, center.Y - halfSize, center.Z - halfSize),
            new(center.X + halfSize, center.Y + halfSize, center.Z - halfSize),
            new(center.X - halfSize, center.Y + halfSize, center.Z - halfSize),

            new(center.X - halfSize, center.Y - halfSize, center.Z + halfSize),
            new(center.X + halfSize, center.Y - halfSize, center.Z + halfSize),
            new(center.X + halfSize, center.Y + halfSize, center.Z + halfSize),
            new(center.X - halfSize, center.Y + halfSize, center.Z + halfSize)
        ];

        // Define cube faces (indices)
        int[][] faces =
        [
            [0, 1, 2, 3], // Front
            [4, 5, 6, 7], // Back
            [0, 1, 5, 4], // Bottom
            [2, 3, 7, 6], // Top
            [0, 3, 7, 4], // Left
            [1, 2, 6, 5] // Right
        ];

        foreach (var face in faces)
        {
            meshBuilder.UsePrimitive(material)
                .AddQuadrangle(
                    new VertexPosition(vertices[face[0]]),
                    new VertexPosition(vertices[face[1]]),
                    new VertexPosition(vertices[face[2]]),
                    new VertexPosition(vertices[face[3]])
                );
        }
    }
}