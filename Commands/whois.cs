using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using MongoDB.Driver;
using MongoDB.Bson;

using GTbot;
using GTbot.Objects;
using GTbot.Utils;

namespace GTbot.Commands
{
	
	public class Whois : BaseCommandModule
	{
		[Command("whois")]
		public async Task TypeError(CommandContext ctx)
		{
			DiscordEmbedBuilder Embed = new DiscordEmbedBuilder();
			Embed
				.AddField("Utilização",$"{ctx.Prefix}whois [**@nome**]")
				.WithDescription("Pesquisa informações sobre um determinado membro do clã.")
				.WithAuthor($"Comando {ctx.Prefix}whois | Pesquisa")
				.WithColor(Colors.SagiriPurple);
			await ctx.RespondAsync(embed:Embed);
		}

		[Command("whois")]
		public async Task WhoIs(CommandContext ctx, DiscordMember member)
		{
			DiscordEmbedBuilder Embed = new DiscordEmbedBuilder();

			ClanMember m = new ClanMember();
			IMongoDatabase Local = Program.Database.GetDatabase("local");
			IMongoCollection<ClanMember> Membros = Local.GetCollection<ClanMember>("membros");
			IMongoCollection<Player> Player = Local.GetCollection<Player>("players");
			IAsyncCursor<Player> query = await Player.FindAsync(x => x.DiscordID == member.Id);
			List<Player> resultado = query.ToList();
			
			if(resultado.Count == 0)
				Player.InsertOne(new Player{DiscordID = member.Id, CurrentXP = 0, CurrentLevel = 0, _id = new ObjectId()});
			else
			{
				Player player = resultado[0];
				IAsyncCursor<ClanMember> query2 = await Membros.FindAsync(x => x.PlayerID == player._id);
				List<ClanMember> resultado2 = query2.ToList();
				if(resultado2.Count > 0)
				{
					m = resultado2[0];
					Embed

						.WithDescription($"Informações do membro {member.Username}\n\n\n[<:steam:570974606354284584>]({m.SteamID}) | [<:youtube:570974841763528706>]({m.YoutubeID})")
						.WithColor(Colors.SagiriBlue)
						.WithAuthor($"Whois [GTdL - {member.Username}]");

					await ctx.RespondAsync(embed:Embed);
				}
				else
				{
					Embed
						.WithAuthor("Este usuário não é um membro oficial do clã")
						.WithColor(Colors.SagiriPink);
					await ctx.RespondAsync(embed:Embed);
				}
			}
		}
	}
	
	[Group("edit")]
	public class WhoIsEdit : BaseCommandModule
	{
		[GroupCommand]
		public async Task TypeError (CommandContext ctx)
		{
			DiscordEmbedBuilder Embed = new DiscordEmbedBuilder();
			Embed
				.AddField("Comandos disponíveis", $"{ctx.Prefix}edit steam - Edita o link da steam\n{ctx.Prefix}edit youtube - Edita o link do youtube")
				.WithDescription($"Comandos responsáveis por editar o {ctx.Prefix}whois")
				.WithAuthor($"Grupo {ctx.Prefix}edit | Edição")
				.WithColor(Colors.SagiriPurple);
			await ctx.RespondAsync(embed:Embed);
		}

		[Command("steam")]
		public async Task steam(CommandContext ctx, string SteamURL)
		{
			DiscordEmbedBuilder Embed = new DiscordEmbedBuilder();
			ClanMember clanMember = new ClanMember();
			IMongoDatabase Local = Program.Database.GetDatabase("local");
			IMongoCollection<Player> Player = Local.GetCollection<Player>("players");
			IMongoCollection<ClanMember> Membros = Local.GetCollection<ClanMember>("membros");
			IAsyncCursor<Player> query = await Player.FindAsync(x => x.DiscordID == ctx.Member.Id);
			List<Player> resultado = query.ToList();

			if(resultado.Count == 0)
				Player.InsertOne(new Player{DiscordID = ctx.Member.Id, CurrentXP = 0, CurrentLevel = 0, _id = new ObjectId()});
			else
			{
				Player player = resultado[0];
				IAsyncCursor<ClanMember> query2 = await Membros.FindAsync(x => x.PlayerID == player._id);
				List<ClanMember> resultado2 = query2.ToList();
				if(resultado2.Count > 0)
				{
					clanMember = resultado2[0];
					clanMember.SteamID = SteamURL;

					Membros.UpdateOne(Builders<ClanMember>.Filter.Eq("PlayerID",player._id), Builders<ClanMember>.Update.Set("SteamID",clanMember.SteamID));
					Embed
						.WithAuthor("Link da steam alterado com sucesso!")
						.WithColor(Colors.SagiriBlue);
					await ctx.RespondAsync(embed:Embed);
					await ctx.Message.DeleteAsync();
				}
				else
				{
					Embed
						.WithAuthor("Você não possui um cadastro no sistema, peça um `protagonista de light novel` para te cadastrar!")
						.WithColor(Colors.SagiriPink);
					await ctx.RespondAsync(embed:Embed);
					
				}
			}
		}
		[Command("youtube")]
		public async Task youtube(CommandContext ctx, string YoutubeURL)
		{
			DiscordEmbedBuilder Embed = new DiscordEmbedBuilder();
			ClanMember clanMember = new ClanMember();
			IMongoDatabase Local = Program.Database.GetDatabase("local");
			IMongoCollection<Player> Player = Local.GetCollection<Player>("players");
			IMongoCollection<ClanMember> Membros = Local.GetCollection<ClanMember>("membros");
			IAsyncCursor<Player> query = await Player.FindAsync(x => x.DiscordID == ctx.Member.Id);
			List<Player> resultado = query.ToList();
			if (resultado.Count == 0)
				Player.InsertOne(new Player{DiscordID = ctx.Member.Id, CurrentXP = 0, CurrentLevel = 0});
			else
			{
				Player player = resultado[0];
				IAsyncCursor<ClanMember> query2 = await Membros.FindAsync(x => x.PlayerID == player._id);
				List<ClanMember> resultado2 = query2.ToList();

				if(resultado2.Count > 0)
				{
					clanMember = resultado2[0];
					clanMember.YoutubeID = YoutubeURL;

					Membros.UpdateOne(Builders<ClanMember>.Filter.Eq("PlayerID",player._id), Builders<ClanMember>.Update.Set("YoutubeID",clanMember.YoutubeID));
					Embed
						.WithAuthor("Link do youtube alterado com sucesso!")
						.WithColor(Colors.SagiriBlue);
					await ctx.RespondAsync(embed:Embed);
					await ctx.Message.DeleteAsync();
				}
				else
				{
					Embed
						.WithAuthor("Você não possui um cadastro no sistema, peça um `protagonista de light novel` para te cadastrar!")
						.WithColor(Colors.SagiriPink);
					await ctx.RespondAsync(embed:Embed);
				}
			}
		}
	}

	public class WhoIsAdmin : BaseCommandModule
	{
		[Command("addmember"),Staff]
		[Aliases("+m","addm")]
		public async Task addMember(CommandContext ctx,DiscordMember m)
		{
			DiscordEmbedBuilder Embed = new DiscordEmbedBuilder();
			ClanMember clanMember = new ClanMember();
			
			clanMember.DiscordID = m.Id;

			IMongoDatabase Local = Program.Database.GetDatabase("local");
			IMongoCollection<ClanMember> Membros;

			Membros = Local.GetCollection<ClanMember>("membros");
			IAsyncCursor<ClanMember> query = await Membros.FindAsync(x => x.DiscordID == m.Id);
			List<ClanMember> resultado = query.ToList();

			if(resultado.Count > 0)
			{
				Embed
					.WithAuthor($"O membro [GTdL - {m.Username}] já foi cadastrado.")
					.WithColor(Colors.SagiriPink);

				await ctx.RespondAsync(embed: Embed);
			}
			else
			{
				Membros.InsertOne(clanMember);
				Embed
					.WithAuthor($"O membro [GTdL - {m.Username}] foi cadastrado com sucesso!")
					.WithColor(Colors.SagiriBlue);
				await ctx.RespondAsync(embed: Embed);
			}
			
		}
		[Command("delmember"),Staff]
		[Aliases("-m","rmm","delm")]
		public async Task delMember(CommandContext ctx, DiscordMember m)
		{
			DiscordEmbedBuilder Embed = new DiscordEmbedBuilder();
			ClanMember clanMember = new ClanMember();

			IMongoDatabase Local = Program.Database.GetDatabase("local");
			IMongoCollection<ClanMember> Membros = Local.GetCollection<ClanMember>("membros");
			IAsyncCursor<ClanMember> query = await Membros.FindAsync(x => x.DiscordID == m.Id);
			List<ClanMember> resultado = query.ToList();

			if(resultado.Count > 0)
			{
				Membros.DeleteOne(x => x.DiscordID == m.Id);
				Embed
					.WithAuthor($"O membro [GTdL - {m.Username}] foi removido com sucesso!")
					.WithColor(Colors.SagiriBlue);
				await ctx.RespondAsync(embed: Embed);
			}
			else
			{
				Embed
					.WithAuthor($"O membro {m.Username} não foi cadastrado.")
					.WithColor(Colors.SagiriPink);
				await ctx.RespondAsync(embed:Embed);
			}
		}


	}
}
