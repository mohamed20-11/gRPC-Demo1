using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ToDogRPC.Models
{
    public class ToDoItem
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string ToDoStatus { get; set; } = "New";
    }
}
