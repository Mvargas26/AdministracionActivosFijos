using BLL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace AdministracionActivosFijos.Controllers
{
    public class DepartamentosController : Controller
    {
        DepartamentoNegocios objDepartamentos = new DepartamentoNegocios();

        [Authorize(Roles = "conta")]
        public ActionResult Index()
        {
            try
            {

                return View(objDepartamentos.consultarTodosDepartamentos());
            }
            catch (Exception)
            {

                return View();
            }
        }



        public ActionResult crearDepartamento()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult crearDepartamento(IFormCollection collection)
        {
            try
            {
                DepartamentoModel newDeparta = new DepartamentoModel();

                newDeparta.idDepartamento= Convert.ToInt32(collection["idDepartamento"]);
                newDeparta.departamento = collection["departamento"];

                objDepartamentos.crearNuevoDepartamento(newDeparta);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public ActionResult EditarDepartamento(int id)
        {

            return View(objDepartamentos.consultarDepartamentoID(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarDepartamento(int id, IFormCollection collection)
        {
            try
            {
                DepartamentoModel modeloEditar = new DepartamentoModel();

                modeloEditar.idDepartamento = Convert.ToInt32(collection["idDepartamento"]);
                modeloEditar.departamento = collection["departamento"];

                objDepartamentos.editarDepartamento(modeloEditar);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public ActionResult borrarDepartamento(int id)
        {
           
            return View(objDepartamentos.consultarDepartamentoID(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult borrarDepartamento(int id, IFormCollection collection)
        {
            try
            {
                objDepartamentos.borraroDepartamento(id);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
