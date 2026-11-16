[TestClass]
public class CommandPromptTests
{
    private static Messages English()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        return messages;
    }

    private static Player TestPlayer() => new Player
    {
        Name = "TestHero",
        Gold = 12,
        HealingPotions = 2,
        Weapon = new Weapon("long sword", 12, "-)=====>")
    };

    // Runs one Ask against scripted input. End of input re-prompts forever (as the game always has),
    // so a script that runs out fails the test instead of hanging the run.
    private static (string command, string output) Ask(CommandSet set, string script, Player? player = null, Action? afterInvalid = null)
    {
        Messages messages = English();
        var output = new StringWriter();
        var prompt = new CommandPrompt(new StringReader(script), output, messages);

        Task<string> ask = Task.Run(() => prompt.Ask(set, player, afterInvalid));
        if (!ask.Wait(TimeSpan.FromSeconds(2)))
            Assert.Fail("Command prompt never returned: the script ran out of valid commands.");

        return (ask.Result, output.ToString());
    }

    private static string Lines(params string[] lines) => string.Join(Environment.NewLine, lines) + Environment.NewLine;

    // --- Main menu ---

    [TestMethod]
    [DataRow("1", "play")]
    [DataRow("2", "quit")]
    [DataRow("  1  ", "play")]
    [DataRow("  2  ", "quit")]
    public void MainMenu_ReturnsTheCommand(string typed, string expected)
    {
        Assert.AreEqual(expected, Ask(CommandSet.MainMenu, typed + "\n").command);
    }

    // No player exists yet, so "i" is just invalid input here.
    [TestMethod]
    [DataRow("3")]
    [DataRow("abc")]
    [DataRow("")]
    [DataRow("i")]
    public void MainMenu_RejectsOtherInput_WithInvalid_ThenAsksAgain(string typed)
    {
        Messages m = English();
        var (command, output) = Ask(CommandSet.MainMenu, typed + "\n2\n");

        Assert.AreEqual("quit", command);
        Assert.AreEqual(
            m.GetMessage("menu") + Environment.NewLine + m.GetMessage("enter_choice") +
            m.GetMessage("invalid") + Environment.NewLine +
            m.GetMessage("menu") + Environment.NewLine + m.GetMessage("enter_choice"),
            output);
    }

    // --- Path ---

    [TestMethod]
    [DataRow("n", "north")]
    [DataRow("north", "north")]
    [DataRow("s", "south")]
    [DataRow("south", "south")]
    [DataRow("e", "exit")]
    [DataRow("exit", "exit")]
    [DataRow("N", "north")]
    [DataRow("NORTH", "north")]
    [DataRow("S", "south")]
    [DataRow("E", "exit")]
    [DataRow("  north  ", "north")]
    public void Path_ReturnsTheCommand(string typed, string expected)
    {
        Assert.AreEqual(expected, Ask(CommandSet.Path, typed + "\n", TestPlayer()).command);
    }

    [TestMethod]
    [DataRow("west")]
    [DataRow("")]
    [DataRow("   ")]
    [DataRow("1")]
    [DataRow("2")]
    [DataRow("3")]
    public void Path_RejectsOtherInput_WithPathInvalid_ThenAsksAgain(string typed)
    {
        Messages m = English();
        var (command, output) = Ask(CommandSet.Path, typed + "\nn\n", TestPlayer());

        Assert.AreEqual("north", command);
        Assert.AreEqual(
            m.GetMessage("path_prompt_full") + Environment.NewLine + m.GetMessage("enter_choice") +
            m.GetMessage("path_invalid") + Environment.NewLine +
            m.GetMessage("path_prompt_full") + Environment.NewLine + m.GetMessage("enter_choice"),
            output);
    }

    // --- Facing the dragon, before the fight ---

    [TestMethod]
    [DataRow("a", "attack")]
    [DataRow("attack", "attack")]
    [DataRow("ATTACK", "attack")]
    [DataRow("  a  ", "attack")]
    [DataRow("r", "retreat")]
    [DataRow("retreat", "retreat")]
    [DataRow("RETREAT", "retreat")]
    public void DragonEncounter_ReturnsTheCommand(string typed, string expected)
    {
        Assert.AreEqual(expected, Ask(CommandSet.DragonEncounter, typed + "\n", TestPlayer()).command);
    }

    // A healing potion can only be drunk during the dragon fight, not before it.
    [TestMethod]
    [DataRow("d")]
    [DataRow("drink")]
    [DataRow("fight")]
    [DataRow("1")]
    [DataRow("")]
    public void DragonEncounter_RejectsOtherInput_WithInvalid_ThenAsksAgain(string typed)
    {
        Messages m = English();
        var (command, output) = Ask(CommandSet.DragonEncounter, typed + "\nr\n", TestPlayer());

        Assert.AreEqual("retreat", command);
        Assert.AreEqual(
            m.GetMessage("dragon_encounter_prompt") + Environment.NewLine + m.GetMessage("enter_choice") +
            m.GetMessage("invalid") + Environment.NewLine +
            m.GetMessage("dragon_encounter_prompt") + Environment.NewLine + m.GetMessage("enter_choice"),
            output);
    }

    // --- During the dragon fight ---

    [TestMethod]
    [DataRow("a", "attack")]
    [DataRow("attack", "attack")]
    [DataRow("A", "attack")]
    [DataRow("d", "drink")]
    [DataRow("drink", "drink")]
    [DataRow("D", "drink")]
    [DataRow("DRINK", "drink")]
    [DataRow("  drink  ", "drink")]
    [DataRow("r", "retreat")]
    [DataRow("retreat", "retreat")]
    [DataRow("R", "retreat")]
    public void Combat_ReturnsTheCommand(string typed, string expected)
    {
        Assert.AreEqual(expected, Ask(CommandSet.Combat, typed + "\n", TestPlayer()).command);
    }

    [TestMethod]
    [DataRow("fight")]
    [DataRow("1")]
    [DataRow("2")]
    [DataRow("   ")]
    public void Combat_RejectsOtherInput_WithInvalid_ThenAsksAgain(string typed)
    {
        Messages m = English();
        var (command, output) = Ask(CommandSet.Combat, typed + "\na\n", TestPlayer());

        Assert.AreEqual("attack", command);
        // The combat prompt has no "Enter choice: " line.
        Assert.AreEqual(Lines(m.GetMessage("attack_prompt"), m.GetMessage("invalid"), m.GetMessage("attack_prompt")), output);
    }

    // --- At the merchant ---

    [TestMethod]
    [DataRow("a", "armor")]
    [DataRow("armor", "armor")]
    [DataRow("ARMOR", "armor")]
    [DataRow("w", "weapon")]
    [DataRow("weapon", "weapon")]
    [DataRow("W", "weapon")]
    [DataRow("p", "potion")]
    [DataRow("  p  ", "potion")]
    [DataRow("potion", "potion")]
    [DataRow("P", "potion")]
    [DataRow("POTION", "potion")]
    [DataRow("  potion  ", "potion")]
    [DataRow("l", "leave")]
    [DataRow("leave", "leave")]
    [DataRow(" Leave ", "leave")]
    public void Merchant_ReturnsTheCommand(string typed, string expected)
    {
        Assert.AreEqual(expected, Ask(CommandSet.Merchant, typed + "\n", TestPlayer()).command);
    }

    // After invalid input the merchant shows the wares again before asking; after the Inventory it doesn't,
    // so the Inventory isn't buried under the wares list.
    [TestMethod]
    [DataRow("buy")]
    [DataRow("x")]
    [DataRow("1")]
    [DataRow("")]
    [DataRow("   ")]
    public void Merchant_RejectsOtherInput_ShowsTheWaresAgain_ThenAsks(string typed)
    {
        Messages m = English();
        Player player = TestPlayer();
        var output = new StringWriter();
        var prompt = new CommandPrompt(new StringReader(typed + "\ni\nl\n"), output, m);

        string command = prompt.Ask(CommandSet.Merchant, player, afterInvalid: () => output.WriteLine("<wares>"));

        Assert.AreEqual("leave", command);
        Assert.AreEqual(
            m.GetMessage("merchant_prompt") +
            Lines(m.GetMessage("invalid"), "<wares>") +
            m.GetMessage("merchant_prompt") +
            Lines(player.GetInventoryDisplay(m).ToArray()) +
            m.GetMessage("merchant_prompt"),
            output.ToString());
    }

    // --- Inventory: a free action wherever there is a player ---

    [TestMethod]
    [DataRow("i")]
    [DataRow("inventory")]
    [DataRow("I")]
    [DataRow("INVENTORY")]
    [DataRow("  i  ")]
    [DataRow(" Inventory ")]
    public void Inventory_ShowsTheInventory_ThenAsksAgain_WithoutReturning(string typed)
    {
        Messages m = English();
        Player player = TestPlayer();
        var (command, output) = Ask(CommandSet.Path, typed + "\ns\n", player);

        Assert.AreEqual("south", command);
        Assert.AreEqual(
            m.GetMessage("path_prompt_full") + Environment.NewLine + m.GetMessage("enter_choice") +
            Lines(player.GetInventoryDisplay(m).ToArray()) +
            m.GetMessage("path_prompt_full") + Environment.NewLine + m.GetMessage("enter_choice"),
            output);
    }

    // Every prompt in the adventure: the fork, the merchant, facing the dragon, and the fight.
    public static IEnumerable<object[]> AdventureSets() => new[]
    {
        new object[] { "Path", "n" },
        new object[] { "Merchant", "l" },
        new object[] { "DragonEncounter", "r" },
        new object[] { "Combat", "r" },
    };

    private static CommandSet SetNamed(string name) => name switch
    {
        "Path" => CommandSet.Path,
        "Merchant" => CommandSet.Merchant,
        "DragonEncounter" => CommandSet.DragonEncounter,
        "Combat" => CommandSet.Combat,
        _ => throw new ArgumentException(name)
    };

    [TestMethod]
    [DynamicData(nameof(AdventureSets), DynamicDataSourceType.Method)]
    public void Inventory_IsAFreeAction_AtEveryAdventurePrompt(string setName, string validCommand)
    {
        Messages m = English();
        var (_, output) = Ask(SetNamed(setName), "i\n" + validCommand + "\n", TestPlayer());

        StringAssert.Contains(output, m.GetMessage("inventory_header"));
        Assert.IsFalse(output.Contains(m.GetMessage("invalid")), "Inventory must not count as invalid input.");
    }

    [TestMethod]
    [DataRow("inv")]
    [DataRow("items")]
    public void Inventory_RejectsOtherWords(string typed)
    {
        Messages m = English();
        var (_, output) = Ask(CommandSet.Path, typed + "\nn\n", TestPlayer());

        StringAssert.Contains(output, m.GetMessage("path_invalid"));
        Assert.IsFalse(output.Contains(m.GetMessage("inventory_header")));
    }
}
