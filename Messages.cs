using System.Text.Json;

public class Messages
{
    private const string FallbackLanguage = "English";
    private readonly string _jsonPath;

    public string CurrentLanguage { get; private set; } = FallbackLanguage;

    private Dictionary<string, string> _dictionary = new();
    private Dictionary<string, string> _raceMap = new();
    private Dictionary<string, string> _occupationMap = new();
    private Dictionary<string, string> _displayRaceMap = new(StringComparer.OrdinalIgnoreCase);
    private Dictionary<string, string> _displayOccupationMap = new(StringComparer.OrdinalIgnoreCase);
    private Dictionary<string, string> _displayWeaponMap = new(StringComparer.OrdinalIgnoreCase);

    public Messages(string? jsonPath = null)
    {
        _jsonPath = jsonPath ?? Path.Combine(AppContext.BaseDirectory, "language_data.json");
    }

    public void SetCurrentLanguage(string language)
    {
        CurrentLanguage = language;
        ReadDictionary();
    }

    public void ReadDictionary()
    {
        try
        {
            using JsonDocument doc = JsonDocument.Parse(File.ReadAllText(_jsonPath));
            JsonElement root = doc.RootElement;

            if (!root.TryGetProperty(CurrentLanguage, out JsonElement lang) &&
                !root.TryGetProperty(FallbackLanguage, out lang))
                return;

            _dictionary = Load(lang, "dictionary");
            _raceMap = Load(lang, "raceMap");
            _occupationMap = Load(lang, "occupationMap");
            _displayRaceMap = Load(lang, "displayRaceMap");
            _displayOccupationMap = Load(lang, "displayOccupationMap");
            _displayWeaponMap = Load(lang, "displayWeaponMap");
        }
        catch (Exception ex) when (ex is IOException or JsonException or UnauthorizedAccessException)
        {
            Console.Error.WriteLine($"Could not load language data: {ex.Message}");
        }
    }

    private static Dictionary<string, string> Load(JsonElement lang, string name)
    {
        var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        if (lang.TryGetProperty(name, out JsonElement section))
            foreach (JsonProperty entry in section.EnumerateObject())
                map[entry.Name] = entry.Value.GetString() ?? string.Empty;
        return map;
    }

    public string GetMessage(string key) =>
        _dictionary.TryGetValue(key, out string? value) ? value : $"[{key}]";

    public bool IsValidRace(string? input) =>
        !string.IsNullOrWhiteSpace(input) && _raceMap.ContainsKey(input.Trim());

    public string NormalizeRace(string input) =>
        _raceMap.TryGetValue(input.Trim(), out string? canonical)
            ? canonical
            : throw new ArgumentException($"Invalid race: {input}", nameof(input));

    public bool IsValidOccupation(string? input) =>
        !string.IsNullOrWhiteSpace(input) && _occupationMap.ContainsKey(input.Trim());

    public string NormalizeOccupation(string input) =>
        _occupationMap.TryGetValue(input.Trim(), out string? canonical)
            ? canonical
            : throw new ArgumentException($"Invalid occupation: {input}", nameof(input));

    public string TranslateRaceForDisplay(string race) =>
        _displayRaceMap.TryGetValue(race, out string? t) ? t : race;

    public string TranslateOccupationForDisplay(string occupation) =>
        _displayOccupationMap.TryGetValue(occupation, out string? t) ? t : occupation;

    public string TranslateWeaponForDisplay(string weaponType) =>
        _displayWeaponMap.TryGetValue(weaponType, out string? t) ? t : weaponType;

    // Translated weapon name, wrapped in "magical {0}" (or its translation) once enchanted.
    public string FormatWeaponForDisplay(Weapon weapon)
    {
        string name = TranslateWeaponForDisplay(weapon.Type);
        return weapon.IsMagical ? string.Format(GetMessage("magical_weapon_format"), name) : name;
    }
}
