using LobbyCompatibility.Enums;
using LobbyCompatibility.Features;

namespace TooManySuits;

internal static class LobbyCompatibility
{
    public static void Init()
    {
        TooManySuits.Logger.LogInfo("Registering plugin with LobbyCompatibility.");
        var pluginVersion = Version.Parse(MyPluginInfo.PLUGIN_VERSION);
        PluginHelper.RegisterPlugin(
            MyPluginInfo.PLUGIN_GUID,
            pluginVersion,
            CompatibilityLevel.ClientOnly,
            VersionStrictness.None
        );
    }
}
