using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
namespace GTbot
{
	public class Config
	{
		[JsonProperty("token")]
		public string Token {get;set;}
		[JsonProperty("prefixes")]
		public IEnumerable<string> Prefixes {get;set;}
		[JsonProperty("enable_dms")]
		public bool EnableDms {get;set;}
	}
}
