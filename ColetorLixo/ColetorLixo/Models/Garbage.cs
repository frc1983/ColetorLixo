using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ColetorLixo.Models
{
    public class Garbage : Cell
    {
        public EnumGarbageType GarbageType;
        public int GarbageLoad { get; set; }

        public Garbage(Cell position, EnumGarbageType type, int load = 1) : base(position)
        {
            this.GarbageType = type;
            this.GarbageLoad = load;
        }
    }
}