using BlendoBot.Core.Entities;
using BlendoBot.Core.Module;
using BlendoBot.Core.Services;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlendoBot.Module.Roll;

[Module(Guid = "com.biendeo.blendobot.module.roll", Name = "Roll", Author = "Biendeo", Version = "2.0.0", Url = "https://github.com/BlendoBot/BlendoBot.Module.Roll")]
public class Roll : IModule {
	public Roll(IDiscordInteractor discordInteractor, IModuleManager moduleManager) {
		Random = new Random();

		DiscordInteractor = discordInteractor;
		ModuleManager = moduleManager;

		RollCommand = new RollCommand(this);
	}

	internal ulong GuildId { get; private set; }

	internal readonly RollCommand RollCommand;

	internal readonly Random Random;
	internal readonly IDiscordInteractor DiscordInteractor;
	internal readonly IModuleManager ModuleManager;

	public const int MinDiceCount = 1;
	public const int MaxDiceCount = 50;
	public const int MinDiceSides = 2;
	public const int MaxDiceSides = 1000000;

	public Task<bool> Startup(ulong guildId) {
		GuildId = guildId;
		return Task.FromResult(ModuleManager.RegisterCommand(this, RollCommand, out _));
	}

	internal async Task RollDice(MessageCreateEventArgs e, int numRolls, int diceValue) {
		List<int> results = Enumerable.Range(0, numRolls).Select(_ => Random.Next(diceValue) + 1).ToList();
		if (results.Count == 1) {
			await DiscordInteractor.Send(this, new SendEventArgs {
				Message = IntToRegionalIndicator(results.Single()),
				Channel = e.Channel,
				Tag = "RollSuccessSingle"
			});
		} else {
			StringBuilder sb = new();
			sb.AppendLine($"The results of the {numRolls} dice-rolls are:");
			sb.AppendLine("```");
			for (int i = 0; i < numRolls; ++i) {
				sb.Append(results[i].ToString().PadLeft(8, ' '));
				if (i % 5 == 4) {
					sb.AppendLine();
				}
			}
			sb.AppendLine("\n```");
			await DiscordInteractor.Send(this, new SendEventArgs {
				Message = sb.ToString(),
				Channel = e.Channel,
				Tag = "RollSuccessMultiple"
			});
		}
	}

	internal static string IntToRegionalIndicator(int x) {
		if (x >= 10) {
			return IntToRegionalIndicator(x / 10) + IntToRegionalIndicator(x % 10);
		} else {
			return x switch {
				0 => ":zero:",
				1 => ":one:",
				2 => ":two:",
				3 => ":three:",
				4 => ":four:",
				5 => ":five:",
				6 => ":six:",
				7 => ":seven:",
				8 => ":eight:",
				9 => ":nine:",
				_ => "?",
			};
		}
	}

	internal async Task FlipCoin(MessageCreateEventArgs e) {
		int result = Random.Next(2);
		if (result == 0) {
			await DiscordInteractor.Send(this, new SendEventArgs {
				Message = ":regional_indicator_h::regional_indicator_e::regional_indicator_a::regional_indicator_d::regional_indicator_s:",
				Channel = e.Channel,
				Tag = "RollSuccessCoinHeads"
			});
		} else {
			await DiscordInteractor.Send(this, new SendEventArgs {
				Message = ":regional_indicator_t::regional_indicator_a::regional_indicator_i::regional_indicator_l::regional_indicator_s:",
				Channel = e.Channel,
				Tag = "RollSuccessCoinTails"
			});
		}
	}
}
