using HarmonyLib;
using UnityEngine;

namespace TooManySuits;

[HarmonyPatch]
internal class Patches
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(StartOfRound), "Start")]
    [HarmonyAfter(TooManySuits.MoreSuitsGuid)]
    private static void StartPatch()
    {
        var pageLabelGO = new GameObject("TooManySuitsPageLabel");
        pageLabelGO.SetActive(false);

        var pageLabelTransform = pageLabelGO.AddComponent<RectTransform>();
        pageLabelTransform.SetParent(StartOfRound.Instance.rightmostSuitPosition, false);
        pageLabelTransform.localPosition = Vector3.zero;
        pageLabelTransform.localEulerAngles = Vector3.zero;
        pageLabelTransform.localScale = Vector3.one * TooManySuits.Config.LabelScale * 0.05f;

        var paginationController = pageLabelGO.AddComponent<PaginationController>();
        paginationController.SuitsPerPage = TooManySuits.Config.SuitsPerPage;

        // This is totally guesstimated. The math is probably wrong, but it looks
        // centered enough to me... oh well
        var centerOffset = StartOfRound.Instance.rightmostSuitPosition.forward
            * (TooManySuits.SuitThickness * (TooManySuits.VanillaSuitsPerPage - 1.5f)) / 2f;

        var autoParentToShip = pageLabelGO.AddComponent<AutoParentToShip>();
        autoParentToShip.overrideOffset = true;
        autoParentToShip.positionOffset = new Vector3(-2.45f, 3f, -8.41f) + centerOffset;
        autoParentToShip.rotationOffset = new Vector3(0f, 180f, 0f);

        pageLabelGO.SetActive(true);
        TooManySuits.SuitManager.UpdateSuits();
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(StartOfRound), "PositionSuitsOnRack")]
    [HarmonyAfter(TooManySuits.MoreSuitsGuid)]
    private static void PositionSuitsOnRackPatch()
    {
        TooManySuits.SuitManager.UpdateSuits();
    }
}
