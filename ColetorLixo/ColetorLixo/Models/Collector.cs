using ColetorLixo.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ColetorLixo.Models
{
    public class Collector : Agent
    {
        public int BatteryLevel { get; set; }
        public EnumCollectorStates ActualState { get; set; }
        public List<Garbage> GarbageInside { get; set; }
        public List<Charger> Chargers { get; set; }
        public List<Trash> Trashes { get; set; }

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

    }
}