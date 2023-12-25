using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using GameNetcodeStuff;
using HarmonyLib;
using TMPro;
using TooManySuits.Helper;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TooManySuits;

[BepInPlugin("verity.TooManySuits", "Too Many Suits", "1.0.4")]
[BepInDependency("x753.More_Suits")]
public class Plugin : BaseUnityPlugin
{
    public static ManualLogSource LogSource = null!;

    public static ConfigEntry<string> NextButton = null!;
    public static ConfigEntry<string> BackButton = null!;
    
    public static ConfigEntry<float> TextScale = null!;
    
    private void Awake()
    {
        LogSource = Logger;
        
        NextButton = Config.Bind("General", "Next-Page-Keybind", "<Keyboard>/n", "Next page button.");
        BackButton = Config.Bind("General", "Back-Page-Keybind", "<Keyboard>/b", "Back page button.");

        TextScale = Config.Bind("General", "Text-Scale", 0.003f, "Size of the text above the suit rack.");
        
        var pluginLoader = new GameObject("TooManySuits");
        pluginLoader.AddComponent<PluginLoader>();
        pluginLoader.hideFlags = HideFlags.HideAndDontSave;
        DontDestroyOnLoad(pluginLoader);
    }
}

public class PluginLoader : MonoBehaviour
{
    private readonly Harmony Harmony = new("TooManySuits");

    private InputAction moveRightAction = null!;
    private InputAction moveLeftAction = null!;

    private int currentPage;
    private int suitsPerPage = 13;


    private int suitsLength;
    private UnlockableSuit[] allSuits = null!;

    private static AssetBundle suitSelectBundle = null!;

    private void Awake()
    {
        Plugin.LogSource.LogInfo("TooManySuits Mod Loaded.");

        moveRightAction = new InputAction(binding: Plugin.NextButton.Value);
        moveRightAction.performed += MoveRightAction;
        moveRightAction.Enable();

        moveLeftAction = new InputAction(binding: Plugin.BackButton.Value);
        moveLeftAction.performed += MoveLeftAction;
        moveLeftAction.Enable();

        var playerInputOriginal =
            typeof(StartOfRound).GetMethod("Start", BindingFlags.Instance | BindingFlags.NonPublic);
        var playerInputPostfix = typeof(Hooks).GetMethod(nameof(Hooks.HookStartGame));
        Harmony.Patch(playerInputOriginal, postfix: new HarmonyMethod(playerInputPostfix));

        var awakeOriginal =
            typeof(PlayerControllerB).GetMethod("Start", BindingFlags.Instance | BindingFlags.NonPublic);
        var awakePost = typeof(LocalPlayer).GetMethod(nameof(LocalPlayer.PlayerControllerStart));
        Harmony.Patch(awakeOriginal, postfix: new HarmonyMethod(awakePost));

        suitSelectBundle = AssetBundle.LoadFromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "suitselect"));

        if (MoreSuits.MoreSuitsMod.MakeSuitsFitOnRack)
        {
            suitsPerPage = 20;
        }
    }

    private void Update()
    {
        if (StartOfRound.Instance == null) return;
        allSuits = Resources.FindObjectsOfTypeAll<UnlockableSuit>().OrderBy(suit => suit.syncedSuitID.Value).ToArray();
        DisplaySuits();
    }

    private void DisplaySuits()
    {
        if (allSuits.Length <= 0) return;

        var startIndex = currentPage * suitsPerPage;
        var endIndex = Mathf.Min(startIndex + suitsPerPage, allSuits.Length);

        var num = 0;
        for (var i = 0; i < allSuits.Length; i++)
        {
            var unlockableSuit = allSuits[i];
            var autoParent = unlockableSuit.gameObject.GetComponent<AutoParentToShip>();
            if (autoParent == null) continue;

            var shouldShow = i >= startIndex && i < endIndex;

            unlockableSuit.gameObject.SetActive(shouldShow);

            if (!shouldShow) continue;

            autoParent.overrideOffset = true;
            if (MoreSuits.MoreSuitsMod.MakeSuitsFitOnRack && suitsLength > 13)
            {
                var offsetModifier = 0.18f;
                offsetModifier /= Math.Min(suitsLength, 20) / 12f;
                
                autoParent.positionOffset = new Vector3(-2.45f, 2.75f, -8.41f) + StartOfRound.Instance.rightmostSuitPosition.forward * (offsetModifier * num);
                autoParent.rotationOffset = new Vector3(0f, 90f, 0f);  
            }
            else
            {
                autoParent.positionOffset = new Vector3(-2.45f, 2.75f, -8.41f) + StartOfRound.Instance.rightmostSuitPosition.forward * (0.18f * num);
                autoParent.rotationOffset = new Vector3(0f, 90f, 0f);
            }

            num++;
        }

        suitsLength = allSuits.Length;

        if (!LocalPlayer.isActive()) return;

        if (LocalPlayer.localPlayer.isInHangarShipRoom)
        {
            var textMesh = Hooks.SuitPanel.GetComponentInChildren<TextMeshProUGUI>();
            textMesh.text = $"Page {currentPage + 1}/{suitsLength / suitsPerPage + 1}";

            Hooks.SuitPanel.SetActive(true);

            return;
        }

        Hooks.SuitPanel.SetActive(false);

        if (!Hooks.SetUI) return;

        Hooks.SetUI = false;

        Hooks.SuitPanel.GetComponentInParent<Canvas>().renderMode = RenderMode.WorldSpace;
        Hooks.SuitPanel.GetComponentInParent<Canvas>().worldCamera = LocalPlayer.localPlayer.gameplayCamera;

        Hooks.SuitPanel.transform.position =
            StartOfRound.Instance.shipBounds.bounds.center - new Vector3(2.8992f, 0.7998f, 2f);
        Hooks.SuitPanel.transform.rotation = Quaternion.Euler(0, 180, 0);
        Hooks.SuitPanel.transform.localScale =
            new Vector3(Plugin.TextScale.Value, Plugin.TextScale.Value, Plugin.TextScale.Value);

        Hooks.SuitPanel.SetActive(true);
    }

    private void MoveRightAction(InputAction.CallbackContext obj)
    {
        currentPage = Mathf.Min(currentPage + 1,
            Mathf.CeilToInt((float)suitsLength / suitsPerPage) - 1);
    }

    private void MoveLeftAction(InputAction.CallbackContext obj)
    {
        currentPage = Mathf.Max(currentPage - 1, 0);
    }

    private class Hooks
    {
        public static bool SetUI;
        public static GameObject SuitPanel = null!;

        public static void HookStartGame()
        {
            Plugin.LogSource.LogInfo("StartOfRound!");

            Instantiate(suitSelectBundle.LoadAsset<GameObject>("SuitSelect"));
            SuitPanel = GameObject.Find("SuitPanel");
            SuitPanel.SetActive(false);
            SetUI = true;
        }
    }
}