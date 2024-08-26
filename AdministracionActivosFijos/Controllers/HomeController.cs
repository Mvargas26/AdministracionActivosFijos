using AdministracionActivosFijos.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using Models;
using Microsoft.AspNetCore.Authorization;
using BLL;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting.Internal;


namespace AdministracionActivosFijos.Controllers
{
    public class HomeController : Controller
    {
        acercaDeNegocios objAcercaDe = new acercaDeNegocios();

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [Authorize]
        public IActionResult Index()
        {
            try
            {
                FuncionarioModel funCapturado = new FuncionarioModel();
                funCapturado = FuncionarioLogueado.retornarDatosFunc();

                if (funCapturado==null)
                {
                    return RedirectToAction("validarUser", "Acceso");
                }

                TempData["mensaje"] = "Bienvenido: "+funCapturado.nombre +" "+funCapturado.apellido1;
                return View();

            }
            catch (Exception)
            {

                return View();
            }
        }

        public async Task<IActionResult> salir()
        {
           await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            //destruimos tambien el objeto capturado en el login
            FuncionarioLogueado.CerrarSesion();

            return RedirectToAction("validarUser", "Acceso");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult acercaDe()
        {
            try
            {
                //ruta del Archivo
                string rutaArchivo = "wwwroot/Archivos/acercaDe.txt"; 

                List<String> listaAcercaDe = new List<String>();

                listaAcercaDe = objAcercaDe.CargarAcercade(rutaArchivo);

          
                return View(listaAcercaDe);
            }
            catch (Exception)
            {
                return View();

            }
        }


    }//fin class
}//fin space
