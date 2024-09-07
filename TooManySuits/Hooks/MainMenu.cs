using HarmonyLib;

namespace TooManySuits.Hooks;

[HarmonyPatch(typeof(MenuManager), "Start")]
public class MainMenu
{
    [HarmonyPostfix]
    public static void MainMenuHook(MenuManager __instance)
    {
        var prefab = TooManySuits.SuitRackPrefab;
        if (__instance.isInitScene || !prefab.activeSelf) return;

        prefab.SetActive(false);
    }
}
