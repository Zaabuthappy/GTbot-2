using System;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;



namespace GTbot.Commands
{
	[Group("help")]
	public class Help : BaseCommandModule
	{
		[GroupCommand]
		public async Task HelpCommand(CommandContext ctx)
		{
			DiscordEmbedBuilder Embed = new DiscordEmbedBuilder();
			Embed
				.WithColor(GTbot.Utils.Colors.SagiriPurple)
				.WithAuthor($"Comando {ctx.Prefix}help | Ajuda")
				.WithDescription("Bot Oficial do GT do Lolizão\nEstes são os grupos desponíveis:")
				.WithFooter("GTbot | by : [GTdL] Hidden");
			await ctx.RespondAsync(embed:Embed);
		}
	}
}
