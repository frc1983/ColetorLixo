using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ColetorLixo.Models
{
    public class Charger : Agent
    {
        public Boolean UsedPositionOne { get; set; }
        public Boolean UsedPositionTwo { get; set; }

        public Charger(int x, int y, int capacity) : base(x, y, EnumAgentType.CHARGER, capacity)
        {
            this.UsedPositionOne = false;
            this.UsedPositionTwo = false;
        }

        public void LoadCollector()
        {
            if (HasEmptyPosition())
            {
                if (!UsedPositionOne)
                    UsedPositionOne = true;
                else if (!UsedPositionTwo)
                    UsedPositionTwo = true;
            }

        }

        private Boolean HasEmptyPosition()
        {
            return UsedPositionOne && UsedPositionTwo;
        }
    }
}