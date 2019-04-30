using System;
using System.Text;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GTbot.Objects
{

	public class ClanMember
	{
		[BsonId]
		public ObjectId _id {get;set;}
		[BsonElement]
		public string SteamID {get;set;}
		[BsonElement]
		public ulong DiscordID {get;set;}
		[BsonElement]
		[BsonIgnoreIfNull]
		public string YoutubeID {get;set;}
	}
}
