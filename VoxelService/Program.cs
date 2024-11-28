using System.Diagnostics;
using System.Numerics;
using SharpGLTF.Scenes;
using SharpGLTF.Schema2;
using VoxelService.Data;
using VoxelService.Services;

if (args.Length < 1)
{
    Console.WriteLine("Usage: VoxelizerApp <path_to_glb_file>");
    return;
}

var glbFilePath = args[0];
const float voxelSizeExtraSmall = 0.5f; // Voxel size in meters
const float voxelSizeSmall = 1f; // Voxel size in meters
const float voxelSizeMedium = 2f; // Voxel size in meters

try
{
    var model = ModelRoot.Load(glbFilePath);

    var swVoxel = new Stopwatch();
    swVoxel.Start();
    var voxels = Voxelizer.VoxelizeModel(model, new VoxelizerConfiguration{VoxelSize = voxelSizeMedium});
    Console.WriteLine($"Voxelization: {swVoxel.ElapsedMilliseconds}ms");

    var swCulling = new Stopwatch();
    swCulling.Start();
    
    var cameraPos = new Vector3(1000, 0, 0);
    voxels = VoxelCuller.CullBlockedVoxels(
        voxels,
        cameraPos,
        -Vector3.Normalize(cameraPos),
        30,
        voxelSizeMedium
    );
    
    Console.WriteLine($"Culling: {swCulling.ElapsedMilliseconds}ms");
    
    // Output the result
    Console.WriteLine($"Voxelized Mesh into {voxels.Count} voxels with size {voxelSizeMedium} meters:");

    var sceneBuilder = new SceneBuilder();
    
    MeshHelpers.BuildVoxelScene(
        sceneBuilder,
        voxels,
        voxelSizeMedium,
        0.5f
    );
    
    var originalScene = model.DefaultScene;
    sceneBuilder.AddScene(originalScene.ToSceneBuilder(), Matrix4x4.Identity);
    
    // Save the GLB file
    sceneBuilder.ToGltf2().SaveGLB("output.glb");
}
catch (Exception ex)
{
    Console.WriteLine($"Error processing GLB file: {ex.Message}");
}