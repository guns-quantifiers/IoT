using System;
using System.Collections.Generic;

namespace BlackjackAPI.Models
{
    public class Game
    {
        public Game()
        {
            Id = new Guid();
        }

        public Guid Id { get; set; }
        public List<Deal> History { get; set; }
        public Strategy CurrentStrategy => Strategy.Draw; //TODO: implement
    }
}
