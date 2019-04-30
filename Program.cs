using System;
using System.Linq;
using System.Reflection;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;

using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.CommandsNext;
using DSharpPlus.Net;
using DSharpPlus.EventArgs;

using MongoDB;
using MongoDB.Driver;
using MongoDB.Bson;


namespace GTbot
{
    class Program
    {
    	public static Config MainCFG {get;set;}
 	public static InteractivityExtension Interactivity {get;set;}
	public static CommandsNextExtension CommandsNext {get;set;}
	public static DiscordClient Client {get;set;}
	public static Program Instance {get;set;}
	public static MongoClient Database {get;set;}

	    static void Main (string[] args)
	    {
		 if (File.Exists($@"{Directory.GetCurrentDirectory()}/Config.json"))
		 {
			MainCFG = JsonConvert.DeserializeObject<Config>(File.ReadAllText($@"{Directory.GetCurrentDirectory()}/Config.json"));
		 }
		 else
		 {
			MainCFG = ScreenConfiguration();
			File.WriteAllText($@"{Directory.GetCurrentDirectory()}/Config.json", JsonConvert.SerializeObject(MainCFG));	
		 }

		 Database = new MongoClient();
		 Instance = new Program();
		 Instance.StartAsync().GetAwaiter().GetResult();
	    }

	    public Program()
	    {
		Client = new DiscordClient
			(
				new DiscordConfiguration
					{
						UseInternalLogHandler = true,
						Token = MainCFG.Token,
						TokenType = TokenType.Bot
					}
			);
		Interactivity = Client.UseInteractivity
			(
		 		new InteractivityConfiguration()			
			);
		CommandsNext = Client.UseCommandsNext
			(
			 	new CommandsNextConfiguration
				{
					StringPrefixes = MainCFG.Prefixes,
					EnableDms = MainCFG.EnableDms,
					EnableDefaultHelp = false,
					EnableMentionPrefix = true
				}
			);
		CommandsNext.RegisterCommands(Assembly.GetEntryAssembly());


	    }

	    public static Config ScreenConfiguration()
	    {
		string s ="";
		Config cfg = new Config();
	    	Console.WriteLine($"Digite o TOKEN :");
		s = Console.ReadLine();
		cfg.Token = s;
	    	Console.WriteLine($"Digite o Prefixo"); 
		s = Console.ReadLine();
		List<string> ls = new List<string>();
		if(s.Contains(","))
		{

			ls = s.Split(',').ToList();
			cfg.Prefixes = ls;
		}
		else
		{
			ls.Add(s);
			cfg.Prefixes = ls;	
		}
		cfg.EnableDms = true;
		return cfg;
	    }

	    public async Task StartAsync ()
	    {
		    await Client.ConnectAsync();
		    await Task.Delay(-1);
	    }
    }
}
