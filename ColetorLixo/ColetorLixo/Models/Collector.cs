using ColetorLixo.ViewModels;
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
        public List<Garbage> GarbageInside { get; set; }

        public Collector(int x, int y, int capacity, int battery)
            : base(x, y, EnumAgentType.COLLECTOR, capacity)
        {
            this.BatteryLevel = battery;
            ActualState = EnumCollectorStates.LIMPAR;
            GarbageInside = new List<Garbage>();
        }

        public static void MoveCollector(MatrixViewModel matrixVM, Cell colCell)
        {
            if (colCell.Y + 1 < matrixVM.Ambient.GetLength(1) &&
                (matrixVM.Ambient[colCell.X, colCell.Y + 1].Agent == null ||
                matrixVM.Ambient[colCell.X, colCell.Y + 1].Agent.AgentType.Equals(EnumAgentType.GARBAGE)))
            {
                Agent tmp = colCell.Agent;
                matrixVM.Ambient[colCell.X, colCell.Y].Agent = null;
                colCell.Y = colCell.Y + 1;
                matrixVM.Ambient[colCell.X, colCell.Y].Agent = tmp;
            }

            ((Collector)matrixVM.Ambient[colCell.X, colCell.Y].Agent).BatteryLevel--;
        }

        public Boolean AddGarbageLoad(Garbage garbage, int load)
        {
            if (!FullLoad && load <= (GarbageCapacity - GarbageLoad))
            {
                GarbageLoad += load;
                GarbageInside.Add(garbage);
                if (GarbageLoad == GarbageCapacity)
                    FullLoad = true;

                return true;
            }

            return false;
        }

        public List<Garbage> LookGarbages(MatrixViewModel matrixVM, int lookDistance)
        {
            List<Garbage> ret = new List<Garbage>();

            return ret;
        }

    }
}