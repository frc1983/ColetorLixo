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
        public static void Move(MatrixViewModel matrixVM, Cell colCell, Cell newCell)
        {
            Agent tmp = (Agent)colCell.Agent;
            matrixVM.Ambient[colCell.X, colCell.Y].Agent = null;
            matrixVM.Ambient[newCell.X, newCell.Y].Agent = tmp;  
        }

        internal static Cell GetNextDefaultMovement(Cell actual, List<Cell> possibleCell)
        {
            Cell objective = null;
            Collector actualColector = ((Collector)actual.Agent);
            
            if (!actualColector.MoveLeft)
            {
                objective = possibleCell.Where(y => y.Y == actual.Y && y.X > actual.X).FirstOrDefault();
                if (objective == null)
                {
                    if (!actualColector.MoveUp)
                    {
                        objective = possibleCell.Where(y => y.Y > actual.Y).OrderByDescending(x => x.X).FirstOrDefault();
                        if (objective == null)
                        {
                            objective = possibleCell.Where(y => y.Y < actual.Y).OrderByDescending(x => x.X).First();
                            actualColector.MoveUp = true;
                        }
                    }
                    else
                    {
                        objective = possibleCell.Where(y => y.Y < actual.Y).OrderByDescending(x => x.X).FirstOrDefault();
                        if (objective == null)
                        {
                            objective = possibleCell.Where(y => y.Y > actual.Y).OrderByDescending(x => x.X).First();
                            actualColector.MoveUp = false;
                        }
                    }
                    actualColector.MoveLeft = true;
                }
            }
            else
            {
                objective = possibleCell.Where(y => y.Y == actual.Y && y.X < actual.X).FirstOrDefault();
                if (objective == null)
                {
                    if (!actualColector.MoveUp)
                    {
                        objective = possibleCell.Where(y => y.Y > actual.Y).OrderByDescending(x => x.X).FirstOrDefault();
                        if (objective == null)
                        {
                            objective = possibleCell.Where(y => y.Y < actual.Y).OrderByDescending(x => x.X).First();
                            actualColector.MoveUp = true;
                        }
                    }
                    else
                    {
                        objective = possibleCell.Where(y => y.Y < actual.Y).OrderByDescending(x => x.X).FirstOrDefault();
                        if (objective == null)
                        {
                            objective = possibleCell.Where(y => y.Y > actual.Y).OrderByDescending(x => x.X).First();
                            actualColector.MoveUp = false;
                        }
                    }
                    actualColector.MoveLeft = false;
                }
            }
            return objective;
        }
    }
}