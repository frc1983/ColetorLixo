using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ColetorLixo.Models
{
    public class Charger : Agent
    {
        public Collector[] UsedPositions { get; set; }

        public Charger(int x, int y, int capacity)
            : base(x, y, EnumAgentType.CHARGER, capacity)
        {
            this.UsedPositions = new Collector[2];
        }

        public Boolean HasEmptyPosition()
        {
            for (int i = 0; i < UsedPositions.Length; i++)
                if (this.UsedPositions[i] == null)
                    return true;

            return false;
        }

        internal void SetAgentInCharge(Collector c)
        {
            for (int i = 0; i < UsedPositions.Length; i++)
                if (this.UsedPositions[i] == null && !this.UsedPositions.Any(x => x != null && x.Id == c.Id))
                {
                    this.UsedPositions[i] = c;
                    break;
                }
        }

        internal void UnsetAgentInCharge(Collector c)
        {
            for (int i = 0; i < UsedPositions.Length; i++)
                if (this.UsedPositions[i] != null && this.UsedPositions[i].Id == c.Id)
                    this.UsedPositions[i] = null;
        }
    }
}