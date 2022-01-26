using BlendoBot.Core.Command;
using BlendoBot.Core.Entities;
using BlendoBot.Core.Module;
using BlendoBot.Core.Utility;
using DSharpPlus.EventArgs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlendoBot.Module.Roll;

internal class RollCommand : ICommand {
	public RollCommand(Roll module) {
		this.module = module;
	}

	private readonly Roll module;
	public IModule Module => module;

	public string Guid => "roll.command";
	public string DesiredTerm => "roll";
	public string Description => "Simulates dice rolls and coin flips";
	public Dictionary<string, string> Usage => new() {
		{ "Note", $"{"x".Code()} and {"y".Code()} must be positive integers." },
		{ "[y]", $"Rolls a {"y".Code()}-sided die, giving a value between 1 and {"y".Code()}." },
		{ "d[y]", $"Same as {"[y]".Code()}." },
		{ "[x]d[y]", $"Rolls a {"y".Code()}-sided die {"x".Code()} number of times." },
		{ "coin", $"Returns either heads or tails." },
	};
		
	public async Task OnMessage(MessageCreateEventArgs e, string[] tokenizedInput) {
		// There must be exactly one term.
		if (tokenizedInput.Length == 1) {
			if (tokenizedInput[0].ToLower() == "coin") {
				await module.FlipCoin(e);
			} else {
				string[] splitRoll = tokenizedInput[0].Split('d');
				if (splitRoll.Length == 1 || (splitRoll.Length == 2 && string.IsNullOrWhiteSpace(splitRoll[0]))) {
					bool success = int.TryParse(splitRoll[^1], out int diceValue);
					if (success) {
						if (diceValue > Roll.MaxDiceSides) {
							await module.DiscordInteractor.Send(this, new SendEventArgs {
								Message = $"You can't roll a {diceValue}-sided die! Please use a lower number (at most {Roll.MaxDiceSides:N0}).",
								Channel = e.Channel,
								Tag = "RollErrorSingleTooHigh"
							});
						} else if (diceValue >= Roll.MinDiceSides) {
							await module.RollDice(e, 1, diceValue);
						} else {
							await module.DiscordInteractor.Send(this, new SendEventArgs {
								Message = $"You can't roll a {diceValue}-sided die! Please use a higher number (at least {Roll.MinDiceSides}).",
								Channel = e.Channel,
								Tag = "RollErrorSingleTooLow"
							});
						}
					} else {
						await module.DiscordInteractor.Send(this, new SendEventArgs {
							Message = $"{splitRoll[^1]} is not a valid number!",
							Channel = e.Channel,
							Tag = "RollErrorSingleInvalidNumber"
						});
					}
				} else if (splitRoll.Length == 2) {
					bool success1 = int.TryParse(splitRoll[0], out int numDice);
					if (success1) {
						if (numDice > Roll.MaxDiceCount) {
							await module.DiscordInteractor.Send(this, new SendEventArgs {
								Message = $"You can't roll {numDice} dice! Please use a lower number (at most {Roll.MaxDiceCount}).",
								Channel = e.Channel,
								Tag = "RollErrorMultipleNumTooHigh"
							});
						} else if (numDice >= Roll.MinDiceCount) {
							bool success2 = int.TryParse(splitRoll[1], out int diceValue);
							if (success2) {
								if (diceValue > Roll.MaxDiceSides) {
									await module.DiscordInteractor.Send(this, new SendEventArgs {
										Message = $"You can't roll a {diceValue}-sided die! Please use a lower number (at most {Roll.MaxDiceSides:N0}).",
										Channel = e.Channel,
										Tag = "RollErrorMultipleValueTooHigh"
									});
								} else if (diceValue >= Roll.MinDiceSides) {
									await module.RollDice(e, numDice, diceValue);
								} else {
									await module.DiscordInteractor.Send(this, new SendEventArgs {
										Message = $"You can't roll a {diceValue}-sided die! Please use a higher number (at least {Roll.MinDiceSides}).",
										Channel = e.Channel,
										Tag = "RollErrorMultipleValueTooLow"
									});
								}
							} else {
								await module.DiscordInteractor.Send(this, new SendEventArgs {
									Message = $"{splitRoll[1]} is not a valid number!",
									Channel = e.Channel,
									Tag = "RollErrorMultipleValueInvalidNumber"
								});
							}
						} else {
							await module.DiscordInteractor.Send(this, new SendEventArgs {
								Message = $"You can't roll {numDice} dice! Please use a higher number (at least {Roll.MinDiceCount}).",
								Channel = e.Channel,
								Tag = "RollErrorMultipleNumTooLow"
							});
						}
					} else {
						await module.DiscordInteractor.Send(this, new SendEventArgs {
							Message = $"{splitRoll[0]} is not a valid number!",
							Channel = e.Channel,
							Tag = "RollErrorMultipleNumInvalidNumber"
						});
					}
				} else {
					await module.DiscordInteractor.Send(this, new SendEventArgs {
						Message = $"I couldn't determine what you wanted. Check {$"{module.ModuleManager.GetHelpTermForCommand(this)}".Code()} for ways to use this command.",
						Channel = e.Channel,
						Tag = "RollErrorTooManyDs"
					});
				}
			}
		} else {
			await module.DiscordInteractor.Send(this, new SendEventArgs {
				Message = $"I couldn't determine what you wanted. Check {$"{module.ModuleManager.GetHelpTermForCommand(this)}".Code()} for ways to use this command.",
				Channel = e.Channel,
				Tag = "RollErrorInvalidArgumentCount"
			});
		}

		await Task.CompletedTask;
	}
}
