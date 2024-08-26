using BLL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace AdministracionActivosFijos.Controllers
{
    [Authorize]
    public class FuncionariosController : Controller
    {
        //objeto de la cap Negocios
        FuncionariosNegocios obj_FuncionarioNegocios = new FuncionariosNegocios();
        DepartamentoNegocios obj_DepartamentoNegocios = new DepartamentoNegocios();

        [Authorize(Roles = "conta")]
        public ActionResult Index()
        {
            //retornamos a la vista lo que nos trae consultarTodosDepartamentos
            ViewData["Departamentos"] = obj_DepartamentoNegocios.consultarTodosDepartamentos();

            //retornamos a la vista lo que nos trae consultarTodosFuncionarios
            return View(obj_FuncionarioNegocios.consultarTodosFuncionarios());
        }

        [Authorize(Roles = "conta")]
        public ActionResult Details(int id)
        {
            return View();
        }

        [Authorize(Roles = "conta")]
        public ActionResult CrearFuncionario()
        {
            try
            {
                ViewData["Departamentos"] = obj_DepartamentoNegocios.consultarTodosDepartamentos();
                return View();
            }
            catch (Exception)
            {

                return View();
            }
        }

        [Authorize(Roles = "conta")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearFuncionario(IFormCollection collection)
        {
            try
            {
                // Obtenemos los valores del formulario en la vista CrearFuncionario
                // y lo asignamos a un nuevo objeto de tipo funcionario
                FuncionarioModel funcionario = new FuncionarioModel();

                funcionario.idFuncionario = Convert.ToInt32(collection["idFuncionario"]);
                funcionario.correoFuncionario = collection["correoFuncionario"];
                funcionario.password = collection["password"];
                funcionario.nombre = collection["nombre"];
                funcionario.apellido1 = collection["apellido1"];
                funcionario.apellido2 = collection["apellido2"];
                funcionario.idDepartamento = Convert.ToInt32(collection["idDepartamento"]);
                if (collection["estado"].Equals("false"))
                {
                    funcionario.estado = false;
                }
                else
                {
                    funcionario.estado = true;
                }

                //Validamos que el correo cumpla con la expresion regular
                if (!obj_FuncionarioNegocios.CorreoCumple(funcionario.correoFuncionario))
                {
                    TempData["mensajeError"] = "La contraseña no cumple con el estandar solicitado.";
                    //volvemos a cargar los departamentos
                    ViewData["Departamentos"] = obj_DepartamentoNegocios.consultarTodosDepartamentos();
                    return View();
                }

                //Validamos que la contraseña cumpla con la expresion regular
                if (!obj_FuncionarioNegocios.PasswordCumple(funcionario.password))
                {
                    TempData["mensajeError"] = "La contraseña no cumple con el estandar solicitado.";
                    //volvemos a cargar los departamentos
                    ViewData["Departamentos"] = obj_DepartamentoNegocios.consultarTodosDepartamentos();
                    return View();
                }

                // enviamos el onjeto creado a negocios
                obj_FuncionarioNegocios.insertarNuevoFuncionario(funcionario);
                //regresamos a la vista de la lista
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        [Authorize(Roles = "conta")]
        public ActionResult EditarFuncionario(int id)
        {

            return View(obj_FuncionarioNegocios.funcionarioXid(id));
        }

        [Authorize(Roles = "conta")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarFuncionario(int id, IFormCollection collection)
        {
            try
            {
                // Obtenemos los valores del formulario en la vista EditarFuncionario
                // y lo asignamos a un nuevo objeto de tipo funcionario
                FuncionarioModel funcionario = new FuncionarioModel();

                funcionario.idFuncionario = Convert.ToInt32(collection["idFuncionario"]);
                funcionario.correoFuncionario = collection["correoFuncionario"];
                funcionario.password = collection["password"];
                funcionario.nombre = collection["nombre"];
                funcionario.apellido1 = collection["apellido1"];
                funcionario.apellido2 = collection["apellido2"];
                funcionario.idDepartamento = Convert.ToInt32(collection["idDepartamento"]);
                if (collection["estado"].Equals("false"))
                {
                    funcionario.estado = false;
                }
                else
                {
                    funcionario.estado = true;
                }

                // enviamos el onjeto creado a negocios
                obj_FuncionarioNegocios.editarFuncionario(funcionario);
                //regresamos a la vista de la lista
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        [Authorize(Roles = "conta")]
        public ActionResult borrarFuncionario(int id)
        {
            //con el id capturado, traemos la info de ese usuario para mostrarla
            return View(obj_FuncionarioNegocios.funcionarioXid(id));
        }

        [Authorize(Roles = "conta")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult borrarFuncionario(int id, IFormCollection collection)
        {
            try
            {
                //llamamos metodo de eliminar en negocios pasandole la cedula del func a eliminar
                obj_FuncionarioNegocios.eliminarFuncionario(id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        [Authorize]
        public ActionResult cambiarPassword()
        {
            FuncionarioModel func = FuncionarioLogueado.retornarDatosFunc();

            if (func == null)
            {
                return RedirectToAction("validarUser", "Acceso");
            }

            return View(obj_FuncionarioNegocios.funcionarioXid(func.idFuncionario));
        }

        [Authorize]
        [HttpPost]
        public ActionResult cambiarPassword(int id, IFormCollection collection)
        {
            try
            {

                FuncionarioModel funcionario = new FuncionarioModel();

                funcionario.idFuncionario = Convert.ToInt32(collection["idFuncionario"]);
                funcionario.correoFuncionario = collection["correoFuncionario"];
                funcionario.password = collection["password"];

                //Validamos que la contraseña cumpla con la expresion regular
                if (!obj_FuncionarioNegocios.PasswordCumple(funcionario.password))
                {
                    TempData["mensajeError"] = "La contraseña no cumple con el estandar solicitado.";
                    return View(funcionario);
                }

                // enviamos el onjeto creado a negocios
                obj_FuncionarioNegocios.cambiarPassword(funcionario);
                //regresamos a la vista de la lista
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

    }//fn class
}//fn space
