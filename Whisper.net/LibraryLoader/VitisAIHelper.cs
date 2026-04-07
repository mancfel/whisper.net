// Licensed under the MIT license: https://opensource.org/licenses/MIT
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Whisper.net.Logger;

#if !IOS && !MACCATALYST && !TVOS && !ANDROID
#if !NETSTANDARD
using System.Runtime.Intrinsics.X86;
#endif
using System.Management;
#endif

namespace Whisper.net.LibraryLoader;

internal static class VitisAIHelper
{
    public static bool IsVitisAIAvailable(string platform, string architecture)
    {
        if (platform != "win" || architecture != "x64")
        {
            WhisperLogger.Log(WhisperLogLevel.Debug, "Vitis AI is only supported on Windows x64.");
            return false;
        }

        //Vitis AI is supported only on Windows for now
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return false;
        }

        var driverPath = Path.Combine(Environment.SystemDirectory, "AMD", "xrt-smi.exe");
        if (!File.Exists(driverPath))
        {
            WhisperLogger.Log(WhisperLogLevel.Debug, $"Vitis AI IPU driver not found at {driverPath}.");
            return false;
        }

        if (!IsAmdNpuDriverEnabled())
        {
            WhisperLogger.Log(WhisperLogLevel.Debug, "Vitis AI IPU driver is not enabled.");
            return false;
        }

        return true;
    }

#if !IOS && !MACCATALYST && !TVOS && !ANDROID
    private static bool IsAmdNpuDriverEnabled()
    {
        try
        {
            // We use a more specific query to avoid enumerating all PNP entities, which can be slow.
            var query = "SELECT Name FROM Win32_PnPEntity WHERE PNPClass = 'ComputeAccelerator' AND Name LIKE '%NPU%' AND Manufacturer LIKE '%AMD%'";
            using var searcher = new ManagementObjectSearcher(query);
            using var collection = searcher.Get();
            return collection.Count >= 1;
        }
        catch (Exception ex)
        {
            WhisperLogger.Log(WhisperLogLevel.Debug, $"Vitis AI: Error checking NPU driver via WMI: {ex.Message}");
            return false;
        }
    }
#else
    private static bool IsAmdNpuDriverEnabled()
    {
        return false;
    }
#endif
}
