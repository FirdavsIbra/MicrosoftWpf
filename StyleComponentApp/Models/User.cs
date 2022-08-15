using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StyleComponentApp.Models
{
    public class User
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public dynamic Image { get; set; }

        [JsonProperty("faculty")]
        public string LastMessage { get; set; }
    }
}
