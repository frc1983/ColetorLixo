using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ColetorLixo.Models
{
    public class Colector : Agent
    {
        public int capacity;
        public int level;
        public int battery;
        public Boolean full;

        public Colector(int capacity, int battery)
        {
            super();
            this.capacity = capacity;
            this.battery = battery;
        }

        public int getCapacity()
        {
            return capacity;
        }

        public int getLevel()
        {
            return level;
        }

        public Boolean getFull()
        {
            return full;
        }

        public int getBattery()
        {
            return battery;
        }

        public void setPosition(Cell position)
        {
            super.position = position;
            battery--;
            
        }

        public Boolean addLevel(int plus)
        {
            if (plus <= (capacity - level))
            {
                level += plus;
                if (level == capacity)
                    full = true;
                return true;
            }
            return false;

        }

    }
}