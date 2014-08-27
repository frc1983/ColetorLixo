using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ColetorLixo.Models
{
    public class Agent
    {
        public int Capacity { get; set; }
        public Cell Position { get; set; }

        public Agent(Cell position)
        {
            Position = position;
        }
    }
}