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

        public Guid Id { get; set; }
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
        public Boolean IsCharged { get; set; }
        public Cell NextCell { get; set; }

        #endregion

        #region Constructor

        public Collector(int x, int y, int capacity, int battery)
            : base(x, y, EnumAgentType.COLLECTOR, capacity)
        {
            Id = Guid.NewGuid();
            BatteryLevel = battery;
            IsCharged = true;
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
            List<Cell> neighbors = GetNeighbors(colCell, matrixVM);
            List<Cell> possibleCell = GetPath(colCell, neighbors);

            if (possibleCell.Count == 0)
            {
                this.VisitedCells = new List<Cell>();
                neighbors = GetNeighbors(colCell, matrixVM);
                possibleCell = GetPath(colCell, neighbors);
            }

            var garbages = LookGarbages(colCell, neighbors);
            if (this.GarbageInside.Count == 0)
                this.FullLoad = false;

            Cell next = null;
            var hasNextCell = this.NextCell != null;
            if (NeedCharge(matrixVM, hasNextCell))
            {
                this.VisitedCells = new List<Cell>();
                neighbors = GetNeighbors(colCell, matrixVM, false);
                possibleCell = GetPath(colCell, neighbors);
                Charger nearest = FindNearestCharger(colCell);

                if (hasNextCell)
                {
                    next = this.NextCell;
                    GoCharge(10, GetRoundNeighbors(colCell, neighbors).Where(x => ((Agent)x.Agent).AgentType.Equals(EnumAgentType.CHARGER)).FirstOrDefault());
                }
                else if (nearest != null)
                {
                    next = MoveToObjectiveWithAStarAlg(colCell, new Cell(nearest.X, nearest.Y), possibleCell);
                    if (GetRoundNeighbors(colCell, neighbors).Any(x => ((Agent)x.Agent).AgentType.Equals(EnumAgentType.CHARGER)))
                    {
                        GoCharge(10, GetRoundNeighbors(colCell, neighbors).Where(x => ((Agent)x.Agent).AgentType.Equals(EnumAgentType.CHARGER)).FirstOrDefault());
                        this.NextCell = next;
                    }
                }
            }
            else if (this.FullLoad)
            {
                this.VisitedCells = new List<Cell>();
                neighbors = GetNeighbors(colCell, matrixVM, false);
                possibleCell = GetPath(colCell, neighbors);
                Trash nearest = FindNearestTrash(colCell, this.GarbageInside);

                if (nearest == null)
                    next = MoveToObjectiveWithAStarAlg(colCell, null, possibleCell);
                else
                {
                    next = MoveToObjectiveWithAStarAlg(colCell, new Cell(nearest.X, nearest.Y), possibleCell);
                    if (GetRoundNeighbors(colCell, neighbors).Any(x => ((Agent)x.Agent).AgentType.Equals(EnumAgentType.TRASH)))
                        GoEmpty(GetRoundNeighbors(colCell, neighbors)
                            .Where(x => ((Agent)x.Agent).AgentType.Equals(EnumAgentType.TRASH))
                            .FirstOrDefault());
                }
            }
            else if (garbages.Count() > 0)
            {
                this.VisitedCells = new List<Cell>();

                next = MoveToObjectiveWithAStarAlg(colCell, new Cell(garbages.First().X, garbages.First().Y), possibleCell);

                if (next.Garbage != null)
                {
                    AddGarbageLoad(next.Garbage, next.Garbage.GarbageSize);
                    matrixVM.Ambient[next.X, next.Y].Garbage = null;
                }
            }
            else
            {
                next = MoveToObjectiveWithAStarAlg(colCell, null, possibleCell);
            }
            if (next != null && BatteryLevel > 0 && !hasNextCell)
            {
                Movement.Move(matrixVM, colCell, next);
                this.VisitedCells.Add(next);
                this.BatteryLevel--;
            }
        }

        public int CalculateDistance(Cell posInit, Cell posFim)
        {
            return Convert.ToInt32(Math.Sqrt(Math.Pow((posInit.X - posFim.X), 2) + Math.Pow((posInit.Y - posFim.Y), 2)));
        }

        public Cell MoveToObjectiveWithAStarAlg(Cell actual, Cell objective, List<Cell> possibleCell)
        {
            if (objective == null)
                objective = Movement.GetNextDefaultMovement(actual, possibleCell);

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

        private List<Cell> GetPath(Cell actual, List<Cell> neighbors)
        {
            var ret = neighbors.Where(x =>
                (x.Agent == null) &&
                (
                    ((actual.X + 1).Equals(x.X) && actual.Y.Equals(x.Y)) ||
                    (actual.X == x.X && (actual.Y + 1).Equals(x.Y)) ||
                    (actual.X == x.X && (actual.Y - 1).Equals(x.Y)) ||
                    ((actual.X - 1).Equals(x.X) && actual.Y.Equals(x.Y)) ||
                    ((actual.X + 1).Equals(x.X) && (actual.Y + 1).Equals(x.Y)) ||
                    ((actual.X - 1).Equals(x.X) && (actual.Y + 1).Equals(x.Y)) ||
                    ((actual.X - 1).Equals(x.X) && (actual.Y - 1).Equals(x.Y)) ||
                    ((actual.X - 1).Equals(x.X) && (actual.Y - 1).Equals(x.Y)) ||
                    ((actual.X + 1).Equals(x.X) && (actual.Y - 1).Equals(x.Y))
                ));

            return ret.ToList();
        }

        private List<Cell> GetRoundNeighbors(Cell actual, List<Cell> neighbors)
        {
            var ret = neighbors.Where(x =>
                (x.Agent != null) &&
                (
                    ((actual.X + 1).Equals(x.X) && actual.Y.Equals(x.Y)) ||
                    (actual.X == x.X && (actual.Y + 1).Equals(x.Y)) ||
                    (actual.X == x.X && (actual.Y - 1).Equals(x.Y)) ||
                    ((actual.X - 1).Equals(x.X) && actual.Y.Equals(x.Y)) ||
                    ((actual.X + 1).Equals(x.X) && (actual.Y + 1).Equals(x.Y)) ||
                    ((actual.X - 1).Equals(x.X) && (actual.Y + 1).Equals(x.Y)) ||                    
                    ((actual.X - 1).Equals(x.X) && (actual.Y - 1).Equals(x.Y)) ||
                    ((actual.X - 1).Equals(x.X) && (actual.Y - 1).Equals(x.Y)) ||                    
                    ((actual.X + 1).Equals(x.X) && (actual.Y - 1).Equals(x.Y))
                ));

            return ret.ToList();
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

        public List<Garbage> LookGarbages(Cell actual, List<Cell> neighbors)
        {
            List<Garbage> ret = new List<Garbage>();

            if (neighbors != null)
                foreach (Cell c in neighbors)
                {
                    if (c.Garbage != null)
                        ret.Add(c.Garbage);
                }

            return ret;
        }

        #endregion

        #region Charger Methods

        private Boolean NeedCharge(MatrixViewModel matrixVM, Boolean need)
        {
            if (need) return true;

            Cell charger = FindNearestCharger(this);

            if (charger != null)
            {
                var more = matrixVM.Ambient.GetLength(0);
                if (matrixVM.Ambient.GetLength(1) > matrixVM.Ambient.GetLength(0))
                    more = matrixVM.Ambient.GetLength(1);
 
                if (BatteryLevel <= more)
                     return true;
            }

            return false;
        }

        private void GoCharge(int charge, Cell charger)
        {
            Charger c = charger.Agent as Charger;

            if (c.HasEmptyPosition())
                c.SetAgentInCharge(this);

            if (BatteryLevel < charge)
                BatteryLevel += 1;
            else
            {
                this.NextCell = null;
                c.UnsetAgentInCharge(this);
            }
        }

        private Charger FindNearestCharger(Cell actual)
        {
            //retorna o carregador mais proximo do coletor na lista de carregadores do coletor
            Charger nearest = Chargers
                .Where(x => x.UsedPositions.Any(y => y == null))
                .FirstOrDefault();

            if (nearest != null)
            {
                //distancia do primeiro da lista
                double d = CalculateDistance(actual, new Cell(nearest.X, nearest.Y));
                foreach (Charger charger in Chargers.Where(x => x.UsedPositions.Count() < 2))
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

        private void GoEmpty(Cell cell)
        {
            if (((Agent)cell.Agent).AgentType.Equals(EnumAgentType.TRASH))
            {
                Trash trash = (Trash)cell.Agent;
                List<Garbage> toRemove = new List<Garbage>();
                foreach (Garbage g in this.GarbageInside)
                {
                    if (g.GarbageType.Equals(trash.GarbageType))
                    {
                        if (trash.FullLoad)
                            break;

                        toRemove.Add(g);
                        trash.GarbageLoad++;
                        this.GarbageLoad--;
                        if (trash.GarbageLoad == trash.GarbageCapacity)
                            trash.FullLoad = true;
                    }
                }
                foreach (Garbage g in toRemove)
                {
                    this.GarbageInside.Remove(g);
                    this.FullLoad = false;
                }
            }
        }

        private Trash FindNearestTrash(Cell actual, List<Garbage> garbages)
        {
            Trash nearest = Trashes.Where(x => !x.FullLoad).FirstOrDefault();
            foreach (Garbage g in garbages)
            {
                if (nearest != null)
                {
                    //distancia do primeiro da lista
                    double d = CalculateDistance(actual, new Cell(nearest.X, nearest.Y));
                    foreach (Trash trash in Trashes.Where(x => !x.FullLoad && x.GarbageType.Equals(g.GarbageType)))
                    {
                        //verifica a distancia de cada carregador da lista
                        double distance = CalculateDistance(actual, new Cell(trash.X, trash.Y));
                        //se a distancia do carregador for menor, atualiza o primeiro
                        if (d > distance)
                            nearest = trash;
                    }
                }
            }

            return nearest;
        }

        #endregion

        #endregion

    }
}