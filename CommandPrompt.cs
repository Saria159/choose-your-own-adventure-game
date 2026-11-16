// How a command prompt sits on screen.
internal enum PromptLayout
{
    // Prompt on its own line, then "Enter choice: " on the line the player types on.
    LineThenEnterChoice,
    // Prompt on its own line; the player types on the next one.
    Line,
    // Prompt with no line break; the player types right after it.
    Inline
}

// The commands one command prompt accepts, and how that prompt looks on screen.
// Commands are always English, whatever the language (ADR-0001).
public sealed class CommandSet
{
    // Before a character exists, so there is no Inventory to check.
    public static readonly CommandSet MainMenu = new(
        "menu", PromptLayout.LineThenEnterChoice, invalidKey: "invalid",
        ("play", new[] { "1" }),
        ("quit", new[] { "2" }));

    public static readonly CommandSet Path = new(
        "path_prompt_full", PromptLayout.LineThenEnterChoice, invalidKey: "path_invalid",
        ("north", new[] { "n", "north" }),
        ("south", new[] { "s", "south" }),
        ("exit", new[] { "e", "exit" }));

    // Facing the dragon, before the fight. No drink: a healing potion is only for the fight itself.
    public static readonly CommandSet DragonEncounter = new(
        "dragon_encounter_prompt", PromptLayout.LineThenEnterChoice, invalidKey: "invalid",
        ("attack", new[] { "a", "attack" }),
        ("retreat", new[] { "r", "retreat" }));

    // During the dragon fight. Drink is a command, not a free action: it spends the player's turn.
    public static readonly CommandSet Combat = new(
        "attack_prompt", PromptLayout.Line, invalidKey: "invalid",
        ("attack", new[] { "a", "attack" }),
        ("drink", new[] { "d", "drink" }),
        ("retreat", new[] { "r", "retreat" }));

    public static readonly CommandSet Merchant = new(
        "merchant_prompt", PromptLayout.Inline, invalidKey: "invalid",
        ("armor", new[] { "a", "armor" }),
        ("weapon", new[] { "w", "weapon" }),
        ("potion", new[] { "p", "potion" }),
        ("leave", new[] { "l", "leave" }));

    private readonly Dictionary<string, string> _commandsByWord = new();

    private CommandSet(string promptKey, PromptLayout layout, string invalidKey, params (string command, string[] words)[] commands)
    {
        PromptKey = promptKey;
        Layout = layout;
        InvalidKey = invalidKey;

        foreach (var (command, words) in commands)
            foreach (string word in words)
                _commandsByWord[word] = command;
    }

    internal string PromptKey { get; }
    internal PromptLayout Layout { get; }
    internal string InvalidKey { get; }

    internal string? Find(string word) => _commandsByWord.GetValueOrDefault(word);
}

// Asks the player for a command and asks again until it gets a valid one.
// Checking the Inventory is a free action whenever a player is given: it is shown and the prompt asks again.
// End of input counts as invalid input, so the prompt keeps asking (as the game always has).
public class CommandPrompt
{
    private readonly TextReader _input;
    private readonly TextWriter _output;
    private readonly Messages _messages;

    public CommandPrompt(TextReader input, TextWriter output, Messages messages)
    {
        _input = input;
        _output = output;
        _messages = messages;
    }

    // Returns the command from the set, e.g. "north" for "n". afterInvalid runs after the invalid
    // message and before asking again (the merchant uses it to show the wares again).
    public string Ask(CommandSet set, Player? player = null, Action? afterInvalid = null)
    {
        while (true)
        {
            WritePrompt(set);

            string word = _input.ReadLine()?.Trim().ToLower() ?? "";

            if (player != null && (word == "i" || word == "inventory"))
            {
                foreach (string line in player.GetInventoryDisplay(_messages))
                    _output.WriteLine(line);
                continue;
            }

            string? command = set.Find(word);
            if (command != null)
                return command;

            _output.WriteLine(_messages.GetMessage(set.InvalidKey));
            afterInvalid?.Invoke();
        }
    }

    private void WritePrompt(CommandSet set)
    {
        string prompt = _messages.GetMessage(set.PromptKey);

        if (set.Layout == PromptLayout.Inline)
        {
            _output.Write(prompt);
            return;
        }

        _output.WriteLine(prompt);
        if (set.Layout == PromptLayout.LineThenEnterChoice)
            _output.Write(_messages.GetMessage("enter_choice"));
    }
}
