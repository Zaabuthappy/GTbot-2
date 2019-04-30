using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using System;
using System.Text;
using System.Threading.Tasks;

namespace GTbot.Objects
{
	public class Staff : CheckBaseAttribute
	{
		public async override Task<bool> ExecuteCheckAsync(CommandContext ctx,bool help)
		{
			return ctx.Member.PermissionsIn(ctx.Channel).HasPermission(Permissions.ManageRoles);
		}
	}
}
