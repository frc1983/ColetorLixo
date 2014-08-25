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
        public int level { get; set; }

        public Garbage(String type, int capacity){
            this.type = type;
            this.capacity = capacity;
        }
    }
}