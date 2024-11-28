namespace VoxelService.Api.Common;

public static class EnvironmentVariableHelper
{
    /// <summary>
    /// Checks the Environment variable ENVIRONMENT to check if it's production.
    /// If none is defined, assumes production
    /// </summary>
    /// <returns></returns>
    public static bool IsProduction()
        => GetEnvironmentVarOrDefault("ENVIRONMENT", "PROD") == "PROD";
    
    public static string GetEnvironmentVarOrDefault(string varName, string defaultValue)
    {
        var envVar = Environment.GetEnvironmentVariable(varName);

        if (string.IsNullOrEmpty(envVar))
        {
            return defaultValue;
        }

        return envVar;
    }

    public static int GetIntEnvironmentVarOrDefault(string varName, int defaultValue)
    {
        var value = GetEnvironmentVarOrDefault(varName, $"{defaultValue}");

        if (!int.TryParse(value, out var intValue))
        {
            return defaultValue;
        }

        return intValue;
    }
}