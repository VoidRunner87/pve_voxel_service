using System.Diagnostics;

namespace VoxelService.Api.Common;

public class TimeMeasure : IDisposable
{
    private readonly ILogger _logger;
    private readonly string _name;
    private readonly Stopwatch _stopwatch = new();

    public TimeMeasure(ILogger logger, string name)
    {
        _logger = logger;
        _name = name;
        _stopwatch.Start();
    }

    public void Dispose()
    {
        var timeElapsed = _stopwatch.ElapsedMilliseconds;

        _logger.LogInformation("{Name} took {Time}ms", _name, timeElapsed);
    }
}