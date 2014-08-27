using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ColetorLixo.Models
{
    public class Colector : Agent
    {
        public int GarbageLoad { get; set; }
        public int BatteryLevel { get; set; }
        public Boolean FullLoad { get; set; }

        public Colector(int capacity, int battery, Cell position) : base(position)
        {
            this.Capacity = capacity;
            this.BatteryLevel = battery;
        }

        public void setPosition(Cell position)
        {
            Position = position;
            BatteryLevel--;            
        }

        public Boolean addGarbageLoad(int load)
        {
            if (load <= (Capacity - GarbageLoad))
            {
                GarbageLoad += load;
                if (GarbageLoad == Capacity)
                    FullLoad = true;

                return true;
            }

            return false;
        }

    }
}