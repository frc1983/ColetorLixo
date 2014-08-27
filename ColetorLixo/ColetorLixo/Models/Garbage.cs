using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ColetorLixo.Models
{
    public class Garbage : Agent
    {
        public EnumGarbageType GarbageType { get; set; }

        public Garbage(int x, int y, EnumGarbageType type, EnumAgentType agentType, int load = 1)
            : base(x, y, agentType, load)
        {
            this.GarbageType = type;
        }
    }
}