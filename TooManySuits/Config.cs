using BepInEx.Configuration;

namespace TooManySuits;

internal class Config
{
    private const float LabelScaleMin = 0.5f;

    private const float LabelScaleMax = 3f;

    private readonly ConfigEntry<float> _configLabelScale;

    private float? _labelScale;

    public float LabelScale
    {
        get
        {
            if (_labelScale != null)
            {
                return (float)_labelScale;
            }

            var value = Math.Clamp(
                value: _configLabelScale.Value,
                min: LabelScaleMin,
                max: LabelScaleMax
            );

            if (value != _configLabelScale.Value)
            {
                TooManySuits.Logger.LogWarning(
                    "LabelScale exceeds the range of"
                    + $" [{LabelScaleMin}, {LabelScaleMax}];"
                    + $" clamping value to {value}"
                );
            }

            _labelScale = value;
            return value;
        }
    }

    private const int SuitsPerPageMin = TooManySuits.VanillaSuitsPerPage;

    private const int SuitsPerPageMax = 20;

    private readonly ConfigEntry<int> _configSuitsPerPage;

    private int? _suitsPerPage;

    public int SuitsPerPage
    {
        get
        {
            if (_suitsPerPage != null)
            {
                return (int)_suitsPerPage;
            }

            var value = Math.Clamp(
                value: _configSuitsPerPage.Value,
                min: SuitsPerPageMin,
                max: SuitsPerPageMax
            );

            if (value != _configSuitsPerPage.Value)
            {
                TooManySuits.Logger.LogWarning(
                    "SuitsPerPage exceeds the range of"
                    + $" [{SuitsPerPageMin}, {SuitsPerPageMax}];"
                    + $" clamping value to {value}"
                );
            }

            _suitsPerPage = value;
            return value;
        }
    }

    public Config(ConfigFile cfg)
    {
        const string SectionPagination = "Pagination";
        _configSuitsPerPage = cfg.Bind(
            section: SectionPagination,
            key: "SuitsPerPage",
            defaultValue: TooManySuits.VanillaSuitsPerPage,
            description: "Number of suits per page in the suit rack."
        );

        const string SectionUI = "UI";
        _configLabelScale = cfg.Bind(
            section: SectionUI,
            key: "LabelScale",
            defaultValue: 1f,
            description: "Size of the text above the suit rack."
        );

        cfg.OrphanedEntries.Clear();
        cfg.Save();
    }
}
