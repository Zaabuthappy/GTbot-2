using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Collections.Generic;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using MongoDB.Driver;
using MongoDB.Bson;

using GTbot.Utils;
using GTbot.Objects;

namespace GTbot.Commands
{
	[Group("perfil")]
	public class Perfil : BaseCommandModule
	{
		[GroupCommand()]
		public async Task GroupCommand(CommandContext ctx)
		{
			DiscordEmbedBuilder Embed = new DiscordEmbedBuilder();
			Embed
				.AddField("Comandos Disponíveis",$"{ctx.Prefix}perfil `imagem` link - Atualiza a imagem de fundo\n{ctx.Prefix}perfil `cor` - Muda a cor da barra")
				.WithDescription("Esse grupo é responsável por alterar os dados que aparecem no perfil do membro do clã.")
				.WithAuthor("Grupo Perfil | Altera os dados do perfil")
				.WithColor(Colors.SagiriPurple);
			await ctx.RespondAsync(embed:Embed);

		}
		//TODO Imagem, Mensagem, Cor do CS.
		
		[Command("imagem")]
		public async Task Imagem(CommandContext ctx, string link)
		{
			IMongoDatabase Local = Program.Database.GetDatabase("local");
			IMongoCollection<ClanMember> Member = Local.GetCollection<ClanMember>("membros");
			IAsyncCursor<ClanMember> query = await Member.FindAsync(x => x.DiscordID == ctx.Member.Id);
			List<ClanMember> responce = query.ToList();
			DiscordEmbedBuilder Embed = new DiscordEmbedBuilder();

			if(responce.Count == 0)
			{
				Embed
					.WithDescription("Só membros do clã possuem acesso ao perfil.")
					.WithAuthor("Você não é um membro do clã!")
					.WithColor(Colors.SagiriPink);
				await ctx.RespondAsync(embed:Embed);
			}
			else
			{
				ClanMember member = responce[0];
				IMongoCollection<Profile> Profile = Local.GetCollection<Profile>("profile");
				IAsyncCursor<Profile> query2 = await Profile.FindAsync(x => x.ClanMemberID == member._id);
				List<Profile> responce2 =  query2.ToList();
				if(responce2.Count == 0)
				{
					Embed
						.WithDescription("eu criei.\n\nEdite seu perfil novamente para salvar.")
						.WithAuthor("Você não tinha um perfil, mas ...")
						.WithColor(Colors.SagiriGray);
					await ctx.RespondAsync(embed:Embed);
				}
				else
				{
					Profile perfil = responce2[0];
					string b64image = "";
					using(WebClient wc = new WebClient())
					{
						wc.DownloadFile(link,"_temp.png");
						byte[] byteStream = File.ReadAllBytes($"{Directory.GetCurrentDirectory()}/_temp.png");
						b64image = Convert.ToBase64String(byteStream);
						File.Delete($"{Directory.GetCurrentDirectory()}/_temp.png");
					}
					Profile.UpdateOne(Builders<Profile>.Filter.Eq("ClanMemberID",member._id), Builders<Profile>.Update.Set("CustomImage",b64image));
					Embed
						.WithAuthor("Imagem atualizada")
						.WithColor(Colors.SagiriBlue);
					await ctx.RespondAsync(embed:Embed);

				}	
			}
		}

		[Command("cor")]
		public async Task Cor(CommandContext ctx)
		{
			IMongoDatabase Local = Program.Database.GetDatabase("local");
			DiscordEmbedBuilder Embed = new DiscordEmbedBuilder();
			IMongoCollection<ClanMember> Member = Local.GetCollection<ClanMember>("membros");
			IAsyncCursor<ClanMember> query = await Member.FindAsync(x => x.DiscordID == ctx.Member.Id);
			List<ClanMember> responce = query.ToList();
			if(responce.Count == 0)
			{
				Embed
					.WithDescription("Só membros do clã possuem acesso aos perfis.")
					.WithAuthor("Você não é um membro do clã!")
					.WithColor(Colors.SagiriPink);
				await ctx.RespondAsync(embed:Embed);
			}
			else
			{
				ClanMember membro = responce[0];
				IMongoCollection<Profile> Profile = Local.GetCollection<Profile>("profile");
				IAsyncCursor<Profile> query2 = await Profile.FindAsync(x => x.ClanMemberID == membro._id);
				List<Profile> responce2 = query2.ToList();
				if(responce2.Count == 0)
				{
					Profile perfil = new Profile();
					perfil.ClanMemberID = membro._id;
					perfil._id = new ObjectId();
					
					Profile.InsertOne(perfil);
					Embed
						.WithDescription("eu criei. Digite o comando novamente para salvar.")
						.WithAuthor("Você não tinha um perfil, mas ...")
						.WithColor(Colors.SagiriGray);
					await ctx.RespondAsync(embed:Embed);
				}
				else
				{
					Profile p = responce2[0];
					Embed
						.WithDescription("Digite a letra referente a sua cor:\n\n**[A]**marelo\n**[V]**erde\nA**[z]**ul\n**[L]**aranja\n**[R]**oxo")
						.WithAuthor("Escolha a cor")
						.WithColor(Colors.SagiriGray);
					DiscordMessage botmsg = await ctx.RespondAsync(embed:Embed);
					
					DiscordMessage msg = await ctx.GetResponce(Program.Interactivity);

					switch(msg.Content.ToLower())
					{
						case "a" :
							p.CsColor = "A";
							break;	
						case "v" :
							p.CsColor = "V";
							break;
						case "z" :
							p.CsColor = "Z";
							break;
						case "l" :
							p.CsColor = "L";
							break;
						case "r" :
							p.CsColor = "R";
								break;
						default :
							p.CsColor = "";
							Embed
								.WithDescription("Tenha certeza que digitou apenas a letra em destaque.")
								.WithAuthor("Cor Inválida")
								.WithColor(Colors.SagiriPink);
							await botmsg.ModifyAsync(embed:Embed.Build());
							
							break;
					}
					if(!String.IsNullOrWhiteSpace(p.CsColor))
					{
						Profile.UpdateOne(Builders<Profile>.Filter.Eq("ClanMemberID",membro._id),Builders<Profile>.Update.Set("CsColor",p.CsColor));
						Embed
							.WithDescription("")
							.WithAuthor("Cor atualizada.")
							.WithColor(Colors.SagiriBlue);
						await botmsg.ModifyAsync(embed: Embed.Build());
					}

				 	
				}
			}

		}
		
	}
}
