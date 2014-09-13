using ColetorLixo.Utils;
using ColetorLixo.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ColetorLixo.Models
{
    public class Collector : Agent
    {
        #region Properties

        public int BatteryLevel { get; set; }
        public EnumCollectorStates ActualState { get; set; }
        public List<Garbage> GarbageInside { get; set; }
        public List<Charger> Chargers { get; set; }
        public List<Trash> Trashes { get; set; }
        public List<Cell> InvalidCells { get; set; }
        public List<Cell> NeighborsCells { get; set; }
        public List<Cell> VisitedCells { get; set; }
        public Cell ActualCell { get; set; }
        public Boolean MoveLeft { get; set; }
        public Boolean MoveUp { get; set; }

        #endregion

        #region Constructor

        public Collector(int x, int y, int capacity, int battery)
            : base(x, y, EnumAgentType.COLLECTOR, capacity)
        {
            this.BatteryLevel = battery;
            ActualState = EnumCollectorStates.LIMPAR;
            GarbageInside = new List<Garbage>();
            Chargers = new List<Charger>();
            Trashes = new List<Trash>();
            InvalidCells = new List<Cell>();
            NeighborsCells = new List<Cell>();
            VisitedCells = new List<Cell>();
            FullLoad = false;
        }

        #endregion

        #region Methods

        public void MoveCollector(MatrixViewModel matrixVM, Cell colCell)
        {
            var garbages = LookGarbages(colCell, matrixVM);
            //if (IsNeedCharge())
            //{
            //    Charger nearest = FindNearestCharger(colCell);

            //    //Guarda o agente atual
            //    Agent tmp = (Agent)colCell.Agent;
            //    //Tira o agente da celula antiga
            //    matrixVM.Ambient[colCell.X, colCell.Y].Agent = null;
            //    //Busca a nova posicao mais prxima do objetivo
            //    Cell newCell = MoveToObjectiveWithAStarAlg(colCell, new Cell(nearest.X, nearest.Y), matrixVM);
            //    //Coloca o agente na celula nova
            //    matrixVM.Ambient[newCell.X, newCell.Y].Agent = tmp; 

            //    List<Cell> neighbors = GetNeighbors(colCell, matrixVM, false);
            //    List<Cell> possibleCell = GetPathToColectorTrash(colCell, neighbors);
            //    if (possibleCell.Any(x => x.Agent != null && ((Agent)x.Agent).AgentType.Equals(EnumAgentType.CHARGER)))
            //        GoCharge(10);                               
            //}
            //else if (IsFullGarbage())
            //{
            //    foreach (Garbage garb in GarbageInside)
            //    {
            //        Trash nearest = FindNearestTrash(colCell, garb.GarbageType);
            //        //TODO: GoEmpty(nearest);
            //    }
            //}
            if (garbages.Count() > 0)
            {
                //Guarda o agente atual
                Agent tmp = (Agent)colCell.Agent;                
                //Tira o agente da celula antiga
                matrixVM.Ambient[colCell.X, colCell.Y].Agent = null;
                //Busca a nova posicao mais prxima do objetivo
                Cell newCell = MoveToObjectiveWithAStarAlg(colCell, new Cell(garbages.First().X, garbages.First().Y), matrixVM);

                if (newCell.Agent!= null && ((Agent)newCell.Agent).AgentType.Equals(EnumAgentType.GARBAGE))
                    AddGarbageLoad((Garbage)newCell.Agent, 1);

                //Coloca o agente na celula nova
                matrixVM.Ambient[newCell.X, newCell.Y].Agent = tmp;
            }
            else
                Movement.DefaultMovement(matrixVM, colCell);

            this.BatteryLevel--;
        }

        public int CalculateDistance(Cell posInit, Cell posFim)
        {
            return Convert.ToInt32(Math.Sqrt(Math.Pow((posInit.X - posFim.X), 2) + Math.Pow((posInit.Y - posFim.Y), 2)));
        }

        public Cell MoveToObjectiveWithAStarAlg(Cell actual, Cell objective, MatrixViewModel matrix)
        {
            List<Cell> neighbors = GetNeighbors(actual, matrix);

            if (neighbors.Count == 0) 
                return actual;

            //Busca os passos possiveis do agente
            List<Cell> possibleCell = GetPathToGarbage(actual, neighbors);

            if (objective == null)
                objective = possibleCell.First();

            int max = int.MaxValue;
            foreach (Cell neighbor in possibleCell)
            {
                if (neighbor == objective)
                    return neighbor;
                else
                {
                    int distance = CalculateDistance(neighbor, objective);

                    if (max > distance)
                    {
                        max = distance;
                        actual = neighbor;
                    }
                }
            }
            
            return actual;
        }

        private List<Cell> GetPathToGarbage(Cell actual, List<Cell> neighbors)
        {
            return neighbors.Where(x =>
                (x.Agent == null || ((Agent)x.Agent).AgentType.Equals(EnumAgentType.GARBAGE)) &&
                (
                    ((actual.X + 1).Equals(x.X) && actual.Y.Equals(x.Y)) ||
                    (actual.X == x.X && (actual.Y + 1).Equals(x.Y)) ||
                    (actual.X == x.X && (actual.Y - 1).Equals(x.Y)) ||
                    ((actual.X - 1).Equals(x.X) && actual.Y.Equals(x.Y)) ||
                    ((actual.X - 1).Equals(x.X) && (actual.Y + 1).Equals(x.Y)) ||
                    ((actual.X - 1).Equals(x.X) && (actual.Y - 1).Equals(x.Y)) ||                    
                    ((actual.X + 1).Equals(x.X) && (actual.Y + 1).Equals(x.Y)) ||
                    ((actual.X + 1).Equals(x.X) && (actual.Y - 1).Equals(x.Y))
                )).ToList();
        }

        private List<Cell> GetPathToColectorTrash(Cell actual, List<Cell> neighbors)
        {
            return neighbors.Where(x =>
                (
                    ((actual.X + 1).Equals(x.X) && actual.Y.Equals(x.Y)) ||
                    (actual.X == x.X && (actual.Y + 1).Equals(x.Y)) ||
                    (actual.X == x.X && (actual.Y - 1).Equals(x.Y)) ||
                    ((actual.X - 1).Equals(x.X) && actual.Y.Equals(x.Y)) ||
                    ((actual.X - 1).Equals(x.X) && (actual.Y + 1).Equals(x.Y)) ||
                    ((actual.X - 1).Equals(x.X) && (actual.Y - 1).Equals(x.Y)) ||
                    ((actual.X + 1).Equals(x.X) && (actual.Y + 1).Equals(x.Y)) ||
                    ((actual.X + 1).Equals(x.X) && (actual.Y - 1).Equals(x.Y))
                )).ToList();
        }
        
        public List<Cell> GetNeighbors(Cell celula, MatrixViewModel matrix, bool removeInvalids = true)
        {
            NeighborsCells = new List<Cell>();

            for (int i = -2; i <= 2; i++)
            {
                for (int j = -2; j <= 2; j++)
                {
                    if (((celula.X + i) >= 0 && (celula.X + i < matrix.Ambient.GetLength(0))) &&
                        (celula.Y + j) >= 0 && (celula.Y + j < matrix.Ambient.GetLength(1)))
                        NeighborsCells.Add(matrix.Ambient[celula.X + i, celula.Y + j]);
                }
            }

            if (removeInvalids)
            {
                NeighborsCells.Remove(celula);
                foreach (Cell c in this.InvalidCells)
                {
                    Cell i = NeighborsCells.Where(x => x.X == c.X && x.Y == c.Y).FirstOrDefault();
                    if (i != null)
                        NeighborsCells.Remove(i);
                }
                foreach (Cell c in this.VisitedCells)
                {
                    Cell i = NeighborsCells.Where(x => x.X == c.X && x.Y == c.Y).FirstOrDefault();
                    if (i != null)
                        NeighborsCells.Remove(i);
                }
            }

            return NeighborsCells;
        }

        #region Garbage Methods

        public void AddGarbageLoad(Garbage garbage, int load)
        {
            if (!FullLoad && load <= (GarbageCapacity - GarbageLoad))
            {
                GarbageLoad += load;
                GarbageInside.Add(garbage);
                if (GarbageLoad == GarbageCapacity)
                    FullLoad = true;
            }
        }

        public List<Garbage> LookGarbages(Cell actual, MatrixViewModel matrixVM)
        {
            List<Garbage> ret = new List<Garbage>();
            var list = GetNeighbors(actual, matrixVM);

            if(list != null)
                foreach (Cell c in list)
                {
                    if(c.Agent != null && ((Agent)c.Agent).AgentType.Equals(EnumAgentType.GARBAGE))
                        ret.Add(c.Agent as Garbage);
                }

            return ret;
        }

        #endregion

        #region Charger Methods

        private Boolean IsNeedCharge()
        {
            if (BatteryLevel < 0)
                return true;

            return false;
        }

        private void GoCharge(int charge)
        {
            BatteryLevel = charge;
        }

        private Charger FindNearestCharger(Cell actual)
        {
            //retorna o carregador mais proximo do coletor na lista de carregadores do coletor
            Charger nearest = Chargers.Where(x => !x.UsedPositionOne || !x.UsedPositionTwo ).FirstOrDefault();
            if(nearest != null)
            {
                //distancia do primeiro da lista
                double d = CalculateDistance(actual, new Cell(nearest.X, nearest.Y));
                foreach (Charger charger in Chargers.Where(x => !x.UsedPositionOne || !x.UsedPositionTwo))
                {
                    //verifica a distancia de cada carregador da lista
                    double distance = CalculateDistance(actual, new Cell(charger.X, charger.Y));
                    //se a distancia do carregador for menor, atualiza o primeiro
                    if (d > distance)
                        nearest = charger;
                }
            }
            return nearest;
        }

        #endregion

        #region Trash Methods

        private void GoEmpty(Trash nearestTrashForType)
        {
            //Movimenta ate a lixeira
            //Coloca o lixo no Load da Lixeira
            FullLoad = false;
            GarbageLoad = 0;
        }

        private Trash FindNearestTrash(Cell actual, EnumGarbageType enumGarbageType)
        {
            //retorna o carregador mais proximo do coletor na lista de carregadores do coletor
            Trash nearest = Trashes.Where(x => !x.IsFullLoaded && x.GarbageType.Equals(enumGarbageType)).FirstOrDefault();
            if (nearest != null)
            {
                //distancia do primeiro da lista
                double d = CalculateDistance(actual, new Cell(nearest.X, nearest.Y));
                foreach (Trash trash in Trashes.Where(x => !x.IsFullLoaded && x.GarbageType.Equals(enumGarbageType)))
        {
                    //verifica a distancia de cada carregador da lista
                    double distance = CalculateDistance(actual, new Cell(trash.X, trash.Y));
                    //se a distancia do carregador for menor, atualiza o primeiro
                    if (d > distance)
                        nearest = trash;
                }
            }
            return nearest;
        }

        private bool IsFullGarbage()
        {
            return FullLoad;
        }

        #endregion

        #endregion

    }
}