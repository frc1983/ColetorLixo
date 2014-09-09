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
            FullLoad = false;
        }

        #endregion

        #region Methods

        public void MoveCollector(MatrixViewModel matrixVM, Cell colCell)
        {
            //Se o coletor procurar por lixo (LookGarbages) nas posicoes ao seu redor e nao achar, move pra frente ate acabar e ter q descer
            //Se o coletor procurar por lixo (LookGarbages) e encontrar, move para a posicao do Agente lixo na celula encontrada

            //Se o agente na celula para onde o coletor for movido for tipo garbage, chama addGarbageLoad e passa o agente da posicao

            //Depois de recolher o lixo, pode voltar para posicao anterior ou so seguir indo pro lado no X

            if (IsNeedCharge())
            {
                Charger nearest = FindNearestCharger();
                GoCharge(nearest, 10);
            }

            if (IsFullGarbage())
            {
                foreach (Garbage garb in GarbageInside)
                {
                    Trash nearest = FindNearestTrash(garb.GarbageType);
                    GoEmpty();
                }
            }

            if (colCell.Y + 1 < matrixVM.Ambient.GetLength(1) &&
                (matrixVM.Ambient[colCell.X, colCell.Y + 1].Agent == null ||
                matrixVM.Ambient[colCell.X, colCell.Y + 1].Agent.AgentType.Equals(EnumAgentType.GARBAGE)))
            {
                Agent tmp = colCell.Agent;
                matrixVM.Ambient[colCell.X, colCell.Y].Agent = null;
                colCell.Y = colCell.Y + 1;
                matrixVM.Ambient[colCell.X, colCell.Y].Agent = tmp;
            }

            ((Collector)matrixVM.Ambient[colCell.X, colCell.Y].Agent).BatteryLevel--;
        }

        public double CalcValueDistance(Cell posInit, Cell posFim)
        {
            return Math.Sqrt(Math.Pow((posInit.X - posFim.X), 2) + Math.Pow((posInit.Y - posFim.Y), 2));
        }

        //TODO: testar função - Nathan Abreu
        public Cell MoveToObjectiveWithAStarAlg(Cell objective,List<Cell> invalid, List<Cell> neighbors, List<Cell> visited, Cell actual, MatrixViewModel matrix)
        {
            //Exclue dos vizinhos as células já visitadas
            neighbors = neighbors.Except(visited).ToList();
            //se a lista de vizinhos ficar vazia quer dizer que não existe caminho válido então retorna a célula atual
            if (neighbors.Count == 0) return actual;

            double menor = 1000;
            Cell result = actual;
            foreach (Cell c in neighbors)
            {
                if (c == objective)
                {
                    return c;
                }
                else
                {
                    double value = CalcValueDistance(actual, c);
                    List<Cell> Nvisitados = null;
                    Nvisitados = visited;
                    Nvisitados.Add(actual);
                    List<Cell> Nneighbors = getNeighbors(c, matrix, invalid, Nvisitados);
                    double value2 = 1000;
                    Cell n2 = null;
                    foreach (Cell n in Nneighbors)
                    {
                        if (n == objective)
                        {
                            return n;
                        }
                        else
                        {
                            double temp = CalcValueDistance(c, objective);
                            if (value2 > temp)
                            {
                                value2 = temp;
                                n2 = n;
                            }
                        }

                    }
                    if (menor > value + value2)
                    {
                        menor = value + value2;
                        result = c;
                    }


                }
            }


            return result;
        }

        //método que retorna todos os vizinhos da celula não considerando inválidos e considerando limite da matriz - Nathan Abreu
        public List<Cell> getNeighborsComLimitesMatriz(Cell celula, MatrixViewModel matrix)
        {
            List<Cell> neighbors = null;

            if (celula.X - 1 >= 0 && celula.Y - 1 >= 0) neighbors.Add(new Cell(celula.X - 1, celula.Y - 1));
            if (celula.X - 1 >= 0) neighbors.Add(new Cell(celula.X - 1, celula.Y));
            if (celula.X - 1 >= matrix.Ambient.GetLength(0) && celula.Y + 1 >= matrix.Ambient.GetLength(1))
                neighbors.Add(new Cell(celula.X - 1, celula.Y + 1));

            if (celula.Y - 1 >= 0) neighbors.Add(new Cell(celula.X, celula.Y - 1));
            if (celula.Y + 1 >= matrix.Ambient.GetLength(0)) neighbors.Add(new Cell(celula.X, celula.Y + 1));

            if (celula.X + 1 >= matrix.Ambient.GetLength(0) && celula.Y - 1 >= 0) neighbors.Add(new Cell(celula.X + 1, celula.Y - 1));
            if (celula.X + 1 >= matrix.Ambient.GetLength(0) && celula.Y >= 0) neighbors.Add(new Cell(celula.X + 1, celula.Y));
            if (celula.X + 1 >= matrix.Ambient.GetLength(0) && celula.Y + 1 >= matrix.Ambient.GetLength(1)) neighbors.Add(new Cell(celula.X + 1, celula.Y + 1));

            return neighbors;
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

        //método que retorna todos os vizinhos da celula considerando inválidos, visitados e limite da matriz - Nathan Abreu
        public List<Cell> getNeighbors(Cell celula, MatrixViewModel matrix, List<Cell> invalidos, List<Cell> visitados)
        {
            return getNeighborsComLimitesMatriz(celula, matrix).Except(invalidos).ToList().Except(visitados).ToList();
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

        public List<Garbage> LookGarbages(MatrixViewModel matrixVM, int lookDistance)
        {
            List<Garbage> ret = new List<Garbage>();

            return ret;
        }

        #endregion

        #region Charger Methods

        private Boolean IsNeedCharge()
        {
            if (BatteryLevel == (FindNearestCharger().Agent.X - this.X) || BatteryLevel == (FindNearestCharger().Agent.Y - this.Y))
                return true;

            return false;
        }

        private void GoCharge(Charger charger, int charge)
        {
            //Moviementa ate onde esta o nearest charger
            BatteryLevel = charge;
        }

        private Charger FindNearestCharger()
        {
            //retorna o carregador mais proximo do coletor na lista de carregadores do coletor
            return Chargers.FirstOrDefault();
        }

        #endregion

        #region Trash Methods

        private void GoEmpty()
        {
            //Movimenta ate a lixeira
            FullLoad = false;
            GarbageLoad = 0;
        }

        private Trash FindNearestTrash(EnumGarbageType enumGarbageType)
        {
            //procura a lixeira mais proxima do tipo parametrizado e retorna
            return Trashes.FirstOrDefault();
        }

        private bool IsFullGarbage()
        {
            return FullLoad;
        }

        #endregion

        #endregion

    }
}