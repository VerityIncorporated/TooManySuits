using System.Numerics;
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
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace TooManySuits;

[BepInPlugin("verity.TooManySuits", "Too Many Suits", "1.0.8")]
[BepInDependency("x753.More_Suits")]
public class Plugin : BaseUnityPlugin
{
    public static ManualLogSource LogSource = null!;

    public static ConfigEntry<string> NextButton = null!;
    public static ConfigEntry<string> BackButton = null!;
    public static ConfigEntry<string> RefreshButton = null!;
    
    public static ConfigEntry<float> TextScale = null!;
    
    private void Awake()
    {
        LogSource = Logger;
        
        NextButton = Config.Bind("General", "Next-Page-Keybind", "<Keyboard>/n", "Next page button.");
        BackButton = Config.Bind("General", "Back-Page-Keybind", "<Keyboard>/b", "Back page button.");
        RefreshButton = Config.Bind("General", "Refresh-SuitRack-Keybind", "<Keyboard>/k", "Refreshes the suit rack, this can fix issues where purchased suits do not appear on the rack.");

        TextScale = Config.Bind("General", "Text-Scale", 0.005f, "Size of the text above the suit rack.");
        
        var pluginLoader = new GameObject("TooManySuits");
        pluginLoader.AddComponent<PluginLoader>();
        pluginLoader.hideFlags = HideFlags.HideAndDontSave;
        DontDestroyOnLoad(pluginLoader);
    }
}

public class PluginLoader : MonoBehaviour
{
    private readonly Harmony _harmony = new("TooManySuits");

    private InputAction _moveRightAction = null!;
    private InputAction _moveLeftAction = null!;
    private InputAction _refreshSuitRackAction = null!;

    private int _currentPage;
    private int _suitsPerPage = 13;


    private int _suitsLength;
    private static UnlockableSuit[] _allSuits = null!;

    private static AssetBundle _suitSelectBundle = null!;

    private void Awake()
    {
        Plugin.LogSource.LogInfo("TooManySuits Mod Loaded.");

        _moveRightAction = new InputAction(binding: Plugin.NextButton.Value);
        _moveRightAction.performed += MoveRightAction;
        _moveRightAction.Enable();

        _moveLeftAction = new InputAction(binding: Plugin.BackButton.Value);
        _moveLeftAction.performed += MoveLeftAction;
        _moveLeftAction.Enable();

        _refreshSuitRackAction = new InputAction(binding: Plugin.RefreshButton.Value);
        _refreshSuitRackAction.performed += RefreshSuitRack;
        _refreshSuitRackAction.Enable();

        var playerInputOriginal =
            typeof(StartOfRound).GetMethod("Start", BindingFlags.Instance | BindingFlags.NonPublic);
        var playerInputPostfix = typeof(Hooks).GetMethod(nameof(Hooks.HookStartGame));
        _harmony.Patch(playerInputOriginal, postfix: new HarmonyMethod(playerInputPostfix));

        var awakeOriginal =
            typeof(PlayerControllerB).GetMethod("Start", BindingFlags.Instance | BindingFlags.NonPublic);
        var awakePost = typeof(LocalPlayer).GetMethod(nameof(LocalPlayer.PlayerControllerStart));
        _harmony.Patch(awakeOriginal, postfix: new HarmonyMethod(awakePost));

        _suitSelectBundle = AssetBundle.LoadFromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "suitselect"));

        if (MoreSuits.MoreSuitsMod.MakeSuitsFitOnRack)
        {
            _suitsPerPage = 20;
        }
    }

    private void Update()
    {
        if (StartOfRound.Instance == null) return;
        try
        {
            DisplaySuits();
        }
        catch
        {
            //Cba to fix null reference exception
        }
    }

    private void DisplaySuits()
    {
        if (_allSuits.Length <= 0) return;

        var startIndex = _currentPage * _suitsPerPage;
        var endIndex = Mathf.Min(startIndex + _suitsPerPage, _allSuits.Length);

        var num = 0;
        for (var i = 0; i < _allSuits.Length; i++)
        {
            var unlockableSuit = _allSuits[i];
            var autoParent = unlockableSuit.gameObject.GetComponent<AutoParentToShip>();
            if (autoParent == null) continue;

            var shouldShow = i >= startIndex && i < endIndex;

            unlockableSuit.gameObject.SetActive(shouldShow);

            if (!shouldShow) continue;

            autoParent.overrideOffset = true;
            if (MoreSuits.MoreSuitsMod.MakeSuitsFitOnRack && _suitsLength > 13)
            {
                var offsetModifier = 0.18f;
                offsetModifier /= Math.Min(_suitsLength, 20) / 12f;
                
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

        _suitsLength = _allSuits.Length;

        if (LocalPlayer.localPlayer.isInHangarShipRoom)
        {
            Hooks.SuitPanel.SetActive(true);
            return;
        }

        Hooks.SuitPanel.SetActive(false);

        if (!Hooks.SetUI) return;

        Hooks.SetUI = false;

        var panelCanvas = Hooks.SuitPanel.GetComponentInChildren<Canvas>();
        
        panelCanvas.renderMode = RenderMode.WorldSpace;
        panelCanvas.worldCamera = LocalPlayer.localPlayer.gameplayCamera;

        panelCanvas.transform.position =
            StartOfRound.Instance.shipBounds.bounds.center - new Vector3(2.8992f, 0.7998f, 2f);
        panelCanvas.transform.rotation = Quaternion.Euler(0, 180, 0);
        panelCanvas.transform.localScale =
            new Vector3(Plugin.TextScale.Value, Plugin.TextScale.Value, Plugin.TextScale.Value);

        SetPageText();
        
        Hooks.SuitPanel.SetActive(true);
    }

    private void MoveRightAction(InputAction.CallbackContext obj)
    {
        if (!LocalPlayer.isActive()) return;
        
        _currentPage = Mathf.Min(_currentPage + 1, Mathf.CeilToInt((float)_suitsLength / _suitsPerPage) - 1);
        SetPageText();
    }

    private void MoveLeftAction(InputAction.CallbackContext obj)
    {
        if (!LocalPlayer.isActive()) return;
        
        _currentPage = Mathf.Max(_currentPage - 1, 0);
        SetPageText();
    }

    private void SetPageText()
    {
        var textMesh = Hooks.SuitPanel.GetComponentInChildren<TextMeshProUGUI>();
        textMesh.text = $"Page {_currentPage + 1}/{Mathf.CeilToInt((float)_suitsLength / _suitsPerPage)}";
    }

    private void RefreshSuitRack(InputAction.CallbackContext obj)
    {
        if (!LocalPlayer.isActive()) return;
        _allSuits = Resources.FindObjectsOfTypeAll<UnlockableSuit>().OrderBy(suit => suit.syncedSuitID.Value).ToArray();
    }

    private class Hooks
    {
        public static bool SetUI;
        public static GameObject SuitPanel = null!;

        public static void HookStartGame()
        {
            Plugin.LogSource.LogInfo("StartOfRound!");
            _allSuits = Resources.FindObjectsOfTypeAll<UnlockableSuit>().OrderBy(suit => suit.syncedSuitID.Value).ToArray();
            SuitPanel = Instantiate(_suitSelectBundle.LoadAsset<GameObject>("SuitSelect"));
            SuitPanel.SetActive(false);
            SetUI = true;
        }
    }
}