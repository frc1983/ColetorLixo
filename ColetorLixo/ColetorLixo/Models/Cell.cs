using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ColetorLixo.Models
{
    public class Cell
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Cell(Cell position)
        {
            this.X = position.X;
            this.Y = position.Y;
        }
    }
}
