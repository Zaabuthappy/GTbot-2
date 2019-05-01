using System;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Collections.Generic;

using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using SixLabors.ImageSharp;
using SixLabors.Fonts;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.Primitives;
using SixLabors.ImageSharp.Processing;

using MongoDB.Driver;
using MongoDB.Bson;

using GTbot.Utils;
using GTbot.Objects;

namespace GTbot.Commands
{
	public class ProfileGeneration : BaseCommandModule
	{
		[Command("generate")]
		public async Task Generate (CommandContext ctx)
		{
			
			DiscordMember requestedUser = ctx.Member;
			DiscordEmbedBuilder Embed = new DiscordEmbedBuilder();	
			IMongoDatabase Local = Program.Database.GetDatabase("local");
			IMongoCollection<Profile> Profile = Local.GetCollection<Profile>("profile");
			IMongoCollection<Player> Player = Local.GetCollection<Player>("players");
			IMongoCollection<ClanMember> Member = Local.GetCollection<ClanMember>("membros");
			IAsyncCursor<Player> query = await Player.FindAsync(x => x.DiscordID == ctx.Member.Id);
			List<Player> responce = query.ToList();
			if(responce.Count == 0)
				Player.InsertOne(new Player{DiscordID = ctx.Member.Id, CurrentXP = 0, CurrentLevel = 0, _id = new ObjectId()});
			else
			{
				Player player = responce[0];
				IAsyncCursor<ClanMember> query2 = await Member.FindAsync(x => x.PlayerID == player._id);
				List<ClanMember> responce2 = query2.ToList();

				if(responce2.Count == 0)
				{
					Embed
						.WithDescription("Somente membros do clã possuem acesso ao perfil.")
						.WithAuthor("Você não é um membro do clã!")
						.WithColor(Colors.SagiriPink);

					await ctx.RespondAsync(embed:Embed);
				}
				else
				{
					ClanMember member = responce2[0];

					IAsyncCursor<Profile> query3 = await Profile.FindAsync(x => x.ClanMemberID == member._id);
					List<Profile> responce3 = query3.ToList();

					if(responce3.Count == 0)
					{
						Embed
							.WithDescription($"eu criei.\n\nConfigure seu perfil com os comandos do grupo `Perfil`. Use {ctx.Prefix}perfil para mais informações.")
							.WithAuthor("Você não tinha um perfil, então...")
							.WithColor(Colors.SagiriGray);
						Profile p = new Profile();
						p.ClanMemberID = member._id;
						p._id = new ObjectId();
					
						Profile.InsertOne(p);	
						await ctx.RespondAsync(embed:Embed);
					}
					else
					{	
						Profile perfil = responce3[0];
					
						string modeloPath = $"{Directory.GetCurrentDirectory()}/Modelo.png";
						using(WebClient wc = new WebClient())
						{
							wc.DownloadFile(requestedUser.AvatarUrl,$"{requestedUser.Id}.png");
						}
						string pfpPath = $"{Directory.GetCurrentDirectory()}/{requestedUser.Id}.png";
						using (Image<Rgba32> modelo = Image.Load(modeloPath))
						{
							Rgba32 White = new Rgba32(255,255,255);
						
							Rgba32 Blue = new Rgba32(Colors.CsBlue.R,Colors.CsBlue.G,Colors.CsBlue.B);
							Rgba32 Yellow = new Rgba32(Colors.CsYellow.R,Colors.CsYellow.G,Colors.CsYellow.B);
							Rgba32 Orange = new Rgba32(Colors.CsOrange.R,Colors.CsOrange.G,Colors.CsOrange.B);
							Rgba32 Green = new Rgba32(Colors.CsGreen.R,Colors.CsGreen.G,Colors.CsGreen.B);
							Rgba32 Purple = new Rgba32(Colors.CsPurple.R,Colors.CsPurple.G,Colors.CsPurple.B);
						
							Pen<Rgba32> MainPen = new Pen<Rgba32>(White,1);
						
							RectangleF ColorRect = new RectangleF(new PointF(0,150),new SizeF(700,6));
						
						
							Font NotoSans = SystemFonts.CreateFont("Noto Sans",29);
							Image<Rgba32> pfp = Image.Load(pfpPath);
							pfp.Mutate(p => p.Resize(121,121));
							modelo.Mutate(m => 
								m.DrawImage(pfp,new Point(567,9),1f)
							     );
							modelo.Mutate(m => 
								m.DrawText($"Perfil do {(ctx.Member.Username.ToLower().Contains("gtdl")?ctx.Member.Username:$"GTdL - {ctx.Member.Username}")}",NotoSans,White,new Point(8,4))
							     );
						
							if(String.IsNullOrWhiteSpace(perfil.CsColor))
							{
								modelo.Mutate(m =>
									m.Fill(White,ColorRect)
								);
							}
							else
							{
								switch(perfil.CsColor)
								{
									case "A":
										modelo.Mutate(m => m.Fill(Yellow,ColorRect));
										break;
									case "Z":
										modelo.Mutate(m => m.Fill(Blue,ColorRect));
										break;
									case "V":
										modelo.Mutate(m => m.Fill(Green,ColorRect));
										break;
									case "L":
										modelo.Mutate(m => m.Fill(Orange,ColorRect));
										break;
									case "R":
										modelo.Mutate(m => m.Fill(Purple,ColorRect));
										break;
										
								}
							}

							if(!String.IsNullOrWhiteSpace(perfil.CustomImage))
							{
								byte[] byteArray = Convert.FromBase64String(perfil.CustomImage);
								Image<Rgba32> CustomImg = Image.Load(byteArray);
								CustomImg.Mutate(c => c.Resize(700,343));
								modelo.Mutate(m =>
									m.DrawImage(CustomImg,new Point(0,156),0.7f)
									);
							}

						modelo.Save("temp.png");
					}
	
					await ctx.Channel.SendFileAsync($"{Directory.GetCurrentDirectory()}/temp.png","Foto :");
					File.Delete($"{Directory.GetCurrentDirectory()}/{requestedUser.Id}.png");
					File.Delete($"{Directory.GetCurrentDirectory()}/temp.png");
					}
			
				}
		
			}
		}
	}
}
