# ADR 0001: Add Italian as a supported language

- **Status:** Accepted
- **Date:** 2026-11-08

## Context
The game supports English, Spanish and French. All player-facing text lives in `language_data.json`, loaded by `Messages` per language, so adding a language needs no code changes beyond the selection menu.

## Decision
Add Italian as language option 4:
- New `"Italian"` section in `language_data.json` with every key the other languages have.
- `Game.cs` language menu lists `4. Italiano` and maps `"4"` to `"Italian"`.
- Races and occupations are typed in Italian (`umano`, `guerriero`, ...), matching the Spanish and French approach.
- Commands stay in English (`roll`, `n/s/e`, `a/r`) because the code checks for them directly.
- Text is written without accents, matching the existing Spanish and French text.

## Consequences
- **Positive:** Italian speakers can play the game; the change was data-only plus a one-line menu edit; all 94 tests still pass.
- **Negative:** One more translation to keep in sync whenever a message key is added. No tests cover Italian yet. Some text stays in English: the title banner, the language prompt, and the dragon's `<<< claws >>>` art. "il tuo" is used for every weapon, which is wrong for feminine nouns like "spada lunga".
