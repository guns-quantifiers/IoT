using System;
using System.Collections.Generic;

namespace BlackjackAPI.Models
{
    public class Game
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public List<Deal> History { get; set; } = new List<Deal>();

        public Strategy CurrentStrategy()
        {
            return Strategy.Draw; //TODO: implement
        }
    }
}
