public class Merchant
{
    public const int ItemPrice = 15;

    public const int ArmorProtection = 10;

    public const int EnchantmentBonus = 4;

    public const int HealingPotionPrice = 5;

    public const int HealingPotionStock = 3;

    private const string ArmorArt = "[|#|]";

    private const string PotionArt = "(_)";

    public string Name { get; } = "Oswin";

    public bool IsArmorSoldOut { get; private set; }
    public bool IsWeaponSoldOut { get; private set; }
    public int HealingPotionsLeft { get; private set; } = HealingPotionStock;

    public bool CanAfford(Player player)
    {
        return player.Gold >= ItemPrice;
    }

    public bool BuyArmor(Player player)
    {
        if (IsArmorSoldOut || !CanAfford(player))
            return false;

        player.Gold -= ItemPrice;
        player.Armor = new Armor("magical armor", ArmorProtection, ArmorArt);
        IsArmorSoldOut = true;
        return true;
    }

    public bool BuyHealingPotion(Player player)
    {
        if (HealingPotionsLeft <= 0 || player.Gold < HealingPotionPrice)
            return false;

        player.Gold -= HealingPotionPrice;
        player.HealingPotions++;
        HealingPotionsLeft--;
        return true;
    }

    public bool BuyWeapon(Player player)
    {
        if (IsWeaponSoldOut || player.Weapon == null || !CanAfford(player))
            return false;

        player.Gold -= ItemPrice;
        player.Weapon.MaxDamage += EnchantmentBonus;
        player.Weapon.IsMagical = true;
        IsWeaponSoldOut = true;
        return true;
    }

    // Console loop for the South path encounter. Purchase rules live in the methods above.
    public void RunEncounter(Player player, Messages messages, CommandPrompt prompt)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(string.Format(messages.GetMessage("merchant_intro"), Name));
        Console.ResetColor();

        while (true)
        {
            ShowWares(player, messages);

            // The prompt shows the Inventory without the wares (so it isn't buried under them),
            // and shows the wares again after invalid input.
            string command = prompt.Ask(CommandSet.Merchant, player, afterInvalid: () => ShowWares(player, messages));

            switch (command)
            {
                case "armor":
                    if (IsArmorSoldOut)
                        Console.WriteLine(messages.GetMessage("merchant_already_sold"));
                    else if (BuyArmor(player))
                        Console.WriteLine(string.Format(messages.GetMessage("merchant_bought_armor"), Name));
                    else
                        Console.WriteLine(messages.GetMessage("merchant_cannot_afford"));
                    break;

                case "weapon" when player.Weapon != null:
                    if (IsWeaponSoldOut)
                        Console.WriteLine(messages.GetMessage("merchant_already_sold"));
                    else if (BuyWeapon(player))
                        Console.WriteLine(string.Format(
                            messages.GetMessage("merchant_bought_weapon"),
                            Name,
                            messages.TranslateWeaponForDisplay(player.Weapon.Type),
                            player.Weapon.MaxDamage));
                    else
                        Console.WriteLine(messages.GetMessage("merchant_cannot_afford"));
                    break;

                case "potion":
                    if (HealingPotionsLeft <= 0)
                        Console.WriteLine(messages.GetMessage("merchant_potions_sold_out"));
                    else if (BuyHealingPotion(player))
                        Console.WriteLine(string.Format(messages.GetMessage("merchant_bought_potion"), Name, player.HealingPotions));
                    else
                        Console.WriteLine(messages.GetMessage("merchant_cannot_afford"));
                    break;

                case "leave":
                    Console.WriteLine(string.Format(messages.GetMessage("merchant_farewell"), Name));
                    return;

                default:
                    Console.WriteLine(messages.GetMessage("invalid"));
                    break;
            }
        }
    }

    private void ShowWares(Player player, Messages messages)
    {
        Console.WriteLine();
        Console.WriteLine(string.Format(messages.GetMessage("merchant_gold"), player.Gold));

        Console.WriteLine(ArmorArt);
        Console.WriteLine(string.Format(messages.GetMessage("merchant_item_armor"), ItemPrice, ArmorProtection)
            + (IsArmorSoldOut ? " " + messages.GetMessage("merchant_sold_out") : ""));

        if (player.Weapon != null)
        {
            Console.WriteLine("*~" + player.Weapon.AsciiArt + "~*");
            Console.WriteLine(string.Format(
                    messages.GetMessage("merchant_item_weapon"),
                    messages.TranslateWeaponForDisplay(player.Weapon.Type),
                    ItemPrice,
                    EnchantmentBonus)
                + (IsWeaponSoldOut ? " " + messages.GetMessage("merchant_sold_out") : ""));
        }

        Console.WriteLine(PotionArt);
        Console.WriteLine(string.Format(messages.GetMessage("merchant_item_potion"), HealingPotionPrice) + " "
            + (HealingPotionsLeft > 0
                ? string.Format(messages.GetMessage("merchant_potions_left"), HealingPotionsLeft)
                : messages.GetMessage("merchant_sold_out")));

        Console.WriteLine(messages.GetMessage("merchant_leave_option"));
    }
}
