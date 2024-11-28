using System.Numerics;
using Microsoft.AspNetCore.Mvc;
using SharpGLTF.Scenes;
using SharpGLTF.Schema2;
using VoxelService.Api.Construct.Data;
using VoxelService.Api.Construct.Services;
using VoxelService.Api.Mesh.Interfaces;
using VoxelService.Data;
using VoxelService.Services;

namespace VoxelService.Api.Mesh.Controllers;

[Route("v1/voxel")]
public class VoxelController : Controller
{
    [HttpGet]
    [Route("stats")]
    public IActionResult GetStats()
    {
        return Ok(
            VoxelPool.GetCount()
        );
    }
}

[Route("v1/mesh/cache/{constructId:long}")]
public class MeshCacheController(IServiceProvider provider) : Controller
{
    private readonly IConstructMeshDownloaderService _constructMeshDownloaderService =
        provider.GetRequiredService<IConstructMeshDownloaderService>();

    private readonly Random _random = new();

    [HttpPost]
    [Route("")]
    public IActionResult RequestMeshCache(ulong constructId)
    {
        VoxelCacheQueue.Queue.Enqueue(constructId);

        return Ok();
    }

    [HttpPost]
    [Route("random-point")]
    public IActionResult QueryRandomPoint(ulong constructId, [FromBody] GetRandomPointRequest request)
    {
        ConstructVoxelCache.RefreshCache(constructId);
        
        if (request.FromPosition == null)
        {
            return BadRequest($"{nameof(request.FromPosition)} {nameof(Vector3)} is required");
        }

        var constructVoxelData = ConstructVoxelCache.Get(constructId);

        if (constructVoxelData == null)
        {
            return NotFound();
        }

        if (constructVoxelData.Voxels.Count == 0)
        {
            return NotFound();
        }

        var position = request.FromPosition.ToVector3();
        position = Vector3.Normalize(position) * 1000;
        
        var voxels = VoxelCuller.CullBlockedVoxels(
            constructVoxelData.Voxels,
            position,
            -Vector3.Normalize(position),
            request.ConeAngle,
            ConfigurationReader.GetVoxelSize()
        );
        
        var items = _random.GetItems(voxels.ToArray(), 1);
        var resultPos = items[0].ToVector3() * ConfigurationReader.GetVoxelSize();

        return Ok(new VectorModel
        {
            X = resultPos.X,
            Y = resultPos.Y,
            Z = resultPos.Z
        });
    }

    [HttpPost]
    [Route("glb")]
    public async Task<IActionResult> DownloadMeshVoxelVisualization(
        ulong constructId,
        [FromBody] DownloadMeshVoxelVisualizationRequest request
    )
    {
        if (request.FromPosition == null)
        {
            return BadRequest($"{nameof(request.FromPosition)} {nameof(Vector3)} is required");
        }

        var outcome = await _constructMeshDownloaderService.DownloadConstructMeshAsync(constructId, 2);

        if (!outcome.Success)
        {
            return BadRequest(outcome.Message);
        }

        var model = ModelRoot.ReadGLB(outcome.Stream);
        var constructVoxelData = ConstructVoxelCache.Get(constructId);

        if (constructVoxelData == null)
        {
            return BadRequest();
        }
        
        var voxels = constructVoxelData.Voxels;

        var position = request.FromPosition.ToVector3();

        if (request.CullVisible)
        {
            voxels = VoxelCuller.CullBlockedVoxels(
                voxels,
                position,
                -Vector3.Normalize(position),
                request.ConeAngle,
                ConfigurationReader.GetVoxelSize()
            );
        }

        var sceneBuilder = new SceneBuilder();

        MeshHelpers.BuildVoxelScene(
            sceneBuilder,
            voxels,
            ConfigurationReader.GetVoxelSize(),
            request.Scale
        );

        if (request.IncludeOriginalMesh)
        {
            var originalScene = model.DefaultScene;
            sceneBuilder.AddScene(originalScene.ToSceneBuilder(), Matrix4x4.Identity);
        }

        // Save the GLB file
        var bytes = sceneBuilder.ToGltf2().WriteGLB();

        return File(bytes.Array!, "model/gltf-binary", "model.glb");
    }

    public class GetRandomPointRequest
    {
        public VectorModel? FromPosition { get; set; }
        public float ConeAngle { get; set; } = 30;
    }
    
    public class DownloadMeshVoxelVisualizationRequest
    {
        public VectorModel? FromPosition { get; set; }
        public float ConeAngle { get; set; } = 30;
        public bool IncludeOriginalMesh { get; set; }
        public bool CullVisible { get; set; }
        public float Scale { get; set; } = 0.5f;
    }
    
    public class VectorModel
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public Vector3 ToVector3() => new(X, Y, Z);
    }
}