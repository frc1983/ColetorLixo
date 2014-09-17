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
        private static readonly Random _global = new Random();

        [ThreadStatic]
        private static Random _local;

        public static int GetNextX(Cell[,] ambient)
        {
            if (_local == null)
            {
                int seed;
                lock (_global)
                {
                    seed = _global.Next(0, ambient.GetLength(0));
                }
                _local = new Random(seed);
            }

            return _local.Next(0, ambient.GetLength(0));
        }

        public static int GetNextY(Cell[,] ambient)
        {
            if (_local == null)
            {
                int seed;
                lock (_global)
                {
                    seed = _global.Next(0, ambient.GetLength(1));
                }
                _local = new Random(seed);
            }

            return _local.Next(0, ambient.GetLength(1));
        }
    }
}
