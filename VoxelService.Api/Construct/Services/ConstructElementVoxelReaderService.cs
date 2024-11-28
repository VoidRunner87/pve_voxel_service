using NQ.Interfaces;
using NQutils.Def;
using Orleans;
using Services;
using VoxelService.Api.Common.Extensions;
using VoxelService.Api.Construct.Data;
using VoxelService.Api.Construct.Interfaces;
using VoxelService.Api.DU.Extensions;
using VoxelService.Data;
using BoundingBox = NQ.BoundingBox;

namespace VoxelService.Api.Construct.Services;

public class ConstructElementVoxelReaderService(IServiceProvider provider)
    : IConstructElementVoxelReaderService
{
    private readonly IClusterClient _orleans = provider.GetOrleans();
    private readonly IElementBoundingBox _elementBoundingBox = provider.GetRequiredService<IElementBoundingBox>();

    public async Task<ConstructElementVoxelsOutcome> QueryConstructElementsBoundingBoxes(ulong constructId)
    {
        var voxelSize = ConfigurationReader.GetVoxelSize();
        var constructElementsGrain = _orleans.GetConstructElementsGrain(constructId);

        var elements = await constructElementsGrain.GetElementsOfType<Element>();

        var voxels = new HashSet<Voxel>();

        foreach (var element in elements)
        {
            var elementInfo = await constructElementsGrain.GetElement(element);
            var offsetVoxel = Voxel.FromVector3(elementInfo.position.ToVector3(), voxelSize);

            voxels.Add(Voxel.FromVector3(elementInfo.position.ToVector3(), voxelSize));

            var boundingBox = _elementBoundingBox.GetBoundingBox(
                elementInfo.elementType
            );

            var voxelList = boundingBox.ToVoxels(voxelSize);
            foreach (var v in voxelList)
            {
                voxels.Add(v + offsetVoxel);
            }
        }

        return ConstructElementVoxelsOutcome.RetrievedVoxels(voxels);
    }

    private static HashSet<Voxel> ConvertBoundingBoxToVoxels(BoundingBox box, float voxelSize)
    {
        var voxels = new HashSet<Voxel>();

        // Convert the bounding box coordinates to voxel grid indices
        var minVoxel = Voxel.FromVector3(box.min.ToVector3(), voxelSize);
        var maxVoxel = Voxel.FromVector3(box.max.ToVector3(), voxelSize);

        // Iterate through all voxel indices in the range
        for (var x = minVoxel.X; x <= maxVoxel.X; x++)
        {
            for (var y = minVoxel.Y; y <= maxVoxel.Y; y++)
            {
                for (var z = minVoxel.Z; z <= maxVoxel.Z; z++)
                {
                    voxels.Add(VoxelPool.Voxel(x, y, z));
                }
            }
        }

        return voxels;
    }
}