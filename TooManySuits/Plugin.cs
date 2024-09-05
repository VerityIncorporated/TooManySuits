using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using TooManySuits.Suits;
using TooManySuits.UI;
using UnityEngine;
using static BepInEx.Logging.Logger;

namespace TooManySuits;

[BepInPlugin("verity.TooManySuits", "Too Many Suits", "1.1.0")]
[BepInDependency(MoreSuitsGuid, BepInDependency.DependencyFlags.HardDependency)]
public class Plugin : BaseUnityPlugin
{
    internal const string MoreSuitsGuid = "x753.More_Suits";

    internal static MoreSuitsInterop MoreSuitsInterop { get; private set; } = null!;

    public static ConfigEntry<string> NextPage = null!;
    public static ConfigEntry<string> PreviousPage = null!;
    public static ConfigEntry<int> SuitsPerPage = null!;
    
    public static ConfigEntry<float> TextScale = null!;
    
    private void Awake()
    {
        NextPage = Config.Bind("Pagination", "Next Page Keybinding", "<keyboard>/n", "Button which changes to the next page.");
        PreviousPage = Config.Bind("Pagination", "Previous Page Keybinding", "<keyboard>/b", "Button which changes to the previous page.");
        SuitsPerPage = Config.Bind("Pagination", "Items Per Page", 20, "Number of suits per page in the suit rack. Anything over 20 will cause clipping issues.");
        
        TextScale = Config.Bind("UI", "Text Scale", 0.005f, "Size of the text above the suit rack.");

        MoreSuitsInterop = new MoreSuitsInterop();

        var pluginGameObject = new GameObject("TooManySuits");
        pluginGameObject.AddComponent<TooManySuits>();
        pluginGameObject.hideFlags = HideFlags.HideAndDontSave;
        DontDestroyOnLoad(pluginGameObject);
    }
}

public class TooManySuits : MonoBehaviour
{
    public static TooManySuits Instance = null!;
    
    public static readonly ManualLogSource GlobalLogSource = new("TooManySuits");
    private static readonly Harmony Harmony = new("TooManySuits");

    private PaginationController _paginationController = null!;
    public SuitManager suitManager = null!;

    public static GameObject SuitRackPrefab = null!;
    
    public void Awake()
    {
        Instance = this;
        
        Sources.Add(GlobalLogSource);
    }

    private void Start()
    {
        LoadAssetBundle();
        
        Harmony.PatchAll();
        
        suitManager = gameObject.AddComponent<SuitManager>();
        DontDestroyOnLoad(suitManager);
        
        _paginationController = new PaginationController();
        _paginationController.Initialize(Plugin.SuitsPerPage.Value);
    }

    private void LoadAssetBundle()
    {
        const string assetBundleName = "TooManySuits.SuitSelect";
        var assetBundle = LoadAssetBundleFromResources(assetBundleName);
        if (assetBundle == null)
        {
            GlobalLogSource.LogError("Failed to load Asset Bundle!");
            return;
        }
        
        var prefab = assetBundle.LoadAsset<GameObject>("SuitSelect");
        if (prefab == null)
        {
            GlobalLogSource.LogError("Failed to load Suit Rack Prefab from Asset Bundle!");
            return;
        }

        SuitRackPrefab = Instantiate(prefab);
        SuitRackPrefab.SetActive(false);
        SuitRackPrefab.AddComponent<AutoParentToShip>();
        
        SuitRackPrefab.hideFlags = HideFlags.HideAndDontSave;
        DontDestroyOnLoad(SuitRackPrefab);
    }
    
    private static AssetBundle LoadAssetBundleFromResources(string resourceName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream(resourceName)!;
        return AssetBundle.LoadFromStream(stream);
    }
}