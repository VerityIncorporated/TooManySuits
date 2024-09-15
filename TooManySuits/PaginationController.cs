using System.Collections;
using TMPro;
using UnityEngine;

namespace TooManySuits;

public class PaginationController : MonoBehaviour
{
    private UnlockableSuit[] _allSuits = null!;

    private TextMeshPro _pageTextMesh = null!;

    private GameObject _pageGO = null!;

    private GameObject _previousGO = null!;

    private GameObject _nextGO = null!;

    private bool _shouldUpdate;

    public int SuitsPerPage { get; set; }

    private int CurrentPage { get; set; }

    private int PageCount { get; set; }

    private static Sprite FindInteractIcon()
    {
        var terminal = FindObjectOfType<Terminal>()
            ?? throw new InvalidOperationException("Can't find Terminal object");
        var interactTrigger = terminal.GetComponent<InteractTrigger>()
            ?? throw new InvalidOperationException("Can't find InteractTrigger component from Terminal object");
        return interactTrigger.hoverIcon;
    }

    private void Awake()
    {
        if (SuitsPerPage <= 0)
        {
            throw new InvalidOperationException("SuitsPerPage must be >= 0");
        }

        static TextMeshPro CreateTMP(GameObject go, string text)
        {
            var tmp = go.AddComponent<TextMeshPro>();
            tmp.autoSizeTextContainer = true;
            tmp.enableWordWrapping = false;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.text = text;
            tmp.font = TooManySuits.AssetManager.VGA437Font;
            tmp.fontMaterial = TooManySuits.AssetManager.VGA437Font.material;
            tmp.color = new Color(255, 255, 255, 255);
            tmp.outlineColor = new Color32(0, 0, 0, 255);
            tmp.outlineWidth = 0.25f;
            return tmp;
        }

        static InteractTrigger CreateInteractTrigger(GameObject go)
        {
            var icon = FindInteractIcon();
            var trigger = go.AddComponent<InteractTrigger>();
            trigger.gameObject.tag = "InteractTrigger";
            trigger.gameObject.layer = LayerMask.NameToLayer("InteractableObject");
            trigger.interactable = true;
            trigger.oneHandedItemAllowed = true;
            trigger.holdInteraction = false;
            trigger.interactCooldown = false;
            trigger.onInteract = new InteractEvent();
            trigger.onInteractEarly = new InteractEvent();
            trigger.onCancelAnimation = new InteractEvent();
            trigger.onStopInteract = new InteractEvent();
            trigger.holdingInteractEvent = new InteractEventFloat();
            trigger.hoverTip = "";
            trigger.disabledHoverTip = "";
            trigger.hoverIcon = icon;
            trigger.disabledHoverIcon = icon;
            return trigger;
        }

        var colliderSize = new Vector2(10f, 10f);

        _pageGO = new GameObject("page");
        _pageGO.transform.SetParent(transform, false);
        _pageTextMesh = CreateTMP(_pageGO, "Page");

        {
            _previousGO = new GameObject("previousButton");
            var btnTransform = _previousGO.AddComponent<RectTransform>();
            btnTransform.SetParent(transform, false);
            btnTransform.anchorMax = new Vector2(0, 0.5f);
            btnTransform.anchorMin = new Vector2(0, 0.5f);
            CreateTMP(_previousGO, "<");

            var triggerGO = new GameObject("trigger");
            var triggerTransform = triggerGO.AddComponent<RectTransform>();
            triggerTransform.SetParent(btnTransform, false);
            var collider = triggerGO.AddComponent<BoxCollider>();
            collider.size = colliderSize;

            var trigger = CreateInteractTrigger(triggerGO);
            trigger.onInteract.AddListener(_ => PreviousPage());
        }

        {
            _nextGO = new GameObject("nextButton");
            var btnTransform = _nextGO.AddComponent<RectTransform>();
            btnTransform.SetParent(transform, false);
            btnTransform.anchorMax = new Vector2(1, 0.5f);
            btnTransform.anchorMin = new Vector2(1, 0.5f);
            CreateTMP(_nextGO, ">");

            var triggerGO = new GameObject("trigger");
            var triggerTransform = triggerGO.AddComponent<RectTransform>();
            triggerTransform.SetParent(btnTransform, false);
            var collider = triggerGO.AddComponent<BoxCollider>();
            collider.size = colliderSize;

            var trigger = CreateInteractTrigger(triggerGO);
            trigger.onInteract.AddListener(_ => NextPage());
        }
    }

    private void Start()
    {
        TooManySuits.SuitManager.SuitsUpdated += OnSuitsUpdated;
        UpdateSuits();
    }

    private void Update()
    {
        if (!_shouldUpdate) return;
        _shouldUpdate = false;
        UpdateSuits();
    }

    private void OnDestroy()
    {
        TooManySuits.SuitManager.SuitsUpdated -= OnSuitsUpdated;
    }

    private void OnSuitsUpdated()
    {
        _shouldUpdate = true;
    }

    private void UpdateSuits()
    {
        _allSuits = TooManySuits.SuitManager.GetUnlockedSuits().ToArray();
        PageCount = Mathf.CeilToInt(_allSuits.Length / (float)SuitsPerPage);

        if (CurrentPage > PageCount - 1)
        {
            CurrentPage = PageCount - 1;
        }

        DisplayCurrentPage();
    }

    private void NextPage()
    {
        if (CurrentPage >= PageCount - 1) return;

        CurrentPage++;
        DisplayCurrentPage();
    }

    private void PreviousPage()
    {
        if (CurrentPage <= 0) return;

        CurrentPage--;
        DisplayCurrentPage();
    }

    private void DisplayCurrentPage()
    {
        var startIndex = CurrentPage * SuitsPerPage;
        var endIndex = Mathf.Min(startIndex + SuitsPerPage, _allSuits.Length);

        var suitThickness = TooManySuits.SuitThickness;
        if (TooManySuits.Config.SuitsPerPage > TooManySuits.VanillaSuitsPerPage && _allSuits.Length > TooManySuits.VanillaSuitsPerPage)
        {
            suitThickness /= Mathf.Min(_allSuits.Length, TooManySuits.Config.SuitsPerPage) / 12f;
        }

        var num = 0;
        for (var i = 0; i < _allSuits.Length; i++)
        {
            var suit = _allSuits[i];
            var autoParent = suit.gameObject.GetComponent<AutoParentToShip>();
            if (autoParent == null) continue;

            var shouldShow = i >= startIndex && i < endIndex;

            foreach (var renderer in suit.gameObject.GetComponentsInChildren<Renderer>())
            {
                renderer.enabled = shouldShow;
            }

            foreach (var collider in suit.gameObject.GetComponentsInChildren<Collider>())
            {
                collider.enabled = shouldShow;
            }

            var interactTrigger = suit.gameObject.GetComponent<InteractTrigger>();
            interactTrigger.enabled = shouldShow;
            interactTrigger.interactable = shouldShow;

            if (!shouldShow) continue;

            autoParent.overrideOffset = true;
            autoParent.positionOffset = new Vector3(-2.45f, 2.75f, -8.41f)
                + StartOfRound.Instance.rightmostSuitPosition.forward * (suitThickness * num);
            autoParent.rotationOffset = new Vector3(0f, 90f, 0f);

            num++;
        }

        StartCoroutine(UpdateLabel());
    }

    private IEnumerator UpdateLabel()
    {
        _pageTextMesh.text = $"<b>{CurrentPage + 1}/{PageCount}</b>";

        // Wait for the next frame so TextMeshPro has time to update the characterInfo.
        yield return null;

        var characterInfo = _pageTextMesh.textInfo.characterInfo;
        var characterCount = _pageTextMesh.textInfo.characterCount;

        var margin = new Vector3(2, 0, 0);
        var outline = new Vector3(0, 1, 0) * _pageTextMesh.outlineWidth;
        _previousGO.transform.localPosition =
            characterInfo[0].topLeft
            - (outline * 2)
            - margin
            + new Vector3(0, characterInfo[0].baseLine, 0);
        _nextGO.transform.localPosition =
            characterInfo[characterCount - 1].topRight
            - (outline * 2)
            + margin
            + new Vector3(0, characterInfo[characterCount - 1].baseLine, 0);

        _previousGO.SetActive(PageCount > 0 && CurrentPage > 0);
        _nextGO.SetActive(PageCount > 0 && CurrentPage < PageCount - 1);
        _pageGO.SetActive(PageCount > 0);
    }
}
