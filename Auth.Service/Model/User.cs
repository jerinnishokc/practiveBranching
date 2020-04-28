using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Auth.Service.Model
{
    public class User
    {
        [JsonProperty(PropertyName = "Id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "id")]
        public string id { get; set; }

        [JsonProperty(PropertyName = "Username")]
        public string Username { get; set; }

        [JsonProperty(PropertyName = "PasswordHash")]
        public byte[] PasswordHash { get; set; }

        [JsonProperty(PropertyName = "PasswordSalt")]
        public byte[] PasswordSalt { get; set; }

        [JsonProperty(PropertyName = "Role")]
        public string Role { get; set; }

        [JsonProperty(PropertyName = "Events")]
        public string[] Events { get; set; }

        [JsonProperty(PropertyName = "FeedbackStatus")]
        public string FeedbackStatus { get; set; }
    }
}
