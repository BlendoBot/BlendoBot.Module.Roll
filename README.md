# Roll
## Simulates dice rolls and coin flips
![GitHub Workflow Status](https://img.shields.io/github/workflow/status/BlendoBot/BlendoBot.Module.Roll/Tests)
![Nuget](https://img.shields.io/nuget/v/BlendoBot.Module.Roll)

Ever wanted to heads or tails something? Randomly choose something from a list? The roll command allows you to get a random number within you likings.

## Usage
All of these commands use the default term `?roll`, but your guild may rename this term. `[x]` should be replaced with a positive integer between 1 and 50. `[y]` should be replaced with a positive integer between 2 and 1,000,000.
- `?roll [y]`
  - Simulates rolling a `y`-sided die. The command will print out a number between 1 and `y` inclusive.
- `?roll d[y]`
  - The same as `?roll [y]`.
- `?roll [x]d[y]`
  - Rolls `x` number of `y`-sided dice, and prints out all the rolled values. All the rolled values are between 1 and y inclusive.
- `?roll coin`
  - Prints either heads or tails with a 50/50 distribution.