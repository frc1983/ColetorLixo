using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ColetorLixo.Models
{
    public class Collector : Agent
    {
        public int BatteryLevel { get; set; }
        public EnumCollectorStates ActualState { get; set; }

        public Collector(int x, int y, int capacity, int battery) : base(x, y, EnumAgentType.COLLECTOR, capacity)
        {
            this.BatteryLevel = battery;
            ActualState = EnumCollectorStates.LIMPAR;
        }

        public void MoveToPosition(int x, int y)
        {
            this.X = x;
            this.Y = y;
            BatteryLevel--;
        }

        public Boolean AddGarbageLoad(int load)
        {
            if (load <= (GarbageCapacity - GarbageLoad))
            {
                GarbageLoad += load;
                if (GarbageLoad == GarbageCapacity)
                    FullLoad = true;

                return true;
            }

            return false;
        }

    }
}