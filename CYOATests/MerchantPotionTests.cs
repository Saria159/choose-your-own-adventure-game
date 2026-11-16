[TestClass]
public class MerchantPotionTests
{
    private static Player PlayerWithGold(int gold) => new Player
    {
        Gold = gold,
        HealthPoints = 10,
        Weapon = new Weapon("long sword", 12, "-)=====>")
    };

    [TestMethod]
    public void BuyHealingPotion_CostsFiveGold_AndGivesOnePotion()
    {
        Merchant merchant = new Merchant();
        Player player = PlayerWithGold(20);

        bool bought = merchant.BuyHealingPotion(player);

        Assert.IsTrue(bought);
        Assert.AreEqual(15, player.Gold);
        Assert.AreEqual(2, player.HealingPotions);
        Assert.AreEqual(2, merchant.HealingPotionsLeft);
    }

    [TestMethod]
    public void BuyHealingPotion_IsAllowed_WithExactlyFiveGold()
    {
        Player player = PlayerWithGold(5);

        Assert.IsTrue(new Merchant().BuyHealingPotion(player));
        Assert.AreEqual(0, player.Gold);
    }

    [TestMethod]
    public void BuyHealingPotion_IsRefused_WithFourGold_AndChangesNothing()
    {
        Merchant merchant = new Merchant();
        Player player = PlayerWithGold(4);

        bool bought = merchant.BuyHealingPotion(player);

        Assert.IsFalse(bought);
        Assert.AreEqual(4, player.Gold);
        Assert.AreEqual(1, player.HealingPotions);
        Assert.AreEqual(3, merchant.HealingPotionsLeft);
    }

    [TestMethod]
    public void BuyHealingPotion_FourthPurchase_IsRefused_AsSoldOut()
    {
        Merchant merchant = new Merchant();
        Player player = PlayerWithGold(30);

        merchant.BuyHealingPotion(player);
        merchant.BuyHealingPotion(player);
        merchant.BuyHealingPotion(player);
        bool fourth = merchant.BuyHealingPotion(player);

        Assert.IsFalse(fourth);
        Assert.AreEqual(15, player.Gold);
        Assert.AreEqual(4, player.HealingPotions);
        Assert.AreEqual(0, merchant.HealingPotionsLeft);
    }

    [TestMethod]
    public void BuyingHealingPotions_DoesNotSellOutArmorOrWeapon()
    {
        Merchant merchant = new Merchant();
        Player player = PlayerWithGold(30);

        merchant.BuyHealingPotion(player);
        merchant.BuyHealingPotion(player);
        merchant.BuyHealingPotion(player);

        Assert.IsFalse(merchant.IsArmorSoldOut);
        Assert.IsFalse(merchant.IsWeaponSoldOut);
    }
}
