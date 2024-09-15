using HarmonyLib;

namespace TooManySuits.Hooks;

[HarmonyPatch(typeof(StartOfRound))]
internal class BuyShipUnlockable
{
    [HarmonyPostfix]
    [HarmonyPatch("PositionSuitsOnRack")]
    [HarmonyAfter(TooManySuits.MoreSuitsGuid)]
    private static void PositionSuitsOnRackPatchHook()
    {
        TooManySuits.SuitManager.UpdateSuits();
    }
}
