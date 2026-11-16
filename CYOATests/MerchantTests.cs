[TestClass]
public class MerchantTests
{
    private static Player PlayerWithGold(int gold) => new Player
    {
        Gold = gold,
        HealthPoints = 10,
        Weapon = new Weapon("long sword", 12, "-)=====>")
    };

    [TestMethod]
    public void CanAfford_IsTrue_WithExactlyFifteenGold()
    {
        Merchant merchant = new Merchant();

        Assert.IsTrue(merchant.CanAfford(PlayerWithGold(15)));
    }

    [TestMethod]
    public void CanAfford_IsFalse_WithFourteenGold()
    {
        Merchant merchant = new Merchant();

        Assert.IsFalse(merchant.CanAfford(PlayerWithGold(14)));
    }

    [TestMethod]
    public void BuyArmor_CostsFifteenGold_AndGivesTenProtection()
    {
        Merchant merchant = new Merchant();
        Player player = PlayerWithGold(20);

        bool bought = merchant.BuyArmor(player);

        Assert.IsTrue(bought);
        Assert.AreEqual(5, player.Gold);
        Assert.IsNotNull(player.Armor);
        Assert.AreEqual(10, player.Armor.Protection);
        Assert.IsTrue(merchant.IsArmorSoldOut);
    }

    [TestMethod]
    public void BuyArmor_IsRefused_WhenUnaffordable_AndChangesNothing()
    {
        Merchant merchant = new Merchant();
        Player player = PlayerWithGold(14);

        bool bought = merchant.BuyArmor(player);

        Assert.IsFalse(bought);
        Assert.AreEqual(14, player.Gold);
        Assert.IsNull(player.Armor);
        Assert.IsFalse(merchant.IsArmorSoldOut);
    }

    [TestMethod]
    public void BuyArmor_IsRefused_WhenSoldOut_AndChangesNothing()
    {
        Merchant merchant = new Merchant();
        Player player = PlayerWithGold(30);
        merchant.BuyArmor(player);
        player.Armor!.Protection = 2;

        bool bought = merchant.BuyArmor(player);

        Assert.IsFalse(bought);
        Assert.AreEqual(15, player.Gold);
        Assert.AreEqual(2, player.Armor.Protection);
    }

    [TestMethod]
    public void BuyWeapon_CostsFifteenGold_AndEnchantsCurrentWeapon()
    {
        Merchant merchant = new Merchant();
        Player player = PlayerWithGold(20);

        bool bought = merchant.BuyWeapon(player);

        Assert.IsTrue(bought);
        Assert.AreEqual(5, player.Gold);
        Assert.AreEqual("long sword", player.Weapon!.Type);
        Assert.AreEqual(16, player.Weapon.MaxDamage);
        Assert.IsTrue(player.Weapon.IsMagical);
        Assert.IsTrue(merchant.IsWeaponSoldOut);
    }

    [TestMethod]
    public void BuyWeapon_IsRefused_WhenUnaffordable_AndChangesNothing()
    {
        Merchant merchant = new Merchant();
        Player player = PlayerWithGold(14);

        bool bought = merchant.BuyWeapon(player);

        Assert.IsFalse(bought);
        Assert.AreEqual(14, player.Gold);
        Assert.AreEqual(12, player.Weapon!.MaxDamage);
        Assert.IsFalse(player.Weapon.IsMagical);
        Assert.IsFalse(merchant.IsWeaponSoldOut);
    }

    [TestMethod]
    public void BuyWeapon_IsRefused_WhenSoldOut_AndDoesNotStackEnchantment()
    {
        Merchant merchant = new Merchant();
        Player player = PlayerWithGold(30);
        merchant.BuyWeapon(player);

        bool bought = merchant.BuyWeapon(player);

        Assert.IsFalse(bought);
        Assert.AreEqual(15, player.Gold);
        Assert.AreEqual(16, player.Weapon!.MaxDamage);
    }

    [TestMethod]
    public void BuyWeapon_IsRefused_WhenPlayerHasNoWeapon_AndChangesNothing()
    {
        Merchant merchant = new Merchant();
        Player player = PlayerWithGold(20);
        player.Weapon = null;

        bool bought = merchant.BuyWeapon(player);

        Assert.IsFalse(bought);
        Assert.AreEqual(20, player.Gold);
        Assert.IsFalse(merchant.IsWeaponSoldOut);
    }

    [TestMethod]
    public void PlayerWithThirtyGold_CanBuyBothItems()
    {
        Merchant merchant = new Merchant();
        Player player = PlayerWithGold(30);

        Assert.IsTrue(merchant.BuyArmor(player));
        Assert.IsTrue(merchant.BuyWeapon(player));

        Assert.AreEqual(0, player.Gold);
        Assert.AreEqual(10, player.Armor!.Protection);
        Assert.AreEqual(16, player.Weapon!.MaxDamage);
    }
}
