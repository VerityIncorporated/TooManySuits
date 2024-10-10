using BepInEx.Configuration;

namespace TooManySuits;

internal class Config
{
    private readonly ConfigEntry<float> _configLabelScale;

    public float LabelScale => _configLabelScale.Value;

    private readonly ConfigEntry<int> _configSuitsPerPage;

    public int SuitsPerPage => _configSuitsPerPage.Value;

    public Config(ConfigFile cfg)
    {
        const string SectionPagination = "Pagination";
        _configSuitsPerPage = cfg.Bind(
            section: SectionPagination,
            key: "SuitsPerPage",
            defaultValue: TooManySuits.VanillaSuitsPerPage,
            configDescription: new ConfigDescription(
                description: "Number of suits per page in the suit rack.",
                acceptableValues: new AcceptableValueRange<int>(TooManySuits.VanillaSuitsPerPage, 20)
            )
        );

        const string SectionUI = "UI";
        _configLabelScale = cfg.Bind(
            section: SectionUI,
            key: "LabelScale",
            defaultValue: 1f,
            configDescription: new ConfigDescription(
                description: "Size of the text above the suit rack.",
                acceptableValues: new AcceptableValueRange<float>(0.5f, 3f)
            )
        );

        cfg.OrphanedEntries.Clear();
        cfg.Save();
    }
}
