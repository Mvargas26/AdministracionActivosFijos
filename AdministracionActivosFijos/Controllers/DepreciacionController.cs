using BLL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AdministracionActivosFijos.Controllers
{
    [Authorize(Roles ="conta")]
    public class DepreciacionController : Controller
    {
        //objeto de la cap Negocios
        ActivosNegocios obj_Activos_Negocios = new ActivosNegocios();
        CalculoDepreciacionNegocios obj_CalculoDepreciacionNegocios = new CalculoDepreciacionNegocios();

        public ActionResult selec_ActivoCalcular()
        {
            return View(obj_Activos_Negocios.consultarTodosActivos());
        }

        public ActionResult calculoActivoSeleccionado(int id)
        {
            //retornamos a la vista lo que nos trae consultarTodosDepartamentos
          ViewData["periodoSinCalc"] = obj_CalculoDepreciacionNegocios.primerPeriodoSinCalcular(id);
            ViewData["listaTipoDepre"] = obj_CalculoDepreciacionNegocios.listarTiposDepre();
            return View(obj_Activos_Negocios.consultarActivoPorCod(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult calculoActivoSeleccionado(int id,string periodoSeleccionado,int tipoDepre, IFormCollection collection)
        {
            try
            {
                obj_CalculoDepreciacionNegocios.Calcular(id,tipoDepre);

                TempData["alertaMensaje"] = "Activo calculado con éxito.";

                return RedirectToAction(nameof(calculoActivoSeleccionado));
            }
            catch
            {
                return View();
            }
        }

        // GET: DepreciacionController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: DepreciacionController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }


        public ActionResult verPeriodosCalculados(int id)
        {

            return View(obj_CalculoDepreciacionNegocios.listarPeriodosCalculadosPorActivo(id));
        }

      
        public ActionResult aprobarPrimerPeriodoCalculado(int id)
        {
            try
            {
                return View(obj_CalculoDepreciacionNegocios.primerPeriodoSinAprobar(id));

            }
            catch (Exception)
            {

                return View();
            }
        }

        [HttpPost]
        public ActionResult aprobarPrimerPeriodoCalculado(int id, IFormCollection collection)
        {
            try
            {
                //capturamos el id del periodo para cambairle el estado
                int idPeriodo = Convert.ToInt32(collection["idPeriodo"]);

                obj_CalculoDepreciacionNegocios.cambiarPeriodoAEstadoAprobado(idPeriodo);

                return RedirectToAction(nameof(selec_ActivoCalcular));
            }
            catch (Exception)
            {
                return View();
            }
        }

        public ActionResult verPeriodosAProbados(int id)
        {

            return View(obj_CalculoDepreciacionNegocios.listarPeriodosAprobadosPorActivo(id));
        }
    }//fn class
}//fn space
