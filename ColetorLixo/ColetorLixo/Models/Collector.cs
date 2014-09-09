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
        private Boolean MoveToLeft { get; set; }

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
            if (IsNeedCharge(colCell))
            {
                Charger nearest = FindNearestCharger(colCell);
                //TODO: GoCharge(nearest, 10);
            }

            if (IsFullGarbage())
            {
                foreach (Garbage garb in GarbageInside)
                {
                    Trash nearest = FindNearestTrash(colCell, garb.GarbageType);
                    //TODO: GoEmpty(nearest);
                }
            }

            //Se o coletor procurar por lixo (LookGarbages) nas posicoes ao seu redor 
            //e nao achar, move pra frente ate acabar e ter q descer
            var garbages = LookGarbages(colCell, matrixVM);
            if (garbages.Count() > 0)
            {
                //Se o coletor procurar por lixo (LookGarbages) e encontrar, 
                //move para a posicao do Agente lixo na celula encontrada
                MoveToObjectiveWithAStarAlg(
                    new Cell(garbages.First().X, garbages.First().Y), NeighborsCells, VisitedCells, colCell, matrixVM);
                //Se o agente na celula para onde o coletor for movido for tipo garbage, 
                //chama addGarbageLoad e passa o agente da posicao

            }        

            DefaultMovement(matrixVM, colCell);            
        }

        private void DefaultMovement(MatrixViewModel matrixVM, Cell colCell)
        {
            if (!MoveToLeft)
            {
                if (colCell.X + 1 < matrixVM.Ambient.GetLength(0) &&
                (matrixVM.Ambient[colCell.X + 1, colCell.Y].Agent == null ||
                matrixVM.Ambient[colCell.X + 1, colCell.Y].Agent.AgentType.Equals(EnumAgentType.GARBAGE)))
                {
                    Agent tmp = colCell.Agent;
                    matrixVM.Ambient[colCell.X, colCell.Y].Agent = null;
                    colCell.X = colCell.X + 1;
                    matrixVM.Ambient[colCell.X, colCell.Y].Agent = tmp;
                }
                else if (colCell.X + 1 == matrixVM.Ambient.GetLength(0) &&
                (matrixVM.Ambient[colCell.X, colCell.Y + 1].Agent == null ||
                matrixVM.Ambient[colCell.X, colCell.Y + 1].Agent.AgentType.Equals(EnumAgentType.GARBAGE)))
            {
                Agent tmp = colCell.Agent;
                matrixVM.Ambient[colCell.X, colCell.Y].Agent = null;
                colCell.Y = colCell.Y + 1;
                matrixVM.Ambient[colCell.X, colCell.Y].Agent = tmp;
                    MoveToLeft = true;
            }
            }
            else if (MoveToLeft)
            {
                if (colCell.X - 1 >= 0 &&
                (matrixVM.Ambient[colCell.X - 1, colCell.Y].Agent == null ||
                matrixVM.Ambient[colCell.X - 1, colCell.Y].Agent.AgentType.Equals(EnumAgentType.GARBAGE)))
                {
                    Agent tmp = colCell.Agent;
                    matrixVM.Ambient[colCell.X, colCell.Y].Agent = null;
                    colCell.X = colCell.X - 1;
                    matrixVM.Ambient[colCell.X, colCell.Y].Agent = tmp;
                }
                else if (colCell.X - 1 < 0 &&
                    (matrixVM.Ambient[colCell.X, colCell.Y + 1].Agent == null ||
                    matrixVM.Ambient[colCell.X, colCell.Y + 1].Agent.AgentType.Equals(EnumAgentType.GARBAGE)))
                {
                    Agent tmp = colCell.Agent;
                    matrixVM.Ambient[colCell.X, colCell.Y].Agent = null;
                    colCell.Y = colCell.Y + 1;
                    matrixVM.Ambient[colCell.X, colCell.Y].Agent = tmp;
                    MoveToLeft = false;
                }
            }

            ((Collector)matrixVM.Ambient[colCell.X, colCell.Y].Agent).VisitedCells.Add(colCell);
            ((Collector)matrixVM.Ambient[colCell.X, colCell.Y].Agent).BatteryLevel--;
        }

        public double CalculateDistance(Cell posInit, Cell posFim)
        {
            return Math.Sqrt(Math.Pow((posInit.X - posFim.X), 2) + Math.Pow((posInit.Y - posFim.Y), 2));
        }

        //TODO: testar função - Nathan Abreu
        public Cell MoveToObjectiveWithAStarAlg(Cell objective,List<Cell> invalid, List<Cell> visited, Cell actual, MatrixViewModel matrix)
        {
            List<Cell> neighbors = GetNeighbors(actual, matrix, invalid, visited);
            //Exclue dos vizinhos as células já visitadas
            neighbors = neighbors.Except(visited).ToList();
            //se a lista de vizinhos ficar vazia quer dizer que não existe caminho válido então retorna a célula atual
            if (neighbors.Count == 0) 
                return actual;

            double menor = 0;
            Cell result = actual;
            foreach (Cell neighbor in neighbors)
            {
                if (neighbor == objective)
                    return neighbor;
                else
                {
                    double value1 = CalcValueDistance(neighbor, objective);
                    List<Cell> Nvisitados = null;
                    Nvisitados = visited;
                    Nvisitados.Add(actual);
                    List<Cell> Nneighbors = getNeighbors(neighbor, matrix, invalid, Nvisitados);
                    double value2 = 1000;
                    foreach (Cell n in Nneighbors)
                    {
                        if (n == objective)
                        {
                            return neighbor;
                        }
                        else
                        {
                            double temp = CalculateDistance(n, objective);
                            if (value2 > temp)
                            {
                                value2 = temp;
                            }
                        }
                    }

                    if (menor > value1 + value2)
                    {
                        menor = value1 + value2;
                        result = neighbor;
                    }
                }
            }

            return result;
        }

        //método que retorna todos os vizinhos da celula não considerando inválidos e considerando limite da matriz - Nathan Abreu
        public List<Cell> GetNeighborsComLimitesMatriz(Cell celula, MatrixViewModel matrix)
        {
            NeighborsCells = new List<Cell>();
            List<Cell> neighbors = null;
            //Add (i-1,j-1)
            if (celula.X - 1 >= 0 && celula.Y - 1 >= 0) 
                NeighborsCells.Add(new Cell(celula.X - 1, celula.Y - 1));
            //Add (i-1,j)
            if (celula.X - 1 >= 0)
                NeighborsCells.Add(new Cell(celula.X - 1, celula.Y));
            //Add (i-1,j+1)
            if (celula.X - 1 >= 0 && celula.Y + 1 >= matrix.Ambient.GetLength(1))
                NeighborsCells.Add(new Cell(celula.X - 1, celula.Y + 1));
            //Add (i,j+1)
            if (celula.Y + 1 >= matrix.Ambient.GetLength(1))
                NeighborsCells.Add(new Cell(celula.X, celula.Y + 1));
            //Add (i+1,j+1)
            if (celula.X + 1 >= matrix.Ambient.GetLength(0) && celula.Y + 1 >= matrix.Ambient.GetLength(1))
                NeighborsCells.Add(new Cell(celula.X+1, celula.Y + 1));
            //Add (i+1,j)
            if (celula.X + 1 >= matrix.Ambient.GetLength(0))
                NeighborsCells.Add(new Cell(celula.X, celula.Y + 1));
            //Add (i+1,j-1)
            if (celula.X + 1 >= matrix.Ambient.GetLength(0) && celula.Y-1 >= 0)
                NeighborsCells.Add(new Cell(celula.X + 1, celula.Y-1));
            //Add (i,j-1)
            if (celula.Y - 1 >= 0)
                NeighborsCells.Add(new Cell(celula.X , celula.Y - 1));

            return NeighborsCells;
        }

        //método que retorna todos os vizinhos da celula considerando inválidos, visitados e limite da matriz - Nathan Abreu
        public List<Cell> GetNeighbors(Cell celula, MatrixViewModel matrix, List<Cell> invalidos, List<Cell> visitados)
        {
            return GetNeighborsComLimitesMatriz(celula, matrix).Except(invalidos).Except(visitados).ToList();
        }

        //método que retorna todos os vizinhos da celula não considerando inválidos e considerando limite da matriz - Nathan Abreu
        public int getValorNeighborsComSentido(Cell celula,Cell celulaN, MatrixViewModel matrix, List<Cell> ocupadas)
        {
            
            if((celulaN.X == celula.X-1) && (celulaN.Y == celula.Y-1)){
                if((celula.X-2<0) && (celula.Y-2<0)){ return 0;}
                else if(ocupadas.Contains(new Cell(celulaN.X-2,celula.Y-2))){return 1;}    
                else return -1;
            }
            if ((celulaN.X == celula.X-1) && (celulaN.Y == celula.Y)) {
            if (celula.X - 2 < 0) { return 0; }
            else if (ocupadas.Contains(new Cell(celulaN.X-2, celula.Y))) { return 1; }
            else return -1;
            }

            if ((celulaN.X == celula.X - 1) && (celulaN.Y == celula.Y + 1)){
            if ((celula.X - 2 < 0) && (celula.Y + 2 < matrix.Ambient.GetLength(1))) { return 0; }
            else if (ocupadas.Contains(new Cell(celulaN.X - 2, celula.Y + 2))) { return 1; }
            else return -1;
            }
            if ((celulaN.X == celula.X) && (celulaN.Y == celula.Y + 1)) {
            if((celula.Y + 2 < matrix.Ambient.GetLength(0))){ return 0;}
                else if(ocupadas.Contains(new Cell(celulaN.X, celula.Y + 2))){return 1;}    
                else return -1;
            }
            if ((celulaN.X == celula.X + 1) && (celulaN.Y == celula.Y + 1)) {
            if((celula.X + 2 < matrix.Ambient.GetLength(0)) && (celula.Y + 2 < matrix.Ambient.GetLength(1))){ return 0;}
                else if(ocupadas.Contains(new Cell(celulaN.X + 2, celula.Y + 2))){return 1;}    
                else return -1;
            }
            if ((celulaN.X == celula.X + 2) && (celulaN.Y == celula.Y)) {
            if((celula.X + 2 < matrix.Ambient.GetLength(1))){ return 0;}
                else if(ocupadas.Contains(new Cell(celulaN.X + 2, celula.Y))){return 1;}    
                else return -1;
            }
            if ((celulaN.X == celula.X + 1) && (celulaN.Y == celula.Y - 1)){
                if ((celula.X + 2 < 0) && (celula.Y - 2 < matrix.Ambient.GetLength(1))) { return 0; }
                else if(ocupadas.Contains(new Cell(celulaN.X - 2, celula.Y - 2))){return 1;}    
                else return -1;
            }
            if ((celulaN.X == celula.X) && (celulaN.Y == celula.Y - 2)) {
                if (celula.Y - 2 < 0) { return 0; }
                else if(ocupadas.Contains(new Cell(celulaN.X, celula.Y - 2))){return 1;}    
                else return -1;
            }
            return 1;
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
            var list = GetNeighbors(actual, matrixVM, InvalidCells, VisitedCells);

            if(list != null)
                foreach (Cell c in list)
                {
                    if(c.Agent != null && c.Agent.AgentType.Equals(EnumAgentType.GARBAGE))
                        ret.Add(c.Agent as Garbage);
                }

            return ret;
        }

        #endregion

        #region Charger Methods

        private Boolean IsNeedCharge(Cell actual)
        {
            var cell = FindNearestCharger(actual);
            if (BatteryLevel == (cell.X - this.X) || BatteryLevel == (cell.Y - this.Y))
                return true;

            return false;
        }

        private void GoCharge(Charger charger, int charge)
        {
            //Moviementa ate onde esta o nearest charger
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