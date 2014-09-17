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
            matrixVM = new MatrixViewModel(5, 5);

            if (!string.IsNullOrEmpty(Request["X"]) && !string.IsNullOrEmpty(Request["Y"]) &&
                Convert.ToInt32(Request["X"].ToString()) > 0 && Convert.ToInt32(Request["Y"].ToString()) > 0)
                matrixVM = new MatrixViewModel(Convert.ToInt32(Request["X"].ToString()), Convert.ToInt32(Request["Y"].ToString()));

            matrixVM.Html = new StringBuilder();

            TempData["matrixVM"] = matrixVM;
            TempData.Keep("matrixVM");

            //matrixVM.AddTrash(RandomPositions.GetNextX(matrixVM.Ambient), RandomPositions.GetNextY(matrixVM.Ambient), EnumGarbageType.Metal);
            //matrixVM.AddTrash(RandomPositions.GetNextX(matrixVM.Ambient), RandomPositions.GetNextY(matrixVM.Ambient), EnumGarbageType.Paper);
            //matrixVM.AddTrash(RandomPositions.GetNextX(matrixVM.Ambient), RandomPositions.GetNextY(matrixVM.Ambient), EnumGarbageType.Plastic);
            //matrixVM.AddTrash(RandomPositions.GetNextX(matrixVM.Ambient), RandomPositions.GetNextY(matrixVM.Ambient), EnumGarbageType.Glass);

            matrixVM.AddCharger(RandomPositions.GetNextX(matrixVM.Ambient), RandomPositions.GetNextY(matrixVM.Ambient), 10);

            //matrixVM.AddGarbage(RandomPositions.GetNextX(matrixVM.Ambient), RandomPositions.GetNextY(matrixVM.Ambient), EnumGarbageType.Glass);
            //matrixVM.AddGarbage(RandomPositions.GetNextX(matrixVM.Ambient), RandomPositions.GetNextY(matrixVM.Ambient), EnumGarbageType.Plastic);
            //matrixVM.AddGarbage(RandomPositions.GetNextX(matrixVM.Ambient), RandomPositions.GetNextY(matrixVM.Ambient), EnumGarbageType.Paper);
            //matrixVM.AddGarbage(RandomPositions.GetNextX(matrixVM.Ambient), RandomPositions.GetNextY(matrixVM.Ambient), EnumGarbageType.Metal);

            ////Importante deixar por ultimo para existirem todas lixeiras e carregadores
            matrixVM.AddCollector(RandomPositions.GetNextX(matrixVM.Ambient), RandomPositions.GetNextY(matrixVM.Ambient), 10, 10);
            matrixVM.AddCollector(RandomPositions.GetNextX(matrixVM.Ambient), RandomPositions.GetNextY(matrixVM.Ambient), 10, 5);

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

        public JsonResult AddAgent(String type, String garbage)
        {
            EnumGarbageType garbageType;
            var result = new { Message = "ERROR: On add new agent" };

            GetMatrix();

            if (EnumGarbageType.TryParse(garbage, out garbageType))
            {
                result = new { Message = "OK" };
                switch (type)
                {
                    case "COLLECTOR":
                        matrixVM.AddCollector(RandomPositions.GetNextX(matrixVM.Ambient), RandomPositions.GetNextY(matrixVM.Ambient), 10, 10);
                        return new JsonResult() { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                    case "TRASH":
                        matrixVM.AddTrash(RandomPositions.GetNextX(matrixVM.Ambient), RandomPositions.GetNextY(matrixVM.Ambient), 10, garbageType);
                        return new JsonResult() { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                    case "CHARGER":
                        matrixVM.AddCharger(RandomPositions.GetNextX(matrixVM.Ambient), RandomPositions.GetNextY(matrixVM.Ambient), 6);
                        return new JsonResult() { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                    case "GARBAGE":
                        matrixVM.AddGarbage(RandomPositions.GetNextX(matrixVM.Ambient), RandomPositions.GetNextY(matrixVM.Ambient), garbageType);
                        return new JsonResult() { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
            }

            return new JsonResult() { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
    }
}
