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

        public Cell[,] Matrix;
        public StringBuilder Html { get; set; }

        #endregion

        #region Constructor

        public MatrixViewModel(int matrixSizeWidth, int matrixSizeHeight)
        {
            Matrix = new Cell[matrixSizeWidth, matrixSizeHeight];

            for (int i = 0; i < Matrix.GetLength(0); i++)
                for (int j = 0; j < Matrix.GetLength(1); j++)
                {
                    Matrix[i, j] = new Cell(i, j);
                }
        }

        #endregion        
    }
}
