using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RPGEngine.Discord
{


    class DiscordConfig
    {
        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("server")]
        public string Server { get; set; }

        [JsonProperty("channel")]
        public string Channel { get; set; }
        [JsonProperty("prefix")]
        public string Prefix { get; set; }
    }
}
