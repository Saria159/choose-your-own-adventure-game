[TestClass]
public class InventoryTests
{
    private static Messages English()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        return messages;
    }

    private static Player NewFighter()
    {
        Player player = new Player(new FakeDie(7));
        player.Weapon = new Weapon("long sword", 12, "-)=====>");
        player.RollGold();
        return player;
    }

    [TestMethod]
    public void NewPlayer_Inventory_ShowsWeaponNoArmorOnePotionAndRolledGold()
    {
        Player player = NewFighter();

        List<string> lines = player.GetInventoryDisplay(English());

        CollectionAssert.AreEqual(new[]
        {
            "----- Inventory -----",
            "Weapon: long sword",
            "Weapon's Maximum Damage Points: 12",
            "Armor: none",
            "Healing Potions: 1",
            "Gold: 17"
        }, lines);
    }

    [TestMethod]
    public void Inventory_ShowsArmorAndRemainingProtection()
    {
        Player player = NewFighter();
        player.Gold = 20;
        new Merchant().BuyArmor(player);
        player.TakeDamage(3);

        List<string> lines = player.GetInventoryDisplay(English());

        CollectionAssert.Contains(lines, "Armor: magical armor");
        CollectionAssert.Contains(lines, "Protection: 7");
        CollectionAssert.DoesNotContain(lines, "Armor: none");
    }

    [TestMethod]
    public void Inventory_ShowsNoneAgain_AfterArmorShatters()
    {
        Player player = NewFighter();
        player.HealthPoints = 20;
        player.Gold = 20;
        new Merchant().BuyArmor(player);
        player.TakeDamage(10);

        List<string> lines = player.GetInventoryDisplay(English());

        CollectionAssert.Contains(lines, "Armor: none");
        Assert.IsFalse(lines.Any(line => line.StartsWith("Protection")));
    }

    [TestMethod]
    public void Inventory_ShowsMagicalWeapon_WithRaisedMaximumDamage()
    {
        Player player = NewFighter();
        player.Gold = 20;
        new Merchant().BuyWeapon(player);

        List<string> lines = player.GetInventoryDisplay(English());

        CollectionAssert.Contains(lines, "Weapon: magical long sword");
        CollectionAssert.Contains(lines, "Weapon's Maximum Damage Points: 16");
    }

    [TestMethod]
    public void Inventory_ShowsZeroPotions()
    {
        Player player = NewFighter();
        player.HealingPotions = 0;

        CollectionAssert.Contains(player.GetInventoryDisplay(English()), "Healing Potions: 0");
    }

    [TestMethod]
    public void Inventory_LeavesOutCharacterStats()
    {
        Player player = NewFighter();
        player.Name = "Bob";
        player.Race = "Elf";
        player.Occupation = "fighter";
        player.Strength = 14;
        player.Agility = 9;
        player.HealthPoints = 18;

        List<string> lines = player.GetInventoryDisplay(English());

        string[] statPrefixes = { "Name", "Race", "Occupation", "Strength", "Agility", "Health" };
        string[] leaked = lines.Where(line => statPrefixes.Any(p => line.StartsWith(p))).ToArray();
        Assert.AreEqual(0, leaked.Length, string.Join(", ", leaked));
    }

    // The Inventory command itself is tested in CommandPromptTests.
}
