namespace VoxelService.Api;

public static class ConfigurationReader
{
    public static string GetMeshDownloadUrl()
    {
        var url = Environment.GetEnvironmentVariable("MESH_DOWNLOAD_BASE_URL") ?? string.Empty;
        
        if (string.IsNullOrEmpty(url))
        {
            //Example: http://localhost:8081/public/voxels/constructs/1003409/mesh.glb?async=1&version=44219
            return "http://localhost:8081";
        }

        return url;
    }

    public static float GetVoxelSize() => 2f;

    public static TimeSpan GetConstructMeshDownloadThrottleTime() => TimeSpan.FromSeconds(5);
}