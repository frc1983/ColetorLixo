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
        public Object Agent { get; set; }

        public Cell(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }
}
