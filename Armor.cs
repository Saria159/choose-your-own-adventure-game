public class Armor
{
    public string Type { get; set; }
    public int Protection { get; set; }
    public string AsciiArt { get; set; }

    public Armor(string type, int protection, string asciiArt)
    {
        Type = type;
        Protection = protection;
        AsciiArt = asciiArt;
    }
}
