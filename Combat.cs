public class Combat
{
    public Player player { get; set; }
    public Dragon dragon { get; set; }
    public Messages messages { get; set; }

    public bool PlayerRetreated { get; private set; }

    public Combat(Player player, Dragon dragon, Messages messages)
    {
        this.player = player;
        this.dragon = dragon;
        this.messages = messages;
        PlayerRetreated = false;
    }

    public bool StartCombat()
    {
        Console.WriteLine(GetCombatStatsDisplay());

        bool playerAttacks = true;

        while (true)
        {
            // In a drink round the player's turn was spent on the potion, so only the dragon attacks.
            if (playerAttacks)
            {
                PlayerAttacksDragonSequence();

                if (dragon.HealthPoints <= 0)
                    return true;
            }

            DragonAttacksPlayerSequence();

            if (player.HealthPoints <= 0)
                return false;

            string choice = PromptForCombatChoice();

            if (choice == "retreat")
            {
                PlayerRetreated = true;
                Console.WriteLine(messages.GetMessage("retreat_combat"));
                return false;
            }

            playerAttacks = choice == "attack";

            if (!playerAttacks)
                PlayerDrinksHealingPotionSequence();
        }
    }

    private string PromptForCombatChoice()
    {
        while (true)
        {
            Console.WriteLine(messages.GetMessage("attack_prompt"));
            string? input = Console.ReadLine()?.Trim().ToLower();

            if (input == "a" || input == "attack")
                return "attack";

            if (input == "d" || input == "drink")
                return "drink";

            if (input == "r" || input == "retreat")
                return "retreat";

            Console.WriteLine(messages.GetMessage("invalid"));
        }
    }

    // A refused drink still uses the player's turn.
    public void PlayerDrinksHealingPotionSequence()
    {
        if (player.HealingPotions <= 0)
        {
            Console.WriteLine(string.Format(messages.GetMessage("potion_refused_none_left"), player.Name));
            return;
        }

        int healed = player.DrinkHealingPotion();

        if (healed == 0)
            Console.WriteLine(string.Format(messages.GetMessage("potion_refused_maximum_health"), player.Name));
        else
            Console.WriteLine(string.Format(messages.GetMessage("potion_healed"), player.Name, healed, player.HealthPoints));
    }

    public string GetCombatStatsDisplay()
    {
        var output = new List<string>();

        output.Add(messages.GetMessage("combat_stats_header"));
        output.Add(messages.GetMessage("combat_player_header"));
        output.Add(messages.GetMessage("stats_name") + player.Name);
        output.Add(messages.GetMessage("stats_strength") + player.Strength);
        output.Add(messages.GetMessage("stats_agility") + player.Agility);
        output.Add(messages.GetMessage("stats_health") + player.HealthPoints);
        output.Add(messages.GetMessage("stats_healing_potions") + player.HealingPotions);

        if (player.Weapon != null)
        {
            output.Add(messages.GetMessage("stats_weapon") + messages.FormatWeaponForDisplay(player.Weapon));
            output.Add(messages.GetMessage("stats_damage") + player.Weapon.MaxDamage);
        }

        if (player.Armor != null)
        {
            output.Add(messages.GetMessage("stats_armor") + messages.GetMessage("magical_armor_name"));
            output.Add(messages.GetMessage("stats_protection") + player.Armor.Protection);
        }

        output.Add("");
        output.Add(messages.GetMessage("combat_dragon_header"));
        output.Add(messages.GetMessage("stats_name") + dragon.Name);
        output.Add(messages.GetMessage("stats_strength") + dragon.Strength);
        output.Add(messages.GetMessage("stats_agility") + dragon.Agility);
        output.Add(messages.GetMessage("stats_health") + dragon.HealthPoints);
        output.Add(messages.GetMessage("stats_weapon") + messages.TranslateWeaponForDisplay(dragon.Weapon.Type));
        output.Add(messages.GetMessage("stats_damage") + dragon.Weapon.MaxDamage);

        return string.Join(Environment.NewLine, output);
    }

    public void PlayerAttacksDragonSequence()
    {
        Console.WriteLine(player.Attack(dragon, messages));
    }

    public void DragonAttacksPlayerSequence()
    {
        Console.WriteLine(dragon.Attack(player, messages));
    }
}