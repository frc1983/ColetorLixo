using ColetorLixo.Models;
using ColetorLixo.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ColetorLixo.Utils
{
    public class RandomPositions : Controller
    {
        public static int GetNextX(Cell[,] ambient)
        {
            return new Random().Next(0, ambient.GetLength(0));
        }

        public static int GetNextY(Cell[,] ambient)
        {
            return new Random().Next(0, ambient.GetLength(1));
        }
    }
}
