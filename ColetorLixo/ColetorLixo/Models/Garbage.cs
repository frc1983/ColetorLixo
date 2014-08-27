using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ColetorLixo.Models
{
    public class Garbage : Cell
    {
        public EnumGarbageType GarbageType;
        public int Capacity;
        public int Level { get; set; }

        public Garbage(EnumGarbageType type, int capacity){
            GarbageType = type;
            Capacity = capacity;
        }
    }
}