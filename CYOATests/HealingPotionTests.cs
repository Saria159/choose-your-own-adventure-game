[TestClass]
public class HealingPotionTests
{
    [TestMethod]
    public void NewPlayer_CarriesOneHealingPotion()
    {
        Player player = new Player();

        Assert.AreEqual(1, player.HealingPotions);
    }

    [TestMethod]
    public void RollHealthPoints_SetsMaximumHealthToTheRolledHealth()
    {
        Player player = new Player(new FakeDie(12));

        player.RollHealthPoints(3);

        Assert.AreEqual(15, player.HealthPoints);
        Assert.AreEqual(15, player.MaxHealthPoints);
    }

    [TestMethod]
    public void DrinkHealingPotion_HealsByTheRoll_AndUsesAPotion()
    {
        Player player = new Player(new FakeDie(5)) { HealthPoints = 6, MaxHealthPoints = 20 };

        int healed = player.DrinkHealingPotion();

        Assert.AreEqual(5, healed);
        Assert.AreEqual(11, player.HealthPoints);
        Assert.AreEqual(0, player.HealingPotions);
    }

    [TestMethod]
    public void DrinkHealingPotion_NeverHealsAboveMaximumHealth()
    {
        Player player = new Player(new FakeDie(8)) { HealthPoints = 17, MaxHealthPoints = 20 };

        int healed = player.DrinkHealingPotion();

        Assert.AreEqual(3, healed);
        Assert.AreEqual(20, player.HealthPoints);
    }

    [TestMethod]
    public void DrinkHealingPotion_IsRefused_AtMaximumHealth_AndKeepsThePotion()
    {
        Player player = new Player(new FakeDie()) { HealthPoints = 20, MaxHealthPoints = 20 };

        int healed = player.DrinkHealingPotion();

        Assert.AreEqual(0, healed);
        Assert.AreEqual(20, player.HealthPoints);
        Assert.AreEqual(1, player.HealingPotions);
    }

    [TestMethod]
    public void DrinkHealingPotion_IsRefused_WithNoPotions()
    {
        Player player = new Player(new FakeDie()) { HealthPoints = 5, MaxHealthPoints = 20, HealingPotions = 0 };

        int healed = player.DrinkHealingPotion();

        Assert.AreEqual(0, healed);
        Assert.AreEqual(5, player.HealthPoints);
        Assert.AreEqual(0, player.HealingPotions);
    }

    [TestMethod]
    public void DrinkHealingPotion_NeverRestoresProtection()
    {
        Player player = new Player(new FakeDie(4))
        {
            HealthPoints = 10,
            MaxHealthPoints = 20,
            Armor = new Armor("magical armor", 3, "[#]")
        };

        player.DrinkHealingPotion();

        Assert.AreEqual(14, player.HealthPoints);
        Assert.AreEqual(3, player.Armor!.Protection);
    }

    [TestMethod]
    public void DrinkHealingPotion_DoesNotBringBackShatteredArmor()
    {
        Player player = new Player(new FakeDie(4))
        {
            HealthPoints = 15,
            MaxHealthPoints = 20,
            Armor = new Armor("magical armor", 3, "[#]")
        };
        player.TakeDamage(8);

        player.DrinkHealingPotion();

        Assert.AreEqual(14, player.HealthPoints);
        Assert.IsNull(player.Armor);
    }
}
