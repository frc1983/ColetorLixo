using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ColetorLixo.Models
{
    public class Trash : Agent
    {
        public EnumGarbageType GarbageType { get; set; }

        public Trash(int x, int y, int capacity, EnumGarbageType garbageType)
            : base(x, y, EnumAgentType.TRASH, capacity)
        {
            this.FullLoad = false;
            this.GarbageType = garbageType;
        }

        public void ReceiveGarbage(int load)
        {
            if (!this.FullLoad && load <= (GarbageCapacity - GarbageLoad))
                this.GarbageLoad += load;

            if (this.GarbageCapacity.Equals(this.GarbageLoad))
                this.FullLoad = true;
        }
    }
}
