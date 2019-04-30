using System;
using System.Text;

using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace GTbot.Objects
{
	public class Player
	{
		[BsonId]
		public ObjectId _id {get;set;}
		[BsonElement]
		public int CurrentXP {get;set;}
		[BsonElement]
		public int CurrentLevel {get;set;}
		[BsonElement]
		public ulong DiscordID {get;set;}

	}
}
