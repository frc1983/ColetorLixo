using ColetorLixo.Models;
using ColetorLixo.Utils;
using ColetorLixo.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ColetorLixo.Controllers
{
    public class HomeController : Controller
    {
        private MatrixViewModel matrixVM;  

        public ActionResult Index()
        {
            matrixVM = new MatrixViewModel(15, 15);//TODO: Parametrizar tamanhos ( X, Y )
            matrixVM.Html = new StringBuilder();

            TempData["matrixVM"] = matrixVM;
            TempData.Keep("matrixVM");

            matrixVM.AddTrash(RandomPositions.GetNextX(matrixVM.Ambient), RandomPositions.GetNextY(matrixVM.Ambient), EnumGarbageType.Metal);
            matrixVM.AddTrash(RandomPositions.GetNextX(matrixVM.Ambient), RandomPositions.GetNextY(matrixVM.Ambient), EnumGarbageType.Paper);
            matrixVM.AddTrash(RandomPositions.GetNextX(matrixVM.Ambient), RandomPositions.GetNextY(matrixVM.Ambient), EnumGarbageType.Plastic);
            matrixVM.AddTrash(RandomPositions.GetNextX(matrixVM.Ambient), RandomPositions.GetNextY(matrixVM.Ambient), EnumGarbageType.Glass);
            matrixVM.AddCharger(RandomPositions.GetNextX(matrixVM.Ambient), RandomPositions.GetNextY(matrixVM.Ambient));
            matrixVM.AddGarbage(RandomPositions.GetNextX(matrixVM.Ambient), RandomPositions.GetNextY(matrixVM.Ambient), EnumGarbageType.Glass);
            matrixVM.AddGarbage(RandomPositions.GetNextX(matrixVM.Ambient), RandomPositions.GetNextY(matrixVM.Ambient), EnumGarbageType.Plastic);
            matrixVM.AddGarbage(RandomPositions.GetNextX(matrixVM.Ambient), RandomPositions.GetNextY(matrixVM.Ambient), EnumGarbageType.Paper);
            matrixVM.AddGarbage(RandomPositions.GetNextX(matrixVM.Ambient), RandomPositions.GetNextY(matrixVM.Ambient), EnumGarbageType.Metal);

            //Importante deixar por ultimo para existirem todas lixeiras e carregadores
            matrixVM.AddCollector(0, 0);

            return View();
        }

        public JsonResult PopulateMatrix()
        {
            GetMatrix();

            //Pega todos coletores e move eles
            List<Cell> collectors = matrixVM.GetCollectors();
            foreach (Cell colCell in collectors)
                ((Collector)colCell.Agent).MoveCollector(matrixVM, colCell);
            
            return DrawTable(true);
        }

        public JsonResult DrawTable(bool? update)
        {
            if (!update.HasValue)
                GetMatrix();

            ConstructTable.BuildTable(matrixVM);
            ConstructStatus.BuildStatus(matrixVM);

            var result = new { Html = matrixVM.Html.ToString(), Status = matrixVM.Status.ToString() };
            return new JsonResult() { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        private void GetMatrix()
        {
            matrixVM = TempData["matrixVM"] as MatrixViewModel;
            TempData["matrixVM"] = matrixVM;
            TempData.Keep("matrixVM");
        }
    }
}
