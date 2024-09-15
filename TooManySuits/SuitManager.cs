using UnityEngine;

namespace TooManySuits;

internal class SuitManager
{
    public SuitManager() { }

    public event Action? SuitsUpdated;

    public IEnumerable<UnlockableSuit> GetUnlockedSuits()
    {
        return Resources.FindObjectsOfTypeAll<UnlockableSuit>()
            .OrderBy(suit => suit.syncedSuitID.Value)
            .Where(suit => suit.IsSpawned);
    }

    internal void UpdateSuits()
    {
        SuitsUpdated?.Invoke();
    }
}
