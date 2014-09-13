using ColetorLixo.Models;
using ColetorLixo.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ColetorLixo.Utils
{
    public static class Movement
    {
        #region Movimentos Simples

        public static void DefaultMovement(MatrixViewModel matrixVM, Cell colCell)
        {
            if (!((Collector)colCell.Agent).MoveLeft)
            {
                if (colCell.X + 1 < matrixVM.Ambient.GetLength(0) &&
                (matrixVM.Ambient[colCell.X + 1, colCell.Y].Agent == null ||
                ((Agent)matrixVM.Ambient[colCell.X + 1, colCell.Y].Agent).AgentType.Equals(EnumAgentType.GARBAGE)))
                {
                    MoveToRight(matrixVM, colCell);
                }
                else if (colCell.X + 1 == matrixVM.Ambient.GetLength(0) &&
                (matrixVM.Ambient[colCell.X, colCell.Y + 1].Agent == null ||
                    ((Agent)matrixVM.Ambient[colCell.X, colCell.Y + 1].Agent).AgentType.Equals(EnumAgentType.GARBAGE)))
                {
                    ((Collector)colCell.Agent).MoveLeft = true;
                    MoveToDown(matrixVM, colCell);                    
                }
            }
            else if (((Collector)colCell.Agent).MoveLeft)
            {
                if (colCell.X - 1 >= 0 &&
                (matrixVM.Ambient[colCell.X - 1, colCell.Y].Agent == null ||
                ((Agent)matrixVM.Ambient[colCell.X - 1, colCell.Y].Agent).AgentType.Equals(EnumAgentType.GARBAGE)))
                {
                    MoveToLeft(matrixVM, colCell);
                }
                else if (colCell.X - 1 < 0 &&
                    (matrixVM.Ambient[colCell.X, colCell.Y + 1].Agent == null ||
                    ((Agent)matrixVM.Ambient[colCell.X, colCell.Y + 1].Agent).AgentType.Equals(EnumAgentType.GARBAGE)))
                {
                    ((Collector)colCell.Agent).MoveLeft = false;
                    MoveToDown(matrixVM, colCell);                    
                }
            }
        }

        public static void MoveToRight(MatrixViewModel matrixVM, Cell colCell)
        {
            Agent tmp = (Agent)colCell.Agent;
            ((Collector)tmp).MoveLeft = false;
            matrixVM.Ambient[colCell.X, colCell.Y].Agent = null;
            colCell.X = colCell.X + 1;
            matrixVM.Ambient[colCell.X, colCell.Y].Agent = tmp;            
        }

        public static void MoveToLeft(MatrixViewModel matrixVM, Cell colCell)
        {
            Agent tmp = (Agent)colCell.Agent;
            ((Collector)tmp).MoveLeft = true;
            matrixVM.Ambient[colCell.X, colCell.Y].Agent = null;
            colCell.X = colCell.X - 1;
            matrixVM.Ambient[colCell.X, colCell.Y].Agent = tmp;            
        }

        public static void MoveToDown(MatrixViewModel matrixVM, Cell colCell)
        {
            Agent tmp = (Agent)colCell.Agent;
            ((Collector)tmp).MoveUp = false;
            matrixVM.Ambient[colCell.X, colCell.Y].Agent = null;
            colCell.Y = colCell.Y + 1;
            matrixVM.Ambient[colCell.X, colCell.Y].Agent = tmp;            
        }

        public static void MoveToUp(MatrixViewModel matrixVM, Cell colCell)
        {
            Agent tmp = (Agent)colCell.Agent;
            ((Collector)tmp).MoveUp = true;
            matrixVM.Ambient[colCell.X, colCell.Y].Agent = null;
            colCell.Y = colCell.Y + 1;
            matrixVM.Ambient[colCell.X, colCell.Y].Agent = tmp;            
        }

        #endregion
    }
}