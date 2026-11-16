[TestClass]
public class HealingPotionMessagesTests
{
    private static readonly string[] NewKeys =
    {
        "stats_healing_potions",
        "merchant_item_potion",
        "merchant_potions_left",
        "merchant_potions_sold_out",
        "merchant_bought_potion",
        "potion_healed",
        "potion_refused_maximum_health",
        "potion_refused_none_left"
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
    public void EveryHealingPotionKey_IsTranslated(string language)
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
    public void AttackPrompt_OffersTheDrinkCommand(string language)
    {
        StringAssert.Contains(GetMessages(language).GetMessage("attack_prompt"), "'d'");
    }
}
