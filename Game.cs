public class Game
{
    public void StartGame()
    {
        // Title screen
        Console.WriteLine("=====================================");
        Console.WriteLine("Welcome to Choose Your Adventure Game");
        Console.WriteLine("=====================================\n");

        // Instantiate Die first (per sequence diagram)
        Die die = new Die();

        Dragon dragon = new Dragon(die);

        // Language selection shown before Messages dictionary is loaded
        Console.WriteLine("Select language / Selecciona idioma / Choisissez la langue / Scegli la lingua:");
        Console.WriteLine("1. English  2. Espanol  3. Francais  4. Italiano");
        Console.Write("Enter choice: ");
        string? langInput = Console.ReadLine()?.Trim();
        string selectedLanguage = langInput switch { "2" => "Spanish", "3" => "French", "4" => "Italian", _ => "English" };

        // Initialize Messages with selected language (per sequence diagram)

        Messages messages = new Messages();
        messages.SetCurrentLanguage(selectedLanguage);
        messages.ReadDictionary();

        CommandPrompt prompt = new CommandPrompt(Console.In, Console.Out, messages);

        // Display welcome message
        Console.WriteLine();
        Console.WriteLine(messages.GetMessage("welcome"));
        Console.WriteLine();

        // Main menu loop
        bool running = true;
        while (running)
        {
            if (prompt.Ask(CommandSet.MainMenu) == "play")
            {
                Player player = new Player();
                player.CreateCharacter(messages);

                // One merchant per adventure, so sold-out items stay sold out between South trips.
                Merchant merchant = new Merchant();

                bool inAdventureMenu = true;
                while (inAdventureMenu)
                {
                    string selectedPath = prompt.Ask(CommandSet.Path, player);

                    if (selectedPath == "exit")
                    {
                        Console.WriteLine(messages.GetMessage("adventure_ends"));
                        running = false;
                        inAdventureMenu = false;
                    }
                    else if (selectedPath == "south")
                    {
                        Console.WriteLine(messages.GetMessage("south_path_narrative"));
                        merchant.RunEncounter(player, messages, prompt);
                    }
                    else if (selectedPath == "north")
                    {
                        HandleDragonEncounter(player, dragon, messages, prompt);

                        running = false;
                        inAdventureMenu = false;
                    }
                }
            }
            else
            {
                Console.WriteLine(messages.GetMessage("goodbye"));
                running = false;
            }
        }
    }

    public void EndGame(bool playerWon, bool playerRetreated, Messages messages)
    {
        if (playerRetreated)
            Console.WriteLine(messages.GetMessage("retreat"));
        else if (playerWon)
            Console.WriteLine(messages.GetMessage("victory"));
        else
            Console.WriteLine(messages.GetMessage("defeat"));
    }

    private void HandleDragonEncounter(Player player, Dragon dragon, Messages messages, CommandPrompt prompt)
    {
        Console.WriteLine(messages.GetMessage("north_path_narrative"));

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(string.Format(messages.GetMessage("dragon_intro"), dragon.Name));
        Console.ResetColor();

        Console.WriteLine(messages.GetMessage("dragon_stats_intro"));
        dragon.DisplayStats(messages);
        if (prompt.Ask(CommandSet.DragonEncounter, player) == "retreat")
        {
            Console.WriteLine(messages.GetMessage("retreat"));
            return;
        }

        Combat combat = new Combat(player, dragon, messages, prompt);
        bool playerWon = combat.StartCombat();

        if (!combat.PlayerRetreated) // combat already printed the retreat message
            EndGame(playerWon, false, messages);
    }
}
