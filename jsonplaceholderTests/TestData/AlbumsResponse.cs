using System.Collections.Generic;

namespace TestData
{
    public class Albums : List<Album>
    {

    }

    public class Album
    {
        public int Id { get; set; }
        public int UserId { get; set; }
    }
}
