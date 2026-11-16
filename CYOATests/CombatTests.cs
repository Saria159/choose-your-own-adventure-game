[TestClass]
public class CombatTests
{
    private Messages CreateEnglishMessages()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();
        return messages;
    }

    private Player CreateTestPlayer()
    {
        return new Player
        {
            Name = "TestHero",
            Race = "Human",
            Occupation = "Fighter",
            Strength = 15,
            Agility = 12,
            HealthPoints = 25,
            Weapon = new Weapon("long sword", 12, "-)=====>")
        };
    }

    private Dragon CreateTestDragon()
    {
        Dragon dragon = new Dragon(new Die())
        {
            Name = "Smolderfang",
            Strength = 18,
            Agility = 10,
            HealthPoints = 30,
            Weapon = new Weapon("claws", 12, "<<< claws >>>")
        };

        return dragon;
    }

    [TestMethod]
    public void Combat_Constructor_InitializesPlayerAndDragon()
    {
        Player player = new Player();
        Dragon dragon = new Dragon(new Die());
        Messages messages = CreateEnglishMessages();

        Combat combat = new Combat(player, dragon, messages);

        Assert.AreSame(player, combat.player);
        Assert.AreSame(dragon, combat.dragon);
    }

    [TestMethod]
    public void Combat_PlayerRetreated_DefaultsToFalse()
    {
        Player player = new Player();
        Dragon dragon = new Dragon(new Die());
        Messages messages = CreateEnglishMessages();

        Combat combat = new Combat(player, dragon, messages);

        Assert.IsFalse(combat.PlayerRetreated);
    }

    [TestMethod]
    public void GetCombatStatsDisplay_ReturnsNonEmptyString()
    {
        Player player = CreateTestPlayer();
        Dragon dragon = CreateTestDragon();
        Messages messages = CreateEnglishMessages();

        Combat combat = new Combat(player, dragon, messages);

        string result = combat.GetCombatStatsDisplay();

        Assert.IsFalse(string.IsNullOrWhiteSpace(result));
    }

    [TestMethod]
    public void GetCombatStatsDisplay_IncludesCombatHeaders()
    {
        Player player = CreateTestPlayer();
        Dragon dragon = CreateTestDragon();
        Messages messages = CreateEnglishMessages();

        Combat combat = new Combat(player, dragon, messages);

        string result = combat.GetCombatStatsDisplay();

        StringAssert.Contains(result, messages.GetMessage("combat_stats_header"));
        StringAssert.Contains(result, messages.GetMessage("combat_player_header"));
        StringAssert.Contains(result, messages.GetMessage("combat_dragon_header"));
    }

    [TestMethod]
    public void GetCombatStatsDisplay_IncludesPlayerCombatStats()
    {
        Player player = CreateTestPlayer();
        Dragon dragon = CreateTestDragon();
        Messages messages = CreateEnglishMessages();

        Combat combat = new Combat(player, dragon, messages);

        string result = combat.GetCombatStatsDisplay();

        StringAssert.Contains(result, player.Name);
        StringAssert.Contains(result, player.Strength.ToString());
        StringAssert.Contains(result, player.Agility.ToString());
        StringAssert.Contains(result, player.HealthPoints.ToString());
        StringAssert.Contains(result, player.Weapon!.Type);
        StringAssert.Contains(result, player.Weapon.MaxDamage.ToString());
    }

    [TestMethod]
    public void GetCombatStatsDisplay_IncludesDragonCombatStats()
    {
        Player player = CreateTestPlayer();
        Dragon dragon = CreateTestDragon();
        Messages messages = CreateEnglishMessages();

        Combat combat = new Combat(player, dragon, messages);

        string result = combat.GetCombatStatsDisplay();

        StringAssert.Contains(result, dragon.Name);
        StringAssert.Contains(result, dragon.Strength.ToString());
        StringAssert.Contains(result, dragon.Agility.ToString());
        StringAssert.Contains(result, dragon.HealthPoints.ToString());
        StringAssert.Contains(result, dragon.Weapon.Type);
        StringAssert.Contains(result, dragon.Weapon.MaxDamage.ToString());
    }

    [TestMethod]
    public void GetCombatStatsDisplay_UsesLocalizedStatLabels()
    {
        Player player = CreateTestPlayer();
        Dragon dragon = CreateTestDragon();
        Messages messages = CreateEnglishMessages();

        Combat combat = new Combat(player, dragon, messages);

        string result = combat.GetCombatStatsDisplay();

        StringAssert.Contains(result, messages.GetMessage("stats_name"));
        StringAssert.Contains(result, messages.GetMessage("stats_strength"));
        StringAssert.Contains(result, messages.GetMessage("stats_agility"));
        StringAssert.Contains(result, messages.GetMessage("stats_health"));
        StringAssert.Contains(result, messages.GetMessage("stats_weapon"));
        StringAssert.Contains(result, messages.GetMessage("stats_damage"));
    }

    // Unlike checking the Inventory, drinking a healing potion is not a free action:
    // it spends the player's turn (no attack) and the dragon still attacks.
    [TestMethod]
    [DoNotParallelize]
    public void StartCombat_DrinkingAHealingPotion_SpendsThePlayersTurn_AndTheDragonStillAttacks()
    {
        Messages messages = CreateEnglishMessages();

        // Round 1: player attack roll 20 misses (Strength 15). Drink round: potion heals 5.
        // A player attack in the drink round would ask for a roll that isn't scripted and throw.
        FakeDie playerDie = new FakeDie(20, 5);
        Player player = new Player(playerDie)
        {
            Name = "TestHero",
            Race = "Human",
            Occupation = "Fighter",
            Strength = 15,
            Agility = 12,
            HealthPoints = 10,
            MaxHealthPoints = 25,
            HealingPotions = 1,
            Weapon = new Weapon("long sword", 12, "-)=====>")
        };

        // Three rolls for the constructor, then one miss (20 vs Strength 18) per dragon attack:
        // round 1 and the drink round.
        FakeDie dragonDie = new FakeDie(10, 10, 30, 20, 20);
        Dragon dragon = new Dragon(dragonDie) { Strength = 18, Agility = 10, HealthPoints = 30 };

        TextWriter originalOut = Console.Out;
        var output = new StringWriter();
        Console.SetOut(output);
        try
        {
            var prompt = new CommandPrompt(new StringReader("d\nr\n"), output, messages);
            Combat combat = new Combat(player, dragon, messages, prompt);

            bool playerWon = combat.StartCombat();

            Assert.IsFalse(playerWon);
            Assert.IsTrue(combat.PlayerRetreated);
            Assert.AreEqual(0, player.HealingPotions, "The potion was drunk.");
            Assert.AreEqual(15, player.HealthPoints, "The potion healed 5.");
            Assert.AreEqual(0, playerDie.RemainingRolls);
            Assert.AreEqual(0, dragonDie.RemainingRolls, "The dragon attacked in the drink round.");
            Assert.AreEqual(30, dragon.HealthPoints, "The player never hit the dragon.");
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    // Checking the Inventory mid-fight is a free action: the dragon gets no extra attack.
    [TestMethod]
    [DoNotParallelize]
    public void StartCombat_CheckingTheInventory_GivesTheDragonNoTurn()
    {
        Messages messages = CreateEnglishMessages();

        FakeDie playerDie = new FakeDie(20); // round 1 attack misses
        Player player = new Player(playerDie)
        {
            Name = "TestHero",
            Occupation = "Fighter",
            Strength = 15,
            HealthPoints = 10,
            MaxHealthPoints = 25,
            Weapon = new Weapon("long sword", 12, "-)=====>")
        };

        // Constructor rolls, then exactly one dragon attack (round 1). An attack after "i" would throw.
        FakeDie dragonDie = new FakeDie(10, 10, 30, 20);
        Dragon dragon = new Dragon(dragonDie) { Strength = 18, Agility = 10, HealthPoints = 30 };

        TextWriter originalOut = Console.Out;
        var output = new StringWriter();
        Console.SetOut(output);
        try
        {
            var prompt = new CommandPrompt(new StringReader("i\ni\nr\n"), output, messages);
            new Combat(player, dragon, messages, prompt).StartCombat();

            Assert.AreEqual(0, dragonDie.RemainingRolls);
            Assert.AreEqual(10, player.HealthPoints);
            StringAssert.Contains(output.ToString(), messages.GetMessage("inventory_header"));
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }
}
