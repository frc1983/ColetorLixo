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
        private const int INITIAL_COLLECTOR_CHARGE = 5;
        private const int CHARGER_RECHARGE_SIZE = 20;
        private const int COLLECTOR_GARBAGE_CAPACITY = 100;
        private const int TRASH_CAPACITY = 10;

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

            //matrixVM.AddCharger(RandomPositions.GetNextX(matrixVM.Ambient), RandomPositions.GetNextY(matrixVM.Ambient), 10);

            //matrixVM.AddGarbage(RandomPositions.GetNextX(matrixVM.Ambient), RandomPositions.GetNextY(matrixVM.Ambient), EnumGarbageType.Glass);
            //matrixVM.AddGarbage(RandomPositions.GetNextX(matrixVM.Ambient), RandomPositions.GetNextY(matrixVM.Ambient), EnumGarbageType.Plastic);
            //matrixVM.AddGarbage(RandomPositions.GetNextX(matrixVM.Ambient), RandomPositions.GetNextY(matrixVM.Ambient), EnumGarbageType.Paper);
            //matrixVM.AddGarbage(RandomPositions.GetNextX(matrixVM.Ambient), RandomPositions.GetNextY(matrixVM.Ambient), EnumGarbageType.Metal);

            ////Importante deixar por ultimo para existirem todas lixeiras e carregadores
            //matrixVM.AddCollector(RandomPositions.GetNextX(matrixVM.Ambient), RandomPositions.GetNextY(matrixVM.Ambient), 10, 10);
            //matrixVM.AddCollector(RandomPositions.GetNextX(matrixVM.Ambient), RandomPositions.GetNextY(matrixVM.Ambient), 10, 5);

            return View();
        }

        public JsonResult PopulateMatrix()
        {
            GetMatrix();

            //Pega cada coletor e move ele
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
                int randomX = RandomPositions.GetNextX(matrixVM.Ambient),
                    randomY = RandomPositions.GetNextY(matrixVM.Ambient);
                result = new { Message = "OK" };
                switch (type)
                {
                    case "COLLECTOR":
                        matrixVM.AddCollector(randomX, randomY, INITIAL_COLLECTOR_CHARGE, COLLECTOR_GARBAGE_CAPACITY);
                        return new JsonResult() { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                    case "TRASH":
                        matrixVM.AddTrash(randomX, randomY, TRASH_CAPACITY, garbageType);
                        return new JsonResult() { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                    case "CHARGER":
                        matrixVM.AddCharger(randomX, randomY, CHARGER_RECHARGE_SIZE);
                        return new JsonResult() { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                    case "GARBAGE":
                        matrixVM.AddGarbage(randomX, randomY, garbageType);
                        return new JsonResult() { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
            }

            return new JsonResult() { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
    }
}
