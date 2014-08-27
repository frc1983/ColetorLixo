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
            matrixVM = new MatrixViewModel(15, 15);//TODO: Parametrizar tamanhos
            matrixVM.Html = new StringBuilder();

            TempData["matrixVM"] = matrixVM;
            TempData.Keep("matrixVM");

            matrixVM.AddTrash(0, 0, EnumGarbageType.Metal);
            matrixVM.AddTrash(1, 0, EnumGarbageType.Papel);
            matrixVM.AddTrash(2, 0, EnumGarbageType.Plastico);
            matrixVM.AddTrash(3, 0, EnumGarbageType.Vidro);
            matrixVM.AddCharger(1, 1);
            matrixVM.AddCollector(2, 2);
            matrixVM.AddGarbage(3, 3, EnumGarbageType.Vidro);

            return View();
        }

        //Altera a posição do coletor e manda para desenhar na tela
        public JsonResult PopulateMatrix()
        {
            GetMatrix();
            //TODO: Criar alteracoes do ColetorLixo ambiente
            return DrawTable(true);
        }

        //Desenha a tablea com a matriz e envia para a view
        public JsonResult DrawTable(bool? update)
        {
            if (!update.HasValue)
                GetMatrix();

            ConstructTable.BuildTable(matrixVM);

            var result = new { Html = matrixVM.Html.ToString() };
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
