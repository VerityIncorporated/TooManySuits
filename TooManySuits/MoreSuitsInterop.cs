using System.Reflection;
using BepInEx;
using BepInEx.Bootstrap;
using HarmonyLib;

namespace TooManySuits;

internal class MoreSuitsInterop
{
    private readonly BaseUnityPlugin _moreSuitsInstance;
    private readonly FieldInfo _makeSuitsFitOnRackField;

    public MoreSuitsInterop()
    {
        var pluginInfo = Chainloader.PluginInfos
            .Where(p => p.Key == Plugin.MoreSuitsGuid)
            .Select(x => x.Value)
            .FirstOrDefault()
            ?? throw new InvalidOperationException("Plugin 'More Suits' not found");

        var pluginType = pluginInfo.Instance.GetType();
        _moreSuitsInstance = pluginInfo.Instance;
        _makeSuitsFitOnRackField = AccessTools.Field(pluginType, "MakeSuitsFitOnRack")
            ?? throw new MemberNotFoundException("Field 'MakeSuitsFitOnRack' not found in MoreSuits plugin class");
    }

    public bool MakeSuitsFitOnRack => (bool)_makeSuitsFitOnRackField.GetValue(_moreSuitsInstance);
}
