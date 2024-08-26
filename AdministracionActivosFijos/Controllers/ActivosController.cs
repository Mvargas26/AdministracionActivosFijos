using BLL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Models;
using System;

namespace AdministracionActivosFijos.Controllers
{
    
    public class ActivosController : Controller
    {
        //objeto de la cap Negocios
        ActivosNegocios obj_Activos_Negocios = new ActivosNegocios();
        FuncionariosNegocios obj_FuncionariosNego = new FuncionariosNegocios();

        [Authorize(Roles = "conta")]
        public ActionResult Index()
        {
            return View(obj_Activos_Negocios.consultarTodosActivos());
        }


        [Authorize(Roles = "conta")]
        public ActionResult CrearActivo()
        {
            ViewData["lsResponsables"] = obj_FuncionariosNego.consultarTodosFuncionarios();
            ViewData["lsTiposAct"] = obj_Activos_Negocios.consultarTiposActivos();
            return View();
        }

        [Authorize(Roles = "conta")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearActivo(IFormCollection collection)
        {
            try
            {
                // Obtenemos los valores del formulario en la vista CrearActivo
                // y lo asignamos a un nuevo objeto de tipo Activo
                ActivoModel newActivo = new ActivoModel
                {
                    cod = Convert.ToInt32(collection["cod"]),
                    descrip = collection["descrip"],
                    fechaCompra = DateTime.ParseExact(collection["fechaCompra"],"yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture),
                    caracteris = collection["caracteris"],
                    costo = double.Parse(collection["costo"]),
                    idMetDepre = 1,//enviamos 1 ya que se asigna en calculo
                    valorLibros = double.Parse(collection["costo"]), //asignamos lo mismo q costo xq aun no se deprecia
                    depreAcumulada= 0,//asignamos 0 xq aun no deprecia
                    codFuncionario = Convert.ToInt32(collection["codFuncionario"]),
                    idTipoActivo = Convert.ToInt32(collection["idTipoActivo"])
                };
                
                // enviamos el objeto creado a negocios
                obj_Activos_Negocios.insertarNuevoActivo(newActivo);

                //regresamos a la vista de la lista
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        [Authorize(Roles = "conta")]
        public ActionResult EditarActivo(int id)
        {
            return View(obj_Activos_Negocios.consultarActivoPorCod(id));
        }

        [Authorize(Roles = "conta")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarActivo(int id, IFormCollection collection)
        {
            try
            {
                ActivoModel act = new ActivoModel
                {
                    cod = Convert.ToInt32(collection["cod"]),
                    descrip = collection["descrip"],
                    fechaCompra = DateTime.ParseExact(collection["fechaCompra"], "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture),
                    caracteris = collection["caracteris"],
                    costo = double.Parse(collection["costo"]),
                    idMetDepre = 1,//enviamos 1 ya que se asigna en calculo
                    valorLibros = double.Parse(collection["costo"]), //asignamos lo mismo q costo xq aun no se deprecia
                    depreAcumulada = 0,//asignamos 0 xq aun no deprecia
                    codFuncionario = Convert.ToInt32(collection["codFuncionario"]),
                    idTipoActivo = Convert.ToInt32(collection["idTipoActivo"])
                };


                obj_Activos_Negocios.updateActivo(act);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        [Authorize(Roles = "conta")]
        public ActionResult BorrarActivo(int id)
        {
            return View(obj_Activos_Negocios.consultarActivoPorCod(id));
        }

        [Authorize(Roles = "conta")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BorrarActivo(int id, IFormCollection collection)
        {
            try
            {
                obj_Activos_Negocios.eliminarActivo(id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["mensajeError"] = "No puede borrar este Activo, verifique las relaciones.";
                return View();
            }
        }

        [Authorize]
        public ActionResult consultarActivosXFuncionario()
        {
            try
            {
                FuncionarioModel funCapturado = new FuncionarioModel();
                funCapturado = FuncionarioLogueado.retornarDatosFunc();

                if (funCapturado == null)
                {
                    return RedirectToAction("validarUser", "Acceso");
                }

                return View(obj_Activos_Negocios.verActivosxFuncionario(funCapturado.idFuncionario));


            }
            catch (Exception)
            {

                return RedirectToAction("validarUser", "Acceso");
            }
        }
    }//fn class
}//fn spcae
