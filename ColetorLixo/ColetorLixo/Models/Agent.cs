using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ColetorLixo.Models
{
    public class Agent
    {

        public Cell position;

        public Agent(Cell position)
        {
            this.position = position;
        }

        public Cell getPosition(){
            return position;
        }

        }
}