using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.Interactivity;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;

namespace GTbot.Utils
{
	public static class GTinteract
	{
		public static async Task<DiscordMessage> GetResponce (this CommandContext ctx, InteractivityExtension interact)
		{
			var responce = await interact.WaitForMessageAsync(m => m.Author == ctx.Member, TimeSpan.FromMinutes(10));
			return responce.Result;
		}
	}
}

