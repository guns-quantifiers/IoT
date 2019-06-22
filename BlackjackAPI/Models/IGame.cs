using System;
using System.Collections.Generic;

namespace BlackjackAPI.Models
{
    public interface IGame
    {
        List<Deal> History { get; set; }
        Guid Id { get; set; }
    }
}