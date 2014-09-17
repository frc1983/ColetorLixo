using ColetorLixo.Models;
using ColetorLixo.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace ColetorLixo.Utils
{
    public class ConstructStatus
    {
        public static void BuildStatus(MatrixViewModel matrixVM)
        {
            matrixVM.Status = new StringBuilder();
            matrixVM.Status.Append("<fieldset><legend>Status dos Agentes:</legend>");
            foreach (Cell cell in matrixVM.GetAllAgents())
            {
                Agent agent = (Agent)cell.Agent;
                matrixVM.Status.Append("Tipo: " + agent.AgentType + "<br />");
                matrixVM.Status.Append("X: " + (cell.X + 1) + " Y: " + (cell.Y + 1) + "<br />");

                if (agent.AgentType == EnumAgentType.COLLECTOR)
                {
                    matrixVM.Status.Append("Bateria: " + ((Collector)agent).BatteryLevel + "<br />");
                    matrixVM.Status.Append("Capacidade de lixo? " + ((Collector)agent).GarbageCapacity + "<br />");
                    matrixVM.Status.Append("Cheio de lixo? " + ((Collector)agent).FullLoad + "<br />");
                    matrixVM.Status.Append("Lixos dentro: " + ((Collector)agent).GarbageInside.Count + "<br />");
                    foreach (Garbage garbage in ((Collector)agent).GarbageInside.OrderBy(x => x.GarbageType))
                        matrixVM.Status.Append("    Tipo: " + garbage.GarbageType + "<br />");
                }
                else if (agent.AgentType == EnumAgentType.TRASH)
                {
                    matrixVM.Status.Append("Lixeira de: " + ((Trash)agent).GarbageType + "<br />");
                    matrixVM.Status.Append("Posição: " + ((Trash)agent).X + ", " + ((Trash)agent).Y + "<br />");
                    matrixVM.Status.Append("Capacidade de lixo? " + ((Trash)agent).GarbageCapacity + "<br />");
                    matrixVM.Status.Append("Lixos dentro: " + ((Trash)agent).GarbageLoad + "<br />");
                    matrixVM.Status.Append("Está cheia? " + ((Trash)agent).FullLoad + "<br />");
                }
                else if (agent.AgentType == EnumAgentType.CHARGER)
                {
                    matrixVM.Status.Append("Carregando:<br />");
                    for (int i = 0; i < ((Charger)agent).UsedPositions.Length; i++)
                        matrixVM.Status.Append("    Na posição " + (i+1) + ": " + (((Charger)agent).UsedPositions[i] != null).ToString() + "<br />");
                }
                matrixVM.Status.Append("<br />");
            }
            matrixVM.Status.Append("</fieldset>");
        }
    }
}