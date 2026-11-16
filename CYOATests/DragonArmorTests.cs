[TestClass]
public class DragonArmorTests
{
    private static Messages GetMessages()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        return messages;
    }

    // Dragon constructor rolls Strength, Agility, Health; Attack then rolls hit, block, damage.
    private static Dragon ScriptedDragon(int hitRoll, int blockRoll, int damageRoll) =>
        new Dragon(new FakeDie(20, 1, 10, hitRoll, blockRoll, damageRoll));

    private static Player ArmoredPlayer(int protection) => new Player
    {
        Name = "Ana",
        Agility = 5,
        HealthPoints = 10,
        Armor = new Armor("magical armor", protection, "[|#|]")
    };

    [TestMethod]
    public void Attack_OnArmoredPlayer_ArmorAbsorbsTheHit()
    {
        Dragon dragon = ScriptedDragon(hitRoll: 1, blockRoll: 20, damageRoll: 6);
        Player player = ArmoredPlayer(10);

        string output = dragon.Attack(player, GetMessages());

        Assert.AreEqual(4, player.Armor!.Protection);
        Assert.AreEqual(10, player.HealthPoints);
        StringAssert.Contains(output, "Your magical armor absorbs 6 damage! Protection remaining: 4.");
    }

    [TestMethod]
    public void Attack_OverflowingArmor_ShattersItAndHurtsHealth()
    {
        Dragon dragon = ScriptedDragon(hitRoll: 1, blockRoll: 20, damageRoll: 8);
        Player player = ArmoredPlayer(3);

        string output = dragon.Attack(player, GetMessages());

        Assert.IsNull(player.Armor);
        Assert.AreEqual(5, player.HealthPoints);
        StringAssert.Contains(output, "Your magical armor absorbs 3 damage and shatters!");
        StringAssert.Contains(output, "Smolderfang deals 5 damage.");
        StringAssert.Contains(output, "Ana's health points are now 5.");
    }

    [TestMethod]
    public void Attack_BlockedByAgility_LeavesProtectionUntouched()
    {
        Dragon dragon = new Dragon(new FakeDie(20, 1, 10, 1, 1));
        Player player = ArmoredPlayer(10);

        dragon.Attack(player, GetMessages());

        Assert.AreEqual(10, player.Armor!.Protection);
        Assert.AreEqual(10, player.HealthPoints);
    }
}
