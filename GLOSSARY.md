# Choose Your Own Adventure

A text adventure where a player creates a character, picks a path through the mountains, and may face a dragon.

## Language

### Travel

**Path**:
A route the player chooses at the fork: North (toward the dragon) or South (into the woods, toward the merchant).
_Avoid_: Direction, road, trail

**Encounter**:
A scene the player meets on a path, such as the merchant encounter or the dragon encounter.
_Avoid_: Event, scene, battle

### Input

**Command**:
A word the player types to choose an action, such as attack, north or inventory. Commands are always English, whatever the language (see ADR-0001).
_Avoid_: Option, input, choice

**Command prompt**:
Asks the player for a command and asks again until it gets a valid one. Checking the Inventory is a free action at every command prompt that has a player.
_Avoid_: Menu, input loop

### Trade

**Merchant**:
A trader met on the South path who sells magical armor, weapon enchantments and healing potions for gold.
_Avoid_: Shopkeeper, vendor, store

**Gold**:
The player's money, rolled during character creation and spent only at the merchant.
_Avoid_: Coins, money, currency

### Equipment

**Inventory**:
Everything the player is carrying or wearing: their weapon, armor, healing potions and gold. The player can check it at the fork, at the merchant, when facing the dragon, and during the dragon fight.
_Avoid_: Bag, backpack, items, gear

**Weapon**:
What a combatant attacks with. Its maximum damage sets the size of the damage roll.

**Magical weapon**:
The player's own weapon after the merchant enchants it. It keeps its type and gains extra maximum damage.
_Avoid_: New weapon, upgraded weapon

**Armor**:
Magical protective gear bought from the merchant.
_Avoid_: Shield, defense

**Protection**:
Extra hit points granted by armor that soak up damage before the player's health. Lost protection never returns.
_Avoid_: Defense, armor class, block, shield, temporary HP

### Health

**Maximum health**:
The health points the player rolls at character creation. Healing can never raise the player's health above it.
_Avoid_: Max HP, health cap, full health

**Healing potion**:
A potion that restores a rolled amount of the player's health, up to maximum health. It never restores protection. The player starts the adventure with one; any more must be bought from the merchant. It can only be drunk during the dragon fight, and drinking it takes the player's turn.
_Avoid_: Health potion, elixir, item, consumable
