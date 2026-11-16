[TestClass]
public class InventoryMessagesTests
{
    private static readonly string[] NewKeys = { "inventory_header", "inventory_no_armor" };

    private static readonly string[] PromptsOfferingInventory =
    {
        "path_prompt_full",
        "merchant_prompt",
        "dragon_encounter_prompt",
        "attack_prompt"
    };

    private static Messages GetMessages(string language)
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage(language);
        return messages;
    }

    [TestMethod]
    [DataRow("English")]
    [DataRow("Spanish")]
    [DataRow("French")]
    [DataRow("Italian")]
    public void EveryInventoryKey_IsTranslated(string language)
    {
        Messages messages = GetMessages(language);

        string[] missing = NewKeys
            .Where(key => messages.GetMessage(key) == $"[{key}]")
            .ToArray();

        Assert.AreEqual(0, missing.Length, $"{language} is missing: {string.Join(", ", missing)}");
    }

    [TestMethod]
    [DataRow("English")]
    [DataRow("Spanish")]
    [DataRow("French")]
    [DataRow("Italian")]
    public void EveryPromptThatAcceptsInventory_OffersTheCommand(string language)
    {
        Messages messages = GetMessages(language);

        string[] silent = PromptsOfferingInventory
            .Where(key => !messages.GetMessage(key).Contains("'i'"))
            .ToArray();

        Assert.AreEqual(0, silent.Length, $"{language} prompts not mentioning 'i': {string.Join(", ", silent)}");
    }

    [TestMethod]
    [DataRow("English")]
    [DataRow("Spanish")]
    [DataRow("French")]
    [DataRow("Italian")]
    public void InventoryDisplay_HasNoPlaceholders(string language)
    {
        Player player = new Player
        {
            Gold = 20,
            HealthPoints = 10,
            Weapon = new Weapon("long sword", 12, "-)=====>")
        };
        Messages messages = GetMessages(language);

        List<string> withoutArmor = player.GetInventoryDisplay(messages);
        new Merchant().BuyArmor(player);
        List<string> withArmor = player.GetInventoryDisplay(messages);

        string[] placeholders = withoutArmor.Concat(withArmor)
            .Where(line => line.Contains('[') && line.Contains(']'))
            .ToArray();

        Assert.AreEqual(0, placeholders.Length, string.Join(" | ", placeholders));
    }
}
