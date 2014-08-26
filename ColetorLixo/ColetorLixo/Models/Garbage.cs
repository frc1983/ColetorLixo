using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ColetorLixo.Models
{
    public class Garbage : Agent
    {
        public String type;
        public int capacity;
        public int level;
        public Boolean full;

        public Garbage(String type, int capacity)
        {
            this.type = type;
            this.capacity = capacity;
            full = false;
        }

        public String getType()
        {
            return type;
        }

        public int getCapacity()
        {
            return capacity;
        }

        public int getLevel()
        {
            return level;
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

        public Boolean getFull()
        {
            return full;
        }
    }
}