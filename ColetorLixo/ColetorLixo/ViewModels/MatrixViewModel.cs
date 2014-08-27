using ColetorLixo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ColetorLixo.ViewModels
{
    public class MatrixViewModel
    {
        #region Properties

        public Cell[,] Ambient;
        public StringBuilder Html { get; set; }

        #endregion

        #region Constructor

        public MatrixViewModel(int matrixSizeWidth, int matrixSizeHeight)
        {
            Ambient = new Cell[matrixSizeWidth, matrixSizeHeight];

            for (int i = 0; i < Ambient.GetLength(0); i++)
                for (int j = 0; j < Ambient.GetLength(1); j++)
                    Ambient[i, j] = new Cell(i, j);
        }

        internal void AddCollector(int x, int y)
        {
            AddAgent(new Collector(x, y, 10, 10));
        }

        internal void AddGarbage(int x, int y, EnumGarbageType garbageType)
        {
            AddAgent(new Garbage(x, y, garbageType, EnumAgentType.GARBAGE));
        }

        public void AddTrash(int x, int y, EnumGarbageType garbageType)
        {
            AddAgent(new Trash(x, y, 10, garbageType));
        }

        public void AddCharger(int x, int y)
        {
            AddAgent(new Charger(x, y, 2));
        }

        private void AddAgent(Agent agent)
        {
            for (int i = 0; i < Ambient.GetLength(0); i++)
                for (int j = 0; j < Ambient.GetLength(1); j++)
                    if (i.Equals(agent.X) && j.Equals(agent.Y))
                        Ambient[i, j].Agent = agent;
        }

        #endregion
    }
}
