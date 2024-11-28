namespace VoxelService.Api.Construct.Data;

public class MeshDownloadOutcome
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public Stream Stream { get; set; }

    public static MeshDownloadOutcome MeshDownloaded(Stream stream) 
        => new() { Success = true, Stream = stream };

    public static MeshDownloadOutcome FailedToReadMesh(HttpResponseMessage responseMessage)
        => new()
        {
            Success = false,
            Message = $"Failed to read mesh. HTTP Status: {responseMessage.StatusCode}. {responseMessage.Content.ReadAsStringAsync().Result}"
        };
}