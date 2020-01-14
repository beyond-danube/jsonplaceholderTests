using System.Collections.Generic;

namespace TestData
{
    public class Comments : List<Comment>
    {

    }

    public class Comment
    {
        public string Email { get; set; }
        public string Body { get; set; }
    }
}
