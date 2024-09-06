using TMPro;
using TooManySuits.Suits;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TooManySuits.UI;

public class PaginationController
{
    private int CurrentPage { get; set; }

    private int _suitsPerPage;
    private int _totalPages;

    private List<UnlockableSuit> _allSuits = null!;

    private InputAction _nextPageAction = null!;
    private InputAction _previousPageAction = null!;

    public void Initialize(int suitsPerPage)
    {
        _suitsPerPage = suitsPerPage;

        CurrentPage = 0;

        _nextPageAction = new InputAction(binding: Plugin.NextPage.Value);
        _previousPageAction = new InputAction(binding: Plugin.PreviousPage.Value);

        _nextPageAction.performed += _ => NextPage();
        _previousPageAction.performed += _ => PreviousPage();

        _nextPageAction.Enable();
        _previousPageAction.Enable();

        SuitManager.Instance.SuitsUpdated += UpdateSuits;
    }

    private void UpdateSuits()
    {
        _allSuits = [..SuitManager.Instance.AllSuits];
        _totalPages = Mathf.CeilToInt(_allSuits.Count / (float)_suitsPerPage);
        DisplayCurrentPage();

        CurrentPage = 0;
    }

    private void NextPage()
    {
        if (CurrentPage >= _totalPages - 1) return;

        CurrentPage++;
        DisplayCurrentPage();
    }

    private void PreviousPage()
    {
        if (CurrentPage <= 0) return;

        CurrentPage--;
        DisplayCurrentPage();
    }

    private void SetPageText()
    {
        var textMesh = TooManySuits.SuitRackPrefab.GetComponentInChildren<TextMeshProUGUI>();
        if (textMesh != null)
        {
            textMesh.text = $"Page {CurrentPage + 1}/{_totalPages}";
        }
    }

    private void DisplayCurrentPage()
    {
        var startIndex = CurrentPage * _suitsPerPage;
        var endIndex = Mathf.Min(startIndex + _suitsPerPage, _allSuits.Count);

        var num = 0;
        for (var i = 0; i < _allSuits.Count; i++)
        {
            var unlockableSuit = _allSuits[i];
            var autoParent = unlockableSuit.gameObject.GetComponent<AutoParentToShip>();
            if (autoParent == null) continue;

            var shouldShow = i >= startIndex && i < endIndex;

            foreach (var renderer in unlockableSuit.gameObject.GetComponentsInChildren<Renderer>())
            {
              renderer.enabled = shouldShow;
            }

            foreach (var collider in unlockableSuit.gameObject.GetComponentsInChildren<Collider>())
            {
              collider.enabled = shouldShow;
            }

            var interactTrigger = unlockableSuit.gameObject.GetComponent<InteractTrigger>();
            interactTrigger.enabled = shouldShow;
            interactTrigger.interactable = shouldShow;

            if (!shouldShow) continue;

            autoParent.overrideOffset = true;
            if (Plugin.MoreSuitsInterop.MakeSuitsFitOnRack && _allSuits.Count > 13)
            {
                var offsetModifier = 0.18f;
                offsetModifier /= Mathf.Min(_allSuits.Count, 20) / 12f;

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

        SetPageText();
    }
}
