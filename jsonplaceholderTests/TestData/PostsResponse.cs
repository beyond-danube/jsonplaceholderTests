using Newtonsoft.Json;
using System.Collections.Generic;

namespace TestData
{
    public class Posts: List<Post>
    {

    }

    public class Post
    {
        [JsonProperty("userId")]
        public int UserId { get; set; }
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
    }
}
