using RestSharp.Deserializers;
using RestSharp.Serializers;
using System.Collections.Generic;

namespace TestData
{
    public class Photos : List<Photo>
    {

    }

    public class Photo
    {
        public int AlbumId { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
    }
}
