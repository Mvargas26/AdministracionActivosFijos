using BLL;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using System.Security.Claims;

namespace AdministracionActivosFijos.Controllers
{
    public class AccesoController : Controller
    {
        //variables
        FuncionariosNegocios objFuncionario = new FuncionariosNegocios();
        public ActionResult validarUser()
        {
            //// Si ya esta logueado, redirige al index de home
            //ClaimsPrincipal claimsUser = HttpContext.User;

            //if (claimsUser.Identity.IsAuthenticated)
            //{
            //    return RedirectToAction("validarUser", "Acceso");
            //}

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> validarUser(FuncionarioModel funcionarioSInValidar)
        {
            try
            {
                FuncionarioModel funciValidado = new FuncionarioModel();

                //valida que el usuario este activo
                if (!objFuncionario.funcionarioEstaActivo(funcionarioSInValidar))
                {
                    TempData["mensajeError"] = "Este usuario no existe o esta inactivo.";
                    return View();
                }

                //valida que el usuario tenga activos asignados
                if (!objFuncionario.funcionarioTieneActivos(funcionarioSInValidar))
                {
                    TempData["mensajeError"] = "Este usuario no tiene activos asignados.";
                    return View();
                }

                //Traemos todos los datos desde la base de datos
                funciValidado = objFuncionario.obtenerFuncdelLogin(funcionarioSInValidar);

                // Guardar datos de quien se logueo en la clase estatica
                FuncionarioLogueado.capturarDatosFunc(funciValidado);

                //verificamos a que departamento pertenece el usuario logueado
                // si es 1 es de conta 
                string rolFuncionario = "otro";
                if (funciValidado.idDepartamento == 1)
                {
                    rolFuncionario = "conta";
                }

                List<Claim> claims = new List<Claim>()
                     {
                         new Claim(ClaimTypes.Email,funcionarioSInValidar.correoFuncionario),
                         new Claim (ClaimTypes.Role,rolFuncionario)
                        
                     };
                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    AuthenticationProperties authenticationProperties = new AuthenticationProperties()
                    {
                        AllowRefresh = true
                    };

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity), authenticationProperties);

                    return RedirectToAction("Index", "Home");

            }
            catch
            {
                return View();
            }
        }//fin validarUser

        public IActionResult AccessDenied()
        {
            return View();
        }
    }//fin class
}//fin space
