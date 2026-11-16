[TestClass]
public class PlayerTests
{
    [TestMethod]
    public void NewPlayer_ShouldHaveDefaultValues()
    {
        Player player = new Player();

        Assert.AreEqual("", player.Name);
        Assert.AreEqual("", player.Race);
        Assert.AreEqual("", player.Occupation);

        Assert.AreEqual(0, player.Strength);
        Assert.AreEqual(0, player.Agility);
        Assert.AreEqual(0, player.HealthPoints);

        Assert.IsNull(player.Weapon);
    }

    [TestMethod]
    public void RollGold_GivesEleven_OnLowestRoll()
    {
        Player player = new Player(new FakeDie(1));

        player.RollGold();

        Assert.AreEqual(11, player.Gold);
    }

    [TestMethod]
    public void RollGold_GivesThirty_OnHighestRoll()
    {
        Player player = new Player(new FakeDie(20));

        player.RollGold();

        Assert.AreEqual(30, player.Gold);
    }

    [TestMethod]
    public void TakeDamage_WithoutArmor_AllDamageGoesToHealth()
    {
        Player player = new Player { HealthPoints = 10 };

        int healthLost = player.TakeDamage(4);

        Assert.AreEqual(4, healthLost);
        Assert.AreEqual(6, player.HealthPoints);
    }

    [TestMethod]
    public void TakeDamage_NeverDropsHealthBelowZero()
    {
        Player player = new Player { HealthPoints = 3 };

        player.TakeDamage(9);

        Assert.AreEqual(0, player.HealthPoints);
    }

    [TestMethod]
    public void TakeDamage_WithArmor_ProtectionAbsorbsSmallHit()
    {
        Player player = new Player { HealthPoints = 10, Armor = new Armor("magical armor", 10, "[#]") };

        int healthLost = player.TakeDamage(6);

        Assert.AreEqual(0, healthLost);
        Assert.AreEqual(10, player.HealthPoints);
        Assert.AreEqual(4, player.Armor!.Protection);
    }

    [TestMethod]
    public void TakeDamage_HitEqualToProtection_BreaksArmorAndSparesHealth()
    {
        Player player = new Player { HealthPoints = 10, Armor = new Armor("magical armor", 5, "[#]") };

        int healthLost = player.TakeDamage(5);

        Assert.AreEqual(0, healthLost);
        Assert.AreEqual(10, player.HealthPoints);
        Assert.IsNull(player.Armor);
    }

    [TestMethod]
    public void TakeDamage_HitBiggerThanProtection_OverflowGoesToHealth()
    {
        Player player = new Player { HealthPoints = 10, Armor = new Armor("magical armor", 3, "[#]") };

        int healthLost = player.TakeDamage(8);

        Assert.AreEqual(5, healthLost);
        Assert.AreEqual(5, player.HealthPoints);
        Assert.IsNull(player.Armor);
    }
}
