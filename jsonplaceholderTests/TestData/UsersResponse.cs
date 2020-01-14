using System.Collections.Generic;

namespace TestData
{
    public class Users : List<User>
    {

    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
