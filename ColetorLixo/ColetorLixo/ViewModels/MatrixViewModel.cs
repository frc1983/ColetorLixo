using ColetorLixo.Models;
using ColetorLixo.Utils;
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
        public StringBuilder Status { get; set; }

        #endregion

        #region Constructor

        public MatrixViewModel(int matrixSizeWidth, int matrixSizeHeight)
        {
            Ambient = new Cell[matrixSizeWidth, matrixSizeHeight];

            for (int i = 0; i < Ambient.GetLength(0); i++)
                for (int j = 0; j < Ambient.GetLength(1); j++)
                    Ambient[i, j] = new Cell(i, j);
        }

        #endregion

        #region Methods

        internal void AddCollector(int x, int y)
        {
            AddAgent(new Collector(x, y, 4, 10));
        }

        internal void AddGarbage(int x, int y, EnumGarbageType garbageType)
        {
            if (Ambient[x, y].Agent == null && Ambient[x, y].Garbage == null)
            {
                Ambient[x, y].Garbage = new Garbage(x, y, garbageType);
                UpdateInvalidCells();
            }
            else
            {
                x = RandomPositions.GetNextX(Ambient);
                y = RandomPositions.GetNextY(Ambient);
                AddGarbage(x, y, garbageType);
                return;
            }
        }

        internal void AddTrash(int x, int y, EnumGarbageType garbageType)
        {
            AddAgent(new Trash(x, y, 2, garbageType));
        }

        internal void AddCharger(int x, int y)
        {
            AddAgent(new Charger(x, y, 2));
        }

        private void AddAgent(Agent agent)
        {
            if (Ambient[agent.X, agent.Y].Agent == null && Ambient[agent.X, agent.Y].Garbage == null)
            {
                Ambient[agent.X, agent.Y].Agent = agent;
                UpdateInvalidCells();
            }
            else
            {
                agent.X = RandomPositions.GetNextX(Ambient);
                agent.Y = RandomPositions.GetNextY(Ambient);
                AddAgent(agent);
                return;
            }
        }

        internal List<Cell> GetCollectors()
        {
            List<Cell> list = new List<Cell>();

            for (int i = 0; i < Ambient.GetLength(0); i++)
                for (int j = 0; j < Ambient.GetLength(1); j++)
                {
                    if (Ambient[i, j].Agent != null && (((Agent)Ambient[i, j].Agent).AgentType.Equals(EnumAgentType.COLLECTOR)))
                        list.Add(Ambient[i, j]);
                }

            return list;
        }

        internal List<Cell> GetAllAgents()
        {
            List<Cell> list = new List<Cell>();

            for (int i = 0; i < Ambient.GetLength(0); i++)
                for (int j = 0; j < Ambient.GetLength(1); j++)
                {
                    if (Ambient[i, j].Agent != null)
                        list.Add(Ambient[i, j]);
                }

            return list;
        }

        private void UpdateInvalidCells()
        {
            foreach (Cell colector in GetCollectors())
            {
                foreach (Cell cell in GetAllAgents())
                {
                    Agent agent = (Agent)cell.Agent;
                    if (agent.AgentType.Equals(EnumAgentType.TRASH))
                    {
                        ((Collector)colector.Agent).Trashes.Add((Trash)agent);
                        ((Collector)colector.Agent).InvalidCells.Add(new Cell(agent.X, agent.Y));
                    }
                    if (agent.AgentType.Equals(EnumAgentType.CHARGER))
                    {
                        ((Collector)colector.Agent).Chargers.Add((Charger)agent);
                        ((Collector)colector.Agent).InvalidCells.Add(new Cell(agent.X, agent.Y));
                    }
                }
            }
        }

        #endregion
    }
}
