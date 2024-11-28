using Microsoft.AspNetCore.Mvc;
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
                Count = VoxelPool.GetCount(),
            }
        );
    }
}