[TestClass]
public class MerchantMessagesTests
{
    private static readonly string[] NewKeys =
    {
        "roll_gold",
        "stats_gold",
        "stats_armor",
        "stats_protection",
        "magical_armor_name",
        "magical_weapon_format",
        "armor_absorbed",
        "armor_shattered",
        "merchant_intro",
        "merchant_gold",
        "merchant_item_armor",
        "merchant_item_weapon",
        "merchant_sold_out",
        "merchant_leave_option",
        "merchant_prompt",
        "merchant_cannot_afford",
        "merchant_already_sold",
        "merchant_bought_armor",
        "merchant_bought_weapon",
        "merchant_farewell"
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
    public void EveryMerchantKey_IsTranslated(string language)
    {
        Messages messages = GetMessages(language);

        string[] missing = NewKeys
            .Where(key => messages.GetMessage(key) == $"[{key}]")
            .ToArray();

        Assert.AreEqual(0, missing.Length, $"{language} is missing: {string.Join(", ", missing)}");
    }

    [TestMethod]
    [DataRow("English", "magical long sword")]
    [DataRow("Spanish", "espada larga magico")]
    [DataRow("French", "epee longue magique")]
    [DataRow("Italian", "spada lunga magico")]
    public void FormatWeaponForDisplay_ShowsMagicalWeaponInEachLanguage(string language, string expected)
    {
        Weapon weapon = new Weapon("long sword", 16, "-)=====>") { IsMagical = true };

        Assert.AreEqual(expected, GetMessages(language).FormatWeaponForDisplay(weapon));
    }

    [TestMethod]
    public void FormatWeaponForDisplay_ShowsPlainWeaponUnchanged()
    {
        Weapon weapon = new Weapon("dagger", 6, "-)==>");

        Assert.AreEqual("daga", GetMessages("Spanish").FormatWeaponForDisplay(weapon));
    }
}
