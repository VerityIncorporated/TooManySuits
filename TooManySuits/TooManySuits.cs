using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using HarmonyLib;

namespace TooManySuits;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency(MoreSuitsGuid)]
public class TooManySuits : BaseUnityPlugin
{
    internal const string MoreSuitsGuid = "x753.More_Suits";

    internal const int VanillaSuitsPerPage = 13;

    internal const float SuitThickness = 0.18f;

    private static readonly Harmony Harmony = new(MyPluginInfo.PLUGIN_GUID);

    internal static SuitManager SuitManager { get; } = new();

    internal static AssetManager AssetManager { get; } = new();

    internal static new ManualLogSource Logger { get; private set; } = null!;

    internal static new Config Config { get; private set; } = null!;

    private void Awake()
    {
        Logger = base.Logger;
        Config = new Config(base.Config);
        Harmony.PatchAll();

        if (Chainloader.PluginInfos.ContainsKey("BMX.LobbyCompatibility"))
        {
            LobbyCompatibility.Init();
        }
    }
}
