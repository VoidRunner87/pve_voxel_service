using Microsoft.AspNetCore.Mvc;
using VoxelService.Api.Construct.Data;
using VoxelService.Api.Construct.Services;
using VoxelService.Data;

namespace VoxelService.Api.Mesh.Controllers;

[Route("v1/voxel")]
public class VoxelController : Controller
{
    [HttpGet]
    [Route("stats")]
    public IActionResult GetStats()
    {
        return Ok(
            new
            {
                ConstructCount = ConstructVoxelCache.Data.Count,
                QueueCount = VoxelCacheQueue.Queue.Count, 
                VoxelCount = VoxelPool.GetCount(),
            }
        );
    }
}