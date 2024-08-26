using Models;

namespace AdministracionActivosFijos
{
    public static class FuncionarioLogueado
    {
        private static FuncionarioModel _funcionarioActual;

        public static void capturarDatosFunc(FuncionarioModel funcionario)
        {
            _funcionarioActual = funcionario;
        }

        public static FuncionarioModel retornarDatosFunc()
        {
            return _funcionarioActual;
        }

        public static void CerrarSesion()
        {
            _funcionarioActual = null;
        }

    }//fn class
}//fn space
