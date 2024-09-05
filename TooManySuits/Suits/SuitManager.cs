using UnityEngine;

namespace TooManySuits.Suits
{
    public class SuitManager : MonoBehaviour
    {
        public static SuitManager Instance { get; private set; } = null!;

        private readonly List<UnlockableSuit> _allSuits = [];
        public IReadOnlyList<UnlockableSuit> AllSuits => _allSuits.AsReadOnly();

        public event Action? SuitsUpdated;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void UpdateSuits(IEnumerable<UnlockableSuit> suits)
        {
            _allSuits.Clear();
            _allSuits.AddRange(suits);
            SuitsUpdated?.Invoke();
        }
    }
}
