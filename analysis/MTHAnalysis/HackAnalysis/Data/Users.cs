using System.Collections.Generic;
using Newtonsoft.Json;

namespace HackAnalysis.Data
{
    public class Users
    {
        /*[JsonProperty("user")]*/
        public List<User> users { get; set; }
    }
}