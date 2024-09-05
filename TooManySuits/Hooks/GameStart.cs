using System.Collections;
using HarmonyLib;
using TooManySuits.Suits;
using UnityEngine;

namespace TooManySuits.Hooks;

[HarmonyPatch(typeof(StartOfRound), "Start")]
public class GameStart
{
    private static Coroutine _waitForPlayerCoroutine = null!;

    [HarmonyPostfix]
    public static void GameStartHook()
    {
        var allSuits = Resources.FindObjectsOfTypeAll<UnlockableSuit>().OrderBy(suit => suit.syncedSuitID.Value).ToArray();
        var validSuits = allSuits.Where(suit => suit.IsSpawned);

        SuitManager.Instance.UpdateSuits(validSuits);

        _waitForPlayerCoroutine = TooManySuits.Instance.StartCoroutine(WaitForPlayerAndSetUpCanvas());
    }

    private static IEnumerator WaitForPlayerAndSetUpCanvas()
    {
        while (StartOfRound.Instance.localPlayerController == null)
        {
            yield return null;
        }

        var suitRackPrefab = TooManySuits.SuitRackPrefab;
        if (suitRackPrefab != null)
        {
            suitRackPrefab.SetActive(true);

            var panelCanvas = suitRackPrefab.GetComponentInChildren<Canvas>();

            panelCanvas.renderMode = RenderMode.WorldSpace;
            panelCanvas.worldCamera = StartOfRound.Instance.localPlayerController.gameplayCamera;

            panelCanvas.transform.position = StartOfRound.Instance.shipBounds.bounds.center - new Vector3(2.8992f, 0.7998f, 2f);
            panelCanvas.transform.rotation = Quaternion.Euler(0, 180, 0);
            panelCanvas.transform.localScale = new Vector3(Plugin.TextScale.Value, Plugin.TextScale.Value, Plugin.TextScale.Value);
        }
        else
        {
            TooManySuits.GlobalLogSource.LogError("Suit Rack Prefab is null!");
        }

        _waitForPlayerCoroutine = null!;
    }
}
