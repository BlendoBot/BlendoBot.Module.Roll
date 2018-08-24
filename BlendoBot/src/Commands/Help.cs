﻿using DSharpPlus.EventArgs;
using System.Text;
using System.Threading.Tasks;

namespace BlendoBot.Commands {
	public static class Help {
		public static async Task HelpCommand(MessageCreateEventArgs e) {
			var sb = new StringBuilder();
			foreach (var command in Command.AvailableCommands) {
				if (command.Key != Command.DummyUnknownCommand && Program.Data.IsCommandEnabled(command.Key, e.Guild)) {
					sb.AppendLine($"**{command.Value.Name}** - `{command.Value.Term}`");
					sb.AppendLine($"{command.Value.Description}");
					sb.AppendLine();
				}
			}
			await Program.SendMessage(sb.ToString(), e.Channel, "Help");
		}
	}
}
