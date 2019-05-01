using System;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace GTbot.Commands
{
	public class EmbedTesting : BaseCommandModule
	{
		[Command("imagetest")]
		public async Task test(CommandContext ctx)
		{
			DiscordEmbedBuilder Embed = new DiscordEmbedBuilder();
			Embed
				.WithImageUrl(ctx.Member.AvatarUrl)
				.WithThumbnailUrl(ctx.Member.AvatarUrl)
				.WithAuthor("author",null,ctx.Member.AvatarUrl);
			await ctx.RespondAsync(embed:Embed);
		}
	}
}
