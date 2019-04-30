using System;
using System.Text;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GTbot.Objects
{
	public class Profile
	{
		[BsonId]
		public ObjectId _id {get;set;}
		[BsonElement]
		public ObjectId ClanMemberID {get;set;}
		[BsonElement]
		[BsonIgnoreIfNull]
		public string CustomImage {get;set;}
		[BsonElement]
		[BsonIgnoreIfNull]
		public string Message {get;set;}
		[BsonElement]
		[BsonIgnoreIfNull]
		public string CsColor {get;set;}
		
	}
}
