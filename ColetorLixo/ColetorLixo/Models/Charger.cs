using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ColetorLixo.Models
{
    public class Charger : Agent
    {
        public Agent UsedPositionOne { get; set; }
        public Agent UsedPositionTwo { get; set; }

        public Charger(int x, int y, int capacity) : base(x, y, EnumAgentType.CHARGER, capacity)
        {
            this.UsedPositionOne = null;
            this.UsedPositionTwo = null;
        }

        public Boolean HasEmptyPosition()
        {
            return UsedPositionOne == null || UsedPositionTwo == null;
        }

        internal void SetAgentInCharge(Collector c)
        {
            if (this.UsedPositionOne == null || this.UsedPositionOne == c) this.UsedPositionOne = c;
            else if (this.UsedPositionTwo == null || this.UsedPositionTwo == c) this.UsedPositionTwo = c;
        }

        internal void UnsetAgentInCharge(Collector c)
        {
            if (this.UsedPositionOne != null && this.UsedPositionOne == c) this.UsedPositionOne = null;
            if (this.UsedPositionTwo != null && this.UsedPositionTwo == c) this.UsedPositionTwo = null;
        }
    }
}