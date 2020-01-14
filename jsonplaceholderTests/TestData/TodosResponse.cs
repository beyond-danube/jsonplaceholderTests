using System.Collections.Generic;

namespace TestData
{
    public class Todos : List<Todo>
    {

    }

    public class Todo
    {
        public int UserId { get; set; }
        public bool Completed { get; set; }
    }
}
